using Comun.DTO.BuscarUsuario;
using Comun.DTO.Generales;
using Comun.DTO.Prestamo;
using Comun.DTO.Solicitud;
using Comun.Enumeracion;
using Comun.Generales;
using Datos.Contratos.Prestamo;
using Datos.Contratos.Solicitud;
using Datos.Contratos.Usuario;
using Datos.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Utilities;
using WepPrestamos.Areas.Solicitud.Controllers;
using WepPrestamos.Helpers;

namespace WepPrestamos.Areas.Prestamo.Controllers
{
    [Area("Prestamo")]
    //[Authorize]
    public class GestionController : Controller
    {
        private readonly ILogger<GestionController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IBLConsultar_Detalle_Master _dominio;
        private readonly IGestionPrestamo _prestamo;

        public GestionController
        (
            IConfiguration configuration,
            ILogger<GestionController> logger,
            IBLConsultar_Detalle_Master dominio,
            IGestionPrestamo prestamo
        )
        {
            _configuration = configuration;
            _logger = logger;
            _dominio = dominio;
            _prestamo = prestamo;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Pago()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> BuscarPorIdentificacion(string nroIdentificacion)
        {

            if (string.IsNullOrWhiteSpace(nroIdentificacion))
            {
                ViewBag.SweetAlert = "warning";
                ViewBag.Titulo = "Campo vacío";
                ViewBag.Mensaje = "Por favor ingresa un número de identificación.";
                return View("Pago");
            }

            var request = new BuscarUsuarioNurIdentificacionPet
            {
                NRO_IDENTIFICACION = Convert.ToInt64(nroIdentificacion)
            };

            var resultado = await _prestamo.Obtener_X_Identificacion_Async<Int64, List<Prestamo_Dto>>(request.NRO_IDENTIFICACION);

            if (resultado.Codigo == EstadoOperacion.Bueno)
            {
                var prestamos = resultado.Respuesta;
                ViewBag.Prestamos = prestamos;

                if (prestamos == null || prestamos.Count() == 0)
                {
                    ViewBag.SweetAlert = "warning";
                    ViewBag.Titulo = "Cliente NO encontrado";
                    ViewBag.Mensaje = $"El numero de identificación ingresado no esta asociado a un prestamo.";
                }
                else
                {
                    ViewBag.SweetAlert = "success";
                    ViewBag.Titulo = "Cliente Encontrado";
                    ViewBag.Mensaje = $"El numero de identificación fue encontrado exitosamente, y esta asociado al usuario {prestamos[0].SOLICITANTE}.";
                }
            }
            else
            {
                ViewBag.SweetAlert = "error";
                ViewBag.Titulo = "No encontrado";
                ViewBag.Mensaje = resultado.Mensaje;
            }

            return View("Pago");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrarPago(InsertPagoPrestamoDto modelo)
        {
            try
            {                
                if (!ModelState.IsValid)
                {
                    return Json(new RespuestaDto<bool>
                    {
                        Codigo = EstadoOperacion.Malo,
                        Mensaje = "Datos incompletos, valide e intente nuevamente."
                    });
                }
                
                if (modelo.MONTO <= 0)
                {
                    return Json(new RespuestaDto<bool>
                    {
                        Codigo = EstadoOperacion.Malo,
                        Mensaje = "El monto debe ser mayor a cero."
                    });
                }

                if (modelo.FECHA_PAGO > DateTime.Now)
                {
                    return Json(new RespuestaDto<bool>
                    {
                        Codigo = EstadoOperacion.Malo,
                        Mensaje = "La fecha de pago no puede ser futura."
                    });
                }

                var resultado = await _prestamo.Obtener_X_ID_Async<Int64, Prestamo_Dto>(modelo.ID_PRESTAMO);

                if (resultado.Codigo == EstadoOperacion.Bueno)
                {
                    var prestamos = resultado.Respuesta;

                    long saldo = long.Parse(prestamos.SALDO_MONTO.ToString()); 
                    long intereses = long.Parse(prestamos.INTERES.ToString());
                    long interes = (long)(saldo * ((double)intereses / 100));

                    if (modelo.PAGO_INTERESES)
                    {
                        if(modelo.MONTO == interes)
                        {
                            //Guardamos el pago
                            RegistrarActualizarPagoDto p = new RegistrarActualizarPagoDto();
                            p.ID = 0;
                            p.ID_PRESTAMO = resultado.Respuesta.ID;
                            p.FECHA_PAGO = modelo.FECHA_PAGO;
                            p.MONTO = modelo.MONTO;
                            p.ID_TIPO_PAGO = 44;

                            var respPagar = await _prestamo.Insertar_Pago_Async<RegistrarActualizarPagoDto, string>(p);

                            return Json(new RespuestaDto<bool>
                            {
                                Codigo = EstadoOperacion.Bueno,
                                Mensaje = "El monto indicado fue registrado exitosamente."
                            });
                        }
                        else
                        {
                            return Json(new RespuestaDto<bool>
                            {
                                Codigo = EstadoOperacion.Malo,
                                Mensaje = "El monto ingresado no corresponde a los intereses calculados. El monto minimo esperado es de $" + interes
                            });
                        }
                    }
                    else
                    {
                        if (modelo.MONTO >= interes)
                        {
                            //=========================================================================
                            //-- Validamos si ya pago los intereses del mes si no se pagan intereses --
                            //=========================================================================

                            var listPagos = await _prestamo.Obtener_Pagos_Async<Int64, List<Pago_Dto>>(modelo.ID_PRESTAMO);

                            var maxCuota = listPagos.Respuesta?
                                                    .Where(p => p.NUMERO_CUOTA.HasValue)
                                                    .Select(p => p.NUMERO_CUOTA.Value)
                                                    .DefaultIfEmpty(0)
                                                    .Max() ?? 0;

                            var sumInter = listPagos.Respuesta ? 
                                                        .Where(p => p.ID_PRESTAMO == modelo.ID_PRESTAMO &&
                                                        p.NUMERO_CUOTA == maxCuota &&
                                                        p.ID_TIPO_PAGO == 44) 
                                                        .Sum(p => p.MONTO);

                            var sumCapital = listPagos.Respuesta?
                                                        .Where(p => p.ID_PRESTAMO == modelo.ID_PRESTAMO &&
                                                        p.NUMERO_CUOTA == maxCuota &&
                                                        p.ID_TIPO_PAGO == 45)
                                                        .Sum(p => p.MONTO);

                            //var CantidadMesesFechas = CantidadMesesEntreFechas(prestamos.FECHA_INICIO, DateTime.Now);


                            var saldoMonto = modelo.MONTO - interes;
                            bool sw_respPagoInte =false;
                            bool sw_respPagoCap = false;

                            if(sumInter < interes)
                            {
                                //-----------------------------------------------------------------
                                //------------------ Guardamos los intereses ----------------------
                                //-----------------------------------------------------------------
                                try
                                {
                                    RegistrarActualizarPagoDto p = new RegistrarActualizarPagoDto();
                                    p.ID = 0;
                                    p.ID_PRESTAMO = resultado.Respuesta.ID;
                                    p.FECHA_PAGO = modelo.FECHA_PAGO;
                                    p.MONTO = interes;
                                    p.ID_TIPO_PAGO = 44;

                                    var respPagoInte = await _prestamo.Insertar_Pago_Async<RegistrarActualizarPagoDto, string>(p);
                                    sw_respPagoInte = respPagoInte.Estado;
                                }
                                catch (Exception) { }
                            }

                           


                            //---------------------------------------------------------------------------
                            //--------Si saldoMonto es mayor a 0 entonces -------------------------------
                            //---------------------------------------------------------------------------

                            if (saldoMonto > 0)
                            {
                                try
                                {
                                    RegistrarActualizarPagoDto p = new RegistrarActualizarPagoDto();
                                    p.ID = 0;
                                    p.ID_PRESTAMO = resultado.Respuesta.ID;
                                    p.FECHA_PAGO = modelo.FECHA_PAGO;
                                    p.MONTO = saldoMonto;
                                    p.ID_TIPO_PAGO = 45;

                                    var respPagoCap = await _prestamo.Insertar_Pago_Async<RegistrarActualizarPagoDto, string>(p);
                                    sw_respPagoCap = respPagoCap.Estado;
                                }
                                catch (Exception) { }

                                try
                                { 
                                    var _saldo = prestamos.SALDO_MONTO - saldoMonto;

                                    ActualizarPrestamoDto a = new ActualizarPrestamoDto();
                                    a.ID = modelo.ID_PRESTAMO;
                                    a.SALDO = _saldo;

                                    if (_saldo <= 0)
                                    {
                                        a.ID_ESTADO = 52;
                                    }

                                    var actuPrestamo = await _prestamo.Actualizar_Prestamo_Async<ActualizarPrestamoDto,bool>(a);
                                }
                                catch (Exception) { }

                            }
                            
                            if (sw_respPagoInte && sw_respPagoCap)
                            {
                                return Json(new RespuestaDto<bool>
                                {
                                    Codigo = EstadoOperacion.Bueno,
                                    Mensaje = "Los montos a intereses y capital fueron registrados exitosamente."
                                });
                            }
                            else
                            {
                                string txt_mensaje = "Precaución: Se almacenó el pago de intereses: " +
                                                     (sw_respPagoInte ? "SI" : "NO") +
                                                     " y Se almacenó el pago a capital: " +
                                                     (sw_respPagoCap ? "SI" : "NO");

                                return Json(new RespuestaDto<bool>
                                {
                                    Codigo = EstadoOperacion.Malo,
                                    Mensaje = txt_mensaje
                                });

                            }

                        }
                        else
                        {
                            return Json(new RespuestaDto<bool>
                            {
                                Codigo = EstadoOperacion.Malo,
                                Mensaje = "El monto ingresado no corresponde a los intereses calculados. El monto mínimo esperado es de $" + interes
                            });
                        }
                    }

                }
                else
                {
                    return Json(new RespuestaDto<bool>
                    {
                        Codigo = EstadoOperacion.Malo,
                        Mensaje = "Se generó un error al insertar el pago"
                    });
                }
            }
            catch (Exception ex)
            {
                // Log del error
                // _logger.LogError(ex, "Error al registrar pago para préstamo {IdPrestamo}", modelo.ID_PRESTAMO);
                return Json(new RespuestaDto<bool>
                {
                    Codigo = EstadoOperacion.Excepcion,
                    Mensaje = "Ocurrió un error interno. Intente nuevamente."
                });                
            }

        }


        private int CalcularCuotasVencidas(DateTime fechaConsulta, DateTime FechaInicio, Int64 NumeroCuotas)
        {
            // Si la fecha de consulta es anterior al inicio, no hay cuotas vencidas
            if (fechaConsulta < FechaInicio)
                return 0;

            // Calcular la diferencia total en meses entre FechaInicio y fechaConsulta
            int mesesTranscurridos = (fechaConsulta.Year - FechaInicio.Year) * 12 + fechaConsulta.Month - FechaInicio.Month;

            // Si el día del mes de fechaConsulta es menor que el día de FechaInicio, restamos un mes
            if (fechaConsulta.Day < FechaInicio.Day)
            {
                mesesTranscurridos--;
            }

            // Las cuotas vencidas no pueden ser mayores a NumeroCuotas
            if (mesesTranscurridos < 0) mesesTranscurridos = 0;
            return (int)Math.Min(mesesTranscurridos, NumeroCuotas);
        }

        private int CantidadMesesEntreFechas(DateTime fechaInicio, DateTime fechaFin)
        {
            // Diferencia en años multiplicada por 12 más la diferencia en meses
            int diferenciaMeses = (fechaFin.Year - fechaInicio.Year) * 12 + fechaFin.Month - fechaInicio.Month;

            // Si quieres considerar solo meses completos, puedes ajustar según el día
            if (fechaFin.Day < fechaInicio.Day)
            {
                diferenciaMeses--;
            }

            return diferenciaMeses;
        }




    }
}
