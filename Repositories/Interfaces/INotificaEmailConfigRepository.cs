using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;

namespace Trasformazioni.Repositories.Interfaces
{
    /// <summary>
    /// Repository per la gestione delle configurazioni notifiche email
    /// </summary>
    public interface INotificaEmailConfigRepository
    {
        // ===================================
        // CONFIGURAZIONI
        // ===================================

        Task<IEnumerable<ConfigurazioneNotificaEmail>> GetAllConfigurazioniAsync();
        Task<ConfigurazioneNotificaEmail?> GetConfigurazioneByIdAsync(Guid id);
        Task<ConfigurazioneNotificaEmail?> GetConfigurazioneByCodiceAsync(string codice);
        Task<ConfigurazioneNotificaEmail?> GetConfigurazioneWithDestinatariAsync(Guid id);
        Task<ConfigurazioneNotificaEmail?> GetConfigurazioneWithDestinatariByCodiceAsync(string codice);
        Task UpdateConfigurazioneAsync(ConfigurazioneNotificaEmail configurazione);
        Task<bool> ConfigurazioneExistsAsync(Guid id);
        Task<bool> ConfigurazioneExistsByCodiceAsync(string codice);

        // ===================================
        // DESTINATARI
        // ===================================

        Task<DestinatarioNotificaEmail?> GetDestinatarioByIdAsync(Guid id);
        Task AddDestinatarioAsync(DestinatarioNotificaEmail destinatario);
        Task UpdateDestinatarioAsync(DestinatarioNotificaEmail destinatario);
        Task DeleteDestinatarioAsync(DestinatarioNotificaEmail destinatario);

        /// <summary>
        /// Verifica se esiste già un destinatario con gli stessi parametri
        /// </summary>
        Task<bool> DestinatarioExistsAsync(
            Guid configurazioneId,
            TipoDestinatarioNotifica tipo,
            Guid? repartoId,
            string? ruolo,
            string? utenteId);

        // ===================================
        // QUERY
        // ===================================

        Task<IEnumerable<ConfigurazioneNotificaEmail>> GetConfigurazioniByModuloAsync(string modulo);
        Task<IEnumerable<ConfigurazioneNotificaEmail>> GetConfigurazioniAttiveAsync();

        // ===================================
        // PERSISTENZA
        // ===================================

        Task SaveChangesAsync();
    }
}