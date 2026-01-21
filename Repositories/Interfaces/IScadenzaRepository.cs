using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;
using Trasformazioni.Models.ViewModels;

namespace Trasformazioni.Data.Repositories
{
    /// <summary>
    /// Interfaccia per il repository delle Scadenze
    /// Definisce le operazioni di accesso ai dati per l'entità Scadenza
    /// Gestisce lo scadenzario con supporto per scadenze automatiche e manuali
    /// CRITICO: utilizzato da background job giornaliero
    /// </summary>
    public interface IScadenzaRepository
    {
        // ===================================
        // OPERAZIONI BASE (CRUD)
        // ===================================

        /// <summary>
        /// Ottiene tutte le scadenze
        /// </summary>
        Task<IEnumerable<Scadenza>> GetAllAsync();

        /// <summary>
        /// Ottiene una scadenza per ID
        /// </summary>
        /// <param name="id">ID della scadenza</param>
        Task<Scadenza?> GetByIdAsync(Guid id);

        /// <summary>
        /// Aggiunge una nuova scadenza
        /// </summary>
        /// <param name="scadenza">Entità scadenza da aggiungere</param>
        Task<Scadenza> AddAsync(Scadenza scadenza);

        /// <summary>
        /// Aggiorna una scadenza esistente
        /// </summary>
        /// <param name="scadenza">Entità scadenza da aggiornare</param>
        Task UpdateAsync(Scadenza scadenza);

        /// <summary>
        /// Elimina una scadenza (soft delete)
        /// </summary>
        /// <param name="id">ID della scadenza</param>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Verifica se esiste una scadenza con l'ID specificato
        /// </summary>
        /// <param name="id">ID della scadenza</param>
        Task<bool> ExistsAsync(Guid id);

        // ===================================
        // OPERAZIONI CON RELAZIONI
        // ===================================

        /// <summary>
        /// Ottiene una scadenza con la gara inclusa
        /// </summary>
        /// <param name="id">ID della scadenza</param>
        Task<Scadenza?> GetWithGaraAsync(Guid id);

        /// <summary>
        /// Ottiene una scadenza con il lotto incluso
        /// </summary>
        /// <param name="id">ID della scadenza</param>
        Task<Scadenza?> GetWithLottoAsync(Guid id);

        /// <summary>
        /// Ottiene una scadenza con il preventivo incluso
        /// </summary>
        /// <param name="id">ID della scadenza</param>
        Task<Scadenza?> GetWithPreventivoAsync(Guid id);

        /// <summary>
        /// Ottiene una scadenza con tutte le relazioni
        /// </summary>
        /// <param name="id">ID della scadenza</param>
        Task<Scadenza?> GetCompleteAsync(Guid id);

        // ===================================
        // RICERCHE PER ENTITÀ
        // ===================================

        /// <summary>
        /// Ottiene tutte le scadenze di una gara
        /// </summary>
        /// <param name="garaId">ID della gara</param>
        /// <param name="soloAutomatiche">Se true, filtra solo scadenze automatiche; se false, solo manuali; se null, tutte</param>
        Task<IEnumerable<Scadenza>> GetByGaraIdAsync(Guid garaId, bool? soloAutomatiche = null);

        /// <summary>
        /// Ottiene tutte le scadenze di un lotto
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        /// <param name="soloAutomatiche">Se true, filtra solo scadenze automatiche; se false, solo manuali; se null, tutte</param>
        Task<IEnumerable<Scadenza>> GetByLottoIdAsync(Guid lottoId, bool? soloAutomatiche = null);

        /// <summary>
        /// Ottiene tutte le scadenze di un preventivo
        /// </summary>
        /// <param name="preventivoId">ID del preventivo</param>
        /// <param name="soloAutomatiche">Se true, filtra solo scadenze automatiche; se false, solo manuali; se null, tutte</param>
        Task<IEnumerable<Scadenza>> GetByPreventivoIdAsync(Guid preventivoId, bool? soloAutomatiche = null);

        /// <summary>
        /// Ottiene una scadenza automatica specifica per gara e tipo
        /// Usato per sincronizzazione scadenze automatiche
        /// </summary>
        /// <param name="garaId">ID della gara</param>
        /// <param name="tipo">Tipo di scadenza</param>
        Task<Scadenza?> GetByGaraAndTipoAsync(Guid garaId, TipoScadenza tipo);

        /// <summary>
        /// Ottiene una scadenza automatica specifica per lotto e tipo
        /// Usato per sincronizzazione scadenze automatiche
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        /// <param name="tipo">Tipo di scadenza</param>
        Task<Scadenza?> GetByLottoAndTipoAsync(Guid lottoId, TipoScadenza tipo);

        // ===================================
        // RICERCHE PER STATO
        // ===================================

