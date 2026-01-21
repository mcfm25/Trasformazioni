using System.Text.Json.Serialization;

namespace Trasformazioni.Models.Entities
{

    public class TraccarDevice
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("uniqueId")]
        public string UniqueId { get; set; } // IMEI

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("lastUpdate")]
        public DateTime? LastUpdate { get; set; }

        [JsonPropertyName("positionId")]
        public int? PositionId { get; set; }

        [JsonPropertyName("phone")]
        public string Phone { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("contact")]
        public string Contact { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; }

        [JsonPropertyName("disabled")]
        public bool Disabled { get; set; }
    }

    public class TraccarPosition
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("deviceId")]
        public int DeviceId { get; set; }

        [JsonPropertyName("protocol")]
        public string Protocol { get; set; }

        [JsonPropertyName("deviceTime")]
        public DateTime DeviceTime { get; set; }

        [JsonPropertyName("fixTime")]
        public DateTime FixTime { get; set; }

        [JsonPropertyName("serverTime")]
        public DateTime ServerTime { get; set; }

        [JsonPropertyName("outdated")]
        public bool Outdated { get; set; }

        [JsonPropertyName("valid")]
        public bool Valid { get; set; }

        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("altitude")]
        public double Altitude { get; set; }

        [JsonPropertyName("speed")]
        public double Speed { get; set; } // nodi

        [JsonPropertyName("course")]
        public double Course { get; set; }

        [JsonPropertyName("address")]
        public string Address { get; set; }

        [JsonPropertyName("accuracy")]
        public double Accuracy { get; set; }

        [JsonPropertyName("network")]
        public object Network { get; set; }

        [JsonPropertyName("attributes")]
        public Dictionary<string, object> Attributes { get; set; }

        // Helper per convertire velocità da nodi a km/h
        public double SpeedKmh => Speed * 1.852;
    }

    public class TraccarTrip
    {
        [JsonPropertyName("deviceId")]
        public int DeviceId { get; set; }

        [JsonPropertyName("deviceName")]
        public string DeviceName { get; set; }

        [JsonPropertyName("startTime")]
        public DateTime StartTime { get; set; }

        [JsonPropertyName("endTime")]
        public DateTime EndTime { get; set; }

        [JsonPropertyName("distance")]
        public double Distance { get; set; } // metri

        [JsonPropertyName("averageSpeed")]
        public double AverageSpeed { get; set; }

        [JsonPropertyName("maxSpeed")]
        public double MaxSpeed { get; set; }

        [JsonPropertyName("duration")]
        public long Duration { get; set; } // millisecondi

        [JsonPropertyName("startLat")]
        public double StartLat { get; set; }

        [JsonPropertyName("startLon")]
        public double StartLon { get; set; }

        [JsonPropertyName("endLat")]
        public double EndLat { get; set; }

        [JsonPropertyName("endLon")]
        public double EndLon { get; set; }

        // Helper per distanza in km
        public double DistanceKm => Distance / 1000.0;
    }

    public class TraccarEvent
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("eventTime")]
        public DateTime EventTime { get; set; }

        [JsonPropertyName("deviceId")]
        public int DeviceId { get; set; }

        [JsonPropertyName("positionId")]
        public int PositionId { get; set; }

        [JsonPropertyName("geofenceId")]
        public int? GeofenceId { get; set; }

        [JsonPropertyName("maintenanceId")]
        public int? MaintenanceId { get; set; }

        [JsonPropertyName("attributes")]
        public Dictionary<string, object> Attributes { get; set; }
    }
}
