using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Trasformazioni.HealthChecks
{
    /// <summary>
    /// Health check per verificare la presenza e validità delle configurazioni richieste
    /// </summary>
    public class ConfigurationHealthCheck : IHealthCheck
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ConfigurationHealthCheck> _logger;

        // Lista delle configurazioni richieste con i loro percorsi
        private readonly Dictionary<string, string> _requiredConfigurations = new()
        {
            // Database
            { "Database:Host", "Database host" },
            { "Database:Port", "Database port" },
            { "Database:Database", "Database name" },
            { "Database:Username", "Database username" },
            { "Database:Password", "Database password" },
            { "Database:Schema", "Database schema" },
            
            // Traccar
            { "Traccar:BaseUrl", "Traccar base URL" },
            { "Traccar:Username", "Traccar username" },
            { "Traccar:Password", "Traccar password" }
        };

        public ConfigurationHealthCheck(
            IConfiguration configuration,
            ILogger<ConfigurationHealthCheck> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            var missingConfigurations = new List<string>();
            var emptyConfigurations = new List<string>();

            // Verifica ogni configurazione richiesta
            foreach (var (key, description) in _requiredConfigurations)
            {
                var value = _configuration[key];

                if (value == null)
                {
                    missingConfigurations.Add($"{description} ({key})");
                    _logger.LogWarning("Missing configuration: {Key}", key);
                }
                else if (string.IsNullOrWhiteSpace(value))
                {
                    emptyConfigurations.Add($"{description} ({key})");
                    _logger.LogWarning("Empty configuration: {Key}", key);
                }
            }

            // Costruisci il risultato
            if (missingConfigurations.Any() || emptyConfigurations.Any())
            {
                var errors = new List<string>();

                if (missingConfigurations.Any())
                {
                    errors.Add($"Missing: {string.Join(", ", missingConfigurations)}");
                }

                if (emptyConfigurations.Any())
                {
                    errors.Add($"Empty: {string.Join(", ", emptyConfigurations)}");
                }

                var errorMessage = string.Join(" | ", errors);

                _logger.LogError("Configuration health check failed: {ErrorMessage}", errorMessage);

                return Task.FromResult(
                    HealthCheckResult.Unhealthy(
                        $"Configuration validation failed: {errorMessage}",
                        data: new Dictionary<string, object>
                        {
                            { "MissingCount", missingConfigurations.Count },
                            { "EmptyCount", emptyConfigurations.Count },
                            { "Missing", missingConfigurations },
                            { "Empty", emptyConfigurations }
                        }
                    )
                );
            }

            // Validazioni aggiuntive (opzionali)
            var warnings = new List<string>();

            // Verifica formato URL Traccar
            var traccarUrl = _configuration["Traccar:BaseUrl"];
            if (traccarUrl != null && !Uri.TryCreate(traccarUrl, UriKind.Absolute, out _))
            {
                warnings.Add("Traccar BaseUrl format is invalid");
                _logger.LogWarning("Invalid Traccar BaseUrl format: {Url}", traccarUrl);
            }

            // Verifica porta database
            var dbPort = _configuration["Database:Port"];
            if (dbPort != null && !int.TryParse(dbPort, out var port))
            {
                warnings.Add("Database Port is not a valid number");
                _logger.LogWarning("Invalid Database Port: {Port}", dbPort);
            }

            if (warnings.Any())
            {
                return Task.FromResult(
                    HealthCheckResult.Degraded(
                        $"Configuration has warnings: {string.Join(", ", warnings)}",
                        data: new Dictionary<string, object>
                        {
                            { "Warnings", warnings }
                        }
                    )
                );
            }

            _logger.LogInformation("Configuration health check passed");

            return Task.FromResult(
                HealthCheckResult.Healthy(
                    "All required configurations are present and valid",
                    data: new Dictionary<string, object>
                    {
                        { "CheckedConfigurations", _requiredConfigurations.Count }
                    }
                )
            );
        }
    }
}