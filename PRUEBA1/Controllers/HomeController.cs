using Microsoft.AspNetCore.Mvc;
using PRUEBA1.Models;
using System.Diagnostics;

namespace PRUEBA1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
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
        //    return RedirectToAction("Lista", "Libro");
        //}
    }
}
