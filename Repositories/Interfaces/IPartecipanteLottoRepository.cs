using Trasformazioni.Models.Entities;

namespace Trasformazioni.Data.Repositories
{
    /// <summary>
    /// Interfaccia per il repository dei Partecipanti Lotto
    /// Definisce le operazioni di accesso ai dati per l'entità PartecipanteLotto
    /// Gestisce il censimento dei partecipanti ad un lotto (tipicamente per lotti non vinti)
    /// </summary>
    public interface IPartecipanteLottoRepository
    {
        // ===================================
        // OPERAZIONI BASE (CRUD)
        // ===================================

        /// <summary>
        /// Ottiene tutti i partecipanti
        /// </summary>
        Task<IEnumerable<PartecipanteLotto>> GetAllAsync();

        /// <summary>
        /// Ottiene un partecipante per ID
        /// </summary>
        /// <param name="id">ID del partecipante</param>
        Task<PartecipanteLotto?> GetByIdAsync(Guid id);

        /// <summary>
        /// Aggiunge un nuovo partecipante
        /// </summary>
        /// <param name="partecipante">Entità partecipante da aggiungere</param>
        Task<PartecipanteLotto> AddAsync(PartecipanteLotto partecipante);

        /// <summary>
        /// Aggiorna un partecipante esistente
        /// </summary>
        /// <param name="partecipante">Entità partecipante da aggiornare</param>
        Task UpdateAsync(PartecipanteLotto partecipante);

        /// <summary>
        /// Elimina un partecipante (soft delete)
        /// </summary>
        /// <param name="id">ID del partecipante</param>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Verifica se esiste un partecipante con l'ID specificato
        /// </summary>
        /// <param name="id">ID del partecipante</param>
        Task<bool> ExistsAsync(Guid id);

        // ===================================
        // OPERAZIONI CON RELAZIONI
        // ===================================

        /// <summary>
        /// Verifica se un lotto ha già un aggiudicatario
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        /// <param name="excludeId">ID del partecipante da escludere (per edit)</param>
        Task<bool> HasAggiudicatarioAsync(Guid lottoId, Guid? excludeId = null);

        /// <summary>
        /// Ottiene un partecipante con il lotto incluso
        /// </summary>
        /// <param name="id">ID del partecipante</param>
        Task<PartecipanteLotto?> GetWithLottoAsync(Guid id);

        /// <summary>
        /// Ottiene un partecipante con il soggetto incluso
        /// </summary>
        /// <param name="id">ID del partecipante</param>
        Task<PartecipanteLotto?> GetWithSoggettoAsync(Guid id);

        /// <summary>
        /// Ottiene un partecipante con tutte le relazioni (lotto, gara, soggetto)
        /// </summary>
        /// <param name="id">ID del partecipante</param>
        Task<PartecipanteLotto?> GetCompleteAsync(Guid id);

        // ===================================
        // RICERCHE PER LOTTO
        // ===================================

        /// <summary>
        /// Ottiene tutti i partecipanti di un lotto
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        Task<IEnumerable<PartecipanteLotto>> GetByLottoIdAsync(Guid lottoId);

        /// <summary>
        /// Ottiene l'aggiudicatario di un lotto (se presente)
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        Task<PartecipanteLotto?> GetAggiudicatarioByLottoIdAsync(Guid lottoId);

        /// <summary>
        /// Ottiene i partecipanti scartati dall'ente per un lotto
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        Task<IEnumerable<PartecipanteLotto>> GetScartatiByLottoIdAsync(Guid lottoId);

        /// <summary>
        /// Ottiene i partecipanti non aggiudicatari per un lotto
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        Task<IEnumerable<PartecipanteLotto>> GetNonAggiudicatariByLottoIdAsync(Guid lottoId);

        /// <summary>
        /// Ottiene i partecipanti ordinati per offerta economica (dal più basso al più alto)
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        Task<IEnumerable<PartecipanteLotto>> GetByLottoIdOrderedByOffertaAsync(Guid lottoId);

