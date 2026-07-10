using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using F1Manager.Web.Data;
using F1Manager.Web.Models;

namespace F1Manager.Web.Pages.Pilotos
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Piloto Piloto { get; set; } = default!;
        public int NumeroVitorias { get; set; }

        // Propriedade que guarda o URL para onde o botão "Voltar" deve apontar
        public string ReturnUrl { get; set; } = "/Pilotos/Index";

        public async Task<IActionResult> OnGetAsync(int id, string? returnUrl = null)
        {
            // Define o destino padrão se não for fornecido um URL de retorno
            ReturnUrl = returnUrl ?? "/Pilotos/Index";

            // Procura o piloto na BD, carrega a equipa e todos os resultados associados
            Piloto = await _context.Pilotos
                .Include(p => p.Equipa)
                .Include(p => p.Resultados)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Piloto == null)
            {
                return NotFound();
            }

            // Calcula o número de vitórias (corridas onde a posição final foi 1)
            NumeroVitorias = Piloto.Resultados.Count(r => r.PosicaoFinal == 1);

            return Page();
        }
    }
}