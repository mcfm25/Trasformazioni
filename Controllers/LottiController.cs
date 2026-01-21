using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Trasformazioni.Helpers;
using Trasformazioni.Mappings;
using Trasformazioni.Models;
using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;
using Trasformazioni.Models.ViewModels;
using Trasformazioni.Services;
using Trasformazioni.Services.Interfaces;
using Trasformazioni.ViewModels;

namespace Trasformazioni.Controllers
{
    [Authorize]
    public class LottiController : Controller
    {
        private readonly ILottoService _lottoService;
        private readonly IGaraService _garaService;
        private readonly ILogger<LottiController> _logger;
        private readonly IScadenzaService _scadenzaService;
        private readonly ITipoDocumentoService _tipoDocumentoService;

        public LottiController(
            ILottoService lottoService,
            IGaraService garaService,
            ILogger<LottiController> logger,
            IScadenzaService scadenzaService,
            ITipoDocumentoService tipoDocumentoService)
        {
            _lottoService = lottoService;
            _garaService = garaService;
            _logger = logger;
            _scadenzaService = scadenzaService;
            _tipoDocumentoService = tipoDocumentoService;
        }

        // GET: /Lotti
        [HttpGet]
        public async Task<IActionResult> Index(LottoFilterViewModel filters)
        {
            try
            {
                // Applica valori di default
                filters.PageNumber = filters.PageNumber < 1 ? 1 : filters.PageNumber;
                filters.PageSize = filters.PageSize < 1 ? 25 : filters.PageSize;

                // Ottieni lotti filtrati e paginati (usa il metodo esistente)
                var result = await _lottoService.GetPagedAsync(filters);

                // Prepara dropdown per filtri
                await PrepareFilterDropdowns(filters.GaraId);

                // Configura ViewData per paginazione generica
                ViewData["PaginationRoute"] = new Dictionary<string, object>
                {
                    ["SearchTerm"] = filters.SearchTerm ?? "",
                    ["Stato"] = filters.Stato?.ToString() ?? "",
                    ["Tipologia"] = filters.Tipologia?.ToString() ?? "",
                    ["GaraId"] = filters.GaraId?.ToString() ?? "",
                    ["OperatoreAssegnatoId"] = filters.OperatoreAssegnatoId ?? "",
                    ["SoloConOperatore"] = filters.SoloConOperatore.ToString(),
                    ["PageSize"] = filters.PageSize,
                    ["OrderBy"] = filters.OrderBy ?? "",
                    ["OrderDescending"] = filters.OrderDescending.ToString()
                };

                return View(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel caricamento della lista lotti");
                TempData["ErrorMessage"] = "Errore nel caricamento dei lotti. Riprova più tardi.";
                return View(new PagedResult<LottoListViewModel>());
            }
        }

        // GET: /Lotti/Details/{id}
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var lotto = await _lottoService.GetByIdAsync(id);

                if (lotto == null)
                {
                    TempData["ErrorMessage"] = "Lotto non trovato.";
                    return RedirectToAction(nameof(Index));
                }

                // Carica gara associata per breadcrumb
                var gara = await _garaService.GetByIdAsync(lotto.GaraId);
                ViewBag.Gara = gara;

                // Statistiche semplici (Fase 1)
                ViewBag.NumeroPartecipanti = await _lottoService.GetNumeroPartecipantiAsync(id);
                ViewBag.NumeroValutazioni = await _lottoService.GetNumeroValutazioniAsync(id);
                ViewBag.NumeroElaborazioni = await _lottoService.GetNumeroElaborazioniAsync(id);

                return View(lotto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel caricamento del dettaglio lotto {LottoId}", id);
                TempData["ErrorMessage"] = "Errore nel caricamento del lotto.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: /Lotti/Create?garaId={id}
        [HttpGet]
        public async Task<IActionResult> Create(Guid? garaId)
        {
            try
            {
                var model = new LottoCreateViewModel();

                if (garaId.HasValue)
                {
                    // Verifica che la gara esista
                    var gara = await _garaService.GetByIdAsync(garaId.Value);
                    if (gara == null)
                    {
                        TempData["ErrorMessage"] = "Gara non trovata.";
                        return RedirectToAction("Index", "Gare");
                    }

                    model.GaraId = garaId.Value;
                    ViewBag.GaraSelezionata = gara;
                    ViewBag.IsGaraPreimpostata = true;
                }
                else
                {
                    // Dropdown gare disponibili
                    await PrepareGareDropdown();
                    ViewBag.IsGaraPreimpostata = false;
                }

                // Dropdown tipologie
                ViewBag.Tipologie = GetTipologieSelectList();

                // Checklist documenti richiesti
                await PrepareChecklistDocumentiDropdown();

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel caricamento form creazione lotto");
                TempData["ErrorMessage"] = "Errore nel caricamento del form.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /Lotti/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LottoCreateViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // Ricarica dropdown
                    if (model.GaraId != Guid.Empty)
                    {
                        var gara = await _garaService.GetByIdAsync(model.GaraId);
                        ViewBag.GaraSelezionata = gara;
                        ViewBag.IsGaraPreimpostata = true;
                    }
                    else
                    {
                        await PrepareGareDropdown();
                        ViewBag.IsGaraPreimpostata = false;
                    }
                    ViewBag.Tipologie = GetTipologieSelectList();
                    await PrepareChecklistDocumentiDropdown(model.DocumentiRichiestiIds);
                    return View(model);
                }

                // Ottieni userId
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Crea lotto tramite service
                //var lottoId = await _lottoService.CreateAsync(model);
                var (success, errorMessage, lottoId) = await _lottoService.CreateAsync(model);

                if (!success)
                {
                    await PrepareGareDropdown();
                    ViewBag.Tipologie = GetTipologieSelectList();
                    await PrepareChecklistDocumentiDropdown(model.DocumentiRichiestiIds);
                    var gara = await _garaService.GetByIdAsync(model.GaraId);
                    ViewBag.GaraSelezionata = gara;
                    ViewBag.IsGaraPreimpostata = true;
                    TempData["ErrorMessage"] = errorMessage ?? "Errore nella creazione del lotto.";
                    return View(model);
                }

                TempData["SuccessMessage"] = $"Lotto '{model.CodiceLotto}' creato con successo!";

                // Redirect al dettaglio della gara per vedere il lotto appena creato
                return RedirectToAction("Details", "Gare", new { id = model.GaraId });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Tentativo di creare lotto duplicato: {CodiceLotto}", model.CodiceLotto);
                ModelState.AddModelError("CodiceLotto", ex.Message);

                // Ricarica dropdown
                if (model.GaraId != Guid.Empty)
                {
                    var gara = await _garaService.GetByIdAsync(model.GaraId);
                    ViewBag.GaraSelezionata = gara;
                    ViewBag.IsGaraPreimpostata = true;
                }
                else
                {
                    await PrepareGareDropdown();
                    ViewBag.IsGaraPreimpostata = false;
                }
                ViewBag.Tipologie = GetTipologieSelectList();
                await PrepareChecklistDocumentiDropdown(model.DocumentiRichiestiIds);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nella creazione del lotto");
                TempData["ErrorMessage"] = "Errore nella creazione del lotto. Riprova.";

                // Ricarica dropdown
                if (model.GaraId != Guid.Empty)
                {
                    var gara = await _garaService.GetByIdAsync(model.GaraId);
                    ViewBag.GaraSelezionata = gara;
                    ViewBag.IsGaraPreimpostata = true;
                }
                else
                {
                    await PrepareGareDropdown();
                    ViewBag.IsGaraPreimpostata = false;
                }
                ViewBag.Tipologie = GetTipologieSelectList();
                await PrepareChecklistDocumentiDropdown(model.DocumentiRichiestiIds);
                return View(model);
            }
        }

        // GET: /Lotti/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                //var lotto = await _lottoService.GetByIdAsync(id);
                var model = await _lottoService.GetForEditAsync(id);

                if (model == null)
                {
                    TempData["ErrorMessage"] = "Lotto non trovato.";
                    return RedirectToAction(nameof(Index));
                }

                // Converte manualmente a EditViewModel
                //var model = new LottoEditViewModel
                //{
                //    Id = lotto.Id,
                //    GaraId = lotto.GaraId,
                //    CodiceLotto = lotto.CodiceLotto,
                //    Descrizione = lotto.Descrizione,
                //    Tipologia = lotto.Tipologia,
                //    Stato = lotto.Stato,
                //    ImportoBaseAsta = lotto.ImportoBaseAsta ?? 0,
                //    Quotazione = lotto.Quotazione,
                //    OperatoreAssegnatoId = lotto.OperatoreAssegnatoId,
                //    CreatedAt = lotto.CreatedAt,
                //    CreatedBy = lotto.CreatedBy,
                //    ModifiedAt = lotto.ModifiedAt,
                //    ModifiedBy = lotto.ModifiedBy,

                //    // Checklist Documenti Richiesti
                //    DocumentiRichiestiIds = lotto.DocumentiRichiesti?
                //    .Select(dr => dr.TipoDocumentoId)
                //    .ToList() ?? new List<Guid>(),
                //};
                // Usa il mapping extension
                //var model = lotto.ToEditViewModel();

                // Carica gara per display
                var gara = await _garaService.GetByIdAsync(model.GaraId);
                ViewBag.GaraSelezionata = gara;

                // Dropdown
                ViewBag.Tipologie = GetTipologieSelectList(model.Tipologia);
                ViewBag.Stati = GetStatiSelectList(model.Stato);
                // Checklist documenti richiesti
                await PrepareChecklistDocumentiDropdown(model.DocumentiRichiestiIds);

                // Warning se stato terminale
                ViewBag.IsStatoTerminale = IsStatoTerminale(model.Stato);

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel caricamento form modifica lotto {LottoId}", id);
                TempData["ErrorMessage"] = "Errore nel caricamento del lotto.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /Lotti/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(LottoEditViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // Ricarica dropdown
                    var gara = await _garaService.GetByIdAsync(model.GaraId);
                    ViewBag.GaraSelezionata = gara;
                    ViewBag.Tipologie = GetTipologieSelectList(model.Tipologia);
                    ViewBag.Stati = GetStatiSelectList(model.Stato);
                    ViewBag.IsStatoTerminale = IsStatoTerminale(model.Stato);
                    await PrepareChecklistDocumentiDropdown(model.DocumentiRichiestiIds);
                    return View(model);
                }

