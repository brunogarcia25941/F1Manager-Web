using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace F1Manager.Web.Models
{
    public class Piloto
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do piloto é obrigatório.")]
        public string Nome { get; set; }

        [Range(1, 99, ErrorMessage = "O número do carro deve estar entre 1 e 99.")]
        public int NumeroCarro { get; set; }

        // LIGAÇÃO AO IDENTITY (Autenticação)
        // Quando um piloto faz login, guardamos o ID da conta dele para saber que este registo pertence apenas a ele
        public string? UserId { get; set; }

        [StringLength(500, ErrorMessage = "A biografia não pode exceder os 500 caracteres.")]
        public string? Biografia { get; set; }

        [Range(40, 150, ErrorMessage = "O peso deve estar entre 40kg e 150kg.")]
        public double? Peso { get; set; }

        // Guarda o caminho relativo da imagem de perfil guardada no servidor
        public string? FotoPerfil { get; set; }

        // Chave Estrangeira (1-para-Muitos) - Equipa
        [ForeignKey("Equipa")]
        public int EquipaId { get; set; }
        public Equipa? Equipa { get; set; }

        // Navegação para a tabela associativa (Muitos-para-Muitos)
        public virtual ICollection<ResultadoCorrida> Resultados { get; set; } = new List<ResultadoCorrida>();
    }
}