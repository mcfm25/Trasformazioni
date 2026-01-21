using Microsoft.Extensions.Logging;
using Trasformazioni.Data.Repositories;
using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Mappings;
using Trasformazioni.Models.ViewModels;
using Trasformazioni.Services.Interfaces;

namespace Trasformazioni.Services
{
    /// <summary>
    /// Service per la gestione delle Elaborazioni Lotto
    /// Implementa la logica di business per prezzi desiderati/reali e scostamenti
    /// </summary>
    public class ElaborazioneLottoService : IElaborazioneLottoService
    {
        private readonly IElaborazioneLottoRepository _elaborazioneRepository;
        private readonly ILottoRepository _lottoRepository;
        private readonly ILogger<ElaborazioneLottoService> _logger;

        public ElaborazioneLottoService(
            IElaborazioneLottoRepository elaborazioneRepository,
            ILottoRepository lottoRepository,
            ILogger<ElaborazioneLottoService> logger)
        {
            _elaborazioneRepository = elaborazioneRepository;
            _lottoRepository = lottoRepository;
            _logger = logger;
        }

        // ===================================
        // QUERY - LETTURA
        // ===================================

        public async Task<IEnumerable<ElaborazioneLottoListViewModel>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Recupero tutte le elaborazioni lotto");

                var elaborazioni = await _elaborazioneRepository.GetAllAsync();

                return elaborazioni.Select(e => e.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero di tutte le elaborazioni");
                throw;
            }
        }

        public async Task<ElaborazioneLottoDetailsViewModel?> GetByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Recupero elaborazione {Id}", id);

                var elaborazione = await _elaborazioneRepository.GetCompleteAsync(id);

                if (elaborazione == null)
                {
                    _logger.LogWarning("Elaborazione {Id} non trovata", id);
                    return null;
                }

