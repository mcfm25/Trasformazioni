using Microsoft.EntityFrameworkCore;
using Trasformazioni.Data;
using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;
using Trasformazioni.Repositories.Interfaces;

namespace Trasformazioni.Repositories
{
    /// <summary>
    /// Implementazione del repository per DocumentoGara
    /// Utilizza EF Core con query ottimizzate e include delle relazioni
    /// </summary>
    public class DocumentoGaraRepository : IDocumentoGaraRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DocumentoGaraRepository> _logger;

        public DocumentoGaraRepository(
            ApplicationDbContext context,
            ILogger<DocumentoGaraRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<DocumentoGara?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _context.DocumentiGara
                .Include(d => d.Gara)
                .Include(d => d.Lotto)
                .Include(d => d.Preventivo)
                .Include(d => d.Integrazione)
                .Include(d => d.CaricatoDa)
                .Include(d => d.TipoDocumento)
                .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted, cancellationToken);
        }

        public async Task<DocumentoGara?> GetByPathAsync(
            string pathMinIO,
            CancellationToken cancellationToken = default)
        {
            return await _context.DocumentiGara
                .Include(d => d.Gara)
                .Include(d => d.Lotto)
                .Include(d => d.Preventivo)
                .Include(d => d.Integrazione)
                .Include(d => d.CaricatoDa)
                .Include(d => d.TipoDocumento)
                .FirstOrDefaultAsync(d => d.PathMinIO == pathMinIO && !d.IsDeleted, cancellationToken);
        }

        public async Task<IEnumerable<DocumentoGara>> GetByGaraIdAsync(
            Guid garaId,
            CancellationToken cancellationToken = default)
        {
            return await _context.DocumentiGara
                .Include(d => d.CaricatoDa)
                .Include(d => d.TipoDocumento)
                .Where(d => d.GaraId == garaId && !d.IsDeleted)
                .OrderByDescending(d => d.DataCaricamento)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<DocumentoGara>> GetByLottoIdAsync(
            Guid lottoId,
            CancellationToken cancellationToken = default)
        {
            return await _context.DocumentiGara
                .Include(d => d.CaricatoDa)
                .Include(d => d.TipoDocumento)
                .Where(d => d.LottoId == lottoId && !d.IsDeleted)
                .OrderByDescending(d => d.DataCaricamento)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<DocumentoGara>> GetByPreventivoIdAsync(
            Guid preventivoId,
            CancellationToken cancellationToken = default)
        {
            return await _context.DocumentiGara
                .Include(d => d.CaricatoDa)
                .Include(d => d.TipoDocumento)
                .Where(d => d.PreventivoId == preventivoId && !d.IsDeleted)
                .OrderByDescending(d => d.DataCaricamento)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<DocumentoGara>> GetByIntegrazioneIdAsync(
            Guid integrazioneId,
            CancellationToken cancellationToken = default)
        {
            return await _context.DocumentiGara
                .Include(d => d.CaricatoDa)
                .Include(d => d.TipoDocumento)
                .Where(d => d.IntegrazioneId == integrazioneId && !d.IsDeleted)
                .OrderByDescending(d => d.DataCaricamento)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<DocumentoGara>> GetByGaraIdAndTipoAsync(
            Guid garaId,
            TipoDocumentoGara tipo,
            CancellationToken cancellationToken = default)
        {
            var codice = tipo.ToString();
            return await _context.DocumentiGara
                .Include(d => d.CaricatoDa)
                .Include(d => d.TipoDocumento)
                .Where(d => d.GaraId == garaId &&
                            d.TipoDocumento != null &&
                            d.TipoDocumento.CodiceRiferimento == codice &&
                            !d.IsDeleted)
                .OrderByDescending(d => d.DataCaricamento)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<DocumentoGara>> GetByLottoIdAndTipoAsync(
            Guid lottoId,
            TipoDocumentoGara tipo,
            CancellationToken cancellationToken = default)
        {
            var codice = tipo.ToString();
            return await _context.DocumentiGara
                .Include(d => d.CaricatoDa)
                .Include(d => d.TipoDocumento)
                .Where(d => d.LottoId == lottoId &&
                            d.TipoDocumento != null &&
                            d.TipoDocumento.CodiceRiferimento == codice &&
                            !d.IsDeleted)
                .OrderByDescending(d => d.DataCaricamento)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<DocumentoGara>> GetByUserIdAsync(
            string userId,
            CancellationToken cancellationToken = default)
        {
            return await _context.DocumentiGara
                .Include(d => d.Gara)
                .Include(d => d.Lotto)
                .Include(d => d.TipoDocumento)
                .Where(d => d.CaricatoDaUserId == userId && !d.IsDeleted)
                .OrderByDescending(d => d.DataCaricamento)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<DocumentoGara>> GetByDateRangeAsync(
            DateTime from,
            DateTime to,
            CancellationToken cancellationToken = default)
        {
            return await _context.DocumentiGara
                .Include(d => d.Gara)
                .Include(d => d.Lotto)
                .Include(d => d.CaricatoDa)
                .Include(d => d.TipoDocumento)
                .Where(d => d.DataCaricamento >= from && d.DataCaricamento <= to && !d.IsDeleted)
                .OrderByDescending(d => d.DataCaricamento)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> CountByGaraIdAsync(
            Guid garaId,
            CancellationToken cancellationToken = default)
        {
            return await _context.DocumentiGara
                .CountAsync(d => d.GaraId == garaId && !d.IsDeleted, cancellationToken);
        }

        public async Task<int> CountByLottoIdAsync(
            Guid lottoId,
            CancellationToken cancellationToken = default)
        {
            return await _context.DocumentiGara
                .CountAsync(d => d.LottoId == lottoId && !d.IsDeleted, cancellationToken);
        }

        public async Task<long> GetTotalSizeByGaraIdAsync(
            Guid garaId,
            CancellationToken cancellationToken = default)
        {
            return await _context.DocumentiGara
                .Where(d => d.GaraId == garaId && !d.IsDeleted)
                .SumAsync(d => d.DimensioneBytes, cancellationToken);
        }

        public async Task<long> GetTotalSizeByLottoIdAsync(
            Guid lottoId,
            CancellationToken cancellationToken = default)
        {
            return await _context.DocumentiGara
                .Where(d => d.LottoId == lottoId && !d.IsDeleted)
                .SumAsync(d => d.DimensioneBytes, cancellationToken);
        }

        public async Task<DocumentoGara> CreateAsync(
            DocumentoGara documento,
            CancellationToken cancellationToken = default)
        {
            _context.DocumentiGara.Add(documento);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Documento creato: {Id}, Path: {Path}, TipoDocumentoId: {TipoDocumentoId}",
                documento.Id, documento.PathMinIO, documento.TipoDocumentoId);

            return documento;
        }

        public async Task<DocumentoGara> UpdateAsync(
            DocumentoGara documento,
            CancellationToken cancellationToken = default)
        {
            _context.DocumentiGara.Update(documento);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Documento aggiornato: {Id}, Path: {Path}",
                documento.Id, documento.PathMinIO);

            return documento;
        }

        public async Task DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var documento = await _context.DocumentiGara
                .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

            if (documento != null)
            {
                documento.IsDeleted = true;
                documento.DeletedAt = DateTime.Now;
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Documento eliminato (soft delete): {Id}, Path: {Path}",
                    documento.Id, documento.PathMinIO);
            }
        }

        public async Task<bool> ExistsByPathAsync(
            string pathMinIO,
            CancellationToken cancellationToken = default)
        {
            return await _context.DocumentiGara
                .AnyAsync(d => d.PathMinIO == pathMinIO && !d.IsDeleted, cancellationToken);
        }

        public async Task<IEnumerable<DocumentoGara>> GetOrphanedDocumentsAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.DocumentiGara
                .Where(d => !d.IsDeleted &&
                           !d.GaraId.HasValue &&
                           !d.LottoId.HasValue &&
                           !d.PreventivoId.HasValue &&
                           !d.IntegrazioneId.HasValue)
                .ToListAsync(cancellationToken);
        }

        public async Task HardDeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var documento = await _context.DocumentiGara
                .IgnoreQueryFilters() // Ignora soft delete filter
                .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

            if (documento != null)
            {
                _context.DocumentiGara.Remove(documento);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Documento eliminato fisicamente: {Id}", id);
            }
        }
    }
}