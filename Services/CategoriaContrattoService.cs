using Microsoft.Extensions.Logging;
using Trasformazioni.Data.Repositories;
using Trasformazioni.Mappings;
using Trasformazioni.Models.ViewModels;
using Trasformazioni.Services.Interfaces;

namespace Trasformazioni.Services
{
    /// <summary>
    /// Implementazione della business logic per la gestione categorie contratto
    /// </summary>
    public class CategoriaContrattoService : ICategoriaContrattoService
    {
        private readonly ICategoriaContrattoRepository _categoriaRepository;
        private readonly ILogger<CategoriaContrattoService> _logger;

        public CategoriaContrattoService(
            ICategoriaContrattoRepository categoriaRepository,
            ILogger<CategoriaContrattoService> logger)
        {
            _categoriaRepository = categoriaRepository;
            _logger = logger;
        }

        // ===================================
        // QUERY - LETTURA
        // ===================================

        /// <summary>
        /// Ottiene tutte le categorie
        /// </summary>
        public async Task<IEnumerable<CategoriaContrattoListViewModel>> GetAllAsync()
        {
            try
            {
                var categorie = await _categoriaRepository.GetAllAsync();
                var viewModels = new List<CategoriaContrattoListViewModel>();

                foreach (var categoria in categorie)
                {
                    var utilizzi = await _categoriaRepository.CountUsageAsync(categoria.Id);
                    viewModels.Add(categoria.ToListViewModel(utilizzi));
                }

                return viewModels;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero di tutte le categorie");
                throw;
            }
        }

