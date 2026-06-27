using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using F1Manager.Web.Data;
using F1Manager.Web.Models;

namespace F1Manager.Web.Pages.Campeonatos
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Campeonato Campeonato { get; set; } = default!;
        public IList<ClassificacaoPiloto> Classificacao { get; set; } = new List<ClassificacaoPiloto>();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Campeonato = await _context.Campeonatos
                .Include(c => c.Corridas)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Campeonato == null)
            {
                return NotFound();
            }

            // Calcular a classificação do campeonato
            Classificacao = await _context.ResultadosCorridas
                .Include(rc => rc.Piloto)
                    .ThenInclude(p => p.Equipa)
                .Include(rc => rc.Corrida)
                .Where(rc => rc.Corrida.CampeonatoId == id)
                .GroupBy(rc => new { rc.PilotoId, rc.Piloto.Nome, NomeEquipa = rc.Piloto.Equipa != null ? rc.Piloto.Equipa.Nome : "Sem Equipa" })
                .Select(g => new ClassificacaoPiloto
                {
                    PilotoId = g.Key.PilotoId,
                    NomePiloto = g.Key.Nome,
                    NomeEquipa = g.Key.NomeEquipa,
                    PontosTotais = g.Sum(x => x.Pontos)
                })
                .OrderByDescending(c => c.PontosTotais)
                .ToListAsync();

            return Page();
        }
    }

    public class ClassificacaoPiloto
    {
        public int PilotoId { get; set; }
        public string NomePiloto { get; set; } = string.Empty;
        public string NomeEquipa { get; set; } = string.Empty;
        public int PontosTotais { get; set; }
    }
}
