using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Trasformazioni.Authorization;
using Trasformazioni.Models.Entities;
using Trasformazioni.Models.ViewModels;
using Trasformazioni.Services;

namespace Trasformazioni.Controllers
{
    /// <summary>
    /// Controller per la gestione delle assegnazioni mezzi
    /// </summary>
    [Authorize]
    public class AssegnazioniController : Controller
    {
        private readonly IAssegnazioneMezzoService _assegnazioneService;
        private readonly IMezzoService _mezzoService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AssegnazioniController> _logger;

        public AssegnazioniController(
            IAssegnazioneMezzoService assegnazioneService,
            IMezzoService mezzoService,
            UserManager<ApplicationUser> userManager,
            ILogger<AssegnazioniController> logger)
        {
            _assegnazioneService = assegnazioneService;
            _mezzoService = mezzoService;
            _userManager = userManager;
            _logger = logger;
        }

        #region Helper Methods

        /// <summary>
        /// Verifica se l'utente corrente è Admin o UfficioGare
        /// </summary>
        private bool IsAdmin()
        {
            return User.IsInRole(RoleNames.Amministrazione) || User.IsInRole(RoleNames.UfficioGare);
        }

        /// <summary>
        /// Ottiene l'ID dell'utente corrente
        /// </summary>
        private string GetCurrentUserId()
        {
            return _userManager.GetUserId(User) ?? string.Empty;
        }

        /// <summary>
        /// Ottiene l'utente corrente
        /// </summary>
        private async Task<ApplicationUser?> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(User);
        }

        /// <summary>
        /// Popola la dropdown degli utenti (solo per Admin)
        /// </summary>
        private async Task<SelectList> GetUtentiSelectListAsync(string? selectedValue = null)
        {
            var utenti = await _userManager.Users
                .Where(u => u.IsAttivo && !u.IsDeleted)
                .OrderBy(u => u.Cognome)
                .ThenBy(u => u.Nome)
                .ToListAsync();

            return new SelectList(
                utenti.Select(u => new
                {
                    Id = u.Id,
                    NomeCompleto = $"{u.Cognome} {u.Nome} ({u.Email})"
                }),
                "Id",
                "NomeCompleto",
                selectedValue
            );
        }

        #endregion

        #region Assegna Mezzo (Create)

        /// <summary>
        /// GET: /Assegnazioni/Assegna/{mezzoId}
        /// Form per assegnare/prenotare un mezzo
        /// Include calendario periodi occupati
        /// </summary>
        [HttpGet]
        [Authorize(Roles = $"{RoleNames.Amministrazione},{RoleNames.UfficioGare},{RoleNames.DataEntry}")]
        public async Task<IActionResult> Assegna(Guid mezzoId)
        {
            // Verifica che il mezzo esista
            var mezzo = await _mezzoService.GetByIdAsync(mezzoId);
            if (mezzo == null)
            {
                _logger.LogWarning("Tentativo di assegnare mezzo non esistente: {MezzoId}", mezzoId);
                return NotFound();
            }

            var currentUserId = GetCurrentUserId();
            var isAdmin = IsAdmin();

            var model = new AssegnazioneMezzoCreateViewModel
            {
                MezzoId = mezzoId,
                MezzoTarga = mezzo.Targa,
                MezzoDescrizioneCompleta = mezzo.DescrizioneCompleta,
                DataInizio = DateTime.Now, // Con ore/minuti
                DataFine = null, // Opzionale - lasciamo vuoto di default
                UtenteId = isAdmin ? string.Empty : currentUserId
            };

            // Popola dropdown utenti solo per Admin
            if (isAdmin)
            {
                ViewBag.Utenti = await GetUtentiSelectListAsync();
            }
            else
            {
                // Per DataEntry, nascondi il campo utente (sarà readonly)
                ViewBag.IsDataEntry = true;
                var currentUser = await GetCurrentUserAsync();
                ViewBag.CurrentUserName = currentUser?.NomeCompleto ?? currentUserId;
            }

            // NUOVO: Passa i periodi occupati alla view per il calendario
            var periodiOccupati = await _assegnazioneService.GetPeriodiOccupatiAsync(mezzoId);
            ViewBag.PeriodiOccupati = periodiOccupati;

            // Info aggiuntiva per UX
            ViewBag.HasPrenotazioniAttive = periodiOccupati.Any();

            return View(model);
        }

