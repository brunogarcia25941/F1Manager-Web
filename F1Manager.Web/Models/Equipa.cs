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

        // Relacionamento 1-para-Muitos: Uma equipa tem vários pilotos
        public virtual ICollection<Piloto> Pilotos { get; set; } = new List<Piloto>();
    }
}