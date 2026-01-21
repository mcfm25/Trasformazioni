using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Trasformazioni.Helpers;
using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;
using Trasformazioni.Models.ViewModels;
using Trasformazioni.Services;
using Trasformazioni.Services.Interfaces;

namespace Trasformazioni.Controllers
{
    [Authorize]
    public class PreventiviController : Controller
    {
        private readonly IPreventivoService _preventivoService;
        private readonly IGaraService _garaService;
        private readonly ILottoService _lottoService;
        private readonly ISoggettoService _soggettoService;
        private readonly IMinIOService _minioService;
        private readonly ILogger<PreventiviController> _logger;

        public PreventiviController(
            IPreventivoService preventivoService,
            ILottoService lottoService,
            ISoggettoService soggettoService,
            IMinIOService minioService,
            ILogger<PreventiviController> logger,
            IGaraService garaService)
        {
            _preventivoService = preventivoService;
            _lottoService = lottoService;
            _soggettoService = soggettoService;
            _minioService = minioService;
            _logger = logger;
            _garaService = garaService;
        }

        // ===================================
        // LISTA PREVENTIVI
        // ===================================

        /// <summary>
        /// GET: /Preventivi?lottoId={id}
        /// Lista preventivi filtrati (principalmente per lotto)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index(PreventivoFilterViewModel filters)
        {
            try
            {
                // Applica valori di default
                filters.PageNumber = filters.PageNumber < 1 ? 1 : filters.PageNumber;
                filters.PageSize = filters.PageSize < 1 ? 25 : filters.PageSize;

                // Ottieni preventivi filtrati e paginati
                var result = await _preventivoService.GetPagedAsync(filters);

                // Prepara dropdown per filtri
                await PrepareFilterDropdowns(filters.LottoId, filters.SoggettoId);

                // Se filtrato per lotto, carica info lotto per breadcrumb
                if (filters.LottoId.HasValue)
                {
                    var lotto = await _lottoService.GetByIdAsync(filters.LottoId.Value);
                    ViewBag.Lotto = lotto;
                }

                // Configura ViewData per paginazione
                ViewData["PaginationRoute"] = new Dictionary<string, object>
                {
                    ["LottoId"] = filters.LottoId?.ToString() ?? "",
                    ["SoggettoId"] = filters.SoggettoId?.ToString() ?? "",
                    ["Stato"] = filters.Stato?.ToString() ?? "",
                    ["SearchTerm"] = filters.SearchTerm ?? "",
                    ["SoloSelezionati"] = filters.SoloSelezionati.ToString(),
                    ["SoloScaduti"] = filters.SoloScaduti.ToString(),
                    ["SoloInScadenza"] = filters.SoloInScadenza.ToString(),
                    ["PageSize"] = filters.PageSize,
                    ["OrderBy"] = filters.OrderBy ?? "",
                    ["OrderDescending"] = filters.OrderDescending.ToString()
                };

                return View(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel caricamento della lista preventivi");
                TempData["ErrorMessage"] = "Errore nel caricamento dei preventivi. Riprova più tardi.";
                return View(new PagedResult<PreventivoListViewModel>());
            }
        }

        // ===================================
        // DETTAGLIO PREVENTIVO
        // ===================================

        /// <summary>
        /// GET: /Preventivi/Details/{id}
        /// Visualizza dettaglio completo preventivo
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var preventivo = await _preventivoService.GetByIdAsync(id);

                if (preventivo == null)
                {
                    TempData["ErrorMessage"] = "Preventivo non trovato.";
                    return RedirectToAction(nameof(Index));
                }

                return View(preventivo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel caricamento dettaglio preventivo {PreventivoId}", id);
                TempData["ErrorMessage"] = "Errore nel caricamento del preventivo.";
                return RedirectToAction(nameof(Index));
            }
        }

        // ===================================
        // CREA PREVENTIVO
        // ===================================

