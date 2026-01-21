using Hangfire;
using Microsoft.Extensions.Options;
using Trasformazioni.Configuration;
using Trasformazioni.Constants;
using Trasformazioni.Models.DTOs;
using Trasformazioni.Models.Enums;
using Trasformazioni.Services.Interfaces;

namespace Trasformazioni.Jobs
{
    /// <summary>
    /// Job schedulati per la gestione del Registro Contratti
    /// </summary>
    public class RegistroContrattiJobs
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly HangfireConfiguration _hangfireConfig;
        private readonly ILogger<RegistroContrattiJobs> _logger;

        public RegistroContrattiJobs(
            IServiceScopeFactory scopeFactory,
            ILogger<RegistroContrattiJobs> logger,
            IOptions<HangfireConfiguration> hangfireConfig)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _hangfireConfig = hangfireConfig.Value;
        }

        /// <summary>
        /// Job per aggiornare gli stati dei registri in base alle scadenze ed invio notifiche mail
        /// Eseguito ogni giorno alle 06:00
        /// </summary>
        [AutomaticRetry(Attempts = 3, OnAttemptsExceeded = AttemptsExceededAction.Fail)]
        [Queue("scheduled")] // Opzionale: specifica una coda dedicata per i job schedulati
        [JobDisplayName("Registro Contratti - Aggiorna Stati Scadenza ed invio notifiche mail")]
        public async Task AggiornaStatiScadenzaAsync()
        {
            _logger.LogInformation("Avvio job AggiornaStatiScadenza");

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var registroService = scope.ServiceProvider.GetRequiredService<IRegistroContrattiService>();
                var notificaConfigService = scope.ServiceProvider.GetRequiredService<INotificaEmailConfigService>();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                // Esegui aggiornamento stati
                var results = await registroService.AggiornaStatiScadenzaAsync();

                _logger.LogInformation(
                    "Job AggiornaStatiScadenza completato: {Count} registri aggiornati",
                    results.Count);

                // Invia email di notifica
                var jobConfig = _hangfireConfig.Jobs.AggiornaStatiScadenza;
                if (jobConfig.SendEmail && results.Any())
                {
                    await InviaEmailNotificheAsync(
                        notificaConfigService,
                        emailService,
                        results);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'esecuzione del job AggiornaStatiScadenza");
                throw; // Rilancia per far registrare il fallimento a Hangfire
            }
        }

        /// <summary>
        /// Job per processare i rinnovi automatici dei contratti ed invio notifiche mail
        /// Eseguito ogni giorno alle 07:00
        /// </summary>
        [AutomaticRetry(Attempts = 3, OnAttemptsExceeded = AttemptsExceededAction.Fail)]
        [Queue("scheduled")]
        [JobDisplayName("Registro Contratti - Processa Rinnovi Automatici ed invio notifiche mail")]
        public async Task ProcessaRinnoviAutomaticiAsync()
        {
            _logger.LogInformation("Avvio job ProcessaRinnoviAutomatici");

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var registroService = scope.ServiceProvider.GetRequiredService<IRegistroContrattiService>();
                var notificaConfigService = scope.ServiceProvider.GetRequiredService<INotificaEmailConfigService>();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                // Esegui rinnovi automatici
                var results = await registroService.ProcessaRinnoviAutomaticiAsync();

                _logger.LogInformation(
                    "Job ProcessaRinnoviAutomatici completato: {Count} rinnovi processati",
                    results.Count);

                // Invia email di notifica
                var jobConfig = _hangfireConfig.Jobs.ProcessaRinnoviAutomatici;
                if (jobConfig.SendEmail && results.Any())
                {
                    await InviaEmailRinnoviAsync(
                        notificaConfigService,
                        emailService,
                        results);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'esecuzione del job ProcessaRinnoviAutomatici");
                throw;
            }
        }



        /// <summary>
        /// Invia email di notifica per i cambi stato (InScadenza, Scaduto)
        /// </summary>
        private async Task InviaEmailNotificheAsync(
            INotificaEmailConfigService notificaConfigService,
            IEmailService emailService,
            List<RegistroStatoChangeResult> results)
        {
            try
            {
                // Raggruppa per tipo di cambio stato
                var inScadenza = results.Where(r => r.NuovoStato == StatoRegistro.InScadenza).ToList();
                var scaduti = results.Where(r => r.NuovoStato == StatoRegistro.Scaduto).ToList();

                // Email per registri in scadenza
                if (inScadenza.Any())
                {
                    var destinatari = await notificaConfigService.GetDestinatariAsync(CodiciNotifica.ContrattoInScadenza);

                    if (destinatari.Any())
                    {
                        var oggettoBase = await notificaConfigService.GetOggettoEmailDefaultAsync(CodiciNotifica.ContrattoInScadenza)
                            ?? "⚠️ Contratto in scadenza";

                        var subject = GeneraOggettoEmail(oggettoBase, inScadenza, "contratto", "contratti");

                        var htmlBody = GeneraEmailHtml("Contratti in Scadenza", inScadenza, "warning");

                        var (success, error) = await emailService.SendEmailAsync(destinatari, subject, htmlBody);

                        if (!success)
                            _logger.LogWarning("Errore invio email scadenze: {Error}", error);
                        else
                            _logger.LogInformation("Email scadenze inviata a {Count} destinatari", destinatari.Count);
                    }
                    else
                    {
                        _logger.LogDebug("Nessun destinatario configurato per {Codice}", CodiciNotifica.ContrattoInScadenza);
                    }
                }

                // Email per registri scaduti
                if (scaduti.Any())
                {
                    var destinatari = await notificaConfigService.GetDestinatariAsync(CodiciNotifica.ContrattoScaduto);

                    if (destinatari.Any())
                    {
                        var oggettoBase = await notificaConfigService.GetOggettoEmailDefaultAsync(CodiciNotifica.ContrattoScaduto)
                            ?? "❌ Contratto scaduto";

                        var subject = GeneraOggettoEmail(oggettoBase, scaduti, "contratto", "contratti");

                        var htmlBody = GeneraEmailHtml("Contratti Scaduti", scaduti, "danger");

                        var (success, error) = await emailService.SendEmailAsync(destinatari, subject, htmlBody);

                        if (!success)
                            _logger.LogWarning("Errore invio email scaduti: {Error}", error);
                        else
                            _logger.LogInformation("Email scaduti inviata a {Count} destinatari", destinatari.Count);
                    }
                    else
                    {
                        _logger.LogDebug("Nessun destinatario configurato per {Codice}", CodiciNotifica.ContrattoScaduto);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log errore ma non bloccare il job
                _logger.LogError(ex, "Errore durante l'invio delle email di notifica");
            }
        }

        /// <summary>
        /// Invia email di notifica per i rinnovi automatici
        /// </summary>
        private async Task InviaEmailRinnoviAsync(
            INotificaEmailConfigService notificaConfigService,
            IEmailService emailService,
            List<RegistroStatoChangeResult> results)
        {
            try
            {
                var destinatari = await notificaConfigService.GetDestinatariAsync(CodiciNotifica.RinnovoAutomatico);

                if (!destinatari.Any())
                {
                    _logger.LogDebug("Nessun destinatario configurato per {Codice}", CodiciNotifica.RinnovoAutomatico);
                    return;
                }

                var oggettoBase = await notificaConfigService.GetOggettoEmailDefaultAsync(CodiciNotifica.RinnovoAutomatico)
                    ?? "🔄 Rinnovo automatico";

                var subject = GeneraOggettoEmail(oggettoBase, results, "contratto rinnovato", "contratti rinnovati");

                var htmlBody = GeneraEmailHtml("Rinnovi Automatici", results, "success");

                var (success, error) = await emailService.SendEmailAsync(destinatari, subject, htmlBody);

                if (!success)
                    _logger.LogWarning("Errore invio email rinnovi: {Error}", error);
                else
                    _logger.LogInformation("Email rinnovi inviata a {Count} destinatari", destinatari.Count);
            }
            catch (Exception ex)
            {
                // Log errore ma non bloccare il job
                _logger.LogError(ex, "Errore durante l'invio delle email di rinnovo");
            }
        }

        /// <summary>
        /// Genera HTML per l'email di notifica
        /// </summary>
        private string GeneraEmailHtml(
            string titolo,
            List<RegistroStatoChangeResult> registri,
            string colorType)
        {
            var colorMap = new Dictionary<string, (string bg, string border, string icon)>
            {
                { "warning", ("#fff3cd", "#ffc107", "⚠️") },
                { "danger", ("#f8d7da", "#dc3545", "❌") },
                { "success", ("#d4edda", "#28a745", "✅") }
            };

            var (bgColor, borderColor, icon) = colorMap.GetValueOrDefault(colorType, ("#e7f3ff", "#2196F3", "ℹ️"));

            var righeTabella = string.Join("", registri.Select(r =>
            {
                var link = $"{_hangfireConfig.BaseUrl}/RegistroContratti/Details/{r.RegistroId}";
                var dataScadenza = r.DataScadenza?.ToString("dd/MM/yyyy") ?? "N/D";
                var rinnovoInfo = r.NuovoRegistroId.HasValue
                    ? $"<br/><small>Nuovo: <a href=\"{_hangfireConfig.BaseUrl}/RegistroContratti/Details/{r.NuovoRegistroId}\">{r.NuovoNumeroProtocollo}</a></small>"
                    : "";

                return $@"
                <tr>
                    <td style=""padding: 10px; border-bottom: 1px solid #dee2e6;"">
                        <a href=""{link}"" style=""color: #007bff; text-decoration: none;"">{r.NumeroProtocollo ?? "N/D"}</a>
                        {rinnovoInfo}
                    </td>
                    <td style=""padding: 10px; border-bottom: 1px solid #dee2e6;"">{r.Oggetto}</td>
                    <td style=""padding: 10px; border-bottom: 1px solid #dee2e6;"">{r.RagioneSociale ?? "N/D"}</td>
                    <td style=""padding: 10px; border-bottom: 1px solid #dee2e6;"">{dataScadenza}</td>
                </tr>";
            }));

            return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset=""utf-8"">
                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                <title>{titolo}</title>
            </head>
            <body style=""font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 0; background-color: #f4f6f9;"">
                <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color: #f4f6f9; padding: 20px;"">
                    <tr>
                        <td align=""center"">
                            <table width=""700"" cellpadding=""0"" cellspacing=""0"" style=""background-color: white; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 4px rgba(0,0,0,0.1);"">
                                <!-- Header -->
                                <tr>
                                    <td style=""background-color: {borderColor}; padding: 25px; text-align: center;"">
                                        <h1 style=""color: white; margin: 0; font-size: 24px;"">{icon} {titolo}</h1>
                                    </td>
                                </tr>
                    
                                <!-- Content -->
                                <tr>
                                    <td style=""padding: 25px;"">
                                        <div style=""background-color: {bgColor}; border-left: 4px solid {borderColor}; padding: 15px; margin-bottom: 20px; border-radius: 4px;"">
                                            <strong>Riepilogo:</strong> {registri.Count} contratti interessati
                                        </div>
                            
                                        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""border-collapse: collapse;"">
                                            <thead>
                                                <tr style=""background-color: #f8f9fa;"">
                                                    <th style=""padding: 12px 10px; text-align: left; border-bottom: 2px solid #dee2e6; font-weight: 600;"">Protocollo</th>
                                                    <th style=""padding: 12px 10px; text-align: left; border-bottom: 2px solid #dee2e6; font-weight: 600;"">Oggetto</th>
                                                    <th style=""padding: 12px 10px; text-align: left; border-bottom: 2px solid #dee2e6; font-weight: 600;"">Cliente</th>
                                                    <th style=""padding: 12px 10px; text-align: left; border-bottom: 2px solid #dee2e6; font-weight: 600;"">Scadenza</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                {righeTabella}
                                            </tbody>
                                        </table>
                                    </td>
                                </tr>
                    
                                <!-- Footer -->
                                <tr>
                                    <td style=""background-color: #f8f9fa; padding: 20px; text-align: center; font-size: 12px; color: #6c757d; border-top: 1px solid #e9ecef;"">
                                        <p style=""margin: 0;"">Questa è una email automatica generata dal sistema.</p>
                                        <p style=""margin: 5px 0 0 0;"">Data elaborazione: {DateTime.Now:dd/MM/yyyy HH:mm}</p>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </body>
            </html>";
        }

        /// <summary>
        /// Genera l'oggetto email in base al numero di registri
        /// </summary>
        private string GeneraOggettoEmail(
            string oggettoBase,
            List<RegistroStatoChangeResult> registri,
            string labelSingolare,
            string labelPlurale)
        {
            if (registri.Count == 1)
            {
                var registro = registri.First();
                var dettaglio = registro.NumeroProtocollo ?? registro.Oggetto ?? "";
                return string.IsNullOrEmpty(dettaglio)
                    ? oggettoBase
                    : $"{oggettoBase}: {dettaglio}";
            }
            else
            {
                return $"{oggettoBase}: {registri.Count} {labelPlurale}";
            }
        }
    }
}