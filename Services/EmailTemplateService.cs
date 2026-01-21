using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;
using Trasformazioni.Configuration;

namespace Trasformazioni.Services
{
    /// <summary>
    /// Servizio helper per la gestione dei template email HTML
    /// Carica template da file e sostituisce placeholder con dati del modello
    /// </summary>
    public class EmailTemplateService
    {
        private readonly EmailConfiguration _config;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<EmailTemplateService> _logger;

        public EmailTemplateService(
            IOptions<EmailConfiguration> config,
            IWebHostEnvironment environment,
            ILogger<EmailTemplateService> logger)
        {
            _config = config.Value;
            _environment = environment;
            _logger = logger;
        }

        /// <summary>
        /// Carica un template HTML da file e sostituisce i placeholder con i dati del modello
        /// Placeholder format: {{PropertyName}}
        /// </summary>
        /// <typeparam name="T">Tipo del modello dati</typeparam>
        /// <param name="templateName">Nome del template (senza estensione)</param>
        /// <param name="model">Modello dati con le proprietà da sostituire</param>
        /// <returns>HTML processato con i valori sostituiti</returns>
        public async Task<string> RenderTemplateAsync<T>(string templateName, T model)
        {
            try
            {
                // Costruisci il percorso del template
                var templatePath = Path.Combine(
                    _environment.WebRootPath,
                    _config.TemplateDirectory,
                    $"{templateName}.html"
                );

                _logger.LogDebug("Caricamento template email: {TemplatePath}", templatePath);

                // Verifica esistenza file
                if (!File.Exists(templatePath))
                {
                    _logger.LogError("Template email non trovato: {TemplatePath}", templatePath);
                    throw new FileNotFoundException($"Template email non trovato: {templateName}");
                }

                // Leggi il contenuto del template
                var templateContent = await File.ReadAllTextAsync(templatePath);

                // Sostituisci i placeholder con i valori del modello
                var processedHtml = ReplacePlaceholders(templateContent, model);

                _logger.LogDebug("Template email {TemplateName} processato con successo", templateName);

                return processedHtml;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel rendering del template {TemplateName}", templateName);
                throw;
            }
        }

        /// <summary>
        /// Sostituisce i placeholder {{PropertyName}} nel template con i valori delle proprietà del modello
        /// Supporta anche placeholder condizionali: {{#if PropertyName}}...{{/if}}
        /// </summary>
        private string ReplacePlaceholders<T>(string template, T model)
        {
            if (model == null)
                return template;

            var result = template;
            var modelType = typeof(T);
            var properties = modelType.GetProperties();

            // Sostituisci placeholder semplici {{PropertyName}}
            foreach (var property in properties)
            {
                var propertyName = property.Name;
                var propertyValue = property.GetValue(model);
                var placeholder = $"{{{{{propertyName}}}}}";

                // Converti il valore in stringa (gestisce null)
                var valueString = propertyValue?.ToString() ?? string.Empty;

                result = result.Replace(placeholder, valueString);
            }

            // Gestisci blocchi condizionali {{#if PropertyName}}...{{/if}}
            result = ProcessConditionalBlocks(result, model, properties);

            return result;
        }

        /// <summary>
        /// Processa i blocchi condizionali nel template
        /// Sintassi: {{#if PropertyName}}contenuto{{/if}}
        /// Il contenuto viene mostrato solo se PropertyName è true, non null, o non vuoto
        /// </summary>
        private string ProcessConditionalBlocks<T>(string template, T model, System.Reflection.PropertyInfo[] properties)
        {
            var result = template;

            // Pattern per trovare blocchi condizionali: {{#if PropertyName}}...{{/if}}
            var ifPattern = @"\{\{#if\s+(\w+)\}\}(.*?)\{\{/if\}\}";
            var regex = new Regex(ifPattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            var matches = regex.Matches(result);

            foreach (Match match in matches)
            {
                var propertyName = match.Groups[1].Value;
                var content = match.Groups[2].Value;
                var fullMatch = match.Groups[0].Value;

                // Trova la proprietà nel modello
                var property = properties.FirstOrDefault(p =>
                    p.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase));

                if (property != null)
                {
                    var propertyValue = property.GetValue(model);
                    var shouldShow = EvaluateCondition(propertyValue);

                    // Sostituisci il blocco con il contenuto o stringa vuota
                    result = result.Replace(fullMatch, shouldShow ? content : string.Empty);
                }
                else
                {
                    // Proprietà non trovata, rimuovi il blocco
                    result = result.Replace(fullMatch, string.Empty);
                    _logger.LogWarning("Proprietà {PropertyName} non trovata nel modello per blocco condizionale", propertyName);
                }
            }

            return result;
        }

        /// <summary>
        /// Valuta se una condizione è vera
        /// true se: bool true, stringa non vuota, numero != 0, oggetto non null
        /// </summary>
        private bool EvaluateCondition(object? value)
        {
            if (value == null)
                return false;

            if (value is bool boolValue)
                return boolValue;

            if (value is string stringValue)
                return !string.IsNullOrWhiteSpace(stringValue);

            if (value is int intValue)
                return intValue != 0;

            if (value is decimal decimalValue)
                return decimalValue != 0;

            // Per altri tipi, considera true se non null
            return true;
        }

        /// <summary>
        /// Genera HTML di fallback per quando il template non è disponibile
        /// </summary>
        public string GenerateFallbackHtml(string titolo, string messaggio, string? link = null)
        {
            var linkHtml = !string.IsNullOrWhiteSpace(link)
                ? $"<p><a href=\"{link}\" style=\"display: inline-block; padding: 10px 20px; background-color: #007bff; color: white; text-decoration: none; border-radius: 5px;\">Vai al dettaglio</a></p>"
                : string.Empty;

            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>{titolo}</title>
</head>
<body style=""font-family: Arial, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 0; background-color: #f4f4f4;"">
    <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color: #f4f4f4; padding: 20px;"">
        <tr>
            <td align=""center"">
                <table width=""600"" cellpadding=""0"" cellspacing=""0"" style=""background-color: white; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 4px rgba(0,0,0,0.1);"">
                    <tr>
                        <td style=""background-color: #007bff; padding: 30px; text-align: center;"">
                            <h1 style=""color: white; margin: 0; font-size: 24px;"">{titolo}</h1>
                        </td>
                    </tr>
                    <tr>
                        <td style=""padding: 30px;"">
                            <p style=""margin: 0 0 20px 0; font-size: 16px;"">{messaggio}</p>
                            {linkHtml}
                        </td>
                    </tr>
                    <tr>
                        <td style=""background-color: #f8f9fa; padding: 20px; text-align: center; font-size: 12px; color: #6c757d;"">
                            <p style=""margin: 0;"">Questa è una email automatica. Si prega di non rispondere.</p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
        }
    }
}