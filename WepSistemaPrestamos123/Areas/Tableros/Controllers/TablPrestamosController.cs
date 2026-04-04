using Comun.DTO.Prestamo;
using Comun.Enumeracion;
using Comun.Generales;
using Datos.Contratos.Prestamo;
using Datos.Contratos.Solicitud;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Negocio.Gestion;
using WepPrestamos.Areas.Prestamo.Controllers;

namespace WepPrestamos.Areas.Tableros.Controllers
{
    [Area("Tableros")]
    [Authorize]
    public class TablPrestamosController : Controller
    {
        private readonly ILogger<TablPrestamosController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IBLConsultar_Detalle_Master _dominio;
        private readonly IGestionPrestamo _prestamo;

        public TablPrestamosController
        (
            IConfiguration configuration,
            ILogger<TablPrestamosController> logger,
            IBLConsultar_Detalle_Master dominio,
            IGestionPrestamo prestamo
        )
        {
            _configuration = configuration;
            _logger = logger;
            _dominio = dominio;
            _prestamo = prestamo;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                decimal rentabilidad = 0m;

                var resultado = await _prestamo.Obtener_All_Prestamos_Async<bool, List<PrestamoResumenDto>>(true);
                var totalDeposito = await _prestamo.Obtener_Sumatoria_Depositos_Inversores_Async<decimal>();
                var totalSaldoCreditos = await _prestamo.Obtener_Sumatoria_Saldos_Creditos_Async<decimal>();
                var totalPagoIntereses = await _prestamo.Obtener_Sumatoria_Monto_Pagos_Async<long,decimal>(44);
                var totalPagoCapital = await _prestamo.Obtener_Sumatoria_Monto_Pagos_Async<long, decimal>(45);
                var totalPagoMora = await _prestamo.Obtener_Sumatoria_Monto_Pagos_Async<long, decimal>(46);

                var _listaTipoMotivoMulta = await _dominio.ListaDetalle(60);
                var _listaEstadoMulta = await _dominio.ListaDetalle(63);


                if (totalSaldoCreditos.Respuesta > 0 && totalDeposito.Respuesta > 0)
                {
                    rentabilidad = totalSaldoCreditos.Respuesta - totalDeposito.Respuesta;
                }

                if (resultado.Codigo == EstadoOperacion.Bueno)
                {
                    ViewBag.ListPrestamos = resultado.Respuesta;

                    ViewBag.CapitalDisponible = (totalDeposito.Respuesta + totalPagoIntereses.Respuesta) - totalSaldoCreditos.Respuesta;
                    ViewBag.TotalCapital = totalDeposito.Respuesta;
                    ViewBag.TotalSaldoCreditos = totalSaldoCreditos.Respuesta;
                    ViewBag.TotalPagoIntereses = totalPagoIntereses.Respuesta;
                    ViewBag.TotalPagoCapital = totalPagoCapital.Respuesta;
                    ViewBag.Rentabilidad = (totalPagoIntereses.Respuesta / totalDeposito.Respuesta) * 100;

                    ViewBag.listTipoTipoMotivoMulta = new SelectList(_listaTipoMotivoMulta, "Id", "Nombre");
                    ViewBag.listEstadoMulta = new SelectList(_listaEstadoMulta, "Id", "Nombre");
                }
                else
                {
                    ViewBag.SweetAlert = "error";
                    ViewBag.Titulo = "No encontrado";
                    ViewBag.Mensaje = resultado.Mensaje;
                }

                return View("TableroPrestamos");
            }
            catch (Exception ex)
            {
                return Json(new RespuestaDto<bool>
                {
                    Codigo = EstadoOperacion.Malo,
                    Mensaje = ex.Message
                });
            }
        }
                

    }
}
