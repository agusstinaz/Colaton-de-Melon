using ColatonDeMelon.Data;
using ColatonDeMelon.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace ColatonDeMelon.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly ColatonDeMelonContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CheckoutController(
            ColatonDeMelonContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var carritoJson =
                HttpContext.Session.GetString("Carrito");

            if (string.IsNullOrEmpty(carritoJson))
            {
                return RedirectToAction("Index", "Carrito");
            }

            var carrito =
                JsonSerializer.Deserialize<List<CarritoItem>>(
                    carritoJson);

            if (carrito == null || carrito.Count == 0)
            {
                return RedirectToAction("Index", "Carrito");
            }

            return View(carrito);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirmar(
            string nombre,
            string apellido,
            string telefono,
            string direccion,
            string localidad,
            string codigoPostal,
            string observaciones)
        {
            if (string.IsNullOrWhiteSpace(nombre) ||
                string.IsNullOrWhiteSpace(apellido) ||
                string.IsNullOrWhiteSpace(telefono) ||
                string.IsNullOrWhiteSpace(direccion) ||
                string.IsNullOrWhiteSpace(localidad) ||
                string.IsNullOrWhiteSpace(codigoPostal))
            {
                TempData["MensajeCheckout"] =
                    "Por favor, completá todos los datos obligatorios.";

                return RedirectToAction(nameof(Index));
            }

            var carritoJson =
                HttpContext.Session.GetString("Carrito");

            if (string.IsNullOrEmpty(carritoJson))
            {
                return RedirectToAction("Index", "Carrito");
            }

            var carrito =
                JsonSerializer.Deserialize<List<CarritoItem>>(
                    carritoJson);

            if (carrito == null || carrito.Count == 0)
            {
                return RedirectToAction("Index", "Carrito");
            }

            var usuario =
                await _userManager.GetUserAsync(User);

            if (usuario == null)
            {
                return RedirectToAction(
                    "Login",
                    "Cuenta",
                    new { returnUrl = "/Checkout" });
            }

            foreach (var item in carrito)
            {
                var producto = await _context.Productos
                    .FirstOrDefaultAsync(
                        p => p.IdProducto == item.IdProducto);

                if (producto == null ||
                    producto.Stock < item.Cantidad)
                {
                    TempData["MensajeCheckout"] =
                        $"No hay stock suficiente para {item.Nombre}.";

                    return RedirectToAction(nameof(Index));
                }
            }

            decimal total = carrito.Sum(
                item => item.Subtotal);

            var pedido = new Pedido
            {
                UsuarioId = usuario.Id,
                Nombre = nombre,
                Apellido = apellido,
                Telefono = telefono,
                Direccion = direccion,
                Localidad = localidad,
                CodigoPostal = codigoPostal,
                Observaciones = observaciones ?? "",
                Total = total,
                FechaPedido = DateTime.Now,
                Estado = "Pendiente"
            };

            _context.Pedidos.Add(pedido);

            await _context.SaveChangesAsync();

            foreach (var item in carrito)
            {
                var detalle = new PedidoDetalle
                {
                    IdPedido = pedido.IdPedido,
                    IdProducto = item.IdProducto,
                    NombreProducto = item.Nombre,
                    Precio = item.Precio,
                    Cantidad = item.Cantidad,
                    Subtotal = item.Subtotal
                };

                _context.PedidoDetalles.Add(detalle);

                var producto =
                    await _context.Productos
                        .FirstAsync(
                            p => p.IdProducto == item.IdProducto);

                producto.Stock -= item.Cantidad;
            }

            await _context.SaveChangesAsync();

            HttpContext.Session.Remove("Carrito");

            return RedirectToAction(
                nameof(Confirmado),
                new { id = pedido.IdPedido });
        }

        [HttpGet]
        public async Task<IActionResult> Confirmado(int id)
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

            // Número de WhatsApp de Colatón de Melón
            const string numeroWhatsApp = "549370479224";

            // Mensaje que aparecerá preparado en WhatsApp
            var mensajeWhatsApp =
                $"Hola, quiero consultar los medios de pago para mi pedido " +
                $"#{pedido.IdPedido} de Colatón de Melón. " +
                $"Mi nombre es {pedido.Nombre} {pedido.Apellido} " +
                $"y el total del pedido es ${pedido.Total:N0}.";

            ViewBag.WhatsAppUrl =
                $"https://wa.me/{numeroWhatsApp}?text=" +
                Uri.EscapeDataString(mensajeWhatsApp);

            return View(pedido);
        }
    }
}