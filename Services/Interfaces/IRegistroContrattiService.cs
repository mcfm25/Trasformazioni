using Trasformazioni.Models.DTOs;
using Trasformazioni.Models.Enums;
using Trasformazioni.Models.ViewModels;

namespace Trasformazioni.Services.Interfaces
{
    /// <summary>
    /// Interfaccia per la business logic della gestione Registro Contratti
    /// </summary>
    public interface IRegistroContrattiService
    {
        // ===================================
        // QUERY - LETTURA
        // ===================================

        /// <summary>
        /// Ottiene tutti i registri
        /// </summary>
        Task<IEnumerable<RegistroContrattiListViewModel>> GetAllAsync();

        /// <summary>
        /// Ottiene il dettaglio di un registro
        /// </summary>
        Task<RegistroContrattiDetailsViewModel?> GetByIdAsync(Guid id);

        /// <summary>
        /// Ottiene un registro per modifica
        /// </summary>
        Task<RegistroContrattiEditViewModel?> GetForEditAsync(Guid id);

        /// <summary>
        /// Ottiene registri paginati con filtri
        /// </summary>
        Task<PagedResult<RegistroContrattiListViewModel>> GetPagedAsync(RegistroContrattiFilterViewModel filters);

        // ===================================
        // QUERY PER TIPO
        // ===================================

        /// <summary>
        /// Ottiene tutti i preventivi
        /// </summary>
        Task<IEnumerable<RegistroContrattiListViewModel>> GetPreventiviAsync();

        /// <summary>
        /// Ottiene tutti i contratti
        /// </summary>
        Task<IEnumerable<RegistroContrattiListViewModel>> GetContrattiAsync();

        // ===================================
        // QUERY PER STATO
        // ===================================

        /// <summary>
        /// Ottiene registri attivi
        /// </summary>
        Task<IEnumerable<RegistroContrattiListViewModel>> GetAttiviAsync();

        /// <summary>
        /// Ottiene registri in scadenza
        /// </summary>
        Task<IEnumerable<RegistroContrattiListViewModel>> GetInScadenzaAsync();

        /// <summary>
        /// Ottiene registri scaduti
        /// </summary>
        Task<IEnumerable<RegistroContrattiListViewModel>> GetScadutiAsync();

        // ===================================
        // QUERY PER RELAZIONI
        // ===================================

        /// <summary>
        /// Ottiene registri per cliente
        /// </summary>
        Task<IEnumerable<RegistroContrattiListViewModel>> GetByClienteIdAsync(Guid clienteId);

        /// <summary>
        /// Ottiene registri per categoria
        /// </summary>
        Task<IEnumerable<RegistroContrattiListViewModel>> GetByCategoriaIdAsync(Guid categoriaId);

        /// <summary>
        /// Ottiene registri per responsabile
        /// </summary>
        Task<IEnumerable<RegistroContrattiListViewModel>> GetByUtenteIdAsync(string utenteId);

        // ===================================
        // COMANDI - SCRITTURA
        // ===================================

        /// <summary>
        /// Crea un nuovo registro
        /// </summary>
        /// <returns>(Success, ErrorMessage, RegistroId)</returns>
        Task<(bool Success, string? ErrorMessage, Guid? RegistroId)> CreateAsync(
            RegistroContrattiCreateViewModel model,
            string currentUserId);

        /// <summary>
        /// Aggiorna un registro esistente
        /// </summary>
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(
            RegistroContrattiEditViewModel model,
            string currentUserId);

        /// <summary>
        /// Elimina un registro (soft delete)
        /// </summary>
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(Guid id, string currentUserId);

        // ===================================
        // GESTIONE STATI
        // ===================================

        /// <summary>
        /// Cambia lo stato di un registro
        /// </summary>
        Task<(bool Success, string? ErrorMessage)> CambiaStatoAsync(
            Guid id,
            StatoRegistro nuovoStato,
            string currentUserId);

        /// <summary>
        /// Invia il documento al cliente
        /// </summary>
        Task<(bool Success, string? ErrorMessage)> InviaAsync(Guid id, string currentUserId);

        /// <summary>
        /// Attiva un contratto
        /// </summary>
        Task<(bool Success, string? ErrorMessage)> AttivaAsync(
            Guid id,
            DateTime? dataAccettazione,
            string currentUserId);

        /// <summary>
        /// Sospende un contratto
        /// </summary>
        Task<(bool Success, string? ErrorMessage)> SospendiAsync(Guid id, string currentUserId);

        /// <summary>
        /// Riattiva un contratto sospeso
        /// </summary>
        Task<(bool Success, string? ErrorMessage)> RiattivaAsync(Guid id, string currentUserId);

        /// <summary>
        /// Annulla un documento
        /// </summary>
        Task<(bool Success, string? ErrorMessage)> AnnullaAsync(Guid id, string currentUserId);

        /// <summary>
        /// Propone il rinnovo per un contratto in scadenza
        /// </summary>
        Task<(bool Success, string? ErrorMessage)> ProponiRinnovoAsync(Guid id, string currentUserId);

