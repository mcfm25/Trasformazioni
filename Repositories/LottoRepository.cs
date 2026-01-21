using Microsoft.EntityFrameworkCore;
using Trasformazioni.Data;
using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;
using Trasformazioni.Models.ViewModels;

namespace Trasformazioni.Data.Repositories
{
    /// <summary>
    /// Repository per l'accesso ai dati dei Lotti
    /// Implementa le operazioni CRUD e query specifiche per il workflow dei lotti
    /// </summary>
    public class LottoRepository : ILottoRepository
    {
        private readonly ApplicationDbContext _context;

        public LottoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===================================
        // OPERAZIONI BASE (CRUD)
        // ===================================

        public async Task<IEnumerable<Lotto>> GetAllAsync()
        {
            return await _context.Lotti
                .Include(l => l.Gara)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }

        public async Task<Lotto?> GetByIdAsync(Guid id)
        {
            return await _context.Lotti
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<Lotto?> GetWithDocumentiRichiestiAsync(Guid id)
        {
            return await _context.Lotti
                .Include(l => l.DocumentiRichiesti)
                    .ThenInclude(dr => dr.TipoDocumento)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<Lotto> AddAsync(Lotto lotto)
        {
            await _context.Lotti.AddAsync(lotto);
            await _context.SaveChangesAsync();
            return lotto;
        }

        public async Task UpdateAsync(Lotto lotto)
        {
            //_context.Lotti.Update(lotto);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var lotto = await GetByIdAsync(id);
            if (lotto != null)
            {
                _context.Lotti.Remove(lotto); // Soft delete gestito dall'interceptor
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Lotti.AnyAsync(l => l.Id == id);
        }

        public async Task UpdateDocumentiRichiestiAsync(Guid lottoId, List<Guid> nuoviTipoDocumentoIds)
        {
            nuoviTipoDocumentoIds ??= new List<Guid>();

            // Carica gli attuali direttamente dal DbSet
            var attuali = await _context.LottoDocumentiRichiesti
                .Where(x => x.LottoId == lottoId)
                .ToListAsync();

            var idsAttuali = attuali.Select(x => x.TipoDocumentoId).ToList();

            // Rimuovi quelli non più presenti
            var daRimuovere = attuali.Where(x => !nuoviTipoDocumentoIds.Contains(x.TipoDocumentoId)).ToList();
            if (daRimuovere.Any())
            {
                _context.LottoDocumentiRichiesti.RemoveRange(daRimuovere);
            }

            // Aggiungi i nuovi
            var daAggiungere = nuoviTipoDocumentoIds.Where(id => !idsAttuali.Contains(id)).ToList();
            foreach (var tipoDocId in daAggiungere)
            {
                _context.Set<LottoDocumentoRichiesto>().Add(new LottoDocumentoRichiesto
                {
                    Id = Guid.NewGuid(),
                    LottoId = lottoId,
                    TipoDocumentoId = tipoDocId
                });
            }

            await _context.SaveChangesAsync();
        }

        // ===================================
        // OPERAZIONI CON RELAZIONI
        // ===================================

        public async Task<Lotto?> GetWithGaraAsync(Guid id)
        {
            return await _context.Lotti
                .Include(l => l.Gara)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<Lotto?> GetWithValutazioneElaborazioneAsync(Guid id)
        {
            return await _context.Lotti
                .Include(l => l.Valutazione)
                    .ThenInclude(v => v!.ValutatoreTecnico)
                .Include(l => l.Valutazione)
                    .ThenInclude(v => v!.ValutatoreEconomico)
                .Include(l => l.Elaborazione)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<Lotto?> GetWithPreventiviAsync(Guid id)
        {
            return await _context.Lotti
                .Include(l => l.Preventivi)
                    .ThenInclude(p => p.Soggetto)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<Lotto?> GetWithIntegrazioniAsync(Guid id)
        {
            return await _context.Lotti
                .Include(l => l.RichiesteIntegrazione)
                    .ThenInclude(r => r.RispostaDa)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<Lotto?> GetWithPartecipantiAsync(Guid id)
        {
            return await _context.Lotti
                .Include(l => l.Partecipanti)
                    .ThenInclude(p => p.Soggetto)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<Lotto?> GetWithDocumentiAsync(Guid id)
        {
            return await _context.Lotti
                .Include(l => l.Documenti)
                    .ThenInclude(d => d.CaricatoDa)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<Lotto?> GetCompleteAsync(Guid id)
        {
            return await _context.Lotti
                .Include(l => l.Gara)
                    .ThenInclude(g => g!.DocumentiRichiesti)
                        .ThenInclude(dr => dr.TipoDocumento)
                .Include(l => l.Gara)
                    .ThenInclude(g => g!.Documenti)
                        .ThenInclude(d => d.TipoDocumento)
                .Include(l => l.OperatoreAssegnato)
                .Include(l => l.Valutazione)
                    .ThenInclude(v => v!.ValutatoreTecnico)
                .Include(l => l.Valutazione)
                    .ThenInclude(v => v!.ValutatoreEconomico)
                .Include(l => l.Elaborazione)
                .Include(l => l.Preventivi)
                    .ThenInclude(p => p.Soggetto)
                .Include(l => l.RichiesteIntegrazione)
                .Include(l => l.Partecipanti)
                    .ThenInclude(p => p.Soggetto)
                .Include(l => l.Documenti)
                    .ThenInclude(l => l.TipoDocumento)
                .Include(l => l.DocumentiRichiesti)
                    .ThenInclude(dr => dr.TipoDocumento)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        // ===================================
        // RICERCHE PER GARA
        // ===================================

        public async Task<IEnumerable<Lotto>> GetByGaraIdAsync(Guid garaId)
        {
            return await _context.Lotti
                .Where(l => l.GaraId == garaId)
                .OrderBy(l => l.CodiceLotto)
                .ToListAsync();
        }

        public async Task<IEnumerable<Lotto>> GetByGaraIdCompleteAsync(Guid garaId)
        {
            return await _context.Lotti
                .Include(l => l.OperatoreAssegnato)
                .Include(l => l.Valutazione)
                .Include(l => l.Elaborazione)
                .Include(l => l.Preventivi)
                .Where(l => l.GaraId == garaId)
                .OrderBy(l => l.CodiceLotto)
                .ToListAsync();
        }

        // ===================================
        // RICERCHE PER STATO (WORKFLOW)
        // ===================================

        public async Task<IEnumerable<Lotto>> GetByStatoAsync(StatoLotto stato)
        {
            return await _context.Lotti
                .Include(l => l.Gara)
                .Include(l => l.OperatoreAssegnato)
                .Where(l => l.Stato == stato)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Lotto>> GetInValutazioneTecnicaAsync()
        {
            return await GetByStatoAsync(StatoLotto.InValutazioneTecnica);
        }

        public async Task<IEnumerable<Lotto>> GetInValutazioneEconomicaAsync()
        {
            return await GetByStatoAsync(StatoLotto.InValutazioneEconomica);
        }

        public async Task<IEnumerable<Lotto>> GetInElaborazioneAsync()
        {
            return await GetByStatoAsync(StatoLotto.InElaborazione);
        }

        public async Task<IEnumerable<Lotto>> GetPresentatiAsync()
        {
            return await GetByStatoAsync(StatoLotto.Presentato);
        }

        public async Task<IEnumerable<Lotto>> GetInEsameAsync()
        {
            return await GetByStatoAsync(StatoLotto.InEsame);
        }

        public async Task<IEnumerable<Lotto>> GetConIntegrazioniAperteAsync()
        {
            return await _context.Lotti
                .Include(l => l.Gara)
                .Include(l => l.RichiesteIntegrazione)
                .Where(l => l.Stato == StatoLotto.RichiestaIntegrazione &&
                           l.RichiesteIntegrazione.Any(r => !r.IsChiusa))
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Lotto>> GetVintiAsync()
        {
            return await GetByStatoAsync(StatoLotto.Vinto);
        }

        public async Task<IEnumerable<Lotto>> GetPersiAsync()
        {
            return await GetByStatoAsync(StatoLotto.Perso);
        }

        // ===================================
        // RICERCHE PER OPERATORE
        // ===================================

        public async Task<IEnumerable<Lotto>> GetByOperatoreAsync(string operatoreId)
        {
            return await _context.Lotti
                .Include(l => l.Gara)
                .Include(l => l.OperatoreAssegnato)
                .Where(l => l.OperatoreAssegnatoId == operatoreId)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Lotto>> GetByOperatoreAndStatoAsync(string operatoreId, StatoLotto stato)
        {
            return await _context.Lotti
                .Include(l => l.Gara)
                .Include(l => l.OperatoreAssegnato)
                .Where(l => l.OperatoreAssegnatoId == operatoreId && l.Stato == stato)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }

        // ===================================
        // RICERCHE SPECIFICHE
        // ===================================

        public async Task<Lotto?> GetByGaraAndCodiceAsync(Guid garaId, string codiceLotto)
        {
            return await _context.Lotti
                .FirstOrDefaultAsync(l => l.GaraId == garaId && l.CodiceLotto == codiceLotto);
        }

        public async Task<IEnumerable<Lotto>> GetByTipologiaAsync(TipologiaLotto tipologia)
        {
            return await _context.Lotti
                .Include(l => l.Gara)
                .Where(l => l.Tipologia == tipologia)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Lotto>> SearchAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return Enumerable.Empty<Lotto>();

            var lowerSearchTerm = searchTerm.ToLower();

            return await _context.Lotti
                .Include(l => l.Gara)
                .Where(l =>
                    l.CodiceLotto.ToLower().Contains(lowerSearchTerm) ||
                    l.Descrizione.ToLower().Contains(lowerSearchTerm) ||
                    l.Gara.CodiceGara.ToLower().Contains(lowerSearchTerm) ||
                    l.Gara.Titolo.ToLower().Contains(lowerSearchTerm))
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }
        
        /// <summary>
        /// Verifica se una gara ha almeno un lotto associato
        /// </summary>
        public async Task<bool> HasLottiByGaraIdAsync(Guid garaId)
        {
            return await _context.Lotti
                .AnyAsync(l => l.GaraId == garaId);
        }

        /// <summary>
        /// Ottiene i lotti non in stato terminale di una gara
        /// Stati terminali: Vinto, Perso, Scartato, Rifiutato
        /// </summary>
        public async Task<IEnumerable<Lotto>> GetLottiNonTerminaliByGaraIdAsync(Guid garaId)
        {
            var statiTerminali = new[]
            {
                StatoLotto.Vinto,
                StatoLotto.Perso,
                StatoLotto.Scartato,
                StatoLotto.Rifiutato
            };

            return await _context.Lotti
                .Where(l => l.GaraId == garaId && !statiTerminali.Contains(l.Stato))
                .ToListAsync();
        }

        // ===================================
        // PAGINAZIONE E FILTRI
        // ===================================

        public async Task<(IEnumerable<Lotto> Items, int TotalCount)> GetPagedAsync(LottoFilterViewModel filters)
        {
            var query = _context.Lotti
                .Include(l => l.Gara)
                .Include(l => l.OperatoreAssegnato)
                .AsQueryable();

            // Applica filtri
            if (!string.IsNullOrWhiteSpace(filters.SearchTerm))
            {
                var lowerSearchTerm = filters.SearchTerm.ToLower();
                query = query.Where(l =>
                    l.CodiceLotto.ToLower().Contains(lowerSearchTerm) ||
                    l.Descrizione.ToLower().Contains(lowerSearchTerm) ||
                    l.Gara.CodiceGara.ToLower().Contains(lowerSearchTerm));
            }

            if (filters.GaraId.HasValue)
            {
                query = query.Where(l => l.GaraId == filters.GaraId.Value);
            }

            if (filters.Stato.HasValue)
            {
                query = query.Where(l => l.Stato == filters.Stato.Value);
            }

            if (filters.Tipologia.HasValue)
            {
                query = query.Where(l => l.Tipologia == filters.Tipologia.Value);
            }

            if (!string.IsNullOrWhiteSpace(filters.OperatoreAssegnatoId))
            {
                query = query.Where(l => l.OperatoreAssegnatoId == filters.OperatoreAssegnatoId);
            }

            if (filters.SoloConOperatore)
            {
                query = query.Where(l => l.OperatoreAssegnatoId != null);
            }

            // Conteggio totale prima della paginazione
            var totalCount = await query.CountAsync();

            // Ordinamento
            query = filters.OrderBy?.ToLower() switch
            {
                "codice" => filters.OrderDescending
                    ? query.OrderByDescending(l => l.CodiceLotto)
                    : query.OrderBy(l => l.CodiceLotto),
                "descrizione" => filters.OrderDescending
                    ? query.OrderByDescending(l => l.Descrizione)
                    : query.OrderBy(l => l.Descrizione),
                "stato" => filters.OrderDescending
                    ? query.OrderByDescending(l => l.Stato)
                    : query.OrderBy(l => l.Stato),
                "importo" => filters.OrderDescending
                    ? query.OrderByDescending(l => l.ImportoBaseAsta)
                    : query.OrderBy(l => l.ImportoBaseAsta),
                "gara" => filters.OrderDescending
                    ? query.OrderByDescending(l => l.Gara.CodiceGara)
                    : query.OrderBy(l => l.Gara.CodiceGara),
                _ => query.OrderByDescending(l => l.CreatedAt) // Default
            };

            // Paginazione
            var items = await query
                .Skip((filters.PageNumber - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        // ===================================
        // VALIDAZIONI / ESISTENZA
        // ===================================

        public async Task<IEnumerable<Lotto>> GetLottiInScadenzaAsync(int giorniProssimi = 7)
        {
            var dataLimite = DateTime.Now.AddDays(giorniProssimi);
            var oggi = DateTime.Now;

            return await _context.Lotti
                .Include(l => l.Gara)
                .Include(l => l.OperatoreAssegnato)
                .Where(l =>
                    l.Gara.DataTerminePresentazioneOfferte.HasValue &&
                    l.Gara.DataTerminePresentazioneOfferte.Value >= oggi &&
                    l.Gara.DataTerminePresentazioneOfferte.Value <= dataLimite &&
                    // Considera solo lotti in stati "attivi" (non terminali)
                    (l.Stato == StatoLotto.Bozza ||
                     l.Stato == StatoLotto.InValutazioneTecnica ||
                     l.Stato == StatoLotto.InValutazioneEconomica ||
                     l.Stato == StatoLotto.Approvato ||
                     l.Stato == StatoLotto.InElaborazione))
                .OrderBy(l => l.Gara.DataTerminePresentazioneOfferte)
                .ToListAsync();
        }

        public async Task<bool> ExistsByGaraAndCodiceAsync(Guid garaId, string codiceLotto, Guid? excludeId = null)
        {
            var query = _context.Lotti
                .Where(l => l.GaraId == garaId && l.CodiceLotto == codiceLotto);

            if (excludeId.HasValue)
            {
                query = query.Where(l => l.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        // ===================================
        // STATISTICHE E REPORT
        // ===================================

        public async Task<Dictionary<StatoLotto, int>> GetCountByStatoAsync()
        {
            return await _context.Lotti
                .GroupBy(l => l.Stato)
                .Select(group => new { Stato = group.Key, Count = group.Count() })
                .ToDictionaryAsync(x => x.Stato, x => x.Count);
        }

        public async Task<Dictionary<StatoLotto, int>> GetCountByStatoForGaraAsync(Guid garaId)
        {
            return await _context.Lotti
                .Where(l => l.GaraId == garaId)
                .GroupBy(l => l.Stato)
                .Select(group => new { Stato = group.Key, Count = group.Count() })
                .ToDictionaryAsync(x => x.Stato, x => x.Count);
        }

        public async Task<Dictionary<StatoLotto, int>> GetCountByStatoForOperatoreAsync(string operatoreId)
        {
            return await _context.Lotti
                .Where(l => l.OperatoreAssegnatoId == operatoreId)
                .GroupBy(l => l.Stato)
                .Select(group => new { Stato = group.Key, Count = group.Count() })
                .ToDictionaryAsync(x => x.Stato, x => x.Count);
        }

        public async Task<decimal> GetImportoTotaleVintiAsync()
        {
            return await _context.Lotti
                .Where(l => l.Stato == StatoLotto.Vinto && l.ImportoBaseAsta.HasValue)
                .SumAsync(l => l.ImportoBaseAsta!.Value);
        }

        public async Task<decimal> GetTassoSuccessoAsync()
        {
            var totalePartecipati = await _context.Lotti
                .Where(l => l.Stato == StatoLotto.Vinto ||
                           l.Stato == StatoLotto.Perso ||
                           l.Stato == StatoLotto.Scartato)
                .CountAsync();

            if (totalePartecipati == 0)
                return 0;

            var totaleVinti = await _context.Lotti
                .Where(l => l.Stato == StatoLotto.Vinto)
                .CountAsync();

            return Math.Round((decimal)totaleVinti / totalePartecipati * 100, 2);
        }

        public async Task<int> CountByGaraIdAsync(Guid garaId)
        {
            return await _context.Lotti.CountAsync(l => l.GaraId == garaId);
        }

    }
}