using F1Manager.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace F1Manager.Web.Pages.Shared.Components.Top3Rankings
{
    public class Top3RankingsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public Top3RankingsViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int corridaId)
        {
            var resultados = await _context.ResultadosCorridas
                .Include(r => r.Piloto)
                .Where(r => r.CorridaId == corridaId)
                .OrderBy(r => r.PosicaoFinal)
                .Take(3)
                .ToListAsync();

            return View(resultados);
        }
    }
}
