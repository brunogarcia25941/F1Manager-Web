using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.EntityFrameworkCore;
    using F1Manager.Web.Data;
    using F1Manager.Web.Models;

    namespace F1Manager.Web.Pages.Campeonatos
    {
        public class IndexModel : PageModel
        {
            private readonly ApplicationDbContext _context;

            public IndexModel(ApplicationDbContext context)
            {
                _context = context;
            }

            public IList<CampeonatoCardDto> CampeonatosExibicao { get; set; } = new List<CampeonatoCardDto>();

            public async Task OnGetAsync()
            {
                var campeonatos = await _context.Campeonatos
                        .Include(c => c.Corridas)
                            .ThenInclude(co => co.Resultados)
                                .ThenInclude(r => r.Piloto)
                                    .ThenInclude(p => p.Equipa)
                        .ToListAsync();

                // Mapeamento e cálculo de estatísticas em tempo real
                CampeonatosExibicao = campeonatos.Select(c => {
                    var totalCorridas = c.Corridas.Count;
                    var corridasConcluidas = c.Corridas.Count(co => co.Resultados.Any());

                    // Determinação do piloto líder do campeonato pelo total de pontos
                    var lider = c.Corridas
                        .SelectMany(co => co.Resultados)
                        .GroupBy(r => new { r.PilotoId, NomePiloto = r.Piloto.Nome, NomeEquipa = r.Piloto.Equipa != null ? r.Piloto.Equipa.Nome : "Sem Equipa" })
                        .Select(g => new {
                            Nome = g.Key.NomePiloto,
                            Equipa = g.Key.NomeEquipa,
                            Pontos = g.Sum(x => x.Pontos)
                        })
                        .OrderByDescending(x => x.Pontos)
                        .FirstOrDefault();

                    return new CampeonatoCardDto
                    {
                        Id = c.Id,
                        Nome = c.Nome,
                        Ano = c.Ano,
                        TotalCorridas = totalCorridas,
                        CorridasConcluidas = corridasConcluidas,
                        NomeLider = lider?.Nome,
                        EquipaLider = lider?.Equipa,
                        PontosLider = lider?.Pontos ?? 0
                    };
                }).ToList();
            }

            // Data Transfer Object (DTO)
            public class CampeonatoCardDto
            {
                public int Id { get; set; }
                public string Nome { get; set; } = string.Empty;
                public int Ano { get; set; }
                public int TotalCorridas { get; set; }
                public int CorridasConcluidas { get; set; }
                public string? NomeLider { get; set; }
                public string? EquipaLider { get; set; }
                public int PontosLider { get; set; }
            }
        }
    }