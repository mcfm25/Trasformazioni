using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Trasformazioni.Helpers;
using Trasformazioni.Models.Entities;
using Trasformazioni.Models.ViewModels;

namespace Trasformazioni.Mappings
{
    /// <summary>
    /// Extension methods per il mapping tra entità Mezzo e ViewModel
    /// Include normalizzazione automatica dei dati e calcolo proprietà derivate
    /// </summary>
    public static class MezzoMappingExtensions
    {
        private const int GIORNI_PREAVVISO_SCADENZA = 30;

        #region Entity → ViewModel

        /// <summary>
        /// Converte un'entità Mezzo in MezzoListViewModel
        /// Calcola numero prenotazioni future e assegnazioni totali
        /// </summary>
        public static MezzoListViewModel ToListViewModel(this Mezzo mezzo)
        {
            var oggi = DateTime.Now.Date;
            var ora = DateTime.Now;

            // Filtra assegnazioni attive (non cancellate)
            var assegnazioniAttive = mezzo.Assegnazioni?
                .Where(a => !a.IsDeleted && (a.DataFine == null || a.DataFine > ora))
                .ToList() ?? new List<AssegnazioneMezzo>();

            // Assegnazione correntemente in corso (non futura)
            var assegnazioneInCorso = assegnazioniAttive
                .FirstOrDefault(a => a.DataInizio <= ora && (a.DataFine == null || a.DataFine > ora));

            // Conta prenotazioni future (non ancora iniziate)
            var numeroPrenotazioniFuture = assegnazioniAttive
                .Count(a => a.DataInizio > ora);

            return new MezzoListViewModel
            {
                Id = mezzo.Id,
                Targa = mezzo.Targa,
                Marca = mezzo.Marca,
                Modello = mezzo.Modello,
                Tipo = mezzo.Tipo,
                TipoDescrizione = mezzo.Tipo.GetDisplayName(),
                Stato = mezzo.Stato,
                StatoDescrizione = mezzo.Stato.GetDisplayName(),
                TipoProprieta = mezzo.TipoProprieta,
                TipoProprietaDescrizione = mezzo.TipoProprieta.GetDisplayName(),
                Chilometraggio = mezzo.Chilometraggio,
                IsAssicurazioneInScadenza = IsInScadenza(mezzo.DataScadenzaAssicurazione, oggi),
                IsRevisioneInScadenza = IsInScadenza(mezzo.DataScadenzaRevisione, oggi),

                // Assegnazione in corso (non future)
                AssegnazioneAttiva = assegnazioneInCorso?.ToDetailsViewModel(),

                // NUOVO: Conteggi prenotazioni
                NumeroPrenotazioniFuture = numeroPrenotazioniFuture,
                NumeroAssegnazioniTotali = assegnazioniAttive.Count
            };
        }

        /// <summary>
        /// Converte un'entità Mezzo in MezzoDetailsViewModel
        /// </summary>
        public static MezzoDetailsViewModel ToDetailsViewModel(this Mezzo mezzo)
        {
            var oggi = DateTime.Now.Date;

            return new MezzoDetailsViewModel
            {
                Id = mezzo.Id,
                Targa = mezzo.Targa,
                TargaFormattata = TargaValidator.FormattaTarga(mezzo.Targa),
                Marca = mezzo.Marca,
                Modello = mezzo.Modello,
                Anno = mezzo.Anno,
                Tipo = mezzo.Tipo,
                TipoDescrizione = mezzo.Tipo.GetDisplayName(),
                Stato = mezzo.Stato,
                StatoDescrizione = mezzo.Stato.GetDisplayName(),
                TipoProprieta = mezzo.TipoProprieta,
                TipoProprietaDescrizione = mezzo.TipoProprieta.GetDisplayName(),
                Chilometraggio = mezzo.Chilometraggio,
                DataImmatricolazione = mezzo.DataImmatricolazione,
                DataAcquisto = mezzo.DataAcquisto,
                DataInizioNoleggio = mezzo.DataInizioNoleggio,
                DataFineNoleggio = mezzo.DataFineNoleggio,
                SocietaNoleggio = mezzo.SocietaNoleggio,
                DataScadenzaAssicurazione = mezzo.DataScadenzaAssicurazione,
                DataScadenzaRevisione = mezzo.DataScadenzaRevisione,
                Note = mezzo.Note,
                DeviceIMEI = mezzo.DeviceIMEI,
                DevicePhoneNumber = mezzo.DevicePhoneNumber,
                CreatedAt = mezzo.CreatedAt,
                CreatedBy = mezzo.CreatedBy,
                ModifiedAt = mezzo.ModifiedAt,
                ModifiedBy = mezzo.ModifiedBy,

                // Calcolo scadenze assicurazione
                IsAssicurazioneScaduta = IsScaduta(mezzo.DataScadenzaAssicurazione, oggi),
                IsAssicurazioneInScadenza = IsInScadenza(mezzo.DataScadenzaAssicurazione, oggi),
                GiorniAllaScadenzaAssicurazione = CalcolaGiorniAllaScadenza(mezzo.DataScadenzaAssicurazione, oggi),

                // Calcolo scadenze revisione
                IsRevisioneScaduta = IsScaduta(mezzo.DataScadenzaRevisione, oggi),
                IsRevisioneInScadenza = IsInScadenza(mezzo.DataScadenzaRevisione, oggi),
                GiorniAllaScadenzaRevisione = CalcolaGiorniAllaScadenza(mezzo.DataScadenzaRevisione, oggi),

                // Calcolo scadenze noleggio
                IsNoleggioScaduto = IsScaduta(mezzo.DataFineNoleggio, oggi),
                IsNoleggioInScadenza = IsInScadenza(mezzo.DataFineNoleggio, oggi),

                // ===== PROPRIETÀ ASSEGNAZIONI =====

                // Assegnazione attualmente attiva
                AssegnazioneAttiva = mezzo.AssegnazioneAttiva?.ToDetailsViewModel(),

                // Ultime 5 assegnazioni (ordinate per data inizio decrescente)
                UltimeAssegnazioni = mezzo.Assegnazioni?
                    .Where(a => !a.IsDeleted)
                    .OrderByDescending(a => a.DataInizio)
                    .Take(5)
                    .Select(a => a.ToListViewModel())
                    .ToList() ?? new List<AssegnazioneMezzoListViewModel>()
            };
        }

