using Trasformazioni.Models.ViewModels;

namespace Trasformazioni.Services.Interfaces
{
    /// <summary>
    /// Interface per il service dei Partecipanti Lotto
    /// Gestisce la logica di business per il censimento partecipanti con validazione aggiudicatario unico
    /// </summary>
    public interface IPartecipanteLottoService
    {
        // ===================================
        // QUERY - LETTURA
        // ===================================

        /// <summary>
        /// Ottiene tutti i partecipanti
        /// </summary>
        Task<IEnumerable<PartecipanteLottoListViewModel>> GetAllAsync();

        /// <summary>
        /// Ottiene un partecipante per ID con tutte le relazioni
        /// </summary>
        /// <param name="id">ID del partecipante</param>
        Task<PartecipanteLottoDetailsViewModel?> GetByIdAsync(Guid id);

        /// <summary>
        /// Ottiene un partecipante per modifica (EditViewModel)
        /// </summary>
        /// <param name="id">ID del partecipante</param>
        Task<PartecipanteLottoEditViewModel?> GetForEditAsync(Guid id);

        // ===================================
        // COMANDI - SCRITTURA
        // ===================================

        /// <summary>
        /// Crea un nuovo partecipante
        /// Validazioni:
        /// - Il lotto deve esistere
        /// - Se SoggettoId specificato, il soggetto deve esistere
        /// - Se SoggettoId null → RagioneSociale obbligatoria
        /// - IsAggiudicatario e IsScartatoDallEnte non possono essere entrambi true
        /// - Se IsAggiudicatario = true → verifica che non ci sia già un aggiudicatario per il lotto
        /// </summary>
        /// <param name="model">Dati per la creazione</param>
        /// <param name="currentUserId">ID dell'utente corrente (per audit)</param>
        /// <returns>Tupla (Success, ErrorMessage, Id)</returns>
        Task<(bool Success, string? ErrorMessage, Guid? Id)> CreateAsync(
            PartecipanteLottoCreateViewModel model,
            string currentUserId
        );

        /// <summary>
        /// Aggiorna un partecipante esistente
        /// Validazioni:
        /// - Il partecipante deve esistere
        /// - Se SoggettoId specificato, il soggetto deve esistere
        /// - Se SoggettoId null → RagioneSociale obbligatoria
        /// - IsAggiudicatario e IsScartatoDallEnte non possono essere entrambi true
        /// - Se IsAggiudicatario = true → verifica che non ci sia già un altro aggiudicatario
        /// </summary>
        /// <param name="model">Dati per l'aggiornamento</param>
        /// <param name="currentUserId">ID dell'utente corrente (per audit)</param>
        /// <returns>Tupla (Success, ErrorMessage)</returns>
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(
            PartecipanteLottoEditViewModel model,
            string currentUserId
        );

        /// <summary>
        /// Elimina un partecipante (soft delete)
        /// </summary>
        /// <param name="id">ID del partecipante</param>
        /// <param name="currentUserId">ID dell'utente corrente (per audit)</param>
        /// <returns>Tupla (Success, ErrorMessage)</returns>
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(Guid id, string currentUserId);

        // ===================================
        // GESTIONE AGGIUDICATARIO
        // ===================================

        /// <summary>
        /// Imposta un partecipante come aggiudicatario del lotto
        /// Rimuove il flag IsAggiudicatario da eventuali altri partecipanti dello stesso lotto
        /// Validazioni:
        /// - Il partecipante deve esistere
        /// - Il partecipante non deve essere scartato
        /// </summary>
        /// <param name="id">ID del partecipante da impostare come aggiudicatario</param>
        /// <param name="currentUserId">ID dell'utente corrente (per audit)</param>
        /// <returns>Tupla (Success, ErrorMessage)</returns>
        Task<(bool Success, string? ErrorMessage)> ImpostaAggiudicatarioAsync(Guid id, string currentUserId);

        /// <summary>
        /// Ottiene l'aggiudicatario di un lotto (se presente)
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        Task<PartecipanteLottoDetailsViewModel?> GetAggiudicatarioByLottoIdAsync(Guid lottoId);

        // ===================================
        // QUERY SPECIFICHE - PER LOTTO
        // ===================================

        /// <summary>
        /// Ottiene tutti i partecipanti di un lotto
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        Task<IEnumerable<PartecipanteLottoListViewModel>> GetByLottoIdAsync(Guid lottoId);

        /// <summary>
        /// Ottiene i partecipanti scartati di un lotto
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        Task<IEnumerable<PartecipanteLottoListViewModel>> GetScartatiByLottoIdAsync(Guid lottoId);

        /// <summary>
        /// Ottiene i partecipanti non aggiudicatari di un lotto
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        Task<IEnumerable<PartecipanteLottoListViewModel>> GetNonAggiudicatariByLottoIdAsync(Guid lottoId);

        /// <summary>
        /// Ottiene i partecipanti ordinati per offerta economica (dal più basso)
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        Task<IEnumerable<PartecipanteLottoListViewModel>> GetByLottoIdOrderedByOffertaAsync(Guid lottoId);

        // ===================================
        // QUERY SPECIFICHE - PER SOGGETTO
        // ===================================

        /// <summary>
        /// Ottiene tutte le partecipazioni di un soggetto
        /// </summary>
        /// <param name="soggettoId">ID del soggetto</param>
        Task<IEnumerable<PartecipanteLottoListViewModel>> GetBySoggettoIdAsync(Guid soggettoId);

        /// <summary>
        /// Ottiene le gare vinte da un soggetto
        /// </summary>
        /// <param name="soggettoId">ID del soggetto</param>
        Task<IEnumerable<PartecipanteLottoListViewModel>> GetVinteBySoggettoIdAsync(Guid soggettoId);

        /// <summary>
        /// Ottiene le gare in cui un soggetto è stato scartato
        /// </summary>
        /// <param name="soggettoId">ID del soggetto</param>
        Task<IEnumerable<PartecipanteLottoListViewModel>> GetScartateBySoggettoIdAsync(Guid soggettoId);

        // ===================================
        // QUERY SPECIFICHE - PER STATO
        // ===================================

        /// <summary>
        /// Ottiene tutti gli aggiudicatari
        /// </summary>
        Task<IEnumerable<PartecipanteLottoListViewModel>> GetAggiudicatariAsync();

        /// <summary>
        /// Ottiene tutti i partecipanti scartati
        /// </summary>
        Task<IEnumerable<PartecipanteLottoListViewModel>> GetScartatiAsync();

        /// <summary>
        /// Ottiene i partecipanti senza offerta economica
        /// </summary>
        Task<IEnumerable<PartecipanteLottoListViewModel>> GetSenzaOffertaAsync();

        // ===================================
        // VALIDAZIONI
        // ===================================

        /// <summary>
        /// Verifica se un lotto ha già un aggiudicatario
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        /// <param name="excludeId">ID da escludere (per edit)</param>
        Task<bool> LottoHasAggiudicatarioAsync(Guid lottoId, Guid? excludeId = null);

        /// <summary>
        /// Verifica se esiste un partecipante con l'ID specificato
        /// </summary>
        /// <param name="id">ID del partecipante</param>
        Task<bool> ExistsAsync(Guid id);

        // ===================================
        // STATISTICHE
        // ===================================

        /// <summary>
        /// Ottiene il conteggio dei partecipanti per un lotto
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        Task<int> GetCountPartecipantiByLottoAsync(Guid lottoId);

        /// <summary>
        /// Ottiene statistiche globali sui partecipanti
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
        Task<Dictionary<string, int>> GetStatistichePartecipazioniAsync();

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
    }
}