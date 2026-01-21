using Trasformazioni.Models.Enums;

namespace Trasformazioni.ViewModels
{
    /// <summary>
    /// ViewModel per visualizzare un mezzo sulla mappa con la sua posizione GPS
    /// </summary>
    public class MezzoMapViewModel
    {
        // === INFORMAZIONI MEZZO ===
        public Guid MezzoId { get; set; }
        public string Targa { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Modello { get; set; } = string.Empty;
        public string DescrizioneCompleta { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string Stato { get; set; } = string.Empty;
        public string? DeviceIMEI { get; set; }

        // === INFORMAZIONI ASSEGNAZIONE ===
        public bool IsAssegnato { get; set; }
        public string? AssegnatoA { get; set; } // Nome completo utente assegnatario

        // === INFORMAZIONI POSIZIONE GPS ===
        public MezzoPositionViewModel? Position { get; set; }

        // === STATO GPS ===
        /// <summary>
        /// Indica se il mezzo ha un dispositivo GPS configurato
        /// </summary>
        public bool HasGpsDevice => !string.IsNullOrWhiteSpace(DeviceIMEI);

        /// <summary>
        /// Indica se è disponibile la posizione GPS corrente
        /// </summary>
        public bool HasCurrentPosition => Position != null;
    }

    /// <summary>
    /// Informazioni sulla posizione GPS di un mezzo
    /// </summary>
    public class MezzoPositionViewModel
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double SpeedKmh { get; set; } // Velocità in km/h (convertita da nodi)
        public double Course { get; set; } // Direzione in gradi (0-360)
        public DateTime Timestamp { get; set; } // FixTime dalla posizione GPS
        public string? Address { get; set; }
        public bool IsValid { get; set; }
        public bool IsOutdated { get; set; }

        // === STATO MOVIMENTO ===
        /// <summary>
        /// Indica se il veicolo è in movimento (speed > 0 km/h)
        /// </summary>
        public bool IsMoving => SpeedKmh > 0;

        /// <summary>
        /// Restituisce lo stato del veicolo per la UI
        /// moving = in movimento, stopped = fermo, offline = dato obsoleto
        /// </summary>
        public string Status
        {
            get
            {
                if (IsOutdated) return "offline";
                if (IsMoving) return "moving";
                return "stopped";
            }
        }
    }

    /// <summary>
    /// Risposta completa per l'endpoint della mappa
    /// Include statistiche e lista completa dei mezzi
    /// </summary>
    public class MapResponseViewModel
    {
        /// <summary>
        /// Timestamp UTC della generazione della risposta
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.Now;

        /// <summary>
        /// Numero totale di mezzi nel sistema
        /// </summary>
        public int TotalVehicles { get; set; }

        /// <summary>
        /// Numero di mezzi con dispositivo GPS configurato (IMEI presente)
        /// </summary>
        public int VehiclesWithGps { get; set; }

        /// <summary>
        /// Numero di mezzi con posizione GPS disponibile
        /// </summary>
        public int VehiclesWithPosition { get; set; }

        /// <summary>
        /// Lista completa dei mezzi con le loro posizioni
        /// </summary>
        public List<MezzoMapViewModel> Vehicles { get; set; } = new();
    }
}