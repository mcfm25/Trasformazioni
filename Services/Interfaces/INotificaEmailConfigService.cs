using Trasformazioni.Models.ViewModels;

namespace Trasformazioni.Services.Interfaces
{
    /// <summary>
    /// Servizio per la gestione delle configurazioni notifiche email
    /// </summary>
    public interface INotificaEmailConfigService
    {
        // ===================================
        // RISOLUZIONE DESTINATARI (per i job/service)
        // ===================================

        /// <summary>
        /// Ottiene la lista di email per un'operazione (risolve tutti i destinatari)
        /// </summary>
        /// <param name="codiceOperazione">Codice operazione (es. CodiciNotifica.ContrattoInScadenza)</param>
        /// <returns>Lista email univoche, vuota se configurazione non attiva o senza destinatari</returns>
        Task<List<string>> GetDestinatariAsync(string codiceOperazione);

        /// <summary>
        /// Verifica se una configurazione è attiva e ha almeno un destinatario
        /// </summary>
        Task<bool> IsNotificaAttivaAsync(string codiceOperazione);

        /// <summary>
        /// Ottiene l'oggetto email di default per un'operazione
        /// </summary>
        Task<string?> GetOggettoEmailDefaultAsync(string codiceOperazione);

        // ===================================
        // CRUD CONFIGURAZIONI (per UI admin)
        // ===================================

        /// <summary>
        /// Ottiene tutte le configurazioni per la lista
        /// </summary>
        Task<List<ConfigurazioneNotificaEmailListViewModel>> GetAllConfigurazioniAsync();

        /// <summary>
        /// Ottiene tutte le configurazioni raggruppate per modulo
        /// </summary>
        Task<Dictionary<string, List<ConfigurazioneNotificaEmailListViewModel>>> GetConfigurazioniGroupedByModuloAsync();

        /// <summary>
        /// Ottiene una configurazione con i suoi destinatari
        /// </summary>
        Task<ConfigurazioneNotificaEmailDetailsViewModel?> GetConfigurazioneAsync(Guid id);

        /// <summary>
        /// Ottiene una configurazione per codice
        /// </summary>
        Task<ConfigurazioneNotificaEmailDetailsViewModel?> GetConfigurazioneByCodiceAsync(string codice);

        /// <summary>
        /// Aggiorna una configurazione (descrizione, oggetto, note)
        /// </summary>
        Task<(bool Success, string? Error)> UpdateConfigurazioneAsync(
            ConfigurazioneNotificaEmailEditViewModel model,
            string currentUserId);

        /// <summary>
        /// Attiva/disattiva una configurazione
        /// </summary>
        Task<(bool Success, string? Error)> ToggleAttivaAsync(Guid configurazioneId, string currentUserId);

        // ===================================
        // CRUD DESTINATARI (per UI admin)
        // ===================================

        /// <summary>
        /// Aggiunge un destinatario a una configurazione
        /// </summary>
        Task<(bool Success, string? Error, Guid? Id)> AddDestinatarioAsync(
            DestinatarioNotificaEmailCreateViewModel model,
            string currentUserId);

        /// <summary>
        /// Rimuove un destinatario (soft delete)
        /// </summary>
        Task<(bool Success, string? Error)> RemoveDestinatarioAsync(Guid destinatarioId, string currentUserId);

        // ===================================
        // DROPDOWN / HELPER
        // ===================================

        /// <summary>
        /// Ottiene tutti i ruoli disponibili per il dropdown
        /// </summary>
        Task<List<string>> GetAllRuoliAsync();

        /// <summary>
        /// Ottiene tutti i moduli disponibili
        /// </summary>
        Task<List<string>> GetAllModuliAsync();
    }
}