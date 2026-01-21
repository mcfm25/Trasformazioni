using Trasformazioni.Models.Enums;

namespace Trasformazioni.Models.Entities
{
    /// <summary>
    /// Entità che rappresenta un mezzo aziendale (auto, furgone, camion, etc.)
    /// </summary>
    public class Mezzo : BaseEntity
    {
        /// <summary>
        /// Identificatore univoco del mezzo
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Targa del veicolo (univoca)
        /// </summary>
        public string Targa { get; set; } = string.Empty;

        /// <summary>
        /// Marca del veicolo (es. Fiat, Ford, Mercedes)
        /// </summary>
        public string Marca { get; set; } = string.Empty;

        /// <summary>
        /// Modello del veicolo (es. Punto, Transit, Sprinter)
        /// </summary>
        public string Modello { get; set; } = string.Empty;

        /// <summary>
        /// Anno di immatricolazione
        /// </summary>
        public int? Anno { get; set; }

        /// <summary>
        /// Tipologia di mezzo (Auto, Furgone, Camion, Moto, Altro)
        /// </summary>
        public TipoMezzo Tipo { get; set; }

        /// <summary>
        /// Stato operativo del mezzo
        /// </summary>
        public StatoMezzo Stato { get; set; }

        /// <summary>
        /// Tipo di proprietà (Proprietà aziendale o Noleggio)
        /// </summary>
        public TipoProprietaMezzo TipoProprieta { get; set; }

        /// <summary>
        /// Chilometraggio attuale del veicolo
        /// </summary>
        public decimal? Chilometraggio { get; set; }

        /// <summary>
        /// Data di immatricolazione del veicolo
        /// </summary>
        public DateTime? DataImmatricolazione { get; set; }

        /// <summary>
        /// Data di acquisto (solo per mezzi di proprietà)
        /// </summary>
        public DateTime? DataAcquisto { get; set; }

        /// <summary>
        /// Data inizio noleggio (solo per mezzi a noleggio)
        /// </summary>
        public DateTime? DataInizioNoleggio { get; set; }

        /// <summary>
        /// Data fine noleggio (solo per mezzi a noleggio)
        /// </summary>
        public DateTime? DataFineNoleggio { get; set; }

        /// <summary>
        /// Nome della società di noleggio (solo per mezzi a noleggio)
        /// </summary>
        public string? SocietaNoleggio { get; set; }

        /// <summary>
        /// Data di scadenza dell'assicurazione
        /// </summary>
        public DateTime? DataScadenzaAssicurazione { get; set; }

        /// <summary>
        /// Data di scadenza della revisione
        /// </summary>
        public DateTime? DataScadenzaRevisione { get; set; }


        /// <summary>
        /// IOT Device Identifier (univoca)
        /// </summary>
        public string? DeviceIMEI { get; set; } = string.Empty;

        /// <summary>
        /// IOT Device Phone Number (univoca)
        /// </summary>
        public string? DevicePhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// Note aggiuntive sul mezzo
        /// </summary>
        public string? Note { get; set; }

        // Navigation Properties

        /// <summary>
        /// Collezione di tutte le assegnazioni del mezzo (storico completo)
        /// </summary>
        public virtual ICollection<AssegnazioneMezzo> Assegnazioni { get; set; } = new List<AssegnazioneMezzo>();


        /// <summary>
        /// Descrizione completa del mezzo (Marca Modello - Targa)
        /// </summary>
        public string DescrizioneCompleta => $"{Marca} {Modello} - {Targa}";

        /// <summary>
        /// Assegnazione attualmente attiva o Prenotazione successiva
        /// </summary>
        public AssegnazioneMezzo? AssegnazioneAttiva =>
            Assegnazioni?
                .Where(a => !a.IsDeleted)
                //.Where(a => a.DataInizio <= DateTime.Now)  // commentata per far vedere assegnazione già iniziata o la prima successiva prenotazione
                .Where(a => a.DataFine == null || a.DataFine > DateTime.Now)  // Non ancora finita
                .OrderBy(a => a.DataInizio)  // Prima assegnazione iniziata
                .FirstOrDefault();

    }
}