using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PRUEBA1.Data;
using PRUEBA1.Models;
using PRUEBA1.ViewModels;

namespace PRUEBA1.Controllers
{
    //Linea que protege a la Vista, comentar para realizar pruebas
    //[Authorize(Roles = "Administrador")]
    [Authorize(Roles = "1")]
    public class LibroController : Controller
    {
        

        private readonly AppDBContext _appDBcontext;
        public LibroController(AppDBContext appDBcontext)
        {
            _appDBcontext = appDBcontext;
        }
        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            List<Libro> lista = await _appDBcontext.Libros.ToListAsync();
            return View(lista);
        }
        [HttpGet]
        public IActionResult Nuevo()
        {
            ViewBag.Autores = new SelectList(_appDBcontext.Autores, "IdAutor", "Nombre","Apellido","Nacionalidad");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Nuevo(Libro libro)
        {
            
            await _appDBcontext.Libros.AddAsync(libro);
            await _appDBcontext.SaveChangesAsync();
            return RedirectToAction(nameof(Lista));
        }
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            Libro libro = await _appDBcontext.Libros.FirstAsync(u => u.IdLibro == id);
            return View(libro);
        }
        [HttpPost]
        public async Task<IActionResult> Editar(Libro libro)
        {
            _appDBcontext.Libros.Update(libro);
            await _appDBcontext.SaveChangesAsync();
            return RedirectToAction(nameof(Lista));
        }
        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {
            Libro libro = await _appDBcontext.Libros.FirstAsync(e => e.IdLibro == id);
            _appDBcontext.Libros.Remove(libro);
            await _appDBcontext.SaveChangesAsync();
            return RedirectToAction(nameof(Lista));
        }


    }
}
