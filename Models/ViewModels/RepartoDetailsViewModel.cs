namespace Trasformazioni.Models.ViewModels
{
    public class RepartoDetailsViewModel
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Descrizione { get; set; }

        public List<RepartoUtenteViewModel> Utenti { get; set; } = new();
        public int UtentiCount => Utenti.Count;

        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? ModifiedAt { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
