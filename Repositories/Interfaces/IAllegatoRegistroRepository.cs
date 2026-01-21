using Trasformazioni.Models.Entities;

namespace Trasformazioni.Data.Repositories
{
    /// <summary>
    /// Interfaccia per l'accesso ai dati degli allegati del Registro Contratti
    /// </summary>
    public interface IAllegatoRegistroRepository
    {
        // ===================================
        // OPERAZIONI BASE
        // ===================================

        /// <summary>
        /// Ottiene tutti gli allegati non cancellati
        /// </summary>
        Task<IEnumerable<AllegatoRegistro>> GetAllAsync();

        /// <summary>
        /// Ottiene un allegato per ID
        /// </summary>
        Task<AllegatoRegistro?> GetByIdAsync(Guid id);

        /// <summary>
        /// Aggiunge un nuovo allegato
        /// </summary>
        Task AddAsync(AllegatoRegistro allegato);

        /// <summary>
        /// Aggiorna un allegato esistente
        /// </summary>
        Task UpdateAsync(AllegatoRegistro allegato);

        /// <summary>
        /// Elimina un allegato (soft delete)
        /// </summary>
        Task DeleteAsync(AllegatoRegistro allegato);

        /// <summary>
        /// Salva le modifiche nel database
        /// </summary>
        Task<int> SaveChangesAsync();

        /// <summary>
        /// Verifica se esiste un allegato con l'ID specificato
        /// </summary>
        Task<bool> ExistsAsync(Guid id);

        // ===================================
        // QUERY PER REGISTRO
        // ===================================

        /// <summary>
        /// Ottiene tutti gli allegati di un registro
        /// </summary>
        Task<IEnumerable<AllegatoRegistro>> GetByRegistroIdAsync(Guid registroContrattiId);

        /// <summary>
        /// Ottiene allegati di un registro per tipo documento
        /// </summary>
        Task<IEnumerable<AllegatoRegistro>> GetByRegistroIdAndTipoAsync(Guid registroContrattiId, Guid tipoDocumentoId);

        /// <summary>
        /// Conta gli allegati di un registro
        /// </summary>
        Task<int> CountByRegistroIdAsync(Guid registroContrattiId);

        // ===================================
        // QUERY PER TIPO DOCUMENTO
        // ===================================

        /// <summary>
        /// Ottiene tutti gli allegati per tipo documento
        /// </summary>
        Task<IEnumerable<AllegatoRegistro>> GetByTipoDocumentoIdAsync(Guid tipoDocumentoId);

        // ===================================
        // QUERY PER UTENTE
        // ===================================

        /// <summary>
        /// Ottiene tutti gli allegati caricati da un utente
        /// </summary>
        Task<IEnumerable<AllegatoRegistro>> GetByUtenteIdAsync(string utenteId);

        // ===================================
        // QUERY PER STATO UPLOAD
        // ===================================

        /// <summary>
        /// Ottiene allegati con upload incompleto (per cleanup)
        /// </summary>
        Task<IEnumerable<AllegatoRegistro>> GetUploadIncompleti();

        /// <summary>
        /// Ottiene allegati con upload incompleto più vecchi di una certa data
        /// </summary>
        Task<IEnumerable<AllegatoRegistro>> GetUploadIncompletiOlderThanAsync(DateTime dataLimite);

        // ===================================
        // RICERCHE
        // ===================================

        /// <summary>
        /// Cerca allegati per nome file
        /// </summary>
        Task<IEnumerable<AllegatoRegistro>> SearchByNomeFileAsync(string searchTerm);

        /// <summary>
        /// Ottiene un allegato per path MinIO
        /// </summary>
        Task<AllegatoRegistro?> GetByPathMinIOAsync(string pathMinIO);

        // ===================================
        // VALIDAZIONI
        // ===================================

        /// <summary>
        /// Verifica se esiste già un allegato con lo stesso nome nello stesso registro
        /// </summary>
        Task<bool> ExistsByNomeFileInRegistroAsync(Guid registroContrattiId, string nomeFile, Guid? excludeId = null);

        // ===================================
        // STATISTICHE
        // ===================================

        /// <summary>
        /// Conta il numero totale di allegati
        /// </summary>
        Task<int> CountAsync();

        /// <summary>
        /// Calcola la dimensione totale degli allegati di un registro (in bytes)
        /// </summary>
        Task<long> GetTotalSizeByRegistroIdAsync(Guid registroContrattiId);

        /// <summary>
        /// Calcola la dimensione totale di tutti gli allegati (in bytes)
        /// </summary>
        Task<long> GetTotalSizeAsync();
    }
}