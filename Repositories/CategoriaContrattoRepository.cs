using Microsoft.EntityFrameworkCore;
using Trasformazioni.Models.Entities;

namespace Trasformazioni.Data.Repositories
{
    /// <summary>
    /// Implementazione del repository per l'accesso ai dati delle categorie contratto
    /// </summary>
    public class CategoriaContrattoRepository : ICategoriaContrattoRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoriaContrattoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===================================
        // OPERAZIONI BASE
        // ===================================

        /// <summary>
        /// Ottiene tutte le categorie non cancellate
        /// </summary>
        public async Task<IEnumerable<CategoriaContratto>> GetAllAsync()
        {
            return await _context.CategorieContratto
                .OrderBy(c => c.Ordine)
                .ThenBy(c => c.Nome)
                .ToListAsync();
        }

        /// <summary>
        /// Ottiene una categoria per ID
        /// </summary>
        public async Task<CategoriaContratto?> GetByIdAsync(Guid id)
        {
            return await _context.CategorieContratto
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        /// <summary>
        /// Aggiunge una nuova categoria
        /// </summary>
        public async Task AddAsync(CategoriaContratto categoria)
        {
            await _context.CategorieContratto.AddAsync(categoria);
        }

        /// <summary>
        /// Aggiorna una categoria esistente
        /// </summary>
        public Task UpdateAsync(CategoriaContratto categoria)
        {
            _context.CategorieContratto.Update(categoria);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Elimina una categoria (soft delete)
        /// </summary>
        public Task DeleteAsync(CategoriaContratto categoria)
        {
            categoria.IsDeleted = true;
            categoria.DeletedAt = DateTime.Now;
            _context.CategorieContratto.Update(categoria);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Salva le modifiche nel database
        /// </summary>
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Verifica se esiste una categoria con l'ID specificato
        /// </summary>
        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.CategorieContratto.AnyAsync(c => c.Id == id);
        }

        // ===================================
        // QUERY SPECIFICHE
        // ===================================

        /// <summary>
        /// Ottiene tutte le categorie attive ordinate
        /// </summary>
        public async Task<IEnumerable<CategoriaContratto>> GetAttiveAsync()
        {
            return await _context.CategorieContratto
                .Where(c => c.IsAttivo)
                .OrderBy(c => c.Ordine)
                .ThenBy(c => c.Nome)
                .ToListAsync();
        }

        /// <summary>
        /// Ottiene una categoria per nome
        /// </summary>
        public async Task<CategoriaContratto?> GetByNomeAsync(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                return null;

            return await _context.CategorieContratto
                .FirstOrDefaultAsync(c => c.Nome.ToLower() == nome.ToLower().Trim());
        }

        /// <summary>
        /// Verifica se esiste una categoria con il nome specificato
        /// </summary>
        public async Task<bool> ExistsByNomeAsync(string nome, Guid? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(nome))
                return false;

            var query = _context.CategorieContratto
                .Where(c => c.Nome.ToLower() == nome.ToLower().Trim());

            if (excludeId.HasValue)
                query = query.Where(c => c.Id != excludeId.Value);

            return await query.AnyAsync();
        }

        /// <summary>
        /// Ottiene il prossimo valore di ordine disponibile
        /// </summary>
        public async Task<int> GetNextOrdineAsync()
        {
            var maxOrdine = await _context.CategorieContratto
                .MaxAsync(c => (int?)c.Ordine) ?? 0;

            return maxOrdine + 1;
        }

        // ===================================
        // VALIDAZIONI
        // ===================================

        /// <summary>
        /// Verifica se la categoria è utilizzata in qualche registro
        /// </summary>
        public async Task<bool> IsUsedAsync(Guid id)
        {
            return await _context.RegistroContratti
                .AnyAsync(r => r.CategoriaContrattoId == id);
        }

        /// <summary>
        /// Conta quanti registri utilizzano questa categoria
        /// </summary>
        public async Task<int> CountUsageAsync(Guid id)
        {
            return await _context.RegistroContratti
                .CountAsync(r => r.CategoriaContrattoId == id);
        }

        // ===================================
        // STATISTICHE
        // ===================================

        /// <summary>
        /// Conta il numero totale di categorie
        /// </summary>
        public async Task<int> CountAsync()
        {
            return await _context.CategorieContratto.CountAsync();
        }

        /// <summary>
        /// Conta il numero di categorie attive
        /// </summary>
        public async Task<int> CountAttiveAsync()
        {
            return await _context.CategorieContratto
                .CountAsync(c => c.IsAttivo);
        }
    }
}