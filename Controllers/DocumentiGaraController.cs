using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Trasformazioni.Configuration;
using Trasformazioni.Extensions;
using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;
using Trasformazioni.Models.ViewModels;
using Trasformazioni.Models.ViewModels.DocumentoGara;
using Trasformazioni.Services.Interfaces;

namespace Trasformazioni.Controllers
{
    [Authorize]
    public class DocumentiGaraController : Controller
    {
        private readonly IDocumentoGaraService _documentoService;
        private readonly IGaraService _garaService;
        private readonly ILottoService _lottoService;
        private readonly ILogger<DocumentiGaraController> _logger;
        private readonly FileUploadConfiguration _fileConfig;
        private readonly ITipoDocumentoService _tipoDocumentoService;

        public DocumentiGaraController(
            IDocumentoGaraService documentoService,
            IGaraService garaService,
            ILottoService lottoService,
            ILogger<DocumentiGaraController> logger,
            IOptions<FileUploadConfiguration> fileConfig,
            ITipoDocumentoService tipoDocumentoService)
        {
            _documentoService = documentoService;
            _garaService = garaService;
            _lottoService = lottoService;
            _logger = logger;
            _fileConfig = fileConfig.Value;
            _tipoDocumentoService = tipoDocumentoService;
        }

        // ===================================
        // LISTA DOCUMENTI
        // ===================================

        /// <summary>
        /// GET: /Documenti?garaId={id}&lottoId={id}
        /// Lista documenti filtrati
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index(DocumentoGaraFilterViewModel filters)
        {
            try
            {
                // Ottieni documenti filtrati
                var documenti = await _documentoService.GetFilteredAsync(filters);

                // Prepara dropdown per filtri
                await PrepareFilterDropdowns(filters.GaraId, filters.LottoId);

                // Se filtrato per gara, carica info gara per breadcrumb
                if (filters.GaraId.HasValue)
                {
                    var gara = await _garaService.GetByIdAsync(filters.GaraId.Value);
                    ViewBag.Gara = gara;
                }

                // Se filtrato per lotto, carica info lotto per breadcrumb
                if (filters.LottoId.HasValue)
                {
                    var lotto = await _lottoService.GetByIdAsync(filters.LottoId.Value);
                    ViewBag.Lotto = lotto;
                }

                ViewBag.Filters = filters;

                return View(documenti);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel caricamento della lista documenti");
                TempData["ErrorMessage"] = "Errore nel caricamento dei documenti. Riprova più tardi.";
                return View(new List<DocumentoGaraListViewModel>());
            }
        }

        // ===================================
        // DETTAGLIO DOCUMENTO
        // ===================================

        /// <summary>
        /// GET: /Documenti/Details/{id}
        /// Visualizza dettaglio completo documento
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var documento = await _documentoService.GetByIdAsync(id);

                if (documento == null)
                {
                    TempData["ErrorMessage"] = "Documento non trovato.";
                    return RedirectToAction(nameof(Index));
                }

