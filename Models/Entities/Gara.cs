using Trasformazioni.Models.Enums;

namespace Trasformazioni.Models.Entities
{
    /// <summary>
    /// Entità principale per la gestione delle gare d'appalto
    /// Lo stato della gara è derivato automaticamente dagli stati dei lotti
    /// </summary>
    public class Gara : BaseEntity
    {
        public Guid Id { get; set; }

        // ===== IDENTIFICAZIONE =====
        public string CodiceGara { get; set; } = string.Empty;
        public string Titolo { get; set; } = string.Empty;
        public bool PNRR { get; set; }
        public string? Descrizione { get; set; }
        public TipologiaGara Tipologia { get; set; }
        public StatoGara Stato { get; set; }

        // ===== INFO AMMINISTRAZIONE =====
        public string? EnteAppaltante { get; set; }
        public string? Regione { get; set; }
        public string? NomePuntoOrdinante { get; set; }
        public string? TelefonoPuntoOrdinante { get; set; }

        // ===== CODICI GARA =====
        public string? CIG { get; set; }
        public string? CUP { get; set; }
        public string? RDO { get; set; }
        public string? Bando { get; set; }
        public string? DenominazioneIniziativa { get; set; }
        public string? Procedura { get; set; }
        public string? CriterioAggiudicazione { get; set; }

        // ===== DATE CRITICHE =====
        public DateTime? DataPubblicazione { get; set; }
        public DateTime? DataInizioPresentazioneOfferte { get; set; }
        public DateTime? DataTermineRichiestaChiarimenti { get; set; }
        public DateTime? DataTerminePresentazioneOfferte { get; set; }

        // ===== INFO ECONOMICHE =====
        public decimal? ImportoTotaleStimato { get; set; }

        // ===== LINK =====
        public string? LinkPiattaforma { get; set; }

        // ===== CHIUSURA MANUALE =====
        public bool IsChiusaManualmente { get; set; }
        public DateTime? DataChiusuraManuale { get; set; }
        public string? MotivoChiusuraManuale { get; set; }
        public string? ChiusaDaUserId { get; set; }

        // ===== RELAZIONI =====
        public ApplicationUser? ChiusaDa { get; set; }
        public ICollection<Lotto> Lotti { get; set; } = new List<Lotto>();
        public ICollection<DocumentoGara> Documenti { get; set; } = new List<DocumentoGara>();

        /// <summary>
        /// Checklist dei tipi documento richiesti per questa gara
        /// </summary>
        public ICollection<GaraDocumentoRichiesto> DocumentiRichiesti { get; set; } = new List<GaraDocumentoRichiesto>();
    }
}