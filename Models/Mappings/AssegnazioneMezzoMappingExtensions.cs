using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Trasformazioni.Helpers;
using Trasformazioni.Models.Entities;
using Trasformazioni.Models.ViewModels;

namespace Trasformazioni.Mappings
{
    /// <summary>
    /// Extension methods per il mapping tra entità AssegnazioneMezzo e ViewModel
    /// Include conversione automatica date in UTC
    /// </summary>
    public static class AssegnazioneMezzoMappingExtensions
    {
        #region Entity → ViewModel

        /// <summary>
        /// Converte un'entità AssegnazioneMezzo in AssegnazioneMezzoListViewModel
        /// </summary>
        public static AssegnazioneMezzoListViewModel ToListViewModel(this AssegnazioneMezzo assegnazione)
        {
            return new AssegnazioneMezzoListViewModel
            {
                Id = assegnazione.Id,
                MezzoId = assegnazione.MezzoId,
                MezzoTarga = assegnazione.Mezzo?.Targa ?? string.Empty,
                MezzoDescrizioneCompleta = assegnazione.Mezzo?.DescrizioneCompleta ?? string.Empty,
                UtenteId = assegnazione.UtenteId,
                UtenteNomeCompleto = assegnazione.Utente?.NomeCompleto ?? string.Empty,
                DataInizio = assegnazione.DataInizio,
                DataFine = assegnazione.DataFine,
                MotivoAssegnazione = assegnazione.MotivoAssegnazione,
                MotivoAssegnazioneDescrizione = assegnazione.MotivoAssegnazione.GetDisplayName(),
                ChilometraggioInizio = assegnazione.ChilometraggioInizio,
                ChilometraggioFine = assegnazione.ChilometraggioFine,
                ChilometriPercorsi = assegnazione.ChilometriPercorsi,
                Note = assegnazione.Note,
                IsAttiva = assegnazione.IsAttiva,
                IsPrenotazione = assegnazione.IsPrenotazione,
                IsInCorso = assegnazione.IsInCorso,
                DurataGiorni = assegnazione.DurataGiorni
            };
        }

        /// <summary>
        /// Converte un'entità AssegnazioneMezzo in AssegnazioneMezzoDetailsViewModel
        /// </summary>
        public static AssegnazioneMezzoDetailsViewModel ToDetailsViewModel(this AssegnazioneMezzo assegnazione)
        {
            return new AssegnazioneMezzoDetailsViewModel
            {
                Id = assegnazione.Id,
                MezzoId = assegnazione.MezzoId,
                MezzoTarga = assegnazione.Mezzo?.Targa ?? string.Empty,
                MezzoTargaFormattata = TargaValidator.FormattaTarga(assegnazione.Mezzo?.Targa),
                MezzoMarca = assegnazione.Mezzo?.Marca ?? string.Empty,
                MezzoModello = assegnazione.Mezzo?.Modello ?? string.Empty,
                MezzoDescrizioneCompleta = assegnazione.Mezzo?.DescrizioneCompleta ?? string.Empty,
                UtenteId = assegnazione.UtenteId,
                UtenteNomeCompleto = assegnazione.Utente?.NomeCompleto ?? string.Empty,
                UtenteEmail = assegnazione.Utente?.Email ?? string.Empty,
                UtenteReparto = assegnazione.Utente?.RepartoNome,
                DataInizio = assegnazione.DataInizio,
                DataFine = assegnazione.DataFine,
                MotivoAssegnazione = assegnazione.MotivoAssegnazione,
                MotivoAssegnazioneDescrizione = assegnazione.MotivoAssegnazione.GetDisplayName(),
                ChilometraggioInizio = assegnazione.ChilometraggioInizio,
                ChilometraggioFine = assegnazione.ChilometraggioFine,
                ChilometriPercorsi = assegnazione.ChilometriPercorsi,
                Note = assegnazione.Note,
                CreatedAt = assegnazione.CreatedAt,
                CreatedBy = assegnazione.CreatedBy,
                ModifiedAt = assegnazione.ModifiedAt,
                ModifiedBy = assegnazione.ModifiedBy,
                IsAttiva = assegnazione.IsAttiva,
                IsPrenotazione = assegnazione.IsPrenotazione,
                IsInCorso = assegnazione.IsInCorso,
                DurataGiorni = assegnazione.DurataGiorni
            };
        }

