using Microsoft.Extensions.Logging;
using Trasformazioni.Data.Repositories;
using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Mappings;
using Trasformazioni.Models.ViewModels;
using Trasformazioni.Services.Interfaces;

namespace Trasformazioni.Services
{
    /// <summary>
    /// Service per la gestione dei Partecipanti Lotto
    /// Implementa la logica di business per censimento partecipanti con validazione aggiudicatario unico
    /// </summary>
    public class PartecipanteLottoService : IPartecipanteLottoService
    {
        private readonly IPartecipanteLottoRepository _partecipanteRepository;
        private readonly ILottoRepository _lottoRepository;
        private readonly ISoggettoRepository _soggettoRepository;
        private readonly ILogger<PartecipanteLottoService> _logger;

        public PartecipanteLottoService(
            IPartecipanteLottoRepository partecipanteRepository,
            ILottoRepository lottoRepository,
            ISoggettoRepository soggettoRepository,
            ILogger<PartecipanteLottoService> logger)
        {
            _partecipanteRepository = partecipanteRepository;
            _lottoRepository = lottoRepository;
            _soggettoRepository = soggettoRepository;
            _logger = logger;
        }

        // ===================================
        // QUERY - LETTURA
        // ===================================

        public async Task<IEnumerable<PartecipanteLottoListViewModel>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Recupero tutti i partecipanti lotto");

                var partecipanti = await _partecipanteRepository.GetAllAsync();

                return partecipanti.Select(p => p.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero di tutti i partecipanti");
                throw;
            }
        }

        public async Task<PartecipanteLottoDetailsViewModel?> GetByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Recupero partecipante {Id}", id);

                var partecipante = await _partecipanteRepository.GetCompleteAsync(id);

                if (partecipante == null)
                {
                    _logger.LogWarning("Partecipante {Id} non trovato", id);
                    return null;
                }

