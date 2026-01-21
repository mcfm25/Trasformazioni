using Trasformazioni.Models.Entities;

namespace Trasformazioni.Data.Repositories
{
    /// <summary>
    /// Interfaccia per l'accesso ai dati delle categorie contratto
    /// </summary>
    public interface ICategoriaContrattoRepository
    {
        // ===================================
        // OPERAZIONI BASE
        // ===================================

        /// <summary>
        /// Ottiene tutte le categorie non cancellate
        /// </summary>
        Task<IEnumerable<CategoriaContratto>> GetAllAsync();

        /// <summary>
        /// Ottiene una categoria per ID
        /// </summary>
        Task<CategoriaContratto?> GetByIdAsync(Guid id);

        /// <summary>
        /// Aggiunge una nuova categoria
        /// </summary>
        Task AddAsync(CategoriaContratto categoria);

        /// <summary>
        /// Aggiorna una categoria esistente
        /// </summary>
        Task UpdateAsync(CategoriaContratto categoria);

        /// <summary>
        /// Elimina una categoria (soft delete)
        /// </summary>
        Task DeleteAsync(CategoriaContratto categoria);

        /// <summary>
        /// Salva le modifiche nel database
        /// </summary>
        Task<int> SaveChangesAsync();

        /// <summary>
        /// Verifica se esiste una categoria con l'ID specificato
        /// </summary>
        Task<bool> ExistsAsync(Guid id);

        // ===================================
        // QUERY SPECIFICHE
        // ===================================

        /// <summary>
        /// Ottiene tutte le categorie attive ordinate
        /// </summary>
        Task<IEnumerable<CategoriaContratto>> GetAttiveAsync();

        /// <summary>
        /// Ottiene una categoria per nome
        /// </summary>
        Task<CategoriaContratto?> GetByNomeAsync(string nome);

        /// <summary>
        /// Verifica se esiste una categoria con il nome specificato
        /// </summary>
        /// <param name="nome">Nome della categoria</param>
        /// <param name="excludeId">ID da escludere (per edit)</param>
        Task<bool> ExistsByNomeAsync(string nome, Guid? excludeId = null);

        /// <summary>
        /// Ottiene il prossimo valore di ordine disponibile
        /// </summary>
        Task<int> GetNextOrdineAsync();

        // ===================================
        // VALIDAZIONI
        // ===================================

        /// <summary>
        /// Verifica se la categoria è utilizzata in qualche registro
        /// </summary>
        Task<bool> IsUsedAsync(Guid id);

        /// <summary>
        /// Conta quanti registri utilizzano questa categoria
        /// </summary>
        Task<int> CountUsageAsync(Guid id);

        // ===================================
        // STATISTICHE
        // ===================================

        /// <summary>
        /// Conta il numero totale di categorie
        /// </summary>
        Task<int> CountAsync();

        /// <summary>
        /// Conta il numero di categorie attive
        /// </summary>
        Task<int> CountAttiveAsync();
    }
}