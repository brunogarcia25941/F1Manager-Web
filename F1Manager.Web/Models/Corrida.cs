using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace F1Manager.Web.Models
{
    public class Corrida
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do Grande Prémio é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome do Grande Prémio não pode exceder 100 caracteres.")]
        public string NomeGrandePremio { get; set; }

        [Required(ErrorMessage = "O nome do circuito é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome do circuito não pode exceder 100 caracteres.")]
        public string Circuito { get; set; }

        [Required]
        public DateTime DataHora { get; set; }

        // Chave Estrangeira (1-para-Muitos) - Campeonato
        [ForeignKey("Campeonato")]
        public int CampeonatoId { get; set; }
        public Campeonato? Campeonato { get; set; }

        // Navegação para a tabela associativa (Muitos-para-Muitos)
        public virtual ICollection<ResultadoCorrida> Resultados { get; set; } = new List<ResultadoCorrida>();
    }
}