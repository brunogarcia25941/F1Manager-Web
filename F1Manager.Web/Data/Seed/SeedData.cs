using Microsoft.AspNetCore.Identity;

namespace F1Manager.Web.Data
{
    public static class SeedData
    {
        // Este método será chamado no Program.cs
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            // Managers necessários para gerir utilizadores e permissões
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // 1. Criar os Roles se não existirem
            string[] roleNames = { "Administrador", "Piloto" };
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    // Cria a role
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // 2. Criar o utilizador Administrador padrão
            var adminEmail = "admin@f1manager.pt";
            var user = await userManager.FindByEmailAsync(adminEmail);

            if (user == null)
            {
                var adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true // Confirmamos o email para permitir o login imediato
                };

                // Criar o utilizador com uma password padrão (em produção, esta password deve ser alterada)
                var createPowerUser = await userManager.CreateAsync(adminUser, "Admin123!");
                if (createPowerUser.Succeeded)
                {
                    // Atribuir a role de Administrador ao novo utilizador
                    await userManager.AddToRoleAsync(adminUser, "Administrador");
                }
                else
                {
                    var errors = string.Join(", ", createPowerUser.Errors.Select(e => e.Description));
                    throw new Exception($"Erro ao criar o utilizador Administrador: {errors}");
                }
            }
        }
    }
}