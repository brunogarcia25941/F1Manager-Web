using System.ComponentModel.DataAnnotations;

namespace F1Manager.Web.Models
{
    public class Campeonato
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do campeonato é obrigatório.")]
        [StringLength(100)]
        public string Nome { get; set; } // Ex: Fórmula 1 - 2025, ou Fórmula 2 - 2025

        [Required(ErrorMessage = "O ano do campeonato é obrigatório.")]
        [Range(1950, 2100, ErrorMessage = "O ano do campeonato deve estar entre 1950 e 2100.")]
        public int Ano { get; set; }

        // Relacionamento 1-para-Muitos: Um Campeonato tem várias Corridas
        public virtual ICollection<Corrida> Corridas { get; set; } = new List<Corrida>();
    }
}