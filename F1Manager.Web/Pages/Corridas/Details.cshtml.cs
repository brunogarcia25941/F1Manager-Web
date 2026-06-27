using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using F1Manager.Web.Data;
using F1Manager.Web.Models;

namespace F1Manager.Web.Pages.Corridas
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Corrida Corrida { get; set; } = default!;
        public IList<ResultadoCorrida> Resultados { get; set; } = new List<ResultadoCorrida>();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Corrida = await _context.Corridas
                .Include(c => c.Campeonato)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Corrida == null)
            {
                return NotFound();
            }

            // Carregar resultados oficiais ordenados por posição final
            Resultados = await _context.ResultadosCorridas
                .Include(rc => rc.Piloto)
                    .ThenInclude(p => p.Equipa)
                .Where(rc => rc.CorridaId == id)
                .OrderBy(rc => rc.PosicaoFinal)
                .ToListAsync();

            return Page();
        }

        // Handler para remover um resultado diretamente da lista
        public async Task<IActionResult> OnPostDeleteResultadoAsync(int pilotoId, int id)
        {
            var resultado = await _context.ResultadosCorridas.FindAsync(pilotoId, id);

            if (resultado != null)
            {
                _context.ResultadosCorridas.Remove(resultado);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage(new { id = id });
        }
    }
}
