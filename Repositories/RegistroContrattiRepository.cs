using Microsoft.EntityFrameworkCore;
using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;

namespace Trasformazioni.Data.Repositories
{
    /// <summary>
    /// Implementazione del repository per l'accesso ai dati del Registro Contratti
    /// </summary>
    public class RegistroContrattiRepository : IRegistroContrattiRepository
    {
        private readonly ApplicationDbContext _context;

        public RegistroContrattiRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===================================
        // OPERAZIONI BASE
        // ===================================

        /// <summary>
        /// Ottiene tutti i registri non cancellati
        /// </summary>
        public async Task<IEnumerable<RegistroContratti>> GetAllAsync()
        {
            return await _context.RegistroContratti
                .Include(r => r.Cliente)
                .Include(r => r.CategoriaContratto)
                .Include(r => r.Utente)
                .OrderByDescending(r => r.DataDocumento)
                .ToListAsync();
        }

        /// <summary>
        /// Ottiene un registro per ID con tutte le relazioni
        /// </summary>
        public async Task<RegistroContratti?> GetByIdAsync(Guid id)
        {
            return await _context.RegistroContratti
                .Include(r => r.Cliente)
                .Include(r => r.CategoriaContratto)
                .Include(r => r.Utente)
                .Include(r => r.Parent)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        /// <summary>
        /// Ottiene un registro per ID con gli allegati
        /// </summary>
        public async Task<RegistroContratti?> GetByIdWithAllegatiAsync(Guid id)
        {
            return await _context.RegistroContratti
                .Include(r => r.Cliente)
                .Include(r => r.CategoriaContratto)
                .Include(r => r.Utente)
                .Include(r => r.Parent)
                .Include(r => r.Children.Where(c => !c.IsDeleted))
                .Include(r => r.Allegati!.Where(a => !a.IsDeleted))
                    .ThenInclude(a => a.TipoDocumento)
                .Include(r => r.Allegati!.Where(a => !a.IsDeleted))
                    .ThenInclude(a => a.CaricatoDa)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        /// <summary>
        /// Aggiunge un nuovo registro
        /// </summary>
        public async Task AddAsync(RegistroContratti registro)
        {
            await _context.RegistroContratti.AddAsync(registro);
        }

        /// <summary>
        /// Aggiorna un registro esistente
        /// </summary>
        public Task UpdateAsync(RegistroContratti registro)
        {
            _context.RegistroContratti.Update(registro);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Elimina un registro (soft delete)
        /// </summary>
        public Task DeleteAsync(RegistroContratti registro)
        {
            registro.IsDeleted = true;
            registro.DeletedAt = DateTime.Now;
            _context.RegistroContratti.Update(registro);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Salva le modifiche nel database
        /// </summary>
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Verifica se esiste un registro con l'ID specificato
        /// </summary>
        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.RegistroContratti.AnyAsync(r => r.Id == id);
        }

        // ===================================
        // QUERY PER TIPO
        // ===================================

        /// <summary>
        /// Ottiene tutti i preventivi
        /// </summary>
        public async Task<IEnumerable<RegistroContratti>> GetPreventiviAsync()
        {
            return await GetByTipoAsync(TipoRegistro.Preventivo);
        }

        /// <summary>
        /// Ottiene tutti i contratti
        /// </summary>
        public async Task<IEnumerable<RegistroContratti>> GetContrattiAsync()
        {
            return await GetByTipoAsync(TipoRegistro.Contratto);
        }

        /// <summary>
        /// Ottiene registri per tipo
        /// </summary>
        public async Task<IEnumerable<RegistroContratti>> GetByTipoAsync(TipoRegistro tipo)
        {
            return await _context.RegistroContratti
                .Include(r => r.Cliente)
                .Include(r => r.CategoriaContratto)
                .Include(r => r.Utente)
                .Where(r => r.TipoRegistro == tipo)
                .OrderByDescending(r => r.DataDocumento)
                .ToListAsync();
        }

        // ===================================
        // QUERY PER STATO
        // ===================================

        /// <summary>
        /// Ottiene registri per stato
        /// </summary>
        public async Task<IEnumerable<RegistroContratti>> GetByStatoAsync(StatoRegistro stato)
        {
            return await _context.RegistroContratti
                .Include(r => r.Cliente)
                .Include(r => r.CategoriaContratto)
                .Include(r => r.Utente)
                .Where(r => r.Stato == stato)
                .OrderByDescending(r => r.DataDocumento)
                .ToListAsync();
        }

        /// <summary>
        /// Ottiene registri attivi
        /// </summary>
        public async Task<IEnumerable<RegistroContratti>> GetAttiviAsync()
        {
            return await GetByStatoAsync(StatoRegistro.Attivo);
        }

        /// <summary>
        /// Ottiene registri in scadenza (stato InScadenza o InScadenzaPropostoRinnovo)
        /// </summary>
        public async Task<IEnumerable<RegistroContratti>> GetInScadenzaAsync()
        {
            return await _context.RegistroContratti
                .Include(r => r.Cliente)
                .Include(r => r.CategoriaContratto)
                .Include(r => r.Utente)
                .Where(r => r.Stato == StatoRegistro.InScadenza ||
                            r.Stato == StatoRegistro.InScadenzaPropostoRinnovo)
                .OrderBy(r => r.DataFineOScadenza)
                .ToListAsync();
        }

        /// <summary>
        /// Ottiene registri scaduti
        /// </summary>
        public async Task<IEnumerable<RegistroContratti>> GetScadutiAsync()
        {
            return await GetByStatoAsync(StatoRegistro.Scaduto);
        }

        // ===================================
        // QUERY PER CLIENTE
        // ===================================

        /// <summary>
        /// Ottiene registri per cliente
        /// </summary>
        public async Task<IEnumerable<RegistroContratti>> GetByClienteIdAsync(Guid clienteId)
        {
            return await _context.RegistroContratti
                .Include(r => r.Cliente)
                .Include(r => r.CategoriaContratto)
                .Include(r => r.Utente)
                .Where(r => r.ClienteId == clienteId)
                .OrderByDescending(r => r.DataDocumento)
                .ToListAsync();
        }

        /// <summary>
        /// Ottiene registri attivi per cliente
        /// </summary>
        public async Task<IEnumerable<RegistroContratti>> GetAttiviByClienteIdAsync(Guid clienteId)
        {
            return await _context.RegistroContratti
                .Include(r => r.Cliente)
                .Include(r => r.CategoriaContratto)
                .Include(r => r.Utente)
                .Where(r => r.ClienteId == clienteId && r.Stato == StatoRegistro.Attivo)
                .OrderByDescending(r => r.DataDocumento)
                .ToListAsync();
        }

        // ===================================
        // QUERY PER CATEGORIA
        // ===================================

        /// <summary>
        /// Ottiene registri per categoria
        /// </summary>
        public async Task<IEnumerable<RegistroContratti>> GetByCategoriaIdAsync(Guid categoriaId)
        {
            return await _context.RegistroContratti
                .Include(r => r.Cliente)
                .Include(r => r.CategoriaContratto)
                .Include(r => r.Utente)
                .Where(r => r.CategoriaContrattoId == categoriaId)
                .OrderByDescending(r => r.DataDocumento)
                .ToListAsync();
        }

        // ===================================
        // QUERY PER RESPONSABILE
        // ===================================

        /// <summary>
        /// Ottiene registri per responsabile interno
        /// </summary>
        public async Task<IEnumerable<RegistroContratti>> GetByUtenteIdAsync(string utenteId)
        {
            return await _context.RegistroContratti
                .Include(r => r.Cliente)
                .Include(r => r.CategoriaContratto)
                .Include(r => r.Utente)
                .Where(r => r.UtenteId == utenteId)
                .OrderByDescending(r => r.DataDocumento)
                .ToListAsync();
        }

        // ===================================
        // QUERY PER GERARCHIA
        // ===================================

        /// <summary>
        /// Ottiene i figli di un registro (versioni successive)
        /// </summary>
        public async Task<IEnumerable<RegistroContratti>> GetChildrenAsync(Guid parentId)
        {
            return await _context.RegistroContratti
                .Include(r => r.Cliente)
                .Include(r => r.CategoriaContratto)
                .Where(r => r.ParentId == parentId)
                .OrderByDescending(r => r.DataDocumento)
                .ToListAsync();
        }

        /// <summary>
        /// Ottiene la catena completa dei padri (storico versioni)
        /// </summary>
        public async Task<IEnumerable<RegistroContratti>> GetParentChainAsync(Guid id)
        {
            var chain = new List<RegistroContratti>();
            var current = await GetByIdAsync(id);

            while (current?.ParentId != null)
            {
                var parent = await GetByIdAsync(current.ParentId.Value);
                if (parent != null)
                {
                    chain.Add(parent);
                    current = parent;
                }
                else
                {
                    break;
                }
            }

            return chain;
        }

        /// <summary>
        /// Ottiene i registri radice (senza parent)
        /// </summary>
        public async Task<IEnumerable<RegistroContratti>> GetRootRegistriAsync()
        {
            return await _context.RegistroContratti
                .Include(r => r.Cliente)
                .Include(r => r.CategoriaContratto)
                .Include(r => r.Utente)
                .Where(r => r.ParentId == null)
                .OrderByDescending(r => r.DataDocumento)
                .ToListAsync();
        }

        // ===================================
        // QUERY PER SCADENZE (JOB)
        // ===================================

        /// <summary>
        /// Ottiene registri che devono passare in stato InScadenza
        /// </summary>
        public async Task<IEnumerable<RegistroContratti>> GetRegistriPerAlertScadenzaAsync()
        {
            var oggi = DateTime.Now.Date;

            return await _context.RegistroContratti
                .Include(r => r.Cliente)
                .Include(r => r.CategoriaContratto)
                .Include(r => r.Utente)
                .Where(r => r.Stato == StatoRegistro.Attivo &&
                            r.DataFineOScadenza.HasValue &&
                            r.GiorniPreavvisoDisdetta.HasValue)
                .ToListAsync()
                .ContinueWith(task => task.Result
                    .Where(r => r.DataAlertScadenza.HasValue && r.DataAlertScadenza.Value.Date <= oggi)
                    .ToList());
        }

        /// <summary>
        /// Ottiene registri che devono passare in stato Scaduto
        /// </summary>
        public async Task<IEnumerable<RegistroContratti>> GetRegistriDaScadereAsync()
        {
            var oggi = DateTime.Now.Date;

            return await _context.RegistroContratti
                .Include(r => r.Cliente)
                .Include(r => r.CategoriaContratto)
                .Include(r => r.Utente)
                .Where(r => (r.Stato == StatoRegistro.Attivo ||
                             r.Stato == StatoRegistro.InScadenza ||
                             r.Stato == StatoRegistro.InScadenzaPropostoRinnovo) &&
                            r.DataFineOScadenza.HasValue &&
                            r.DataFineOScadenza.Value.Date < oggi)
                .ToListAsync();
        }

        /// <summary>
        /// Ottiene registri con rinnovo automatico da processare
        /// </summary>
        public async Task<IEnumerable<RegistroContratti>> GetRegistriPerRinnovoAutomaticoAsync()
        {
            var oggi = DateTime.Now.Date;

            return await _context.RegistroContratti
                .Include(r => r.Cliente)
                .Include(r => r.CategoriaContratto)
                .Include(r => r.Utente)
                .Where(r => r.IsRinnovoAutomatico &&
                            r.GiorniRinnovoAutomatico.HasValue &&
                            r.Stato == StatoRegistro.InScadenza &&
                            r.DataFineOScadenza.HasValue &&
                            r.DataFineOScadenza.Value.Date <= oggi)
                .ToListAsync();
        }

        // ===================================
        // RICERCHE
        // ===================================

        /// <summary>
        /// Cerca registri per testo (oggetto, ragione sociale, protocollo)
        /// </summary>
        public async Task<IEnumerable<RegistroContratti>> SearchAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllAsync();

            var term = searchTerm.ToLower().Trim();

            return await _context.RegistroContratti
                .Include(r => r.Cliente)
                .Include(r => r.CategoriaContratto)
                .Include(r => r.Utente)
                .Where(r =>
                    r.Oggetto.ToLower().Contains(term) ||
                    (r.RagioneSociale != null && r.RagioneSociale.ToLower().Contains(term)) ||
                    (r.NumeroProtocollo != null && r.NumeroProtocollo.ToLower().Contains(term)) ||
                    (r.IdRiferimentoEsterno != null && r.IdRiferimentoEsterno.ToLower().Contains(term)) ||
                    (r.ResponsabileInterno != null && r.ResponsabileInterno.ToLower().Contains(term)) ||
                    r.CategoriaContratto.Nome.ToLower().Contains(term))
                .OrderByDescending(r => r.DataDocumento)
                .ToListAsync();
        }

        /// <summary>
        /// Ottiene un registro per numero protocollo
        /// </summary>
        public async Task<RegistroContratti?> GetByNumeroProtocolloAsync(string numeroProtocollo)
        {
            if (string.IsNullOrWhiteSpace(numeroProtocollo))
                return null;

            return await _context.RegistroContratti
                .Include(r => r.Cliente)
                .Include(r => r.CategoriaContratto)
                .Include(r => r.Utente)
                .FirstOrDefaultAsync(r => r.NumeroProtocollo == numeroProtocollo.Trim());
        }

        /// <summary>
        /// Ottiene un registro per riferimento esterno
        /// </summary>
        public async Task<RegistroContratti?> GetByIdRiferimentoEsternoAsync(string idRiferimentoEsterno)
        {
            if (string.IsNullOrWhiteSpace(idRiferimentoEsterno))
                return null;

            return await _context.RegistroContratti
                .Include(r => r.Cliente)
                .Include(r => r.CategoriaContratto)
                .Include(r => r.Utente)
                .FirstOrDefaultAsync(r => r.IdRiferimentoEsterno == idRiferimentoEsterno.Trim());
        }

        // ===================================
        // VALIDAZIONI
        // ===================================

        /// <summary>
        /// Verifica se esiste un registro con il numero protocollo specificato
        /// </summary>
        public async Task<bool> ExistsByNumeroProtocolloAsync(string numeroProtocollo, Guid? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(numeroProtocollo))
                return false;

            var query = _context.RegistroContratti
                .Where(r => r.NumeroProtocollo == numeroProtocollo.Trim());

            if (excludeId.HasValue)
                query = query.Where(r => r.Id != excludeId.Value);

            return await query.AnyAsync();
        }

        /// <summary>
        /// Ottiene il prossimo numero progressivo per il protocollo
        /// </summary>
        public async Task<int> GetNextProgressivoProtocolloAsync()
        {
            // Recupera tutti i protocolli esistenti
            var protocolli = await _context.RegistroContratti
                .Where(r => !r.IsDeleted && r.NumeroProtocollo != null && r.NumeroProtocollo != "")
                .Select(r => r.NumeroProtocollo)
                .ToListAsync();

            if (!protocolli.Any())
                return 1;

            // Estrai il numero progressivo dal formato XXXX-YYYY-NNNN
            var maxProgressivo = protocolli
                .Select(p =>
                {
                    if (string.IsNullOrEmpty(p))
                        return 0;

                    var parts = p.Split('-');
                    // Formato atteso: PREV-2025-0001 o CONTR-2025-0001
                    if (parts.Length >= 3 && int.TryParse(parts[^1], out var num))
                        return num;

                    // Prova anche a estrarre solo numeri dalla fine (per protocolli custom)
                    var numericPart = new string(p.Reverse().TakeWhile(char.IsDigit).Reverse().ToArray());
                    if (int.TryParse(numericPart, out var numAlt))
                        return numAlt;

                    return 0;
                })
                .Max();

            return maxProgressivo + 1;
        }

        // ===================================
        // PAGINAZIONE
        // ===================================

        /// <summary>
        /// Ottiene registri paginati con filtri
        /// </summary>
        public async Task<(IEnumerable<RegistroContratti> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            TipoRegistro? tipo = null,
            StatoRegistro? stato = null,
            Guid? clienteId = null,
            Guid? categoriaId = null,
            bool? soloRoot = null,
            bool? soloConVersioni = null,
            string? utenteId = null,
            string? searchTerm = null,
            string orderBy = "DataDocumento",
            string orderDirection = "desc")
        {
            var query = _context.RegistroContratti
                .Include(r => r.Cliente)
                .Include(r => r.CategoriaContratto)
                .Include(r => r.Utente)
                .AsQueryable();

            // Applica filtri
            if (tipo.HasValue)
                query = query.Where(r => r.TipoRegistro == tipo.Value);

            if (stato.HasValue)
                query = query.Where(r => r.Stato == stato.Value);

            if (clienteId.HasValue)
                query = query.Where(r => r.ClienteId == clienteId.Value);

            if (categoriaId.HasValue)
                query = query.Where(r => r.CategoriaContrattoId == categoriaId.Value);

            if (soloRoot == true)
            {
                query = query.Where(r => r.ParentId == null);
            }

            if (soloConVersioni == true)
            {
                query = query.Where(r => r.ParentId != null || r.Children.Any(c => !c.IsDeleted));
            }

            if (!string.IsNullOrWhiteSpace(utenteId))
                query = query.Where(r => r.UtenteId == utenteId);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower().Trim();
                query = query.Where(r =>
                    r.Oggetto.ToLower().Contains(term) ||
                    (r.RagioneSociale != null && r.RagioneSociale.ToLower().Contains(term)) ||
                    (r.NumeroProtocollo != null && r.NumeroProtocollo.ToLower().Contains(term)) ||
                    (r.IdRiferimentoEsterno != null && r.IdRiferimentoEsterno.ToLower().Contains(term)));
            }

            // Conta totale
            var totalCount = await query.CountAsync();

            // Applica ordinamento
            query = ApplyOrdering(query, orderBy, orderDirection);

            // Applica paginazione
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        /// <summary>
        /// Applica ordinamento alla query
        /// </summary>
        private IQueryable<RegistroContratti> ApplyOrdering(
            IQueryable<RegistroContratti> query,
            string orderBy,
            string orderDirection)
        {
            var isAscending = orderDirection.ToLower() == "asc";

            return orderBy.ToLower() switch
            {
                "datadocumento" => isAscending
                    ? query.OrderBy(r => r.DataDocumento)
                    : query.OrderByDescending(r => r.DataDocumento),

                "datafineoscadenza" => isAscending
                    ? query.OrderBy(r => r.DataFineOScadenza)
                    : query.OrderByDescending(r => r.DataFineOScadenza),

                "ragionesociale" => isAscending
                    ? query.OrderBy(r => r.RagioneSociale)
                    : query.OrderByDescending(r => r.RagioneSociale),

                "oggetto" => isAscending
                    ? query.OrderBy(r => r.Oggetto)
                    : query.OrderByDescending(r => r.Oggetto),

                "stato" => isAscending
                    ? query.OrderBy(r => r.Stato).ThenByDescending(r => r.DataDocumento)
                    : query.OrderByDescending(r => r.Stato).ThenByDescending(r => r.DataDocumento),

                "tipo" => isAscending
                    ? query.OrderBy(r => r.TipoRegistro).ThenByDescending(r => r.DataDocumento)
                    : query.OrderByDescending(r => r.TipoRegistro).ThenByDescending(r => r.DataDocumento),

                "importo" => isAscending
                    ? query.OrderBy(r => r.ImportoCanoneAnnuo)
                    : query.OrderByDescending(r => r.ImportoCanoneAnnuo),

                "categoria" => isAscending
                    ? query.OrderBy(r => r.CategoriaContratto.Nome).ThenByDescending(r => r.DataDocumento)
                    : query.OrderByDescending(r => r.CategoriaContratto.Nome).ThenByDescending(r => r.DataDocumento),

                _ => isAscending
                    ? query.OrderBy(r => r.DataDocumento)
                    : query.OrderByDescending(r => r.DataDocumento)
            };
        }

        // ===================================
        // STATISTICHE
        // ===================================

        /// <summary>
        /// Conta il numero totale di registri
        /// </summary>
        public async Task<int> CountAsync()
        {
            return await _context.RegistroContratti.CountAsync();
        }

        /// <summary>
        /// Conta registri per tipo
        /// </summary>
        public async Task<int> CountByTipoAsync(TipoRegistro tipo)
        {
            return await _context.RegistroContratti
                .CountAsync(r => r.TipoRegistro == tipo);
        }

        /// <summary>
        /// Conta registri per stato
        /// </summary>
        public async Task<int> CountByStatoAsync(StatoRegistro stato)
        {
            return await _context.RegistroContratti
                .CountAsync(r => r.Stato == stato);
        }

        /// <summary>
        /// Calcola il totale importi canone annuo per stato
        /// </summary>
        public async Task<decimal> SumImportoCanoneByStatoAsync(StatoRegistro stato)
        {
            return await _context.RegistroContratti
                .Where(r => r.Stato == stato && r.ImportoCanoneAnnuo.HasValue)
                .SumAsync(r => r.ImportoCanoneAnnuo!.Value);
        }

        /// <summary>
        /// Calcola il totale importi canone annuo per cliente
        /// </summary>
        public async Task<decimal> SumImportoCanoneByClienteAsync(Guid clienteId)
        {
            return await _context.RegistroContratti
                .Where(r => r.ClienteId == clienteId &&
                            r.Stato == StatoRegistro.Attivo &&
                            r.ImportoCanoneAnnuo.HasValue)
                .SumAsync(r => r.ImportoCanoneAnnuo!.Value);
        }
    }
}