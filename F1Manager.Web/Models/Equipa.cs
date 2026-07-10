using System.ComponentModel.DataAnnotations;

namespace F1Manager.Web.Models
{
    public class Equipa
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome da equipa é obrigatório.")]
        [StringLength(50)]
        public string Nome { get; set; }

        public string FabricanteMotor { get; set; }

        public string Pais { get; set; }

        [StringLength(100, ErrorMessage = "O nome do chefe de equipa não pode exceder os 100 caracteres.")]
        public string? ChefeEquipa { get; set; }

        [Range(1900, 2026, ErrorMessage = "O ano de fundação deve estar entre 1900 e 2026.")]
        public int? AnoFundacao { get; set; }

        [StringLength(1000, ErrorMessage = "A história não pode exceder os 1000 caracteres.")]
        public string? Historia { get; set; }

        // Guarda o caminho relativo do logótipo da equipa no servidor
        public string? Logotipo { get; set; }

        // Relacionamento 1-para-Muitos: Uma equipa tem vários pilotos
        public virtual ICollection<Piloto> Pilotos { get; set; } = new List<Piloto>();
    }
}