using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using F1Manager.Web.Data;

namespace F1Manager.Web.Pages.Admin.Gerenciar
{
    [Authorize(Roles = "Administrador")]
    public class PilotosModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public PilotosModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<PilotoComEquipa> Pilotos { get; set; } = new();

        public async Task OnGetAsync()
        {
            var pilotos = await _context.Pilotos
                .Include(p => p.Equipa)
                .Include(p => p.Resultados)
                .ToListAsync();

            foreach (var piloto in pilotos)
            {
                Pilotos.Add(new PilotoComEquipa
                {
                    Id = piloto.Id,
                    Nome = piloto.Nome,
                    NumeroCarro = piloto.NumeroCarro,
                    Equipa = piloto.Equipa?.Nome ?? "Sem Equipa",
                    NumeroResultados = piloto.Resultados.Count
                });
            }
        }
    }

    public class PilotoComEquipa
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public int NumeroCarro { get; set; }
        public string Equipa { get; set; }
        public int NumeroResultados { get; set; }
    }
}
