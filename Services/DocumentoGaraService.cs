using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Trasformazioni.Configuration;
using Trasformazioni.Extensions;
using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;
using Trasformazioni.Models.ViewModels.DocumentoGara;
using Trasformazioni.Repositories.Interfaces;
using Trasformazioni.Services.Interfaces;

namespace Trasformazioni.Services
{
    /// <summary>
    /// Implementazione del servizio per la gestione dei documenti delle gare
    /// Coordina Repository, MinIO Service e business logic
    /// </summary>
    public class DocumentoGaraService : IDocumentoGaraService
    {
        private readonly IDocumentoGaraRepository _repository;
        private readonly IMinIOService _minioService;
        private readonly FileUploadConfiguration _fileConfig;
        private readonly ILogger<DocumentoGaraService> _logger;

        // Configurazione validazione file
        //private const long MAX_FILE_SIZE = 128 * 1024 * 1024; // 128 MB
        //private static readonly string[] ALLOWED_EXTENSIONS = { ".pdf", ".docx", ".doc", ".zip", ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
        //private static readonly string[] ALLOWED_MIME_TYPES = {
        //    "application/pdf",
        //    "application/msword",
        //    "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        //    "application/zip",
        //    "application/x-zip-compressed",
        //    "image/jpeg",
        //    "image/png",
        //    "image/gif",
        //    "image/bmp"
        //};

        public DocumentoGaraService(
            IDocumentoGaraRepository repository,
            IMinIOService minioService,
            IOptions<FileUploadConfiguration> fileConfig,
            ILogger<DocumentoGaraService> logger)
        {
            _repository = repository;
            _minioService = minioService;
            _logger = logger;
            _fileConfig = fileConfig.Value;
        }

        public async Task<DocumentoGara?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _repository.GetByIdAsync(id, cancellationToken);
        }

        public async Task<IEnumerable<DocumentoGaraListViewModel>> GetByGaraIdAsync(
            Guid garaId,
            CancellationToken cancellationToken = default)
        {
            var documenti = await _repository.GetByGaraIdAsync(garaId, cancellationToken);
            return documenti.ToListViewModels();
        }

        public async Task<IEnumerable<DocumentoGaraListViewModel>> GetByLottoIdAsync(
            Guid lottoId,
            CancellationToken cancellationToken = default)
        {
            var documenti = await _repository.GetByLottoIdAsync(lottoId, cancellationToken);
            return documenti.ToListViewModels();
        }

        public async Task<IEnumerable<DocumentoGaraListViewModel>> GetByPreventivoIdAsync(
            Guid preventivoId,
            CancellationToken cancellationToken = default)
        {
            var documenti = await _repository.GetByPreventivoIdAsync(preventivoId, cancellationToken);
            return documenti.ToListViewModels();
        }

        public async Task<IEnumerable<DocumentoGaraListViewModel>> GetByIntegrazioneIdAsync(
            Guid integrazioneId,
            CancellationToken cancellationToken = default)
        {
            var documenti = await _repository.GetByIntegrazioneIdAsync(integrazioneId, cancellationToken);
            return documenti.ToListViewModels();
        }

        public async Task<IEnumerable<DocumentoGaraListViewModel>> GetFilteredAsync(
            DocumentoGaraFilterViewModel filter,
            CancellationToken cancellationToken = default)
        {
            IEnumerable<DocumentoGara> documenti;

            // Applica filtri gerarchici
            if (filter.IntegrazioneId.HasValue)
            {
                documenti = await _repository.GetByIntegrazioneIdAsync(filter.IntegrazioneId.Value, cancellationToken);
            }
            else if (filter.PreventivoId.HasValue)
            {
                documenti = await _repository.GetByPreventivoIdAsync(filter.PreventivoId.Value, cancellationToken);
            }
            else if (filter.LottoId.HasValue)
            {
                documenti = await _repository.GetByLottoIdAsync(filter.LottoId.Value, cancellationToken);
            }
            else if (filter.GaraId.HasValue)
            {
                documenti = await _repository.GetByGaraIdAsync(filter.GaraId.Value, cancellationToken);
            }
            else
            {
                // Nessun filtro gerarchico, usa filtri avanzati
                documenti = await ApplyAdvancedFilters(filter, cancellationToken);
            }

            // Applica filtri aggiuntivi
            documenti = ApplyAdditionalFilters(documenti, filter);

            // Applica ordinamento
            documenti = ApplyOrdering(documenti, filter.OrderBy, filter.OrderAscending);

            return documenti.ToListViewModels();
        }

