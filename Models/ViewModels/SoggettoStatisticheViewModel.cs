namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per le statistiche sui soggetti
    /// </summary>
    public class SoggettoStatisticheViewModel
    {
        /// <summary>
        /// Numero totale di soggetti
        /// </summary>
        public int TotaleSoggetti { get; set; }

        /// <summary>
        /// Numero totale di clienti
        /// </summary>
        public int TotaleClienti { get; set; }

        /// <summary>
        /// Numero totale di fornitori
        /// </summary>
        public int TotaleFornitori { get; set; }

        /// <summary>
        /// Numero di soggetti che sono sia clienti che fornitori
        /// </summary>
        public int ClientiEFornitori { get; set; }

        /// <summary>
        /// Percentuale di clienti sul totale
        /// </summary>
        public decimal PercentualeClienti => TotaleSoggetti > 0
            ? Math.Round((decimal)TotaleClienti / TotaleSoggetti * 100, 2)
            : 0;

        /// <summary>
        /// Percentuale di fornitori sul totale
        /// </summary>
        public decimal PercentualeFornitori => TotaleSoggetti > 0
            ? Math.Round((decimal)TotaleFornitori / TotaleSoggetti * 100, 2)
            : 0;

        /// <summary>
        /// Percentuale di soggetti che sono sia clienti che fornitori
        /// </summary>
        public decimal PercentualeClientiEFornitori => TotaleSoggetti > 0
            ? Math.Round((decimal)ClientiEFornitori / TotaleSoggetti * 100, 2)
            : 0;
    }
}