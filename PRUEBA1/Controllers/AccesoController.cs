using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRUEBA1.Data;
using PRUEBA1.Models;
using PRUEBA1.ViewModels;
using System.Security.Claims;

namespace PRUEBA1.Controllers
{

    public class AccesoController : Controller
    {
        private readonly AppDBContext _appDBcontext;

        public AccesoController(AppDBContext appDBcontext)
        {
            _appDBcontext = appDBcontext;
            InitialzeUsuario().Wait();
        }

        //Unico usuario Administrador
        private async Task InitialzeUsuario()
        {
            if (!await _appDBcontext.Usuarios.AnyAsync())
            {
                var usuario = new List<Usuario>
                {
                    new Usuario { Nombre= "Abed",Apellido = "Llovera", Correo="master@gmail.com", Contraseña="$2a$11$ftbwkevIm.kVrZCJtWwVFOnz8EICF6isvJeKp0ZIC.dCNM0t6ZHti", IdRol=1}
                };
                await _appDBcontext.Usuarios.AddRangeAsync(usuario);
                await _appDBcontext.SaveChangesAsync();
            }
        }


        [HttpGet]
        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registro(UsuarioVM usuariovm)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Mensaje"] = "Por favor complete todos los campos correctamente";
                return View(usuariovm);
            }

            if (usuariovm.Contraseña != usuariovm.ConfirmarContraseña)
            {
                ViewData["Mensaje"] = "Las contraseñas no coinciden";
                return View(usuariovm);
            }

            if (await _appDBcontext.Usuarios.AnyAsync(u => u.Correo == usuariovm.Correo))
            {
                ViewData["Mensaje"] = "Este correo electrónico ya está registrado";
                return View(usuariovm);
            }

            usuariovm.IdRol = 2;

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(usuariovm.Contraseña);

            Usuario usuario = new Usuario()
            {
                Nombre = usuariovm.Nombre,
                Apellido = usuariovm.Apellido,
                Correo = usuariovm.Correo,
                Contraseña = hashedPassword,
                IdRol = usuariovm.IdRol,
            };

            await _appDBcontext.Usuarios.AddAsync(usuario);
            await _appDBcontext.SaveChangesAsync();

            if (usuario.IdUsuario != 0)
            {
                TempData["SuccessMessage"] = "Registro exitoso. Por favor inicie sesión.";
                return RedirectToAction("Login", "Acceso");
            }

            ViewData["Mensaje"] = "El usuario no se ha creado, verifique que los campos estén llenos";
            return View(usuariovm);
        }

        [HttpGet]
        public IActionResult Login()
        {
            //if (User.Identity.IsAuthenticated)
            //{
              //  HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            //}
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM Log)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Mensaje"] = "Por favor complete todos los campos correctamente";
                return View(Log);
            }

            Usuario? usuarioEncontrado = await _appDBcontext.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Correo == Log.Correo);

            if (usuarioEncontrado == null ||
                !BCrypt.Net.BCrypt.Verify(Log.Contraseña, usuarioEncontrado.Contraseña))
            {
                ViewData["Mensaje"] = "Usuario o Contraseña incorrectos";
                return View(Log);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuarioEncontrado.IdUsuario.ToString()),
                new Claim(ClaimTypes.Email, usuarioEncontrado.Correo),
                new Claim(ClaimTypes.Name, usuarioEncontrado.Nombre + " " + usuarioEncontrado.Apellido),
                new Claim(ClaimTypes.Role, usuarioEncontrado.IdRol.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = Log.Recordarme,
                AllowRefresh = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);


            if (usuarioEncontrado.IdRol == 1)
            {
                return RedirectToAction("Lista", "Producto");
            }

            return RedirectToAction("Lista", "Libro");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}