using Comun.DTO.Auditoria;
using Comun.DTO.Generales;
using Comun.Enumeracion;
using Datos.Administracion;
using Datos.Contexto;
using Datos.Contratos.Auditoria;
using Datos.Contratos.Solicitud;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Negocio.InsertUsuario;

namespace WepPrestamos.Areas.Administracion.Controllers
{
    [Area("Administracion")]
    //[Authorize]
    public class GestionUsuariosController : Controller
    {

        private readonly IInsertUsuario _insertUsuario;
        private readonly IInsert_Auditoria _insertAuditoria;
        private readonly IBLConsultar_Detalle_Master _dominio;
        private readonly IInsertRolUsuario _insertrolusuario;
        private readonly ContextoGeneral _context;

        public GestionUsuariosController(IInsertUsuario insertUsuario, IInsert_Auditoria insertAuditoria, IBLConsultar_Detalle_Master dominio, IInsertRolUsuario insertrolusuario, ContextoGeneral context)
        {
            _insertUsuario = insertUsuario;
            _insertAuditoria = insertAuditoria;
            _insertrolusuario = insertrolusuario;
            _dominio = dominio;
            _context = context;
           
        }


        [HttpGet]
       
        public async Task<IActionResult> GestionUsuarios()
        {
            await CargarListasAsync(); // Espera correctamente a que cargue la ViewBag
            return View();
        }

        // capturamos y enviamos todo al servicio la capa de negocio 
        [HttpPost]
        public async Task<IActionResult> GestionUsuarios(IFormCollection form, IFormFile FOTO)
        {
            var ObjRegistro = new UsuarioDto
            {
                USUARIO_EMPRESARIAL = form["USUARIO_EMPRESARIAL"].ToString().Trim().ToUpper(),
                ID_TIPO_IDENTIFICACION = Convert.ToInt64(form["ID_TIPO_IDENTIFICACION"]),
                NRO_IDENTIFICACION = Convert.ToInt64(form["NRO_IDENTIFICACION"]),
                PRIMER_NOMBRE = form["PRIMER_NOMBRE"].ToString().Trim().ToUpper(),
                SEGUNDO_NOMBRE = form["SEGUNDO_NOMBRE"].ToString().Trim().ToUpper(),
                PRIMER_APELLIDO = form["PRIMER_APELLIDO"].ToString().Trim().ToUpper(),
                SEGUNDO_APELLIDO = form["SEGUNDO_APELLIDO"].ToString().Trim().ToUpper(),
                EMAIL = form["EMAIL"],
                TELEFONO = form["TELEFONO"],
                CONTRASENA = form["NEWCONTRASENA"],
                HABILITADO = form["HABILITADO"] == "on",
                FOTO = await ConvertirArchivoABytes(FOTO)
            };

            try
            {
                var respuesta = await _insertUsuario.CrearUsuarioAsync(ObjRegistro);

                if (respuesta.Respuesta)
                {
                    var claimIdUsuario = User.FindFirst("IdUsuario")?.Value;

                    if (string.IsNullOrWhiteSpace(claimIdUsuario))
                    {
                        TempData["SweetAlert"] = "error";
                        TempData["Titulo"] = "Error de Autenticación";
                        TempData["Mensaje"] = "No se pudo identificar al usuario actual. Vuelva a iniciar sesión.";
                        ViewBag.LimpiarFormulario = false;

                        await CargarListasAsync();
                        return View();
                    }

                    var ObjAuditoria = new Insert_AuditoriaDto
                    {
                        Id_Tipo_Auditoria = 2,
                        Ip_Maquina = HttpContext.Connection.RemoteIpAddress?.ToString(),
                        fecha = DateTime.Now.Date,
                        Id_Usuario = long.Parse(claimIdUsuario),
                        Observacion = $"Creación de usuario nuevo:{ObjRegistro.USUARIO_EMPRESARIAL}"
                    };

                    var respuestaAuditoria = await _insertAuditoria.InsertarAuditoriaAsync(ObjAuditoria);
                  

                    TempData["SweetAlert"] = "success";
                    TempData["Mensaje"] = respuestaAuditoria.Codigo != EstadoOperacion.Bueno
                        ? "Usuario creado, pero no se pudo registrar la auditoría."
                        : "¡Usuario registrado correctamente!";

                    TempData["Titulo"] = "Operación Exitosa";
                    ViewBag.LimpiarFormulario = true;
                }
                else
                {
                    TempData["SweetAlert"] = "error";
                    TempData["Titulo"] = "Error en Registro";
                    TempData["Mensaje"] = respuesta.Mensaje ?? "Ocurrió un error al crear el usuario.";
                    ViewBag.LimpiarFormulario = false;
                }
            }
            catch (Exception ex)
            {
                TempData["SweetAlert"] = "error";
                TempData["Titulo"] = "Error en Sistema";
                TempData["Mensaje"] = $"Error técnico al registrar usuario: {ex.Message}";
                ViewBag.LimpiarFormulario = false;
            }

            await CargarListasAsync();
            return View();
        }

        private async Task<byte[]> ConvertirArchivoABytes(IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0)
                return null;

            using var ms = new MemoryStream();
            await archivo.CopyToAsync(ms);
            return ms.ToArray();
        }



        private async Task CargarListasAsync()
        {
            try
            {
                var _listaTipoDoc = await _dominio.ListaDetalle(1);

                // Clave del ViewBag debe coincidir EXACTAMENTE con el primer parámetro del DropDownList
                ViewBag.ID_TIPO_IDENTIFICACION = new SelectList(_listaTipoDoc, "Id", "Nombre");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error cargando listas: " + ex.Message;
            }
        }

    }

}


