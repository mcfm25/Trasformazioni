namespace Trasformazioni.Models.Entities
{
    /// <summary>
    /// Entità per la gestione degli allegati al Registro Contratti
    /// I file sono archiviati su MinIO con pattern: registro-contratti/{registroId}/{guid}_{filename}
    /// </summary>
    public class AllegatoRegistro : BaseEntity
    {
        /// <summary>
        /// Identificatore univoco
        /// </summary>
        public Guid Id { get; set; }

        // ===================================
        // RIFERIMENTO AL REGISTRO
        // ===================================

        /// <summary>
        /// FK al record del registro contratti
        /// </summary>
        public Guid RegistroContrattiId { get; set; }

        // ===================================
        // TIPO DOCUMENTO
        // ===================================

        /// <summary>
        /// FK al tipo documento (tabella TipoDocumento con Area = RegistroContratti)
        /// </summary>
        public Guid TipoDocumentoId { get; set; }

        /// <summary>
        /// Descrizione aggiuntiva dell'allegato
        /// </summary>
        public string? Descrizione { get; set; }

        // ===================================
        // INFO FILE
        // ===================================

        /// <summary>
        /// Nome originale del file
        /// </summary>
        public string NomeFile { get; set; } = string.Empty;

        /// <summary>
        /// Percorso del file in MinIO
        /// Pattern: registro-contratti/{registroId}/{guid}_{filename}
        /// </summary>
        public string PathMinIO { get; set; } = string.Empty;

        /// <summary>
        /// Dimensione del file in bytes
        /// </summary>
        public long DimensioneBytes { get; set; }

        /// <summary>
        /// MIME type del file
        /// </summary>
        public string MimeType { get; set; } = string.Empty;

        // ===================================
        // STATO UPLOAD
        // ===================================

        /// <summary>
        /// Indica se l'upload è stato completato con successo
        /// </summary>
        public bool IsUploadCompleto { get; set; } = false;

        /// <summary>
        /// Data e ora del caricamento
        /// </summary>
        public DateTime DataCaricamento { get; set; }

        /// <summary>
        /// FK all'utente che ha caricato il file
        /// </summary>
        public string CaricatoDaUserId { get; set; } = string.Empty;

        // ===================================
        // NAVIGATION PROPERTIES
        // ===================================

        /// <summary>
        /// Registro contratti a cui appartiene l'allegato
        /// </summary>
        public virtual RegistroContratti RegistroContratti { get; set; } = null!;

        /// <summary>
        /// Tipo documento associato
        /// </summary>
        public virtual TipoDocumento TipoDocumento { get; set; } = null!;

        /// <summary>
        /// Utente che ha caricato il file
        /// </summary>
        public virtual ApplicationUser CaricatoDa { get; set; } = null!;
    }
}