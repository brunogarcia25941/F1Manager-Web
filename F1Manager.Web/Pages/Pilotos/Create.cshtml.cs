using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using F1Manager.Web.Data;
using F1Manager.Web.Models;
using Microsoft.AspNetCore.Authorization;

namespace F1Manager.Web.Pages.Pilotos
{
    [Authorize(Roles = "Administrador")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public CreateModel(ApplicationDbContext context) => _context = context;

        [BindProperty]
        public Piloto Piloto { get; set; } = default!;

        public IActionResult OnGet()
        {
            // Preenche a lista de equipas para o dropdown
            // Mostra o 'Nome' mas guarda o 'Id'
            ViewData["EquipaId"] = new SelectList(_context.Equipas, "Id", "Nome");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Se o modelo for inválido, recarrega a página e a lista de equipas
            if (!ModelState.IsValid)
            {
                ViewData["EquipaId"] = new SelectList(_context.Equipas, "Id", "Nome");
                return Page();
            }

            _context.Pilotos.Add(Piloto);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}