using Microsoft.Extensions.Logging;
using Trasformazioni.Data.Repositories;
using Trasformazioni.Mappings;
using Trasformazioni.Models.Enums;
using Trasformazioni.Models.ViewModels;
using Trasformazioni.Services.Interfaces;

namespace Trasformazioni.Services
{
    /// <summary>
    /// Implementazione della business logic per la gestione dei TipiDocumento.
    /// Fornisce validazioni, controlli e operazioni CRUD.
    /// </summary>
    public class TipoDocumentoService : ITipoDocumentoService
    {
        private readonly ITipoDocumentoRepository _repository;
        private readonly ILogger<TipoDocumentoService> _logger;

        public TipoDocumentoService(
            ITipoDocumentoRepository repository,
            ILogger<TipoDocumentoService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // ===================================
        // QUERY - LETTURA
        // ===================================

        public async Task<IEnumerable<TipoDocumentoListViewModel>> GetAllAsync()
        {
            try
            {
                var tipi = await _repository.GetAllAsync();
                var result = new List<TipoDocumentoListViewModel>();

                foreach (var tipo in tipi)
                {
                    var count = await _repository.GetDocumentiCountAsync(tipo.Id);
                    result.Add(tipo.ToListViewModel(count));
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero di tutti i tipi documento");
                throw;
            }
        }

        public async Task<TipoDocumentoDetailsViewModel?> GetByIdAsync(Guid id)
        {
            try
            {
                var tipo = await _repository.GetByIdAsync(id);
                if (tipo == null) return null;

                var count = await _repository.GetDocumentiCountAsync(id);
                var (canDelete, _) = await CanDeleteAsync(id);
                var (canEdit, _) = await CanEditAsync(id);

                return tipo.ToDetailsViewModel(count, canDelete, canEdit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero del tipo documento {Id}", id);
                throw;
            }
        }

        public async Task<TipoDocumentoEditViewModel?> GetForEditAsync(Guid id)
        {
            try
            {
                var tipo = await _repository.GetByIdAsync(id);
                if (tipo == null) return null;

                // L'area può essere cambiata solo se non ci sono documenti associati
                var isInUse = await _repository.IsInUseAsync(id);
                
                return tipo.ToEditViewModel(!isInUse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero del tipo documento per modifica {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<TipoDocumentoListViewModel>> GetByAreaAsync(AreaDocumento area)
        {
            try
            {
                var tipi = await _repository.GetByAreaAsync(area);
                var result = new List<TipoDocumentoListViewModel>();

                foreach (var tipo in tipi)
                {
                    var count = await _repository.GetDocumentiCountAsync(tipo.Id);
                    result.Add(tipo.ToListViewModel(count));
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei tipi documento per area {Area}", area);
                throw;
            }
        }

        public async Task<IEnumerable<TipoDocumentoDropdownViewModel>> GetForDropdownAsync(AreaDocumento? area = null)
        {
            try
            {
                var tipi = area.HasValue
                    ? await _repository.GetByAreaAsync(area.Value)
                    : await _repository.GetAllAsync();

                return tipi.Select(t => t.ToDropdownViewModel()).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei tipi documento per dropdown");
                throw;
            }
        }

        public async Task<bool> IsNomeDisponibileAsync(string nome, AreaDocumento area, Guid? excludeId = null)
        {
            try
            {
                return !await _repository.ExistsByNomeAndAreaAsync(nome, area, excludeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la verifica disponibilità nome {Nome} per area {Area}", nome, area);
                throw;
            }
        }

        // ===================================
        // COMANDI - SCRITTURA
        // ===================================

        public async Task<(bool Success, string? ErrorMessage, Guid? Id)> CreateAsync(
            TipoDocumentoCreateViewModel model, string userId)
        {
            try
            {
                _logger.LogInformation("Creazione tipo documento '{Nome}' per area {Area} da utente {UserId}",
                    model.Nome, model.Area, userId);

                // Validazione unicità nome per area
                if (await _repository.ExistsByNomeAndAreaAsync(model.Nome, model.Area))
                {
                    return (false, $"Esiste già un tipo documento con nome '{model.Nome}' per l'area {model.Area}", null);
                }

                var entity = model.ToEntity();
                entity.CreatedAt = DateTime.Now;
                entity.CreatedBy = userId;

                await _repository.AddAsync(entity);
                await _repository.SaveChangesAsync();

                _logger.LogInformation("Tipo documento creato con successo: {Id}", entity.Id);
                return (true, null, entity.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la creazione del tipo documento");
                return (false, "Errore durante la creazione del tipo documento", null);
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(
            TipoDocumentoEditViewModel model, string userId)
        {
            try
            {
                _logger.LogInformation("Modifica tipo documento {Id} da utente {UserId}", model.Id, userId);

                var entity = await _repository.GetByIdAsync(model.Id);
                if (entity == null)
                {
                    return (false, "Tipo documento non trovato");
                }

                // Validazione: i tipi di sistema possono essere modificati solo nella descrizione
                if (entity.IsSystem && entity.Nome != model.Nome)
                {
                    return (false, "Il nome dei tipi di sistema non può essere modificato");
                }

                // Validazione unicità nome per area (se cambiato)
                if (entity.Nome != model.Nome || entity.Area != model.Area)
                {
                    if (await _repository.ExistsByNomeAndAreaAsync(model.Nome, model.Area, model.Id))
                    {
                        return (false, $"Esiste già un tipo documento con nome '{model.Nome}' per l'area {model.Area}");
                    }
                }

                // Validazione: l'area non può essere cambiata se ci sono documenti associati
                var isInUse = await _repository.IsInUseAsync(model.Id);
                if (isInUse && entity.Area != model.Area)
                {
                    return (false, "L'area non può essere modificata perché ci sono documenti associati a questo tipo");
                }

                entity.UpdateFromViewModel(model, !isInUse);
                entity.ModifiedAt = DateTime.Now;
                entity.ModifiedBy = userId;

                await _repository.UpdateAsync(entity);
                await _repository.SaveChangesAsync();

                _logger.LogInformation("Tipo documento {Id} modificato con successo", model.Id);
                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la modifica del tipo documento {Id}", model.Id);
                return (false, "Errore durante la modifica del tipo documento");
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(Guid id, string userId)
        {
            try
            {
                _logger.LogInformation("Eliminazione tipo documento {Id} da utente {UserId}", id, userId);

                var entity = await _repository.GetByIdAsync(id);
                if (entity == null)
                {
                    return (false, "Tipo documento non trovato");
                }

                // Validazione: non si possono eliminare tipi di sistema
                if (entity.IsSystem)
                {
                    return (false, "I tipi di sistema non possono essere eliminati");
                }

                // Validazione: non si possono eliminare tipi con documenti associati
                if (await _repository.IsInUseAsync(id))
                {
                    var count = await _repository.GetDocumentiCountAsync(id);
                    return (false, $"Impossibile eliminare: ci sono {count} documenti associati a questo tipo");
                }

                entity.DeletedBy = userId;
                await _repository.DeleteAsync(entity);
                await _repository.SaveChangesAsync();

                _logger.LogInformation("Tipo documento {Id} eliminato con successo", id);
                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'eliminazione del tipo documento {Id}", id);
                return (false, "Errore durante l'eliminazione del tipo documento");
            }
        }

        // ===================================
        // VALIDAZIONI
        // ===================================

        public async Task<(bool CanDelete, string? Reason)> CanDeleteAsync(Guid id)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(id);
                if (entity == null)
                {
                    return (false, "Tipo documento non trovato");
                }

                if (entity.IsSystem)
                {
                    return (false, "I tipi di sistema non possono essere eliminati");
                }

                if (await _repository.IsInUseAsync(id))
                {
                    var count = await _repository.GetDocumentiCountAsync(id);
                    return (false, $"Ci sono {count} documenti associati a questo tipo");
                }

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la verifica eliminabilità del tipo documento {Id}", id);
                return (false, "Errore durante la verifica");
            }
        }

        public async Task<(bool CanEdit, string? Reason)> CanEditAsync(Guid id)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(id);
                if (entity == null)
                {
                    return (false, "Tipo documento non trovato");
                }

                // I tipi di sistema possono essere modificati ma con limitazioni
                if (entity.IsSystem)
                {
                    return (true, "Per i tipi di sistema è possibile modificare solo la descrizione");
                }

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la verifica modificabilità del tipo documento {Id}", id);
                return (false, "Errore durante la verifica");
            }
        }

        // ===================================
        // STATISTICHE
        // ===================================

        public async Task<Dictionary<string, int>> GetStatisticheAsync()
        {
            try
            {
                return await _repository.GetStatisticheAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero delle statistiche");
                throw;
            }
        }

        public async Task<Dictionary<AreaDocumento, int>> GetCountByAreaAsync()
        {
            try
            {
                return await _repository.GetCountByAreaAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero del conteggio per area");
                throw;
            }
        }
    }
}
