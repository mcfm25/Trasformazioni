using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;
using Trasformazioni.Models.ViewModels;
using Trasformazioni.Services.Interfaces;

namespace Trasformazioni.Controllers
{
    [Authorize]
    public class ValutazioniController : Controller
    {
        private readonly IValutazioneLottoService _valutazioneLottoService;
        private readonly ILottoService _lottoService;
        private readonly IUserService _userService;
        private readonly ILogger<ValutazioniController> _logger;

        public ValutazioniController(
            IValutazioneLottoService valutazioneLottoService,
            ILottoService lottoService,
            IUserService userService,
            ILogger<ValutazioniController> logger)
        {
            _valutazioneLottoService = valutazioneLottoService;
            _lottoService = lottoService;
            _userService = userService;
            _logger = logger;
        }

        // ===================================
        // VALUTAZIONE TECNICA
        // ===================================

        /// <summary>
        /// GET: /Valutazioni/CreateTecnica?lottoId={id}
        /// Form per creare valutazione tecnica
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> CreateTecnica(Guid lottoId)
        {
            try
            {
                // Verifica che il lotto esista
                var lotto = await _lottoService.GetByIdAsync(lottoId);
                if (lotto == null)
                {
                    TempData["ErrorMessage"] = "Lotto non trovato.";
                    return RedirectToAction("Index", "Lotti");
                }

                // Verifica se esiste già una valutazione
                var valutazioneEsistente = await _valutazioneLottoService.GetByLottoIdAsync(lottoId);
                if (valutazioneEsistente != null && valutazioneEsistente.DataValutazioneTecnica.HasValue)
                {
                    TempData["WarningMessage"] = "Esiste già una valutazione tecnica per questo lotto.";
                    return RedirectToAction("Details", "Lotti", new { id = lottoId });
                }

                // Prepara model con info lotto
                var model = new ValutazioneTecnicaViewModel
                {
                    LottoId = lottoId,
                    GaraId = lotto.GaraId,
                    CodiceLotto = lotto.CodiceLotto,
                    DescrizioneLotto = lotto.Descrizione,
                    CodiceGara = lotto.CodiceGara,
                    TitoloGara = lotto.TitoloGara,
                    ImportoBaseAsta = lotto.ImportoBaseAsta,
                    TecnicaApprovata = false // Default a false per sicurezza
                };

                // Dropdown valutatori (tutti gli utenti)
                await PrepareValutatoriDropdown();

                // Info preventivi disponibili (per riferimento)
                ViewBag.NumeroDocTecnici = lotto.NumeroDocumentiValutazioneTecnica;

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel caricamento form valutazione tecnica per lotto {LottoId}", lottoId);
                TempData["ErrorMessage"] = "Errore nel caricamento del form.";
                return RedirectToAction("Details", "Lotti", new { id = lottoId });
            }
        }

        /// <summary>
        /// POST: /Valutazioni/CreateTecnica
        /// Salva valutazione tecnica
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTecnica(ValutazioneTecnicaViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await PrepareValutatoriDropdown();
                    return View(model);
                }

                // Ottieni userId corrente
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    TempData["ErrorMessage"] = "Utente non autenticato.";
                    return RedirectToAction("Login", "Account");
                }

                // Cambio stato automatico del lotto
                if (model.TecnicaApprovata)
                {
                    // Approvata → passa a InValutazioneEconomica
                    await _lottoService.UpdateStatoAsync(model.LottoId, StatoLotto.InValutazioneEconomica, userId);
                    TempData["SuccessMessage"] = "Valutazione tecnica approvata! Il lotto è passato in valutazione economica.";
                }
                else
                {
                    // Rifiutata → passa a Rifiutato
                    await _lottoService.UpdateStatoAsync(model.LottoId, StatoLotto.Rifiutato, userId);
                    TempData["WarningMessage"] = "Valutazione tecnica rifiutata. Il lotto è stato rifiutato.";
                }

