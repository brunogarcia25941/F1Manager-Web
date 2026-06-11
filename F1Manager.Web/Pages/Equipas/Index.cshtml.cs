using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using F1Manager.Web.Data;
using F1Manager.Web.Models;

namespace F1Manager.Web.Pages.Equipas
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Propriedade para guardar a lista de equipas que será exibida na página
        public IList<Equipa> Equipas { get; set; } = default!;

        public async Task OnGetAsync()
        {
            // Carrega todas as equipas da base de dados
            Equipas = await _context.Equipas.ToListAsync();
        }
    }
}