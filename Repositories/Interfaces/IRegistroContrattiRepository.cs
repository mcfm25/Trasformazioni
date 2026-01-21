using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;

namespace Trasformazioni.Data.Repositories
{
    /// <summary>
    /// Interfaccia per l'accesso ai dati del Registro Contratti
    /// </summary>
    public interface IRegistroContrattiRepository
    {
        // ===================================
        // OPERAZIONI BASE
        // ===================================

        /// <summary>
        /// Ottiene tutti i registri non cancellati
        /// </summary>
        Task<IEnumerable<RegistroContratti>> GetAllAsync();

        /// <summary>
        /// Ottiene un registro per ID con tutte le relazioni
        /// </summary>
        Task<RegistroContratti?> GetByIdAsync(Guid id);

        /// <summary>
        /// Ottiene un registro per ID con gli allegati
        /// </summary>
        Task<RegistroContratti?> GetByIdWithAllegatiAsync(Guid id);

        /// <summary>
        /// Aggiunge un nuovo registro
        /// </summary>
        Task AddAsync(RegistroContratti registro);

        /// <summary>
        /// Aggiorna un registro esistente
        /// </summary>
        Task UpdateAsync(RegistroContratti registro);

        /// <summary>
        /// Elimina un registro (soft delete)
        /// </summary>
        Task DeleteAsync(RegistroContratti registro);

        /// <summary>
        /// Salva le modifiche nel database
        /// </summary>
        Task<int> SaveChangesAsync();

        /// <summary>
        /// Verifica se esiste un registro con l'ID specificato
        /// </summary>
        Task<bool> ExistsAsync(Guid id);

        // ===================================
        // QUERY PER TIPO
        // ===================================

        /// <summary>
        /// Ottiene tutti i preventivi
        /// </summary>
        Task<IEnumerable<RegistroContratti>> GetPreventiviAsync();

        /// <summary>
        /// Ottiene tutti i contratti
        /// </summary>
        Task<IEnumerable<RegistroContratti>> GetContrattiAsync();

        /// <summary>
        /// Ottiene registri per tipo
        /// </summary>
        Task<IEnumerable<RegistroContratti>> GetByTipoAsync(TipoRegistro tipo);

        // ===================================
        // QUERY PER STATO
        // ===================================

        /// <summary>
        /// Ottiene registri per stato
        /// </summary>
        Task<IEnumerable<RegistroContratti>> GetByStatoAsync(StatoRegistro stato);

        /// <summary>
        /// Ottiene registri attivi
        /// </summary>
        Task<IEnumerable<RegistroContratti>> GetAttiviAsync();

        /// <summary>
        /// Ottiene registri in scadenza (stato InScadenza o InScadenzaPropostoRinnovo)
        /// </summary>
        Task<IEnumerable<RegistroContratti>> GetInScadenzaAsync();

        /// <summary>
        /// Ottiene registri scaduti
        /// </summary>
        Task<IEnumerable<RegistroContratti>> GetScadutiAsync();

        // ===================================
        // QUERY PER CLIENTE
        // ===================================

        /// <summary>
        /// Ottiene registri per cliente
        /// </summary>
        Task<IEnumerable<RegistroContratti>> GetByClienteIdAsync(Guid clienteId);

        /// <summary>
        /// Ottiene registri attivi per cliente
        /// </summary>
        Task<IEnumerable<RegistroContratti>> GetAttiviByClienteIdAsync(Guid clienteId);

        // ===================================
        // QUERY PER CATEGORIA
        // ===================================

        /// <summary>
        /// Ottiene registri per categoria
        /// </summary>
        Task<IEnumerable<RegistroContratti>> GetByCategoriaIdAsync(Guid categoriaId);

        // ===================================
        // QUERY PER RESPONSABILE
        // ===================================

        /// <summary>
        /// Ottiene registri per responsabile interno
        /// </summary>
        Task<IEnumerable<RegistroContratti>> GetByUtenteIdAsync(string utenteId);

