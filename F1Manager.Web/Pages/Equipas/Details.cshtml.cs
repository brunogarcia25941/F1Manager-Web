using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using F1Manager.Web.Data;
using F1Manager.Web.Models;

namespace F1Manager.Web.Pages.Equipas
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Equipa Equipa { get; set; } = default!;
        public int TotalPontos { get; set; }

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

            return Page();
        }
    }
}