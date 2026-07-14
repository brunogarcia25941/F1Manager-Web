using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using F1Manager.Web.Data;

namespace F1Manager.Web.Pages.Admin
{
    [Authorize(Roles = "Administrador")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public IndexModel(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public bool IsAdmin { get; set; }
        public int TotalUtilizadores { get; set; }
        public int TotalEquipas { get; set; }
        public int TotalPilotos { get; set; }
        public int TotalCorridas { get; set; }
        public Dictionary<string, int> UtilizadoresPorRole { get; set; } = new();

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                IsAdmin = await _userManager.IsInRoleAsync(user, "Administrador");
            }

            if (!IsAdmin)
                return;

            // Conta utilizadores
            TotalUtilizadores = _userManager.Users.Count();

            // Conta equipas
            TotalEquipas = await _context.Equipas.CountAsync();

            // Conta pilotos
            TotalPilotos = await _context.Pilotos.CountAsync();

            // Conta corridas
            TotalCorridas = await _context.Corridas.CountAsync();

            // Utilizadores por Role
            var roles = await _roleManager.Roles.ToListAsync();
            foreach (var role in roles)
            {
                var count = (await _userManager.GetUsersInRoleAsync(role.Name ?? "")).Count;
                UtilizadoresPorRole[role.Name ?? "Sem Role"] = count;
            }
        }
    }
}
