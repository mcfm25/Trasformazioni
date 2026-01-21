using Trasformazioni.Models.Entities;
using Trasformazioni.Models.ViewModels;

namespace Trasformazioni.Services.Interfaces
{
    /// <summary>
    /// Interfaccia per la business logic della gestione Richieste Integrazione
    /// Gestisce il ping-pong di richieste/risposte con l'ente
    /// </summary>
    public interface IRichiestaIntegrazioneService
    {
        // ===================================
        // QUERY - LETTURA
        // ===================================

        /// <summary>
        /// Ottiene tutte le richieste di integrazione
        /// </summary>
        Task<IEnumerable<RichiestaIntegrazioneListViewModel>> GetAllAsync();

        /// <summary>
        /// Ottiene il dettaglio di una richiesta di integrazione
        /// </summary>
        Task<RichiestaIntegrazioneDetailsViewModel?> GetByIdAsync(Guid id);

        /// <summary>
        /// Ottiene richieste di integrazione di un lotto specifico
        /// </summary>
        Task<IEnumerable<RichiestaIntegrazioneListViewModel>> GetByLottoIdAsync(Guid lottoId);

        /// <summary>
        /// Ottiene richieste di integrazione aperte (non chiuse)
        /// </summary>
        Task<IEnumerable<RichiestaIntegrazioneListViewModel>> GetAperteAsync();

        /// <summary>
        /// Ottiene richieste di integrazione chiuse
        /// </summary>
        Task<IEnumerable<RichiestaIntegrazioneListViewModel>> GetChiuseAsync();

        /// <summary>
        /// Ottiene richieste non ancora risposte (DataRispostaAzienda = null)
        /// PRIORITÀ MASSIMA per dashboard
        /// </summary>
        Task<IEnumerable<RichiestaIntegrazioneListViewModel>> GetNonRisposteAsync();

        /// <summary>
        /// Ottiene richieste risposte ma non ancora chiuse
        /// </summary>
        Task<IEnumerable<RichiestaIntegrazioneListViewModel>> GetRisposteNonChiuseAsync();

        /// <summary>
        /// Ottiene richieste scadute (oltre X giorni senza risposta)
        /// </summary>
        /// <param name="giorniScadenza">Numero di giorni oltre i quali una richiesta è considerata scaduta</param>
        Task<IEnumerable<RichiestaIntegrazioneListViewModel>> GetScaduteAsync(int giorniScadenza = 7);

        /// <summary>
        /// Ottiene richieste di integrazione paginate con filtri e ordinamento
        /// </summary>
        /// <param name="filters">Filtri e parametri di paginazione</param>
        /// <returns>Risultato paginato con ViewModels</returns>
        Task<PagedResult<RichiestaIntegrazioneListViewModel>> GetPagedAsync(RichiestaIntegrazioneFilterViewModel filters);

        // ===================================
        // COMANDI - SCRITTURA
        // ===================================

        /// <summary>
        /// Crea una nuova richiesta di integrazione
        /// Gestisce validazioni business (esistenza lotto, numero progressivo automatico, ecc.)
        /// </summary>
        /// <returns>(Success, ErrorMessage, RichiestaId)</returns>
        Task<(bool Success, string? ErrorMessage, Guid? RichiestaId)> CreateAsync(
            RichiestaIntegrazioneCreateViewModel model,
            string? documentoRichiestaPath = null);

        /// <summary>
        /// Aggiorna una richiesta di integrazione esistente
        /// </summary>
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(
            RichiestaIntegrazioneEditViewModel model,
            string? nuovoDocumentoRichiestaPath = null,
            string? nuovoDocumentoRispostaPath = null);

        /// <summary>
        /// Elimina una richiesta di integrazione (soft delete)
        /// Verifica che non sia già stata risposta
        /// </summary>
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(Guid id);

        // ===================================
        // OPERAZIONI BUSINESS SPECIFICHE
        // ===================================

        /// <summary>
        /// Aggiungi risposta dell'azienda a una richiesta
        /// </summary>
        /// <param name="richiestaId">ID della richiesta</param>
        /// <param name="testoRisposta">Testo della risposta</param>
        /// <param name="dataRisposta">Data risposta</param>
        /// <param name="documentoRispostaPath">Path del documento di risposta (opzionale)</param>
        /// <returns>(Success, ErrorMessage)</returns>
        Task<(bool Success, string? ErrorMessage)> RispondiAsync(
            Guid richiestaId,
            string testoRisposta,
            DateTime dataRisposta,
            string? documentoRispostaPath = null);

        /// <summary>
        /// Chiudi una richiesta di integrazione
        /// Può essere chiamato solo dopo che è stata data risposta
        /// </summary>
        /// <param name="richiestaId">ID della richiesta</param>
        /// <returns>(Success, ErrorMessage)</returns>
        Task<(bool Success, string? ErrorMessage)> ChiudiAsync(Guid richiestaId);

        /// <summary>
        /// Riapri una richiesta di integrazione precedentemente chiusa
        /// </summary>
        /// <param name="richiestaId">ID della richiesta</param>
        /// <returns>(Success, ErrorMessage)</returns>
        Task<(bool Success, string? ErrorMessage)> RiapriAsync(Guid richiestaId);

        // ===================================
        // VALIDAZIONI
        // ===================================

        /// <summary>
        /// Verifica se le date sono valide
        /// </summary>
        Task<bool> ValidaDateAsync(DateTime dataRichiesta, DateTime? dataRisposta);

        /// <summary>
        /// Verifica se una richiesta può essere chiusa
        /// </summary>
        Task<bool> CanChiudiAsync(Guid richiestaId);

        /// <summary>
        /// Verifica se tutte le richieste di un lotto sono chiuse
        /// Usato per decidere il cambio stato automatico RichiestaIntegrazione → InEsame
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        /// <returns>True se tutte le richieste sono chiuse (o non ce ne sono), False altrimenti</returns>
        Task<bool> AreAllRequestsClosedAsync(Guid lottoId);

        // ===================================
        // STATISTICHE
        // ===================================

        /// <summary>
        /// Ottiene il conteggio delle richieste aperte
        /// </summary>
        Task<int> GetCountAperteAsync();

        /// <summary>
        /// Ottiene il conteggio delle richieste non risposte
        /// </summary>
        Task<int> GetCountNonRisposteAsync();

        /// <summary>
        /// Ottiene il conteggio delle richieste per lotto
        /// </summary>
        Task<int> GetCountByLottoIdAsync(Guid lottoId);

        /// <summary>
        /// Ottiene il tempo medio di risposta (in giorni)
        /// </summary>
        Task<double?> GetTempoMedioRispostaAsync();
    }
}