        public async Task<DocumentoGara> UploadAsync(
            IFormFile file,
            DocumentoGaraUploadViewModel viewModel,
            string userId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(
        "Inizio upload documento per Gara {GaraId}, TipoDocumentoId: {TipoDocumentoId}, User: {UserId}",
        viewModel.GaraId, viewModel.TipoDocumentoId, userId);

            // Validazione file
            var (isValid, errorMessage) = ValidateFile(file);
            if (!isValid)
            {
                _logger.LogWarning("Validazione file fallita: {ErrorMessage}", errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

            // Genera path MinIO
            var objectPath = _minioService.GenerateObjectPath(
                viewModel.GaraId,
                viewModel.LottoId,
                viewModel.PreventivoId,
                viewModel.IntegrazioneId,
                file.FileName);

            _logger.LogInformation("Path MinIO generato: {Path}", objectPath);

            // STEP 1: Crea record DB con IsUploadCompleto = false
            var documento = viewModel.ToEntity(
                file.FileName,
                objectPath,
                file.Length,
                file.ContentType,
                userId);

            documento.IsUploadCompleto = false;

            try
            {
                documento = await _repository.CreateAsync(documento, cancellationToken);
                _logger.LogInformation(
                    "Record DB creato (pending): {Id}, Path: {Path}",
                    documento.Id, objectPath);
            }
            catch (Exception dbEx)
            {
                _logger.LogError(dbEx, "Errore creazione record DB per documento");
                throw;
            }

            // STEP 2: Upload su MinIO
            try
            {
                using var stream = file.OpenReadStream();
                await _minioService.UploadFileAsync(
                    stream,
                    objectPath,
                    file.ContentType,
                    cancellationToken);

                _logger.LogInformation("File caricato su MinIO: {Path}", objectPath);
            }
            catch (Exception minioEx)
            {
                _logger.LogError(minioEx,
                    "Errore upload MinIO, eseguo rollback record DB: {DocumentoId}",
                    documento.Id);

                // COMPENSAZIONE: Elimina record DB (hard delete)
                await RollbackDocumentoAsync(documento.Id, cancellationToken);

                throw new InvalidOperationException(
                    "Errore durante il caricamento del file. Riprova.", minioEx);
            }

            // STEP 3: Marca upload come completato
            try
            {
                documento.IsUploadCompleto = true;
                documento = await _repository.UpdateAsync(documento, cancellationToken);

                _logger.LogInformation(
                    "Documento caricato con successo: {Id}, Path: {Path}",
                    documento.Id, objectPath);

                return documento;
            }
            catch (Exception updateEx)
            {
                // Caso raro: file su MinIO ma DB non aggiornato
                // Il file rimane, il record ha IsUploadCompleto = false
                // Un job di pulizia potrà gestirlo
                _logger.LogError(updateEx,
                    "Errore aggiornamento stato upload. File presente su MinIO ma record pending: {DocumentoId}",
                    documento.Id);

                throw;
            }
        }

        /// <summary>
        /// Elimina un file da MinIO in modo sicuro (ignora errori)
        /// </summary>
        private async Task CleanupMinIOFileAsync(string objectPath, CancellationToken cancellationToken)
        {
            try
            {
                if (await _minioService.FileExistsAsync(objectPath, cancellationToken))
                {
                    await _minioService.DeleteFileAsync(objectPath, cancellationToken);
                    _logger.LogInformation("File pulito da MinIO: {Path}", objectPath);
                }
            }
            catch (Exception cleanupEx)
            {
                _logger.LogError(cleanupEx, "Errore durante il cleanup del file: {Path}", objectPath);
            }
        }

        /// <summary>
        /// Elimina un record documento in caso di fallimento upload (hard delete)
        /// </summary>
        private async Task RollbackDocumentoAsync(Guid documentoId, CancellationToken cancellationToken)
        {
            try
            {
                await _repository.HardDeleteAsync(documentoId, cancellationToken);
                _logger.LogInformation("Rollback record DB completato: {DocumentoId}", documentoId);
            }
            catch (Exception ex)
            {
                // Log ma non rilanciare - il record rimarrà orfano con IsUploadCompleto = false
                // Un job di pulizia lo gestirà
                _logger.LogError(ex,
                    "Errore rollback record DB. Record orfano: {DocumentoId}",
                    documentoId);
            }
        }

        public async Task<(Stream FileStream, string FileName, string ContentType)> DownloadAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Download documento: {Id}", id);

            var documento = await _repository.GetByIdAsync(id, cancellationToken);

            if (documento == null)
            {
                _logger.LogWarning("Documento non trovato: {Id}", id);
                throw new InvalidOperationException($"Documento con ID {id} non trovato");
            }

            try
            {
                var stream = await _minioService.DownloadFileAsync(
                    documento.PathMinIO,
                    cancellationToken);

                _logger.LogInformation(
                    "Documento scaricato con successo: {Id}, File: {FileName}",
                    id, documento.NomeFile);

                return (stream, documento.NomeFile, documento.MimeType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Errore durante il download del documento: {Id}, Path: {Path}",
                    id, documento.PathMinIO);
                throw;
            }
        }

        public async Task<DocumentoGara> UpdateAsync(
            Guid id,
            DocumentoGaraEditViewModel viewModel,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Aggiornamento documento: {Id}", id);

            var documento = await _repository.GetByIdAsync(id, cancellationToken);

            if (documento == null)
            {
                _logger.LogWarning("Documento non trovato per aggiornamento: {Id}", id);
                throw new InvalidOperationException($"Documento con ID {id} non trovato");
            }

            documento.UpdateFromEditViewModel(viewModel);

            return await _repository.UpdateAsync(documento, cancellationToken);
        }

        public async Task DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Eliminazione (soft delete) documento: {Id}", id);

            await _repository.DeleteAsync(id, cancellationToken);
        }