        /// <summary>
        /// Ottiene scadenze paginate con filtri e ordinamento
        /// </summary>
        /// <param name="filters">Filtri e parametri di paginazione</param>
        /// <returns>Tupla con (Items, TotalCount)</returns>
        Task<(IEnumerable<Scadenza> Items, int TotalCount)> GetPagedAsync(ScadenzaFilterViewModel filters);

        /// <summary>
        /// Ottiene tutte le scadenze attive (non completate)
        /// </summary>
        Task<IEnumerable<Scadenza>> GetAttiveAsync();

        /// <summary>
        /// Ottiene tutte le scadenze completate
        /// </summary>
        Task<IEnumerable<Scadenza>> GetCompletateAsync();

        /// <summary>
        /// Ottiene le scadenze scadute (data passata e non completate)
        /// </summary>
        Task<IEnumerable<Scadenza>> GetScaduteAsync();

        /// <summary>
        /// Ottiene le scadenze attive per una gara
        /// </summary>
        /// <param name="garaId">ID della gara</param>
        Task<IEnumerable<Scadenza>> GetAttiveByGaraIdAsync(Guid garaId);

        /// <summary>
        /// Ottiene le scadenze attive per un lotto
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        Task<IEnumerable<Scadenza>> GetAttiveByLottoIdAsync(Guid lottoId);

        // ===================================
        // RICERCHE PER TIPO
        // ===================================

        /// <summary>
        /// Ottiene scadenze per tipo
        /// </summary>
        /// <param name="tipo">Tipo di scadenza</param>
        Task<IEnumerable<Scadenza>> GetByTipoAsync(TipoScadenza tipo);

        /// <summary>
        /// Ottiene scadenze attive per tipo
        /// </summary>
        /// <param name="tipo">Tipo di scadenza</param>
        Task<IEnumerable<Scadenza>> GetAttiveByTipoAsync(TipoScadenza tipo);

        // ===================================
        // RICERCHE TEMPORALI (CRITICHE per background job)
        // ===================================

        /// <summary>
        /// Ottiene scadenze in scadenza (entro X giorni e non completate)
        /// CRITICO: usato dal background job per notifiche
        /// </summary>
        /// <param name="giorniPreavviso">Numero di giorni per considerare la scadenza imminente</param>
        Task<IEnumerable<Scadenza>> GetInScadenzaAsync(int giorniPreavviso = 7);

        /// <summary>
        /// Ottiene scadenze per oggi (non completate)
        /// </summary>
        Task<IEnumerable<Scadenza>> GetOggiAsync();

        /// <summary>
        /// Ottiene scadenze in un range di date
        /// </summary>
        /// <param name="dataInizio">Data inizio</param>
        /// <param name="dataFine">Data fine</param>
        Task<IEnumerable<Scadenza>> GetByDataScadenzaRangeAsync(DateTime dataInizio, DateTime dataFine);

        /// <summary>
        /// Ottiene scadenze per una data specifica
        /// </summary>
        /// <param name="data">Data scadenza</param>
        Task<IEnumerable<Scadenza>> GetByDataScadenzaAsync(DateTime data);

        // ===================================
        // RICERCHE PER ORIGINE
        // ===================================

        /// <summary>
        /// Ottiene scadenze automatiche (generate dal sistema)
        /// </summary>
        Task<IEnumerable<Scadenza>> GetAutomaticheAsync();

        /// <summary>
        /// Ottiene scadenze manuali (inserite dall'utente)
        /// </summary>
        Task<IEnumerable<Scadenza>> GetManualiAsync();

        /// <summary>
        /// Ottiene scadenze automatiche attive
        /// </summary>
        Task<IEnumerable<Scadenza>> GetAutomaticheAttiveAsync();

        // ===================================
        // STATISTICHE
        // ===================================

        /// <summary>
        /// Ottiene il conteggio delle scadenze attive
        /// </summary>
        Task<int> GetCountAttiveAsync();

        /// <summary>
        /// Ottiene il conteggio delle scadenze scadute
        /// </summary>
        Task<int> GetCountScaduteAsync();

        /// <summary>
        /// Ottiene il conteggio delle scadenze in scadenza (entro X giorni)
        /// </summary>
        /// <param name="giorniPreavviso">Numero di giorni</param>
        Task<int> GetCountInScadenzaAsync(int giorniPreavviso = 7);

        /// <summary>
        /// Ottiene il conteggio delle scadenze per tipo
        /// </summary>
        Task<Dictionary<TipoScadenza, int>> GetCountByTipoAsync();

        /// <summary>
        /// Ottiene statistiche sulle scadenze
        /// </summary>
        /// <returns>
        /// Dictionary con chiavi:
        /// - TotaleScadenze
        /// - Attive
        /// - Completate
        /// - Scadute
        /// - InScadenza (entro 7 giorni)
        /// - Automatiche
        /// - Manuali
        /// </returns>
        Task<Dictionary<string, int>> GetStatisticheAsync();
    }
}