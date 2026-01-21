namespace Trasformazioni.Configuration
{
    /// <summary>
    /// Configurazione per Hangfire Job Scheduler
    /// </summary>
    public class HangfireConfiguration
    {
        /// <summary>
        /// Schema database per le tabelle Hangfire
        /// </summary>
        public string Schema { get; set; } = "public";

        /// <summary>
        /// Path per la dashboard Hangfire
        /// </summary>
        public string DashboardPath { get; set; } = "/hangfire";

        /// <summary>
        /// Titolo della dashboard
        /// </summary>
        public string DashboardTitle { get; set; } = "Job Scheduler";

        /// <summary>
        /// Numero di worker paralleli
        /// </summary>
        public int WorkerCount { get; set; } = 2;

        /// <summary>
        /// Intervallo di polling delle code in secondi
        /// </summary>
        public int QueuePollInterval { get; set; } = 15;

        /// <summary>
        /// Timeout invisibilità job in minuti.
        /// Se un job impiega più di questo tempo, viene considerato fallito e rimesso in coda.
        /// </summary>
        public int InvisibilityTimeoutMinutes { get; set; } = 30;

        /// <summary>
        /// Timeout per i lock distribuiti in minuti.
        /// Importante per ambienti multi-server.
        /// </summary>
        public int DistributedLockTimeoutMinutes { get; set; } = 10;

        /// <summary>
        /// Numero di tentativi di retry di default
        /// </summary>
        public int DefaultRetryAttempts { get; set; } = 3;

        /// <summary>
        /// URL base dell'applicazione per i link nelle email
        /// </summary>
        public string BaseUrl { get; set; } = "https://localhost:5001";

        /// <summary>
        /// Configurazione dei singoli job
        /// </summary>
        public HangfireJobsConfiguration Jobs { get; set; } = new();
    }

    /// <summary>
    /// Configurazione dei job schedulati
    /// </summary>
    public class HangfireJobsConfiguration
    {
        public HangfireJobConfig AggiornaStatiScadenza { get; set; } = new()
        {
            Enabled = true,
            CronExpression = "0 6 * * *", // Ogni giorno alle 06:00
            Queue = "scheduled",
            RetryAttempts = 3,
            SendEmail = true,
            NotificaEmailDestinatari = ""
        };

        public HangfireJobConfig ProcessaRinnoviAutomatici { get; set; } = new()
        {
            Enabled = true,
            CronExpression = "0 7 * * *", // Ogni giorno alle 07:00
            Queue = "scheduled",
            RetryAttempts = 3,
            SendEmail = true,
            NotificaEmailDestinatari = ""
        };
    }

    /// <summary>
    /// Configurazione singolo job
    /// </summary>
    public class HangfireJobConfig
    {
        /// <summary>
        /// Se true, il job è attivo
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Espressione CRON per la schedulazione
        /// </summary>
        public string CronExpression { get; set; } = string.Empty;

        /// <summary>
        /// Nome della coda su cui eseguire il job
        /// </summary>
        public string Queue { get; set; } = "default";

        /// <summary>
        /// Numero di tentativi in caso di fallimento
        /// </summary>
        public int RetryAttempts { get; set; } = 3;

        /// <summary>
        /// Se true, invia email di notifica
        /// </summary>
        public bool SendEmail { get; set; } = true;

        /// <summary>
        /// Lista email destinatari notifiche (separati da ;)
        /// </summary>
        public string NotificaEmailDestinatari { get; set; } = string.Empty;

        /// <summary>
        /// Restituisce la lista di email come array
        /// </summary>
        public IEnumerable<string> GetDestinatariEmail()
        {
            if (string.IsNullOrWhiteSpace(NotificaEmailDestinatari))
                return Enumerable.Empty<string>();

            return NotificaEmailDestinatari
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(e => e.Trim())
                .Where(e => !string.IsNullOrWhiteSpace(e));
        }
    }
}