namespace Trasformazioni.Configuration
{
    /// <summary>
    /// Configurazione per il servizio MinIO Object Storage
    /// Mappa le impostazioni dalla sezione "MinIO" in appsettings.json
    /// </summary>
    public class MinIOConfiguration
    {
        /// <summary>
        /// Endpoint del server MinIO (es: "dev01.dev.it")
        /// </summary>
        public string Endpoint { get; set; } = string.Empty;

        /// <summary>
        /// Porta del server MinIO (default: 80 per HTTP, 443 per HTTPS)
        /// </summary>
        public int Port { get; set; } = 80;

        /// <summary>
        /// Access Key per l'autenticazione MinIO
        /// </summary>
        public string AccessKey { get; set; } = string.Empty;

        /// <summary>
        /// Secret Key per l'autenticazione MinIO
        /// </summary>
        public string SecretKey { get; set; } = string.Empty;

        /// <summary>
        /// Indica se utilizzare SSL/TLS per la connessione
        /// </summary>
        public bool UseSSL { get; set; } = false;

        /// <summary>
        /// Nome del bucket di default per i documenti
        /// </summary>
        public string DefaultBucket { get; set; } = string.Empty;

        /// <summary>
        /// Dimensione massima del file in byte (default: 134217728 == 128 MB)
        /// </summary>
        public int MaxFileSizeBytes { get; set; } = 134217728;

        /// <summary>
        /// Gets or sets del timeout in milliseconds per le richieste
        /// </summary>
        public int TimeoutMilliseconds { get; set; } = 60000;
    }
}