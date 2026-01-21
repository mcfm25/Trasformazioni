using System.Text.Json.Serialization;

namespace Trasformazioni.Models.Entities
{
    public class TraccarWebhookPosition
    {
        [JsonPropertyName("position")]
        public PositionData Position { get; set; }

        [JsonPropertyName("device")]
        public DeviceData Device { get; set; }
    }

    public class TraccarWebhookEvent
    {
        [JsonPropertyName("event")]
        public EventData? Event { get; set; }

        [JsonPropertyName("position")]
        public PositionData? Position { get; set; }

        [JsonPropertyName("device")]
        public required DeviceData Device { get; set; }

        [JsonPropertyName("geofence")]
        public GeofenceData? Geofence { get; set; }

        [JsonPropertyName("maintenance")]
        public MaintenanceData? Maintenance { get; set; }
    }

    public class EventData
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("eventTime")]
        public DateTime EventTime { get; set; }

        [JsonPropertyName("deviceId")]
        public int DeviceId { get; set; }

        [JsonPropertyName("positionId")]
        public int? PositionId { get; set; }

        [JsonPropertyName("geofenceId")]
        public int? GeofenceId { get; set; }

        [JsonPropertyName("maintenanceId")]
        public int? MaintenanceId { get; set; }

        [JsonPropertyName("attributes")]
        public Dictionary<string, object>? Attributes { get; set; }
    }

    public class PositionAttributes
    {
        [JsonPropertyName("course")]
        public double? Course { get; set; }

        [JsonPropertyName("distance")]
        public double? Distance { get; set; }

        [JsonPropertyName("totalDistance")]
        public double? TotalDistance { get; set; }

        [JsonPropertyName("motion")]
        public bool? Motion { get; set; }
    }

    public class PositionData
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("attributes")]
        public PositionAttributes? Attributes { get; set; }

        //[JsonPropertyName("attributes")]
        //public Dictionary<string, object>? Attributes { get; set; }

        [JsonPropertyName("deviceId")]
        public int DeviceId { get; set; }

        [JsonPropertyName("protocol")]
        public string Protocol { get; set; }

        [JsonPropertyName("serverTime")]
        public DateTime ServerTime { get; set; }

        [JsonPropertyName("deviceTime")]
        public DateTime DeviceTime { get; set; }

        [JsonPropertyName("fixTime")]
        public DateTime FixTime { get; set; }

        [JsonPropertyName("valid")]
        public bool Valid { get; set; }

        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("altitude")]
        public double Altitude { get; set; }

        [JsonPropertyName("speed")]
        public double Speed { get; set; }

        [JsonPropertyName("course")]
        public double Course { get; set; }

        [JsonPropertyName("address")]
        public string? Address { get; set; }

        [JsonPropertyName("accuracy")]
        public double Accuracy { get; set; }

        [JsonPropertyName("network")]
        public object? Network { get; set; }

        [JsonPropertyName("geofenceIds")]
        public object? GeofenceIds { get; set; }
    }

    public class DeviceData
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("attributes")]
        public Dictionary<string, object>? Attributes { get; set; }

        [JsonPropertyName("groupId")]
        public int GroupId { get; set; }

        [JsonPropertyName("calendarId")]
        public int CalendarId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("uniqueId")]
        public string UniqueId { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("lastUpdate")]
        public DateTime LastUpdate { get; set; }

        [JsonPropertyName("positionId")]
        public long PositionId { get; set; }

        [JsonPropertyName("phone")]
        public string? Phone { get; set; }

        [JsonPropertyName("model")]
        public string? Model { get; set; }

        [JsonPropertyName("contact")]
        public string? Contact { get; set; }

        [JsonPropertyName("category")]
        public string? Category { get; set; }

        [JsonPropertyName("disabled")]
        public bool Disabled { get; set; }

        [JsonPropertyName("expirationTime")]
        public DateTime? ExpirationTime { get; set; }
    }

    public class GeofenceData
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("area")]
        public string Area { get; set; }
    }

    public class MaintenanceData
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("start")]
        public double Start { get; set; }

        [JsonPropertyName("period")]
        public double Period { get; set; }
    }
}
