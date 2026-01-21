using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Trasformazioni.Data.Repositories;
using Trasformazioni.Mappings;
using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;
using Trasformazioni.Models.ViewModels;
using Trasformazioni.Services.Interfaces;

namespace Trasformazioni.Services
{
    /// <summary>
    /// Implementazione della business logic per la gestione allegati del Registro Contratti
    /// </summary>
    public class AllegatoRegistroService : IAllegatoRegistroService
    {
        private readonly ITipoDocumentoService _tipoDocumentoService;
        private readonly IAllegatoRegistroRepository _allegatoRepository;
        private readonly IRegistroContrattiRepository _registroRepository;
        private readonly IMinIOService _minIOService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AllegatoRegistroService> _logger;

        // Configurazione limiti file (da appsettings)
        private readonly long _maxFileSize;
        private readonly string[] _allowedExtensions;
        private readonly string[] _allowedMimeTypes;
        private readonly int _maxFilesPerUpload;
        private readonly long _maxTotalSize;

        public AllegatoRegistroService(
            IAllegatoRegistroRepository allegatoRepository,
            IRegistroContrattiRepository registroRepository,
            IMinIOService minIOService,
            IConfiguration configuration,
            ILogger<AllegatoRegistroService> logger,
            ITipoDocumentoService tipoDocumentoService)
        {
            _allegatoRepository = allegatoRepository;
            _registroRepository = registroRepository;
            _minIOService = minIOService;
            _configuration = configuration;
            _logger = logger;

            // Carica configurazione da appsettings
            var uploadConfig = _configuration.GetSection("FileUpload");
            _maxFileSize = uploadConfig.GetValue<long>("MaxFileSize", 50 * 1024 * 1024); // Default 50 MB
            _allowedExtensions = uploadConfig.GetSection("AllowedExtensions").Get<string[]>()
                ?? new[] { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".jpg", ".png" };
            _allowedMimeTypes = uploadConfig.GetSection("AllowedMimeTypes").Get<string[]>()
                ?? new[] { "application/pdf", "application/msword", "image/jpeg", "image/png" };
            _maxFilesPerUpload = uploadConfig.GetValue<int>("MaxFilesPerUpload", 10);
            _maxTotalSize = uploadConfig.GetValue<long>("MaxTotalSize", 200 * 1024 * 1024); // Default 200 MB
            _tipoDocumentoService = tipoDocumentoService;
        }

        // ===================================
        // QUERY - LETTURA
        // ===================================

        /// <summary>
        /// Ottiene tutti gli allegati di un registro
        /// </summary>
        public async Task<IEnumerable<AllegatoRegistroListViewModel>> GetByRegistroIdAsync(Guid registroContrattiId)
        {
            try
            {
                var allegati = await _allegatoRepository.GetByRegistroIdAsync(registroContrattiId);
                return allegati.ToListViewModels();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero degli allegati per registro {RegistroId}", registroContrattiId);
                throw;
            }
        }

        /// <summary>
        /// Ottiene il dettaglio di un allegato
        /// </summary>
        public async Task<AllegatoRegistroDetailsViewModel?> GetByIdAsync(Guid id)
        {
            try
            {
                var allegato = await _allegatoRepository.GetByIdAsync(id);
                return allegato?.ToDetailsViewModel();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dell'allegato {AllegatoId}", id);
                throw;
            }
        }

        /// <summary>
        /// Conta gli allegati di un registro
        /// </summary>
        public async Task<int> CountByRegistroIdAsync(Guid registroContrattiId)
        {
            return await _allegatoRepository.CountByRegistroIdAsync(registroContrattiId);
        }

        // ===================================
        // UPLOAD
        // ===================================

