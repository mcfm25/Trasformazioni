using Microsoft.EntityFrameworkCore;
using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;
using Trasformazioni.Models.ViewModels;

namespace Trasformazioni.Data.Repositories
{
    /// <summary>
    /// Implementazione del repository per l'accesso ai dati dei soggetti
    /// </summary>
    public class SoggettoRepository : ISoggettoRepository
    {
        private readonly ApplicationDbContext _context;

        public SoggettoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===================================
        // OPERAZIONI BASE
        // ===================================

        /// <summary>
        /// Ottiene tutti i soggetti non cancellati
        /// </summary>
        public async Task<IEnumerable<Soggetto>> GetAllAsync()
        {
            return await _context.Soggetti
                .OrderBy(s => s.TipoSoggetto)
                .ThenBy(s => s.Denominazione ?? s.Nome)
                .ToListAsync();
        }

        /// <summary>
        /// Ottiene un soggetto per ID
        /// </summary>
        public async Task<Soggetto?> GetByIdAsync(Guid id)
        {
            return await _context.Soggetti
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        /// <summary>
        /// Aggiunge un nuovo soggetto
        /// </summary>
        public async Task AddAsync(Soggetto soggetto)
        {
            await _context.Soggetti.AddAsync(soggetto);
        }

        /// <summary>
        /// Aggiorna un soggetto esistente
        /// </summary>
        public async Task UpdateAsync(Soggetto soggetto)
        {
            _context.Soggetti.Update(soggetto);
        }

        /// <summary>
        /// Elimina un soggetto (soft delete)
        /// </summary>
        public async Task DeleteAsync(Soggetto soggetto)
        {
            soggetto.IsDeleted = true;
            soggetto.DeletedAt = DateTime.Now;
            // DeletedBy sarà gestito dall'interceptor
            _context.Soggetti.Update(soggetto);
            await Task.CompletedTask;
        }

        /// <summary>
        /// Verifica se esiste un soggetto con l'ID specificato
        /// </summary>
        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Soggetti.AnyAsync(s => s.Id == id);
        }

        /// <summary>
        /// Salva le modifiche nel database
        /// </summary>
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // ===================================
        // QUERY SPECIFICHE - FILTRI
        // ===================================

        /// <summary>
        /// Ottiene tutti i clienti
        /// </summary>
        public async Task<IEnumerable<Soggetto>> GetClientiAsync()
        {
            return await _context.Soggetti
                .Where(s => s.IsCliente)
                .OrderBy(s => s.Denominazione ?? s.Nome)
                .ToListAsync();
        }

        /// <summary>
        /// Ottiene tutti i fornitori
        /// </summary>
        public async Task<IEnumerable<Soggetto>> GetFornitoriAsync()
        {
            return await _context.Soggetti
                .Where(s => s.IsFornitore)
                .OrderBy(s => s.Denominazione ?? s.Nome)
                .ToListAsync();
        }

        /// <summary>
        /// Ottiene soggetti filtrati per tipo (Azienda o Persona Fisica)
        /// </summary>
        public async Task<IEnumerable<Soggetto>> GetByTipoAsync(TipoSoggetto tipo)
        {
            return await _context.Soggetti
                .Where(s => s.TipoSoggetto == tipo)
                .OrderBy(s => s.Denominazione ?? s.Nome)
                .ToListAsync();
        }

        /// <summary>
        /// Ottiene soggetti filtrati per natura giuridica (PA o Privato)
        /// </summary>
        public async Task<IEnumerable<Soggetto>> GetByNaturaGiuridicaAsync(NaturaGiuridica natura)
        {
            return await _context.Soggetti
                .Where(s => s.NaturaGiuridica == natura)
                .OrderBy(s => s.Denominazione ?? s.Nome)
                .ToListAsync();
        }

        /// <summary>
        /// Ottiene soggetti con filtri multipli
        /// </summary>
        public async Task<IEnumerable<Soggetto>> GetFilteredAsync(
            TipoSoggetto? tipo = null,
            NaturaGiuridica? natura = null,
            bool? isCliente = null,
            bool? isFornitore = null)
        {
            var query = _context.Soggetti.AsQueryable();

            if (tipo.HasValue)
                query = query.Where(s => s.TipoSoggetto == tipo.Value);

            if (natura.HasValue)
                query = query.Where(s => s.NaturaGiuridica == natura.Value);

            if (isCliente.HasValue)
                query = query.Where(s => s.IsCliente == isCliente.Value);

            if (isFornitore.HasValue)
                query = query.Where(s => s.IsFornitore == isFornitore.Value);

            return await query
                .OrderBy(s => s.Denominazione ?? s.Nome)
                .ToListAsync();
        }

        // ===================================
        // PAGINAZIONE
        // ===================================

