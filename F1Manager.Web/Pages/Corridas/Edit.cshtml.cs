using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using F1Manager.Web.Data;
using F1Manager.Web.Models;
using Microsoft.AspNetCore.Authorization;

namespace F1Manager.Web.Pages.Corridas
{
    [Authorize(Roles = "Administrador")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Corrida Corrida { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Corrida = await _context.Corridas.FindAsync(id);

            if (Corrida == null)
            {
                return NotFound();
            }

            ViewData["CampeonatoId"] = new SelectList(_context.Campeonatos, "Id", "Nome", Corrida.CampeonatoId);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ViewData["CampeonatoId"] = new SelectList(_context.Campeonatos, "Id", "Nome", Corrida.CampeonatoId);
                return Page();
            }

            _context.Attach(Corrida).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CorridaExists(Corrida.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool CorridaExists(int id)
        {
            return _context.Corridas.Any(e => e.Id == id);
        }
    }
}
