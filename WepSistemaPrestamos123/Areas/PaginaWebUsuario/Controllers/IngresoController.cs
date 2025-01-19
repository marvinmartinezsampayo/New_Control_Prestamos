using Microsoft.AspNetCore.Mvc;

namespace WepPrestamos.Areas.PaginaWebUsuario.Controllers
{
    public class IngresoController : Controller
    {
        [Area("PaginaWebUsuario")]
        public IActionResult Registro()
        {
            return View();
        }
    }
}
