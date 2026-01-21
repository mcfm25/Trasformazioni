using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;
using Trasformazioni.Models.ViewModels;
using Trasformazioni.Services.Interfaces;

namespace Trasformazioni.Controllers
{
    /// <summary>
    /// Controller per la configurazione delle notifiche email
    /// </summary>
    [Authorize]
    public class NotificheConfigController : Controller
    {
        private readonly INotificaEmailConfigService _notificaConfigService;
        private readonly IRepartoService _repartoService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<NotificheConfigController> _logger;

        public NotificheConfigController(
            INotificaEmailConfigService notificaConfigService,
            IRepartoService repartoService,
            UserManager<ApplicationUser> userManager,
            ILogger<NotificheConfigController> logger)
        {
            _notificaConfigService = notificaConfigService;
            _repartoService = repartoService;
            _userManager = userManager;
            _logger = logger;
        }

        private string GetCurrentUserId() =>
            User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "";

        // ===================================
        // INDEX
        // ===================================

        /// <summary>
        /// GET: /NotificheConfig
        /// Lista tutte le configurazioni raggruppate per modulo
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var configurazioniPerModulo = await _notificaConfigService.GetConfigurazioniGroupedByModuloAsync();

            var model = new NotificheConfigIndexViewModel
            {
                ConfigurazioniPerModulo = configurazioniPerModulo
            };

            return View(model);
        }

        // ===================================
        // DETAILS
        // ===================================

        /// <summary>
        /// GET: /NotificheConfig/Details/{id}
        /// Mostra i dettagli di una configurazione con i suoi destinatari
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var model = await _notificaConfigService.GetConfigurazioneAsync(id);

            if (model == null)
            {
                TempData["ErrorMessage"] = "Configurazione non trovata.";
                return RedirectToAction(nameof(Index));
            }

            // Prepara i dati per il modal di aggiunta destinatario
            await PrepareDestinatarioDropdownsAsync(id);

