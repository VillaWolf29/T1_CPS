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
            Console.WriteLine($"=== DATOS RECIBIDOS ===");
            Console.WriteLine($"IdUsuario: {pedido.IdUsuario}");
            Console.WriteLine($"IdLibro: {pedido.IdLibro}");
            Console.WriteLine($"IdProducto: {pedido.IdProducto}");
            Console.WriteLine($"Cantidad: {pedido.Cantidad}");
            Console.WriteLine($"FechaEntrega: {pedido.FechaEntrega}");

            // Convertir 0 a null para campos nullable
            if (pedido.IdLibro == 0) pedido.IdLibro = null;
            if (pedido.IdProducto == 0) pedido.IdProducto = null;

            // Validación personalizada
            if (pedido.IdLibro == null && pedido.IdProducto == null)
            {
                ModelState.AddModelError("", "Debe seleccionar al menos un libro o un producto");
            }
            ModelState.Remove("Usuario");
            ModelState.Remove("Libro");
            ModelState.Remove("Producto");

            // Verificar estado del ModelState
            Console.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("=== ERRORES DE VALIDACIÓN ===");
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"{state.Key}: {error.ErrorMessage}");
                    }
                }

                CargarSelectListas();
                return View(pedido);
            }

            try
            {
                Console.WriteLine("=== INTENTANDO GUARDAR ===");

                // Verificar que existan las entidades relacionadas
                var usuarioExists = await _dbContext.Usuarios.AnyAsync(u => u.IdUsuario == pedido.IdUsuario);
                if (!usuarioExists)
                {
                    ModelState.AddModelError("IdUsuario", "El usuario seleccionado no existe");
                    CargarSelectListas();
                    return View(pedido);
                }

                if (pedido.IdLibro.HasValue)
                {
                    var libroExists = await _dbContext.Libros.AnyAsync(l => l.IdLibro == pedido.IdLibro.Value);
                    if (!libroExists)
                    {
                        ModelState.AddModelError("IdLibro", "El libro seleccionado no existe");
                        CargarSelectListas();
                        return View(pedido);
                    }
                }

                if (pedido.IdProducto.HasValue)
                {
                    var productoExists = await _dbContext.productos.AnyAsync(p => p.IdProducto == pedido.IdProducto.Value);
                    if (!productoExists)
                    {
                        ModelState.AddModelError("IdProducto", "El producto seleccionado no existe");
                        CargarSelectListas();
                        return View(pedido);
                    }
                }

                // Asegurar valores por defecto
                pedido.FechaReserva = DateTime.Now;
                pedido.Estado = "Pendiente";

                await _dbContext.pedidos.AddAsync(pedido);
                await _dbContext.SaveChangesAsync();

                Console.WriteLine("=== PEDIDO GUARDADO EXITOSAMENTE ===");

                TempData["success"] = "Pedido creado exitosamente";
                return RedirectToAction(nameof(Lista));
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"=== ERROR DE BASE DE DATOS ===");
                Console.WriteLine($"Mensaje: {dbEx.Message}");
                Console.WriteLine($"Inner Exception: {dbEx.InnerException?.Message}");

                ModelState.AddModelError("", "Error al guardar en la base de datos. Verifique que los datos sean válidos.");
                CargarSelectListas();
                return View(pedido);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== ERROR GENERAL ===");
                Console.WriteLine($"Mensaje: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");

                ModelState.AddModelError("", "Error inesperado al guardar el pedido.");
                CargarSelectListas();
                return View(pedido);
            }
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
            ModelState.Remove("Usuario");
            ModelState.Remove("Libro");
            ModelState.Remove("Producto");
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
            ViewBag.Productos = new SelectList(_dbContext.productos, "IdProducto", "Nombre");
        }
    }
}
