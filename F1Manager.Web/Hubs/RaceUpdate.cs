namespace F1Manager.Web.Hubs
{
    public class RaceUpdate
    {
        public int PosicaoFinal { get; set; }
        public string PilotoNome { get; set; } = string.Empty;
        public string EquipaNome { get; set; } = string.Empty;
        public string TempoVoltaRapida { get; set; } = string.Empty;
    }
}
