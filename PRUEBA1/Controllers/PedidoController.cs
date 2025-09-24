using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PRUEBA1.Data;
using PRUEBA1.Models;

namespace PRUEBA1.Controllers
{
    public class PedidoController : Controller
    {
        private readonly AppDBContext _dbContext;

        public PedidoController(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        // LISTA
        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            var pedidos = await _dbContext.pedidos
                .Include(p => p.Usuario)
                .Include(p => p.Libro)
                .Include(p => p.Producto)
                .ToListAsync();

            return View(pedidos);
        }

        // NUEVO (GET)
        [HttpGet]
        public IActionResult Nuevo()
        {
            CargarSelectListas();
            return View();
        }

        // NUEVO (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Nuevo(Pedido pedido)
        {
            if (ModelState.IsValid)
            {
                // valores por defecto
                pedido.FechaReserva = DateTime.Now;
                pedido.Estado = "Pendiente";

                await _dbContext.pedidos.AddAsync(pedido);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Lista));
            }

            CargarSelectListas();
            return View(pedido);
        }

        // EDITAR (GET)
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var pedido = await _dbContext.pedidos.FindAsync(id);
            if (pedido == null) return NotFound();

            CargarSelectListas();
            return View(pedido);
        }

        // EDITAR (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Pedido pedido)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _dbContext.pedidos.Update(pedido);
                    await _dbContext.SaveChangesAsync();
                    return RedirectToAction(nameof(Lista));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_dbContext.pedidos.Any(p => p.IdReserva == pedido.IdReserva))
                        return NotFound();
                    throw;
                }
            }

            CargarSelectListas();
            return View(pedido);
        }

        // ELIMINAR (GET CONFIRMACION)
        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {
            var pedido = await _dbContext.pedidos
                .Include(p => p.Usuario)
                .Include(p => p.Libro)
                .Include(p => p.Producto)
                .FirstOrDefaultAsync(p => p.IdReserva == id);

            if (pedido == null) return NotFound();
            return View(pedido);
        }

        // ELIMINAR (POST)
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            var pedido = await _dbContext.pedidos.FindAsync(id);
            if (pedido == null) return NotFound();

            _dbContext.pedidos.Remove(pedido);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Lista));
        }

        // Método privado para cargar SelectLists
        private void CargarSelectListas()
        {
            ViewBag.Usuarios = new SelectList(_dbContext.Usuarios, "IdUsuario", "Nombre");
            ViewBag.Libros = new SelectList(_dbContext.Libros, "IdLibro", "Titulo");
            ViewBag.Productos = new SelectList(_dbContext.pedidos, "IdProducto", "Nombre");
        }
    }
}