        /// <summary>
        /// Ottiene soggetti paginati con filtri e ordinamento
        /// </summary>
        public async Task<(IEnumerable<Soggetto> Items, int TotalCount)> GetPagedAsync(SoggettoFilterViewModel filters)
        {
            // 1. Costruisci la query base
            var query = _context.Soggetti.AsQueryable();

            // 2. Applica FILTRI

            // Filtro per tipo soggetto
            if (filters.TipoSoggetto.HasValue)
            {
                query = query.Where(s => s.TipoSoggetto == filters.TipoSoggetto.Value);
            }

            // Filtro per natura giuridica
            if (filters.NaturaGiuridica.HasValue)
            {
                query = query.Where(s => s.NaturaGiuridica == filters.NaturaGiuridica.Value);
            }

            // Filtro per cliente
            if (filters.IsCliente.HasValue)
            {
                query = query.Where(s => s.IsCliente == filters.IsCliente.Value);
            }

            // Filtro per fornitore
            if (filters.IsFornitore.HasValue)
            {
                query = query.Where(s => s.IsFornitore == filters.IsFornitore.Value);
            }

            // Filtro per ricerca full-text
            if (!string.IsNullOrWhiteSpace(filters.SearchTerm))
            {
                var term = filters.SearchTerm.ToLower().Trim();
                query = query.Where(s =>
                    (s.Denominazione != null && s.Denominazione.ToLower().Contains(term)) ||
                    (s.Nome != null && s.Nome.ToLower().Contains(term)) ||
                    (s.Cognome != null && s.Cognome.ToLower().Contains(term)) ||
                    s.Email.ToLower().Contains(term) ||
                    (s.CodiceInterno != null && s.CodiceInterno.ToLower().Contains(term)) ||
                    (s.PartitaIVA != null && s.PartitaIVA.ToLower().Contains(term)) ||
                    (s.CodiceFiscale != null && s.CodiceFiscale.ToLower().Contains(term)) ||
                    (s.Citta != null && s.Citta.ToLower().Contains(term)));
            }

            // 3. CONTA il totale PRIMA di applicare paginazione
            var totalCount = await query.CountAsync();

            // 4. Applica ORDINAMENTO
            query = ApplyOrdering(query, filters.OrderBy, filters.OrderDirection);

            // 5. Applica PAGINAZIONE (Skip e Take)
            var items = await query
                .Skip((filters.PageNumber - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        /// <summary>
        /// Applica ordinamento alla query
        /// </summary>
        private IQueryable<Soggetto> ApplyOrdering(IQueryable<Soggetto> query, string orderBy, string orderDirection)
        {
            var isAscending = orderDirection.ToLower() == "asc";

            return orderBy.ToLower() switch
            {
                "nome" => isAscending
                    ? query.OrderBy(s => s.Denominazione ?? s.Nome)
                    : query.OrderByDescending(s => s.Denominazione ?? s.Nome),

                "tipo" => isAscending
                    ? query.OrderBy(s => s.TipoSoggetto).ThenBy(s => s.Denominazione ?? s.Nome)
                    : query.OrderByDescending(s => s.TipoSoggetto).ThenBy(s => s.Denominazione ?? s.Nome),

                "natura" => isAscending
                    ? query.OrderBy(s => s.NaturaGiuridica).ThenBy(s => s.Denominazione ?? s.Nome)
                    : query.OrderByDescending(s => s.NaturaGiuridica).ThenBy(s => s.Denominazione ?? s.Nome),

                "citta" => isAscending
                    ? query.OrderBy(s => s.Citta).ThenBy(s => s.Denominazione ?? s.Nome)
                    : query.OrderByDescending(s => s.Citta).ThenBy(s => s.Denominazione ?? s.Nome),

                "email" => isAscending
                    ? query.OrderBy(s => s.Email).ThenBy(s => s.Denominazione ?? s.Nome)
                    : query.OrderByDescending(s => s.Email).ThenBy(s => s.Denominazione ?? s.Nome),

                "ruolo" => isAscending
                    ? query.OrderBy(s => s.IsCliente).ThenBy(s => s.IsFornitore).ThenBy(s => s.Denominazione ?? s.Nome)
                    : query.OrderByDescending(s => s.IsCliente).ThenByDescending(s => s.IsFornitore).ThenBy(s => s.Denominazione ?? s.Nome),

                // Default: ordina per nome
                _ => isAscending
                    ? query.OrderBy(s => s.Denominazione ?? s.Nome)
                    : query.OrderByDescending(s => s.Denominazione ?? s.Nome)
            };
        }

        // ===================================
        // RICERCHE SPECIFICHE
        // ===================================

        /// <summary>
        /// Cerca soggetti per testo (denominazione, nome, cognome, email)
        /// </summary>
        public async Task<IEnumerable<Soggetto>> SearchAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllAsync();

            var term = searchTerm.ToLower().Trim();

            return await _context.Soggetti
                .Where(s =>
                    (s.Denominazione != null && s.Denominazione.ToLower().Contains(term)) ||
                    (s.Nome != null && s.Nome.ToLower().Contains(term)) ||
                    (s.Cognome != null && s.Cognome.ToLower().Contains(term)) ||
                    s.Email.ToLower().Contains(term) ||
                    (s.CodiceInterno != null && s.CodiceInterno.ToLower().Contains(term)) ||
                    (s.PartitaIVA != null && s.PartitaIVA.ToLower().Contains(term)) ||
                    (s.CodiceFiscale != null && s.CodiceFiscale.ToLower().Contains(term)))
                .OrderBy(s => s.Denominazione ?? s.Nome)
                .ToListAsync();
        }

        /// <summary>
        /// Ottiene un soggetto per Codice Interno
        /// </summary>
        public async Task<Soggetto?> GetByCodiceInternoAsync(string codiceInterno)
        {
            if (string.IsNullOrWhiteSpace(codiceInterno))
                return null;

            return await _context.Soggetti
                .FirstOrDefaultAsync(s => s.CodiceInterno == codiceInterno);
        }

        /// <summary>
        /// Ottiene un soggetto per Partita IVA
        /// </summary>
        public async Task<Soggetto?> GetByPartitaIVAAsync(string partitaIva)
        {
            if (string.IsNullOrWhiteSpace(partitaIva))
                return null;

            return await _context.Soggetti
                .FirstOrDefaultAsync(s => s.PartitaIVA == partitaIva);
        }

        /// <summary>
        /// Ottiene un soggetto per Codice Fiscale
        /// </summary>
        public async Task<Soggetto?> GetByCodiceFiscaleAsync(string codiceFiscale)
        {
            if (string.IsNullOrWhiteSpace(codiceFiscale))
                return null;

            return await _context.Soggetti
                .FirstOrDefaultAsync(s => s.CodiceFiscale == codiceFiscale);
        }

        /// <summary>
        /// Ottiene un soggetto per Email
        /// </summary>
        public async Task<Soggetto?> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            return await _context.Soggetti
                .FirstOrDefaultAsync(s => s.Email == email);
        }

        // ===================================
        // VALIDAZIONI / ESISTENZA
        // ===================================

        /// <summary>
        /// Verifica se esiste un soggetto con il Codice Interno specificato
        /// </summary>
        public async Task<bool> ExistsByCodiceInternoAsync(string codiceInterno, Guid? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(codiceInterno))
                return false;

            var query = _context.Soggetti
                .Where(s => s.CodiceInterno == codiceInterno);

            if (excludeId.HasValue)
                query = query.Where(s => s.Id != excludeId.Value);

            return await query.AnyAsync();
        }