        /// <summary>
        /// Converte un'entità AssegnazioneMezzo in AssegnazioneMezzoCloseViewModel (per form riconsegna)
        /// </summary>
        public static AssegnazioneMezzoCloseViewModel ToCloseViewModel(this AssegnazioneMezzo assegnazione)
        {
            return new AssegnazioneMezzoCloseViewModel
            {
                Id = assegnazione.Id,
                MezzoId = assegnazione.MezzoId,
                MezzoTarga = assegnazione.Mezzo?.Targa ?? string.Empty,
                MezzoDescrizioneCompleta = assegnazione.Mezzo?.DescrizioneCompleta ?? string.Empty,
                UtenteNomeCompleto = assegnazione.Utente?.NomeCompleto ?? string.Empty,
                DataInizio = assegnazione.DataInizio,
                ChilometraggioInizio = assegnazione.ChilometraggioInizio,
                DataFine = DateTime.Now,  // Cambiato da Today a Now (con ora corrente)
                ChilometraggioFine = null,
                NoteRiconsegna = null
            };
        }

        #endregion

        #region ViewModel → Entity

        /// <summary>
        /// Converte un AssegnazioneMezzoCreateViewModel in entità AssegnazioneMezzo
        /// Include conversione automatica date in UTC
        /// Supporta DataFine opzionale per assegnazioni temporanee
        /// </summary>
        public static AssegnazioneMezzo ToEntity(this AssegnazioneMezzoCreateViewModel viewModel)
        {
            return new AssegnazioneMezzo
            {
                Id = Guid.NewGuid(),
                MezzoId = viewModel.MezzoId,
                UtenteId = viewModel.UtenteId,
                DataInizio = viewModel.DataInizio,
                DataFine = viewModel.DataFine,
                MotivoAssegnazione = viewModel.MotivoAssegnazione,
                ChilometraggioInizio = viewModel.ChilometraggioInizio,
                ChilometraggioFine = null,
                Note = viewModel.Note?.Trim()
                // CreatedAt, CreatedBy gestiti da AuditInterceptor
            };
        }

        /// <summary>
        /// Aggiorna un'entità AssegnazioneMezzo con i dati di chiusura dal ViewModel
        /// Include conversione automatica date in UTC
        /// </summary>
        public static void UpdateEntityForClose(this AssegnazioneMezzoCloseViewModel viewModel, AssegnazioneMezzo assegnazione)
        {
            assegnazione.DataFine = viewModel.DataFine; // ConvertToUtc(viewModel.DataFine);
            assegnazione.ChilometraggioFine = viewModel.ChilometraggioFine;

            // Aggiungi note riconsegna alle note esistenti
            if (!string.IsNullOrWhiteSpace(viewModel.NoteRiconsegna))
            {
                if (string.IsNullOrWhiteSpace(assegnazione.Note))
                {
                    assegnazione.Note = $"Riconsegna: {viewModel.NoteRiconsegna.Trim()}";
                }
                else
                {
                    assegnazione.Note += $"\n\nRiconsegna: {viewModel.NoteRiconsegna.Trim()}";
                }
            }
            // ModifiedAt, ModifiedBy gestiti da AuditInterceptor
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Converte un DateTime in UTC se ha valore
        /// </summary>
        private static DateTime ConvertToUtc(DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Utc)
                return dateTime;

            if (dateTime.Kind == DateTimeKind.Local)
                return dateTime.ToUniversalTime();

            // Se è Unspecified (dal form HTML datetime-local),
            // interpreta come ora locale e converte in UTC
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Local).ToUniversalTime();
        }

        /// <summary>
        /// Ottiene il valore dell'attributo Display di un enum
        /// </summary>
        private static string GetDisplayName(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            if (field == null) return value.ToString();

            var attribute = field.GetCustomAttribute<DisplayAttribute>();
            return attribute?.Name ?? value.ToString();
        }

        #endregion
    }
}