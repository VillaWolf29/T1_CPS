using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRUEBA1.Controllers;
using PRUEBA1.Models;
using PRUEBA1.ViewModels;
using Xunit;

public class AccesoControllerTests
{
    [Fact]
    public async Task Registro_UsuarioNuevo_DeberiaRegistrarCorrectamente()
    {
        // Arrange
        var context = DbContextMock.GetInMemoryDBContext();
        var controller = new AccesoController(context);

        var nuevoUsuario = new UsuarioVM
        {
            Nombre = "Juan",
            Apellido = "Perez",
            Correo = "juan@test.com",
            Contraseña = "123456",
            ConfirmarContraseña = "123456"
        };

        // Act
        var resultado = await controller.Registro(nuevoUsuario);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(resultado);
        Assert.Equal("Login", redirect.ActionName); // se redirige al login
        Assert.Single(context.Usuarios); // la BD en memoria tiene 1 usuario
    }

    [Fact]
    public async Task Registro_ContraseñasNoCoinciden_DeberiaRetornarVista()
    {
        // Arrange
        var context = DbContextMock.GetInMemoryDBContext();
        var controller = new AccesoController(context);

        var usuario = new UsuarioVM
        {
            Nombre = "Ana",
            Apellido = "Lopez",
            Correo = "ana@test.com",
            Contraseña = "123456",
            ConfirmarContraseña = "654321"
        };

        // Act
        var resultado = await controller.Registro(usuario);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(resultado);
        Assert.Equal(usuario, viewResult.Model); // se devuelve la vista con el modelo
    }

    [Fact]
    public async Task Login_UsuarioNoExiste_DeberiaRetornarVistaConError()
    {
        // Arrange
        var context = DbContextMock.GetInMemoryDBContext();
        var controller = new AccesoController(context);

        var loginVM = new LoginVM
        {
            Correo = "noexiste@test.com",
            Contraseña = "123456"
        };

        // Act
        var resultado = await controller.Login(loginVM);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(resultado);
        Assert.Equal(loginVM, viewResult.Model);
        Assert.True(controller.ViewData["Mensaje"].ToString().Contains("incorrectos"));
    }
}
