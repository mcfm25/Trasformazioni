using Trasformazioni.Models.ViewModels;

namespace Trasformazioni.Services.Interfaces
{
    /// <summary>
    /// Interfaccia per la business logic della gestione allegati del Registro Contratti
    /// </summary>
    public interface IAllegatoRegistroService
    {
        // ===================================
        // QUERY - LETTURA
        // ===================================

        /// <summary>
        /// Ottiene tutti gli allegati di un registro
        /// </summary>
        Task<IEnumerable<AllegatoRegistroListViewModel>> GetByRegistroIdAsync(Guid registroContrattiId);

        /// <summary>
        /// Ottiene il dettaglio di un allegato
        /// </summary>
        Task<AllegatoRegistroDetailsViewModel?> GetByIdAsync(Guid id);

        /// <summary>
        /// Conta gli allegati di un registro
        /// </summary>
        Task<int> CountByRegistroIdAsync(Guid registroContrattiId);

        // ===================================
        // UPLOAD
        // ===================================

        /// <summary>
        /// Carica un nuovo allegato
        /// </summary>
        /// <returns>(Success, ErrorMessage, AllegatoId)</returns>
        Task<(bool Success, string? ErrorMessage, Guid? AllegatoId)> UploadAsync(
            AllegatoRegistroUploadViewModel model,
            string currentUserId);

        /// <summary>
        /// Carica più allegati contemporaneamente
        /// </summary>
        /// <returns>(Success, ErrorMessage, AllegatiIds)</returns>
        Task<(bool Success, string? ErrorMessage, List<Guid>? AllegatiIds)> UploadMultiploAsync(
            AllegatoRegistroUploadMultiploViewModel model,
            string currentUserId);

        // ===================================
        // DOWNLOAD
        // ===================================

        /// <summary>
        /// Ottiene lo stream del file per il download
        /// </summary>
        /// <returns>(Success, ErrorMessage, FileStream, FileName, MimeType)</returns>
        Task<(bool Success, string? ErrorMessage, Stream? FileStream, string? FileName, string? MimeType)> DownloadAsync(Guid id);

        ///// <summary>
        ///// Ottiene l'URL temporaneo per il download diretto
        ///// </summary>
        ///// <returns>(Success, ErrorMessage, Url)</returns>
        //Task<(bool Success, string? ErrorMessage, string? Url)> GetDownloadUrlAsync(Guid id, int expiryMinutes = 60);

        // ===================================
        // MODIFICA
        // ===================================

        /// <summary>
        /// Aggiorna la descrizione di un allegato
        /// </summary>
        Task<(bool Success, string? ErrorMessage)> UpdateDescrizioneAsync(
            Guid id,
            string? descrizione,
            string currentUserId);

        /// <summary>
        /// Aggiorna il tipo documento di un allegato
        /// </summary>
        Task<(bool Success, string? ErrorMessage)> UpdateTipoDocumentoAsync(
            Guid id,
            Guid tipoDocumentoId,
            string currentUserId);

        // ===================================
        // ELIMINAZIONE
        // ===================================

        /// <summary>
        /// Elimina un allegato (soft delete + rimozione da MinIO)
        /// </summary>
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(Guid id, string currentUserId);

        /// <summary>
        /// Elimina tutti gli allegati di un registro
        /// </summary>
        Task<(bool Success, string? ErrorMessage, int DeletedCount)> DeleteByRegistroIdAsync(
            Guid registroContrattiId,
            string currentUserId);

        // ===================================
        // VALIDAZIONI
        // ===================================

        /// <summary>
        /// Valida un file per l'upload
        /// </summary>
        /// <returns>(IsValid, ErrorMessage)</returns>
        (bool IsValid, string? ErrorMessage) ValidateFile(
            Microsoft.AspNetCore.Http.IFormFile file);

        /// <summary>
        /// Verifica se un allegato esiste
        /// </summary>
        Task<bool> ExistsAsync(Guid id);

        /// <summary>
        /// Verifica se esiste già un file con lo stesso nome nel registro
        /// </summary>
        Task<bool> ExistsByNomeFileAsync(Guid registroContrattiId, string nomeFile, Guid? excludeId = null);

        // ===================================
        // CLEANUP
        // ===================================

        /// <summary>
        /// Rimuove allegati con upload incompleto più vecchi di X ore
        /// </summary>
        /// <returns>Numero di allegati rimossi</returns>
        Task<int> CleanupUploadIncompletiAsync(int oreVecchiaia = 24);

        // ===================================
        // STATISTICHE
        // ===================================

        /// <summary>
        /// Ottiene la dimensione totale degli allegati di un registro
        /// </summary>
        Task<long> GetTotalSizeByRegistroIdAsync(Guid registroContrattiId);

        /// <summary>
        /// Ottiene statistiche sugli allegati
        /// </summary>
        Task<AllegatoRegistroStatisticheViewModel> GetStatisticheAsync();

        // ===================================
        // PREPARAZIONE VIEWMODEL
        // ===================================

        /// <summary>
        /// Prepara il ViewModel per l'upload
        /// </summary>
        Task<AllegatoRegistroUploadViewModel?> PrepareUploadViewModelAsync(Guid registroContrattiId);

        /// <summary>
        /// Prepara il ViewModel per l'upload multiplo
        /// </summary>
        Task<AllegatoRegistroUploadMultiploViewModel?> PrepareUploadMultiploViewModelAsync(Guid registroContrattiId);
    }

    /// <summary>
    /// ViewModel per le statistiche degli allegati
    /// </summary>
    public class AllegatoRegistroStatisticheViewModel
    {
        /// <summary>
        /// Numero totale di allegati
        /// </summary>
        public int TotaleAllegati { get; set; }

        /// <summary>
        /// Dimensione totale in bytes
        /// </summary>
        public long DimensioneTotaleBytes { get; set; }

        /// <summary>
        /// Dimensione totale formattata
        /// </summary>
        public string DimensioneTotaleFormattata
        {
            get
            {
                string[] sizes = { "B", "KB", "MB", "GB", "TB" };
                double len = DimensioneTotaleBytes;
                int order = 0;

                while (len >= 1024 && order < sizes.Length - 1)
                {
                    order++;
                    len = len / 1024;
                }

                return $"{len:0.##} {sizes[order]}";
            }
        }

        /// <summary>
        /// Numero di upload incompleti
        /// </summary>
        public int UploadIncompleti { get; set; }

        /// <summary>
        /// Distribuzione per tipo MIME
        /// </summary>
        public Dictionary<string, int> DistribuzionePerTipo { get; set; } = new();
    }
}