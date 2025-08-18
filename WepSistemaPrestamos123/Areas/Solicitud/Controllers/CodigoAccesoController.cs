using Comun.DTO.Auditoria;
using Comun.DTO.CodigoAcceso;
using Comun.Enumeracion;
using Comun.Generales;
using Datos.Administracion;
using Datos.CodigoAcceso;
using Datos.Contexto;
using Datos.Contratos.Auditoria;
using Datos.Contratos.Solicitud;
using Microsoft.AspNetCore.Mvc;
using Negocio.CodigoAcceso;

namespace WepPrestamos.Areas.Solicitud.Controllers
{
    [Area("Solicitud")]
    //[Authorize]
    public class CodigoAccesoController : Controller
    {

        private readonly IInsertCodigoAcceso _insertCodigoAcceso;
        private readonly IInsert_Auditoria _insertAuditoria;
        private readonly IListarCodigosAcceso _listarCodigosAcceso;
        private readonly ContextoGeneral _context;

        public CodigoAccesoController(IInsertUsuario insertUsuario, IInsert_Auditoria insertAuditoria, IInsertCodigoAcceso insertCodigoAcceso, IListarCodigosAcceso listarCodigosAcceso, ContextoGeneral context)
        {
            _insertCodigoAcceso = insertCodigoAcceso;
            _insertAuditoria = insertAuditoria;
            _listarCodigosAcceso = listarCodigosAcceso;
            _context = context;

        }
        public IActionResult IndexCodigoAcceso()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GuardarCodigoAcceso([FromBody] Codigo_AccesoDto dto)
        {


            try
            {
                var resultado = await _insertCodigoAcceso.InsertarCodigoAccesoAsync(dto);

                if (resultado.Estado)
                {
                    var auditoria = new Insert_AuditoriaDto
                    {
                        Id_Tipo_Auditoria = 5, // = creación de código
                        Ip_Maquina = HttpContext.Connection.RemoteIpAddress?.ToString(),
                        fecha = DateTime.Now,
                        Id_Usuario = dto.Id_Usuario,
                        Observacion = $"Se generó el código de acceso: {dto.CodigoAcesso}"
                    };

                    await _insertAuditoria.InsertarAuditoriaAsync(auditoria);

                    return Json(new { exito = true, mensaje = "Código guardado exitosamente." });
                }

                return Json(new { exito = false, mensaje = resultado.Mensaje });
            }
            catch (Exception ex)
            {
                return Json(new { exito = false, mensaje = "Error al guardar: " + ex.Message });
            }
        }


        [HttpGet]
        public async Task<IActionResult> ObtenerCodigosHoy()
        {
            var resultado = await _listarCodigosAcceso.ObtenerCodigosDeHoyGenerico<List<Codigo_AccesoDto>>();

            if (resultado.Codigo == EstadoOperacion.Bueno && resultado.Respuesta != null)
                return Json(resultado.Respuesta);

            return Json(new List<Codigo_AccesoDto>());
        }

        [HttpGet]
        public async Task<IActionResult> BuscarCodigosPorFechas(DateTime fechaInicio, DateTime fechaFin, string Codigo)
        {
            var resultado = await _listarCodigosAcceso.ObtenerCodigosPorFechasAsync<List<Codigo_AccesoDto>>(fechaInicio, fechaFin,Codigo);

            if (resultado.Codigo == EstadoOperacion.Bueno && resultado.Respuesta != null)
                return Json(resultado.Respuesta);

            return Json(new List<Codigo_AccesoDto>());
        }


        [HttpPost]
        public async Task<IActionResult> ActualizarCodigoAcceso([FromBody] UpdateCodigoosAcccesoDto dto)
        {
            try
            {
                var resultado = await _listarCodigosAcceso.ActualizarCodigoAccesoAsync(dto);
                            
                if (resultado.Estado)
                {
                     var auditoria = new Insert_AuditoriaDto
                    {
                        Id_Tipo_Auditoria = 6, // = Update Codigo Acceso
                        Ip_Maquina = HttpContext.Connection.RemoteIpAddress?.ToString(),
                        fecha = DateTime.Now,
                        Id_Usuario = dto.Id_Usuario,
                        Observacion = $"modigo el Codigo de Acceo: {dto.Codigo}"
                    };

                    await _insertAuditoria.InsertarAuditoriaAsync(auditoria);
                }
                // Ya contiene Estado, Mensaje y Respuesta
                return Json(resultado);
            }
            catch (Exception ex)
            {
                // Si algo falla antes o fuera del servicio
                return Json(new RespuestaDto<string>
                {
                    Codigo = EstadoOperacion.Excepcion,
                    Mensaje = "Error inesperado: " + ex.Message,
                    Respuesta = null
                });
            }
        }

    }
}


