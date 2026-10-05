using ColatonDeMelon.Data;
using ColatonDeMelon.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ColatonDeMelon.Controllers
{
    public class CuentaController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ColatonDeMelonContext _context;
        private readonly IEmailSender _emailSender;

        public CuentaController(
     UserManager<IdentityUser> userManager,
     SignInManager<IdentityUser> signInManager,
     ColatonDeMelonContext context,
     IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _emailSender = emailSender;
        }

        // ==========================================
        // REGISTRO - MOSTRAR FORMULARIO
        // ==========================================

        [HttpGet]
        public IActionResult Registro(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        // ==========================================
        // REGISTRO - PROCESAR FORMULARIO
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registro(
            RegistroViewModel modelo,
            string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;

                return View(modelo);
            }

            var usuarioExistente =
                await _userManager.FindByEmailAsync(modelo.Email);

            if (usuarioExistente != null)
            {
                ModelState.AddModelError(
                    "Email",
                    "Ya existe una cuenta con ese correo electrónico.");

                ViewBag.ReturnUrl = returnUrl;

                return View(modelo);
            }

            var usuario = new IdentityUser
            {
                UserName = modelo.Email,
                Email = modelo.Email,
                EmailConfirmed = true
            };

            var resultado = await _userManager.CreateAsync(
                usuario,
                modelo.Password);

            if (resultado.Succeeded)
            {
                // Todo usuario que se registre
                // será Cliente.

                await _userManager.AddToRoleAsync(
                    usuario,
                    "Cliente");

                // Iniciar sesión automáticamente
                // después del registro.

                await _emailSender.SendEmailAsync(
    usuario.Email!,
    "¡Bienvenida/o a Colatón de Melón! 🩷",
    $@"
    <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; color: #5f5058;'>

        <div style='text-align: center; padding: 25px 0;'>
            <h1 style='color: #d9a9bc; margin-bottom: 10px;'>
                ¡Bienvenida/o a Colatón de Melón! 🩷
            </h1>
        </div>

        <p>
            Hola <strong>{modelo.Email}</strong>:
        </p>

        <p>
            Tu cuenta fue creada correctamente.
        </p>

        <p>
            Ahora podés iniciar sesión y disfrutar de nuestra tienda
            de ropa infantil.
        </p>

        <div style='background-color: #fff7fa; padding: 18px; margin: 25px 0;'>
            <p style='margin: 0;'>
                Si vos no creaste esta cuenta, podés ignorar este correo.
            </p>
        </div>

        <p>
            ¡Gracias por elegir <strong>Colatón de Melón</strong>! 🩷
        </p>

    </div>
    ");

                await _signInManager.SignInAsync(
                    usuario,
                    isPersistent: false);

                // Si venía de una página protegida,
                // volver a esa página.

                if (!string.IsNullOrEmpty(returnUrl) &&
                    Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction(
                    "Index",
                    "Home");
            }

            foreach (var error in resultado.Errors)
            {
                ModelState.AddModelError(
                    "",
                    error.Description);
            }

            ViewBag.ReturnUrl = returnUrl;

            return View(modelo);
        }

        // ==========================================
        // LOGIN - MOSTRAR FORMULARIO
        // ==========================================

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        // ==========================================
        // LOGIN - PROCESAR FORMULARIO
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            string email,
            string password,
            bool recordar = false,
            string? returnUrl = null)
        {
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError(
                    "",
                    "Ingresá tu correo y contraseña.");

                ViewBag.ReturnUrl = returnUrl;

                return View();
            }

            var resultado = await _signInManager.PasswordSignInAsync(
                email,
                password,
                recordar,
                lockoutOnFailure: false);

            if (resultado.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) &&
                    Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction(
                    "Index",
                    "Home");
            }

            ModelState.AddModelError(
                "",
                "El correo o la contraseña son incorrectos.");

            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        // ==========================================
        // CERRAR SESIÓN
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction(
                "Index",
                "Home");
        }

        // ==========================================
        // MI CUENTA
        // ==========================================

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MiCuenta()
        {
            var usuario = await _userManager.GetUserAsync(User);

            if (usuario == null)
            {
                return RedirectToAction(nameof(Login));
            }

            var pedidos = await _context.Pedidos
                .Include(p => p.Detalles)
                .Where(p => p.UsuarioId == usuario.Id)
                .OrderByDescending(p => p.FechaPedido)
                .ToListAsync();

            ViewBag.Pedidos = pedidos;

            return View(usuario);
        }

        // ==========================================
        // DETALLE DE UN PEDIDO DEL CLIENTE
        // ==========================================

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> DetallePedido(int id)
        {
            var usuario = await _userManager.GetUserAsync(User);

            if (usuario == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // Buscamos el pedido por su ID y también
            // comprobamos que pertenezca al usuario actual.

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

        // ==========================================
        // ACCESO DENEGADO
        // ==========================================

        [HttpGet]
        public IActionResult AccesoDenegado()
        {
            return View();

        }


        // ==========================================
        // RECUPERAR CONTRASEÑA - MOSTRAR FORMULARIO
        // ==========================================

        [HttpGet]
        public IActionResult OlvidePassword()
        {
            return View();
        }

        // ==========================================
        // RECUPERAR CONTRASEÑA - ENVIAR ENLACE
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OlvidePassword(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError(
                    "",
                    "Ingresá tu correo electrónico.");

                return View();
            }

            var usuario = await _userManager.FindByEmailAsync(email);

            // Por seguridad, mostramos el mismo mensaje
            // exista o no la cuenta.

            if (usuario == null || string.IsNullOrWhiteSpace(usuario.Email))
            {
                return View("EmailEnviado");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(usuario);

            var resetUrl = Url.Action(
                "RestablecerPassword",
                "Cuenta",
                new
                {
                    email = usuario.Email,
                    token = token
                },
                Request.Scheme);

            await _emailSender.SendEmailAsync(
                usuario.Email,
                "Restablecé tu contraseña - Colatón de Melón 🩷",
                $@"
        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; color: #5f5058;'>

            <div style='text-align: center; padding: 25px 0;'>
                <h1 style='color: #d9a9bc;'>
                    Recuperación de contraseña 🩷
                </h1>
            </div>

            <p>
                Recibimos una solicitud para restablecer la contraseña
                de tu cuenta de Colatón de Melón.
            </p>

            <p>
                Para crear una nueva contraseña, hacé clic en el siguiente botón:
            </p>

            <div style='text-align: center; margin: 30px 0;'>

                <a href='{resetUrl}'
                   style='display: inline-block;
                          padding: 12px 24px;
                          background-color: #d9a9bc;
                          color: white;
                          text-decoration: none;
                          border-radius: 4px;'>
                    RESTABLECER CONTRASEÑA
                </a>

            </div>

            <p style='font-size: 13px; color: #777;'>
                Si vos no solicitaste cambiar tu contraseña,
                podés ignorar este correo.
            </p>

            <p>
                <strong>Colatón de Melón</strong>
            </p>

        </div>
        ");

            return View("EmailEnviado");
        }

        // ==========================================
        // RESTABLECER CONTRASEÑA - MOSTRAR FORMULARIO
        // ==========================================

        [HttpGet]
        public IActionResult RestablecerPassword(
            string email,
            string token)
        {
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(token))
            {
                return RedirectToAction(nameof(Login));
            }

            ViewBag.Email = email;
            ViewBag.Token = token;

            return View();
        }

        // ==========================================
        // RESTABLECER CONTRASEÑA - PROCESAR
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestablecerPassword(
            string email,
            string token,
            string password,
            string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(token))
            {
                return RedirectToAction(nameof(Login));
            }

            if (string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(confirmPassword))
            {
                ModelState.AddModelError(
                    "",
                    "Completá ambos campos de contraseña.");

                ViewBag.Email = email;
                ViewBag.Token = token;

                return View();
            }

            if (password != confirmPassword)
            {
                ModelState.AddModelError(
                    "",
                    "Las contraseñas no coinciden.");

                ViewBag.Email = email;
                ViewBag.Token = token;

                return View();
            }

            var usuario = await _userManager.FindByEmailAsync(email);

            if (usuario == null)
            {
                return RedirectToAction(nameof(Login));
            }

            var resultado = await _userManager.ResetPasswordAsync(
                usuario,
                token,
                password);

            if (resultado.Succeeded)
            {
                return RedirectToAction(
                    nameof(PasswordRestablecida));
            }

            foreach (var error in resultado.Errors)
            {
                ModelState.AddModelError(
                    "",
                    error.Description);
            }

            ViewBag.Email = email;
            ViewBag.Token = token;

            return View();
        }

        // ==========================================
        // CONTRASEÑA RESTABLECIDA CORRECTAMENTE
        // ==========================================

        [HttpGet]
        public IActionResult PasswordRestablecida()
        {
            return View();
        }

        // ==========================================
        // CAMBIAR CONTRASEÑA DESDE MI CUENTA
        // ==========================================

        [HttpGet]
        [Authorize]
        public IActionResult CambiarPassword()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarPassword(
            CambiarPasswordViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var usuario = await _userManager.GetUserAsync(User);

            if (usuario == null)
            {
                return RedirectToAction(nameof(Login));
            }

            var resultado = await _userManager.ChangePasswordAsync(
                usuario,
                modelo.PasswordActual,
                modelo.NuevaPassword);

            if (resultado.Succeeded)
            {
                // Actualizamos la sesión para mantener
                // al usuario correctamente autenticado.
                await _signInManager.RefreshSignInAsync(usuario);

                return RedirectToAction(nameof(PasswordCambiada));
            }

            foreach (var error in resultado.Errors)
            {
                ModelState.AddModelError(
                    "",
                    error.Description);
            }

            return View(modelo);
        }

        // ==========================================
        // CONTRASEÑA CAMBIADA CORRECTAMENTE
        // ==========================================

        [HttpGet]
        [Authorize]
        public IActionResult PasswordCambiada()
        {
            return View();
        }

    }

}