        /// <summary>
        /// Carica un nuovo allegato
        /// </summary>
        public async Task<(bool Success, string? ErrorMessage, Guid? AllegatoId)> UploadAsync(
            AllegatoRegistroUploadViewModel model,
            string currentUserId)
        {
            try
            {
                // 1. Verifica esistenza registro
                if (!await _registroRepository.ExistsAsync(model.RegistroContrattiId))
                {
                    return (false, "Registro non trovato", null);
                }

                // 2. Validazione file
                if (model.File == null || model.File.Length == 0)
                {
                    return (false, "Nessun file selezionato", null);
                }

                var (isValid, validationError) = ValidateFile(model.File);
                if (!isValid)
                {
                    return (false, validationError, null);
                }

                // 3. Verifica nome file duplicato
                if (await ExistsByNomeFileAsync(model.RegistroContrattiId, model.File.FileName))
                {
                    return (false, "Esiste già un file con questo nome", null);
                }

                // 4. Genera ID e path
                var allegatoId = Guid.NewGuid();
                var pathMinIO = AllegatoRegistroMappingExtensions.GeneraPathMinIO(
                    model.RegistroContrattiId,
                    allegatoId,
                    model.File.FileName);

                // 5. Crea record allegato
                var allegato = new AllegatoRegistro
                {
                    Id = allegatoId,
                    RegistroContrattiId = model.RegistroContrattiId,
                    TipoDocumentoId = model.TipoDocumentoId,
                    Descrizione = model.Descrizione?.Trim(),
                    NomeFile = model.File.FileName,
                    PathMinIO = pathMinIO,
                    DimensioneBytes = model.File.Length,
                    MimeType = model.File.ContentType ?? AllegatoRegistroMappingExtensions.GetMimeType(model.File.FileName),
                    IsUploadCompleto = false,
                    DataCaricamento = DateTime.Now,
                    CaricatoDaUserId = currentUserId
                };

                await _allegatoRepository.AddAsync(allegato);
                await _allegatoRepository.SaveChangesAsync();

                // 6. Upload su MinIO
                try
                {
                    using var stream = model.File.OpenReadStream();
                    await _minIOService.UploadFileAsync(
                        stream,
                        pathMinIO,
                        model.File.ContentType ?? "application/octet-stream");

                    // 7. Marca upload come completato
                    allegato.MarcaUploadCompletato();
                    await _allegatoRepository.UpdateAsync(allegato);
                    await _allegatoRepository.SaveChangesAsync();

                    _logger.LogInformation(
                        "Allegato caricato con successo - ID: {AllegatoId}, File: {FileName}, Registro: {RegistroId}",
                        allegatoId, model.File.FileName, model.RegistroContrattiId);

                    return (true, null, allegatoId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Errore durante l'upload su MinIO per allegato {AllegatoId}", allegatoId);

                    // Rollback: elimina record
                    await _allegatoRepository.DeleteAsync(allegato);
                    await _allegatoRepository.SaveChangesAsync();

                    return (false, "Errore durante il caricamento del file", null);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'upload dell'allegato");
                return (false, "Errore durante il caricamento", null);
            }
        }

        /// <summary>
        /// Carica più allegati contemporaneamente
        /// </summary>
        public async Task<(bool Success, string? ErrorMessage, List<Guid>? AllegatiIds)> UploadMultiploAsync(
            AllegatoRegistroUploadMultiploViewModel model,
            string currentUserId)
        {
            try
            {
                // 1. Verifica esistenza registro
                if (!await _registroRepository.ExistsAsync(model.RegistroContrattiId))
                {
                    return (false, "Registro non trovato", null);
                }

                // 2. Validazione files
                if (model.Files == null || !model.Files.Any())
                {
                    return (false, "Nessun file selezionato", null);
                }

                if (model.Files.Count > _maxFilesPerUpload)
                {
                    return (false, $"È possibile caricare massimo {_maxFilesPerUpload} file alla volta", null);
                }

                // Validazione dimensione totale
                var totalSize = model.Files.Sum(f => f.Length);
                if (totalSize > _maxTotalSize)
                {
                    return (false, $"Dimensione totale superiore al limite consentito", null);
                }

                // Validazione singoli file
                foreach (var file in model.Files)
                {
                    var (isValid, validationError) = ValidateFile(file);
                    if (!isValid)
                    {
                        return (false, $"File '{file.FileName}': {validationError}", null);
                    }
                }

                // 3. Upload di ogni file
                var allegatiIds = new List<Guid>();
                var errors = new List<string>();

                foreach (var file in model.Files)
                {
                    var singleModel = new AllegatoRegistroUploadViewModel
                    {
                        RegistroContrattiId = model.RegistroContrattiId,
                        TipoDocumentoId = model.TipoDocumentoId,
                        File = file
                    };

                    var (success, error, allegatoId) = await UploadAsync(singleModel, currentUserId);

                    if (success && allegatoId.HasValue)
                    {
                        allegatiIds.Add(allegatoId.Value);
                    }
                    else
                    {
                        errors.Add($"{file.FileName}: {error}");
                    }
                }

                if (allegatiIds.Count == 0)
                {
                    return (false, string.Join("; ", errors), null);
                }

                if (errors.Any())
                {
                    _logger.LogWarning(
                        "Upload multiplo parziale - Caricati: {SuccessCount}, Errori: {ErrorCount}",
                        allegatiIds.Count, errors.Count);

                    return (true, $"Caricati {allegatiIds.Count} file. Errori: {string.Join("; ", errors)}", allegatiIds);
                }

                _logger.LogInformation(
                    "Upload multiplo completato - Registro: {RegistroId}, Files: {Count}",
                    model.RegistroContrattiId, allegatiIds.Count);

                return (true, null, allegatiIds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'upload multiplo");
                return (false, "Errore durante il caricamento", null);
            }
        }

        // ===================================
        // DOWNLOAD
        // ===================================

        /// <summary>
        /// Ottiene lo stream del file per il download
        /// </summary>
        public async Task<(bool Success, string? ErrorMessage, Stream? FileStream, string? FileName, string? MimeType)> DownloadAsync(Guid id)
        {
            try
            {
                var allegato = await _allegatoRepository.GetByIdAsync(id);
                if (allegato == null)
                {
                    return (false, "Allegato non trovato", null, null, null);
                }

                if (!allegato.IsUploadCompleto)
                {
                    return (false, "Upload non completato", null, null, null);
                }

                var stream = await _minIOService.DownloadFileAsync(allegato.PathMinIO);
                if (stream == null)
                {
                    return (false, "File non trovato nello storage", null, null, null);
                }

                return (true, null, stream, allegato.NomeFile, allegato.MimeType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il download dell'allegato {AllegatoId}", id);
                return (false, "Errore durante il download", null, null, null);
            }
        }

        ///// <summary>
        ///// Ottiene l'URL temporaneo per il download diretto
        ///// </summary>
        //public async Task<(bool Success, string? ErrorMessage, string? Url)> GetDownloadUrlAsync(Guid id, int expiryMinutes = 60)
        //{
        //    try
        //    {
        //        var allegato = await _allegatoRepository.GetByIdAsync(id);
        //        if (allegato == null)
        //        {
        //            return (false, "Allegato non trovato", null);
        //        }

        //        if (!allegato.IsUploadCompleto)
        //        {
        //            return (false, "Upload non completato", null);
        //        }

        //        var url = await _minIOService.GetPresignedUrlAsync(allegato.PathMinIO, expiryMinutes);
        //        if (string.IsNullOrEmpty(url))
        //        {
        //            return (false, "Impossibile generare URL di download", null);
        //        }

        //        return (true, null, url);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Errore durante la generazione URL per allegato {AllegatoId}", id);
        //        return (false, "Errore durante la generazione dell'URL", null);
        //    }
        //}

        // ===================================
        // MODIFICA
        // ===================================

        /// <summary>
        /// Aggiorna la descrizione di un allegato
        /// </summary>
        public async Task<(bool Success, string? ErrorMessage)> UpdateDescrizioneAsync(
            Guid id,
            string? descrizione,
            string currentUserId)
        {
            try
            {
                var allegato = await _allegatoRepository.GetByIdAsync(id);
                if (allegato == null)
                {
                    return (false, "Allegato non trovato");
                }

                allegato.UpdateDescrizione(descrizione);
                await _allegatoRepository.UpdateAsync(allegato);
                await _allegatoRepository.SaveChangesAsync();

                _logger.LogInformation("Aggiornata descrizione allegato {AllegatoId}", id);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'aggiornamento della descrizione allegato {AllegatoId}", id);
                return (false, "Errore durante l'aggiornamento");
            }
        }

        /// <summary>
        /// Aggiorna il tipo documento di un allegato
        /// </summary>
        public async Task<(bool Success, string? ErrorMessage)> UpdateTipoDocumentoAsync(
            Guid id,
            Guid tipoDocumentoId,
            string currentUserId)
        {
            try
            {
                var allegato = await _allegatoRepository.GetByIdAsync(id);
                if (allegato == null)
                {
                    return (false, "Allegato non trovato");
                }

                allegato.UpdateTipoDocumento(tipoDocumentoId);
                await _allegatoRepository.UpdateAsync(allegato);
                await _allegatoRepository.SaveChangesAsync();

                _logger.LogInformation("Aggiornato tipo documento allegato {AllegatoId}", id);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'aggiornamento tipo documento allegato {AllegatoId}", id);
                return (false, "Errore durante l'aggiornamento");
            }
        }

        // ===================================
        // ELIMINAZIONE
        // ===================================

        /// <summary>
        /// Elimina un allegato (soft delete + rimozione da MinIO)
        /// </summary>
        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(Guid id, string currentUserId)
        {
            try
            {
                var allegato = await _allegatoRepository.GetByIdAsync(id);
                if (allegato == null)
                {
                    return (false, "Allegato non trovato");
                }

                // Elimina da MinIO
                if (allegato.IsUploadCompleto)
                {
                    try
                    {
                        await _minIOService.DeleteFileAsync(allegato.PathMinIO);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Errore durante l'eliminazione da MinIO per allegato {AllegatoId}", id);
                        // Continua comunque con soft delete
                    }
                }

                // Soft delete
                await _allegatoRepository.DeleteAsync(allegato);
                await _allegatoRepository.SaveChangesAsync();

                _logger.LogInformation("Allegato eliminato - ID: {AllegatoId}", id);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'eliminazione dell'allegato {AllegatoId}", id);
                return (false, "Errore durante l'eliminazione");
            }
        }

        /// <summary>
        /// Elimina tutti gli allegati di un registro
        /// </summary>
        public async Task<(bool Success, string? ErrorMessage, int DeletedCount)> DeleteByRegistroIdAsync(
            Guid registroContrattiId,
            string currentUserId)
        {
            try
            {
                var allegati = await _allegatoRepository.GetByRegistroIdAsync(registroContrattiId);
                var count = 0;

                foreach (var allegato in allegati)
                {
                    var (success, _) = await DeleteAsync(allegato.Id, currentUserId);
                    if (success) count++;
                }

                _logger.LogInformation(
                    "Eliminati {Count} allegati per registro {RegistroId}",
                    count, registroContrattiId);

                return (true, null, count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'eliminazione allegati per registro {RegistroId}", registroContrattiId);
                return (false, "Errore durante l'eliminazione", 0);
            }
        }

        // ===================================
        // VALIDAZIONI
        // ===================================

        /// <summary>
        /// Valida un file per l'upload
        /// </summary>
        public (bool IsValid, string? ErrorMessage) ValidateFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return (false, "File vuoto o non valido");
            }

            // Validazione dimensione
            if (file.Length > _maxFileSize)
            {
                var maxSizeMB = _maxFileSize / (1024 * 1024);
                return (false, $"Dimensione file superiore a {maxSizeMB} MB");
            }

            // Validazione estensione
            var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
            if (string.IsNullOrEmpty(extension))
            {
                return (false, "Estensione file non riconosciuta");
            }

            if (!_allowedExtensions.Contains(extension))
            {
                return (false, $"Estensione '{extension}' non consentita");
            }

            // Validazione MIME type
            var mimeType = file.ContentType?.ToLowerInvariant();
            if (!string.IsNullOrEmpty(mimeType) && _allowedMimeTypes.Length > 0)
            {
                if (!_allowedMimeTypes.Contains(mimeType))
                {
                    return (false, $"Tipo file '{mimeType}' non consentito");
                }
            }

            // Validazione nome file
            var fileName = file.FileName;
            if (fileName.Length > 255)
            {
                return (false, "Nome file troppo lungo (max 255 caratteri)");
            }

            var invalidChars = Path.GetInvalidFileNameChars();
            if (fileName.Any(c => invalidChars.Contains(c)))
            {
                return (false, "Nome file contiene caratteri non validi");
            }

            return (true, null);
        }

        /// <summary>
        /// Verifica se un allegato esiste
        /// </summary>
        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _allegatoRepository.ExistsAsync(id);
        }

        /// <summary>
        /// Verifica se esiste già un file con lo stesso nome nel registro
        /// </summary>
        public async Task<bool> ExistsByNomeFileAsync(Guid registroContrattiId, string nomeFile, Guid? excludeId = null)
        {
            return await _allegatoRepository.ExistsByNomeFileInRegistroAsync(registroContrattiId, nomeFile, excludeId);
        }

        // ===================================
        // CLEANUP
        // ===================================

        /// <summary>
        /// Rimuove allegati con upload incompleto più vecchi di X ore
        /// </summary>
        public async Task<int> CleanupUploadIncompletiAsync(int oreVecchiaia = 24)
        {
            try
            {
                var dataLimite = DateTime.Now.AddHours(-oreVecchiaia);
                var uploadIncompleti = await _allegatoRepository.GetUploadIncompletiOlderThanAsync(dataLimite);
                var count = 0;

                foreach (var allegato in uploadIncompleti)
                {
                    try
                    {
                        // Tenta eliminazione da MinIO (potrebbe non esistere)
                        try
                        {
                            await _minIOService.DeleteFileAsync(allegato.PathMinIO);
                        }
                        catch
                        {
                            // Ignora errori MinIO per upload incompleti
                        }

                        // Hard delete per upload incompleti
                        await _allegatoRepository.DeleteAsync(allegato);
                        count++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Errore durante cleanup allegato {AllegatoId}", allegato.Id);
                    }
                }

                if (count > 0)
                {
                    await _allegatoRepository.SaveChangesAsync();
                    _logger.LogInformation("Cleanup: rimossi {Count} upload incompleti", count);
                }

                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante cleanup upload incompleti");
                throw;
            }
        }

        // ===================================
        // STATISTICHE
        // ===================================

        /// <summary>
        /// Ottiene la dimensione totale degli allegati di un registro
        /// </summary>
        public async Task<long> GetTotalSizeByRegistroIdAsync(Guid registroContrattiId)
        {
            return await _allegatoRepository.GetTotalSizeByRegistroIdAsync(registroContrattiId);
        }

        /// <summary>
        /// Ottiene statistiche sugli allegati
        /// </summary>
        public async Task<AllegatoRegistroStatisticheViewModel> GetStatisticheAsync()
        {
            try
            {
                var totale = await _allegatoRepository.CountAsync();
                var dimensione = await _allegatoRepository.GetTotalSizeAsync();
                var incompleti = (await _allegatoRepository.GetUploadIncompleti()).Count();

                // Distribuzione per tipo (semplificata)
                var allegati = await _allegatoRepository.GetAllAsync();
                var distribuzione = allegati
                    .GroupBy(a => GetTipoCategoria(a.MimeType))
                    .ToDictionary(g => g.Key, g => g.Count());

                return new AllegatoRegistroStatisticheViewModel
                {
                    TotaleAllegati = totale,
                    DimensioneTotaleBytes = dimensione,
                    UploadIncompleti = incompleti,
                    DistribuzionePerTipo = distribuzione
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero delle statistiche allegati");
                throw;
            }
        }

        // ===================================
        // PREPARAZIONE VIEWMODEL
        // ===================================

        /// <summary>
        /// Prepara il ViewModel per l'upload
        /// </summary>
        public async Task<AllegatoRegistroUploadViewModel?> PrepareUploadViewModelAsync(Guid registroContrattiId)
        {
            try
            {
                var registro = await _registroRepository.GetByIdAsync(registroContrattiId);
                if (registro == null)
                    return null;

                var viewModel = new AllegatoRegistroUploadViewModel
                {
                    RegistroContrattiId = registroContrattiId,
                    RegistroNumeroProtocollo = registro.NumeroProtocollo,
                    RegistroOggetto = registro.Oggetto,
                    RegistroRagioneSociale = registro.RagioneSociale,
                    MaxFileSize = _maxFileSize,
                    AllowedExtensions = _allowedExtensions,
                    AllowedMimeTypes = _allowedMimeTypes,
                    AllegatiEsistenti = (await GetByRegistroIdAsync(registroContrattiId)).OrderByDescending(a => a.DataCaricamento)
                };

                await PopolaTipiDocumentoSelectListAsync(viewModel);

                return viewModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la preparazione ViewModel upload per registro {RegistroId}", registroContrattiId);
                throw;
            }
        }

        /// <summary>
        /// Prepara il ViewModel per l'upload multiplo
        /// </summary>
        public async Task<AllegatoRegistroUploadMultiploViewModel?> PrepareUploadMultiploViewModelAsync(Guid registroContrattiId)
        {
            try
            {
                var registro = await _registroRepository.GetByIdAsync(registroContrattiId);
                if (registro == null)
                    return null;

                var viewModel = new AllegatoRegistroUploadMultiploViewModel
                {
                    RegistroContrattiId = registroContrattiId,
                    RegistroNumeroProtocollo = registro.NumeroProtocollo,
                    RegistroOggetto = registro.Oggetto,
                    MaxFiles = _maxFilesPerUpload,
                    MaxFileSize = _maxFileSize,
                    MaxTotalSize = _maxTotalSize,
                    AllowedExtensions = _allowedExtensions,
                    AllowedMimeTypes = _allowedMimeTypes,
                    AllegatiEsistenti = (await GetByRegistroIdAsync(registroContrattiId)).OrderByDescending(a => a.DataCaricamento)
                };

                await PopolaTipiDocumentoSelectListAsync(viewModel);

                return viewModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la preparazione ViewModel upload multiplo per registro {RegistroId}", registroContrattiId);
                throw;
            }
        }

        // ===================================
        // METODI PRIVATI
        // ===================================

        //private async Task PopolaTipiDocumentoSelectListAsync(AllegatoRegistroUploadViewModel viewModel)
        //{
        //    // TODO: Recuperare tipi documento per area RegistroContratti
        //    // Per ora placeholder - da implementare con repository TipoDocumento
        //    viewModel.TipiDocumentoSelectList = new SelectList(
        //        new[]
        //        {
        //            new { Id = Guid.NewGuid(), Nome = "Contratto Firmato" },
        //            new { Id = Guid.NewGuid(), Nome = "Allegato Tecnico" },
        //            new { Id = Guid.NewGuid(), Nome = "Preventivo" },
        //            new { Id = Guid.NewGuid(), Nome = "Altro" }
        //        },
        //        "Id", "Nome", viewModel.TipoDocumentoId);
        //}
        /// <summary>
        /// Popola la SelectList dei tipi documento per l'area RegistroContratti
        /// </summary>
        private async Task PopolaTipiDocumentoSelectListAsync(AllegatoRegistroUploadViewModel viewModel)
        {
            var tipiDocumento = await _tipoDocumentoService.GetForDropdownAsync(AreaDocumento.RegistroContratti);

            viewModel.TipiDocumentoSelectList = new SelectList(
                tipiDocumento,
                nameof(TipoDocumentoDropdownViewModel.Id),
                nameof(TipoDocumentoDropdownViewModel.DisplayText),
                viewModel.TipoDocumentoId != Guid.Empty ? viewModel.TipoDocumentoId : null
            );
        }

        //private async Task PopolaTipiDocumentoSelectListMultiploAsync(AllegatoRegistroUploadMultiploViewModel viewModel)
        //{
        //    // TODO: Recuperare tipi documento per area RegistroContratti
        //    viewModel.TipiDocumentoSelectList = new SelectList(
        //        new[]
        //        {
        //            new { Id = Guid.NewGuid(), Nome = "Contratto Firmato" },
        //            new { Id = Guid.NewGuid(), Nome = "Allegato Tecnico" },
        //            new { Id = Guid.NewGuid(), Nome = "Preventivo" },
        //            new { Id = Guid.NewGuid(), Nome = "Altro" }
        //        },
        //        "Id", "Nome", viewModel.TipoDocumentoId);
        //}
        /// <summary>
        /// Popola la SelectList dei tipi documento per upload multiplo
        /// </summary>
        private async Task PopolaTipiDocumentoSelectListAsync(AllegatoRegistroUploadMultiploViewModel viewModel)
        {
            var tipiDocumento = await _tipoDocumentoService.GetForDropdownAsync(AreaDocumento.RegistroContratti);

            viewModel.TipiDocumentoSelectList = new SelectList(
                tipiDocumento,
                nameof(TipoDocumentoDropdownViewModel.Id),
                nameof(TipoDocumentoDropdownViewModel.DisplayText),
                viewModel.TipoDocumentoId != Guid.Empty ? viewModel.TipoDocumentoId : null
            );
        }

        private string GetTipoCategoria(string mimeType)
        {
            return mimeType.ToLowerInvariant() switch
            {
                "application/pdf" => "PDF",
                var m when m.StartsWith("image/") => "Immagini",
                var m when m.Contains("word") || m.Contains("document") => "Documenti Word",
                var m when m.Contains("excel") || m.Contains("spreadsheet") => "Fogli di calcolo",
                var m when m.Contains("powerpoint") || m.Contains("presentation") => "Presentazioni",
                var m when m.StartsWith("text/") => "Testo",
                var m when m.Contains("zip") || m.Contains("rar") || m.Contains("7z") => "Archivi",
                _ => "Altri"
            };
        }
    }
}