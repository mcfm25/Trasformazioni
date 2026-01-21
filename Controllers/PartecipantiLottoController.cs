using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using Trasformazioni.Models.ViewModels;
using Trasformazioni.Services.Interfaces;

namespace Trasformazioni.Controllers
{
    /// <summary>
    /// Controller per la gestione dei Partecipanti Lotto
    /// Gestisce il censimento dei partecipanti ad un lotto (tipicamente per lotti non vinti)
    /// Accessibile solo da Lotti/Details (tab Partecipanti)
    /// </summary>
    [Authorize]
    public class PartecipantiLottoController : Controller
    {
        private readonly IPartecipanteLottoService _partecipanteService;
        private readonly ILottoService _lottoService;
        private readonly ISoggettoService _soggettoService;
        private readonly ILogger<PartecipantiLottoController> _logger;

        public PartecipantiLottoController(
            IPartecipanteLottoService partecipanteService,
            ILottoService lottoService,
            ISoggettoService soggettoService,
            ILogger<PartecipantiLottoController> logger)
        {
            _partecipanteService = partecipanteService;
            _lottoService = lottoService;
            _soggettoService = soggettoService;
            _logger = logger;
        }

        // ===================================
        // INDEX - Lista Partecipanti del Lotto
        // ===================================

        /// <summary>
        /// Mostra la lista dei partecipanti per un lotto specifico
        /// </summary>
        /// <param name="lottoId">ID del lotto (obbligatorio)</param>
        [HttpGet]
        public async Task<IActionResult> Index(Guid lottoId)
        {
            try
            {
                if (lottoId == Guid.Empty)
                {
                    _logger.LogWarning("Tentativo di accesso a Index senza lottoId");
                    return BadRequest("LottoId obbligatorio");
                }

                // Verifica esistenza lotto
                var lotto = await _lottoService.GetByIdAsync(lottoId);
                if (lotto == null)
                {
                    _logger.LogWarning("Lotto {LottoId} non trovato", lottoId);
                    return NotFound("Lotto non trovato");
                }

                // Recupera partecipanti del lotto
                var partecipanti = await _partecipanteService.GetByLottoIdAsync(lottoId);

                // Passa info lotto alla view
                ViewBag.LottoId = lottoId;
                ViewBag.LottoCodice = lotto.CodiceLotto;
                ViewBag.LottoTitolo = lotto.Descrizione;
                ViewBag.GaraCig = lotto.CIG;

                return View(partecipanti);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento dei partecipanti per lotto {LottoId}", lottoId);
                TempData["ErrorMessage"] = "Errore durante il caricamento dei partecipanti";
                return RedirectToAction("Details", "Lotti", new { id = lottoId });
            }
        }

        // ===================================
        // CREATE - Creazione Partecipante
        // ===================================

        /// <summary>
        /// Mostra il form di creazione
        /// </summary>
        /// <param name="lottoId">ID del lotto (opzionale, se presente viene preimpostato)</param>
        [HttpGet]
        public async Task<IActionResult> Create(Guid? lottoId)
        {
            try
            {
                var model = new PartecipanteLottoCreateViewModel();

                // Se lottoId presente, preimposta
                if (lottoId.HasValue && lottoId.Value != Guid.Empty)
                {
                    var lotto = await _lottoService.GetByIdAsync(lottoId.Value);
                    if (lotto == null)
                    {
                        _logger.LogWarning("Lotto {LottoId} non trovato per creazione partecipante", lottoId);
                        return NotFound("Lotto non trovato");
                    }

                    model.LottoId = lottoId.Value;
                    ViewBag.LottoPreimpostato = true;
                    ViewBag.LottoCodice = lotto.CodiceLotto;
                    ViewBag.LottoTitolo = lotto.Descrizione;
                }
                else
                {
                    ViewBag.LottoPreimpostato = false;
                }

                // Carica dropdown
                await LoadDropdownsAsync(model.LottoId);

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento del form di creazione partecipante");
                TempData["ErrorMessage"] = "Errore durante il caricamento del form";
                return RedirectToAction("Index", "Lotti");
            }
        }

