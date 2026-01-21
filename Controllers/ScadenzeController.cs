using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using Trasformazioni.Models.Enums;
using Trasformazioni.Models.ViewModels;
using Trasformazioni.Services.Interfaces;

namespace Trasformazioni.Controllers
{
    /// <summary>
    /// Controller per la gestione delle Scadenze (Scadenzario)
    /// Supporta sia scadenze manuali che automatiche
    /// Include dashboard con calendario e azioni rapide
    /// </summary>
    [Authorize]
    public class ScadenzeController : Controller
    {
        private readonly IScadenzaService _scadenzaService;
        private readonly IGaraService _garaService;
        private readonly ILottoService _lottoService;
        private readonly IPreventivoService _preventivoService;
        private readonly ILogger<ScadenzeController> _logger;

        public ScadenzeController(
            IScadenzaService scadenzaService,
            IGaraService garaService,
            ILottoService lottoService,
            IPreventivoService preventivoService,
            ILogger<ScadenzeController> logger)
        {
            _scadenzaService = scadenzaService;
            _garaService = garaService;
            _lottoService = lottoService;
            _preventivoService = preventivoService;
            _logger = logger;
        }

        // ===================================
        // INDEX - DASHBOARD SCADENZARIO
        // ===================================

        /// <summary>
        /// Dashboard principale con calendario e lista scadenze imminenti
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index(ScadenzaFilterViewModel? filters = null)
        {
            try
            {
                filters ??= new ScadenzaFilterViewModel();

                // Carica scadenze imminenti (prossimi 7 giorni)
                var scadenzeImminenti = await _scadenzaService.GetInScadenzaAsync(7);
                ViewBag.ScadenzeImminenti = scadenzeImminenti;

                // Carica scadenze scadute (non completate)
                var scadenzeScadute = await _scadenzaService.GetScaduteAsync();
                ViewBag.ScadenzeScadute = scadenzeScadute;

                // Carica scadenze di oggi
                var scadenzeOggi = await _scadenzaService.GetOggiAsync();
                ViewBag.ScadenzeOggi = scadenzeOggi;

                // Statistiche rapide
                var tutteScadenze = await _scadenzaService.GetAllAsync();
                ViewBag.TotaleScadenze = tutteScadenze.Count();
                ViewBag.TotaleAttive = tutteScadenze.Count(s => !s.IsCompletata);
                ViewBag.TotaleCompletate = tutteScadenze.Count(s => s.IsCompletata);
                ViewBag.TotaleScadute = scadenzeScadute.Count();

                // Carica dropdown per filtri
                await LoadFilterDropdownsAsync();

                return View(filters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento della dashboard scadenze");
                TempData["ErrorMessage"] = "Errore durante il caricamento dello scadenzario";
                return RedirectToAction("Index", "Home");
            }
        }

        // ===================================
        // API JSON PER CALENDARIO
        // ===================================

        /// <summary>
        /// Restituisce le scadenze in formato JSON per FullCalendar
        /// </summary>
        /// <param name="start">Data inizio range</param>
        /// <param name="end">Data fine range</param>
        /// <param name="tipo">Filtro tipo scadenza (opzionale)</param>
        /// <param name="garaId">Filtro gara (opzionale)</param>
        /// <param name="lottoId">Filtro lotto (opzionale)</param>
        /// <param name="mostraCompletate">Mostra anche completate (default true)</param>
        [HttpGet]
        public async Task<IActionResult> GetEventiCalendario(
            DateTime start,
            DateTime end,
            TipoScadenza? tipo = null,
            Guid? garaId = null,
            Guid? lottoId = null,
            bool mostraCompletate = true)
        {
            try
            {
                // Recupera scadenze nel range
                var scadenze = await _scadenzaService.GetByDataRangeAsync(start, end);

                // Applica filtri
                if (tipo.HasValue)
                {
                    scadenze = scadenze.Where(s => s.Tipo == tipo.Value);
                }

                if (garaId.HasValue)
                {
                    scadenze = scadenze.Where(s => s.GaraId == garaId.Value);
                }

                if (lottoId.HasValue)
                {
                    scadenze = scadenze.Where(s => s.LottoId == lottoId.Value);
                }

                if (!mostraCompletate)
                {
                    scadenze = scadenze.Where(s => !s.IsCompletata);
                }

                // Mappa a formato FullCalendar
                var eventi = scadenze.Select(s => new
                {
                    id = s.Id.ToString(),
                    title = GetTitoloEvento(s),
                    start = s.DataScadenza.ToString("yyyy-MM-dd"),
                    end = s.DataScadenza.ToString("yyyy-MM-dd"),
                    url = Url.Action("Details", new { id = s.Id }),
                    backgroundColor = GetColoreEvento(s),
                    borderColor = GetColoreEvento(s),
                    textColor = GetTestoColoreEvento(s),
                    extendedProps = new
                    {
                        tipo = s.Tipo.ToString(),
                        descrizione = s.Descrizione,
                        isCompletata = s.IsCompletata,
                        isAutomatica = s.IsAutomatica,
                        codiceGara = s.CodiceGara,
                        codiceLotto = s.CodiceLotto
                    }
                }).ToList();

                return Json(eventi);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero degli eventi calendario");
                return Json(new List<object>());
            }
        }

        // ===================================
        // CREATE - CREAZIONE SCADENZA
        // ===================================

        /// <summary>
        /// Mostra il form di creazione
        /// Supporta preimpostazione da Gara, Lotto o Preventivo
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Create(Guid? garaId = null, Guid? lottoId = null, Guid? preventivoId = null)
        {
            try
            {
                var model = new ScadenzaCreateViewModel
                {
                    DataScadenza = DateTime.Today.AddDays(7),
                    GiorniPreavviso = 7
                };

                // Preimposta da Gara
                if (garaId.HasValue)
                {
                    var gara = await _garaService.GetByIdAsync(garaId.Value);
                    if (gara != null)
                    {
                        model.GaraId = garaId.Value;
                        model.CodiceGara = gara.CodiceGara;
                        model.TitoloGara = gara.Titolo;
                        ViewBag.EntitaPreimpostata = "Gara";
                        ViewBag.EntitaCodice = gara.CodiceGara;
                    }
                }

                // Preimposta da Lotto
                if (lottoId.HasValue)
                {
                    var lotto = await _lottoService.GetByIdAsync(lottoId.Value);
                    if (lotto != null)
                    {
                        model.LottoId = lottoId.Value;
                        model.GaraId = lotto.GaraId;
                        model.CodiceLotto = lotto.CodiceLotto;
                        model.DescrizioneLotto = lotto.Descrizione;
                        model.CodiceGara = lotto.CIG;
                        ViewBag.EntitaPreimpostata = "Lotto";
                        ViewBag.EntitaCodice = lotto.CodiceLotto;
                    }
                }

                // Preimposta da Preventivo
                if (preventivoId.HasValue)
                {
                    var preventivo = await _preventivoService.GetByIdAsync(preventivoId.Value);
                    if (preventivo != null)
                    {
                        model.PreventivoId = preventivoId.Value;
                        model.LottoId = preventivo.LottoId;
                        model.FornitorePreventivo = preventivo.NomeFornitore;
                        ViewBag.EntitaPreimpostata = "Preventivo";
                        ViewBag.EntitaCodice = preventivo.NomeFornitore;
                    }
                }

                // Carica dropdown
                await LoadDropdownsAsync(model.GaraId, model.LottoId);

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento del form di creazione scadenza");
                TempData["ErrorMessage"] = "Errore durante il caricamento del form";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Salva la nuova scadenza
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ScadenzaCreateViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await LoadDropdownsAsync(model.GaraId, model.LottoId);
                    return View(model);
                }

                var (success, errorMessage, id) = await _scadenzaService.CreateAsync(model);

                if (!success)
                {
                    ModelState.AddModelError(string.Empty, errorMessage ?? "Errore durante la creazione della scadenza");
                    await LoadDropdownsAsync(model.GaraId, model.LottoId);
                    return View(model);
                }

                TempData["SuccessMessage"] = "Scadenza creata con successo";

                // Redirect intelligente basato sul contesto
                if (model.LottoId.HasValue)
                {
                    return RedirectToAction("Details", "Lotti", new { id = model.LottoId.Value });
                }
                if (model.GaraId.HasValue)
                {
                    return RedirectToAction("Details", "Gare", new { id = model.GaraId.Value });
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la creazione della scadenza");
                ModelState.AddModelError(string.Empty, "Errore durante la creazione della scadenza");
                await LoadDropdownsAsync(model.GaraId, model.LottoId);
                return View(model);
            }
        }

        // ===================================
        // EDIT - MODIFICA SCADENZA
        // ===================================

        /// <summary>
        /// Mostra il form di modifica
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var details = await _scadenzaService.GetByIdAsync(id);
                if (details == null)
                {
                    _logger.LogWarning("Scadenza {Id} non trovata per modifica", id);
                    TempData["ErrorMessage"] = "Scadenza non trovata.";
                    return RedirectToAction(nameof(Index));
                }


                // Mappa manualmente a EditViewModel
                var model = new ScadenzaEditViewModel
                {
                    Id = details.Id,
                    Tipo = details.Tipo,
                    DataScadenza = details.DataScadenza,
                    Descrizione = details.Descrizione,
                    Note = details.Note,
                    GiorniPreavviso = details.GiorniPreavviso,
                    IsAutomatica = details.IsAutomatica,
                    IsCompletata = details.IsCompletata,
                    DataCompletamento = details.DataCompletamento,
                    GaraId = details.GaraId,
                    LottoId = details.LottoId,
                    PreventivoId = details.PreventivoId,
                    CodiceGara = details.CodiceGara,
                    TitoloGara = details.TitoloGara,
                    CodiceLotto = details.CodiceLotto,
                    DescrizioneLotto = details.DescrizioneLotto,
                    FornitorePreventivo = details.FornitorePreventivo,
                    CreatedAt = details.CreatedAt,
                    ModifiedAt = details.ModifiedAt
                };

                // Avviso per scadenze automatiche
                if (model.IsAutomatica)
                {
                    TempData["WarningMessage"] = "Questa è una scadenza automatica. Alcune modifiche potrebbero essere limitate.";
                }

                await LoadDropdownsAsync(model.GaraId, model.LottoId);

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento del form di modifica scadenza {Id}", id);
                TempData["ErrorMessage"] = "Errore durante il caricamento del form di modifica";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Salva le modifiche alla scadenza
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ScadenzaEditViewModel model)
        {
            try
            {
                if (id != model.Id)
                {
                    return BadRequest("ID non corrispondente");
                }

                if (!ModelState.IsValid)
                {
                    await LoadDropdownsAsync(model.GaraId, model.LottoId);
                    return View(model);
                }

                var (success, errorMessage) = await _scadenzaService.UpdateAsync(model);

                if (!success)
                {
                    ModelState.AddModelError(string.Empty, errorMessage ?? "Errore durante l'aggiornamento della scadenza");
                    await LoadDropdownsAsync(model.GaraId, model.LottoId);
                    return View(model);
                }

                TempData["SuccessMessage"] = "Scadenza aggiornata con successo";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'aggiornamento della scadenza {Id}", id);
                ModelState.AddModelError(string.Empty, "Errore durante l'aggiornamento della scadenza");
                await LoadDropdownsAsync(model.GaraId, model.LottoId);
                return View(model);
            }
        }