                return partecipante.ToDetailsViewModel();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero del partecipante {Id}", id);
                throw;
            }
        }

        public async Task<PartecipanteLottoEditViewModel?> GetForEditAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Recupero partecipante {Id} per modifica", id);

                var partecipante = await _partecipanteRepository.GetWithLottoAsync(id);

                if (partecipante == null)
                {
                    _logger.LogWarning("Partecipante {Id} non trovato", id);
                    return null;
                }

                return partecipante.ToEditViewModel();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero del partecipante {Id} per modifica", id);
                throw;
            }
        }

        // ===================================
        // COMANDI - SCRITTURA
        // ===================================

        public async Task<(bool Success, string? ErrorMessage, Guid? Id)> CreateAsync(
            PartecipanteLottoCreateViewModel model,
            string currentUserId)
        {
            try
            {
                _logger.LogInformation(
                    "Creazione partecipante per lotto {LottoId} da utente {UserId}",
                    model.LottoId,
                    currentUserId
                );

                // Validazione 1: Verifica esistenza lotto
                var lottoExists = await _lottoRepository.ExistsAsync(model.LottoId);
                if (!lottoExists)
                {
                    _logger.LogWarning("Tentativo di creare partecipante per lotto inesistente {LottoId}", model.LottoId);
                    return (false, "Il lotto specificato non esiste", null);
                }

                // Validazione 2: Se SoggettoId specificato, verifica esistenza soggetto
                if (model.SoggettoId.HasValue)
                {
                    var soggettoExists = await _soggettoRepository.ExistsAsync(model.SoggettoId.Value);
                    if (!soggettoExists)
                    {
                        _logger.LogWarning("Tentativo di creare partecipante con soggetto inesistente {SoggettoId}", model.SoggettoId);
                        return (false, "Il soggetto specificato non esiste", null);
                    }
                }

                // Validazione 3: Se SoggettoId null → RagioneSociale obbligatoria
                if (!model.SoggettoId.HasValue && string.IsNullOrWhiteSpace(model.RagioneSociale))
                {
                    return (false, "La Ragione Sociale è obbligatoria se non si seleziona un Soggetto esistente", null);
                }

                // Validazione 4: IsAggiudicatario e IsScartatoDallEnte non possono essere entrambi true
                if (model.IsAggiudicatario && model.IsScartatoDallEnte)
                {
                    return (false, "Un partecipante non può essere contemporaneamente Aggiudicatario e Scartato dall'Ente", null);
                }

                // Validazione 5: Se IsAggiudicatario = true → verifica che non ci sia già un aggiudicatario
                if (model.IsAggiudicatario)
                {
                    var hasAggiudicatario = await _partecipanteRepository.HasAggiudicatarioAsync(model.LottoId);
                    if (hasAggiudicatario)
                    {
                        _logger.LogWarning("Tentativo di creare un secondo aggiudicatario per lotto {LottoId}", model.LottoId);
                        return (false, "Il lotto ha già un aggiudicatario. Usa la funzione 'Imposta Aggiudicatario' per cambiarlo.", null);
                    }
                }

                // Crea entità
                var partecipante = model.ToEntity();

                // Salva (l'AuditInterceptor gestisce CreatedAt, CreatedBy automaticamente)
                var createdPartecipante = await _partecipanteRepository.AddAsync(partecipante);

                _logger.LogInformation(
                    "Partecipante {Id} creato con successo per lotto {LottoId}",
                    createdPartecipante.Id,
                    model.LottoId
                );

                return (true, null, createdPartecipante.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nella creazione del partecipante per lotto {LottoId}", model.LottoId);
                return (false, "Errore durante la creazione del partecipante", null);
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(
            PartecipanteLottoEditViewModel model,
            string currentUserId)
        {
            try
            {
                _logger.LogInformation(
                    "Aggiornamento partecipante {Id} da utente {UserId}",
                    model.Id,
                    currentUserId
                );

                // Validazione 1: Verifica esistenza partecipante
                var partecipante = await _partecipanteRepository.GetByIdAsync(model.Id);
                if (partecipante == null)
                {
                    _logger.LogWarning("Tentativo di aggiornare partecipante inesistente {Id}", model.Id);
                    return (false, "Partecipante non trovato");
                }

                // Validazione 2: Se SoggettoId specificato, verifica esistenza soggetto
                if (model.SoggettoId.HasValue)
                {
                    var soggettoExists = await _soggettoRepository.ExistsAsync(model.SoggettoId.Value);
                    if (!soggettoExists)
                    {
                        _logger.LogWarning("Tentativo di aggiornare partecipante con soggetto inesistente {SoggettoId}", model.SoggettoId);
                        return (false, "Il soggetto specificato non esiste");
                    }
                }

                // Validazione 3: Se SoggettoId null → RagioneSociale obbligatoria
                if (!model.SoggettoId.HasValue && string.IsNullOrWhiteSpace(model.RagioneSociale))
                {
                    return (false, "La Ragione Sociale è obbligatoria se non si seleziona un Soggetto esistente");
                }

                // Validazione 4: IsAggiudicatario e IsScartatoDallEnte non possono essere entrambi true
                if (model.IsAggiudicatario && model.IsScartatoDallEnte)
                {
                    return (false, "Un partecipante non può essere contemporaneamente Aggiudicatario e Scartato dall'Ente");
                }

                // Validazione 5: Se IsAggiudicatario = true → verifica che non ci sia già un altro aggiudicatario
                if (model.IsAggiudicatario)
                {
                    var hasAggiudicatario = await _partecipanteRepository.HasAggiudicatarioAsync(model.LottoId, model.Id);
                    if (hasAggiudicatario)
                    {
                        _logger.LogWarning("Tentativo di impostare un secondo aggiudicatario per lotto {LottoId}", model.LottoId);
                        return (false, "Il lotto ha già un aggiudicatario. Usa la funzione 'Imposta Aggiudicatario' per cambiarlo.");
                    }
                }

                // Aggiorna entità
                model.UpdateEntity(partecipante);

                // Salva (l'AuditInterceptor gestisce ModifiedAt, ModifiedBy automaticamente)
                await _partecipanteRepository.UpdateAsync(partecipante);

                _logger.LogInformation("Partecipante {Id} aggiornato con successo", model.Id);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nell'aggiornamento del partecipante {Id}", model.Id);
                return (false, "Errore durante l'aggiornamento del partecipante");
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(Guid id, string currentUserId)
        {
            try
            {
                _logger.LogInformation(
                    "Eliminazione partecipante {Id} da utente {UserId}",
                    id,
                    currentUserId
                );

                // Validazione: Verifica esistenza
                var exists = await _partecipanteRepository.ExistsAsync(id);
                if (!exists)
                {
                    _logger.LogWarning("Tentativo di eliminare partecipante inesistente {Id}", id);
                    return (false, "Partecipante non trovato");
                }

                // Soft delete (l'AuditInterceptor gestisce IsDeleted, DeletedAt, DeletedBy)
                await _partecipanteRepository.DeleteAsync(id);

                _logger.LogInformation("Partecipante {Id} eliminato con successo", id);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nell'eliminazione del partecipante {Id}", id);
                return (false, "Errore durante l'eliminazione del partecipante");
            }
        }

        // ===================================
        // GESTIONE AGGIUDICATARIO
        // ===================================

        public async Task<(bool Success, string? ErrorMessage)> ImpostaAggiudicatarioAsync(Guid id, string currentUserId)
        {
            try
            {
                _logger.LogInformation(
                    "Impostazione aggiudicatario {Id} da utente {UserId}",
                    id,
                    currentUserId
                );

                // Validazione 1: Verifica esistenza partecipante
                var partecipante = await _partecipanteRepository.GetByIdAsync(id);
                if (partecipante == null)
                {
                    _logger.LogWarning("Tentativo di impostare aggiudicatario inesistente {Id}", id);
                    return (false, "Partecipante non trovato");
                }

                // Validazione 2: Il partecipante non deve essere scartato
                if (partecipante.IsScartatoDallEnte)
                {
                    _logger.LogWarning("Tentativo di impostare come aggiudicatario un partecipante scartato {Id}", id);
                    return (false, "Non è possibile impostare come aggiudicatario un partecipante scartato dall'ente");
                }

                // Rimuovi flag IsAggiudicatario da eventuali altri partecipanti dello stesso lotto
                var altriPartecipanti = await _partecipanteRepository.GetByLottoIdAsync(partecipante.LottoId);
                foreach (var altro in altriPartecipanti.Where(p => p.Id != id && p.IsAggiudicatario))
                {
                    altro.IsAggiudicatario = false;
                    await _partecipanteRepository.UpdateAsync(altro);
                    _logger.LogInformation("Rimosso flag aggiudicatario da partecipante {AltroId}", altro.Id);
                }

                // Imposta il nuovo aggiudicatario
                partecipante.IsAggiudicatario = true;
                await _partecipanteRepository.UpdateAsync(partecipante);

                _logger.LogInformation("Partecipante {Id} impostato come aggiudicatario del lotto {LottoId}", id, partecipante.LottoId);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nell'impostazione dell'aggiudicatario {Id}", id);
                return (false, "Errore durante l'impostazione dell'aggiudicatario");
            }
        }

        public async Task<PartecipanteLottoDetailsViewModel?> GetAggiudicatarioByLottoIdAsync(Guid lottoId)
        {
            try
            {
                _logger.LogInformation("Recupero aggiudicatario per lotto {LottoId}", lottoId);

                var aggiudicatario = await _partecipanteRepository.GetAggiudicatarioByLottoIdAsync(lottoId);

                if (aggiudicatario == null)
                {
                    _logger.LogDebug("Nessun aggiudicatario trovato per lotto {LottoId}", lottoId);
                    return null;
                }

                // Carica relazioni complete
                var aggiudicatarioCompleto = await _partecipanteRepository.GetCompleteAsync(aggiudicatario.Id);

                return aggiudicatarioCompleto?.ToDetailsViewModel();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero dell'aggiudicatario per lotto {LottoId}", lottoId);
                throw;
            }
        }

        // ===================================
        // QUERY SPECIFICHE - PER LOTTO
        // ===================================

        public async Task<IEnumerable<PartecipanteLottoListViewModel>> GetByLottoIdAsync(Guid lottoId)
        {
            try
            {
                _logger.LogInformation("Recupero partecipanti per lotto {LottoId}", lottoId);

                var partecipanti = await _partecipanteRepository.GetByLottoIdAsync(lottoId);

                return partecipanti.Select(p => p.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero dei partecipanti per lotto {LottoId}", lottoId);
                throw;
            }
        }

        public async Task<IEnumerable<PartecipanteLottoListViewModel>> GetScartatiByLottoIdAsync(Guid lottoId)
        {
            try
            {
                _logger.LogInformation("Recupero partecipanti scartati per lotto {LottoId}", lottoId);

                var scartati = await _partecipanteRepository.GetScartatiByLottoIdAsync(lottoId);

                return scartati.Select(p => p.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero dei partecipanti scartati per lotto {LottoId}", lottoId);
                throw;
            }
        }

        public async Task<IEnumerable<PartecipanteLottoListViewModel>> GetNonAggiudicatariByLottoIdAsync(Guid lottoId)
        {
            try
            {
                _logger.LogInformation("Recupero partecipanti non aggiudicatari per lotto {LottoId}", lottoId);

                var nonAggiudicatari = await _partecipanteRepository.GetNonAggiudicatariByLottoIdAsync(lottoId);

                return nonAggiudicatari.Select(p => p.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero dei non aggiudicatari per lotto {LottoId}", lottoId);
                throw;
            }
        }

        public async Task<IEnumerable<PartecipanteLottoListViewModel>> GetByLottoIdOrderedByOffertaAsync(Guid lottoId)
        {
            try
            {
                _logger.LogInformation("Recupero partecipanti ordinati per offerta per lotto {LottoId}", lottoId);

                var partecipanti = await _partecipanteRepository.GetByLottoIdOrderedByOffertaAsync(lottoId);

                return partecipanti.Select(p => p.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero dei partecipanti ordinati per lotto {LottoId}", lottoId);
                throw;
            }
        }

        // ===================================
        // QUERY SPECIFICHE - PER SOGGETTO
        // ===================================

        public async Task<IEnumerable<PartecipanteLottoListViewModel>> GetBySoggettoIdAsync(Guid soggettoId)
        {
            try
            {
                _logger.LogInformation("Recupero partecipazioni per soggetto {SoggettoId}", soggettoId);

                var partecipazioni = await _partecipanteRepository.GetBySoggettoIdAsync(soggettoId);

                return partecipazioni.Select(p => p.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero delle partecipazioni per soggetto {SoggettoId}", soggettoId);
                throw;
            }
        }

        public async Task<IEnumerable<PartecipanteLottoListViewModel>> GetVinteBySoggettoIdAsync(Guid soggettoId)
        {
            try
            {
                _logger.LogInformation("Recupero gare vinte per soggetto {SoggettoId}", soggettoId);

                var vinte = await _partecipanteRepository.GetVinteBySoggettoIdAsync(soggettoId);

                return vinte.Select(p => p.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero delle gare vinte per soggetto {SoggettoId}", soggettoId);
                throw;
            }
        }

        public async Task<IEnumerable<PartecipanteLottoListViewModel>> GetScartateBySoggettoIdAsync(Guid soggettoId)
        {
            try
            {
                _logger.LogInformation("Recupero gare scartate per soggetto {SoggettoId}", soggettoId);

                var scartate = await _partecipanteRepository.GetScartateBySoggettoIdAsync(soggettoId);

                return scartate.Select(p => p.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero delle gare scartate per soggetto {SoggettoId}", soggettoId);
                throw;
            }
        }

        // ===================================
        // QUERY SPECIFICHE - PER STATO
        // ===================================

        public async Task<IEnumerable<PartecipanteLottoListViewModel>> GetAggiudicatariAsync()
        {
            try
            {
                _logger.LogInformation("Recupero tutti gli aggiudicatari");

                var aggiudicatari = await _partecipanteRepository.GetAggiudicatariAsync();

                return aggiudicatari.Select(p => p.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero degli aggiudicatari");
                throw;
            }
        }

        public async Task<IEnumerable<PartecipanteLottoListViewModel>> GetScartatiAsync()
        {
            try
            {
                _logger.LogInformation("Recupero tutti i partecipanti scartati");

                var scartati = await _partecipanteRepository.GetScartatiAsync();

                return scartati.Select(p => p.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero dei partecipanti scartati");
                throw;
            }
        }

        public async Task<IEnumerable<PartecipanteLottoListViewModel>> GetSenzaOffertaAsync()
        {
            try
            {
                _logger.LogInformation("Recupero partecipanti senza offerta");

                var senzaOfferta = await _partecipanteRepository.GetSenzaOffertaAsync();

                return senzaOfferta.Select(p => p.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero dei partecipanti senza offerta");
                throw;
            }
        }

        // ===================================
        // VALIDAZIONI
        // ===================================

        public async Task<bool> LottoHasAggiudicatarioAsync(Guid lottoId, Guid? excludeId = null)
        {
            try
            {
                return await _partecipanteRepository.HasAggiudicatarioAsync(lottoId, excludeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nella verifica aggiudicatario per lotto {LottoId}", lottoId);
                throw;
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            try
            {
                return await _partecipanteRepository.ExistsAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nella verifica esistenza partecipante {Id}", id);
                throw;
            }
        }

        // ===================================
        // STATISTICHE
        // ===================================

        public async Task<int> GetCountPartecipantiByLottoAsync(Guid lottoId)
        {
            try
            {
                _logger.LogInformation("Recupero conteggio partecipanti per lotto {LottoId}", lottoId);

                return await _partecipanteRepository.GetCountByLottoAsync(lottoId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero del conteggio partecipanti per lotto {LottoId}", lottoId);
                throw;
            }
        }

        public async Task<Dictionary<string, int>> GetStatistichePartecipazioniAsync()
        {
            try
            {
                _logger.LogInformation("Recupero statistiche globali partecipazioni");

                return await _partecipanteRepository.GetStatisticheAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero delle statistiche partecipazioni");
                throw;
            }
        }

        public async Task<Dictionary<string, object>> GetStatisticheLottoAsync(Guid lottoId)
        {
            try
            {
                _logger.LogInformation("Recupero statistiche per lotto {LottoId}", lottoId);

                return await _partecipanteRepository.GetStatisticheLottoAsync(lottoId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero delle statistiche per lotto {LottoId}", lottoId);
                throw;
            }
        }
    }
}