        /// <summary>
        /// Converte un'entità Mezzo in MezzoEditViewModel
        /// </summary>
        public static MezzoEditViewModel ToEditViewModel(this Mezzo mezzo)
        {
            return new MezzoEditViewModel
            {
                Id = mezzo.Id,
                Targa = mezzo.Targa,
                Marca = mezzo.Marca,
                Modello = mezzo.Modello,
                Anno = mezzo.Anno,
                Tipo = mezzo.Tipo,
                Stato = mezzo.Stato,
                TipoProprieta = mezzo.TipoProprieta,
                Chilometraggio = mezzo.Chilometraggio,
                DataImmatricolazione = mezzo.DataImmatricolazione,
                DataAcquisto = mezzo.DataAcquisto,
                DataInizioNoleggio = mezzo.DataInizioNoleggio,
                DataFineNoleggio = mezzo.DataFineNoleggio,
                SocietaNoleggio = mezzo.SocietaNoleggio,
                DataScadenzaAssicurazione = mezzo.DataScadenzaAssicurazione,
                DataScadenzaRevisione = mezzo.DataScadenzaRevisione,
                Note = mezzo.Note,
                DeviceIMEI = mezzo.DeviceIMEI,
                DevicePhoneNumber = mezzo.DevicePhoneNumber
            };
        }

        #endregion

        #region ViewModel → Entity

        /// <summary>
        /// Converte un MezzoCreateViewModel in entità Mezzo con normalizzazione automatica
        /// </summary>
        public static Mezzo ToEntity(this MezzoCreateViewModel viewModel)
        {
            return new Mezzo
            {
                Id = Guid.NewGuid(),
                Targa = TargaValidator.NormalizzaTarga(viewModel.Targa),
                Marca = NormalizzaStringa(viewModel.Marca),
                Modello = NormalizzaStringa(viewModel.Modello),
                Anno = viewModel.Anno,
                Tipo = viewModel.Tipo,
                Stato = viewModel.Stato,
                TipoProprieta = viewModel.TipoProprieta,
                Chilometraggio = viewModel.Chilometraggio,
                DataImmatricolazione = viewModel.DataImmatricolazione, // ConvertToUtc(viewModel.DataImmatricolazione),
                DataAcquisto = viewModel.DataAcquisto, // ConvertToUtc(viewModel.DataAcquisto),
                DataInizioNoleggio = viewModel.DataInizioNoleggio, //ConvertToUtc(viewModel.DataInizioNoleggio),
                DataFineNoleggio = viewModel.DataFineNoleggio, //ConvertToUtc(viewModel.DataFineNoleggio),
                SocietaNoleggio = NormalizzaStringa(viewModel.SocietaNoleggio),
                DataScadenzaAssicurazione = viewModel.DataScadenzaAssicurazione, // ConvertToUtc(viewModel.DataScadenzaAssicurazione),
                DataScadenzaRevisione = viewModel.DataScadenzaRevisione, //ConvertToUtc(viewModel.DataScadenzaRevisione),
                Note = viewModel.Note?.Trim(),
                DeviceIMEI = viewModel.DeviceIMEI?.Trim(),
                DevicePhoneNumber = viewModel.DevicePhoneNumber?.Trim()
                // CreatedAt, CreatedBy, IsDeleted gestiti da AuditInterceptor
            };
        }

