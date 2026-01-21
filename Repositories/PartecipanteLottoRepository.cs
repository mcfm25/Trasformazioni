using Microsoft.EntityFrameworkCore;
using Trasformazioni.Data;
using Trasformazioni.Models.Entities;

namespace Trasformazioni.Data.Repositories
{
    /// <summary>
    /// Repository per l'accesso ai dati dei Partecipanti Lotto
    /// Implementa le operazioni CRUD e query specifiche per il censimento partecipanti
    /// </summary>
    public class PartecipanteLottoRepository : IPartecipanteLottoRepository
    {
        private readonly ApplicationDbContext _context;

        public PartecipanteLottoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===================================
        // OPERAZIONI BASE (CRUD)
        // ===================================

        public async Task<IEnumerable<PartecipanteLotto>> GetAllAsync()
        {
            return await _context.PartecipantiLotti
                .Include(p => p.Lotto)
                .Include(p => p.Soggetto)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<PartecipanteLotto?> GetByIdAsync(Guid id)
        {
            return await _context.PartecipantiLotti
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<PartecipanteLotto> AddAsync(PartecipanteLotto partecipante)
        {
            await _context.PartecipantiLotti.AddAsync(partecipante);
            await _context.SaveChangesAsync();
            return partecipante;
        }

        public async Task UpdateAsync(PartecipanteLotto partecipante)
        {
            _context.PartecipantiLotti.Update(partecipante);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var partecipante = await GetByIdAsync(id);
            if (partecipante != null)
            {
                _context.PartecipantiLotti.Remove(partecipante); // Soft delete gestito dall'interceptor
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.PartecipantiLotti.AnyAsync(p => p.Id == id);
        }

        // ===================================
        // OPERAZIONI CON RELAZIONI
        // ===================================

        /// <summary>
        /// Verifica se un lotto ha già un aggiudicatario
        /// </summary>
        public async Task<bool> HasAggiudicatarioAsync(Guid lottoId, Guid? excludeId = null)
        {
            var query = _context.PartecipantiLotti.Where(p => p.LottoId == lottoId && p.IsAggiudicatario);

            if (excludeId.HasValue)
            {
                query = query.Where(p => p.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<PartecipanteLotto?> GetWithLottoAsync(Guid id)
        {
            return await _context.PartecipantiLotti
                .Include(p => p.Lotto)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<PartecipanteLotto?> GetWithSoggettoAsync(Guid id)
        {
            return await _context.PartecipantiLotti
                .Include(p => p.Soggetto)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<PartecipanteLotto?> GetCompleteAsync(Guid id)
        {
            return await _context.PartecipantiLotti
                .Include(p => p.Lotto)
                    .ThenInclude(l => l.Gara)
                .Include(p => p.Soggetto)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        // ===================================
        // RICERCHE PER LOTTO
        // ===================================

        public async Task<IEnumerable<PartecipanteLotto>> GetByLottoIdAsync(Guid lottoId)
        {
            return await _context.PartecipantiLotti
                .Include(p => p.Soggetto)
                .Where(p => p.LottoId == lottoId)
                .OrderBy(p => p.OffertaEconomica)
                .ToListAsync();
        }

        public async Task<PartecipanteLotto?> GetAggiudicatarioByLottoIdAsync(Guid lottoId)
        {
            return await _context.PartecipantiLotti
                .Include(p => p.Soggetto)
                .FirstOrDefaultAsync(p => p.LottoId == lottoId && p.IsAggiudicatario);
        }

        public async Task<IEnumerable<PartecipanteLotto>> GetScartatiByLottoIdAsync(Guid lottoId)
        {
            return await _context.PartecipantiLotti
                .Include(p => p.Soggetto)
                .Where(p => p.LottoId == lottoId && p.IsScartatoDallEnte)
                .OrderBy(p => p.RagioneSociale)
                .ToListAsync();
        }

        public async Task<IEnumerable<PartecipanteLotto>> GetNonAggiudicatariByLottoIdAsync(Guid lottoId)
        {
            return await _context.PartecipantiLotti
                .Include(p => p.Soggetto)
                .Where(p => p.LottoId == lottoId && !p.IsAggiudicatario)
                .OrderBy(p => p.OffertaEconomica)
                .ToListAsync();
        }

        public async Task<IEnumerable<PartecipanteLotto>> GetByLottoIdOrderedByOffertaAsync(Guid lottoId)
        {
            return await _context.PartecipantiLotti
                .Include(p => p.Soggetto)
                .Where(p => p.LottoId == lottoId && p.OffertaEconomica.HasValue)
                .OrderBy(p => p.OffertaEconomica)
                .ToListAsync();
        }

        // ===================================
        // RICERCHE PER SOGGETTO
        // ===================================

        public async Task<IEnumerable<PartecipanteLotto>> GetBySoggettoIdAsync(Guid soggettoId)
        {
            return await _context.PartecipantiLotti
                .Include(p => p.Lotto)
                    .ThenInclude(l => l.Gara)
                .Where(p => p.SoggettoId == soggettoId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<PartecipanteLotto>> GetVinteBySoggettoIdAsync(Guid soggettoId)
        {
            return await _context.PartecipantiLotti
                .Include(p => p.Lotto)
                    .ThenInclude(l => l.Gara)
                .Where(p => p.SoggettoId == soggettoId && p.IsAggiudicatario)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<PartecipanteLotto>> GetScartateBySoggettoIdAsync(Guid soggettoId)
        {
            return await _context.PartecipantiLotti
                .Include(p => p.Lotto)
                    .ThenInclude(l => l.Gara)
                .Where(p => p.SoggettoId == soggettoId && p.IsScartatoDallEnte)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        // ===================================
        // RICERCHE PER STATO
        // ===================================

        public async Task<IEnumerable<PartecipanteLotto>> GetAggiudicatariAsync()
        {
            return await _context.PartecipantiLotti
                .Include(p => p.Lotto)
                    .ThenInclude(l => l.Gara)
                .Include(p => p.Soggetto)
                .Where(p => p.IsAggiudicatario)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<PartecipanteLotto>> GetScartatiAsync()
        {
            return await _context.PartecipantiLotti
                .Include(p => p.Lotto)
                    .ThenInclude(l => l.Gara)
                .Include(p => p.Soggetto)
                .Where(p => p.IsScartatoDallEnte)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<PartecipanteLotto>> GetNonAggiudicatariAsync()
        {
            return await _context.PartecipantiLotti
                .Include(p => p.Lotto)
                    .ThenInclude(l => l.Gara)
                .Include(p => p.Soggetto)
                .Where(p => !p.IsAggiudicatario && !p.IsScartatoDallEnte)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        // ===================================
        // RICERCHE PER OFFERTA
        // ===================================

        public async Task<IEnumerable<PartecipanteLotto>> GetByOffertaRangeAsync(decimal min, decimal max)
        {
            return await _context.PartecipantiLotti
                .Include(p => p.Lotto)
                    .ThenInclude(l => l.Gara)
                .Include(p => p.Soggetto)
                .Where(p => p.OffertaEconomica.HasValue &&
                           p.OffertaEconomica >= min &&
                           p.OffertaEconomica <= max)
                .OrderBy(p => p.OffertaEconomica)
                .ToListAsync();
        }

        public async Task<IEnumerable<PartecipanteLotto>> GetSenzaOffertaAsync()
        {
            return await _context.PartecipantiLotti
                .Include(p => p.Lotto)
                    .ThenInclude(l => l.Gara)
                .Include(p => p.Soggetto)
                .Where(p => !p.OffertaEconomica.HasValue)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        // ===================================
        // STATISTICHE
        // ===================================

        public async Task<int> GetCountByLottoAsync(Guid lottoId)
        {
            return await _context.PartecipantiLotti
                .Where(p => p.LottoId == lottoId)
                .CountAsync();
        }

        public async Task<decimal?> GetMediaOffertaByLottoAsync(Guid lottoId)
        {
            var offerte = await _context.PartecipantiLotti
                .Where(p => p.LottoId == lottoId && p.OffertaEconomica.HasValue)
                .Select(p => p.OffertaEconomica!.Value)
                .ToListAsync();

            return offerte.Any() ? offerte.Average() : null;
        }

        public async Task<decimal?> GetOffertaMinimaByLottoAsync(Guid lottoId)
        {
            return await _context.PartecipantiLotti
                .Where(p => p.LottoId == lottoId && p.OffertaEconomica.HasValue)
                .MinAsync(p => (decimal?)p.OffertaEconomica);
        }

        public async Task<decimal?> GetOffertaMassimaByLottoAsync(Guid lottoId)
        {
            return await _context.PartecipantiLotti
                .Where(p => p.LottoId == lottoId && p.OffertaEconomica.HasValue)
                .MaxAsync(p => (decimal?)p.OffertaEconomica);
        }

        public async Task<Dictionary<string, int>> GetStatisticheAsync()
        {
            var tuttiPartecipanti = await _context.PartecipantiLotti.ToListAsync();

            return new Dictionary<string, int>
            {
                ["TotalePartecipanti"] = tuttiPartecipanti.Count,
                ["Aggiudicatari"] = tuttiPartecipanti.Count(p => p.IsAggiudicatario),
                ["Scartati"] = tuttiPartecipanti.Count(p => p.IsScartatoDallEnte),
                ["NonAggiudicatari"] = tuttiPartecipanti.Count(p => !p.IsAggiudicatario && !p.IsScartatoDallEnte),
                ["ConOfferta"] = tuttiPartecipanti.Count(p => p.OffertaEconomica.HasValue),
                ["SenzaOfferta"] = tuttiPartecipanti.Count(p => !p.OffertaEconomica.HasValue)
            };
        }

        public async Task<Dictionary<string, object>> GetStatisticheLottoAsync(Guid lottoId)
        {
            var partecipanti = await _context.PartecipantiLotti
                .Where(p => p.LottoId == lottoId)
                .ToListAsync();

            var offerteValide = partecipanti
                .Where(p => p.OffertaEconomica.HasValue)
                .Select(p => p.OffertaEconomica!.Value)
                .ToList();

            return new Dictionary<string, object>
            {
                ["NumeroPartecipanti"] = partecipanti.Count,
                ["NumeroScartati"] = partecipanti.Count(p => p.IsScartatoDallEnte),
                ["OffertaMinima"] = offerteValide.Any() ? (object)offerteValide.Min() : null!,
                ["OffertaMedia"] = offerteValide.Any() ? (object)offerteValide.Average() : null!,
                ["OffertaMassima"] = offerteValide.Any() ? (object)offerteValide.Max() : null!,
                ["HasAggiudicatario"] = partecipanti.Any(p => p.IsAggiudicatario)
            };
        }

        public async Task<int> CountByLottoIdAsync(Guid lottoId)
        {
            return await _context.PartecipantiLotti
                .CountAsync(d => d.LottoId == lottoId && !d.IsDeleted);
        }
    }
}