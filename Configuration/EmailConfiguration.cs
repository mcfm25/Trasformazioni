namespace Trasformazioni.Configuration
{
    /// <summary>
    /// Configurazione per il servizio Email
    /// Mappa le impostazioni dalla sezione "Email" in appsettings.json
    /// Supporta qualsiasi provider SMTP (Gmail, SendGrid, Office365, etc.)
    /// </summary>
    public class EmailConfiguration
    {
        /// <summary>
        /// Host del server SMTP (es: "smtp.gmail.com", "smtp.office365.com")
        /// </summary>
        public string Host { get; set; } = string.Empty;

        /// <summary>
        /// Porta del server SMTP
        /// - 587: TLS/STARTTLS (raccomandato)
        /// - 465: SSL
        /// - 25: Non criptato (sconsigliato)
        /// </summary>
        public int Port { get; set; } = 587;

        /// <summary>
        /// Username per l'autenticazione SMTP (solitamente l'email)
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Password per l'autenticazione SMTP
        /// IMPORTANTE: Usa User Secrets in development e Azure Key Vault in production
        /// Per Gmail: usa "App Password" invece della password principale
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Abilita SSL/TLS per la connessione
        /// true = usa SSL/TLS (raccomandato)
        /// false = connessione non criptata (solo per test locali)
        /// </summary>
        public bool EnableSSL { get; set; } = true;

        /// <summary>
        /// Indirizzo email del mittente (es: "noreply@yourdomain.com")
        /// </summary>
        public string FromEmail { get; set; } = string.Empty;

        /// <summary>
        /// Nome visualizzato del mittente (es: "Sistema Gestione Gare")
        /// </summary>
        public string FromName { get; set; } = string.Empty;

        /// <summary>
        /// Abilita/disabilita l'invio effettivo delle email
        /// false = simula invio con logging (utile in development)
        /// true = invia email realmente (production)
        /// </summary>
        public bool EnableEmailSending { get; set; } = false;

        /// <summary>
        /// Timeout in secondi per la connessione SMTP
        /// Default: 30 secondi
        /// </summary>
        public int TimeoutSeconds { get; set; } = 30;

        /// <summary>
        /// Directory dei template email (relativa a wwwroot)
        /// Default: "email-templates"
        /// </summary>
        public string TemplateDirectory { get; set; } = "email-templates";
    }
}