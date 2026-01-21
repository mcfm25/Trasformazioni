namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per le statistiche di un cliente
    /// </summary>
    public class RegistroContrattiStatisticheClienteViewModel
    {
        public Guid ClienteId { get; set; }
        public string RagioneSociale { get; set; } = string.Empty;

        public int TotaleRegistri { get; set; }
        public int ContrattiAttivi { get; set; }
        public int ContrattiInScadenza { get; set; }

        public decimal TotaleCanoneAnnuo { get; set; }
    }
}
