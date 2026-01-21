namespace Trasformazioni.Models.ViewModels
{
    public class RepartoListViewModel
    {
        public IEnumerable<RepartoListItemViewModel> Items { get; set; } = new List<RepartoListItemViewModel>();
        public int TotalCount { get; set; }
        public RepartoFilterViewModel Filter { get; set; } = new();

        public int TotalPages => (int)Math.Ceiling((double)TotalCount / Filter.PageSize);
    }
}
