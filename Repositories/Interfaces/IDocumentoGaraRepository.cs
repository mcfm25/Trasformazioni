using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;

namespace Trasformazioni.Repositories.Interfaces
{
    /// <summary>
    /// Repository per la gestione dei documenti delle gare
    /// Fornisce operazioni CRUD e query specializzate per DocumentoGara
    /// </summary>
    public interface IDocumentoGaraRepository
    {
        /// <summary>
        /// Ottiene un documento per ID
        /// </summary>
        Task<DocumentoGara?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Ottiene un documento per path MinIO
        /// </summary>
        Task<DocumentoGara?> GetByPathAsync(string pathMinIO, CancellationToken cancellationToken = default);

        /// <summary>
        /// Ottiene tutti i documenti di una gara
        /// </summary>
        Task<IEnumerable<DocumentoGara>> GetByGaraIdAsync(
            Guid garaId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ottiene tutti i documenti di un lotto
        /// </summary>
        Task<IEnumerable<DocumentoGara>> GetByLottoIdAsync(
            Guid lottoId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ottiene tutti i documenti di un preventivo
        /// </summary>
        Task<IEnumerable<DocumentoGara>> GetByPreventivoIdAsync(
            Guid preventivoId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ottiene tutti i documenti di un'integrazione
        /// </summary>
        Task<IEnumerable<DocumentoGara>> GetByIntegrazioneIdAsync(
            Guid integrazioneId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ottiene documenti filtrati per gara e tipo
        /// </summary>
        Task<IEnumerable<DocumentoGara>> GetByGaraIdAndTipoAsync(
            Guid garaId,
            TipoDocumentoGara tipo,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ottiene documenti filtrati per lotto e tipo
        /// </summary>
        Task<IEnumerable<DocumentoGara>> GetByLottoIdAndTipoAsync(
            Guid lottoId,
            TipoDocumentoGara tipo,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ottiene tutti i documenti caricati da un utente
        /// </summary>
        Task<IEnumerable<DocumentoGara>> GetByUserIdAsync(
            string userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ottiene documenti caricati in un range di date
        /// </summary>
        Task<IEnumerable<DocumentoGara>> GetByDateRangeAsync(
            DateTime from,
            DateTime to,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Conta il numero di documenti di una gara
        /// </summary>
        Task<int> CountByGaraIdAsync(
            Guid garaId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Conta il numero di documenti di un lotto
        /// </summary>
        Task<int> CountByLottoIdAsync(
            Guid lottoId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ottiene la dimensione totale dei documenti di una gara (in bytes)
        /// </summary>
        Task<long> GetTotalSizeByGaraIdAsync(
            Guid garaId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ottiene la dimensione totale dei documenti di un lotto (in bytes)
        /// </summary>
        Task<long> GetTotalSizeByLottoIdAsync(
            Guid lottoId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Crea un nuovo documento
        /// </summary>
        Task<DocumentoGara> CreateAsync(
            DocumentoGara documento,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Aggiorna un documento esistente
        /// </summary>
        Task<DocumentoGara> UpdateAsync(
            DocumentoGara documento,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Elimina un documento (soft delete)
        /// </summary>
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Verifica se esiste un documento con un determinato path MinIO
        /// </summary>
        Task<bool> ExistsByPathAsync(
            string pathMinIO,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ottiene documenti orfani (senza FK validi)
        /// </summary>
        Task<IEnumerable<DocumentoGara>> GetOrphanedDocumentsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Elimina fisicamente un record (senza soft delete)
        /// Usato per rollback transazioni
        /// </summary>
        Task HardDeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}