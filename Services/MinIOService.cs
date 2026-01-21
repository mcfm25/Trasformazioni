using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using Trasformazioni.Configuration;
using Trasformazioni.Services.Interfaces;

namespace Trasformazioni.Services
{
    /// <summary>
    /// Implementazione del servizio MinIO per la gestione dello storage
    /// Utilizza il pattern repository per astrarre le operazioni su MinIO
    /// </summary>
    public class MinIOService : IMinIOService
    {
        private readonly IMinioClient _minioClient;
        private readonly MinIOConfiguration _config;
        private readonly ILogger<MinIOService> _logger;

        public MinIOService(
            IOptions<MinIOConfiguration> config,
            ILogger<MinIOService> logger)
        {
            _config = config.Value;
            _logger = logger;

            // Inizializza il client MinIO
            _minioClient = new MinioClient()
                .WithEndpoint(_config.Endpoint)
                .WithCredentials(_config.AccessKey, _config.SecretKey)
                .WithSSL(_config.UseSSL)
                //.WithTimeout(60000) // 1 minuto (60000 ms)                
                .Build();

            // Verifica che il bucket esista all'avvio
            EnsureBucketExistsAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Verifica che il bucket di default esista, altrimenti lo crea
        /// </summary>
        private async Task EnsureBucketExistsAsync()
        {
            try
            {
                var bucketExists = await _minioClient.BucketExistsAsync(
                    new BucketExistsArgs()
                        .WithBucket(_config.DefaultBucket));

                if (!bucketExists)
                {
                    await _minioClient.MakeBucketAsync(
                        new MakeBucketArgs()
                            .WithBucket(_config.DefaultBucket));

                    _logger.LogInformation(
                        "Bucket MinIO '{Bucket}' creato con successo",
                        _config.DefaultBucket);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Errore durante la verifica/creazione del bucket MinIO '{Bucket}'",
                    _config.DefaultBucket);
                throw;
            }
        }

        public async Task<string> UploadFileAsync(
            Stream stream,
            string objectName,
            string contentType,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation(
                    "Caricamento file su MinIO: {ObjectName}, ContentType: {ContentType}, Size: {Size} bytes",
                    objectName, contentType, stream.Length);

                await _minioClient.PutObjectAsync(
                    new PutObjectArgs()
                        .WithBucket(_config.DefaultBucket)
                        .WithObject(objectName)
                        .WithStreamData(stream)
                        .WithObjectSize(stream.Length)
                        .WithContentType(contentType),
                    cancellationToken);

                _logger.LogInformation(
                    "File caricato con successo su MinIO: {ObjectName}",
                    objectName);

                return objectName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Errore durante il caricamento del file su MinIO: {ObjectName}",
                    objectName);
                throw;
            }
        }