                // Crea valutazione tecnica
                var (success, errorMessage, valutazioneId) = await _valutazioneLottoService.ValutaTecnicamenteAsync(model, userId);

                if (!success)
                {
                    ModelState.AddModelError("", errorMessage ?? "Errore nella creazione della valutazione tecnica.");
                    await PrepareValutatoriDropdown();
                    return View(model);
                }

                return RedirectToAction("Details", "Lotti", new { id = model.LottoId });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Errore nella creazione della valutazione tecnica per lotto {LottoId}", model.LottoId);
                TempData["ErrorMessage"] = ex.Message;
                await PrepareValutatoriDropdown();
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nella creazione della valutazione tecnica per lotto {LottoId}", model.LottoId);
                TempData["ErrorMessage"] = "Errore nel salvataggio della valutazione tecnica.";
                await PrepareValutatoriDropdown();
                return View(model);
            }
        }

        // ===================================
        // VALUTAZIONE ECONOMICA
        // ===================================

        /// <summary>
        /// GET: /Valutazioni/CreateEconomica?lottoId={id}
        /// Form per completare valutazione economica
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> CreateEconomica(Guid lottoId)
        {
            try
            {
                // Verifica che il lotto esista
                var lotto = await _lottoService.GetByIdAsync(lottoId);
                if (lotto == null)
                {
                    TempData["ErrorMessage"] = "Lotto non trovato.";
                    return RedirectToAction("Index", "Lotti");
                }

                // Verifica che esista una valutazione tecnica approvata
                var valutazione = await _valutazioneLottoService.GetByLottoIdAsync(lottoId);
                if (valutazione == null || !valutazione.DataValutazioneTecnica.HasValue)
                {
                    TempData["ErrorMessage"] = "Non esiste una valutazione tecnica per questo lotto.";
                    return RedirectToAction("Details", "Lotti", new { id = lottoId });
                }

                if (valutazione.TecnicaApprovata != true)
                {
                    TempData["ErrorMessage"] = "La valutazione tecnica non è stata approvata. Impossibile procedere con la valutazione economica.";
                    return RedirectToAction("Details", "Lotti", new { id = lottoId });
                }

                // Verifica se esiste già valutazione economica
                if (valutazione.DataValutazioneEconomica.HasValue)
                {
                    TempData["WarningMessage"] = "Esiste già una valutazione economica per questo lotto.";
                    return RedirectToAction("Details", "Lotti", new { id = lottoId });
                }

                // Prepara model con info lotto e valutazione tecnica
                var model = new ValutazioneEconomicaViewModel
                {
                    GaraId = lotto.GaraId,
                    LottoId = lottoId,
                    CodiceLotto = lotto.CodiceLotto,
                    DescrizioneLotto = lotto.Descrizione,
                    CodiceGara = lotto.CodiceGara,
                    TitoloGara = lotto.TitoloGara,
                    ImportoBaseAsta = lotto.ImportoBaseAsta,
                    DataValutazioneTecnica = valutazione.DataValutazioneTecnica,
                    ValutatoreTecnicoNome = valutazione.ValutatoreTecnicoNome,
                    TecnicaApprovata = valutazione.TecnicaApprovata,
                    EconomicaApprovata = false // Default a false per sicurezza
                };

                // Dropdown valutatori (tutti gli utenti)
                await PrepareValutatoriDropdown();

                // Info preventivi disponibili (per riferimento)
                ViewBag.NumeroPreventivi = lotto.NumeroPreventivi;
                ViewBag.NumeroDocumentiValutazioneEconomica = lotto.NumeroDocumentiValutazioneEconomica;
                ViewBag.NumeroDocumentiValutazioneTecnica = lotto.NumeroDocumentiValutazioneTecnica;

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel caricamento form valutazione economica per lotto {LottoId}", lottoId);
                TempData["ErrorMessage"] = "Errore nel caricamento del form.";
                return RedirectToAction("Details", "Lotti", new { id = lottoId });
            }
        }

        /// <summary>
        /// POST: /Valutazioni/CreateEconomica
        /// Salva valutazione economica
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEconomica(ValutazioneEconomicaViewModel model)
        {
            try
            {
                var lotto = await _lottoService.GetByIdAsync(model.LottoId);
                // Info preventivi disponibili (per riferimento)
                ViewBag.NumeroPreventivi = lotto.NumeroPreventivi;
                if (!ModelState.IsValid)
                {
                    await PrepareValutatoriDropdown();
                    return View(model);
                }

                // Ottieni userId corrente
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    TempData["ErrorMessage"] = "Utente non autenticato.";
                    return RedirectToAction("Login", "Account");
                }

                // Cambio stato automatico del lotto
                if (model.EconomicaApprovata)
                {
                    // Approvata → passa a Approvato
                    await _lottoService.UpdateStatoAsync(model.LottoId, StatoLotto.Approvato, userId);
                    TempData["SuccessMessage"] = "Valutazione economica approvata! Il lotto è stato approvato.";
                }
                else
                {
                    // Rifiutata → passa a Rifiutato
                    await _lottoService.UpdateStatoAsync(model.LottoId, StatoLotto.Rifiutato, userId);
                    TempData["WarningMessage"] = "Valutazione economica rifiutata. Il lotto è stato rifiutato.";
                }

                // Crea valutazione economica
                var (success, errorMessage, valutazioneId) = await _valutazioneLottoService.ValutaEconomicamenteAsync(model, userId);

                if (!success)
                {
                    ModelState.AddModelError("", errorMessage ?? "Errore nella creazione della valutazione economica.");
                    await PrepareValutatoriDropdown();
                    return View(model);
                }

                return RedirectToAction("Details", "Lotti", new { id = model.LottoId });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Errore nella creazione della valutazione tecnica per lotto {LottoId}", model.LottoId);
                TempData["ErrorMessage"] = ex.Message;
                await PrepareValutatoriDropdown();
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nella creazione della valutazione economica per lotto {LottoId}", model.LottoId);
                TempData["ErrorMessage"] = "Errore nel salvataggio della valutazione economica.";
                await PrepareValutatoriDropdown();
                return View(model);
            }
        }

        // ===================================
        // DETTAGLIO VALUTAZIONE
        // ===================================

        /// <summary>
        /// GET: /Valutazioni/Details/{lottoId}
        /// Visualizza dettaglio valutazione completa
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Details(Guid lottoId)
        {
            try
            {
                var valutazione = await _valutazioneLottoService.GetByLottoIdAsync(lottoId);

                if (valutazione == null)
                {
                    TempData["ErrorMessage"] = "Valutazione non trovata.";
                    return RedirectToAction("Details", "Lotti", new { id = lottoId });
                }

                return View(valutazione);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel caricamento dettaglio valutazione per lotto {LottoId}", lottoId);
                TempData["ErrorMessage"] = "Errore nel caricamento della valutazione.";
                return RedirectToAction("Details", "Lotti", new { id = lottoId });
            }
        }

        // ===================================
        // HELPER METHODS
        // ===================================

        /// <summary>
        /// Prepara dropdown valutatori (tutti gli utenti attivi)
        /// </summary>
        private async Task PrepareValutatoriDropdown()
        {
            try
            {
                var utenti = await _userService.GetAllUsersAsync();

                ViewBag.Valutatori = utenti
                    .Where(u => u.IsAttivo)
                    .Select(u => new SelectListItem
                    {
                        Value = u.Id,
                        Text = u.NomeCompleto
                    })
                    .OrderBy(x => x.Text)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel caricamento lista valutatori");
                ViewBag.Valutatori = new List<SelectListItem>();
            }
        }
    }
}