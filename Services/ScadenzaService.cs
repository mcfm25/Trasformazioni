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
    /// Implementazione della business logic per la gestione Scadenze
    /// Gestisce lo scadenzario con supporto per scadenze automatiche e manuali
    /// </summary>
    public class ScadenzaService : IScadenzaService
    {
        private readonly IScadenzaRepository _scadenzaRepository;
        private readonly IGaraRepository _garaRepository;
        private readonly ILottoRepository _lottoRepository;
        private readonly IPreventivoRepository _preventivoRepository;
        private readonly ILogger<ScadenzaService> _logger;

        public ScadenzaService(
            IScadenzaRepository scadenzaRepository,
            IGaraRepository garaRepository,
            ILottoRepository lottoRepository,
            IPreventivoRepository preventivoRepository,
            ILogger<ScadenzaService> logger)
        {
            _scadenzaRepository = scadenzaRepository;
            _garaRepository = garaRepository;
            _lottoRepository = lottoRepository;
            _preventivoRepository = preventivoRepository;
            _logger = logger;
        }

        // ===================================
        // QUERY - LETTURA
        // ===================================

        public async Task<IEnumerable<ScadenzaListViewModel>> GetAllAsync()
        {
            try
            {
                var scadenze = await _scadenzaRepository.GetAllAsync();
                return scadenze.Select(s => s.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero di tutte le scadenze");
                throw;
            }
        }

        public async Task<IEnumerable<ScadenzaListViewModel>> GetByDataRangeAsync(DateTime start, DateTime end)
        {
            try
            {
                var scadenze = await _scadenzaRepository.GetByDataScadenzaRangeAsync(start, end);
                return scadenze.Select(s => s.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore recupero scadenze per range {Start} - {End}", start, end);
                return Enumerable.Empty<ScadenzaListViewModel>();
            }
        }

        public async Task<ScadenzaDetailsViewModel?> GetByIdAsync(Guid id)
        {
            try
            {
                var scadenza = await _scadenzaRepository.GetCompleteAsync(id);
                return scadenza?.ToDetailsViewModel();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero della scadenza ID: {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<ScadenzaListViewModel>> GetByGaraIdAsync(Guid garaId, bool? soloAutomatiche = null)
        {
            try
            {
                var scadenze = await _scadenzaRepository.GetByGaraIdAsync(garaId, soloAutomatiche);
                return scadenze.Select(s => s.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero delle scadenze per gara {GaraId}", garaId);
                return Enumerable.Empty<ScadenzaListViewModel>();
            }
        }

        // Stessa logica per GetByLottoIdAsync e GetByPreventivoIdAsync

        public async Task<IEnumerable<ScadenzaListViewModel>> GetByLottoIdAsync(Guid lottoId, bool? soloAutomatiche = null)
        {
            try
            {
                var scadenze = await _scadenzaRepository.GetByLottoIdAsync(lottoId, soloAutomatiche);
                return scadenze.Select(s => s.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero delle scadenze per lotto ID: {LottoId}", lottoId);
                throw;
            }
        }

        public async Task<IEnumerable<ScadenzaListViewModel>> GetByPreventivoIdAsync(Guid preventivoId, bool? soloAutomatiche = null)
        {
            try
            {
                var scadenze = await _scadenzaRepository.GetByPreventivoIdAsync(preventivoId, soloAutomatiche);
                return scadenze.Select(s => s.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero delle scadenze per preventivo ID: {PreventivoId}", preventivoId);
                throw;
            }
        }

        public async Task<ScadenzaDetailsViewModel?> GetScadenzaAutomaticaByGaraAndTipoAsync(Guid garaId, TipoScadenza tipo)
        {
            try
            {
                var scadenza = await _scadenzaRepository.GetByGaraAndTipoAsync(garaId, tipo);
                return scadenza?.ToDetailsViewModel();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero della scadenza automatica per gara {GaraId} e tipo {Tipo}", garaId, tipo);
                return null;
            }
        }

        public async Task<ScadenzaDetailsViewModel?> GetScadenzaAutomaticaByLottoAndTipoAsync(Guid lottoId, TipoScadenza tipo)
        {
            try
            {
                var scadenza = await _scadenzaRepository.GetByLottoAndTipoAsync(lottoId, tipo);
                return scadenza?.ToDetailsViewModel();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero della scadenza automatica per lotto {LottoId} e tipo {Tipo}", lottoId, tipo);
                return null;
            }
        }

        public async Task<IEnumerable<ScadenzaListViewModel>> GetByTipoAsync(TipoScadenza tipo)
        {
            try
            {
                var scadenze = await _scadenzaRepository.GetByTipoAsync(tipo);
                return scadenze.Select(s => s.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero delle scadenze per tipo: {Tipo}", tipo);
                throw;
            }
        }

        public async Task<IEnumerable<ScadenzaListViewModel>> GetAttiveAsync()
        {
            try
            {
                var scadenze = await _scadenzaRepository.GetAttiveAsync();
                return scadenze.Select(s => s.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero delle scadenze attive");
                throw;
            }
        }

        public async Task<IEnumerable<ScadenzaListViewModel>> GetCompletateAsync()
        {
            try
            {
                var scadenze = await _scadenzaRepository.GetCompletateAsync();
                return scadenze.Select(s => s.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero delle scadenze completate");
                throw;
            }
        }

        public async Task<IEnumerable<ScadenzaListViewModel>> GetInScadenzaAsync(int giorniPreavviso = 7)
        {
            try
            {
                var scadenze = await _scadenzaRepository.GetInScadenzaAsync(giorniPreavviso);
                return scadenze.Select(s => s.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero delle scadenze in scadenza");
                throw;
            }
        }

        public async Task<IEnumerable<ScadenzaListViewModel>> GetScaduteAsync()
        {
            try
            {
                var scadenze = await _scadenzaRepository.GetScaduteAsync();
                return scadenze.Select(s => s.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero delle scadenze scadute");
                throw;
            }
        }

        public async Task<IEnumerable<ScadenzaListViewModel>> GetOggiAsync()
        {
            try
            {
                var scadenze = await _scadenzaRepository.GetOggiAsync();
                return scadenze.Select(s => s.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero delle scadenze di oggi");
                throw;
            }
        }

        public async Task<IEnumerable<ScadenzaListViewModel>> GetAutomaticheAsync()
        {
            try
            {
                var scadenze = await _scadenzaRepository.GetAutomaticheAsync();
                return scadenze.Select(s => s.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero delle scadenze automatiche");
                throw;
            }
        }

        public async Task<IEnumerable<ScadenzaListViewModel>> GetManualiAsync()
        {
            try
            {
                var scadenze = await _scadenzaRepository.GetManualiAsync();
                return scadenze.Select(s => s.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero delle scadenze manuali");
                throw;
            }
        }

        public async Task<PagedResult<ScadenzaListViewModel>> GetPagedAsync(ScadenzaFilterViewModel filters)
        {
            try
            {
                var (items, totalCount) = await _scadenzaRepository.GetPagedAsync(filters);
                var viewModels = items.Select(s => s.ToListViewModel()).ToList();

                return new PagedResult<ScadenzaListViewModel>
                {
                    Items = viewModels,
                    TotalItems = totalCount,
                    PageNumber = filters.PageNumber,
                    PageSize = filters.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero delle scadenze paginate");
                throw;
            }
        }

        // ===================================
        // COMANDI - SCRITTURA
        // ===================================

        public async Task<(bool Success, string? ErrorMessage, Guid? ScadenzaId)> CreateAsync(ScadenzaCreateViewModel model)
        {
            try
            {
                // 1. Validazione coerenza tipo e relazioni
                var (isValid, errorMessage) = await ValidaTipoERelazioneAsync(
                    model.Tipo,
                    model.GaraId,
                    model.LottoId,
                    model.PreventivoId);

                if (!isValid)
                {
                    return (false, errorMessage, null);
                }

                // 2. Verifica esistenza relazioni se specificate
                if (model.GaraId.HasValue)
                {
                    var garaExists = await _garaRepository.ExistsAsync(model.GaraId.Value);
                    if (!garaExists)
                    {
                        return (false, "Gara non trovata", null);
                    }
                }

                if (model.LottoId.HasValue)
                {
                    var lottoExists = await _lottoRepository.ExistsAsync(model.LottoId.Value);
                    if (!lottoExists)
                    {
                        return (false, "Lotto non trovato", null);
                    }
                }

                if (model.PreventivoId.HasValue)
                {
                    var preventivoExists = await _preventivoRepository.ExistsAsync(model.PreventivoId.Value);
                    if (!preventivoExists)
                    {
                        return (false, "Preventivo non trovato", null);
                    }
                }

                // 3. Validazione date
                if (model.DataScadenza < DateTime.Today.AddDays(-30))
                {
                    return (false, "La data di scadenza non può essere più vecchia di 30 giorni", null);
                }

                // 4. Validazione giorni preavviso
                if (model.GiorniPreavviso < 0 || model.GiorniPreavviso > 365)
                {
                    return (false, "I giorni di preavviso devono essere compresi tra 0 e 365", null);
                }

                // 5. Mapping e salvataggio
                var scadenza = model.ToEntity();
                await _scadenzaRepository.AddAsync(scadenza);

                _logger.LogInformation("Scadenza creata con successo: Tipo {Tipo} - ID: {Id}",
                    model.Tipo, scadenza.Id);

                return (true, null, scadenza.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la creazione della scadenza tipo: {Tipo}", model.Tipo);
                return (false, "Errore durante la creazione della scadenza", null);
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(ScadenzaEditViewModel model)
        {
            try
            {
                // 1. Verifica esistenza
                var scadenza = await _scadenzaRepository.GetByIdAsync(model.Id);
                if (scadenza == null)
                {
                    return (false, "Scadenza non trovata");
                }

                // 2. Validazione modifica scadenze automatiche
                if (scadenza.IsAutomatica && !model.IsAutomatica)
                {
                    return (false, "Non è possibile rimuovere il flag 'Automatica' da una scadenza generata dal sistema");
                }

                // 3. Validazione coerenza tipo e relazioni
                var (isValid, errorMessage) = await ValidaTipoERelazioneAsync(
                    model.Tipo,
                    model.GaraId,
                    model.LottoId,
                    model.PreventivoId);

                if (!isValid)
                {
                    return (false, errorMessage);
                }

                // 4. Verifica esistenza relazioni se specificate
                if (model.GaraId.HasValue)
                {
                    var garaExists = await _garaRepository.ExistsAsync(model.GaraId.Value);
                    if (!garaExists)
                    {
                        return (false, "Gara non trovata");
                    }
                }

                if (model.LottoId.HasValue)
                {
                    var lottoExists = await _lottoRepository.ExistsAsync(model.LottoId.Value);
                    if (!lottoExists)
                    {
                        return (false, "Lotto non trovato");
                    }
                }

                if (model.PreventivoId.HasValue)
                {
                    var preventivoExists = await _preventivoRepository.ExistsAsync(model.PreventivoId.Value);
                    if (!preventivoExists)
                    {
                        return (false, "Preventivo non trovato");
                    }
                }

                // 5. Validazione date
                if (!model.IsCompletata && model.DataScadenza < DateTime.Today.AddDays(-30))
                {
                    return (false, "La data di scadenza di una scadenza attiva non può essere più vecchia di 30 giorni");
                }

                // 6. Validazione completamento
                if (model.IsCompletata && !model.DataCompletamento.HasValue)
                {
                    return (false, "Se la scadenza è completata, è necessario specificare la data di completamento");
                }

                if (!model.IsCompletata && model.DataCompletamento.HasValue)
                {
                    return (false, "Se la scadenza non è completata, la data di completamento deve essere vuota");
                }

                if (model.DataCompletamento.HasValue && model.DataCompletamento.Value > DateTime.Today)
                {
                    return (false, "La data di completamento non può essere nel futuro");
                }

                // 7. Aggiorna l'entità
                model.UpdateEntity(scadenza);
                await _scadenzaRepository.UpdateAsync(scadenza);

                _logger.LogInformation("Scadenza aggiornata con successo: ID {Id}", scadenza.Id);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'aggiornamento della scadenza ID: {Id}", model.Id);
                return (false, "Errore durante l'aggiornamento della scadenza");
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(Guid id)
        {
            try
            {
                // 1. Verifica esistenza
                var scadenza = await _scadenzaRepository.GetByIdAsync(id);
                if (scadenza == null)
                {
                    return (false, "Scadenza non trovata");
                }

                // 2. Verifica che non sia automatica
                if (scadenza.IsAutomatica)
                {
                    return (false, "Le scadenze automatiche non possono essere eliminate manualmente. Vengono gestite dal sistema.");
                }

                // 3. Elimina (soft delete)
                await _scadenzaRepository.DeleteAsync(id);

                _logger.LogInformation("Scadenza eliminata con successo: ID {Id}", id);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'eliminazione della scadenza ID: {Id}", id);
                return (false, "Errore durante l'eliminazione della scadenza");
            }
        }

        // ===================================
        // OPERAZIONI BUSINESS SPECIFICHE
        // ===================================

        public async Task<(bool Success, string? ErrorMessage)> CompletaAsync(Guid scadenzaId, DateTime? dataCompletamento = null)
        {
            try
            {
                var scadenza = await _scadenzaRepository.GetByIdAsync(scadenzaId);
                if (scadenza == null)
                {
                    return (false, "Scadenza non trovata");
                }

                // Verifica se già completata
                if (scadenza.IsCompletata)
                {
                    return (false, "La scadenza è già completata");
                }

                // Usa oggi se non specificata
                var dataCompleta = dataCompletamento ?? DateTime.Today;

                // Validazione data
                if (dataCompleta > DateTime.Today)
                {
                    return (false, "La data di completamento non può essere nel futuro");
                }

                scadenza.IsCompletata = true;
                scadenza.DataCompletamento = dataCompleta;
                await _scadenzaRepository.UpdateAsync(scadenza);

                _logger.LogInformation("Scadenza {ScadenzaId} completata in data {DataCompletamento}",
                    scadenzaId, dataCompleta);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il completamento della scadenza ID: {ScadenzaId}", scadenzaId);
                return (false, "Errore durante il completamento della scadenza");
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> RiattivaAsync(Guid scadenzaId)
        {
            try
            {
                var scadenza = await _scadenzaRepository.GetByIdAsync(scadenzaId);
                if (scadenza == null)
                {
                    return (false, "Scadenza non trovata");
                }

                // Verifica se non completata
                if (!scadenza.IsCompletata)
                {
                    return (false, "La scadenza non è completata");
                }

                scadenza.IsCompletata = false;
                scadenza.DataCompletamento = null;
                await _scadenzaRepository.UpdateAsync(scadenza);

                _logger.LogInformation("Scadenza {ScadenzaId} riattivata", scadenzaId);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la riattivazione della scadenza ID: {ScadenzaId}", scadenzaId);
                return (false, "Errore durante la riattivazione della scadenza");
            }
        }

        // ===================================
        // VALIDAZIONI
        // ===================================

        public Task<(bool IsValid, string? ErrorMessage)> ValidaTipoERelazioneAsync(
            TipoScadenza tipo,
            Guid? garaId,
            Guid? lottoId,
            Guid? preventivoId)
        {
            // Validazione coerenza tipo e relazioni
            switch (tipo)
            {
                case TipoScadenza.PresentazioneOfferta:
                case TipoScadenza.RichiestaChiarimenti:
                    if (!garaId.HasValue)
                    {
                        return Task.FromResult((false, $"Per il tipo '{tipo}' è necessario specificare una gara"));
                    }
                    break;

                case TipoScadenza.ScadenzaPreventivo:
                    if (!preventivoId.HasValue)
                    {
                        return Task.FromResult((false, "Per il tipo 'Scadenza Preventivo' è necessario specificare un preventivo"));
                    }
                    break;

                case TipoScadenza.StipulaContratto:
                case TipoScadenza.ScadenzaContratto:
                    if (!lottoId.HasValue)
                    {
                        return Task.FromResult((false, $"Per il tipo '{tipo}' è necessario specificare un lotto"));
                    }
                    break;

                case TipoScadenza.IntegrazioneDocumentazione:
                    // Può essere associata a gara o lotto
                    if (!garaId.HasValue && !lottoId.HasValue)
                    {
                        return Task.FromResult((false, "Per il tipo 'Integrazione Documentazione' è necessario specificare una gara o un lotto"));
                    }
                    break;

                case TipoScadenza.Altro:
                    // Nessuna validazione specifica
                    break;
            }

            // Validazione gerarchica: se ha lotto, dovrebbe avere anche gara
            if (lottoId.HasValue && !garaId.HasValue)
            {
                return Task.FromResult((false, "Se si specifica un lotto, è necessario specificare anche la gara di appartenenza"));
            }

            // Validazione gerarchica: se ha preventivo, dovrebbe avere anche lotto
            if (preventivoId.HasValue && !lottoId.HasValue)
            {
                return Task.FromResult((false, "Se si specifica un preventivo, è necessario specificare anche il lotto di appartenenza"));
            }

            return Task.FromResult((true, (string?)null));
        }

        // ===================================
        // STATISTICHE
        // ===================================

        public async Task<Dictionary<string, int>> GetStatisticheAsync()
        {
            try
            {
                return await _scadenzaRepository.GetStatisticheAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero delle statistiche scadenze");
                throw;
            }
        }

        public async Task<Dictionary<TipoScadenza, int>> GetCountByTipoAsync()
        {
            try
            {
                return await _scadenzaRepository.GetCountByTipoAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero del conteggio per tipo");
                throw;
            }
        }

        public async Task<int> GetCountAttiveAsync()
        {
            try
            {
                return await _scadenzaRepository.GetCountAttiveAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero del conteggio scadenze attive");
                throw;
            }
        }

        public async Task<int> GetCountInScadenzaAsync(int giorniPreavviso = 7)
        {
            try
            {
                return await _scadenzaRepository.GetCountInScadenzaAsync(giorniPreavviso);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero del conteggio scadenze in scadenza");
                throw;
            }
        }

        public async Task<int> GetCountScaduteAsync()
        {
            try
            {
                return await _scadenzaRepository.GetCountScaduteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero del conteggio scadenze scadute");
                throw;
            }
        }
    }
}