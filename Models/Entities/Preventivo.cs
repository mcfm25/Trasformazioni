using Trasformazioni.Models.Enums;

namespace Trasformazioni.Models.Entities
{
    /// <summary>
    /// Entità Preventivo - gestisce i preventivi dei fornitori per i lotti
    /// Include gestione scadenze e auto-rinnovo
    /// CRITICO: i preventivi hanno scadenze (20-30 giorni) monitorate da background job
    /// </summary>
    public class Preventivo : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid LottoId { get; set; }
        public Guid SoggettoId { get; set; }

        public string Descrizione { get; set; } = string.Empty;
        public decimal? ImportoOfferto { get; set; }

        public DateTime DataRichiesta { get; set; }
        public DateTime? DataRicezione { get; set; }
        public DateTime DataScadenza { get; set; }

        public int? GiorniAutoRinnovo { get; set; }
        public StatoPreventivo Stato { get; set; }

        public string DocumentPath { get; set; } = string.Empty;
        public string NomeFile { get; set; } = string.Empty;

        public bool IsSelezionato { get; set; }
        public string? Note { get; set; }

        // ===== RELAZIONI =====
        public Lotto Lotto { get; set; } = null!;
        public Soggetto Soggetto { get; set; } = null!;
    }
}