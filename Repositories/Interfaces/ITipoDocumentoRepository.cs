using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;

namespace Trasformazioni.Data.Repositories
{
    /// <summary>
    /// Interfaccia repository per la gestione dei TipiDocumento.
    /// Fornisce operazioni CRUD e query specifiche per i tipi documento.
    /// </summary>
    public interface ITipoDocumentoRepository
    {
        // ===================================
        // QUERY - LETTURA
        // ===================================

        /// <summary>
        /// Ottiene tutti i tipi documento attivi
        /// </summary>
        Task<IEnumerable<TipoDocumento>> GetAllAsync();

        /// <summary>
        /// Ottiene un tipo documento per ID
        /// </summary>
        Task<TipoDocumento?> GetByIdAsync(Guid id);

        /// <summary>
        /// Ottiene tutti i tipi documento per una specifica area
        /// </summary>
        /// <param name="area">Area documento</param>
        Task<IEnumerable<TipoDocumento>> GetByAreaAsync(AreaDocumento area);

        /// <summary>
        /// Ottiene tutti i tipi documento di sistema
        /// </summary>
        Task<IEnumerable<TipoDocumento>> GetSystemTypesAsync();

        /// <summary>
        /// Ottiene tutti i tipi documento personalizzati (non di sistema)
        /// </summary>
        Task<IEnumerable<TipoDocumento>> GetCustomTypesAsync();

        /// <summary>
        /// Ottiene i tipi documento di sistema per una specifica area
        /// </summary>
        Task<IEnumerable<TipoDocumento>> GetSystemTypesByAreaAsync(AreaDocumento area);

        /// <summary>
        /// Verifica se esiste un tipo documento con lo stesso nome nella stessa area
        /// </summary>
        /// <param name="nome">Nome da verificare</param>
        /// <param name="area">Area di appartenenza</param>
        /// <param name="excludeId">ID da escludere (per update)</param>
        Task<bool> ExistsByNomeAndAreaAsync(string nome, AreaDocumento area, Guid? excludeId = null);

        /// <summary>
        /// Verifica se un tipo documento è utilizzato da documenti
        /// </summary>
        /// <param name="id">ID del tipo documento</param>
        Task<bool> IsInUseAsync(Guid id);

        /// <summary>
        /// Ottiene il conteggio dei documenti che usano questo tipo
        /// </summary>
        /// <param name="id">ID del tipo documento</param>
        Task<int> GetDocumentiCountAsync(Guid id);

        // ===================================
        // COMANDI - SCRITTURA
        // ===================================

        /// <summary>
        /// Aggiunge un nuovo tipo documento
        /// </summary>
        Task<TipoDocumento> AddAsync(TipoDocumento tipoDocumento);

        /// <summary>
        /// Aggiorna un tipo documento esistente
        /// </summary>
        Task UpdateAsync(TipoDocumento tipoDocumento);

        /// <summary>
        /// Elimina (soft delete) un tipo documento
        /// </summary>
        Task DeleteAsync(TipoDocumento tipoDocumento);

        /// <summary>
        /// Salva le modifiche nel database
        /// </summary>
        Task<int> SaveChangesAsync();

        // ===================================
        // STATISTICHE
        // ===================================

        /// <summary>
        /// Ottiene il conteggio totale dei tipi documento per area
        /// </summary>
        Task<Dictionary<AreaDocumento, int>> GetCountByAreaAsync();

        /// <summary>
        /// Ottiene statistiche generali sui tipi documento
        /// </summary>
        Task<Dictionary<string, int>> GetStatisticheAsync();
    }
}
