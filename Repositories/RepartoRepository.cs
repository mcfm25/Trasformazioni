using Microsoft.EntityFrameworkCore;
using Trasformazioni.Data;
using Trasformazioni.Models.Entities;
using Trasformazioni.Repositories.Interfaces;

namespace Trasformazioni.Repositories
{
    /// <summary>
    /// Implementazione repository per l'entità Reparto
    /// </summary>
    public class RepartoRepository : IRepartoRepository
    {
        private readonly ApplicationDbContext _context;

        public RepartoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===================================
        // CRUD
        // ===================================

        public async Task<IEnumerable<Reparto>> GetAllAsync()
        {
            return await _context.Reparti
                .OrderBy(r => r.Nome)
                .ToListAsync();
        }

        public async Task<Reparto?> GetByIdAsync(Guid id)
        {
            return await _context.Reparti
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Reparto?> GetByIdWithUtentiAsync(Guid id)
        {
            return await _context.Reparti
                .Include(r => r.Utenti.Where(u => !u.IsDeleted))
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task AddAsync(Reparto reparto)
        {
            await _context.Reparti.AddAsync(reparto);
        }

        public Task UpdateAsync(Reparto reparto)
        {
            _context.Reparti.Update(reparto);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Reparto reparto)
        {
            // Soft delete
            reparto.IsDeleted = true;
            reparto.DeletedAt = DateTime.Now;
            _context.Reparti.Update(reparto);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        // ===================================
        // QUERY
        // ===================================

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Reparti.AnyAsync(r => r.Id == id);
        }

        public async Task<bool> IsNomeUniqueAsync(string nome, Guid? excludeId = null)
        {
            var query = _context.Reparti.Where(r => r.Nome.ToLower() == nome.ToLower());

            if (excludeId.HasValue)
                query = query.Where(r => r.Id != excludeId.Value);

            return !await query.AnyAsync();
        }

        public async Task<bool> IsEmailUniqueAsync(string email, Guid? excludeId = null)
        {
            var query = _context.Reparti.Where(r => r.Email.ToLower() == email.ToLower());

            if (excludeId.HasValue)
                query = query.Where(r => r.Id != excludeId.Value);

            return !await query.AnyAsync();
        }

        public async Task<int> CountUtentiAsync(Guid repartoId)
        {
            return await _context.Users
                .Where(u => u.RepartoId == repartoId && !u.IsDeleted)
                .CountAsync();
        }

        // ===================================
        // PAGINAZIONE
        // ===================================

        public async Task<(IEnumerable<Reparto> Items, int TotalCount)> GetPagedAsync(
            string? searchTerm = null,
            int pageNumber = 1,
            int pageSize = 10,
            string orderBy = "Nome",
            string orderDirection = "asc")
        {
            var query = _context.Reparti.AsQueryable();

            // Filtro ricerca
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower().Trim();
                query = query.Where(r =>
                    r.Nome.ToLower().Contains(term) ||
                    r.Email.ToLower().Contains(term) ||
                    (r.Descrizione != null && r.Descrizione.ToLower().Contains(term)));
            }

            // Conta totale
            var totalCount = await query.CountAsync();

            // Ordinamento
            query = orderBy.ToLower() switch
            {
                "email" => orderDirection == "asc"
                    ? query.OrderBy(r => r.Email)
                    : query.OrderByDescending(r => r.Email),
                "createdat" => orderDirection == "asc"
                    ? query.OrderBy(r => r.CreatedAt)
                    : query.OrderByDescending(r => r.CreatedAt),
                _ => orderDirection == "asc"
                    ? query.OrderBy(r => r.Nome)
                    : query.OrderByDescending(r => r.Nome)
            };

            // Paginazione
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
}