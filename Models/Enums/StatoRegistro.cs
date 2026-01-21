using System.ComponentModel.DataAnnotations;

namespace Trasformazioni.Models.Enums
{
    /// <summary>
    /// Stati possibili per un documento nel Registro Contratti
    /// Gestisce il ciclo di vita completo: dalla bozza alla scadenza/rinnovo
    /// </summary>
    public enum StatoRegistro
    {
        /// <summary>
        /// Documento in fase di redazione
        /// </summary>
        [Display(Name = "In bozza")]
        Bozza = 0,

        /// <summary>
        /// Documento completato, in attesa di revisione/approvazione
        /// </summary>
        [Display(Name = "In revisione")]
        InRevisione = 1,

        /// <summary>
        /// Documento inviato al cliente
        /// </summary>
        [Display(Name = "Inviato")]
        Inviato = 2,

        /// <summary>
        /// Contratto attivo e operativo
        /// </summary>
        [Display(Name = "Attivo")]
        Attivo = 3,

        /// <summary>
        /// Contratto in prossimità della scadenza (entro GiorniAlertScadenza)
        /// </summary>
        [Display(Name = "In scadenza")]
        InScadenza = 4,

        /// <summary>
        /// Contratto in scadenza con proposta di rinnovo inviata
        /// </summary>
        [Display(Name = "In scadenza - Proposto rinnovo")]
        InScadenzaPropostoRinnovo = 5,

        /// <summary>
        /// Contratto scaduto senza rinnovo
        /// </summary>
        [Display(Name = "Scaduto")]
        Scaduto = 6,

        /// <summary>
        /// Contratto rinnovato (sostituito da nuova versione)
        /// </summary>
        [Display(Name = "Rinnovato")]
        Rinnovato = 7,

        /// <summary>
        /// Documento annullato
        /// </summary>
        [Display(Name = "Annullato")]
        Annullato = 8,

        /// <summary>
        /// Contratto temporaneamente sospeso
        /// </summary>
        [Display(Name = "Sospeso")]
        Sospeso = 9
    }
}