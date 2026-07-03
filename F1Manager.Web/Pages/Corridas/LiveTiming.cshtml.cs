using F1Manager.Web.Data;
using F1Manager.Web.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace F1Manager.Web.Pages.Corridas
{
    public class LiveTimingModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public LiveTimingModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Corrida? Corrida { get; set; }
        public IList<ResultadoCorrida> Resultados { get; set; } = new List<ResultadoCorrida>();

        public async Task OnGetAsync(int id)
        {
            Corrida = await _context.Corridas.FindAsync(id);
            Resultados = await _context.ResultadosCorridas
                .Include(r => r.Piloto)
                    .ThenInclude(p => p.Equipa)
                .Where(r => r.CorridaId == id)
                .OrderBy(r => r.PosicaoFinal)
                .ToListAsync();
        }
    }
}
