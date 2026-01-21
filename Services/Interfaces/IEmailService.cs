namespace Trasformazioni.Services.Interfaces
{
    /// <summary>
    /// Interface per il servizio di invio email
    /// Supporta invio base, template HTML e integrazione con sistema Notifiche
    /// </summary>
    public interface IEmailService
    {
        // ===================================
        // INVIO EMAIL BASE
        // ===================================

        /// <summary>
        /// Invia un'email con contenuto HTML
        /// </summary>
        /// <param name="toEmail">Indirizzo email destinatario</param>
        /// <param name="subject">Oggetto dell'email</param>
        /// <param name="htmlBody">Corpo dell'email in formato HTML</param>
        /// <param name="cancellationToken">Token di cancellazione</param>
        /// <returns>Tupla (Success, ErrorMessage)</returns>
        Task<(bool Success, string? ErrorMessage)> SendEmailAsync(
            string toEmail,
            string subject,
            string htmlBody,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Invia un'email a più destinatari
        /// </summary>
        /// <param name="toEmails">Lista indirizzi email destinatari</param>
        /// <param name="subject">Oggetto dell'email</param>
        /// <param name="htmlBody">Corpo dell'email in formato HTML</param>
        /// <param name="cancellationToken">Token di cancellazione</param>
        /// <returns>Tupla (Success, ErrorMessage)</returns>
        Task<(bool Success, string? ErrorMessage)> SendEmailAsync(
            IEnumerable<string> toEmails,
            string subject,
            string htmlBody,
            CancellationToken cancellationToken = default
        );

        // ===================================
        // INVIO EMAIL CON TEMPLATE
        // ===================================

        /// <summary>
        /// Invia un'email utilizzando un template HTML
        /// </summary>
        /// <typeparam name="T">Tipo del modello dati per il template</typeparam>
        /// <param name="toEmail">Indirizzo email destinatario</param>
        /// <param name="subject">Oggetto dell'email</param>
        /// <param name="templateName">Nome del template (senza estensione .html)</param>
        /// <param name="model">Modello dati da utilizzare nel template</param>
        /// <param name="cancellationToken">Token di cancellazione</param>
        /// <returns>Tupla (Success, ErrorMessage)</returns>
        Task<(bool Success, string? ErrorMessage)> SendEmailWithTemplateAsync<T>(
            string toEmail,
            string subject,
            string templateName,
            T model,
            CancellationToken cancellationToken = default
        );

        // ===================================
        // INTEGRAZIONE CON NOTIFICHE
        // ===================================

        /// <summary>
        /// Invia un'email per una notifica esistente
        /// Recupera i dati della notifica dal database e invia l'email al destinatario
        /// Aggiorna automaticamente IsInviataEmail e DataInvioEmail
        /// </summary>
        /// <param name="notificaId">ID della notifica</param>
        /// <param name="cancellationToken">Token di cancellazione</param>
        /// <returns>Tupla (Success, ErrorMessage)</returns>
        Task<(bool Success, string? ErrorMessage)> SendNotificationEmailAsync(
            Guid notificaId,
            CancellationToken cancellationToken = default
        );

        // ===================================
        // UTILITY
        // ===================================

        /// <summary>
        /// Testa la connessione al server SMTP
        /// Utile per verificare la configurazione
        /// </summary>
        /// <param name="cancellationToken">Token di cancellazione</param>
        /// <returns>True se la connessione è riuscita, False altrimenti</returns>
        Task<(bool Success, string? ErrorMessage)> TestConnectionAsync(
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Verifica se l'invio email è abilitato nella configurazione
        /// </summary>
        /// <returns>True se EnableEmailSending = true</returns>
        bool IsEmailSendingEnabled();
    }
}