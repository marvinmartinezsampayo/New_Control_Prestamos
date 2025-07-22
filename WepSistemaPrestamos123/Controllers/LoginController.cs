using Comun.DTO.Seguridad;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Comun.DTO.Generales;
using Datos.Contratos.Login;
using Microsoft.AspNetCore.Http;

namespace WepPrestamos.Controllers
{
    public class LoginController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGestionUsuario _gestionUsuario;

        public LoginController(IGestionUsuario gestionUsuario, IHttpContextAccessor httpContextAccessor)
        {
            _gestionUsuario = gestionUsuario;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnurl = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (string.IsNullOrEmpty(returnurl))
                {
                    return RedirectToAction(nameof(HomeController.Index), "Home");
                }
                else
                {
                    return Redirect(returnurl);
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
            Response.Headers.Add("Cache-Control", "no-store, no-cache, must-revalidate, max-age=0");
            Response.Headers.Add("Pragma", "no-cache");
            Response.Headers.Add("Expires", "0");

            ViewData["ReturnUrl"] = returnurl ?? Url.Action(nameof(HomeController.Index), "Home");

            if (!ModelState.IsValid)
                return View(loginUsuario);

            UsuarioDto UsuarioActivo = await _gestionUsuario.ValidarLogin(loginUsuario);

            if (string.IsNullOrWhiteSpace(UsuarioActivo.PRIMER_APELLIDO) || !UsuarioActivo.HABILITADO)
            {
                ModelState.AddModelError("", "Usuario o Contraseña incorrecta.");
                return View();
            }

            // Obtener IP
            var Ip = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
            HttpContext.Session.SetString("IpMaquina", Ip ?? "");

            // Roles
            List<Roles_X_UsuarioDto> listRoles = await _gestionUsuario.ConsultarRolesUsuario(UsuarioActivo.ID);

            // Claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, Convert.ToString(UsuarioActivo.NRO_IDENTIFICACION)),
                new Claim("IdUsuario", UsuarioActivo.ID.ToString()),
                new Claim("UsuarioEmpresarial", UsuarioActivo.USUARIO_EMPRESARIAL ?? ""),
                new Claim("Identificacion", Convert.ToString(UsuarioActivo.NRO_IDENTIFICACION)),
                new Claim("IdTipoIdentificacion", UsuarioActivo.ID_TIPO_IDENTIFICACION.ToString()),
                new Claim("PrimerNombre", UsuarioActivo.PRIMER_NOMBRE ?? ""),
                new Claim("SegundoNombre", UsuarioActivo.SEGUNDO_NOMBRE ?? ""),
                new Claim("PrimerApellido", UsuarioActivo.PRIMER_APELLIDO ?? ""),
                new Claim("SegundoApellido", UsuarioActivo.SEGUNDO_APELLIDO ?? ""),
                new Claim("NombreFull", $"{UsuarioActivo.PRIMER_NOMBRE} {UsuarioActivo.SEGUNDO_NOMBRE} {UsuarioActivo.PRIMER_APELLIDO} {UsuarioActivo.SEGUNDO_APELLIDO}".Trim()),
                new Claim("Email", UsuarioActivo.EMAIL ?? ""),
                new Claim("Telefono", UsuarioActivo.TELEFONO ?? ""),
                new Claim("Estado", UsuarioActivo.HABILITADO.ToString())
            };

            foreach (var rol in listRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, rol.ID_ROL.ToString()));
                claims.Add(new Claim(ClaimTypes.Actor, rol.ROL_STR ?? ""));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(30) // o el tiempo que desees
                });

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpGet]
        public async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }
    }
}
