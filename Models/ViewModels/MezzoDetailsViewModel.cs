using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;

namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per la visualizzazione dettagliata di un mezzo
    /// </summary>
    public class MezzoDetailsViewModel
    {
        public Guid Id { get; set; }
        public string Targa { get; set; } = string.Empty;
        public string TargaFormattata { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Modello { get; set; } = string.Empty;
        public int? Anno { get; set; }
        public TipoMezzo Tipo { get; set; }
        public string TipoDescrizione { get; set; } = string.Empty;
        public StatoMezzo Stato { get; set; }
        public string StatoDescrizione { get; set; } = string.Empty;
        public TipoProprietaMezzo TipoProprieta { get; set; }
        public string TipoProprietaDescrizione { get; set; } = string.Empty;
        public decimal? Chilometraggio { get; set; }
        public DateTime? DataImmatricolazione { get; set; }
        public DateTime? DataAcquisto { get; set; }
        public DateTime? DataInizioNoleggio { get; set; }
        public DateTime? DataFineNoleggio { get; set; }
        public string? SocietaNoleggio { get; set; }
        public DateTime? DataScadenzaAssicurazione { get; set; }
        public DateTime? DataScadenzaRevisione { get; set; }
        public string? Note { get; set; }
        public string? DeviceIMEI { get; set; }
        public string? DevicePhoneNumber { get; set; }

        // Campi audit
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? ModifiedAt { get; set; }
        public string? ModifiedBy { get; set; }

        // Proprietà calcolate
        public bool IsAssicurazioneScaduta { get; set; }
        public bool IsAssicurazioneInScadenza { get; set; }
        public int? GiorniAllaScadenzaAssicurazione { get; set; }

        public bool IsRevisioneScaduta { get; set; }
        public bool IsRevisioneInScadenza { get; set; }
        public int? GiorniAllaScadenzaRevisione { get; set; }

        public bool IsMezzoANoleggio => TipoProprieta == TipoProprietaMezzo.Noleggio;
        public bool IsNoleggioScaduto { get; set; }
        public bool IsNoleggioInScadenza { get; set; }

        public string DescrizioneCompleta => $"{Marca} {Modello} - {Targa}";

        /// <summary>
        /// Assegnazione attualmente attiva (se presente)
        /// </summary>
        public AssegnazioneMezzoDetailsViewModel? AssegnazioneAttiva { get; set; }

        /// <summary>
        /// Ultime 5 assegnazioni dello storico
        /// </summary>
        public List<AssegnazioneMezzoListViewModel> UltimeAssegnazioni { get; set; } = new List<AssegnazioneMezzoListViewModel>();
    }
}