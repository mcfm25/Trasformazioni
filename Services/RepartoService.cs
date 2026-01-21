using Microsoft.AspNetCore.Mvc.Rendering;
using Trasformazioni.Models.Entities;
using Trasformazioni.Models.ViewModels;
using Trasformazioni.Repositories.Interfaces;
using Trasformazioni.Services.Interfaces;

namespace Trasformazioni.Services
{
    /// <summary>
    /// Implementazione service per la gestione dei Reparti
    /// </summary>
    public class RepartoService : IRepartoService
    {
        private readonly IRepartoRepository _repository;
        private readonly ILogger<RepartoService> _logger;

        public RepartoService(
            IRepartoRepository repository,
            ILogger<RepartoService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // ===================================
        // QUERY
        // ===================================

        public async Task<RepartoListViewModel> GetPagedAsync(RepartoFilterViewModel filter)
        {
            var (items, totalCount) = await _repository.GetPagedAsync(
                filter.SearchTerm,
                filter.PageNumber,
                filter.PageSize,
                filter.OrderBy,
                filter.OrderDirection);

            var viewModels = new List<RepartoListItemViewModel>();
            foreach (var item in items)
            {
                var utentiCount = await _repository.CountUtentiAsync(item.Id);
                viewModels.Add(new RepartoListItemViewModel
                {
                    Id = item.Id,
                    Nome = item.Nome,
                    Email = item.Email,
                    Descrizione = item.Descrizione,
                    UtentiCount = utentiCount,
                    CreatedAt = item.CreatedAt
                });
            }

            return new RepartoListViewModel
            {
                Items = viewModels,
                TotalCount = totalCount,
                Filter = filter
            };
        }

        public async Task<RepartoDetailsViewModel?> GetByIdAsync(Guid id)
        {
            var reparto = await _repository.GetByIdWithUtentiAsync(id);
            if (reparto == null)
                return null;

            return new RepartoDetailsViewModel
            {
                Id = reparto.Id,
                Nome = reparto.Nome,
                Email = reparto.Email,
                Descrizione = reparto.Descrizione,
                Utenti = reparto.Utenti.Select(u => new RepartoUtenteViewModel
                {
                    Id = u.Id,
                    NomeCompleto = u.NomeCompleto,
                    Email = u.Email ?? "",
                    IsAttivo = u.IsAttivo
                }).ToList(),
                CreatedAt = reparto.CreatedAt,
                CreatedBy = reparto.CreatedBy,
                ModifiedAt = reparto.ModifiedAt,
                ModifiedBy = reparto.ModifiedBy
            };
        }

        public async Task<RepartoEditViewModel?> GetForEditAsync(Guid id)
        {
            var reparto = await _repository.GetByIdAsync(id);
            if (reparto == null)
                return null;

            return new RepartoEditViewModel
            {
                Id = reparto.Id,
                Nome = reparto.Nome,
                Email = reparto.Email,
                Descrizione = reparto.Descrizione
            };
        }

        // ===================================
        // CRUD
        // ===================================

        public async Task<(bool Success, string? ErrorMessage, Guid? RepartoId)> CreateAsync(
            RepartoCreateViewModel model, string currentUserId)
        {
            try
            {
                // Validazione unicità nome
                if (!await IsNomeUniqueAsync(model.Nome))
                    return (false, "Esiste già un reparto con questo nome", null);

                // Validazione unicità email
                if (!await IsEmailUniqueAsync(model.Email))
                    return (false, "Esiste già un reparto con questa email", null);

                var reparto = new Reparto
                {
                    Id = Guid.NewGuid(),
                    Nome = model.Nome.Trim(),
                    Email = model.Email.Trim(),
                    Descrizione = model.Descrizione?.Trim()
                };

                await _repository.AddAsync(reparto);
                await _repository.SaveChangesAsync();

                _logger.LogInformation("Reparto {RepartoId} creato da {UserId}", reparto.Id, currentUserId);

                return (true, null, reparto.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la creazione del reparto");
                return (false, "Errore durante la creazione del reparto", null);
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(
            RepartoEditViewModel model, string currentUserId)
        {
            try
            {
                var reparto = await _repository.GetByIdAsync(model.Id);
                if (reparto == null)
                    return (false, "Reparto non trovato");

                // Validazione unicità nome
                if (!await IsNomeUniqueAsync(model.Nome, model.Id))
                    return (false, "Esiste già un reparto con questo nome");

                // Validazione unicità email
                if (!await IsEmailUniqueAsync(model.Email, model.Id))
                    return (false, "Esiste già un reparto con questa email");

                reparto.Nome = model.Nome.Trim();
                reparto.Email = model.Email.Trim();
                reparto.Descrizione = model.Descrizione?.Trim();

                await _repository.UpdateAsync(reparto);
                await _repository.SaveChangesAsync();

                _logger.LogInformation("Reparto {RepartoId} modificato da {UserId}", reparto.Id, currentUserId);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la modifica del reparto {RepartoId}", model.Id);
                return (false, "Errore durante la modifica del reparto");
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(Guid id, string currentUserId)
        {
            try
            {
                var reparto = await _repository.GetByIdAsync(id);
                if (reparto == null)
                    return (false, "Reparto non trovato");

                // Verifica se ci sono utenti associati
                if (!await CanDeleteAsync(id))
                    return (false, "Impossibile eliminare: ci sono utenti associati a questo reparto");

                await _repository.DeleteAsync(reparto);
                await _repository.SaveChangesAsync();

                _logger.LogInformation("Reparto {RepartoId} eliminato da {UserId}", id, currentUserId);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'eliminazione del reparto {RepartoId}", id);
                return (false, "Errore durante l'eliminazione del reparto");
            }
        }

        // ===================================
        // VALIDAZIONE
        // ===================================

        public async Task<bool> IsNomeUniqueAsync(string nome, Guid? excludeId = null)
        {
            return await _repository.IsNomeUniqueAsync(nome, excludeId);
        }

        public async Task<bool> IsEmailUniqueAsync(string email, Guid? excludeId = null)
        {
            return await _repository.IsEmailUniqueAsync(email, excludeId);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _repository.ExistsAsync(id);
        }

        public async Task<bool> CanDeleteAsync(Guid id)
        {
            var count = await _repository.CountUtentiAsync(id);
            return count == 0;
        }

        // ===================================
        // DROPDOWN
        // ===================================

        public async Task<SelectList> GetSelectListAsync(Guid? selectedId = null)
        {
            var reparti = await _repository.GetAllAsync();
            return new SelectList(
                reparti.Select(r => new { r.Id, r.Nome }),
                "Id", "Nome", selectedId);
        }
    }
}