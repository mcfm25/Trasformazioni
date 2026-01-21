using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;
using Trasformazioni.Models.ViewModels;
using Trasformazioni.Repositories.Interfaces;
using Trasformazioni.Services.Interfaces;

namespace Trasformazioni.Services
{
    /// <summary>
    /// Servizio per la gestione delle configurazioni notifiche email
    /// </summary>
    public class NotificaEmailConfigService : INotificaEmailConfigService
    {
        private readonly INotificaEmailConfigRepository _repository;
        private readonly IRepartoRepository _repartoRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<NotificaEmailConfigService> _logger;

        public NotificaEmailConfigService(
            INotificaEmailConfigRepository repository,
            IRepartoRepository repartoRepository,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<NotificaEmailConfigService> logger)
        {
            _repository = repository;
            _repartoRepository = repartoRepository;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        // ===================================
        // RISOLUZIONE DESTINATARI (per i job/service)
        // ===================================

        /// <summary>
        /// Ottiene la lista di email per un'operazione (risolve tutti i destinatari)
        /// </summary>
        public async Task<List<string>> GetDestinatariAsync(string codiceOperazione)
        {
            try
            {
                var configurazione = await _repository.GetConfigurazioneWithDestinatariByCodiceAsync(codiceOperazione);

                if (configurazione == null)
                {
                    _logger.LogDebug("Configurazione {Codice} non trovata", codiceOperazione);
                    return new List<string>();
                }

                if (!configurazione.IsAttiva)
                {
                    _logger.LogDebug("Configurazione {Codice} non attiva", codiceOperazione);
                    return new List<string>();
                }

                var emails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (var destinatario in configurazione.Destinatari.Where(d => !d.IsDeleted))
                {
                    var emailsRisolte = await RisolviDestinatarioAsync(destinatario);
                    foreach (var email in emailsRisolte)
                    {
                        emails.Add(email);
                    }
                }

                _logger.LogDebug("Risolti {Count} destinatari per {Codice}", emails.Count, codiceOperazione);

                return emails.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la risoluzione destinatari per {Codice}", codiceOperazione);
                return new List<string>();
            }
        }

        /// <summary>
        /// Risolve un singolo destinatario in una o più email
        /// </summary>
        private async Task<List<string>> RisolviDestinatarioAsync(DestinatarioNotificaEmail destinatario)
        {
            var emails = new List<string>();

            switch (destinatario.Tipo)
            {
                case TipoDestinatarioNotifica.Reparto:
                    // Risolve a 1 email: quella del reparto
                    if (destinatario.Reparto?.Email != null && !destinatario.Reparto.IsDeleted)
                    {
                        emails.Add(destinatario.Reparto.Email);
                    }
                    break;

                case TipoDestinatarioNotifica.Ruolo:
                    // Risolve a N email: tutti gli utenti attivi con quel ruolo
                    if (!string.IsNullOrEmpty(destinatario.Ruolo))
                    {
                        var utentiRuolo = await _userManager.GetUsersInRoleAsync(destinatario.Ruolo);
                        foreach (var utente in utentiRuolo.Where(u => u.IsAttivo && !u.IsDeleted && !string.IsNullOrEmpty(u.Email)))
                        {
                            emails.Add(utente.Email!);
                        }
                    }
                    break;

                case TipoDestinatarioNotifica.Utente:
                    // Risolve a 1 email: quella dell'utente specifico
                    if (destinatario.Utente?.IsAttivo == true &&
                        !destinatario.Utente.IsDeleted &&
                        !string.IsNullOrEmpty(destinatario.Utente.Email))
                    {
                        emails.Add(destinatario.Utente.Email);
                    }
                    break;
            }

            return emails;
        }

        /// <summary>
        /// Verifica se una configurazione è attiva e ha almeno un destinatario
        /// </summary>
        public async Task<bool> IsNotificaAttivaAsync(string codiceOperazione)
        {
            try
            {
                var configurazione = await _repository.GetConfigurazioneByCodiceAsync(codiceOperazione);
                return configurazione?.IsAttiva == true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante verifica stato notifica {Codice}", codiceOperazione);
                return false;
            }
        }

        /// <summary>
        /// Ottiene l'oggetto email di default per un'operazione
        /// </summary>
        public async Task<string?> GetOggettoEmailDefaultAsync(string codiceOperazione)
        {
            try
            {
                var configurazione = await _repository.GetConfigurazioneByCodiceAsync(codiceOperazione);
                return configurazione?.OggettoEmailDefault;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante recupero oggetto email per {Codice}", codiceOperazione);
                return null;
            }
        }

        // ===================================
        // CRUD CONFIGURAZIONI (per UI admin)
        // ===================================

        /// <summary>
        /// Ottiene tutte le configurazioni per la lista
        /// </summary>
        public async Task<List<ConfigurazioneNotificaEmailListViewModel>> GetAllConfigurazioniAsync()
        {
            var configurazioni = await _repository.GetAllConfigurazioniAsync();

            return configurazioni.Select(c => new ConfigurazioneNotificaEmailListViewModel
            {
                Id = c.Id,
                Codice = c.Codice,
                Descrizione = c.Descrizione,
                Modulo = c.Modulo,
                IsAttiva = c.IsAttiva,
                DestinatariCount = c.Destinatari.Count(d => !d.IsDeleted),
                OggettoEmailDefault = c.OggettoEmailDefault
            }).ToList();
        }

        /// <summary>
        /// Ottiene tutte le configurazioni raggruppate per modulo
        /// </summary>
        public async Task<Dictionary<string, List<ConfigurazioneNotificaEmailListViewModel>>> GetConfigurazioniGroupedByModuloAsync()
        {
            var configurazioni = await GetAllConfigurazioniAsync();

            return configurazioni
                .GroupBy(c => c.Modulo)
                .OrderBy(g => g.Key)
                .ToDictionary(g => g.Key, g => g.ToList());
        }

        /// <summary>
        /// Ottiene una configurazione con i suoi destinatari
        /// </summary>
        public async Task<ConfigurazioneNotificaEmailDetailsViewModel?> GetConfigurazioneAsync(Guid id)
        {
            var configurazione = await _repository.GetConfigurazioneWithDestinatariAsync(id);

            if (configurazione == null)
                return null;

            return await MapToDetailsViewModelAsync(configurazione);
        }

        /// <summary>
        /// Ottiene una configurazione per codice
        /// </summary>
        public async Task<ConfigurazioneNotificaEmailDetailsViewModel?> GetConfigurazioneByCodiceAsync(string codice)
        {
            var configurazione = await _repository.GetConfigurazioneWithDestinatariByCodiceAsync(codice);

            if (configurazione == null)
                return null;

            return await MapToDetailsViewModelAsync(configurazione);
        }

        /// <summary>
        /// Mappa una configurazione al DetailsViewModel
        /// </summary>
        private async Task<ConfigurazioneNotificaEmailDetailsViewModel> MapToDetailsViewModelAsync(ConfigurazioneNotificaEmail configurazione)
        {
            var destinatariVm = new List<DestinatarioNotificaEmailViewModel>();

            foreach (var dest in configurazione.Destinatari.Where(d => !d.IsDeleted).OrderBy(d => d.Ordine))
            {
                var vm = new DestinatarioNotificaEmailViewModel
                {
                    Id = dest.Id,
                    Tipo = dest.Tipo,
                    Note = dest.Note,
                    Ordine = dest.Ordine
                };

                switch (dest.Tipo)
                {
                    case TipoDestinatarioNotifica.Reparto:
                        vm.RepartoNome = dest.Reparto?.Nome;
                        vm.RepartoEmail = dest.Reparto?.Email;
                        break;

                    case TipoDestinatarioNotifica.Ruolo:
                        vm.Ruolo = dest.Ruolo;
                        // Conta utenti con questo ruolo
                        if (!string.IsNullOrEmpty(dest.Ruolo))
                        {
                            var utenti = await _userManager.GetUsersInRoleAsync(dest.Ruolo);
                            var count = utenti.Count(u => u.IsAttivo && !u.IsDeleted);
                            vm.Ruolo = $"{dest.Ruolo} ({count} utenti)";
                        }
                        break;

                    case TipoDestinatarioNotifica.Utente:
                        vm.UtenteNome = dest.Utente?.NomeCompleto;
                        vm.UtenteEmail = dest.Utente?.Email;
                        break;
                }

                destinatariVm.Add(vm);
            }

            return new ConfigurazioneNotificaEmailDetailsViewModel
            {
                Id = configurazione.Id,
                Codice = configurazione.Codice,
                Descrizione = configurazione.Descrizione,
                Modulo = configurazione.Modulo,
                IsAttiva = configurazione.IsAttiva,
                OggettoEmailDefault = configurazione.OggettoEmailDefault,
                Note = configurazione.Note,
                Destinatari = destinatariVm,
                CreatedAt = configurazione.CreatedAt,
                CreatedBy = configurazione.CreatedBy,
                ModifiedAt = configurazione.ModifiedAt,
                ModifiedBy = configurazione.ModifiedBy
            };
        }

        /// <summary>
        /// Aggiorna una configurazione
        /// </summary>
        public async Task<(bool Success, string? Error)> UpdateConfigurazioneAsync(
            ConfigurazioneNotificaEmailEditViewModel model,
            string currentUserId)
        {
            try
            {
                var configurazione = await _repository.GetConfigurazioneByIdAsync(model.Id);

                if (configurazione == null)
                    return (false, "Configurazione non trovata");

                configurazione.Descrizione = model.Descrizione.Trim();
                configurazione.IsAttiva = model.IsAttiva;
                configurazione.OggettoEmailDefault = model.OggettoEmailDefault?.Trim();
                configurazione.Note = model.Note?.Trim();
                configurazione.ModifiedAt = DateTime.Now;
                configurazione.ModifiedBy = currentUserId;

                await _repository.UpdateConfigurazioneAsync(configurazione);
                await _repository.SaveChangesAsync();

                _logger.LogInformation("Configurazione {Codice} aggiornata da {UserId}",
                    configurazione.Codice, currentUserId);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'aggiornamento della configurazione {Id}", model.Id);
                return (false, "Errore durante l'aggiornamento");
            }
        }

        /// <summary>
        /// Attiva/disattiva una configurazione
        /// </summary>
        public async Task<(bool Success, string? Error)> ToggleAttivaAsync(Guid configurazioneId, string currentUserId)
        {
            try
            {
                var configurazione = await _repository.GetConfigurazioneByIdAsync(configurazioneId);

                if (configurazione == null)
                    return (false, "Configurazione non trovata");

                configurazione.IsAttiva = !configurazione.IsAttiva;
                configurazione.ModifiedAt = DateTime.Now;
                configurazione.ModifiedBy = currentUserId;

                await _repository.UpdateConfigurazioneAsync(configurazione);
                await _repository.SaveChangesAsync();

                var stato = configurazione.IsAttiva ? "attivata" : "disattivata";
                _logger.LogInformation("Configurazione {Codice} {Stato} da {UserId}",
                    configurazione.Codice, stato, currentUserId);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante toggle attiva per configurazione {Id}", configurazioneId);
                return (false, "Errore durante l'operazione");
            }
        }

        // ===================================
        // CRUD DESTINATARI (per UI admin)
        // ===================================

        /// <summary>
        /// Aggiunge un destinatario a una configurazione
        /// </summary>
        public async Task<(bool Success, string? Error, Guid? Id)> AddDestinatarioAsync(
            DestinatarioNotificaEmailCreateViewModel model,
            string currentUserId)
        {
            try
            {
                // Valida esistenza configurazione
                if (!await _repository.ConfigurazioneExistsAsync(model.ConfigurazioneNotificaEmailId))
                    return (false, "Configurazione non trovata", null);

                // Valida dati in base al tipo
                var validationError = await ValidateDestinatarioAsync(model);
                if (validationError != null)
                    return (false, validationError, null);

                var destinatario = new DestinatarioNotificaEmail
                {
                    Id = Guid.NewGuid(),
                    ConfigurazioneNotificaEmailId = model.ConfigurazioneNotificaEmailId,
                    Tipo = model.Tipo,
                    RepartoId = model.Tipo == TipoDestinatarioNotifica.Reparto ? model.RepartoId : null,
                    Ruolo = model.Tipo == TipoDestinatarioNotifica.Ruolo ? model.Ruolo : null,
                    UtenteId = model.Tipo == TipoDestinatarioNotifica.Utente ? model.UtenteId : null,
                    Note = model.Note?.Trim(),
                    Ordine = 0,
                    CreatedAt = DateTime.Now,
                    CreatedBy = currentUserId
                };

                await _repository.AddDestinatarioAsync(destinatario);
                await _repository.SaveChangesAsync();

                _logger.LogInformation("Destinatario {Id} aggiunto a configurazione {ConfigId} da {UserId}",
                    destinatario.Id, model.ConfigurazioneNotificaEmailId, currentUserId);

                return (true, null, destinatario.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'aggiunta del destinatario");
                return (false, "Errore durante l'aggiunta del destinatario", null);
            }
        }

        /// <summary>
        /// Valida i dati del destinatario
        /// </summary>
        private async Task<string?> ValidateDestinatarioAsync(DestinatarioNotificaEmailCreateViewModel model)
        {
            switch (model.Tipo)
            {
                case TipoDestinatarioNotifica.Reparto:
                    if (!model.RepartoId.HasValue)
                        return "Seleziona un reparto";
                    if (!await _repartoRepository.ExistsAsync(model.RepartoId.Value))
                        return "Reparto non trovato";
                    // Controllo unicità
                    if (await _repository.DestinatarioExistsAsync(
                        model.ConfigurazioneNotificaEmailId,
                        model.Tipo,
                        model.RepartoId,
                        null,
                        null))
                        return "Questo reparto è già presente come destinatario";
                    break;

                case TipoDestinatarioNotifica.Ruolo:
                    if (string.IsNullOrWhiteSpace(model.Ruolo))
                        return "Seleziona un ruolo";
                    if (!await _roleManager.RoleExistsAsync(model.Ruolo))
                        return "Ruolo non trovato";
                    // Controllo unicità
                    if (await _repository.DestinatarioExistsAsync(
                        model.ConfigurazioneNotificaEmailId,
                        model.Tipo,
                        null,
                        model.Ruolo,
                        null))
                        return "Questo ruolo è già presente come destinatario";
                    break;

                case TipoDestinatarioNotifica.Utente:
                    if (string.IsNullOrWhiteSpace(model.UtenteId))
                        return "Seleziona un utente";
                    var utente = await _userManager.FindByIdAsync(model.UtenteId);
                    if (utente == null || utente.IsDeleted)
                        return "Utente non trovato";
                    // Controllo unicità
                    if (await _repository.DestinatarioExistsAsync(
                        model.ConfigurazioneNotificaEmailId,
                        model.Tipo,
                        null,
                        null,
                        model.UtenteId))
                        return "Questo utente è già presente come destinatario";
                    break;

                default:
                    return "Tipo destinatario non valido";
            }

            return null;
        }

        /// <summary>
        /// Rimuove un destinatario (soft delete)
        /// </summary>
        public async Task<(bool Success, string? Error)> RemoveDestinatarioAsync(Guid destinatarioId, string currentUserId)
        {
            try
            {
                var destinatario = await _repository.GetDestinatarioByIdAsync(destinatarioId);

                if (destinatario == null)
                    return (false, "Destinatario non trovato");

                destinatario.DeletedBy = currentUserId;
                await _repository.DeleteDestinatarioAsync(destinatario);
                await _repository.SaveChangesAsync();

                _logger.LogInformation("Destinatario {Id} rimosso da {UserId}", destinatarioId, currentUserId);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la rimozione del destinatario {Id}", destinatarioId);
                return (false, "Errore durante la rimozione");
            }
        }

        // ===================================
        // DROPDOWN / HELPER
        // ===================================

        /// <summary>
        /// Ottiene tutti i ruoli disponibili per il dropdown
        /// </summary>
        public async Task<List<string>> GetAllRuoliAsync()
        {
            return await _roleManager.Roles
                .Select(r => r.Name!)
                .OrderBy(r => r)
                .ToListAsync();
        }

        /// <summary>
        /// Ottiene tutti i moduli disponibili
        /// </summary>
        public async Task<List<string>> GetAllModuliAsync()
        {
            var configurazioni = await _repository.GetAllConfigurazioniAsync();
            return configurazioni
                .Select(c => c.Modulo)
                .Distinct()
                .OrderBy(m => m)
                .ToList();
        }
    }
}