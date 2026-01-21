using System.ComponentModel.DataAnnotations;
using Trasformazioni.Models.Enums;

namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per la modifica di un soggetto esistente
    /// </summary>
    public class SoggettoEditViewModel
    {
        // ===================================
        // IDENTIFICATIVO (READ-ONLY)
        // ===================================

        /// <summary>
        /// ID del soggetto (non modificabile)
        /// </summary>
        [Required]
        public Guid Id { get; set; }

        // ===================================
        // IDENTIFICATIVI
        // ===================================

        [Display(Name = "Codice Interno")]
        [MaxLength(50, ErrorMessage = "Il Codice Interno non può superare i 50 caratteri")]
        public string? CodiceInterno { get; set; }

        // ===================================
        // CLASSIFICAZIONE
        // ===================================

        [Display(Name = "Tipo Soggetto")]
        [Required(ErrorMessage = "Il Tipo Soggetto è obbligatorio")]
        public TipoSoggetto TipoSoggetto { get; set; }

        [Display(Name = "Natura Giuridica")]
        [Required(ErrorMessage = "La Natura Giuridica è obbligatoria")]
        public NaturaGiuridica NaturaGiuridica { get; set; }

        [Display(Name = "Cliente")]
        public bool IsCliente { get; set; }

        [Display(Name = "Fornitore")]
        public bool IsFornitore { get; set; }

        // ===================================
        // DATI ANAGRAFICI
        // ===================================

        [Display(Name = "Denominazione / Ragione Sociale")]
        [MaxLength(200, ErrorMessage = "La Denominazione non può superare i 200 caratteri")]
        public string? Denominazione { get; set; }

        [Display(Name = "Nome")]
        [MaxLength(100, ErrorMessage = "Il Nome non può superare i 100 caratteri")]
        public string? Nome { get; set; }

        [Display(Name = "Cognome")]
        [MaxLength(100, ErrorMessage = "Il Cognome non può superare i 100 caratteri")]
        public string? Cognome { get; set; }

        [Display(Name = "Codice Fiscale")]
        [MaxLength(16, ErrorMessage = "Il Codice Fiscale non può superare i 16 caratteri")]
        public string? CodiceFiscale { get; set; }

        [Display(Name = "Partita IVA")]
        [MaxLength(20, ErrorMessage = "La Partita IVA non può superare i 20 caratteri")]
        public string? PartitaIVA { get; set; }

        [Display(Name = "Codice SDI")]
        [MaxLength(7, ErrorMessage = "Il Codice SDI deve essere di 7 caratteri")]
        [MinLength(7, ErrorMessage = "Il Codice SDI deve essere di 7 caratteri")]
        public string? CodiceSDI { get; set; }

        [Display(Name = "Codice IPA")]
        [MaxLength(6, ErrorMessage = "Il Codice IPA non può superare i 6 caratteri")]
        public string? CodiceIPA { get; set; }

        [Display(Name = "Referente")]
        [MaxLength(200, ErrorMessage = "Il Referente non può superare i 200 caratteri")]
        public string? Referente { get; set; }

        // ===================================
        // CONTATTI
        // ===================================

        [Display(Name = "Email")]
        [Required(ErrorMessage = "L'Email è obbligatoria")]
        [EmailAddress(ErrorMessage = "Formato Email non valido")]
        [MaxLength(100, ErrorMessage = "L'Email non può superare i 100 caratteri")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Telefono")]
        [MaxLength(20, ErrorMessage = "Il Telefono non può superare i 20 caratteri")]
        [Phone(ErrorMessage = "Formato Telefono non valido")]
        public string? Telefono { get; set; }

        [Display(Name = "PEC")]
        [EmailAddress(ErrorMessage = "Formato PEC non valido")]
        [MaxLength(100, ErrorMessage = "La PEC non può superare i 100 caratteri")]
        public string? PEC { get; set; }

        // ===================================
        // INDIRIZZO
        // ===================================

        [Display(Name = "Tipo Via")]
        [MaxLength(20, ErrorMessage = "Il Tipo Via non può superare i 20 caratteri")]
        public string? TipoVia { get; set; }

        [Display(Name = "Nome Via")]
        [MaxLength(200, ErrorMessage = "Il Nome Via non può superare i 200 caratteri")]
        public string? NomeVia { get; set; }

        [Display(Name = "Numero Civico")]
        [MaxLength(20, ErrorMessage = "Il Numero Civico non può superare i 20 caratteri")]
        public string? NumeroCivico { get; set; }

        [Display(Name = "Città")]
        [MaxLength(100, ErrorMessage = "La Città non può superare i 100 caratteri")]
        public string? Citta { get; set; }

        [Display(Name = "CAP")]
        [MaxLength(10, ErrorMessage = "Il CAP non può superare i 10 caratteri")]
        public string? CAP { get; set; }

        [Display(Name = "Provincia")]
        [MaxLength(2, ErrorMessage = "La Provincia deve essere la sigla di 2 caratteri")]
        [MinLength(2, ErrorMessage = "La Provincia deve essere la sigla di 2 caratteri")]
        public string? Provincia { get; set; }

        [Display(Name = "Nazione")]
        [MaxLength(50, ErrorMessage = "La Nazione non può superare i 50 caratteri")]
        public string? Nazione { get; set; }

        // ===================================
        // DATI COMMERCIALI
        // ===================================

        [Display(Name = "Condizioni di Pagamento")]
        [MaxLength(100, ErrorMessage = "Le Condizioni di Pagamento non possono superare i 100 caratteri")]
        public string? CondizioniPagamento { get; set; }

        [Display(Name = "IBAN")]
        [MaxLength(34, ErrorMessage = "L'IBAN non può superare i 34 caratteri")]
        public string? IBAN { get; set; }

        [Display(Name = "Sconto Partner (%)")]
        [Range(0, 100, ErrorMessage = "Lo Sconto Partner deve essere tra 0 e 100")]
        public decimal? ScontoPartner { get; set; }

        // ===================================
        // ALTRO
        // ===================================

        [Display(Name = "Note")]
        [MaxLength(1000, ErrorMessage = "Le Note non possono superare i 1000 caratteri")]
        [DataType(DataType.MultilineText)]
        public string? Note { get; set; }
    }
}