        public async Task<Stream> DownloadFileAsync(
            string objectName,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation(
                    "Download file da MinIO: {ObjectName}",
                    objectName);

                var memoryStream = new MemoryStream();

                await _minioClient.GetObjectAsync(
                    new GetObjectArgs()
                        .WithBucket(_config.DefaultBucket)
                        .WithObject(objectName)
                        .WithCallbackStream(stream => stream.CopyTo(memoryStream)), cancellationToken);

                memoryStream.Position = 0;

                _logger.LogInformation(
                    "File scaricato con successo da MinIO: {ObjectName}, Size: {Size} bytes",
                    objectName, memoryStream.Length);

                return memoryStream;
            }
            catch (ObjectNotFoundException)
            {
                _logger.LogWarning(
                    "File non trovato su MinIO: {ObjectName}",
                    objectName);
                throw new FileNotFoundException(
                    $"File non trovato su MinIO: {objectName}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Errore durante il download del file da MinIO: {ObjectName}",
                    objectName);
                throw;
            }
        }

        public async Task DeleteFileAsync(
            string objectName,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation(
                    "Eliminazione file da MinIO: {ObjectName}",
                    objectName);

                await _minioClient.RemoveObjectAsync(
                    new RemoveObjectArgs()
                        .WithBucket(_config.DefaultBucket)
                        .WithObject(objectName),
                    cancellationToken);

                _logger.LogInformation(
                    "File eliminato con successo da MinIO: {ObjectName}",
                    objectName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Errore durante l'eliminazione del file da MinIO: {ObjectName}",
                    objectName);
                throw;
            }
        }

        public async Task<bool> FileExistsAsync(
            string objectName,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _minioClient.StatObjectAsync(
                    new StatObjectArgs()
                        .WithBucket(_config.DefaultBucket)
                        .WithObject(objectName),
                    cancellationToken);

                return true;
            }
            catch (ObjectNotFoundException)
            {
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Errore durante la verifica dell'esistenza del file su MinIO: {ObjectName}",
                    objectName);
                throw;
            }
        }

        public async Task<FileMetadata?> GetFileMetadataAsync(
            string objectName,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var stat = await _minioClient.StatObjectAsync(
                    new StatObjectArgs()
                        .WithBucket(_config.DefaultBucket)
                        .WithObject(objectName),
                    cancellationToken);

                return new FileMetadata
                {
                    ObjectName = stat.ObjectName,
                    Size = stat.Size,
                    ContentType = stat.ContentType,
                    LastModified = stat.LastModified,
                    ETag = stat.ETag
                };
            }
            catch (ObjectNotFoundException)
            {
                _logger.LogWarning(
                    "File non trovato su MinIO durante il recupero dei metadata: {ObjectName}",
                    objectName);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Errore durante il recupero dei metadata del file da MinIO: {ObjectName}",
                    objectName);
                throw;
            }
        }

        public async Task<IEnumerable<string>> ListFilesAsync(
            string prefix,
            bool recursive = false,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation(
                    "Listing file da MinIO con prefix: {Prefix}, Recursive: {Recursive}",
                    prefix, recursive);

                var files = new List<string>();

                var listArgs = new ListObjectsArgs()
                    .WithBucket(_config.DefaultBucket)
                    .WithPrefix(prefix)
                    .WithRecursive(recursive);
                                
                // Usa ListObjectsEnumAsync che restituisce IAsyncEnumerable
                await foreach (var item in _minioClient.ListObjectsEnumAsync(listArgs).WithCancellation(cancellationToken))
                {
                    if (!item.IsDir) // Ignora le "directory" virtuali
                    {
                        files.Add(item.Key);
                    }
                }

                _logger.LogInformation(
                    "Trovati {Count} file con prefix: {Prefix}",
                    files.Count, prefix);

                return files;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Errore durante il listing dei file da MinIO con prefix: {Prefix}",
                    prefix);
                throw;
            }
        }

        public string GenerateObjectPath(
            Guid garaId,
            Guid? lottoId = null,
            Guid? preventivoId = null,
            Guid? integrazioneId = null,
            string fileName = "")
        {
            // Genera un GUID univoco per il file
            var fileGuid = Guid.NewGuid();

            // Combina GUID con nome originale (se fornito)
            var objectFileName = string.IsNullOrWhiteSpace(fileName)
                ? fileGuid.ToString()
                : $"{fileGuid}_{fileName}";

            // Costruisce il path secondo la struttura definita
            if (integrazioneId.HasValue && lottoId.HasValue)
            {
                // Documento di integrazione
                return $"gare/{garaId}/lotti/{lottoId.Value}/integrazioni/{integrazioneId.Value}/{objectFileName}";
            }
            else if (preventivoId.HasValue && lottoId.HasValue)
            {
                // Documento di preventivo
                return $"gare/{garaId}/lotti/{lottoId.Value}/preventivi/{preventivoId.Value}/{objectFileName}";
            }
            else if (lottoId.HasValue)
            {
                // Documento di lotto
                return $"gare/{garaId}/lotti/{lottoId.Value}/{objectFileName}";
            }
            else
            {
                // Documento di gara
                return $"gare/{garaId}/{objectFileName}";
            }
        }

        public async Task<string> GetPresignedUrlAsync(
            string objectName,
            int expiryMinutes = 60,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation(
                    "Generazione URL presigned per: {ObjectName}, Scadenza: {Minutes} minuti",
                    objectName, expiryMinutes);

                var url = await _minioClient.PresignedGetObjectAsync(
                    new PresignedGetObjectArgs()
                        .WithBucket(_config.DefaultBucket)
                        .WithObject(objectName)
                        .WithExpiry(expiryMinutes * 60)); // in secondi

                _logger.LogInformation(
                    "URL presigned generato per: {ObjectName}",
                    objectName);

                return url;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Errore durante la generazione URL presigned per: {ObjectName}",
                    objectName);
                throw;
            }
        }
    }
}