                // Ottieni userId
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Aggiorna tramite service
                await _lottoService.UpdateAsync(model);

                TempData["SuccessMessage"] = $"Lotto '{model.CodiceLotto}' aggiornato con successo!";
                return RedirectToAction(nameof(Details), new { id = model.Id });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Errore validazione aggiornamento lotto {LottoId}", model.Id);
                ModelState.AddModelError("", ex.Message);

                // Ricarica dropdown
                var gara = await _garaService.GetByIdAsync(model.GaraId);
                ViewBag.GaraSelezionata = gara;
                ViewBag.Tipologie = GetTipologieSelectList(model.Tipologia);
                ViewBag.Stati = GetStatiSelectList(model.Stato);
                ViewBag.IsStatoTerminale = IsStatoTerminale(model.Stato);
                await PrepareChecklistDocumentiDropdown(model.DocumentiRichiestiIds);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nell'aggiornamento del lotto {LottoId}", model.Id);
                TempData["ErrorMessage"] = "Errore nell'aggiornamento del lotto. Riprova.";

                // Ricarica dropdown
                var gara = await _garaService.GetByIdAsync(model.GaraId);
                ViewBag.GaraSelezionata = gara;
                ViewBag.Tipologie = GetTipologieSelectList(model.Tipologia);
                ViewBag.Stati = GetStatiSelectList(model.Stato);
                ViewBag.IsStatoTerminale = IsStatoTerminale(model.Stato);
                await PrepareChecklistDocumentiDropdown(model.DocumentiRichiestiIds);
                return View(model);
            }
        }

        // POST: /Lotti/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var lotto = await _lottoService.GetByIdAsync(id);

                if (lotto == null)
                {
                    return Json(new { success = false, message = "Lotto non trovato." });
                }

                var garaId = lotto.GaraId;

                // Verifica se ci sono entità dipendenti
                var canDelete = await _lottoService.CanDeleteAsync(id);

                if (!canDelete)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Impossibile eliminare: il lotto ha partecipanti, valutazioni o elaborazioni associate."
                    });
                }

                // Elimina
                await _lottoService.DeleteAsync(id);

                _logger.LogInformation("Lotto {LottoId} eliminato con successo", id);

                TempData["SuccessMessage"] = "Lotto eliminato con successo.";
                return Json(new { success = true, redirectUrl = Url.Action("Details", "Gare", new { id = garaId }) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nell'eliminazione del lotto {LottoId}", id);
                return Json(new { success = false, message = "Errore nell'eliminazione del lotto." });
            }
        }

        // POST: /Lotti/CambiaStato
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiaStato(Guid id, StatoLotto nuovoStato)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Cambia stato tramite service (con validazione workflow)
                var success = await _lottoService.UpdateStatoAsync(id, nuovoStato, userId);

                if (success)
                {
                    _logger.LogInformation("Stato lotto {LottoId} cambiato a {NuovoStato}", id, nuovoStato);
                    return Json(new { success = true, message = $"Stato cambiato a: {nuovoStato}" });
                }
                else
                {
                    return Json(new { success = false, message = "Lotto non trovato." });
                }
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Transizione stato non valida per lotto {LottoId}", id);
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel cambio stato del lotto {LottoId}", id);
                return Json(new { success = false, message = "Errore nel cambio stato. Riprova." });
            }
        }

        // GET: /Lotti/IsCodiceLottoUnique (Remote Validation)
        [HttpGet]
        public async Task<IActionResult> IsCodiceLottoUnique(string codiceLotto, Guid garaId, Guid? id = null)
        {
            try
            {
                var exists = await _lottoService.IsCodiceLottoUniqueInGaraAsync(codiceLotto, garaId, id);
                return Json(!exists);
            }
            catch
            {
                // In caso di errore, permettiamo la validazione
                return Json(true);
            }
        }

        #region Helper Methods

        private async Task PrepareFilterDropdowns(Guid? selectedGaraId = null)
        {
            // Stati lotto (tutti gli stati)
            ViewBag.Stati = Enum.GetValues<StatoLotto>()
                .Select(s => new SelectListItem
                {
                    Value = ((int)s).ToString(),
                    Text = EnumHelper.GetDisplayName(s)
                })
                .ToList();

            // Tipologie
            ViewBag.Tipologie = Enum.GetValues<TipologiaLotto>()
                .Select(t => new SelectListItem
                {
                    Value = ((int)t).ToString(),
                    Text = EnumHelper.GetDisplayName(t)
                })
                .ToList();

            // Gare disponibili
            var gare = await _garaService.GetAllAsync();
            ViewBag.Gare = gare.Select(g => new SelectListItem
            {
                Value = g.Id.ToString(),
                Text = $"{g.CodiceGara} - {g.Titolo}",
                Selected = selectedGaraId.HasValue && g.Id == selectedGaraId.Value
            }).ToList();
        }

        private async Task PrepareGareDropdown()
        {
            var gare = await _garaService.GetAllAsync();
            ViewBag.Gare = gare.Select(g => new SelectListItem
            {
                Value = g.Id.ToString(),
                Text = $"{g.CodiceGara} - {g.Titolo}"
            }).ToList();
        }

        private SelectList GetTipologieSelectList(TipologiaLotto? selected = null)
        {
            var tipologie = Enum.GetValues<TipologiaLotto>()
                .Select(t => new
                {
                    Value = (int)t,
                    Text = EnumHelper.GetDisplayName(t)
                });

            return new SelectList(tipologie, "Value", "Text", (int?)selected);
        }

        private SelectList GetStatiSelectList(StatoLotto? selected = null)
        {
            var stati = Enum.GetValues<StatoLotto>()
                .Select(s => new
                {
                    Value = (int)s,
                    Text = EnumHelper.GetDisplayName(s)
                });

            return new SelectList(stati, "Value", "Text", (int?)selected);
        }

        private bool IsStatoTerminale(StatoLotto stato)
        {
            return stato == StatoLotto.Vinto ||
                   stato == StatoLotto.Perso ||
                   stato == StatoLotto.Scartato ||
                   stato == StatoLotto.Rifiutato;
        }

        /// <summary>
        /// Prepara la lista dei tipi documento area Lotti per la checklist
        /// </summary>
        private async Task PrepareChecklistDocumentiDropdown(List<Guid>? selectedIds = null)
        {
            var tipiLotti = await _tipoDocumentoService.GetByAreaAsync(AreaDocumento.Lotti);

            ViewBag.TipiDocumentoChecklist = tipiLotti
                .OrderBy(t => t.Nome)
                .Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.Nome,
                    Selected = selectedIds?.Contains(t.Id) ?? false
                })
                .ToList();
        }

        #endregion

        // ========================================
        // NUOVE ACTION METHODS PER TABS AJAX
        // Da aggiungere al LottiController esistente
        // ========================================

        /// <summary>
        /// Carica partial view Valutazioni per tab AJAX
        /// GET: /Lotti/LoadValutazioni/{id}
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> LoadValutazioni(Guid id)
        {
            try
            {
                var lotto = await _lottoService.GetByIdAsync(id);

                if (lotto == null)
                {
                    return PartialView("_ErrorPartial", "Lotto non trovato.");
                }

                //lotto.Documenti.RemoveAll(d => !d.TipoDocumentoCodiceRiferimento.Equals(nameof(TipoDocumentoGara.DocumentoValutazioneTecnica)));

                return PartialView("_ValutazioniPartial", lotto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel caricamento valutazioni per lotto {LottoId}", id);
                return PartialView("_ErrorPartial", "Errore nel caricamento delle valutazioni.");
            }
        }

        /// <summary>
        /// Carica partial view Elaborazioni per tab AJAX
        /// GET: /Lotti/LoadElaborazioni/{id}
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> LoadElaborazioni(Guid id)
        {
            try
            {
                var lotto = await _lottoService.GetByIdAsync(id);

                if (lotto == null)
                {
                    return PartialView("_ErrorPartial", "Lotto non trovato.");
                }

                return PartialView("_ElaborazioniPartial", lotto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel caricamento elaborazioni per lotto {LottoId}", id);
                return PartialView("_ErrorPartial", "Errore nel caricamento delle elaborazioni.");
            }
        }

        /// <summary>
        /// Carica partial view Richieste Integrazione per tab AJAX
        /// GET: /Lotti/LoadRichiesteIntegrazione/{id}
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> LoadRichiesteIntegrazione(Guid id)
        {
            try
            {
                var lotto = await _lottoService.GetByIdAsync(id);

                if (lotto == null)
                {
                    return PartialView("_ErrorPartial", "Lotto non trovato.");
                }

                return PartialView("_RichiesteIntegrazionePartial", lotto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel caricamento richieste integrazione per lotto {LottoId}", id);
                return PartialView("_ErrorPartial", "Errore nel caricamento delle richieste.");
            }
        }

        /// <summary>
        /// Carica partial view Partecipanti per tab AJAX
        /// GET: /Lotti/LoadPartecipanti/{id}
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> LoadPartecipanti(Guid id)
        {
            try
            {
                var lotto = await _lottoService.GetByIdAsync(id);

                if (lotto == null)
                {
                    return PartialView("_ErrorPartial", "Lotto non trovato.");
                }

                return PartialView("_PartecipantiPartial", lotto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel caricamento partecipanti per lotto {LottoId}", id);
                return PartialView("_ErrorPartial", "Errore nel caricamento dei partecipanti.");
            }
        }

        /// <summary>
        /// Carica le scadenze del lotto (AJAX per tab)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> LoadScadenze(Guid id)
        {
            try
            {
                var scadenze = await _scadenzaService.GetByLottoIdAsync(id);
                ViewBag.LottoId = id;
                return PartialView("_ScadenzePartial", scadenze);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel caricamento scadenze per lotto {LottoId}", id);
                return Content("<div class='alert alert-danger'>Errore nel caricamento delle scadenze</div>");
            }
        }
    }
}