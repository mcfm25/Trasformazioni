using Microsoft.EntityFrameworkCore;
using Trasformazioni.Data;
using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;
using Trasformazioni.Models.ViewModels;

namespace Trasformazioni.Data.Repositories
{
    /// <summary>
    /// Repository per l'accesso ai dati delle Scadenze
    /// Implementa le operazioni CRUD e query specifiche per lo scadenzario
    /// CRITICO: utilizzato da background job giornaliero
    /// </summary>
    public class ScadenzaRepository : IScadenzaRepository
    {
        private readonly ApplicationDbContext _context;

        public ScadenzaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===================================
        // OPERAZIONI BASE (CRUD)
        // ===================================

        public async Task<IEnumerable<Scadenza>> GetAllAsync()
        {
            return await _context.Scadenze
                .OrderBy(s => s.DataScadenza)
                .ToListAsync();
        }

        public async Task<Scadenza?> GetByIdAsync(Guid id)
        {
            return await _context.Scadenze
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Scadenza> AddAsync(Scadenza scadenza)
        {
            await _context.Scadenze.AddAsync(scadenza);
            await _context.SaveChangesAsync();
            return scadenza;
        }

        public async Task UpdateAsync(Scadenza scadenza)
        {
            _context.Scadenze.Update(scadenza);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var scadenza = await GetByIdAsync(id);
            if (scadenza != null)
            {
                _context.Scadenze.Remove(scadenza); // Soft delete gestito dall'interceptor
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Scadenze.AnyAsync(s => s.Id == id);
        }

        // ===================================
        // OPERAZIONI CON RELAZIONI
        // ===================================

        public async Task<Scadenza?> GetWithGaraAsync(Guid id)
        {
            return await _context.Scadenze
                .Include(s => s.Gara)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Scadenza?> GetWithLottoAsync(Guid id)
        {
            return await _context.Scadenze
                .Include(s => s.Lotto)
                    .ThenInclude(l => l!.Gara)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Scadenza?> GetWithPreventivoAsync(Guid id)
        {
            return await _context.Scadenze
                .Include(s => s.Preventivo)
                    .ThenInclude(p => p!.Lotto)
                        .ThenInclude(l => l.Gara)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Scadenza?> GetCompleteAsync(Guid id)
        {
            return await _context.Scadenze
                .Include(s => s.Gara)
                .Include(s => s.Lotto)
                    .ThenInclude(l => l!.Gara)
                .Include(s => s.Preventivo)
                    .ThenInclude(p => p!.Lotto)
                        .ThenInclude(l => l.Gara)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        // ===================================
        // RICERCHE PER ENTITÀ
        // ===================================

        public async Task<IEnumerable<Scadenza>> GetByGaraIdAsync(Guid garaId, bool? soloAutomatiche = null)
        {
            var query = _context.Scadenze
                .Where(s => s.GaraId == garaId);

            // Filtro condizionale per scadenze automatiche/manuali
            if (soloAutomatiche.HasValue)
            {
                query = query.Where(s => s.IsAutomatica == soloAutomatiche.Value);
            }

            return await query
                .OrderBy(s => s.DataScadenza)
                .ToListAsync();
        }

        public async Task<IEnumerable<Scadenza>> GetByLottoIdAsync(Guid lottoId, bool? soloAutomatiche = null)
        {
            var query = _context.Scadenze
                .Include(s => s.Lotto)
                .Where(s => s.LottoId == lottoId);

            // Filtro condizionale per scadenze automatiche/manuali
            if (soloAutomatiche.HasValue)
            {
                query = query.Where(s => s.IsAutomatica == soloAutomatiche.Value);
            }

            return await query
                .OrderBy(s => s.DataScadenza)
                .ToListAsync();
        }

        public async Task<IEnumerable<Scadenza>> GetByPreventivoIdAsync(Guid preventivoId, bool? soloAutomatiche = null)
        {
            var query = _context.Scadenze
                .Include(s => s.Preventivo)
                .Where(s => s.PreventivoId == preventivoId);

            // Filtro condizionale per scadenze automatiche/manuali
            if (soloAutomatiche.HasValue)
            {
                query = query.Where(s => s.IsAutomatica == soloAutomatiche.Value);
            }

            return await query
                .OrderBy(s => s.DataScadenza)
                .ToListAsync();
        }

        /// <summary>
        /// Ottiene una scadenza automatica specifica per gara e tipo
        /// </summary>
        public async Task<Scadenza?> GetByGaraAndTipoAsync(Guid garaId, TipoScadenza tipo)
        {
            return await _context.Scadenze
                .Include(s => s.Gara)
                .Include(s => s.Lotto)
                .Include(s => s.Preventivo)
                .FirstOrDefaultAsync(s => s.GaraId == garaId && s.Tipo == tipo && s.IsAutomatica);
        }

        /// <summary>
        /// Ottiene una scadenza automatica specifica per lotto e tipo
        /// </summary>
        public async Task<Scadenza?> GetByLottoAndTipoAsync(Guid lottoId, TipoScadenza tipo)
        {
            return await _context.Scadenze
                .Include(s => s.Gara)
                .Include(s => s.Lotto)
                .Include(s => s.Preventivo)
                .FirstOrDefaultAsync(s => s.LottoId == lottoId && s.Tipo == tipo && s.IsAutomatica);
        }

        // ===================================
        // RICERCHE PER STATO
        // ===================================

        public async Task<(IEnumerable<Scadenza> Items, int TotalCount)> GetPagedAsync(ScadenzaFilterViewModel filters)
        {
            var query = _context.Scadenze
                .Include(s => s.Gara)
                .Include(s => s.Lotto)
                    .ThenInclude(l => l.Gara)
                .Include(s => s.Preventivo)
                    .ThenInclude(p => p.Soggetto)
                .AsQueryable();

            // Applica filtri
            if (!string.IsNullOrWhiteSpace(filters.SearchTerm))
            {
                var lowerSearchTerm = filters.SearchTerm.ToLower();
                query = query.Where(s =>
                    s.Descrizione.ToLower().Contains(lowerSearchTerm) ||
                    (s.Note != null && s.Note.ToLower().Contains(lowerSearchTerm)));
            }

            if (filters.GaraId.HasValue)
            {
                query = query.Where(s => s.GaraId == filters.GaraId.Value ||
                                         s.Lotto.GaraId == filters.GaraId.Value);
            }

            if (filters.LottoId.HasValue)
            {
                query = query.Where(s => s.LottoId == filters.LottoId.Value);
            }

            if (filters.PreventivoId.HasValue)
            {
                query = query.Where(s => s.PreventivoId == filters.PreventivoId.Value);
            }

            if (filters.Tipo.HasValue)
            {
                query = query.Where(s => s.Tipo == filters.Tipo.Value);
            }

            if (filters.IsCompletata.HasValue)
            {
                query = query.Where(s => s.IsCompletata == filters.IsCompletata.Value);
            }

            if (filters.IsAutomatica.HasValue)
            {
                query = query.Where(s => s.IsAutomatica == filters.IsAutomatica.Value);
            }

            if (filters.SoloScadute)
            {
                var oggi = DateTime.Now.Date;
                query = query.Where(s => !s.IsCompletata && s.DataScadenza < oggi);
            }

            if (filters.SoloInScadenza)
            {
                var oggi = DateTime.Now.Date;
                var dataLimite = oggi.AddDays(filters.GiorniScadenza);
                query = query.Where(s => !s.IsCompletata &&
                                         s.DataScadenza >= oggi &&
                                         s.DataScadenza <= dataLimite);
            }

            if (filters.SoloOggi)
            {
                var oggi = DateTime.Now.Date;
                query = query.Where(s => !s.IsCompletata && s.DataScadenza.Date == oggi);
            }

            if (filters.DataScadenzaDa.HasValue)
            {
                query = query.Where(s => s.DataScadenza >= filters.DataScadenzaDa.Value);
            }

            if (filters.DataScadenzaA.HasValue)
            {
                query = query.Where(s => s.DataScadenza <= filters.DataScadenzaA.Value);
            }

            if (filters.DataCompletamentoDa.HasValue)
            {
                query = query.Where(s => s.DataCompletamento.HasValue &&
                                         s.DataCompletamento.Value >= filters.DataCompletamentoDa.Value);
            }

            if (filters.DataCompletamentoA.HasValue)
            {
                query = query.Where(s => s.DataCompletamento.HasValue &&
                                         s.DataCompletamento.Value <= filters.DataCompletamentoA.Value);
            }

            // Conteggio totale prima della paginazione
            var totalCount = await query.CountAsync();

            // Ordinamento
            query = filters.OrderBy?.ToLower() switch
            {
                "data" => filters.OrderDescending
                    ? query.OrderByDescending(s => s.DataScadenza)
                    : query.OrderBy(s => s.DataScadenza),
                "tipo" => filters.OrderDescending
                    ? query.OrderByDescending(s => s.Tipo)
                    : query.OrderBy(s => s.Tipo),
                "descrizione" => filters.OrderDescending
                    ? query.OrderByDescending(s => s.Descrizione)
                    : query.OrderBy(s => s.Descrizione),
                "gara" => filters.OrderDescending
                    ? query.OrderByDescending(s => s.Gara.CodiceGara)
                    : query.OrderBy(s => s.Gara.CodiceGara),
                "lotto" => filters.OrderDescending
                    ? query.OrderByDescending(s => s.Lotto.CodiceLotto)
                    : query.OrderBy(s => s.Lotto.CodiceLotto),
                "stato" => filters.OrderDescending
                    ? query.OrderByDescending(s => s.IsCompletata)
                          .ThenByDescending(s => s.DataScadenza)
                    : query.OrderBy(s => s.IsCompletata)
                          .ThenBy(s => s.DataScadenza),
                _ => filters.OrderDescending
                    ? query.OrderByDescending(s => s.DataScadenza)
                    : query.OrderBy(s => s.DataScadenza)
            };

            // Paginazione
            var items = await query
                .Skip((filters.PageNumber - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<IEnumerable<Scadenza>> GetAttiveAsync()
        {
            return await _context.Scadenze
                .Include(s => s.Gara)
                .Include(s => s.Lotto)
                .Include(s => s.Preventivo)
                .Where(s => !s.IsCompletata)
                .OrderBy(s => s.DataScadenza)
                .ToListAsync();
        }

        public async Task<IEnumerable<Scadenza>> GetCompletateAsync()
        {
            return await _context.Scadenze
                .Include(s => s.Gara)
                .Include(s => s.Lotto)
                .Include(s => s.Preventivo)
                .Where(s => s.IsCompletata)
                .OrderByDescending(s => s.DataCompletamento)
                .ToListAsync();
        }

        public async Task<IEnumerable<Scadenza>> GetScaduteAsync()
        {
            var oggi = DateTime.Now.Date;

            return await _context.Scadenze
                .Include(s => s.Gara)
                .Include(s => s.Lotto)
                .Include(s => s.Preventivo)
                .Where(s => !s.IsCompletata && s.DataScadenza < oggi)
                .OrderBy(s => s.DataScadenza)
                .ToListAsync();
        }

        public async Task<IEnumerable<Scadenza>> GetAttiveByGaraIdAsync(Guid garaId)
        {
            return await _context.Scadenze
                .Where(s => s.GaraId == garaId && !s.IsCompletata)
                .OrderBy(s => s.DataScadenza)
                .ToListAsync();
        }

        public async Task<IEnumerable<Scadenza>> GetAttiveByLottoIdAsync(Guid lottoId)
        {
            return await _context.Scadenze
                .Where(s => s.LottoId == lottoId && !s.IsCompletata)
                .OrderBy(s => s.DataScadenza)
                .ToListAsync();
        }

        // ===================================
        // RICERCHE PER TIPO
        // ===================================

        public async Task<IEnumerable<Scadenza>> GetByTipoAsync(TipoScadenza tipo)
        {
            return await _context.Scadenze
                .Include(s => s.Gara)
                .Include(s => s.Lotto)
                .Include(s => s.Preventivo)
                .Where(s => s.Tipo == tipo)
                .OrderBy(s => s.DataScadenza)
                .ToListAsync();
        }

        public async Task<IEnumerable<Scadenza>> GetAttiveByTipoAsync(TipoScadenza tipo)
        {
            return await _context.Scadenze
                .Include(s => s.Gara)
                .Include(s => s.Lotto)
                .Include(s => s.Preventivo)
                .Where(s => s.Tipo == tipo && !s.IsCompletata)
                .OrderBy(s => s.DataScadenza)
                .ToListAsync();
        }

        // ===================================
        // RICERCHE TEMPORALI (CRITICHE per background job)
        // ===================================

        public async Task<IEnumerable<Scadenza>> GetInScadenzaAsync(int giorniPreavviso = 7)
        {
            var oggi = DateTime.Now.Date;
            var dataLimite = oggi.AddDays(giorniPreavviso);

            return await _context.Scadenze
                .Include(s => s.Gara)
                .Include(s => s.Lotto)
                .Include(s => s.Preventivo)
                .Where(s => !s.IsCompletata &&
                           s.DataScadenza >= oggi &&
                           s.DataScadenza <= dataLimite)
                .OrderBy(s => s.DataScadenza)
                .ToListAsync();
        }

        public async Task<IEnumerable<Scadenza>> GetOggiAsync()
        {
            var oggi = DateTime.Now.Date;

            return await _context.Scadenze
                .Include(s => s.Gara)
                .Include(s => s.Lotto)
                .Include(s => s.Preventivo)
                .Where(s => !s.IsCompletata && s.DataScadenza.Date == oggi)
                .OrderBy(s => s.Descrizione)
                .ToListAsync();
        }

        public async Task<IEnumerable<Scadenza>> GetByDataScadenzaRangeAsync(DateTime dataInizio, DateTime dataFine)
        {
            return await _context.Scadenze
                .Include(s => s.Gara)
                .Include(s => s.Lotto)
                .Include(s => s.Preventivo)
                .Where(s => s.DataScadenza >= dataInizio && s.DataScadenza <= dataFine)
                .OrderBy(s => s.DataScadenza)
                .ToListAsync();
        }

        public async Task<IEnumerable<Scadenza>> GetByDataScadenzaAsync(DateTime data)
        {
            var dataDate = data.Date;

            return await _context.Scadenze
                .Include(s => s.Gara)
                .Include(s => s.Lotto)
                .Include(s => s.Preventivo)
                .Where(s => s.DataScadenza.Date == dataDate)
                .OrderBy(s => s.Descrizione)
                .ToListAsync();
        }

        // ===================================
        // RICERCHE PER ORIGINE
        // ===================================

        public async Task<IEnumerable<Scadenza>> GetAutomaticheAsync()
        {
            return await _context.Scadenze
                .Include(s => s.Gara)
                .Include(s => s.Lotto)
                .Include(s => s.Preventivo)
                .Where(s => s.IsAutomatica)
                .OrderBy(s => s.DataScadenza)
                .ToListAsync();
        }

        public async Task<IEnumerable<Scadenza>> GetManualiAsync()
        {
            return await _context.Scadenze
                .Include(s => s.Gara)
                .Include(s => s.Lotto)
                .Include(s => s.Preventivo)
                .Where(s => !s.IsAutomatica)
                .OrderBy(s => s.DataScadenza)
                .ToListAsync();
        }

        public async Task<IEnumerable<Scadenza>> GetAutomaticheAttiveAsync()
        {
            return await _context.Scadenze
                .Include(s => s.Gara)
                .Include(s => s.Lotto)
                .Include(s => s.Preventivo)
                .Where(s => s.IsAutomatica && !s.IsCompletata)
                .OrderBy(s => s.DataScadenza)
                .ToListAsync();
        }

        // ===================================
        // STATISTICHE
        // ===================================

        public async Task<int> GetCountAttiveAsync()
        {
            return await _context.Scadenze
                .Where(s => !s.IsCompletata)
                .CountAsync();
        }

        public async Task<int> GetCountScaduteAsync()
        {
            var oggi = DateTime.Now.Date;

            return await _context.Scadenze
                .Where(s => !s.IsCompletata && s.DataScadenza < oggi)
                .CountAsync();
        }

        public async Task<int> GetCountInScadenzaAsync(int giorniPreavviso = 7)
        {
            var oggi = DateTime.Now.Date;
            var dataLimite = oggi.AddDays(giorniPreavviso);

            return await _context.Scadenze
                .Where(s => !s.IsCompletata &&
                           s.DataScadenza >= oggi &&
                           s.DataScadenza <= dataLimite)
                .CountAsync();
        }

        public async Task<Dictionary<TipoScadenza, int>> GetCountByTipoAsync()
        {
            return await _context.Scadenze
                .Where(s => !s.IsCompletata)
                .GroupBy(s => s.Tipo)
                .Select(g => new { Tipo = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Tipo, x => x.Count);
        }

        public async Task<Dictionary<string, int>> GetStatisticheAsync()
        {
            var tutteScadenze = await _context.Scadenze.ToListAsync();
            var oggi = DateTime.Now.Date;
            var dataLimite = oggi.AddDays(7);

            return new Dictionary<string, int>
            {
                ["TotaleScadenze"] = tutteScadenze.Count,
                ["Attive"] = tutteScadenze.Count(s => !s.IsCompletata),
                ["Completate"] = tutteScadenze.Count(s => s.IsCompletata),
                ["Scadute"] = tutteScadenze.Count(s => !s.IsCompletata && s.DataScadenza < oggi),
                ["InScadenza"] = tutteScadenze.Count(s => !s.IsCompletata && s.DataScadenza >= oggi && s.DataScadenza <= dataLimite),
                ["Automatiche"] = tutteScadenze.Count(s => s.IsAutomatica),
                ["Manuali"] = tutteScadenze.Count(s => !s.IsAutomatica)
            };
        }
    }
}