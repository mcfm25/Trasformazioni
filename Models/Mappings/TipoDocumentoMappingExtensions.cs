using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;
using Trasformazioni.Models.ViewModels;

namespace Trasformazioni.Mappings
{
    /// <summary>
    /// Extension methods per il mapping tra entità TipoDocumento e ViewModels
    /// </summary>
    public static class TipoDocumentoMappingExtensions
    {
        /// <summary>
        /// Converte TipoDocumento in TipoDocumentoListViewModel
        /// </summary>
        public static TipoDocumentoListViewModel ToListViewModel(this TipoDocumento entity, int numeroDocumenti = 0)
        {
            return new TipoDocumentoListViewModel
            {
                Id = entity.Id,
                Nome = entity.Nome,
                Descrizione = entity.Descrizione,
                Area = entity.Area,
                AreaDisplayName = GetAreaDisplayName(entity.Area),
                IsSystem = entity.IsSystem,
                NumeroDocumenti = numeroDocumenti,
                CreatedAt = entity.CreatedAt
            };
        }

        /// <summary>
        /// Converte TipoDocumento in TipoDocumentoDetailsViewModel
        /// </summary>
        public static TipoDocumentoDetailsViewModel ToDetailsViewModel(
            this TipoDocumento entity, 
            int numeroDocumenti = 0,
            bool canDelete = true,
            bool canEdit = true)
        {
            return new TipoDocumentoDetailsViewModel
            {
                Id = entity.Id,
                Nome = entity.Nome,
                Descrizione = entity.Descrizione,
                Area = entity.Area,
                AreaDisplayName = GetAreaDisplayName(entity.Area),
                IsSystem = entity.IsSystem,
                NumeroDocumenti = numeroDocumenti,
                CanDelete = canDelete,
                CanEdit = canEdit,
                CreatedAt = entity.CreatedAt,
                CreatedBy = entity.CreatedBy,
                ModifiedAt = entity.ModifiedAt,
                ModifiedBy = entity.ModifiedBy
            };
        }

        /// <summary>
        /// Converte TipoDocumento in TipoDocumentoEditViewModel
        /// </summary>
        public static TipoDocumentoEditViewModel ToEditViewModel(this TipoDocumento entity, bool canChangeArea = true)
        {
            return new TipoDocumentoEditViewModel
            {
                Id = entity.Id,
                Nome = entity.Nome,
                Descrizione = entity.Descrizione,
                Area = entity.Area,
                IsSystem = entity.IsSystem,
                CanChangeArea = canChangeArea
            };
        }

        /// <summary>
        /// Converte TipoDocumento in TipoDocumentoDropdownViewModel
        /// </summary>
        public static TipoDocumentoDropdownViewModel ToDropdownViewModel(this TipoDocumento entity)
        {
            return new TipoDocumentoDropdownViewModel
            {
                Id = entity.Id,
                Nome = entity.Nome,
                Area = entity.Area,
                IsSystem = entity.IsSystem
            };
        }

        /// <summary>
        /// Converte TipoDocumentoCreateViewModel in entità TipoDocumento
        /// </summary>
        public static TipoDocumento ToEntity(this TipoDocumentoCreateViewModel model)
        {
            return new TipoDocumento
            {
                Id = Guid.NewGuid(),
                Nome = model.Nome.Trim(),
                Descrizione = model.Descrizione?.Trim(),
                Area = model.Area,
                IsSystem = false  // I nuovi tipi sono sempre personalizzati
            };
        }

        /// <summary>
        /// Aggiorna un'entità TipoDocumento con i dati di TipoDocumentoEditViewModel
        /// </summary>
        public static void UpdateFromViewModel(this TipoDocumento entity, TipoDocumentoEditViewModel model, bool canChangeArea)
        {
            entity.Nome = model.Nome.Trim();
            entity.Descrizione = model.Descrizione?.Trim();
            
            // L'area può essere cambiata solo se non ci sono documenti associati
            if (canChangeArea)
            {
                entity.Area = model.Area;
            }
            
            // IsSystem non può essere modificato
        }

        /// <summary>
        /// Ottiene il display name per un'area documento
        /// </summary>
        public static string GetAreaDisplayName(AreaDocumento area)
        {
            return area switch
            {
                AreaDocumento.Azienda => "Azienda",
                AreaDocumento.Gare => "Gare",
                AreaDocumento.Lotti => "Lotti",
                AreaDocumento.Mezzi => "Mezzi",
                AreaDocumento.Soggetti => "Soggetti",
                AreaDocumento.Scadenze => "Scadenze",
                _ => area.ToString()
            };
        }

        /// <summary>
        /// Ottiene l'icona Bootstrap per un'area documento
        /// </summary>
        public static string GetAreaIcon(AreaDocumento area)
        {
            return area switch
            {
                AreaDocumento.Azienda => "bi-building",
                AreaDocumento.Gare => "bi-trophy",
                AreaDocumento.Lotti => "bi-collection",
                AreaDocumento.Mezzi => "bi-truck",
                AreaDocumento.Soggetti => "bi-people",
                AreaDocumento.Scadenze => "bi-calendar-event",
                _ => "bi-file-earmark"
            };
        }

        /// <summary>
        /// Ottiene il colore badge per un'area documento
        /// </summary>
        public static string GetAreaBadgeClass(AreaDocumento area)
        {
            return area switch
            {
                AreaDocumento.Azienda => "bg-primary",
                AreaDocumento.Gare => "bg-success",
                AreaDocumento.Lotti => "bg-info",
                AreaDocumento.Mezzi => "bg-warning text-dark",
                AreaDocumento.Soggetti => "bg-secondary",
                AreaDocumento.Scadenze => "bg-danger",
                _ => "bg-secondary"
            };
        }
    }
}
