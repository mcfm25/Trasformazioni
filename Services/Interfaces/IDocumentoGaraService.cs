using Microsoft.AspNetCore.Http;
using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;
using Trasformazioni.Models.ViewModels;
using Trasformazioni.Models.ViewModels.DocumentoGara;

namespace Trasformazioni.Services.Interfaces
{
    /// <summary>
    /// Servizio per la gestione dei documenti delle gare
    /// Coordina le operazioni tra Repository e MinIO Service
    /// </summary>
    public interface IDocumentoGaraService
    {
        /// <summary>
        /// Ottiene un documento per ID
        /// </summary>
        Task<DocumentoGara?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Ottiene tutti i documenti di una gara
        /// </summary>
        Task<IEnumerable<DocumentoGaraListViewModel>> GetByGaraIdAsync(
            Guid garaId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ottiene tutti i documenti di un lotto
        /// </summary>
        Task<IEnumerable<DocumentoGaraListViewModel>> GetByLottoIdAsync(
            Guid lottoId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ottiene tutti i documenti di un preventivo
        /// </summary>
        Task<IEnumerable<DocumentoGaraListViewModel>> GetByPreventivoIdAsync(
            Guid preventivoId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ottiene tutti i documenti di un'integrazione
        /// </summary>
        Task<IEnumerable<DocumentoGaraListViewModel>> GetByIntegrazioneIdAsync(
            Guid integrazioneId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ottiene documenti filtrati
        /// </summary>
        Task<IEnumerable<DocumentoGaraListViewModel>> GetFilteredAsync(
            DocumentoGaraFilterViewModel filter,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Carica un nuovo documento
        /// </summary>
        /// <param name="file">File da caricare</param>
        /// <param name="viewModel">ViewModel con i dati del documento</param>
        /// <param name="userId">ID dell'utente che carica il documento</param>
        /// <param name="cancellationToken">Token di cancellazione</param>
        /// <returns>Documento creato</returns>
        Task<DocumentoGara> UploadAsync(
            IFormFile file,
            DocumentoGaraUploadViewModel viewModel,
            string userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Scarica un documento
        /// </summary>
        /// <param name="id">ID del documento</param>
        /// <param name="cancellationToken">Token di cancellazione</param>
        /// <returns>Tuple con Stream del file e informazioni (nome, content-type)</returns>
        Task<(Stream FileStream, string FileName, string ContentType)> DownloadAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Aggiorna le informazioni di un documento (descrizione, tipo)
        /// NON cambia il file fisico
        /// </summary>
        Task<DocumentoGara> UpdateAsync(
            Guid id,
            DocumentoGaraEditViewModel viewModel,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Elimina un documento (soft delete su DB, file rimane su MinIO)
        /// </summary>
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Elimina fisicamente un documento (soft delete su DB + eliminazione da MinIO)
        /// Usare solo per cleanup o in casi eccezionali
        /// </summary>
        Task HardDeleteAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Valida un file prima dell'upload
        /// </summary>
        /// <param name="file">File da validare</param>
        /// <returns>Tuple con (IsValid, ErrorMessage)</returns>
        (bool IsValid, string? ErrorMessage) ValidateFile(IFormFile file);

        /// <summary>
        /// Ottiene statistiche sui documenti di una gara
        /// </summary>
        Task<DocumentoGaraStatisticsViewModel> GetGaraStatisticsAsync(
            Guid garaId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ottiene statistiche sui documenti di un lotto
        /// </summary>
        Task<DocumentoGaraStatisticsViewModel> GetLottoStatisticsAsync(
            Guid lottoId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Verifica l'integrità di un documento (confronta DB con MinIO)
        /// </summary>
        Task<bool> VerifyIntegrityAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Trova e riporta documenti orfani o inconsistenti
        /// </summary>
        Task<IEnumerable<DocumentoGara>> FindOrphanedDocumentsAsync(
            CancellationToken cancellationToken = default);


        #region Workflow Document Validation

        /// <summary>
        /// Verifica se esiste almeno un documento del tipo specificato per il lotto.
        /// Usa il CodiceRiferimento del TipoDocumento per il confronto.
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        /// <param name="tipo">Tipo documento da cercare (enum TipoDocumentoGara)</param>
        /// <param name="cancellationToken">Token di cancellazione</param>
        /// <returns>True se esiste almeno un documento del tipo specificato</returns>
        Task<bool> HasDocumentoTipoAsync(
            Guid lottoId,
            TipoDocumentoGara tipo,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Verifica se esiste almeno un documento del tipo specificato per la gara.
        /// Usa il CodiceRiferimento del TipoDocumento per il confronto.
        /// </summary>
        /// <param name="garaId">ID della gara</param>
        /// <param name="tipo">Tipo documento da cercare (enum TipoDocumentoGara)</param>
        /// <param name="cancellationToken">Token di cancellazione</param>
        /// <returns>True se esiste almeno un documento del tipo specificato</returns>
        Task<bool> HasDocumentoTipoGaraAsync(
            Guid garaId,
            TipoDocumentoGara tipo,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Verifica la presenza di più tipi documento per il lotto.
        /// Restituisce un dizionario con lo stato di presenza per ogni tipo richiesto.
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        /// <param name="tipiRichiesti">Elenco dei tipi documento da verificare</param>
        /// <param name="cancellationToken">Token di cancellazione</param>
        /// <returns>Dizionario con TipoDocumentoGara come chiave e bool (presente/assente) come valore</returns>
        Task<Dictionary<TipoDocumentoGara, bool>> CheckDocumentiRequisitiAsync(
            Guid lottoId,
            IEnumerable<TipoDocumentoGara> tipiRichiesti,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ottiene il conteggio dei documenti per ogni tipo (CodiceRiferimento) di un lotto.
        /// Utile per dashboard e visualizzazione stato documentale.
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        /// <param name="cancellationToken">Token di cancellazione</param>
        /// <returns>Dizionario con CodiceRiferimento come chiave e conteggio come valore</returns>
        Task<Dictionary<string, int>> GetConteggiPerTipoAsync(
            Guid lottoId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Valida i requisiti documentali per una specifica fase del workflow del lotto.
        /// </summary>
        /// <param name="lottoId">ID del lotto</param>
        /// <param name="fase">Fase del workflow (es. "ValutazioneTecnica", "Elaborazione")</param>
        /// <param name="cancellationToken">Token di cancellazione</param>
        /// <returns>Tupla con (IsValid, lista errori/documenti mancanti)</returns>
        Task<(bool IsValid, List<string> DocumentiMancanti)> ValidaRequisitiDocumentaliFaseAsync(
            Guid lottoId,
            string fase,
            CancellationToken cancellationToken = default);

        #endregion
    }
}