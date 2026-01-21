using System.ComponentModel.DataAnnotations;
using Trasformazioni.Models.Enums;

namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per visualizzare i dettagli completi di una richiesta di integrazione
    /// Include tutte le informazioni della richiesta e le relazioni associate
    /// Utilizzato nella pagina Details
    /// </summary>
    public class RichiestaIntegrazioneDetailsViewModel
    {
        public Guid Id { get; set; }
        public Guid LottoId { get; set; }

        // ===== IDENTIFICAZIONE =====

        [Display(Name = "Numero Progressivo")]
        public int NumeroProgressivo { get; set; }

        [Display(Name = "Chiusa")]
        public bool IsChiusa { get; set; }

        // ===== RICHIESTA ENTE =====

        [Display(Name = "Data Richiesta Ente")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime DataRichiestaEnte { get; set; }

        [Display(Name = "Testo Richiesta Ente")]
        public string TestoRichiestaEnte { get; set; } = string.Empty;

        [Display(Name = "Percorso Documento Richiesta")]
        public string? DocumentoRichiestaPath { get; set; }

        [Display(Name = "Nome File Richiesta")]
        public string? NomeFileRichiesta { get; set; }

        // ===== RISPOSTA AZIENDA =====

        [Display(Name = "Data Risposta Azienda")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime? DataRispostaAzienda { get; set; }

        [Display(Name = "Testo Risposta Azienda")]
        public string? TestoRispostaAzienda { get; set; }

        [Display(Name = "Percorso Documento Risposta")]
        public string? DocumentoRispostaPath { get; set; }

        [Display(Name = "Nome File Risposta")]
        public string? NomeFileRisposta { get; set; }

        [Display(Name = "Risposto Da (User ID)")]
        public string? RispostaDaUserId { get; set; }

        [Display(Name = "Risposto Da")]
        public string? RispostoDaNome { get; set; }

        [Display(Name = "Email Utente")]
        public string? RispostoDaEmail { get; set; }

        // ===== INFO LOTTO =====

        [Display(Name = "Codice Lotto")]
        public string CodiceLotto { get; set; } = string.Empty;

        [Display(Name = "Descrizione Lotto")]
        public string DescrizioneLotto { get; set; } = string.Empty;

        [Display(Name = "Tipologia Lotto")]
        public TipologiaLotto TipologiaLotto { get; set; }

        [Display(Name = "Stato Lotto")]
        public StatoLotto StatoLotto { get; set; }

        [Display(Name = "Importo Base Asta")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal? ImportoBaseAstaLotto { get; set; }

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

        [Display(Name = "Data Termine Presentazione Offerte")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime? DataTerminePresentazioneOfferte { get; set; }

        // ===== STATISTICHE LOTTO =====

        [Display(Name = "Totale Richieste Integrazione")]
        public int TotaleRichiesteIntegrazioneLotto { get; set; }

        [Display(Name = "Richieste Aperte Lotto")]
        public int RichiesteAperteLotto { get; set; }

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

        // ===== PROPRIETÀ COMPUTATE =====

        /// <summary>
        /// Verifica se la richiesta ha una risposta
        /// </summary>
        public bool HasRisposta => DataRispostaAzienda.HasValue;

        /// <summary>
        /// Verifica se la richiesta è aperta (non chiusa)
        /// </summary>
        public bool IsAperta => !IsChiusa;

        /// <summary>
        /// Verifica se la richiesta è in attesa di risposta
        /// </summary>
        public bool IsInAttesaRisposta => !HasRisposta && !IsChiusa;

        /// <summary>
        /// Verifica se la richiesta è stata risposta ma non ancora chiusa
        /// </summary>
        public bool IsRispostaNonChiusa => HasRisposta && !IsChiusa;

        /// <summary>
        /// Verifica se la richiesta ha documento allegato (richiesta)
        /// </summary>
        public bool HasDocumentoRichiesta => !string.IsNullOrEmpty(DocumentoRichiestaPath);

        /// <summary>
        /// Verifica se la risposta ha documento allegato
        /// </summary>
        public bool HasDocumentoRisposta => !string.IsNullOrEmpty(DocumentoRispostaPath);

        /// <summary>
        /// Verifica se la richiesta è scaduta (più di 7 giorni senza risposta)
        /// </summary>
        public bool IsScaduta => IsInAttesaRisposta &&
                                 GiorniDallaRichiesta > 7;

        /// <summary>
        /// Verifica se la richiesta è urgente (più di 5 giorni senza risposta)
        /// </summary>
        public bool IsUrgente => IsInAttesaRisposta &&
                                 GiorniDallaRichiesta > 5;

        /// <summary>
        /// Indica se la richiesta può essere modificata
        /// </summary>
        public bool CanEdit => IsAperta;

        /// <summary>
        /// Indica se la richiesta può essere risposta
        /// </summary>
        public bool CanRespond => !HasRisposta && IsAperta;

        /// <summary>
        /// Indica se la risposta può essere modificata
        /// </summary>
        public bool CanEditResponse => HasRisposta && IsAperta;

        /// <summary>
        /// Indica se la richiesta può essere chiusa
        /// </summary>
        public bool CanClose => HasRisposta && IsAperta;

        /// <summary>
        /// Indica se la richiesta può essere riaperta
        /// </summary>
        public bool CanReopen => IsChiusa;

        /// <summary>
        /// Badge CSS class basato sullo stato
        /// </summary>
        public string StatoBadgeClass
        {
            get
            {
                if (IsChiusa)
                    return "badge bg-success";

                if (IsScaduta)
                    return "badge bg-danger";

                if (IsUrgente)
                    return "badge bg-warning text-dark";

                if (HasRisposta)
                    return "badge bg-info";

                return "badge bg-secondary";
            }
        }

        /// <summary>
        /// Testo dello stato
        /// </summary>
        public string StatoText
        {
            get
            {
                if (IsChiusa)
                    return "Chiusa";

                if (IsScaduta)
                    return "Scaduta";

                if (IsUrgente)
                    return "Urgente";

                if (HasRisposta)
                    return "Risposta Inviata";

                return "In Attesa";
            }
        }

        /// <summary>
        /// Giorni trascorsi dalla richiesta dell'ente
        /// </summary>
        public int GiorniDallaRichiesta
        {
            get
            {
                var diff = (DateTime.Now - DataRichiestaEnte).TotalDays;
                return (int)Math.Floor(diff);
            }
        }

        /// <summary>
        /// Giorni trascorsi dalla risposta (null se non ancora risposto)
        /// </summary>
        public int? GiorniDallaRisposta
        {
            get
            {
                if (!DataRispostaAzienda.HasValue)
                    return null;

                var diff = (DateTime.Now - DataRispostaAzienda.Value).TotalDays;
                return (int)Math.Floor(diff);
            }
        }

        /// <summary>
        /// Tempo di risposta in giorni (giorni tra richiesta e risposta)
        /// Null se non ancora risposto
        /// </summary>
        public int? TempoRispostaGiorni
        {
            get
            {
                if (!DataRispostaAzienda.HasValue)
                    return null;

                var diff = (DataRispostaAzienda.Value - DataRichiestaEnte).TotalDays;
                return (int)Math.Floor(diff);
            }
        }

        /// <summary>
        /// Tempo medio di risposta alle richieste del lotto
        /// </summary>
        public decimal? TempoMedioRispostaLotto { get; set; }

        /// <summary>
        /// Performance della risposta rispetto alla media del lotto
        /// </summary>
        public string? PerformanceRisposta
        {
            get
            {
                if (!TempoRispostaGiorni.HasValue || !TempoMedioRispostaLotto.HasValue)
                    return null;

                var tempo = TempoRispostaGiorni.Value;
                var media = TempoMedioRispostaLotto.Value;

                if (tempo <= media * 0.5m)
                    return "Eccellente (molto più veloce della media)";

                if (tempo <= media)
                    return "Buona (più veloce della media)";

                if (tempo <= media * 1.5m)
                    return "Nella media";

                if (tempo <= media * 2m)
                    return "Lenta (più lenta della media)";

                return "Molto lenta (significativamente più lenta della media)";
            }
        }

        /// <summary>
        /// CSS class per la performance
        /// </summary>
        public string? PerformanceClass
        {
            get
            {
                if (!TempoRispostaGiorni.HasValue || !TempoMedioRispostaLotto.HasValue)
                    return null;

                var tempo = TempoRispostaGiorni.Value;
                var media = TempoMedioRispostaLotto.Value;

                if (tempo <= media * 0.5m)
                    return "text-success fw-bold";

                if (tempo <= media)
                    return "text-success";

                if (tempo <= media * 1.5m)
                    return "text-info";

                if (tempo <= media * 2m)
                    return "text-warning";

                return "text-danger";
            }
        }

        /// <summary>
        /// Testo descrittivo completo del tempo di risposta
        /// </summary>
        public string TempoRispostaTestoCompleto
        {
            get
            {
                if (!HasRisposta)
                {
                    var giorni = GiorniDallaRichiesta;
                    var statoText = giorni == 0 ? "Ricevuta oggi" :
                                   giorni == 1 ? "Ricevuta 1 giorno fa" :
                                   $"Ricevuta {giorni} giorni fa";

                    if (IsScaduta)
                        return $"{statoText} - ⚠️ SCADUTA (oltre 7 giorni)";

                    if (IsUrgente)
                        return $"{statoText} - ⚡ URGENTE (oltre 5 giorni)";

                    return $"{statoText} - In attesa di risposta";
                }

                var tempoRisposta = TempoRispostaGiorni ?? 0;
                var testoRisposta = tempoRisposta == 0 ? "Risposta in giornata" :
                                   tempoRisposta == 1 ? "Risposta in 1 giorno" :
                                   $"Risposta in {tempoRisposta} giorni";

                if (IsChiusa)
                    return $"{testoRisposta} - ✓ Chiusa";

                return $"{testoRisposta} - In attesa di chiusura dall'ente";
            }
        }

        /// <summary>
        /// Titolo completo della richiesta
        /// </summary>
        public string TitoloCompleto => $"Richiesta Integrazione #{NumeroProgressivo} - Lotto {CodiceLotto}";

        /// <summary>
        /// Contesto completo
        /// </summary>
        public string ContestoCompleto => $"Gara: {CodiceGara} - {TitoloGara} | Lotto: {CodiceLotto} - {DescrizioneLotto}";
    }
}