using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Trasformazioni.Models.Entities;
using Trasformazioni.Models.ViewModels;
using Trasformazioni.Services.Interfaces;

namespace Trasformazioni.Services
{
    /// <summary>
    /// Servizio per la gestione completa degli utenti
    /// </summary>
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<UserService> _logger;

        public UserService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<UserService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        #region Autenticazione

        public async Task<SignInResult> LoginAsync(LoginViewModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    _logger.LogWarning("Tentativo di login con email non esistente: {Email}", model.Email);
                    return SignInResult.Failed;
                }

                if (!user.IsAttivo)
                {
                    _logger.LogWarning("Tentativo di login con utente disattivato: {Email}", model.Email);
                    return SignInResult.NotAllowed;
                }

                var result = await _signInManager.PasswordSignInAsync(
                    user.UserName!,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Login effettuato con successo per: {Email}", model.Email);
                }
                else if (result.IsLockedOut)
                {
                    _logger.LogWarning("Account bloccato per troppi tentativi: {Email}", model.Email);
                }
                else
                {
                    _logger.LogWarning("Login fallito per: {Email}", model.Email);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il login per: {Email}", model.Email);
                throw;
            }
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("Logout effettuato");
        }

        #endregion

        #region Registrazione

        public async Task<IdentityResult> RegisterAsync(RegisterViewModel model)
        {
            try
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Nome = model.Nome,
                    Cognome = model.Cognome,
                    //Reparto = model.Reparto,
                    RepartoId = model.RepartoId,
                    DataAssunzione = model.DataAssunzione,
                    IsAttivo = true,
                    EmailConfirmed = false // Impostare a true se non si vuole conferma email
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Nuovo utente registrato: {Email}", model.Email);

                    // Assegna un ruolo di default (opzionale)
                    // await _userManager.AddToRoleAsync(user, RoleNames.DataEntry);
                }
                else
                {
                    _logger.LogWarning("Registrazione fallita per: {Email}. Errori: {Errors}",
                        model.Email,
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la registrazione di: {Email}", model.Email);
                throw;
            }
        }

        #endregion

        #region Create User (Admin)

        public async Task<IdentityResult> CreateUserAsync(UserCreateViewModel model)
        {
            try
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Nome = model.Nome,
                    Cognome = model.Cognome,
                    PhoneNumber = model.PhoneNumber,
                    //Reparto = model.Reparto,
                    RepartoId = model.RepartoId,
                    DataAssunzione = model.DataAssunzione,
                    IsAttivo = model.IsAttivo,
                    EmailConfirmed = true // Admin crea utenti già confermati
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                {
                    _logger.LogWarning("Creazione utente fallita per: {Email}. Errori: {Errors}",
                        model.Email,
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                    return result;
                }

                // Assegna i ruoli selezionati
                if (model.SelectedRoles.Any())
                {
                    var roleResult = await _userManager.AddToRolesAsync(user, model.SelectedRoles);
                    if (!roleResult.Succeeded)
                    {
                        _logger.LogWarning("Assegnazione ruoli fallita per: {Email}", model.Email);
                        // Non ritornare errore, l'utente è stato creato
                    }
                }

                _logger.LogInformation("Nuovo utente creato da admin: {Email}", model.Email);
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la creazione dell'utente: {Email}", model.Email);
                throw;
            }
        }

        #endregion

        #region Gestione Profilo

        public async Task<UserProfileViewModel?> GetUserProfileAsync(string userId)
        {
            try
            {
                //var user = await _userManager.FindByIdAsync(userId);
                var user = await _userManager.Users
                    .Include(u => u.Reparto)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                    return null;

                return new UserProfileViewModel
                {
                    Id = user.Id,
                    Nome = user.Nome,
                    Cognome = user.Cognome,
                    Email = user.Email!,
                    PhoneNumber = user.PhoneNumber,
                    //Reparto = user.Reparto,
                    RepartoId = user.RepartoId,
                    RepartoNome = user.Reparto?.Nome,
                    DataAssunzione = user.DataAssunzione
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero del profilo per userId: {UserId}", userId);
                throw;
            }
        }

        public async Task<IdentityResult> UpdateProfileAsync(string userId, UserProfileViewModel model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return IdentityResult.Failed(new IdentityError
                    {
                        Description = "Utente non trovato"
                    });
                }

                user.Nome = model.Nome;
                user.Cognome = model.Cognome;
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;
                //user.Reparto = model.Reparto;
                user.RepartoId = model.RepartoId;
                user.DataAssunzione = model.DataAssunzione;

                // Se l'email è cambiata, aggiorna anche lo username
                if (user.UserName != model.Email)
                {
                    user.UserName = model.Email;
                    user.EmailConfirmed = false; // Richiedere conferma per la nuova email
                }

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Profilo aggiornato per userId: {UserId}", userId);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'aggiornamento del profilo per userId: {UserId}", userId);
                throw;
            }
        }

        public async Task<IdentityResult> ChangePasswordAsync(string userId, ChangePasswordViewModel model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return IdentityResult.Failed(new IdentityError
                    {
                        Description = "Utente non trovato"
                    });
                }

                var result = await _userManager.ChangePasswordAsync(
                    user,
                    model.CurrentPassword,
                    model.NewPassword);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Password cambiata per userId: {UserId}", userId);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il cambio password per userId: {UserId}", userId);
                throw;
            }
        }

        #endregion

        #region Gestione Utenti (Admin)

        public async Task<List<UserListViewModel>> GetAllUsersAsync()
        {
            try
            {
                var users = await _userManager.Users
                    .Include(u => u.Reparto)
                    .Where(u => !u.IsDeleted)
                    .OrderBy(u => u.Cognome)
                    .ThenBy(u => u.Nome)
                    .ToListAsync();

                var userList = new List<UserListViewModel>();

                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);

                    userList.Add(new UserListViewModel
                    {
                        Id = user.Id,
                        Nome = user.Nome,
                        Cognome = user.Cognome,
                        Email = user.Email!,
                        //Reparto = user.Reparto,
                        RepartoId = user.RepartoId,
                        RepartoNome = user.Reparto?.Nome,
                        DataAssunzione = user.DataAssunzione,
                        IsAttivo = user.IsAttivo,
                        Ruoli = roles.ToList()
                    });
                }

                return userList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero di tutti gli utenti");
                throw;
            }
        }

        public async Task<List<UserListViewModel>> GetActiveUsersAsync()
        {
            try
            {
                var users = await _userManager.Users
                    .Include(u => u.Reparto)
                    .Where(u => !u.IsDeleted && u.IsAttivo)
                    .OrderBy(u => u.Cognome)
                    .ThenBy(u => u.Nome)
                    .ToListAsync();

                var userList = new List<UserListViewModel>();

                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);

                    userList.Add(new UserListViewModel
                    {
                        Id = user.Id,
                        Nome = user.Nome,
                        Cognome = user.Cognome,
                        Email = user.Email!,
                        //Reparto = user.Reparto,
                        RepartoId = user.RepartoId,
                        RepartoNome = user.Reparto?.Nome,
                        DataAssunzione = user.DataAssunzione,
                        IsAttivo = user.IsAttivo,
                        Ruoli = roles.ToList()
                    });
                }

                return userList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero degli utenti attivi");
                throw;
            }
        }

        public async Task<UserEditViewModel?> GetUserForEditAsync(string userId)
        {
            try
            {
                //var user = await _userManager.FindByIdAsync(userId);
                var user = await _userManager.Users
                    .Include(u => u.Reparto)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                    return null;

                var userRoles = await _userManager.GetRolesAsync(user);
                var allRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();

                var model = new UserEditViewModel
                {
                    Id = user.Id,
                    Nome = user.Nome,
                    Cognome = user.Cognome,
                    Email = user.Email!,
                    PhoneNumber = user.PhoneNumber,
                    //Reparto = user.Reparto,
                    RepartoId = user.RepartoId,
                    DataAssunzione = user.DataAssunzione,
                    IsAttivo = user.IsAttivo,
                    SelectedRoles = userRoles.ToList(),
                    AvailableRoles = allRoles.Select(r => new RoleOption
                    {
                        RoleName = r!,
                        IsSelected = userRoles.Contains(r!)
                    }).ToList()
                };

                return model;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dell'utente per modifica: {UserId}", userId);
                throw;
            }
        }

        public async Task<IdentityResult> UpdateUserAsync(UserEditViewModel model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(model.Id);

                if (user == null)
                {
                    return IdentityResult.Failed(new IdentityError
                    {
                        Description = "Utente non trovato"
                    });
                }

                // Aggiorna i dati dell'utente
                user.Nome = model.Nome;
                user.Cognome = model.Cognome;
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;
                //user.Reparto = model.Reparto;
                user.RepartoId = model.RepartoId;
                user.DataAssunzione = model.DataAssunzione;
                user.IsAttivo = model.IsAttivo;

                if (user.UserName != model.Email)
                {
                    user.UserName = model.Email;
                }

                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                    return result;

                // Aggiorna i ruoli
                var currentRoles = await _userManager.GetRolesAsync(user);
                var rolesToRemove = currentRoles.Except(model.SelectedRoles).ToList();
                var rolesToAdd = model.SelectedRoles.Except(currentRoles).ToList();

                if (rolesToRemove.Any())
                {
                    var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                    if (!removeResult.Succeeded)
                        return removeResult;
                }

                if (rolesToAdd.Any())
                {
                    var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
                    if (!addResult.Succeeded)
                        return addResult;
                }

                _logger.LogInformation("Utente aggiornato: {UserId}", model.Id);
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'aggiornamento dell'utente: {UserId}", model.Id);
                throw;
            }
        }

        public async Task<IdentityResult> DeleteUserAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return IdentityResult.Failed(new IdentityError
                    {
                        Description = "Utente non trovato"
                    });
                }

                // Soft delete
                user.IsDeleted = true;
                user.DeletedAt = DateTime.Now;
                user.IsAttivo = false;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Utente eliminato (soft delete): {UserId}", userId);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'eliminazione dell'utente: {UserId}", userId);
                throw;
            }
        }

        public async Task<IdentityResult> ActivateUserAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return IdentityResult.Failed(new IdentityError
                    {
                        Description = "Utente non trovato"
                    });
                }

                user.IsAttivo = true;
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Utente attivato: {UserId}", userId);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'attivazione dell'utente: {UserId}", userId);
                throw;
            }
        }

        public async Task<IdentityResult> DeactivateUserAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return IdentityResult.Failed(new IdentityError
                    {
                        Description = "Utente non trovato"
                    });
                }

                user.IsAttivo = false;
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Utente disattivato: {UserId}", userId);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la disattivazione dell'utente: {UserId}", userId);
                throw;
            }
        }

        #endregion

        #region Gestione Ruoli

        public async Task<IdentityResult> AssignRoleAsync(string userId, string roleName)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return IdentityResult.Failed(new IdentityError
                    {
                        Description = "Utente non trovato"
                    });
                }

                var result = await _userManager.AddToRoleAsync(user, roleName);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Ruolo {RoleName} assegnato a userId: {UserId}", roleName, userId);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'assegnazione del ruolo {RoleName} a userId: {UserId}", roleName, userId);
                throw;
            }
        }

        public async Task<IdentityResult> RemoveRoleAsync(string userId, string roleName)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return IdentityResult.Failed(new IdentityError
                    {
                        Description = "Utente non trovato"
                    });
                }

                var result = await _userManager.RemoveFromRoleAsync(user, roleName);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Ruolo {RoleName} rimosso da userId: {UserId}", roleName, userId);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la rimozione del ruolo {RoleName} da userId: {UserId}", roleName, userId);
                throw;
            }
        }

        public async Task<List<string>> GetUserRolesAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                    return new List<string>();

                var roles = await _userManager.GetRolesAsync(user);
                return roles.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei ruoli per userId: {UserId}", userId);
                throw;
            }
        }

        public async Task<List<string>> GetAllRolesAsync()
        {
            try
            {
                return await _roleManager.Roles
                    .Select(r => r.Name!)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero di tutti i ruoli");
                throw;
            }
        }

        #endregion

        #region Utility

        public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
        {
            try
            {
                return await _userManager.FindByIdAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dell'utente per userId: {UserId}", userId);
                throw;
            }
        }

        public async Task<ApplicationUser?> GetUserByIdWithRepartoAsync(string userId)
        {
            return await _userManager.Users
                .Include(u => u.Reparto)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
        {
            try
            {
                return await _userManager.FindByEmailAsync(email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dell'utente per email: {Email}", email);
                throw;
            }
        }

        public async Task<bool> UserExistsAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                return user != null && !user.IsDeleted;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la verifica esistenza utente: {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                return user != null && !user.IsDeleted;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la verifica esistenza email: {Email}", email);
                throw;
            }
        }

        #endregion

        #region Password Reset

        public async Task<string?> GeneratePasswordResetTokenAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);

                // Verifica che l'utente esista e non sia eliminato
                if (user == null || user.IsDeleted)
                {
                    _logger.LogWarning(
                        "Tentativo di reset password per email non valida o utente eliminato: {Email}",
                        email);
                    return null;
                }

                // Verifica che l'utente sia attivo
                if (!user.IsAttivo)
                {
                    _logger.LogWarning(
                        "Tentativo di reset password per utente disattivato: {Email}",
                        email);
                    return null;
                }

                // Genera il token
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                _logger.LogInformation(
                    "Token di reset password generato per: {Email}",
                    email);

                return token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Errore durante la generazione del token di reset per: {Email}",
                    email);
                throw;
            }
        }

        public async Task<IdentityResult> ResetPasswordAsync(string email, string token, string newPassword)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);

                if (user == null || user.IsDeleted)
                {
                    _logger.LogWarning(
                        "Tentativo di reset password con email non valida: {Email}",
                        email);
                    return IdentityResult.Failed(new IdentityError
                    {
                        Code = "InvalidEmail",
                        Description = "Email non valida o utente non trovato."
                    });
                }

                if (!user.IsAttivo)
                {
                    _logger.LogWarning(
                        "Tentativo di reset password per utente disattivato: {Email}",
                        email);
                    return IdentityResult.Failed(new IdentityError
                    {
                        Code = "UserDisabled",
                        Description = "L'account è disattivato. Contatta l'amministratore."
                    });
                }

                var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

                if (result.Succeeded)
                {
                    _logger.LogInformation(
                        "Password resettata con successo per: {Email}",
                        email);
                }
                else
                {
                    _logger.LogWarning(
                        "Reset password fallito per: {Email}. Errori: {Errors}",
                        email,
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Errore durante il reset della password per: {Email}",
                    email);
                throw;
            }
        }

        public async Task<bool> CanResetPasswordAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);

                // Può resettare solo se: esiste, non è eliminato, è attivo
                return user != null && !user.IsDeleted && user.IsAttivo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Errore durante la verifica CanResetPassword per: {Email}",
                    email);
                return false;
            }
        }

        #endregion

    }
}