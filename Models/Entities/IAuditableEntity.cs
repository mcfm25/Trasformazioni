namespace Trasformazioni.Models.Entities
{
    /// <summary>
    /// Interfaccia per entità che supportano l'audit trail automatico
    /// </summary>
    public interface IAuditableEntity
    {
        DateTime CreatedAt { get; set; }
        string CreatedBy { get; set; }
        DateTime? ModifiedAt { get; set; }
        string? ModifiedBy { get; set; }

        DateTime? DeletedAt { get; set; }
        string? DeletedBy { get; set; }
        bool IsDeleted { get; set; }
    }
}