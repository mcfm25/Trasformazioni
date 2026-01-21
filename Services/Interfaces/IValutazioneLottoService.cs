using Trasformazioni.Models.ViewModels;

namespace Trasformazioni.Services.Interfaces
{
    /// <summary>
    /// Interfaccia per la business logic della gestione Valutazioni Lotto
    /// Gestisce sia la valutazione tecnica che economica
    /// Le due fasi sono separate ma gestite nella stessa entità
    /// </summary>
    public interface IValutazioneLottoService
    {
        // ===================================
        // QUERY - LETTURA
        // ===================================

        /// <summary>
        /// Ottiene tutte le valutazioni
        /// </summary>
        Task<IEnumerable<ValutazioneLottoListViewModel>> GetAllAsync();

        /// <summary>
        /// Ottiene il dettaglio di una valutazione
        /// </summary>
        Task<ValutazioneLottoDetailsViewModel?> GetByIdAsync(Guid id);

        /// <summary>
        /// Ottiene la valutazione di un lotto specifico
        /// Un lotto può avere al massimo una valutazione
        /// </summary>
        Task<ValutazioneLottoDetailsViewModel?> GetByLottoIdAsync(Guid lottoId);

        // ===================================
        // QUERY - VALUTAZIONI TECNICHE
        // ===================================

        /// <summary>
        /// Ottiene valutazioni con valutazione tecnica approvata
        /// </summary>
        Task<IEnumerable<ValutazioneLottoListViewModel>> GetValutazioniTecnicheApprovateAsync();

        /// <summary>
        /// Ottiene valutazioni con valutazione tecnica rifiutata
        /// </summary>
        Task<IEnumerable<ValutazioneLottoListViewModel>> GetValutazioniTecnicheRifiutateAsync();

        /// <summary>
        /// Ottiene valutazioni con valutazione tecnica pendente (non ancora effettuata)
        /// </summary>
        Task<IEnumerable<ValutazioneLottoListViewModel>> GetValutazioniTecnichePendentiAsync();

        // ===================================
        // QUERY - VALUTAZIONI ECONOMICHE
        // ===================================

        /// <summary>
        /// Ottiene valutazioni con valutazione economica approvata
        /// </summary>
        Task<IEnumerable<ValutazioneLottoListViewModel>> GetValutazioniEconomicheApprovateAsync();

        /// <summary>
        /// Ottiene valutazioni con valutazione economica rifiutata
        /// </summary>
        Task<IEnumerable<ValutazioneLottoListViewModel>> GetValutazioniEconomicheRifiutateAsync();

        /// <summary>
        /// Ottiene valutazioni con valutazione economica pendente (non ancora effettuata)
        /// </summary>
        Task<IEnumerable<ValutazioneLottoListViewModel>> GetValutazioniEconomichePendentiAsync();

        // ===================================
        // QUERY - VALUTAZIONI COMBINATE
        // ===================================

        /// <summary>
        /// Ottiene valutazioni con entrambe le fasi approvate
        /// </summary>
        Task<IEnumerable<ValutazioneLottoListViewModel>> GetValutazioniCompleteApprovateAsync();

        /// <summary>
        /// Ottiene valutazioni con almeno una fase rifiutata
        /// </summary>
        Task<IEnumerable<ValutazioneLottoListViewModel>> GetValutazioniConRifiutiAsync();

        /// <summary>
        /// Ottiene valutazioni incomplete (almeno una fase mancante)
        /// </summary>
        Task<IEnumerable<ValutazioneLottoListViewModel>> GetValutazioniIncompleteAsync();

        // ===================================
        // QUERY - PER VALUTATORE
        // ===================================

        /// <summary>
        /// Ottiene tutte le valutazioni tecniche effettuate da un valutatore
        /// </summary>
        Task<IEnumerable<ValutazioneLottoListViewModel>> GetByValutatoreTecnicoAsync(string valutatoreTecnicoId);

        /// <summary>
        /// Ottiene tutte le valutazioni economiche effettuate da un valutatore
        /// </summary>
        Task<IEnumerable<ValutazioneLottoListViewModel>> GetByValutatoreEconomicoAsync(string valutatoreEconomicoId);

        // ===================================
        // COMANDI - VALUTAZIONE TECNICA
        // ===================================

        /// <summary>
        /// Effettua la valutazione tecnica di un lotto
        /// Se la valutazione non esiste, la crea
        /// Se esiste già, la aggiorna (solo parte tecnica)
        /// </summary>
        /// <param name="model">Dati della valutazione tecnica</param>
        /// <param name="currentUserId">ID utente che effettua l'operazione</param>
        /// <returns>(Success, ErrorMessage, ValutazioneId)</returns>
        Task<(bool Success, string? ErrorMessage, Guid? ValutazioneId)> ValutaTecnicamenteAsync(
            ValutazioneTecnicaViewModel model,
            string currentUserId);

        // ===================================
        // COMANDI - VALUTAZIONE ECONOMICA
        // ===================================

        /// <summary>
        /// Effettua la valutazione economica di un lotto
        /// PREREQUISITO: Valutazione tecnica deve essere approvata
        /// Aggiorna la valutazione esistente con i dati economici
        /// </summary>
        /// <param name="model">Dati della valutazione economica</param>
        /// <param name="currentUserId">ID utente che effettua l'operazione</param>
        /// <returns>(Success, ErrorMessage, ValutazioneId)</returns>
        Task<(bool Success, string? ErrorMessage, Guid? ValutazioneId)> ValutaEconomicamenteAsync(
            ValutazioneEconomicaViewModel model,
            string currentUserId);

        // ===================================
        // COMANDI - ELIMINAZIONE
        // ===================================

        /// <summary>
        /// Elimina una valutazione (soft delete)
        /// </summary>
        /// <param name="id">ID della valutazione</param>
        /// <param name="currentUserId">ID utente che effettua l'operazione</param>
        /// <returns>(Success, ErrorMessage)</returns>
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(Guid id, string currentUserId);

        // ===================================
        // VALIDAZIONI
        // ===================================

        /// <summary>
        /// Verifica se esiste già una valutazione per un lotto
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        /// <param name="excludeId">ID valutazione da escludere (per edit)</param>
        Task<bool> ExistsByLottoIdAsync(Guid lottoId, Guid? excludeId = null);

        /// <summary>
        /// Verifica se un lotto può essere valutato economicamente
        /// Prerequisito: valutazione tecnica deve essere approvata
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        Task<bool> CanValutareEconomicamenteAsync(Guid lottoId);

        /// <summary>
        /// Verifica se una valutazione può essere eliminata
        /// </summary>
        /// <param name="id">ID della valutazione</param>
        Task<(bool CanDelete, string? Reason)> CanDeleteAsync(Guid id);

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
        /// - Incomplete
        /// </returns>
        Task<Dictionary<string, int>> GetStatisticheApprovazioniAsync();
    }
}