using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using F1Manager.Web.Data;
using F1Manager.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace F1Manager.Web.Pages.Equipas
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DetailsModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Equipa Equipa { get; set; } = default!;
        public int TotalPontos { get; set; }
        public bool EFavorito { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Carrega a equipa, os seus pilotos e todos os resultados de corridas deles
            Equipa = await _context.Equipas
                .Include(e => e.Pilotos)
                    .ThenInclude(p => p.Resultados)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Equipa == null)
            {
                return NotFound();
            }

            // Calcula o total de pontos da equipa no campeonato
            TotalPontos = Equipa.Pilotos
                .SelectMany(p => p.Resultados)
                .Sum(r => r.Pontos);

            // Verifica se a equipa atual está nos favoritos do utilizador autenticado
            var userId = _userManager.GetUserId(User);
            if (userId != null)
            {
                EFavorito = await _context.Favoritos.AnyAsync(f => f.UserId == userId && f.EquipaId == id);
            }

            return Page();
        }

        // Método POST para alternar a equipa favorita
        public async Task<IActionResult> OnPostToggleFavoritoAsync(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Challenge();

            var favorito = await _context.Favoritos.FirstOrDefaultAsync(f => f.UserId == userId);

            if (favorito == null)
            {
                favorito = new Favorito { UserId = userId, EquipaId = id };
                _context.Favoritos.Add(favorito);
            }
            else
            {
                // Se já for a favorita, desmarca. Caso contrário, atualiza para o novo ID
                favorito.EquipaId = (favorito.EquipaId == id) ? null : id;
                _context.Favoritos.Update(favorito);
            }

            await _context.SaveChangesAsync();
            return RedirectToPage(new { id });
        }
    }
}