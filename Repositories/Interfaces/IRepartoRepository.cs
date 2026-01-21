using Trasformazioni.Models.Entities;

namespace Trasformazioni.Repositories.Interfaces
{
    /// <summary>
    /// Interfaccia repository per l'entità Reparto
    /// </summary>
    public interface IRepartoRepository
    {
        // CRUD
        Task<IEnumerable<Reparto>> GetAllAsync();
        Task<Reparto?> GetByIdAsync(Guid id);
        Task<Reparto?> GetByIdWithUtentiAsync(Guid id);
        Task AddAsync(Reparto reparto);
        Task UpdateAsync(Reparto reparto);
        Task DeleteAsync(Reparto reparto);
        Task SaveChangesAsync();

        // Query
        Task<bool> ExistsAsync(Guid id);
        Task<bool> IsNomeUniqueAsync(string nome, Guid? excludeId = null);
        Task<bool> IsEmailUniqueAsync(string email, Guid? excludeId = null);
        Task<int> CountUtentiAsync(Guid repartoId);

        // Paginazione
        Task<(IEnumerable<Reparto> Items, int TotalCount)> GetPagedAsync(
            string? searchTerm = null,
            int pageNumber = 1,
            int pageSize = 10,
            string orderBy = "Nome",
            string orderDirection = "asc");
    }
}