using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using F1Manager.Web.Data;

namespace F1Manager.Web.Pages.Admin.Gerenciar
{
    [Authorize(Roles = "Administrador")]
    public class CorridasModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CorridasModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<CorridaComDetalhes> Corridas { get; set; } = new();

        public async Task OnGetAsync()
        {
            var corridas = await _context.Corridas
                .Include(c => c.Campeonato)
                .Include(c => c.Resultados)
                .ToListAsync();

            foreach (var corrida in corridas)
            {
                Corridas.Add(new CorridaComDetalhes
                {
                    Id = corrida.Id,
                    NomeGrandePremio = corrida.NomeGrandePremio,
                    Circuito = corrida.Circuito,
                    DataHora = corrida.DataHora,
                    Campeonato = corrida.Campeonato?.Nome ?? "Sem Campeonato",
                    NumeroParticipantes = corrida.Resultados.Count
                });
            }
        }
    }

    public class CorridaComDetalhes
    {
        public int Id { get; set; }
        public string NomeGrandePremio { get; set; }
        public string Circuito { get; set; }
        public DateTime DataHora { get; set; }
        public string Campeonato { get; set; }
        public int NumeroParticipantes { get; set; }
    }
}
