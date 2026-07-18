 using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using F1Manager.Web.Data;

    namespace F1Manager.Web.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class SearchController : ControllerBase
        {
            private readonly ApplicationDbContext _context;

            public SearchController(ApplicationDbContext context)
            {
                _context = context;
            }

            // Endpoint GET: api/search?q=termo 
            [HttpGet]
            public async Task<IActionResult> Search([FromQuery] string q)
            {
                if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
                {
                    return Ok(new object[] { });
                }

                var queryLimpa = q.Trim().ToLower();

                // 1. Pesquisa nos Pilotos
                var pilotos = await _context.Pilotos
                    .Where(p => p.Nome.ToLower().Contains(queryLimpa))
                    .Select(p => new SearchResult
                    {
                        Title = p.Nome,
                        Subtitle = $"Piloto #{p.NumeroCarro}",
                        Type = "Piloto",
                        Url = $"/Pilotos/Details/{p.Id}"
                    })
                    .Take(5)
                    .ToListAsync();

                // 2. Pesquisa nas Equipas
                var equipas = await _context.Equipas
                    .Where(e => e.Nome.ToLower().Contains(queryLimpa))
                    .Select(e => new SearchResult
                    {
                        Title = e.Nome,
                        Subtitle = $"Equipa ({e.Pais})",
                        Type = "Equipa",
                        Url = $"/Equipas/Details/{e.Id}"
                    })
                    .Take(5)
                    .ToListAsync();

                // 3. Pesquisa nos Campeonatos
                var campeonatos = await _context.Campeonatos
                    .Where(c => c.Nome.ToLower().Contains(queryLimpa))
                    .Select(c => new SearchResult
                    {
                        Title = c.Nome,
                        Subtitle = $"Campeonato - Época {c.Ano}",
                        Type = "Campeonato",
                        Url = $"/Campeonatos/Details/{c.Id}"
                    })
                    .Take(5)
                    .ToListAsync();

                // 4. Pesquisa nas Corridas
                var corridas = await _context.Corridas
                    .Where(co => co.NomeGrandePremio.ToLower().Contains(queryLimpa) || co.Circuito.ToLower().Contains(queryLimpa))
                    .Select(co => new SearchResult
                    {
                        Title = co.NomeGrandePremio,
                        Subtitle = $"GP - Circuito: {co.Circuito}",
                        Type = "Corrida",
                        Url = $"/Corridas/Details?id={co.Id}"
                    })
                    .Take(5)
                    .ToListAsync();

                // Agrupa todos os resultados numa única lista ordenada
                var resultados = pilotos
                    .Concat(equipas)
                    .Concat(campeonatos)
                    .Concat(corridas)
                    .ToList();

                return Ok(resultados);
            }

            // Classe DTO interna para formatação do JSON
            public class SearchResult
            {
                public string Title { get; set; } = string.Empty;
                public string Subtitle { get; set; } = string.Empty;
                public string Type { get; set; } = string.Empty;
                public string Url { get; set; } = string.Empty;
            }
        }
    }