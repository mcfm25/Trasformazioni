using System.ComponentModel.DataAnnotations;

namespace Trasformazioni.Models.Enums
{
    /// <summary>
    /// Tipologie di gara d'appalto
    /// </summary>
    public enum TipologiaGara
    {
        /// <summary>
        /// Appalto pubblico tradizionale
        /// </summary>
        [Display(Name = "Appalto Pubblico")]
        AppaltoPubblico,

        /// <summary>
        /// Richiesta di Offerta
        /// </summary>
        [Display(Name = "RDO - Richiesta di Offerta")]
        RDO,

        /// <summary>
        /// Gara telematica su piattaforma digitale
        /// </summary>
        [Display(Name = "Gara Telematica")]
        GaraTelematica,

        /// <summary>
        /// Accordo quadro con più fornitori
        /// </summary>
        [Display(Name = "Accordo Quadro")]
        AccordoQuadro,

        /// <summary>
        /// Sistema dinamico di acquisizione
        /// </summary>
        [Display(Name = "Sistema Dinamico")]
        SistemaDinamico,

        /// <summary>
        /// Procedura negoziata (trattativa privata)
        /// </summary>
        [Display(Name = "Procedura Negoziata")]
        ProceduraNegoziata,

        /// <summary>
        /// Altra tipologia non classificata
        /// </summary>
        [Display(Name = "Altra")]
        Altra
    }
}