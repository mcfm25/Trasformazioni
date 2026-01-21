using Microsoft.AspNetCore.Mvc.Rendering;
using Trasformazioni.Models.ViewModels;

namespace Trasformazioni.Services.Interfaces
{
    /// <summary>
    /// Interfaccia service per la gestione dei Reparti
    /// </summary>
    public interface IRepartoService
    {
        // Query
        Task<RepartoListViewModel> GetPagedAsync(RepartoFilterViewModel filter);
        Task<RepartoDetailsViewModel?> GetByIdAsync(Guid id);
        Task<RepartoEditViewModel?> GetForEditAsync(Guid id);

        // CRUD
        Task<(bool Success, string? ErrorMessage, Guid? RepartoId)> CreateAsync(
            RepartoCreateViewModel model, string currentUserId);
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(
            RepartoEditViewModel model, string currentUserId);
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(Guid id, string currentUserId);

        // Validazione
        Task<bool> IsNomeUniqueAsync(string nome, Guid? excludeId = null);
        Task<bool> IsEmailUniqueAsync(string email, Guid? excludeId = null);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> CanDeleteAsync(Guid id);

        // Dropdown
        Task<SelectList> GetSelectListAsync(Guid? selectedId = null);
    }
}