        public async Task HardDeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            _logger.LogWarning("Eliminazione fisica (hard delete) documento: {Id}", id);

            var documento = await _repository.GetByIdAsync(id, cancellationToken);

            if (documento == null)
            {
                _logger.LogWarning("Documento non trovato per hard delete: {Id}", id);
                throw new InvalidOperationException($"Documento con ID {id} non trovato");
            }

            try
            {
                // Elimina da MinIO
                await _minioService.DeleteFileAsync(documento.PathMinIO, cancellationToken);

                // Soft delete su DB (per mantenere audit trail)
                await _repository.DeleteAsync(id, cancellationToken);

                _logger.LogInformation(
                    "Documento eliminato fisicamente: {Id}, Path: {Path}",
                    id, documento.PathMinIO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Errore durante l'hard delete del documento: {Id}, Path: {Path}",
                    id, documento.PathMinIO);
                throw;
            }
        }

        public (bool IsValid, string? ErrorMessage) ValidateFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return (false, "Il file è obbligatorio");
            }

            // Validazione dimensione
            if (file.Length > _fileConfig.MaxFileSizeBytes)
            {
                return (false, $"Il file supera la dimensione massima consentita di {_fileConfig.MaxFileSizeMB / 1024 / 1024} MB");
            }

            // Validazione estensione
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_fileConfig.AllowedExtensions.Contains(extension))
            {
                return (false, $"Estensione file non consentita. Estensioni valide: {string.Join(", ", _fileConfig.AllowedExtensions)}");
            }

            // Validazione MIME type
            if (!_fileConfig.AllowedMimeTypes.Contains(file.ContentType))
            {
                _logger.LogWarning(
                    "MIME type non valido: {MimeType} per file {FileName}",
                    file.ContentType, file.FileName);

                // Fallback: accetta se l'estensione è valida
                if (!_fileConfig.AllowedExtensions.Contains(extension))
                {
                    return (false, $"Tipo di file non consentito: {file.ContentType}");
                }
            }

            return (true, null);
        }

        public async Task<DocumentoGaraStatisticsViewModel> GetGaraStatisticsAsync(
            Guid garaId,
            CancellationToken cancellationToken = default)
        {
            var documenti = await _repository.GetByGaraIdAsync(garaId, cancellationToken);
            return BuildStatistics(documenti);
        }

        public async Task<DocumentoGaraStatisticsViewModel> GetLottoStatisticsAsync(
            Guid lottoId,
            CancellationToken cancellationToken = default)
        {
            var documenti = await _repository.GetByLottoIdAsync(lottoId, cancellationToken);
            return BuildStatistics(documenti);
        }

        public async Task<bool> VerifyIntegrityAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var documento = await _repository.GetByIdAsync(id, cancellationToken);

            if (documento == null)
            {
                return false;
            }

            // Verifica che il file esista su MinIO
            var exists = await _minioService.FileExistsAsync(documento.PathMinIO, cancellationToken);

            if (!exists)
            {
                _logger.LogWarning(
                    "Documento inconsistente: esiste su DB ma non su MinIO - {Id}, Path: {Path}",
                    id, documento.PathMinIO);
                return false;
            }

            // Verifica metadata (dimensione)
            var metadata = await _minioService.GetFileMetadataAsync(documento.PathMinIO, cancellationToken);

            if (metadata != null && metadata.Size != documento.DimensioneBytes)
            {
                _logger.LogWarning(
                    "Documento inconsistente: dimensione diversa - DB: {DbSize}, MinIO: {MinIOSize} - {Id}",
                    documento.DimensioneBytes, metadata.Size, id);
                return false;
            }

            return true;
        }

        public async Task<IEnumerable<DocumentoGara>> FindOrphanedDocumentsAsync(
            CancellationToken cancellationToken = default)
        {
            return await _repository.GetOrphanedDocumentsAsync(cancellationToken);
        }

        // ===== PRIVATE HELPER METHODS =====

        private async Task<IEnumerable<DocumentoGara>> ApplyAdvancedFilters(
            DocumentoGaraFilterViewModel filter,
            CancellationToken cancellationToken)
        {
            IEnumerable<DocumentoGara> documenti;

            if (filter.DataCaricamentoDa.HasValue && filter.DataCaricamentoA.HasValue)
            {
                documenti = await _repository.GetByDateRangeAsync(
                    filter.DataCaricamentoDa.Value,
                    filter.DataCaricamentoA.Value,
                    cancellationToken);
            }
            else if (!string.IsNullOrWhiteSpace(filter.CaricatoDaUserId))
            {
                documenti = await _repository.GetByUserIdAsync(
                    filter.CaricatoDaUserId,
                    cancellationToken);
            }
            else
            {
                // Ritorna lista vuota se nessun filtro è specificato
                documenti = Enumerable.Empty<DocumentoGara>();
            }

            return documenti;
        }

        private IEnumerable<DocumentoGara> ApplyAdditionalFilters(
            IEnumerable<DocumentoGara> documenti,
            DocumentoGaraFilterViewModel filter)
        {
            var query = documenti.AsQueryable();

            //if (filter.Tipo.HasValue)
            //{
            //    query = query.Where(d => d.Tipo == filter.Tipo.Value);
            //}
            if (filter.TipoDocumentoId.HasValue)
            {
                query = query.Where(d => d.TipoDocumentoId == filter.TipoDocumentoId.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.NomeFile))
            {
                query = query.Where(d => d.NomeFile.Contains(filter.NomeFile, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(filter.MimeType))
            {
                query = query.Where(d => d.MimeType == filter.MimeType);
            }

            if (filter.DimensioneMinima.HasValue)
            {
                query = query.Where(d => d.DimensioneBytes >= filter.DimensioneMinima.Value);
            }

            if (filter.DimensioneMassima.HasValue)
            {
                query = query.Where(d => d.DimensioneBytes <= filter.DimensioneMassima.Value);
            }

            return query;
        }

        private IEnumerable<DocumentoGara> ApplyOrdering(
            IEnumerable<DocumentoGara> documenti,
            string orderBy,
            bool ascending)
        {
            return orderBy.ToLowerInvariant() switch
            {
                "nomefile" => ascending
                    ? documenti.OrderBy(d => d.NomeFile)
                    : documenti.OrderByDescending(d => d.NomeFile),
                //"tipo" => ascending
                //    ? documenti.OrderBy(d => d.Tipo)
                //    : documenti.OrderByDescending(d => d.Tipo),
                "tipo" => ascending
                    ? documenti.OrderBy(d => d.TipoDocumento?.Nome)
                    : documenti.OrderByDescending(d => d.TipoDocumento?.Nome),
                "dimensione" => ascending
                    ? documenti.OrderBy(d => d.DimensioneBytes)
                    : documenti.OrderByDescending(d => d.DimensioneBytes),
                "datacaricamento" or _ => ascending
                    ? documenti.OrderBy(d => d.DataCaricamento)
                    : documenti.OrderByDescending(d => d.DataCaricamento)
            };
        }

        private DocumentoGaraStatisticsViewModel BuildStatistics(IEnumerable<DocumentoGara> documenti)
        {
            var documentList = documenti.ToList();

            var stats = new DocumentoGaraStatisticsViewModel
            {
                TotaleDocumenti = documentList.Count,
                DimensioneTotaleBytes = documentList.Sum(d => d.DimensioneBytes),
                //DocumentiPerTipo = documentList
                //    .GroupBy(d => d.Tipo)
                //    .ToDictionary(g => g.Key, g => g.Count()),
                DocumentiPerTipo = documentList
                    .Where(d => d.TipoDocumento != null)
                    .GroupBy(d => d.TipoDocumento!.Nome)
                    .ToDictionary(g => g.Key, g => g.Count()),
                DocumentiPerMimeType = documentList
                    .GroupBy(d => d.MimeType)
                    .ToDictionary(g => g.Key, g => g.Count()),
                DimensioneMedia = documentList.Count > 0
                    ? (long)documentList.Average(d => d.DimensioneBytes)
                    : 0,
                DataUltimoCaricamento = documentList
                    .OrderByDescending(d => d.DataCaricamento)
                    .FirstOrDefault()?.DataCaricamento,
                DataPrimoCaricamento = documentList
                    .OrderBy(d => d.DataCaricamento)
                    .FirstOrDefault()?.DataCaricamento,
                TopUtenti = documentList
                    .GroupBy(d => new { d.CaricatoDaUserId, d.CaricatoDa?.UserName })
                    .OrderByDescending(g => g.Count())
                    .Take(5)
                    .Select(g => (
                        g.Key.CaricatoDaUserId,
                        g.Key.UserName ?? "Sconosciuto",
                        g.Count()))
                    .ToList()
            };

            stats.DimensioneTotaleFormatted = FormatFileSize(stats.DimensioneTotaleBytes);

            return stats;
        }

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;

            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            return $"{len:0.##} {sizes[order]}";
        }


        #region Workflow Document Validation

        public async Task<bool> HasDocumentoTipoAsync(
            Guid lottoId,
            TipoDocumentoGara tipo,
            CancellationToken cancellationToken = default)
        {
            var codiceRiferimento = tipo.ToString();

            var documenti = await _repository.GetByLottoIdAsync(lottoId, cancellationToken);

            return documenti.Any(d =>
                d.TipoDocumento != null &&
                d.TipoDocumento.CodiceRiferimento == codiceRiferimento);
        }

        public async Task<bool> HasDocumentoTipoGaraAsync(
            Guid garaId,
            TipoDocumentoGara tipo,
            CancellationToken cancellationToken = default)
        {
            var codiceRiferimento = tipo.ToString();

            var documenti = await _repository.GetByGaraIdAsync(garaId, cancellationToken);

            return documenti.Any(d =>
                d.TipoDocumento != null &&
                d.TipoDocumento.CodiceRiferimento == codiceRiferimento);
        }

        public async Task<Dictionary<TipoDocumentoGara, bool>> CheckDocumentiRequisitiAsync(
            Guid lottoId,
            IEnumerable<TipoDocumentoGara> tipiRichiesti,
            CancellationToken cancellationToken = default)
        {
            var documenti = await _repository.GetByLottoIdAsync(lottoId, cancellationToken);
            var documentList = documenti.ToList();

            // Estrai i codici riferimento presenti nei documenti del lotto
            var codiciPresenti = documentList
                .Where(d => d.TipoDocumento?.CodiceRiferimento != null)
                .Select(d => d.TipoDocumento!.CodiceRiferimento!)
                .Distinct()
                .ToHashSet();

            // Verifica ogni tipo richiesto
            var result = new Dictionary<TipoDocumentoGara, bool>();

            foreach (var tipo in tipiRichiesti)
            {
                var codice = tipo.ToString();
                result[tipo] = codiciPresenti.Contains(codice);
            }

            return result;
        }

        public async Task<Dictionary<string, int>> GetConteggiPerTipoAsync(
            Guid lottoId,
            CancellationToken cancellationToken = default)
        {
            var documenti = await _repository.GetByLottoIdAsync(lottoId, cancellationToken);

            return documenti
                .Where(d => d.TipoDocumento?.CodiceRiferimento != null)
                .GroupBy(d => d.TipoDocumento!.CodiceRiferimento!)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public async Task<(bool IsValid, List<string> DocumentiMancanti)> ValidaRequisitiDocumentaliFaseAsync(
            Guid lottoId,
            string fase,
            CancellationToken cancellationToken = default)
        {
            // Definisci i requisiti per ogni fase
            var requisitiPerFase = GetRequisitiDocumentaliPerFase(fase);

            if (!requisitiPerFase.Any())
            {
                // Nessun requisito per questa fase
                return (true, new List<string>());
            }

            // Verifica i requisiti
            var checkResult = await CheckDocumentiRequisitiAsync(lottoId, requisitiPerFase, cancellationToken);

            var mancanti = checkResult
                .Where(kv => !kv.Value)
                .Select(kv => GetDisplayNameTipoDocumento(kv.Key))
                .ToList();

            return (!mancanti.Any(), mancanti);
        }

        /// <summary>
        /// Definisce i documenti richiesti per ogni fase del workflow.
        /// Personalizza in base alle tue esigenze di business.
        /// </summary>
        private static IEnumerable<TipoDocumentoGara> GetRequisitiDocumentaliPerFase(string fase)
        {
            return fase.ToLowerInvariant() switch
            {
                "valutazionetecnica" => new[]
                {
                    TipoDocumentoGara.DocumentoValutazioneTecnica
                },

                "valutazioneeconomica" => new[]
                {
                    TipoDocumentoGara.DocumentoValutazioneEconomica,
                    TipoDocumentoGara.Preventivo
                },

                "elaborazione" => new[]
                {
                    TipoDocumentoGara.DocumentoValutazioneTecnica,
                    TipoDocumentoGara.DocumentoValutazioneEconomica
                },

                "presentazione" => new[]
                {
                    TipoDocumentoGara.OffertaTecnica,
                    TipoDocumentoGara.OffertaEconomica,
                    TipoDocumentoGara.DocumentoPresentazione
                },

                "aggiudicazione" => new[]
                {
                    TipoDocumentoGara.OffertaTecnica,
                    TipoDocumentoGara.OffertaEconomica,
                    TipoDocumentoGara.Contratto
                },

                _ => Enumerable.Empty<TipoDocumentoGara>()
            };
        }

        /// <summary>
        /// Ottiene il display name dell'enum per messaggi user-friendly.
        /// </summary>
        private static string GetDisplayNameTipoDocumento(TipoDocumentoGara tipo)
        {
            // Usa reflection per ottenere il Display attribute
            var memberInfo = typeof(TipoDocumentoGara).GetMember(tipo.ToString()).FirstOrDefault();

            if (memberInfo != null)
            {
                var displayAttribute = memberInfo
                    .GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.DisplayAttribute), false)
                    .FirstOrDefault() as System.ComponentModel.DataAnnotations.DisplayAttribute;

                if (displayAttribute != null)
                {
                    return displayAttribute.Name ?? tipo.ToString();
                }
            }

            return tipo.ToString();
        }

        #endregion
    }
}