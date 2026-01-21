using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trasformazioni.Authorization;
using Trasformazioni.Models.ViewModels;
using Trasformazioni.Services.Interfaces;

namespace Trasformazioni.Controllers
{
    /// <summary>
    /// Controller per la gestione delle Categorie Contratto
    /// </summary>
    [Authorize(Roles = RoleNames.Amministrazione)]
    public class CategorieContrattoController : Controller
    {
        private readonly ICategoriaContrattoService _categoriaService;
        private readonly ILogger<CategorieContrattoController> _logger;

        public CategorieContrattoController(
            ICategoriaContrattoService categoriaService,
            ILogger<CategorieContrattoController> logger)
        {
            _categoriaService = categoriaService;
            _logger = logger;
        }

        #region READ - Visualizzazione

        /// <summary>
        /// GET: /CategorieContratto
        /// Lista delle categorie
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var categorie = await _categoriaService.GetAllAsync();
                var statistiche = await _categoriaService.GetStatisticheAsync();

                ViewBag.Statistiche = statistiche;

                return View(categorie);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento della lista categorie");
                TempData["ErrorMessage"] = "Errore durante il caricamento delle categorie. Riprova.";
                return View(Enumerable.Empty<CategoriaContrattoListViewModel>());
            }
        }

        /// <summary>
        /// GET: /CategorieContratto/Details/5
        /// Dettagli di una categoria
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var categoria = await _categoriaService.GetByIdAsync(id);

                if (categoria == null)
                {
                    _logger.LogWarning("Categoria con ID {CategoriaId} non trovata", id);
                    TempData["ErrorMessage"] = "Categoria non trovata.";
                    return RedirectToAction(nameof(Index));
                }

                return View(categoria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento dei dettagli categoria {CategoriaId}", id);
                TempData["ErrorMessage"] = "Errore durante il caricamento dei dettagli. Riprova.";
                return RedirectToAction(nameof(Index));
            }
        }

        #endregion

        #region CREATE

        /// <summary>
        /// GET: /CategorieContratto/Create
        /// Form per creare una nuova categoria
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new CategoriaContrattoCreateViewModel
            {
                Ordine = await _categoriaService.GetNextOrdineAsync(),
                IsAttivo = true
            };

            return View(model);
        }

        /// <summary>
        /// POST: /CategorieContratto/Create
        /// Salva una nuova categoria
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoriaContrattoCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Verifica unicità nome
                if (!await _categoriaService.IsNomeUniqueAsync(model.Nome))
                {
                    ModelState.AddModelError(nameof(model.Nome), "Esiste già una categoria con questo nome.");
                    return View(model);
                }

                var (success, errorMessage, categoriaId) = await _categoriaService.CreateAsync(model);

                if (!success)
                {
                    ModelState.AddModelError("", errorMessage ?? "Errore durante la creazione della categoria.");
                    return View(model);
                }

                _logger.LogInformation("Categoria '{Nome}' creata con successo da {User}",
                    model.Nome, User.Identity?.Name);

                TempData["SuccessMessage"] = $"Categoria '{model.Nome}' creata con successo!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la creazione della categoria");
                ModelState.AddModelError("", "Errore durante la creazione della categoria. Riprova.");
                return View(model);
            }
        }

        #endregion

        #region EDIT

        /// <summary>
        /// GET: /CategorieContratto/Edit/5
        /// Form per modificare una categoria
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var categoria = await _categoriaService.GetForEditAsync(id);

                if (categoria == null)
                {
                    _logger.LogWarning("Categoria con ID {CategoriaId} non trovata per modifica", id);
                    TempData["ErrorMessage"] = "Categoria non trovata.";
                    return RedirectToAction(nameof(Index));
                }

                return View(categoria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento della categoria per modifica {CategoriaId}", id);
                TempData["ErrorMessage"] = "Errore durante il caricamento. Riprova.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// POST: /CategorieContratto/Edit/5
        /// Salva le modifiche a una categoria
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, CategoriaContrattoEditViewModel model)
        {
            if (id != model.Id)
            {
                TempData["ErrorMessage"] = "ID non corrispondente.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Verifica esistenza
                if (!await _categoriaService.ExistsAsync(id))
                {
                    TempData["ErrorMessage"] = "Categoria non trovata.";
                    return RedirectToAction(nameof(Index));
                }

                // Verifica unicità nome
                if (!await _categoriaService.IsNomeUniqueAsync(model.Nome, id))
                {
                    ModelState.AddModelError(nameof(model.Nome), "Esiste già un'altra categoria con questo nome.");
                    return View(model);
                }

                var (success, errorMessage) = await _categoriaService.UpdateAsync(model);

                if (!success)
                {
                    ModelState.AddModelError("", errorMessage ?? "Errore durante il salvataggio delle modifiche.");
                    return View(model);
                }

                _logger.LogInformation("Categoria '{Nome}' modificata con successo da {User}",
                    model.Nome, User.Identity?.Name);

                TempData["SuccessMessage"] = $"Categoria '{model.Nome}' modificata con successo!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la modifica della categoria {CategoriaId}", id);
                ModelState.AddModelError("", "Errore durante il salvataggio delle modifiche. Riprova.");
                return View(model);
            }
        }

        #endregion

        #region DELETE

        /// <summary>
        /// POST: /CategorieContratto/Delete/5
        /// Elimina una categoria (soft delete)
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var categoria = await _categoriaService.GetByIdAsync(id);

                if (categoria == null)
                {
                    _logger.LogWarning("Tentativo di eliminazione categoria inesistente: {CategoriaId}", id);
                    return Json(new { success = false, message = "Categoria non trovata." });
                }

                // Verifica se utilizzata
                if (await _categoriaService.IsUsedAsync(id))
                {
                    var utilizzi = await _categoriaService.CountUsageAsync(id);
                    return Json(new
                    {
                        success = false,
                        message = $"Impossibile eliminare: la categoria è utilizzata da {utilizzi} registri."
                    });
                }

                var (success, errorMessage) = await _categoriaService.DeleteAsync(id);

                if (!success)
                {
                    return Json(new { success = false, message = errorMessage ?? "Errore durante l'eliminazione." });
                }

                _logger.LogInformation("Categoria '{Nome}' eliminata da {User}",
                    categoria.Nome, User.Identity?.Name);

                return Json(new
                {
                    success = true,
                    message = $"Categoria '{categoria.Nome}' eliminata con successo!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'eliminazione della categoria {CategoriaId}", id);
                return Json(new
                {
                    success = false,
                    message = "Errore durante l'eliminazione. Riprova."
                });
            }
        }

        #endregion

        #region WORKFLOW - Attiva/Disattiva

        /// <summary>
        /// POST: /CategorieContratto/Attiva/5
        /// Attiva una categoria
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Attiva(Guid id)
        {
            try
            {
                var (success, errorMessage) = await _categoriaService.AttivaAsync(id);

                if (!success)
                {
                    return Json(new { success = false, message = errorMessage ?? "Errore durante l'attivazione." });
                }

                _logger.LogInformation("Categoria {CategoriaId} attivata da {User}",
                    id, User.Identity?.Name);

                return Json(new
                {
                    success = true,
                    message = "Categoria attivata con successo!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'attivazione della categoria {CategoriaId}", id);
                return Json(new
                {
                    success = false,
                    message = "Errore durante l'attivazione. Riprova."
                });
            }
        }

        /// <summary>
        /// POST: /CategorieContratto/Disattiva/5
        /// Disattiva una categoria
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Disattiva(Guid id)
        {
            try
            {
                var (success, errorMessage) = await _categoriaService.DisattivaAsync(id);

                if (!success)
                {
                    return Json(new { success = false, message = errorMessage ?? "Errore durante la disattivazione." });
                }

                _logger.LogInformation("Categoria {CategoriaId} disattivata da {User}",
                    id, User.Identity?.Name);

                return Json(new
                {
                    success = true,
                    message = "Categoria disattivata con successo!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la disattivazione della categoria {CategoriaId}", id);
                return Json(new
                {
                    success = false,
                    message = "Errore durante la disattivazione. Riprova."
                });
            }
        }

        #endregion

        #region UTILITY

        /// <summary>
        /// GET: /CategorieContratto/CheckNome
        /// Verifica se un nome categoria è già utilizzato (per validazione AJAX)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> CheckNome(string nome, Guid? excludeId = null)
        {
            try
            {
                var isUnique = await _categoriaService.IsNomeUniqueAsync(nome, excludeId);

                if (isUnique)
                    return Json(true);

                return Json($"Il nome '{nome}' è già utilizzato.");
            }
            catch
            {
                return Json(true); // In caso di errore, non blocchiamo
            }
        }

        #endregion
    }
}