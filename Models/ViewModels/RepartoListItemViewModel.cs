namespace Trasformazioni.Models.ViewModels
{
    public class RepartoListItemViewModel
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Descrizione { get; set; }
        public int UtentiCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
