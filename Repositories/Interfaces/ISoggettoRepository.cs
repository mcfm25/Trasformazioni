using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;
using Trasformazioni.Models.ViewModels;

namespace Trasformazioni.Data.Repositories
{
    /// <summary>
    /// Interfaccia per l'accesso ai dati dei soggetti (clienti e fornitori)
    /// </summary>
    public interface ISoggettoRepository
    {
        // ===================================
        // OPERAZIONI BASE
        // ===================================

        /// <summary>
        /// Ottiene tutti i soggetti non cancellati
        /// </summary>
        Task<IEnumerable<Soggetto>> GetAllAsync();

        /// <summary>
        /// Ottiene un soggetto per ID
        /// </summary>
        /// <param name="id">ID del soggetto</param>
        Task<Soggetto?> GetByIdAsync(Guid id);

        /// <summary>
        /// Aggiunge un nuovo soggetto
        /// </summary>
        /// <param name="soggetto">Soggetto da aggiungere</param>
        Task AddAsync(Soggetto soggetto);

        /// <summary>
        /// Aggiorna un soggetto esistente
        /// </summary>
        /// <param name="soggetto">Soggetto da aggiornare</param>
        Task UpdateAsync(Soggetto soggetto);

        /// <summary>
        /// Elimina un soggetto (soft delete)
        /// </summary>
        /// <param name="soggetto">Soggetto da eliminare</param>
        Task DeleteAsync(Soggetto soggetto);

        /// <summary>
        /// Salva le modifiche nel database
        /// </summary>
        Task<int> SaveChangesAsync();

        /// <summary>
        /// Verifica se esiste un soggetto con l'ID specificato
        /// </summary>
        /// <param name="id">ID del soggetto</param>
        Task<bool> ExistsAsync(Guid id);

        // ===================================
        // QUERY SPECIFICHE - FILTRI
        // ===================================

        /// <summary>
        /// Ottiene tutti i clienti
        /// </summary>
        Task<IEnumerable<Soggetto>> GetClientiAsync();

        /// <summary>
        /// Ottiene tutti i fornitori
        /// </summary>
        Task<IEnumerable<Soggetto>> GetFornitoriAsync();

        /// <summary>
        /// Ottiene soggetti filtrati per tipo (Azienda o Persona Fisica)
        /// </summary>
        /// <param name="tipo">Tipo di soggetto</param>
        Task<IEnumerable<Soggetto>> GetByTipoAsync(TipoSoggetto tipo);

        /// <summary>
        /// Ottiene soggetti filtrati per natura giuridica (PA o Privato)
        /// </summary>
        /// <param name="natura">Natura giuridica</param>
        Task<IEnumerable<Soggetto>> GetByNaturaGiuridicaAsync(NaturaGiuridica natura);

        /// <summary>
        /// Ottiene soggetti con filtri multipli
        /// </summary>
        /// <param name="tipo">Tipo soggetto (opzionale)</param>
        /// <param name="natura">Natura giuridica (opzionale)</param>
        /// <param name="isCliente">Filtra per clienti (opzionale)</param>
        /// <param name="isFornitore">Filtra per fornitori (opzionale)</param>
        Task<IEnumerable<Soggetto>> GetFilteredAsync(
            TipoSoggetto? tipo = null,
            NaturaGiuridica? natura = null,
            bool? isCliente = null,
            bool? isFornitore = null);

        /// <summary>
        /// Ottiene soggetti paginati con filtri e ordinamento
        /// </summary>
        /// <param name="filters">Filtri e parametri di paginazione</param>
        /// <returns>Tupla con (Items, TotalCount)</returns>
        Task<(IEnumerable<Soggetto> Items, int TotalCount)> GetPagedAsync(SoggettoFilterViewModel filters);

        // ===================================
        // RICERCHE SPECIFICHE
        // ===================================

        /// <summary>
        /// Cerca soggetti per testo (denominazione, nome, cognome, email)
        /// </summary>
        /// <param name="searchTerm">Termine di ricerca</param>
        Task<IEnumerable<Soggetto>> SearchAsync(string searchTerm);

        /// <summary>
        /// Ottiene un soggetto per Codice Interno
        /// </summary>
        /// <param name="codiceInterno">Codice interno</param>
        Task<Soggetto?> GetByCodiceInternoAsync(string codiceInterno);

        /// <summary>
        /// Ottiene un soggetto per Partita IVA
        /// </summary>
        /// <param name="partitaIva">Partita IVA</param>
        Task<Soggetto?> GetByPartitaIVAAsync(string partitaIva);

        /// <summary>
        /// Ottiene un soggetto per Codice Fiscale
        /// </summary>
        /// <param name="codiceFiscale">Codice Fiscale</param>
        Task<Soggetto?> GetByCodiceFiscaleAsync(string codiceFiscale);

        /// <summary>
        /// Ottiene un soggetto per Email
        /// </summary>
        /// <param name="email">Email</param>
        Task<Soggetto?> GetByEmailAsync(string email);

        // ===================================
        // VALIDAZIONI / ESISTENZA
        // ===================================

        /// <summary>
        /// Verifica se esiste un soggetto con il Codice Interno specificato
        /// </summary>
        /// <param name="codiceInterno">Codice interno</param>
        /// <param name="excludeId">ID da escludere (per edit)</param>
        Task<bool> ExistsByCodiceInternoAsync(string codiceInterno, Guid? excludeId = null);

        /// <summary>
        /// Verifica se esiste un soggetto con la Partita IVA specificata
        /// </summary>
        /// <param name="partitaIva">Partita IVA</param>
        /// <param name="excludeId">ID da escludere (per edit)</param>
        Task<bool> ExistsByPartitaIVAAsync(string partitaIva, Guid? excludeId = null);

        /// <summary>
        /// Verifica se esiste un soggetto con il Codice Fiscale specificato
        /// </summary>
        /// <param name="codiceFiscale">Codice Fiscale</param>
        /// <param name="excludeId">ID da escludere (per edit)</param>
        Task<bool> ExistsByCodiceFiscaleAsync(string codiceFiscale, Guid? excludeId = null);

        /// <summary>
        /// Verifica se esiste un soggetto con l'Email specificata
        /// </summary>
        /// <param name="email">Email</param>
        /// <param name="excludeId">ID da escludere (per edit)</param>
        Task<bool> ExistsByEmailAsync(string email, Guid? excludeId = null);

        // ===================================
        // STATISTICHE / CONTEGGI
        // ===================================

        /// <summary>
        /// Conta il numero totale di soggetti
        /// </summary>
        Task<int> CountAsync();

        /// <summary>
        /// Conta il numero di clienti
        /// </summary>
        Task<int> CountClientiAsync();

        /// <summary>
        /// Conta il numero di fornitori
        /// </summary>
        Task<int> CountFornitoriAsync();
    }
}