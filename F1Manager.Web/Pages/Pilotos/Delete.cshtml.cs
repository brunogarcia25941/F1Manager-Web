using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using F1Manager.Web.Data;
using F1Manager.Web.Models;
using Microsoft.AspNetCore.Authorization;

namespace F1Manager.Web.Pages.Pilotos
{
    [Authorize(Roles = "Administrador")]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public DeleteModel(ApplicationDbContext context) => _context = context;

        [BindProperty] public Piloto Piloto { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Piloto = await _context.Pilotos.FindAsync(id);
            if (Piloto == null) return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var piloto = await _context.Pilotos.FindAsync(Piloto.Id);
            if (piloto != null)
            {
                _context.Pilotos.Remove(piloto);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage("./Index");
        }
    }
}