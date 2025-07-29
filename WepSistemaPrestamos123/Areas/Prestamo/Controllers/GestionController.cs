using Comun.DTO.BuscarUsuario;
using Comun.DTO.Generales;
using Comun.DTO.Prestamo;
using Comun.DTO.Solicitud;
using Comun.Enumeracion;
using Datos.Contratos.Prestamo;
using Datos.Contratos.Solicitud;
using Datos.Contratos.Usuario;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

            var resultado = await _prestamo.Obtener_X_Identificacion_Async<Int64 ,List<Prestamo_Dto>>(request.NRO_IDENTIFICACION);

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
        public async Task<IActionResult> RegistrarPago([FromBody] RegistrarPagoDto modelo)
        {
            try
            {
                // Validar modelo
                if (!ModelState.IsValid)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Datos inválidos",
                        errors = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                            .ToList()
                    });
                }

                //// Validaciones adicionales de negocio
                //if (modelo.MONTO <= 0)
                //{
                //    return Json(new
                //    {
                //        success = false,
                //        message = "El monto debe ser mayor a cero"
                //    });
                //}

                //if (modelo.FECHA_PAGO > DateTime.Now)
                //{
                //    return Json(new
                //    {
                //        success = false,
                //        message = "La fecha de pago no puede ser futura"
                //    });
                //}

                //// Aquí iría tu lógica de negocio
                //// Ejemplo: obtener información del préstamo
                //var prestamo = await ObtenerPrestamoPorId(modelo.ID_PRESTAMO);
                //if (prestamo == null)
                //{
                //    return Json(new
                //    {
                //        success = false,
                //        message = "Préstamo no encontrado"
                //    });
                //}

                //// Calcular distribución del pago (interés vs capital)
                //var distribucionPago = CalcularDistribucionPago(prestamo, modelo.MONTO);

                //// Registrar el pago en la base de datos
                //var resultadoPago = await RegistrarPagoEnBD(new PagoEntity
                //{
                //    IdPrestamo = modelo.ID_PRESTAMO,
                //    FechaPago = modelo.FECHA_PAGO,
                //    MontoTotal = modelo.MONTO,
                //    MontoInteres = distribucionPago.MontoInteres,
                //    MontoCapital = distribucionPago.MontoCapital,
                //    FechaRegistro = DateTime.Now
                //});

                //if (resultadoPago.Exitoso)
                //{
                //    return Json(new
                //    {
                //        success = true,
                //        message = "Pago registrado exitosamente",
                //        data = new
                //        {
                //            idPago = resultadoPago.IdPago,
                //            montoInteres = distribucionPago.MontoInteres,
                //            montoCapital = distribucionPago.MontoCapital,
                //            saldoPendiente = prestamo.SaldoPendiente - distribucionPago.MontoCapital
                //        }
                //    });
                //}
                //else
                //{
                //    return Json(new
                //    {
                //        success = false,
                //        message = resultadoPago.MensajeError
                //    });
                //}

                return View("Pago");

            }
            catch (Exception ex)
            {
                // Log del error
                // _logger.LogError(ex, "Error al registrar pago para préstamo {IdPrestamo}", modelo.ID_PRESTAMO);

                return Json(new
                {
                    success = false,
                    message = "Ocurrió un error interno. Intente nuevamente."
                });
            }
        }




    }
}
