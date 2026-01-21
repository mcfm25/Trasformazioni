using Microsoft.EntityFrameworkCore;
using Trasformazioni.Data;
using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;
using Trasformazioni.Repositories.Interfaces;

namespace Trasformazioni.Repositories
{
    /// <summary>
    /// Implementazione repository per le configurazioni notifiche email
    /// </summary>
    public class NotificaEmailConfigRepository : INotificaEmailConfigRepository
    {
        private readonly ApplicationDbContext _context;

        public NotificaEmailConfigRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===================================
        // CONFIGURAZIONI
        // ===================================

        public async Task<IEnumerable<ConfigurazioneNotificaEmail>> GetAllConfigurazioniAsync()
        {
            return await _context.ConfigurazioniNotificaEmail
                .Include(c => c.Destinatari.Where(d => !d.IsDeleted))
                .OrderBy(c => c.Modulo)
                .ThenBy(c => c.Descrizione)
                .ToListAsync();
        }

        public async Task<ConfigurazioneNotificaEmail?> GetConfigurazioneByIdAsync(Guid id)
        {
            return await _context.ConfigurazioniNotificaEmail
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<ConfigurazioneNotificaEmail?> GetConfigurazioneByCodiceAsync(string codice)
        {
            return await _context.ConfigurazioniNotificaEmail
                .FirstOrDefaultAsync(c => c.Codice == codice);
        }

        public async Task<ConfigurazioneNotificaEmail?> GetConfigurazioneWithDestinatariAsync(Guid id)
        {
            return await _context.ConfigurazioniNotificaEmail
                .Include(c => c.Destinatari.Where(d => !d.IsDeleted))
                    .ThenInclude(d => d.Reparto)
                .Include(c => c.Destinatari.Where(d => !d.IsDeleted))
                    .ThenInclude(d => d.Utente)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<ConfigurazioneNotificaEmail?> GetConfigurazioneWithDestinatariByCodiceAsync(string codice)
        {
            return await _context.ConfigurazioniNotificaEmail
                .Include(c => c.Destinatari.Where(d => !d.IsDeleted))
                    .ThenInclude(d => d.Reparto)
                .Include(c => c.Destinatari.Where(d => !d.IsDeleted))
                    .ThenInclude(d => d.Utente)
                .FirstOrDefaultAsync(c => c.Codice == codice);
        }

        public Task UpdateConfigurazioneAsync(ConfigurazioneNotificaEmail configurazione)
        {
            _context.ConfigurazioniNotificaEmail.Update(configurazione);
            return Task.CompletedTask;
        }

        public async Task<bool> ConfigurazioneExistsAsync(Guid id)
        {
            return await _context.ConfigurazioniNotificaEmail.AnyAsync(c => c.Id == id);
        }

        public async Task<bool> ConfigurazioneExistsByCodiceAsync(string codice)
        {
            return await _context.ConfigurazioniNotificaEmail.AnyAsync(c => c.Codice == codice);
        }

        // ===================================
        // DESTINATARI
        // ===================================

        public async Task<DestinatarioNotificaEmail?> GetDestinatarioByIdAsync(Guid id)
        {
            return await _context.DestinatariNotificaEmail
                .Include(d => d.Reparto)
                .Include(d => d.Utente)
                .Include(d => d.ConfigurazioneNotificaEmail)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task AddDestinatarioAsync(DestinatarioNotificaEmail destinatario)
        {
            await _context.DestinatariNotificaEmail.AddAsync(destinatario);
        }

        public Task UpdateDestinatarioAsync(DestinatarioNotificaEmail destinatario)
        {
            _context.DestinatariNotificaEmail.Update(destinatario);
            return Task.CompletedTask;
        }

        public Task DeleteDestinatarioAsync(DestinatarioNotificaEmail destinatario)
        {
            // Soft delete
            destinatario.IsDeleted = true;
            destinatario.DeletedAt = DateTime.Now;
            _context.DestinatariNotificaEmail.Update(destinatario);
            return Task.CompletedTask;
        }

        public async Task<bool> DestinatarioExistsAsync(
            Guid configurazioneId,
            TipoDestinatarioNotifica tipo,
            Guid? repartoId,
            string? ruolo,
            string? utenteId)
        {
            return await _context.DestinatariNotificaEmail
                .Where(d => !d.IsDeleted)
                .Where(d => d.ConfigurazioneNotificaEmailId == configurazioneId)
                .Where(d => d.Tipo == tipo)
                .Where(d =>
                    (tipo == TipoDestinatarioNotifica.Reparto && d.RepartoId == repartoId) ||
                    (tipo == TipoDestinatarioNotifica.Ruolo && d.Ruolo == ruolo) ||
                    (tipo == TipoDestinatarioNotifica.Utente && d.UtenteId == utenteId))
                .AnyAsync();
        }

        // ===================================
        // QUERY
        // ===================================

        public async Task<IEnumerable<ConfigurazioneNotificaEmail>> GetConfigurazioniByModuloAsync(string modulo)
        {
            return await _context.ConfigurazioniNotificaEmail
                .Include(c => c.Destinatari.Where(d => !d.IsDeleted))
                .Where(c => c.Modulo == modulo)
                .OrderBy(c => c.Descrizione)
                .ToListAsync();
        }

        public async Task<IEnumerable<ConfigurazioneNotificaEmail>> GetConfigurazioniAttiveAsync()
        {
            return await _context.ConfigurazioniNotificaEmail
                .Include(c => c.Destinatari.Where(d => !d.IsDeleted))
                .Where(c => c.IsAttiva)
                .OrderBy(c => c.Modulo)
                .ThenBy(c => c.Descrizione)
                .ToListAsync();
        }

        // ===================================
        // PERSISTENZA
        // ===================================

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}