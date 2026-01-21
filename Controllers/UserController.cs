using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trasformazioni.Authorization;
using Trasformazioni.Models.ViewModels;
using Trasformazioni.Services.Interfaces;

namespace Trasformazioni.Controllers
{
    /// <summary>
    /// Controller per la gestione degli utenti (solo Amministratori)
    /// </summary>
    [Authorize(Roles = RoleNames.Amministrazione)]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        private readonly IRepartoService _repartoService;

        public UserController(
            IUserService userService,
            ILogger<UserController> logger,
            IRepartoService repartoService)
        {
            _userService = userService;
            _logger = logger;
            _repartoService = repartoService;
        }

        #region List Users

        /// <summary>
        /// GET: /User/Index
        /// Lista di tutti gli utenti
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return View(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento della lista utenti");
                TempData["ErrorMessage"] = "Si è verificato un errore durante il caricamento degli utenti.";
                return View(new List<UserListViewModel>());
            }
        }

        /// <summary>
        /// GET: /User/Active
        /// Lista solo degli utenti attivi
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Active()
        {
            try
            {
                var users = await _userService.GetActiveUsersAsync();
                return View("Index", users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento della lista utenti attivi");
                TempData["ErrorMessage"] = "Si è verificato un errore durante il caricamento degli utenti.";
                return View("Index", new List<UserListViewModel>());
            }
        }

        #endregion

        #region Create User

        /// <summary>
        /// GET: /User/Create
        /// Mostra il form per creare un nuovo utente
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                var allRoles = await _userService.GetAllRolesAsync();

                var model = new UserCreateViewModel
                {
                    DataAssunzione = DateTime.Today,
                    IsAttivo = true,
                    AvailableRoles = allRoles.Select(r => new RoleOption
                    {
                        RoleName = r,
                        IsSelected = false
                    }).ToList(),
                    RepartiSelectList = await _repartoService.GetSelectListAsync()
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento del form di creazione utente");
                TempData["ErrorMessage"] = "Si è verificato un errore durante il caricamento del form.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// POST: /User/Create
        /// Crea un nuovo utente
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Ricarica i ruoli disponibili in caso di errore
                model.AvailableRoles = (await _userService.GetAllRolesAsync())
                    .Select(r => new RoleOption
                    {
                        RoleName = r,
                        IsSelected = model.SelectedRoles.Contains(r)
                    }).ToList();
                model.RepartiSelectList = await _repartoService.GetSelectListAsync(model.RepartoId);

                return View(model);
            }

            try
            {
                // Verifica se l'email esiste già
                if (await _userService.EmailExistsAsync(model.Email))
                {
                    ModelState.AddModelError("Email", "Questa email è già registrata.");

                    // Ricarica i ruoli disponibili
                    model.AvailableRoles = (await _userService.GetAllRolesAsync())
                        .Select(r => new RoleOption
                        {
                            RoleName = r,
                            IsSelected = model.SelectedRoles.Contains(r)
                        }).ToList();
                    model.RepartiSelectList = await _repartoService.GetSelectListAsync(model.RepartoId);

                    return View(model);
                }

                var result = await _userService.CreateUserAsync(model);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Nuovo utente creato: {Email}", model.Email);
                    TempData["SuccessMessage"] = "Utente creato con successo!";
                    return RedirectToAction(nameof(Index));
                }

                // Aggiungi gli errori al ModelState
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                // Ricarica i ruoli disponibili
                model.AvailableRoles = (await _userService.GetAllRolesAsync())
                    .Select(r => new RoleOption
                    {
                        RoleName = r,
                        IsSelected = model.SelectedRoles.Contains(r)
                    }).ToList();
                model.RepartiSelectList = await _repartoService.GetSelectListAsync(model.RepartoId);

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la creazione dell'utente: {Email}", model.Email);
                ModelState.AddModelError(string.Empty, "Si è verificato un errore durante la creazione dell'utente.");

                // Ricarica i ruoli disponibili
                model.AvailableRoles = (await _userService.GetAllRolesAsync())
                    .Select(r => new RoleOption
                    {
                        RoleName = r,
                        IsSelected = model.SelectedRoles.Contains(r)
                    }).ToList();
                model.RepartiSelectList = await _repartoService.GetSelectListAsync(model.RepartoId);

                return View(model);
            }
        }

        #endregion

        #region View User Details

        /// <summary>
        /// GET: /User/Details/{id}
        /// Visualizza i dettagli di un utente
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "ID utente non valido.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                //var user = await _userService.GetUserByIdAsync(id);
                var user = await _userService.GetUserByIdWithRepartoAsync(id);

                if (user == null)
                {
                    _logger.LogWarning("Utente non trovato: {UserId}", id);
                    TempData["ErrorMessage"] = "Utente non trovato.";
                    return RedirectToAction(nameof(Index));
                }

                var roles = await _userService.GetUserRolesAsync(id);

                var viewModel = new UserListViewModel
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
                    Ruoli = roles
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento dei dettagli utente: {UserId}", id);
                TempData["ErrorMessage"] = "Si è verificato un errore durante il caricamento dei dettagli.";
                return RedirectToAction(nameof(Index));
            }
        }

        #endregion

        #region Edit User

        /// <summary>
        /// GET: /User/Edit/{id}
        /// Mostra il form di modifica utente
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "ID utente non valido.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var user = await _userService.GetUserForEditAsync(id);

                if (user == null)
                {
                    _logger.LogWarning("Utente non trovato per modifica: {UserId}", id);
                    TempData["ErrorMessage"] = "Utente non trovato.";
                    return RedirectToAction(nameof(Index));
                }

                user.RepartiSelectList = await _repartoService.GetSelectListAsync(user.RepartoId);

                return View(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento del form di modifica: {UserId}", id);
                TempData["ErrorMessage"] = "Si è verificato un errore durante il caricamento del form.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// POST: /User/Edit/{id}
        /// Salva le modifiche all'utente
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Ricarica i ruoli disponibili in caso di errore
                model.AvailableRoles = (await _userService.GetAllRolesAsync())
                    .Select(r => new RoleOption
                    {
                        RoleName = r,
                        IsSelected = model.SelectedRoles.Contains(r)
                    }).ToList();
                model.RepartiSelectList = await _repartoService.GetSelectListAsync(model.RepartoId);

                return View(model);
            }

            try
            {
                var result = await _userService.UpdateUserAsync(model);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Utente aggiornato con successo: {UserId}", model.Id);
                    TempData["SuccessMessage"] = "Utente aggiornato con successo!";
                    return RedirectToAction(nameof(Details), new { id = model.Id });
                }

                // Aggiungi gli errori al ModelState
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                // Ricarica i ruoli disponibili
                model.AvailableRoles = (await _userService.GetAllRolesAsync())
                    .Select(r => new RoleOption
                    {
                        RoleName = r,
                        IsSelected = model.SelectedRoles.Contains(r)
                    }).ToList();
                model.RepartiSelectList = await _repartoService.GetSelectListAsync(model.RepartoId);

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'aggiornamento dell'utente: {UserId}", model.Id);
                ModelState.AddModelError(string.Empty, "Si è verificato un errore durante l'aggiornamento.");

                model.RepartiSelectList = await _repartoService.GetSelectListAsync(model.RepartoId);

                return View(model);
            }
        }

        #endregion

        #region Activate/Deactivate User

        /// <summary>
        /// POST: /User/Activate/{id}
        /// Attiva un utente
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "ID utente non valido.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var result = await _userService.ActivateUserAsync(id);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Utente attivato: {UserId}", id);
                    TempData["SuccessMessage"] = "Utente attivato con successo!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Errore durante l'attivazione dell'utente.";
                }

                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'attivazione dell'utente: {UserId}", id);
                TempData["ErrorMessage"] = "Si è verificato un errore durante l'attivazione.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        /// <summary>
        /// POST: /User/Deactivate/{id}
        /// Disattiva un utente
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "ID utente non valido.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var result = await _userService.DeactivateUserAsync(id);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Utente disattivato: {UserId}", id);
                    TempData["SuccessMessage"] = "Utente disattivato con successo!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Errore durante la disattivazione dell'utente.";
                }

                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la disattivazione dell'utente: {UserId}", id);
                TempData["ErrorMessage"] = "Si è verificato un errore durante la disattivazione.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        #endregion

        #region Delete User

        /// <summary>
        /// POST: /User/Delete/{id}
        /// Elimina un utente (soft delete)
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "ID utente non valido.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // Verifica che l'admin non stia eliminando se stesso
                var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (currentUserId == id)
                {
                    TempData["ErrorMessage"] = "Non puoi eliminare il tuo stesso account.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                var result = await _userService.DeleteUserAsync(id);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Utente eliminato: {UserId}", id);
                    TempData["SuccessMessage"] = "Utente eliminato con successo!";
                    return RedirectToAction(nameof(Index));
                }

                TempData["ErrorMessage"] = "Errore durante l'eliminazione dell'utente.";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'eliminazione dell'utente: {UserId}", id);
                TempData["ErrorMessage"] = "Si è verificato un errore durante l'eliminazione.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        #endregion

        #region Manage Roles

        /// <summary>
        /// POST: /User/AssignRole
        /// Assegna un ruolo a un utente
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRole(string userId, string roleName)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(roleName))
            {
                TempData["ErrorMessage"] = "Parametri non validi.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var result = await _userService.AssignRoleAsync(userId, roleName);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Ruolo {RoleName} assegnato a userId: {UserId}", roleName, userId);
                    TempData["SuccessMessage"] = $"Ruolo '{roleName}' assegnato con successo!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Errore durante l'assegnazione del ruolo.";
                }

                return RedirectToAction(nameof(Details), new { id = userId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'assegnazione del ruolo {RoleName} a userId: {UserId}", roleName, userId);
                TempData["ErrorMessage"] = "Si è verificato un errore durante l'assegnazione del ruolo.";
                return RedirectToAction(nameof(Details), new { id = userId });
            }
        }

        /// <summary>
        /// POST: /User/RemoveRole
        /// Rimuove un ruolo da un utente
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveRole(string userId, string roleName)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(roleName))
            {
                TempData["ErrorMessage"] = "Parametri non validi.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var result = await _userService.RemoveRoleAsync(userId, roleName);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Ruolo {RoleName} rimosso da userId: {UserId}", roleName, userId);
                    TempData["SuccessMessage"] = $"Ruolo '{roleName}' rimosso con successo!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Errore durante la rimozione del ruolo.";
                }

                return RedirectToAction(nameof(Details), new { id = userId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la rimozione del ruolo {RoleName} da userId: {UserId}", roleName, userId);
                TempData["ErrorMessage"] = "Si è verificato un errore durante la rimozione del ruolo.";
                return RedirectToAction(nameof(Details), new { id = userId });
            }
        }

        #endregion
    }
}