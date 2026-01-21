using Microsoft.EntityFrameworkCore;
using Trasformazioni.Data;
using Trasformazioni.Models.Entities;
using Trasformazioni.Models.ViewModels;

namespace Trasformazioni.Data.Repositories
{
    /// <summary>
    /// Repository per l'accesso ai dati delle Richieste Integrazione
    /// Implementa le operazioni CRUD e query specifiche per le richieste
    /// </summary>
    public class RichiestaIntegrazioneRepository : IRichiestaIntegrazioneRepository
    {
        private readonly ApplicationDbContext _context;

        public RichiestaIntegrazioneRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===================================
        // OPERAZIONI BASE (CRUD)
        // ===================================

        public async Task<IEnumerable<RichiestaIntegrazione>> GetAllAsync()
        {
            return await _context.RichiesteIntegrazione
                .OrderByDescending(r => r.DataRichiestaEnte)
                .ToListAsync();
        }

        public async Task<RichiestaIntegrazione?> GetByIdAsync(Guid id)
        {
            return await _context.RichiesteIntegrazione
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<RichiestaIntegrazione> AddAsync(RichiestaIntegrazione richiesta)
        {
            await _context.RichiesteIntegrazione.AddAsync(richiesta);
            await _context.SaveChangesAsync();
            return richiesta;
        }

        public async Task UpdateAsync(RichiestaIntegrazione richiesta)
        {
            _context.RichiesteIntegrazione.Update(richiesta);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var richiesta = await GetByIdAsync(id);
            if (richiesta != null)
            {
                _context.RichiesteIntegrazione.Remove(richiesta); // Soft delete gestito dall'interceptor
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.RichiesteIntegrazione.AnyAsync(r => r.Id == id);
        }

        // ===================================
        // OPERAZIONI CON RELAZIONI
        // ===================================

        public async Task<RichiestaIntegrazione?> GetWithLottoAsync(Guid id)
        {
            return await _context.RichiesteIntegrazione
                .Include(r => r.Lotto)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<RichiestaIntegrazione?> GetWithRispostaDaAsync(Guid id)
        {
            return await _context.RichiesteIntegrazione
                .Include(r => r.RispostaDa)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<RichiestaIntegrazione?> GetCompleteAsync(Guid id)
        {
            return await _context.RichiesteIntegrazione
                .Include(r => r.Lotto)
                    .ThenInclude(l => l.Gara)
                .Include(r => r.RispostaDa)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        // ===================================
        // RICERCHE PER LOTTO
        // ===================================

        public async Task<IEnumerable<RichiestaIntegrazione>> GetByLottoIdAsync(Guid lottoId)
        {
            return await _context.RichiesteIntegrazione
                .Include(r => r.RispostaDa)
                .Where(r => r.LottoId == lottoId)
                .OrderByDescending(r => r.DataRichiestaEnte)
                .ToListAsync();
        }

        public async Task<IEnumerable<RichiestaIntegrazione>> GetByLottoIdOrderedAsync(Guid lottoId)
        {
            return await _context.RichiesteIntegrazione
                .Include(r => r.RispostaDa)
                .Where(r => r.LottoId == lottoId)
                .OrderBy(r => r.NumeroProgressivo)
                .ToListAsync();
        }

        public async Task<IEnumerable<RichiestaIntegrazione>> GetByLottoIdAperteAsync(Guid lottoId)
        {
            return await _context.RichiesteIntegrazione
                .Include(r => r.RispostaDa)
                .Where(r => r.LottoId == lottoId && !r.IsChiusa)
                .OrderBy(r => r.NumeroProgressivo)
                .ToListAsync();
        }

        public async Task<IEnumerable<RichiestaIntegrazione>> GetByLottoIdChiuseAsync(Guid lottoId)
        {
            return await _context.RichiesteIntegrazione
                .Include(r => r.RispostaDa)
                .Where(r => r.LottoId == lottoId && r.IsChiusa)
                .OrderBy(r => r.NumeroProgressivo)
                .ToListAsync();
        }

        // ===================================
        // RICERCHE PER STATO
        // ===================================

        public async Task<(IEnumerable<RichiestaIntegrazione> Items, int TotalCount)> GetPagedAsync(RichiestaIntegrazioneFilterViewModel filters)
        {
            var query = _context.RichiesteIntegrazione
                .Include(r => r.Lotto)
                    .ThenInclude(l => l.Gara)
                .Include(r => r.RispostaDa)
                .AsQueryable();

            // Applica filtri
            if (!string.IsNullOrWhiteSpace(filters.SearchTerm))
            {
                var lowerSearchTerm = filters.SearchTerm.ToLower();
                query = query.Where(r =>
                    r.TestoRichiestaEnte.ToLower().Contains(lowerSearchTerm) ||
                    (r.TestoRispostaAzienda != null && r.TestoRispostaAzienda.ToLower().Contains(lowerSearchTerm)));
            }

            if (filters.GaraId.HasValue)
            {
                query = query.Where(r => r.Lotto.GaraId == filters.GaraId.Value);
            }

            if (filters.LottoId.HasValue)
            {
                query = query.Where(r => r.LottoId == filters.LottoId.Value);
            }

            if (filters.RispostaDaUserId != null)
            {
                query = query.Where(r => r.RispostaDaUserId == filters.RispostaDaUserId);
            }

            if (filters.NumeroProgressivo.HasValue)
            {
                query = query.Where(r => r.NumeroProgressivo == filters.NumeroProgressivo.Value);
            }

            if (filters.IsChiusa.HasValue)
            {
                query = query.Where(r => r.IsChiusa == filters.IsChiusa.Value);
            }

            if (filters.SoloNonRisposte)
            {
                query = query.Where(r => !r.DataRispostaAzienda.HasValue);
            }

            if (filters.SoloRisposteNonChiuse)
            {
                query = query.Where(r => r.DataRispostaAzienda.HasValue && !r.IsChiusa);
            }

            if (filters.SoloScadute)
            {
                var dataLimite = DateTime.Now.AddDays(-filters.GiorniScadenza);
                query = query.Where(r => !r.DataRispostaAzienda.HasValue && r.DataRichiestaEnte <= dataLimite);
            }

            if (filters.DataRichiestaDa.HasValue)
            {
                query = query.Where(r => r.DataRichiestaEnte >= filters.DataRichiestaDa.Value);
            }

            if (filters.DataRichiestaA.HasValue)
            {
                query = query.Where(r => r.DataRichiestaEnte <= filters.DataRichiestaA.Value);
            }

            if (filters.DataRispostaDa.HasValue)
            {
                query = query.Where(r => r.DataRispostaAzienda.HasValue && r.DataRispostaAzienda.Value >= filters.DataRispostaDa.Value);
            }

            if (filters.DataRispostaA.HasValue)
            {
                query = query.Where(r => r.DataRispostaAzienda.HasValue && r.DataRispostaAzienda.Value <= filters.DataRispostaA.Value);
            }

            // Conteggio totale prima della paginazione
            var totalCount = await query.CountAsync();

            // Ordinamento
            query = filters.OrderBy?.ToLower() switch
            {
                "numero" => filters.OrderDescending
                    ? query.OrderByDescending(r => r.NumeroProgressivo)
                    : query.OrderBy(r => r.NumeroProgressivo),
                "datarichiesta" => filters.OrderDescending
                    ? query.OrderByDescending(r => r.DataRichiestaEnte)
                    : query.OrderBy(r => r.DataRichiestaEnte),
                "datarisposta" => filters.OrderDescending
                    ? query.OrderByDescending(r => r.DataRispostaAzienda)
                    : query.OrderBy(r => r.DataRispostaAzienda),
                "lotto" => filters.OrderDescending
                    ? query.OrderByDescending(r => r.Lotto.CodiceLotto)
                    : query.OrderBy(r => r.Lotto.CodiceLotto),
                "gara" => filters.OrderDescending
                    ? query.OrderByDescending(r => r.Lotto.Gara.CodiceGara)
                    : query.OrderBy(r => r.Lotto.Gara.CodiceGara),
                _ => filters.OrderDescending
                    ? query.OrderByDescending(r => r.CreatedAt)
                    : query.OrderBy(r => r.CreatedAt)
            };

            // Paginazione
            var items = await query
                .Skip((filters.PageNumber - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<IEnumerable<RichiestaIntegrazione>> GetAperteAsync()
        {
            return await _context.RichiesteIntegrazione
                .Include(r => r.Lotto)
                    .ThenInclude(l => l.Gara)
                .Include(r => r.RispostaDa)
                .Where(r => !r.IsChiusa)
                .OrderByDescending(r => r.DataRichiestaEnte)
                .ToListAsync();
        }

        public async Task<IEnumerable<RichiestaIntegrazione>> GetChiuseAsync()
        {
            return await _context.RichiesteIntegrazione
                .Include(r => r.Lotto)
                    .ThenInclude(l => l.Gara)
                .Include(r => r.RispostaDa)
                .Where(r => r.IsChiusa)
                .OrderByDescending(r => r.DataRichiestaEnte)
                .ToListAsync();
        }

        public async Task<IEnumerable<RichiestaIntegrazione>> GetNonRisposteAsync()
        {
            return await _context.RichiesteIntegrazione
                .Include(r => r.Lotto)
                    .ThenInclude(l => l.Gara)
                .Where(r => !r.DataRispostaAzienda.HasValue)
                .OrderBy(r => r.DataRichiestaEnte) // Le più vecchie prima (priorità)
                .ToListAsync();
        }

        public async Task<IEnumerable<RichiestaIntegrazione>> GetRisposteNonChiuseAsync()
        {
            return await _context.RichiesteIntegrazione
                .Include(r => r.Lotto)
                    .ThenInclude(l => l.Gara)
                .Include(r => r.RispostaDa)
                .Where(r => r.DataRispostaAzienda.HasValue && !r.IsChiusa)
                .OrderByDescending(r => r.DataRispostaAzienda)
                .ToListAsync();
        }

        public async Task<IEnumerable<RichiestaIntegrazione>> GetScaduteAsync(int giorniScadenza = 7)
        {
            var dataLimite = DateTime.Now.AddDays(-giorniScadenza);

            return await _context.RichiesteIntegrazione
                .Include(r => r.Lotto)
                    .ThenInclude(l => l.Gara)
                .Where(r => !r.DataRispostaAzienda.HasValue &&
                           r.DataRichiestaEnte <= dataLimite)
                .OrderBy(r => r.DataRichiestaEnte)
                .ToListAsync();
        }

        // ===================================
        // RICERCHE PER UTENTE
        // ===================================

        public async Task<IEnumerable<RichiestaIntegrazione>> GetByRispostaDaUserIdAsync(string userId)
        {
            return await _context.RichiesteIntegrazione
                .Include(r => r.Lotto)
                    .ThenInclude(l => l.Gara)
                .Where(r => r.RispostaDaUserId == userId)
                .OrderByDescending(r => r.DataRispostaAzienda)
                .ToListAsync();
        }

        // ===================================
        // RICERCHE PER DATA
        // ===================================

        public async Task<IEnumerable<RichiestaIntegrazione>> GetByDataRichiestaRangeAsync(DateTime dataInizio, DateTime dataFine)
        {
            return await _context.RichiesteIntegrazione
                .Include(r => r.Lotto)
                    .ThenInclude(l => l.Gara)
                .Include(r => r.RispostaDa)
                .Where(r => r.DataRichiestaEnte >= dataInizio && r.DataRichiestaEnte <= dataFine)
                .OrderBy(r => r.DataRichiestaEnte)
                .ToListAsync();
        }

        // ===================================
        // VALIDAZIONI / UTILITY
        // ===================================

        public async Task<int> GetNextNumeroProgressivoAsync(Guid lottoId)
        {
            var maxNumero = await _context.RichiesteIntegrazione
                .Where(r => r.LottoId == lottoId)
                .MaxAsync(r => (int?)r.NumeroProgressivo);

            return (maxNumero ?? 0) + 1;
        }

        public async Task<bool> ExistsByLottoAndNumeroAsync(Guid lottoId, int numeroProgressivo, Guid? excludeId = null)
        {
            var query = _context.RichiesteIntegrazione
                .Where(r => r.LottoId == lottoId && r.NumeroProgressivo == numeroProgressivo);

            if (excludeId.HasValue)
            {
                query = query.Where(r => r.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        // ===================================
        // STATISTICHE
        // ===================================

        public async Task<int> GetCountAperteAsync()
        {
            return await _context.RichiesteIntegrazione
                .Where(r => !r.IsChiusa)
                .CountAsync();
        }

        public async Task<int> GetCountByLottoAsync(Guid lottoId)
        {
            return await _context.RichiesteIntegrazione
                .Where(r => r.LottoId == lottoId)
                .CountAsync();
        }

        public async Task<int> GetCountAperteByLottoAsync(Guid lottoId)
        {
            return await _context.RichiesteIntegrazione
                .Where(r => r.LottoId == lottoId && !r.IsChiusa)
                .CountAsync();
        }

        public async Task<Dictionary<string, object>> GetStatisticheAsync()
        {
            var tutteRichieste = await _context.RichiesteIntegrazione.ToListAsync();

            var richiesteConRisposta = tutteRichieste
                .Where(r => r.DataRispostaAzienda.HasValue)
                .ToList();

            var tempoMedioRisposta = richiesteConRisposta.Any()
                ? richiesteConRisposta.Average(r => (r.DataRispostaAzienda!.Value - r.DataRichiestaEnte).TotalDays)
                : 0;

            return new Dictionary<string, object>
            {
                ["TotaleRichieste"] = tutteRichieste.Count,
                ["Aperte"] = tutteRichieste.Count(r => !r.IsChiusa),
                ["Chiuse"] = tutteRichieste.Count(r => r.IsChiusa),
                ["NonRisposte"] = tutteRichieste.Count(r => !r.DataRispostaAzienda.HasValue),
                ["RisposteNonChiuse"] = tutteRichieste.Count(r => r.DataRispostaAzienda.HasValue && !r.IsChiusa),
                ["TempoMedioRisposta"] = Math.Round(tempoMedioRisposta, 1)
            };
        }
    }
}