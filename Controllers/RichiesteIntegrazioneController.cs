using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Trasformazioni.Models.Enums;
using Trasformazioni.Models.ViewModels;
using Trasformazioni.Services.Interfaces;

namespace Trasformazioni.Controllers
{
    /// <summary>
    /// Controller per la gestione delle Richieste di Integrazione
    /// Gestisce il ping-pong di richieste/risposte con l'ente durante lo stato InEsame
    /// Cambio stato automatico: Prima richiesta → RichiestaIntegrazione, Tutte chiuse → InEsame
    /// </summary>
    [Authorize]
    //[Route("[controller]")]
    public class RichiesteIntegrazioneController : Controller
    {
        private readonly IRichiestaIntegrazioneService _richiestaService;
        private readonly ILottoService _lottoService;
        private readonly ILogger<RichiesteIntegrazioneController> _logger;

        public RichiesteIntegrazioneController(
            IRichiestaIntegrazioneService richiestaService,
            ILottoService lottoService,
            ILogger<RichiesteIntegrazioneController> logger)
        {
            _richiestaService = richiestaService;
            _lottoService = lottoService;
            _logger = logger;
        }

        // ===================================
        // INDEX - LISTA RICHIESTE LOTTO
        // ===================================

        /// <summary>
        /// GET: Lista richieste di integrazione per un lotto specifico
        /// Mostra tutte le richieste con filtri (Tutte/Aperte/Chiuse)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index(Guid lottoId, string? filter = "tutte")
        {
            try
            {
                // 1. Verifica esistenza lotto
                var lotto = await _lottoService.GetByIdAsync(lottoId);
                if (lotto == null)
                {
                    TempData["ErrorMessage"] = "Lotto non trovato";
                    return RedirectToAction("Index", "Lotti");
                }

                // 2. Recupera richieste in base al filtro
                IEnumerable<RichiestaIntegrazioneListViewModel> richieste;

                switch (filter?.ToLower())
                {
                    case "aperte":
                        richieste = (await _richiestaService.GetByLottoIdAsync(lottoId)).Where(r => !r.IsChiusa);
                        break;
                    case "chiuse":
                        richieste = (await _richiestaService.GetByLottoIdAsync(lottoId)).Where(r => r.IsChiusa);
                        break;
                    default: // "tutte"
                        richieste = await _richiestaService.GetByLottoIdAsync(lottoId);
                        break;
                }

                // 3. Ordina per numero progressivo (più recente prima se stesso numero)
                var richiesteList = richieste
                    .OrderByDescending(r => r.NumeroProgressivo)
                    .ThenByDescending(r => r.DataRichiestaEnte)
                    .ToList();

                // 4. Calcola statistiche
                var totaleRichieste = richiesteList.Count;
                var richiesteAperte = richiesteList.Count(r => !r.IsChiusa);
                var richiesteChiuse = richiesteList.Count(r => r.IsChiusa);
                var richiesteNonRisposte = richiesteList.Count(r => !r.DataRispostaAzienda.HasValue);

                // 5. Popola ViewBag
                ViewBag.LottoId = lotto.Id;
                ViewBag.CodiceLotto = lotto.CodiceLotto;
                ViewBag.DescrizioneLotto = lotto.Descrizione;
                ViewBag.StatoLotto = lotto.Stato;
                ViewBag.CodiceGara = lotto.CodiceGara;
                ViewBag.TitoloGara = lotto.TitoloGara;

                ViewBag.Filter = filter ?? "tutte";
                ViewBag.TotaleRichieste = totaleRichieste;
                ViewBag.RichiesteAperte = richiesteAperte;
                ViewBag.RichiesteChiuse = richiesteChiuse;
                ViewBag.RichiesteNonRisposte = richiesteNonRisposte;

                // 6. Verifica permesso registrare nuova richiesta
                // Può registrare solo se lotto in InEsame o RichiestaIntegrazione
                var canCreateNewRequest = lotto.Stato == StatoLotto.InEsame ||
                                         lotto.Stato == StatoLotto.RichiestaIntegrazione;
                ViewBag.CanCreateNewRequest = canCreateNewRequest;

                return View(richiesteList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento lista richieste per Lotto {LottoId}", lottoId);
                TempData["ErrorMessage"] = "Errore durante il caricamento delle richieste";
                return RedirectToAction("Details", "Lotti", new { id = lottoId });
            }
        }

        // ===================================
        // CREATE - REGISTRA RICHIESTA ENTE
        // ===================================

        /// <summary>
        /// GET: Form per registrare una nuova richiesta dell'ente
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Create(Guid lottoId)
        {
            try
            {
                // 1. Verifica esistenza lotto
                var lotto = await _lottoService.GetByIdAsync(lottoId);
                if (lotto == null)
                {
                    TempData["ErrorMessage"] = "Lotto non trovato";
                    return RedirectToAction("Index", "Lotti");
                }

                // 2. Verifica stato lotto (deve essere InEsame o RichiestaIntegrazione)
                if (lotto.Stato != StatoLotto.InEsame && lotto.Stato != StatoLotto.RichiestaIntegrazione)
                {
                    TempData["ErrorMessage"] = $"Impossibile registrare richiesta. Il lotto deve essere in stato 'In Esame' o 'Richiesta Integrazione'. Stato attuale: {lotto.Stato}";
                    return RedirectToAction("Details", "Lotti", new { id = lottoId });
                }

                // 3. Ottieni prossimo numero progressivo
                var richieste = await _richiestaService.GetByLottoIdAsync(lottoId);
                var prossimoNumero = richieste.Any()
                    ? richieste.Max(r => r.NumeroProgressivo) + 1
                    : 1;

                // 4. Crea ViewModel con valori default
                var model = new RichiestaIntegrazioneCreateViewModel
                {
                    LottoId = lottoId,
                    NumeroProgressivoSuggerito = prossimoNumero,
                    DataRichiestaEnte = DateTime.Now, // Default oggi

                    // Info lotto per display
                    CodiceLotto = lotto.CodiceLotto,
                    DescrizioneLotto = lotto.Descrizione,
                    CodiceGara = lotto.CodiceGara,
                    TitoloGara = lotto.TitoloGara
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento form registrazione richiesta per Lotto {LottoId}", lottoId);
                TempData["ErrorMessage"] = "Errore durante il caricamento del form";
                return RedirectToAction("Index", new { lottoId });
            }
        }

        /// <summary>
        /// POST: Salva nuova richiesta ente
        /// LOGICA CAMBIO STATO: Prima richiesta → Lotto passa a RichiestaIntegrazione
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid lottoId, RichiestaIntegrazioneCreateViewModel model)
        {
            try
            {
                // 1. Verifica esistenza lotto
                var lotto = await _lottoService.GetByIdAsync(lottoId);
                if (lotto == null)
                {
                    TempData["ErrorMessage"] = "Lotto non trovato";
                    return RedirectToAction("Index", "Lotti");
                }

                // 2. Verifica stato lotto
                if (lotto.Stato != StatoLotto.InEsame && lotto.Stato != StatoLotto.RichiestaIntegrazione)
                {
                    TempData["ErrorMessage"] = $"Impossibile registrare richiesta. Stato attuale: {lotto.Stato}";
                    return RedirectToAction("Details", "Lotti", new { id = lottoId });
                }

                // 3. Validazione ModelState
                if (!ModelState.IsValid)
                {
                    // Ricarica info lotto per il form
                    model.CodiceLotto = lotto.CodiceLotto;
                    model.DescrizioneLotto = lotto.Descrizione;
                    model.CodiceGara = lotto.CodiceGara;
                    model.TitoloGara = lotto.TitoloGara;

                    return View(model);
                }

                // 4. User ID per audit
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";

                // 5. Verifica se è la PRIMA richiesta del lotto
                var richiesteEsistenti = await _richiestaService.GetByLottoIdAsync(lottoId);
                var isPrimaRichiesta = !richiesteEsistenti.Any();

                // 6. Crea richiesta
                var (createSuccess, createError, richiestaId) = await _richiestaService.CreateAsync(model, userId);

                if (!createSuccess)
                {
                    ModelState.AddModelError(string.Empty, createError ?? "Errore durante la registrazione della richiesta");

                    // Ricarica info lotto
                    model.CodiceLotto = lotto.CodiceLotto;
                    model.DescrizioneLotto = lotto.Descrizione;
                    model.CodiceGara = lotto.CodiceGara;
                    model.TitoloGara = lotto.TitoloGara;

                    return View(model);
                }

                // 7. ⭐ LOGICA CAMBIO STATO AUTOMATICO
                if (isPrimaRichiesta && lotto.Stato == StatoLotto.InEsame)
                {
                    // Prima richiesta → Cambia stato a RichiestaIntegrazione
                    var statoAggiornato = await _lottoService.UpdateStatoAsync(
                        lottoId,
                        StatoLotto.RichiestaIntegrazione,
                        userId
                    );

                    if (statoAggiornato)
                    {
                        _logger.LogInformation(
                            "Prima richiesta registrata per Lotto {LottoId}. Stato cambiato: InEsame → RichiestaIntegrazione",
                            lottoId
                        );

                        TempData["SuccessMessage"] = $"Richiesta #{model.NumeroProgressivoSuggerito} registrata con successo. Il lotto è passato in stato 'Richiesta Integrazione'.";
                    }
                    else
                    {
                        _logger.LogWarning(
                            "Richiesta registrata ma errore nel cambio stato per Lotto {LottoId}",
                            lottoId
                        );

                        TempData["SuccessMessage"] = $"Richiesta #{model.NumeroProgressivoSuggerito} registrata, ma errore nel cambio stato del lotto.";
                    }
                }
                else
                {
                    // Richiesta successiva → Stato invariato
                    _logger.LogInformation(
                        "Richiesta registrata per Lotto {LottoId} (già in RichiestaIntegrazione). Stato invariato.",
                        lottoId
                    );

                    TempData["SuccessMessage"] = $"Richiesta #{model.NumeroProgressivoSuggerito} registrata con successo.";
                }

                // 8. Redirect a lista richieste
                return RedirectToAction("Index", new { lottoId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la registrazione richiesta per Lotto {LottoId}", lottoId);
                ModelState.AddModelError(string.Empty, "Errore durante la registrazione della richiesta");
                return View(model);
            }
        }

        // ===================================
        // RISPONDI - FORM RISPOSTA AZIENDA
        // ===================================

        /// <summary>
        /// GET: Form per rispondere a una richiesta dell'ente
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Rispondi(Guid richiestaId)
        {
            try
            {
                // 1. Recupera richiesta con lotto
                var richiesta = await _richiestaService.GetByIdAsync(richiestaId);
                if (richiesta == null)
                {
                    TempData["ErrorMessage"] = "Richiesta non trovata";
                    return RedirectToAction("Index", "Lotti");
                }

                // 2. Verifica che non sia già stata risposta
                if (richiesta.IsChiusa || richiesta.DataRispostaAzienda.HasValue)
                {
                    TempData["WarningMessage"] = "Questa richiesta ha già ricevuto risposta.";
                    return RedirectToAction("Details", new { richiestaId });
                }

                // 3. Verifica lotto
                var lotto = await _lottoService.GetByIdAsync(richiesta.LottoId);
                if (lotto == null)
                {
                    TempData["ErrorMessage"] = "Lotto non trovato";
                    return RedirectToAction("Index", "Lotti");
                }

                // 4. Popola ViewBag con info richiesta (read-only)
                ViewBag.RichiestaId = richiesta.Id;
                ViewBag.NumeroProgressivo = richiesta.NumeroProgressivo;
                ViewBag.DataRichiestaEnte = richiesta.DataRichiestaEnte;
                ViewBag.TestoRichiestaEnte = richiesta.TestoRichiestaEnte;
                ViewBag.NomeFileRichiesta = richiesta.NomeFileRichiesta;
                ViewBag.DocumentoRichiestaPath = richiesta.DocumentoRichiestaPath;

                ViewBag.LottoId = lotto.Id;
                ViewBag.CodiceLotto = lotto.CodiceLotto;
                ViewBag.DescrizioneLotto = lotto.Descrizione;
                ViewBag.CodiceGara = lotto.CodiceGara;
                ViewBag.TitoloGara = lotto.TitoloGara;

                // 5. Crea ViewModel con valori default
                var model = new RichiestaIntegrazioneEditViewModel
                {
                    Id = richiesta.Id,
                    DataRispostaAzienda = DateTime.Now, // Default oggi

                    // Info richiesta (per display, non editabili)
                    NumeroProgressivo = richiesta.NumeroProgressivo,
                    DataRichiestaEnte = richiesta.DataRichiestaEnte,
                    TestoRichiestaEnte = richiesta.TestoRichiestaEnte
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento form risposta per Richiesta {RichiestaId}", richiestaId);
                TempData["ErrorMessage"] = "Errore durante il caricamento del form";
                return RedirectToAction("Index", "Lotti");
            }
        }

        /// <summary>
        /// POST: Salva risposta azienda
        /// LOGICA CAMBIO STATO: Se tutte le richieste chiuse → Lotto torna a InEsame
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rispondi(Guid richiestaId, RichiestaIntegrazioneEditViewModel model)
        {
            try
            {
                // 1. Recupera richiesta
                var richiesta = await _richiestaService.GetByIdAsync(richiestaId);
                if (richiesta == null)
                {
                    TempData["ErrorMessage"] = "Richiesta non trovata";
                    return RedirectToAction("Index", "Lotti");
                }

                // 2. Verifica che non sia già stata risposta
                if (richiesta.IsChiusa || richiesta.DataRispostaAzienda.HasValue)
                {
                    TempData["WarningMessage"] = "Questa richiesta ha già ricevuto risposta.";
                    return RedirectToAction("Details", new { richiestaId });
                }

                // 3. Validazione ModelState
                if (!ModelState.IsValid)
                {
                    // Ricarica dati per il form
                    await ReloadRispondiViewDataAsync(richiestaId);
                    return View(model);
                }

                // 4. User ID per audit
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";

                // 5. Salva risposta con chiusura automatica
                model.IsChiusa = true; // Chiude automaticamente quando si risponde

                var (updateSuccess, updateError) = await _richiestaService.UpdateAsync(model, userId);

                if (!updateSuccess)
                {
                    ModelState.AddModelError(string.Empty, updateError ?? "Errore durante il salvataggio della risposta");
                    await ReloadRispondiViewDataAsync(richiestaId);
                    return View(model);
                }

                // 6. ⭐ LOGICA CAMBIO STATO AUTOMATICO
                // Verifica se TUTTE le richieste del lotto sono chiuse
                var lottoId = richiesta.LottoId;
                var tutteChiuse = await _richiestaService.AreAllRequestsClosedAsync(lottoId);

                if (tutteChiuse)
                {
                    // Tutte chiuse → Riporta lotto a InEsame
                    var lotto = await _lottoService.GetByIdAsync(lottoId);

                    if (lotto != null && lotto.Stato == StatoLotto.RichiestaIntegrazione)
                    {
                        var statoAggiornato = await _lottoService.UpdateStatoAsync(
                            lottoId,
                            StatoLotto.InEsame,
                            userId
                        );

                        if (statoAggiornato)
                        {
                            _logger.LogInformation(
                                "Tutte le richieste chiuse per Lotto {LottoId}. Stato cambiato: RichiestaIntegrazione → InEsame",
                                lottoId
                            );

                            TempData["SuccessMessage"] = $"Risposta inviata con successo. Tutte le richieste sono state evase. Il lotto è tornato in stato 'In Esame'.";
                        }
                        else
                        {
                            _logger.LogWarning(
                                "Risposta salvata ma errore nel cambio stato per Lotto {LottoId}",
                                lottoId
                            );

                            TempData["SuccessMessage"] = "Risposta inviata, ma errore nel cambio stato del lotto.";
                        }
                    }
                    else
                    {
                        TempData["SuccessMessage"] = "Risposta inviata con successo. Tutte le richieste sono state evase.";
                    }
                }
                else
                {
                    // Ci sono ancora richieste aperte
                    _logger.LogInformation(
                        "Risposta inviata per Richiesta {RichiestaId}. Ci sono ancora richieste aperte per Lotto {LottoId}",
                        richiestaId,
                        lottoId
                    );

                    TempData["SuccessMessage"] = "Risposta inviata con successo. Ci sono ancora richieste aperte da evadere.";
                }

                // 7. Redirect a lista richieste
                return RedirectToAction("Index", new { lottoId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il salvataggio risposta per Richiesta {RichiestaId}", richiestaId);
                ModelState.AddModelError(string.Empty, "Errore durante il salvataggio della risposta");
                await ReloadRispondiViewDataAsync(richiestaId);
                return View(model);
            }
        }

        // ===================================
        // DETAILS - VISUALIZZAZIONE RICHIESTA
        // ===================================

        /// <summary>
        /// GET: Visualizzazione dettagliata richiesta e risposta (read-only)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Details(Guid richiestaId)
        {
            try
            {
                // 1. Recupera richiesta completa
                var richiesta = await _richiestaService.GetByIdAsync(richiestaId);
                if (richiesta == null)
                {
                    TempData["ErrorMessage"] = "Richiesta non trovata";
                    return RedirectToAction("Index", "Lotti");
                }

                // 2. Recupera lotto
                var lotto = await _lottoService.GetByIdAsync(richiesta.LottoId);
                if (lotto == null)
                {
                    TempData["ErrorMessage"] = "Lotto non trovato";
                    return RedirectToAction("Index", "Lotti");
                }

                // 3. Popola ViewBag con info lotto
                ViewBag.LottoId = lotto.Id;
                ViewBag.CodiceLotto = lotto.CodiceLotto;
                ViewBag.DescrizioneLotto = lotto.Descrizione;
                ViewBag.StatoLotto = lotto.Stato;
                ViewBag.CodiceGara = lotto.CodiceGara;
                ViewBag.TitoloGara = lotto.TitoloGara;

                // 4. Verifica permesso rispondere
                var canRispondi = !richiesta.IsChiusa && !richiesta.DataRispostaAzienda.HasValue;
                ViewBag.CanRispondi = canRispondi;

                return View(richiesta);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento dettaglio per Richiesta {RichiestaId}", richiestaId);
                TempData["ErrorMessage"] = "Errore durante il caricamento del dettaglio";
                return RedirectToAction("Index", "Lotti");
            }
        }

        // ===================================
        // DELETE - ELIMINA RICHIESTA
        // ===================================

        /// <summary>
        /// POST: Elimina richiesta (soft delete)
        /// Solo per richieste NON ancora risposte (errori di inserimento)
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid richiestaId)
        {
            try
            {
                // 1. Recupera richiesta
                var richiesta = await _richiestaService.GetByIdAsync(richiestaId);
                if (richiesta == null)
                {
                    TempData["ErrorMessage"] = "Richiesta non trovata";
                    return RedirectToAction("Index", "Lotti");
                }

                var lottoId = richiesta.LottoId;

                // 2. Verifica che NON sia stata risposta
                if (richiesta.DataRispostaAzienda.HasValue)
                {
                    TempData["ErrorMessage"] = "Impossibile eliminare una richiesta che ha già ricevuto risposta.";
                    return RedirectToAction("Index", new { lottoId });
                }

                // 3. User ID per audit
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";

                // 4. Elimina (soft delete)
                var (deleteSuccess, deleteError) = await _richiestaService.DeleteAsync(richiestaId);

                if (!deleteSuccess)
                {
                    TempData["ErrorMessage"] = deleteError ?? "Errore durante l'eliminazione della richiesta";
                    _logger.LogError("Impossibile eliminare Richiesta {RichiestaId}", richiestaId);
                    return RedirectToAction("Index", new { lottoId });
                }

                // 5. Log successo
                _logger.LogInformation(
                    "Richiesta eliminata: {RichiestaId} per Lotto {LottoId} da utente {UserId}",
                    richiestaId,
                    lottoId,
                    userId
                );

                TempData["SuccessMessage"] = $"Richiesta #{richiesta.NumeroProgressivo} eliminata con successo.";

                // 6. Redirect a lista richieste
                return RedirectToAction("Index", new { lottoId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'eliminazione Richiesta {RichiestaId}", richiestaId);
                TempData["ErrorMessage"] = "Errore durante l'eliminazione della richiesta";
                return RedirectToAction("Index", "Lotti");
            }
        }

        // ===================================
        // HELPER METHODS
        // ===================================

        /// <summary>
        /// Ricarica dati ViewBag per form Rispondi in caso di errore validazione
        /// </summary>
        private async Task ReloadRispondiViewDataAsync(Guid richiestaId)
        {
            var richiesta = await _richiestaService.GetByIdAsync(richiestaId);
            if (richiesta != null)
            {
                ViewBag.RichiestaId = richiesta.Id;
                ViewBag.NumeroProgressivo = richiesta.NumeroProgressivo;
                ViewBag.DataRichiestaEnte = richiesta.DataRichiestaEnte;
                ViewBag.TestoRichiestaEnte = richiesta.TestoRichiestaEnte;
                ViewBag.NomeFileRichiesta = richiesta.NomeFileRichiesta;
                ViewBag.DocumentoRichiestaPath = richiesta.DocumentoRichiestaPath;

                var lotto = await _lottoService.GetByIdAsync(richiesta.LottoId);
                if (lotto != null)
                {
                    ViewBag.LottoId = lotto.Id;
                    ViewBag.CodiceLotto = lotto.CodiceLotto;
                    ViewBag.DescrizioneLotto = lotto.Descrizione;
                    ViewBag.CodiceGara = lotto.CodiceGara;
                    ViewBag.TitoloGara = lotto.TitoloGara;
                }
            }
        }
    }
}