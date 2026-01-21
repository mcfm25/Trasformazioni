using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;
using Trasformazioni.Models.ViewModels;

namespace Trasformazioni.Data.Repositories
{
    /// <summary>
    /// Interfaccia per il repository dei Preventivi
    /// Definisce le operazioni di accesso ai dati per l'entità Preventivo
    /// Include operazioni specifiche per scadenze e auto-rinnovo (monitorati da background job)
    /// </summary>
    public interface IPreventivoRepository
    {
        // ===================================
        // OPERAZIONI BASE (CRUD)
        // ===================================

        /// <summary>
        /// Ottiene tutti i preventivi
        /// </summary>
        Task<IEnumerable<Preventivo>> GetAllAsync();

        /// <summary>
        /// Ottiene un preventivo per ID
        /// </summary>
        /// <param name="id">ID del preventivo</param>
        Task<Preventivo?> GetByIdAsync(Guid id);

        /// <summary>
        /// Aggiunge un nuovo preventivo
        /// </summary>
        /// <param name="preventivo">Entità preventivo da aggiungere</param>
        Task<Preventivo> AddAsync(Preventivo preventivo);

        /// <summary>
        /// Aggiorna un preventivo esistente
        /// </summary>
        /// <param name="preventivo">Entità preventivo da aggiornare</param>
        Task UpdateAsync(Preventivo preventivo);

        /// <summary>
        /// Elimina un preventivo (soft delete)
        /// </summary>
        /// <param name="id">ID del preventivo</param>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Verifica se esiste un preventivo con l'ID specificato
        /// </summary>
        /// <param name="id">ID del preventivo</param>
        Task<bool> ExistsAsync(Guid id);

        // ===================================
        // OPERAZIONI CON RELAZIONI
        // ===================================

        /// <summary>
        /// Ottiene un preventivo con lotto e soggetto inclusi
        /// </summary>
        /// <param name="id">ID del preventivo</param>
        Task<Preventivo?> GetWithRelationsAsync(Guid id);

        /// <summary>
        /// Ottiene un preventivo completo (con lotto, soggetto e gara)
        /// </summary>
        /// <param name="id">ID del preventivo</param>
        Task<Preventivo?> GetCompleteAsync(Guid id);

        // ===================================
        // RICERCHE PER LOTTO
        // ===================================

        /// <summary>
        /// Ottiene tutti i preventivi di un lotto
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        Task<IEnumerable<Preventivo>> GetByLottoIdAsync(Guid lottoId);

        /// <summary>
        /// Ottiene i preventivi validi di un lotto
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        Task<IEnumerable<Preventivo>> GetValidiByLottoIdAsync(Guid lottoId);

        /// <summary>
        /// Ottiene il preventivo selezionato per un lotto
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        Task<Preventivo?> GetSelezionatoByLottoIdAsync(Guid lottoId);

        // ===================================
        // RICERCHE PER SOGGETTO (FORNITORE)
        // ===================================

        /// <summary>
        /// Ottiene tutti i preventivi di un fornitore
        /// </summary>
        /// <param name="soggettoId">ID del soggetto (fornitore)</param>
        Task<IEnumerable<Preventivo>> GetBySoggettoIdAsync(Guid soggettoId);

        /// <summary>
        /// Ottiene i preventivi attivi di un fornitore
        /// </summary>
        /// <param name="soggettoId">ID del soggetto (fornitore)</param>
        Task<IEnumerable<Preventivo>> GetAttiviByForgnitoreAsync(Guid soggettoId);

        // ===================================
        // RICERCHE PER STATO
        // ===================================

        /// <summary>
        /// Ottiene preventivi per stato
        /// </summary>
        /// <param name="stato">Stato del preventivo</param>
        Task<IEnumerable<Preventivo>> GetByStatoAsync(StatoPreventivo stato);

        /// <summary>
        /// Ottiene preventivi in attesa (richiesti ma non ricevuti)
        /// </summary>
        Task<IEnumerable<Preventivo>> GetInAttesaAsync();

        /// <summary>
        /// Ottiene preventivi ricevuti
        /// </summary>
        Task<IEnumerable<Preventivo>> GetRicevutiAsync();

        /// <summary>
        /// Ottiene preventivi validi
        /// </summary>
        Task<IEnumerable<Preventivo>> GetValidiAsync();

        /// <summary>
        /// Ottiene preventivi selezionati
        /// </summary>
        Task<IEnumerable<Preventivo>> GetSelezionatiAsync();

        // ===================================
        // GESTIONE SCADENZE (CRITICO PER BACKGROUND JOB)
        // ===================================

        /// <summary>
        /// Ottiene preventivi in scadenza (entro X giorni)
        /// CRITICO: usato dal background job per notifiche
        /// </summary>
        /// <param name="giorniPreavviso">Numero di giorni per considerare la scadenza imminente</param>
        Task<IEnumerable<Preventivo>> GetInScadenzaAsync(int giorniPreavviso = 7);

        /// <summary>
        /// Ottiene preventivi scaduti (DataScadenza passata)
        /// CRITICO: usato dal background job per aggiornare stati
        /// </summary>
        Task<IEnumerable<Preventivo>> GetScadutiAsync();

        /// <summary>
        /// Ottiene preventivi con auto-rinnovo attivo
        /// CRITICO: usato dal background job per rinnovare automaticamente
        /// </summary>
        Task<IEnumerable<Preventivo>> GetConAutoRinnovoAsync();

        /// <summary>
        /// Ottiene preventivi da rinnovare (scaduti con auto-rinnovo)
        /// CRITICO: usato dal background job
        /// </summary>
        Task<IEnumerable<Preventivo>> GetDaRinnovareAsync();

        // ===================================
        // RICERCHE SPECIFICHE
        // ===================================

        /// <summary>
        /// Cerca preventivi per testo (descrizione, lotto, fornitore)
        /// </summary>
        /// <param name="searchTerm">Termine di ricerca</param>
        Task<IEnumerable<Preventivo>> SearchAsync(string searchTerm);

        /// <summary>
        /// Ottiene preventivi per range di date di scadenza
        /// </summary>
        /// <param name="dataInizio">Data inizio range</param>
        /// <param name="dataFine">Data fine range</param>
        Task<IEnumerable<Preventivo>> GetByScadenzaRangeAsync(DateTime dataInizio, DateTime dataFine);

        // ===================================
        // PAGINAZIONE E FILTRI
        // ===================================

        /// <summary>
        /// Ottiene preventivi paginati con filtri e ordinamento
        /// </summary>
        /// <param name="filters">Filtri e parametri di paginazione</param>
        /// <returns>Tupla con (Items, TotalCount)</returns>
        Task<(IEnumerable<Preventivo> Items, int TotalCount)> GetPagedAsync(PreventivoFilterViewModel filters);

        // ===================================
        // STATISTICHE E REPORT
        // ===================================

        /// <summary>
        /// Ottiene il conteggio dei preventivi per stato
        /// </summary>
        Task<Dictionary<StatoPreventivo, int>> GetCountByStatoAsync();

        /// <summary>
        /// Ottiene il conteggio dei preventivi per lotto
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        Task<int> GetCountByLottoIdAsync(Guid lottoId);

        /// <summary>
        /// Ottiene l'importo medio dei preventivi per un lotto
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        Task<decimal?> GetImportoMedioByLottoIdAsync(Guid lottoId);

        /// <summary>
        /// Ottiene l'importo minimo dei preventivi validi per un lotto
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        Task<decimal?> GetImportoMinimoValidiByLottoIdAsync(Guid lottoId);
    }
}