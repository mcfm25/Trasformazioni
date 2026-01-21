using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using Trasformazioni.Authorization;
using Trasformazioni.Models.Enums;
using Trasformazioni.Models.ViewModels;
using Trasformazioni.Services.Interfaces;

namespace Trasformazioni.Controllers
{
    /// <summary>
    /// Controller per la gestione dei soggetti (Clienti e Fornitori)
    /// </summary>
    [Authorize]
    public class SoggettiController : Controller
    {
        private readonly ISoggettoService _soggettoService;
        private readonly ILogger<SoggettiController> _logger;

        public SoggettiController(
            ISoggettoService soggettoService,
            ILogger<SoggettiController> logger)
        {
            _soggettoService = soggettoService;
            _logger = logger;
        }

        // ===================================
        // INDEX - LISTA PAGINATA
        // ===================================

        /// <summary>
        /// GET: /Soggetti
        /// Lista paginata dei soggetti con filtri
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index(SoggettoFilterViewModel filters)
        {
            try
            {
                // Ottieni risultati paginati
                var pagedResult = await _soggettoService.GetPagedAsync(filters);

                // Prepara dropdown per filtri
                PrepareFilterDropdowns();

                // Passa i filtri correnti alla view tramite ViewBag
                ViewBag.CurrentFilters = filters;

                return View(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento della lista soggetti");
                TempData["ErrorMessage"] = "Errore durante il caricamento della lista soggetti";
                return View(new PagedResult<SoggettoListViewModel>());
            }
        }

        // ===================================
        // DETAILS - DETTAGLIO
        // ===================================

        /// <summary>
        /// GET: /Soggetti/Details/5
        /// Visualizza i dettagli di un soggetto
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var soggetto = await _soggettoService.GetByIdAsync(id);

                if (soggetto == null)
                {
                    TempData["ErrorMessage"] = "Soggetto non trovato";
                    return RedirectToAction(nameof(Index));
                }

                return View(soggetto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento del dettaglio soggetto {SoggettoId}", id);
                TempData["ErrorMessage"] = "Errore durante il caricamento del soggetto";
                return RedirectToAction(nameof(Index));
            }
        }

        // ===================================
        // CREATE - CREAZIONE
        // ===================================

        /// <summary>
        /// GET: /Soggetti/Create
        /// Form di creazione nuovo soggetto
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Amministrazione,Manager")]
        public IActionResult Create()
        {
            PrepareCreateEditDropdowns();
            return View(new SoggettoCreateViewModel());
        }

        /// <summary>
        /// POST: /Soggetti/Create
        /// Salva nuovo soggetto
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Amministrazione,Manager")]
        public async Task<IActionResult> Create(SoggettoCreateViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    PrepareCreateEditDropdowns();
                    return View(model);
                }

                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(currentUserId))
                {
                    TempData["ErrorMessage"] = "Utente non autenticato";
                    return RedirectToAction(nameof(Index));
                }

                var (success, errorMessage, soggettoId) = await _soggettoService.CreateAsync(model, currentUserId);

                if (!success)
                {
                    ModelState.AddModelError(string.Empty, errorMessage ?? "Errore durante la creazione del soggetto");
                    PrepareCreateEditDropdowns();
                    return View(model);
                }

                _logger.LogInformation("Soggetto creato con successo: {SoggettoId} da utente {UserId}", soggettoId, currentUserId);
                TempData["SuccessMessage"] = "Soggetto creato con successo";
                return RedirectToAction(nameof(Details), new { id = soggettoId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la creazione del soggetto");
                ModelState.AddModelError(string.Empty, "Errore imprevisto durante la creazione del soggetto");
                PrepareCreateEditDropdowns();
                return View(model);
            }
        }

        // ===================================
        // EDIT - MODIFICA
        // ===================================

        /// <summary>
        /// GET: /Soggetti/Edit/5
        /// Form di modifica soggetto esistente
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Amministrazione,Manager")]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var soggetto = await _soggettoService.GetByIdAsync(id);

                if (soggetto == null)
                {
                    TempData["ErrorMessage"] = "Soggetto non trovato";
                    return RedirectToAction(nameof(Index));
                }

