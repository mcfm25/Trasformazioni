using System.ComponentModel.DataAnnotations;

namespace Trasformazioni.Models.Enums
{
    /// <summary>
    /// Stati possibili per un Lotto
    /// Il workflow prevede transizioni sequenziali con possibilità di rifiuto
    /// </summary>
    public enum StatoLotto
    {
        /// <summary>
        /// Lotto in fase di creazione/modifica iniziale
        /// </summary>
        [Display(Name = "Bozza")]
        Bozza,

        /// <summary>
        /// Lotto in fase di valutazione tecnica
        /// </summary>
        [Display(Name = "In Valutazione Tecnica")]
        InValutazioneTecnica,

        /// <summary>
        /// Lotto in fase di valutazione economica (dopo approvazione tecnica)
        /// </summary>
        [Display(Name = "In Valutazione Economica")]
        InValutazioneEconomica,

        /// <summary>
        /// Lotto approvato dopo valutazioni tecnica ed economica
        /// </summary>
        [Display(Name = "Approvato")]
        Approvato,

        /// <summary>
        /// Lotto rifiutato internamente durante le valutazioni
        /// </summary>
        [Display(Name = "Rifiutato")]
        Rifiutato,

        /// <summary>
        /// Lotto in fase di elaborazione (definizione prezzi e documenti)
        /// </summary>
        [Display(Name = "In Elaborazione")]
        InElaborazione,

        /// <summary>
        /// Documentazione presentata all'ente
        /// </summary>
        [Display(Name = "Presentato")]
        Presentato,

        /// <summary>
        /// Lotto in esame da parte dell'ente (stato automatico se DataInizioEsameEnte popolata)
        /// </summary>
        [Display(Name = "In Esame")]
        InEsame,

        /// <summary>
        /// Ente ha richiesto integrazioni documentali (ping-pong attivo)
        /// </summary>
        [Display(Name = "Richiesta Integrazione")]
        RichiestaIntegrazione,

        /// <summary>
        /// Lotto aggiudicato alla nostra azienda
        /// </summary>
        [Display(Name = "Vinto")]
        Vinto,

        /// <summary>
        /// Lotto aggiudicato ad altro partecipante
        /// </summary>
        [Display(Name = "Perso")]
        Perso,

        /// <summary>
        /// Lotto scartato dall'ente (noi o altri)
        /// </summary>
        [Display(Name = "Scartato")]
        Scartato
    }
}