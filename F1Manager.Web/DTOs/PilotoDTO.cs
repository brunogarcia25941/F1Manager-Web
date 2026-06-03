namespace F1Manager.Web.DTOs
{
    public class PilotoDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public int NumeroCarro { get; set; }
        public string? UserId { get; set; }
        public int EquipaId { get; set; }
        public string? NomeEquipa { get; set; }
    }
}