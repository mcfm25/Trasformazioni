using Trasformazioni.Models.Enums;

namespace Trasformazioni.Models.Entities
{
    /// <summary>
    /// Entità Lotto - rappresenta una suddivisione della gara
    /// Ogni lotto ha un proprio workflow indipendente
    /// Il cambio di stato a InEsame avviene automaticamente se DataInizioEsameEnte è valorizzata
    /// </summary>
    public class Lotto : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid GaraId { get; set; }

        // ===== IDENTIFICAZIONE =====
        public string CodiceLotto { get; set; } = string.Empty;
        public string Descrizione { get; set; } = string.Empty;
        public TipologiaLotto Tipologia { get; set; }
        public StatoLotto Stato { get; set; }

        // ===== RIFIUTO =====
        public string? MotivoRifiuto { get; set; }

        // ===== INFO GENERALI =====
        public string? LinkPiattaforma { get; set; }
        public string? OperatoreAssegnatoId { get; set; }
        public int? GiorniFornitura { get; set; }

        // ===== INFO ECONOMICHE =====
        public decimal? ImportoBaseAsta { get; set; }
        public decimal? Quotazione { get; set; }

        // ===== INFO CONTRATTO =====
        public string? DurataContratto { get; set; }
        public DateTime? DataStipulaContratto { get; set; }
        public DateTime? DataScadenzaContratto { get; set; }
        public string? Fatturazione { get; set; }

        // ===== INFO PARTECIPAZIONE =====
        public bool RichiedeFideiussione { get; set; }

        // ===== ESAME ENTE - cambio stato automatico =====
        public DateTime? DataInizioEsameEnte { get; set; }

        // ===== RELAZIONI =====
        public Gara Gara { get; set; } = null!;
        public ApplicationUser? OperatoreAssegnato { get; set; }
        public ValutazioneLotto? Valutazione { get; set; }
        public ElaborazioneLotto? Elaborazione { get; set; }
        public ICollection<Preventivo> Preventivi { get; set; } = new List<Preventivo>();
        public ICollection<RichiestaIntegrazione> RichiesteIntegrazione { get; set; } = new List<RichiestaIntegrazione>();
        public ICollection<PartecipanteLotto> Partecipanti { get; set; } = new List<PartecipanteLotto>();
        public ICollection<DocumentoGara> Documenti { get; set; } = new List<DocumentoGara>();

        /// <summary>
        /// Checklist dei tipi documento richiesti per questo lotto
        /// </summary>
        public virtual ICollection<LottoDocumentoRichiesto> DocumentiRichiesti { get; set; } = new List<LottoDocumentoRichiesto>();
    }
}