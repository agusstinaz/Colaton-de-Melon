using Microsoft.AspNetCore.Identity;

namespace ColatonDeMelon.Data
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(
            IServiceProvider serviceProvider,
            IConfiguration configuration)
        {
            var userManager =
                serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            var roleManager =
                serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // ==========================================
            // CREAR ROL ADMIN
            // ==========================================

            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(
                    new IdentityRole("Admin"));
            }

            // ==========================================
            // CREAR ROL CLIENTE
            // ==========================================

            if (!await roleManager.RoleExistsAsync("Cliente"))
            {
                await roleManager.CreateAsync(
                    new IdentityRole("Cliente"));
            }

            // ==========================================
            // DATOS DEL ADMIN
            // ==========================================

            var email = configuration["AdminUser:Email"];
            var password = configuration["AdminUser:Password"];

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                return;
            }

            // ==========================================
            // BUSCAR ADMINISTRADOR
            // ==========================================

            var admin = await userManager.FindByEmailAsync(email);

            // ==========================================
            // CREAR ADMINISTRADOR
            // ==========================================

            if (admin == null)
            {
                admin = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(
                    admin,
                    password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(
                        admin,
                        "Admin");
                }
            }
            else
            {
                // ==========================================
                // ASEGURAR QUE SEA ADMIN
                // ==========================================

                if (!await userManager.IsInRoleAsync(
                    admin,
                    "Admin"))
                {
                    await userManager.AddToRoleAsync(
                        admin,
                        "Admin");
                }
            }
        }
    }
}