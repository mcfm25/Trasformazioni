namespace Trasformazioni.Models.Entities
{
    /// <summary>
    /// Entità per gestire il ping-pong di richieste/risposte integrazioni con l'ente
    /// Le richieste possono essere parallele (non necessariamente sequenziali)
    /// </summary>
    public class RichiestaIntegrazione : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid LottoId { get; set; }

        public int NumeroProgressivo { get; set; }

        // ===== RICHIESTA ENTE =====
        public DateTime DataRichiestaEnte { get; set; }
        public string TestoRichiestaEnte { get; set; } = string.Empty;
        public string? DocumentoRichiestaPath { get; set; }
        public string? NomeFileRichiesta { get; set; }

        // ===== RISPOSTA AZIENDA =====
        public DateTime? DataRispostaAzienda { get; set; }
        public string? TestoRispostaAzienda { get; set; }
        public string? DocumentoRispostaPath { get; set; }
        public string? NomeFileRisposta { get; set; }
        public string? RispostaDaUserId { get; set; }

        public bool IsChiusa { get; set; }

        // ===== RELAZIONI =====
        public Lotto Lotto { get; set; } = null!;
        public ApplicationUser? RispostaDa { get; set; }
    }
}