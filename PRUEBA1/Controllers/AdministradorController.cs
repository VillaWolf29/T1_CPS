using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PRUEBA1.Data;
using Microsoft.EntityFrameworkCore;
using PRUEBA1.Models;
using Microsoft.AspNetCore.Authorization;

namespace PRUEBA1.Controllers
{
    //Linea que protege a la Vista, comentar para realizar pruebas
    [Authorize(Roles= "1")]
    public class AdministradorController : Controller
    {
        private readonly AppDBContext _appDBcontext;
        public AdministradorController(AppDBContext appDBcontext)
        {
            _appDBcontext = appDBcontext;
        }

        public async Task<IActionResult> Admin() 
        {
            List<Usuario> usuarios = await _appDBcontext.Usuarios.Include(col => col.Rol).ToListAsync();
            return View(usuarios);
        }
        

    }
}
