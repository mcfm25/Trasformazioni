using Trasformazioni.Data.Repositories;
using Trasformazioni.Helpers;
using Trasformazioni.Mappings;
using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;
using Trasformazioni.Models.ViewModels;

namespace Trasformazioni.Services
{
    /// <summary>
    /// Servizio per la gestione della business logic dei mezzi aziendali
    /// </summary>
    public class MezzoService : IMezzoService
    {
        private readonly IMezzoRepository _repository;
        private readonly ILogger<MezzoService> _logger;

        public MezzoService(
            IMezzoRepository repository,
            ILogger<MezzoService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<MezzoListViewModel>> GetAllAsync()
        {
            var mezzi = await _repository.GetAllAsync();
            return mezzi.Select(m => m.ToListViewModel());
        }

        public async Task<IEnumerable<Mezzo>> GetAllEntitiesAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<MezzoDetailsViewModel?> GetByIdAsync(Guid id)
        {
            var mezzo = await _repository.GetByIdAsync(id);
            return mezzo?.ToDetailsViewModel();
        }

        public async Task<IEnumerable<MezzoListViewModel>> GetByStatoAsync(StatoMezzo stato)
        {
            var mezzi = await _repository.GetByStatoAsync(stato);
            return mezzi.Select(m => m.ToListViewModel());
        }

        public async Task<IEnumerable<MezzoListViewModel>> GetByTipoProprietaAsync(TipoProprietaMezzo tipoProprieta)
        {
            var mezzi = await _repository.GetByTipoProprietaAsync(tipoProprieta);
            return mezzi.Select(m => m.ToListViewModel());
        }

        public async Task<IEnumerable<MezzoListViewModel>> SearchByTargaAsync(string targa)
        {
            if (string.IsNullOrWhiteSpace(targa))
                return Enumerable.Empty<MezzoListViewModel>();

            var mezzi = await _repository.SearchByTargaAsync(targa);
            return mezzi.Select(m => m.ToListViewModel());
        }

        public async Task<bool> IsTargaUniqueAsync(string targa, Guid? excludeId = null)
        {
            var targaNormalizzata = TargaValidator.NormalizzaTarga(targa);
            return !await _repository.ExistsTargaAsync(targaNormalizzata, excludeId);
        }

        public async Task<bool> IsDeviceIMEIUniqueAsync(string deviceIMEI, Guid? excludeId = null)
        {
            //var targaNormalizzata = TargaValidator.NormalizzaTarga(deviceIMEI);
            return !await _repository.ExistsDeviceIMEIAsync(deviceIMEI, excludeId);
        }

        public bool IsTargaValida(string targa)
        {
            return TargaValidator.IsTargaValida(targa);
        }

        public async Task<(bool Success, string? ErrorMessage, Guid? MezzoId)> CreateAsync(MezzoCreateViewModel model)
        {
            try
            {
                // Validazione targa formato
                if (!IsTargaValida(model.Targa))
                {
                    return (false, TargaValidator.GetErrorMessage(), null);
                }

                // Validazione targa univoca
                if (!await IsTargaUniqueAsync(model.Targa))
                {
                    return (false, "La targa è già presente nel sistema", null);
                }

                // Validazione business logic per noleggio
                if (model.TipoProprieta == TipoProprietaMezzo.Noleggio)
                {
                    if (!model.DataInizioNoleggio.HasValue)
                    {
                        return (false, "Per i mezzi a noleggio è obbligatoria la data di inizio noleggio", null);
                    }

                    if (model.DataFineNoleggio.HasValue &&
                        model.DataFineNoleggio.Value < model.DataInizioNoleggio.Value)
                    {
                        return (false, "La data fine noleggio non può essere precedente alla data inizio", null);
                    }
                }

                // Validazione date per mezzi di proprietà
                if (model.TipoProprieta == TipoProprietaMezzo.Proprieta)
                {
                    if (model.DataAcquisto.HasValue && model.DataImmatricolazione.HasValue &&
                        model.DataAcquisto.Value < model.DataImmatricolazione.Value)
                    {
                        return (false, "La data di acquisto non può essere precedente alla data di immatricolazione", null);
                    }
                }

                // Mapping e salvataggio
                var mezzo = model.ToEntity();
                await _repository.AddAsync(mezzo);

                _logger.LogInformation("Mezzo creato con successo: {Targa} - ID: {Id}", mezzo.Targa, mezzo.Id);

                return (true, null, mezzo.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la creazione del mezzo {Targa}", model.Targa);
                return (false, "Errore durante la creazione del mezzo", null);
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(Guid id, MezzoEditViewModel model)
        {
            try
            {
                var mezzo = await _repository.GetByIdAsync(id);
                if (mezzo == null)
                {
                    return (false, "Mezzo non trovato");
                }

                // Validazione targa formato
                if (!IsTargaValida(model.Targa))
                {
                    return (false, TargaValidator.GetErrorMessage());
                }

                // Validazione targa univoca (escludendo il mezzo corrente)
                if (!await IsTargaUniqueAsync(model.Targa, id))
                {
                    return (false, "La targa è già presente nel sistema");
                }

                // Validazione business logic per noleggio
                if (model.TipoProprieta == TipoProprietaMezzo.Noleggio)
                {
                    if (!model.DataInizioNoleggio.HasValue)
                    {
                        return (false, "Per i mezzi a noleggio è obbligatoria la data di inizio noleggio");
                    }

                    if (model.DataFineNoleggio.HasValue &&
                        model.DataFineNoleggio.Value < model.DataInizioNoleggio.Value)
                    {
                        return (false, "La data fine noleggio non può essere precedente alla data inizio");
                    }
                }

                // Validazione date per mezzi di proprietà
                if (model.TipoProprieta == TipoProprietaMezzo.Proprieta)
                {
                    if (model.DataAcquisto.HasValue && model.DataImmatricolazione.HasValue &&
                        model.DataAcquisto.Value < model.DataImmatricolazione.Value)
                    {
                        return (false, "La data di acquisto non può essere precedente alla data di immatricolazione");
                    }
                }

                // Validazione chilometraggio: non può diminuire
                if (model.Chilometraggio.HasValue && mezzo.Chilometraggio.HasValue &&
                    model.Chilometraggio.Value < mezzo.Chilometraggio.Value)
                {
                    return (false, "Il chilometraggio non può essere ridotto");
                }

                // Aggiorna l'entità
                model.UpdateEntity(mezzo);
                await _repository.UpdateAsync(mezzo);

                _logger.LogInformation("Mezzo aggiornato con successo: {Targa} - ID: {Id}", mezzo.Targa, mezzo.Id);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'aggiornamento del mezzo ID: {Id}", id);
                return (false, "Errore durante l'aggiornamento del mezzo");
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(Guid id)
        {
            try
            {
                var mezzo = await _repository.GetByIdAsync(id);
                if (mezzo == null)
                {
                    return (false, "Mezzo non trovato");
                }

                // Validazione: non eliminare mezzi in uso
                if (mezzo.Stato == StatoMezzo.InUso)
                {
                    return (false, "Non è possibile eliminare un mezzo attualmente in uso");
                }

                await _repository.DeleteAsync(mezzo);

                _logger.LogInformation("Mezzo eliminato con successo: {Targa} - ID: {Id}", mezzo.Targa, mezzo.Id);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'eliminazione del mezzo ID: {Id}", id);
                return (false, "Errore durante l'eliminazione del mezzo");
            }
        }

        public async Task<bool> CambiaStatoAsync(Guid id, StatoMezzo nuovoStato)
        {
            try
            {
                var mezzo = await _repository.GetByIdAsync(id);
                if (mezzo == null)
                {
                    _logger.LogWarning("Tentativo di cambio stato per mezzo non esistente: ID {Id}", id);
                    return false;
                }

                mezzo.Stato = nuovoStato;
                await _repository.UpdateAsync(mezzo);

                _logger.LogInformation("Stato mezzo {Targa} cambiato in {Stato}", mezzo.Targa, nuovoStato);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il cambio stato del mezzo ID: {Id}", id);
                return false;
            }
        }

        public async Task<bool> AggiornaChilometraggioAsync(Guid id, decimal chilometraggio)
        {
            try
            {
                if (chilometraggio < 0)
                {
                    _logger.LogWarning("Tentativo di impostare chilometraggio negativo per mezzo ID: {Id}", id);
                    return false;
                }

                var mezzo = await _repository.GetByIdAsync(id);
                if (mezzo == null)
                {
                    _logger.LogWarning("Tentativo di aggiornare chilometraggio per mezzo non esistente: ID {Id}", id);
                    return false;
                }

                // Validazione: il nuovo chilometraggio non può essere inferiore al precedente
                if (mezzo.Chilometraggio.HasValue && chilometraggio < mezzo.Chilometraggio.Value)
                {
                    _logger.LogWarning("Tentativo di ridurre il chilometraggio del mezzo {Targa}", mezzo.Targa);
                    return false;
                }

                mezzo.Chilometraggio = chilometraggio;
                await _repository.UpdateAsync(mezzo);

                _logger.LogInformation("Chilometraggio mezzo {Targa} aggiornato a {Km} km", mezzo.Targa, chilometraggio);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'aggiornamento del chilometraggio per mezzo ID: {Id}", id);
                return false;
            }
        }

        public async Task<MezzoEditViewModel?> GetEditViewModelAsync(Guid id)
        {
            var mezzo = await _repository.GetByIdAsync(id);
            return mezzo?.ToEditViewModel();
        }
    }
}