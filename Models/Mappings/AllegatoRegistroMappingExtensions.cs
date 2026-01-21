using Trasformazioni.Models.Entities;
using Trasformazioni.Models.ViewModels;

namespace Trasformazioni.Mappings
{
    /// <summary>
    /// Extension methods per il mapping tra entità AllegatoRegistro e ViewModels
    /// </summary>
    public static class AllegatoRegistroMappingExtensions
    {
        // ===================================
        // ENTITY → VIEWMODEL
        // ===================================

        /// <summary>
        /// Mappa un'entità AllegatoRegistro a AllegatoRegistroListViewModel
        /// </summary>
        public static AllegatoRegistroListViewModel ToListViewModel(this AllegatoRegistro allegato)
        {
            return new AllegatoRegistroListViewModel
            {
                Id = allegato.Id,

                // Riferimento Registro
                RegistroContrattiId = allegato.RegistroContrattiId,
                RegistroOggetto = allegato.RegistroContratti?.Oggetto,

                // Tipo Documento
                TipoDocumentoId = allegato.TipoDocumentoId,
                TipoDocumentoNome = allegato.TipoDocumento?.Nome ?? string.Empty,
                Descrizione = allegato.Descrizione,

                // Info File
                NomeFile = allegato.NomeFile,
                DimensioneBytes = allegato.DimensioneBytes,
                MimeType = allegato.MimeType,

                // Stato Upload
                IsUploadCompleto = allegato.IsUploadCompleto,
                DataCaricamento = allegato.DataCaricamento,
                CaricatoDaNome = allegato.CaricatoDa?.NomeCompleto ?? string.Empty
            };
        }

        /// <summary>
        /// Mappa un'entità AllegatoRegistro a AllegatoRegistroDetailsViewModel
        /// </summary>
        public static AllegatoRegistroDetailsViewModel ToDetailsViewModel(this AllegatoRegistro allegato)
        {
            return new AllegatoRegistroDetailsViewModel
            {
                Id = allegato.Id,

                // Riferimento Registro
                RegistroContrattiId = allegato.RegistroContrattiId,
                RegistroNumeroProtocollo = allegato.RegistroContratti?.NumeroProtocollo,
                RegistroOggetto = allegato.RegistroContratti?.Oggetto,
                RegistroRagioneSociale = allegato.RegistroContratti?.RagioneSociale,

                // Tipo Documento
                TipoDocumentoId = allegato.TipoDocumentoId,
                TipoDocumentoNome = allegato.TipoDocumento?.Nome ?? string.Empty,
                Descrizione = allegato.Descrizione,

                // Info File
                NomeFile = allegato.NomeFile,
                PathMinIO = allegato.PathMinIO,
                DimensioneBytes = allegato.DimensioneBytes,
                MimeType = allegato.MimeType,

                // Stato Upload
                IsUploadCompleto = allegato.IsUploadCompleto,
                DataCaricamento = allegato.DataCaricamento,
                CaricatoDaUserId = allegato.CaricatoDaUserId,
                CaricatoDaNome = allegato.CaricatoDa?.NomeCompleto ?? string.Empty,

                // Audit
                CreatedAt = allegato.CreatedAt,
                CreatedBy = allegato.CreatedBy,
                ModifiedAt = allegato.ModifiedAt,
                ModifiedBy = allegato.ModifiedBy
            };
        }

        // ===================================
        // VIEWMODEL → ENTITY
        // ===================================

        /// <summary>
        /// Crea un'entità AllegatoRegistro da AllegatoRegistroUploadViewModel
        /// </summary>
        public static AllegatoRegistro ToEntity(
            this AllegatoRegistroUploadViewModel viewModel,
            string pathMinIO,
            long dimensioneBytes,
            string mimeType,
            string caricatoDaUserId)
        {
            return new AllegatoRegistro
            {
                Id = Guid.NewGuid(),

                // Riferimento Registro
                RegistroContrattiId = viewModel.RegistroContrattiId,

                // Tipo Documento
                TipoDocumentoId = viewModel.TipoDocumentoId,
                Descrizione = NormalizzaStringa(viewModel.Descrizione),

                // Info File
                NomeFile = viewModel.File?.FileName ?? string.Empty,
                PathMinIO = pathMinIO,
                DimensioneBytes = dimensioneBytes,
                MimeType = mimeType,

                // Stato Upload
                IsUploadCompleto = false, // Verrà impostato a true dopo upload completato
                DataCaricamento = DateTime.Now,
                CaricatoDaUserId = caricatoDaUserId

                // CreatedAt, CreatedBy, IsDeleted gestiti da AuditInterceptor
            };
        }

