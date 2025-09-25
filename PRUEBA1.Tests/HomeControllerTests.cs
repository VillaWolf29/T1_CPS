using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using PRUEBA1.Controllers;
using PRUEBA1.Data;   // Ajusta si tu DbContext está en otro namespace
using PRUEBA1.Models; // Ajusta si tus modelos están en otro namespace
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace PRUEBA1.Tests
{
    [TestClass]
    public class HomeControllerTests
    {
        private HomeController _controller;
        private Mock<ILogger<HomeController>> _loggerMock;
        private AppDBContext _context;

        [TestInitialize]
        public void Setup()
        {
            // Logger simulado
            _loggerMock = new Mock<ILogger<HomeController>>();

            // Base de datos en memoria
            var options = new DbContextOptionsBuilder<AppDBContext>()
                .UseInMemoryDatabase(databaseName: "TestDB")
                .Options;

            _context = new AppDBContext(options);

            // Agregamos datos de prueba
            _context.productos.Add(new Producto { Id = 1, Nombre = "Producto1" });
            _context.Libros.Add(new Libro { Id = 1, Titulo = "Libro1" });
            _context.SaveChanges();

            // Instanciamos el controlador con mocks
            _controller = new HomeController(_loggerMock.Object, _context);
        }

        [TestMethod]
        public void Index_Returns_View_With_Data()
        {
            // Act
            var result = _controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result, "El resultado no debe ser nulo");
            Assert.IsTrue(result.ViewData.ContainsKey("Productos"), "Debe contener Productos en el ViewBag");
            Assert.IsTrue(result.ViewData.ContainsKey("Libros"), "Debe contener Libros en el ViewBag");

            var productos = result.ViewData["Productos"] as List<Producto>;
            var libros = result.ViewData["Libros"] as List<Libro>;

            Assert.AreEqual(1, productos.Count);
            Assert.AreEqual(1, libros.Count);
        }

        [TestMethod]
        public void Privacy_Returns_View()
        {
            var result = _controller.Privacy() as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Salir_Redirects_To_Login()
        {
            var result = _controller.Salir() as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Login", result.ActionName);
            Assert.AreEqual("Acceso", result.ControllerName);
        }

        [TestMethod]
        public void Rol_Redirects_To_Admin()
        {
            var result = _controller.Rol() as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Admin", result.ActionName);
            Assert.AreEqual("Administrador", result.ControllerName);
        }
    }
}