        /// <summary>
        /// Verifica se esiste un soggetto con la Partita IVA specificata
        /// </summary>
        public async Task<bool> ExistsByPartitaIVAAsync(string partitaIva, Guid? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(partitaIva))
                return false;

            var query = _context.Soggetti
                .Where(s => s.PartitaIVA == partitaIva);

            if (excludeId.HasValue)
                query = query.Where(s => s.Id != excludeId.Value);

            return await query.AnyAsync();
        }

        /// <summary>
        /// Verifica se esiste un soggetto con il Codice Fiscale specificato
        /// </summary>
        public async Task<bool> ExistsByCodiceFiscaleAsync(string codiceFiscale, Guid? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(codiceFiscale))
                return false;

            var query = _context.Soggetti
                .Where(s => s.CodiceFiscale == codiceFiscale);

            if (excludeId.HasValue)
                query = query.Where(s => s.Id != excludeId.Value);

            return await query.AnyAsync();
        }

        /// <summary>
        /// Verifica se esiste un soggetto con l'Email specificata
        /// </summary>
        public async Task<bool> ExistsByEmailAsync(string email, Guid? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var query = _context.Soggetti
                .Where(s => s.Email == email);

            if (excludeId.HasValue)
                query = query.Where(s => s.Id != excludeId.Value);

            return await query.AnyAsync();
        }

        // ===================================
        // STATISTICHE / CONTEGGI
        // ===================================

        /// <summary>
        /// Conta il numero totale di soggetti
        /// </summary>
        public async Task<int> CountAsync()
        {
            return await _context.Soggetti.CountAsync();
        }

        /// <summary>
        /// Conta il numero di clienti
        /// </summary>
        public async Task<int> CountClientiAsync()
        {
            return await _context.Soggetti
                .CountAsync(s => s.IsCliente);
        }

        /// <summary>
        /// Conta il numero di fornitori
        /// </summary>
        public async Task<int> CountFornitoriAsync()
        {
            return await _context.Soggetti
                .CountAsync(s => s.IsFornitore);
        }
    }
}