        /// <summary>
        /// POST: /Assegnazioni/Assegna
        /// Salva nuova assegnazione/prenotazione
        /// Validazione sovrapposizione periodi
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{RoleNames.Amministrazione},{RoleNames.UfficioGare},{RoleNames.DataEntry}")]
        public async Task<IActionResult> Assegna(AssegnazioneMezzoCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Ripopola dati per form
                var mezzo = await _mezzoService.GetByIdAsync(model.MezzoId);
                if (mezzo != null)
                {
                    model.MezzoTarga = mezzo.Targa;
                    model.MezzoDescrizioneCompleta = mezzo.DescrizioneCompleta;
                }

                if (IsAdmin())
                {
                    ViewBag.Utenti = await GetUtentiSelectListAsync(model.UtenteId);
                }

                // Ripassa periodi occupati
                ViewBag.PeriodiOccupati = await _assegnazioneService.GetPeriodiOccupatiAsync(model.MezzoId);
                return View(model);
            }

            var currentUserId = GetCurrentUserId();
            var isAdmin = IsAdmin();

            // Validazione: DataEntry può assegnare solo a sé stesso
            if (!isAdmin && model.UtenteId != currentUserId)
            {
                ModelState.AddModelError(string.Empty, "Non hai i permessi per assegnare mezzi ad altri utenti");
                ViewBag.PeriodiOccupati = await _assegnazioneService.GetPeriodiOccupatiAsync(model.MezzoId);

                if (IsAdmin())
                {
                    ViewBag.Utenti = await GetUtentiSelectListAsync(model.UtenteId);
                }
                else
                {
                    // Per DataEntry, nascondi il campo utente (sarà readonly)
                    ViewBag.IsDataEntry = true;
                    var currentUser = await GetCurrentUserAsync();
                    ViewBag.CurrentUserName = currentUser?.NomeCompleto ?? currentUserId;
                }

                return View(model);
            }

            // Validazione: Solo Admin può creare assegnazioni immediate (DataInizio = oggi/passato)
            if (!isAdmin && model.DataInizio <= DateTime.Now)
            {
                ModelState.AddModelError("DataInizio",
                    "Solo gli amministratori possono creare assegnazioni immediate. Seleziona una data futura per una prenotazione.");
                ViewBag.PeriodiOccupati = await _assegnazioneService.GetPeriodiOccupatiAsync(model.MezzoId);

                if (IsAdmin())
                {
                    ViewBag.Utenti = await GetUtentiSelectListAsync(model.UtenteId);
                }
                else
                {
                    // Per DataEntry, nascondi il campo utente (sarà readonly)
                    ViewBag.IsDataEntry = true;
                    var currentUser = await GetCurrentUserAsync();
                    ViewBag.CurrentUserName = currentUser?.NomeCompleto ?? currentUserId;
                }

                return View(model);
            }

            // MODIFICATO: Validazione disponibilità nel periodo specificato
            if (!await _assegnazioneService.IsMezzoDisponibileAsync(
                model.MezzoId,
                model.DataInizio,
                model.DataFine))
            {
                ModelState.AddModelError(string.Empty,
                    "Il mezzo non è disponibile nel periodo selezionato. Controlla il calendario per vedere i periodi già occupati.");
                ViewBag.PeriodiOccupati = await _assegnazioneService.GetPeriodiOccupatiAsync(model.MezzoId);

                if (IsAdmin())
                {
                    ViewBag.Utenti = await GetUtentiSelectListAsync(model.UtenteId);
                }
                else
                {
                    // Per DataEntry, nascondi il campo utente (sarà readonly)
                    ViewBag.IsDataEntry = true;
                    var currentUser = await GetCurrentUserAsync();
                    ViewBag.CurrentUserName = currentUser?.NomeCompleto ?? currentUserId;
                }

                return View(model);
            }

