namespace Trasformazioni.Models.Entities
{
    /// <summary>
    /// Entità per censire i partecipanti ad un lotto
    /// I partecipanti sono opzionali e vengono inseriti tipicamente per lotti non vinti
    /// Se siamo noi i vincitori, è implicito (non ci inseriamo come partecipante)
    /// </summary>
    public class PartecipanteLotto : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid LottoId { get; set; }
        public Guid? SoggettoId { get; set; }

        public string RagioneSociale { get; set; } = string.Empty;
        public decimal? OffertaEconomica { get; set; }
        public bool IsAggiudicatario { get; set; }
        public bool IsScartatoDallEnte { get; set; }
        public string? Note { get; set; }

        // ===== RELAZIONI =====
        public Lotto Lotto { get; set; } = null!;
        public Soggetto? Soggetto { get; set; }
    }
}