using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using F1Manager.Web.Data;
using F1Manager.Web.Models;
using Microsoft.AspNetCore.Authorization;

namespace F1Manager.Web.Pages.Pilotos
{
    [Authorize(Roles = "Administrador")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public EditModel(ApplicationDbContext context) => _context = context;

        [BindProperty]
        public Piloto Piloto { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Piloto = await _context.Pilotos.FindAsync(id);
            if (Piloto == null) return NotFound();

            // Recarrega a lista de equipas para o dropdown
            ViewData["EquipaId"] = new SelectList(_context.Equipas, "Id", "Nome");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ViewData["EquipaId"] = new SelectList(_context.Equipas, "Id", "Nome");
                return Page();
            }

            _context.Attach(Piloto).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}