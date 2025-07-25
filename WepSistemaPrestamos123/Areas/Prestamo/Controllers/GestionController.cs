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

    }
}