            // Crea assegnazione
            var (success, errorMessage, assegnazioneId) = await _assegnazioneService.CreateAsync(model);

            if (!success)
            {
                ModelState.AddModelError(string.Empty, errorMessage ?? "Errore durante la creazione dell'assegnazione");
                ViewBag.PeriodiOccupati = await _assegnazioneService.GetPeriodiOccupatiAsync(model.MezzoId);

                if (IsAdmin())
                {
                    ViewBag.Utenti = await GetUtentiSelectListAsync(model.UtenteId);
                }
                else
                {
                    // Per DataEntry, nascondi il campo utente (sarà readonly)
                    ViewBag.IsDataEntry = true;
                    var currentUser = await GetCurrentUserAsync();
                    ViewBag.CurrentUserName = currentUser?.NomeCompleto ?? currentUserId;
                }

                return View(model);
            }

            var tipoAssegnazione = model.DataFine.HasValue ? "temporanea" : "a tempo indeterminato";
            TempData["SuccessMessage"] = $"Assegnazione {tipoAssegnazione} creata con successo!";

            return RedirectToAction("Details", "Mezzi", new { id = model.MezzoId });
        }

        #endregion

        #region Riconsegna Mezzo (Close)

        /// <summary>
        /// GET: /Assegnazioni/Riconsegna/{id}
        /// Form per riconsegnare un mezzo (chiudere assegnazione)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Riconsegna(Guid id)
        {
            var currentUserId = GetCurrentUserId();
            var isAdmin = IsAdmin();

            // Verifica che l'utente possa chiudere questa assegnazione
            if (!await _assegnazioneService.CanUserCloseAssegnazioneAsync(id, currentUserId, isAdmin))
            {
                TempData["ErrorMessage"] = "Non hai i permessi per chiudere questa assegnazione";
                return RedirectToAction("Index", "Mezzi");
            }

            var model = await _assegnazioneService.GetCloseViewModelAsync(id);
            if (model == null)
            {
                _logger.LogWarning("Tentativo di riconsegnare assegnazione non esistente: {AssegnazioneId}", id);
                return NotFound();
            }

            return View(model);
        }

        /// <summary>
        /// POST: /Assegnazioni/Riconsegna
        /// Salva chiusura assegnazione
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Riconsegna(Guid id, AssegnazioneMezzoCloseViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var currentUserId = GetCurrentUserId();
            var isAdmin = IsAdmin();

            // Verifica autorizzazioni
            if (!await _assegnazioneService.CanUserCloseAssegnazioneAsync(id, currentUserId, isAdmin))
            {
                TempData["ErrorMessage"] = "Non hai i permessi per chiudere questa assegnazione";
                return RedirectToAction("Index", "Mezzi");
            }

            var (success, errorMessage) = await _assegnazioneService.CloseAsync(id, model);

            if (!success)
            {
                ModelState.AddModelError(string.Empty, errorMessage ?? "Errore durante la chiusura dell'assegnazione");
                return View(model);
            }

            TempData["SuccessMessage"] = "Mezzo riconsegnato con successo";
            return RedirectToAction("Details", "Mezzi", new { id = model.MezzoId });
        }

        #endregion

        #region Cancella Prenotazione

        /// <summary>
        /// POST: /Assegnazioni/CancellaPrenotazione/{id}
        /// Cancella una prenotazione futura
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancellaPrenotazione(Guid id, Guid mezzoId)
        {
            var currentUserId = GetCurrentUserId();
            var isAdmin = IsAdmin();

            var (success, errorMessage) = await _assegnazioneService.CancellaPrenotazioneAsync(id, currentUserId, isAdmin);

            if (!success)
            {
                TempData["ErrorMessage"] = errorMessage ?? "Errore durante la cancellazione della prenotazione";
            }
            else
            {
                TempData["SuccessMessage"] = "Prenotazione cancellata con successo";
            }

            return RedirectToAction("Details", "Mezzi", new { id = mezzoId });
        }

        #endregion

        #region Storico

        /// <summary>
        /// GET: /Assegnazioni/Storico/{mezzoId}
        /// Visualizza lo storico completo delle assegnazioni di un mezzo
        /// </summary>
        [HttpGet]
        [Authorize(Roles = $"{RoleNames.Amministrazione},{RoleNames.UfficioGare}")]
        public async Task<IActionResult> Storico(Guid mezzoId)
        {
            var mezzo = await _mezzoService.GetByIdAsync(mezzoId);
            if (mezzo == null)
            {
                return NotFound();
            }

            var assegnazioni = await _assegnazioneService.GetByMezzoIdAsync(mezzoId, includeChiuse: true);

            ViewBag.Mezzo = mezzo;
            return View(assegnazioni);
        }

        /// <summary>
        /// GET: /Assegnazioni/StoricoUtente/{userId?}
        /// Visualizza lo storico delle assegnazioni di un utente
        /// Se userId è null, mostra lo storico dell'utente corrente
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> StoricoUtente(string? userId = null)
        {
            var currentUserId = GetCurrentUserId();
            var isAdmin = IsAdmin();

            // Se userId non specificato, usa l'utente corrente
            if (string.IsNullOrEmpty(userId))
            {
                userId = currentUserId;
            }

            // Se l'utente richiede lo storico di qualcun altro, deve essere Admin
            if (userId != currentUserId && !isAdmin)
            {
                TempData["ErrorMessage"] = "Non hai i permessi per visualizzare lo storico di altri utenti";
                return RedirectToAction("Index", "Mezzi");
            }

            var utente = await _userManager.FindByIdAsync(userId);
            if (utente == null)
            {
                return NotFound();
            }

            var assegnazioni = await _assegnazioneService.GetByUtenteIdAsync(userId, includeChiuse: true);

            ViewBag.Utente = utente;
            ViewBag.IsCurrentUser = (userId == currentUserId);
            return View(assegnazioni);
        }

        #endregion

        // 3. Endpoint API JSON per ottenere periodi occupati (per AJAX/refresh calendario)

        /// <summary>
        /// API: Ottiene periodi occupati per il calendario (JSON)
        /// Endpoint pubblico per refresh dinamico del calendario
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPeriodiOccupati(Guid mezzoId)
        {
            try
            {
                var periodi = await _assegnazioneService.GetPeriodiOccupatiAsync(mezzoId);
                return Json(periodi);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero periodi occupati per mezzo {MezzoId}", mezzoId);
                return Json(new { error = "Errore nel caricamento dei periodi" });
            }
        }

        // 4. Endpoint API per validare disponibilità periodo (opzionale, per UX avanzata)

        /// <summary>
        /// API: Verifica disponibilità mezzo in un periodo specifico
        /// Utile per validazione real-time durante compilazione form
        /// </summary>
        [HttpPost]
        [Authorize(Roles = $"{RoleNames.Amministrazione},{RoleNames.UfficioGare},{RoleNames.DataEntry}")]
        public async Task<IActionResult> CheckDisponibilita([FromBody] CheckDisponibilitaRequest request)
        {
            try
            {
                var isDisponibile = await _assegnazioneService.IsMezzoDisponibileAsync(
                    request.MezzoId,
                    request.DataInizio,
                    request.DataFine);

                return Json(new { disponibile = isDisponibile });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nella verifica disponibilità");
                return Json(new { error = "Errore nella verifica" });
            }
        }

        // Helper class per request API
        public class CheckDisponibilitaRequest
        {
            public Guid MezzoId { get; set; }
            public DateTime DataInizio { get; set; }
            public DateTime? DataFine { get; set; }
        }
    }
}