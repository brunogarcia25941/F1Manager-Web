using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using F1Manager.Web.Data;
using F1Manager.Web.Models;
using Microsoft.AspNetCore.Authorization;

namespace F1Manager.Web.Pages.Equipas
{
    [Authorize(Roles = "Administrador")]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public DeleteModel(ApplicationDbContext context) => _context = context;

        [BindProperty] public Equipa Equipa { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Equipa = await _context.Equipas.FindAsync(id);
            if (Equipa == null) return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var equipa = await _context.Equipas.FindAsync(Equipa.Id);
            if (equipa != null)
            {
                _context.Equipas.Remove(equipa);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage("./Index");
        }
    }
}