using System.ComponentModel.DataAnnotations;

namespace Trasformazioni.Models.ViewModels
{
    public class RepartoFilterViewModel
    {
        [Display(Name = "Ricerca")]
        public string? SearchTerm { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string OrderBy { get; set; } = "Nome";
        public string OrderDirection { get; set; } = "asc";

        public bool HasActiveFilters => !string.IsNullOrWhiteSpace(SearchTerm);
    }
}
