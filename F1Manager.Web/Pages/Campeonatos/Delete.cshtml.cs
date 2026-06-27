using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using F1Manager.Web.Data;
using F1Manager.Web.Models;
using Microsoft.AspNetCore.Authorization;

namespace F1Manager.Web.Pages.Campeonatos
{
    [Authorize(Roles = "Administrador")]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Campeonato Campeonato { get; set; } = default!;

        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Campeonato = await _context.Campeonatos.FindAsync(id);

            if (Campeonato == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var campeonato = await _context.Campeonatos
                .Include(c => c.Corridas)
                .FirstOrDefaultAsync(c => c.Id == Campeonato.Id);

            if (campeonato == null)
            {
                return NotFound();
            }

            // Regra de Negócio: Não apagar campeonatos com corridas associadas
            if (campeonato.Corridas != null && campeonato.Corridas.Any())
            {
                Campeonato = campeonato;
                ErrorMessage = "Não é possível apagar um campeonato que já tenha corridas associadas.";
                return Page();
            }

            _context.Campeonatos.Remove(campeonato);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