        /// <summary>
        /// Crea un'entità AllegatoRegistro per upload multiplo
        /// </summary>
        public static AllegatoRegistro ToEntity(
            this AllegatoRegistroUploadMultiploViewModel viewModel,
            string nomeFile,
            string pathMinIO,
            long dimensioneBytes,
            string mimeType,
            string caricatoDaUserId)
        {
            return new AllegatoRegistro
            {
                Id = Guid.NewGuid(),

                // Riferimento Registro
                RegistroContrattiId = viewModel.RegistroContrattiId,

                // Tipo Documento
                TipoDocumentoId = viewModel.TipoDocumentoId,
                Descrizione = null, // Nessuna descrizione per upload multiplo

                // Info File
                NomeFile = nomeFile,
                PathMinIO = pathMinIO,
                DimensioneBytes = dimensioneBytes,
                MimeType = mimeType,

                // Stato Upload
                IsUploadCompleto = false,
                DataCaricamento = DateTime.Now,
                CaricatoDaUserId = caricatoDaUserId
            };
        }

        /// <summary>
        /// Aggiorna la descrizione di un allegato esistente
        /// </summary>
        public static void UpdateDescrizione(
            this AllegatoRegistro allegato,
            string? descrizione)
        {
            allegato.Descrizione = NormalizzaStringa(descrizione);

            // ModifiedAt, ModifiedBy gestiti da AuditInterceptor
        }

        /// <summary>
        /// Marca l'upload come completato
        /// </summary>
        public static void MarcaUploadCompletato(this AllegatoRegistro allegato)
        {
            allegato.IsUploadCompleto = true;
        }

        /// <summary>
        /// Aggiorna il tipo documento di un allegato
        /// </summary>
        public static void UpdateTipoDocumento(
            this AllegatoRegistro allegato,
            Guid tipoDocumentoId)
        {
            allegato.TipoDocumentoId = tipoDocumentoId;
        }

        // ===================================
        // COLLECTION MAPPING
        // ===================================

        /// <summary>
        /// Mappa una collezione di AllegatoRegistro a lista di AllegatoRegistroListViewModel
        /// </summary>
        public static IEnumerable<AllegatoRegistroListViewModel> ToListViewModels(
            this IEnumerable<AllegatoRegistro> allegati)
        {
            return allegati.Select(a => a.ToListViewModel());
        }

        // ===================================
        // HELPER METHODS
        // ===================================

        /// <summary>
        /// Normalizza una stringa: trim e null se vuota
        /// </summary>
        private static string? NormalizzaStringa(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            var trimmed = value.Trim();
            return string.IsNullOrEmpty(trimmed) ? null : trimmed;
        }

        /// <summary>
        /// Genera il path MinIO per un allegato
        /// Pattern: registro-contratti/{registroId}/{guid}_{filename}
        /// </summary>
        public static string GeneraPathMinIO(Guid registroContrattiId, Guid allegatoId, string nomeFile)
        {
            // Sanitizza il nome file
            var safeName = SanitizzaNomeFile(nomeFile);
            return $"registro-contratti/{registroContrattiId}/{allegatoId}_{safeName}";
        }

        /// <summary>
        /// Sanitizza un nome file rimuovendo caratteri non validi
        /// </summary>
        private static string SanitizzaNomeFile(string nomeFile)
        {
            if (string.IsNullOrWhiteSpace(nomeFile))
                return "file";

            // Caratteri non validi per nomi file
            var invalidChars = Path.GetInvalidFileNameChars();
            var safeName = new string(nomeFile
                .Where(c => !invalidChars.Contains(c))
                .ToArray());

            // Rimuovi spazi multipli e trim
            safeName = string.Join("_", safeName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));

            // Se vuoto dopo sanitizzazione
            if (string.IsNullOrWhiteSpace(safeName))
                return "file";

            // Limita lunghezza
            if (safeName.Length > 200)
                safeName = safeName.Substring(0, 200);

            return safeName;
        }

        /// <summary>
        /// Ottiene il MIME type da un nome file
        /// </summary>
        public static string GetMimeType(string nomeFile)
        {
            var estensione = Path.GetExtension(nomeFile)?.ToLowerInvariant();

            return estensione switch
            {
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".ppt" => "application/vnd.ms-powerpoint",
                ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
                ".txt" => "text/plain",
                ".csv" => "text/csv",
                ".rtf" => "application/rtf",
                ".odt" => "application/vnd.oasis.opendocument.text",
                ".ods" => "application/vnd.oasis.opendocument.spreadsheet",
                ".odp" => "application/vnd.oasis.opendocument.presentation",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".tiff" or ".tif" => "image/tiff",
                ".svg" => "image/svg+xml",
                ".webp" => "image/webp",
                ".zip" => "application/zip",
                ".rar" => "application/x-rar-compressed",
                ".7z" => "application/x-7z-compressed",
                ".xml" => "application/xml",
                ".json" => "application/json",
                ".html" or ".htm" => "text/html",
                _ => "application/octet-stream"
            };
        }
    }
}