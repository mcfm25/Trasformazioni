using Microsoft.AspNetCore.Identity;
using Trasformazioni.Models.Entities;
using Trasformazioni.Models.ViewModels;

namespace Trasformazioni.Services.Interfaces
{
    /// <summary>
    /// Interfaccia per il servizio di gestione utenti
    /// </summary>
    public interface IUserService
    {
        // === Autenticazione ===
        Task<SignInResult> LoginAsync(LoginViewModel model);
        Task LogoutAsync();

        // === Registrazione ===
        Task<IdentityResult> RegisterAsync(RegisterViewModel model);

        // === Creazione ===
        Task<IdentityResult> CreateUserAsync(UserCreateViewModel model);

        // === Gestione Profilo ===
        Task<UserProfileViewModel?> GetUserProfileAsync(string userId);
        Task<IdentityResult> UpdateProfileAsync(string userId, UserProfileViewModel model);
        Task<IdentityResult> ChangePasswordAsync(string userId, ChangePasswordViewModel model);

        // === Gestione Utenti (Admin) ===
        Task<List<UserListViewModel>> GetAllUsersAsync();
        Task<List<UserListViewModel>> GetActiveUsersAsync();
        Task<UserEditViewModel?> GetUserForEditAsync(string userId);
        Task<IdentityResult> UpdateUserAsync(UserEditViewModel model);
        Task<IdentityResult> DeleteUserAsync(string userId);
        Task<IdentityResult> ActivateUserAsync(string userId);
        Task<IdentityResult> DeactivateUserAsync(string userId);

        // === Gestione Ruoli ===
        Task<IdentityResult> AssignRoleAsync(string userId, string roleName);
        Task<IdentityResult> RemoveRoleAsync(string userId, string roleName);
        Task<List<string>> GetUserRolesAsync(string userId);
        Task<List<string>> GetAllRolesAsync();

        // === Utility ===
        Task<ApplicationUser?> GetUserByIdAsync(string userId);
        Task<ApplicationUser?> GetUserByIdWithRepartoAsync(string userId);
        Task<ApplicationUser?> GetUserByEmailAsync(string email);
        Task<bool> UserExistsAsync(string userId);
        Task<bool> EmailExistsAsync(string email);

        // === Password Reset ===

        /// <summary>
        /// Genera un token per il reset della password
        /// </summary>
        /// <param name="email">Email dell'utente</param>
        /// <returns>Token di reset se l'utente esiste e non è eliminato, null altrimenti</returns>
        Task<string?> GeneratePasswordResetTokenAsync(string email);

        /// <summary>
        /// Resetta la password usando il token
        /// </summary>
        /// <param name="email">Email dell'utente</param>
        /// <param name="token">Token di reset</param>
        /// <param name="newPassword">Nuova password</param>
        /// <returns>IdentityResult con esito operazione</returns>
        Task<IdentityResult> ResetPasswordAsync(string email, string token, string newPassword);

        /// <summary>
        /// Verifica se un utente può richiedere il reset password
        /// (esiste, non è eliminato, è attivo)
        /// </summary>
        /// <param name="email">Email dell'utente</param>
        /// <returns>True se può richiedere reset</returns>
        Task<bool> CanResetPasswordAsync(string email);
    }
}