using Microsoft.EntityFrameworkCore;
using Trasformazioni.Data;
using Trasformazioni.Models.Entities;

namespace Trasformazioni.Data.Repositories
{
    /// <summary>
    /// Repository per l'accesso ai dati delle assegnazioni mezzi
    /// </summary>
    public class AssegnazioneMezzoRepository : IAssegnazioneMezzoRepository
    {
        private readonly ApplicationDbContext _context;

        public AssegnazioneMezzoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AssegnazioneMezzo>> GetAllAsync()
        {
            return await _context.AssegnazioniMezzi
                .Include(a => a.Mezzo)
                .Include(a => a.Utente)
                .OrderByDescending(a => a.DataInizio)
                .ToListAsync();
        }

        public async Task<AssegnazioneMezzo?> GetByIdAsync(Guid id)
        {
            return await _context.AssegnazioniMezzi
                .Include(a => a.Mezzo)
                .Include(a => a.Utente)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<AssegnazioneMezzo>> GetByMezzoIdAsync(Guid mezzoId, bool includeChiuse = true)
        {
            var query = _context.AssegnazioniMezzi
                .Include(a => a.Utente)
                .Where(a => a.MezzoId == mezzoId);

            if (!includeChiuse)
            {
                query = query.Where(a => a.DataFine == null);
            }

            return await query
                .OrderByDescending(a => a.DataInizio)
                .ToListAsync();
        }

        public async Task<IEnumerable<AssegnazioneMezzo>> GetByUtenteIdAsync(string utenteId, bool includeChiuse = true)
        {
            var query = _context.AssegnazioniMezzi
                .Include(a => a.Mezzo)
                .Where(a => a.UtenteId == utenteId);

            if (!includeChiuse)
            {
                query = query.Where(a => a.DataFine == null);
            }

            return await query
                .OrderByDescending(a => a.DataInizio)
                .ToListAsync();
        }

        public async Task<AssegnazioneMezzo?> GetAssegnazioneAttivaByMezzoIdAsync(Guid mezzoId)
        {
            return await _context.AssegnazioniMezzi
                .Include(a => a.Mezzo)
                .Include(a => a.Utente)
                .FirstOrDefaultAsync(a => a.MezzoId == mezzoId && a.DataFine == null);
        }

        public async Task<IEnumerable<AssegnazioneMezzo>> GetAssegnazioniAttiveByUtenteIdAsync(string utenteId)
        {
            return await _context.AssegnazioniMezzi
                .Include(a => a.Mezzo)
                .Where(a => a.UtenteId == utenteId && a.DataFine == null)
                .OrderByDescending(a => a.DataInizio)
                .ToListAsync();
        }

        public async Task<IEnumerable<AssegnazioneMezzo>> GetPrenotazioniFutureAsync()
        {
            var oggi = DateTime.Now.Date;

            return await _context.AssegnazioniMezzi
                .Include(a => a.Mezzo)
                .Include(a => a.Utente)
                .Where(a => a.DataFine == null && a.DataInizio > oggi)
                .OrderBy(a => a.DataInizio)
                .ToListAsync();
        }

        public async Task<IEnumerable<AssegnazioneMezzo>> GetAssegnazioniInCorsoAsync()
        {
            var oggi = DateTime.Now.Date;

            return await _context.AssegnazioniMezzi
                .Include(a => a.Mezzo)
                .Include(a => a.Utente)
                .Where(a => a.DataFine == null && a.DataInizio <= oggi)
                .OrderBy(a => a.DataInizio)
                .ToListAsync();
        }

        public async Task<bool> HasAssegnazioneAttivaAsync(Guid mezzoId)
        {
            return await _context.AssegnazioniMezzi
                .AnyAsync(a => a.MezzoId == mezzoId && a.DataFine == null);
        }

        public async Task<bool> HasUtenteAssegnazioneAttivaAsync(string utenteId)
        {
            return await _context.AssegnazioniMezzi
                .AnyAsync(a => a.UtenteId == utenteId && a.DataFine == null);
        }

        /// <summary>
        /// Verifica se esiste una sovrapposizione di periodi per il mezzo specificato
        /// LOGICA COMPLESSA per supportare assegnazioni multiple in coda
        /// </summary>
        public async Task<bool> HasSovrapposizionePeriodoAsync(
            Guid mezzoId,
            DateTime dataInizio,
            DateTime? dataFine,
            Guid? excludeAssegnazioneId = null)
        {
            // Converti date in UTC per PostgreSQL
            //var dataInizioUtc = dataInizio.Kind == DateTimeKind.Utc
            //    ? dataInizio
            //    : dataInizio.ToUniversalTime();

            //var dataFineUtc = dataFine.HasValue
            //    ? (dataFine.Value.Kind == DateTimeKind.Utc
            //        ? dataFine.Value
            //        : dataFine.Value.ToUniversalTime())
            //    : (DateTime?)null;
            var dataInizioUtc = dataInizio;

            var dataFineUtc = dataFine.HasValue
                ? dataFine.Value
                : (DateTime?)null;

            var query = _context.AssegnazioniMezzi
                .Where(a => a.MezzoId == mezzoId && !a.IsDeleted);

            // Escludi assegnazione specifica (per modifica)
            if (excludeAssegnazioneId.HasValue)
                query = query.Where(a => a.Id != excludeAssegnazioneId.Value);

            // CASO 1: La nuova assegnazione è a tempo indeterminato (dataFine == null)
            if (!dataFineUtc.HasValue)
            {
                // NON deve esistere NESSUNA assegnazione attiva o futura
                // - Altre assegnazioni a tempo indeterminato
                // - Altre assegnazioni che finiscono dopo il nostro inizio
                // - Prenotazioni future che iniziano dopo di noi
                return await query.AnyAsync(a =>
                    a.DataFine == null ||                           // Altra assegnazione a tempo indeterminato
                    a.DataFine > dataInizioUtc ||                  // Altra assegnazione che finisce dopo il nostro inizio
                    (a.DataInizio >= dataInizioUtc)                // Prenotazione futura che inizia dopo di noi
                );
            }

            // CASO 2: La nuova assegnazione ha DataFine definita
            // Verifica:
            // 2a) NON deve esistere un'assegnazione a tempo indeterminato che si sovrappone
            var haAssegnazioneIndeterminata = await query.AnyAsync(a =>
                a.DataFine == null &&
                a.DataInizio < dataFineUtc.Value  // Inizia prima della nostra fine
            );

            if (haAssegnazioneIndeterminata)
                return true;

            // 2b) Verifica overlap con assegnazioni con DataFine popolata
            // Overlap classico: (DataInizio1 < DataFine2) AND (DataInizio2 < DataFine1)
            return await query.AnyAsync(a =>
                a.DataFine.HasValue &&
                a.DataInizio < dataFineUtc.Value &&
                dataInizioUtc < a.DataFine.Value
            );
        }

        /// <summary>
        /// Ottiene tutti i periodi occupati per un mezzo (per calendario)
        /// Include assegnazioni in corso E prenotazioni future
        /// </summary>
        public async Task<IEnumerable<AssegnazioneMezzo>> GetPeriodiOccupatiAsync(Guid mezzoId)
        {
            //var ora = Datetime.Now;
            var ora = DateTime.Now;

            return await _context.AssegnazioniMezzi
                .Include(a => a.Utente)
                .Where(a =>
                    a.MezzoId == mezzoId &&
                    !a.IsDeleted &&
                    (a.DataFine == null || a.DataFine > ora)) // Solo attive o future
                .OrderBy(a => a.DataInizio)
                .ToListAsync();
        }

        public async Task AddAsync(AssegnazioneMezzo assegnazione)
        {
            await _context.AssegnazioniMezzi.AddAsync(assegnazione);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(AssegnazioneMezzo assegnazione)
        {
            _context.AssegnazioniMezzi.Update(assegnazione);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(AssegnazioneMezzo assegnazione)
        {
            // Soft delete gestito da AuditInterceptor
            _context.AssegnazioniMezzi.Remove(assegnazione);
            await _context.SaveChangesAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}