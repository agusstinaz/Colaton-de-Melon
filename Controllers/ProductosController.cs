using ColatonDeMelon.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ColatonDeMelon.Controllers
{
    public class ProductosController : Controller
    {
        private readonly ColatonDeMelonContext _context;

        public ProductosController(
            ColatonDeMelonContext context)
        {
            _context = context;
        }


        // =========================================
        // TODOS LOS PRODUCTOS
        // =========================================

        public async Task<IActionResult> Index(
            int? edad,
            int? tipo,
            string busqueda)
        {
            var productosQuery = _context.Productos
                .Include(p => p.Edad)
                .Include(p => p.TipoPrenda)
                .AsQueryable();


            // =========================================
            // FILTRO POR EDAD
            // =========================================

            if (edad.HasValue)
            {
                productosQuery = productosQuery
                    .Where(p => p.IdEdad == edad.Value);
            }


            // =========================================
            // FILTRO POR TIPO DE PRENDA
            // =========================================

            if (tipo.HasValue)
            {
                productosQuery = productosQuery
                    .Where(p => p.IdTipoPrenda == tipo.Value);
            }


            // =========================================
            // BUSCADOR
            // =========================================

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                busqueda = busqueda.Trim();

                productosQuery = productosQuery
                    .Where(p =>
                       p.Nombre.Contains(busqueda) ||
(p.Descripcion != null && p.Descripcion.Contains(busqueda)));
            }


            var productos = await productosQuery
                .OrderBy(p => p.Nombre)
                .ToListAsync();


            // =========================================
            // CATEGORÍAS POR EDAD
            // =========================================

            ViewBag.Edades = await _context.Edades
                .OrderBy(e => e.IdEdad)
                .ToListAsync();


            // =========================================
            // TIPOS DE PRENDA
            // =========================================

            ViewBag.TiposPrenda = await _context.TiposPrenda
                .OrderBy(t => t.IdTipoPrenda)
                .ToListAsync();


            ViewBag.EdadSeleccionada = edad;
            ViewBag.TipoSeleccionado = tipo;
            ViewBag.Busqueda = busqueda;


            return View(productos);
        }


        // =========================================
        // DETALLE DEL PRODUCTO
        // =========================================

        public async Task<IActionResult> Details(int id)
        {
            var producto = await _context.Productos
                .Include(p => p.Edad)
                .Include(p => p.TipoPrenda)
                .FirstOrDefaultAsync(
                    p => p.IdProducto == id);

            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }
    }
}