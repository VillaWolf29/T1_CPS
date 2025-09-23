using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PRUEBA1.Data;
using PRUEBA1.Models;
using PRUEBA1.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace PRUEBA1.Controllers
{
    public class AccesoController : Controller
    {
        private readonly AppDBContext _appDBcontext;
        public AccesoController(AppDBContext appDBcontext)
        {
            _appDBcontext = appDBcontext;
        }
        [HttpGet]
        public ActionResult Registro()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Registro(UsuarioVM usuariovm)
        {
            if (usuariovm.Contraseña != usuariovm.ConfirmarContraseña)
            {
                ViewData["Mensaje"] = "Las contraseñas no coinciden";
                return View();
            }

            Usuario usuario = new Usuario()
            {
                Nombre = usuariovm.Nombre,
                Apellido = usuariovm.Apellido,
                Correo = usuariovm.Correo,
                Contraseña = usuariovm.Contraseña,
                IdRol = usuariovm.IdRol,

            };
            await _appDBcontext.Usuarios.AddAsync(usuario);
            await _appDBcontext.SaveChangesAsync();
            if (usuario.IdUsuario != 0)
            return RedirectToAction("Login", "Acceso");
            ViewData["Mensaje"] = "El usuario no se a creado, verifique que los campos esten llenos";
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM Log)
        {
            Usuario? usuarioEncontrado = await _appDBcontext.Usuarios.Where(u => u.Correo == Log.Correo && u.Contraseña == Log.Contraseña).FirstOrDefaultAsync();
            if (usuarioEncontrado == null)
            {
                ViewData["Mensaje"] = "Usuario no encontrado";
                return View();
            }
            if (usuarioEncontrado.IdRol == 1) 
            {
                return RedirectToAction("Admin", "Administrador");
            }
            return RedirectToAction("Lista", "Libro");

        }
    }
}
