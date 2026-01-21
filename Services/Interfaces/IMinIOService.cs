namespace Trasformazioni.Services.Interfaces
{
    /// <summary>
    /// Servizio per la gestione dello storage MinIO
    /// Fornisce operazioni CRUD per i file su MinIO Object Storage
    /// </summary>
    public interface IMinIOService
    {
        /// <summary>
        /// Carica un file su MinIO
        /// </summary>
        /// <param name="stream">Stream del file da caricare</param>
        /// <param name="objectName">Path completo nel bucket (es: "gare/{garaId}/{guid}_{filename}")</param>
        /// <param name="contentType">MIME type del file</param>
        /// <param name="cancellationToken">Token per la cancellazione dell'operazione</param>
        /// <returns>Path completo del file caricato</returns>
        Task<string> UploadFileAsync(
            Stream stream,
            string objectName,
            string contentType,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Scarica un file da MinIO
        /// </summary>
        /// <param name="objectName">Path completo nel bucket</param>
        /// <param name="cancellationToken">Token per la cancellazione dell'operazione</param>
        /// <returns>Stream del file scaricato</returns>
        Task<Stream> DownloadFileAsync(
            string objectName,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Elimina un file da MinIO
        /// </summary>
        /// <param name="objectName">Path completo nel bucket</param>
        /// <param name="cancellationToken">Token per la cancellazione dell'operazione</param>
        Task DeleteFileAsync(
            string objectName,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Verifica se un file esiste su MinIO
        /// </summary>
        /// <param name="objectName">Path completo nel bucket</param>
        /// <param name="cancellationToken">Token per la cancellazione dell'operazione</param>
        /// <returns>True se il file esiste, False altrimenti</returns>
        Task<bool> FileExistsAsync(
            string objectName,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ottiene le informazioni su un file (dimensione, content-type, metadata)
        /// </summary>
        /// <param name="objectName">Path completo nel bucket</param>
        /// <param name="cancellationToken">Token per la cancellazione dell'operazione</param>
        /// <returns>Oggetto con le informazioni del file</returns>
        Task<FileMetadata?> GetFileMetadataAsync(
            string objectName,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Lista tutti i file in un determinato path (prefix)
        /// </summary>
        /// <param name="prefix">Prefix path (es: "gare/a1b2c3.../lotti/b2c3d4...")</param>
        /// <param name="recursive">Se true, include anche le sottocartelle</param>
        /// <param name="cancellationToken">Token per la cancellazione dell'operazione</param>
        /// <returns>Lista dei path dei file trovati</returns>
        Task<IEnumerable<string>> ListFilesAsync(
            string prefix,
            bool recursive = false,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Genera un path MinIO secondo la struttura definita
        /// </summary>
        /// <param name="garaId">ID della gara (obbligatorio)</param>
        /// <param name="lottoId">ID del lotto (opzionale)</param>
        /// <param name="preventivoId">ID del preventivo (opzionale)</param>
        /// <param name="integrazioneId">ID dell'integrazione (opzionale)</param>
        /// <param name="fileName">Nome del file (senza GUID prefix)</param>
        /// <returns>Path completo per MinIO</returns>
        string GenerateObjectPath(
            Guid garaId,
            Guid? lottoId = null,
            Guid? preventivoId = null,
            Guid? integrazioneId = null,
            string fileName = "");

        /// <summary>
        /// Attualmente non utilizzato.
        /// Genera un URL presigned per l'accesso temporaneo a un file su Min
        /// </summary>
        /// <param name="objectName"></param>
        /// <param name="expiryMinutes"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<string> GetPresignedUrlAsync(string objectName, int expiryMinutes = 60, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Metadata di un file su MinIO
    /// </summary>
    public class FileMetadata
    {
        public string ObjectName { get; set; } = string.Empty;
        public long Size { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public DateTime LastModified { get; set; }
        public string ETag { get; set; } = string.Empty;
    }
}