using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;
using Trasformazioni.Models.ViewModels;

namespace Trasformazioni.Services.Interfaces
{
    /// <summary>
    /// Interfaccia per la business logic della gestione Lotti
    /// Include operazioni per il workflow complesso e gestione stati
    /// </summary>
    public interface ILottoService
    {
        // ===================================
        // QUERY - LETTURA
        // ===================================

        /// <summary>
        /// Ottiene tutti i lotti
        /// </summary>
        Task<IEnumerable<LottoListViewModel>> GetAllAsync();

        /// <summary>
        /// Ottiene il dettaglio di un lotto
        /// </summary>
        Task<LottoDetailsViewModel?> GetByIdAsync(Guid id);

        /// <summary>
        /// Recupera un lotto per la modifica
        /// </summary>
        Task<LottoEditViewModel?> GetForEditAsync(Guid id);

        /// <summary>
        /// Ottiene lotti di una specifica gara
        /// </summary>
        Task<IEnumerable<LottoListViewModel>> GetByGaraIdAsync(Guid garaId);

        /// <summary>
        /// Ottiene lotti filtrati per stato
        /// </summary>
        Task<IEnumerable<LottoListViewModel>> GetByStatoAsync(StatoLotto stato);

        /// <summary>
        /// Ottiene lotti assegnati a un operatore
        /// </summary>
        Task<IEnumerable<LottoListViewModel>> GetByOperatoreAsync(string operatoreId);

        /// <summary>
        /// Ottiene lotti in valutazione tecnica
        /// </summary>
        Task<IEnumerable<LottoListViewModel>> GetInValutazioneTecnicaAsync();

        /// <summary>
        /// Ottiene lotti in valutazione economica
        /// </summary>
        Task<IEnumerable<LottoListViewModel>> GetInValutazioneEconomicaAsync();

        /// <summary>
        /// Ottiene lotti in elaborazione
        /// </summary>
        Task<IEnumerable<LottoListViewModel>> GetInElaborazioneAsync();

        /// <summary>
        /// Ottiene lotti presentati
        /// </summary>
        Task<IEnumerable<LottoListViewModel>> GetPresentatiAsync();

        /// <summary>
        /// Ottiene lotti vinti
        /// </summary>
        Task<IEnumerable<LottoListViewModel>> GetVintiAsync();

        /// <summary>
        /// Ottiene lotti con richieste di integrazione aperte
        /// </summary>
        Task<IEnumerable<LottoListViewModel>> GetConIntegrazioniAperteAsync();

        /// <summary>
        /// Ottiene lotti con scadenza offerte imminente
        /// Considera la DataTerminePresentazioneOfferte della gara padre
        /// </summary>
        /// <param name="giorniProssimi">Numero di giorni entro cui cercare scadenze (default 7)</param>
        Task<IEnumerable<LottoListViewModel>> GetLottiInScadenzaAsync(int giorniProssimi = 7);

        /// <summary>
        /// Ottiene lotti paginati con filtri e ordinamento
        /// </summary>
        /// <param name="filters">Filtri e parametri di paginazione</param>
        /// <returns>Risultato paginato con ViewModels</returns>
        Task<PagedResult<LottoListViewModel>> GetPagedAsync(LottoFilterViewModel filters);

        // ===================================
        // COMANDI - SCRITTURA
        // ===================================

        /// <summary>
        /// Crea un nuovo lotto
        /// Gestisce validazioni business (unicità codice lotto per gara, campi obbligatori, ecc.)
        /// </summary>
        /// <returns>(Success, ErrorMessage, LottoId)</returns>
        Task<(bool Success, string? ErrorMessage, Guid? LottoId)> CreateAsync(LottoCreateViewModel model);

        /// <summary>
        /// Aggiorna un lotto esistente
        /// </summary>
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(LottoEditViewModel model);

        /// <summary>
        /// Elimina un lotto (soft delete)
        /// Verifica che non abbia preventivi associati
        /// </summary>
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(Guid id);

        // ===================================
        // OPERAZIONI BUSINESS SPECIFICHE - WORKFLOW
        // ===================================

        /// <summary>
        /// Assegna un operatore a un lotto
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        /// <param name="operatoreId">ID dell'operatore da assegnare</param>
        /// <returns>(Success, ErrorMessage)</returns>
        Task<(bool Success, string? ErrorMessage)> AssegnaOperatoreAsync(Guid lottoId, string operatoreId);

        /// <summary>
        /// Imposta la data di inizio esame ente per un lotto
        /// Il cambio di stato a InEsame avverrà automaticamente tramite background job
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        /// <param name="dataInizioEsame">Data di inizio esame</param>
        /// <returns>(Success, ErrorMessage)</returns>
        Task<(bool Success, string? ErrorMessage)> ImpostaDataInizioEsameAsync(Guid lottoId, DateTime dataInizioEsame);

        /// <summary>
        /// Rifiuta un lotto con motivazione
        /// Cambia lo stato a Rifiutato e salva il motivo
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        /// <param name="motivo">Motivo del rifiuto</param>
        /// <returns>(Success, ErrorMessage)</returns>
        Task<(bool Success, string? ErrorMessage)> RifiutaLottoAsync(Guid lottoId, string motivo);

        /// <summary>
        /// Cambia lo stato di un lotto
        /// Valida che la transizione di stato sia consentita
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        /// <param name="nuovoStato">Nuovo stato da applicare</param>
        /// <returns>(Success, ErrorMessage)</returns>
        Task<(bool Success, string? ErrorMessage)> CambiaStatoAsync(Guid lottoId, StatoLotto nuovoStato);

        // ===================================
        // VALIDAZIONI
        // ===================================

        /// <summary>
        /// Verifica se un codice lotto è unico all'interno di una gara
        /// </summary>
        Task<bool> IsCodiceLottoUniqueInGaraAsync(string codiceLotto, Guid garaId, Guid? excludeId = null);

        /// <summary>
        /// Verifica se la transizione di stato è valida
        /// </summary>
        Task<bool> IsTransizioneStatoValidaAsync(StatoLotto statoCorrente, StatoLotto nuovoStato);

        /// <summary>
        /// Verifica se esiste un lotto con l'ID specificato
        /// </summary>
        /// <param name="id">ID del lotto</param>
        Task<bool> ExistsAsync(Guid id);

        // ===================================
        // STATISTICHE
        // ===================================

        /// <summary>
        /// Ottiene il conteggio dei lotti per stato
        /// </summary>
        Task<Dictionary<StatoLotto, int>> GetCountByStatoAsync();

        /// <summary>
        /// Ottiene il conteggio dei lotti per stato per una gara specifica
        /// </summary>
        Task<Dictionary<StatoLotto, int>> GetCountByStatoForGaraAsync(Guid garaId);

        /// <summary>
        /// Ottiene l'importo totale dei lotti vinti
        /// </summary>
        Task<decimal> GetImportoTotaleVintiAsync();

        /// <summary>
        /// Ottiene il tasso di successo (percentuale lotti vinti)
        /// </summary>
        Task<decimal> GetTassoSuccessoAsync();

        // ===================================
        // METODI AUSILIARI - Fase 1 - Modulo LOTTI
        // ===================================

        Task<bool> UpdateStatoAsync(Guid lottoId, StatoLotto nuovoStato, string userId);
        Task<int> GetNumeroPartecipantiAsync(Guid lottoId);
        Task<int> GetNumeroValutazioniAsync(Guid lottoId);
        Task<int> GetNumeroElaborazioniAsync(Guid lottoId);
        Task<bool> CanDeleteAsync(Guid lottoId);
    }
}