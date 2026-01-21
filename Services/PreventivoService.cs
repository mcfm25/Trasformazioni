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
    /// Implementazione della business logic per la gestione Preventivi
    /// Include gestione scadenze e auto-rinnovo
    /// </summary>
    public class PreventivoService : IPreventivoService
    {
        private readonly IPreventivoRepository _preventivoRepository;
        private readonly ILottoRepository _lottoRepository;
        private readonly ISoggettoRepository _soggettoRepository;
        private readonly ILogger<PreventivoService> _logger;

        public PreventivoService(
            IPreventivoRepository preventivoRepository,
            ILottoRepository lottoRepository,
            ISoggettoRepository soggettoRepository,
            ILogger<PreventivoService> logger)
        {
            _preventivoRepository = preventivoRepository;
            _lottoRepository = lottoRepository;
            _soggettoRepository = soggettoRepository;
            _logger = logger;
        }

        // ===================================
        // QUERY - LETTURA
        // ===================================

        public async Task<IEnumerable<PreventivoListViewModel>> GetAllAsync()
        {
            try
            {
                var preventivi = await _preventivoRepository.GetAllAsync();
                return preventivi.Select(p => p.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero di tutti i preventivi");
                throw;
            }
        }

        public async Task<PreventivoDetailsViewModel?> GetByIdAsync(Guid id)
        {
            try
            {
                var preventivo = await _preventivoRepository.GetCompleteAsync(id);
                return preventivo?.ToDetailsViewModel();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero del preventivo ID: {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<PreventivoListViewModel>> GetByLottoIdAsync(Guid lottoId)
        {
            try
            {
                var preventivi = await _preventivoRepository.GetByLottoIdAsync(lottoId);
                return preventivi.Select(p => p.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei preventivi per lotto ID: {LottoId}", lottoId);
                throw;
            }
        }

        public async Task<IEnumerable<PreventivoListViewModel>> GetByFornitoreAsync(Guid soggettoId)
        {
            try
            {
                var preventivi = await _preventivoRepository.GetBySoggettoIdAsync(soggettoId);
                return preventivi.Select(p => p.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei preventivi per fornitore ID: {SoggettoId}", soggettoId);
                throw;
            }
        }

        public async Task<IEnumerable<PreventivoListViewModel>> GetByStatoAsync(StatoPreventivo stato)
        {
            try
            {
                var preventivi = await _preventivoRepository.GetByStatoAsync(stato);
                return preventivi.Select(p => p.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei preventivi per stato: {Stato}", stato);
                throw;
            }
        }

        public async Task<IEnumerable<PreventivoListViewModel>> GetInAttesaAsync()
        {
            try
            {
                var preventivi = await _preventivoRepository.GetInAttesaAsync();
                return preventivi.Select(p => p.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei preventivi in attesa");
                throw;
            }
        }

        public async Task<IEnumerable<PreventivoListViewModel>> GetRicevutiAsync()
        {
            try
            {
                var preventivi = await _preventivoRepository.GetRicevutiAsync();
                return preventivi.Select(p => p.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei preventivi ricevuti");
                throw;
            }
        }

        public async Task<IEnumerable<PreventivoListViewModel>> GetValidiAsync()
        {
            try
            {
                var preventivi = await _preventivoRepository.GetValidiAsync();
                return preventivi.Select(p => p.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei preventivi validi");
                throw;
            }
        }

        public async Task<IEnumerable<PreventivoListViewModel>> GetSelezionatiAsync()
        {
            try
            {
                var preventivi = await _preventivoRepository.GetSelezionatiAsync();
                return preventivi.Select(p => p.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei preventivi selezionati");
                throw;
            }
        }

        public async Task<IEnumerable<PreventivoListViewModel>> GetInScadenzaAsync(int giorniPreavviso = 7)
        {
            try
            {
                var preventivi = await _preventivoRepository.GetInScadenzaAsync(giorniPreavviso);
                return preventivi.Select(p => p.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei preventivi in scadenza");
                throw;
            }
        }

        public async Task<IEnumerable<PreventivoListViewModel>> GetScadutiAsync()
        {
            try
            {
                var preventivi = await _preventivoRepository.GetScadutiAsync();
                return preventivi.Select(p => p.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei preventivi scaduti");
                throw;
            }
        }

        public async Task<PagedResult<PreventivoListViewModel>> GetPagedAsync(PreventivoFilterViewModel filters)
        {
            try
            {
                var (items, totalCount) = await _preventivoRepository.GetPagedAsync(filters);
                var viewModels = items.Select(p => p.ToListViewModel()).ToList();

                return new PagedResult<PreventivoListViewModel>
                {
                    Items = viewModels,
                    TotalItems = totalCount,
                    PageNumber = filters.PageNumber,
                    PageSize = filters.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei preventivi paginati");
                throw;
            }
        }

        // ===================================
        // COMANDI - SCRITTURA
        // ===================================

        public async Task<(bool Success, string? ErrorMessage, Guid? PreventivoId)> CreateAsync(
            PreventivoCreateViewModel model,
            string? documentPath = null)
        {
            try
            {
                // 1. Verifica esistenza lotto
                var lottoExists = await _lottoRepository.ExistsAsync(model.LottoId);
                if (!lottoExists)
                {
                    return (false, "Lotto non trovato", null);
                }

                // 2. Verifica esistenza fornitore
                //var soggettoExists = await _soggettoRepository.ExistsAsync(model.SoggettoId);
                var soggettoExists = await _soggettoRepository.GetByIdAsync(model.SoggettoId);
                if (soggettoExists is null)
                {
                    return (false, "Fornitore non trovato", null);
                }

                // 3. Validazione date
                if (model.DataScadenza <= model.DataRichiesta)
                {
                    return (false, "La data di scadenza deve essere successiva alla data di richiesta", null);
                }

                if (model.DataRicezione.HasValue && model.DataRicezione.Value < model.DataRichiesta)
                {
                    return (false, "La data di ricezione non può essere precedente alla data di richiesta", null);
                }

                // 4. Validazione importo
                if (model.ImportoOfferto.HasValue && model.ImportoOfferto.Value < 0)
                {
                    return (false, "L'importo offerto non può essere negativo", null);
                }

                // 5. Validazione giorni auto-rinnovo
                if (model.GiorniAutoRinnovo.HasValue && model.GiorniAutoRinnovo.Value < 1)
                {
                    return (false, "I giorni di auto-rinnovo devono essere almeno 1", null);
                }

                // 6. Mapping e salvataggio
                var preventivo = model.ToEntity(documentPath);
                await _preventivoRepository.AddAsync(preventivo);

                _logger.LogInformation("Preventivo creato con successo per lotto {LottoId} - Fornitore {SoggettoId} - ID: {Id}",
                    model.LottoId, model.SoggettoId, preventivo.Id);

                return (true, null, preventivo.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la creazione del preventivo per lotto {LottoId}", model.LottoId);
                return (false, "Errore durante la creazione del preventivo", null);
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(
            PreventivoEditViewModel model,
            string? newDocumentPath = null)
        {
            try
            {
                // 1. Verifica esistenza
                var preventivo = await _preventivoRepository.GetByIdAsync(model.Id);
                if (preventivo == null)
                {
                    return (false, "Preventivo non trovato");
                }

                // 2. Validazione date
                if (model.DataScadenza <= model.DataRichiesta)
                {
                    return (false, "La data di scadenza deve essere successiva alla data di richiesta");
                }

                if (model.DataRicezione.HasValue && model.DataRicezione.Value < model.DataRichiesta)
                {
                    return (false, "La data di ricezione non può essere precedente alla data di richiesta");
                }

                // 3. Validazione importo
                if (model.ImportoOfferto.HasValue && model.ImportoOfferto.Value < 0)
                {
                    return (false, "L'importo offerto non può essere negativo");
                }

                // 4. Validazione cambio stato
                if (preventivo.IsSelezionato && model.Stato == StatoPreventivo.Scaduto)
                {
                    return (false, "Non è possibile impostare come scaduto un preventivo selezionato");
                }

                // 5. Aggiorna l'entità
                model.UpdateEntity(preventivo, newDocumentPath);
                await _preventivoRepository.UpdateAsync(preventivo);

                _logger.LogInformation("Preventivo aggiornato con successo: ID {Id}", preventivo.Id);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'aggiornamento del preventivo ID: {Id}", model.Id);
                return (false, "Errore durante l'aggiornamento del preventivo");
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(Guid id)
        {
            try
            {
                // 1. Verifica esistenza
                var preventivo = await _preventivoRepository.GetByIdAsync(id);
                if (preventivo == null)
                {
                    return (false, "Preventivo non trovato");
                }

                // 2. Verifica che non sia selezionato
                if (preventivo.IsSelezionato)
                {
                    return (false, "Impossibile eliminare un preventivo selezionato. Deselezionarlo prima di eliminarlo.");
                }

                // 3. Elimina (soft delete)
                await _preventivoRepository.DeleteAsync(id);

                _logger.LogInformation("Preventivo eliminato con successo: ID {Id}", id);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'eliminazione del preventivo ID: {Id}", id);
                return (false, "Errore durante l'eliminazione del preventivo");
            }
        }

        // ===================================
        // OPERAZIONI BUSINESS SPECIFICHE
        // ===================================

        public async Task<(bool Success, string? ErrorMessage, bool NuovoStato)> ToggleSelezionatoAsync(Guid preventivoId)
        {
            try
            {
                _logger.LogInformation("Toggle selezione preventivo {PreventivoId}", preventivoId);

                // Verifica esistenza preventivo
                var preventivo = await _preventivoRepository.GetByIdAsync(preventivoId);
                if (preventivo == null)
                {
                    _logger.LogWarning("Preventivo non trovato: {PreventivoId}", preventivoId);
                    return (false, "Preventivo non trovato", false);
                }

                // Toggle IsSelezionato
                preventivo.IsSelezionato = !preventivo.IsSelezionato;

                // Salva modifiche
                await _preventivoRepository.UpdateAsync(preventivo);

                _logger.LogInformation(
                    "Preventivo {PreventivoId} {Action}",
                    preventivoId,
                    preventivo.IsSelezionato ? "selezionato" : "deselezionato"
                );

                return (true, null, preventivo.IsSelezionato);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante toggle selezione preventivo {PreventivoId}", preventivoId);
                return (false, "Errore durante il toggle della selezione", false);
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> SelezionaPreventivoAsync(Guid preventivoId)
        {
            try
            {
                // 1. Verifica esistenza e validità
                var preventivo = await _preventivoRepository.GetByIdAsync(preventivoId);
                if (preventivo == null)
                {
                    return (false, "Preventivo non trovato");
                }

                // 2. Verifica che sia in stato valido per selezione
                if (!await CanSelectPreventivoAsync(preventivoId))
                {
                    return (false, "Il preventivo non può essere selezionato. Deve essere in stato Valido.");
                }

                // 3. Deseleziona tutti gli altri preventivi dello stesso lotto
                var altriPreventivi = await _preventivoRepository.GetByLottoIdAsync(preventivo.LottoId);
                foreach (var altro in altriPreventivi.Where(p => p.Id != preventivoId && p.IsSelezionato))
                {
                    altro.IsSelezionato = false;
                    await _preventivoRepository.UpdateAsync(altro);
                }

                // 4. Seleziona il preventivo corrente
                preventivo.IsSelezionato = true;
                await _preventivoRepository.UpdateAsync(preventivo);

                _logger.LogInformation("Preventivo {PreventivoId} selezionato per lotto {LottoId}",
                    preventivoId, preventivo.LottoId);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la selezione del preventivo ID: {PreventivoId}", preventivoId);
                return (false, "Errore durante la selezione del preventivo");
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> ConfermaRicezioneAsync(
            Guid preventivoId,
            DateTime dataRicezione,
            decimal? importoOfferto)
        {
            try
            {
                var preventivo = await _preventivoRepository.GetByIdAsync(preventivoId);
                if (preventivo == null)
                {
                    return (false, "Preventivo non trovato");
                }

                // Verifica stato
                if (preventivo.Stato != StatoPreventivo.InAttesa)
                {
                    return (false, "Il preventivo non è in stato 'In Attesa'");
                }

                // Validazione data
                if (dataRicezione < preventivo.DataRichiesta)
                {
                    return (false, "La data di ricezione non può essere precedente alla data di richiesta");
                }

                // Validazione importo
                if (importoOfferto.HasValue && importoOfferto.Value < 0)
                {
                    return (false, "L'importo offerto non può essere negativo");
                }

                preventivo.DataRicezione = dataRicezione;
                preventivo.ImportoOfferto = importoOfferto;
                preventivo.Stato = StatoPreventivo.Ricevuto;
                await _preventivoRepository.UpdateAsync(preventivo);

                _logger.LogInformation("Ricezione confermata per preventivo {PreventivoId}", preventivoId);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la conferma ricezione del preventivo ID: {PreventivoId}", preventivoId);
                return (false, "Errore durante la conferma di ricezione");
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> ValidaPreventivoAsync(Guid preventivoId)
        {
            try
            {
                var preventivo = await _preventivoRepository.GetByIdAsync(preventivoId);
                if (preventivo == null)
                {
                    return (false, "Preventivo non trovato");
                }

                // Verifica stato
                if (preventivo.Stato != StatoPreventivo.Ricevuto)
                {
                    return (false, "Il preventivo deve essere in stato 'Ricevuto' per essere validato");
                }

                // Verifica che abbia importo offerto
                if (!preventivo.ImportoOfferto.HasValue)
                {
                    return (false, "Il preventivo deve avere un importo offerto per essere validato");
                }

                preventivo.Stato = StatoPreventivo.Valido;
                await _preventivoRepository.UpdateAsync(preventivo);

                _logger.LogInformation("Preventivo {PreventivoId} validato", preventivoId);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la validazione del preventivo ID: {PreventivoId}", preventivoId);
                return (false, "Errore durante la validazione del preventivo");
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> RinnovaPreventivoAsync(Guid preventivoId)
        {
            try
            {
                var preventivo = await _preventivoRepository.GetByIdAsync(preventivoId);
                if (preventivo == null)
                {
                    return (false, "Preventivo non trovato");
                }

                // Verifica che abbia auto-rinnovo
                if (!preventivo.GiorniAutoRinnovo.HasValue)
                {
                    return (false, "Il preventivo non ha auto-rinnovo configurato");
                }

                // Verifica che sia scaduto
                if (preventivo.DataScadenza > DateTime.Now)
                {
                    return (false, "Il preventivo non è ancora scaduto");
                }

                // Rinnova la scadenza
                preventivo.DataScadenza = DateTime.Now.AddDays(preventivo.GiorniAutoRinnovo.Value);

                // Se era scaduto, ripristina lo stato precedente
                if (preventivo.Stato == StatoPreventivo.Scaduto)
                {
                    preventivo.Stato = preventivo.ImportoOfferto.HasValue
                        ? StatoPreventivo.Valido
                        : StatoPreventivo.InAttesa;
                }

                await _preventivoRepository.UpdateAsync(preventivo);

                _logger.LogInformation("Preventivo {PreventivoId} rinnovato. Nuova scadenza: {DataScadenza}",
                    preventivoId, preventivo.DataScadenza);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il rinnovo del preventivo ID: {PreventivoId}", preventivoId);
                return (false, "Errore durante il rinnovo del preventivo");
            }
        }

        // ===================================
        // VALIDAZIONI
        // ===================================

        public Task<bool> ValidaScadenzaAsync(DateTime dataScadenza, DateTime dataRichiesta)
        {
            // La scadenza deve essere successiva alla richiesta
            var isValid = dataScadenza > dataRichiesta;
            return Task.FromResult(isValid);
        }

        public async Task<bool> CanSelectPreventivoAsync(Guid preventivoId)
        {
            var preventivo = await _preventivoRepository.GetByIdAsync(preventivoId);
            if (preventivo == null)
                return false;

            // Può essere selezionato solo se è valido
            return preventivo.Stato == StatoPreventivo.Valido;
        }

        // ===================================
        // STATISTICHE
        // ===================================

        public async Task<Dictionary<StatoPreventivo, int>> GetCountByStatoAsync()
        {
            try
            {
                return await _preventivoRepository.GetCountByStatoAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero delle statistiche preventivi per stato");
                throw;
            }
        }

        public async Task<decimal?> GetImportoMedioByLottoIdAsync(Guid lottoId)
        {
            try
            {
                return await _preventivoRepository.GetImportoMedioByLottoIdAsync(lottoId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dell'importo medio per lotto ID: {LottoId}", lottoId);
                throw;
            }
        }

        public async Task<decimal?> GetImportoMinimoValidiByLottoIdAsync(Guid lottoId)
        {
            try
            {
                return await _preventivoRepository.GetImportoMinimoValidiByLottoIdAsync(lottoId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dell'importo minimo per lotto ID: {LottoId}", lottoId);
                throw;
            }
        }
    }
}