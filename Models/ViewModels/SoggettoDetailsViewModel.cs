using Trasformazioni.Models.Enums;

namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per la visualizzazione dettagliata di un soggetto
    /// </summary>
    public class SoggettoDetailsViewModel
    {
        // ===================================
        // IDENTIFICATIVI
        // ===================================

        /// <summary>
        /// Identificatore univoco
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Codice identificativo interno
        /// </summary>
        public string? CodiceInterno { get; set; }

        // ===================================
        // CLASSIFICAZIONE
        // ===================================

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
        public string NaturaGiuridicaDescrizione => NaturaGiuridica == NaturaGiuridica.PA ? "Pubblica Amministrazione" : "Privato";

        /// <summary>
        /// Indica se è un cliente
        /// </summary>
        public bool IsCliente { get; set; }

        /// <summary>
        /// Indica se è un fornitore
        /// </summary>
        public bool IsFornitore { get; set; }

        // ===================================
        // DATI ANAGRAFICI
        // ===================================

        /// <summary>
        /// Denominazione / Ragione Sociale (per Azienda)
        /// </summary>
        public string? Denominazione { get; set; }

        /// <summary>
        /// Nome (per Persona Fisica)
        /// </summary>
        public string? Nome { get; set; }

        /// <summary>
        /// Cognome (per Persona Fisica)
        /// </summary>
        public string? Cognome { get; set; }

        /// <summary>
        /// Codice Fiscale
        /// </summary>
        public string? CodiceFiscale { get; set; }

        /// <summary>
        /// Partita IVA
        /// </summary>
        public string? PartitaIVA { get; set; }

        /// <summary>
        /// Codice SDI per fatturazione elettronica
        /// </summary>
        public string? CodiceSDI { get; set; }

        /// <summary>
        /// Codice IPA
        /// </summary>
        public string? CodiceIPA { get; set; }

        /// <summary>
        /// Nome del referente aziendale (per Aziende)
        /// </summary>
        public string? Referente { get; set; }

        // ===================================
        // CONTATTI
        // ===================================

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Telefono
        /// </summary>
        public string? Telefono { get; set; }

        /// <summary>
        /// PEC (Posta Elettronica Certificata)
        /// </summary>
        public string? PEC { get; set; }

        // ===================================
        // INDIRIZZO
        // ===================================

        /// <summary>
        /// Tipo via (es. "Via", "Piazza", "Viale")
        /// </summary>
        public string? TipoVia { get; set; }

        /// <summary>
        /// Nome della via
        /// </summary>
        public string? NomeVia { get; set; }

        /// <summary>
        /// Numero civico
        /// </summary>
        public string? NumeroCivico { get; set; }

        /// <summary>
        /// Città
        /// </summary>
        public string? Citta { get; set; }

        /// <summary>
        /// CAP
        /// </summary>
        public string? CAP { get; set; }

        /// <summary>
        /// Provincia (sigla)
        /// </summary>
        public string? Provincia { get; set; }

        /// <summary>
        /// Nazione
        /// </summary>
        public string? Nazione { get; set; }

        // ===================================
        // DATI COMMERCIALI
        // ===================================

        /// <summary>
        /// Condizioni di pagamento
        /// </summary>
        public string? CondizioniPagamento { get; set; }

        /// <summary>
        /// IBAN
        /// </summary>
        public string? IBAN { get; set; }

        /// <summary>
        /// Percentuale sconto partner (per Fornitori)
        /// </summary>
        public decimal? ScontoPartner { get; set; }

        // ===================================
        // ALTRO
        // ===================================

        /// <summary>
        /// Note
        /// </summary>
        public string? Note { get; set; }

        // ===================================
        // COMPUTED PROPERTIES
        // ===================================

        /// <summary>
        /// Nome completo (Denominazione per aziende, Nome Cognome per persone fisiche)
        /// </summary>
        public string NomeCompleto { get; set; } = string.Empty;

        /// <summary>
        /// Indirizzo completo formattato
        /// </summary>
        public string IndirizzoCompleto { get; set; } = string.Empty;

        /// <summary>
        /// Descrizione del ruolo (Cliente, Fornitore, o entrambi)
        /// </summary>
        public string RuoloDescrizione { get; set; } = string.Empty;

        // ===================================
        // AUDIT
        // ===================================

        /// <summary>
        /// Data di creazione
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Creato da (UserId)
        /// </summary>
        public string? CreatedBy { get; set; }

        /// <summary>
        /// Data ultima modifica
        /// </summary>
        public DateTime? ModifiedAt { get; set; }

        /// <summary>
        /// Modificato da (UserId)
        /// </summary>
        public string? ModifiedBy { get; set; }
    }
}