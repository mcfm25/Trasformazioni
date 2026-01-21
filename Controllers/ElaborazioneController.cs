using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Security.Claims;
using Trasformazioni.Models.Enums;
using Trasformazioni.Models.ViewModels;
using Trasformazioni.Services.Interfaces;

namespace Trasformazioni.Controllers
{
    /// <summary>
    /// Controller per la gestione della fase di Elaborazione del Lotto
    /// Gestisce il workflow: Approvato → InElaborazione → Presentato
    /// Include calcolo prezzi basato su preventivi selezionati
    /// </summary>
    [Authorize]
    public class ElaborazioneController : Controller
    {
        private readonly ILottoService _lottoService;
        private readonly IElaborazioneLottoService _elaborazioneService;
        private readonly IPreventivoService _preventivoService;
        private readonly IDocumentoGaraService _documentoGaraService;
        private readonly ILogger<ElaborazioneController> _logger;

        public ElaborazioneController(
            ILottoService lottoService,
            IElaborazioneLottoService elaborazioneService,
            IPreventivoService preventivoService,
            ILogger<ElaborazioneController> logger,
            IDocumentoGaraService documentoGaraService)
        {
            _lottoService = lottoService;
            _elaborazioneService = elaborazioneService;
            _preventivoService = preventivoService;
            _logger = logger;
            _documentoGaraService = documentoGaraService;
        }

        // ===================================
        // AVVIA ELABORAZIONE
        // ===================================

        /// <summary>
        /// GET: Mostra form conferma per avviare l'elaborazione
        /// Prerequisito: Lotto in stato Approvato
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Avvia(Guid lottoId)
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

                // 2. Verifica stato = Approvato
                if (lotto.Stato != StatoLotto.Approvato)
                {
                    TempData["ErrorMessage"] = $"Il lotto deve essere in stato 'Approvato' per avviare l'elaborazione. Stato attuale: {lotto.Stato}";
                    return RedirectToAction("Details", "Lotti", new { id = lottoId });
                }

                // 3. Verifica che NON abbia già un'elaborazione
                var elaborazioneEsistente = await _elaborazioneService.GetByLottoIdAsync(lottoId);
                if (elaborazioneEsistente != null)
                {
                    TempData["ErrorMessage"] = "Il lotto ha già un'elaborazione in corso";
                    return RedirectToAction("Edit", new { lottoId });
                }

