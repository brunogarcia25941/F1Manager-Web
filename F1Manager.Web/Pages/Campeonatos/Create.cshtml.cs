using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using F1Manager.Web.Data;
using F1Manager.Web.Models;
using Microsoft.AspNetCore.Authorization;

namespace F1Manager.Web.Pages.Campeonatos
{
    [Authorize(Roles = "Administrador")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Campeonato Campeonato { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Campeonatos.Add(Campeonato);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