            return View(model);
        }

        // ===================================
        // EDIT
        // ===================================

        /// <summary>
        /// GET: /NotificheConfig/Edit/{id}
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var configurazione = await _notificaConfigService.GetConfigurazioneAsync(id);

            if (configurazione == null)
            {
                TempData["ErrorMessage"] = "Configurazione non trovata.";
                return RedirectToAction(nameof(Index));
            }

            var model = new ConfigurazioneNotificaEmailEditViewModel
            {
                Id = configurazione.Id,
                Codice = configurazione.Codice,
                Descrizione = configurazione.Descrizione,
                Modulo = configurazione.Modulo,
                IsAttiva = configurazione.IsAttiva,
                OggettoEmailDefault = configurazione.OggettoEmailDefault,
                Note = configurazione.Note
            };

            return View(model);
        }

        /// <summary>
        /// POST: /NotificheConfig/Edit/{id}
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ConfigurazioneNotificaEmailEditViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            var (success, error) = await _notificaConfigService.UpdateConfigurazioneAsync(model, GetCurrentUserId());

            if (!success)
            {
                ModelState.AddModelError(string.Empty, error ?? "Errore durante il salvataggio.");
                return View(model);
            }

            TempData["SuccessMessage"] = "Configurazione aggiornata con successo!";
            return RedirectToAction(nameof(Details), new { id });
        }

        // ===================================
        // TOGGLE ATTIVA
        // ===================================

        /// <summary>
        /// POST: /NotificheConfig/ToggleAttiva/{id}
        /// Attiva/disattiva una configurazione
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleAttiva(Guid id, string? returnUrl = null)
        {
            var (success, error) = await _notificaConfigService.ToggleAttivaAsync(id, GetCurrentUserId());

            if (!success)
            {
                TempData["ErrorMessage"] = error ?? "Errore durante l'operazione.";
            }
            else
            {
                TempData["SuccessMessage"] = "Stato notifica aggiornato!";
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction(nameof(Index));
        }

        // ===================================
        // ADD DESTINATARIO
        // ===================================

        /// <summary>
        /// GET: /NotificheConfig/AddDestinatario/{configurazioneId}
        /// Mostra il form per aggiungere un destinatario
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> AddDestinatario(Guid configurazioneId)
        {
            var configurazione = await _notificaConfigService.GetConfigurazioneAsync(configurazioneId);

            if (configurazione == null)
            {
                TempData["ErrorMessage"] = "Configurazione non trovata.";
                return RedirectToAction(nameof(Index));
            }

            var model = new DestinatarioNotificaEmailCreateViewModel
            {
                ConfigurazioneNotificaEmailId = configurazioneId
            };

            await PopulateDestinatarioDropdownsAsync(model);

            ViewBag.ConfigurazioneCodice = configurazione.Codice;
            ViewBag.ConfigurazioneDescrizione = configurazione.Descrizione;

            return View(model);
        }

        /// <summary>
        /// POST: /NotificheConfig/AddDestinatario
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDestinatario(DestinatarioNotificaEmailCreateViewModel model)
        {
            // Rimuovi validazione per campi non pertinenti al tipo selezionato
            ClearIrrelevantValidation(model);

            if (!ModelState.IsValid)
            {
                await PopulateDestinatarioDropdownsAsync(model);
                var config = await _notificaConfigService.GetConfigurazioneAsync(model.ConfigurazioneNotificaEmailId);
                ViewBag.ConfigurazioneCodice = config?.Codice;
                ViewBag.ConfigurazioneDescrizione = config?.Descrizione;
                return View(model);
            }

            var (success, error, _) = await _notificaConfigService.AddDestinatarioAsync(model, GetCurrentUserId());

            if (!success)
            {
                ModelState.AddModelError(string.Empty, error ?? "Errore durante l'aggiunta.");
                await PopulateDestinatarioDropdownsAsync(model);
                var config = await _notificaConfigService.GetConfigurazioneAsync(model.ConfigurazioneNotificaEmailId);
                ViewBag.ConfigurazioneCodice = config?.Codice;
                ViewBag.ConfigurazioneDescrizione = config?.Descrizione;
                return View(model);
            }

            TempData["SuccessMessage"] = "Destinatario aggiunto con successo!";
            return RedirectToAction(nameof(Details), new { id = model.ConfigurazioneNotificaEmailId });
        }

        /// <summary>
        /// Rimuove la validazione per i campi non pertinenti al tipo selezionato
        /// </summary>
        private void ClearIrrelevantValidation(DestinatarioNotificaEmailCreateViewModel model)
        {
            switch (model.Tipo)
            {
                case TipoDestinatarioNotifica.Reparto:
                    ModelState.Remove(nameof(model.Ruolo));
                    ModelState.Remove(nameof(model.UtenteId));
                    break;
                case TipoDestinatarioNotifica.Ruolo:
                    ModelState.Remove(nameof(model.RepartoId));
                    ModelState.Remove(nameof(model.UtenteId));
                    break;
                case TipoDestinatarioNotifica.Utente:
                    ModelState.Remove(nameof(model.RepartoId));
                    ModelState.Remove(nameof(model.Ruolo));
                    break;
            }
        }

        // ===================================
        // REMOVE DESTINATARIO
        // ===================================

        /// <summary>
        /// POST: /NotificheConfig/RemoveDestinatario/{id}
        /// Rimuove un destinatario
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveDestinatario(Guid id, Guid configurazioneId)
        {
            var (success, error) = await _notificaConfigService.RemoveDestinatarioAsync(id, GetCurrentUserId());

            if (!success)
            {
                TempData["ErrorMessage"] = error ?? "Errore durante la rimozione.";
            }
            else
            {
                TempData["SuccessMessage"] = "Destinatario rimosso con successo!";
            }

            return RedirectToAction(nameof(Details), new { id = configurazioneId });
        }

        // ===================================
        // HELPERS
        // ===================================

        /// <summary>
        /// Prepara i dropdown per il ViewBag (usato nel Details per il modal)
        /// </summary>
        private async Task PrepareDestinatarioDropdownsAsync(Guid configurazioneId)
        {
            ViewBag.ConfigurazioneId = configurazioneId;
            ViewBag.RepartiSelectList = await _repartoService.GetSelectListAsync();
            ViewBag.RuoliSelectList = new SelectList(await _notificaConfigService.GetAllRuoliAsync());
            ViewBag.UtentiSelectList = new SelectList(
                await _userManager.Users
                    .Where(u => u.IsAttivo && !u.IsDeleted)
                    .OrderBy(u => u.Cognome)
                    .ThenBy(u => u.Nome)
                    .Select(u => new { u.Id, Nome = u.Cognome + " " + u.Nome + " (" + u.Email + ")" })
                    .ToListAsync(),
                "Id", "Nome");
        }

        /// <summary>
        /// Popola i dropdown nel model
        /// </summary>
        private async Task PopulateDestinatarioDropdownsAsync(DestinatarioNotificaEmailCreateViewModel model)
        {
            model.RepartiSelectList = await _repartoService.GetSelectListAsync(model.RepartoId);
            model.RuoliSelectList = new SelectList(
                await _notificaConfigService.GetAllRuoliAsync(),
                model.Ruolo);
            model.UtentiSelectList = new SelectList(
                await _userManager.Users
                    .Where(u => u.IsAttivo && !u.IsDeleted)
                    .OrderBy(u => u.Cognome)
                    .ThenBy(u => u.Nome)
                    .Select(u => new { u.Id, Nome = u.Cognome + " " + u.Nome + " (" + u.Email + ")" })
                    .ToListAsync(),
                "Id", "Nome", model.UtenteId);
        }
    }
}