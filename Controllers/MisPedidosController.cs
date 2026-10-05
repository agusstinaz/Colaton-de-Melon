using ColatonDeMelon.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ColatonDeMelon.Controllers
{
    [Authorize]
    public class MisPedidosController : Controller
    {
        private readonly ColatonDeMelonContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public MisPedidosController(
            ColatonDeMelonContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var usuario =
                await _userManager.GetUserAsync(User);

            if (usuario == null)
            {
                return RedirectToAction(
                    "Login",
                    "Cuenta");
            }

            var pedidos = await _context.Pedidos
                .Where(p => p.UsuarioId == usuario.Id)
                .OrderByDescending(p => p.FechaPedido)
                .ToListAsync();

            return View(pedidos);
        }

        public async Task<IActionResult> Details(int id)
        {
            var usuario =
                await _userManager.GetUserAsync(User);

            if (usuario == null)
            {
                return RedirectToAction(
                    "Login",
                    "Cuenta");
            }

            var pedido = await _context.Pedidos
                .Include(p => p.Detalles)
                .FirstOrDefaultAsync(
                    p => p.IdPedido == id &&
                         p.UsuarioId == usuario.Id);

            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }
    }
}