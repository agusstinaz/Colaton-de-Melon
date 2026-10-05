using ColatonDeMelon.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ColatonDeMelon.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminPedidosController : Controller
    {
        private readonly ColatonDeMelonContext _context;

        public AdminPedidosController(
            ColatonDeMelonContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var pedidos = await _context.Pedidos
                .Include(p => p.Detalles)
                .OrderByDescending(p => p.FechaPedido)
                .ToListAsync();

            ViewBag.Pendientes = pedidos.Count(p => p.Estado == "Pendiente");

            ViewBag.PagosConfirmados =
                pedidos.Count(p => p.Estado == "Pago confirmado");

            ViewBag.Preparando =
                pedidos.Count(p => p.Estado == "Preparando");

            ViewBag.Enviados =
                pedidos.Count(p => p.Estado == "Enviado");

            ViewBag.Entregados =
                pedidos.Count(p => p.Estado == "Entregado");

            ViewBag.Cancelados =
                pedidos.Count(p => p.Estado == "Cancelado");

            return View(pedidos);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Detalles)
                .FirstOrDefaultAsync(p => p.IdPedido == id);

            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(
    int id,
    string estado)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Detalles)
                .FirstOrDefaultAsync(p => p.IdPedido == id);

            if (pedido == null)
            {
                return NotFound();
            }

            var estadosPermitidos = new[]
            {
        "Pendiente",
        "Pago confirmado",
        "Preparando",
        "Enviado",
        "Entregado",
        "Cancelado"
    };

            if (!estadosPermitidos.Contains(estado))
            {
                return RedirectToAction(
                    nameof(Details),
                    new { id });
            }

            string estadoAnterior = pedido.Estado;

            // Si el pedido se cancela, devolvemos el stock
            if (estadoAnterior != "Cancelado" &&
                estado == "Cancelado")
            {
                foreach (var detalle in pedido.Detalles)
                {
                    var producto = await _context.Productos
                        .FirstOrDefaultAsync(
                            p => p.IdProducto == detalle.IdProducto);

                    if (producto != null)
                    {
                        producto.Stock += detalle.Cantidad;
                    }
                }
            }

            // Si un pedido cancelado vuelve a estar activo,
            // volvemos a descontar el stock.
            if (estadoAnterior == "Cancelado" &&
                estado != "Cancelado")
            {
                foreach (var detalle in pedido.Detalles)
                {
                    var producto = await _context.Productos
                        .FirstOrDefaultAsync(
                            p => p.IdProducto == detalle.IdProducto);

                    if (producto == null)
                    {
                        TempData["MensajePedido"] =
                            $"No se encontró el producto {detalle.NombreProducto}.";

                        return RedirectToAction(
                            nameof(Details),
                            new { id });
                    }

                    if (producto.Stock < detalle.Cantidad)
                    {
                        TempData["MensajePedido"] =
                            $"No hay stock suficiente para volver a activar el pedido. Producto: {detalle.NombreProducto}.";

                        return RedirectToAction(
                            nameof(Details),
                            new { id });
                    }
                }

                foreach (var detalle in pedido.Detalles)
                {
                    var producto = await _context.Productos
                        .FirstOrDefaultAsync(
                            p => p.IdProducto == detalle.IdProducto);

                    if (producto != null)
                    {
                        producto.Stock -= detalle.Cantidad;
                    }
                }
            }

            pedido.Estado = estado;

            await _context.SaveChangesAsync();

            return RedirectToAction(
                nameof(Details),
                new { id });
        }
    }
}