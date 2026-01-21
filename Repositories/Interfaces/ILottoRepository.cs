using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;
using Trasformazioni.Models.ViewModels;

namespace Trasformazioni.Data.Repositories
{
    /// <summary>
    /// Interfaccia per il repository dei Lotti
    /// Definisce le operazioni di accesso ai dati per l'entità Lotto
    /// Include operazioni specifiche per il workflow complesso
    /// </summary>
    public interface ILottoRepository
    {
        // ===================================
        // OPERAZIONI BASE (CRUD)
        // ===================================

        /// <summary>
        /// Ottiene tutti i lotti
        /// </summary>
        Task<IEnumerable<Lotto>> GetAllAsync();

        /// <summary>
        /// Ottiene un lotto per ID
        /// </summary>
        /// <param name="id">ID del lotto</param>
        Task<Lotto?> GetByIdAsync(Guid id);

        /// <summary>
        /// Recupera un lotto con i documenti richiesti (checklist)
        /// </summary>
        Task<Lotto?> GetWithDocumentiRichiestiAsync(Guid id);

        /// <summary>
        /// Aggiunge un nuovo lotto
        /// </summary>
        /// <param name="lotto">Entità lotto da aggiungere</param>
        Task<Lotto> AddAsync(Lotto lotto);

        /// <summary>
        /// Aggiorna un lotto esistente
        /// </summary>
        /// <param name="lotto">Entità lotto da aggiornare</param>
        Task UpdateAsync(Lotto lotto);

        /// <summary>
        /// Elimina un lotto (soft delete)
        /// </summary>
        /// <param name="id">ID del lotto</param>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Verifica se esiste un lotto con l'ID specificato
        /// </summary>
        /// <param name="id">ID del lotto</param>
        Task<bool> ExistsAsync(Guid id);

        Task UpdateDocumentiRichiestiAsync(Guid lottoId, List<Guid> nuoviTipoDocumentoIds);

        // ===================================
        // OPERAZIONI CON RELAZIONI
        // ===================================

        /// <summary>
        /// Ottiene un lotto con la gara inclusa
        /// </summary>
        /// <param name="id">ID del lotto</param>
        Task<Lotto?> GetWithGaraAsync(Guid id);

        /// <summary>
        /// Ottiene un lotto con valutazione e elaborazione incluse
        /// </summary>
        /// <param name="id">ID del lotto</param>
        Task<Lotto?> GetWithValutazioneElaborazioneAsync(Guid id);

        /// <summary>
        /// Ottiene un lotto con tutti i preventivi
        /// </summary>
        /// <param name="id">ID del lotto</param>
        Task<Lotto?> GetWithPreventiviAsync(Guid id);

        /// <summary>
        /// Ottiene un lotto con richieste di integrazione
        /// </summary>
        /// <param name="id">ID del lotto</param>
        Task<Lotto?> GetWithIntegrazioniAsync(Guid id);

        /// <summary>
        /// Ottiene un lotto con partecipanti
        /// </summary>
        /// <param name="id">ID del lotto</param>
        Task<Lotto?> GetWithPartecipantiAsync(Guid id);

        /// <summary>
        /// Ottiene un lotto con documenti
        /// </summary>
        /// <param name="id">ID del lotto</param>
        Task<Lotto?> GetWithDocumentiAsync(Guid id);

        /// <summary>
        /// Ottiene un lotto con tutte le relazioni (completo per workflow)
        /// </summary>
        /// <param name="id">ID del lotto</param>
        Task<Lotto?> GetCompleteAsync(Guid id);

        // ===================================
        // RICERCHE PER GARA
        // ===================================

        /// <summary>
        /// Ottiene tutti i lotti di una gara
        /// </summary>
        /// <param name="garaId">ID della gara</param>
        Task<IEnumerable<Lotto>> GetByGaraIdAsync(Guid garaId);

        /// <summary>
        /// Ottiene i lotti di una gara con tutte le relazioni
        /// </summary>
        /// <param name="garaId">ID della gara</param>
        Task<IEnumerable<Lotto>> GetByGaraIdCompleteAsync(Guid garaId);

        // ===================================
        // RICERCHE PER STATO (WORKFLOW)
        // ===================================

        /// <summary>
        /// Ottiene lotti per stato
        /// </summary>
        /// <param name="stato">Stato del lotto</param>
        Task<IEnumerable<Lotto>> GetByStatoAsync(StatoLotto stato);

        /// <summary>
        /// Ottiene lotti in valutazione tecnica
        /// </summary>
        Task<IEnumerable<Lotto>> GetInValutazioneTecnicaAsync();

        /// <summary>
        /// Ottiene lotti in valutazione economica
        /// </summary>
        Task<IEnumerable<Lotto>> GetInValutazioneEconomicaAsync();

        /// <summary>
        /// Ottiene lotti in elaborazione
        /// </summary>
        Task<IEnumerable<Lotto>> GetInElaborazioneAsync();

        /// <summary>
        /// Ottiene lotti presentati (in attesa esame ente)
        /// </summary>
        Task<IEnumerable<Lotto>> GetPresentatiAsync();

        /// <summary>
        /// Ottiene lotti in esame dall'ente
        /// </summary>
        Task<IEnumerable<Lotto>> GetInEsameAsync();

