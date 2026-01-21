using Microsoft.EntityFrameworkCore;
using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;

namespace Trasformazioni.Data.Repositories
{
    /// <summary>
    /// Implementazione repository per la gestione dei TipiDocumento.
    /// Utilizza Entity Framework Core per l'accesso ai dati.
    /// </summary>
    public class TipoDocumentoRepository : ITipoDocumentoRepository
    {
        private readonly ApplicationDbContext _context;

        public TipoDocumentoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===================================
        // QUERY - LETTURA
        // ===================================

        public async Task<IEnumerable<TipoDocumento>> GetAllAsync()
        {
            return await _context.TipiDocumento
                .OrderBy(t => t.Area)
                .ThenBy(t => t.Nome)
                .ToListAsync();
        }

        public async Task<TipoDocumento?> GetByIdAsync(Guid id)
        {
            return await _context.TipiDocumento
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<TipoDocumento>> GetByAreaAsync(AreaDocumento area)
        {
            return await _context.TipiDocumento
                .Where(t => t.Area == area)
                .OrderBy(t => t.Nome)
                .ToListAsync();
        }

        public async Task<IEnumerable<TipoDocumento>> GetSystemTypesAsync()
        {
            return await _context.TipiDocumento
                .Where(t => t.IsSystem)
                .OrderBy(t => t.Area)
                .ThenBy(t => t.Nome)
                .ToListAsync();
        }

        public async Task<IEnumerable<TipoDocumento>> GetCustomTypesAsync()
        {
            return await _context.TipiDocumento
                .Where(t => !t.IsSystem)
                .OrderBy(t => t.Area)
                .ThenBy(t => t.Nome)
                .ToListAsync();
        }

        public async Task<IEnumerable<TipoDocumento>> GetSystemTypesByAreaAsync(AreaDocumento area)
        {
            return await _context.TipiDocumento
                .Where(t => t.IsSystem && t.Area == area)
                .OrderBy(t => t.Nome)
                .ToListAsync();
        }

        public async Task<bool> ExistsByNomeAndAreaAsync(string nome, AreaDocumento area, Guid? excludeId = null)
        {
            var query = _context.TipiDocumento
                .Where(t => t.Nome.ToLower() == nome.ToLower() && t.Area == area);

            if (excludeId.HasValue)
            {
                query = query.Where(t => t.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> IsInUseAsync(Guid id)
        {
            // Verifica se è usato in DocumentiGara
            var usedInGara = await _context.DocumentiGara
                .AnyAsync(d => d.TipoDocumentoId == id);

            if (usedInGara) return true;

            // Future: aggiungere controlli per DocumentiLotto, DocumentiMezzo, ecc.

            return false;
        }

        public async Task<int> GetDocumentiCountAsync(Guid id)
        {
            var count = 0;

            // Conta DocumentiGara
            count += await _context.DocumentiGara
                .CountAsync(d => d.TipoDocumentoId == id);

            // Future: aggiungere conteggi per DocumentiLotto, DocumentiMezzo, ecc.

            return count;
        }

        // ===================================
        // COMANDI - SCRITTURA
        // ===================================

        public async Task<TipoDocumento> AddAsync(TipoDocumento tipoDocumento)
        {
            await _context.TipiDocumento.AddAsync(tipoDocumento);
            return tipoDocumento;
        }

        public Task UpdateAsync(TipoDocumento tipoDocumento)
        {
            _context.TipiDocumento.Update(tipoDocumento);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(TipoDocumento tipoDocumento)
        {
            // Soft delete gestito dall'AuditInterceptor
            tipoDocumento.IsDeleted = true;
            tipoDocumento.DeletedAt = DateTime.Now;
            _context.TipiDocumento.Update(tipoDocumento);
            return Task.CompletedTask;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // ===================================
        // STATISTICHE
        // ===================================

        public async Task<Dictionary<AreaDocumento, int>> GetCountByAreaAsync()
        {
            return await _context.TipiDocumento
                .GroupBy(t => t.Area)
                .Select(g => new { Area = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Area, x => x.Count);
        }

        public async Task<Dictionary<string, int>> GetStatisticheAsync()
        {
            var tutti = await _context.TipiDocumento.ToListAsync();

            return new Dictionary<string, int>
            {
                ["Totale"] = tutti.Count,
                ["DiSistema"] = tutti.Count(t => t.IsSystem),
                ["Personalizzati"] = tutti.Count(t => !t.IsSystem),
                ["AreaAzienda"] = tutti.Count(t => t.Area == AreaDocumento.Azienda),
                ["AreaGare"] = tutti.Count(t => t.Area == AreaDocumento.Gare),
                ["AreaLotti"] = tutti.Count(t => t.Area == AreaDocumento.Lotti),
                ["AreaMezzi"] = tutti.Count(t => t.Area == AreaDocumento.Mezzi),
                ["AreaSoggetti"] = tutti.Count(t => t.Area == AreaDocumento.Soggetti),
                ["AreaScadenze"] = tutti.Count(t => t.Area == AreaDocumento.Scadenze)
            };
        }
    }
}
