using System.ComponentModel.DataAnnotations;
using Trasformazioni.Models.Enums;

namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per visualizzare i dettagli completi di un lotto
    /// Include tutte le informazioni del lotto e le relazioni associate
    /// Utilizzato nella pagina Details
    /// </summary>
    public class LottoDetailsViewModel
    {
        public Guid Id { get; set; }
        public Guid GaraId { get; set; }

        // ===== IDENTIFICAZIONE =====

        [Display(Name = "Codice Lotto")]
        public string CodiceLotto { get; set; } = string.Empty;

        [Display(Name = "Descrizione")]
        public string Descrizione { get; set; } = string.Empty;

        [Display(Name = "Tipologia")]
        public TipologiaLotto Tipologia { get; set; }

        [Display(Name = "Stato")]
        public StatoLotto Stato { get; set; }

        // ===== INFO GARA =====

        [Display(Name = "Codice Gara")]
        public string CodiceGara { get; set; } = string.Empty;

        [Display(Name = "Titolo Gara")]
        public string TitoloGara { get; set; } = string.Empty;

        [Display(Name = "Ente Appaltante")]
        public string? EnteAppaltante { get; set; }

        [Display(Name = "Regione")]
        public string? Regione { get; set; }

        [Display(Name = "CIG")]
        public string? CIG { get; set; }

        // ===== RIFIUTO =====

        [Display(Name = "Motivo Rifiuto")]
        public string? MotivoRifiuto { get; set; }

        // ===== INFO GENERALI =====

        [Display(Name = "Link Piattaforma")]
        public string? LinkPiattaforma { get; set; }

        [Display(Name = "Operatore Assegnato")]
        public string? OperatoreAssegnatoId { get; set; }

        [Display(Name = "Operatore Assegnato")]
        public string? OperatoreAssegnatoNome { get; set; }

        [Display(Name = "Giorni Fornitura")]
        public int? GiorniFornitura { get; set; }

        // ===== INFO ECONOMICHE =====

        [Display(Name = "Importo Base Asta")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal? ImportoBaseAsta { get; set; }

        [Display(Name = "Quotazione")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal? Quotazione { get; set; }

        // ===== INFO CONTRATTO =====

        [Display(Name = "Durata Contratto")]
        public string? DurataContratto { get; set; }

        [Display(Name = "Data Stipula Contratto")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? DataStipulaContratto { get; set; }

        [Display(Name = "Data Scadenza Contratto")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? DataScadenzaContratto { get; set; }

        [Display(Name = "Fatturazione")]
        public string? Fatturazione { get; set; }

        // ===== INFO PARTECIPAZIONE =====

        [Display(Name = "Richiede Fideiussione")]
        public bool RichiedeFideiussione { get; set; }

        // ===== ESAME ENTE =====

        [Display(Name = "Data Inizio Esame Ente")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? DataInizioEsameEnte { get; set; }

        // ===== DATE CRITICHE DALLA GARA =====

        [Display(Name = "Data Pubblicazione Gara")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? DataPubblicazioneGara { get; set; }

        [Display(Name = "Data Termine Presentazione Offerte")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime? DataTerminePresentazioneOfferte { get; set; }

        // ===== VALUTAZIONE TECNICA =====

        [Display(Name = "Data Valutazione Tecnica")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime? DataValutazioneTecnica { get; set; }

        [Display(Name = "Valutatore Tecnico")]
        public string? ValutatoreTecnicoNome { get; set; }

        [Display(Name = "Tecnica Approvata")]
        public bool? TecnicaApprovata { get; set; }

        [Display(Name = "Motivo Rifiuto Tecnico")]
        public string? MotivoRifiutoTecnico { get; set; }

        [Display(Name = "Note Tecniche")]
        public string? NoteTecniche { get; set; }

        // ===== VALUTAZIONE ECONOMICA =====

        [Display(Name = "Data Valutazione Economica")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime? DataValutazioneEconomica { get; set; }

        [Display(Name = "Valutatore Economico")]
        public string? ValutatoreEconomicoNome { get; set; }

        [Display(Name = "Economica Approvata")]
        public bool? EconomicaApprovata { get; set; }

        [Display(Name = "Motivo Rifiuto Economico")]
        public string? MotivoRifiutoEconomico { get; set; }

        [Display(Name = "Note Economiche")]
        public string? NoteEconomiche { get; set; }

        // ===== ELABORAZIONE =====

        [Display(Name = "Prezzo Desiderato")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal? PrezzoDesiderato { get; set; }

        [Display(Name = "Prezzo Reale Uscita")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal? PrezzoRealeUscita { get; set; }

        [Display(Name = "Motivazione Adattamento")]
        public string? MotivazioneAdattamento { get; set; }

        [Display(Name = "Note Elaborazione")]
        public string? NoteElaborazione { get; set; }

        [Display(Name = "Data Elaborazione")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime? DataElaborazione { get; set; }

        [Display(Name = "Elaborato Da")]
        public string? ElaboratoDa { get; set; }

        // ===== AUDIT =====

        [Display(Name = "Data Creazione")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Creato Da")]
        public string? CreatedBy { get; set; }

        [Display(Name = "Ultima Modifica")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime? ModifiedAt { get; set; }

        [Display(Name = "Modificato Da")]
        public string? ModifiedBy { get; set; }

        // ===== COLLEZIONI =====

        [Display(Name = "Preventivi")]
        public List<PreventivoListItemViewModel> Preventivi { get; set; } = new List<PreventivoListItemViewModel>();

        [Display(Name = "Richieste Integrazione")]
        public List<RichiestaIntegrazioneListItemViewModel> RichiesteIntegrazione { get; set; } = new List<RichiestaIntegrazioneListItemViewModel>();

        [Display(Name = "Partecipanti")]
        public List<PartecipanteLottoListItemViewModel> Partecipanti { get; set; } = new List<PartecipanteLottoListItemViewModel>();

        [Display(Name = "Documenti")]
        public List<DocumentoGaraListItemViewModel> Documenti { get; set; } = new List<DocumentoGaraListItemViewModel>();

        [Display(Name = "Checklist Documenti")]
        public List<ChecklistDocumentoViewModel> ChecklistDocumenti { get; set; } = new List<ChecklistDocumentoViewModel>();

        [Display(Name = "Checklist Documenti Gara")]
        public List<ChecklistDocumentoViewModel> ChecklistDocumentiGara { get; set; } = new List<ChecklistDocumentoViewModel>();

        // ===== STATISTICHE CALCOLATE =====

        [Display(Name = "Numero Preventivi")]
        public int NumeroPreventivi => Preventivi.Count;

        [Display(Name = "Preventivi Selezionati")]
        public int PreventiviSelezionati => Preventivi.Count(p => p.IsSelezionato);

        [Display(Name = "Numero Richieste Integrazione")]
        public int NumeroRichiesteIntegrazione => RichiesteIntegrazione.Count;

        [Display(Name = "Richieste Aperte")]
        public int RichiesteIntegrazioneAperte => RichiesteIntegrazione.Count(r => !r.IsChiusa);

        [Display(Name = "Numero Partecipanti")]
        public int NumeroPartecipanti => Partecipanti.Count;

        [Display(Name = "Numero Documenti")]
        public int NumeroDocumenti => Documenti.Count;

        [Display(Name = "Numero Documenti Valutazione Tecnica")]
        public int NumeroDocumentiValutazioneTecnica => Documenti.Count(d => d.TipoDocumentoCodiceRiferimento.Equals(nameof(TipoDocumentoGara.DocumentoValutazioneTecnica)));

        [Display(Name = "Numero Documenti Valutazione Economica")]
        public int NumeroDocumentiValutazioneEconomica => Documenti.Count(d => d.TipoDocumentoCodiceRiferimento.Equals(nameof(TipoDocumentoGara.DocumentoValutazioneEconomica)));

        [Display(Name = "Documenti Richiesti")]
        public int NumeroDocumentiRichiesti => ChecklistDocumenti.Count;

        [Display(Name = "Documenti Completati")]
        public int NumeroDocumentiCompletati => ChecklistDocumenti.Count(c => c.IsPresente);

        /// <summary>
        /// Percentuale di completamento checklist
        /// </summary>
        public int PercentualeCompletamentoChecklist =>
            NumeroDocumentiRichiesti > 0
                ? (int)Math.Round((decimal)NumeroDocumentiCompletati / NumeroDocumentiRichiesti * 100)
                : 100;

        /// <summary>
        /// Indica se la checklist è completa
        /// </summary>
        public bool IsChecklistCompleta => NumeroDocumentiRichiesti == 0 ||
                                            NumeroDocumentiCompletati == NumeroDocumentiRichiesti;

        // ===== STATISTICHE CHECKLIST GARA =====

        [Display(Name = "Documenti Richiesti Gara")]
        public int NumeroDocumentiRichiestiGara => ChecklistDocumentiGara.Count;

        [Display(Name = "Documenti Completati Gara")]
        public int NumeroDocumentiCompletatiGara => ChecklistDocumentiGara.Count(c => c.IsPresente);

        /// <summary>
        /// Percentuale di completamento checklist Gara
        /// </summary>
        public int PercentualeCompletamentoChecklistGara =>
            NumeroDocumentiRichiestiGara > 0
                ? (int)Math.Round((decimal)NumeroDocumentiCompletatiGara / NumeroDocumentiRichiestiGara * 100)
                : 100;

        /// <summary>
        /// Indica se la checklist Gara è completa
        /// </summary>
        public bool IsChecklistGaraCompleta => NumeroDocumentiRichiestiGara == 0 ||
                                                NumeroDocumentiCompletatiGara == NumeroDocumentiRichiestiGara;

        // ===== PROPRIETÀ COMPUTATE =====

        /// <summary>
        /// Verifica se la scadenza offerte è imminente (entro 7 giorni)
        /// </summary>
        public bool IsInScadenza => DataTerminePresentazioneOfferte.HasValue &&
                                    DataTerminePresentazioneOfferte.Value > DateTime.Now &&
                                    DataTerminePresentazioneOfferte.Value <= DateTime.Now.AddDays(7);

        /// <summary>
        /// Verifica se la scadenza offerte è passata
        /// </summary>
        public bool IsScaduta => DataTerminePresentazioneOfferte.HasValue &&
                                 DataTerminePresentazioneOfferte.Value < DateTime.Now;

        /// <summary>
        /// Indica se il lotto ha un operatore assegnato
        /// </summary>
        public bool HasOperatore => !string.IsNullOrEmpty(OperatoreAssegnatoId);

        /// <summary>
        /// Indica se il lotto è in fase attiva (non concluso)
        /// </summary>
        public bool IsAttivo => Stato != StatoLotto.Vinto &&
                                Stato != StatoLotto.Perso &&
                                Stato != StatoLotto.Scartato &&
                                Stato != StatoLotto.Rifiutato;

        /// <summary>
        /// Indica se il lotto è concluso (vinto/perso/scartato/rifiutato)
        /// </summary>
        public bool IsConcluso => !IsAttivo;

        /// <summary>
        /// Indica se il lotto è stato vinto
        /// </summary>
        public bool IsVinto => Stato == StatoLotto.Vinto;

        /// <summary>
        /// Indica se il lotto è stato rifiutato
        /// </summary>
        public bool IsRifiutato => Stato == StatoLotto.Rifiutato;

        /// <summary>
        /// Indica se ci sono richieste di integrazione aperte
        /// </summary>
        public bool HasRichiesteAperte => RichiesteIntegrazioneAperte > 0;

        /// <summary>
        /// Indica se ha valutazione tecnica
        /// </summary>
        public bool HasValutazioneTecnica => DataValutazioneTecnica.HasValue;

        /// <summary>
        /// Indica se ha valutazione economica
        /// </summary>
        public bool HasValutazioneEconomica => DataValutazioneEconomica.HasValue;

        /// <summary>
        /// Indica se ha elaborazione
        /// </summary>
        public bool HasElaborazione => DataElaborazione.HasValue;

        /// <summary>
        /// Verifica se il lotto può essere modificato
        /// </summary>
        public bool CanEdit => Stato == StatoLotto.Bozza ||
                              Stato == StatoLotto.InValutazioneTecnica ||
                              Stato == StatoLotto.InValutazioneEconomica;

        /// <summary>
        /// Verifica se il lotto può essere approvato
        /// </summary>
        public bool CanApprove => (Stato == StatoLotto.InValutazioneTecnica ||
                                  Stato == StatoLotto.InValutazioneEconomica) &&
                                  !IsRifiutato;

        /// <summary>
        /// Verifica se il lotto può essere rifiutato
        /// </summary>
        public bool CanReject => Stato == StatoLotto.InValutazioneTecnica ||
                                Stato == StatoLotto.InValutazioneEconomica;

        /// <summary>
        /// Verifica se si può aggiungere un preventivo
        /// </summary>
        public bool CanAddPreventivo => IsAttivo && Stato != StatoLotto.Bozza;

        /// <summary>
        /// Badge CSS class basato sullo stato
        /// </summary>
        public string StatoBadgeClass => Stato switch
        {
            StatoLotto.Bozza => "badge bg-secondary",
            StatoLotto.InValutazioneTecnica => "badge bg-info",
            StatoLotto.InValutazioneEconomica => "badge bg-info",
            StatoLotto.Approvato => "badge bg-success",
            StatoLotto.Rifiutato => "badge bg-danger",
            StatoLotto.InElaborazione => "badge bg-primary",
            StatoLotto.Presentato => "badge bg-warning text-dark",
            StatoLotto.InEsame => "badge bg-warning text-dark",
            StatoLotto.RichiestaIntegrazione => "badge bg-warning text-dark",
            StatoLotto.Vinto => "badge bg-success",
            StatoLotto.Perso => "badge bg-danger",
            StatoLotto.Scartato => "badge bg-dark",
            _ => "badge bg-light text-dark"
        };

        /// <summary>
        /// Giorni rimanenti alla scadenza offerte (null se non applicabile)
        /// </summary>
        public int? GiorniRimanenti
        {
            get
            {
                if (!DataTerminePresentazioneOfferte.HasValue)
                    return null;

                var diff = (DataTerminePresentazioneOfferte.Value - DateTime.Now).TotalDays;
                return diff > 0 ? (int)Math.Ceiling(diff) : 0;
            }
        }

        /// <summary>
        /// Differenza tra prezzo desiderato e prezzo reale (può essere negativo)
        /// </summary>
        public decimal? DifferenzaPrezzi
        {
            get
            {
                if (!PrezzoDesiderato.HasValue || !PrezzoRealeUscita.HasValue)
                    return null;

                return PrezzoDesiderato.Value - PrezzoRealeUscita.Value;
            }
        }

        /// <summary>
        /// Percentuale di scostamento tra prezzo desiderato e prezzo reale
        /// </summary>
        public decimal? PercentualeScostamento
        {
            get
            {
                if (!PrezzoDesiderato.HasValue || !PrezzoRealeUscita.HasValue || PrezzoDesiderato.Value == 0)
                    return null;

                return Math.Round((PrezzoRealeUscita.Value - PrezzoDesiderato.Value) / PrezzoDesiderato.Value * 100, 2);
            }
        }

        /// <summary>
        /// Giorni rimanenti alla scadenza contratto (null se non applicabile)
        /// </summary>
        public int? GiorniRimanentiContratto
        {
            get
            {
                if (!DataScadenzaContratto.HasValue)
                    return null;

                var diff = (DataScadenzaContratto.Value - DateTime.Now).TotalDays;
                return diff > 0 ? (int)Math.Ceiling(diff) : 0;
            }
        }

        /// <summary>
        /// Verifica se il contratto è in scadenza (entro 30 giorni)
        /// </summary>
        public bool IsContrattoInScadenza => GiorniRimanentiContratto.HasValue &&
                                            GiorniRimanentiContratto.Value <= 30 &&
                                            GiorniRimanentiContratto.Value > 0;
    }

    /// <summary>
    /// ViewModel per un elemento preventivo nella lista dei preventivi di un lotto
    /// Versione semplificata per visualizzazione nel lotto
    /// </summary>
    public class PreventivoListItemViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Fornitore")]
        public string NomeFornitore { get; set; } = string.Empty;

        [Display(Name = "Descrizione")]
        public string Descrizione { get; set; } = string.Empty;

        [Display(Name = "Stato")]
        public StatoPreventivo Stato { get; set; }

        [Display(Name = "Importo")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal? Importo { get; set; }

        [Display(Name = "Data Richiesta")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime DataRichiesta { get; set; }

        [Display(Name = "Data Scadenza")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? DataScadenza { get; set; }

        [Display(Name = "Selezionato")]
        public bool IsSelezionato { get; set; }
    }

    /// <summary>
    /// ViewModel per un elemento richiesta integrazione nella lista delle richieste di un lotto
    /// Versione semplificata per visualizzazione nel lotto
    /// </summary>
    public class RichiestaIntegrazioneListItemViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Numero")]
        public int NumeroProgressivo { get; set; }

        [Display(Name = "Data Richiesta Ente")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime DataRichiestaEnte { get; set; }

        [Display(Name = "Data Risposta")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime? DataRispostaAzienda { get; set; }

        [Display(Name = "Chiusa")]
        public bool IsChiusa { get; set; }

        [Display(Name = "Oggetto")]
        public string? Oggetto { get; set; }
    }

    /// <summary>
    /// ViewModel per un elemento partecipante nella lista dei partecipanti di un lotto
    /// Versione semplificata per visualizzazione nel lotto
    /// </summary>
    public class PartecipanteLottoListItemViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Nome")]
        public string Nome { get; set; } = string.Empty;

        [Display(Name = "È Nostra Azienda")]
        public bool IsNostraAzienda { get; set; }

        [Display(Name = "Classificato")]
        public int? Posizione { get; set; }

        [Display(Name = "Importo Offerto")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal? ImportoOfferto { get; set; }
    }

    /// <summary>
    /// ViewModel per un elemento documento nella lista dei documenti di un lotto
    /// Versione semplificata per visualizzazione nel lotto
    /// </summary>
    public class DocumentoGaraListItemViewModel
    {
        public Guid Id { get; set; }

        //[Display(Name = "Tipo")]
        //public TipoDocumentoGara Tipo { get; set; }

        // DOPO
        public Guid TipoDocumentoId { get; set; }

        // DOPO
        [Display(Name = "Tipo")]
        public string TipoDocumentoNome { get; set; } = string.Empty;

        /// <summary>
        /// Codice riferimento per confronti nel workflow
        /// </summary>
        public string? TipoDocumentoCodiceRiferimento { get; set; }

        [Display(Name = "Nome File")]
        public string NomeFile { get; set; } = string.Empty;

        [Display(Name = "Descrizione")]
        public string? Descrizione { get; set; }

        [Display(Name = "Data Upload")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime DataUpload { get; set; }
    }

    /// <summary>
    /// ViewModel per un elemento della checklist documenti richiesti
    /// Mostra stato di completamento (documento presente/mancante)
    /// </summary>
    public class ChecklistDocumentoViewModel
    {
        /// <summary>
        /// ID del tipo documento richiesto
        /// </summary>
        public Guid TipoDocumentoId { get; set; }

        /// <summary>
        /// Nome del tipo documento
        /// </summary>
        [Display(Name = "Tipo Documento")]
        public string TipoDocumentoNome { get; set; } = string.Empty;

        /// <summary>
        /// Codice riferimento (per eventuali logiche)
        /// </summary>
        public string? TipoDocumentoCodiceRiferimento { get; set; }

        /// <summary>
        /// Indica se esiste almeno un documento di questo tipo per il lotto
        /// </summary>
        public bool IsPresente { get; set; }

        /// <summary>
        /// Data di caricamento del documento (se presente)
        /// </summary>
        [Display(Name = "Data Caricamento")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime? DataCaricamento { get; set; }

        /// <summary>
        /// ID del documento caricato (se presente, per link a dettaglio)
        /// </summary>
        public Guid? DocumentoId { get; set; }

        /// <summary>
        /// Nome del file caricato (se presente)
        /// </summary>
        public string? NomeFile { get; set; }
    }
}