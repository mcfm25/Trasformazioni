namespace Trasformazioni.Models.ViewModels.DocumentoGara
{
    /// <summary>
    /// ViewModel per il filtro e la ricerca dei documenti
    /// </summary>
    public class DocumentoGaraFilterViewModel
    {
        /// <summary>
        /// Filtra per ID gara
        /// </summary>
        public Guid? GaraId { get; set; }

        /// <summary>
        /// Filtra per ID lotto
        /// </summary>
        public Guid? LottoId { get; set; }

        /// <summary>
        /// Filtra per ID preventivo
        /// </summary>
        public Guid? PreventivoId { get; set; }

        /// <summary>
        /// Filtra per ID integrazione
        /// </summary>
        public Guid? IntegrazioneId { get; set; }

        ///// <summary>
        ///// Filtra per tipo di documento
        ///// </summary>
        //public TipoDocumentoGara? Tipo { get; set; }

        // DOPO
        /// <summary>
        /// Filtra per tipo documento (da tabella TipiDocumento)
        /// </summary>
        public Guid? TipoDocumentoId { get; set; }

        /// <summary>
        /// Ricerca per nome file (case-insensitive)
        /// </summary>
        public string? NomeFile { get; set; }

        /// <summary>
        /// Filtra per utente che ha caricato
        /// </summary>
        public string? CaricatoDaUserId { get; set; }

        /// <summary>
        /// Filtra per data caricamento da
        /// </summary>
        public DateTime? DataCaricamentoDa { get; set; }

        /// <summary>
        /// Filtra per data caricamento a
        /// </summary>
        public DateTime? DataCaricamentoA { get; set; }

        /// <summary>
        /// Filtra per dimensione minima (in bytes)
        /// </summary>
        public long? DimensioneMinima { get; set; }

        /// <summary>
        /// Filtra per dimensione massima (in bytes)
        /// </summary>
        public long? DimensioneMassima { get; set; }

        /// <summary>
        /// Filtra per MIME type
        /// </summary>
        public string? MimeType { get; set; }

        /// <summary>
        /// Campo di ordinamento
        /// </summary>
        public string OrderBy { get; set; } = "DataCaricamento";

        /// <summary>
        /// Direzione ordinamento (true = crescente, false = decrescente)
        /// </summary>
        public bool OrderAscending { get; set; } = false;
    }
}