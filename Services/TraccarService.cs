using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Trasformazioni.Models.Entities;
using Trasformazioni.Services.Interfaces;

namespace Trasformazioni.Services
{

    public class TraccarService : ITraccarService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TraccarService> _logger;
        private readonly string _traccarBaseUrl;
        private readonly string _traccarUsername;
        private readonly string _traccarPassword;

        public TraccarService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<TraccarService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            //// Leggi configurazione da appsettings.json
            //_traccarBaseUrl = configuration["Traccar:BaseUrl"] ?? "http://localhost:8082";
            //_traccarUsername = configuration["Traccar:Username"] ?? "admin";
            //_traccarPassword = configuration["Traccar:Password"] ?? "admin";

            //// Configura HttpClient
            //_httpClient.BaseAddress = new Uri(_traccarBaseUrl);
            //_httpClient.DefaultRequestHeaders.Accept.Add(
            //    new MediaTypeWithQualityHeaderValue("application/json"));

            //// Basic Authentication
            //var authToken = Convert.ToBase64String(
            //    Encoding.ASCII.GetBytes($"{_traccarUsername}:{_traccarPassword}"));
            //_httpClient.DefaultRequestHeaders.Authorization =
            //    new AuthenticationHeaderValue("Basic", authToken);
        }

        public async Task<List<TraccarDevice>> GetDevicesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/devices");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var devices = JsonSerializer.Deserialize<List<TraccarDevice>>(json);

                return devices ?? new List<TraccarDevice>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero dispositivi da Traccar");
                throw;
            }
        }

        public async Task<TraccarDevice> GetDeviceByIdAsync(int deviceId)
        {
            var devices = await GetDevicesAsync();
            return devices.FirstOrDefault(d => d.Id == deviceId);
        }

        public async Task<TraccarDevice> GetDeviceByImeiAsync(string imei)
        {
            var devices = await GetDevicesAsync();
            return devices.FirstOrDefault(d => d.UniqueId == imei);
        }

        public async Task<List<TraccarPosition>> GetPositionsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/positions");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var positions = JsonSerializer.Deserialize<List<TraccarPosition>>(json);

                return positions ?? new List<TraccarPosition>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero posizioni da Traccar");
                throw;
            }
        }

        public async Task<TraccarPosition> GetLatestPositionAsync(int deviceId)
        {
            var positions = await GetPositionsAsync();
            return positions.FirstOrDefault(p => p.DeviceId == deviceId);
        }

        public async Task<List<TraccarPosition>> GetPositionHistoryAsync(
            int deviceId,
            DateTime from,
            DateTime to)
        {
            try
            {
                var fromStr = from.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
                var toStr = to.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");

                var url = $"/api/positions?deviceId={deviceId}&from={fromStr}&to={toStr}";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var positions = JsonSerializer.Deserialize<List<TraccarPosition>>(json);

                return positions ?? new List<TraccarPosition>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero storico posizioni da Traccar");
                throw;
            }
        }

        public async Task<List<TraccarTrip>> GetTripsAsync(
            int deviceId,
            DateTime from,
            DateTime to)
        {
            try
            {
                var fromStr = from.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
                var toStr = to.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");

                var url = $"/api/reports/trips?deviceId={deviceId}&from={fromStr}&to={toStr}";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var trips = JsonSerializer.Deserialize<List<TraccarTrip>>(json);

                return trips ?? new List<TraccarTrip>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero viaggi da Traccar");
                throw;
            }
        }

        public async Task<List<TraccarEvent>> GetEventsAsync(
            int deviceId,
            DateTime from,
            DateTime to)
        {
            try
            {
                var fromStr = from.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
                var toStr = to.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");

                var url = $"/api/reports/events?deviceId={deviceId}&from={fromStr}&to={toStr}";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var events = JsonSerializer.Deserialize<List<TraccarEvent>>(json);

                return events ?? new List<TraccarEvent>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero eventi da Traccar");
                throw;
            }
        }
    }
}
