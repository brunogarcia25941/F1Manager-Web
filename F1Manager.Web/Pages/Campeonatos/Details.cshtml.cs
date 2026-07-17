using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using F1Manager.Web.Data;
using F1Manager.Web.Models;

namespace F1Manager.Web.Pages.Campeonatos
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Campeonato Campeonato { get; set; } = default!;
        public IList<ClassificacaoPiloto> Classificacao { get; set; } = new List<ClassificacaoPiloto>();
        public int TotalCorridas { get; set; }
        public int TotalResultados { get; set; }
        public EstatisticaPiloto? MaisCorridasTerminadas { get; set; }
        public EstatisticaPiloto? MaisVitorias { get; set; }
        public EstatisticaPiloto? MaisPodios { get; set; }
        public EstatisticaPiloto? MaisVoltasRapidas { get; set; }
        public EstatisticaPiloto? MaisMediaPontos { get; set; }
        public EquipaEstatistica? EquipaMaisPontos { get; set; }
        public IList<EquipaEstatistica> ClassificacaoEquipas { get; set; } = new List<EquipaEstatistica>();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Campeonato = await _context.Campeonatos
                .Include(c => c.Corridas)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Campeonato == null)
            {
                return NotFound();
            }

            // Carrega todos os resultados do campeonato para estatísticas e classificação
            var resultados = await _context.ResultadosCorridas
                .Include(rc => rc.Piloto)
                    .ThenInclude(p => p.Equipa)
                .Include(rc => rc.Corrida)
                .Where(rc => rc.Corrida.CampeonatoId == id)
                .ToListAsync();

            TotalCorridas = Campeonato.Corridas?.Count ?? 0;
            TotalResultados = resultados.Count;

            var estatisticas = resultados
                .GroupBy(rc => new { rc.PilotoId, rc.Piloto.Nome, NomeEquipa = rc.Piloto.Equipa != null ? rc.Piloto.Equipa.Nome : "Sem Equipa" })
                .Select(g => new EstatisticaPiloto
                {
                    PilotoId = g.Key.PilotoId,
                    NomePiloto = g.Key.Nome,
                    NomeEquipa = g.Key.NomeEquipa,
                    CorridasTerminadas = g.Count(),
                    Vitorias = g.Count(x => x.PosicaoFinal == 1),
                    Podios = g.Count(x => x.PosicaoFinal <= 3),
                    VoltasRapidas = g.Count(x => !string.IsNullOrEmpty(x.TempoVoltaRapida) && x.TempoVoltaRapida != "--:--.---"),
                    PontosTotais = g.Sum(x => x.Pontos),
                    MediaPontos = g.Any() ? (double)g.Sum(x => x.Pontos) / g.Count() : 0
                })
                .ToList();

            MaisCorridasTerminadas = estatisticas.OrderByDescending(e => e.CorridasTerminadas).FirstOrDefault();
            MaisVitorias = estatisticas.OrderByDescending(e => e.Vitorias).FirstOrDefault();
            MaisPodios = estatisticas.OrderByDescending(e => e.Podios).FirstOrDefault();
            MaisVoltasRapidas = estatisticas.OrderByDescending(e => e.VoltasRapidas).FirstOrDefault();
            MaisMediaPontos = estatisticas.OrderByDescending(e => e.MediaPontos).FirstOrDefault();

            // Agrupar resultados por equipa para calcular a classificação completa
            ClassificacaoEquipas = resultados
                .Where(rc => rc.Piloto.Equipa != null)
                .GroupBy(rc => new { rc.Piloto.EquipaId, rc.Piloto.Equipa!.Nome })
                .Select(g => new EquipaEstatistica
                {
                    EquipaId = g.Key.EquipaId,
                    NomeEquipa = g.Key.Nome,
                    PontosTotais = g.Sum(x => x.Pontos),
                    Corridas = g.Select(x => x.CorridaId).Distinct().Count(),
                    MediaPontos = g.Any() ? (double)g.Sum(x => x.Pontos) / g.Count() : 0
                })
                .OrderByDescending(e => e.PontosTotais)
                .ToList();

            EquipaMaisPontos = ClassificacaoEquipas.FirstOrDefault();

            Classificacao = estatisticas
                .OrderByDescending(c => c.PontosTotais)
                .Select(g => new ClassificacaoPiloto
                {
                    PilotoId = g.PilotoId,
                    NomePiloto = g.NomePiloto,
                    NomeEquipa = g.NomeEquipa,
                    PontosTotais = g.PontosTotais
                })
                .ToList();

            return Page();
        }
    }

    public class ClassificacaoPiloto
    {
        public int PilotoId { get; set; }
        public string NomePiloto { get; set; } = string.Empty;
        public string NomeEquipa { get; set; } = string.Empty;
        public int PontosTotais { get; set; }
    }

    public class EstatisticaPiloto
    {
        public int PilotoId { get; set; }
        public string NomePiloto { get; set; } = string.Empty;
        public string NomeEquipa { get; set; } = string.Empty;
        public int CorridasTerminadas { get; set; }
        public int Vitorias { get; set; }
        public int Podios { get; set; }
        public int VoltasRapidas { get; set; }
        public int PontosTotais { get; set; }
        public double MediaPontos { get; set; }
    }

    public class EquipaEstatistica
    {
        public int EquipaId { get; set; }
        public string NomeEquipa { get; set; } = string.Empty;
        public int PontosTotais { get; set; }
        public int Corridas { get; set; }
        public double MediaPontos { get; set; }
    }
}