        /// <summary>
        /// Salva il nuovo partecipante
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PartecipanteLottoCreateViewModel model)
        {
            try
            {
                // Rimuovi validazione RagioneSociale se SoggettoId presente
                if (model.SoggettoId.HasValue)
                {
                    ModelState.Remove(nameof(model.RagioneSociale));
                }

                if (!ModelState.IsValid)
                {
                    await LoadDropdownsAsync(model.LottoId);

                    // Gestisci visualizzazione lotto preimpostato
                    if (model.LottoId != Guid.Empty)
                    {
                        var lotto = await _lottoService.GetByIdAsync(model.LottoId);
                        if (lotto != null)
                        {
                            ViewBag.LottoPreimpostato = true;
                            ViewBag.LottoCodice = lotto.CodiceLotto;
                            ViewBag.LottoTitolo = lotto.Descrizione;
                        }
                    }
                    else
                    {
                        ViewBag.LottoPreimpostato = false;
                    }

                    return View(model);
                }

                // Ottieni userId corrente
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";

                // Crea partecipante
                var (success, errorMessage, id) = await _partecipanteService.CreateAsync(model, userId);

                if (!success)
                {
                    ModelState.AddModelError(string.Empty, errorMessage ?? "Errore durante la creazione del partecipante");
                    await LoadDropdownsAsync(model.LottoId);

                    // Gestisci visualizzazione lotto preimpostato
                    if (model.LottoId != Guid.Empty)
                    {
                        var lotto = await _lottoService.GetByIdAsync(model.LottoId);
                        if (lotto != null)
                        {
                            ViewBag.LottoPreimpostato = true;
                            ViewBag.LottoCodice = lotto.CodiceLotto;
                            ViewBag.LottoTitolo = lotto.Descrizione;
                        }
                    }
                    else
                    {
                        ViewBag.LottoPreimpostato = false;
                    }

                    return View(model);
                }

                TempData["SuccessMessage"] = "Partecipante creato con successo";
                return RedirectToAction(nameof(Index), new { lottoId = model.LottoId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la creazione del partecipante");
                ModelState.AddModelError(string.Empty, "Errore durante la creazione del partecipante");
                await LoadDropdownsAsync(model.LottoId);
                return View(model);
            }
        }

        // ===================================
        // EDIT - Modifica Partecipante
        // ===================================

        /// <summary>
        /// Mostra il form di modifica
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var model = await _partecipanteService.GetForEditAsync(id);
                if (model == null)
                {
                    _logger.LogWarning("Partecipante {Id} non trovato per modifica", id);
                    return NotFound("Partecipante non trovato");
                }

                // Carica dropdown
                await LoadDropdownsAsync(model.LottoId);

                // Info lotto per display
                var lotto = await _lottoService.GetByIdAsync(model.LottoId);
                if (lotto != null)
                {
                    ViewBag.LottoCodice = lotto.CodiceLotto;
                    ViewBag.LottoTitolo = lotto.Descrizione;
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento del form di modifica per partecipante {Id}", id);
                TempData["ErrorMessage"] = "Errore durante il caricamento del form di modifica";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Salva le modifiche al partecipante
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, PartecipanteLottoEditViewModel model)
        {
            try
            {
                if (id != model.Id)
                {
                    return BadRequest("ID non corrispondente");
                }

                // Rimuovi validazione RagioneSociale se SoggettoId presente
                if (model.SoggettoId.HasValue)
                {
                    ModelState.Remove(nameof(model.RagioneSociale));
                }

                if (!ModelState.IsValid)
                {
                    await LoadDropdownsAsync(model.LottoId);

                    // Info lotto per display
                    var lottoInfo = await _lottoService.GetByIdAsync(model.LottoId);
                    if (lottoInfo != null)
                    {
                        ViewBag.LottoCodice = lottoInfo.CodiceLotto;
                        ViewBag.LottoTitolo = lottoInfo.Descrizione;
                    }

                    return View(model);
                }

                // Ottieni userId corrente
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";

                // Aggiorna partecipante
                var (success, errorMessage) = await _partecipanteService.UpdateAsync(model, userId);

                if (!success)
                {
                    ModelState.AddModelError(string.Empty, errorMessage ?? "Errore durante l'aggiornamento del partecipante");
                    await LoadDropdownsAsync(model.LottoId);

                    // Info lotto per display
                    var lottoInfo = await _lottoService.GetByIdAsync(model.LottoId);
                    if (lottoInfo != null)
                    {
                        ViewBag.LottoCodice = lottoInfo.CodiceLotto;
                        ViewBag.LottoTitolo = lottoInfo.Descrizione;
                    }

                    return View(model);
                }

                TempData["SuccessMessage"] = "Partecipante aggiornato con successo";
                return RedirectToAction(nameof(Index), new { lottoId = model.LottoId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'aggiornamento del partecipante {Id}", id);
                ModelState.AddModelError(string.Empty, "Errore durante l'aggiornamento del partecipante");
                await LoadDropdownsAsync(model.LottoId);
                return View(model);
            }
        }

        // ===================================
        // DETAILS - Dettaglio Partecipante
        // ===================================

        /// <summary>
        /// Mostra il dettaglio completo del partecipante
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var model = await _partecipanteService.GetByIdAsync(id);
                if (model == null)
                {
                    _logger.LogWarning("Partecipante {Id} non trovato", id);
                    return NotFound("Partecipante non trovato");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento del dettaglio partecipante {Id}", id);
                TempData["ErrorMessage"] = "Errore durante il caricamento del dettaglio";
                return RedirectToAction(nameof(Index));
            }
        }

        // ===================================
        // DELETE - Eliminazione Partecipante
        // ===================================

        /// <summary>
        /// Mostra la conferma di eliminazione
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var model = await _partecipanteService.GetByIdAsync(id);
                if (model == null)
                {
                    _logger.LogWarning("Partecipante {Id} non trovato per eliminazione", id);
                    return NotFound("Partecipante non trovato");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento della conferma eliminazione per partecipante {Id}", id);
                TempData["ErrorMessage"] = "Errore durante il caricamento della conferma eliminazione";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Elimina il partecipante (soft delete)
        /// </summary>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                // Recupera partecipante per ottenere LottoId prima dell'eliminazione
                var partecipante = await _partecipanteService.GetByIdAsync(id);
                if (partecipante == null)
                {
                    _logger.LogWarning("Partecipante {Id} non trovato per eliminazione", id);
                    return NotFound("Partecipante non trovato");
                }

                var lottoId = partecipante.LottoId;

                // Ottieni userId corrente
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";

                // Elimina partecipante
                var (success, errorMessage) = await _partecipanteService.DeleteAsync(id, userId);

                if (!success)
                {
                    TempData["ErrorMessage"] = errorMessage ?? "Errore durante l'eliminazione del partecipante";
                    return RedirectToAction(nameof(Delete), new { id });
                }

                TempData["SuccessMessage"] = "Partecipante eliminato con successo";
                return RedirectToAction(nameof(Index), new { lottoId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'eliminazione del partecipante {Id}", id);
                TempData["ErrorMessage"] = "Errore durante l'eliminazione del partecipante";
                return RedirectToAction(nameof(Delete), new { id });
            }
        }

        // ===================================
        // HELPER METHODS
        // ===================================

        /// <summary>
        /// Carica i dropdown per i form
        /// </summary>
        private async Task LoadDropdownsAsync(Guid? currentLottoId = null)
        {
            // Dropdown Lotti
            var lotti = await _lottoService.GetAllAsync();
            ViewBag.Lotti = new SelectList(
                lotti.Select(l => new
                {
                    Id = l.Id,
                    Display = $"{l.CodiceLotto} - {l.Descrizione}"
                }),
                "Id",
                "Display",
                currentLottoId
            );

            // Dropdown Soggetti (opzionale - solo operatori economici)
            var soggetti = await _soggettoService.GetFornitoriAsync();
            ViewBag.Soggetti = new SelectList(
                soggetti.Select(s => new
                {
                    Id = s.Id,
                    Display = s.NomeCompleto
                }),
                "Id",
                "Display"
            );
        }
    }
}