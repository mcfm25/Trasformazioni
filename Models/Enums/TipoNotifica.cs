using System.ComponentModel.DataAnnotations;

namespace Trasformazioni.Models.Enums
{
    /// <summary>
    /// Tipologie di notifiche del sistema
    /// Utilizzate per classificare le notifiche in-app ed email
    /// </summary>
    public enum TipoNotifica
    {
        /// <summary>
        /// Notifica per scadenza imminente (entro GiorniPreavviso)
        /// </summary>
        [Display(Name = "Scadenza Imminente")]
        ScadenzaImminente,

        /// <summary>
        /// Notifica per preventivo scaduto
        /// </summary>
        [Display(Name = "Preventivo Scaduto")]
        PreventivoScaduto,

        /// <summary>
        /// Notifica per nuova richiesta di integrazione dall'ente
        /// </summary>
        [Display(Name = "Richiesta Integrazione")]
        RichiestaIntegrazione,

        /// <summary>
        /// Notifica per cambio di stato di un lotto
        /// </summary>
        [Display(Name = "Cambio Stato Lotto")]
        CambioStatoLotto,

        /// <summary>
        /// Notifica per nuovo documento caricato
        /// </summary>
        [Display(Name = "Nuovo Documento")]
        NuovoDocumento,

        /// <summary>
        /// Notifica informativa generica
        /// </summary>
        [Display(Name = "Informazione")]
        Info,

        /// <summary>
        /// Notifica di avviso/attenzione
        /// </summary>
        [Display(Name = "Avviso")]
        Warning,

        /// <summary>
        /// Notifica di errore
        /// </summary>
        [Display(Name = "Errore")]
        Errore
    }
}