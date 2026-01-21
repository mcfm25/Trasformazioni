namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per filtrare e paginare le richieste di integrazione
    /// Utilizzato nella dashboard e nella pagina Gestione Integrazioni
    /// </summary>
    public class RichiestaIntegrazioneFilterViewModel
    {
        // ===================================
        // PARAMETRI DI RICERCA
        // ===================================

        /// <summary>
        /// Termine di ricerca libera (testo richiesta, testo risposta)
        /// </summary>
        public string? SearchTerm { get; set; }

        // ===================================
        // FILTRI SPECIFICI
        // ===================================

        /// <summary>
        /// Filtro per gara specifica
        /// </summary>
        public Guid? GaraId { get; set; }

        /// <summary>
        /// Filtro per lotto specifico
        /// </summary>
        public Guid? LottoId { get; set; }

        /// <summary>
        /// Filtro per utente che ha risposto
        /// </summary>
        public string? RispostaDaUserId { get; set; }

        /// <summary>
        /// Filtro per numero progressivo (utile per lotti con molte richieste)
        /// </summary>
        public int? NumeroProgressivo { get; set; }

        /// <summary>
        /// Filtro per richieste chiuse (null = tutte, true = solo chiuse, false = solo aperte)
        /// </summary>
        public bool? IsChiusa { get; set; }

        /// <summary>
        /// Mostra solo richieste non ancora risposte (DataRispostaAzienda = null)
        /// PRIORITÀ MASSIMA per dashboard
        /// </summary>
        public bool SoloNonRisposte { get; set; } = false;

        /// <summary>
        /// Mostra solo richieste risposte ma non ancora chiuse
        /// </summary>
        public bool SoloRisposteNonChiuse { get; set; } = false;

        /// <summary>
        /// Mostra solo richieste scadute (più di X giorni senza risposta)
        /// </summary>
        public bool SoloScadute { get; set; } = false;

        /// <summary>
        /// Numero di giorni per considerare una richiesta "scaduta"
        /// Usato solo se SoloScadute = true
        /// </summary>
        public int GiorniScadenza { get; set; } = 7;

        /// <summary>
        /// Filtro data richiesta ente da
        /// </summary>
        public DateTime? DataRichiestaDa { get; set; }

        /// <summary>
        /// Filtro data richiesta ente a
        /// </summary>
        public DateTime? DataRichiestaA { get; set; }

        /// <summary>
        /// Filtro data risposta azienda da
        /// </summary>
        public DateTime? DataRispostaDa { get; set; }

        /// <summary>
        /// Filtro data risposta azienda a
        /// </summary>
        public DateTime? DataRispostaA { get; set; }

        // ===================================
        // PARAMETRI DI PAGINAZIONE
        // ===================================

        /// <summary>
        /// Numero di pagina corrente (1-based)
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Numero di elementi per pagina
        /// </summary>
        public int PageSize { get; set; } = 20;

        // ===================================
        // PARAMETRI DI ORDINAMENTO
        // ===================================

        /// <summary>
        /// Campo per ordinamento
        /// Valori: "datarichiesta", "dataresposta", "numero", "lotto", "stato"
        /// Default: "datarichiesta" (le più vecchie prima = priorità)
        /// </summary>
        public string? OrderBy { get; set; } = "datarichiesta";

        /// <summary>
        /// Ordinamento decrescente
        /// </summary>
        public bool OrderDescending { get; set; } = false; // Default ASC (più vecchie prima = priorità)

        // ===================================
        // METODI DI VALIDAZIONE
        // ===================================

        /// <summary>
        /// Assicura che i parametri di paginazione siano validi
        /// </summary>
        public void Validate()
        {
            if (PageNumber < 1)
                PageNumber = 1;

            if (PageSize < 1)
                PageSize = 20;

            if (PageSize > 100)
                PageSize = 100; // Limite massimo per evitare problemi di performance

            if (GiorniScadenza < 1)
                GiorniScadenza = 7;

            if (GiorniScadenza > 365)
                GiorniScadenza = 365; // Limite massimo sensato
        }

        // ===================================
        // UTILITY METHODS
        // ===================================

        /// <summary>
        /// Verifica se ci sono filtri attivi
        /// </summary>
        public bool HasFilters => !string.IsNullOrWhiteSpace(SearchTerm)
            || GaraId.HasValue
            || LottoId.HasValue
            || !string.IsNullOrWhiteSpace(RispostaDaUserId)
            || NumeroProgressivo.HasValue
            || IsChiusa.HasValue
            || SoloNonRisposte
            || SoloRisposteNonChiuse
            || SoloScadute
            || DataRichiestaDa.HasValue
            || DataRichiestaA.HasValue
            || DataRispostaDa.HasValue
            || DataRispostaA.HasValue;

        /// <summary>
        /// Crea una copia dei filtri per una nuova pagina
        /// </summary>
        public RichiestaIntegrazioneFilterViewModel ForPage(int pageNumber)
        {
            return new RichiestaIntegrazioneFilterViewModel
            {
                PageNumber = pageNumber,
                PageSize = this.PageSize,
                SearchTerm = this.SearchTerm,
                GaraId = this.GaraId,
                LottoId = this.LottoId,
                RispostaDaUserId = this.RispostaDaUserId,
                NumeroProgressivo = this.NumeroProgressivo,
                IsChiusa = this.IsChiusa,
                SoloNonRisposte = this.SoloNonRisposte,
                SoloRisposteNonChiuse = this.SoloRisposteNonChiuse,
                SoloScadute = this.SoloScadute,
                GiorniScadenza = this.GiorniScadenza,
                DataRichiestaDa = this.DataRichiestaDa,
                DataRichiestaA = this.DataRichiestaA,
                DataRispostaDa = this.DataRispostaDa,
                DataRispostaA = this.DataRispostaA,
                OrderBy = this.OrderBy,
                OrderDescending = this.OrderDescending
            };
        }

        /// <summary>
        /// Resetta tutti i filtri mantenendo solo paginazione e ordinamento
        /// </summary>
        public void ResetFilters()
        {
            SearchTerm = null;
            GaraId = null;
            LottoId = null;
            RispostaDaUserId = null;
            NumeroProgressivo = null;
            IsChiusa = null;
            SoloNonRisposte = false;
            SoloRisposteNonChiuse = false;
            SoloScadute = false;
            GiorniScadenza = 7;
            DataRichiestaDa = null;
            DataRichiestaA = null;
            DataRispostaDa = null;
            DataRispostaA = null;
            PageNumber = 1; // Torna alla prima pagina quando resetti i filtri
        }

        /// <summary>
        /// Imposta filtri per vedere richieste NON RISPOSTE (PRIORITÀ MASSIMA)
        /// Usato nel widget dashboard "⚠️ Richieste Non Risposte"
        /// </summary>
        public void SetFiltroNonRisposte()
        {
            ResetFilters();
            SoloNonRisposte = true;
            OrderBy = "datarichiesta";
            OrderDescending = false; // Le più vecchie prima = maggiore priorità
        }

        /// <summary>
        /// Imposta filtri per vedere richieste APERTE (non chiuse)
        /// Usato nel widget dashboard "📋 Richieste Aperte"
        /// </summary>
        public void SetFiltroAperte()
        {
            ResetFilters();
            IsChiusa = false;
            OrderBy = "datarichiesta";
            OrderDescending = false; // Le più vecchie prima
        }

        /// <summary>
        /// Imposta filtri per vedere richieste CHIUSE
        /// </summary>
        public void SetFiltroChiuse()
        {
            ResetFilters();
            IsChiusa = true;
            OrderBy = "dataresposta";
            OrderDescending = true; // Le più recenti prima
        }

        /// <summary>
        /// Imposta filtri per vedere richieste SCADUTE (>X giorni senza risposta)
        /// Usato per alert e monitoraggio performance
        /// </summary>
        public void SetFiltroScadute(int giorni = 7)
        {
            ResetFilters();
            SoloScadute = true;
            GiorniScadenza = giorni;
            OrderBy = "datarichiesta";
            OrderDescending = false; // Le più vecchie prima = più urgenti
        }

        /// <summary>
        /// Imposta filtri per vedere richieste risposte ma non ancora chiuse
        /// Utile per monitorare richieste in attesa chiusura dall'ente
        /// </summary>
        public void SetFiltroRisposteNonChiuse()
        {
            ResetFilters();
            SoloRisposteNonChiuse = true;
            OrderBy = "dataresposta";
            OrderDescending = false; // Le più vecchie prima
        }

        /// <summary>
        /// Imposta filtri per vedere richieste di un lotto specifico
        /// </summary>
        public void SetFiltroPerLotto(Guid lottoId)
        {
            ResetFilters();
            LottoId = lottoId;
            OrderBy = "numero"; // Ordina per numero progressivo
            OrderDescending = false;
        }

        /// <summary>
        /// Imposta filtri per vedere le mie risposte (utente corrente)
        /// </summary>
        public void SetFiltroMieRisposte(string userId)
        {
            ResetFilters();
            RispostaDaUserId = userId;
            OrderBy = "dataresposta";
            OrderDescending = true; // Le più recenti prima
        }
    }
}