                // 4. Passa dati al form conferma
                ViewBag.LottoId = lotto.Id;
                ViewBag.CodiceLotto = lotto.CodiceLotto;
                ViewBag.DescrizioneLotto = lotto.Descrizione;
                ViewBag.CodiceGara = lotto.CodiceGara;
                ViewBag.TitoloGara = lotto.TitoloGara;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel caricamento form avvio elaborazione per Lotto {LottoId}", lottoId);
                TempData["ErrorMessage"] = "Errore nel caricamento del form";
                return RedirectToAction("Index", "Lotti");
            }
        }

        /// <summary>
        /// POST: Avvia elaborazione
        /// - Crea ElaborazioneLotto vuoto
        /// - Cambia stato Lotto → InElaborazione
        /// - Redirect a Edit per compilazione prezzi
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Avvia(Guid lottoId, string? dummy)
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

                // 2. Verifica stato = Approvato
                if (lotto.Stato != StatoLotto.Approvato)
                {
                    TempData["ErrorMessage"] = $"Il lotto deve essere in stato 'Approvato'. Stato attuale: {lotto.Stato}";
                    return RedirectToAction("Details", "Lotti", new { id = lottoId });
                }

                // 3. Verifica che NON abbia già un'elaborazione
                var elaborazioneEsistente = await _elaborazioneService.GetByLottoIdAsync(lottoId);
                if (elaborazioneEsistente != null)
                {
                    TempData["ErrorMessage"] = "Il lotto ha già un'elaborazione in corso";
                    return RedirectToAction("Edit", new { lottoId });
                }

                // 4. Crea ElaborazioneLotto vuoto
                var createModel = new ElaborazioneLottoCreateViewModel
                {
                    LottoId = lottoId,
                    PrezzoDesiderato = null,
                    PrezzoRealeUscita = null,
                    MotivazioneAdattamento = null,
                    Note = null
                };

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";
                var (success, errorMessage, elaborazioneId) = await _elaborazioneService.CreateAsync(createModel, userId);

                if (!success)
                {
                    TempData["ErrorMessage"] = errorMessage ?? "Errore nella creazione dell'elaborazione";
                    return RedirectToAction("Details", "Lotti", new { id = lottoId });
                }

                // 5. Cambia stato Lotto → InElaborazione
                var statoAggiornato = await _lottoService.UpdateStatoAsync(lottoId, StatoLotto.InElaborazione, userId);
                if (!statoAggiornato)
                {
                    _logger.LogWarning("Elaborazione creata ma cambio stato fallito per Lotto {LottoId}", lottoId);
                    // Non blocchiamo, l'elaborazione è stata creata
                }

                _logger.LogInformation("Elaborazione avviata con successo per Lotto {LottoId} da utente {UserId}", lottoId, userId);

                TempData["SuccessMessage"] = "Elaborazione avviata con successo. Compila i prezzi per procedere.";
                return RedirectToAction("Edit", new { lottoId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nell'avvio elaborazione per Lotto {LottoId}", lottoId);
                TempData["ErrorMessage"] = "Errore nell'avvio dell'elaborazione";
                return RedirectToAction("Details", "Lotti", new { id = lottoId });
            }
        }

        // ===================================
        // EDIT ELABORAZIONE
        // ===================================

        /// <summary>
        /// GET: Form elaborazione con preventivi selezionati e calcolo prezzi
        /// Include statistiche preventivi (min/max/medio) per aiutare nella decisione
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit(Guid lottoId)
        {
            try
            {
                // 1. Verifica esistenza lotto con relazioni
                var lotto = await _lottoService.GetByIdAsync(lottoId);
                if (lotto == null)
                {
                    TempData["ErrorMessage"] = "Lotto non trovato";
                    return RedirectToAction("Index", "Lotti");
                }

                // 2. Verifica stato = InElaborazione
                if (lotto.Stato != StatoLotto.InElaborazione)
                {
                    TempData["ErrorMessage"] = $"Il lotto deve essere in stato 'In Elaborazione'. Stato attuale: {lotto.Stato}";
                    return RedirectToAction("Details", "Lotti", new { id = lottoId });
                }

                // 3. Recupera elaborazione esistente
                var elaborazione = await _elaborazioneService.GetByLottoIdAsync(lottoId);
                if (elaborazione == null)
                {
                    TempData["ErrorMessage"] = "Elaborazione non trovata. Avvia prima l'elaborazione.";
                    return RedirectToAction("Avvia", new { lottoId });
                }

                // 4. Recupera preventivi selezionati per il lotto ⭐
                var preventiviSelezionati = await _preventivoService.GetByLottoIdAsync(lottoId);
                var preventiviSelezionatiList = preventiviSelezionati
                    .Where(p => p.IsSelezionato)
                    .OrderBy(p => p.ImportoOfferto)
                    .ToList();

                // 5. Recupera documenti caricati per il lotto ⭐
                var documentiLotto = await _documentoGaraService.GetByLottoIdAsync(lottoId);
                //var documentiList = documentiLotto
                //    .Where(d => d.Tipo == TipoDocumentoGara.DocumentoPresentazione
                //                || d.Tipo == TipoDocumentoGara.OffertaTecnica
                //                || d.Tipo == TipoDocumentoGara.OffertaEconomica)
                //    .OrderByDescending(d => d.DataCaricamento)
                //    .ToList();
                var codiciElaborazione = new[] // QUI INSERIRE LISTA DOCUMENTI DICHIARATI IN CHECKLIST
                {
                    nameof(TipoDocumentoGara.DocumentoPresentazione),
                    nameof(TipoDocumentoGara.OffertaTecnica),
                    nameof(TipoDocumentoGara.OffertaEconomica)
                };
                var documentiList = documentiLotto
                    .Where(d => d.TipoDocumentoCodiceRiferimento != null
                                && codiciElaborazione.Contains(d.TipoDocumentoCodiceRiferimento))
                    .OrderByDescending(d => d.DataCaricamento)
                    .ToList();

                // 6. Calcola statistiche preventivi (per suggerimenti) ⭐
                decimal? importoMinimo = null;
                decimal? importoMassimo = null;
                decimal? importoMedio = null;
                int numeroPreventiviSelezionati = preventiviSelezionatiList.Count;

                if (numeroPreventiviSelezionati > 0)
                {
                    var importi = preventiviSelezionatiList
                        .Where(p => p.ImportoOfferto.HasValue)
                        .Select(p => p.ImportoOfferto!.Value)
                        .ToList();

                    if (importi.Any())
                    {
                        importoMinimo = importi.Min();
                        importoMassimo = importi.Max();
                        importoMedio = Math.Round(importi.Average(), 2);
                    }
                }

                // 6. Passa dati alla view
                ViewBag.GaraId = lotto.GaraId;
                ViewBag.LottoId = lotto.Id;
                ViewBag.CodiceLotto = lotto.CodiceLotto;
                ViewBag.DescrizioneLotto = lotto.Descrizione;
                ViewBag.CodiceGara = lotto.CodiceGara;
                ViewBag.TitoloGara = lotto.TitoloGara;
                ViewBag.StatoLotto = lotto.Stato;

                // Info economiche lotto
                ViewBag.ImportoBaseAsta = lotto.ImportoBaseAsta;
                ViewBag.Quotazione = lotto.Quotazione;
                ViewBag.GiorniFornitura = lotto.GiorniFornitura;

                // Preventivi selezionati
                ViewBag.PreventiviSelezionati = preventiviSelezionatiList;
                ViewBag.NumeroPreventiviSelezionati = numeroPreventiviSelezionati;

                // Documenti caricati
                ViewBag.DocumentiLotto = documentiList;
                ViewBag.NumeroDocumenti = documentiList.Count;

                // Statistiche preventivi (per suggerimenti)
                ViewBag.ImportoMinimo = importoMinimo;
                ViewBag.ImportoMassimo = importoMassimo;
                ViewBag.ImportoMedio = importoMedio;

                // Warning se nessun preventivo selezionato
                if (numeroPreventiviSelezionati == 0)
                {
                    TempData["WarningMessage"] = "Attenzione: Non ci sono preventivi selezionati per questo lotto. " +
                                                  "Si consiglia di selezionare almeno un preventivo prima di definire il prezzo.";
                }

                // Converti elaborazione in EditViewModel
                var model = new ElaborazioneLottoEditViewModel
                {
                    Id = elaborazione.Id,
                    LottoId = elaborazione.LottoId,
                    PrezzoDesiderato = elaborazione.PrezzoDesiderato,
                    PrezzoRealeUscita = elaborazione.PrezzoRealeUscita,
                    MotivazioneAdattamento = elaborazione.MotivazioneAdattamento,
                    Note = elaborazione.Note,
                    CodiceLotto = lotto.CodiceLotto,
                    DescrizioneLotto = lotto.Descrizione
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel caricamento form elaborazione per Lotto {LottoId}", lottoId);
                TempData["ErrorMessage"] = "Errore nel caricamento del form";
                return RedirectToAction("Details", "Lotti", new { id = lottoId });
            }
        }

        /// <summary>
        /// POST: Salva elaborazione (bozza)
        /// Aggiorna prezzi e note senza cambiare lo stato del lotto
        /// Lotto rimane in stato InElaborazione
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid lottoId, ElaborazioneLottoEditViewModel model)
        {
            try
            {
                // 1. Verifica che lottoId nel model corrisponda
                if (model.LottoId != lottoId)
                {
                    TempData["ErrorMessage"] = "Errore: ID lotto non corrispondente";
                    return RedirectToAction("Edit", new { lottoId });
                }

                // 2. Validazione ModelState
                if (!ModelState.IsValid)
                {
                    // Ricarica dati per la view
                    await ReloadEditViewDataAsync(lottoId);
                    return View(model);
                }

                // 3. Validazione business: Se prezzi diversi → Motivazione obbligatoria
                if (model.PrezzoDesiderato.HasValue && model.PrezzoRealeUscita.HasValue &&
                    model.PrezzoDesiderato.Value != model.PrezzoRealeUscita.Value)
                {
                    if (string.IsNullOrWhiteSpace(model.MotivazioneAdattamento))
                    {
                        ModelState.AddModelError(nameof(model.MotivazioneAdattamento),
                            "La motivazione è obbligatoria quando il prezzo reale è diverso dal prezzo desiderato");

                        await ReloadEditViewDataAsync(lottoId);
                        return View(model);
                    }
                }

                // 4. Aggiorna elaborazione tramite service
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";
                var (success, errorMessage) = await _elaborazioneService.UpdateAsync(model, userId);

                if (!success)
                {
                    TempData["ErrorMessage"] = errorMessage ?? "Errore durante il salvataggio dell'elaborazione";
                    await ReloadEditViewDataAsync(lottoId);
                    return View(model);
                }

                // 5. Successo
                _logger.LogInformation("Elaborazione salvata (bozza) per Lotto {LottoId} da utente {UserId}", lottoId, userId);

                TempData["SuccessMessage"] = "Elaborazione salvata con successo. Puoi continuare a modificare o finalizzare quando pronto.";

                return RedirectToAction("Edit", new { lottoId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel salvataggio elaborazione per Lotto {LottoId}", lottoId);
                TempData["ErrorMessage"] = "Errore durante il salvataggio dell'elaborazione";
                return RedirectToAction("Edit", new { lottoId });
            }
        }

        /// <summary>
        /// Helper method per ricaricare i dati necessari alla view Edit in caso di errore
        /// </summary>
        private async Task ReloadEditViewDataAsync(Guid lottoId)
        {
            var lotto = await _lottoService.GetByIdAsync(lottoId);
            if (lotto == null) return;

            // Preventivi selezionati
            var preventiviSelezionati = await _preventivoService.GetByLottoIdAsync(lottoId);
            var preventiviSelezionatiList = preventiviSelezionati
                .Where(p => p.IsSelezionato)
                .OrderBy(p => p.ImportoOfferto)
                .ToList();

            // Documenti
            var documentiLotto = await _documentoGaraService.GetByLottoIdAsync(lottoId);
            var documentiList = documentiLotto
                .OrderByDescending(d => d.DataCaricamento)
                .ToList();

            // Statistiche preventivi
            decimal? importoMinimo = null;
            decimal? importoMassimo = null;
            decimal? importoMedio = null;
            int numeroPreventiviSelezionati = preventiviSelezionatiList.Count;

            if (numeroPreventiviSelezionati > 0)
            {
                var importi = preventiviSelezionatiList
                    .Where(p => p.ImportoOfferto.HasValue)
                    .Select(p => p.ImportoOfferto!.Value)
                    .ToList();

                if (importi.Any())
                {
                    importoMinimo = importi.Min();
                    importoMassimo = importi.Max();
                    importoMedio = Math.Round(importi.Average(), 2);
                }
            }

            // Popola ViewBag
            ViewBag.LottoId = lotto.Id;
            ViewBag.CodiceLotto = lotto.CodiceLotto;
            ViewBag.DescrizioneLotto = lotto.Descrizione;
            ViewBag.CodiceGara = lotto.CodiceGara;
            ViewBag.TitoloGara = lotto.TitoloGara;
            ViewBag.StatoLotto = lotto.Stato;
            ViewBag.ImportoBaseAsta = lotto.ImportoBaseAsta;
            ViewBag.Quotazione = lotto.Quotazione;
            ViewBag.GiorniFornitura = lotto.GiorniFornitura;
            ViewBag.PreventiviSelezionati = preventiviSelezionatiList;
            ViewBag.NumeroPreventiviSelezionati = numeroPreventiviSelezionati;
            ViewBag.DocumentiLotto = documentiList;
            ViewBag.NumeroDocumenti = documentiList.Count;
            ViewBag.ImportoMinimo = importoMinimo;
            ViewBag.ImportoMassimo = importoMassimo;
            ViewBag.ImportoMedio = importoMedio;
        }

        // ===================================
        // FINALIZZA ELABORAZIONE
        // ===================================

        /// <summary>
        /// POST: Finalizza elaborazione
        /// Validazioni rigide:
        /// - Prezzo Reale obbligatorio
        /// - Almeno 1 documento caricato
        /// - Elaborazione completa
        /// Cambia stato Lotto → Presentato
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Finalizza(Guid lottoId)
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

                // 2. Verifica stato = InElaborazione
                if (lotto.Stato != StatoLotto.InElaborazione)
                {
                    TempData["ErrorMessage"] = $"Il lotto deve essere in stato 'In Elaborazione' per finalizzare. Stato attuale: {GetDisplayName(lotto.Stato)}";
                    return RedirectToAction("Details", "Lotti", new { id = lottoId });
                }

                // 3. Recupera elaborazione
                var elaborazione = await _elaborazioneService.GetByLottoIdAsync(lottoId);
                if (elaborazione == null)
                {
                    TempData["ErrorMessage"] = "Elaborazione non trovata. Avvia prima l'elaborazione.";
                    return RedirectToAction("Avvia", new { lottoId });
                }

                // 4. VALIDAZIONE RIGIDA: Prezzo Reale obbligatorio ⭐
                if (!elaborazione.PrezzoRealeUscita.HasValue || elaborazione.PrezzoRealeUscita.Value <= 0)
                {
                    TempData["ErrorMessage"] = "Impossibile finalizzare: il Prezzo Reale Uscita è obbligatorio per presentare l'offerta.";
                    _logger.LogWarning("Tentativo finalizzazione senza Prezzo Reale per Lotto {LottoId}", lottoId);
                    return RedirectToAction("Edit", new { lottoId });
                }

                // 5. VALIDAZIONE RIGIDA: Almeno 1 documento caricato ⭐
                var documenti = await _documentoGaraService.GetByLottoIdAsync(lottoId);
                if (!documenti.Any())
                {
                    TempData["ErrorMessage"] = "Impossibile finalizzare: caricare almeno un documento prima di presentare l'offerta.";
                    _logger.LogWarning("Tentativo finalizzazione senza documenti per Lotto {LottoId}", lottoId);
                    return RedirectToAction("Edit", new { lottoId });
                }

                // 6. Validazione business: Se prezzi diversi → Motivazione obbligatoria
                if (elaborazione.PrezzoDesiderato.HasValue &&
                    elaborazione.PrezzoDesiderato.Value != elaborazione.PrezzoRealeUscita.Value)
                {
                    if (string.IsNullOrWhiteSpace(elaborazione.MotivazioneAdattamento))
                    {
                        TempData["ErrorMessage"] = "Impossibile finalizzare: la motivazione è obbligatoria quando il prezzo reale è diverso dal prezzo desiderato.";
                        _logger.LogWarning("Tentativo finalizzazione senza motivazione per Lotto {LottoId}", lottoId);
                        return RedirectToAction("Edit", new { lottoId });
                    }
                }

                // 7. Cambia stato Lotto → Presentato
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";
                var statoAggiornato = await _lottoService.UpdateStatoAsync(lottoId, StatoLotto.Presentato, userId);

                if (!statoAggiornato)
                {
                    TempData["ErrorMessage"] = "Errore nel cambio stato del lotto. Contattare l'amministratore.";
                    _logger.LogError("Impossibile cambiare stato a Presentato per Lotto {LottoId}", lottoId);
                    return RedirectToAction("Edit", new { lottoId });
                }

                // 8. Log successo
                _logger.LogInformation(
                    "Elaborazione finalizzata con successo per Lotto {LottoId} da utente {UserId}. " +
                    "Prezzo Reale: {PrezzoReale}, Documenti: {NumDocumenti}",
                    lottoId, userId, elaborazione.PrezzoRealeUscita, documenti.Count()
                );

                // 9. Redirect a Details Lotto con messaggio successo
                TempData["SuccessMessage"] = $"Elaborazione finalizzata con successo! Il lotto è stato presentato con prezzo di {elaborazione.PrezzoRealeUscita:C2}.";

                return RedirectToAction("Details", "Lotti", new { id = lottoId });
            }
            catch (InvalidOperationException ex)
            {
                // Errore transizione stato (da IsValidTransition)
                _logger.LogError(ex, "Errore transizione stato durante finalizzazione Lotto {LottoId}", lottoId);
                TempData["ErrorMessage"] = $"Errore: {ex.Message}";
                return RedirectToAction("Edit", new { lottoId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante finalizzazione elaborazione per Lotto {LottoId}", lottoId);
                TempData["ErrorMessage"] = "Errore durante la finalizzazione dell'elaborazione";
                return RedirectToAction("Edit", new { lottoId });
            }
        }

        // ===================================
        // ANNULLA ELABORAZIONE
        // ===================================

        /// <summary>
        /// POST: Annulla elaborazione
        /// - Elimina ElaborazioneLotto (soft delete)
        /// - Cambia stato Lotto → Approvato
        /// - Permette di riavviare l'elaborazione da zero
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Annulla(Guid lottoId)
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

                // 2. Verifica stato = InElaborazione
                if (lotto.Stato != StatoLotto.InElaborazione)
                {
                    TempData["ErrorMessage"] = $"Solo i lotti in stato 'In Elaborazione' possono essere annullati. Stato attuale: {GetDisplayName(lotto.Stato)}";
                    return RedirectToAction("Details", "Lotti", new { id = lottoId });
                }

                // 3. Recupera elaborazione
                var elaborazione = await _elaborazioneService.GetByLottoIdAsync(lottoId);
                if (elaborazione == null)
                {
                    // Elaborazione già eliminata o mai creata
                    _logger.LogWarning("Tentativo annullamento ma elaborazione non trovata per Lotto {LottoId}", lottoId);

                    // Riporta comunque lo stato ad Approvato
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";
                    await _lottoService.UpdateStatoAsync(lottoId, StatoLotto.Approvato, userId);

                    TempData["SuccessMessage"] = "Elaborazione annullata. Il lotto è stato riportato allo stato 'Approvato'.";
                    return RedirectToAction("Details", "Lotti", new { id = lottoId });
                }

                // 4. Elimina elaborazione (soft delete)
                var userId2 = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";
                var (deleteSuccess, deleteError) = await _elaborazioneService.DeleteAsync(elaborazione.Id, userId2);

                if (!deleteSuccess)
                {
                    TempData["ErrorMessage"] = deleteError ?? "Errore durante l'eliminazione dell'elaborazione";
                    _logger.LogError("Impossibile eliminare elaborazione {ElaborazioneId} per Lotto {LottoId}", elaborazione.Id, lottoId);
                    return RedirectToAction("Edit", new { lottoId });
                }

                // 5. Cambia stato Lotto → Approvato
                var statoAggiornato = await _lottoService.UpdateStatoAsync(lottoId, StatoLotto.Approvato, userId2);

                if (!statoAggiornato)
                {
                    TempData["ErrorMessage"] = "Elaborazione eliminata ma errore nel cambio stato del lotto.";
                    _logger.LogError("Elaborazione eliminata ma impossibile cambiare stato ad Approvato per Lotto {LottoId}", lottoId);
                    return RedirectToAction("Details", "Lotti", new { id = lottoId });
                }

                // 6. Log successo
                _logger.LogInformation(
                    "Elaborazione annullata con successo per Lotto {LottoId} da utente {UserId}. " +
                    "Elaborazione {ElaborazioneId} eliminata.",
                    lottoId, userId2, elaborazione.Id
                );

                // 7. Redirect a Details Lotto con messaggio successo
                TempData["SuccessMessage"] = "Elaborazione annullata con successo. Il lotto è stato riportato allo stato 'Approvato' e può essere rielaborato.";

                return RedirectToAction("Details", "Lotti", new { id = lottoId });
            }
            catch (InvalidOperationException ex)
            {
                // Errore transizione stato (da IsValidTransition)
                _logger.LogError(ex, "Errore transizione stato durante annullamento Lotto {LottoId}", lottoId);
                TempData["ErrorMessage"] = $"Errore: {ex.Message}";
                return RedirectToAction("Edit", new { lottoId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante annullamento elaborazione per Lotto {LottoId}", lottoId);
                TempData["ErrorMessage"] = "Errore durante l'annullamento dell'elaborazione";
                return RedirectToAction("Edit", new { lottoId });
            }
        }

        // ===================================
        // DETAILS ELABORAZIONE
        // ===================================

        /// <summary>
        /// GET: Visualizzazione read-only elaborazione completa
        /// Mostra prezzi, preventivi selezionati, documenti, audit trail
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Details(Guid lottoId)
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

                // 2. Recupera elaborazione
                var elaborazione = await _elaborazioneService.GetByLottoIdAsync(lottoId);
                if (elaborazione == null)
                {
                    TempData["ErrorMessage"] = "Elaborazione non trovata per questo lotto.";
                    return RedirectToAction("Details", "Lotti", new { id = lottoId });
                }

                // 3. Recupera preventivi selezionati
                var preventiviSelezionati = await _preventivoService.GetByLottoIdAsync(lottoId);
                var preventiviSelezionatiList = preventiviSelezionati
                    .Where(p => p.IsSelezionato)
                    .OrderBy(p => p.ImportoOfferto)
                    .ToList();

                // 4. Recupera documenti
                var documentiLotto = await _documentoGaraService.GetByLottoIdAsync(lottoId);
                var documentiList = documentiLotto
                    .OrderByDescending(d => d.DataCaricamento)
                    .ToList();

                // 5. Calcola statistiche preventivi
                decimal? importoMinimo = null;
                decimal? importoMassimo = null;
                decimal? importoMedio = null;
                int numeroPreventiviSelezionati = preventiviSelezionatiList.Count;

                if (numeroPreventiviSelezionati > 0)
                {
                    var importi = preventiviSelezionatiList
                        .Where(p => p.ImportoOfferto.HasValue)
                        .Select(p => p.ImportoOfferto!.Value)
                        .ToList();

                    if (importi.Any())
                    {
                        importoMinimo = importi.Min();
                        importoMassimo = importi.Max();
                        importoMedio = Math.Round(importi.Average(), 2);
                    }
                }

                // 6. Popola ViewBag
                ViewBag.LottoId = lotto.Id;
                ViewBag.CodiceLotto = lotto.CodiceLotto;
                ViewBag.DescrizioneLotto = lotto.Descrizione;
                ViewBag.CodiceGara = lotto.CodiceGara;
                ViewBag.TitoloGara = lotto.TitoloGara;
                ViewBag.StatoLotto = lotto.Stato;
                ViewBag.ImportoBaseAsta = lotto.ImportoBaseAsta;
                ViewBag.Quotazione = lotto.Quotazione;
                ViewBag.GiorniFornitura = lotto.GiorniFornitura;

                ViewBag.PreventiviSelezionati = preventiviSelezionatiList;
                ViewBag.NumeroPreventiviSelezionati = numeroPreventiviSelezionati;
                ViewBag.DocumentiLotto = documentiList;
                ViewBag.NumeroDocumenti = documentiList.Count;

                ViewBag.ImportoMinimo = importoMinimo;
                ViewBag.ImportoMassimo = importoMassimo;
                ViewBag.ImportoMedio = importoMedio;

                // 7. Usa DetailsViewModel per visualizzazione ricca
                // (include scostamenti, differenze, stato elaborazione)
                return View(elaborazione);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel caricamento dettaglio elaborazione per Lotto {LottoId}", lottoId);
                TempData["ErrorMessage"] = "Errore nel caricamento del dettaglio elaborazione";
                return RedirectToAction("Details", "Lotti", new { id = lottoId });
            }
        }

        /// <summary>
        /// Ottiene il nome visualizzabile di un enum dal Display attribute
        /// </summary>
        private static string GetDisplayName(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            if (field == null) return value.ToString();

            var attribute = field.GetCustomAttribute<DisplayAttribute>();
            return attribute?.Name ?? value.ToString();
        }
    }
}