        // ===================================
        // DETAILS - DETTAGLIO SCADENZA
        // ===================================

        /// <summary>
        /// Mostra il dettaglio completo della scadenza
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var model = await _scadenzaService.GetByIdAsync(id);
                if (model == null)
                {
                    _logger.LogWarning("Scadenza {Id} non trovata", id);
                    TempData["ErrorMessage"] = "La scadenza non è stata trovata o è stata eliminata.";
                    return RedirectToAction(nameof(Index));
                }


                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento del dettaglio scadenza {Id}", id);
                TempData["ErrorMessage"] = "Errore durante il caricamento del dettaglio";
                return RedirectToAction(nameof(Index));
            }
        }

        // ===================================
        // DELETE - ELIMINAZIONE SCADENZA
        // ===================================

        /// <summary>
        /// Mostra la conferma di eliminazione
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var model = await _scadenzaService.GetByIdAsync(id);
                if (model == null)
                {
                    _logger.LogWarning("Scadenza {Id} non trovata per eliminazione", id);
                    TempData["ErrorMessage"] = "Scadenza non trovata.";
                    return RedirectToAction(nameof(Index));
                }


                // Blocca eliminazione scadenze automatiche
                if (model.IsAutomatica)
                {
                    TempData["ErrorMessage"] = "Le scadenze automatiche non possono essere eliminate manualmente.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento della conferma eliminazione scadenza {Id}", id);
                TempData["ErrorMessage"] = "Errore durante il caricamento della conferma eliminazione";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Elimina la scadenza (soft delete)
        /// </summary>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var (success, errorMessage) = await _scadenzaService.DeleteAsync(id);

                if (!success)
                {
                    TempData["ErrorMessage"] = errorMessage ?? "Errore durante l'eliminazione della scadenza";
                    return RedirectToAction(nameof(Delete), new { id });
                }

                TempData["SuccessMessage"] = "Scadenza eliminata con successo";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'eliminazione della scadenza {Id}", id);
                TempData["ErrorMessage"] = "Errore durante l'eliminazione della scadenza";
                return RedirectToAction(nameof(Delete), new { id });
            }
        }

        // ===================================
        // AZIONI RAPIDE (AJAX)
        // ===================================

        /// <summary>
        /// Marca una scadenza come completata (AJAX)
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Completa(Guid id)
        {
            try
            {
                var (success, errorMessage) = await _scadenzaService.CompletaAsync(id);

                if (!success)
                {
                    return Json(new { success = false, message = errorMessage ?? "Errore durante il completamento" });
                }

                return Json(new { success = true, message = "Scadenza completata con successo" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il completamento della scadenza {Id}", id);
                return Json(new { success = false, message = "Errore durante il completamento della scadenza" });
            }
        }

        /// <summary>
        /// Riapre una scadenza completata (AJAX)
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Riapri(Guid id)
        {
            try
            {
                var (success, errorMessage) = await _scadenzaService.RiattivaAsync(id);

                if (!success)
                {
                    return Json(new { success = false, message = errorMessage ?? "Errore durante la riapertura" });
                }

                return Json(new { success = true, message = "Scadenza riaperta con successo" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la riapertura della scadenza {Id}", id);
                return Json(new { success = false, message = "Errore durante la riapertura della scadenza" });
            }
        }

        // ===================================
        // LISTE PER ENTITÀ CORRELATE
        // ===================================

        /// <summary>
        /// Lista scadenze per una gara specifica
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> PerGara(Guid garaId)
        {
            try
            {
                var scadenze = await _scadenzaService.GetByGaraIdAsync(garaId);
                var gara = await _garaService.GetByIdAsync(garaId);

                if (gara == null)
                {
                    return NotFound("Gara non trovata");
                }

                ViewBag.GaraId = garaId;
                ViewBag.GaraCodice = gara.CodiceGara;
                ViewBag.GaraTitolo = gara.Titolo;

                return View("ListaPerEntita", scadenze);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento delle scadenze per gara {GaraId}", garaId);
                TempData["ErrorMessage"] = "Errore durante il caricamento delle scadenze";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Lista scadenze per un lotto specifico
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> PerLotto(Guid lottoId)
        {
            try
            {
                var scadenze = await _scadenzaService.GetByLottoIdAsync(lottoId);
                var lotto = await _lottoService.GetByIdAsync(lottoId);

                if (lotto == null)
                {
                    return NotFound("Lotto non trovato");
                }

                ViewBag.LottoId = lottoId;
                ViewBag.LottoCodice = lotto.CodiceLotto;
                ViewBag.LottoTitolo = lotto.Descrizione;
                ViewBag.GaraCig = lotto.CIG;

                return View("ListaPerEntita", scadenze);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento delle scadenze per lotto {LottoId}", lottoId);
                TempData["ErrorMessage"] = "Errore durante il caricamento delle scadenze";
                return RedirectToAction(nameof(Index));
            }
        }

        // ===================================
        // HELPER METHODS
        // ===================================

        /// <summary>
        /// Carica i dropdown per i filtri nella Index
        /// </summary>
        private async Task LoadFilterDropdownsAsync()
        {
            // Dropdown Gare
            var gare = await _garaService.GetAllAsync();
            ViewBag.GareFilter = new SelectList(
                gare.Select(g => new { Id = g.Id, Display = $"{g.CodiceGara} - {g.Titolo}" }),
                "Id", "Display"
            );

            // Dropdown Lotti
            var lotti = await _lottoService.GetAllAsync();
            ViewBag.LottiFilter = new SelectList(
                lotti.Select(l => new { Id = l.Id, Display = $"{l.CodiceLotto} - {l.Descrizione}" }),
                "Id", "Display"
            );

            // Dropdown Tipi Scadenza
            ViewBag.TipiScadenza = Enum.GetValues(typeof(TipoScadenza))
                .Cast<TipoScadenza>()
                .Select(t => new SelectListItem
                {
                    Value = ((int)t).ToString(),
                    Text = GetTipoScadenzaDisplayName(t)
                });
        }

        /// <summary>
        /// Carica i dropdown per i form Create/Edit
        /// </summary>
        private async Task LoadDropdownsAsync(Guid? currentGaraId = null, Guid? currentLottoId = null)
        {
            // Dropdown Gare
            var gare = await _garaService.GetAllAsync();
            ViewBag.Gare = new SelectList(
                gare.Select(g => new { Id = g.Id, Display = $"{g.CodiceGara} - {g.Titolo}" }),
                "Id", "Display", currentGaraId
            );

            // Dropdown Lotti (filtrati per gara se specificata)
            var lotti = await _lottoService.GetAllAsync();
            if (currentGaraId.HasValue)
            {
                lotti = lotti.Where(l => l.GaraId == currentGaraId.Value);
            }
            ViewBag.Lotti = new SelectList(
                lotti.Select(l => new { Id = l.Id, Display = $"{l.CodiceLotto} - {l.Descrizione}" }),
                "Id", "Display", currentLottoId
            );

            // Dropdown Preventivi (filtrati per lotto se specificato)
            if (currentLottoId.HasValue)
            {
                var preventivi = await _preventivoService.GetByLottoIdAsync(currentLottoId.Value);
                ViewBag.Preventivi = new SelectList(
                    preventivi.Select(p => new { Id = p.Id, Display = $"{p.NomeFornitore} - {p.ImportoOfferto:C2}" }),
                    "Id", "Display"
                );
            }
            else
            {
                ViewBag.Preventivi = new SelectList(Enumerable.Empty<SelectListItem>());
            }

            // Dropdown Tipi Scadenza
            ViewBag.TipiScadenza = Enum.GetValues(typeof(TipoScadenza))
                .Cast<TipoScadenza>()
                .Select(t => new SelectListItem
                {
                    Value = ((int)t).ToString(),
                    Text = GetTipoScadenzaDisplayName(t)
                });
        }

        /// <summary>
        /// Ottiene il titolo per l'evento calendario
        /// </summary>
        private string GetTitoloEvento(ScadenzaListViewModel scadenza)
        {
            var prefix = scadenza.IsCompletata ? "✓ " : "";
            var tipo = GetTipoScadenzaShortName(scadenza.Tipo);

            if (!string.IsNullOrEmpty(scadenza.CodiceLotto))
            {
                return $"{prefix}{tipo}: {scadenza.CodiceLotto}";
            }
            if (!string.IsNullOrEmpty(scadenza.CodiceGara))
            {
                return $"{prefix}{tipo}: {scadenza.CodiceGara}";
            }

            return $"{prefix}{tipo}";
        }

        /// <summary>
        /// Ottiene il colore di sfondo per l'evento calendario basato sullo stato
        /// </summary>
        private string GetColoreEvento(ScadenzaListViewModel scadenza)
        {
            if (scadenza.IsCompletata)
                return "#198754"; // Verde (success)

            if (scadenza.IsScaduta)
                return "#dc3545"; // Rosso (danger)

            if (scadenza.IsOggi)
                return "#fd7e14"; // Arancione (warning intenso)

            if (scadenza.IsInScadenza)
                return "#ffc107"; // Giallo (warning)

            return "#0d6efd"; // Blu (primary)
        }

        /// <summary>
        /// Ottiene il colore del testo per l'evento calendario
        /// </summary>
        private string GetTestoColoreEvento(ScadenzaListViewModel scadenza)
        {
            // Per sfondi chiari (giallo), usa testo scuro
            if (scadenza.IsInScadenza && !scadenza.IsOggi && !scadenza.IsScaduta && !scadenza.IsCompletata)
                return "#000000";

            return "#ffffff";
        }

        /// <summary>
        /// Ottiene il nome breve del tipo scadenza
        /// </summary>
        private string GetTipoScadenzaShortName(TipoScadenza tipo)
        {
            return tipo switch
            {
                TipoScadenza.PresentazioneOfferta => "Offerta",
                TipoScadenza.RichiestaChiarimenti => "Chiarimenti",
                TipoScadenza.ScadenzaPreventivo => "Preventivo",
                TipoScadenza.IntegrazioneDocumentazione => "Integrazione",
                TipoScadenza.StipulaContratto => "Stipula",
                TipoScadenza.ScadenzaContratto => "Contratto",
                TipoScadenza.Altro => "Altro",
                _ => tipo.ToString()
            };
        }

        /// <summary>
        /// Ottiene il nome display del tipo scadenza
        /// </summary>
        private string GetTipoScadenzaDisplayName(TipoScadenza tipo)
        {
            return tipo switch
            {
                TipoScadenza.PresentazioneOfferta => "Presentazione Offerta",
                TipoScadenza.RichiestaChiarimenti => "Richiesta Chiarimenti",
                TipoScadenza.ScadenzaPreventivo => "Scadenza Preventivo",
                TipoScadenza.IntegrazioneDocumentazione => "Integrazione Documentazione",
                TipoScadenza.StipulaContratto => "Stipula Contratto",
                TipoScadenza.ScadenzaContratto => "Scadenza Contratto",
                TipoScadenza.Altro => "Altro",
                _ => tipo.ToString()
            };
        }
    }
}