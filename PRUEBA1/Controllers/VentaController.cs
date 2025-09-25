using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PRUEBA1.Data;
using PRUEBA1.Models;
using Microsoft.EntityFrameworkCore;

namespace PRUEBA1.Controllers
{
    public class VentaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

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
                .Where(v => v.Estado == "Completada" || v.Estado == "Pendiente")
                .ToListAsync();

            return View(ventas);
        }

        [HttpGet]
        public IActionResult Nueva()
        {
            ViewBag.Usuarios = new SelectList(_dbContext.Usuarios.ToList(), "IdUsuario", "Nombre");
            ViewBag.MetodosPago = new SelectList(new List<string> { "Efectivo", "Tarjeta", "Transferencia", "PayPal" });
            return View();
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Nueva(Venta v)
        {
            if (ModelState.IsValid)
            {
                v.FechaVenta = DateTime.Now;
                v.Estado = "Completada"; // Se asigna el estado por defecto aquí
                await _dbContext.ventas.AddAsync(v);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Lista));
            }

            ViewBag.Usuarios = new SelectList(_dbContext.Usuarios.ToList(), "IdUsuario", "Nombre", v.IdUsuario);
            ViewBag.MetodosPago = new SelectList(new List<string> { "Efectivo", "Tarjeta", "Transferencia", "PayPal" }, v.MetodoPago);
            return View(v);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            Venta venta = await _dbContext.ventas.Include(v => v.Usuario).FirstAsync(v => v.IdVenta == id);

            ViewBag.Usuarios = new SelectList(_dbContext.Usuarios.ToList(), "IdUsuario", "Nombre", venta.IdUsuario);
            ViewBag.MetodosPago = new SelectList(new List<string> { "Efectivo", "Tarjeta", "Transferencia", "PayPal" }, venta.MetodoPago);

            return View(venta);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Venta v)
        {
            if (ModelState.IsValid)
            {
                _dbContext.ventas.Update(v);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Lista));
            }

            ViewBag.Usuarios = new SelectList(_dbContext.Usuarios.ToList(), "IdUsuario", "Nombre", v.IdUsuario);
            ViewBag.MetodosPago = new SelectList(new List<string> { "Efectivo", "Tarjeta", "Transferencia", "PayPal" }, v.MetodoPago);
            return View(v);
        }

        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {
            Venta v = await _dbContext.ventas.FirstAsync(o => o.IdVenta == id);

            v.Estado = "Cancelada";
            _dbContext.ventas.Update(v);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Lista));
        }

        [HttpGet]
        public async Task<IActionResult> Activar(int id)
        {
            Venta venta = await _dbContext.ventas.FirstAsync(v => v.IdVenta == id);
            venta.Estado = "Completada";
            _dbContext.ventas.Update(venta);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Lista));
        }
    }
}
