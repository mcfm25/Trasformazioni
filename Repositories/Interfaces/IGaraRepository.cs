using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;
using Trasformazioni.Models.ViewModels;

namespace Trasformazioni.Data.Repositories
{
    /// <summary>
    /// Interfaccia per il repository delle Gare
    /// Definisce le operazioni di accesso ai dati per l'entità Gara
    /// </summary>
    public interface IGaraRepository
    {
        // ===================================
        // OPERAZIONI BASE (CRUD)
        // ===================================

        /// <summary>
        /// Ottiene tutte le gare
        /// </summary>
        Task<IEnumerable<Gara>> GetAllAsync();

        /// <summary>
        /// Ottiene una gara per ID
        /// </summary>
        /// <param name="id">ID della gara</param>
        Task<Gara?> GetByIdAsync(Guid id);

        /// <summary>
        /// Aggiunge una nuova gara
        /// </summary>
        /// <param name="gara">Entità gara da aggiungere</param>
        Task<Gara> AddAsync(Gara gara);

        /// <summary>
        /// Aggiorna una gara esistente
        /// </summary>
        /// <param name="gara">Entità gara da aggiornare</param>
        Task UpdateAsync(Gara gara);

        /// <summary>
        /// Elimina una gara (soft delete)
        /// </summary>
        /// <param name="id">ID della gara</param>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Verifica se esiste una gara con l'ID specificato
        /// </summary>
        /// <param name="id">ID della gara</param>
        Task<bool> ExistsAsync(Guid id);

        // ===================================
        // OPERAZIONI CON RELAZIONI
        // ===================================

        /// <summary>
        /// Ottiene una gara con i lotti inclusi
        /// </summary>
        /// <param name="id">ID della gara</param>
        Task<Gara?> GetWithLottiAsync(Guid id);

        /// <summary>
        /// Ottiene una gara con lotti e documenti inclusi
        /// </summary>
        /// <param name="id">ID della gara</param>
        Task<Gara?> GetWithLottiAndDocumentiAsync(Guid id);

        /// <summary>
        /// Ottiene una gara con tutte le relazioni (lotti, documenti, utente chiusura)
        /// </summary>
        /// <param name="id">ID della gara</param>
        Task<Gara?> GetCompleteAsync(Guid id);

        Task<Gara?> GetWithDocumentiRichiestiAsync(Guid id);

        Task UpdateDocumentiRichiestiAsync(Guid garaId, List<Guid> nuoviTipoDocumentoIds);

        // ===================================
        // RICERCHE SPECIFICHE
        // ===================================

        /// <summary>
        /// Ottiene una gara per codice
        /// </summary>
        /// <param name="codiceGara">Codice identificativo della gara</param>
        Task<Gara?> GetByCodiceAsync(string codiceGara);

        /// <summary>
        /// Ottiene una gara per CIG
        /// </summary>
        /// <param name="cig">Codice CIG</param>
        Task<Gara?> GetByCIGAsync(string cig);

        /// <summary>
        /// Ottiene gare per stato
        /// </summary>
        /// <param name="stato">Stato della gara</param>
        Task<IEnumerable<Gara>> GetByStatoAsync(StatoGara stato);

        /// <summary>
        /// Ottiene gare per tipologia
        /// </summary>
        /// <param name="tipologia">Tipologia della gara</param>
        Task<IEnumerable<Gara>> GetByTipologiaAsync(TipologiaGara tipologia);

        /// <summary>
        /// Ottiene gare per regione
        /// </summary>
        /// <param name="regione">Regione dell'ente appaltante</param>
        Task<IEnumerable<Gara>> GetByRegioneAsync(string regione);

        /// <summary>
        /// Ottiene gare attive (non chiuse manualmente)
        /// </summary>
        Task<IEnumerable<Gara>> GetActiveGareAsync();

        /// <summary>
        /// Ottiene gare con scadenza offerte imminente
        /// </summary>
        /// <param name="giorniProssimi">Numero di giorni per considerare la scadenza imminente</param>
        Task<IEnumerable<Gara>> GetGareInScadenzaAsync(int giorniProssimi = 7);

        /// <summary>
        /// Cerca gare per testo (titolo, descrizione, codice, ente)
        /// </summary>
        /// <param name="searchTerm">Termine di ricerca</param>
        Task<IEnumerable<Gara>> SearchAsync(string searchTerm);

        // ===================================
        // PAGINAZIONE E FILTRI
        // ===================================

        /// <summary>
        /// Ottiene gare paginate con filtri e ordinamento
        /// </summary>
        /// <param name="filters">Filtri e parametri di paginazione</param>
        /// <returns>Tupla con (Items, TotalCount)</returns>
        Task<(IEnumerable<Gara> Items, int TotalCount)> GetPagedAsync(GaraFilterViewModel filters);

        // ===================================
        // VALIDAZIONI / ESISTENZA
        // ===================================

        /// <summary>
        /// Verifica se esiste una gara con il codice specificato
        /// </summary>
        /// <param name="codiceGara">Codice della gara</param>
        /// <param name="excludeId">ID da escludere (per edit)</param>
        Task<bool> ExistsByCodiceAsync(string codiceGara, Guid? excludeId = null);

        /// <summary>
        /// Verifica se esiste una gara con il CIG specificato
        /// </summary>
        /// <param name="cig">Codice CIG</param>
        /// <param name="excludeId">ID da escludere (per edit)</param>
        Task<bool> ExistsByCIGAsync(string cig, Guid? excludeId = null);

        // ===================================
        // STATISTICHE E REPORT
        // ===================================

        /// <summary>
        /// Ottiene il conteggio delle gare per stato
        /// </summary>
        Task<Dictionary<StatoGara, int>> GetCountByStatoAsync();

        /// <summary>
        /// Ottiene il conteggio delle gare per regione
        /// </summary>
        Task<Dictionary<string, int>> GetCountByRegioneAsync();

        /// <summary>
        /// Ottiene l'importo totale delle gare per stato
        /// </summary>
        Task<Dictionary<StatoGara, decimal>> GetImportoTotaleByStatoAsync();
    }
}