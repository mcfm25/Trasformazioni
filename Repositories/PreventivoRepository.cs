using Microsoft.EntityFrameworkCore;
using Trasformazioni.Data;
using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;
using Trasformazioni.Models.ViewModels;

namespace Trasformazioni.Data.Repositories
{
    /// <summary>
    /// Repository per l'accesso ai dati dei Preventivi
    /// Implementa le operazioni CRUD e query specifiche per gestione scadenze
    /// CRITICO: include metodi per background job di monitoraggio scadenze
    /// </summary>
    public class PreventivoRepository : IPreventivoRepository
    {
        private readonly ApplicationDbContext _context;

        public PreventivoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===================================
        // OPERAZIONI BASE (CRUD)
        // ===================================

        public async Task<IEnumerable<Preventivo>> GetAllAsync()
        {
            return await _context.Preventivi
                .Include(p => p.Lotto)
                .Include(p => p.Soggetto)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<Preventivo?> GetByIdAsync(Guid id)
        {
            return await _context.Preventivi
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Preventivo> AddAsync(Preventivo preventivo)
        {
            await _context.Preventivi.AddAsync(preventivo);
            await _context.SaveChangesAsync();
            return preventivo;
        }

        public async Task UpdateAsync(Preventivo preventivo)
        {
            _context.Preventivi.Update(preventivo);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var preventivo = await GetByIdAsync(id);
            if (preventivo != null)
            {
                _context.Preventivi.Remove(preventivo); // Soft delete
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Preventivi.AnyAsync(p => p.Id == id);
        }

        // ===================================
        // OPERAZIONI CON RELAZIONI
        // ===================================

        public async Task<Preventivo?> GetWithRelationsAsync(Guid id)
        {
            return await _context.Preventivi
                .Include(p => p.Lotto)
                .Include(p => p.Soggetto)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Preventivo?> GetCompleteAsync(Guid id)
        {
            return await _context.Preventivi
                .Include(p => p.Lotto)
                    .ThenInclude(l => l.Gara)
                .Include(p => p.Soggetto)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        // ===================================
        // RICERCHE PER LOTTO
        // ===================================

        public async Task<IEnumerable<Preventivo>> GetByLottoIdAsync(Guid lottoId)
        {
            return await _context.Preventivi
                .Include(p => p.Soggetto)
                .Where(p => p.LottoId == lottoId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Preventivo>> GetValidiByLottoIdAsync(Guid lottoId)
        {
            return await _context.Preventivi
                .Include(p => p.Soggetto)
                .Where(p => p.LottoId == lottoId && p.Stato == StatoPreventivo.Valido)
                .OrderBy(p => p.ImportoOfferto)
                .ToListAsync();
        }

        public async Task<Preventivo?> GetSelezionatoByLottoIdAsync(Guid lottoId)
        {
            return await _context.Preventivi
                .Include(p => p.Soggetto)
                .FirstOrDefaultAsync(p => p.LottoId == lottoId && p.IsSelezionato);
        }

        // ===================================
        // RICERCHE PER SOGGETTO (FORNITORE)
        // ===================================

        public async Task<IEnumerable<Preventivo>> GetBySoggettoIdAsync(Guid soggettoId)
        {
            return await _context.Preventivi
                .Include(p => p.Lotto)
                    .ThenInclude(l => l.Gara)
                .Where(p => p.SoggettoId == soggettoId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Preventivo>> GetAttiviByForgnitoreAsync(Guid soggettoId)
        {
            return await _context.Preventivi
                .Include(p => p.Lotto)
                    .ThenInclude(l => l.Gara)
                .Where(p => p.SoggettoId == soggettoId &&
                           (p.Stato == StatoPreventivo.InAttesa ||
                            p.Stato == StatoPreventivo.Ricevuto ||
                            p.Stato == StatoPreventivo.Valido))
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        // ===================================
        // RICERCHE PER STATO
        // ===================================

        public async Task<IEnumerable<Preventivo>> GetByStatoAsync(StatoPreventivo stato)
        {
            return await _context.Preventivi
                .Include(p => p.Lotto)
                    .ThenInclude(l => l.Gara)
                .Include(p => p.Soggetto)
                .Where(p => p.Stato == stato)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Preventivo>> GetInAttesaAsync()
        {
            return await GetByStatoAsync(StatoPreventivo.InAttesa);
        }

        public async Task<IEnumerable<Preventivo>> GetRicevutiAsync()
        {
            return await GetByStatoAsync(StatoPreventivo.Ricevuto);
        }

        public async Task<IEnumerable<Preventivo>> GetValidiAsync()
        {
            return await GetByStatoAsync(StatoPreventivo.Valido);
        }

        public async Task<IEnumerable<Preventivo>> GetSelezionatiAsync()
        {
            return await _context.Preventivi
                .Include(p => p.Lotto)
                    .ThenInclude(l => l.Gara)
                .Include(p => p.Soggetto)
                .Where(p => p.IsSelezionato)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        // ===================================
        // GESTIONE SCADENZE (CRITICO PER BACKGROUND JOB)
        // ===================================

        public async Task<IEnumerable<Preventivo>> GetInScadenzaAsync(int giorniPreavviso = 7)
        {
            var dataLimite = DateTime.Now.AddDays(giorniPreavviso);

            return await _context.Preventivi
                .Include(p => p.Lotto)
                    .ThenInclude(l => l.Gara)
                .Include(p => p.Soggetto)
                .Where(p => p.DataScadenza <= dataLimite &&
                           p.DataScadenza > DateTime.Now &&
                           (p.Stato == StatoPreventivo.Valido ||
                            p.Stato == StatoPreventivo.Ricevuto))
                .OrderBy(p => p.DataScadenza)
                .ToListAsync();
        }

        public async Task<IEnumerable<Preventivo>> GetScadutiAsync()
        {
            return await _context.Preventivi
                .Include(p => p.Lotto)
                    .ThenInclude(l => l.Gara)
                .Include(p => p.Soggetto)
                .Where(p => p.DataScadenza < DateTime.Now &&
                           p.Stato != StatoPreventivo.Scaduto &&
                           p.Stato != StatoPreventivo.Annullato)
                .OrderBy(p => p.DataScadenza)
                .ToListAsync();
        }

        public async Task<IEnumerable<Preventivo>> GetConAutoRinnovoAsync()
        {
            return await _context.Preventivi
                .Include(p => p.Lotto)
                .Include(p => p.Soggetto)
                .Where(p => p.GiorniAutoRinnovo.HasValue && p.GiorniAutoRinnovo > 0)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Preventivo>> GetDaRinnovareAsync()
        {
            return await _context.Preventivi
                .Include(p => p.Lotto)
                    .ThenInclude(l => l.Gara)
                .Include(p => p.Soggetto)
                .Where(p => p.DataScadenza < DateTime.Now &&
                           p.GiorniAutoRinnovo.HasValue &&
                           p.GiorniAutoRinnovo > 0 &&
                           p.Stato != StatoPreventivo.Rinnovato &&
                           p.Stato != StatoPreventivo.Annullato)
                .OrderBy(p => p.DataScadenza)
                .ToListAsync();
        }

        // ===================================
        // RICERCHE SPECIFICHE
        // ===================================

        public async Task<IEnumerable<Preventivo>> SearchAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return Enumerable.Empty<Preventivo>();

            var lowerSearchTerm = searchTerm.ToLower();

            return await _context.Preventivi
                .Include(p => p.Lotto)
                    .ThenInclude(l => l.Gara)
                .Include(p => p.Soggetto)
                .Where(p =>
                    p.Descrizione.ToLower().Contains(lowerSearchTerm) ||
                    p.Soggetto.Denominazione!.ToLower().Contains(lowerSearchTerm) ||
                    p.Lotto.CodiceLotto.ToLower().Contains(lowerSearchTerm) ||
                    p.Lotto.Gara.CodiceGara.ToLower().Contains(lowerSearchTerm))
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Preventivo>> GetByScadenzaRangeAsync(DateTime dataInizio, DateTime dataFine)
        {
            return await _context.Preventivi
                .Include(p => p.Lotto)
                .Include(p => p.Soggetto)
                .Where(p => p.DataScadenza >= dataInizio && p.DataScadenza <= dataFine)
                .OrderBy(p => p.DataScadenza)
                .ToListAsync();
        }

        // ===================================
        // PAGINAZIONE E FILTRI
        // ===================================

        public async Task<(IEnumerable<Preventivo> Items, int TotalCount)> GetPagedAsync(PreventivoFilterViewModel filters)
        {
            var query = _context.Preventivi
                .Include(p => p.Lotto)
                    .ThenInclude(l => l.Gara)
                .Include(p => p.Soggetto)
                .AsQueryable();

            // Applica filtri
            if (!string.IsNullOrWhiteSpace(filters.SearchTerm))
            {
                var lowerSearchTerm = filters.SearchTerm.ToLower();
                query = query.Where(p =>
                    p.Descrizione.ToLower().Contains(lowerSearchTerm) ||
                    p.Soggetto.Denominazione!.ToLower().Contains(lowerSearchTerm) ||
                    p.Lotto.CodiceLotto.ToLower().Contains(lowerSearchTerm));
            }

            if (filters.LottoId.HasValue)
            {
                query = query.Where(p => p.LottoId == filters.LottoId.Value);
            }

            if (filters.SoggettoId.HasValue)
            {
                query = query.Where(p => p.SoggettoId == filters.SoggettoId.Value);
            }

            if (filters.Stato.HasValue)
            {
                query = query.Where(p => p.Stato == filters.Stato.Value);
            }

            if (filters.SoloSelezionati)
            {
                query = query.Where(p => p.IsSelezionato);
            }

            if (filters.SoloInScadenza)
            {
                var dataLimite = DateTime.Now.AddDays(7);
                query = query.Where(p => p.DataScadenza <= dataLimite && p.DataScadenza > DateTime.Now);
            }

            if (filters.DataScadenzaDa.HasValue)
            {
                query = query.Where(p => p.DataScadenza >= filters.DataScadenzaDa.Value);
            }

            if (filters.DataScadenzaA.HasValue)
            {
                query = query.Where(p => p.DataScadenza <= filters.DataScadenzaA.Value);
            }

            // Conteggio totale
            var totalCount = await query.CountAsync();

            // Ordinamento
            query = filters.OrderBy?.ToLower() switch
            {
                "fornitore" => filters.OrderDescending
                    ? query.OrderByDescending(p => p.Soggetto.Denominazione)
                    : query.OrderBy(p => p.Soggetto.Denominazione),
                "importo" => filters.OrderDescending
                    ? query.OrderByDescending(p => p.ImportoOfferto)
                    : query.OrderBy(p => p.ImportoOfferto),
                "stato" => filters.OrderDescending
                    ? query.OrderByDescending(p => p.Stato)
                    : query.OrderBy(p => p.Stato),
                "scadenza" => filters.OrderDescending
                    ? query.OrderByDescending(p => p.DataScadenza)
                    : query.OrderBy(p => p.DataScadenza),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            // Paginazione
            var items = await query
                .Skip((filters.PageNumber - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        // ===================================
        // STATISTICHE E REPORT
        // ===================================

        public async Task<Dictionary<StatoPreventivo, int>> GetCountByStatoAsync()
        {
            return await _context.Preventivi
                .GroupBy(p => p.Stato)
                .Select(group => new { Stato = group.Key, Count = group.Count() })
                .ToDictionaryAsync(x => x.Stato, x => x.Count);
        }

        public async Task<int> GetCountByLottoIdAsync(Guid lottoId)
        {
            return await _context.Preventivi
                .Where(p => p.LottoId == lottoId)
                .CountAsync();
        }

        public async Task<decimal?> GetImportoMedioByLottoIdAsync(Guid lottoId)
        {
            var preventivi = await _context.Preventivi
                .Where(p => p.LottoId == lottoId && p.ImportoOfferto.HasValue)
                .ToListAsync();

            if (!preventivi.Any())
                return null;

            return preventivi.Average(p => p.ImportoOfferto!.Value);
        }

        public async Task<decimal?> GetImportoMinimoValidiByLottoIdAsync(Guid lottoId)
        {
            return await _context.Preventivi
                .Where(p => p.LottoId == lottoId &&
                           p.Stato == StatoPreventivo.Valido &&
                           p.ImportoOfferto.HasValue)
                .MinAsync(p => (decimal?)p.ImportoOfferto);
        }
    }
}