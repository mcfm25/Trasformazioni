using Trasformazioni.Models.Enums;

namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per la visualizzazione in lista dei soggetti
    /// </summary>
    public class SoggettoListViewModel
    {
        /// <summary>
        /// Identificatore univoco
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Codice identificativo interno
        /// </summary>
        public string? CodiceInterno { get; set; }

        /// <summary>
        /// Tipo soggetto (Azienda o Persona Fisica)
        /// </summary>
        public TipoSoggetto TipoSoggetto { get; set; }

        /// <summary>
        /// Descrizione tipo soggetto per display
        /// </summary>
        public string TipoSoggettoDescrizione => TipoSoggetto == TipoSoggetto.Azienda ? "Azienda" : "Persona Fisica";

        /// <summary>
        /// Natura giuridica (PA o Privato)
        /// </summary>
        public NaturaGiuridica NaturaGiuridica { get; set; }

        /// <summary>
        /// Descrizione natura giuridica per display
        /// </summary>
        public string NaturaGiuridicaDescrizione => NaturaGiuridica == NaturaGiuridica.PA ? "PA" : "Privato";

        /// <summary>
        /// Nome completo (Denominazione per aziende, Nome Cognome per persone fisiche)
        /// </summary>
        public string NomeCompleto { get; set; } = string.Empty;

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Telefono
        /// </summary>
        public string? Telefono { get; set; }

        /// <summary>
        /// Città
        /// </summary>
        public string? Citta { get; set; }

        /// <summary>
        /// Indica se è un cliente
        /// </summary>
        public bool IsCliente { get; set; }

        /// <summary>
        /// Indica se è un fornitore
        /// </summary>
        public bool IsFornitore { get; set; }

        /// <summary>
        /// Descrizione del ruolo (Cliente, Fornitore, o entrambi)
        /// </summary>
        public string RuoloDescrizione { get; set; } = string.Empty;
    }
}