        /// <summary>
        /// Ottiene lotti con richieste di integrazione aperte
        /// </summary>
        Task<IEnumerable<Lotto>> GetConIntegrazioniAperteAsync();

        /// <summary>
        /// Ottiene lotti vinti
        /// </summary>
        Task<IEnumerable<Lotto>> GetVintiAsync();

        /// <summary>
        /// Ottiene lotti persi
        /// </summary>
        Task<IEnumerable<Lotto>> GetPersiAsync();

        // ===================================
        // RICERCHE PER OPERATORE
        // ===================================

        /// <summary>
        /// Ottiene lotti assegnati a un operatore
        /// </summary>
        /// <param name="operatoreId">ID dell'operatore</param>
        Task<IEnumerable<Lotto>> GetByOperatoreAsync(string operatoreId);

        /// <summary>
        /// Ottiene lotti assegnati a un operatore per stato
        /// </summary>
        /// <param name="operatoreId">ID dell'operatore</param>
        /// <param name="stato">Stato del lotto</param>
        Task<IEnumerable<Lotto>> GetByOperatoreAndStatoAsync(string operatoreId, StatoLotto stato);

        // ===================================
        // RICERCHE SPECIFICHE
        // ===================================

        /// <summary>
        /// Ottiene un lotto per codice lotto all'interno di una gara
        /// </summary>
        /// <param name="garaId">ID della gara</param>
        /// <param name="codiceLotto">Codice del lotto</param>
        Task<Lotto?> GetByGaraAndCodiceAsync(Guid garaId, string codiceLotto);

        /// <summary>
        /// Ottiene lotti per tipologia
        /// </summary>
        /// <param name="tipologia">Tipologia del lotto</param>
        Task<IEnumerable<Lotto>> GetByTipologiaAsync(TipologiaLotto tipologia);

        /// <summary>
        /// Cerca lotti per testo (codice, descrizione)
        /// </summary>
        /// <param name="searchTerm">Termine di ricerca</param>
        Task<IEnumerable<Lotto>> SearchAsync(string searchTerm);

        /// <summary>
        /// Verifica se una gara ha almeno un lotto associato
        /// </summary>
        /// <param name="garaId">ID della gara</param>
        Task<bool> HasLottiByGaraIdAsync(Guid garaId);

        /// <summary>
        /// Ottiene i lotti non in stato terminale di una gara
        /// Lotti terminali: Vinto, Perso, Scartato, Rifiutato
        /// </summary>
        /// <param name="garaId">ID della gara</param>
        Task<IEnumerable<Lotto>> GetLottiNonTerminaliByGaraIdAsync(Guid garaId);

        // ===================================
        // PAGINAZIONE E FILTRI
        // ===================================

        /// <summary>
        /// Ottiene lotti paginati con filtri e ordinamento
        /// </summary>
        /// <param name="filters">Filtri e parametri di paginazione</param>
        /// <returns>Tupla con (Items, TotalCount)</returns>
        Task<(IEnumerable<Lotto> Items, int TotalCount)> GetPagedAsync(LottoFilterViewModel filters);

        // ===================================
        // VALIDAZIONI / ESISTENZA
        // ===================================

        /// <summary>
        /// Ottiene lotti con scadenza offerte imminente
        /// Considera la DataTerminePresentazioneOfferte della gara associata
        /// </summary>
        /// <param name="giorniProssimi">Numero di giorni entro cui cercare scadenze</param>
        Task<IEnumerable<Lotto>> GetLottiInScadenzaAsync(int giorniProssimi = 7);

        /// <summary>
        /// Verifica se esiste un lotto con il codice specificato in una gara
        /// </summary>
        /// <param name="garaId">ID della gara</param>
        /// <param name="codiceLotto">Codice del lotto</param>
        /// <param name="excludeId">ID da escludere (per edit)</param>
        Task<bool> ExistsByGaraAndCodiceAsync(Guid garaId, string codiceLotto, Guid? excludeId = null);

        // ===================================
        // STATISTICHE E REPORT
        // ===================================

        /// <summary>
        /// Ottiene il conteggio dei lotti per stato
        /// </summary>
        Task<Dictionary<StatoLotto, int>> GetCountByStatoAsync();

        /// <summary>
        /// Ottiene il conteggio dei lotti per gara
        /// </summary>
        /// <param name="garaId">ID della gara</param>
        Task<Dictionary<StatoLotto, int>> GetCountByStatoForGaraAsync(Guid garaId);

        /// <summary>
        /// Ottiene il conteggio dei lotti per operatore
        /// </summary>
        /// <param name="operatoreId">ID dell'operatore</param>
        Task<Dictionary<StatoLotto, int>> GetCountByStatoForOperatoreAsync(string operatoreId);

        /// <summary>
        /// Ottiene l'importo totale dei lotti vinti
        /// </summary>
        Task<decimal> GetImportoTotaleVintiAsync();

        /// <summary>
        /// Ottiene il tasso di successo (percentuale lotti vinti)
        /// </summary>
        Task<decimal> GetTassoSuccessoAsync();

        Task<int> CountByGaraIdAsync(Guid garaId);

    }
}