using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using F1Manager.Web.Data;
using F1Manager.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace F1Manager.Web.Pages.Pilotos
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

        public Piloto Piloto { get; set; } = default!;
        public int NumeroVitorias { get; set; }

        // Propriedade que guarda o URL para onde o botão "Voltar" deve apontar
        public string ReturnUrl { get; set; } = "/Pilotos/Index";
        public bool EFavorito { get; set; }

        public async Task<IActionResult> OnGetAsync(int id, string? returnUrl = null)
        {
            // Define o destino padrão se não for fornecido um URL de retorno
            ReturnUrl = returnUrl ?? "/Pilotos/Index";

            // Procura o piloto na BD, carrega a equipa e todos os resultados associados
            Piloto = await _context.Pilotos
                .Include(p => p.Equipa)
                .Include(p => p.Resultados)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Piloto == null)
            {
                return NotFound();
            }

            // Calcula o número de vitórias (corridas onde a posição final foi 1)
            NumeroVitorias = Piloto.Resultados.Count(r => r.PosicaoFinal == 1);

            // Verifica se o piloto atual está nos favoritos do utilizador autenticado
            var userId = _userManager.GetUserId(User);
            if (userId != null)
            {
                EFavorito = await _context.Favoritos.AnyAsync(f => f.UserId == userId && f.PilotoId == id);
            }

            return Page();
        }

        // Método POST para alternar o piloto favorito
            public async Task<IActionResult> OnPostToggleFavoritoAsync(int id)
            {
                var userId = _userManager.GetUserId(User);
                if (userId == null) return Challenge();

                var favorito = await _context.Favoritos.FirstOrDefaultAsync(f => f.UserId == userId);

                if (favorito == null)
                {
                    favorito = new Favorito { UserId = userId, PilotoId = id };
                    _context.Favoritos.Add(favorito);
                }
                else
                {
                    // Se já for o favorito, desmarca. Caso contrário, atualiza para o novo ID
                    favorito.PilotoId = (favorito.PilotoId == id) ? null : id;
                    _context.Favoritos.Update(favorito);
                }

                await _context.SaveChangesAsync();
                return RedirectToPage(new { id });
            }
    }
}