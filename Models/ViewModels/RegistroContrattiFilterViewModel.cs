using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Trasformazioni.Models.Enums;

namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per i filtri e la paginazione della lista registro contratti
    /// </summary>
    public class RegistroContrattiFilterViewModel
    {
        // ===================================
        // FILTRI
        // ===================================

        /// <summary>
        /// Filtro per tipo registro
        /// </summary>
        [Display(Name = "Tipo")]
        public TipoRegistro? TipoRegistro { get; set; }

        /// <summary>
        /// Filtro per stato
        /// </summary>
        [Display(Name = "Stato")]
        public StatoRegistro? Stato { get; set; }

        /// <summary>
        /// Filtro per cliente
        /// </summary>
        [Display(Name = "Cliente")]
        public Guid? ClienteId { get; set; }

        /// <summary>
        /// Filtro per categoria
        /// </summary>
        [Display(Name = "Categoria")]
        public Guid? CategoriaContrattoId { get; set; }

        /// <summary>
        /// Filtro per responsabile interno
        /// </summary>
        [Display(Name = "Responsabile")]
        public string? UtenteId { get; set; }

        /// <summary>
        /// Termine di ricerca full-text
        /// </summary>
        [Display(Name = "Cerca")]
        public string? SearchTerm { get; set; }

        /// <summary>
        /// Filtro data documento da
        /// </summary>
        [Display(Name = "Data Documento Da")]
        [DataType(DataType.Date)]
        public DateTime? DataDocumentoDa { get; set; }

        /// <summary>
        /// Filtro data documento a
        /// </summary>
        [Display(Name = "Data Documento A")]
        [DataType(DataType.Date)]
        public DateTime? DataDocumentoA { get; set; }

        /// <summary>
        /// Filtro data scadenza da
        /// </summary>
        [Display(Name = "Data Scadenza Da")]
        [DataType(DataType.Date)]
        public DateTime? DataScadenzaDa { get; set; }

        /// <summary>
        /// Filtro data scadenza a
        /// </summary>
        [Display(Name = "Data Scadenza A")]
        [DataType(DataType.Date)]
        public DateTime? DataScadenzaA { get; set; }

        /// <summary>
        /// Filtra solo contratti in scadenza
        /// </summary>
        [Display(Name = "Solo In Scadenza")]
        public bool SoloInScadenza { get; set; }

        /// <summary>
        /// Filtra solo contratti scaduti
        /// </summary>
        [Display(Name = "Solo Scaduti")]
        public bool SoloScaduti { get; set; }

        /// <summary>
        /// Se true, mostra solo documenti senza parent (documenti originali)
        /// </summary>
        public bool SoloRoot { get; set; } = false;

        /// <summary>
        /// Se true, mostra solo documenti con versioni (parent o children)
        /// </summary>
        public bool SoloConVersioni { get; set; } = false;

        // ===================================
        // PAGINAZIONE
        // ===================================

        /// <summary>
        /// Numero pagina corrente (1-based)
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Numero di elementi per pagina
        /// </summary>
        public int PageSize { get; set; } = 20;

        // ===================================
        // ORDINAMENTO
        // ===================================

        /// <summary>
        /// Campo di ordinamento
        /// </summary>
        [Display(Name = "Ordina per")]
        public string OrderBy { get; set; } = "DataDocumento";

        /// <summary>
        /// Direzione ordinamento (asc/desc)
        /// </summary>
        public string OrderDirection { get; set; } = "desc";

        // ===================================
        // SELECT LIST PER DROPDOWN
        // ===================================

        /// <summary>
        /// Lista clienti per dropdown filtro
        /// </summary>
        public SelectList? ClientiSelectList { get; set; }

        /// <summary>
        /// Lista categorie per dropdown filtro
        /// </summary>
        public SelectList? CategorieSelectList { get; set; }

        /// <summary>
        /// Lista utenti per dropdown filtro
        /// </summary>
        public SelectList? UtentiSelectList { get; set; }

        /// <summary>
        /// Lista tipi registro per dropdown filtro
        /// </summary>
        public SelectList? TipiRegistroSelectList { get; set; }

        /// <summary>
        /// Lista stati per dropdown filtro
        /// </summary>
        public SelectList? StatiSelectList { get; set; }

        /// <summary>
        /// Lista opzioni ordinamento
        /// </summary>
        public SelectList? OrderBySelectList { get; set; }

        // ===================================
        // PROPRIETÀ CALCOLATE
        // ===================================

        /// <summary>
        /// Indica se ci sono filtri attivi
        /// </summary>
        public bool HasActiveFilters =>
            TipoRegistro.HasValue ||
            Stato.HasValue ||
            ClienteId.HasValue ||
            CategoriaContrattoId.HasValue ||
            !string.IsNullOrWhiteSpace(UtenteId) ||
            !string.IsNullOrWhiteSpace(SearchTerm) ||
            DataDocumentoDa.HasValue ||
            DataDocumentoA.HasValue ||
            DataScadenzaDa.HasValue ||
            DataScadenzaA.HasValue ||
            SoloInScadenza ||
            SoloScaduti;

        /// <summary>
        /// Numero di filtri attivi
        /// </summary>
        public int ActiveFiltersCount
        {
            get
            {
                var count = 0;
                if (TipoRegistro.HasValue) count++;
                if (Stato.HasValue) count++;
                if (ClienteId.HasValue) count++;
                if (CategoriaContrattoId.HasValue) count++;
                if (!string.IsNullOrWhiteSpace(UtenteId)) count++;
                if (!string.IsNullOrWhiteSpace(SearchTerm)) count++;
                if (DataDocumentoDa.HasValue || DataDocumentoA.HasValue) count++;
                if (DataScadenzaDa.HasValue || DataScadenzaA.HasValue) count++;
                if (SoloInScadenza) count++;
                if (SoloScaduti) count++;
                return count;
            }
        }

        /// <summary>
        /// Indica se l'ordinamento è ascendente
        /// </summary>
        public bool IsAscending => OrderDirection.ToLower() == "asc";

        /// <summary>
        /// Restituisce la direzione opposta per toggle
        /// </summary>
        public string OppositeDirection => IsAscending ? "desc" : "asc";

        // ===================================
        // METODI HELPER
        // ===================================

        /// <summary>
        /// Resetta tutti i filtri
        /// </summary>
        public void ResetFilters()
        {
            TipoRegistro = null;
            Stato = null;
            ClienteId = null;
            CategoriaContrattoId = null;
            UtenteId = null;
            SearchTerm = null;
            DataDocumentoDa = null;
            DataDocumentoA = null;
            DataScadenzaDa = null;
            DataScadenzaA = null;
            SoloInScadenza = false;
            SoloScaduti = false;
            PageNumber = 1;
        }

        /// <summary>
        /// Costruisce la query string per i link di paginazione
        /// </summary>
        public string ToQueryString(int? pageNumber = null)
        {
            var parameters = new List<string>();

            if (TipoRegistro.HasValue)
                parameters.Add($"TipoRegistro={TipoRegistro.Value}");

            if (Stato.HasValue)
                parameters.Add($"Stato={Stato.Value}");

            if (ClienteId.HasValue)
                parameters.Add($"ClienteId={ClienteId.Value}");

            if (CategoriaContrattoId.HasValue)
                parameters.Add($"CategoriaContrattoId={CategoriaContrattoId.Value}");

            if (!string.IsNullOrWhiteSpace(UtenteId))
                parameters.Add($"UtenteId={Uri.EscapeDataString(UtenteId)}");

            if (!string.IsNullOrWhiteSpace(SearchTerm))
                parameters.Add($"SearchTerm={Uri.EscapeDataString(SearchTerm)}");

            if (DataDocumentoDa.HasValue)
                parameters.Add($"DataDocumentoDa={DataDocumentoDa.Value:yyyy-MM-dd}");

            if (DataDocumentoA.HasValue)
                parameters.Add($"DataDocumentoA={DataDocumentoA.Value:yyyy-MM-dd}");

            if (DataScadenzaDa.HasValue)
                parameters.Add($"DataScadenzaDa={DataScadenzaDa.Value:yyyy-MM-dd}");

            if (DataScadenzaA.HasValue)
                parameters.Add($"DataScadenzaA={DataScadenzaA.Value:yyyy-MM-dd}");

            if (SoloInScadenza)
                parameters.Add("SoloInScadenza=true");

            if (SoloScaduti)
                parameters.Add("SoloScaduti=true");

            parameters.Add($"PageNumber={pageNumber ?? PageNumber}");
            parameters.Add($"PageSize={PageSize}");
            parameters.Add($"OrderBy={OrderBy}");
            parameters.Add($"OrderDirection={OrderDirection}");

            return string.Join("&", parameters);
        }
    }
}