        /// <summary>
        /// Ottiene tutte le categorie attive (per dropdown)
        /// </summary>
        public async Task<IEnumerable<CategoriaContrattoListViewModel>> GetAttiveAsync()
        {
            try
            {
                var categorie = await _categoriaRepository.GetAttiveAsync();
                return categorie.Select(c => c.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero delle categorie attive");
                throw;
            }
        }

        /// <summary>
        /// Ottiene il dettaglio di una categoria
        /// </summary>
        public async Task<CategoriaContrattoDetailsViewModel?> GetByIdAsync(Guid id)
        {
            try
            {
                var categoria = await _categoriaRepository.GetByIdAsync(id);
                if (categoria == null)
                    return null;

                var utilizzi = await _categoriaRepository.CountUsageAsync(id);
                return categoria.ToDetailsViewModel(utilizzi);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero della categoria {CategoriaId}", id);
                throw;
            }
        }

        /// <summary>
        /// Ottiene una categoria per modifica
        /// </summary>
        public async Task<CategoriaContrattoEditViewModel?> GetForEditAsync(Guid id)
        {
            try
            {
                var categoria = await _categoriaRepository.GetByIdAsync(id);
                if (categoria == null)
                    return null;

                var utilizzi = await _categoriaRepository.CountUsageAsync(id);
                return categoria.ToEditViewModel(utilizzi);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero della categoria per modifica {CategoriaId}", id);
                throw;
            }
        }

        // ===================================
        // COMANDI - SCRITTURA
        // ===================================

        /// <summary>
        /// Crea una nuova categoria
        /// </summary>
        public async Task<(bool Success, string? ErrorMessage, Guid? CategoriaId)> CreateAsync(
            CategoriaContrattoCreateViewModel model)
        {
            try
            {
                // 1. Validazione nome univoco
                if (!await IsNomeUniqueAsync(model.Nome))
                {
                    return (false, "Esiste già una categoria con questo nome", null);
                }

                // 2. Validazione nome obbligatorio
                if (string.IsNullOrWhiteSpace(model.Nome))
                {
                    return (false, "Il nome è obbligatorio", null);
                }

                // 3. Mapping e salvataggio
                var categoria = model.ToEntity();
                await _categoriaRepository.AddAsync(categoria);
                await _categoriaRepository.SaveChangesAsync();

                _logger.LogInformation("Categoria creata con successo - ID: {CategoriaId}, Nome: {Nome}",
                    categoria.Id, categoria.Nome);

                return (true, null, categoria.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la creazione della categoria {Nome}", model.Nome);
                return (false, "Errore durante la creazione della categoria", null);
            }
        }

        /// <summary>
        /// Aggiorna una categoria esistente
        /// </summary>
        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(
            CategoriaContrattoEditViewModel model)
        {
            try
            {
                // 1. Verifica esistenza
                var categoria = await _categoriaRepository.GetByIdAsync(model.Id);
                if (categoria == null)
                {
                    return (false, "Categoria non trovata");
                }

                // 2. Validazione nome univoco
                if (!await IsNomeUniqueAsync(model.Nome, model.Id))
                {
                    return (false, "Esiste già una categoria con questo nome");
                }

                // 3. Validazione nome obbligatorio
                if (string.IsNullOrWhiteSpace(model.Nome))
                {
                    return (false, "Il nome è obbligatorio");
                }

                // 4. Aggiornamento
                model.UpdateEntity(categoria);
                await _categoriaRepository.UpdateAsync(categoria);
                await _categoriaRepository.SaveChangesAsync();

                _logger.LogInformation("Categoria aggiornata con successo - ID: {CategoriaId}, Nome: {Nome}",
                    categoria.Id, categoria.Nome);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'aggiornamento della categoria {CategoriaId}", model.Id);
                return (false, "Errore durante l'aggiornamento della categoria");
            }
        }

        /// <summary>
        /// Elimina una categoria (soft delete)
        /// </summary>
        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(Guid id)
        {
            try
            {
                // 1. Verifica esistenza
                var categoria = await _categoriaRepository.GetByIdAsync(id);
                if (categoria == null)
                {
                    return (false, "Categoria non trovata");
                }

                // 2. Verifica se utilizzata
                if (await _categoriaRepository.IsUsedAsync(id))
                {
                    var utilizzi = await _categoriaRepository.CountUsageAsync(id);
                    return (false, $"Impossibile eliminare: la categoria è utilizzata da {utilizzi} registri");
                }

                // 3. Soft delete
                await _categoriaRepository.DeleteAsync(categoria);
                await _categoriaRepository.SaveChangesAsync();

                _logger.LogInformation("Categoria eliminata con successo - ID: {CategoriaId}, Nome: {Nome}",
                    categoria.Id, categoria.Nome);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'eliminazione della categoria {CategoriaId}", id);
                return (false, "Errore durante l'eliminazione della categoria");
            }
        }

        /// <summary>
        /// Attiva una categoria
        /// </summary>
        public async Task<(bool Success, string? ErrorMessage)> AttivaAsync(Guid id)
        {
            try
            {
                var categoria = await _categoriaRepository.GetByIdAsync(id);
                if (categoria == null)
                {
                    return (false, "Categoria non trovata");
                }

                if (categoria.IsAttivo)
                {
                    return (false, "La categoria è già attiva");
                }

                categoria.IsAttivo = true;
                await _categoriaRepository.UpdateAsync(categoria);
                await _categoriaRepository.SaveChangesAsync();

                _logger.LogInformation("Categoria attivata con successo - ID: {CategoriaId}", id);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'attivazione della categoria {CategoriaId}", id);
                return (false, "Errore durante l'attivazione della categoria");
            }
        }

        /// <summary>
        /// Disattiva una categoria
        /// </summary>
        public async Task<(bool Success, string? ErrorMessage)> DisattivaAsync(Guid id)
        {
            try
            {
                var categoria = await _categoriaRepository.GetByIdAsync(id);
                if (categoria == null)
                {
                    return (false, "Categoria non trovata");
                }

                if (!categoria.IsAttivo)
                {
                    return (false, "La categoria è già disattiva");
                }

                categoria.IsAttivo = false;
                await _categoriaRepository.UpdateAsync(categoria);
                await _categoriaRepository.SaveChangesAsync();

                _logger.LogInformation("Categoria disattivata con successo - ID: {CategoriaId}", id);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la disattivazione della categoria {CategoriaId}", id);
                return (false, "Errore durante la disattivazione della categoria");
            }
        }

        // ===================================
        // VALIDAZIONI
        // ===================================

        /// <summary>
        /// Verifica se il nome è univoco
        /// </summary>
        public async Task<bool> IsNomeUniqueAsync(string nome, Guid? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(nome))
                return true;

            var exists = await _categoriaRepository.ExistsByNomeAsync(nome, excludeId);
            return !exists;
        }

        /// <summary>
        /// Verifica se la categoria è utilizzata
        /// </summary>
        public async Task<bool> IsUsedAsync(Guid id)
        {
            return await _categoriaRepository.IsUsedAsync(id);
        }

        /// <summary>
        /// Verifica se la categoria esiste
        /// </summary>
        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _categoriaRepository.ExistsAsync(id);
        }

        // ===================================
        // UTILITY
        // ===================================

        /// <summary>
        /// Ottiene il prossimo ordine disponibile
        /// </summary>
        public async Task<int> GetNextOrdineAsync()
        {
            return await _categoriaRepository.GetNextOrdineAsync();
        }

        /// <summary>
        /// Conta quanti registri utilizzano la categoria
        /// </summary>
        public async Task<int> CountUsageAsync(Guid id)
        {
            return await _categoriaRepository.CountUsageAsync(id);
        }

        /// <summary>
        /// Ottiene statistiche sulle categorie
        /// </summary>
        public async Task<(int Totale, int Attive, int Inattive)> GetStatisticheAsync()
        {
            try
            {
                var totale = await _categoriaRepository.CountAsync();
                var attive = await _categoriaRepository.CountAttiveAsync();
                var inattive = totale - attive;

                return (totale, attive, inattive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero delle statistiche categorie");
                throw;
            }
        }
    }
}