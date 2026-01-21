using Microsoft.AspNetCore.Mvc;
using Trasformazioni.Services.Interfaces;

namespace Trasformazioni.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehiclesController : ControllerBase
    {
        private readonly ITraccarService _traccarService;
        private readonly ILogger<VehiclesController> _logger;

        public VehiclesController(
            ITraccarService traccarService,
            ILogger<VehiclesController> logger)
        {
            _traccarService = traccarService;
            _logger = logger;
        }

        // GET: api/vehicles
        [HttpGet]
        public async Task<IActionResult> GetVehicles()
        {
            try
            {
                var devices = await _traccarService.GetDevicesAsync();
                return Ok(devices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero veicoli");
                return StatusCode(500, "Errore nel recupero veicoli");
            }
        }

        // GET: api/vehicles/{id}/position
        [HttpGet("{id}/position")]
        public async Task<IActionResult> GetVehiclePosition(int id)
        {
            try
            {
                var position = await _traccarService.GetLatestPositionAsync(id);

                if (position == null)
                    return NotFound($"Nessuna posizione trovata per veicolo {id}");

                return Ok(position);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero posizione veicolo");
                return StatusCode(500, "Errore nel recupero posizione");
            }
        }

        // GET: api/vehicles/{id}/history?from=2025-10-01&to=2025-10-07
        [HttpGet("{id}/history")]
        public async Task<IActionResult> GetVehicleHistory(
            int id,
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            try
            {
                var positions = await _traccarService.GetPositionHistoryAsync(id, from, to);
                return Ok(positions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero storico veicolo");
                return StatusCode(500, "Errore nel recupero storico");
            }
        }

        // GET: api/vehicles/{id}/trips?from=2025-10-01&to=2025-10-07
        [HttpGet("{id}/trips")]
        public async Task<IActionResult> GetVehicleTrips(
            int id,
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            try
            {
                var trips = await _traccarService.GetTripsAsync(id, from, to);
                return Ok(trips);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero viaggi");
                return StatusCode(500, "Errore nel recupero viaggi");
            }
        }
    }
}
