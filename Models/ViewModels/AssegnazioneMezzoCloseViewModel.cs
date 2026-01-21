using System.ComponentModel.DataAnnotations;

namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per la chiusura (riconsegna) di un'assegnazione mezzo
    /// </summary>
    public class AssegnazioneMezzoCloseViewModel
    {
        [Required]
        public Guid Id { get; set; }

        // Info display (readonly, per mostrare nel form)
        public Guid MezzoId { get; set; }
        public string MezzoTarga { get; set; } = string.Empty;
        public string MezzoDescrizioneCompleta { get; set; } = string.Empty;
        public string UtenteNomeCompleto { get; set; } = string.Empty;
        public DateTime DataInizio { get; set; }
        public decimal? ChilometraggioInizio { get; set; }

        [Required(ErrorMessage = "La data e ora di fine è obbligatoria")]
        [Display(Name = "Data e Ora Riconsegna")]
        [DataType(DataType.DateTime)]  // ← Cambiato da Date a DateTime
        public DateTime DataFine { get; set; } = DateTime.Now;  // ← Cambiato da Today a Now

        [Display(Name = "Chilometraggio Finale (km)")]
        [Range(0, 9999999.99, ErrorMessage = "Chilometraggio non valido")]
        public decimal? ChilometraggioFine { get; set; }

        [Display(Name = "Note Riconsegna")]
        [StringLength(1000, ErrorMessage = "Le note non possono superare i 1000 caratteri")]
        [DataType(DataType.MultilineText)]
        public string? NoteRiconsegna { get; set; }

        /// <summary>
        /// Durata dell'assegnazione in giorni
        /// </summary>
        public int DurataGiorni => (DataFine.Date - DataInizio.Date).Days;

        /// <summary>
        /// Chilometri percorsi (se disponibili entrambi i valori)
        /// </summary>
        public decimal? ChilometriPercorsi
        {
            get
            {
                if (ChilometraggioInizio.HasValue && ChilometraggioFine.HasValue)
                {
                    return ChilometraggioFine.Value - ChilometraggioInizio.Value;
                }
                return null;
            }
        }
    }
}