using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Trasformazioni.Models.Entities;
using Trasformazioni.Models.ViewModels.DocumentoGara;

namespace Trasformazioni.Extensions
{
    /// <summary>
    /// Extension methods per il mapping tra DocumentoGara entity e ViewModels
    /// </summary>
    public static class DocumentoGaraMappingExtensions
    {
        /// <summary>
        /// Converte DocumentoGara entity in ListViewModel
        /// </summary>
        public static DocumentoGaraListViewModel ToListViewModel(this DocumentoGara documento)
        {
            return new DocumentoGaraListViewModel
            {
                Id = documento.Id,
                GaraId = documento.GaraId,
                GaraCodice = documento.Gara?.CodiceGara,
                GaraOggetto = documento.Gara?.Titolo,
                LottoId = documento.LottoId,
                LottoCodice = documento.Lotto?.CodiceLotto,
                LottoDescrizione = documento.Lotto?.Descrizione,
                PreventivoId = documento.PreventivoId,
                IntegrazioneId = documento.IntegrazioneId,
                //Tipo = documento.Tipo,
                //TipoDisplay = GetEnumDisplayName(documento.Tipo),
                TipoDocumentoId = documento.TipoDocumentoId,
                TipoDocumentoNome = documento.TipoDocumento?.Nome ?? string.Empty,
                TipoDocumentoCodiceRiferimento = documento.TipoDocumento?.CodiceRiferimento,
                TipoDisplay = documento.TipoDocumento?.Nome ?? string.Empty,
                NomeFile = documento.NomeFile,
                DimensioneBytes = documento.DimensioneBytes,
                DimensioneFormatted = FormatFileSize(documento.DimensioneBytes),
                MimeType = documento.MimeType,
                Descrizione = documento.Descrizione,
                DataCaricamento = documento.DataCaricamento,
                CaricatoDaUserId = documento.CaricatoDaUserId,
                CaricatoDaNome = documento.CaricatoDa?.UserName ?? "Utente Sconosciuto",
                FileIcon = GetFileIcon(documento.MimeType)
            };
        }

        /// <summary>
        /// Converte DocumentoGara entity in DetailsViewModel
        /// </summary>
        public static DocumentoGaraDetailsViewModel ToDetailsViewModel(this DocumentoGara documento)
        {
            return new DocumentoGaraDetailsViewModel
            {
                Id = documento.Id,
                GaraId = documento.GaraId,
                GaraCodice = documento.Gara?.CodiceGara,
                GaraOggetto = documento.Gara?.Titolo,
                //GaraEnteCodice = documento.Gara?.EnteCodice,
                GaraEnteDenominazione = documento.Gara?.EnteAppaltante,
                LottoId = documento.LottoId,
                LottoCodice = documento.Lotto?.CodiceLotto,
                LottoDescrizione = documento.Lotto?.Descrizione,
                PreventivoId = documento.PreventivoId,
                PreventivoFornitore = documento.Preventivo?.Soggetto.NomeCompleto,
                IntegrazioneId = documento.IntegrazioneId,
                IntegrazioneOggetto = documento.Integrazione?.TestoRichiestaEnte,
                //Tipo = documento.Tipo,
                //TipoDisplay = GetEnumDisplayName(documento.Tipo),
                TipoDocumentoId = documento.TipoDocumentoId,
                TipoDocumentoNome = documento.TipoDocumento?.Nome ?? string.Empty,
                TipoDocumentoCodiceRiferimento = documento.TipoDocumento?.CodiceRiferimento,
                TipoDisplay = documento.TipoDocumento?.Nome ?? string.Empty,
                NomeFile = documento.NomeFile,
                PathMinIO = documento.PathMinIO,
                DimensioneBytes = documento.DimensioneBytes,
                DimensioneFormatted = FormatFileSize(documento.DimensioneBytes),
                MimeType = documento.MimeType,
                Descrizione = documento.Descrizione,
                DataCaricamento = documento.DataCaricamento,
                CaricatoDaUserId = documento.CaricatoDaUserId,
                CaricatoDaNome = documento.CaricatoDa?.UserName ?? "Utente Sconosciuto",
                CaricatoDaEmail = documento.CaricatoDa?.Email ?? string.Empty,
                CreatedAt = documento.CreatedAt,
                ModifiedAt = documento.ModifiedAt,
                FileIcon = GetFileIcon(documento.MimeType),
                CanPreview = CanPreviewFile(documento.MimeType)
            };
        }

