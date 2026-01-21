using System.ComponentModel.DataAnnotations;
using Trasformazioni.Models.Enums;

namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per la visualizzazione dettagliata di un Partecipante Lotto
    /// Include tutte le proprietà, relazioni complete e audit trail
    /// </summary>
    public class PartecipanteLottoDetailsViewModel
    {
        public Guid Id { get; set; }
        public Guid LottoId { get; set; }
        public Guid? SoggettoId { get; set; }

        // ===== DATI PARTECIPANTE =====
        [Display(Name = "Ragione Sociale")]
        public string RagioneSociale { get; set; } = string.Empty;

        [Display(Name = "Offerta Economica")]
        [DisplayFormat(DataFormatString = "{0:C2}", NullDisplayText = "Non specificata")]
        public decimal? OffertaEconomica { get; set; }

        [Display(Name = "Note")]
        public string? Note { get; set; }

        // ===== FLAGS STATO =====
        [Display(Name = "Aggiudicatario")]
        public bool IsAggiudicatario { get; set; }

        [Display(Name = "Scartato dall'Ente")]
        public bool IsScartatoDallEnte { get; set; }

        /// <summary>
        /// Indica se il partecipante è collegato a un Soggetto esistente
        /// </summary>
        [Display(Name = "Soggetto Censito")]
        public bool HasSoggetto { get; set; }

        // ===== STATO CALCOLATO =====
        /// <summary>
        /// Stato del partecipante calcolato in base ai flag
        /// Valori possibili: "Aggiudicatario", "Scartato", "Partecipante"
        /// </summary>
        [Display(Name = "Stato Partecipante")]
        public string StatoPartecipante { get; set; } = string.Empty;

        // ===== INFO LOTTO =====
        [Display(Name = "Codice Lotto")]
        public string CodiceLotto { get; set; } = string.Empty;

        [Display(Name = "Descrizione Lotto")]
        public string DescrizioneLotto { get; set; } = string.Empty;

        [Display(Name = "Tipologia Lotto")]
        public TipologiaLotto? TipologiaLotto { get; set; }

        [Display(Name = "Stato Lotto")]
        public StatoLotto? StatoLotto { get; set; }

        [Display(Name = "Importo Base d'Asta")]
        [DisplayFormat(DataFormatString = "{0:C2}", NullDisplayText = "Non definito")]
        public decimal? ImportoBaseAstaLotto { get; set; }

        // ===== INFO GARA =====
        [Display(Name = "Codice Gara")]
        public string CodiceGara { get; set; } = string.Empty;

        [Display(Name = "Titolo Gara")]
        public string TitoloGara { get; set; } = string.Empty;

        [Display(Name = "Tipologia Gara")]
        public TipologiaGara? TipologiaGara { get; set; }

        [Display(Name = "Stato Gara")]
        public StatoGara? StatoGara { get; set; }

        [Display(Name = "Ente Appaltante")]
        public string? EnteAppaltante { get; set; }

        [Display(Name = "Regione")]
        public string? Regione { get; set; }

        [Display(Name = "Data Termine Presentazione Offerte")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", NullDisplayText = "Non definita")]
        public DateTime? DataTerminePresentazioneOfferte { get; set; }

        // ===== INFO SOGGETTO (se collegato) =====
        /// <summary>
        /// Tipo soggetto (Azienda / Persona Fisica)
        /// </summary>
        [Display(Name = "Tipo Soggetto")]
        public TipoSoggetto? TipoSoggetto { get; set; }

        /// <summary>
        /// Denominazione (per aziende) o Nome Completo (per persone fisiche)
        /// </summary>
        [Display(Name = "Denominazione/Nome")]
        public string? DenominazioneSoggetto { get; set; }

        [Display(Name = "Codice Fiscale / P.IVA")]
        public string? CodiceFiscaleSoggetto { get; set; }

        [Display(Name = "Email Soggetto")]
        public string? EmailSoggetto { get; set; }

        [Display(Name = "Telefono Soggetto")]
        public string? TelefonoSoggetto { get; set; }

        [Display(Name = "Città Soggetto")]
        public string? CittaSoggetto { get; set; }

        [Display(Name = "Provincia Soggetto")]
        public string? ProvinciaSoggetto { get; set; }

        // ===== AUDIT - CREAZIONE =====
        [Display(Name = "Data Censimento")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Censito Da")]
        public string CreatedBy { get; set; } = string.Empty;

        // ===== AUDIT - MODIFICA =====
        [Display(Name = "Ultima Modifica")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", NullDisplayText = "Mai modificato")]
        public DateTime? ModifiedAt { get; set; }

        [Display(Name = "Modificato Da")]
        public string? ModifiedBy { get; set; }
    }
}