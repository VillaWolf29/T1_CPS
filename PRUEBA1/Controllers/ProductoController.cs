using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PRUEBA1.Data;
using PRUEBA1.Models;

namespace PRUEBA1.Controllers
{
    //Linea que protege a la Vista, comentar para realizar pruebas
    [Authorize(Roles = "1")]

    public class ProductoController : Controller
    {
        private readonly AppDBContext _dbContext;

        public ProductoController(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            var productos = _dbContext.productos
                .Where(p => p.Activo)
                .ToList();

            return View(productos);
        }

        [HttpGet]
        public IActionResult Nuevo()
        {
            var categorias = new List<string> { "Papeleria", "Libreria", "Libros" };
            ViewBag.Categorias = new SelectList(categorias);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Nuevo(Producto p)
        {
            if (ModelState.IsValid)
            {
                p.Activo = true;
                await _dbContext.productos.AddAsync(p);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Lista));
            }

            var categorias = new List<string> { "Papeleria", "Libreria", "Libros"};
            ViewBag.Categorias = new SelectList(categorias);
            return View(p);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            Producto producto = await _dbContext.productos.FirstAsync(p => p.IdProducto == id);

            var categorias = new List<string> { "Papeleria", "Libreria", "Libros" };

            ViewBag.Categorias = new SelectList(categorias, producto.Categoria);

            return View(producto);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Producto p)
        {
            if (ModelState.IsValid)
            {
                _dbContext.productos.Update(p);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Lista));
            }

            var categorias = new List<string> { "Papeleria", "Libreria", "Libros" };
            ViewBag.Categorias = new SelectList(categorias, p.Categoria);
            return View(p);
        }

        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {
            Producto p = await _dbContext.productos.FirstAsync(o => o.IdProducto == id);

            p.Activo = false;
            _dbContext.productos.Update(p);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Lista));
        }

        [HttpGet]
        public async Task<IActionResult> Activar(int id)
        {
            Producto producto = await _dbContext.productos.FirstAsync(p => p.IdProducto == id);
            producto.Activo = true;
            _dbContext.productos.Update(producto);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Lista));
        }

    }
}
