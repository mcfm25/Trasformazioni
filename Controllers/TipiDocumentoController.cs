using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Trasformazioni.Authorization;
using Trasformazioni.Helpers;
using Trasformazioni.Models.Enums;
using Trasformazioni.Models.ViewModels;
using Trasformazioni.Services.Interfaces;

namespace Trasformazioni.Controllers
{
    /// <summary>
    /// Controller per la gestione dei Tipi Documento.
    /// Accessibile solo agli amministratori.
    /// Fornisce CRUD completo per la configurazione dei tipi documento.
    /// </summary>
    [Authorize(Roles = RoleNames.Amministrazione)]
    public class TipiDocumentoController : Controller
    {
        private readonly ITipoDocumentoService _tipoDocumentoService;
        private readonly ILogger<TipiDocumentoController> _logger;

        public TipiDocumentoController(
            ITipoDocumentoService tipoDocumentoService,
            ILogger<TipiDocumentoController> logger)
        {
            _tipoDocumentoService = tipoDocumentoService;
            _logger = logger;
        }

        // ===================================
        // INDEX - LISTA
        // ===================================

        /// <summary>
        /// GET: /TipiDocumento
        /// Lista di tutti i tipi documento con filtri
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index(AreaDocumento? area = null, bool? soloSistema = null)
        {
            try
            {
                IEnumerable<TipoDocumentoListViewModel> tipi;

                if (area.HasValue)
                {
                    tipi = await _tipoDocumentoService.GetByAreaAsync(area.Value);
                }
                else
                {
                    tipi = await _tipoDocumentoService.GetAllAsync();
                }

                // Filtro tipi sistema/personalizzati
                if (soloSistema == true)
                {
                    tipi = tipi.Where(t => t.IsSystem);
                }
                else if (soloSistema == false)
                {
                    tipi = tipi.Where(t => !t.IsSystem);
                }

                // Statistiche per area
                ViewBag.CountByArea = await _tipoDocumentoService.GetCountByAreaAsync();
                ViewBag.Statistiche = await _tipoDocumentoService.GetStatisticheAsync();
                
                // Filtri correnti
                ViewBag.AreaFiltro = area;
                ViewBag.SoloSistemaFiltro = soloSistema;

                // Dropdown aree
                PrepareAreaDropdown(area);

                return View(tipi);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento della lista tipi documento");
                TempData["ErrorMessage"] = "Errore durante il caricamento dei tipi documento";
                return View(Enumerable.Empty<TipoDocumentoListViewModel>());
            }
        }

        // ===================================
        // DETAILS - DETTAGLIO
        // ===================================

        /// <summary>
        /// GET: /TipiDocumento/Details/{id}
        /// Visualizza dettagli di un tipo documento
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var tipo = await _tipoDocumentoService.GetByIdAsync(id);

                if (tipo == null)
                {
                    TempData["ErrorMessage"] = "Tipo documento non trovato";
                    return RedirectToAction(nameof(Index));
                }

                return View(tipo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento del dettaglio tipo documento {Id}", id);
                TempData["ErrorMessage"] = "Errore durante il caricamento del tipo documento";
                return RedirectToAction(nameof(Index));
            }
        }

        // ===================================
        // CREATE - CREAZIONE
        // ===================================

        /// <summary>
        /// GET: /TipiDocumento/Create
        /// Form per creare un nuovo tipo documento
        /// </summary>
        [HttpGet]
        public IActionResult Create(AreaDocumento? area = null)
        {
            var model = new TipoDocumentoCreateViewModel();

            if (area.HasValue)
            {
                model.Area = area.Value;
            }

            PrepareAreaDropdown(model.Area);
            return View(model);
        }

        /// <summary>
        /// POST: /TipiDocumento/Create
        /// Salvataggio nuovo tipo documento
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TipoDocumentoCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                PrepareAreaDropdown(model.Area);
                return View(model);
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";
                var (success, errorMessage, id) = await _tipoDocumentoService.CreateAsync(model, userId);

                if (!success)
                {
                    ModelState.AddModelError(string.Empty, errorMessage ?? "Errore durante la creazione");
                    PrepareAreaDropdown(model.Area);
                    return View(model);
                }

