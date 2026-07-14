using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using F1Manager.Web.Data;
using F1Manager.Web.Models;

namespace F1Manager.Web.Pages.Admin.Gerenciar
{
    [Authorize(Roles = "Administrador")]
    public class VincularEquipaModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public VincularEquipaModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<IdentityUser> Utilizadores { get; set; } = new();
        public List<Equipa> Equipas { get; set; } = new();
        public List<EquipaComUtilizador> EquipasVinculadas { get; set; } = new();
        
        public IdentityUser? UtilizadorSelecionado { get; set; }
        public string? UtilizadorSelecionadoId { get; set; }
        public List<string> RolesUtilizador { get; set; } = new();

        public Equipa? EquipaSelecionada { get; set; }
        public int? EquipaSelecionadaId { get; set; }

        public async Task OnGetAsync(string? utilizadorId = null, int? equipaId = null)
        {
            // Carregar todos os utilizadores
            Utilizadores = _userManager.Users.ToList();

            // Carregar todas as equipas
            Equipas = _context.Equipas.ToList();

            // Carregar equipas vinculadas
            EquipasVinculadas = _context.Equipas
                .Where(e => !string.IsNullOrEmpty(e.UserId))
                .Select(e => new EquipaComUtilizador
                {
                    EquipaId = e.Id,
                    Nome = e.Nome,
                    EmailUtilizador = _context.Users
                        .Where(u => u.Id == e.UserId)
                        .Select(u => u.Email)
                        .FirstOrDefault() ?? "Utilizador Eliminado"
                })
                .ToList();

            // Se foi selecionado um utilizador
            if (!string.IsNullOrEmpty(utilizadorId))
            {
                UtilizadorSelecionadoId = utilizadorId;
                UtilizadorSelecionado = await _userManager.FindByIdAsync(utilizadorId);
                
                if (UtilizadorSelecionado != null)
                {
                    RolesUtilizador = (await _userManager.GetRolesAsync(UtilizadorSelecionado)).ToList();
                }
            }

            // Se foi selecionada uma equipa
            if (equipaId.HasValue)
            {
                EquipaSelecionadaId = equipaId;
                EquipaSelecionada = _context.Equipas.Find(equipaId);
            }
        }

        public async Task<IActionResult> OnPostAsync(string command, string utilizadorId, int equipaId)
        {
            if (command == "vincular")
            {
                var equipa = _context.Equipas.Find(equipaId);
                if (equipa == null)
                {
                    ModelState.AddModelError("", "Equipa não encontrada.");
                    await OnGetAsync(utilizadorId, equipaId);
                    return Page();
                }

                var utilizador = await _userManager.FindByIdAsync(utilizadorId);
                if (utilizador == null)
                {
                    ModelState.AddModelError("", "Utilizador não encontrado.");
                    await OnGetAsync(utilizadorId, equipaId);
                    return Page();
                }

                // Verificar se o utilizador tem o role "Equipa"
                var roles = await _userManager.GetRolesAsync(utilizador);
                if (!roles.Contains("Equipa"))
                {
                    ModelState.AddModelError("", "O utilizador deve ter o role 'Equipa' atribuído antes de vincular a uma equipa.");
                    await OnGetAsync(utilizadorId, equipaId);
                    return Page();
                }

                // Vincular a equipa ao utilizador
                equipa.UserId = utilizador.Id;
                _context.Equipas.Update(equipa);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Equipa '{equipa.Nome}' vinculada com sucesso a '{utilizador.Email}'.";
                return RedirectToPage();
            }
            else if (command == "desvincular")
            {
                var equipa = _context.Equipas.Find(equipaId);
                if (equipa == null)
                {
                    ModelState.AddModelError("", "Equipa não encontrada.");
                    await OnGetAsync();
                    return Page();
                }

                var emailAnterior = "";
                if (!string.IsNullOrEmpty(equipa.UserId))
                {
                    var userAnterior = await _userManager.FindByIdAsync(equipa.UserId);
                    if (userAnterior != null)
                        emailAnterior = userAnterior.Email ?? "Utilizador Desconhecido";
                }

                equipa.UserId = null;
                _context.Equipas.Update(equipa);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Equipa '{equipa.Nome}' desvinculada de '{emailAnterior}'.";
                return RedirectToPage();
            }

            await OnGetAsync(utilizadorId, equipaId);
            return Page();
        }

        public class EquipaComUtilizador
        {
            public int EquipaId { get; set; }
            public string? Nome { get; set; }
            public string? EmailUtilizador { get; set; }
        }
    }
}
