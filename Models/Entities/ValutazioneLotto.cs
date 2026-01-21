namespace Trasformazioni.Models.Entities
{
    /// <summary>
    /// Entità per la valutazione tecnica ed economica di un lotto
    /// Entità unificata che gestisce entrambe le fasi di valutazione
    /// </summary>
    public class ValutazioneLotto : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid LottoId { get; set; }

        // ===== VALUTAZIONE TECNICA =====
        public DateTime? DataValutazioneTecnica { get; set; }
        public string? ValutatoreTecnicoId { get; set; }
        public bool? TecnicaApprovata { get; set; }
        public string? MotivoRifiutoTecnico { get; set; }
        public string? NoteTecniche { get; set; }

        // ===== VALUTAZIONE ECONOMICA =====
        public DateTime? DataValutazioneEconomica { get; set; }
        public string? ValutatoreEconomicoId { get; set; }
        public bool? EconomicaApprovata { get; set; }
        public string? MotivoRifiutoEconomico { get; set; }
        public string? NoteEconomiche { get; set; }

        // ===== RELAZIONI =====
        public Lotto Lotto { get; set; } = null!;
        public ApplicationUser? ValutatoreTecnico { get; set; }
        public ApplicationUser? ValutatoreEconomico { get; set; }
    }
}