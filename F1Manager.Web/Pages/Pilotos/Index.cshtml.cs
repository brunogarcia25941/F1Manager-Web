using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using F1Manager.Web.Data;
using F1Manager.Web.Models;

namespace F1Manager.Web.Pages.Pilotos
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public IndexModel(ApplicationDbContext context) => _context = context;

        public IList<Piloto> Pilotos { get; set; } = default!;

        public async Task OnGetAsync()
        {
            // O .Include(p => p.Equipa) é essencial para trazer os dados da equipa (Join)
            Pilotos = await _context.Pilotos.Include(p => p.Equipa).ToListAsync();
        }
    }
}