        /// <summary>
        /// Converte DocumentoGara entity in EditViewModel
        /// </summary>
        public static DocumentoGaraEditViewModel ToEditViewModel(this DocumentoGara documento)
        {
            return new DocumentoGaraEditViewModel
            {
                Id = documento.Id,
                //Tipo = documento.Tipo,
                TipoDocumentoId = documento.TipoDocumentoId,
                Descrizione = documento.Descrizione,
                NomeFile = documento.NomeFile,
                DimensioneBytes = documento.DimensioneBytes,
                DimensioneFormatted = FormatFileSize(documento.DimensioneBytes),
                DataCaricamento = documento.DataCaricamento,
                CaricatoDaNome = documento.CaricatoDa?.UserName ?? "Utente Sconosciuto"
            };
        }

        /// <summary>
        /// Converte collection di DocumentoGara in collection di ListViewModel
        /// </summary>
        public static IEnumerable<DocumentoGaraListViewModel> ToListViewModels(
            this IEnumerable<DocumentoGara> documenti)
        {
            return documenti.Select(d => d.ToListViewModel());
        }

        /// <summary>
        /// Applica le modifiche da EditViewModel a DocumentoGara entity
        /// </summary>
        public static void UpdateFromEditViewModel(
            this DocumentoGara documento,
            DocumentoGaraEditViewModel viewModel)
        {
            //documento.Tipo = viewModel.Tipo;
            documento.TipoDocumentoId = viewModel.TipoDocumentoId;
            documento.Descrizione = viewModel.Descrizione;
        }

        /// <summary>
        /// Crea una nuova entità DocumentoGara da UploadViewModel
        /// </summary>
        public static DocumentoGara ToEntity(
            this DocumentoGaraUploadViewModel viewModel,
            string nomeFile,
            string pathMinIO,
            long dimensioneBytes,
            string mimeType,
            string userId)
        {
            return new DocumentoGara
            {
                Id = Guid.NewGuid(),
                GaraId = viewModel.GaraId,
                LottoId = viewModel.LottoId,
                PreventivoId = viewModel.PreventivoId,
                IntegrazioneId = viewModel.IntegrazioneId,
                //Tipo = viewModel.Tipo,
                TipoDocumentoId = viewModel.TipoDocumentoId,
                NomeFile = nomeFile,
                PathMinIO = pathMinIO,
                DimensioneBytes = dimensioneBytes,
                MimeType = mimeType,
                Descrizione = viewModel.Descrizione,
                DataCaricamento = DateTime.Now,
                CaricatoDaUserId = userId
            };
        }

        // ===== HELPER METHODS =====

        /// <summary>
        /// Ottiene il nome display di un enum dal suo attributo [Display]
        /// </summary>
        private static string GetEnumDisplayName(Enum enumValue)
        {
            var displayAttribute = enumValue
                .GetType()
                .GetMember(enumValue.ToString())
                .First()
                .GetCustomAttribute<DisplayAttribute>();

            return displayAttribute?.Name ?? enumValue.ToString();
        }

        /// <summary>
        /// Formatta la dimensione del file in formato leggibile
        /// </summary>
        private static string FormatFileSize(long bytes)
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

        /// <summary>
        /// Restituisce l'icona Font Awesome appropriata per il MIME type
        /// </summary>
        private static string GetFileIcon(string mimeType)
        {
            return mimeType.ToLowerInvariant() switch
            {
                "application/pdf" => "fa-file-pdf",
                "application/msword" or
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document" => "fa-file-word",
                "application/vnd.ms-excel" or
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" => "fa-file-excel",
                "application/vnd.ms-powerpoint" or
                "application/vnd.openxmlformats-officedocument.presentationml.presentation" => "fa-file-powerpoint",
                "application/zip" or
                "application/x-zip-compressed" or
                "application/x-rar-compressed" or
                "application/x-7z-compressed" => "fa-file-zipper",
                "text/plain" => "fa-file-lines",
                var mime when mime.StartsWith("image/") => "fa-file-image",
                var mime when mime.StartsWith("video/") => "fa-file-video",
                var mime when mime.StartsWith("audio/") => "fa-file-audio",
                _ => "fa-file"
            };
        }

        /// <summary>
        /// Verifica se il file può essere visualizzato in anteprima nel browser
        /// </summary>
        private static bool CanPreviewFile(string mimeType)
        {
            return mimeType.ToLowerInvariant() switch
            {
                "application/pdf" => true,
                var mime when mime.StartsWith("image/") => true,
                var mime when mime.StartsWith("text/") => true,
                _ => false
            };
        }
    }
}