        /// <summary>
        /// GET: /Preventivi/Create?lottoId={id}
        /// Form creazione nuovo preventivo
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Create(Guid? lottoId)
        {
            try
            {
                var model = new PreventivoCreateViewModel
                {
                    DataRichiesta = DateTime.Today,
                    DataScadenza = DateTime.Today.AddDays(30) // Default 30 giorni
                };

                if (lottoId.HasValue)
                {
                    // Verifica che il lotto esista
                    var lotto = await _lottoService.GetByIdAsync(lottoId.Value);
                    if (lotto == null)
                    {
                        TempData["ErrorMessage"] = "Lotto non trovato.";
                        return RedirectToAction("Index", "Lotti");
                    }

                    model.LottoId = lottoId.Value;
                    model.CodiceLotto = lotto.CodiceLotto;
                    model.DescrizioneLotto = lotto.Descrizione;
                    ViewBag.LottoPreimpostato = true;
                }

                // Dropdown gare e tipi documento
                await PrepareGareDropdown();
                // Dropdown fornitori e stati
                await PrepareFornitioriDropdown();
                PrepareStatiDropdown();

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel caricamento form creazione preventivo");
                TempData["ErrorMessage"] = "Errore nel caricamento del form.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// POST: /Preventivi/Create
        /// Salva nuovo preventivo
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PreventivoCreateViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await PrepareFornitioriDropdown(model.SoggettoId);
                    PrepareStatiDropdown();
                    return View(model);
                }

                string? documentPath = null;

                // Upload documento se presente
                if (model.DocumentoFile != null && model.DocumentoFile.Length > 0)
                {
                    try
                    {
                        // Genera path MinIO: preventivi/{lottoId}/{guid}_{filename}
                        var fileName = Path.GetFileName(model.DocumentoFile.FileName);
                        //var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";

                        // Ottieni info lotto per generare path corretto
                        var lotto = await _lottoService.GetByIdAsync(model.LottoId);
                        var objectPath = _minioService.GenerateObjectPath(
                            garaId: lotto.GaraId,
                            lottoId: model.LottoId,
                            //fileName: uniqueFileName
                            fileName: fileName
                        );

                        using var stream = model.DocumentoFile.OpenReadStream();
                        documentPath = await _minioService.UploadFileAsync(
                            stream,
                            objectPath,
                            model.DocumentoFile.ContentType
                        );

                        _logger.LogInformation("Documento preventivo caricato: {Path}", documentPath);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Errore durante l'upload del documento preventivo");
                        ModelState.AddModelError("DocumentoFile", "Errore durante il caricamento del file.");
                        await PrepareFornitioriDropdown(model.SoggettoId);
                        PrepareStatiDropdown();
                        return View(model);
                    }
                }

                // Crea preventivo tramite service
                var (success, errorMessage, preventivoId) = await _preventivoService.CreateAsync(model, documentPath);

                if (!success)
                {
                    // Elimina documento se upload riuscito ma creazione fallita
                    if (!string.IsNullOrEmpty(documentPath))
                    {
                        try
                        {
                            await _minioService.DeleteFileAsync(documentPath);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Impossibile eliminare documento dopo errore creazione preventivo");
                        }
                    }

                    ModelState.AddModelError("", errorMessage ?? "Errore nella creazione del preventivo.");
                    await PrepareFornitioriDropdown(model.SoggettoId);
                    PrepareStatiDropdown();
                    return View(model);
                }

                TempData["SuccessMessage"] = "Preventivo creato con successo!";
                return RedirectToAction(nameof(Index), new { lottoId = model.LottoId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nella creazione del preventivo");
                TempData["ErrorMessage"] = "Errore nella creazione del preventivo. Riprova.";
                await PrepareFornitioriDropdown(model.SoggettoId);
                PrepareStatiDropdown();
                return View(model);
            }
        }

        // ===================================
        // MODIFICA PREVENTIVO
        // ===================================

        /// <summary>
        /// GET: /Preventivi/Edit/{id}
        /// Form modifica preventivo
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var preventivo = await _preventivoService.GetByIdAsync(id);

                if (preventivo == null)
                {
                    TempData["ErrorMessage"] = "Preventivo non trovato.";
                    return RedirectToAction(nameof(Index));
                }

                // Mappa Details → Edit ViewModel (manuale perché non c'è extension)
                var model = new PreventivoEditViewModel
                {
                    Id = preventivo.Id,
                    LottoId = preventivo.LottoId,
                    SoggettoId = preventivo.SoggettoId,
                    Descrizione = preventivo.Descrizione,
                    Stato = preventivo.Stato,
                    ImportoOfferto = preventivo.ImportoOfferto,
                    DataRichiesta = preventivo.DataRichiesta,
                    DataRicezione = preventivo.DataRicezione,
                    DataScadenza = preventivo.DataScadenza,
                    GiorniAutoRinnovo = preventivo.GiorniAutoRinnovo,
                    IsSelezionato = preventivo.IsSelezionato,
                    Note = preventivo.Note,
                    DocumentPathCorrente = preventivo.DocumentPath,
                    NomeFileCorrente = preventivo.NomeFile,
                    // Info contesto
                    CodiceLotto = preventivo.CodiceLotto,
                    DescrizioneLotto = preventivo.DescrizioneLotto
                };

                // Dropdown fornitori e stati
                await PrepareFornitioriDropdown(model.SoggettoId);
                PrepareStatiDropdown(model.Stato);

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel caricamento form modifica preventivo {PreventivoId}", id);
                TempData["ErrorMessage"] = "Errore nel caricamento del preventivo.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// POST: /Preventivi/Edit/{id}
        /// Salva modifiche preventivo
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, PreventivoEditViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    await PrepareFornitioriDropdown(model.SoggettoId);
                    PrepareStatiDropdown(model.Stato);
                    return View(model);
                }

