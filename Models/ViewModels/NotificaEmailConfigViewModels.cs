using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Trasformazioni.Models.Enums;

namespace Trasformazioni.Models.ViewModels
{
    // ===================================
    // CONFIGURAZIONE - LIST
    // ===================================

    public class ConfigurazioneNotificaEmailListViewModel
    {
        public Guid Id { get; set; }
        public string Codice { get; set; } = string.Empty;
        public string Descrizione { get; set; } = string.Empty;
        public string Modulo { get; set; } = string.Empty;
        public bool IsAttiva { get; set; }
        public int DestinatariCount { get; set; }
        public string? OggettoEmailDefault { get; set; }
    }

    // ===================================
    // CONFIGURAZIONE - DETAILS
    // ===================================

    public class ConfigurazioneNotificaEmailDetailsViewModel
    {
        public Guid Id { get; set; }
        public string Codice { get; set; } = string.Empty;
        public string Descrizione { get; set; } = string.Empty;
        public string Modulo { get; set; } = string.Empty;
        public bool IsAttiva { get; set; }
        public string? OggettoEmailDefault { get; set; }
        public string? Note { get; set; }

        public List<DestinatarioNotificaEmailViewModel> Destinatari { get; set; } = new();

        // Audit
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? ModifiedAt { get; set; }
        public string? ModifiedBy { get; set; }

        // Helper
        public int DestinatariCount => Destinatari.Count;
    }

    // ===================================
    // CONFIGURAZIONE - EDIT
    // ===================================

    public class ConfigurazioneNotificaEmailEditViewModel
    {
        public Guid Id { get; set; }

        public string Codice { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descrizione è obbligatoria")]
        [StringLength(255, ErrorMessage = "La descrizione non può superare i 255 caratteri")]
        [Display(Name = "Descrizione")]
        public string Descrizione { get; set; } = string.Empty;

        public string Modulo { get; set; } = string.Empty;

        [Display(Name = "Attiva")]
        public bool IsAttiva { get; set; }

        [StringLength(500, ErrorMessage = "L'oggetto email non può superare i 500 caratteri")]
        [Display(Name = "Oggetto Email Default")]
        public string? OggettoEmailDefault { get; set; }

        [StringLength(1000, ErrorMessage = "Le note non possono superare i 1000 caratteri")]
        [Display(Name = "Note")]
        public string? Note { get; set; }
    }

    // ===================================
    // DESTINATARIO - VIEW
    // ===================================

    public class DestinatarioNotificaEmailViewModel
    {
        public Guid Id { get; set; }
        public TipoDestinatarioNotifica Tipo { get; set; }
        public string TipoDescrizione => Tipo switch
        {
            TipoDestinatarioNotifica.Reparto => "Reparto",
            TipoDestinatarioNotifica.Ruolo => "Ruolo",
            TipoDestinatarioNotifica.Utente => "Utente",
            _ => "N/D"
        };

        // Dati risolti
        public string? RepartoNome { get; set; }
        public string? RepartoEmail { get; set; }
        public string? Ruolo { get; set; }
        public string? UtenteNome { get; set; }
        public string? UtenteEmail { get; set; }

        public string? Note { get; set; }
        public int Ordine { get; set; }

        /// <summary>
        /// Descrizione leggibile per l'UI
        /// </summary>
        public string Descrizione => Tipo switch
        {
            TipoDestinatarioNotifica.Reparto => $"{RepartoNome ?? "N/D"} ({RepartoEmail ?? "N/D"})",
            TipoDestinatarioNotifica.Ruolo => Ruolo ?? "N/D",
            TipoDestinatarioNotifica.Utente => $"{UtenteNome ?? "N/D"} ({UtenteEmail ?? "N/D"})",
            _ => "N/D"
        };

        /// <summary>
        /// Icona Bootstrap per il tipo
        /// </summary>
        public string Icona => Tipo switch
        {
            TipoDestinatarioNotifica.Reparto => "bi-building",
            TipoDestinatarioNotifica.Ruolo => "bi-shield",
            TipoDestinatarioNotifica.Utente => "bi-person",
            _ => "bi-question"
        };

        /// <summary>
        /// Colore badge per il tipo
        /// </summary>
        public string BadgeClass => Tipo switch
        {
            TipoDestinatarioNotifica.Reparto => "bg-info",
            TipoDestinatarioNotifica.Ruolo => "bg-warning text-dark",
            TipoDestinatarioNotifica.Utente => "bg-primary",
            _ => "bg-secondary"
        };
    }

    // ===================================
    // DESTINATARIO - CREATE
    // ===================================

    public class DestinatarioNotificaEmailCreateViewModel
    {
        [Required]
        public Guid ConfigurazioneNotificaEmailId { get; set; }

        [Required(ErrorMessage = "Seleziona il tipo di destinatario")]
        [Display(Name = "Tipo")]
        public TipoDestinatarioNotifica Tipo { get; set; }

        [Display(Name = "Reparto")]
        public Guid? RepartoId { get; set; }

        [Display(Name = "Ruolo")]
        public string? Ruolo { get; set; }

        [Display(Name = "Utente")]
        public string? UtenteId { get; set; }

        [StringLength(500, ErrorMessage = "Le note non possono superare i 500 caratteri")]
        [Display(Name = "Note")]
        public string? Note { get; set; }

        // Dropdown
        public SelectList? RepartiSelectList { get; set; }
        public SelectList? RuoliSelectList { get; set; }
        public SelectList? UtentiSelectList { get; set; }
    }

    // ===================================
    // INDEX VIEW MODEL
    // ===================================

    public class NotificheConfigIndexViewModel
    {
        public Dictionary<string, List<ConfigurazioneNotificaEmailListViewModel>> ConfigurazioniPerModulo { get; set; } = new();

        public int TotaleConfigurazioni => ConfigurazioniPerModulo.Values.Sum(l => l.Count);
        public int TotaleAttive => ConfigurazioniPerModulo.Values.Sum(l => l.Count(c => c.IsAttiva));
        public int TotaleConDestinatari => ConfigurazioniPerModulo.Values.Sum(l => l.Count(c => c.DestinatariCount > 0));
    }
}