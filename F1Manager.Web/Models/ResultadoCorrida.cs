using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace F1Manager.Web.Models
{
    public class ResultadoCorrida
    {

        [ForeignKey("Piloto")]
        public int PilotoId { get; set; }
        public Piloto Piloto { get; set; }

        [ForeignKey("Corrida")]
        public int CorridaId { get; set; }
        public Corrida Corrida { get; set; }

        [Range(1, 25, ErrorMessage = "A posição tem de ser entre 1 e 25.")]
        public int PosicaoFinal { get; set; }
        
        public int Pontos { get; set; }
        
        public string TempoVoltaRapida { get; set; }
    }
}