        /// <summary>
        /// Aggiorna un'entità Mezzo esistente con i dati del MezzoEditViewModel
        /// Include normalizzazione automatica dei campi
        /// </summary>
        public static void UpdateEntity(this MezzoEditViewModel viewModel, Mezzo mezzo)
        {
            mezzo.Targa = TargaValidator.NormalizzaTarga(viewModel.Targa);
            mezzo.Marca = NormalizzaStringa(viewModel.Marca);
            mezzo.Modello = NormalizzaStringa(viewModel.Modello);
            mezzo.Anno = viewModel.Anno;
            mezzo.Tipo = viewModel.Tipo;
            mezzo.Stato = viewModel.Stato;
            mezzo.TipoProprieta = viewModel.TipoProprieta;
            mezzo.Chilometraggio = viewModel.Chilometraggio;
            mezzo.DataImmatricolazione = viewModel.DataImmatricolazione; // ConvertToUtc(viewModel.DataImmatricolazione);
            mezzo.DataAcquisto = viewModel.DataAcquisto; // ConvertToUtc(viewModel.DataAcquisto);
            mezzo.DataInizioNoleggio = viewModel.DataInizioNoleggio; // ConvertToUtc(viewModel.DataInizioNoleggio);
            mezzo.DataFineNoleggio = viewModel.DataFineNoleggio; // ConvertToUtc(viewModel.DataFineNoleggio);
            mezzo.SocietaNoleggio = NormalizzaStringa(viewModel.SocietaNoleggio);
            mezzo.DataScadenzaAssicurazione = viewModel.DataScadenzaAssicurazione; // ConvertToUtc(viewModel.DataScadenzaAssicurazione);
            mezzo.DataScadenzaRevisione = viewModel.DataScadenzaRevisione; // ConvertToUtc(viewModel.DataScadenzaRevisione);
            mezzo.Note = viewModel.Note?.Trim();
            mezzo.DeviceIMEI = viewModel.DeviceIMEI?.Trim();
            mezzo.DevicePhoneNumber = viewModel.DevicePhoneNumber?.Trim();
            // ModifiedAt, ModifiedBy gestiti da AuditInterceptor
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Normalizza una stringa: trim + capitalizza prima lettera di ogni parola
        /// </summary>
        private static string NormalizzaStringa(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            var trimmed = input.Trim();

            // Capitalizza prima lettera di ogni parola (es. "fiat punto" → "Fiat Punto")
            var words = trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var capitalized = words.Select(word =>
                char.ToUpperInvariant(word[0]) + word.Substring(1).ToLowerInvariant()
            );

            return string.Join(" ", capitalized);
        }

        /// <summary>
        /// Verifica se una data è scaduta
        /// </summary>
        private static bool IsScaduta(DateTime? dataScadenza, DateTime oggi)
        {
            return dataScadenza.HasValue && dataScadenza.Value.Date < oggi;
        }

        /// <summary>
        /// Verifica se una data è in scadenza (entro i prossimi N giorni)
        /// </summary>
        private static bool IsInScadenza(DateTime? dataScadenza, DateTime oggi)
        {
            if (!dataScadenza.HasValue) return false;

            var giorniMancanti = (dataScadenza.Value.Date - oggi).Days;
            return giorniMancanti >= 0 && giorniMancanti <= GIORNI_PREAVVISO_SCADENZA;
        }

        /// <summary>
        /// Calcola i giorni mancanti alla scadenza (null se non c'è scadenza)
        /// </summary>
        private static int? CalcolaGiorniAllaScadenza(DateTime? dataScadenza, DateTime oggi)
        {
            if (!dataScadenza.HasValue) return null;

            return (dataScadenza.Value.Date - oggi).Days;
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

        /// <summary>
        /// Converte un DateTime nullable in UTC se ha valore
        /// </summary>
        private static DateTime? ConvertToUtc(DateTime? dateTime)
        {
            if (!dateTime.HasValue)
                return null;

            // Se è già UTC, ritorna così com'è
            if (dateTime.Value.Kind == DateTimeKind.Utc)
                return dateTime.Value;

            // Se è Unspecified (come arriva dai form HTML), specifica come UTC
            if (dateTime.Value.Kind == DateTimeKind.Unspecified)
                return DateTime.SpecifyKind(dateTime.Value, DateTimeKind.Utc);

            // Se è Local, converte in UTC
            return dateTime.Value.ToUniversalTime();
        }

        #endregion
    }
}