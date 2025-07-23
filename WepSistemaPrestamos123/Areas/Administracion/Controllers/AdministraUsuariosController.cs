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

namespace WepPrestamos.Areas.Administracion.Controllers
{
    [Area("Administracion")]
    public class AdministraUsuariosController : Controller
    {
        private readonly IBusacrUsuarioNurIdentificacion _iBusacrUsuarioNurIdentificacion;
        private readonly IInsert_Auditoria _insertAuditoria;
        private readonly IBLConsultar_Detalle_Master _dominio;
        private readonly IGestionUsuario _rolesUsuarioService;
        private readonly ContextoGeneral _context;

        public AdministraUsuariosController(
            IInsert_Auditoria insertAuditoria,
            IBLConsultar_Detalle_Master dominio,
            IBusacrUsuarioNurIdentificacion buscarUsuarioIdentificacion,
            IGestionUsuario rolesUsuarioService,
            ContextoGeneral context)
        {
            _iBusacrUsuarioNurIdentificacion = buscarUsuarioIdentificacion;
            _insertAuditoria = insertAuditoria;
            _dominio = dominio;
            rolesUsuarioService = rolesUsuarioService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            await CargarListasAsync();
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
