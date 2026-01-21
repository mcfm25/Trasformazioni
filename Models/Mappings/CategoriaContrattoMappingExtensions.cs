using Trasformazioni.Models.Entities;
using Trasformazioni.Models.ViewModels;

namespace Trasformazioni.Mappings
{
    /// <summary>
    /// Extension methods per il mapping tra entità CategoriaContratto e ViewModels
    /// </summary>
    public static class CategoriaContrattoMappingExtensions
    {
        // ===================================
        // ENTITY → VIEWMODEL
        // ===================================

        /// <summary>
        /// Mappa un'entità CategoriaContratto a CategoriaContrattoListViewModel
        /// </summary>
        public static CategoriaContrattoListViewModel ToListViewModel(
            this CategoriaContratto categoria,
            int numeroUtilizzi = 0)
        {
            return new CategoriaContrattoListViewModel
            {
                Id = categoria.Id,
                Nome = categoria.Nome,
                Descrizione = categoria.Descrizione,
                Ordine = categoria.Ordine,
                IsAttivo = categoria.IsAttivo,
                NumeroUtilizzi = numeroUtilizzi
            };
        }

        /// <summary>
        /// Mappa un'entità CategoriaContratto a CategoriaContrattoDetailsViewModel
        /// </summary>
        public static CategoriaContrattoDetailsViewModel ToDetailsViewModel(
            this CategoriaContratto categoria,
            int numeroUtilizzi = 0)
        {
            return new CategoriaContrattoDetailsViewModel
            {
                Id = categoria.Id,
                Nome = categoria.Nome,
                Descrizione = categoria.Descrizione,
                Ordine = categoria.Ordine,
                IsAttivo = categoria.IsAttivo,
                NumeroUtilizzi = numeroUtilizzi,

                // Audit
                CreatedAt = categoria.CreatedAt,
                CreatedBy = categoria.CreatedBy,
                ModifiedAt = categoria.ModifiedAt,
                ModifiedBy = categoria.ModifiedBy
            };
        }

        /// <summary>
        /// Mappa un'entità CategoriaContratto a CategoriaContrattoEditViewModel
        /// </summary>
        public static CategoriaContrattoEditViewModel ToEditViewModel(
            this CategoriaContratto categoria,
            int numeroUtilizzi = 0)
        {
            return new CategoriaContrattoEditViewModel
            {
                Id = categoria.Id,
                Nome = categoria.Nome,
                Descrizione = categoria.Descrizione,
                Ordine = categoria.Ordine,
                IsAttivo = categoria.IsAttivo,
                NumeroUtilizzi = numeroUtilizzi
            };
        }

        // ===================================
        // VIEWMODEL → ENTITY
        // ===================================

        /// <summary>
        /// Crea un'entità CategoriaContratto da CategoriaContrattoCreateViewModel
        /// </summary>
        public static CategoriaContratto ToEntity(this CategoriaContrattoCreateViewModel viewModel)
        {
            return new CategoriaContratto
            {
                Id = Guid.NewGuid(),
                Nome = NormalizzaStringa(viewModel.Nome)!,
                Descrizione = NormalizzaStringa(viewModel.Descrizione),
                Ordine = viewModel.Ordine,
                IsAttivo = viewModel.IsAttivo

                // CreatedAt, CreatedBy, IsDeleted gestiti da AuditInterceptor
            };
        }

        /// <summary>
        /// Aggiorna un'entità CategoriaContratto con i dati del CategoriaContrattoEditViewModel
        /// </summary>
        public static void UpdateEntity(
            this CategoriaContrattoEditViewModel viewModel,
            CategoriaContratto categoria)
        {
            categoria.Nome = NormalizzaStringa(viewModel.Nome)!;
            categoria.Descrizione = NormalizzaStringa(viewModel.Descrizione);
            categoria.Ordine = viewModel.Ordine;
            categoria.IsAttivo = viewModel.IsAttivo;

            // ModifiedAt, ModifiedBy gestiti da AuditInterceptor
        }

        // ===================================
        // COLLECTION MAPPING
        // ===================================

        /// <summary>
        /// Mappa una collezione di CategoriaContratto a lista di CategoriaContrattoListViewModel
        /// </summary>
        public static IEnumerable<CategoriaContrattoListViewModel> ToListViewModels(
            this IEnumerable<CategoriaContratto> categorie,
            Func<Guid, int>? getNumeroUtilizzi = null)
        {
            return categorie.Select(c => c.ToListViewModel(
                getNumeroUtilizzi?.Invoke(c.Id) ?? 0
            ));
        }

        // ===================================
        // HELPER METHODS
        // ===================================

        /// <summary>
        /// Normalizza una stringa: trim e null se vuota
        /// </summary>
        private static string? NormalizzaStringa(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            var trimmed = value.Trim();
            return string.IsNullOrEmpty(trimmed) ? null : trimmed;
        }
    }
}