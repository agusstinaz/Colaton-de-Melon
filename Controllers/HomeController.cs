using ColatonDeMelon.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ColatonDeMelon.Controllers
{
    public class HomeController : Controller
    {
        private readonly ColatonDeMelonContext _context;

        public HomeController(
            ColatonDeMelonContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            var productos = await _context.Productos
                .Include(p => p.Edad)
                .Include(p => p.TipoPrenda)
                .OrderByDescending(p => p.IdProducto)
                .Take(8)
                .ToListAsync();

            return View(productos);
        }


        public IActionResult Privacy()
        {
            return View();
        }
    }
}