                // Mappa DetailsViewModel → EditViewModel
                var editModel = new SoggettoEditViewModel
                {
                    Id = soggetto.Id,
                    CodiceInterno = soggetto.CodiceInterno,
                    TipoSoggetto = soggetto.TipoSoggetto,
                    NaturaGiuridica = soggetto.NaturaGiuridica,
                    IsCliente = soggetto.IsCliente,
                    IsFornitore = soggetto.IsFornitore,
                    Denominazione = soggetto.Denominazione,
                    Nome = soggetto.Nome,
                    Cognome = soggetto.Cognome,
                    CodiceFiscale = soggetto.CodiceFiscale,
                    PartitaIVA = soggetto.PartitaIVA,
                    CodiceSDI = soggetto.CodiceSDI,
                    Referente = soggetto.Referente,
                    Email = soggetto.Email,
                    Telefono = soggetto.Telefono,
                    PEC = soggetto.PEC,
                    TipoVia = soggetto.TipoVia,
                    NomeVia = soggetto.NomeVia,
                    NumeroCivico = soggetto.NumeroCivico,
                    Citta = soggetto.Citta,
                    CAP = soggetto.CAP,
                    Provincia = soggetto.Provincia,
                    Nazione = soggetto.Nazione,
                    CondizioniPagamento = soggetto.CondizioniPagamento,
                    IBAN = soggetto.IBAN,
                    ScontoPartner = soggetto.ScontoPartner,
                    Note = soggetto.Note,
                    CodiceIPA = soggetto.CodiceIPA
                };

