using Comun.DTO.Auditoria;
using Comun.DTO.BuscarUsuario;
using Comun.DTO.Generales;
using Comun.DTO.InsertRolUsuario;
using Comun.Enumeracion;
using Datos.Administracion;
using Datos.Contexto;
using Datos.Contratos.Auditoria;
using Datos.Contratos.Login;
using Datos.Contratos.Solicitud;
using Datos.Contratos.Usuario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Negocio.InsertUsuario;
using WepPrestamos.Helpers;

namespace WepPrestamos.Areas.Administracion.Controllers
{
    [Area("Administracion")]
    [Authorize]
    public class AdministraUsuariosController : Controller
    {
        private readonly IBusacrUsuarioNurIdentificacion _iBusacrUsuarioNurIdentificacion;
        private readonly IInsert_Auditoria _insertAuditoria;
        private readonly IBLConsultar_Detalle_Master _dominio;
        private readonly IGestionUsuario _rolesUsuarioService;
        private readonly IInsertRolUsuario _insertrolusuario;
        private readonly ContextoGeneral _context;

        public AdministraUsuariosController(
            IInsert_Auditoria insertAuditoria,
            IBLConsultar_Detalle_Master dominio,
            IBusacrUsuarioNurIdentificacion buscarUsuarioIdentificacion,
            IGestionUsuario rolesUsuarioService,
            IInsertRolUsuario insertrolusuario,
            ContextoGeneral context)
        {
            _iBusacrUsuarioNurIdentificacion = buscarUsuarioIdentificacion;
            _insertAuditoria = insertAuditoria;
            _dominio = dominio;
            _rolesUsuarioService = rolesUsuarioService;
            _insertrolusuario = insertrolusuario;
            _context = context;
        }

        // Solo carga la vista vacía
        public async Task<IActionResult> Index()
        {
            await CargarListasAsync();
            return View();
        }

        // ✅ Ahora retorna JSON — el JS maneja la UI
        [HttpGet]
        public async Task<IActionResult> BuscarPorIdentificacion(string nroIdentificacion)
        {
            if (string.IsNullOrWhiteSpace(nroIdentificacion))
                return Json(new { exito = false, mensaje = "Por favor ingresa un número de identificación." });

            var request = new BuscarUsuarioNurIdentificacionPet
            {
                NRO_IDENTIFICACION = Convert.ToInt64(nroIdentificacion)
            };

            var resultado = await _iBusacrUsuarioNurIdentificacion
                .ObtenerUsuarioPorIdentificacionAsync<BuscarUsuarioNurIdentificacionPet, UsuarioDto>(request);

            if (resultado.Codigo != EstadoOperacion.Bueno)
                return Json(new { exito = false, mensaje = resultado.Mensaje });

            var usuario = resultado.Respuesta;
            var roles = await _rolesUsuarioService.ConsultarRolesUsuario(usuario.ID);
            var foto = ImagenHelper.ObtenerFotoBase64(usuario.FOTO);

            return Json(new
            {
                exito = true,
                usuario = new
                {
                    id = usuario.ID,
                    nombreCompleto = $"{usuario.PRIMER_NOMBRE} {usuario.SEGUNDO_NOMBRE} {usuario.PRIMER_APELLIDO}",
                    usuarioEmpresarial = usuario.USUARIO_EMPRESARIAL,
                    nroIdentificacion = usuario.NRO_IDENTIFICACION,
                    telefono = usuario.TELEFONO,
                    foto = foto
                },
                roles = roles?.Select(r => new
                {
                    nombre = r.ROL_STR,
                    descripcion = r.ROL_DESCRIPCION
                })
            });
        }

        // ✅ Ahora retorna JSON — el JS maneja la UI
        [HttpPost]
        public async Task<IActionResult> GestionarRol([FromBody] GestionarRolRequest request)
        {
            if (request == null || request.IdUsuario == 0)
                return Json(new { exito = false, mensaje = "Datos inválidos." });

            var obj = new InsertRolUsuarioDto
            {
                Id_Usuario = request.IdUsuario,
                Id_Rol = request.IdRol,
                Habilitado = request.Accion == "asignar"
            };

            try
            {
                var resultado = await _insertrolusuario.InsertarRolUsuarioAsync(obj);

                if (!resultado.Estado)
                    return Json(new { exito = false, mensaje = resultado.Mensaje });

                var objAuditoria = new Insert_AuditoriaDto
                {
                    Id_Tipo_Auditoria = request.Accion == "asignar" ? 3 : 4,
                    Ip_Maquina = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    fecha = DateTime.Now,
                    Id_Usuario = long.Parse(User.FindFirst("IdUsuario")?.Value),
                    Observacion = $"Se {(request.Accion == "asignar" ? "asignó" : "quitó")} el rol ID {request.IdRol} al usuario ID {request.IdUsuario}."
                };

                await _insertAuditoria.InsertarAuditoriaAsync(objAuditoria);

                return Json(new { exito = true, mensaje = resultado.Mensaje });
            }
            catch (Exception ex)
            {
                return Json(new { exito = false, mensaje = $"Error inesperado: {ex.Message}" });
            }
        }

        private async Task CargarListasAsync()
        {
            try
            {
                var listaRoles = await _dominio.ListaDetalle(2);
                ViewBag.Roles = new SelectList(listaRoles, "Id", "Descripcion");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error cargando listas: " + ex.Message;
            }
        }
    }

    // Clase para recibir el body del fetch de GestionarRol
    public class GestionarRolRequest
    {
        public int IdUsuario { get; set; }
        public int IdRol { get; set; }
        public string Accion { get; set; }
    }
}