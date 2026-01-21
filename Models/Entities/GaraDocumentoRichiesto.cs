namespace Trasformazioni.Models.Entities
{
    /// <summary>
    /// Entità per la checklist dei documenti richiesti a livello Gara.
    /// Tabella ponte senza soft delete (come LottoDocumentoRichiesto).
    /// </summary>
    public class GaraDocumentoRichiesto
    {
        public Guid Id { get; set; }
        public Guid GaraId { get; set; }
        public Guid TipoDocumentoId { get; set; }

        // Navigation properties
        public virtual Gara Gara { get; set; } = null!;
        public virtual TipoDocumento TipoDocumento { get; set; } = null!;
    }
}