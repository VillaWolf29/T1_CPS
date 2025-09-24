using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PRUEBA1.Data;
using PRUEBA1.Models;

namespace PRUEBA1.Controllers
{
    //Linea que protege a la Vista, comentar para realizar pruebas
    //[Authorize(Roles = "Administrador")]
    public class InventarioController : Controller
    {
        private readonly AppDBContext _dbContext;

        public InventarioController(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            var inventario = await _dbContext.inventario
                .Include(i => i.Producto)
                .Include(i => i.Libro)
                .Where(i => i.Stock >= 0)
                .ToListAsync();

            return View(inventario);
        }

        [HttpGet]
        public IActionResult Nuevo()
        {
            CargarListasDesplegables();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Nuevo(Inventario inventario)
        {
            if (ModelState.IsValid)
            {
                // Validar que no exista un registro duplicado
                var existe = await _dbContext.inventario
                    .AnyAsync(i => i.IdProducto == inventario.IdProducto && i.IdLibro == inventario.IdLibro);

                if (existe)
                {
                    ModelState.AddModelError("", "Ya existe un registro de inventario para este producto y libro.");
                    CargarListasDesplegables();
                    return View(inventario);
                }

                // Validar stock no negativo
                if (inventario.Stock < 0)
                {
                    ModelState.AddModelError("Stock", "El stock no puede ser negativo.");
                    CargarListasDesplegables();
                    return View(inventario);
                }

                inventario.FechaActualizacion = DateTime.Now;

                await _dbContext.inventario.AddAsync(inventario);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Lista));
            }

            CargarListasDesplegables();
            return View(inventario);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            Inventario inventario = await _dbContext.inventario
                .FirstAsync(i => i.IdInventario == id);

            CargarListasDesplegables();
            return View(inventario);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Inventario inventario)
        {
            if (ModelState.IsValid)
            {
                // Validar stock no negativo
                if (inventario.Stock < 0)
                {
                    ModelState.AddModelError("Stock", "El stock no puede ser negativo.");
                    CargarListasDesplegables();
                    return View(inventario);
                }

                inventario.FechaActualizacion = DateTime.Now;

                _dbContext.inventario.Update(inventario);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Lista));
            }

            CargarListasDesplegables();
            return View(inventario);
        }

        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {
            Inventario inventario = await _dbContext.inventario
                .FirstAsync(i => i.IdInventario == id);

            _dbContext.inventario.Remove(inventario);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Lista));
        }

        [HttpGet]
        public async Task<IActionResult> AjustarStock(int id)
        {
            Inventario inventario = await _dbContext.inventario
                .Include(i => i.Producto)
                .Include(i => i.Libro)
                .FirstAsync(i => i.IdInventario == id);

            return View(inventario);
        }

        [HttpPost]
        public async Task<IActionResult> AjustarStock(int id, int cantidad, string motivo)
        {
            Inventario inventario = await _dbContext.inventario
                .FirstAsync(i => i.IdInventario == id);

            // Validar que el ajuste no deje stock negativo
            if (inventario.Stock + cantidad < 0)
            {
                TempData["Error"] = "No se puede ajustar el stock a un valor negativo.";
                return RedirectToAction(nameof(Lista));
            }

            inventario.Stock += cantidad;
            inventario.FechaActualizacion = DateTime.Now;

            _dbContext.inventario.Update(inventario);
            await _dbContext.SaveChangesAsync();

            TempData["Success"] = $"Stock ajustado exitosamente. {Math.Abs(cantidad)} unidades {(cantidad >= 0 ? "agregadas" : "retiradas")}. Motivo: {motivo}";

            return RedirectToAction(nameof(Lista));
        }

        [HttpGet]
        public async Task<IActionResult> ReporteStockBajo()
        {
            var stockBajo = await _dbContext.inventario
                .Include(i => i.Producto)
                .Include(i => i.Libro)
                .Where(i => i.Stock <= 10) // Stock menor o igual a 10
                .OrderBy(i => i.Stock)
                .ToListAsync();

            return View(stockBajo);
        }

        // Método auxiliar para cargar las listas desplegables
        private void CargarListasDesplegables()
        {
            // Cargar productos activos
            var productos = _dbContext.productos
                .Where(p => p.Activo)
                .Select(p => new { p.IdProducto, p.Nombre })
                .ToList();

            ViewBag.Productos = new SelectList(productos, "IdProducto", "Nombre");

            // Cargar libros activos (asumiendo que tienes un modelo Libro con propiedad Activo)
            var libros = _dbContext.Libros
                .Select(l => new { l.IdLibro, l.Titulo })
                .ToList();

            ViewBag.Libros = new SelectList(libros, "IdLibro", "Titulo");
        }
    }
}