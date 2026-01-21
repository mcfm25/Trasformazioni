using Trasformazioni.Models.Entities;

namespace Trasformazioni.Data.Repositories
{
    /// <summary>
    /// Interfaccia per il repository delle Valutazioni Lotto
    /// Definisce le operazioni di accesso ai dati per l'entità ValutazioneLotto
    /// Gestisce sia la valutazione tecnica che economica
    /// </summary>
    public interface IValutazioneLottoRepository
    {
        // ===================================
        // OPERAZIONI BASE (CRUD)
        // ===================================

        /// <summary>
        /// Ottiene tutte le valutazioni
        /// </summary>
        Task<IEnumerable<ValutazioneLotto>> GetAllAsync();

        /// <summary>
        /// Ottiene una valutazione per ID
        /// </summary>
        /// <param name="id">ID della valutazione</param>
        Task<ValutazioneLotto?> GetByIdAsync(Guid id);

        /// <summary>
        /// Aggiunge una nuova valutazione
        /// </summary>
        /// <param name="valutazione">Entità valutazione da aggiungere</param>
        Task<ValutazioneLotto> AddAsync(ValutazioneLotto valutazione);

        /// <summary>
        /// Aggiorna una valutazione esistente
        /// </summary>
        /// <param name="valutazione">Entità valutazione da aggiornare</param>
        Task UpdateAsync(ValutazioneLotto valutazione);

        /// <summary>
        /// Elimina una valutazione (soft delete)
        /// </summary>
        /// <param name="id">ID della valutazione</param>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Verifica se esiste una valutazione con l'ID specificato
        /// </summary>
        /// <param name="id">ID della valutazione</param>
        Task<bool> ExistsAsync(Guid id);

        // ===================================
        // OPERAZIONI CON RELAZIONI
        // ===================================

        /// <summary>
        /// Ottiene una valutazione con il lotto incluso
        /// </summary>
        /// <param name="id">ID della valutazione</param>
        Task<ValutazioneLotto?> GetWithLottoAsync(Guid id);

        /// <summary>
        /// Ottiene una valutazione con i valutatori inclusi
        /// </summary>
        /// <param name="id">ID della valutazione</param>
        Task<ValutazioneLotto?> GetWithValutatoriAsync(Guid id);

        /// <summary>
        /// Ottiene una valutazione con tutte le relazioni (lotto, valutatori)
        /// </summary>
        /// <param name="id">ID della valutazione</param>
        Task<ValutazioneLotto?> GetCompleteAsync(Guid id);

        // ===================================
        // RICERCHE SPECIFICHE
        // ===================================

        /// <summary>
        /// Ottiene la valutazione di un lotto specifico
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        Task<ValutazioneLotto?> GetByLottoIdAsync(Guid lottoId);

        /// <summary>
        /// Ottiene tutte le valutazioni tecniche effettuate da un valutatore
        /// </summary>
        /// <param name="valutatoreTecnicoId">ID del valutatore tecnico</param>
        Task<IEnumerable<ValutazioneLotto>> GetByValutatoreTecnicoAsync(string valutatoreTecnicoId);

        /// <summary>
        /// Ottiene tutte le valutazioni economiche effettuate da un valutatore
        /// </summary>
        /// <param name="valutatoreEconomicoId">ID del valutatore economico</param>
        Task<IEnumerable<ValutazioneLotto>> GetByValutatoreEconomicoAsync(string valutatoreEconomicoId);

        // ===================================
        // RICERCHE PER STATO VALUTAZIONE TECNICA
        // ===================================

        /// <summary>
        /// Ottiene valutazioni con valutazione tecnica approvata
        /// </summary>
        Task<IEnumerable<ValutazioneLotto>> GetValutazioniTecnicheApprovateAsync();

        /// <summary>
        /// Ottiene valutazioni con valutazione tecnica rifiutata
        /// </summary>
        Task<IEnumerable<ValutazioneLotto>> GetValutazioniTecnicheRifiutateAsync();

        /// <summary>
        /// Ottiene valutazioni con valutazione tecnica pendente (non ancora effettuata)
        /// </summary>
        Task<IEnumerable<ValutazioneLotto>> GetValutazioniTecnichePendentiAsync();

        // ===================================
        // RICERCHE PER STATO VALUTAZIONE ECONOMICA
        // ===================================

        /// <summary>
        /// Ottiene valutazioni con valutazione economica approvata
        /// </summary>
        Task<IEnumerable<ValutazioneLotto>> GetValutazioniEconomicheApprovateAsync();

        /// <summary>
        /// Ottiene valutazioni con valutazione economica rifiutata
        /// </summary>
        Task<IEnumerable<ValutazioneLotto>> GetValutazioniEconomicheRifiutateAsync();

        /// <summary>
        /// Ottiene valutazioni con valutazione economica pendente (non ancora effettuata)
        /// </summary>
        Task<IEnumerable<ValutazioneLotto>> GetValutazioniEconomichePendentiAsync();

        // ===================================
        // RICERCHE COMBINATE
        // ===================================

        /// <summary>
        /// Ottiene valutazioni con entrambe le fasi approvate
        /// </summary>
        Task<IEnumerable<ValutazioneLotto>> GetValutazioniCompleteApprovateAsync();

        /// <summary>
        /// Ottiene valutazioni con almeno una fase rifiutata
        /// </summary>
        Task<IEnumerable<ValutazioneLotto>> GetValutazioniConRifiutiAsync();

        /// <summary>
        /// Ottiene valutazioni incomplete (almeno una fase mancante)
        /// </summary>
        Task<IEnumerable<ValutazioneLotto>> GetValutazioniIncompleteAsync();

        // ===================================
        // VALIDAZIONI / ESISTENZA
        // ===================================

        /// <summary>
        /// Verifica se esiste già una valutazione per un lotto
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        /// <param name="excludeId">ID da escludere (per edit)</param>
        Task<bool> ExistsByLottoIdAsync(Guid lottoId, Guid? excludeId = null);

        // ===================================
        // STATISTICHE
        // ===================================

        /// <summary>
        /// Ottiene il conteggio delle valutazioni per stato
        /// </summary>
        /// <returns>
        /// Dictionary con chiavi:
        /// - TecnicaApprovata
        /// - TecnicaRifiutata
        /// - TecnicaPendente
        /// - EconomicaApprovata
        /// - EconomicaRifiutata
        /// - EconomicaPendente
        /// - CompleteApprovate
        /// - ConRifiuti
        /// </returns>
        Task<Dictionary<string, int>> GetCountByApprovazioniAsync();

        Task<int> CountByLottoIdAsync(Guid lottoId);
    }
}