                PrepareCreateEditDropdowns();
                return View(editModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento del form di modifica soggetto {SoggettoId}", id);
                TempData["ErrorMessage"] = "Errore durante il caricamento del soggetto";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// POST: /Soggetti/Edit/5
        /// Salva modifiche al soggetto
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Amministrazione,Manager")]
        public async Task<IActionResult> Edit(Guid id, SoggettoEditViewModel model)
        {
            try
            {
                if (id != model.Id)
                {
                    TempData["ErrorMessage"] = "ID non corrispondente";
                    return RedirectToAction(nameof(Index));
                }

                if (!ModelState.IsValid)
                {
                    PrepareCreateEditDropdowns();
                    return View(model);
                }

                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(currentUserId))
                {
                    TempData["ErrorMessage"] = "Utente non autenticato";
                    return RedirectToAction(nameof(Index));
                }

                var (success, errorMessage) = await _soggettoService.UpdateAsync(model, currentUserId);

                if (!success)
                {
                    ModelState.AddModelError(string.Empty, errorMessage ?? "Errore durante l'aggiornamento del soggetto");
                    PrepareCreateEditDropdowns();
                    return View(model);
                }

                _logger.LogInformation("Soggetto aggiornato con successo: {SoggettoId} da utente {UserId}", id, currentUserId);
                TempData["SuccessMessage"] = "Soggetto aggiornato con successo";
                return RedirectToAction(nameof(Details), new { id = model.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'aggiornamento del soggetto {SoggettoId}", id);
                ModelState.AddModelError(string.Empty, "Errore imprevisto durante l'aggiornamento del soggetto");
                PrepareCreateEditDropdowns();
                return View(model);
            }
        }

        // ===================================
        // DELETE - ELIMINAZIONE
        // ===================================

        /// <summary>
        /// GET: /Soggetti/Delete/5
        /// Conferma eliminazione soggetto
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Amministrazione")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var soggetto = await _soggettoService.GetByIdAsync(id);

                if (soggetto == null)
                {
                    TempData["ErrorMessage"] = "Soggetto non trovato";
                    return RedirectToAction(nameof(Index));
                }

                return View(soggetto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il caricamento della conferma eliminazione soggetto {SoggettoId}", id);
                TempData["ErrorMessage"] = "Errore durante il caricamento del soggetto";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// POST: /Soggetti/Delete/5
        /// Elimina il soggetto (soft delete)
        /// </summary>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Amministrazione")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(currentUserId))
                {
                    TempData["ErrorMessage"] = "Utente non autenticato";
                    return RedirectToAction(nameof(Index));
                }

                var (success, errorMessage) = await _soggettoService.DeleteAsync(id, currentUserId);

                if (!success)
                {
                    TempData["ErrorMessage"] = errorMessage ?? "Errore durante l'eliminazione del soggetto";
                    return RedirectToAction(nameof(Delete), new { id });
                }

                _logger.LogInformation("Soggetto eliminato con successo: {SoggettoId} da utente {UserId}", id, currentUserId);
                TempData["SuccessMessage"] = "Soggetto eliminato con successo";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'eliminazione del soggetto {SoggettoId}", id);
                TempData["ErrorMessage"] = "Errore imprevisto durante l'eliminazione del soggetto";
                return RedirectToAction(nameof(Delete), new { id });
            }
        }

        // ===================================
        // METODI PRIVATI - UTILITY
        // ===================================

        /// <summary>
        /// Prepara dropdown per filtri nella Index
        /// </summary>
        private void PrepareFilterDropdowns()
        {
            // Dropdown Tipo Soggetto
            ViewBag.TipiSoggetto = new SelectList(new[]
            {
                new { Value = "", Text = "Tutti i tipi" },
                new { Value = ((int)TipoSoggetto.Azienda).ToString(), Text = "Azienda" },
                new { Value = ((int)TipoSoggetto.PersonaFisica).ToString(), Text = "Persona Fisica" }
            }, "Value", "Text");

            // Dropdown Natura Giuridica
            ViewBag.NatureGiuridiche = new SelectList(new[]
            {
                new { Value = "", Text = "Tutte le nature" },
                new { Value = ((int)NaturaGiuridica.PA).ToString(), Text = "Pubblica Amministrazione" },
                new { Value = ((int)NaturaGiuridica.Privato).ToString(), Text = "Privato" }
            }, "Value", "Text");

            // Dropdown Filtro Cliente
            ViewBag.FiltriCliente = new SelectList(new[]
            {
                new { Value = "", Text = "Tutti" },
                new { Value = "true", Text = "Solo Clienti" },
                new { Value = "false", Text = "Non Clienti" }
            }, "Value", "Text");

            // Dropdown Filtro Fornitore
            ViewBag.FiltriFornitore = new SelectList(new[]
            {
                new { Value = "", Text = "Tutti" },
                new { Value = "true", Text = "Solo Fornitori" },
                new { Value = "false", Text = "Non Fornitori" }
            }, "Value", "Text");

            // Dropdown Ordinamento
            ViewBag.CampiOrdinamento = new SelectList(new[]
            {
                new { Value = "nome", Text = "Nome" },
                new { Value = "tipo", Text = "Tipo" },
                new { Value = "natura", Text = "Natura Giuridica" },
                new { Value = "citta", Text = "Città" },
                new { Value = "email", Text = "Email" },
                new { Value = "ruolo", Text = "Ruolo" }
            }, "Value", "Text");
        }

        /// <summary>
        /// Prepara dropdown per form Create/Edit
        /// </summary>
        private void PrepareCreateEditDropdowns()
        {
            // Dropdown Tipo Soggetto (obbligatorio, no opzione vuota)
            ViewBag.TipiSoggetto = new SelectList(new[]
            {
                new { Value = ((int)TipoSoggetto.Azienda).ToString(), Text = "Azienda" },
                new { Value = ((int)TipoSoggetto.PersonaFisica).ToString(), Text = "Persona Fisica" }
            }, "Value", "Text");

            // Dropdown Natura Giuridica (obbligatorio, no opzione vuota)
            ViewBag.NatureGiuridiche = new SelectList(new[]
            {
                new { Value = ((int)NaturaGiuridica.PA).ToString(), Text = "Pubblica Amministrazione" },
                new { Value = ((int)NaturaGiuridica.Privato).ToString(), Text = "Privato" }
            }, "Value", "Text");

            // Dropdown Tipo Via (opzionale)
            ViewBag.TipiVia = new SelectList(new[]
            {
                "Via", "Viale", "Piazza", "Corso", "Largo", "Vicolo", "Strada", "Contrada"
            });
        }
    }
}