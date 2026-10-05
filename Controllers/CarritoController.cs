using ColatonDeMelon.Data;
using ColatonDeMelon.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace ColatonDeMelon.Controllers
{
    public class CarritoController : Controller
    {
        private readonly ColatonDeMelonContext _context;

        public CarritoController(ColatonDeMelonContext context)
        {
            _context = context;
        }

        // ==========================================
        // VER CARRITO
        // ==========================================

        public IActionResult Index()
        {
            var carrito = ObtenerCarrito();

            return View(carrito);
        }

        // ==========================================
        // AGREGAR PRODUCTO
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Agregar(int id)
        {
            var producto = await _context.Productos
                .FirstOrDefaultAsync(p => p.IdProducto == id);

            if (producto == null)
            {
                return NotFound();
            }

            if (producto.Stock <= 0)
            {
                TempData["MensajeCarrito"] =
                    "Este producto no tiene stock disponible.";

                return RedirectToAction(
                    "Details",
                    "Productos",
                    new { id = id });
            }

            var carrito = ObtenerCarrito();

            var item = carrito.FirstOrDefault(
                x => x.IdProducto == id);

            if (item != null)
            {
                if (item.Cantidad < producto.Stock)
                {
                    item.Cantidad++;
                }
            }
            else
            {
                carrito.Add(new CarritoItem
                {
                    IdProducto = producto.IdProducto,
                    Nombre = producto.Nombre!,
                    Precio = producto.Precio,
                    Imagen = producto.Imagen!,
                    Cantidad = 1
                });
            }

            GuardarCarrito(carrito);

            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // AUMENTAR CANTIDAD
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Aumentar(int id)
        {
            var producto = await _context.Productos
                .FirstOrDefaultAsync(p => p.IdProducto == id);

            if (producto == null)
            {
                return NotFound();
            }

            var carrito = ObtenerCarrito();

            var item = carrito.FirstOrDefault(
                x => x.IdProducto == id);

            if (item != null &&
                item.Cantidad < producto.Stock)
            {
                item.Cantidad++;
            }

            GuardarCarrito(carrito);

            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // DISMINUIR CANTIDAD
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Disminuir(int id)
        {
            var carrito = ObtenerCarrito();

            var item = carrito.FirstOrDefault(
                x => x.IdProducto == id);

            if (item != null)
            {
                item.Cantidad--;

                if (item.Cantidad <= 0)
                {
                    carrito.Remove(item);
                }
            }

            GuardarCarrito(carrito);

            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // ELIMINAR PRODUCTO
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(int id)
        {
            var carrito = ObtenerCarrito();

            var item = carrito.FirstOrDefault(
                x => x.IdProducto == id);

            if (item != null)
            {
                carrito.Remove(item);
            }

            GuardarCarrito(carrito);

            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // VACIAR CARRITO
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Vaciar()
        {
            GuardarCarrito(new List<CarritoItem>());

            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // OBTENER CARRITO DE SESIÓN
        // ==========================================

        private List<CarritoItem> ObtenerCarrito()
        {
            var carritoJson =
                HttpContext.Session.GetString("Carrito");

            if (string.IsNullOrEmpty(carritoJson))
            {
                return new List<CarritoItem>();
            }

            var carrito =
                JsonSerializer.Deserialize<List<CarritoItem>>(
                    carritoJson);

            return carrito ?? new List<CarritoItem>();
        }

        // ==========================================
        // GUARDAR CARRITO EN SESIÓN
        // ==========================================

        private void GuardarCarrito(
            List<CarritoItem> carrito)
        {
            var carritoJson =
                JsonSerializer.Serialize(carrito);

            HttpContext.Session.SetString(
                "Carrito",
                carritoJson);
        }
    }
}