namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per le statistiche del Registro Contratti
    /// </summary>
    public class RegistroContrattiStatisticheViewModel
    {
        public int TotaleRegistri { get; set; }
        public int TotalePreventivi { get; set; }
        public int TotaleContratti { get; set; }

        public int ContrattiAttivi { get; set; }
        public int ContrattiInScadenza { get; set; }
        public int ContrattiScaduti { get; set; }

        public decimal TotaleCanoneAnnuoAttivi { get; set; }
        public decimal TotaleCanoneAnnuoInScadenza { get; set; }

        public int PreventiviInBozza { get; set; }
        public int PreventiviInviati { get; set; }
    }
}
