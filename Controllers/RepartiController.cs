using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trasformazioni.Models.ViewModels;
using Trasformazioni.Services.Interfaces;

namespace Trasformazioni.Controllers
{
    /// <summary>
    /// Controller per la gestione dei Reparti
    /// </summary>
    [Authorize]
    public class RepartiController : Controller
    {
        private readonly IRepartoService _repartoService;
        private readonly ILogger<RepartiController> _logger;

        public RepartiController(
            IRepartoService repartoService,
            ILogger<RepartiController> logger)
        {
            _repartoService = repartoService;
            _logger = logger;
        }

        private string GetCurrentUserId() => User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "";

        // ===================================
        // INDEX
        // ===================================

        /// <summary>
        /// GET: /Reparti
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index(RepartoFilterViewModel filter)
        {
            var model = await _repartoService.GetPagedAsync(filter);
            return View(model);
        }

        // ===================================
        // DETAILS
        // ===================================

        /// <summary>
        /// GET: /Reparti/Details/5
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var model = await _repartoService.GetByIdAsync(id);
            if (model == null)
            {
                TempData["ErrorMessage"] = "Reparto non trovato.";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // ===================================
        // CREATE
        // ===================================

        /// <summary>
        /// GET: /Reparti/Create
        /// </summary>
        [HttpGet]
        public IActionResult Create()
        {
            return View(new RepartoCreateViewModel());
        }

        /// <summary>
        /// POST: /Reparti/Create
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RepartoCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var (success, errorMessage, repartoId) = await _repartoService.CreateAsync(model, GetCurrentUserId());

            if (!success)
            {
                ModelState.AddModelError(string.Empty, errorMessage ?? "Errore durante la creazione.");
                return View(model);
            }

            TempData["SuccessMessage"] = "Reparto creato con successo!";
            return RedirectToAction(nameof(Details), new { id = repartoId });
        }

        // ===================================
        // EDIT
        // ===================================

        /// <summary>
        /// GET: /Reparti/Edit/5
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _repartoService.GetForEditAsync(id);
            if (model == null)
            {
                TempData["ErrorMessage"] = "Reparto non trovato.";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        /// <summary>
        /// POST: /Reparti/Edit/5
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, RepartoEditViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            var (success, errorMessage) = await _repartoService.UpdateAsync(model, GetCurrentUserId());

            if (!success)
            {
                ModelState.AddModelError(string.Empty, errorMessage ?? "Errore durante la modifica.");
                return View(model);
            }

            TempData["SuccessMessage"] = "Reparto modificato con successo!";
            return RedirectToAction(nameof(Details), new { id });
        }

        // ===================================
        // DELETE
        // ===================================

        /// <summary>
        /// GET: /Reparti/Delete/5
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var model = await _repartoService.GetByIdAsync(id);
            if (model == null)
            {
                TempData["ErrorMessage"] = "Reparto non trovato.";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        /// <summary>
        /// POST: /Reparti/Delete/5
        /// </summary>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var (success, errorMessage) = await _repartoService.DeleteAsync(id, GetCurrentUserId());

            if (!success)
            {
                TempData["ErrorMessage"] = errorMessage ?? "Errore durante l'eliminazione.";
                return RedirectToAction(nameof(Delete), new { id });
            }

            TempData["SuccessMessage"] = "Reparto eliminato con successo!";
            return RedirectToAction(nameof(Index));
        }

        // ===================================
        // AJAX VALIDATION
        // ===================================

        /// <summary>
        /// Verifica unicità nome (per validazione client)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> CheckNomeUnique(string nome, Guid? id = null)
        {
            var isUnique = await _repartoService.IsNomeUniqueAsync(nome, id);
            return Json(isUnique);
        }

        /// <summary>
        /// Verifica unicità email (per validazione client)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> CheckEmailUnique(string email, Guid? id = null)
        {
            var isUnique = await _repartoService.IsEmailUniqueAsync(email, id);
            return Json(isUnique);
        }
    }
}