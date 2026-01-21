using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Xml;
using Trasformazioni.Models.Entities;

namespace Trasformazioni.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/traccar/webhook")]
    public class TraccarWebhookController : ControllerBase
    {
        private readonly ILogger<TraccarWebhookController> _logger;

        public TraccarWebhookController(ILogger<TraccarWebhookController> logger)
        {
            _logger = logger;
        }

        [HttpPost("events")]
        public async Task<IActionResult> ReceiveEvent([FromBody] TraccarWebhookEvent webhookEvent)
        {
            try
            {
                _logger.LogInformation(
                    "Evento Traccar ricevuto: {Type} per device {DeviceId}",
                    webhookEvent.Event?.Type,
                    webhookEvent.Device?.Id);

                // Gestisci diversi tipi di eventi
                switch (webhookEvent.Event?.Type)
                {
                    case "deviceOnline":
                        await HandleDeviceOnline(webhookEvent);
                        break;

                    case "deviceOffline":
                        await HandleDeviceOffline(webhookEvent);
                        break;

                    case "deviceMoving":
                        await HandleDeviceMoving(webhookEvent);
                        break;

                    case "deviceStopped":
                        await HandleDeviceStopped(webhookEvent);
                        break;

                    case "geofenceEnter":
                        await HandleGeofenceEnter(webhookEvent);
                        break;

                    case "geofenceExit":
                        await HandleGeofenceExit(webhookEvent);
                        break;

                    case "alarm":
                        await HandleAlarm(webhookEvent);
                        break;

                    case "ignitionOn":
                        await HandleIgnitionOn(webhookEvent);
                        break;

                    case "ignitionOff":
                        await HandleIgnitionOff(webhookEvent);
                        break;

                    case "maintenance":
                        await HandleMaintenance(webhookEvent);
                        break;

                    default:
                        _logger.LogWarning(
                            "Tipo evento non gestito: {Type}",
                            webhookEvent.Event?.Type);
                        break;
                }

                return Ok(new { message = "Evento processato con successo" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nella gestione evento Traccar");
                return StatusCode(500, "Errore nella gestione evento");
            }
        }

        [HttpPost("positions")]
        public async Task<IActionResult> ReceivePosition([FromBody] TraccarWebhookPosition payload)
        {
            try
            {
                //Leggi il body RAW
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();

                _logger.LogWarning("[HTTPGET]========================================");
                _logger.LogWarning("WEBHOOK RICEVUTO DA TRACCAR");
                _logger.LogWarning("Content-Type: {ContentType}", Request.ContentType);
                _logger.LogWarning("Body RAW:");
                _logger.LogWarning(body);
                _logger.LogWarning("========================================");

                // Log anche headers
                foreach (var header in Request.Headers)
                {
                    _logger.LogInformation("Header: {Key} = {Value}", header.Key, header.Value);
                }

                //return Ok(new { message = "Ricevuto", timestamp = DateTime.Now });
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nella gestione webhook");
                return StatusCode(500);
            }
        }

        [HttpGet]
        public IActionResult Status()
        {
            try
            {
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nella gestione webhook");
                return StatusCode(500);
            }
        }

        private async Task HandleDeviceOnline(TraccarWebhookEvent evt)
        {
            _logger.LogInformation(
                "Device {DeviceId} ({Name}) è tornato online",
                evt.Device?.Id,
                evt.Device?.Name);

            // TODO: Aggiorna stato nel tuo DB
            // TODO: Invia notifica push/email se necessario
        }

        private async Task HandleDeviceOffline(TraccarWebhookEvent evt)
        {
            _logger.LogWarning(
                "Device {DeviceId} ({Name}) è offline",
                evt.Device?.Id,
                evt.Device?.Name);

            // TODO: Crea alert nel tuo sistema
            // TODO: Notifica amministratore
        }

        private async Task HandleDeviceMoving(TraccarWebhookEvent evt)
        {
            _logger.LogInformation(
                "Device {DeviceId} ha iniziato a muoversi",
                evt.Device?.Id);

            // TODO: Registra inizio viaggio
            // TODO: Aggiorna dashboard real-time
        }

        private async Task HandleDeviceStopped(TraccarWebhookEvent evt)
        {
            _logger.LogInformation(
                "Device {DeviceId} si è fermato a lat:{Lat}, lon:{Lon}",
                evt.Device?.Id,
                evt.Position?.Latitude,
                evt.Position?.Longitude);

            // TODO: Registra fine viaggio
            // TODO: Calcola statistiche viaggio
        }

        private async Task HandleGeofenceEnter(TraccarWebhookEvent evt)
        {
            _logger.LogInformation(
                "Device {DeviceId} è entrato in geofence {GeofenceId}",
                evt.Device?.Id,
                evt.Geofence?.Id);

            // TODO: Registra ingresso in zona
            // TODO: Genera notifica se necessario
        }

        private async Task HandleGeofenceExit(TraccarWebhookEvent evt)
        {
            _logger.LogInformation(
                "Device {DeviceId} è uscito da geofence {GeofenceId}",
                evt.Device?.Id,
                evt.Geofence?.Id);

            // TODO: Registra uscita da zona
            // TODO: Verifica permessi di uscita
        }

        private async Task HandleAlarm(TraccarWebhookEvent evt)
        {
            var alarmType = evt.Event?.Attributes?
                .GetValueOrDefault("alarm")?.ToString();

            _logger.LogWarning(
                "ALLARME su device {DeviceId}: {AlarmType}",
                evt.Device?.Id,
                alarmType);

            // TODO: Crea alert urgente
            // TODO: Invia notifica immediata
            // TODO: Se tamper/sos, avvia procedura emergenza
        }

        private async Task HandleIgnitionOn(TraccarWebhookEvent evt)
        {
            _logger.LogInformation(
                "Accensione veicolo {DeviceId}",
                evt.Device?.Id);

            // TODO: Registra accensione
            // TODO: Verifica autorizzazione orario
        }

        private async Task HandleIgnitionOff(TraccarWebhookEvent evt)
        {
            _logger.LogInformation(
                "Spegnimento veicolo {DeviceId}",
                evt.Device?.Id);

            // TODO: Registra spegnimento
            // TODO: Calcola tempo di utilizzo
        }

        private async Task HandleMaintenance(TraccarWebhookEvent evt)
        {
            _logger.LogInformation(
                "Manutenzione necessaria per device {DeviceId}",
                evt.Device?.Id);

            // TODO: Crea task manutenzione
            // TODO: Notifica responsabile manutenzione
        }
    }
}
