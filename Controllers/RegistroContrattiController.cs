using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Minio.DataModel.Notification;
using System.Security.Claims;
using System.Text;
using Trasformazioni.Models.Enums;
using Trasformazioni.Models.ViewModels;
using Trasformazioni.Services.Interfaces;

namespace Trasformazioni.Controllers
{
    /// <summary>
    /// Controller per la gestione del Registro Contratti e Preventivi
    /// </summary>
    [Authorize]
    public class RegistroContrattiController : Controller
    {
        private readonly IRegistroContrattiService _registroService;
        private readonly IAllegatoRegistroService _allegatoService;
        private readonly ICategoriaContrattoService _categoriaService;
        private readonly ILogger<RegistroContrattiController> _logger;

        public RegistroContrattiController(
            IRegistroContrattiService registroService,
            IAllegatoRegistroService allegatoService,
            ICategoriaContrattoService categoriaService,
            ILogger<RegistroContrattiController> logger)
        {
            _registroService = registroService;
            _allegatoService = allegatoService;
            _categoriaService = categoriaService;
            _logger = logger;
        }

        #region Helper Methods

        /// <summary>
        /// Ottiene l'ID dell'utente corrente
        /// </summary>
        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        }

        #endregion

        #region READ - Visualizzazione

        /// <summary>
        /// GET: /RegistroContratti
        /// Lista paginata dei registri con filtri
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index(RegistroContrattiFilterViewModel filters)
        {
            try
            {
                // Prepara filtri con dropdown
                filters = await _registroService.PrepareFilterViewModelAsync(filters);

                // Ottieni risultati paginati
                var pagedResult = await _registroService.GetPagedAsync(filters);

                // Statistiche per dashboard
                ViewBag.Statistiche = await _registroService.GetStatisticheAsync();
                ViewBag.CurrentFilters = filters;

                return View(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento della lista registri");
                TempData["ErrorMessage"] = "Errore durante il caricamento dei registri. Riprova.";
                return View(new PagedResult<RegistroContrattiListViewModel>());
            }
        }

        /// <summary>
        /// GET: /RegistroContratti/Preventivi
        /// Lista filtrata per soli preventivi
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Preventivi(RegistroContrattiFilterViewModel filters)
        {
            filters.TipoRegistro = TipoRegistro.Preventivo;
            return RedirectToAction(nameof(Index), filters);
        }

        /// <summary>
        /// GET: /RegistroContratti/Contratti
        /// Lista filtrata per soli contratti
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Contratti(RegistroContrattiFilterViewModel filters)
        {
            filters.TipoRegistro = TipoRegistro.Contratto;
            return RedirectToAction(nameof(Index), filters);
        }

        /// <summary>
        /// GET: /RegistroContratti/InScadenza
        /// Lista contratti in scadenza
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> InScadenza(RegistroContrattiFilterViewModel filters)
        {
            try
            {
                // sostituiti per utilizzare la stessa index con filtri invece della view separata
                //var registri = await _registroService.GetInScadenzaAsync();
                //ViewBag.Title = "Contratti in Scadenza";
                //return View("Lista", registri);

                filters.SoloInScadenza = true;
                filters.SoloScaduti = false;
                ViewData["Title"] = "Contratti in Scadenza";
                return RedirectToAction(nameof(Index), filters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento dei contratti in scadenza");
                TempData["ErrorMessage"] = "Errore durante il caricamento. Riprova.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// GET: /RegistroContratti/Scaduti
        /// Lista contratti scaduti
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Scaduti(RegistroContrattiFilterViewModel filters)
        {
            try
            {
                // sostituiti per utilizzare la stessa index con filtri invece della view separata
                //var registri = await _registroService.GetScadutiAsync();
                //ViewBag.Title = "Contratti Scaduti";
                //return View("Lista", registri);

                filters.SoloScaduti = true;
                filters.SoloInScadenza = false;
                ViewData["Title"] = "Contratti Scaduti";
                return RedirectToAction(nameof(Index), filters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento dei contratti scaduti");
                TempData["ErrorMessage"] = "Errore durante il caricamento. Riprova.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// GET: /RegistroContratti/Details/5
        /// Dettagli completi di un registro con allegati
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var registro = await _registroService.GetByIdAsync(id);

                if (registro == null)
                {
                    _logger.LogWarning("Registro con ID {RegistroId} non trovato", id);
                    TempData["ErrorMessage"] = "Registro non trovato.";
                    return RedirectToAction(nameof(Index));
                }

                return View(registro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento dei dettagli registro {RegistroId}", id);
                TempData["ErrorMessage"] = "Errore durante il caricamento dei dettagli. Riprova.";
                return RedirectToAction(nameof(Index));
            }
        }

        #endregion

        #region CREATE

        /// <summary>
        /// GET: /RegistroContratti/Create
        /// Form per creare un nuovo registro
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Create(TipoRegistro? tipo = null, Guid? parentId = null)
        {
            try
            {
                var model = await _registroService.PrepareCreateViewModelAsync(parentId);

                if (tipo.HasValue)
                {
                    model.TipoRegistro = tipo.Value;
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la preparazione del form di creazione");
                TempData["ErrorMessage"] = "Errore durante il caricamento del form. Riprova.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// POST: /RegistroContratti/Create
        /// Salva un nuovo registro
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegistroContrattiCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model = await _registroService.PrepareCreateViewModelAsync(model.ParentId);
                // Ripristina i valori inseriti
                return View(model);
            }

            try
            {
                // Verifica unicità numero protocollo
                if (!string.IsNullOrWhiteSpace(model.NumeroProtocollo))
                {
                    if (!await _registroService.IsNumeroProtocolloUniqueAsync(model.NumeroProtocollo))
                    {
                        ModelState.AddModelError(nameof(model.NumeroProtocollo), "Esiste già un registro con questo numero protocollo.");
                        model = await _registroService.PrepareCreateViewModelAsync(model.ParentId);
                        return View(model);
                    }
                }

                var (success, errorMessage, registroId) = await _registroService.CreateAsync(model, GetCurrentUserId());

                if (!success)
                {
                    ModelState.AddModelError("", errorMessage ?? "Errore durante la creazione del registro.");
                    model = await _registroService.PrepareCreateViewModelAsync(model.ParentId);
                    return View(model);
                }

                var tipoDesc = model.TipoRegistro == TipoRegistro.Preventivo ? "Preventivo" : "Contratto";
                _logger.LogInformation("{Tipo} creato con successo - ID: {RegistroId} da {User}",
                    tipoDesc, registroId, User.Identity?.Name);

                TempData["SuccessMessage"] = $"{tipoDesc} creato con successo!";
                return RedirectToAction(nameof(Details), new { id = registroId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la creazione del registro");
                ModelState.AddModelError("", "Errore durante la creazione. Riprova.");
                model = await _registroService.PrepareCreateViewModelAsync(model.ParentId);
                return View(model);
            }
        }

        #endregion

        #region EDIT

        /// <summary>
        /// GET: /RegistroContratti/Edit/5
        /// Form per modificare un registro
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var model = await _registroService.PrepareEditViewModelAsync(id);

                if (model == null)
                {
                    _logger.LogWarning("Registro con ID {RegistroId} non trovato per modifica", id);
                    TempData["ErrorMessage"] = "Registro non trovato.";
                    return RedirectToAction(nameof(Index));
                }

                // Verifica modificabilità
                if (!model.CanChangeStato && model.Stato == StatoRegistro.Annullato)
                {
                    TempData["ErrorMessage"] = "Non è possibile modificare un registro annullato.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento del registro per modifica {RegistroId}", id);
                TempData["ErrorMessage"] = "Errore durante il caricamento. Riprova.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// POST: /RegistroContratti/Edit/5
        /// Salva le modifiche a un registro
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, RegistroContrattiEditViewModel model)
        {
            if (id != model.Id)
            {
                TempData["ErrorMessage"] = "ID non corrispondente.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                model = await _registroService.PrepareEditViewModelAsync(id);
                return View(model);
            }

            try
            {
                // Verifica esistenza
                if (!await _registroService.ExistsAsync(id))
                {
                    TempData["ErrorMessage"] = "Registro non trovato.";
                    return RedirectToAction(nameof(Index));
                }

                // Verifica unicità numero protocollo (se modificato)
                if (!string.IsNullOrWhiteSpace(model.NumeroProtocollo))
                {
                    if (!await _registroService.IsNumeroProtocolloUniqueAsync(model.NumeroProtocollo, id))
                    {
                        ModelState.AddModelError(nameof(model.NumeroProtocollo), "Esiste già un altro registro con questo numero protocollo.");
                        model = await _registroService.PrepareEditViewModelAsync(id);
                        return View(model);
                    }
                }

                var (success, errorMessage) = await _registroService.UpdateAsync(model, GetCurrentUserId());

                if (!success)
                {
                    ModelState.AddModelError("", errorMessage ?? "Errore durante il salvataggio delle modifiche.");
                    model = await _registroService.PrepareEditViewModelAsync(id);
                    return View(model);
                }

                _logger.LogInformation("Registro {RegistroId} modificato con successo da {User}",
                    id, User.Identity?.Name);

                TempData["SuccessMessage"] = "Registro modificato con successo!";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la modifica del registro {RegistroId}", id);
                ModelState.AddModelError("", "Errore durante il salvataggio delle modifiche. Riprova.");
                model = await _registroService.PrepareEditViewModelAsync(id);
                return View(model);
            }
        }

        #endregion

        #region DELETE

        /// <summary>
        /// POST: /RegistroContratti/Delete/5
        /// Elimina un registro (soft delete)
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var registro = await _registroService.GetByIdAsync(id);

                if (registro == null)
                {
                    _logger.LogWarning("Tentativo di eliminazione registro inesistente: {RegistroId}", id);
                    return Json(new { success = false, message = "Registro non trovato." });
                }

                // Verifica eliminabilità (solo bozze senza figli)
                if (!registro.CanDelete)
                {
                    return Json(new
                    {
                        success = false,
                        message = "È possibile eliminare solo registri in stato Bozza senza versioni successive."
                    });
                }

                var (success, errorMessage) = await _registroService.DeleteAsync(id, GetCurrentUserId());

                if (!success)
                {
                    return Json(new { success = false, message = errorMessage ?? "Errore durante l'eliminazione." });
                }

                _logger.LogInformation("Registro {RegistroId} eliminato da {User}",
                    id, User.Identity?.Name);

                return Json(new
                {
                    success = true,
                    message = "Registro eliminato con successo!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'eliminazione del registro {RegistroId}", id);
                return Json(new
                {
                    success = false,
                    message = "Errore durante l'eliminazione. Riprova."
                });
            }
        }

        #endregion

        #region WORKFLOW - Gestione Stati

        /// <summary>
        /// POST: /RegistroContratti/CambiaStato/5
        /// Cambia lo stato di un registro
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiaStato(Guid id, StatoRegistro nuovoStato)
        {
            try
            {
                var registro = await _registroService.GetByIdAsync(id);

                if (registro == null)
                {
                    return Json(new { success = false, message = "Registro non trovato." });
                }

                var (success, errorMessage) = await _registroService.CambiaStatoAsync(id, nuovoStato, GetCurrentUserId());

                if (!success)
                {
                    return Json(new { success = false, message = errorMessage ?? "Errore durante il cambio stato." });
                }

                _logger.LogInformation("Stato registro {RegistroId} cambiato da {VecchioStato} a {NuovoStato} da {User}",
                    id, registro.Stato, nuovoStato, User.Identity?.Name);

                return Json(new
                {
                    success = true,
                    message = $"Stato cambiato in '{GetStatoDescrizione(nuovoStato)}' con successo!",
                    nuovoStato = nuovoStato.ToString()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il cambio stato registro {RegistroId}", id);
                return Json(new { success = false, message = "Errore durante il cambio stato. Riprova." });
            }
        }

        /// <summary>
        /// POST: /RegistroContratti/Invia/5
        /// Invia il documento al cliente
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Invia(Guid id)
        {
            try
            {
                var (success, errorMessage) = await _registroService.InviaAsync(id, GetCurrentUserId());

                if (!success)
                {
                    return Json(new { success = false, message = errorMessage ?? "Errore durante l'invio." });
                }

                _logger.LogInformation("Registro {RegistroId} inviato da {User}", id, User.Identity?.Name);

                return Json(new
                {
                    success = true,
                    message = "Documento inviato con successo!",
                    nuovoStato = StatoRegistro.Inviato.ToString()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'invio del registro {RegistroId}", id);
                return Json(new { success = false, message = "Errore durante l'invio. Riprova." });
            }
        }

        /// <summary>
        /// POST: /RegistroContratti/Attiva/5
        /// Attiva un contratto
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Attiva(Guid id, DateTime? dataAccettazione = null)
        {
            try
            {
                var (success, errorMessage) = await _registroService.AttivaAsync(id, dataAccettazione, GetCurrentUserId());

                if (!success)
                {
                    return Json(new { success = false, message = errorMessage ?? "Errore durante l'attivazione." });
                }

                _logger.LogInformation("Registro {RegistroId} attivato da {User}", id, User.Identity?.Name);

                return Json(new
                {
                    success = true,
                    message = "Contratto attivato con successo!",
                    nuovoStato = StatoRegistro.Attivo.ToString()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'attivazione del registro {RegistroId}", id);
                return Json(new { success = false, message = "Errore durante l'attivazione. Riprova." });
            }
        }

        /// <summary>
        /// POST: /RegistroContratti/Sospendi/5
        /// Sospende un contratto
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sospendi(Guid id)
        {
            try
            {
                var (success, errorMessage) = await _registroService.SospendiAsync(id, GetCurrentUserId());

                if (!success)
                {
                    return Json(new { success = false, message = errorMessage ?? "Errore durante la sospensione." });
                }

                _logger.LogInformation("Registro {RegistroId} sospeso da {User}", id, User.Identity?.Name);

                return Json(new
                {
                    success = true,
                    message = "Contratto sospeso con successo!",
                    nuovoStato = StatoRegistro.Sospeso.ToString()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la sospensione del registro {RegistroId}", id);
                return Json(new { success = false, message = "Errore durante la sospensione. Riprova." });
            }
        }

        /// <summary>
        /// POST: /RegistroContratti/Riattiva/5
        /// Riattiva un contratto sospeso
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Riattiva(Guid id)
        {
            try
            {
                var (success, errorMessage) = await _registroService.RiattivaAsync(id, GetCurrentUserId());

                if (!success)
                {
                    return Json(new { success = false, message = errorMessage ?? "Errore durante la riattivazione." });
                }

                _logger.LogInformation("Registro {RegistroId} riattivato da {User}", id, User.Identity?.Name);

                return Json(new
                {
                    success = true,
                    message = "Contratto riattivato con successo!",
                    nuovoStato = StatoRegistro.Attivo.ToString()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la riattivazione del registro {RegistroId}", id);
                return Json(new { success = false, message = "Errore durante la riattivazione. Riprova." });
            }
        }

        /// <summary>
        /// POST: /RegistroContratti/Annulla/5
        /// Annulla un documento
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Annulla(Guid id)
        {
            try
            {
                var (success, errorMessage) = await _registroService.AnnullaAsync(id, GetCurrentUserId());

                if (!success)
                {
                    return Json(new { success = false, message = errorMessage ?? "Errore durante l'annullamento." });
                }

                _logger.LogInformation("Registro {RegistroId} annullato da {User}", id, User.Identity?.Name);

                return Json(new
                {
                    success = true,
                    message = "Documento annullato con successo!",
                    nuovoStato = StatoRegistro.Annullato.ToString()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'annullamento del registro {RegistroId}", id);
                return Json(new { success = false, message = "Errore durante l'annullamento. Riprova." });
            }
        }

        /// <summary>
        /// POST: /RegistroContratti/ProponiRinnovo/5
        /// Propone il rinnovo per un contratto in scadenza
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProponiRinnovo(Guid id)
        {
            try
            {
                var (success, errorMessage) = await _registroService.ProponiRinnovoAsync(id, GetCurrentUserId());

                if (!success)
                {
                    return Json(new { success = false, message = errorMessage ?? "Errore durante la proposta di rinnovo." });
                }

                _logger.LogInformation("Proposta rinnovo per registro {RegistroId} da {User}", id, User.Identity?.Name);

                return Json(new
                {
                    success = true,
                    message = "Proposta di rinnovo registrata con successo!",
                    nuovoStato = StatoRegistro.InScadenzaPropostoRinnovo.ToString()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la proposta di rinnovo del registro {RegistroId}", id);
                return Json(new { success = false, message = "Errore durante la proposta di rinnovo. Riprova." });
            }
        }

        #endregion

        #region RINNOVO

        /// <summary>
        /// GET: /RegistroContratti/Rinnova/5
        /// Form per rinnovare un contratto
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Rinnova(Guid id)
        {
            try
            {
                var model = await _registroService.PrepareRinnovoViewModelAsync(id);

                if (model == null)
                {
                    TempData["ErrorMessage"] = "Registro non trovato o non rinnovabile.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                ViewBag.IsRinnovo = true;
                return View("Create", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la preparazione del rinnovo per registro {RegistroId}", id);
                TempData["ErrorMessage"] = "Errore durante la preparazione del rinnovo. Riprova.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        /// <summary>
        /// POST: /RegistroContratti/RinnovaRapido/5
        /// Rinnova rapidamente un contratto con i parametri esistenti
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RinnovaRapido(Guid id, int? giorniRinnovo = null)
        {
            try
            {
                var (success, errorMessage, nuovoRegistroId) = await _registroService.RinnovaAsync(id, giorniRinnovo, GetCurrentUserId());

                if (!success)
                {
                    return Json(new { success = false, message = errorMessage ?? "Errore durante il rinnovo." });
                }

                _logger.LogInformation("Registro {RegistroId} rinnovato in {NuovoRegistroId} da {User}",
                    id, nuovoRegistroId, User.Identity?.Name);

                return Json(new
                {
                    success = true,
                    message = "Contratto rinnovato con successo!",
                    nuovoRegistroId = nuovoRegistroId,
                    redirectUrl = Url.Action(nameof(Details), new { id = nuovoRegistroId })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il rinnovo rapido del registro {RegistroId}", id);
                return Json(new { success = false, message = "Errore durante il rinnovo. Riprova." });
            }
        }

        #endregion

        #region ALLEGATI

        /// <summary>
        /// GET: /RegistroContratti/Upload/5
        /// Form per caricare un allegato
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Upload(Guid id)
        {
            try
            {
                var model = await _allegatoService.PrepareUploadViewModelAsync(id);

                if (model == null)
                {
                    TempData["ErrorMessage"] = "Registro non trovato.";
                    return RedirectToAction(nameof(Index));
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la preparazione dell'upload per registro {RegistroId}", id);
                TempData["ErrorMessage"] = "Errore durante il caricamento del form. Riprova.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        /// <summary>
        /// POST: /RegistroContratti/Upload
        /// Carica un allegato
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(52428800)] // 50 MB
        public async Task<IActionResult> Upload(AllegatoRegistroUploadViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model = await _allegatoService.PrepareUploadViewModelAsync(model.RegistroContrattiId);
                return View(model);
            }

            try
            {
                var (success, errorMessage, allegatoId) = await _allegatoService.UploadAsync(model, GetCurrentUserId());

                if (!success)
                {
                    ModelState.AddModelError("", errorMessage ?? "Errore durante il caricamento.");
                    model = await _allegatoService.PrepareUploadViewModelAsync(model.RegistroContrattiId);
                    return View(model);
                }

                _logger.LogInformation("Allegato {AllegatoId} caricato per registro {RegistroId} da {User}",
                    allegatoId, model.RegistroContrattiId, User.Identity?.Name);

                TempData["SuccessMessage"] = "Allegato caricato con successo!";
                return RedirectToAction(nameof(Details), new { id = model.RegistroContrattiId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'upload dell'allegato per registro {RegistroId}", model.RegistroContrattiId);
                ModelState.AddModelError("", "Errore durante il caricamento. Riprova.");
                model = await _allegatoService.PrepareUploadViewModelAsync(model.RegistroContrattiId);
                return View(model);
            }
        }

        /// <summary>
        /// POST: /RegistroContratti/UploadAjax
        /// Carica un allegato via AJAX
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(52428800)] // 50 MB
        public async Task<IActionResult> UploadAjax(AllegatoRegistroUploadViewModel model)
        {
            try
            {
                var (success, errorMessage, allegatoId) = await _allegatoService.UploadAsync(model, GetCurrentUserId());

                if (!success)
                {
                    return Json(new { success = false, message = errorMessage ?? "Errore durante il caricamento." });
                }

                _logger.LogInformation("Allegato {AllegatoId} caricato via AJAX per registro {RegistroId} da {User}",
                    allegatoId, model.RegistroContrattiId, User.Identity?.Name);

                return Json(new
                {
                    success = true,
                    message = "Allegato caricato con successo!",
                    allegatoId = allegatoId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'upload AJAX dell'allegato");
                return Json(new { success = false, message = "Errore durante il caricamento. Riprova." });
            }
        }

        /// <summary>
        /// GET: /RegistroContratti/UploadMultiplo/{id}
        /// Form upload multiplo allegati
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> UploadMultiplo(Guid id)
        {
            var viewModel = await _allegatoService.PrepareUploadMultiploViewModelAsync(id);
            if (viewModel == null)
            {
                TempData["ErrorMessage"] = "Registro non trovato.";
                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }

        /// <summary>
        /// POST: /RegistroContratti/UploadMultiplo
        /// Upload multiplo allegati
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(104857600)] // 100MB totali
        public async Task<IActionResult> UploadMultiplo(AllegatoRegistroUploadMultiploViewModel model)
        {
            if (model.Files == null || !model.Files.Any())
            {
                ModelState.AddModelError("Files", "Selezionare almeno un file.");
            }

            if (!ModelState.IsValid)
            {
                var viewModel = await _allegatoService.PrepareUploadMultiploViewModelAsync(model.RegistroContrattiId);
                if (viewModel == null)
                    return RedirectToAction(nameof(Index));

                viewModel.TipoDocumentoId = model.TipoDocumentoId;
                return View(viewModel);
            }

            var userId = GetCurrentUserId();
            var (success, errorMessage, allegatiIds) = await _allegatoService.UploadMultiploAsync(model, userId);

            if (success)
            {
                var count = allegatiIds?.Count ?? 0;
                if (string.IsNullOrEmpty(errorMessage))
                {
                    TempData["SuccessMessage"] = $"{count} allegat{(count == 1 ? "o caricato" : "i caricati")} con successo.";
                }
                else
                {
                    // Upload parziale
                    TempData["WarningMessage"] = errorMessage;
                }
            }
            else
            {
                TempData["ErrorMessage"] = errorMessage ?? "Errore durante il caricamento.";
                return RedirectToAction(nameof(UploadMultiplo), new { id = model.RegistroContrattiId });
            }

            return RedirectToAction(nameof(Details), new { id = model.RegistroContrattiId });
        }

        /// <summary>
        /// GET: /RegistroContratti/Download/5
        /// Scarica un allegato
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Download(Guid id)
        {
            try
            {
                var (success, errorMessage, stream, fileName, mimeType) = await _allegatoService.DownloadAsync(id);

                if (!success || stream == null)
                {
                    TempData["ErrorMessage"] = errorMessage ?? "Errore durante il download.";
                    return RedirectToAction(nameof(Index));
                }

                return File(stream, mimeType ?? "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il download dell'allegato {AllegatoId}", id);
                TempData["ErrorMessage"] = "Errore durante il download. Riprova.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// POST: /RegistroContratti/DeleteAllegato/5
        /// Elimina un allegato
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAllegato(Guid id)
        {
            try
            {
                var allegato = await _allegatoService.GetByIdAsync(id);

                if (allegato == null)
                {
                    return Json(new { success = false, message = "Allegato non trovato." });
                }

                var registroId = allegato.RegistroContrattiId;

                var (success, errorMessage) = await _allegatoService.DeleteAsync(id, GetCurrentUserId());

                if (!success)
                {
                    return Json(new { success = false, message = errorMessage ?? "Errore durante l'eliminazione." });
                }

                _logger.LogInformation("Allegato {AllegatoId} eliminato da {User}",
                    id, User.Identity?.Name);

                return Json(new
                {
                    success = true,
                    message = "Allegato eliminato con successo!",
                    registroId = registroId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'eliminazione dell'allegato {AllegatoId}", id);
                return Json(new { success = false, message = "Errore durante l'eliminazione. Riprova." });
            }
        }

        /// <summary>
        /// GET: /RegistroContratti/AllegatoDetails/5
        /// Mostra i dettagli di un allegato con preview
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> AllegatoDetails(Guid id)
        {
            try
            {
                var allegato = await _allegatoService.GetByIdAsync(id);

                if (allegato == null)
                {
                    TempData["ErrorMessage"] = "Allegato non trovato.";
                    return RedirectToAction(nameof(Index));
                }

                return View(allegato);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei dettagli dell'allegato {AllegatoId}", id);
                TempData["ErrorMessage"] = "Errore durante il caricamento dei dettagli.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// GET: /RegistroContratti/Preview/5
        /// Restituisce il file per visualizzazione inline (PDF, immagini)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Preview(Guid id)
        {
            try
            {
                var (success, errorMessage, stream, fileName, mimeType) = await _allegatoService.DownloadAsync(id);

                if (!success || stream == null)
                {
                    return NotFound(errorMessage ?? "File non trovato.");
                }

                // Verifica se il tipo è supportato per la preview
                var supportedMimeTypes = new[]
                {
                    "application/pdf",
                    "image/jpeg",
                    "image/png",
                    "image/gif",
                    "image/webp",
                    "image/svg+xml",
                    "text/plain",
                    "text/html",
                    "text/csv"
                };

                if (!supportedMimeTypes.Contains(mimeType?.ToLowerInvariant()))
                {
                    // Se non supportato, forza il download
                    return File(stream, mimeType ?? "application/octet-stream", fileName);
                }

                // Restituisce il file per visualizzazione inline
                Response.Headers.Append("Content-Disposition", $"inline; filename=\"{fileName}\"");
                return File(stream, mimeType ?? "application/octet-stream");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la preview dell'allegato {AllegatoId}", id);
                return NotFound("Errore durante il caricamento del file.");
            }
        }

        /// <summary>
        /// POST: /RegistroContratti/UpdateAllegatoDescrizione
        /// Aggiorna la descrizione di un allegato via AJAX
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAllegatoDescrizione(Guid id, string? descrizione)
        {
            try
            {
                var (success, errorMessage) = await _allegatoService.UpdateDescrizioneAsync(
                    id,
                    descrizione,
                    GetCurrentUserId());

                if (!success)
                {
                    return Json(new { success = false, message = errorMessage ?? "Errore durante l'aggiornamento." });
                }

                return Json(new { success = true, message = "Descrizione aggiornata con successo!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'aggiornamento della descrizione dell'allegato {AllegatoId}", id);
                return Json(new { success = false, message = "Errore durante l'aggiornamento." });
            }
        }

        #endregion

        #region CLONAZIONE

        /// <summary>
        /// GET: /RegistroContratti/Clona/5
        /// Form per clonare un registro esistente
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Clona(Guid id)
        {
            try
            {
                var model = await _registroService.PrepareCloneViewModelAsync(id);

                if (model == null)
                {
                    TempData["ErrorMessage"] = "Registro non trovato.";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.IsClone = true;
                ViewBag.SourceId = id;
                return View("Create", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la preparazione della clonazione per registro {RegistroId}", id);
                TempData["ErrorMessage"] = "Errore durante la preparazione della clonazione. Riprova.";
                return RedirectToAction(nameof(Index));
            }
        }

        #endregion

        #region EXPORT

        /// <summary>
        /// GET: /RegistroContratti/ExportCsv
        /// Esporta i registri attivi e rinnovati in CSV
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ExportCsv()
        {
            try
            {
                var csvContent = await _registroService.ExportAttiviCsvAsync();

                var fileName = $"RegistroContratti_Attivi_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csvContent)).ToArray();

                return File(bytes, "text/csv; charset=utf-8", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'esportazione CSV");
                TempData["ErrorMessage"] = "Errore durante l'esportazione. Riprova.";
                return RedirectToAction(nameof(Index));
            }
        }

        #endregion

        #region UTILITY

        /// <summary>
        /// GET: /RegistroContratti/CheckNumeroProtocollo
        /// Verifica se un numero protocollo è già utilizzato (per validazione AJAX)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> CheckNumeroProtocollo(string numeroProtocollo, Guid? excludeId = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(numeroProtocollo))
                    return Json(true);

                var isUnique = await _registroService.IsNumeroProtocolloUniqueAsync(numeroProtocollo, excludeId);

                if (isUnique)
                    return Json(true);

                return Json($"Il numero protocollo '{numeroProtocollo}' è già utilizzato.");
            }
            catch
            {
                return Json(true); // In caso di errore, non blocchiamo
            }
        }

        /// <summary>
        /// GET: /RegistroContratti/Statistiche
        /// Dashboard con statistiche
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Statistiche()
        {
            try
            {
                var statistiche = await _registroService.GetStatisticheAsync();
                return View(statistiche);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento delle statistiche");
                TempData["ErrorMessage"] = "Errore durante il caricamento delle statistiche. Riprova.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Helper per descrizione stato
        /// </summary>
        private string GetStatoDescrizione(StatoRegistro stato)
        {
            return stato switch
            {
                StatoRegistro.Bozza => "In bozza",
                StatoRegistro.InRevisione => "In revisione",
                StatoRegistro.Inviato => "Inviato",
                StatoRegistro.Attivo => "Attivo",
                StatoRegistro.InScadenza => "In scadenza",
                StatoRegistro.InScadenzaPropostoRinnovo => "In scadenza - Proposto rinnovo",
                StatoRegistro.Scaduto => "Scaduto",
                StatoRegistro.Rinnovato => "Rinnovato",
                StatoRegistro.Annullato => "Annullato",
                StatoRegistro.Sospeso => "Sospeso",
                _ => "Sconosciuto"
            };
        }

        /// <summary>
        /// GET: /RegistroContratti/GeneraProtocollo
        /// Genera un nuovo numero protocollo (AJAX)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GeneraProtocollo(TipoRegistro tipo)
        {
            try
            {
                var protocollo = await _registroService.GeneraNumeroProtocolloAsync(tipo);
                return Json(new { success = true, protocollo });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la generazione del protocollo");
                return Json(new { success = false, message = "Errore durante la generazione" });
            }
        }

        #endregion
    }
}