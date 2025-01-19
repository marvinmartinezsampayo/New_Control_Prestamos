using Comun.DTO.Seguridad;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Comun.DTO.Generales;
using Datos.Contratos.Login;
using Microsoft.AspNetCore.Identity;

namespace WepPrestamos.Controllers
{
    public class LoginController : Controller
    {
        //private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGestionUsuario _gestionUsuario;

        public LoginController(IGestionUsuario gestionUsuario)
        {           
            _gestionUsuario = gestionUsuario;
        }
              


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnurl = null)
        {

            if (User.Identity.IsAuthenticated == true)
            {
                if (string.IsNullOrEmpty(returnurl))
                {
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(HttpContext.User.Identity));
                    return RedirectToAction(nameof(HomeController.Index), "Home");
                }
                else
                {
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(HttpContext.User.Identity));
                    return RedirectToAction(nameof(HomeController.Index), "Home", new { id = returnurl });
                }
            }
            ViewData["ReturnUrl"] = returnurl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDto loginUsuario, string returnurl = null)
        {
            // Configurar encabezados para evitar caché
            Response.Headers.Add("Cache-Control", "no-store, no-cache, must-revalidate, max-age=0");
            Response.Headers.Add("Pragma", "no-cache");
            Response.Headers.Add("Expires", "0");

            ViewData["ReturnUrl"] = returnurl;
            returnurl = returnurl ?? Url.Action(nameof(HomeController.Index), "Home");
            //Url.Content("~/Home/Index");

            if (!ModelState.IsValid)
                return View(loginUsuario);

            string usuario = loginUsuario.Usuario;
            string contrasena = loginUsuario.Clave;


            //****** Validamos la contraseña del usuario
            UsuarioDto UsuarioActivo = await _gestionUsuario.ValidarLogin(loginUsuario);            

            if (UsuarioActivo.PRIMER_APELLIDO =="" || UsuarioActivo.HABILITADO == false)
            {
                ViewData["ReturnUrl"] = returnurl;
                ModelState.AddModelError("", "Usuario o Contraseña incorrecta, revise");
                return View();
            }
            else
            {
                //Obtener IP
                //var Ip = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();            
                //HttpContext.Session.SetString("IpMaquina", Ip);

                //Consultamos los Roles

                List <Roles_X_UsuarioDto> listRoles = await _gestionUsuario.ConsultarRolesUsuario(UsuarioActivo.ID);

                //generamos los claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, Convert.ToString(UsuarioActivo.NRO_IDENTIFICACION)),
                    new Claim("IdUsuario", Convert.ToString(UsuarioActivo.ID)),
                    new Claim("UsuarioEmpresarial", Convert.ToString(UsuarioActivo.USUARIO_EMPRESARIAL)),
                    new Claim("Identificacion", Convert.ToString(UsuarioActivo.NRO_IDENTIFICACION)),
                    new Claim("IdTipoIdentificacion", Convert.ToString(UsuarioActivo.ID_TIPO_IDENTIFICACION)),
                    new Claim("PrimerNombre", Convert.ToString(UsuarioActivo.PRIMER_NOMBRE)),
                    new Claim("SegundoNombre", Convert.ToString(UsuarioActivo.SEGUNDO_NOMBRE)),
                    new Claim("PrimerApellido", Convert.ToString(UsuarioActivo.PRIMER_APELLIDO)),
                    new Claim("SegundoApellido", Convert.ToString( UsuarioActivo.SEGUNDO_APELLIDO)),
                    new Claim("NombreFull", Convert.ToString(UsuarioActivo.PRIMER_NOMBRE + " " + UsuarioActivo.SEGUNDO_NOMBRE + " " + UsuarioActivo.PRIMER_APELLIDO + " " + UsuarioActivo.SEGUNDO_APELLIDO)),
                    new Claim("Email",Convert.ToString( UsuarioActivo.EMAIL)),
                    new Claim("Telefono",Convert.ToString( UsuarioActivo.TELEFONO)),
                    new Claim("Clave",Convert.ToString( UsuarioActivo.CONTRASENA)),
                    new Claim("Estado",Convert.ToString( UsuarioActivo.HABILITADO))
                };

                foreach (var rol in listRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, Convert.ToString(rol.ID_ROL)));
                    claims.Add(new Claim(ClaimTypes.Actor, Convert.ToString(rol.ROL_STR)));
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        new AuthenticationProperties
                        {
                            IsPersistent = true, // Para que la cookie sea persistente
                            ExpiresUtc = DateTime.UtcNow.AddMinutes(1) // Tiempo de expiración
                        });
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }


        [HttpGet]
        public async Task<IActionResult> CerrarSesion()
        {
            //HttpContext.Session.Clear();
            //await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            //return RedirectToAction(nameof(Login));
            //// Cierra la sesión del usuario
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }
    }
}
