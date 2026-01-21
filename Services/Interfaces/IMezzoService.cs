using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;
using Trasformazioni.Models.ViewModels;

namespace Trasformazioni.Services
{
    /// <summary>
    /// Interfaccia per la gestione della business logic dei mezzi aziendali
    /// </summary>
    public interface IMezzoService
    {
        /// <summary>
        /// Ottiene tutti i mezzi
        /// </summary>
        Task<IEnumerable<MezzoListViewModel>> GetAllAsync();

        /// <summary>
        /// Ottiene tutte le entità Mezzo complete (con navigazioni caricate)
        /// Utile per operazioni che necessitano dell'entità completa anzichè del ViewModel
        /// </summary>
        Task<IEnumerable<Mezzo>> GetAllEntitiesAsync();

        /// <summary>
        /// Ottiene il dettaglio di un mezzo
        /// </summary>
        Task<MezzoDetailsViewModel?> GetByIdAsync(Guid id);

        /// <summary>
        /// Ottiene mezzi filtrati per stato
        /// </summary>
        Task<IEnumerable<MezzoListViewModel>> GetByStatoAsync(StatoMezzo stato);

        /// <summary>
        /// Ottiene mezzi filtrati per tipo di proprietà
        /// </summary>
        Task<IEnumerable<MezzoListViewModel>> GetByTipoProprietaAsync(TipoProprietaMezzo tipoProprieta);

        /// <summary>
        /// Cerca mezzi per targa
        /// </summary>
        Task<IEnumerable<MezzoListViewModel>> SearchByTargaAsync(string targa);

        /// <summary>
        /// Verifica se una targa è unica (non esiste già)
        /// </summary>
        Task<bool> IsTargaUniqueAsync(string targa, Guid? excludeId = null);

        /// <summary>
        /// Verifica se un Device IMEI è unico (non esiste già)
        /// </summary>
        Task<bool> IsDeviceIMEIUniqueAsync(string deviceIMEI, Guid? excludeId = null);

        /// <summary>
        /// Valida se una targa è nel formato corretto
        /// </summary>
        bool IsTargaValida(string targa);

        /// <summary>
        /// Crea un nuovo mezzo
        /// </summary>
        Task<(bool Success, string? ErrorMessage, Guid? MezzoId)> CreateAsync(MezzoCreateViewModel model);

        /// <summary>
        /// Aggiorna un mezzo esistente
        /// </summary>
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(Guid id, MezzoEditViewModel model);

        /// <summary>
        /// Elimina un mezzo (soft delete)
        /// </summary>
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(Guid id);

        /// <summary>
        /// Cambia lo stato di un mezzo
        /// </summary>
        Task<bool> CambiaStatoAsync(Guid id, StatoMezzo nuovoStato);

        /// <summary>
        /// Aggiorna il chilometraggio di un mezzo
        /// </summary>
        Task<bool> AggiornaChilometraggioAsync(Guid id, decimal chilometraggio);

        /// <summary>
        /// Ottiene il ViewModel per la modifica
        /// </summary>
        Task<MezzoEditViewModel?> GetEditViewModelAsync(Guid id);
    }
}