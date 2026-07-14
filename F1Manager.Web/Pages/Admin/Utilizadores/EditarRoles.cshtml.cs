using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using F1Manager.Web.Data;

namespace F1Manager.Web.Pages.Admin.Utilizadores
{
    [Authorize(Roles = "Administrador")]
    public class EditarRolesModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public EditarRolesModel(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        [BindProperty]
        public UtilizadorModel Utilizador { get; set; } = new();

        [BindProperty]
        public List<string> RolesSelecionadas { get; set; } = new();

        public List<string> RolesDisponiveis { get; set; } = new();
        public string SuccessMessage { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToPage("./Index");

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound("Utilizador não encontrado.");

            Utilizador = new UtilizadorModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName
            };

            // Carrega as roles disponíveis
            RolesDisponiveis = await _roleManager.Roles
                .Select(r => r.Name)
                .ToListAsync();

            // Carrega as roles atuais do utilizador
            RolesSelecionadas = (await _userManager.GetRolesAsync(user)).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.FindByIdAsync(Utilizador.Id);
            if (user == null)
                return NotFound("Utilizador não encontrado.");

            try
            {
                // Remove todas as roles atuais
                var rolesAtuais = await _userManager.GetRolesAsync(user);
                if (rolesAtuais.Any())
                {
                    await _userManager.RemoveFromRolesAsync(user, rolesAtuais);
                }

                // Adiciona as novas roles selecionadas
                if (RolesSelecionadas.Any())
                {
                    await _userManager.AddToRolesAsync(user, RolesSelecionadas);
                }

                SuccessMessage = $"Roles do utilizador {user.Email} atualizadas com sucesso!";

                // Recarrega os dados
                Utilizador = new UtilizadorModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName
                };

                RolesDisponiveis = await _roleManager.Roles
                    .Select(r => r.Name)
                    .ToListAsync();

                RolesSelecionadas = (await _userManager.GetRolesAsync(user)).ToList();

                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erro ao atualizar roles: {ex.Message}";
                RolesDisponiveis = await _roleManager.Roles
                    .Select(r => r.Name)
                    .ToListAsync();
                return Page();
            }
        }
    }

    public class UtilizadorModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
    }
}
