using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRUEBA1.Controllers;
using PRUEBA1.Models; // Ajusta según dónde tengas Inventario, Producto, Libro
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace PRUEBA1.Tests
{
    [TestClass]
    public class InventarioControllerTests
    {
        private AppDBContext _context;
        private InventarioController _controller;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDBContext>()
                .UseInMemoryDatabase(databaseName: "InventarioTestDB")
                .Options;

            _context = new AppDBContext(options);

            // Insertar datos iniciales
            _context.productos.Add(new Producto { IdProducto = 1, Nombre = "Producto Test", Activo = true });
            _context.Libros.Add(new Libro { IdLibro = 1, Titulo = "Libro Test" });
            _context.inventario.Add(new Inventario { IdInventario = 1, IdProducto = 1, IdLibro = 1, Stock = 5 });
            _context.SaveChanges();

            _controller = new InventarioController(_context);
        }

        [TestMethod]
        public async Task Lista_Returns_View_With_Inventario()
        {
            // Act
            var result = await _controller.Lista();

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            var model = viewResult.Model as List<Inventario>;
            Assert.AreEqual(1, model.Count);
        }

        [TestMethod]
        public async Task Nuevo_Post_Creates_New_Inventario()
        {
            var inventario = new Inventario
            {
                IdProducto = 1,
                IdLibro = 1,
                Stock = 10
            };

            var result = await _controller.Nuevo(inventario);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            Assert.AreEqual(2, _context.inventario.Count());
        }

        [TestMethod]
        public async Task Editar_Post_Updates_Inventario()
        {
            var inventario = _context.inventario.First();
            inventario.Stock = 20;

            var result = await _controller.Editar(inventario);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            Assert.AreEqual(20, _context.inventario.First().Stock);
        }

        [TestMethod]
        public async Task Eliminar_Removes_Inventario()
        {
            var result = await _controller.Eliminar(1);

            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            Assert.AreEqual(0, _context.inventario.Count());
        }

        [TestMethod]
        public async Task AjustarStock_Negative_NotAllowed()
        {
            var result = await _controller.AjustarStock(1, -10, "Prueba");

            var redirect = result as RedirectToActionResult;
            Assert.AreEqual("Lista", redirect.ActionName);

            var inv = _context.inventario.First();
            Assert.AreEqual(5, inv.Stock); // no debe cambiar
        }
    }
}
