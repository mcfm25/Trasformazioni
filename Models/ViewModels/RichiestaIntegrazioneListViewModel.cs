using System.ComponentModel.DataAnnotations;

namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per visualizzare una richiesta di integrazione nelle liste paginate
    /// Contiene solo le informazioni essenziali per la visualizzazione in elenco
    /// Utilizzato in combinazione con RichiestaIntegrazioneFilterViewModel e PagedResult
    /// </summary>
    public class RichiestaIntegrazioneListViewModel
    {
        public Guid Id { get; set; }
        public Guid LottoId { get; set; }

        // ===== IDENTIFICAZIONE =====

        [Display(Name = "Numero")]
        public int NumeroProgressivo { get; set; }

        [Display(Name = "Chiusa")]
        public bool IsChiusa { get; set; }

        // ===== RICHIESTA ENTE =====

        [Display(Name = "Data Richiesta Ente")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime DataRichiestaEnte { get; set; }

        [Display(Name = "Testo Richiesta")]
        public string TestoRichiestaEnte { get; set; } = string.Empty;

        [Display(Name = "Nome File Richiesta")]
        public string? NomeFileRichiesta { get; set; }

        [Display(Name = "Percorso Documento Richiesta")]
        public string? DocumentoRichiestaPath { get; set; }

        // ===== RISPOSTA AZIENDA =====

        [Display(Name = "Data Risposta Azienda")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime? DataRispostaAzienda { get; set; }

        [Display(Name = "Testo Risposta")]
        public string? TestoRispostaAzienda { get; set; }

        [Display(Name = "Nome File Risposta")]
        public string? NomeFileRisposta { get; set; }

        [Display(Name = "Risposto Da")]
        public string? RispostoDaNome { get; set; }

        // ===== INFO LOTTO =====

        [Display(Name = "Codice Lotto")]
        public string CodiceLotto { get; set; } = string.Empty;

        [Display(Name = "Descrizione Lotto")]
        public string DescrizioneLotto { get; set; } = string.Empty;

        [Display(Name = "Codice Gara")]
        public string CodiceGara { get; set; } = string.Empty;

        [Display(Name = "Ente Appaltante")]
        public string? EnteAppaltante { get; set; }

        // ===== AUDIT =====

        [Display(Name = "Data Creazione")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Ultima Modifica")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime? ModifiedAt { get; set; }

        // ===== PROPRIETÀ COMPUTATE (per UI) =====

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
        public bool HasDocumentoRisposta => !string.IsNullOrEmpty(NomeFileRisposta);

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
        /// Icona FontAwesome basata sullo stato
        /// </summary>
        public string StatoIcon
        {
            get
            {
                if (IsChiusa)
                    return "fa-solid fa-check-circle";

                if (IsScaduta)
                    return "fa-solid fa-times-circle";

                if (IsUrgente)
                    return "fa-solid fa-exclamation-triangle";

                if (HasRisposta)
                    return "fa-solid fa-paper-plane";

                return "fa-solid fa-clock";
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
        /// Testo descrittivo del tempo di risposta
        /// </summary>
        public string TempoRispostaText
        {
            get
            {
                if (!HasRisposta)
                {
                    var giorni = GiorniDallaRichiesta;
                    return giorni == 0 ? "Ricevuta oggi - non ancora risposta" :
                           giorni == 1 ? "1 giorno fa - non ancora risposta" :
                           $"{giorni} giorni fa - non ancora risposta";
                }

                var tempoRisposta = TempoRispostaGiorni ?? 0;
                return tempoRisposta == 0 ? "Risposta in giornata" :
                       tempoRisposta == 1 ? "Risposta in 1 giorno" :
                       $"Risposta in {tempoRisposta} giorni";
            }
        }

        /// <summary>
        /// CSS class per il tempo di risposta
        /// </summary>
        public string TempoRispostaClass
        {
            get
            {
                if (!HasRisposta)
                {
                    if (IsScaduta)
                        return "text-danger fw-bold";

                    if (IsUrgente)
                        return "text-warning fw-bold";

                    return "text-muted";
                }

                var tempoRisposta = TempoRispostaGiorni ?? 0;

                return tempoRisposta <= 1 ? "text-success fw-bold" :
                       tempoRisposta <= 3 ? "text-success" :
                       tempoRisposta <= 7 ? "text-info" :
                       "text-warning";
            }
        }

        /// <summary>
        /// Icona per il documento richiesta
        /// </summary>
        public string DocumentoRichiestaIcon => HasDocumentoRichiesta
            ? "fa-solid fa-file-pdf text-danger"
            : "fa-solid fa-file-circle-xmark text-muted";

        /// <summary>
        /// Tooltip per il documento richiesta
        /// </summary>
        public string DocumentoRichiestaTooltip => HasDocumentoRichiesta
            ? $"File: {NomeFileRichiesta}"
            : "Nessun documento allegato";

        /// <summary>
        /// Icona per il documento risposta
        /// </summary>
        public string DocumentoRispostaIcon => HasDocumentoRisposta
            ? "fa-solid fa-file-pdf text-success"
            : "fa-solid fa-file-circle-xmark text-muted";

        /// <summary>
        /// Tooltip per il documento risposta
        /// </summary>
        public string DocumentoRispostaTooltip => HasDocumentoRisposta
            ? $"File: {NomeFileRisposta}"
            : "Nessun documento di risposta";

        /// <summary>
        /// Breve estratto del testo richiesta (primi 100 caratteri)
        /// </summary>
        public string TestoRichiestaBreve
        {
            get
            {
                if (string.IsNullOrEmpty(TestoRichiestaEnte))
                    return string.Empty;

                return TestoRichiestaEnte.Length <= 100
                    ? TestoRichiestaEnte
                    : TestoRichiestaEnte.Substring(0, 100) + "...";
            }
        }

        /// <summary>
        /// Titolo completo per tooltip o riferimenti
        /// </summary>
        public string TitoloCompleto => $"Richiesta #{NumeroProgressivo} - Lotto {CodiceLotto}";
    }
}