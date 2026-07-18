using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    namespace F1Manager.Web.Models
    {
        // Classe associativa para guardar os favoritos de cada utilizador
        public class Favorito
        {
            [Key]
            public int Id { get; set; }

            [Required]
            [StringLength(450)]
            public string UserId { get; set; } // ID da conta (IdentityUser)

            public int? PilotoId { get; set; }
            [ForeignKey("PilotoId")]
            public virtual Piloto? Piloto { get; set; }

            public int? EquipaId { get; set; }
            [ForeignKey("EquipaId")]
            public virtual Equipa? Equipa { get; set; }
        }
    }