        // ===================================
        // QUERY PER GERARCHIA
        // ===================================

        /// <summary>
        /// Ottiene i figli di un registro (versioni successive)
        /// </summary>
        Task<IEnumerable<RegistroContratti>> GetChildrenAsync(Guid parentId);

        /// <summary>
        /// Ottiene la catena completa dei padri (storico versioni)
        /// </summary>
        Task<IEnumerable<RegistroContratti>> GetParentChainAsync(Guid id);

        /// <summary>
        /// Ottiene i registri radice (senza parent)
        /// </summary>
        Task<IEnumerable<RegistroContratti>> GetRootRegistriAsync();

        // ===================================
        // QUERY PER SCADENZE (JOB)
        // ===================================

        /// <summary>
        /// Ottiene registri che devono passare in stato InScadenza
        /// (DataLimiteDisdetta - GiorniAlertScadenza <= oggi)
        /// </summary>
        Task<IEnumerable<RegistroContratti>> GetRegistriPerAlertScadenzaAsync();

        /// <summary>
        /// Ottiene registri che devono passare in stato Scaduto
        /// (DataFineOScadenza < oggi)
        /// </summary>
        Task<IEnumerable<RegistroContratti>> GetRegistriDaScadereAsync();

        /// <summary>
        /// Ottiene registri con rinnovo automatico da processare
        /// </summary>
        Task<IEnumerable<RegistroContratti>> GetRegistriPerRinnovoAutomaticoAsync();

        // ===================================
        // RICERCHE
        // ===================================

        /// <summary>
        /// Cerca registri per testo (oggetto, ragione sociale, protocollo)
        /// </summary>
        Task<IEnumerable<RegistroContratti>> SearchAsync(string searchTerm);

        /// <summary>
        /// Ottiene un registro per numero protocollo
        /// </summary>
        Task<RegistroContratti?> GetByNumeroProtocolloAsync(string numeroProtocollo);

        /// <summary>
        /// Ottiene un registro per riferimento esterno
        /// </summary>
        Task<RegistroContratti?> GetByIdRiferimentoEsternoAsync(string idRiferimentoEsterno);

        // ===================================
        // VALIDAZIONI
        // ===================================

        /// <summary>
        /// Verifica se esiste un registro con il numero protocollo specificato
        /// </summary>
        Task<bool> ExistsByNumeroProtocolloAsync(string numeroProtocollo, Guid? excludeId = null);

        /// <summary>
        /// Ottiene il prossimo numero progressivo per il protocollo
        /// </summary>
        Task<int> GetNextProgressivoProtocolloAsync();

        // ===================================
        // PAGINAZIONE
        // ===================================

        /// <summary>
        /// Ottiene registri paginati con filtri
        /// </summary>
        Task<(IEnumerable<RegistroContratti> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            TipoRegistro? tipo = null,
            StatoRegistro? stato = null,
            Guid? clienteId = null,
            Guid? categoriaId = null,
            bool? soloRoot = null,
            bool? soloConVersioni = null,
            string? utenteId = null,
            string? searchTerm = null,
            string orderBy = "DataDocumento",
            string orderDirection = "desc");

        // ===================================
        // STATISTICHE
        // ===================================

        /// <summary>
        /// Conta il numero totale di registri
        /// </summary>
        Task<int> CountAsync();

        /// <summary>
        /// Conta registri per tipo
        /// </summary>
        Task<int> CountByTipoAsync(TipoRegistro tipo);

        /// <summary>
        /// Conta registri per stato
        /// </summary>
        Task<int> CountByStatoAsync(StatoRegistro stato);

        /// <summary>
        /// Calcola il totale importi canone annuo per stato
        /// </summary>
        Task<decimal> SumImportoCanoneByStatoAsync(StatoRegistro stato);

        /// <summary>
        /// Calcola il totale importi canone annuo per cliente
        /// </summary>
        Task<decimal> SumImportoCanoneByClienteAsync(Guid clienteId);
    }
}