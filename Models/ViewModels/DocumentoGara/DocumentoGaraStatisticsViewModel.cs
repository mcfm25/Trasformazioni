namespace Trasformazioni.Models.ViewModels.DocumentoGara
{
    /// <summary>
    /// ViewModel per le statistiche sui documenti
    /// </summary>
    public class DocumentoGaraStatisticsViewModel
    {
        /// <summary>
        /// Numero totale di documenti
        /// </summary>
        public int TotaleDocumenti { get; set; }

        /// <summary>
        /// Dimensione totale in bytes
        /// </summary>
        public long DimensioneTotaleBytes { get; set; }

        /// <summary>
        /// Dimensione totale formattata (es: "1.5 GB")
        /// </summary>
        public string DimensioneTotaleFormatted { get; set; } = string.Empty;

        ///// <summary>
        ///// Numero di documenti per tipo
        ///// </summary>
        //public Dictionary<TipoDocumentoGara, int> DocumentiPerTipo { get; set; } = new();

        // DOPO
        /// <summary>
        /// Numero di documenti per tipo (chiave = nome tipo documento)
        /// </summary>
        public Dictionary<string, int> DocumentiPerTipo { get; set; } = new();


        /// <summary>
        /// Numero di documenti per MIME type
        /// </summary>
        public Dictionary<string, int> DocumentiPerMimeType { get; set; } = new();

        /// <summary>
        /// Dimensione media dei documenti (in bytes)
        /// </summary>
        public long DimensioneMedia { get; set; }

        /// <summary>
        /// Data del documento più recente
        /// </summary>
        public DateTime? DataUltimoCaricamento { get; set; }

        /// <summary>
        /// Data del documento più vecchio
        /// </summary>
        public DateTime? DataPrimoCaricamento { get; set; }

        /// <summary>
        /// Top 5 utenti per numero di documenti caricati
        /// </summary>
        public List<(string UserId, string UserName, int Count)> TopUtenti { get; set; } = new();
    }
}