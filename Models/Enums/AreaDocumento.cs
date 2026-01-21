using System.ComponentModel.DataAnnotations;

namespace Trasformazioni.Models.Enums
{
    /// <summary>
    /// Aree dell'applicativo per la categorizzazione dei tipi documento.
    /// Ogni area rappresenta un modulo/sezione che può avere documenti allegati.
    /// </summary>
    public enum AreaDocumento
    {
        /// <summary>
        /// Documenti aziendali generali (es. visura camerale, certificazioni, ecc.)
        /// </summary>
        [Display(Name = "Azienda")]
        Azienda = 0,

        /// <summary>
        /// Documenti relativi alle gare d'appalto
        /// </summary>
        [Display(Name = "Gare")]
        Gare = 1,

        /// <summary>
        /// Documenti relativi ai lotti delle gare
        /// </summary>
        [Display(Name = "Lotti")]
        Lotti = 2,

        /// <summary>
        /// Documenti relativi ai mezzi (es. libretto, assicurazione, ecc.)
        /// </summary>
        [Display(Name = "Mezzi")]
        Mezzi = 3,

        /// <summary>
        /// Documenti relativi ai soggetti/fornitori (es. DURC, visure, ecc.)
        /// </summary>
        [Display(Name = "Soggetti")]
        Soggetti = 4,

        /// <summary>
        /// Documenti relativi alle scadenze
        /// </summary>
        [Display(Name = "Scadenze")]
        Scadenze = 5,

        /// <summary>
        /// Documenti relativi al registro contratti e preventivi
        /// </summary>
        [Display(Name = "Registro Contratti")]
        RegistroContratti = 6
    }
}
