using System.ComponentModel.DataAnnotations;
using Trasformazioni.Models.Enums;

namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per visualizzare i dettagli completi di una gara
    /// Include tutte le informazioni della gara e i lotti associati
    /// Utilizzato nella pagina Details
    /// </summary>
    public class GaraDetailsViewModel
    {
        public Guid Id { get; set; }

        // ===== IDENTIFICAZIONE =====

        [Display(Name = "Codice Gara")]
        public string CodiceGara { get; set; } = string.Empty;

        [Display(Name = "Titolo")]
        public string Titolo { get; set; } = string.Empty;

        [Display(Name = "PNRR")]
        public bool PNRR { get; set; }

        [Display(Name = "Descrizione")]
        public string? Descrizione { get; set; }

        [Display(Name = "Tipologia")]
        public TipologiaGara Tipologia { get; set; }

        [Display(Name = "Stato")]
        public StatoGara Stato { get; set; }

        // ===== INFO AMMINISTRAZIONE =====

        [Display(Name = "Ente Appaltante")]
        public string? EnteAppaltante { get; set; }

        [Display(Name = "Regione")]
        public string? Regione { get; set; }

        [Display(Name = "Nome Punto Ordinante")]
        public string? NomePuntoOrdinante { get; set; }

        [Display(Name = "Telefono Punto Ordinante")]
        public string? TelefonoPuntoOrdinante { get; set; }

        // ===== CODICI GARA =====

        [Display(Name = "CIG")]
        public string? CIG { get; set; }

        [Display(Name = "CUP")]
        public string? CUP { get; set; }

        [Display(Name = "RDO")]
        public string? RDO { get; set; }

        [Display(Name = "Bando")]
        public string? Bando { get; set; }

        [Display(Name = "Denominazione Iniziativa")]
        public string? DenominazioneIniziativa { get; set; }

        [Display(Name = "Procedura")]
        public string? Procedura { get; set; }

        [Display(Name = "Criterio Aggiudicazione")]
        public string? CriterioAggiudicazione { get; set; }

        // ===== DATE CRITICHE =====

        [Display(Name = "Data Pubblicazione")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? DataPubblicazione { get; set; }

        [Display(Name = "Data Inizio Presentazione Offerte")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime? DataInizioPresentazioneOfferte { get; set; }

        [Display(Name = "Data Termine Richiesta Chiarimenti")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime? DataTermineRichiestaChiarimenti { get; set; }

        [Display(Name = "Data Termine Presentazione Offerte")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime? DataTerminePresentazioneOfferte { get; set; }

        // ===== INFO ECONOMICHE =====

        [Display(Name = "Importo Totale Stimato")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal? ImportoTotaleStimato { get; set; }

        // ===== LINK =====

        [Display(Name = "Link Piattaforma")]
        public string? LinkPiattaforma { get; set; }

        // ===== CHIUSURA MANUALE =====

        [Display(Name = "Chiusa Manualmente")]
        public bool IsChiusaManualmente { get; set; }

        [Display(Name = "Data Chiusura Manuale")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime? DataChiusuraManuale { get; set; }

        [Display(Name = "Motivo Chiusura Manuale")]
        public string? MotivoChiusuraManuale { get; set; }

        [Display(Name = "Chiusa Da")]
        public string? ChiusaDaNomeCompleto { get; set; }

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

        [Display(Name = "Documenti")]
        public List<DocumentoGaraListItemViewModel> Documenti { get; set; } = new List<DocumentoGaraListItemViewModel>();

        // ===== CHECKLIST DOCUMENTI RICHIESTI =====

        [Display(Name = "Checklist Documenti")]
        public List<ChecklistDocumentoViewModel> ChecklistDocumenti { get; set; } = new();

        [Display(Name = "Documenti Richiesti")]
        public int NumeroDocumentiRichiesti => ChecklistDocumenti.Count;

        [Display(Name = "Documenti Completati")]
        public int NumeroDocumentiCompletati => ChecklistDocumenti.Count(c => c.IsPresente);

        [Display(Name = "Completamento Checklist (%)")]
        public decimal PercentualeCompletamentoChecklist => NumeroDocumentiRichiesti > 0
            ? Math.Round((decimal)NumeroDocumentiCompletati / NumeroDocumentiRichiesti * 100, 0)
            : 0;

        [Display(Name = "Checklist Completa")]
        public bool IsChecklistCompleta => NumeroDocumentiRichiesti > 0 && NumeroDocumentiCompletati == NumeroDocumentiRichiesti;

        // ===== LOTTI ASSOCIATI =====

        [Display(Name = "Lotti")]
        public List<LottoListItemViewModel> Lotti { get; set; } = new List<LottoListItemViewModel>();

        // ===== DOCUMENTI =====

        [Display(Name = "Numero Documenti")]
        //public int NumeroDocumenti { get; set; }
        public int NumeroDocumenti => Documenti.Count();

        // ===== STATISTICHE CALCOLATE =====

        [Display(Name = "Numero Lotti")]
        public int NumeroLotti => Lotti.Count;

        [Display(Name = "Lotti in Bozza")]
        public int LottiInBozza => Lotti.Count(l => l.Stato == StatoLotto.Bozza);

        [Display(Name = "Lotti in Valutazione")]
        public int LottiInValutazione => Lotti.Count(l =>
            l.Stato == StatoLotto.InValutazioneTecnica ||
            l.Stato == StatoLotto.InValutazioneEconomica);

        [Display(Name = "Lotti Approvati")]
        public int LottiApprovati => Lotti.Count(l => l.Stato == StatoLotto.Approvato);

        [Display(Name = "Lotti Rifiutati")]
        public int LottiRifiutati => Lotti.Count(l => l.Stato == StatoLotto.Rifiutato);

        [Display(Name = "Lotti in Elaborazione")]
        public int LottiInElaborazione => Lotti.Count(l => l.Stato == StatoLotto.InElaborazione);

        [Display(Name = "Lotti Presentati")]
        public int LottiPresentati => Lotti.Count(l => l.Stato == StatoLotto.Presentato);

        [Display(Name = "Lotti in Esame")]
        public int LottiInEsame => Lotti.Count(l => l.Stato == StatoLotto.InEsame);

        [Display(Name = "Lotti con Richiesta Integrazione")]
        public int LottiConRichiestaIntegrazione => Lotti.Count(l => l.Stato == StatoLotto.RichiestaIntegrazione);

        [Display(Name = "Lotti Vinti")]
        public int LottiVinti => Lotti.Count(l => l.Stato == StatoLotto.Vinto);

        [Display(Name = "Lotti Persi")]
        public int LottiPersi => Lotti.Count(l => l.Stato == StatoLotto.Perso);

        [Display(Name = "Lotti Scartati")]
        public int LottiScartati => Lotti.Count(l => l.Stato == StatoLotto.Scartato);

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
        /// Lotti attivi (in lavorazione, non conclusi)
        /// </summary>
        public int LottiAttivi => NumeroLotti - (LottiVinti + LottiPersi + LottiScartati + LottiRifiutati);

        /// <summary>
        /// Lotti conclusi (vinti + persi + scartati)
        /// </summary>
        public int LottiConclusi => LottiVinti + LottiPersi + LottiScartati;

        /// <summary>
        /// Percentuale di successo (lotti vinti / totale lotti conclusi)
        /// Restituisce null se non ci sono lotti conclusi
        /// </summary>
        public decimal? PercentualeSuccesso
        {
            get
            {
                if (LottiConclusi == 0)
                    return null;

                return Math.Round((decimal)LottiVinti / LottiConclusi * 100, 1);
            }
        }

        /// <summary>
        /// Importo totale dei lotti vinti
        /// </summary>
        public decimal ImportoLottiVinti => Lotti
            .Where(l => l.Stato == StatoLotto.Vinto && l.ImportoBaseAsta.HasValue)
            .Sum(l => l.ImportoBaseAsta!.Value);

        /// <summary>
        /// Badge CSS class basato sullo stato
        /// </summary>
        public string StatoBadgeClass => Stato switch
        {
            StatoGara.Bozza => "badge bg-secondary",
            StatoGara.InLavorazione => "badge bg-primary",
            StatoGara.Conclusa => "badge bg-success",
            StatoGara.ChiusaManualmente => "badge bg-dark",
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
        /// Verifica se la gara può essere modificata (non conclusa/chiusa)
        /// </summary>
        public bool CanEdit => Stato != StatoGara.Conclusa && !IsChiusaManualmente;

        /// <summary>
        /// Verifica se la gara può essere chiusa manualmente
        /// </summary>
        public bool CanClose => !IsChiusaManualmente;

        /// <summary>
        /// Verifica se ci sono lotti in lavorazione
        /// </summary>
        public bool HasLottiInLavorazione => LottiAttivi > 0;
    }

    /// <summary>
    /// ViewModel per un elemento lotto nella lista dei lotti di una gara
    /// Versione semplificata per visualizzazione nella gara
    /// </summary>
    public class LottoListItemViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Codice Lotto")]
        public string CodiceLotto { get; set; } = string.Empty;

        [Display(Name = "Descrizione")]
        public string Descrizione { get; set; } = string.Empty;

        [Display(Name = "Tipologia")]
        public TipologiaLotto Tipologia { get; set; }

        [Display(Name = "Stato")]
        public StatoLotto Stato { get; set; }

        [Display(Name = "Importo Base Asta")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal? ImportoBaseAsta { get; set; }

        [Display(Name = "Operatore Assegnato")]
        public string? OperatoreAssegnatoNome { get; set; }

        [Display(Name = "Quotazione")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal? Quotazione { get; set; }

        [Display(Name = "Numero Preventivi")]
        public int NumeroPreventivi { get; set; }

        [Display(Name = "Numero Richieste Integrazione")]
        public int NumeroRichiesteIntegrazione { get; set; }

        /// <summary>
        /// Badge CSS class basato sullo stato del lotto
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
        /// Icona FontAwesome basata sullo stato
        /// </summary>
        public string StatoIcon => Stato switch
        {
            StatoLotto.Bozza => "fa-solid fa-file",
            StatoLotto.InValutazioneTecnica => "fa-solid fa-magnifying-glass",
            StatoLotto.InValutazioneEconomica => "fa-solid fa-euro-sign",
            StatoLotto.Approvato => "fa-solid fa-check-circle",
            StatoLotto.Rifiutato => "fa-solid fa-times-circle",
            StatoLotto.InElaborazione => "fa-solid fa-edit",
            StatoLotto.Presentato => "fa-solid fa-paper-plane",
            StatoLotto.InEsame => "fa-solid fa-hourglass-half",
            StatoLotto.RichiestaIntegrazione => "fa-solid fa-exclamation-triangle",
            StatoLotto.Vinto => "fa-solid fa-trophy",
            StatoLotto.Perso => "fa-solid fa-thumbs-down",
            StatoLotto.Scartato => "fa-solid fa-ban",
            _ => "fa-solid fa-question"
        };
    }
}