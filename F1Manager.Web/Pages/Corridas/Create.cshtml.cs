using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using F1Manager.Web.Data;
using F1Manager.Web.Models;
using Microsoft.AspNetCore.Authorization;

namespace F1Manager.Web.Pages.Corridas
{
    [Authorize(Roles = "Administrador")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Corrida Corrida { get; set; } = default!;

        public IActionResult OnGet(int? campeonatoId)
        {
            // Se o campeonatoId vier por parâmetro, definimos no objeto para pré-selecionar
            Corrida = new Corrida();
            if (campeonatoId.HasValue)
            {
                Corrida.CampeonatoId = campeonatoId.Value;
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

            _context.Corridas.Add(Corrida);
            await _context.SaveChangesAsync();

            // Redireciona para os detalhes do campeonato caso tenhamos vindo de lá, ou para o Index de corridas
            return RedirectToPage("/Campeonatos/Details", new { id = Corrida.CampeonatoId });
        }
    }
}
