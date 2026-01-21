using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MimeKit;
using Trasformazioni.Configuration;
using Trasformazioni.Data;
using Trasformazioni.Services.Interfaces;

namespace Trasformazioni.Services
{
    /// <summary>
    /// Implementazione del servizio Email utilizzando MailKit
    /// Supporta SMTP con qualsiasi provider, template HTML e integrazione con Notifiche
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly EmailConfiguration _config;
        private readonly EmailTemplateService _templateService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IOptions<EmailConfiguration> config,
            EmailTemplateService templateService,
            ApplicationDbContext context,
            ILogger<EmailService> logger)
        {
            _config = config.Value;
            _templateService = templateService;
            _context = context;
            _logger = logger;
        }

        // ===================================
        // INVIO EMAIL BASE
        // ===================================

        public async Task<(bool Success, string? ErrorMessage)> SendEmailAsync(
            string toEmail,
            string subject,
            string htmlBody,
            CancellationToken cancellationToken = default)
        {
            return await SendEmailAsync(new[] { toEmail }, subject, htmlBody, cancellationToken);
        }

        public async Task<(bool Success, string? ErrorMessage)> SendEmailAsync(
            IEnumerable<string> toEmails,
            string subject,
            string htmlBody,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Validazione input
                var emailList = toEmails.Where(e => !string.IsNullOrWhiteSpace(e)).ToList();
                if (!emailList.Any())
                {
                    _logger.LogWarning("Tentativo di invio email senza destinatari validi");
                    return (false, "Nessun destinatario valido specificato");
                }

                // Check se invio email è abilitato
                if (!_config.EnableEmailSending)
                {
                    _logger.LogInformation(
                        "Invio email disabilitato. Email simulata per: {Recipients}, Subject: {Subject}",
                        string.Join(", ", emailList),
                        subject
                    );
                    return (true, null); // Simula successo in development
                }

                _logger.LogInformation(
                    "Invio email in corso - To: {Recipients}, Subject: {Subject}",
                    string.Join(", ", emailList),
                    subject
                );

                // Crea il messaggio email
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_config.FromName, _config.FromEmail));

                // Aggiungi destinatari
                foreach (var email in emailList)
                {
                    try
                    {
                        message.To.Add(MailboxAddress.Parse(email));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Indirizzo email non valido: {Email}", email);
                    }
                }

                if (!message.To.Any())
                {
                    return (false, "Nessun indirizzo email valido tra i destinatari");
                }

                message.Subject = subject;

                // Crea il corpo dell'email (HTML)
                var builder = new BodyBuilder
                {
                    HtmlBody = htmlBody
                };
                message.Body = builder.ToMessageBody();

                // Invia l'email tramite SMTP
                using var client = new SmtpClient();

                // Configura timeout
                client.Timeout = _config.TimeoutSeconds * 1000;

                // Connetti al server SMTP
                await client.ConnectAsync(
                    _config.Host,
                    _config.Port,
                    _config.EnableSSL ? SecureSocketOptions.StartTls : SecureSocketOptions.None,
                    cancellationToken
                );

                // Autenticazione
                if (!string.IsNullOrWhiteSpace(_config.Username) && !string.IsNullOrWhiteSpace(_config.Password))
                {
                    await client.AuthenticateAsync(_config.Username, _config.Password, cancellationToken);
                }

                // Invia il messaggio
                await client.SendAsync(message, cancellationToken);

                // Disconnetti
                await client.DisconnectAsync(true, cancellationToken);

                _logger.LogInformation(
                    "Email inviata con successo a {Count} destinatari: {Recipients}",
                    emailList.Count,
                    string.Join(", ", emailList)
                );

                return (true, null);
            }
            catch (MailKit.Security.AuthenticationException ex)
            {
                _logger.LogError(ex, "Errore di autenticazione SMTP. Verifica username e password");
                return (false, "Errore di autenticazione SMTP. Verifica le credenziali.");
            }
            catch (MailKit.Net.Smtp.SmtpCommandException ex)
            {
                _logger.LogError(ex, "Errore comando SMTP: {Message}", ex.Message);
                return (false, $"Errore SMTP: {ex.Message}");
            }
            catch (MailKit.Net.Smtp.SmtpProtocolException ex)
            {
                _logger.LogError(ex, "Errore protocollo SMTP: {Message}", ex.Message);
                return (false, $"Errore protocollo SMTP: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore generico durante l'invio email");
                return (false, $"Errore durante l'invio email: {ex.Message}");
            }
        }

        // ===================================
        // INVIO EMAIL CON TEMPLATE
        // ===================================

        public async Task<(bool Success, string? ErrorMessage)> SendEmailWithTemplateAsync<T>(
            string toEmail,
            string subject,
            string templateName,
            T model,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation(
                    "Rendering template {TemplateName} per email a {Email}",
                    templateName,
                    toEmail
                );

                // Renderizza il template con il modello
                var htmlBody = await _templateService.RenderTemplateAsync(templateName, model);

                // Invia l'email con il corpo HTML processato
                return await SendEmailAsync(toEmail, subject, htmlBody, cancellationToken);
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogError(ex, "Template email non trovato: {TemplateName}", templateName);
                return (false, $"Template non trovato: {templateName}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il rendering del template {TemplateName}", templateName);
                return (false, $"Errore nel rendering del template: {ex.Message}");
            }
        }

        // ===================================
        // INTEGRAZIONE CON NOTIFICHE
        // ===================================

        public async Task<(bool Success, string? ErrorMessage)> SendNotificationEmailAsync(
            Guid notificaId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Invio email per notifica {NotificaId}", notificaId);

                // Recupera la notifica con il destinatario
                var notifica = await _context.Notifiche
                    .Include(n => n.Destinatario)
                    .FirstOrDefaultAsync(n => n.Id == notificaId, cancellationToken);

                if (notifica == null)
                {
                    _logger.LogWarning("Notifica {NotificaId} non trovata", notificaId);
                    return (false, "Notifica non trovata");
                }

                if (notifica.IsInviataEmail)
                {
                    _logger.LogInformation("Email per notifica {NotificaId} già inviata in precedenza", notificaId);
                    return (true, "Email già inviata in precedenza");
                }

                // Verifica che il destinatario abbia un'email
                if (string.IsNullOrWhiteSpace(notifica.Destinatario.Email))
                {
                    _logger.LogWarning(
                        "Il destinatario della notifica {NotificaId} non ha un indirizzo email",
                        notificaId
                    );
                    return (false, "Il destinatario non ha un indirizzo email");
                }

                // Prepara il modello per il template
                var emailModel = new
                {
                    Titolo = notifica.Titolo,
                    Messaggio = notifica.Messaggio,
                    Link = notifica.Link,
                    TipoNotifica = notifica.Tipo.ToString(),
                    DataCreazione = notifica.CreatedAt.ToString("dd/MM/yyyy HH:mm")
                };

                // Determina il template da usare in base al tipo di notifica
                var templateName = GetTemplateNameForNotificationType(notifica.Tipo);

                // Prova a usare il template specifico, altrimenti usa fallback HTML
                string htmlBody;
                try
                {
                    htmlBody = await _templateService.RenderTemplateAsync(templateName, emailModel);
                }
                catch (FileNotFoundException)
                {
                    _logger.LogWarning(
                        "Template {TemplateName} non trovato, uso template fallback",
                        templateName
                    );
                    htmlBody = _templateService.GenerateFallbackHtml(
                        notifica.Titolo,
                        notifica.Messaggio,
                        notifica.Link
                    );
                }

                // Invia l'email
                var (success, errorMessage) = await SendEmailAsync(
                    notifica.Destinatario.Email,
                    notifica.Titolo,
                    htmlBody,
                    cancellationToken
                );

                // Se l'invio è riuscito (o simulato in dev), aggiorna la notifica
                if (success)
                {
                    notifica.IsInviataEmail = true;
                    notifica.DataInvioEmail = DateTime.Now;
                    await _context.SaveChangesAsync(cancellationToken);

                    _logger.LogInformation(
                        "Notifica {NotificaId} aggiornata: email inviata a {Email}",
                        notificaId,
                        notifica.Destinatario.Email
                    );
                }

                return (success, errorMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'invio email per notifica {NotificaId}", notificaId);
                return (false, $"Errore: {ex.Message}");
            }
        }

        /// <summary>
        /// Determina il nome del template in base al tipo di notifica
        /// </summary>
        private string GetTemplateNameForNotificationType(Models.Enums.TipoNotifica tipo)
        {
            return tipo switch
            {
                Models.Enums.TipoNotifica.ScadenzaImminente => "scadenza",
                Models.Enums.TipoNotifica.RichiestaIntegrazione => "richiesta-integrazione",
                Models.Enums.TipoNotifica.CambioStatoLotto => "cambio-stato",
                Models.Enums.TipoNotifica.NuovoDocumento => "nuovo-documento",
                _ => "notification" // Template generico
            };
        }

        // ===================================
        // UTILITY
        // ===================================

        public async Task<(bool Success, string? ErrorMessage)> TestConnectionAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Test connessione SMTP a {Host}:{Port}", _config.Host, _config.Port);

                using var client = new SmtpClient();
                client.Timeout = _config.TimeoutSeconds * 1000;

                // Connetti
                await client.ConnectAsync(
                    _config.Host,
                    _config.Port,
                    _config.EnableSSL ? SecureSocketOptions.StartTls : SecureSocketOptions.None,
                    cancellationToken
                );

                // Autentica
                if (!string.IsNullOrWhiteSpace(_config.Username) && !string.IsNullOrWhiteSpace(_config.Password))
                {
                    await client.AuthenticateAsync(_config.Username, _config.Password, cancellationToken);
                }

                // Disconnetti
                await client.DisconnectAsync(true, cancellationToken);

                _logger.LogInformation("Connessione SMTP riuscita");
                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Test connessione SMTP fallito");
                return (false, ex.Message);
            }
        }

        public bool IsEmailSendingEnabled()
        {
            return _config.EnableEmailSending;
        }
    }
}