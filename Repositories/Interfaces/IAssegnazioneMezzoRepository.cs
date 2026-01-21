using Trasformazioni.Models.Entities;

namespace Trasformazioni.Data.Repositories
{
    /// <summary>
    /// Interfaccia per l'accesso ai dati delle assegnazioni mezzi
    /// </summary>
    public interface IAssegnazioneMezzoRepository
    {
        /// <summary>
        /// Ottiene tutte le assegnazioni non cancellate
        /// </summary>
        Task<IEnumerable<AssegnazioneMezzo>> GetAllAsync();

        /// <summary>
        /// Ottiene un'assegnazione per ID (include Mezzo e Utente)
        /// </summary>
        Task<AssegnazioneMezzo?> GetByIdAsync(Guid id);

        /// <summary>
        /// Ottiene tutte le assegnazioni di un mezzo specifico (include Utente)
        /// </summary>
        /// <param name="mezzoId">ID del mezzo</param>
        /// <param name="includeChiuse">Se true include anche le assegnazioni chiuse</param>
        Task<IEnumerable<AssegnazioneMezzo>> GetByMezzoIdAsync(Guid mezzoId, bool includeChiuse = true);

        /// <summary>
        /// Ottiene tutte le assegnazioni di un utente specifico (include Mezzo)
        /// </summary>
        /// <param name="utenteId">ID dell'utente</param>
        /// <param name="includeChiuse">Se true include anche le assegnazioni chiuse</param>
        Task<IEnumerable<AssegnazioneMezzo>> GetByUtenteIdAsync(string utenteId, bool includeChiuse = true);

        /// <summary>
        /// Ottiene l'assegnazione attualmente attiva per un mezzo (DataFine = null)
        /// </summary>
        Task<AssegnazioneMezzo?> GetAssegnazioneAttivaByMezzoIdAsync(Guid mezzoId);

        /// <summary>
        /// Ottiene tutte le assegnazioni attive di un utente (DataFine = null)
        /// </summary>
        Task<IEnumerable<AssegnazioneMezzo>> GetAssegnazioniAttiveByUtenteIdAsync(string utenteId);

        /// <summary>
        /// Ottiene tutte le prenotazioni future (DataInizio > oggi, DataFine = null)
        /// </summary>
        Task<IEnumerable<AssegnazioneMezzo>> GetPrenotazioniFutureAsync();

        /// <summary>
        /// Ottiene tutte le assegnazioni in corso (DataInizio <= oggi, DataFine = null)
        /// </summary>
        Task<IEnumerable<AssegnazioneMezzo>> GetAssegnazioniInCorsoAsync();

        /// <summary>
        /// Verifica se un mezzo ha un'assegnazione attiva
        /// </summary>
        Task<bool> HasAssegnazioneAttivaAsync(Guid mezzoId);

        /// <summary>
        /// Verifica se un utente ha già un'assegnazione attiva
        /// </summary>
        Task<bool> HasUtenteAssegnazioneAttivaAsync(string utenteId);

        /// <summary>
        /// Verifica se esiste una sovrapposizione di periodi per il mezzo specificato
        /// </summary>
        /// <param name="mezzoId">ID del mezzo</param>
        /// <param name="dataInizio">Data/ora inizio della nuova assegnazione</param>
        /// <param name="dataFine">Data/ora fine della nuova assegnazione (null per assegnazioni a tempo indeterminato)</param>
        /// <param name="excludeAssegnazioneId">ID assegnazione da escludere (per modifica)</param>
        /// <returns>True se esiste sovrapposizione, False se il periodo è libero</returns>
        Task<bool> HasSovrapposizionePeriodoAsync(
            Guid mezzoId,
            DateTime dataInizio,
            DateTime? dataFine,
            Guid? excludeAssegnazioneId = null);

        /// <summary>
        /// Ottiene tutti i periodi occupati per un mezzo (per calendario)
        /// Include assegnazioni in corso E prenotazioni future
        /// </summary>
        /// <param name="mezzoId">ID del mezzo</param>
        /// <returns>Lista di assegnazioni attive/future ordinate per DataInizio</returns>
        Task<IEnumerable<AssegnazioneMezzo>> GetPeriodiOccupatiAsync(Guid mezzoId);

        /// <summary>
        /// Aggiunge una nuova assegnazione
        /// </summary>
        Task AddAsync(AssegnazioneMezzo assegnazione);

        /// <summary>
        /// Aggiorna un'assegnazione esistente
        /// </summary>
        Task UpdateAsync(AssegnazioneMezzo assegnazione);

        /// <summary>
        /// Elimina un'assegnazione (soft delete)
        /// </summary>
        Task DeleteAsync(AssegnazioneMezzo assegnazione);

        /// <summary>
        /// Salva le modifiche nel database
        /// </summary>
        Task<int> SaveChangesAsync();
    }
}