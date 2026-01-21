using Microsoft.EntityFrameworkCore;
using Trasformazioni.Data;
using Trasformazioni.Models.Entities;

namespace Trasformazioni.Data.Repositories
{
    /// <summary>
    /// Repository per l'accesso ai dati delle Valutazioni Lotto
    /// Implementa le operazioni CRUD e query specifiche per le valutazioni
    /// </summary>
    public class ValutazioneLottoRepository : IValutazioneLottoRepository
    {
        private readonly ApplicationDbContext _context;

        public ValutazioneLottoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===================================
        // OPERAZIONI BASE (CRUD)
        // ===================================

        public async Task<IEnumerable<ValutazioneLotto>> GetAllAsync()
        {
            return await _context.ValutazioniLotti
                .OrderByDescending(v => v.CreatedAt)
                .ToListAsync();
        }

        public async Task<ValutazioneLotto?> GetByIdAsync(Guid id)
        {
            return await _context.ValutazioniLotti
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<ValutazioneLotto> AddAsync(ValutazioneLotto valutazione)
        {
            await _context.ValutazioniLotti.AddAsync(valutazione);
            await _context.SaveChangesAsync();
            return valutazione;
        }

        public async Task UpdateAsync(ValutazioneLotto valutazione)
        {
            _context.ValutazioniLotti.Update(valutazione);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var valutazione = await GetByIdAsync(id);
            if (valutazione != null)
            {
                _context.ValutazioniLotti.Remove(valutazione); // Soft delete gestito dall'interceptor
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.ValutazioniLotti.AnyAsync(v => v.Id == id);
        }

        // ===================================
        // OPERAZIONI CON RELAZIONI
        // ===================================

        public async Task<ValutazioneLotto?> GetWithLottoAsync(Guid id)
        {
            return await _context.ValutazioniLotti
                .Include(v => v.Lotto)
                    .ThenInclude(l => l.Gara)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<ValutazioneLotto?> GetWithValutatoriAsync(Guid id)
        {
            return await _context.ValutazioniLotti
                .Include(v => v.ValutatoreTecnico)
                .Include(v => v.ValutatoreEconomico)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<ValutazioneLotto?> GetCompleteAsync(Guid id)
        {
            return await _context.ValutazioniLotti
                .Include(v => v.Lotto)
                    .ThenInclude(l => l.Gara)
                .Include(v => v.ValutatoreTecnico)
                .Include(v => v.ValutatoreEconomico)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        // ===================================
        // RICERCHE SPECIFICHE
        // ===================================

        public async Task<ValutazioneLotto?> GetByLottoIdAsync(Guid lottoId)
        {
            return await _context.ValutazioniLotti
                .Include(v => v.Lotto)
                    .ThenInclude(v => v.Gara)
                .Include(v => v.ValutatoreTecnico)
                .Include(v => v.ValutatoreEconomico)
                .FirstOrDefaultAsync(v => v.LottoId == lottoId);
        }

        public async Task<IEnumerable<ValutazioneLotto>> GetByValutatoreTecnicoAsync(string valutatoreTecnicoId)
        {
            return await _context.ValutazioniLotti
                .Include(v => v.Lotto)
                    .ThenInclude(l => l.Gara)
                .Where(v => v.ValutatoreTecnicoId == valutatoreTecnicoId)
                .OrderByDescending(v => v.DataValutazioneTecnica)
                .ToListAsync();
        }

        public async Task<IEnumerable<ValutazioneLotto>> GetByValutatoreEconomicoAsync(string valutatoreEconomicoId)
        {
            return await _context.ValutazioniLotti
                .Include(v => v.Lotto)
                    .ThenInclude(l => l.Gara)
                .Where(v => v.ValutatoreEconomicoId == valutatoreEconomicoId)
                .OrderByDescending(v => v.DataValutazioneEconomica)
                .ToListAsync();
        }

        // ===================================
        // RICERCHE PER STATO VALUTAZIONE TECNICA
        // ===================================

        public async Task<IEnumerable<ValutazioneLotto>> GetValutazioniTecnicheApprovateAsync()
        {
            return await _context.ValutazioniLotti
                .Include(v => v.Lotto)
                    .ThenInclude(l => l.Gara)
                .Include(v => v.ValutatoreTecnico)
                .Where(v => v.TecnicaApprovata == true)
                .OrderByDescending(v => v.DataValutazioneTecnica)
                .ToListAsync();
        }

        public async Task<IEnumerable<ValutazioneLotto>> GetValutazioniTecnicheRifiutateAsync()
        {
            return await _context.ValutazioniLotti
                .Include(v => v.Lotto)
                    .ThenInclude(l => l.Gara)
                .Include(v => v.ValutatoreTecnico)
                .Where(v => v.TecnicaApprovata == false)
                .OrderByDescending(v => v.DataValutazioneTecnica)
                .ToListAsync();
        }

        public async Task<IEnumerable<ValutazioneLotto>> GetValutazioniTecnichePendentiAsync()
        {
            return await _context.ValutazioniLotti
                .Include(v => v.Lotto)
                    .ThenInclude(l => l.Gara)
                .Where(v => v.DataValutazioneTecnica == null)
                .OrderByDescending(v => v.CreatedAt)
                .ToListAsync();
        }

        // ===================================
        // RICERCHE PER STATO VALUTAZIONE ECONOMICA
        // ===================================

        public async Task<IEnumerable<ValutazioneLotto>> GetValutazioniEconomicheApprovateAsync()
        {
            return await _context.ValutazioniLotti
                .Include(v => v.Lotto)
                    .ThenInclude(l => l.Gara)
                .Include(v => v.ValutatoreEconomico)
                .Where(v => v.EconomicaApprovata == true)
                .OrderByDescending(v => v.DataValutazioneEconomica)
                .ToListAsync();
        }

        public async Task<IEnumerable<ValutazioneLotto>> GetValutazioniEconomicheRifiutateAsync()
        {
            return await _context.ValutazioniLotti
                .Include(v => v.Lotto)
                    .ThenInclude(l => l.Gara)
                .Include(v => v.ValutatoreEconomico)
                .Where(v => v.EconomicaApprovata == false)
                .OrderByDescending(v => v.DataValutazioneEconomica)
                .ToListAsync();
        }

        public async Task<IEnumerable<ValutazioneLotto>> GetValutazioniEconomichePendentiAsync()
        {
            return await _context.ValutazioniLotti
                .Include(v => v.Lotto)
                    .ThenInclude(l => l.Gara)
                .Where(v => v.DataValutazioneEconomica == null)
                .OrderByDescending(v => v.CreatedAt)
                .ToListAsync();
        }

        // ===================================
        // RICERCHE COMBINATE
        // ===================================

        public async Task<IEnumerable<ValutazioneLotto>> GetValutazioniCompleteApprovateAsync()
        {
            return await _context.ValutazioniLotti
                .Include(v => v.Lotto)
                    .ThenInclude(l => l.Gara)
                .Include(v => v.ValutatoreTecnico)
                .Include(v => v.ValutatoreEconomico)
                .Where(v => v.TecnicaApprovata == true && v.EconomicaApprovata == true)
                .OrderByDescending(v => v.DataValutazioneEconomica)
                .ToListAsync();
        }

        public async Task<IEnumerable<ValutazioneLotto>> GetValutazioniConRifiutiAsync()
        {
            return await _context.ValutazioniLotti
                .Include(v => v.Lotto)
                    .ThenInclude(l => l.Gara)
                .Include(v => v.ValutatoreTecnico)
                .Include(v => v.ValutatoreEconomico)
                .Where(v => v.TecnicaApprovata == false || v.EconomicaApprovata == false)
                .OrderByDescending(v => v.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ValutazioneLotto>> GetValutazioniIncompleteAsync()
        {
            return await _context.ValutazioniLotti
                .Include(v => v.Lotto)
                    .ThenInclude(l => l.Gara)
                .Where(v => v.DataValutazioneTecnica == null || v.DataValutazioneEconomica == null)
                .OrderByDescending(v => v.CreatedAt)
                .ToListAsync();
        }

        // ===================================
        // VALIDAZIONI / ESISTENZA
        // ===================================

        public async Task<bool> ExistsByLottoIdAsync(Guid lottoId, Guid? excludeId = null)
        {
            var query = _context.ValutazioniLotti.Where(v => v.LottoId == lottoId);

            if (excludeId.HasValue)
            {
                query = query.Where(v => v.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        // ===================================
        // STATISTICHE
        // ===================================

        public async Task<Dictionary<string, int>> GetCountByApprovazioniAsync()
        {
            var allValutazioni = await _context.ValutazioniLotti.ToListAsync();

            return new Dictionary<string, int>
            {
                ["TecnicaApprovata"] = allValutazioni.Count(v => v.TecnicaApprovata == true),
                ["TecnicaRifiutata"] = allValutazioni.Count(v => v.TecnicaApprovata == false),
                ["TecnicaPendente"] = allValutazioni.Count(v => v.DataValutazioneTecnica == null),
                ["EconomicaApprovata"] = allValutazioni.Count(v => v.EconomicaApprovata == true),
                ["EconomicaRifiutata"] = allValutazioni.Count(v => v.EconomicaApprovata == false),
                ["EconomicaPendente"] = allValutazioni.Count(v => v.DataValutazioneEconomica == null),
                ["CompleteApprovate"] = allValutazioni.Count(v => v.TecnicaApprovata == true && v.EconomicaApprovata == true),
                ["ConRifiuti"] = allValutazioni.Count(v => v.TecnicaApprovata == false || v.EconomicaApprovata == false)
            };
        }

        public async Task<int> CountByLottoIdAsync(Guid lottoId)
        {
            return await _context.ValutazioniLotti
                .CountAsync(d => d.LottoId == lottoId && !d.IsDeleted);
        }
    }
}