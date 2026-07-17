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

        [Range(0, 50, ErrorMessage = "A pontuação atribuída deve estar entre 0 e 50 pontos.")]
        public int Pontos { get; set; }


        [Required(ErrorMessage = "O tempo da volta mais rápida é obrigatório.")]
        [StringLength(20, ErrorMessage = "O tempo de volta não pode exceder 20 caracteres.")]
        [RegularExpression(@"^[0-9]{1,2}:[0-9]{2}\.[0-9]{3}$", ErrorMessage = "O tempo de volta deve seguir o formato de minutos:segundos.milésimos (ex: 1:23.456).")]
        public string TempoVoltaRapida { get; set; }
    }
}