                string? newDocumentPath = null;

                // Gestione documento
                if (model.NuovoDocumentoFile != null && model.NuovoDocumentoFile.Length > 0)
                {
                    try
                    {
                        // Upload nuovo documento
                        var fileName = Path.GetFileName(model.NuovoDocumentoFile.FileName);
                        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";

                        var lotto = await _lottoService.GetByIdAsync(model.LottoId);
                        var objectPath = _minioService.GenerateObjectPath(
                            garaId: lotto.GaraId,
                            lottoId: model.LottoId,
                            fileName: uniqueFileName
                        );

                        using var stream = model.NuovoDocumentoFile.OpenReadStream();
                        newDocumentPath = await _minioService.UploadFileAsync(
                            stream,
                            objectPath,
                            model.NuovoDocumentoFile.ContentType
                        );

                        // Elimina vecchio documento se presente
                        if (!string.IsNullOrEmpty(model.DocumentPathCorrente))
                        {
                            try
                            {
                                await _minioService.DeleteFileAsync(model.DocumentPathCorrente);
                                _logger.LogInformation("Vecchio documento eliminato: {Path}", model.DocumentPathCorrente);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning(ex, "Impossibile eliminare vecchio documento: {Path}", model.DocumentPathCorrente);
                            }
                        }

                        _logger.LogInformation("Nuovo documento caricato: {Path}", newDocumentPath);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Errore durante l'upload del nuovo documento");
                        ModelState.AddModelError("DocumentoFile", "Errore durante il caricamento del file.");
                        await PrepareFornitioriDropdown(model.SoggettoId);
                        PrepareStatiDropdown(model.Stato);
                        return View(model);
                    }
                }
                else if (!model.MantieniFIleEsistente && !string.IsNullOrEmpty(model.DocumentPathCorrente))
                {
                    // Elimina documento esistente
                    try
                    {
                        await _minioService.DeleteFileAsync(model.DocumentPathCorrente);
                        newDocumentPath = string.Empty; // Segnala al service di rimuovere il path
                        _logger.LogInformation("Documento eliminato: {Path}", model.DocumentPathCorrente);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Impossibile eliminare documento: {Path}", model.DocumentPathCorrente);
                    }
                }

                // Aggiorna preventivo tramite service
                var (success, errorMessage) = await _preventivoService.UpdateAsync(model, newDocumentPath);

                if (!success)
                {
                    ModelState.AddModelError("", errorMessage ?? "Errore nell'aggiornamento del preventivo.");
                    await PrepareFornitioriDropdown(model.SoggettoId);
                    PrepareStatiDropdown(model.Stato);
                    return View(model);
                }

                TempData["SuccessMessage"] = "Preventivo aggiornato con successo!";
                return RedirectToAction(nameof(Details), new { id = model.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nell'aggiornamento del preventivo {PreventivoId}", model.Id);
                TempData["ErrorMessage"] = "Errore nell'aggiornamento del preventivo. Riprova.";
                await PrepareFornitioriDropdown(model.SoggettoId);
                PrepareStatiDropdown(model.Stato);
                return View(model);
            }
        }

        // ===================================
        // ELIMINA PREVENTIVO
        // ===================================

        /// <summary>
        /// POST: /Preventivi/Delete/{id}
        /// Elimina preventivo
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var preventivo = await _preventivoService.GetByIdAsync(id);

                if (preventivo == null)
                {
                    return Json(new { success = false, message = "Preventivo non trovato." });
                }

                var lottoId = preventivo.LottoId;

                // Elimina preventivo
                var (success, errorMessage) = await _preventivoService.DeleteAsync(id);

                if (!success)
                {
                    return Json(new { success = false, message = errorMessage ?? "Errore nell'eliminazione del preventivo." });
                }

                // Elimina documento se presente
                if (!string.IsNullOrEmpty(preventivo.DocumentPath))
                {
                    try
                    {
                        await _minioService.DeleteFileAsync(preventivo.DocumentPath);
                        _logger.LogInformation("Documento eliminato con preventivo: {Path}", preventivo.DocumentPath);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Impossibile eliminare documento del preventivo eliminato");
                    }
                }

                TempData["SuccessMessage"] = "Preventivo eliminato con successo.";
                return Json(new { success = true, redirectUrl = Url.Action(nameof(Index), new { lottoId }) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nell'eliminazione del preventivo {PreventivoId}", id);
                return Json(new { success = false, message = "Errore nell'eliminazione del preventivo." });
            }
        }

        // ===================================
        // TOGGLE SELEZIONATO (AJAX)
        // ===================================

