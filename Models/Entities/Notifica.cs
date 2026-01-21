using Trasformazioni.Models.Enums;

namespace Trasformazioni.Models.Entities
{
    /// <summary>
    /// Entità per il sistema di notifiche in-app ed email
    /// Gestisce notifiche per scadenze, cambi stato, richieste integrazione, etc.
    /// </summary>
    public class Notifica : BaseEntity
    {
        public Guid Id { get; set; }

        public string DestinatarioUserId { get; set; } = string.Empty;
        public TipoNotifica Tipo { get; set; }
        public string Titolo { get; set; } = string.Empty;
        public string Messaggio { get; set; } = string.Empty;
        public string? Link { get; set; }

        public bool IsLetta { get; set; }
        public DateTime? DataLettura { get; set; }

        public bool IsInviataEmail { get; set; }
        public DateTime? DataInvioEmail { get; set; }

        // ===== RIFERIMENTI OPZIONALI =====
        public Guid? GaraId { get; set; }
        public Guid? LottoId { get; set; }
        public Guid? ScadenzaId { get; set; }

        // ===== RELAZIONI =====
        public ApplicationUser Destinatario { get; set; } = null!;
        public Gara? Gara { get; set; }
        public Lotto? Lotto { get; set; }
        public Scadenza? Scadenza { get; set; }
    }
}