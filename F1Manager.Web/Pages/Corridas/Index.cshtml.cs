using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using F1Manager.Web.Data;
using F1Manager.Web.Models;

namespace F1Manager.Web.Pages.Corridas
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Corrida> Corridas { get; set; } = default!;

        public async Task OnGetAsync()
        {
            // Carrega corridas, campeonatos e resultados com os respetivos pilotos
            Corridas = await _context.Corridas
                .Include(c => c.Campeonato)
                .Include(c => c.Resultados)
                    .ThenInclude(r => r.Piloto)
                .OrderBy(c => c.DataHora)
                .ToListAsync();
        }
    }
}