                // Mappa a DetailsViewModel
                var viewModel = documento.ToDetailsViewModel();

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel caricamento dettaglio documento {DocumentoId}", id);
                TempData["ErrorMessage"] = "Errore nel caricamento del documento.";
                return RedirectToAction(nameof(Index));
            }
        }

        // ===================================
        // UPLOAD DOCUMENTO SINGOLO
        // ===================================

        /// <summary>
        /// GET: /Documenti/Upload?garaId={id}&lottoId={id}
        /// Form upload nuovo documento (singolo o multiplo)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Upload(Guid? garaId, Guid? lottoId)
        {
            try
            {
                var model = new DocumentoGaraUploadViewModel();

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
                    ViewBag.GaraCodice = gara.CodiceGara;
                    ViewBag.GaraTitolo = gara.Titolo;
                    ViewBag.GaraPreimpostata = true;
                }

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
                    ViewBag.LottoCodice = lotto.CodiceLotto;
                    ViewBag.LottoDescrizione = lotto.Descrizione;
                }

                ViewBag.MaxFileSizeMB = _fileConfig.MaxFileSizeMB;

                // Dropdown gare e tipi documento
                await PrepareGareDropdown(garaId);
                await PrepareLottiDropdown(garaId, lottoId);
                //PrepareTipiDocumentoDropdown();

                // DOPO
                // Determina l'area in base al contesto (se c'è lotto = Lotti, altrimenti = Gare)
                AreaDocumento? area = lottoId.HasValue ? AreaDocumento.Lotti :
                                garaId.HasValue ? AreaDocumento.Gare : null;
                await PrepareTipiDocumentoDropdownAsync(area);

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel caricamento form upload documento");
                TempData["ErrorMessage"] = "Errore nel caricamento del form.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// POST: /Documenti/Upload
        /// Salva nuovo documento (singolo)
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(DocumentoGaraUploadViewModel model, IFormFile? file)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await PrepareGareDropdown(model.GaraId);
                    await PrepareLottiDropdown(model.GaraId, model.LottoId);
                    //PrepareTipiDocumentoDropdown(model.Tipo);
                    // DOPO
                    var area = model.LottoId.HasValue ? AreaDocumento.Lotti : AreaDocumento.Gare;
                    await PrepareTipiDocumentoDropdownAsync(area, model.TipoDocumentoId);
                    return View(model);
                }

                if (file == null || file.Length == 0)
                {
                    ModelState.AddModelError("", "Selezionare un file da caricare.");
                    await PrepareGareDropdown(model.GaraId);
                    await PrepareLottiDropdown(model.GaraId, model.LottoId);
                    //PrepareTipiDocumentoDropdown(model.Tipo);
                    // DOPO
                    var area = model.LottoId.HasValue ? AreaDocumento.Lotti : AreaDocumento.Gare;
                    await PrepareTipiDocumentoDropdownAsync(area, model.TipoDocumentoId);
                    return View(model);
                }

                // Validazione file
                var (isValid, errorMessage) = _documentoService.ValidateFile(file);
                if (!isValid)
                {
                    ModelState.AddModelError("", errorMessage ?? "File non valido.");
                    await PrepareGareDropdown(model.GaraId);
                    await PrepareLottiDropdown(model.GaraId, model.LottoId);
                    //PrepareTipiDocumentoDropdown(model.Tipo);
                    // DOPO
                    var area = model.LottoId.HasValue ? AreaDocumento.Lotti : AreaDocumento.Gare;
                    await PrepareTipiDocumentoDropdownAsync(area, model.TipoDocumentoId);
                    return View(model);
                }

                // Upload documento
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
                var documento = await _documentoService.UploadAsync(file, model, userId);

                _logger.LogInformation("Documento {DocumentoId} caricato con successo da {UserId}",
                    documento.Id, userId);

                TempData["SuccessMessage"] = "Documento caricato con successo!";
                return RedirectToAction(nameof(Index), new { garaId = model.GaraId, lottoId = model.LottoId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nell'upload del documento per Gara {GaraId}", model.GaraId);
                ModelState.AddModelError("", "Errore durante il caricamento del documento. Riprova.");
                TempData["ErrorMessage"] = "Errore durante il caricamento del documento. Riprova.";
                await PrepareGareDropdown(model.GaraId);
                await PrepareLottiDropdown(model.GaraId, model.LottoId);
                //PrepareTipiDocumentoDropdown(model.Tipo);
                // DOPO
                var area = model.LottoId.HasValue ? AreaDocumento.Lotti : AreaDocumento.Gare;
                await PrepareTipiDocumentoDropdownAsync(area, model.TipoDocumentoId);
                return View(model);
            }
        }

        // ===================================
        // UPLOAD MULTIPLO
        // ===================================

        /// <summary>
        /// POST: /Documenti/UploadMultiple
        /// Upload multiplo documenti (Opzione C: tipo comune, descrizioni individuali opzionali)
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadMultiple(
            Guid garaId,
            Guid? lottoId,
            //TipoDocumentoGara tipo, // PRIMA
            Guid tipoDocumentoId, // DOPO
            List<IFormFile> files,
            List<string>? descrizioni)
        {
            try
            {
                if (files == null || !files.Any())
                {
                    return Json(new { success = false, message = "Nessun file selezionato." });
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
                var uploadedCount = 0;
                var errors = new List<string>();

                // Upload ogni file
                for (int i = 0; i < files.Count; i++)
                {
                    var file = files[i];

                    // Skip file vuoti
                    if (file == null || file.Length == 0)
                        continue;

                    try
                    {
                        // Validazione file
                        var (isValid, errorMessage) = _documentoService.ValidateFile(file);
                        if (!isValid)
                        {
                            errors.Add($"{file.FileName}: {errorMessage}");
                            continue;
                        }

                        // Crea ViewModel per questo file
                        var viewModel = new DocumentoGaraUploadViewModel
                        {
                            GaraId = garaId,
                            LottoId = lottoId,
                            //Tipo = tipo, // PRIMA
                            TipoDocumentoId = tipoDocumentoId, // DOPO
                            Descrizione = descrizioni != null && i < descrizioni.Count
                                ? descrizioni[i]
                                : null
                        };

                        // Upload
                        await _documentoService.UploadAsync(file, viewModel, userId);
                        uploadedCount++;

                        _logger.LogInformation(
                            "Documento {FileName} caricato (upload multiplo) da {UserId}",
                            file.FileName, userId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Errore upload file {FileName}", file.FileName);
                        errors.Add($"{file.FileName}: errore durante il caricamento");
                    }
                }

                if (uploadedCount == files.Count)
                {
                    return Json(new
                    {
                        success = true,
                        message = $"Tutti i {uploadedCount} file caricati con successo!",
                        count = uploadedCount
                    });
                }
                else if (uploadedCount > 0)
                {
                    return Json(new
                    {
                        success = true,
                        message = $"{uploadedCount} file caricati su {files.Count}. Alcuni errori riscontrati.",
                        count = uploadedCount,
                        errors = errors
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "Nessun file caricato. Errori: " + string.Join(", ", errors),
                        errors = errors
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nell'upload multiplo per Gara {GaraId}", garaId);
                return Json(new { success = false, message = "Errore durante l'upload multiplo." });
            }
        }

        // ===================================
        // MODIFICA DOCUMENTO
        // ===================================

        /// <summary>
        /// GET: /Documenti/Edit/{id}
        /// Form modifica metadati documento (NON modifica il file)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var documento = await _documentoService.GetByIdAsync(id);

                if (documento == null)
                {
                    TempData["ErrorMessage"] = "Documento non trovato.";
                    return RedirectToAction(nameof(Index));
                }

                // Mappa a EditViewModel
                var model = documento.ToEditViewModel();

                // Dropdown tipi documento
                //PrepareTipiDocumentoDropdown(model.Tipo);
                // DOPO
                // Per Edit, carichiamo tutti i tipi (Gare + Lotti) per permettere la modifica
                await PrepareTipiDocumentoDropdownAsync(model.TipoDocumentoId);

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel caricamento form modifica documento {DocumentoId}", id);
                TempData["ErrorMessage"] = "Errore nel caricamento del documento.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// POST: /Documenti/Edit/{id}
        /// Salva modifiche metadati documento
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, DocumentoGaraEditViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    //PrepareTipiDocumentoDropdown(model.Tipo);
                    // DOPO
                    await PrepareTipiDocumentoDropdownAsync(model.TipoDocumentoId);
                    return View(model);
                }

                // Aggiorna documento
                await _documentoService.UpdateAsync(id, model);

                _logger.LogInformation("Documento {DocumentoId} aggiornato con successo", id);

                TempData["SuccessMessage"] = "Documento aggiornato con successo!";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nell'aggiornamento del documento {DocumentoId}", id);
                ModelState.AddModelError("", "Errore nell'aggiornamento del documento. Riprova.");
                //PrepareTipiDocumentoDropdown(model.Tipo);
                // DOPO
                await PrepareTipiDocumentoDropdownAsync(model.TipoDocumentoId);
                return View(model);
            }
        }

        // ===================================
        // ELIMINA DOCUMENTO
        // ===================================

        /// <summary>
        /// POST: /Documenti/Delete/{id}
        /// Elimina documento (soft delete)
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, Guid? garaId, Guid? lottoId)
        {
            try
            {
                var documento = await _documentoService.GetByIdAsync(id);

                if (documento == null)
                {
                    return Json(new { success = false, message = "Documento non trovato." });
                }

                // Elimina documento (soft delete)
                await _documentoService.DeleteAsync(id);

                _logger.LogInformation("Documento {DocumentoId} eliminato con successo", id);

                TempData["SuccessMessage"] = "Documento eliminato con successo.";

                return Json(new
                {
                    success = true,
                    redirectUrl = Url.Action(nameof(Index), new { garaId, lottoId })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nell'eliminazione del documento {DocumentoId}", id);
                return Json(new { success = false, message = "Errore nell'eliminazione del documento." });
            }
        }

        // ===================================
        // DOWNLOAD DOCUMENTO
        // ===================================

        /// <summary>
        /// GET: /Documenti/Download/{id}
        /// Download file documento
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Download(Guid id)
        {
            try
            {
                var (fileStream, fileName, contentType) = await _documentoService.DownloadAsync(id);

                _logger.LogInformation("Download documento {DocumentoId}: {FileName}", id, fileName);

                return File(fileStream, contentType, fileName);
            }
            catch (FileNotFoundException)
            {
                TempData["ErrorMessage"] = "File non trovato.";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel download del documento {DocumentoId}", id);
                TempData["ErrorMessage"] = "Errore nel download del documento.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        /// <summary>
        /// GET: /DocumentiGara/ViewInline/{id}
        /// Visualizzazione inline del documento (Content-Disposition: inline)
        /// Usato per preview PDF e immagini senza forzare download
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ViewInline_OLD(Guid id)
        {
            try
            {
                var (fileStream, fileName, contentType) = await _documentoService.DownloadAsync(id);

                _logger.LogInformation("Visualizzazione inline documento {DocumentoId}: {FileName}", id, fileName);

                // Content-Disposition: inline (non attachment)
                Response.Headers.Add("Content-Disposition", $"inline; filename=\"{fileName}\"");

                return File(fileStream, contentType);
            }
            catch (FileNotFoundException)
            {
                TempData["ErrorMessage"] = "File non trovato.";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nella visualizzazione del documento {DocumentoId}", id);
                TempData["ErrorMessage"] = "Errore nella visualizzazione del documento.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }
        /// <summary>
        /// GET: /DocumentiGara/ViewInline/{id}
        /// Visualizzazione inline del documento (Content-Disposition: inline)
        /// Usato per preview PDF e immagini senza forzare download
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ViewInline(Guid id)
        {
            try
            {
                _logger.LogInformation("Richiesta visualizzazione inline documento {DocumentoId}", id);

                var documento = await _documentoService.GetByIdAsync(id);
                if (documento == null)
                {
                    _logger.LogWarning("Documento {DocumentoId} non trovato nel database", id);
                    return NotFound("Documento non trovato");
                }

                _logger.LogInformation(
                    "Documento trovato: {NomeFile}, Path: {Path}, MimeType: {MimeType}",
                    documento.NomeFile, documento.PathMinIO, documento.MimeType);

                var (fileStream, fileName, contentType) = await _documentoService.DownloadAsync(id);

                if (fileStream == null || fileStream.Length == 0)
                {
                    _logger.LogError("Stream vuoto per documento {DocumentoId}", id);
                    return NotFound("File vuoto o non disponibile");
                }

                _logger.LogInformation(
                    "Stream ottenuto per documento {DocumentoId}: {FileName}, Size: {Size} bytes",
                    id, fileName, fileStream.Length);

                // Content-Disposition: inline (non attachment)
                Response.Headers.Add("Content-Disposition", $"inline; filename=\"{fileName}\"");

                return File(fileStream, contentType);
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogWarning(ex, "File non trovato per documento {DocumentoId}", id);
                return NotFound("File non trovato su MinIO");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nella visualizzazione del documento {DocumentoId}", id);
                return StatusCode(500, $"Errore nella visualizzazione del documento: {ex.Message}");
            }
        }

        // ===================================
        // API METHODS (AJAX)
        // ===================================

        /// <summary>
        /// GET: /DocumentiGara/GetLottiByGara?garaId={id}
        /// Ottiene lotti per una gara (per filtro dipendente AJAX)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetLottiByGara(Guid garaId)
        {
            try
            {
                var lotti = await _lottoService.GetByGaraIdAsync(garaId);

                var result = lotti.Select(l => new
                {
                    id = l.Id,
                    codiceLotto = l.CodiceLotto,
                    descrizione = l.Descrizione
                });

                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel caricamento lotti per gara {GaraId}", garaId);
                return Json(new List<object>());
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
        /// Prepara dropdown lotti (filtrati per gara)
        /// </summary>
        private async Task PrepareLottiDropdown(Guid? garaId, Guid? selectedId = null)
        {
            try
            {
                if (garaId.HasValue)
                {
                    var lotti = await _lottoService.GetByGaraIdAsync(garaId.Value);

                    ViewBag.Lotti = lotti
                        .Select(l => new SelectListItem
                        {
                            Value = l.Id.ToString(),
                            Text = $"{l.CodiceLotto} - {l.Descrizione}",
                            Selected = selectedId.HasValue && l.Id == selectedId.Value
                        })
                        .OrderBy(x => x.Text)
                        .ToList();
                }
                else
                {
                    ViewBag.Lotti = new List<SelectListItem>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel caricamento lista lotti per gara {GaraId}", garaId);
                ViewBag.Lotti = new List<SelectListItem>();
            }
        }

        ///// <summary>
        ///// Prepara dropdown tipi documento (raggruppati per categoria)
        ///// </summary>
        //private void PrepareTipiDocumentoDropdown(TipoDocumentoGara? selected = null)
        //{
        //    var tipi = new List<SelectListGroup>
        //    {
        //        new SelectListGroup { Name = "📋 Livello Gara" },
        //        new SelectListGroup { Name = "✅ Valutazione" },
        //        new SelectListGroup { Name = "📝 Elaborazione" },
        //        new SelectListGroup { Name = "🔄 Integrazioni" },
        //        new SelectListGroup { Name = "📄 Altro" }
        //    };

        //    var items = new List<SelectListItem>
        //    {
        //        // Livello Gara
        //        new SelectListItem
        //        {
        //            Value = ((int)TipoDocumentoGara.DocumentoGeneraleGara).ToString(),
        //            Text = "Documento Generale",
        //            Group = tipi[0],
        //            Selected = selected == TipoDocumentoGara.DocumentoGeneraleGara
        //        },
        //        new SelectListItem
        //        {
        //            Value = ((int)TipoDocumentoGara.BandoOriginale).ToString(),
        //            Text = "Bando Originale",
        //            Group = tipi[0],
        //            Selected = selected == TipoDocumentoGara.BandoOriginale
        //        },
        //        new SelectListItem
        //        {
        //            Value = ((int)TipoDocumentoGara.Disciplinare).ToString(),
        //            Text = "Disciplinare",
        //            Group = tipi[0],
        //            Selected = selected == TipoDocumentoGara.Disciplinare
        //        },
        //        new SelectListItem
        //        {
        //            Value = ((int)TipoDocumentoGara.CapitolatoSpeciale).ToString(),
        //            Text = "Capitolato Speciale",
        //            Group = tipi[0],
        //            Selected = selected == TipoDocumentoGara.CapitolatoSpeciale
        //        },

        //        // Valutazione
        //        new SelectListItem
        //        {
        //            Value = ((int)TipoDocumentoGara.DocumentoValutazioneTecnica).ToString(),
        //            Text = "Documento Valutazione Tecnica",
        //            Group = tipi[1],
        //            Selected = selected == TipoDocumentoGara.DocumentoValutazioneTecnica
        //        },
        //        new SelectListItem
        //        {
        //            Value = ((int)TipoDocumentoGara.DocumentoValutazioneEconomica).ToString(),
        //            Text = "Documento Valutazione Economica",
        //            Group = tipi[1],
        //            Selected = selected == TipoDocumentoGara.DocumentoValutazioneEconomica
        //        },
        //        // rimosso -> gestione preventivi separata e richiede form dedicato
        //        //new SelectListItem
        //        //{
        //        //    Value = ((int)TipoDocumentoGara.Preventivo).ToString(),
        //        //    Text = "Preventivo",
        //        //    Group = tipi[1],
        //        //    Selected = selected == TipoDocumentoGara.Preventivo
        //        //},

        //        // Elaborazione
        //        new SelectListItem
        //        {
        //            Value = ((int)TipoDocumentoGara.DocumentoPresentazione).ToString(),
        //            Text = "Documento Presentazione",
        //            Group = tipi[2],
        //            Selected = selected == TipoDocumentoGara.DocumentoPresentazione
        //        },
        //        new SelectListItem
        //        {
        //            Value = ((int)TipoDocumentoGara.OffertaTecnica).ToString(),
        //            Text = "Offerta Tecnica",
        //            Group = tipi[2],
        //            Selected = selected == TipoDocumentoGara.OffertaTecnica
        //        },
        //        new SelectListItem
        //        {
        //            Value = ((int)TipoDocumentoGara.OffertaEconomica).ToString(),
        //            Text = "Offerta Economica",
        //            Group = tipi[2],
        //            Selected = selected == TipoDocumentoGara.OffertaEconomica
        //        },

        //        // Integrazioni
        //        new SelectListItem
        //        {
        //            Value = ((int)TipoDocumentoGara.RichiestaIntegrazioneEnte).ToString(),
        //            Text = "Richiesta Integrazione Ente",
        //            Group = tipi[3],
        //            Selected = selected == TipoDocumentoGara.RichiestaIntegrazioneEnte
        //        },
        //        new SelectListItem
        //        {
        //            Value = ((int)TipoDocumentoGara.RispostaIntegrazione).ToString(),
        //            Text = "Risposta Integrazione",
        //            Group = tipi[3],
        //            Selected = selected == TipoDocumentoGara.RispostaIntegrazione
        //        },

        //        // Altro
        //        new SelectListItem
        //        {
        //            Value = ((int)TipoDocumentoGara.ComunicazioneEnte).ToString(),
        //            Text = "Comunicazione Ente",
        //            Group = tipi[4],
        //            Selected = selected == TipoDocumentoGara.ComunicazioneEnte
        //        },
        //        new SelectListItem
        //        {
        //            Value = ((int)TipoDocumentoGara.Contratto).ToString(),
        //            Text = "Contratto",
        //            Group = tipi[4],
        //            Selected = selected == TipoDocumentoGara.Contratto
        //        },
        //        new SelectListItem
        //        {
        //            Value = ((int)TipoDocumentoGara.Altro).ToString(),
        //            Text = "Altro",
        //            Group = tipi[4],
        //            Selected = selected == TipoDocumentoGara.Altro
        //        }
        //    };

        //    ViewBag.TipiDocumento = items;
        //}

        // DOPO
        /// <summary>
        /// Prepara dropdown tipi documento caricando dal DB
        /// Filtra per area in base al contesto (Gare o Lotti)
        /// </summary>
        /// <param name="area">Area documento (Gare o Lotti)</param>
        /// <param name="selectedId">ID del tipo selezionato (per edit)</param>
        private async Task PrepareTipiDocumentoDropdownAsync(AreaDocumento? area, Guid? selectedId = null)
        {
            try
            {
                var tipi = await _tipoDocumentoService.GetForDropdownAsync(area);

                //ViewBag.TipiDocumento = tipi
                //    .OrderBy(t => t.Nome)
                //    .Select(t => new SelectListItem
                //    {
                //        Value = t.Id.ToString(),
                //        Text = t.Nome,
                //        Selected = selectedId.HasValue && t.Id == selectedId.Value
                //    })
                //    .ToList();

                // Raggruppa per area
                var items = new List<SelectListItem>();
                var gruppi = tipi
                    .GroupBy(t => t.Area)
                    .OrderBy(g => g.Key);

                foreach (var gruppo in gruppi)
                {
                    var groupName = gruppo.Key.ToString();
                    var selectGroup = new SelectListGroup { Name = groupName };

                    foreach (var tipo in gruppo.OrderBy(t => t.Nome))
                    {
                        items.Add(new SelectListItem
                        {
                            Value = tipo.Id.ToString(),
                            Text = tipo.Nome,
                            Group = selectGroup,
                            Selected = selectedId.HasValue && tipo.Id == selectedId.Value
                        });
                    }
                }

                ViewBag.TipiDocumento = items;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel caricamento tipi documento per area {Area}", area);
                ViewBag.TipiDocumento = new List<SelectListItem>();
            }
        }

        /// <summary>
        /// Prepara dropdown con tutti i tipi documento (per filtri)
        /// </summary>
        private async Task PrepareTipiDocumentoDropdownAsync(Guid? selectedId = null)
        {
            try
            {
                var tipi = await _tipoDocumentoService.GetForDropdownAsync();

                // Raggruppa per area
                var items = new List<SelectListItem>();
                var gruppi = tipi
                    .GroupBy(t => t.Area)
                    .OrderBy(g => g.Key);

                foreach (var gruppo in gruppi)
                {
                    var groupName = gruppo.Key.ToString();
                    var selectGroup = new SelectListGroup { Name = groupName };

                    foreach (var tipo in gruppo.OrderBy(t => t.Nome))
                    {
                        items.Add(new SelectListItem
                        {
                            Value = tipo.Id.ToString(),
                            Text = tipo.Nome,
                            Group = selectGroup,
                            Selected = selectedId.HasValue && tipo.Id == selectedId.Value
                        });
                    }
                }

                ViewBag.TipiDocumento = items;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel caricamento tipi documento");
                ViewBag.TipiDocumento = new List<SelectListItem>();
            }
        }

        /// <summary>
        /// Prepara dropdown per filtri
        /// </summary>
        private async Task PrepareFilterDropdowns(Guid? selectedGaraId = null, Guid? selectedLottoId = null)
        {
            // Gare
            await PrepareGareDropdown(selectedGaraId);

            // Lotti (se gara selezionata)
            await PrepareLottiDropdown(selectedGaraId, selectedLottoId);

            // Tipi documento
            //PrepareTipiDocumentoDropdown();
            // DOPO
            await PrepareTipiDocumentoDropdownAsync();
        }
    }
}