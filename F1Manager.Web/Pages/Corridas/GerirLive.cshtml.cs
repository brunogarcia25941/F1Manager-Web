using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using F1Manager.Web.Data;
using F1Manager.Web.Models;
using Microsoft.AspNetCore.Authorization;

namespace F1Manager.Web.Pages.Corridas
{
    [Authorize(Roles = "Administrador")]
    public class GerirLiveModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public GerirLiveModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Corrida Corrida { get; set; } = default!;
        public List<Piloto> Pilotos { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Corrida = await _context.Corridas.Include(c => c.Campeonato).FirstOrDefaultAsync(c => c.Id == id);
            if (Corrida == null)
            {
                return NotFound();
            }

            // Carrega todos os pilotos com as respetivas equipas para a seleção inicial
            Pilotos = await _context.Pilotos.Include(p => p.Equipa).ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id, List<int> FinalPilotoIds, List<string> FinalTempos)
        {
            Corrida = await _context.Corridas.FindAsync(id);
            if (Corrida == null)
            {
                return NotFound();
            }

            // Limpa registos de classificação antigos da corrida para evitar duplicados
            var resultadosExistentes = _context.ResultadosCorridas.Where(r => r.CorridaId == id);
            _context.ResultadosCorridas.RemoveRange(resultadosExistentes);

            // Grava os resultados oficiais gerados no Live na Base de Dados
            for (int i = 0; i < FinalPilotoIds.Count; i++)
            {
                int posicao = i + 1;
                int pontos = CalcularPontos(posicao);

                var resultado = new ResultadoCorrida
                {
                    CorridaId = id,
                    PilotoId = FinalPilotoIds[i],
                    PosicaoFinal = posicao,
                    Pontos = pontos,
                    TempoVoltaRapida = string.IsNullOrWhiteSpace(FinalTempos[i]) ? "--:--.---" : FinalTempos[i]
                };

                _context.ResultadosCorridas.Add(resultado);
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("./Details", new { id = id });
        }

        // Tabela padrão de atribuição de pontos da Fórmula 1
        private int CalcularPontos(int posicao)
        {
            return posicao switch
            {
                1 => 25,
                2 => 18,
                3 => 15,
                4 => 12,
                5 => 10,
                6 => 8,
                7 => 6,
                8 => 4,
                9 => 2,
                10 => 1,
                _ => 0
            };
        }
    }
}