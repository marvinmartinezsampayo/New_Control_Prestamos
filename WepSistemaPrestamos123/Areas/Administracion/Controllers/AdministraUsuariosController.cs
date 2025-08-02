using Comun.DTO.BuscarUsuario;
using Comun.DTO.Generales;
using Comun.Enumeracion;
using Datos.Contexto;
using Datos.Contratos.Auditoria;
using Datos.Contratos.Solicitud;
using Datos.Contratos.Usuario;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WepPrestamos.Helpers;
using Negocio.InsertUsuario;
using Newtonsoft.Json;
using Datos.Contratos.Login;
using Datos.Administracion;
using Comun.DTO.InsertRolUsuario;
using Comun.DTO.Auditoria;

namespace WepPrestamos.Areas.Administracion.Controllers
{
    [Area("Administracion")]
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

        public async Task<IActionResult> Index()
        {
            await CargarListasAsync();
            ViewBag.Mensaje = TempData["Mensaje"];
            ViewBag.Titulo = TempData["Titulo"];
            ViewBag.SweetAlert = TempData["SweetAlert"];
            ViewBag.LimpiarFormulario = TempData["LimpiarFormulario"];
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> BuscarPorIdentificacion(string nroIdentificacion)
        {
            await CargarListasAsync();

            if (string.IsNullOrWhiteSpace(nroIdentificacion))
            {
                ViewBag.SweetAlert = "warning";
                ViewBag.Titulo = "Campo vacío";
                ViewBag.Mensaje = "Por favor ingresa un número de identificación.";
                return View("Index");
            }

            var request = new BuscarUsuarioNurIdentificacionPet
            {
                NRO_IDENTIFICACION = Convert.ToInt64(nroIdentificacion)
            };

            var resultado = await _iBusacrUsuarioNurIdentificacion
                .ObtenerUsuarioPorIdentificacionAsync<BuscarUsuarioNurIdentificacionPet, UsuarioDto>(request);
           
            if (resultado.Codigo == EstadoOperacion.Bueno)
            {
               
                var usuario = resultado.Respuesta;
                ViewBag.Usuario = usuario;
                ViewBag.FotoBase64 = ImagenHelper.ObtenerFotoBase64(usuario.FOTO);

                if (usuario == null)
                {
                    throw new Exception("El objeto 'usuario' es null");
                }

                // validamos que roles tiene 
                var roles = await _rolesUsuarioService.ConsultarRolesUsuario(usuario.ID);
                ViewBag.RolesUsuario = roles ?? new List<Roles_X_UsuarioDto>();

                ViewBag.SweetAlert = "success";
                ViewBag.Titulo = "Usuario Encontrado";
                ViewBag.Mensaje = $"El usuario con identificación {nroIdentificacion} fue encontrado exitosamente.";
            }
            else
            {
                ViewBag.SweetAlert = "error";
                ViewBag.Titulo = "No encontrado";
                ViewBag.Mensaje = resultado.Mensaje;
            }

            return View("Index");
        }


        [HttpPost]
        public async Task<IActionResult> GestionarRol(string rolSeleccionado, string accion, int idUsuario)
        {
            if (string.IsNullOrEmpty(rolSeleccionado))
            {
                TempData["Mensaje"] = "Debes seleccionar un rol.";
                TempData["Titulo"] = "Error";
                TempData["SweetAlert"] = "error";
                return RedirectToAction("Index");
            }

            var Obj = new InsertRolUsuarioDto
            {
                Id_Usuario = idUsuario,
                Id_Rol = int.Parse(rolSeleccionado),
                Habilitado = accion == "asignar"
            };

            try
            {
                var resultado = await _insertrolusuario.InsertarRolUsuarioAsync(Obj);

                if (resultado.Estado)
                {
                    TempData["SweetAlert"] = "success";
                    TempData["Titulo"] = "Rol actualizado";
                    TempData["Mensaje"] = resultado.Mensaje;

                    // Traer usuario afectado por ID
                    var usuarioAfectado = await _iBusacrUsuarioNurIdentificacion.ObtenerUsuarioPorIdentificacionAsync<BuscarUsuarioNurIdentificacionPet, UsuarioDto>(
                        new BuscarUsuarioNurIdentificacionPet
                        {
                            NRO_IDENTIFICACION = idUsuario 
                        }
                    );

                    var nombreAfectado = usuarioAfectado.Respuesta?.USUARIO_EMPRESARIAL ?? $"ID: {idUsuario}";

                    var ObjAuditoria = new Insert_AuditoriaDto
                    {
                        Id_Tipo_Auditoria = accion == "asignar" ? 3 : 4, 
                        Ip_Maquina = HttpContext.Connection.RemoteIpAddress?.ToString(),
                        fecha = DateTime.Now,
                        Id_Usuario = long.Parse(User.FindFirst("IdUsuario")?.Value),
                        Observacion = $"Se {(accion == "asignar" ? "asignó" : "quitó")} el rol ID {rolSeleccionado} al usuario {nombreAfectado}."
                    };

                    await _insertAuditoria.InsertarAuditoriaAsync(ObjAuditoria);
                }
                else
                {
                    TempData["SweetAlert"] = "error";
                    TempData["Titulo"] = "Error al actualizar";
                    TempData["Mensaje"] = resultado.Mensaje;
                }
            }
            catch (Exception ex)
            {
                TempData["SweetAlert"] = "error";
                TempData["Titulo"] = "Error inesperado";
                TempData["Mensaje"] = "Ocurrió un error al asignar o quitar el rol. Detalles: " + ex.Message;
            }

            return RedirectToAction("Index");
        }



        private async Task CargarListasAsync()
        {
            try
            {
                var _listaRoles = await _dominio.ListaDetalle(2);
                ViewBag.Roles = new SelectList(_listaRoles, "Id", "Descripcion");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error cargando listas: " + ex.Message;
            }
        }
    }
}
