using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using F1Manager.Web.Data;
using F1Manager.Web.Models;
using Microsoft.AspNetCore.Authorization;

namespace F1Manager.Web.Pages.Corridas
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
        public Corrida Corrida { get; set; } = default!;

        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Corrida = await _context.Corridas.FindAsync(id);

            if (Corrida == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var corrida = await _context.Corridas
                .Include(c => c.Resultados)
                .FirstOrDefaultAsync(c => c.Id == Corrida.Id);

            if (corrida == null)
            {
                return NotFound();
            }

            // Regra de Negócio: Não apagar corridas com resultados registados
            if (corrida.Resultados != null && corrida.Resultados.Any())
            {
                Corrida = corrida;
                ErrorMessage = "Não é possível apagar uma corrida que já tenha resultados oficiais registados. Apague primeiro os resultados.";
                return Page();
            }

            _context.Corridas.Remove(corrida);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
