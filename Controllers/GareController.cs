using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Trasformazioni.Authorization;
using Trasformazioni.Helpers;
using Trasformazioni.Models.Enums;
using Trasformazioni.Models.ViewModels;
using Trasformazioni.Services.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace Trasformazioni.Controllers
{
    /// <summary>
    /// Controller per la gestione delle Gare d'Appalto
    /// </summary>
    [Authorize(Roles = $"{RoleNames.Amministrazione},{RoleNames.UfficioGare}")]
    public class GareController : Controller
    {
        private readonly IGaraService _garaService;
        private readonly ILottoService _lottoService;
        private readonly ITipoDocumentoService _tipoDocumentoService;
        private readonly ILogger<GareController> _logger;

        public GareController(
            IGaraService garaService,
            ILottoService lottoService,
            ILogger<GareController> logger,
            ITipoDocumentoService tipoDocumentoService)
        {
            _garaService = garaService;
            _lottoService = lottoService;
            _logger = logger;
            _tipoDocumentoService = tipoDocumentoService;
        }

        #region Helper Methods

        /// <summary>
        /// Popola le dropdown per i filtri
        /// </summary>
        private void PrepareFilterDropdowns()
        {
            // Stati Gara
            ViewBag.StatiGara = Enum.GetValues<StatoGara>()
                .Select(s => new SelectListItem
                {
                    Value = ((int)s).ToString(),
                    Text = EnumHelper.GetDisplayName(s)
                })
                .ToList();

            // Tipologie Gara
            ViewBag.TipologieGara = Enum.GetValues<TipologiaGara>()
                .Select(t => new SelectListItem
                {
                    Value = ((int)t).ToString(),
                    Text = EnumHelper.GetDisplayName(t)
                })
                .ToList();
        }

        /// <summary>
        /// Popola le dropdown per i form di creazione
        /// </summary>
        private void PrepareCreateFormDropdowns(TipologiaGara? selectedTipologia = null)
        {
            // Tipologie Gara
            ViewBag.TipologieGara = new SelectList(
                Enum.GetValues<TipologiaGara>()
                    .Select(t => new { Value = (int)t, Text = EnumHelper.GetDisplayName(t) }),
                "Value",
                "Text",
                selectedTipologia
            );
        }

        /// <summary>
        /// Popola le dropdown per i form di modifica
        /// </summary>
        private void PrepareEditFormDropdowns(TipologiaGara? selectedTipologia = null, StatoGara? selectedStato = null)
        {
            // Tipologie Gara
            ViewBag.TipologieGara = new SelectList(
                Enum.GetValues<TipologiaGara>()
                    .Select(t => new { Value = (int)t, Text = EnumHelper.GetDisplayName(t) }),
                "Value",
                "Text",
                selectedTipologia
            );

            // Stati Gara
            ViewBag.StatiGara = new SelectList(
                Enum.GetValues<StatoGara>()
                    .Select(s => new { Value = (int)s, Text = EnumHelper.GetDisplayName(s) }),
                "Value",
                "Text",
                selectedStato
            );
        }

        /// <summary>
        /// Prepara la dropdown per la checklist documenti richiesti (area Gare)
        /// </summary>
        private async Task PrepareChecklistDocumentiDropdown(List<Guid>? selectedIds = null)
        {
            var tipiGare = await _tipoDocumentoService.GetByAreaAsync(AreaDocumento.Gare);
            ViewBag.TipiDocumentoChecklist = tipiGare
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

        #region READ - Visualizzazione

        /// <summary>
        /// GET: /Gare
        /// Lista paginata delle gare con filtri
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index(GaraFilterViewModel filters)
        {
            try
            {
                // Ottieni risultati paginati
                var pagedResult = await _garaService.GetPagedAsync(filters);

                // Prepara dropdown per filtri
                PrepareFilterDropdowns();

                // Passa i filtri correnti alla view per mantenerli
                ViewBag.CurrentFilters = filters;

                return View(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento della lista gare");
                TempData["ErrorMessage"] = "Errore durante il caricamento delle gare. Riprova.";
                return View(new PagedResult<GaraListViewModel>());
            }
        }

        /// <summary>
        /// GET: /Gare/Details/5
        /// Dettagli completi di una gara con i suoi lotti
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var gara = await _garaService.GetByIdAsync(id);

                if (gara == null)
                {
                    _logger.LogWarning("Gara con ID {GaraId} non trovata", id);
                    TempData["ErrorMessage"] = "Gara non trovata.";
                    return RedirectToAction(nameof(Index));
                }

                // Carica anche i lotti associati
                var lottiFilter = new LottoFilterViewModel { GaraId = id, PageSize = 100 };
                var lotti = await _lottoService.GetPagedAsync(lottiFilter);
                ViewBag.Lotti = lotti.Items;

                return View(gara);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento dei dettagli gara {GaraId}", id);
                TempData["ErrorMessage"] = "Errore durante il caricamento dei dettagli. Riprova.";
                return RedirectToAction(nameof(Index));
            }
        }

        #endregion

        #region CREATE

        /// <summary>
        /// GET: /Gare/Create
        /// Form per creare una nuova gara
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new GaraCreateViewModel
            {
                DataPubblicazione = DateTime.Today
            };

            PrepareCreateFormDropdowns();
            await PrepareChecklistDocumentiDropdown();
            return View(model);
        }

        /// <summary>
        /// POST: /Gare/Create
        /// Salva una nuova gara
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GaraCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                PrepareCreateFormDropdowns(model.Tipologia);
                await PrepareChecklistDocumentiDropdown(model.DocumentiRichiestiIds);
                return View(model);
            }

            try
            {
                // Verifica unicità codice gara
                var isUnique = await _garaService.IsCodiceGaraUniqueAsync(model.CodiceGara);
                if (!isUnique)
                {
                    ModelState.AddModelError(nameof(model.CodiceGara), "Esiste già una gara con questo codice.");
                    PrepareCreateFormDropdowns(model.Tipologia);
                    await PrepareChecklistDocumentiDropdown(model.DocumentiRichiestiIds);
                    return View(model);
                }

                var result = await _garaService.CreateAsync(model);

                if (!result.Success)
                {
                    ModelState.AddModelError("", result.ErrorMessage ?? "Errore durante la creazione della gara.");
                    PrepareCreateFormDropdowns(model.Tipologia);
                    await PrepareChecklistDocumentiDropdown(model.DocumentiRichiestiIds);
                    return View(model);
                }

                _logger.LogInformation("Gara {CodiceGara} creata con successo da {User}",
                    model.CodiceGara, User.Identity?.Name);

                TempData["SuccessMessage"] = $"Gara '{model.CodiceGara}' creata con successo!";
                return RedirectToAction(nameof(Details), new { id = result.GaraId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la creazione della gara {CodiceGara}", model.CodiceGara);
                ModelState.AddModelError("", "Errore durante la creazione della gara. Riprova.");
                PrepareCreateFormDropdowns(model.Tipologia);
                await PrepareChecklistDocumentiDropdown(model.DocumentiRichiestiIds);
                return View(model);
            }
        }

        #endregion

        #region UPDATE

        /// <summary>
        /// GET: /Gare/Edit/5
        /// Form per modificare una gara esistente
        /// </summary>
        //[HttpGet]
        //public async Task<IActionResult> Edit(Guid id)
        //{
        //    try
        //    {
        //        var gara = await _garaService.GetByIdAsync(id);

        //        if (gara == null)
        //        {
        //            _logger.LogWarning("Tentativo di modifica gara inesistente: {GaraId}", id);
        //            TempData["ErrorMessage"] = "Gara non trovata.";
        //            return RedirectToAction(nameof(Index));
        //        }

        //        // Usa il mapping extension per creare l'EditViewModel
        //        // Nota: Devi creare il metodo ToEditViewModel in GaraMappingExtensions
        //        var model = new GaraEditViewModel
        //        {
        //            Id = gara.Id,
        //            CodiceGara = gara.CodiceGara,
        //            Titolo = gara.Titolo,
        //            Descrizione = gara.Descrizione,
        //            Tipologia = gara.Tipologia,
        //            Stato = gara.Stato,
        //            EnteAppaltante = gara.EnteAppaltante,
        //            Regione = gara.Regione,
        //            NomePuntoOrdinante = gara.NomePuntoOrdinante,
        //            TelefonoPuntoOrdinante = gara.TelefonoPuntoOrdinante,
        //            CIG = gara.CIG,
        //            CUP = gara.CUP,
        //            RDO = gara.RDO,
        //            Bando = gara.Bando,
        //            DenominazioneIniziativa = gara.DenominazioneIniziativa,
        //            Procedura = gara.Procedura,
        //            CriterioAggiudicazione = gara.CriterioAggiudicazione,
        //            DataPubblicazione = gara.DataPubblicazione,
        //            DataInizioPresentazioneOfferte = gara.DataInizioPresentazioneOfferte,
        //            DataTermineRichiestaChiarimenti = gara.DataTermineRichiestaChiarimenti,
        //            DataTerminePresentazioneOfferte = gara.DataTerminePresentazioneOfferte,
        //            ImportoTotaleStimato = gara.ImportoTotaleStimato,
        //            LinkPiattaforma = gara.LinkPiattaforma
        //        };

        //        PrepareEditFormDropdowns(model.Tipologia, model.Stato);
        //        return View(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Errore durante il caricamento della gara {GaraId} per modifica", id);
        //        TempData["ErrorMessage"] = "Errore durante il caricamento della gara. Riprova.";
        //        return RedirectToAction(nameof(Index));
        //    }
        //}
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var model = await _garaService.GetForEditAsync(id);

                if (model == null)
                {
                    _logger.LogWarning("Tentativo di modifica gara inesistente: {GaraId}", id);
                    TempData["ErrorMessage"] = "Gara non trovata.";
                    return RedirectToAction(nameof(Index));
                }

                PrepareEditFormDropdowns(model.Tipologia, model.Stato);
                await PrepareChecklistDocumentiDropdown(model.DocumentiRichiestiIds);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento della gara {GaraId} per modifica", id);
                TempData["ErrorMessage"] = "Errore durante il caricamento della gara. Riprova.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// POST: /Gare/Edit/5
        /// Salva le modifiche a una gara
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, GaraEditViewModel model)
        {
            if (id != model.Id)
            {
                _logger.LogWarning("Mismatch ID in Edit: URL={UrlId}, Model={ModelId}", id, model.Id);
                TempData["ErrorMessage"] = "Errore nella richiesta.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                PrepareEditFormDropdowns(model.Tipologia, model.Stato);
                await PrepareChecklistDocumentiDropdown(model.DocumentiRichiestiIds);
                return View(model);
            }

            try
            {
                // Verifica che la gara esista
                var garaEsistente = await _garaService.GetByIdAsync(id);
                if (garaEsistente == null)
                {
                    TempData["ErrorMessage"] = "Gara non trovata.";
                    return RedirectToAction(nameof(Index));
                }

                // Verifica unicità codice gara (se modificato)
                if (model.CodiceGara != garaEsistente.CodiceGara)
                {
                    var isUnique = await _garaService.IsCodiceGaraUniqueAsync(model.CodiceGara, id);
                    if (!isUnique)
                    {
                        ModelState.AddModelError(nameof(model.CodiceGara), "Esiste già un'altra gara con questo codice.");
                        PrepareEditFormDropdowns(model.Tipologia, model.Stato);
                        await PrepareChecklistDocumentiDropdown(model.DocumentiRichiestiIds);
                        return View(model);
                    }
                }

                var result = await _garaService.UpdateAsync(model);

                if (!result.Success)
                {
                    ModelState.AddModelError("", result.ErrorMessage ?? "Errore durante il salvataggio delle modifiche.");
                    PrepareEditFormDropdowns(model.Tipologia, model.Stato);
                    await PrepareChecklistDocumentiDropdown(model.DocumentiRichiestiIds);
                    return View(model);
                }

                _logger.LogInformation("Gara {CodiceGara} modificata con successo da {User}",
                    model.CodiceGara, User.Identity?.Name);

                TempData["SuccessMessage"] = $"Gara '{model.CodiceGara}' modificata con successo!";
                return RedirectToAction(nameof(Details), new { id = model.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la modifica della gara {GaraId}", id);
                ModelState.AddModelError("", "Errore durante il salvataggio delle modifiche. Riprova.");
                PrepareEditFormDropdowns(model.Tipologia, model.Stato);
                await PrepareChecklistDocumentiDropdown(model.DocumentiRichiestiIds);
                return View(model);
            }
        }

        #endregion

        #region DELETE

        /// <summary>
        /// POST: /Gare/Delete/5
        /// Elimina una gara (soft delete)
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var gara = await _garaService.GetByIdAsync(id);

                if (gara == null)
                {
                    _logger.LogWarning("Tentativo di eliminazione gara inesistente: {GaraId}", id);
                    return Json(new { success = false, message = "Gara non trovata." });
                }

                // Verifica se ha lotti associati
                var lottiFilter = new LottoFilterViewModel { GaraId = id, PageSize = 1 };
                var lotti = await _lottoService.GetPagedAsync(lottiFilter);

                if (lotti.TotalItems > 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = $"Impossibile eliminare la gara. Ci sono {lotti.TotalItems} lotti associati. Eliminare prima i lotti."
                    });
                }

                await _garaService.DeleteAsync(id);

                _logger.LogInformation("Gara {CodiceGara} eliminata da {User}",
                    gara.CodiceGara, User.Identity?.Name);

                return Json(new
                {
                    success = true,
                    message = $"Gara '{gara.CodiceGara}' eliminata con successo!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'eliminazione della gara {GaraId}", id);
                return Json(new
                {
                    success = false,
                    message = "Errore durante l'eliminazione. Riprova."
                });
            }
        }

        #endregion

        #region WORKFLOW - Cambio Stato

        /// <summary>
        /// POST: /Gare/CambiaStato/5
        /// Cambia lo stato di una gara
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiaStato(Guid id, StatoGara nuovoStato)
        {
            try
            {
                var gara = await _garaService.GetByIdAsync(id);

                if (gara == null)
                {
                    return Json(new { success = false, message = "Gara non trovata." });
                }

                // Validazione transizioni di stato
                var transizioneValida = ValidaTransizioneStato(gara.Stato, nuovoStato);
                if (!transizioneValida.isValid)
                {
                    return Json(new { success = false, message = transizioneValida.message });
                }

                // Usa il nuovo metodo del service per aggiornare solo lo stato
                var result = await _garaService.UpdateStatoAsync(id, nuovoStato);

                if (!result.Success)
                {
                    return Json(new { success = false, message = result.ErrorMessage ?? "Errore durante il cambio stato." });
                }

                _logger.LogInformation("Stato gara {CodiceGara} cambiato da {VecchioStato} a {NuovoStato} da {User}",
                    gara.CodiceGara, gara.Stato, nuovoStato, User.Identity?.Name);

                return Json(new
                {
                    success = true,
                    message = $"Stato cambiato in '{nuovoStato}' con successo!",
                    nuovoStato = nuovoStato.ToString()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il cambio stato gara {GaraId}", id);
                return Json(new { success = false, message = "Errore durante il cambio stato. Riprova." });
            }
        }

        /// <summary>
        /// Valida se una transizione di stato è permessa
        /// Stati: Bozza, InLavorazione, Conclusa, ChiusaManualmente
        /// </summary>
        private (bool isValid, string message) ValidaTransizioneStato(StatoGara statoCorrente, StatoGara nuovoStato)
        {
            // Stesso stato - sempre valido
            if (statoCorrente == nuovoStato)
                return (true, "");

            // Transizioni valide basate su StatoGara enum reale
            var transizioniValide = new Dictionary<StatoGara, List<StatoGara>>
            {
                // Da Bozza posso andare in InLavorazione o ChiusaManualmente
                { StatoGara.Bozza, new List<StatoGara> { StatoGara.InLavorazione, StatoGara.ChiusaManualmente } },
                
                // Da InLavorazione posso andare in Conclusa o ChiusaManualmente
                { StatoGara.InLavorazione, new List<StatoGara> { StatoGara.Conclusa, StatoGara.ChiusaManualmente } },
                
                // Da ChiusaManualmente posso riaprire in InLavorazione
                { StatoGara.ChiusaManualmente, new List<StatoGara> { StatoGara.InLavorazione } },
                
                // Conclusa è stato finale, no transizioni
                { StatoGara.Conclusa, new List<StatoGara> { } }
            };

            if (transizioniValide.TryGetValue(statoCorrente, out var statiPermessi))
            {
                if (statiPermessi.Contains(nuovoStato))
                    return (true, "");
                else
                    return (false, $"Non è possibile passare da '{statoCorrente}' a '{nuovoStato}'.");
            }

            return (false, "Transizione di stato non valida.");
        }

        #endregion

        #region UTILITY

        /// <summary>
        /// GET: /Gare/CheckCodiceGara
        /// Verifica se un codice gara è già utilizzato (per validazione AJAX)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> CheckCodiceGara(string codiceGara, Guid? excludeId = null)
        {
            try
            {
                var isUnique = await _garaService.IsCodiceGaraUniqueAsync(codiceGara, excludeId);

                // Se è unico, ritorna true (valido)
                if (isUnique)
                    return Json(true);

                // Altrimenti è già in uso
                return Json($"Il codice gara '{codiceGara}' è già utilizzato.");
            }
            catch
            {
                return Json(true); // In caso di errore, non blocchiamo
            }
        }

        #endregion
    }
}