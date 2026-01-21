using Microsoft.EntityFrameworkCore;
using Trasformazioni.Models.Entities;

namespace Trasformazioni.Data.Repositories
{
    /// <summary>
    /// Implementazione del repository per l'accesso ai dati degli allegati del Registro Contratti
    /// </summary>
    public class AllegatoRegistroRepository : IAllegatoRegistroRepository
    {
        private readonly ApplicationDbContext _context;

        public AllegatoRegistroRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===================================
        // OPERAZIONI BASE
        // ===================================

        /// <summary>
        /// Ottiene tutti gli allegati non cancellati
        /// </summary>
        public async Task<IEnumerable<AllegatoRegistro>> GetAllAsync()
        {
            return await _context.AllegatiRegistro
                .Include(a => a.RegistroContratti)
                .Include(a => a.TipoDocumento)
                .Include(a => a.CaricatoDa)
                .OrderByDescending(a => a.DataCaricamento)
                .ToListAsync();
        }

        /// <summary>
        /// Ottiene un allegato per ID
        /// </summary>
        public async Task<AllegatoRegistro?> GetByIdAsync(Guid id)
        {
            return await _context.AllegatiRegistro
                .Include(a => a.RegistroContratti)
                .Include(a => a.TipoDocumento)
                .Include(a => a.CaricatoDa)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        /// <summary>
        /// Aggiunge un nuovo allegato
        /// </summary>
        public async Task AddAsync(AllegatoRegistro allegato)
        {
            await _context.AllegatiRegistro.AddAsync(allegato);
        }

        /// <summary>
        /// Aggiorna un allegato esistente
        /// </summary>
        public Task UpdateAsync(AllegatoRegistro allegato)
        {
            _context.AllegatiRegistro.Update(allegato);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Elimina un allegato (soft delete)
        /// </summary>
        public Task DeleteAsync(AllegatoRegistro allegato)
        {
            allegato.IsDeleted = true;
            allegato.DeletedAt = DateTime.Now;
            _context.AllegatiRegistro.Update(allegato);
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
        /// Verifica se esiste un allegato con l'ID specificato
        /// </summary>
        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.AllegatiRegistro.AnyAsync(a => a.Id == id);
        }

        // ===================================
        // QUERY PER REGISTRO
        // ===================================

        /// <summary>
        /// Ottiene tutti gli allegati di un registro
        /// </summary>
        public async Task<IEnumerable<AllegatoRegistro>> GetByRegistroIdAsync(Guid registroContrattiId)
        {
            return await _context.AllegatiRegistro
                .Include(a => a.TipoDocumento)
                .Include(a => a.CaricatoDa)
                .Where(a => a.RegistroContrattiId == registroContrattiId)
                .OrderByDescending(a => a.DataCaricamento)
                .ToListAsync();
        }

        /// <summary>
        /// Ottiene allegati di un registro per tipo documento
        /// </summary>
        public async Task<IEnumerable<AllegatoRegistro>> GetByRegistroIdAndTipoAsync(Guid registroContrattiId, Guid tipoDocumentoId)
        {
            return await _context.AllegatiRegistro
                .Include(a => a.TipoDocumento)
                .Include(a => a.CaricatoDa)
                .Where(a => a.RegistroContrattiId == registroContrattiId &&
                            a.TipoDocumentoId == tipoDocumentoId)
                .OrderByDescending(a => a.DataCaricamento)
                .ToListAsync();
        }

        /// <summary>
        /// Conta gli allegati di un registro
        /// </summary>
        public async Task<int> CountByRegistroIdAsync(Guid registroContrattiId)
        {
            return await _context.AllegatiRegistro
                .CountAsync(a => a.RegistroContrattiId == registroContrattiId);
        }

        // ===================================
        // QUERY PER TIPO DOCUMENTO
        // ===================================

        /// <summary>
        /// Ottiene tutti gli allegati per tipo documento
        /// </summary>
        public async Task<IEnumerable<AllegatoRegistro>> GetByTipoDocumentoIdAsync(Guid tipoDocumentoId)
        {
            return await _context.AllegatiRegistro
                .Include(a => a.RegistroContratti)
                .Include(a => a.TipoDocumento)
                .Include(a => a.CaricatoDa)
                .Where(a => a.TipoDocumentoId == tipoDocumentoId)
                .OrderByDescending(a => a.DataCaricamento)
                .ToListAsync();
        }

        // ===================================
        // QUERY PER UTENTE
        // ===================================

        /// <summary>
        /// Ottiene tutti gli allegati caricati da un utente
        /// </summary>
        public async Task<IEnumerable<AllegatoRegistro>> GetByUtenteIdAsync(string utenteId)
        {
            return await _context.AllegatiRegistro
                .Include(a => a.RegistroContratti)
                .Include(a => a.TipoDocumento)
                .Include(a => a.CaricatoDa)
                .Where(a => a.CaricatoDaUserId == utenteId)
                .OrderByDescending(a => a.DataCaricamento)
                .ToListAsync();
        }

        // ===================================
        // QUERY PER STATO UPLOAD
        // ===================================

        /// <summary>
        /// Ottiene allegati con upload incompleto (per cleanup)
        /// </summary>
        public async Task<IEnumerable<AllegatoRegistro>> GetUploadIncompleti()
        {
            return await _context.AllegatiRegistro
                .Include(a => a.RegistroContratti)
                .Where(a => !a.IsUploadCompleto)
                .OrderBy(a => a.DataCaricamento)
                .ToListAsync();
        }

        /// <summary>
        /// Ottiene allegati con upload incompleto più vecchi di una certa data
        /// </summary>
        public async Task<IEnumerable<AllegatoRegistro>> GetUploadIncompletiOlderThanAsync(DateTime dataLimite)
        {
            return await _context.AllegatiRegistro
                .Include(a => a.RegistroContratti)
                .Where(a => !a.IsUploadCompleto && a.DataCaricamento < dataLimite)
                .OrderBy(a => a.DataCaricamento)
                .ToListAsync();
        }

        // ===================================
        // RICERCHE
        // ===================================

        /// <summary>
        /// Cerca allegati per nome file
        /// </summary>
        public async Task<IEnumerable<AllegatoRegistro>> SearchByNomeFileAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllAsync();

            var term = searchTerm.ToLower().Trim();

            return await _context.AllegatiRegistro
                .Include(a => a.RegistroContratti)
                .Include(a => a.TipoDocumento)
                .Include(a => a.CaricatoDa)
                .Where(a => a.NomeFile.ToLower().Contains(term) ||
                            (a.Descrizione != null && a.Descrizione.ToLower().Contains(term)))
                .OrderByDescending(a => a.DataCaricamento)
                .ToListAsync();
        }

        /// <summary>
        /// Ottiene un allegato per path MinIO
        /// </summary>
        public async Task<AllegatoRegistro?> GetByPathMinIOAsync(string pathMinIO)
        {
            if (string.IsNullOrWhiteSpace(pathMinIO))
                return null;

            return await _context.AllegatiRegistro
                .Include(a => a.RegistroContratti)
                .Include(a => a.TipoDocumento)
                .Include(a => a.CaricatoDa)
                .FirstOrDefaultAsync(a => a.PathMinIO == pathMinIO);
        }

        // ===================================
        // VALIDAZIONI
        // ===================================

        /// <summary>
        /// Verifica se esiste già un allegato con lo stesso nome nello stesso registro
        /// </summary>
        public async Task<bool> ExistsByNomeFileInRegistroAsync(Guid registroContrattiId, string nomeFile, Guid? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(nomeFile))
                return false;

            var query = _context.AllegatiRegistro
                .Where(a => a.RegistroContrattiId == registroContrattiId &&
                            a.NomeFile.ToLower() == nomeFile.ToLower().Trim());

            if (excludeId.HasValue)
                query = query.Where(a => a.Id != excludeId.Value);

            return await query.AnyAsync();
        }

        // ===================================
        // STATISTICHE
        // ===================================

        /// <summary>
        /// Conta il numero totale di allegati
        /// </summary>
        public async Task<int> CountAsync()
        {
            return await _context.AllegatiRegistro.CountAsync();
        }

        /// <summary>
        /// Calcola la dimensione totale degli allegati di un registro (in bytes)
        /// </summary>
        public async Task<long> GetTotalSizeByRegistroIdAsync(Guid registroContrattiId)
        {
            return await _context.AllegatiRegistro
                .Where(a => a.RegistroContrattiId == registroContrattiId)
                .SumAsync(a => a.DimensioneBytes);
        }

        /// <summary>
        /// Calcola la dimensione totale di tutti gli allegati (in bytes)
        /// </summary>
        public async Task<long> GetTotalSizeAsync()
        {
            return await _context.AllegatiRegistro
                .SumAsync(a => a.DimensioneBytes);
        }
    }
}