        // ===================================
        // RINNOVO
        // ===================================

        /// <summary>
        /// Rinnova un contratto creando una nuova versione
        /// </summary>
        /// <returns>(Success, ErrorMessage, NuovoRegistroId)</returns>
        Task<(bool Success, string? ErrorMessage, Guid? NuovoRegistroId)> RinnovaAsync(
            Guid id,
            int? giorniRinnovo,
            string currentUserId);

        /// <summary>
        /// Prepara il ViewModel per il rinnovo
        /// </summary>
        Task<RegistroContrattiCreateViewModel?> PrepareRinnovoViewModelAsync(Guid id);

        // ===================================
        // VALIDAZIONI
        // ===================================

        /// <summary>
        /// Valida i dati di un registro
        /// </summary>
        Task<(bool IsValid, List<string> Errors)> ValidateAsync(
            RegistroContrattiCreateViewModel model,
            Guid? excludeId = null);

        /// <summary>
        /// Verifica se il numero protocollo è univoco
        /// </summary>
        Task<bool> IsNumeroProtocolloUniqueAsync(string numeroProtocollo, Guid? excludeId = null);

        /// <summary>
        /// Verifica se un registro esiste
        /// </summary>
        Task<bool> ExistsAsync(Guid id);

        /// <summary>
        /// Genera un nuovo numero protocollo automatico
        /// </summary>
        Task<string> GeneraNumeroProtocolloAsync(TipoRegistro tipo);

        // ===================================
        // OPERAZIONI BATCH (per job)
        // ===================================

        /// <summary>
        /// Aggiorna gli stati in base alle scadenze
        /// </summary>
        /// <returns>Lista dei registri modificati con dettagli per notifica</returns>
        Task<List<RegistroStatoChangeResult>> AggiornaStatiScadenzaAsync();

        /// <summary>
        /// Processa i rinnovi automatici
        /// </summary>
        /// <returns>Lista dei rinnovi creati con dettagli per notifica</returns>
        Task<List<RegistroStatoChangeResult>> ProcessaRinnoviAutomaticiAsync();

        // ===================================
        // STATISTICHE
        // ===================================

        /// <summary>
        /// Ottiene statistiche generali sui registri
        /// </summary>
        Task<RegistroContrattiStatisticheViewModel> GetStatisticheAsync();

        /// <summary>
        /// Ottiene statistiche per un cliente
        /// </summary>
        Task<RegistroContrattiStatisticheClienteViewModel> GetStatisticheClienteAsync(Guid clienteId);

        // ===================================
        // DROPDOWN HELPERS
        // ===================================

        /// <summary>
        /// Prepara il ViewModel per la creazione con tutti i dropdown
        /// </summary>
        Task<RegistroContrattiCreateViewModel> PrepareCreateViewModelAsync(Guid? parentId = null);

        /// <summary>
        /// Prepara il ViewModel per la modifica con tutti i dropdown
        /// </summary>
        Task<RegistroContrattiEditViewModel?> PrepareEditViewModelAsync(Guid id);

        /// <summary>
        /// Prepara il FilterViewModel con tutti i dropdown
        /// </summary>
        Task<RegistroContrattiFilterViewModel> PrepareFilterViewModelAsync(RegistroContrattiFilterViewModel? filters = null);

        /// <summary>
        /// Prepara il ViewModel per la clonazione di un registro esistente
        /// </summary>
        Task<RegistroContrattiCreateViewModel?> PrepareCloneViewModelAsync(Guid id);

        /// <summary>
        /// Esporta i registri attivi e rinnovati in formato CSV
        /// </summary>
        /// <returns>Contenuto CSV come stringa</returns>
        Task<string> ExportAttiviCsvAsync();
    }

    ///// <summary>
    ///// ViewModel per le statistiche del Registro Contratti
    ///// </summary>
    //public class RegistroContrattiStatisticheViewModel
    //{
    //    public int TotaleRegistri { get; set; }
    //    public int TotalePreventivi { get; set; }
    //    public int TotaleContratti { get; set; }

    //    public int ContrattiAttivi { get; set; }
    //    public int ContrattiInScadenza { get; set; }
    //    public int ContrattiScaduti { get; set; }

    //    public decimal TotaleCanoneAnnuoAttivi { get; set; }
    //    public decimal TotaleCanoneAnnuoInScadenza { get; set; }

    //    public int PreventiviInBozza { get; set; }
    //    public int PreventiviInviati { get; set; }
    //}

    ///// <summary>
    ///// ViewModel per le statistiche di un cliente
    ///// </summary>
    //public class RegistroContrattiStatisticheClienteViewModel
    //{
    //    public Guid ClienteId { get; set; }
    //    public string RagioneSociale { get; set; } = string.Empty;

    //    public int TotaleRegistri { get; set; }
    //    public int ContrattiAttivi { get; set; }
    //    public int ContrattiInScadenza { get; set; }

    //    public decimal TotaleCanoneAnnuo { get; set; }
    //}
}