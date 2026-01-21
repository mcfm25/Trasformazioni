using Microsoft.EntityFrameworkCore;
using Trasformazioni.Data;
using Trasformazioni.Models.Entities;

namespace Trasformazioni.Data.Repositories
{
    /// <summary>
    /// Repository per l'accesso ai dati delle Elaborazioni Lotto
    /// Implementa le operazioni CRUD e query specifiche per le elaborazioni
    /// </summary>
    public class ElaborazioneLottoRepository : IElaborazioneLottoRepository
    {
        private readonly ApplicationDbContext _context;

        public ElaborazioneLottoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===================================
        // OPERAZIONI BASE (CRUD)
        // ===================================

        public async Task<IEnumerable<ElaborazioneLotto>> GetAllAsync()
        {
            return await _context.ElaborazioniLotti
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
        }

        public async Task<ElaborazioneLotto?> GetByIdAsync(Guid id)
        {
            return await _context.ElaborazioniLotti
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<ElaborazioneLotto> AddAsync(ElaborazioneLotto elaborazione)
        {
            await _context.ElaborazioniLotti.AddAsync(elaborazione);
            await _context.SaveChangesAsync();
            return elaborazione;
        }

        public async Task UpdateAsync(ElaborazioneLotto elaborazione)
        {
            _context.ElaborazioniLotti.Update(elaborazione);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var elaborazione = await GetByIdAsync(id);
            if (elaborazione != null)
            {
                _context.ElaborazioniLotti.Remove(elaborazione); // Soft delete gestito dall'interceptor
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.ElaborazioniLotti.AnyAsync(e => e.Id == id);
        }

        // ===================================
        // OPERAZIONI CON RELAZIONI
        // ===================================

        public async Task<ElaborazioneLotto?> GetWithLottoAsync(Guid id)
        {
            return await _context.ElaborazioniLotti
                .Include(e => e.Lotto)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<ElaborazioneLotto?> GetCompleteAsync(Guid id)
        {
            return await _context.ElaborazioniLotti
                .Include(e => e.Lotto)
                    .ThenInclude(l => l.Gara)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        // ===================================
        // RICERCHE SPECIFICHE
        // ===================================

        public async Task<ElaborazioneLotto?> GetByLottoIdAsync(Guid lottoId)
        {
            return await _context.ElaborazioniLotti
                .Include(e => e.Lotto)
                    .ThenInclude(l => l.Gara)
                .FirstOrDefaultAsync(e => e.LottoId == lottoId);
        }

        // ===================================
        // RICERCHE PER PREZZO
        // ===================================

        public async Task<IEnumerable<ElaborazioneLotto>> GetByPrezzoDesideratoRangeAsync(decimal min, decimal max)
        {
            return await _context.ElaborazioniLotti
                .Include(e => e.Lotto)
                    .ThenInclude(l => l.Gara)
                .Where(e => e.PrezzoDesiderato.HasValue &&
                           e.PrezzoDesiderato >= min &&
                           e.PrezzoDesiderato <= max)
                .OrderBy(e => e.PrezzoDesiderato)
                .ToListAsync();
        }

        public async Task<IEnumerable<ElaborazioneLotto>> GetByPrezzoRealeRangeAsync(decimal min, decimal max)
        {
            return await _context.ElaborazioniLotti
                .Include(e => e.Lotto)
                    .ThenInclude(l => l.Gara)
                .Where(e => e.PrezzoRealeUscita.HasValue &&
                           e.PrezzoRealeUscita >= min &&
                           e.PrezzoRealeUscita <= max)
                .OrderBy(e => e.PrezzoRealeUscita)
                .ToListAsync();
        }

        public async Task<IEnumerable<ElaborazioneLotto>> GetWithScostamentoAsync()
        {
            return await _context.ElaborazioniLotti
                .Include(e => e.Lotto)
                    .ThenInclude(l => l.Gara)
                .Where(e => e.PrezzoDesiderato.HasValue &&
                           e.PrezzoRealeUscita.HasValue &&
                           e.PrezzoDesiderato != e.PrezzoRealeUscita)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ElaborazioneLotto>> GetConPrezzoRealeSuperioreAsync()
        {
            return await _context.ElaborazioniLotti
                .Include(e => e.Lotto)
                    .ThenInclude(l => l.Gara)
                .Where(e => e.PrezzoDesiderato.HasValue &&
                           e.PrezzoRealeUscita.HasValue &&
                           e.PrezzoRealeUscita > e.PrezzoDesiderato)
                .OrderByDescending(e => e.PrezzoRealeUscita - e.PrezzoDesiderato)
                .ToListAsync();
        }

        public async Task<IEnumerable<ElaborazioneLotto>> GetConPrezzoRealeInferioreAsync()
        {
            return await _context.ElaborazioniLotti
                .Include(e => e.Lotto)
                    .ThenInclude(l => l.Gara)
                .Where(e => e.PrezzoDesiderato.HasValue &&
                           e.PrezzoRealeUscita.HasValue &&
                           e.PrezzoRealeUscita < e.PrezzoDesiderato)
                .OrderByDescending(e => e.PrezzoDesiderato - e.PrezzoRealeUscita)
                .ToListAsync();
        }

        public async Task<IEnumerable<ElaborazioneLotto>> GetSenzaPrezziAsync()
        {
            return await _context.ElaborazioniLotti
                .Include(e => e.Lotto)
                    .ThenInclude(l => l.Gara)
                .Where(e => !e.PrezzoDesiderato.HasValue && !e.PrezzoRealeUscita.HasValue)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
        }

        // ===================================
        // VALIDAZIONI / ESISTENZA
        // ===================================

        public async Task<bool> ExistsByLottoIdAsync(Guid lottoId, Guid? excludeId = null)
        {
            var query = _context.ElaborazioniLotti.Where(e => e.LottoId == lottoId);

            if (excludeId.HasValue)
            {
                query = query.Where(e => e.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        // ===================================
        // STATISTICHE
        // ===================================

        public async Task<decimal?> GetScostamentoMedioAsync()
        {
            var elaborazioni = await _context.ElaborazioniLotti
                .Where(e => e.PrezzoDesiderato.HasValue &&
                           e.PrezzoRealeUscita.HasValue &&
                           e.PrezzoDesiderato > 0)
                .ToListAsync();

            if (!elaborazioni.Any())
                return null;

            // Calcola scostamento percentuale medio
            var scostamenti = elaborazioni
                .Select(e => Math.Abs((e.PrezzoRealeUscita!.Value - e.PrezzoDesiderato!.Value) / e.PrezzoDesiderato.Value * 100))
                .ToList();

            return scostamenti.Any() ? (decimal?)scostamenti.Average() : null;
        }

        public async Task<int> GetCountWithScostamentoAsync()
        {
            return await _context.ElaborazioniLotti
                .Where(e => e.PrezzoDesiderato.HasValue &&
                           e.PrezzoRealeUscita.HasValue &&
                           e.PrezzoDesiderato != e.PrezzoRealeUscita)
                .CountAsync();
        }

        public async Task<Dictionary<string, int>> GetStatistichePrezziAsync()
        {
            var allElaborazioni = await _context.ElaborazioniLotti.ToListAsync();

            return new Dictionary<string, int>
            {
                ["TotaleElaborazioni"] = allElaborazioni.Count,
                ["ConPrezzoDesiderato"] = allElaborazioni.Count(e => e.PrezzoDesiderato.HasValue),
                ["ConPrezzoReale"] = allElaborazioni.Count(e => e.PrezzoRealeUscita.HasValue),
                ["ConEntrambiPrezzi"] = allElaborazioni.Count(e => e.PrezzoDesiderato.HasValue && e.PrezzoRealeUscita.HasValue),
                ["ConScostamento"] = allElaborazioni.Count(e => e.PrezzoDesiderato.HasValue &&
                                                                e.PrezzoRealeUscita.HasValue &&
                                                                e.PrezzoDesiderato != e.PrezzoRealeUscita),
                ["PrezzoRealeSuperiore"] = allElaborazioni.Count(e => e.PrezzoDesiderato.HasValue &&
                                                                     e.PrezzoRealeUscita.HasValue &&
                                                                     e.PrezzoRealeUscita > e.PrezzoDesiderato),
                ["PrezzoRealeInferiore"] = allElaborazioni.Count(e => e.PrezzoDesiderato.HasValue &&
                                                                     e.PrezzoRealeUscita.HasValue &&
                                                                     e.PrezzoRealeUscita < e.PrezzoDesiderato)
            };
        }

        public async Task<int> CountByLottoIdAsync(Guid lottoId)
        {
            return await _context.ElaborazioniLotti
                .CountAsync(d => d.LottoId == lottoId && !d.IsDeleted);
        }
    }
}