namespace Trasformazioni.Models.Entities
{
    /// <summary>
    /// Rappresenta un tipo documento richiesto per un lotto.
    /// Usato come checklist per verificare la completezza documentale
    /// prima del passaggio di stato.
    /// </summary>
    public class LottoDocumentoRichiesto
    {
        /// <summary>
        /// Identificativo univoco
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// ID del lotto a cui appartiene questa richiesta
        /// </summary>
        public Guid LottoId { get; set; }

        /// <summary>
        /// ID del tipo documento richiesto
        /// </summary>
        public Guid TipoDocumentoId { get; set; }

        // ===== NAVIGATION PROPERTIES =====

        /// <summary>
        /// Lotto di riferimento
        /// </summary>
        public virtual Lotto Lotto { get; set; } = null!;

        /// <summary>
        /// Tipo documento richiesto
        /// </summary>
        public virtual TipoDocumento TipoDocumento { get; set; } = null!;
    }
}