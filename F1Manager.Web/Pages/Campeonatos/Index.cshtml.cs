using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using F1Manager.Web.Data;
using F1Manager.Web.Models;

namespace F1Manager.Web.Pages.Campeonatos
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Campeonato> Campeonatos { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Campeonatos = await _context.Campeonatos
                    .Include(c => c.Corridas)
                    .ToListAsync();
        }
    }
}
