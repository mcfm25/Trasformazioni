using Microsoft.EntityFrameworkCore;
using Trasformazioni.Data;
using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;
using Trasformazioni.Models.ViewModels;

namespace Trasformazioni.Data.Repositories
{
    /// <summary>
    /// Repository per l'accesso ai dati delle Gare
    /// Implementa le operazioni CRUD e query specifiche per l'entità Gara
    /// </summary>
    public class GaraRepository : IGaraRepository
    {
        private readonly ApplicationDbContext _context;

        public GaraRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===================================
        // OPERAZIONI BASE (CRUD)
        // ===================================

        public async Task<IEnumerable<Gara>> GetAllAsync()
        {
            return await _context.Gare
                .AsNoTracking()
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync();
        }

        public async Task<Gara?> GetByIdAsync(Guid id)
        {
            return await _context.Gare
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<Gara> AddAsync(Gara gara)
        {
            await _context.Gare.AddAsync(gara);
            await _context.SaveChangesAsync();
            return gara;
        }

        public async Task UpdateAsync(Gara gara)
        {
            _context.Gare.Update(gara);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var gara = await GetByIdAsync(id);
            if (gara != null)
            {
                _context.Gare.Remove(gara); // Soft delete gestito dall'interceptor
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Gare.AnyAsync(g => g.Id == id);
        }

        // ===================================
        // OPERAZIONI CON RELAZIONI
        // ===================================

        public async Task<Gara?> GetWithLottiAsync(Guid id)
        {
            return await _context.Gare
                .AsNoTracking()
                .Include(g => g.Lotti)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<Gara?> GetWithLottiAndDocumentiAsync(Guid id)
        {
            return await _context.Gare
                .AsNoTracking()
                .Include(g => g.Lotti)
                .Include(g => g.Documenti)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<Gara?> GetCompleteAsync(Guid id)
        {
            return await _context.Gare
                .AsNoTracking()
                .Include(g => g.Lotti)
                    .ThenInclude(l => l.OperatoreAssegnato)
                .Include(g => g.Lotti)
                    .ThenInclude(l => l.Valutazione)
                .Include(g => g.Lotti)
                    .ThenInclude(l => l.Elaborazione)
                .Include(g => g.Lotti)
                    .ThenInclude(l => l.Preventivi)
                .Include(g => g.Lotti)
                    .ThenInclude(l => l.RichiesteIntegrazione)
                .Include(g => g.Documenti)
                    .ThenInclude(d => d.TipoDocumento)
                .Include(g => g.DocumentiRichiesti)
                    .ThenInclude(dr => dr.TipoDocumento)
                .Include(g => g.ChiusaDa)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<Gara?> GetWithDocumentiRichiestiAsync(Guid id)
        {
            return await _context.Gare
                .AsNoTracking()
                .Include(g => g.DocumentiRichiesti)
                    .ThenInclude(dr => dr.TipoDocumento)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task UpdateDocumentiRichiestiAsync(Guid garaId, List<Guid> nuoviTipoDocumentoIds)
        {
            nuoviTipoDocumentoIds ??= new List<Guid>();

            // Carica gli attuali direttamente dal DbSet
            var attuali = await _context.Set<GaraDocumentoRichiesto>()
                .Where(x => x.GaraId == garaId)
                .ToListAsync();

            var idsAttuali = attuali.Select(x => x.TipoDocumentoId).ToList();

            // Rimuovi quelli non più presenti
            var daRimuovere = attuali.Where(x => !nuoviTipoDocumentoIds.Contains(x.TipoDocumentoId)).ToList();
            if (daRimuovere.Any())
            {
                _context.Set<GaraDocumentoRichiesto>().RemoveRange(daRimuovere);
            }

            // Aggiungi i nuovi
            var daAggiungere = nuoviTipoDocumentoIds.Where(id => !idsAttuali.Contains(id)).ToList();
            foreach (var tipoDocId in daAggiungere)
            {
                _context.Set<GaraDocumentoRichiesto>().Add(new GaraDocumentoRichiesto
                {
                    Id = Guid.NewGuid(),
                    GaraId = garaId,
                    TipoDocumentoId = tipoDocId
                });
            }

            await _context.SaveChangesAsync();
        }

        // ===================================
        // RICERCHE SPECIFICHE
        // ===================================

        public async Task<Gara?> GetByCodiceAsync(string codiceGara)
        {
            return await _context.Gare
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.CodiceGara == codiceGara);
        }

        public async Task<Gara?> GetByCIGAsync(string cig)
        {
            return await _context.Gare
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.CIG == cig);
        }

        public async Task<IEnumerable<Gara>> GetByStatoAsync(StatoGara stato)
        {
            return await _context.Gare
                .AsNoTracking()
                .Where(g => g.Stato == stato)
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Gara>> GetByTipologiaAsync(TipologiaGara tipologia)
        {
            return await _context.Gare
                .AsNoTracking()
                .Where(g => g.Tipologia == tipologia)
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Gara>> GetByRegioneAsync(string regione)
        {
            return await _context.Gare
                .AsNoTracking()
                .Where(g => g.Regione == regione)
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Gara>> GetActiveGareAsync()
        {
            return await _context.Gare
                .AsNoTracking()
                .Where(g => !g.IsChiusaManualmente)
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Gara>> GetGareInScadenzaAsync(int giorniProssimi = 7)
        {
            var dataLimite = DateTime.Now.AddDays(giorniProssimi);

            return await _context.Gare
                .AsNoTracking()
                .Where(g => g.DataTerminePresentazioneOfferte != null &&
                           g.DataTerminePresentazioneOfferte <= dataLimite &&
                           g.DataTerminePresentazioneOfferte > DateTime.Now &&
                           !g.IsChiusaManualmente)
                .OrderBy(g => g.DataTerminePresentazioneOfferte)
                .ToListAsync();
        }

        public async Task<IEnumerable<Gara>> SearchAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return Enumerable.Empty<Gara>();

            var lowerSearchTerm = searchTerm.ToLower();

            return await _context.Gare
                .AsNoTracking()
                .Where(g =>
                    g.CodiceGara.ToLower().Contains(lowerSearchTerm) ||
                    g.Titolo.ToLower().Contains(lowerSearchTerm) ||
                    (g.Descrizione != null && g.Descrizione.ToLower().Contains(lowerSearchTerm)) ||
                    (g.EnteAppaltante != null && g.EnteAppaltante.ToLower().Contains(lowerSearchTerm)) ||
                    (g.CIG != null && g.CIG.ToLower().Contains(lowerSearchTerm)))
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync();
        }

        // ===================================
        // PAGINAZIONE E FILTRI
        // ===================================

        public async Task<(IEnumerable<Gara> Items, int TotalCount)> GetPagedAsync(GaraFilterViewModel filters)
        {
            //var query = _context.Gare.AsQueryable();
            var query = _context.Gare.AsNoTracking().AsQueryable();


            // Applica filtri
            if (!string.IsNullOrWhiteSpace(filters.SearchTerm))
            {
                var lowerSearchTerm = filters.SearchTerm.ToLower();
                query = query.Where(g =>
                    g.CodiceGara.ToLower().Contains(lowerSearchTerm) ||
                    g.Titolo.ToLower().Contains(lowerSearchTerm) ||
                    (g.Descrizione != null && g.Descrizione.ToLower().Contains(lowerSearchTerm)) ||
                    (g.EnteAppaltante != null && g.EnteAppaltante.ToLower().Contains(lowerSearchTerm)));
            }

            if (filters.Stato.HasValue)
            {
                query = query.Where(g => g.Stato == filters.Stato.Value);
            }

            if (filters.Tipologia.HasValue)
            {
                query = query.Where(g => g.Tipologia == filters.Tipologia.Value);
            }

            if (!string.IsNullOrWhiteSpace(filters.Regione))
            {
                query = query.Where(g => g.Regione == filters.Regione);
            }

            if (filters.DataPubblicazioneDa.HasValue)
            {
                query = query.Where(g => g.DataPubblicazione >= filters.DataPubblicazioneDa.Value);
            }

            if (filters.DataPubblicazioneA.HasValue)
            {
                query = query.Where(g => g.DataPubblicazione <= filters.DataPubblicazioneA.Value);
            }

            if (filters.SoloAttive)
            {
                query = query.Where(g => !g.IsChiusaManualmente);
            }

            // Conteggio totale prima della paginazione
            var totalCount = await query.CountAsync();

            // Ordinamento
            query = filters.OrderBy?.ToLower() switch
            {
                "codice" => filters.OrderDescending
                    ? query.OrderByDescending(g => g.CodiceGara)
                    : query.OrderBy(g => g.CodiceGara),
                "titolo" => filters.OrderDescending
                    ? query.OrderByDescending(g => g.Titolo)
                    : query.OrderBy(g => g.Titolo),
                "stato" => filters.OrderDescending
                    ? query.OrderByDescending(g => g.Stato)
                    : query.OrderBy(g => g.Stato),
                "datapubblicazione" => filters.OrderDescending
                    ? query.OrderByDescending(g => g.DataPubblicazione)
                    : query.OrderBy(g => g.DataPubblicazione),
                "importo" => filters.OrderDescending
                    ? query.OrderByDescending(g => g.ImportoTotaleStimato)
                    : query.OrderBy(g => g.ImportoTotaleStimato),
                _ => query.OrderByDescending(g => g.CreatedAt) // Default: più recenti prima
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

        public async Task<bool> ExistsByCodiceAsync(string codiceGara, Guid? excludeId = null)
        {
            var query = _context.Gare.Where(g => g.CodiceGara == codiceGara);

            if (excludeId.HasValue)
            {
                query = query.Where(g => g.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> ExistsByCIGAsync(string cig, Guid? excludeId = null)
        {
            var query = _context.Gare.Where(g => g.CIG == cig);

            if (excludeId.HasValue)
            {
                query = query.Where(g => g.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        // ===================================
        // STATISTICHE E REPORT
        // ===================================

        public async Task<Dictionary<StatoGara, int>> GetCountByStatoAsync()
        {
            return await _context.Gare
                .AsNoTracking()
                .GroupBy(g => g.Stato)
                .Select(group => new { Stato = group.Key, Count = group.Count() })
                .ToDictionaryAsync(x => x.Stato, x => x.Count);
        }

        public async Task<Dictionary<string, int>> GetCountByRegioneAsync()
        {
            return await _context.Gare
                .AsNoTracking()
                .Where(g => g.Regione != null)
                .GroupBy(g => g.Regione!)
                .Select(group => new { Regione = group.Key, Count = group.Count() })
                .ToDictionaryAsync(x => x.Regione, x => x.Count);
        }

        public async Task<Dictionary<StatoGara, decimal>> GetImportoTotaleByStatoAsync()
        {
            return await _context.Gare
                .AsNoTracking()
                .Where(g => g.ImportoTotaleStimato.HasValue)
                .GroupBy(g => g.Stato)
                .Select(group => new
                {
                    Stato = group.Key,
                    ImportoTotale = group.Sum(g => g.ImportoTotaleStimato!.Value)
                })
                .ToDictionaryAsync(x => x.Stato, x => x.ImportoTotale);
        }
    }
}