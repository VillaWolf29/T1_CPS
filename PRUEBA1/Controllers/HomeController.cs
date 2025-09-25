using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRUEBA1.Data;
using PRUEBA1.Models;
using System.Diagnostics;
using System.Linq;

namespace PRUEBA1.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDBContext _context;

        public HomeController(ILogger<HomeController> logger, AppDBContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var productos = _context.productos.ToList();
            var libros = _context.Libros.ToList();

            ViewBag.Productos = productos;
            ViewBag.Libros = libros;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Salir()
        {
            return RedirectToAction("Login", "Acceso");
        }
        public IActionResult Rol()
        {
            return RedirectToAction("Admin", "Administrador");
        }

        //public IActionResult Lista()
        //{
        // return RedirectToAction("Lista", "Libro");
        //}


    }
}
