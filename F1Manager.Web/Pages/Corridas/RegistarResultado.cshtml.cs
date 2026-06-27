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
    public class RegistarResultadoModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public RegistarResultadoModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Corrida Corrida { get; set; } = default!;

        [BindProperty]
        public ResultadoCorrida Resultado { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Corrida = await _context.Corridas.FindAsync(id);
            if (Corrida == null)
            {
                return NotFound();
            }

            Resultado = new ResultadoCorrida { CorridaId = id };

            await PreparaDropdownPilotosAsync(id);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            Corrida = await _context.Corridas.FindAsync(id);
            if (Corrida == null)
            {
                return NotFound();
            }

            // Atribui explicitamente a CorridaId recebida
            Resultado.CorridaId = id;

            // Limpa erros de navegação de modelo associados a objetos de navegação complexos que não são submetidos
            ModelState.Remove("Resultado.Piloto");
            ModelState.Remove("Resultado.Corrida");

            if (!ModelState.IsValid)
            {
                await PreparaDropdownPilotosAsync(id);
                return Page();
            }

            // Validação 1: O piloto já tem um resultado registado nesta corrida?
            var pilotoJaRegistado = await _context.ResultadosCorridas
                .AnyAsync(rc => rc.CorridaId == id && rc.PilotoId == Resultado.PilotoId);
            if (pilotoJaRegistado)
            {
                ModelState.AddModelError("Resultado.PilotoId", "Este piloto já tem um resultado registado nesta corrida.");
            }

            // Validação 2: A posição final já está ocupada por outro piloto nesta corrida?
            var posicaoOcupada = await _context.ResultadosCorridas
                .AnyAsync(rc => rc.CorridaId == id && rc.PosicaoFinal == Resultado.PosicaoFinal);
            if (posicaoOcupada)
            {
                ModelState.AddModelError("Resultado.PosicaoFinal", "Esta posição já foi registada para outro piloto nesta corrida.");
            }

            if (!ModelState.IsValid)
            {
                await PreparaDropdownPilotosAsync(id);
                return Page();
            }

            _context.ResultadosCorridas.Add(Resultado);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Details", new { id = id });
        }

        private async Task PreparaDropdownPilotosAsync(int corridaId)
        {
            // Descobrir IDs dos pilotos que já participaram nesta corrida
            var pilotosComResultadoIds = await _context.ResultadosCorridas
                .Where(rc => rc.CorridaId == corridaId)
                .Select(rc => rc.PilotoId)
                .ToListAsync();

            // Filtrar pilotos elegíveis (que não participaram ainda nesta corrida)
            var pilotosElegiveis = await _context.Pilotos
                .Where(p => !pilotosComResultadoIds.Contains(p.Id))
                .ToListAsync();

            ViewData["PilotoId"] = new SelectList(pilotosElegiveis, "Id", "Nome");
        }
    }
}