                _logger.LogInformation("Tipo documento '{Nome}' creato con ID {Id}", model.Nome, id);
                TempData["SuccessMessage"] = $"Tipo documento '{model.Nome}' creato con successo";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la creazione del tipo documento");
                ModelState.AddModelError(string.Empty, "Errore imprevisto durante la creazione");
                PrepareAreaDropdown(model.Area);
                return View(model);
            }
        }

        // ===================================
        // EDIT - MODIFICA
        // ===================================

        /// <summary>
        /// GET: /TipiDocumento/Edit/{id}
        /// Form per modificare un tipo documento
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var tipo = await _tipoDocumentoService.GetForEditAsync(id);

                if (tipo == null)
                {
                    TempData["ErrorMessage"] = "Tipo documento non trovato";
                    return RedirectToAction(nameof(Index));
                }

                PrepareAreaDropdown(tipo.Area);
                return View(tipo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento del form modifica tipo documento {Id}", id);
                TempData["ErrorMessage"] = "Errore durante il caricamento del tipo documento";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// POST: /TipiDocumento/Edit/{id}
        /// Salvataggio modifiche tipo documento
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, TipoDocumentoEditViewModel model)
        {
            if (id != model.Id)
            {
                TempData["ErrorMessage"] = "ID non corrispondente";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                PrepareAreaDropdown(model.Area);
                return View(model);
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";
                var (success, errorMessage) = await _tipoDocumentoService.UpdateAsync(model, userId);

                if (!success)
                {
                    ModelState.AddModelError(string.Empty, errorMessage ?? "Errore durante la modifica");
                    PrepareAreaDropdown(model.Area);
                    return View(model);
                }

                _logger.LogInformation("Tipo documento {Id} modificato", id);
                TempData["SuccessMessage"] = $"Tipo documento '{model.Nome}' modificato con successo";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la modifica del tipo documento {Id}", id);
                ModelState.AddModelError(string.Empty, "Errore imprevisto durante la modifica");
                PrepareAreaDropdown(model.Area);
                return View(model);
            }
        }

        // ===================================
        // DELETE - ELIMINAZIONE
        // ===================================

        /// <summary>
        /// GET: /TipiDocumento/Delete/{id}
        /// Pagina conferma eliminazione
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var tipo = await _tipoDocumentoService.GetByIdAsync(id);

                if (tipo == null)
                {
                    TempData["ErrorMessage"] = "Tipo documento non trovato";
                    return RedirectToAction(nameof(Index));
                }

                return View(tipo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento della conferma eliminazione tipo documento {Id}", id);
                TempData["ErrorMessage"] = "Errore durante il caricamento del tipo documento";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// POST: /TipiDocumento/Delete/{id}
        /// Conferma eliminazione tipo documento
        /// </summary>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";
                var (success, errorMessage) = await _tipoDocumentoService.DeleteAsync(id, userId);

                if (!success)
                {
                    TempData["ErrorMessage"] = errorMessage ?? "Errore durante l'eliminazione";
                    return RedirectToAction(nameof(Delete), new { id });
                }

                _logger.LogInformation("Tipo documento {Id} eliminato", id);
                TempData["SuccessMessage"] = "Tipo documento eliminato con successo";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'eliminazione del tipo documento {Id}", id);
                TempData["ErrorMessage"] = "Errore imprevisto durante l'eliminazione";
                return RedirectToAction(nameof(Delete), new { id });
            }
        }

        // ===================================
        // API - VALIDAZIONI REMOTE
        // ===================================

        /// <summary>
        /// GET: /TipiDocumento/IsNomeDisponibile
        /// Verifica disponibilità nome (Remote Validation)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> IsNomeDisponibile(string nome, AreaDocumento area, Guid? id = null)
        {
            try
            {
                var disponibile = await _tipoDocumentoService.IsNomeDisponibileAsync(nome, area, id);
                return Json(disponibile ? true : $"Esiste già un tipo documento con nome '{nome}' per quest'area");
            }
            catch
            {
                return Json(true);
            }
        }

        /// <summary>
        /// GET: /TipiDocumento/GetByArea
        /// Ottiene tipi documento per area (AJAX per dropdown dinamiche)
        /// </summary>
        [HttpGet]
        [AllowAnonymous]  // Permette a tutti gli utenti autenticati di usare le dropdown
        public async Task<IActionResult> GetByArea(AreaDocumento area)
        {
            try
            {
                var tipi = await _tipoDocumentoService.GetForDropdownAsync(area);
                return Json(tipi);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero tipi documento per area {Area}", area);
                return Json(new List<TipoDocumentoDropdownViewModel>());
            }
        }

        // ===================================
        // HELPER METHODS
        // ===================================

        private void PrepareAreaDropdown(AreaDocumento? selectedArea = null)
        {
            var aree = Enum.GetValues<AreaDocumento>()
                .Select(a => new SelectListItem
                {
                    Value = ((int)a).ToString(),
                    Text = EnumHelper.GetDisplayName(a),
                    Selected = selectedArea.HasValue && a == selectedArea.Value
                })
                .ToList();

            ViewBag.Aree = aree;
        }
    }
}
