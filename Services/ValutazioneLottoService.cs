using Microsoft.Extensions.Logging;
using Trasformazioni.Data.Repositories;
using Trasformazioni.Extensions;
using Trasformazioni.Models.ViewModels;
using Trasformazioni.Services.Interfaces;

namespace Trasformazioni.Services
{
    /// <summary>
    /// Service per la business logic della gestione Valutazioni Lotto
    /// Gestisce sia la valutazione tecnica che economica
    /// </summary>
    public class ValutazioneLottoService : IValutazioneLottoService
    {
        private readonly IValutazioneLottoRepository _valutazioneLottoRepository;
        private readonly ILottoRepository _lottoRepository;
        private readonly ILogger<ValutazioneLottoService> _logger;

        public ValutazioneLottoService(
            IValutazioneLottoRepository valutazioneLottoRepository,
            ILottoRepository lottoRepository,
            ILogger<ValutazioneLottoService> logger)
        {
            _valutazioneLottoRepository = valutazioneLottoRepository;
            _lottoRepository = lottoRepository;
            _logger = logger;
        }

        // ===================================
        // QUERY - LETTURA
        // ===================================

        public async Task<IEnumerable<ValutazioneLottoListViewModel>> GetAllAsync()
        {
            try
            {
                var valutazioni = await _valutazioneLottoRepository.GetAllAsync();
                return valutazioni.Select(v => v.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero di tutte le valutazioni");
                return Enumerable.Empty<ValutazioneLottoListViewModel>();
            }
        }

        public async Task<ValutazioneLottoDetailsViewModel?> GetByIdAsync(Guid id)
        {
            try
            {
                var valutazione = await _valutazioneLottoRepository.GetCompleteAsync(id);
                return valutazione?.ToDetailsViewModel();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero della valutazione ID: {Id}", id);
                return null;
            }
        }

        public async Task<ValutazioneLottoDetailsViewModel?> GetByLottoIdAsync(Guid lottoId)
        {
            try
            {
                var valutazione = await _valutazioneLottoRepository.GetByLottoIdAsync(lottoId);
                return valutazione?.ToDetailsViewModel();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero della valutazione per lotto ID: {LottoId}", lottoId);
                return null;
            }
        }

        // ===================================
        // QUERY - VALUTAZIONI TECNICHE
        // ===================================

        public async Task<IEnumerable<ValutazioneLottoListViewModel>> GetValutazioniTecnicheApprovateAsync()
        {
            try
            {
                var valutazioni = await _valutazioneLottoRepository.GetValutazioniTecnicheApprovateAsync();
                return valutazioni.Select(v => v.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero delle valutazioni tecniche approvate");
                return Enumerable.Empty<ValutazioneLottoListViewModel>();
            }
        }

        public async Task<IEnumerable<ValutazioneLottoListViewModel>> GetValutazioniTecnicheRifiutateAsync()
        {
            try
            {
                var valutazioni = await _valutazioneLottoRepository.GetValutazioniTecnicheRifiutateAsync();
                return valutazioni.Select(v => v.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero delle valutazioni tecniche rifiutate");
                return Enumerable.Empty<ValutazioneLottoListViewModel>();
            }
        }

        public async Task<IEnumerable<ValutazioneLottoListViewModel>> GetValutazioniTecnichePendentiAsync()
        {
            try
            {
                var valutazioni = await _valutazioneLottoRepository.GetValutazioniTecnichePendentiAsync();
                return valutazioni.Select(v => v.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero delle valutazioni tecniche pendenti");
                return Enumerable.Empty<ValutazioneLottoListViewModel>();
            }
        }

        // ===================================
        // QUERY - VALUTAZIONI ECONOMICHE
        // ===================================

        public async Task<IEnumerable<ValutazioneLottoListViewModel>> GetValutazioniEconomicheApprovateAsync()
        {
            try
            {
                var valutazioni = await _valutazioneLottoRepository.GetValutazioniEconomicheApprovateAsync();
                return valutazioni.Select(v => v.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero delle valutazioni economiche approvate");
                return Enumerable.Empty<ValutazioneLottoListViewModel>();
            }
        }

        public async Task<IEnumerable<ValutazioneLottoListViewModel>> GetValutazioniEconomicheRifiutateAsync()
        {
            try
            {
                var valutazioni = await _valutazioneLottoRepository.GetValutazioniEconomicheRifiutateAsync();
                return valutazioni.Select(v => v.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero delle valutazioni economiche rifiutate");
                return Enumerable.Empty<ValutazioneLottoListViewModel>();
            }
        }

        public async Task<IEnumerable<ValutazioneLottoListViewModel>> GetValutazioniEconomichePendentiAsync()
        {
            try
            {
                var valutazioni = await _valutazioneLottoRepository.GetValutazioniEconomichePendentiAsync();
                return valutazioni.Select(v => v.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero delle valutazioni economiche pendenti");
                return Enumerable.Empty<ValutazioneLottoListViewModel>();
            }
        }

        // ===================================
        // QUERY - VALUTAZIONI COMBINATE
        // ===================================

        public async Task<IEnumerable<ValutazioneLottoListViewModel>> GetValutazioniCompleteApprovateAsync()
        {
            try
            {
                var valutazioni = await _valutazioneLottoRepository.GetValutazioniCompleteApprovateAsync();
                return valutazioni.Select(v => v.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero delle valutazioni complete approvate");
                return Enumerable.Empty<ValutazioneLottoListViewModel>();
            }
        }

        public async Task<IEnumerable<ValutazioneLottoListViewModel>> GetValutazioniConRifiutiAsync()
        {
            try
            {
                var valutazioni = await _valutazioneLottoRepository.GetValutazioniConRifiutiAsync();
                return valutazioni.Select(v => v.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero delle valutazioni con rifiuti");
                return Enumerable.Empty<ValutazioneLottoListViewModel>();
            }
        }

        public async Task<IEnumerable<ValutazioneLottoListViewModel>> GetValutazioniIncompleteAsync()
        {
            try
            {
                var valutazioni = await _valutazioneLottoRepository.GetValutazioniIncompleteAsync();
                return valutazioni.Select(v => v.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero delle valutazioni incomplete");
                return Enumerable.Empty<ValutazioneLottoListViewModel>();
            }
        }

        // ===================================
        // QUERY - PER VALUTATORE
        // ===================================

        public async Task<IEnumerable<ValutazioneLottoListViewModel>> GetByValutatoreTecnicoAsync(string valutatoreTecnicoId)
        {
            try
            {
                var valutazioni = await _valutazioneLottoRepository.GetByValutatoreTecnicoAsync(valutatoreTecnicoId);
                return valutazioni.Select(v => v.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero delle valutazioni del valutatore tecnico: {ValutatoreId}", valutatoreTecnicoId);
                return Enumerable.Empty<ValutazioneLottoListViewModel>();
            }
        }

        public async Task<IEnumerable<ValutazioneLottoListViewModel>> GetByValutatoreEconomicoAsync(string valutatoreEconomicoId)
        {
            try
            {
                var valutazioni = await _valutazioneLottoRepository.GetByValutatoreEconomicoAsync(valutatoreEconomicoId);
                return valutazioni.Select(v => v.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero delle valutazioni del valutatore economico: {ValutatoreId}", valutatoreEconomicoId);
                return Enumerable.Empty<ValutazioneLottoListViewModel>();
            }
        }

        // ===================================
        // COMANDI - VALUTAZIONE TECNICA
        // ===================================

        public async Task<(bool Success, string? ErrorMessage, Guid? ValutazioneId)> ValutaTecnicamenteAsync(
            ValutazioneTecnicaViewModel model,
            string currentUserId)
        {
            try
            {
                // 1. Verifica che il lotto esista
                var lottoExists = await _lottoRepository.ExistsAsync(model.LottoId);
                if (!lottoExists)
                {
                    return (false, "Lotto non trovato", null);
                }

                // 2. Verifica se esiste già una valutazione per questo lotto
                var valutazioneEsistente = await _valutazioneLottoRepository.GetByLottoIdAsync(model.LottoId);

                if (valutazioneEsistente != null)
                {
                    // 2a. AGGIORNA valutazione esistente (solo parte tecnica)
                    model.ValutatoreTecnicoId ??= currentUserId;
                    model.UpdateFromTecnicaViewModel(valutazioneEsistente);
                    await _valutazioneLottoRepository.UpdateAsync(valutazioneEsistente);

                    _logger.LogInformation(
                        "Valutazione tecnica aggiornata per lotto {LottoId} da utente {UserId}. Approvata: {Approvata}",
                        model.LottoId, currentUserId, model.TecnicaApprovata);

                    return (true, null, valutazioneEsistente.Id);
                }
                else
                {
                    // 2b. CREA nuova valutazione
                    model.ValutatoreTecnicoId ??= currentUserId;
                    var nuovaValutazione = model.ToEntity();
                    var valutazioneCreata = await _valutazioneLottoRepository.AddAsync(nuovaValutazione);

                    _logger.LogInformation(
                        "Valutazione tecnica creata per lotto {LottoId} da utente {UserId}. Approvata: {Approvata}",
                        model.LottoId, currentUserId, model.TecnicaApprovata);

                    return (true, null, valutazioneCreata.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la valutazione tecnica del lotto {LottoId}", model.LottoId);
                return (false, "Errore durante la valutazione tecnica", null);
            }
        }

        // ===================================
        // COMANDI - VALUTAZIONE ECONOMICA
        // ===================================

        public async Task<(bool Success, string? ErrorMessage, Guid? ValutazioneId)> ValutaEconomicamenteAsync(
            ValutazioneEconomicaViewModel model,
            string currentUserId)
        {
            try
            {
                // 1. Verifica che il lotto esista
                var lottoExists = await _lottoRepository.ExistsAsync(model.LottoId);
                if (!lottoExists)
                {
                    return (false, "Lotto non trovato", null);
                }

                // 2. Verifica che esista una valutazione per questo lotto
                var valutazioneEsistente = await _valutazioneLottoRepository.GetByLottoIdAsync(model.LottoId);
                if (valutazioneEsistente == null)
                {
                    return (false, "Non esiste una valutazione per questo lotto. Effettuare prima la valutazione tecnica.", null);
                }

                // 3. VALIDAZIONE CRITICA: Verifica che la valutazione tecnica sia approvata
                if (valutazioneEsistente.TecnicaApprovata != true)
                {
                    return (false, "La valutazione economica può essere effettuata solo se la valutazione tecnica è stata approvata", null);
                }

                // 4. Verifica che la valutazione economica non sia già stata effettuata
                if (valutazioneEsistente.DataValutazioneEconomica.HasValue)
                {
                    _logger.LogWarning(
                        "Tentativo di rieseguire valutazione economica già esistente per lotto {LottoId}",
                        model.LottoId);
                }

                // 5. AGGIORNA valutazione esistente (solo parte economica)
                model.ValutatoreEconomicoId ??= currentUserId;
                model.UpdateFromEconomicaViewModel(valutazioneEsistente);
                await _valutazioneLottoRepository.UpdateAsync(valutazioneEsistente);

                _logger.LogInformation(
                    "Valutazione economica aggiornata per lotto {LottoId} da utente {UserId}. Approvata: {Approvata}",
                    model.LottoId, currentUserId, model.EconomicaApprovata);

                return (true, null, valutazioneEsistente.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la valutazione economica del lotto {LottoId}", model.LottoId);
                return (false, "Errore durante la valutazione economica", null);
            }
        }

        // ===================================
        // COMANDI - ELIMINAZIONE
        // ===================================

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(Guid id, string currentUserId)
        {
            try
            {
                // 1. Verifica esistenza
                var valutazione = await _valutazioneLottoRepository.GetByIdAsync(id);
                if (valutazione == null)
                {
                    return (false, "Valutazione non trovata");
                }

                // 2. Verifica se può essere eliminata
                var (canDelete, reason) = await CanDeleteAsync(id);
                if (!canDelete)
                {
                    return (false, reason ?? "Impossibile eliminare la valutazione");
                }

                // 3. Elimina (soft delete)
                await _valutazioneLottoRepository.DeleteAsync(id);

                _logger.LogInformation("Valutazione eliminata: ID {Id} da utente {UserId}", id, currentUserId);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'eliminazione della valutazione ID: {Id}", id);
                return (false, "Errore durante l'eliminazione della valutazione");
            }
        }

        // ===================================
        // VALIDAZIONI
        // ===================================

        public async Task<bool> ExistsByLottoIdAsync(Guid lottoId, Guid? excludeId = null)
        {
            try
            {
                return await _valutazioneLottoRepository.ExistsByLottoIdAsync(lottoId, excludeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la verifica esistenza valutazione per lotto {LottoId}", lottoId);
                return false;
            }
        }

        public async Task<bool> CanValutareEconomicamenteAsync(Guid lottoId)
        {
            try
            {
                var valutazione = await _valutazioneLottoRepository.GetByLottoIdAsync(lottoId);

                // Può valutare economicamente se:
                // 1. Esiste una valutazione
                // 2. La valutazione tecnica è approvata
                // 3. La valutazione economica non è ancora stata effettuata
                return valutazione != null &&
                       valutazione.TecnicaApprovata == true &&
                       !valutazione.DataValutazioneEconomica.HasValue;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la verifica possibilità valutazione economica per lotto {LottoId}", lottoId);
                return false;
            }
        }

        public async Task<(bool CanDelete, string? Reason)> CanDeleteAsync(Guid id)
        {
            try
            {
                var valutazione = await _valutazioneLottoRepository.GetByIdAsync(id);
                if (valutazione == null)
                {
                    return (false, "Valutazione non trovata");
                }

                // La valutazione può sempre essere eliminata
                // Non ci sono vincoli particolari
                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la verifica possibilità eliminazione valutazione ID: {Id}", id);
                return (false, "Errore durante la verifica");
            }
        }

        // ===================================
        // STATISTICHE
        // ===================================

        public async Task<Dictionary<string, int>> GetStatisticheApprovazioniAsync()
        {
            try
            {
                return await _valutazioneLottoRepository.GetCountByApprovazioniAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero delle statistiche approvazioni");
                return new Dictionary<string, int>();
            }
        }
    }
}