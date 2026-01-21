namespace Trasformazioni.Models.Entities
{
    /// <summary>
    /// Entità per la fase di elaborazione del lotto
    /// Gestisce la definizione dei prezzi (desiderato vs reale)
    /// Dati di elaborazione (tempi e utente) sono gestiti automaticamente da BaseEntity
    /// </summary>
    public class ElaborazioneLotto : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid LottoId { get; set; }

        public decimal? PrezzoDesiderato { get; set; }
        public decimal? PrezzoRealeUscita { get; set; }
        public string? MotivazioneAdattamento { get; set; }
        public string? Note { get; set; }

        // ===== RELAZIONI =====
        public Lotto Lotto { get; set; } = null!;
    }
}