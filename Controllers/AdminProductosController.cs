using ColatonDeMelon.Data;
using ColatonDeMelon.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ColatonDeMelon.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminProductosController : Controller
    {
        private readonly ColatonDeMelonContext _context;
        private readonly IWebHostEnvironment _environment;

    public AdminProductosController(
        ColatonDeMelonContext context,
        IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }


        public async Task<IActionResult> Index()
        {
            var productos = await _context.Productos
                .Include(p => p.Edad)
                .Include(p => p.TipoPrenda)
                .OrderBy(p => p.Nombre)
                .ToListAsync();

            return View(productos);
        }


        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            await CargarOpciones();

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(
            Producto producto,
            IFormFile imagenArchivo)
        {
            if (!ModelState.IsValid)
            {
                await CargarOpciones();

                return View(producto);
            }


            if (producto.IdCategoria == 0)
            {
                producto.IdCategoria = 1;
            }


            // Si se seleccionó una imagen
            if (imagenArchivo != null &&
                imagenArchivo.Length > 0)
            {
                var extensionesPermitidas = new[]
                {
                ".jpg",
                ".jpeg",
                ".png",
                ".webp"
            };

                var extension = Path
                    .GetExtension(imagenArchivo.FileName)
                    .ToLowerInvariant();


                if (!extensionesPermitidas.Contains(extension))
                {
                    ModelState.AddModelError(
                        "imagenArchivo",
                        "La imagen debe ser JPG, JPEG, PNG o WEBP.");

                    await CargarOpciones();

                    return View(producto);
                }


                // Crear un nombre único para la imagen
                var nombreArchivo =
                    $"{Guid.NewGuid()}{extension}";


                // Ruta de la carpeta de imágenes
                var carpetaImagenes = Path.Combine(
                    _environment.WebRootPath,
                    "images",
                    "productos");


                // Crear la carpeta si no existe
                if (!Directory.Exists(carpetaImagenes))
                {
                    Directory.CreateDirectory(carpetaImagenes);
                }


                // Ruta completa del archivo
                var rutaArchivo = Path.Combine(
                    carpetaImagenes,
                    nombreArchivo);


                // Guardar la imagen
                using (var stream = new FileStream(
                    rutaArchivo,
                    FileMode.Create))
                {
                    await imagenArchivo.CopyToAsync(stream);
                }


                // Guardar el nombre en la base de datos
                producto.Imagen = nombreArchivo;
            }


            _context.Productos.Add(producto);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var producto = await _context.Productos
                .FirstOrDefaultAsync(
                    p => p.IdProducto == id);

            if (producto == null)
            {
                return NotFound();
            }


            await CargarOpciones();

            return View(producto);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(
 int id,
 Producto producto,
 IFormFile imagenArchivo)
        {
            if (id != producto.IdProducto)
            {
                return NotFound();
            }

if (!ModelState.IsValid)
            {
                await CargarOpciones();

                return View(producto);
            }

            try
            {
                var productoExistente = await _context.Productos
                    .FirstOrDefaultAsync(
                        p => p.IdProducto == id);

                if (productoExistente == null)
                {
                    return NotFound();
                }


                // Guardamos el nombre de la imagen anterior
                var imagenAnterior = productoExistente.Imagen;


                // Actualizar los datos
                productoExistente.Nombre = producto.Nombre;
                productoExistente.Descripcion = producto.Descripcion;
                productoExistente.Precio = producto.Precio;
                productoExistente.Stock = producto.Stock;
                productoExistente.IdEdad = producto.IdEdad;
                productoExistente.IdTipoPrenda = producto.IdTipoPrenda;
                productoExistente.IdCategoria = producto.IdCategoria;


                // Si se seleccionó una nueva imagen
                if (imagenArchivo != null &&
                    imagenArchivo.Length > 0)
                {
                    var extensionesPermitidas = new[]
                    {
            ".jpg",
            ".jpeg",
            ".png",
            ".webp"
        };

                    var extension = Path
                        .GetExtension(imagenArchivo.FileName)
                        .ToLowerInvariant();


                    if (!extensionesPermitidas.Contains(extension))
                    {
                        ModelState.AddModelError(
                            "imagenArchivo",
                            "La imagen debe ser JPG, JPEG, PNG o WEBP.");

                        await CargarOpciones();

                        return View(producto);
                    }


                    var carpetaImagenes = Path.Combine(
                        _environment.WebRootPath,
                        "images",
                        "productos");


                    if (!Directory.Exists(carpetaImagenes))
                    {
                        Directory.CreateDirectory(carpetaImagenes);
                    }


                    var nuevoNombreArchivo =
                        $"{Guid.NewGuid()}{extension}";


                    var nuevaRutaArchivo = Path.Combine(
                        carpetaImagenes,
                        nuevoNombreArchivo);


                    // Guardar la nueva imagen
                    using (var stream = new FileStream(
                        nuevaRutaArchivo,
                        FileMode.Create))
                    {
                        await imagenArchivo.CopyToAsync(stream);
                    }


                    // Actualizar el nombre en la base de datos
                    productoExistente.Imagen = nuevoNombreArchivo;


                    // Eliminar la imagen anterior
                    if (!string.IsNullOrWhiteSpace(imagenAnterior))
                    {
                        var rutaImagenAnterior = Path.Combine(
                            carpetaImagenes,
                            imagenAnterior);

                        if (System.IO.File.Exists(
                            rutaImagenAnterior))
                        {
                            System.IO.File.Delete(
                                rutaImagenAnterior);
                        }
                    }
                }


                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                var existe = await _context.Productos
                    .AnyAsync(
                        p => p.IdProducto == id);

                if (!existe)
                {
                    return NotFound();
                }

                throw;
            }

}


        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            var producto = await _context.Productos
            .FirstOrDefaultAsync(
            p => p.IdProducto == id);

if (producto == null)
            {
                return NotFound();
            }

            // Guardamos el nombre de la imagen antes de eliminar el producto.
            var nombreImagen = producto.Imagen;

            // Eliminamos el producto de la base de datos.
            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            // Eliminamos también la imagen física.
            if (!string.IsNullOrWhiteSpace(nombreImagen))
            {
                var carpetaImagenes = Path.Combine(
                    _environment.WebRootPath,
                    "images",
                    "productos");

                var rutaImagen = Path.Combine(
                    carpetaImagenes,
                    nombreImagen);

                if (System.IO.File.Exists(rutaImagen))
                {
                    System.IO.File.Delete(rutaImagen);
                }
            }

            return RedirectToAction(nameof(Index));

}


        private async Task CargarOpciones()
        {
            ViewBag.Edades = await _context.Edades
                .OrderBy(e => e.IdEdad)
                .ToListAsync();

            ViewBag.TiposPrenda = await _context.TiposPrenda
                .OrderBy(t => t.IdTipoPrenda)
                .ToListAsync();
        }
    }

}
