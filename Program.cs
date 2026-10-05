using ColatonDeMelon.Data;
using ColatonDeMelon.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// CONEXIÓN CON SQL SERVER
builder.Services.AddDbContext<ColatonDeMelonContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("ConexionSQL")
    ));

// IDENTITY
builder.Services
    .AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        // Requisitos de contraseña
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 8;

        // Cada email debe ser único
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<ColatonDeMelonContext>()
    .AddDefaultTokenProviders();

builder.Services.AddTransient<IEmailSender, EmailSender>();

// MVC
builder.Services.AddControllersWithViews();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Cuenta/Login";
    options.AccessDeniedPath = "/Cuenta/AccesoDenegado";
});

builder.Services.AddSession();

var app = builder.Build();

// CONFIGURACIÓN
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseSession();

// IDENTIFICAR USUARIO
app.UseAuthentication();

// COMPROBAR PERMISOS
app.UseAuthorization();

// CREAR ROLES Y ADMINISTRADOR
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    await IdentitySeeder.SeedAsync(
        services,
        builder.Configuration);
}

// RUTA PRINCIPAL
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();