                return elaborazione.ToDetailsViewModel();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero dell'elaborazione {Id}", id);
                throw;
            }
        }

        public async Task<ElaborazioneLottoDetailsViewModel?> GetByLottoIdAsync(Guid lottoId)
        {
            try
            {
                _logger.LogInformation("Recupero elaborazione per lotto {LottoId}", lottoId);

                var elaborazione = await _elaborazioneRepository.GetByLottoIdAsync(lottoId);

                if (elaborazione == null)
                {
                    _logger.LogWarning("Nessuna elaborazione trovata per lotto {LottoId}", lottoId);
                    return null;
                }

                // Carica relazioni complete
                var elaborazioneCompleta = await _elaborazioneRepository.GetCompleteAsync(elaborazione.Id);

                return elaborazioneCompleta?.ToDetailsViewModel();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero dell'elaborazione per lotto {LottoId}", lottoId);
                throw;
            }
        }

        public async Task<ElaborazioneLottoEditViewModel?> GetForEditAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Recupero elaborazione {Id} per modifica", id);

                var elaborazione = await _elaborazioneRepository.GetWithLottoAsync(id);

                if (elaborazione == null)
                {
                    _logger.LogWarning("Elaborazione {Id} non trovata", id);
                    return null;
                }

                return elaborazione.ToEditViewModel();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero dell'elaborazione {Id} per modifica", id);
                throw;
            }
        }

        // ===================================
        // COMANDI - SCRITTURA
        // ===================================

        public async Task<(bool Success, string? ErrorMessage, Guid? Id)> CreateAsync(
            ElaborazioneLottoCreateViewModel model,
            string currentUserId)
        {
            try
            {
                _logger.LogInformation(
                    "Creazione elaborazione per lotto {LottoId} da utente {UserId}",
                    model.LottoId,
                    currentUserId
                );

                // Validazione 1: Verifica esistenza lotto
                var lottoExists = await _lottoRepository.ExistsAsync(model.LottoId);
                if (!lottoExists)
                {
                    _logger.LogWarning("Tentativo di creare elaborazione per lotto inesistente {LottoId}", model.LottoId);
                    return (false, "Il lotto specificato non esiste", null);
                }

                // Validazione 2: Verifica che il lotto non abbia già un'elaborazione
                var hasElaborazione = await _elaborazioneRepository.ExistsByLottoIdAsync(model.LottoId);
                if (hasElaborazione)
                {
                    _logger.LogWarning("Tentativo di creare elaborazione duplicata per lotto {LottoId}", model.LottoId);
                    return (false, "Il lotto ha già un'elaborazione. È possibile modificare quella esistente.", null);
                }

                // Eliminato controllo per permettere inserimento bozza di elaborazione,
                // il controllo viene fatto in fase di finalizzazione
                //// Validazione 3: Verificare che almeno un prezzo sia valorizzato
                //if (!model.PrezzoDesiderato.HasValue && !model.PrezzoRealeUscita.HasValue)
                //{
                //    return (false, "Almeno uno tra Prezzo Desiderato e Prezzo Reale deve essere valorizzato", null);
                //}

                // Validazione 4: Se prezzi diversi → motivazione obbligatoria
                if (model.PrezzoDesiderato.HasValue &&
                    model.PrezzoRealeUscita.HasValue &&
                    model.PrezzoDesiderato.Value != model.PrezzoRealeUscita.Value &&
                    string.IsNullOrWhiteSpace(model.MotivazioneAdattamento))
                {
                    return (false, "La Motivazione Adattamento è obbligatoria quando i prezzi sono diversi", null);
                }

                // Crea entità
                var elaborazione = model.ToEntity();

                // Salva (l'AuditInterceptor gestisce CreatedAt, CreatedBy automaticamente)
                var createdElaborazione = await _elaborazioneRepository.AddAsync(elaborazione);

                _logger.LogInformation(
                    "Elaborazione {Id} creata con successo per lotto {LottoId}",
                    createdElaborazione.Id,
                    model.LottoId
                );

                return (true, null, createdElaborazione.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nella creazione dell'elaborazione per lotto {LottoId}", model.LottoId);
                return (false, "Errore durante la creazione dell'elaborazione", null);
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(
            ElaborazioneLottoEditViewModel model,
            string currentUserId)
        {
            try
            {
                _logger.LogInformation(
                    "Aggiornamento elaborazione {Id} da utente {UserId}",
                    model.Id,
                    currentUserId
                );

                // Validazione 1: Verifica esistenza elaborazione
                var elaborazione = await _elaborazioneRepository.GetByIdAsync(model.Id);
                if (elaborazione == null)
                {
                    _logger.LogWarning("Tentativo di aggiornare elaborazione inesistente {Id}", model.Id);
                    return (false, "Elaborazione non trovata");
                }

                // Validazione 2: Verificare che almeno un prezzo sia valorizzato
                if (!model.PrezzoDesiderato.HasValue && !model.PrezzoRealeUscita.HasValue)
                {
                    return (false, "Almeno uno tra Prezzo Desiderato e Prezzo Reale deve essere valorizzato");
                }

                // Validazione 3: Se prezzi diversi → motivazione obbligatoria
                if (model.PrezzoDesiderato.HasValue &&
                    model.PrezzoRealeUscita.HasValue &&
                    model.PrezzoDesiderato.Value != model.PrezzoRealeUscita.Value &&
                    string.IsNullOrWhiteSpace(model.MotivazioneAdattamento))
                {
                    return (false, "La Motivazione Adattamento è obbligatoria quando i prezzi sono diversi");
                }

                // Aggiorna entità
                model.UpdateEntity(elaborazione);

                // Salva (l'AuditInterceptor gestisce ModifiedAt, ModifiedBy automaticamente)
                await _elaborazioneRepository.UpdateAsync(elaborazione);

                _logger.LogInformation("Elaborazione {Id} aggiornata con successo", model.Id);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nell'aggiornamento dell'elaborazione {Id}", model.Id);
                return (false, "Errore durante l'aggiornamento dell'elaborazione");
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(Guid id, string currentUserId)
        {
            try
            {
                _logger.LogInformation(
                    "Eliminazione elaborazione {Id} da utente {UserId}",
                    id,
                    currentUserId
                );

                // Validazione: Verifica esistenza
                var exists = await _elaborazioneRepository.ExistsAsync(id);
                if (!exists)
                {
                    _logger.LogWarning("Tentativo di eliminare elaborazione inesistente {Id}", id);
                    return (false, "Elaborazione non trovata");
                }

                // Soft delete (l'AuditInterceptor gestisce IsDeleted, DeletedAt, DeletedBy)
                await _elaborazioneRepository.DeleteAsync(id);

                _logger.LogInformation("Elaborazione {Id} eliminata con successo", id);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nell'eliminazione dell'elaborazione {Id}", id);
                return (false, "Errore durante l'eliminazione dell'elaborazione");
            }
        }

        // ===================================
        // QUERY SPECIFICHE - PREZZI
        // ===================================

        public async Task<IEnumerable<ElaborazioneLottoListViewModel>> GetWithScostamentoAsync()
        {
            try
            {
                _logger.LogInformation("Recupero elaborazioni con scostamento");

                var elaborazioni = await _elaborazioneRepository.GetWithScostamentoAsync();

                return elaborazioni.Select(e => e.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero elaborazioni con scostamento");
                throw;
            }
        }

        public async Task<IEnumerable<ElaborazioneLottoListViewModel>> GetConPrezzoRealeSuperioreAsync()
        {
            try
            {
                _logger.LogInformation("Recupero elaborazioni con prezzo reale superiore");

                var elaborazioni = await _elaborazioneRepository.GetConPrezzoRealeSuperioreAsync();

                return elaborazioni.Select(e => e.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero elaborazioni con prezzo reale superiore");
                throw;
            }
        }

        public async Task<IEnumerable<ElaborazioneLottoListViewModel>> GetConPrezzoRealeInferioreAsync()
        {
            try
            {
                _logger.LogInformation("Recupero elaborazioni con prezzo reale inferiore");

                var elaborazioni = await _elaborazioneRepository.GetConPrezzoRealeInferioreAsync();

                return elaborazioni.Select(e => e.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero elaborazioni con prezzo reale inferiore");
                throw;
            }
        }

        public async Task<IEnumerable<ElaborazioneLottoListViewModel>> GetSenzaPrezziAsync()
        {
            try
            {
                _logger.LogInformation("Recupero elaborazioni senza prezzi");

                var elaborazioni = await _elaborazioneRepository.GetSenzaPrezziAsync();

                return elaborazioni.Select(e => e.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero elaborazioni senza prezzi");
                throw;
            }
        }

        // ===================================
        // CALCOLI
        // ===================================

        public async Task<decimal?> CalcolaScostamentoAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Calcolo scostamento per elaborazione {Id}", id);

                var elaborazione = await _elaborazioneRepository.GetByIdAsync(id);

                if (elaborazione == null)
                {
                    _logger.LogWarning("Elaborazione {Id} non trovata", id);
                    return null;
                }

                if (!elaborazione.PrezzoDesiderato.HasValue ||
                    !elaborazione.PrezzoRealeUscita.HasValue ||
                    elaborazione.PrezzoDesiderato.Value == 0)
                {
                    _logger.LogDebug("Impossibile calcolare scostamento per elaborazione {Id}: prezzi mancanti", id);
                    return null;
                }

                var differenza = Math.Abs(elaborazione.PrezzoRealeUscita.Value - elaborazione.PrezzoDesiderato.Value);
                var scostamento = (differenza / elaborazione.PrezzoDesiderato.Value) * 100;

                return Math.Round(scostamento, 2);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel calcolo dello scostamento per elaborazione {Id}", id);
                throw;
            }
        }

        public async Task<decimal?> GetScostamentoMedioAsync()
        {
            try
            {
                _logger.LogInformation("Recupero scostamento medio di tutte le elaborazioni");

                return await _elaborazioneRepository.GetScostamentoMedioAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero dello scostamento medio");
                throw;
            }
        }

        // ===================================
        // VALIDAZIONI
        // ===================================

        public async Task<bool> LottoHasElaborazioneAsync(Guid lottoId, Guid? excludeId = null)
        {
            try
            {
                return await _elaborazioneRepository.ExistsByLottoIdAsync(lottoId, excludeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nella verifica elaborazione per lotto {LottoId}", lottoId);
                throw;
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            try
            {
                return await _elaborazioneRepository.ExistsAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nella verifica esistenza elaborazione {Id}", id);
                throw;
            }
        }

        // ===================================
        // STATISTICHE
        // ===================================

        public async Task<Dictionary<string, int>> GetStatistichePrezziAsync()
        {
            try
            {
                _logger.LogInformation("Recupero statistiche prezzi elaborazioni");

                return await _elaborazioneRepository.GetStatistichePrezziAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero delle statistiche prezzi");
                throw;
            }
        }
    }
}