        /// <summary>
        /// POST: /Preventivi/ToggleSelezionato/{id}
        /// Toggle flag IsSelezionato (AJAX)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ToggleSelezionato(Guid id)
        {
            try
            {
                // Chiama il nuovo metodo del service (da implementare)
                var (success, errorMessage, nuovoStato) = await _preventivoService.ToggleSelezionatoAsync(id);

                if (!success)
                {
                    return Json(new { success = false, message = errorMessage ?? "Errore nel toggle selezione." });
                }

                return Json(new
                {
                    success = true,
                    isSelezionato = nuovoStato,
                    message = nuovoStato ? "Preventivo selezionato" : "Preventivo deselezionato"//,
                    //redirectUrl = Url.Action(nameof(Index))
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel toggle selezione preventivo {PreventivoId}", id);
                return Json(new { success = false, message = "Errore nel toggle selezione." });
            }
        }

        // ===================================
        // DOWNLOAD DOCUMENTO
        // ===================================

        /// <summary>
        /// GET: /Preventivi/DownloadDocumento/{id}
        /// Download documento preventivo
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> DownloadDocumento(Guid id)
        {
            try
            {
                var preventivo = await _preventivoService.GetByIdAsync(id);

                if (preventivo == null || string.IsNullOrEmpty(preventivo.DocumentPath))
                {
                    TempData["ErrorMessage"] = "Documento non trovato.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                // Download da MinIO
                var stream = await _minioService.DownloadFileAsync(preventivo.DocumentPath);
                var fileName = preventivo.NomeFile ?? "documento.pdf";
                var contentType = "application/octet-stream";

                // Determina content type basato su estensione
                var extension = Path.GetExtension(fileName).ToLowerInvariant();
                contentType = extension switch
                {
                    ".pdf" => "application/pdf",
                    ".doc" => "application/msword",
                    ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    ".xls" => "application/vnd.ms-excel",
                    ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    _ => "application/octet-stream"
                };

                return File(stream, contentType, fileName);
            }
            catch (FileNotFoundException)
            {
                TempData["ErrorMessage"] = "File non trovato.";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel download documento preventivo {PreventivoId}", id);
                TempData["ErrorMessage"] = "Errore nel download del documento.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // ===================================
        // HELPER METHODS
        // ===================================

        /// <summary>
        /// Prepara dropdown gare
        /// </summary>
        private async Task PrepareGareDropdown(Guid? selectedId = null)
        {
            try
            {
                var gare = await _garaService.GetAllAsync();

                ViewBag.Gare = gare
                    .Select(g => new SelectListItem
                    {
                        Value = g.Id.ToString(),
                        Text = $"{g.CodiceGara} - {g.Titolo}",
                        Selected = selectedId.HasValue && g.Id == selectedId.Value
                    })
                    .OrderByDescending(x => x.Selected)
                    .ThenBy(x => x.Text)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel caricamento lista gare");
                ViewBag.Gare = new List<SelectListItem>();
            }
        }

        /// <summary>
        /// Prepara dropdown fornitori (Soggetti con IsFornitore = true)
        /// </summary>
        private async Task PrepareFornitioriDropdown(Guid? selectedId = null)
        {
            try
            {
                var fornitori = await _soggettoService.GetFornitoriAsync();

                ViewBag.Fornitori = fornitori
                    .Select(f => new SelectListItem
                    {
                        Value = f.Id.ToString(),
                        Text = f.NomeCompleto,
                        Selected = selectedId.HasValue && f.Id == selectedId.Value
                    })
                    .OrderBy(x => x.Text)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel caricamento lista fornitori");
                ViewBag.Fornitori = new List<SelectListItem>();
            }
        }

        /// <summary>
        /// Prepara dropdown stati preventivo
        /// </summary>
        private void PrepareStatiDropdown(StatoPreventivo? selected = null)
        {
            var stati = Enum.GetValues<StatoPreventivo>()
                .Select(s => new SelectListItem
                {
                    Value = ((int)s).ToString(),
                    Text = EnumHelper.GetDisplayName(s),
                    Selected = selected.HasValue && s == selected.Value
                })
                .ToList();

            ViewBag.Stati = stati;
        }

        /// <summary>
        /// Prepara dropdown per filtri
        /// </summary>
        private async Task PrepareFilterDropdowns(Guid? selectedLottoId = null, Guid? selectedSoggettoId = null)
        {
            // Stati
            ViewBag.StatiFilter = Enum.GetValues<StatoPreventivo>()
                .Select(s => new SelectListItem
                {
                    Value = ((int)s).ToString(),
                    Text = EnumHelper.GetDisplayName(s)
                })
                .ToList();

            // Fornitori
            await PrepareFornitioriDropdown(selectedSoggettoId);
        }
    }
}