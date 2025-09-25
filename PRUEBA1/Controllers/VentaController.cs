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
    public class VentaController : Controller
    {
        private readonly AppDBContext _dbContext;

        public VentaController(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            var ventas = await _dbContext.ventas
                .Include(v => v.Usuario)
                .Include(v => v.Detalles)
                .ThenInclude(d => d.Producto)
                .OrderByDescending(v => v.FechaVenta)
                .ToListAsync();

            return View(ventas);
        }

        [HttpGet]
        public IActionResult Nuevo()
        {
            // Cargar datos para los dropdowns
            var usuarios = _dbContext.Usuarios.ToList();
            var productos = _dbContext.productos.Where(p => p.Activo).ToList();
            var metodosPago = new List<string> { "Efectivo", "Tarjeta Crédito", "Tarjeta Débito", "Transferencia" };
            var estados = new List<string> { "Pendiente", "Completada", "Cancelada" };

            ViewBag.Usuarios = new SelectList(usuarios, "IdUsuario", "Nombre");
            ViewBag.Productos = new SelectList(productos, "IdProducto", "Nombre");
            ViewBag.MetodosPago = new SelectList(metodosPago);
            ViewBag.Estados = new SelectList(estados);

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Nuevo(Venta venta, List<DetalleVenta> detalles)
        {
            ModelState.Remove("Usuario");
            ModelState.Remove("Libro");
            ModelState.Remove("Producto");
            ModelState.Remove("Detalle");
            if (ModelState.IsValid)
            {
                try
                {
                    // Validar que el usuario existe
                    var usuario = await _dbContext.Usuarios.FindAsync(venta.IdUsuario);
                    if (usuario == null)
                    {
                        ModelState.AddModelError("IdUsuario", "El usuario seleccionado no existe");
                        return View(venta);
                    }

                    // Validar detalles
                    if (detalles == null || !detalles.Any())
                    {
                        ModelState.AddModelError("", "La venta debe tener al menos un producto");
                        return View(venta);
                    }

                    // Calcular total si no viene especificado
                    if (venta.Total == 0)
                    {
                        venta.Total = detalles.Sum(d => d.Cantidad * d.PrecioUnitario);
                    }

                    // Asignar fecha actual si no viene
                    if (venta.FechaVenta == DateTime.MinValue)
                    {
                        venta.FechaVenta = DateTime.Now;
                    }

                    // Guardar venta
                    await _dbContext.ventas.AddAsync(venta);
                    await _dbContext.SaveChangesAsync();

                    // Asignar IdVenta a los detalles y guardarlos
                    foreach (var detalle in detalles)
                    {
                        detalle.IdVenta = venta.IdVenta;
                        await _dbContext.detalle.AddAsync(detalle);
                    }

                    await _dbContext.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Venta registrada exitosamente";
                    return RedirectToAction(nameof(Lista));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al registrar la venta: " + ex.Message);
                }
            }

            // Si hay error, recargar los dropdowns
            await CargarViewBags();
            return View(venta);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var venta = await _dbContext.ventas
                .Include(v => v.Detalles)
                .FirstOrDefaultAsync(v => v.IdVenta == id);

            if (venta == null)
            {
                return NotFound();
            }

            await CargarViewBags();
            return View(venta);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Venta venta)
        {
            ModelState.Remove("Usuario");
            ModelState.Remove("Libro");
            ModelState.Remove("Producto");
            ModelState.Remove("Detalle");
            if (ModelState.IsValid)
            {
                try
                {
                    _dbContext.ventas.Update(venta);
                    await _dbContext.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Venta actualizada exitosamente";
                    return RedirectToAction(nameof(Lista));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al actualizar la venta: " + ex.Message);
                }
            }

            await CargarViewBags();
            return View(venta);
        }

        [HttpGet]
        public async Task<IActionResult> Detalles(int id)
        {
            var venta = await _dbContext.ventas
                .Include(v => v.Usuario)
                .Include(v => v.Detalles)
                .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(v => v.IdVenta == id);

            if (venta == null)
            {
                return NotFound();
            }

            return View(venta);
        }

        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var venta = await _dbContext.ventas
                    .Include(v => v.Detalles)
                    .FirstOrDefaultAsync(v => v.IdVenta == id);

                if (venta == null)
                {
                    return NotFound();
                }

                // Eliminar primero los detalles
                _dbContext.detalle.RemoveRange(venta.Detalles);
                _dbContext.ventas.Remove(venta);
                await _dbContext.SaveChangesAsync();

                TempData["SuccessMessage"] = "Venta eliminada exitosamente";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al eliminar la venta: " + ex.Message;
            }

            return RedirectToAction(nameof(Lista));
        }

        [HttpGet]
        public async Task<IActionResult> Cancelar(int id)
        {
            try
            {
                var venta = await _dbContext.ventas.FindAsync(id);
                if (venta == null)
                {
                    return NotFound();
                }

                venta.Estado = "Cancelada";
                _dbContext.ventas.Update(venta);
                await _dbContext.SaveChangesAsync();

                TempData["SuccessMessage"] = "Venta cancelada exitosamente";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al cancelar la venta: " + ex.Message;
            }

            return RedirectToAction(nameof(Lista));
        }

        [HttpGet]
        public async Task<IActionResult> Completar(int id)
        {
            try
            {
                var venta = await _dbContext.ventas.FindAsync(id);
                if (venta == null)
                {
                    return NotFound();
                }

                venta.Estado = "Completada";
                _dbContext.ventas.Update(venta);
                await _dbContext.SaveChangesAsync();

                TempData["SuccessMessage"] = "Venta completada exitosamente";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al completar la venta: " + ex.Message;
            }

            return RedirectToAction(nameof(Lista));
        }

        [HttpGet]
        public async Task<JsonResult> ObtenerProducto(int id)
        {
            var producto = await _dbContext.productos.FindAsync(id);
            if (producto == null)
            {
                return Json(new { success = false });
            }

            return Json(new
            {
                success = true,
                nombre = producto.Nombre,
                precio = producto.Precio,
                stock = producto.Stock
            });
        }

        private async Task CargarViewBags()
        {
            var usuarios = await _dbContext.Usuarios.ToListAsync();
            var productos = await _dbContext.productos.Where(p => p.Activo).ToListAsync();
            var metodosPago = new List<string> { "Efectivo", "Tarjeta Crédito", "Tarjeta Débito", "Transferencia" };
            var estados = new List<string> { "Pendiente", "Completada", "Cancelada" };

            ViewBag.Usuarios = new SelectList(usuarios, "IdUsuario", "Nombre");
            ViewBag.Productos = new SelectList(productos, "IdProducto", "Nombre");
            ViewBag.MetodosPago = new SelectList(metodosPago);
            ViewBag.Estados = new SelectList(estados);
        }
    }
}