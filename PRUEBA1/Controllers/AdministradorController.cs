using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PRUEBA1.Data;
using Microsoft.EntityFrameworkCore;
using PRUEBA1.Models;

namespace PRUEBA1.Controllers
{
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
