using Trasformazioni.Models.Enums;

namespace Trasformazioni.Models.Entities
{
    /// <summary>
    /// Entità per la gestione dello scadenzario
    /// Supporta scadenze automatiche (generate dal sistema) e manuali
    /// Monitorate da background job giornaliero per notifiche e aggiornamenti stati
    /// </summary>
    public class Scadenza : BaseEntity
    {
        public Guid Id { get; set; }

        public Guid? GaraId { get; set; }
        public Guid? LottoId { get; set; }
        public Guid? PreventivoId { get; set; }

        public TipoScadenza Tipo { get; set; }
        public DateTime DataScadenza { get; set; }
        public string Descrizione { get; set; } = string.Empty;

        public bool IsAutomatica { get; set; }
        public bool IsCompletata { get; set; }
        public DateTime? DataCompletamento { get; set; }

        public int GiorniPreavviso { get; set; }
        public string? Note { get; set; }

        // ===== RELAZIONI =====
        public Gara? Gara { get; set; }
        public Lotto? Lotto { get; set; }
        public Preventivo? Preventivo { get; set; }
    }
}