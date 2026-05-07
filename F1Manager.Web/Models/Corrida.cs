using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace F1Manager.Web.Models
{
    public class Corrida
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string NomeGrandePremio { get; set; }

        [Required]
        public string Circuito { get; set; }

        [Required]
        public DateTime DataHora { get; set; }

        // Chave Estrangeira (1-para-Muitos) - Campeonato
        [ForeignKey("Campeonato")]
        public int CampeonatoId { get; set; }
        public Campeonato Campeonato { get; set; }

        // Navegação para a tabela associativa (Muitos-para-Muitos)
        public ICollection<ResultadoCorrida> Resultados { get; set; }
    }
}