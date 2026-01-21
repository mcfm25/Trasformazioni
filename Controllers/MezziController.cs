using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Trasformazioni.Authorization;
using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;
using Trasformazioni.Models.ViewModels;
using Trasformazioni.Services;
using Trasformazioni.Services.Interfaces;
using Trasformazioni.ViewModels;

namespace Trasformazioni.Controllers
{
    /// <summary>
    /// Controller per la gestione dei mezzi aziendali
    /// </summary>
    [Authorize]
    public class MezziController : Controller
    {
        private readonly IMezzoService _mezzoService;
        private readonly ITraccarService _traccarService;
        //private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<MezziController> _logger;

        public MezziController(
            IMezzoService mezzoService,
            ITraccarService traccarService,
            //UserManager<ApplicationUser> userManager,
            ILogger<MezziController> logger)
        {
            _mezzoService = mezzoService;
            _traccarService = traccarService;
            //_userManager = userManager;
            _logger = logger;
        }

        #region READ - Visualizzazione

        /// <summary>
        /// GET: /Mezzi
        /// Lista di tutti i mezzi
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index(StatoMezzo? stato = null, TipoProprietaMezzo? tipoProprieta = null, string? search = null)
        {
            IEnumerable<MezzoListViewModel> mezzi;

            if (!string.IsNullOrWhiteSpace(search))
            {
                mezzi = await _mezzoService.SearchByTargaAsync(search);
                ViewBag.Search = search;
            }
            else if (stato.HasValue)
            {
                mezzi = await _mezzoService.GetByStatoAsync(stato.Value);
                ViewBag.StatoFiltro = stato.Value;
            }
            else if (tipoProprieta.HasValue)
            {
                mezzi = await _mezzoService.GetByTipoProprietaAsync(tipoProprieta.Value);
                ViewBag.TipoProprietaFiltro = tipoProprieta.Value;
            }
            else
            {
                mezzi = await _mezzoService.GetAllAsync();
            }

            return View(mezzi);
        }

        /// <summary>
        /// GET: /Mezzi/Details/5
        /// Dettaglio di un mezzo specifico
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var mezzo = await _mezzoService.GetByIdAsync(id);

            if (mezzo == null)
            {
                _logger.LogWarning("Tentativo di visualizzare mezzo non esistente: {Id}", id);
                return NotFound();
            }

            return View(mezzo);
        }

        #endregion

        #region CREATE - Creazione

        /// <summary>
        /// GET: /Mezzi/Create
        /// Form per creare un nuovo mezzo
        /// </summary>
        [HttpGet]
        [Authorize(Roles = RoleNames.Amministrazione)]
        public IActionResult Create()
        {
            var model = new MezzoCreateViewModel
            {
                Stato = StatoMezzo.Disponibile,
                TipoProprieta = TipoProprietaMezzo.Proprieta
            };

            return View(model);
        }