        // ===================================
        // RICERCHE PER SOGGETTO
        // ===================================

        /// <summary>
        /// Ottiene tutte le partecipazioni di un soggetto
        /// </summary>
        /// <param name="soggettoId">ID del soggetto</param>
        Task<IEnumerable<PartecipanteLotto>> GetBySoggettoIdAsync(Guid soggettoId);

        /// <summary>
        /// Ottiene le gare vinte da un soggetto
        /// </summary>
        /// <param name="soggettoId">ID del soggetto</param>
        Task<IEnumerable<PartecipanteLotto>> GetVinteBySoggettoIdAsync(Guid soggettoId);

        /// <summary>
        /// Ottiene le gare in cui un soggetto è stato scartato
        /// </summary>
        /// <param name="soggettoId">ID del soggetto</param>
        Task<IEnumerable<PartecipanteLotto>> GetScartateBySoggettoIdAsync(Guid soggettoId);

        // ===================================
        // RICERCHE PER STATO
        // ===================================

        /// <summary>
        /// Ottiene tutti gli aggiudicatari
        /// </summary>
        Task<IEnumerable<PartecipanteLotto>> GetAggiudicatariAsync();

        /// <summary>
        /// Ottiene tutti i partecipanti scartati
        /// </summary>
        Task<IEnumerable<PartecipanteLotto>> GetScartatiAsync();

        /// <summary>
        /// Ottiene tutti i non aggiudicatari (né vincitori né scartati)
        /// </summary>
        Task<IEnumerable<PartecipanteLotto>> GetNonAggiudicatariAsync();

        // ===================================
        // RICERCHE PER OFFERTA
        // ===================================

        /// <summary>
        /// Ottiene partecipanti con offerta in un determinato range
        /// </summary>
        /// <param name="min">Offerta minima</param>
        /// <param name="max">Offerta massima</param>
        Task<IEnumerable<PartecipanteLotto>> GetByOffertaRangeAsync(decimal min, decimal max);

        /// <summary>
        /// Ottiene partecipanti senza offerta valorizzata
        /// </summary>
        Task<IEnumerable<PartecipanteLotto>> GetSenzaOffertaAsync();

        // ===================================
        // STATISTICHE
        // ===================================

        /// <summary>
        /// Ottiene il conteggio dei partecipanti per un lotto
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        Task<int> GetCountByLottoAsync(Guid lottoId);

        /// <summary>
        /// Ottiene l'offerta media per un lotto
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        Task<decimal?> GetMediaOffertaByLottoAsync(Guid lottoId);

        /// <summary>
        /// Ottiene l'offerta più bassa per un lotto
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        Task<decimal?> GetOffertaMinimaByLottoAsync(Guid lottoId);

        /// <summary>
        /// Ottiene l'offerta più alta per un lotto
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        Task<decimal?> GetOffertaMassimaByLottoAsync(Guid lottoId);

        /// <summary>
        /// Ottiene statistiche sui partecipanti
        /// </summary>
        /// <returns>
        /// Dictionary con chiavi:
        /// - TotalePartecipanti
        /// - Aggiudicatari
        /// - Scartati
        /// - NonAggiudicatari
        /// - ConOfferta
        /// - SenzaOfferta
        /// </returns>
        Task<Dictionary<string, int>> GetStatisticheAsync();

        /// <summary>
        /// Ottiene statistiche per un lotto specifico
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        /// <returns>
        /// Dictionary con chiavi:
        /// - NumeroPartecipanti
        /// - NumeroScartati
        /// - OffertaMinima
        /// - OffertaMedia
        /// - OffertaMassima
        /// - HasAggiudicatario
        /// </returns>
        Task<Dictionary<string, object>> GetStatisticheLottoAsync(Guid lottoId);

        Task<int> CountByLottoIdAsync(Guid lottoId);
    }
}