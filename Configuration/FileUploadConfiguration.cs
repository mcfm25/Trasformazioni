namespace Trasformazioni.Configuration
{
    /// <summary>
    /// Configurazione per la gestione degli upload di file
    /// </summary>
    public class FileUploadConfiguration
    {
        /// <summary>
        /// Dimensione massima file in MB (default: 128 MB)
        /// </summary>
        public int MaxFileSizeMB { get; set; } = 128;

        /// <summary>
        /// Dimensione massima file in bytes (calcolata)
        /// </summary>
        public long MaxFileSizeBytes => MaxFileSizeMB * 1024L * 1024L;

        /// <summary>
        /// Estensioni file consentite
        /// </summary>
        public string[] AllowedExtensions { get; set; } =
        {
            ".pdf", ".docx", ".doc", ".xlsx", ".xls",
            ".zip", ".jpg", ".jpeg", ".png", ".gif", ".bmp"
        };

        /// <summary>
        /// MIME types consentiti
        /// </summary>
        public string[] AllowedMimeTypes { get; set; } =
        {
            "application/pdf",
            "application/msword",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "application/vnd.ms-excel",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "application/zip",
            "application/x-zip-compressed",
            "image/jpeg",
            "image/png",
            "image/gif",
            "image/bmp"
        };
    }
}