        /// <summary>
        /// POST: /Mezzi/Create
        /// Salvataggio nuovo mezzo
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Amministrazione)]
        public async Task<IActionResult> Create(MezzoCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (success, errorMessage, mezzoId) = await _mezzoService.CreateAsync(model);

            if (!success)
            {
                ModelState.AddModelError(string.Empty, errorMessage ?? "Errore durante la creazione del mezzo");
                return View(model);
            }

            TempData["SuccessMessage"] = $"Mezzo {model.Targa} creato con successo";
            return RedirectToAction(nameof(Details), new { id = mezzoId });
        }

        #endregion

        #region UPDATE - Modifica

        /// <summary>
        /// GET: /Mezzi/Edit/5
        /// Form per modificare un mezzo esistente
        /// </summary>
        [HttpGet]
        [Authorize(Roles = RoleNames.Amministrazione)]
        public async Task<IActionResult> Edit(Guid id)
        {
            var model = await _mezzoService.GetEditViewModelAsync(id);

            if (model == null)
            {
                _logger.LogWarning("Tentativo di modificare mezzo non esistente: {Id}", id);
                return NotFound();
            }

            return View(model);
        }

        /// <summary>
        /// POST: /Mezzi/Edit/5
        /// Salvataggio modifiche mezzo
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Amministrazione)]
        public async Task<IActionResult> Edit(Guid id, MezzoEditViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (success, errorMessage) = await _mezzoService.UpdateAsync(id, model);

            if (!success)
            {
                ModelState.AddModelError(string.Empty, errorMessage ?? "Errore durante l'aggiornamento del mezzo");
                return View(model);
            }

            TempData["SuccessMessage"] = $"Mezzo {model.Targa} aggiornato con successo";
            return RedirectToAction(nameof(Details), new { id });
        }

        #endregion

        #region DELETE - Eliminazione

        /// <summary>
        /// GET: /Mezzi/Delete/5
        /// Pagina di conferma eliminazione
        /// </summary>
        [HttpGet]
        [Authorize(Roles = RoleNames.Amministrazione)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var mezzo = await _mezzoService.GetByIdAsync(id);

            if (mezzo == null)
            {
                _logger.LogWarning("Tentativo di eliminare mezzo non esistente: {Id}", id);
                return NotFound();
            }

            return View(mezzo);
        }

        /// <summary>
        /// POST: /Mezzi/Delete/5
        /// Conferma eliminazione mezzo
        /// </summary>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Amministrazione)]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var (success, errorMessage) = await _mezzoService.DeleteAsync(id);

            if (!success)
            {
                TempData["ErrorMessage"] = errorMessage ?? "Errore durante l'eliminazione del mezzo";
                return RedirectToAction(nameof(Delete), new { id });
            }

            TempData["SuccessMessage"] = "Mezzo eliminato con successo";
            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region AJAX Actions

        /// <summary>
        /// POST: /Mezzi/CambiaStato
        /// Cambia lo stato di un mezzo (AJAX)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = RoleNames.Amministrazione)]
        public async Task<IActionResult> CambiaStato(Guid id, StatoMezzo nuovoStato)
        {
            var success = await _mezzoService.CambiaStatoAsync(id, nuovoStato);

            if (!success)
            {
                return Json(new { success = false, message = "Errore durante il cambio stato" });
            }

            return Json(new { success = true, message = "Stato aggiornato con successo" });
        }

        /// <summary>
        /// POST: /Mezzi/AggiornaChilometraggio
        /// Aggiorna il chilometraggio di un mezzo (AJAX)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = RoleNames.Amministrazione)]
        public async Task<IActionResult> AggiornaChilometraggio(Guid id, decimal chilometraggio)
        {
            var success = await _mezzoService.AggiornaChilometraggioAsync(id, chilometraggio);

            if (!success)
            {
                return Json(new { success = false, message = "Errore durante l'aggiornamento del chilometraggio" });
            }

            return Json(new { success = true, message = "Chilometraggio aggiornato con successo" });
        }

        /// <summary>
        /// GET: /Mezzi/CheckTarga
        /// Verifica se una targa è già presente (AJAX - per validazione lato client)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> CheckTarga(string targa, Guid? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(targa))
            {
                return Json(new { isUnique = false, message = "Targa non valida" });
            }

            var isValid = _mezzoService.IsTargaValida(targa);
            if (!isValid)
            {
                return Json(new { isUnique = false, message = "Formato targa non valido" });
            }

            var isUnique = await _mezzoService.IsTargaUniqueAsync(targa, excludeId);

            return Json(new { isUnique, message = isUnique ? "Targa disponibile" : "Targa già presente" });
        }


        /// <summary>
        /// GET: /Mezzi/CheckDeviceIMEI
        /// Verifica se un Device IMEI è già presente (AJAX - per validazione lato client)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> CheckDeviceIMEI(string deviceIMEI, Guid? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(deviceIMEI))
            {
                return Json(new { isUnique = false, message = "Device IMEI non valida" });
            }

            var isUnique = await _mezzoService.IsDeviceIMEIUniqueAsync(deviceIMEI, excludeId);

            return Json(new { isUnique, message = isUnique ? "Device IMEI disponibile" : "Device IMEI già presente" });
        }

        #endregion

        #region MAPPA GPS

        /// <summary>
        /// GET: /Mezzi/Map
        /// Visualizza la mappa con tutti i mezzi e le loro posizioni GPS
        /// </summary>
        [HttpGet]
        public IActionResult Map()
        {
            return View();
        }

        /// <summary>
        /// GET: /Mezzi/Map/Positions
        /// API Endpoint per recuperare le posizioni GPS di tutti i mezzi (formato JSON per la mappa)
        /// </summary>
        [HttpGet("Mezzi/Map/Positions")]
        public async Task<IActionResult> GetMapPositions()
        {
            try
            {
                // 1. Recupera tutti i mezzi con assegnazioni attive
                var mezzi = await _mezzoService.GetAllEntitiesAsync();
                var mezziList = mezzi.ToList();

                // 2. Recupera tutti i dispositivi Traccar
                var traccarDevices = await _traccarService.GetDevicesAsync();

                // 3. Crea lookup map IMEI → TraccarDevice per performance
                var devicesByImei = traccarDevices
                    .Where(d => !string.IsNullOrWhiteSpace(d.UniqueId))
                    .ToDictionary(d => d.UniqueId, d => d);

                // 4. Recupera tutte le posizioni GPS correnti
                var allPositions = await _traccarService.GetPositionsAsync();

                // 5. Crea lookup map DeviceId → TraccarPosition
                var positionsByDeviceId = allPositions
                    .GroupBy(p => p.DeviceId)
                    .ToDictionary(g => g.Key, g => g.First()); // Prendi la prima (ultima posizione)

                // 6. Mappa i dati nei ViewModel
                var vehicleViewModels = new List<MezzoMapViewModel>();

                foreach (var mezzo in mezziList)
                {
                    //var assegnazioneAttiva = mezzo.Assegnazioni?
                    //    .Where(a => !a.IsDeleted)
                    //    .Where(a => a.DataInizio <= DateTime.Now)
                    //    .Where(a => a.DataFine == null || a.DataFine > DateTime.Now)
                    //    .OrderBy(a => a.DataInizio)
                    //    .FirstOrDefault();

                    var viewModel = new MezzoMapViewModel
                    {
                        MezzoId = mezzo.Id,
                        Targa = mezzo.Targa,
                        Marca = mezzo.Marca,
                        Modello = mezzo.Modello,
                        DescrizioneCompleta = mezzo.DescrizioneCompleta,
                        Tipo = mezzo.Tipo.ToString(),
                        Stato = mezzo.Stato.ToString(),
                        DeviceIMEI = mezzo.DeviceIMEI,
                        //IsAssegnato = assegnazioneAttiva != null,
                        //AssegnatoA = assegnazioneAttiva != null
                        //    ? $"{assegnazioneAttiva.Utente?.Nome} {assegnazioneAttiva.Utente?.Cognome}".Trim()
                        //    : null
                        IsAssegnato = mezzo.AssegnazioneAttiva != null,
                        AssegnatoA = mezzo.AssegnazioneAttiva != null
                                    ? $"{mezzo.AssegnazioneAttiva.Utente?.Nome} {mezzo.AssegnazioneAttiva.Utente?.Cognome}".Trim()
                                    : null
                    };

                    // Cerca posizione GPS se il mezzo ha IMEI configurato
                    if (!string.IsNullOrWhiteSpace(mezzo.DeviceIMEI))
                    {
                        if (devicesByImei.TryGetValue(mezzo.DeviceIMEI, out var traccarDevice))
                        {
                            if (positionsByDeviceId.TryGetValue(traccarDevice.Id, out var position))
                            {
                                viewModel.Position = new MezzoPositionViewModel
                                {
                                    Latitude = position.Latitude,
                                    Longitude = position.Longitude,
                                    SpeedKmh = position.SpeedKmh, // Già convertita da nodi a km/h
                                    Course = position.Course,
                                    Timestamp = position.FixTime,
                                    Address = position.Address,
                                    IsValid = position.Valid,
                                    IsOutdated = position.Outdated
                                };
                            }
                        }
                    }

                    vehicleViewModels.Add(viewModel);
                }

                // 7. Prepara risposta con statistiche
                var response = new MapResponseViewModel
                {
                    Timestamp = DateTime.Now,
                    TotalVehicles = vehicleViewModels.Count,
                    VehiclesWithGps = vehicleViewModels.Count(v => v.HasGpsDevice),
                    VehiclesWithPosition = vehicleViewModels.Count(v => v.HasCurrentPosition),
                    Vehicles = vehicleViewModels
                };

                return Json(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero delle posizioni GPS dei mezzi");
                return StatusCode(500, new { error = "Errore nel recupero delle posizioni GPS" });
            }
        }

        #endregion
    }
}