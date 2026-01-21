using Trasformazioni.Models.ViewModels;

namespace Trasformazioni.Services
{
    /// <summary>
    /// Interfaccia per la gestione della business logic delle assegnazioni mezzi
    /// </summary>
    public interface IAssegnazioneMezzoService
    {
        /// <summary>
        /// Ottiene tutte le assegnazioni
        /// </summary>
        Task<IEnumerable<AssegnazioneMezzoListViewModel>> GetAllAsync();

        /// <summary>
        /// Ottiene il dettaglio di un'assegnazione
        /// </summary>
        Task<AssegnazioneMezzoDetailsViewModel?> GetByIdAsync(Guid id);

        /// <summary>
        /// Ottiene lo storico assegnazioni di un mezzo
        /// </summary>
        /// <param name="mezzoId">ID del mezzo</param>
        /// <param name="includeChiuse">Se true include anche le assegnazioni chiuse</param>
        Task<IEnumerable<AssegnazioneMezzoListViewModel>> GetByMezzoIdAsync(Guid mezzoId, bool includeChiuse = true);

        /// <summary>
        /// Ottiene lo storico assegnazioni di un utente
        /// </summary>
        /// <param name="utenteId">ID dell'utente</param>
        /// <param name="includeChiuse">Se true include anche le assegnazioni chiuse</param>
        Task<IEnumerable<AssegnazioneMezzoListViewModel>> GetByUtenteIdAsync(string utenteId, bool includeChiuse = true);

        /// <summary>
        /// Ottiene l'assegnazione attualmente attiva per un mezzo
        /// </summary>
        Task<AssegnazioneMezzoDetailsViewModel?> GetAssegnazioneAttivaByMezzoIdAsync(Guid mezzoId);

        /// <summary>
        /// Ottiene tutte le assegnazioni attive di un utente
        /// </summary>
        Task<IEnumerable<AssegnazioneMezzoListViewModel>> GetAssegnazioniAttiveByUtenteIdAsync(string utenteId);

        /// <summary>
        /// Ottiene il ViewModel pre-compilato per la riconsegna
        /// </summary>
        Task<AssegnazioneMezzoCloseViewModel?> GetCloseViewModelAsync(Guid assegnazioneId);

        /// <summary>
        /// Ottiene i periodi occupati per visualizzazione calendario
        /// </summary>
        /// <param name="mezzoId">ID del mezzo</param>
        /// <returns>Lista di periodi occupati con dati utente</returns>
        Task<IEnumerable<PeriodoOccupatoViewModel>> GetPeriodiOccupatiAsync(Guid mezzoId);

        /// <summary>
        /// Crea una nuova assegnazione/prenotazione
        /// Gestisce automaticamente il cambio stato del mezzo (InUso o Prenotato)
        /// SUPPORTA VALIDAZIONE SOVRAPPOSIZIONE PERIODI
        /// </summary>
        Task<(bool Success, string? ErrorMessage, Guid? AssegnazioneId)> CreateAsync(AssegnazioneMezzoCreateViewModel model);

        /// <summary>
        /// Chiude un'assegnazione (riconsegna mezzo)
        /// Gestisce automaticamente il cambio stato del mezzo a Disponibile
        /// </summary>
        Task<(bool Success, string? ErrorMessage)> CloseAsync(Guid assegnazioneId, AssegnazioneMezzoCloseViewModel model);

        /// <summary>
        /// Cancella una prenotazione futura (soft delete)
        /// Gestisce automaticamente il cambio stato del mezzo a Disponibile
        /// </summary>
        Task<(bool Success, string? ErrorMessage)> CancellaPrenotazioneAsync(Guid assegnazioneId, string currentUserId, bool isAdmin);

        /// <summary>
        /// Verifica se un mezzo è disponibile per assegnazione nel periodo specificato
        /// Validazione avanzata per supportare assegnazioni multiple in coda
        /// </summary>
        /// <param name="mezzoId">ID del mezzo</param>
        /// <param name="dataInizio">Data/ora inizio assegnazione</param>
        /// <param name="dataFine">Data/ora fine assegnazione (null per tempo indeterminato)</param>
        /// <returns>True se disponibile, False se esiste sovrapposizione</returns>
        Task<bool> IsMezzoDisponibileAsync(Guid mezzoId, DateTime dataInizio, DateTime? dataFine);

        /// <summary>
        /// Verifica se un utente può essere assegnato (già utente non ha troppe assegnazioni attive)
        /// </summary>
        Task<bool> CanUtenteBeAssignedAsync(string utenteId);

        /// <summary>
        /// Verifica se l'utente corrente può chiudere l'assegnazione specificata
        /// </summary>
        Task<bool> CanUserCloseAssegnazioneAsync(Guid assegnazioneId, string currentUserId, bool isAdmin);
    }
}