using System.ComponentModel.DataAnnotations;

namespace Trasformazioni.Models.Enums
{
    /// <summary>
    /// Tipologie di documenti gestiti nel modulo Gare
    /// Organizzati per livello (Gara, Lotto) e fase del workflow
    /// </summary>
    public enum TipoDocumentoGara
    {
        // ===== LIVELLO GARA =====

        /// <summary>
        /// Documento generico a livello di gara
        /// </summary>
        [Display(Name = "Documento Generale")]
        DocumentoGeneraleGara,

        /// <summary>
        /// Bando originale pubblicato dall'ente
        /// </summary>
        [Display(Name = "Bando Originale")]
        BandoOriginale,

        /// <summary>
        /// Disciplinare di gara
        /// </summary>
        [Display(Name = "Disciplinare")]
        Disciplinare,

        /// <summary>
        /// Capitolato speciale d'appalto
        /// </summary>
        [Display(Name = "Capitolato Speciale")]
        CapitolatoSpeciale,

        // ===== LIVELLO LOTTO - VALUTAZIONE =====

        /// <summary>
        /// Documento relativo alla valutazione tecnica del lotto
        /// </summary>
        [Display(Name = "Documento Valutazione Tecnica")]
        DocumentoValutazioneTecnica,

        /// <summary>
        /// Documento relativo alla valutazione economica del lotto
        /// </summary>
        [Display(Name = "Documento Valutazione Economica")]
        DocumentoValutazioneEconomica,

        /// <summary>
        /// Preventivo da fornitore
        /// </summary>
        [Display(Name = "Preventivo")]
        Preventivo,

        // ===== LIVELLO LOTTO - ELABORAZIONE =====

        /// <summary>
        /// Documento di presentazione per il lotto
        /// </summary>
        [Display(Name = "Documento Presentazione")]
        DocumentoPresentazione,

        /// <summary>
        /// Offerta tecnica da presentare
        /// </summary>
        [Display(Name = "Offerta Tecnica")]
        OffertaTecnica,

        /// <summary>
        /// Offerta economica da presentare
        /// </summary>
        [Display(Name = "Offerta Economica")]
        OffertaEconomica,

        // ===== LIVELLO LOTTO - INTEGRAZIONI =====

        /// <summary>
        /// Richiesta di integrazione documentale da parte dell'ente
        /// </summary>
        [Display(Name = "Richiesta Integrazione Ente")]
        RichiestaIntegrazioneEnte,

        /// <summary>
        /// Risposta ad integrazione richiesta
        /// </summary>
        [Display(Name = "Risposta Integrazione")]
        RispostaIntegrazione,

        // ===== ALTRO =====

        /// <summary>
        /// Comunicazione generica dall'ente
        /// </summary>
        [Display(Name = "Comunicazione Ente")]
        ComunicazioneEnte,

        /// <summary>
        /// Contratto stipulato
        /// </summary>
        [Display(Name = "Contratto")]
        Contratto,

        /// <summary>
        /// Altro tipo di documento non classificato
        /// </summary>
        [Display(Name = "Altro")]
        Altro
    }
}