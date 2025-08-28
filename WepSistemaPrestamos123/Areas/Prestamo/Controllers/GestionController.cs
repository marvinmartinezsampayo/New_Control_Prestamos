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
                    long Valorcuota = (long)(prestamos.MONTO / prestamos.NUMERO_CUOTAS);
                    

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
                                                    .DefaultIfEmpty(1)
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

                            var saldoMonto = 0;

                            var interesAnterior = ((double)(sumCapital + saldo)) * ((double)intereses / 100);


                            if (sumInter == (decimal)interesAnterior)// Validamos si ya pago la cuota de intereses del maxCuota (Ya pago)
                            {
                                //Validamos si ya pago el valor de la cuota de capital (Falta completar cuota)
                                if (sumCapital < Valorcuota)
                                {
                                    var faltanteCuota = Valorcuota - sumCapital;

                                    if(modelo.MONTO <= faltanteCuota)
                                    {                                        
                                            RegistrarActualizarPagoDto p = new RegistrarActualizarPagoDto();
                                            p.ID = 0;
                                            p.ID_PRESTAMO = resultado.Respuesta.ID;
                                            p.FECHA_PAGO = modelo.FECHA_PAGO;
                                            p.MONTO = modelo.MONTO;
                                            p.ID_TIPO_PAGO = 45;
                                            p.NUMERO_CUOTA = maxCuota;

                                            var respPagar = await _prestamo.Insertar_Pago_Async<RegistrarActualizarPagoDto, string>(p);


                                            //Actualizar el saldo del monto del prestamo
                                            try
                                            {
                                                var _suma = listPagos.Respuesta?
                                                                    .Where(p => p.ID_PRESTAMO == modelo.ID_PRESTAMO &&
                                                                    p.ID_TIPO_PAGO == 45)
                                                                    .Sum(p => p.MONTO);

                                                decimal _total = (decimal)(_suma + modelo.MONTO);
                                                decimal _newSaldo = prestamos.MONTO - _total;

                                                ActualizarPrestamoDto a = new ActualizarPrestamoDto();
                                                a.ID = modelo.ID_PRESTAMO;
                                                a.SALDO = _newSaldo;

                                                if (_newSaldo <= 0)
                                                {
                                                    a.ID_ESTADO = 52;
                                                }

                                                var actuPrestamo = await _prestamo.Actualizar_Prestamo_Async<ActualizarPrestamoDto, bool>(a);
                                            }
                                            catch (Exception) { }

                                            return Json(new RespuestaDto<bool>
                                            {
                                                Codigo = EstadoOperacion.Bueno,
                                                Mensaje = "El monto indicado fue registrado exitosamente."
                                            });

                                    }
                                    else //El monto es mayor al faltante
                                    {
                                        //Guardamos el faltante del capital en la cuota actual                                        

                                        RegistrarActualizarPagoDto p = new RegistrarActualizarPagoDto();
                                        p.ID = 0;
                                        p.ID_PRESTAMO = resultado.Respuesta.ID;
                                        p.FECHA_PAGO = modelo.FECHA_PAGO;
                                        p.MONTO = (decimal)faltanteCuota;
                                        p.ID_TIPO_PAGO = 45;
                                        p.NUMERO_CUOTA = maxCuota;

                                        var respPagar = await _prestamo.Insertar_Pago_Async<RegistrarActualizarPagoDto, string>(p);

                                        
                                        //Determinar cual seria la cuota en la que quedaria
                                        var sumTotalCapital = listPagos.Respuesta?
                                                                .Where(p => p.ID_PRESTAMO == modelo.ID_PRESTAMO &&                                                        
                                                                p.ID_TIPO_PAGO == 45)
                                                                .Sum(p => p.MONTO);

                                        sumTotalCapital += modelo.MONTO;

                                        int cuotaProxima = (int)Math.Ceiling((double)sumTotalCapital / Valorcuota);

                                        RegistrarActualizarPagoDto pc = new RegistrarActualizarPagoDto();
                                        pc.ID = 0;
                                        pc.ID_PRESTAMO = resultado.Respuesta.ID;
                                        pc.FECHA_PAGO = modelo.FECHA_PAGO;
                                        pc.MONTO = modelo.MONTO - (decimal)faltanteCuota;
                                        pc.ID_TIPO_PAGO = 45;
                                        pc.NUMERO_CUOTA = cuotaProxima;

                                        var respPagarc = await _prestamo.Insertar_Pago_Async<RegistrarActualizarPagoDto, string>(pc);

                                        //Actualizar el saldo del monto del prestamo
                                        try
                                        {
                                            var _suma = listPagos.Respuesta?
                                                                .Where(p => p.ID_PRESTAMO == modelo.ID_PRESTAMO &&
                                                                p.ID_TIPO_PAGO == 45)
                                                                .Sum(p => p.MONTO);

                                            decimal _total = (decimal)(_suma + modelo.MONTO);
                                            decimal _newSaldo = prestamos.MONTO - _total;

                                            ActualizarPrestamoDto a = new ActualizarPrestamoDto();
                                            a.ID = modelo.ID_PRESTAMO;
                                            a.SALDO = _newSaldo;

                                            if (_newSaldo <= 0)
                                            {
                                                a.ID_ESTADO = 52;
                                            }

                                            var actuPrestamo = await _prestamo.Actualizar_Prestamo_Async<ActualizarPrestamoDto, bool>(a);
                                        }
                                        catch (Exception) { }

                                        return Json(new RespuestaDto<bool>
                                        {
                                            Codigo = EstadoOperacion.Bueno,
                                            Mensaje = "El monto indicado fue registrado exitosamente."
                                        });

                                    }

                                }



                            }
                            else //Como no pago, pagar los intereses y el capital.
                            {
                                var saldoIntereses = (decimal)interesAnterior - sumInter;
                                var nuevoCapital = modelo.MONTO - saldoIntereses;





                                //Determinar cual seria la cuota en la que quedaria
                                var sumTotalCapital = listPagos.Respuesta?
                                                    .Where(p => p.ID_PRESTAMO == modelo.ID_PRESTAMO &&
                                                    p.ID_TIPO_PAGO == 45)
                                                    .Sum(p => p.MONTO);

                                sumTotalCapital += nuevoCapital;

                                int cuotaMax = (int)Math.Ceiling((double)sumTotalCapital / Valorcuota);


                                //Agregamos el faltante a los intereses.

                                RegistrarActualizarPagoDto p = new RegistrarActualizarPagoDto();
                                p.ID = 0;
                                p.ID_PRESTAMO = resultado.Respuesta.ID;
                                p.FECHA_PAGO = modelo.FECHA_PAGO;
                                p.MONTO = (decimal)saldoIntereses;
                                p.ID_TIPO_PAGO = 44;
                                p.NUMERO_CUOTA = maxCuota;

                                var respPagar = await _prestamo.Insertar_Pago_Async<RegistrarActualizarPagoDto, string>(p);

                                //Agregamos el pago al capital

                                RegistrarActualizarPagoDto pi = new RegistrarActualizarPagoDto();
                                pi.ID = 0;
                                pi.ID_PRESTAMO = resultado.Respuesta.ID;
                                pi.FECHA_PAGO = modelo.FECHA_PAGO;
                                pi.MONTO = (decimal)nuevoCapital;
                                pi.ID_TIPO_PAGO = 45;
                                pi.NUMERO_CUOTA = cuotaMax;

                                var respPagarCap = await _prestamo.Insertar_Pago_Async<RegistrarActualizarPagoDto, string>(pi);


                                //Actualizar el saldo del monto del prestamo
                                try
                                {
                                    var _suma = listPagos.Respuesta?
                                                        .Where(p => p.ID_PRESTAMO == modelo.ID_PRESTAMO &&
                                                        p.ID_TIPO_PAGO == 45)
                                                        .Sum(p => p.MONTO);
                                    var _sumaFull = _suma + nuevoCapital;


                                    decimal _newSaldo = prestamos.MONTO - (decimal)_sumaFull;

                                    ActualizarPrestamoDto a = new ActualizarPrestamoDto();
                                    a.ID = modelo.ID_PRESTAMO;
                                    a.SALDO = _newSaldo;

                                    if (_newSaldo <= 0)
                                    {
                                        a.ID_ESTADO = 52;
                                    }

                                    var actuPrestamo = await _prestamo.Actualizar_Prestamo_Async<ActualizarPrestamoDto, bool>(a);
                                }
                                catch (Exception) { }

                                return Json(new RespuestaDto<bool>
                                {
                                    Codigo = EstadoOperacion.Bueno,
                                    Mensaje = "El monto indicado fue registrado exitosamente."
                                });

                            }




                            return Json(new RespuestaDto<bool>
                            {
                                Codigo = EstadoOperacion.Bueno,
                                Mensaje = "El pago fue registrado exitosamente."
                            });




                            //bool sw_respPagoInte =false;
                            //bool sw_respPagoCap = false;

                            //if(sumInter < interes)
                            //{
                            //    //-----------------------------------------------------------------
                            //    //------------------ Guardamos los intereses ----------------------
                            //    //-----------------------------------------------------------------
                            //    var cuota = 1;

                            //    if(maxCuota > 0)
                            //    {
                            //        cuota = maxCuota;
                            //    }
                                
                            //    try
                            //    {
                            //        RegistrarActualizarPagoDto p = new RegistrarActualizarPagoDto();
                            //        p.ID = 0;
                            //        p.ID_PRESTAMO = resultado.Respuesta.ID;
                            //        p.FECHA_PAGO = modelo.FECHA_PAGO;
                            //        p.MONTO = interes;
                            //        p.ID_TIPO_PAGO = 44;
                            //        p.NUMERO_CUOTA = cuota;

                            //        var respPagoInte = await _prestamo.Insertar_Pago_Async<RegistrarActualizarPagoDto, string>(p);
                            //        sw_respPagoInte = respPagoInte.Estado;
                            //    }
                            //    catch (Exception) { }
                            //}

                           


                            ////---------------------------------------------------------------------------
                            ////--------Si saldoMonto es mayor a 0 entonces -------------------------------
                            ////---------------------------------------------------------------------------

                            //if (saldoMonto > 0)
                            //{
                            //    var cuota = 1;

                            //    if (maxCuota > 0)
                            //    {
                            //        //Validar la suma de capital contra el valor de la cuota
                            //        if (sumCapital < Valorcuota)
                            //        {
                            //            var salCapital = Valorcuota - sumCapital;

                            //            if(salCapital <= saldoMonto)
                            //            {
                            //                cuota = maxCuota;
                            //            }
                            //            else
                            //            {
                            //            //Insertar 




                            //            }

                            //        }
                            //        else
                            //        {


                            //        }

                                        

                            //    }

                            //    try
                            //    {
                            //        RegistrarActualizarPagoDto p = new RegistrarActualizarPagoDto();
                            //        p.ID = 0;
                            //        p.ID_PRESTAMO = resultado.Respuesta.ID;
                            //        p.FECHA_PAGO = modelo.FECHA_PAGO;
                            //        p.MONTO = saldoMonto;
                            //        p.ID_TIPO_PAGO = 45;
                            //        p.NUMERO_CUOTA = cuota;

                            //        var respPagoCap = await _prestamo.Insertar_Pago_Async<RegistrarActualizarPagoDto, string>(p);
                            //        sw_respPagoCap = respPagoCap.Estado;
                            //    }
                            //    catch (Exception) { }

                            //    try
                            //    { 
                            //        var _saldo = prestamos.SALDO_MONTO - saldoMonto;

                            //        ActualizarPrestamoDto a = new ActualizarPrestamoDto();
                            //        a.ID = modelo.ID_PRESTAMO;
                            //        a.SALDO = _saldo;

                            //        if (_saldo <= 0)
                            //        {
                            //            a.ID_ESTADO = 52;
                            //        }

                            //        var actuPrestamo = await _prestamo.Actualizar_Prestamo_Async<ActualizarPrestamoDto,bool>(a);
                            //    }
                            //    catch (Exception) { }

                            //}
                            
                            //if (sw_respPagoInte && sw_respPagoCap)
                            //{
                            //    return Json(new RespuestaDto<bool>
                            //    {
                            //        Codigo = EstadoOperacion.Bueno,
                            //        Mensaje = "Los montos a intereses y capital fueron registrados exitosamente."
                            //    });
                            //}
                            //else
                            //{
                            //    string txt_mensaje = "Precaución: Se almacenó el pago de intereses: " +
                            //                         (sw_respPagoInte ? "SI" : "NO") +
                            //                         " y Se almacenó el pago a capital: " +
                            //                         (sw_respPagoCap ? "SI" : "NO");

                            //    return Json(new RespuestaDto<bool>
                            //    {
                            //        Codigo = EstadoOperacion.Bueno,
                            //        Mensaje = txt_mensaje
                            //    });

                            //}


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

        async private Task<RespuestaDto<string>> InsertarPagos(RegistrarActualizarPagoDto data)
        {
            try
            {
                RegistrarActualizarPagoDto p = new RegistrarActualizarPagoDto();
                p.ID = data.ID;
                p.ID_PRESTAMO = data.ID_PRESTAMO;
                p.FECHA_PAGO = data.FECHA_PAGO;
                p.MONTO = data.MONTO;
                p.ID_TIPO_PAGO = data.ID_TIPO_PAGO;
                p.NUMERO_CUOTA = data.NUMERO_CUOTA;

                return await _prestamo.Insertar_Pago_Async<RegistrarActualizarPagoDto, string>(p);                               
            }
            catch (Exception ex)
            {
                return new RespuestaDto<string>
                {
                    Codigo = EstadoOperacion.Excepcion,
                    Mensaje = $"Excepción ocurrida: {ex.Message} {ex.InnerException}"
                };
            }
            
        }


    }
}
