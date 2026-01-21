using Microsoft.EntityFrameworkCore;
using Trasformazioni.Data;
using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;

namespace Trasformazioni.Data.Repositories
{
    /// <summary>
    /// Repository per l'accesso ai dati dei mezzi aziendali
    /// </summary>
    public class MezzoRepository : IMezzoRepository
    {
        private readonly ApplicationDbContext _context;

        public MezzoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Mezzo>> GetAllAsync()
        {
            return await _context.Set<Mezzo>()
                .Include(m => m.Assegnazioni)
                    .ThenInclude(a => a.Utente)
                .OrderBy(m => m.Targa)
                .ToListAsync();
        }

        public async Task<Mezzo?> GetByIdAsync(Guid id)
        {
            return await _context.Set<Mezzo>()
                .Include(m => m.Assegnazioni.Where(a => !a.IsDeleted))
                    .ThenInclude(a => a.Utente)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Mezzo?> GetByTargaAsync(string targa)
        {
            var targaNormalizzata = targa.Trim().ToUpperInvariant();
            return await _context.Set<Mezzo>()
                .FirstOrDefaultAsync(m => m.Targa == targaNormalizzata);
        }

        public async Task<IEnumerable<Mezzo>> GetByStatoAsync(StatoMezzo stato)
        {
            return await _context.Set<Mezzo>()
                .Where(m => m.Stato == stato)
                .Include(m => m.Assegnazioni)
                    .ThenInclude(a => a.Utente)
                .OrderBy(m => m.Targa)
                .OrderBy(m => m.Targa)
                .ToListAsync();
        }

        public async Task<IEnumerable<Mezzo>> GetByTipoProprietaAsync(TipoProprietaMezzo tipoProprieta)
        {
            return await _context.Set<Mezzo>()
                .Where(m => m.TipoProprieta == tipoProprieta)
                .Include(m => m.Assegnazioni)
                    .ThenInclude(a => a.Utente)
                .OrderBy(m => m.Targa)
                .ToListAsync();
        }

        public async Task<IEnumerable<Mezzo>> GetByTipoAsync(TipoMezzo tipo)
        {
            return await _context.Set<Mezzo>()
                .Where(m => m.Tipo == tipo)
                .OrderBy(m => m.Targa)
                .ToListAsync();
        }

        public async Task<IEnumerable<Mezzo>> SearchByTargaAsync(string targa)
        {
            var targaNormalizzata = targa.Trim().ToUpperInvariant();
            return await _context.Set<Mezzo>()
                .Where(m => m.Targa.Contains(targaNormalizzata))
                .Include(m => m.Assegnazioni)
                    .ThenInclude(a => a.Utente)
                .OrderBy(m => m.Targa)
                .ToListAsync();
        }

        public async Task<bool> ExistsTargaAsync(string targa, Guid? excludeId = null)
        {
            var targaNormalizzata = targa.Trim().ToUpperInvariant();

            var query = _context.Set<Mezzo>()
                .Where(m => m.Targa == targaNormalizzata);

            if (excludeId.HasValue)
            {
                query = query.Where(m => m.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> ExistsDeviceIMEIAsync(string deviceIMEI, Guid? excludeId = null)
        {
            //var targaNormalizzata = deviceIMEI.Trim().ToUpperInvariant();

            var query = _context.Set<Mezzo>()
                .Where(m => m.DeviceIMEI == deviceIMEI);

            if (excludeId.HasValue)
            {
                query = query.Where(m => m.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<IEnumerable<Mezzo>> GetMezziConAssicurazioneInScadenzaAsync(int giorniPreavviso = 30)
        {
            var dataLimite = DateTime.Now.Date.AddDays(giorniPreavviso);
            var oggi = DateTime.Now.Date;

            return await _context.Set<Mezzo>()
                .Where(m => m.DataScadenzaAssicurazione.HasValue &&
                           m.DataScadenzaAssicurazione.Value >= oggi &&
                           m.DataScadenzaAssicurazione.Value <= dataLimite)
                .OrderBy(m => m.DataScadenzaAssicurazione)
                .ToListAsync();
        }

        public async Task<IEnumerable<Mezzo>> GetMezziConRevisioneInScadenzaAsync(int giorniPreavviso = 30)
        {
            var dataLimite = DateTime.Now.Date.AddDays(giorniPreavviso);
            var oggi = DateTime.Now.Date;

            return await _context.Set<Mezzo>()
                .Where(m => m.DataScadenzaRevisione.HasValue &&
                           m.DataScadenzaRevisione.Value >= oggi &&
                           m.DataScadenzaRevisione.Value <= dataLimite)
                .OrderBy(m => m.DataScadenzaRevisione)
                .ToListAsync();
        }

        public async Task AddAsync(Mezzo mezzo)
        {
            await _context.Set<Mezzo>().AddAsync(mezzo);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Mezzo mezzo)
        {
            _context.Set<Mezzo>().Update(mezzo);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Mezzo mezzo)
        {
            // Soft delete gestito da AuditInterceptor
            _context.Set<Mezzo>().Remove(mezzo);
            await _context.SaveChangesAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}