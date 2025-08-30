using Comun.DTO.Solicitud;
using Comun.Enumeracion;
using Datos.Contratos.Auditoria;
using Datos.Contratos.Solicitud;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WepPrestamos.Areas.Gestion.Controllers
{
    [Area("Gestion")]
    [Authorize]
    public class AprobarPrestamoController : Controller
    {
        private readonly IRegistroSolicitud _solicitud;
        private readonly IBLConsultar_Detalle_Master _dominio;
        private readonly ILugaresGeograficos _lugares;
        private readonly IInsert_Auditoria _InserAuditoria;


        public AprobarPrestamoController(IRegistroSolicitud solicitud, IBLConsultar_Detalle_Master dominio, ILugaresGeograficos lugares, IInsert_Auditoria insertAuditoria)
        {
            _solicitud = solicitud;
            _dominio = dominio;
            _lugares = lugares;
            _InserAuditoria = insertAuditoria;

        }
        public IActionResult AprobarPrestamo()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerPrestamos()
        {
            var parametros = new Parametros_Consulta_Solicitudes_X_Estado_Dto
            {
                ID_ESTADO = 14 // los pre-aprobados
            };

            var resultado = await _solicitud.ObtenerAsync<Parametros_Consulta_Solicitudes_X_Estado_Dto, List<Respuesta_Consulta_Solicitudes_Prestamo_Dto>>(parametros);

            return Json(resultado);
        }


        public async Task<IActionResult> PrestamoAprobado([FromBody] Parametros_Actualizar_Estado_Solicitud_Dto Obj)
        {
            // Paso de PRE-APROBADO (14) a CREDITO APROBADO (56)
            if (Obj.NuevoEstadoId == 14)
            {
                Obj.NuevoEstadoId = 56;
            }

            try
            {
                var resultado = await _solicitud.ActualizarEstadoSolicitudAsync(Obj);

                if (resultado.Codigo == EstadoOperacion.Bueno)
                    return Json(new { success = true, message = "Solicitud aprobada correctamente." });

                return Json(new { success = false, message = resultado.Mensaje });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error en el servidor: {ex.Message}" });
            }
        }

        public async Task<IActionResult> CancelarPrestamo([FromBody] Parametros_Actualizar_Estado_Solicitud_Dto Obj)
        {
            // Si venía en estado PRE-APROBADO (14) pasa a DENEGADA (17)
            if (Obj.NuevoEstadoId == 14)
            {
                Obj.NuevoEstadoId = 17;
            }

            try
            {
                var resultado = await _solicitud.ActualizarEstadoSolicitudAsync(Obj);

                if (resultado.Codigo == EstadoOperacion.Bueno)
                    return Json(new { success = true, message = "Solicitud cancelada correctamente." });

                return Json(new { success = false, message = resultado.Mensaje });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error en el servidor: {ex.Message}" });
            }
        }


    }
}
