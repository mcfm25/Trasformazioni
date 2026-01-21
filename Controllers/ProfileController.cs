using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Trasformazioni.Models.ViewModels;
using Trasformazioni.Services.Interfaces;

namespace Trasformazioni.Controllers
{
    /// <summary>
    /// Controller per la gestione del profilo personale dell'utente autenticato
    /// </summary>
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<ProfileController> _logger;
        private readonly IRepartoService _repartoService;

        public ProfileController(
            IUserService userService,
            ILogger<ProfileController> logger,
            IRepartoService repartoService)
        {
            _userService = userService;
            _logger = logger;
            _repartoService = repartoService;
        }

        #region View Profile

        /// <summary>
        /// GET: /Profile/Index
        /// Visualizza il profilo dell'utente corrente
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("UserId non trovato nei claims");
                    return RedirectToAction("Login", "Account");
                }

                var profile = await _userService.GetUserProfileAsync(userId);

                if (profile == null)
                {
                    _logger.LogWarning("Profilo non trovato per userId: {UserId}", userId);
                    TempData["ErrorMessage"] = "Profilo non trovato.";
                    return RedirectToAction("Index", "Home");
                }

                return View(profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento del profilo");
                TempData["ErrorMessage"] = "Si è verificato un errore durante il caricamento del profilo.";
                return RedirectToAction("Index", "Home");
            }
        }

        #endregion

        #region Edit Profile

        /// <summary>
        /// GET: /Profile/Edit
        /// Mostra il form di modifica profilo
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("UserId non trovato nei claims");
                    return RedirectToAction("Login", "Account");
                }

                var profile = await _userService.GetUserProfileAsync(userId);

                if (profile == null)
                {
                    _logger.LogWarning("Profilo non trovato per userId: {UserId}", userId);
                    TempData["ErrorMessage"] = "Profilo non trovato.";
                    return RedirectToAction("Index");
                }

                // Popola dropdown reparti
                profile.RepartiSelectList = await _repartoService.GetSelectListAsync(profile.RepartoId);

                return View(profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento del form di modifica profilo");
                TempData["ErrorMessage"] = "Si è verificato un errore durante il caricamento del profilo.";
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// POST: /Profile/Edit
        /// Salva le modifiche al profilo
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("UserId non trovato nei claims");
                    return RedirectToAction("Login", "Account");
                }

                // Verifica che l'utente stia modificando il proprio profilo
                if (model.Id != userId)
                {
                    _logger.LogWarning("Tentativo di modifica profilo non autorizzato. UserId: {UserId}, ProfileId: {ProfileId}",
                        userId, model.Id);
                    TempData["ErrorMessage"] = "Non sei autorizzato a modificare questo profilo.";
                    return RedirectToAction("Index");
                }

                var result = await _userService.UpdateProfileAsync(userId, model);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Profilo aggiornato con successo per userId: {UserId}", userId);
                    TempData["SuccessMessage"] = "Profilo aggiornato con successo!";
                    return RedirectToAction("Index");
                }

                // Aggiungi gli errori al ModelState
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                model.RepartiSelectList = await _repartoService.GetSelectListAsync(model.RepartoId);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'aggiornamento del profilo");
                ModelState.AddModelError(string.Empty, "Si è verificato un errore durante l'aggiornamento del profilo.");
                model.RepartiSelectList = await _repartoService.GetSelectListAsync(model.RepartoId);
                return View(model);
            }
        }

        #endregion

        #region Change Password

        /// <summary>
        /// GET: /Profile/ChangePassword
        /// Mostra il form per il cambio password
        /// </summary>
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        /// <summary>
        /// POST: /Profile/ChangePassword
        /// Elabora il cambio password
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("UserId non trovato nei claims");
                    return RedirectToAction("Login", "Account");
                }

                var result = await _userService.ChangePasswordAsync(userId, model);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Password cambiata con successo per userId: {UserId}", userId);
                    TempData["SuccessMessage"] = "Password cambiata con successo!";
                    return RedirectToAction("Index");
                }

                // Aggiungi gli errori al ModelState
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il cambio password");
                ModelState.AddModelError(string.Empty, "Si è verificato un errore durante il cambio password.");
                return View(model);
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Ottiene l'ID dell'utente corrente dai claims
        /// </summary>
        private string? GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        /// <summary>
        /// Ottiene l'email dell'utente corrente dai claims
        /// </summary>
        private string? GetCurrentUserEmail()
        {
            return User.FindFirstValue(ClaimTypes.Email);
        }

        #endregion
    }
}