using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text;
using Trasformazioni.Data.Repositories;
using Trasformazioni.Mappings;
using Trasformazioni.Models.DTOs;
using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;
using Trasformazioni.Models.ViewModels;
using Trasformazioni.Services.Interfaces;

namespace Trasformazioni.Services
{
    /// <summary>
    /// Implementazione della business logic per la gestione Registro Contratti
    /// </summary>
    public class RegistroContrattiService : IRegistroContrattiService
    {
        private readonly IRegistroContrattiRepository _registroRepository;
        private readonly ICategoriaContrattoRepository _categoriaRepository;
        private readonly ISoggettoRepository _soggettoRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegistroContrattiService> _logger;

        public RegistroContrattiService(
            IRegistroContrattiRepository registroRepository,
            ICategoriaContrattoRepository categoriaRepository,
            ISoggettoRepository soggettoRepository,
            UserManager<ApplicationUser> userManager,
            ILogger<RegistroContrattiService> logger)
        {
            _registroRepository = registroRepository;
            _categoriaRepository = categoriaRepository;
            _soggettoRepository = soggettoRepository;
            _userManager = userManager;
            _logger = logger;
        }

        // ===================================
        // QUERY - LETTURA
        // ===================================

        /// <summary>
        /// Ottiene tutti i registri
        /// </summary>
        public async Task<IEnumerable<RegistroContrattiListViewModel>> GetAllAsync()
        {
            try
            {
                var registri = await _registroRepository.GetAllAsync();
                return registri.ToListViewModels();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero di tutti i registri");
                throw;
            }
        }

        /// <summary>
        /// Ottiene il dettaglio di un registro
        /// </summary>
        public async Task<RegistroContrattiDetailsViewModel?> GetByIdAsync(Guid id)
        {
            try
            {
                var registro = await _registroRepository.GetByIdWithAllegatiAsync(id);
                return registro?.ToDetailsViewModel();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero del registro {RegistroId}", id);
                throw;
            }
        }

        /// <summary>
        /// Ottiene un registro per modifica
        /// </summary>
        public async Task<RegistroContrattiEditViewModel?> GetForEditAsync(Guid id)
        {
            try
            {
                var registro = await _registroRepository.GetByIdAsync(id);
                return registro?.ToEditViewModel();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero del registro per modifica {RegistroId}", id);
                throw;
            }
        }

        /// <summary>
        /// Ottiene registri paginati con filtri
        /// </summary>
        public async Task<PagedResult<RegistroContrattiListViewModel>> GetPagedAsync(RegistroContrattiFilterViewModel filters)
        {
            try
            {
                // Validazione parametri
                if (filters.PageNumber < 1) filters.PageNumber = 1;
                if (filters.PageSize < 1) filters.PageSize = 20;
                if (filters.PageSize > 100) filters.PageSize = 100;

                // Gestione filtri speciali
                StatoRegistro? statoFiltro = filters.Stato;
                if (filters.SoloInScadenza)
                {
                    statoFiltro = StatoRegistro.InScadenza;
                }
                else if (filters.SoloScaduti)
                {
                    statoFiltro = StatoRegistro.Scaduto;
                }

                var (items, totalCount) = await _registroRepository.GetPagedAsync(
                    filters.PageNumber,
                    filters.PageSize,
                    filters.TipoRegistro,
                    statoFiltro,
                    filters.ClienteId,
                    filters.CategoriaContrattoId,
                    filters.SoloRoot,
                    filters.SoloConVersioni,
                    filters.UtenteId,
                    filters.SearchTerm,
                    filters.OrderBy,
                    filters.OrderDirection);

                var viewModels = items.ToListViewModels().ToList();

                return new PagedResult<RegistroContrattiListViewModel>
                {
                    Items = viewModels,
                    PageNumber = filters.PageNumber,
                    PageSize = filters.PageSize,
                    TotalItems = totalCount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero paginato dei registri");
                throw;
            }
        }

        // ===================================
        // QUERY PER TIPO
        // ===================================

        /// <summary>
        /// Ottiene tutti i preventivi
        /// </summary>
        public async Task<IEnumerable<RegistroContrattiListViewModel>> GetPreventiviAsync()
        {
            try
            {
                var registri = await _registroRepository.GetPreventiviAsync();
                return registri.ToListViewModels();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei preventivi");
                throw;
            }
        }

        /// <summary>
        /// Ottiene tutti i contratti
        /// </summary>
        public async Task<IEnumerable<RegistroContrattiListViewModel>> GetContrattiAsync()
        {
            try
            {
                var registri = await _registroRepository.GetContrattiAsync();
                return registri.ToListViewModels();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei contratti");
                throw;
            }
        }

        // ===================================
        // QUERY PER STATO
        // ===================================

        /// <summary>
        /// Ottiene registri attivi
        /// </summary>
        public async Task<IEnumerable<RegistroContrattiListViewModel>> GetAttiviAsync()
        {
            try
            {
                var registri = await _registroRepository.GetAttiviAsync();
                return registri.ToListViewModels();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei registri attivi");
                throw;
            }
        }

        /// <summary>
        /// Ottiene registri in scadenza
        /// </summary>
        public async Task<IEnumerable<RegistroContrattiListViewModel>> GetInScadenzaAsync()
        {
            try
            {
                var registri = await _registroRepository.GetInScadenzaAsync();
                return registri.ToListViewModels();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei registri in scadenza");
                throw;
            }
        }

        /// <summary>
        /// Ottiene registri scaduti
        /// </summary>
        public async Task<IEnumerable<RegistroContrattiListViewModel>> GetScadutiAsync()
        {
            try
            {
                var registri = await _registroRepository.GetScadutiAsync();
                return registri.ToListViewModels();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei registri scaduti");
                throw;
            }
        }

        // ===================================
        // QUERY PER RELAZIONI
        // ===================================

        /// <summary>
        /// Ottiene registri per cliente
        /// </summary>
        public async Task<IEnumerable<RegistroContrattiListViewModel>> GetByClienteIdAsync(Guid clienteId)
        {
            try
            {
                var registri = await _registroRepository.GetByClienteIdAsync(clienteId);
                return registri.ToListViewModels();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei registri per cliente {ClienteId}", clienteId);
                throw;
            }
        }

        /// <summary>
        /// Ottiene registri per categoria
        /// </summary>
        public async Task<IEnumerable<RegistroContrattiListViewModel>> GetByCategoriaIdAsync(Guid categoriaId)
        {
            try
            {
                var registri = await _registroRepository.GetByCategoriaIdAsync(categoriaId);
                return registri.ToListViewModels();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei registri per categoria {CategoriaId}", categoriaId);
                throw;
            }
        }

        /// <summary>
        /// Ottiene registri per responsabile
        /// </summary>
        public async Task<IEnumerable<RegistroContrattiListViewModel>> GetByUtenteIdAsync(string utenteId)
        {
            try
            {
                var registri = await _registroRepository.GetByUtenteIdAsync(utenteId);
                return registri.ToListViewModels();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei registri per utente {UtenteId}", utenteId);
                throw;
            }
        }

        // ===================================
        // COMANDI - SCRITTURA
        // ===================================

        /// <summary>
        /// Crea un nuovo registro
        /// </summary>
        public async Task<(bool Success, string? ErrorMessage, Guid? RegistroId)> CreateAsync(
            RegistroContrattiCreateViewModel model,
            string currentUserId)
        {
            try
            {
                // Genera protocollo automatico se non specificato
                if (string.IsNullOrWhiteSpace(model.NumeroProtocollo))
                {
                    model.NumeroProtocollo = await GeneraNumeroProtocolloAsync(model.TipoRegistro);
                }
                else
                {
                    // Verifica unicità del protocollo inserito manualmente
                    if (!await IsNumeroProtocolloUniqueAsync(model.NumeroProtocollo))
                    {
                        return (false, "Il numero protocollo inserito è già in uso", null);
                    }
                }

                // 1. Validazione
                var (isValid, errors) = await ValidateAsync(model);
                if (!isValid)
                {
                    return (false, string.Join("; ", errors), null);
                }

                // 2. Mapping
                var registro = model.ToEntity();

                // 3. Popola dati derivati da Cliente
                if (model.ClienteId.HasValue)
                {
                    var cliente = await _soggettoRepository.GetByIdAsync(model.ClienteId.Value);
                    registro.PopolaDaCliente(cliente);
                }

                // 4. Popola dati derivati da Utente
                if (!string.IsNullOrEmpty(model.UtenteId))
                {
                    var utente = await _userManager.FindByIdAsync(model.UtenteId);
                    registro.PopolaDaUtente(utente);
                }

                // 5. Salvataggio
                await _registroRepository.AddAsync(registro);
                await _registroRepository.SaveChangesAsync();

                _logger.LogInformation(
                    "Registro creato con successo - ID: {RegistroId}, Tipo: {Tipo}, Oggetto: {Oggetto}",
                    registro.Id, registro.TipoRegistro, registro.Oggetto);

                return (true, null, registro.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la creazione del registro");
                return (false, "Errore durante la creazione del registro", null);
            }
        }

        /// <summary>
        /// Aggiorna un registro esistente
        /// </summary>
        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(
            RegistroContrattiEditViewModel model,
            string currentUserId)
        {
            try
            {
                // 1. Verifica esistenza
                var registro = await _registroRepository.GetByIdAsync(model.Id);
                if (registro == null)
                {
                    return (false, "Registro non trovato");
                }

                // 2. Verifica modificabilità
                if (registro.Stato == StatoRegistro.Annullato || registro.Stato == StatoRegistro.Rinnovato)
                {
                    return (false, "Non è possibile modificare un registro annullato o rinnovato");
                }

                // 3. Validazione numero protocollo
                if (!string.IsNullOrWhiteSpace(model.NumeroProtocollo))
                {
                    if (!await IsNumeroProtocolloUniqueAsync(model.NumeroProtocollo, model.Id))
                    {
                        return (false, "Numero protocollo già esistente");
                    }
                }

                // 4. Aggiornamento
                model.UpdateEntity(registro);

                // 5. Aggiorna dati derivati da Cliente
                if (model.ClienteId.HasValue)
                {
                    var cliente = await _soggettoRepository.GetByIdAsync(model.ClienteId.Value);
                    registro.PopolaDaCliente(cliente);
                }
                else
                {
                    // Mantieni ragione sociale manuale se non c'è cliente selezionato
                    registro.RagioneSociale = model.RagioneSociale;
                    registro.TipoControparte = model.TipoControparte;
                }

                // 6. Aggiorna dati derivati da Utente
                if (!string.IsNullOrEmpty(model.UtenteId))
                {
                    var utente = await _userManager.FindByIdAsync(model.UtenteId);
                    registro.PopolaDaUtente(utente);
                }
                else
                {
                    registro.ResponsabileInterno = null;
                }

                await _registroRepository.UpdateAsync(registro);
                await _registroRepository.SaveChangesAsync();

                _logger.LogInformation(
                    "Registro aggiornato con successo - ID: {RegistroId}",
                    registro.Id);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'aggiornamento del registro {RegistroId}", model.Id);
                return (false, "Errore durante l'aggiornamento del registro");
            }
        }

        /// <summary>
        /// Elimina un registro (soft delete)
        /// </summary>
        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(Guid id, string currentUserId)
        {
            try
            {
                var registro = await _registroRepository.GetByIdAsync(id);
                if (registro == null)
                {
                    return (false, "Registro non trovato");
                }

                // Verifica eliminabilità
                if (registro.Stato != StatoRegistro.Bozza)
                {
                    return (false, "È possibile eliminare solo registri in stato Bozza");
                }

                // Verifica se ha figli
                var children = await _registroRepository.GetChildrenAsync(id);
                if (children.Any())
                {
                    return (false, "Non è possibile eliminare un registro che ha versioni successive");
                }

                await _registroRepository.DeleteAsync(registro);
                await _registroRepository.SaveChangesAsync();

                _logger.LogInformation("Registro eliminato con successo - ID: {RegistroId}", id);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'eliminazione del registro {RegistroId}", id);
                return (false, "Errore durante l'eliminazione del registro");
            }
        }

        // ===================================
        // GESTIONE STATI
        // ===================================

        /// <summary>
        /// Cambia lo stato di un registro
        /// </summary>
        public async Task<(bool Success, string? ErrorMessage)> CambiaStatoAsync(
            Guid id,
            StatoRegistro nuovoStato,
            string currentUserId)
        {
            try
            {
                var registro = await _registroRepository.GetByIdAsync(id);
                if (registro == null)
                {
                    return (false, "Registro non trovato");
                }

                // Validazione transizione stato
                var (canChange, errorMessage) = ValidateTransizioneStato(registro.Stato, nuovoStato);
                if (!canChange)
                {
                    return (false, errorMessage);
                }

                registro.Stato = nuovoStato;
                await _registroRepository.UpdateAsync(registro);
                await _registroRepository.SaveChangesAsync();

                _logger.LogInformation(
                    "Stato registro cambiato - ID: {RegistroId}, Da: {OldStato}, A: {NewStato}",
                    id, registro.Stato, nuovoStato);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il cambio stato del registro {RegistroId}", id);
                return (false, "Errore durante il cambio stato");
            }
        }

        /// <summary>
        /// Invia il documento al cliente
        /// </summary>
        public async Task<(bool Success, string? ErrorMessage)> InviaAsync(Guid id, string currentUserId)
        {
            try
            {
                var registro = await _registroRepository.GetByIdAsync(id);
                if (registro == null)
                {
                    return (false, "Registro non trovato");
                }

                if (registro.Stato != StatoRegistro.Bozza && registro.Stato != StatoRegistro.InRevisione)
                {
                    return (false, "È possibile inviare solo documenti in Bozza o In Revisione");
                }

                registro.Stato = StatoRegistro.Inviato;
                registro.DataInvio = DateTime.Now;

                await _registroRepository.UpdateAsync(registro);
                await _registroRepository.SaveChangesAsync();

                _logger.LogInformation("Registro inviato - ID: {RegistroId}", id);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'invio del registro {RegistroId}", id);
                return (false, "Errore durante l'invio");
            }
        }

        /// <summary>
        /// Attiva un contratto
        /// </summary>
        public async Task<(bool Success, string? ErrorMessage)> AttivaAsync(
            Guid id,
            DateTime? dataAccettazione,
            string currentUserId)
        {
            try
            {
                var registro = await _registroRepository.GetByIdAsync(id);
                if (registro == null)
                {
                    return (false, "Registro non trovato");
                }

                if (registro.Stato != StatoRegistro.Inviato)
                {
                    return (false, "È possibile attivare solo documenti Inviati");
                }

                registro.Stato = StatoRegistro.Attivo;
                registro.DataAccettazione = dataAccettazione ?? DateTime.Now;

                await _registroRepository.UpdateAsync(registro);
                await _registroRepository.SaveChangesAsync();

                _logger.LogInformation("Registro attivato - ID: {RegistroId}", id);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'attivazione del registro {RegistroId}", id);
                return (false, "Errore durante l'attivazione");
            }
        }

        /// <summary>
        /// Sospende un contratto
        /// </summary>
        public async Task<(bool Success, string? ErrorMessage)> SospendiAsync(Guid id, string currentUserId)
        {
            try
            {
                var registro = await _registroRepository.GetByIdAsync(id);
                if (registro == null)
                {
                    return (false, "Registro non trovato");
                }

                if (registro.Stato != StatoRegistro.Attivo)
                {
                    return (false, "È possibile sospendere solo contratti Attivi");
                }

                registro.Stato = StatoRegistro.Sospeso;

                await _registroRepository.UpdateAsync(registro);
                await _registroRepository.SaveChangesAsync();

                _logger.LogInformation("Registro sospeso - ID: {RegistroId}", id);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la sospensione del registro {RegistroId}", id);
                return (false, "Errore durante la sospensione");
            }
        }

        /// <summary>
        /// Riattiva un contratto sospeso
        /// </summary>
        public async Task<(bool Success, string? ErrorMessage)> RiattivaAsync(Guid id, string currentUserId)
        {
            try
            {
                var registro = await _registroRepository.GetByIdAsync(id);
                if (registro == null)
                {
                    return (false, "Registro non trovato");
                }

                if (registro.Stato != StatoRegistro.Sospeso)
                {
                    return (false, "È possibile riattivare solo contratti Sospesi");
                }

                registro.Stato = StatoRegistro.Attivo;

                await _registroRepository.UpdateAsync(registro);
                await _registroRepository.SaveChangesAsync();

                _logger.LogInformation("Registro riattivato - ID: {RegistroId}", id);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la riattivazione del registro {RegistroId}", id);
                return (false, "Errore durante la riattivazione");
            }
        }

        /// <summary>
        /// Annulla un documento
        /// </summary>
        public async Task<(bool Success, string? ErrorMessage)> AnnullaAsync(Guid id, string currentUserId)
        {
            try
            {
                var registro = await _registroRepository.GetByIdAsync(id);
                if (registro == null)
                {
                    return (false, "Registro non trovato");
                }

                if (registro.Stato == StatoRegistro.Annullato ||
                    registro.Stato == StatoRegistro.Rinnovato ||
                    registro.Stato == StatoRegistro.Scaduto)
                {
                    return (false, "Non è possibile annullare questo documento");
                }

                registro.Stato = StatoRegistro.Annullato;

                await _registroRepository.UpdateAsync(registro);
                await _registroRepository.SaveChangesAsync();

                _logger.LogInformation("Registro annullato - ID: {RegistroId}", id);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'annullamento del registro {RegistroId}", id);
                return (false, "Errore durante l'annullamento");
            }
        }

        /// <summary>
        /// Propone il rinnovo per un contratto in scadenza
        /// </summary>
        public async Task<(bool Success, string? ErrorMessage)> ProponiRinnovoAsync(Guid id, string currentUserId)
        {
            try
            {
                var registro = await _registroRepository.GetByIdAsync(id);
                if (registro == null)
                {
                    return (false, "Registro non trovato");
                }

                if (registro.Stato != StatoRegistro.InScadenza)
                {
                    return (false, "È possibile proporre il rinnovo solo per contratti In Scadenza");
                }

                registro.Stato = StatoRegistro.InScadenzaPropostoRinnovo;

                await _registroRepository.UpdateAsync(registro);
                await _registroRepository.SaveChangesAsync();

                _logger.LogInformation("Proposto rinnovo per registro - ID: {RegistroId}", id);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la proposta di rinnovo del registro {RegistroId}", id);
                return (false, "Errore durante la proposta di rinnovo");
            }
        }

        // ===================================
        // RINNOVO
        // ===================================

        /// <summary>
        /// Rinnova un contratto creando una nuova versione
        /// </summary>
        public async Task<(bool Success, string? ErrorMessage, Guid? NuovoRegistroId)> RinnovaAsync(
            Guid id,
            int? giorniRinnovo,
            string currentUserId)
        {
            try
            {
                var registroOriginale = await _registroRepository.GetByIdAsync(id);
                if (registroOriginale == null)
                {
                    return (false, "Registro non trovato", null);
                }

                // Verifica stato
                if (registroOriginale.Stato != StatoRegistro.Attivo &&
                    registroOriginale.Stato != StatoRegistro.InScadenza &&
                    registroOriginale.Stato != StatoRegistro.InScadenzaPropostoRinnovo)
                {
                    return (false, "Non è possibile rinnovare questo contratto", null);
                }

                // Crea nuovo registro come rinnovo
                var nuovoRegistro = registroOriginale.ToRinnovoEntity(giorniRinnovo);
                nuovoRegistro.NumeroProtocollo = await GeneraNumeroProtocolloAsync(nuovoRegistro.TipoRegistro);

                // Aggiorna stato originale
                registroOriginale.Stato = StatoRegistro.Rinnovato;

                // Salva entrambi
                await _registroRepository.AddAsync(nuovoRegistro);
                await _registroRepository.UpdateAsync(registroOriginale);
                await _registroRepository.SaveChangesAsync();

                _logger.LogInformation(
                    "Registro rinnovato - Originale: {OriginalId}, Nuovo: {NewId}",
                    id, nuovoRegistro.Id);

                return (true, null, nuovoRegistro.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il rinnovo del registro {RegistroId}", id);
                return (false, "Errore durante il rinnovo", null);
            }
        }

        /// <summary>
        /// Prepara il ViewModel per il rinnovo
        /// </summary>
        public async Task<RegistroContrattiCreateViewModel?> PrepareRinnovoViewModelAsync(Guid id)
        {
            try
            {
                var registro = await _registroRepository.GetByIdAsync(id);
                if (registro == null)
                    return null;

                var durata = registro.GiorniRinnovoAutomatico ?? 365;
                var nuovaDecorrenza = registro.DataFineOScadenza?.AddDays(1) ?? DateTime.Now.Date;

                var viewModel = new RegistroContrattiCreateViewModel
                {
                    IdRiferimentoEsterno = registro.IdRiferimentoEsterno,
                    TipoRegistro = registro.TipoRegistro,
                    ClienteId = registro.ClienteId,
                    RagioneSociale = registro.RagioneSociale,
                    TipoControparte = registro.TipoControparte,
                    Oggetto = registro.Oggetto,
                    CategoriaContrattoId = registro.CategoriaContrattoId,
                    UtenteId = registro.UtenteId,
                    DataDocumento = DateTime.Now.Date,
                    DataDecorrenza = nuovaDecorrenza,
                    DataFineOScadenza = nuovaDecorrenza.AddDays(durata),
                    GiorniPreavvisoDisdetta = registro.GiorniPreavvisoDisdetta,
                    GiorniAlertScadenza = registro.GiorniAlertScadenza,
                    IsRinnovoAutomatico = registro.IsRinnovoAutomatico,
                    GiorniRinnovoAutomatico = registro.GiorniRinnovoAutomatico,
                    ImportoCanoneAnnuo = registro.ImportoCanoneAnnuo,
                    ParentId = registro.Id,
                    ParentNumeroProtocollo = registro.NumeroProtocollo,
                    ParentOggetto = registro.Oggetto
                };

                // Popola dropdown
                await PopolaDropdownCreateAsync(viewModel);

                return viewModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la preparazione del rinnovo per {RegistroId}", id);
                throw;
            }
        }

        // ===================================
        // VALIDAZIONI
        // ===================================

        /// <summary>
        /// Valida i dati di un registro
        /// </summary>
        public async Task<(bool IsValid, List<string> Errors)> ValidateAsync(
            RegistroContrattiCreateViewModel model,
            Guid? excludeId = null)
        {
            var errors = new List<string>();

            // Oggetto obbligatorio
            if (string.IsNullOrWhiteSpace(model.Oggetto))
            {
                errors.Add("L'oggetto è obbligatorio");
            }

            // Categoria obbligatoria
            if (model.CategoriaContrattoId == Guid.Empty)
            {
                errors.Add("La categoria è obbligatoria");
            }
            else
            {
                var categoriaExists = await _categoriaRepository.ExistsAsync(model.CategoriaContrattoId);
                if (!categoriaExists)
                {
                    errors.Add("Categoria non valida");
                }
            }

            // Numero protocollo univoco
            if (!string.IsNullOrWhiteSpace(model.NumeroProtocollo))
            {
                if (!await IsNumeroProtocolloUniqueAsync(model.NumeroProtocollo, excludeId))
                {
                    errors.Add("Numero protocollo già esistente");
                }
            }

            // Cliente valido (se specificato)
            if (model.ClienteId.HasValue)
            {
                var clienteExists = await _soggettoRepository.ExistsAsync(model.ClienteId.Value);
                if (!clienteExists)
                {
                    errors.Add("Cliente non valido");
                }
            }

            // Validazione date
            if (model.DataDecorrenza.HasValue && model.DataFineOScadenza.HasValue)
            {
                if (model.DataDecorrenza.Value > model.DataFineOScadenza.Value)
                {
                    errors.Add("La data di decorrenza non può essere successiva alla data di scadenza");
                }
            }

            // Validazione rinnovo automatico
            if (model.IsRinnovoAutomatico && !model.GiorniRinnovoAutomatico.HasValue)
            {
                errors.Add("Specificare i giorni di rinnovo automatico");
            }

            return (!errors.Any(), errors);
        }

        /// <summary>
        /// Verifica se il numero protocollo è univoco
        /// </summary>
        public async Task<bool> IsNumeroProtocolloUniqueAsync(string numeroProtocollo, Guid? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(numeroProtocollo))
                return true;

            var exists = await _registroRepository.ExistsByNumeroProtocolloAsync(numeroProtocollo, excludeId);
            return !exists;
        }

        /// <summary>
        /// Verifica se un registro esiste
        /// </summary>
        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _registroRepository.ExistsAsync(id);
        }

        /// <summary>
        /// Genera un nuovo numero protocollo automatico
        /// </summary>
        public async Task<string> GeneraNumeroProtocolloAsync(TipoRegistro tipo)
        {
            var prefisso = tipo == TipoRegistro.Preventivo ? "PREV" : "CONTR";
            var anno = DateTime.Now.Year;
            var progressivo = await _registroRepository.GetNextProgressivoProtocolloAsync();

            return $"{prefisso}-{anno}-{progressivo:D4}";
        }

        // ===================================
        // OPERAZIONI BATCH (per job)
        // ===================================

        ///// <summary>
        ///// Aggiorna gli stati in base alle scadenze
        ///// </summary>
        //public async Task<int> AggiornaStatiScadenzaAsync()
        //{
        //    var count = 0;

        //    try
        //    {
        //        // 1. Registri da mettere in scadenza
        //        var registriPerAlert = await _registroRepository.GetRegistriPerAlertScadenzaAsync();
        //        foreach (var registro in registriPerAlert)
        //        {
        //            registro.Stato = StatoRegistro.InScadenza;
        //            await _registroRepository.UpdateAsync(registro);
        //            count++;
        //        }

        //        // 2. Registri da marcare come scaduti
        //        var registriDaScadere = await _registroRepository.GetRegistriDaScadereAsync();
        //        foreach (var registro in registriDaScadere)
        //        {
        //            registro.Stato = StatoRegistro.Scaduto;
        //            await _registroRepository.UpdateAsync(registro);
        //            count++;
        //        }

        //        if (count > 0)
        //        {
        //            await _registroRepository.SaveChangesAsync();
        //            _logger.LogInformation("Aggiornati {Count} stati scadenza", count);
        //        }

        //        return count;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Errore durante l'aggiornamento degli stati scadenza");
        //        throw;
        //    }
        //}

        ///// <summary>
        ///// Processa i rinnovi automatici
        ///// </summary>
        //public async Task<int> ProcessaRinnoviAutomaticiAsync()
        //{
        //    var count = 0;

        //    try
        //    {
        //        var registriPerRinnovo = await _registroRepository.GetRegistriPerRinnovoAutomaticoAsync();

        //        foreach (var registro in registriPerRinnovo)
        //        {
        //            var (success, _, _) = await RinnovaAsync(
        //                registro.Id,
        //                registro.GiorniRinnovoAutomatico,
        //                "SYSTEM");

        //            if (success)
        //                count++;
        //        }

        //        _logger.LogInformation("Processati {Count} rinnovi automatici", count);

        //        return count;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Errore durante il processamento dei rinnovi automatici");
        //        throw;
        //    }
        //}

        /// <summary>
        /// Aggiorna gli stati in base alle scadenze
        /// </summary>
        public async Task<List<RegistroStatoChangeResult>> AggiornaStatiScadenzaAsync()
        {
            var results = new List<RegistroStatoChangeResult>();

            try
            {
                // 1. Registri da mettere in scadenza
                var registriPerAlert = await _registroRepository.GetRegistriPerAlertScadenzaAsync();
                foreach (var registro in registriPerAlert)
                {
                    var statoPrecedente = registro.Stato;
                    registro.Stato = StatoRegistro.InScadenza;
                    await _registroRepository.UpdateAsync(registro);

                    results.Add(new RegistroStatoChangeResult
                    {
                        RegistroId = registro.Id,
                        NumeroProtocollo = registro.NumeroProtocollo,
                        Oggetto = registro.Oggetto,
                        RagioneSociale = registro.RagioneSociale ?? registro.Cliente?.NomeCompleto,
                        DataScadenza = registro.DataFineOScadenza,
                        StatoPrecedente = statoPrecedente,
                        NuovoStato = StatoRegistro.InScadenza
                    });
                }

                // 2. Registri da marcare come scaduti
                var registriDaScadere = await _registroRepository.GetRegistriDaScadereAsync();
                foreach (var registro in registriDaScadere)
                {
                    var statoPrecedente = registro.Stato;
                    registro.Stato = StatoRegistro.Scaduto;
                    await _registroRepository.UpdateAsync(registro);

                    results.Add(new RegistroStatoChangeResult
                    {
                        RegistroId = registro.Id,
                        NumeroProtocollo = registro.NumeroProtocollo,
                        Oggetto = registro.Oggetto,
                        RagioneSociale = registro.RagioneSociale ?? registro.Cliente?.NomeCompleto,
                        DataScadenza = registro.DataFineOScadenza,
                        StatoPrecedente = statoPrecedente,
                        NuovoStato = StatoRegistro.Scaduto
                    });
                }

                if (results.Any())
                {
                    await _registroRepository.SaveChangesAsync();
                    _logger.LogInformation("Aggiornati {Count} stati scadenza", results.Count);
                }

                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'aggiornamento degli stati scadenza");
                throw;
            }
        }

        /// <summary>
        /// Processa i rinnovi automatici
        /// </summary>
        public async Task<List<RegistroStatoChangeResult>> ProcessaRinnoviAutomaticiAsync()
        {
            var results = new List<RegistroStatoChangeResult>();

            try
            {
                var registriPerRinnovo = await _registroRepository.GetRegistriPerRinnovoAutomaticoAsync();

                foreach (var registro in registriPerRinnovo)
                {
                    var (success, _, nuovoRegistroId) = await RinnovaAsync(
                        registro.Id,
                        registro.GiorniRinnovoAutomatico,
                        "SYSTEM");

                    if (success && nuovoRegistroId.HasValue)
                    {
                        // Recupera il nuovo registro per ottenere il protocollo
                        var nuovoRegistro = await _registroRepository.GetByIdAsync(nuovoRegistroId.Value);

                        results.Add(new RegistroStatoChangeResult
                        {
                            RegistroId = registro.Id,
                            NumeroProtocollo = registro.NumeroProtocollo,
                            Oggetto = registro.Oggetto,
                            RagioneSociale = registro.RagioneSociale ?? registro.Cliente?.NomeCompleto,
                            DataScadenza = registro.DataFineOScadenza,
                            StatoPrecedente = StatoRegistro.InScadenza,
                            NuovoStato = StatoRegistro.Rinnovato,
                            NuovoRegistroId = nuovoRegistroId,
                            NuovoNumeroProtocollo = nuovoRegistro?.NumeroProtocollo
                        });
                    }
                }

                _logger.LogInformation("Processati {Count} rinnovi automatici", results.Count);

                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il processamento dei rinnovi automatici");
                throw;
            }
        }

        // ===================================
        // STATISTICHE
        // ===================================

        /// <summary>
        /// Ottiene statistiche generali sui registri
        /// </summary>
        public async Task<RegistroContrattiStatisticheViewModel> GetStatisticheAsync()
        {
            try
            {
                return new RegistroContrattiStatisticheViewModel
                {
                    TotaleRegistri = await _registroRepository.CountAsync(),
                    TotalePreventivi = await _registroRepository.CountByTipoAsync(TipoRegistro.Preventivo),
                    TotaleContratti = await _registroRepository.CountByTipoAsync(TipoRegistro.Contratto),
                    ContrattiAttivi = await _registroRepository.CountByStatoAsync(StatoRegistro.Attivo),
                    ContrattiInScadenza = await _registroRepository.CountByStatoAsync(StatoRegistro.InScadenza) +
                                          await _registroRepository.CountByStatoAsync(StatoRegistro.InScadenzaPropostoRinnovo),
                    ContrattiScaduti = await _registroRepository.CountByStatoAsync(StatoRegistro.Scaduto),
                    TotaleCanoneAnnuoAttivi = await _registroRepository.SumImportoCanoneByStatoAsync(StatoRegistro.Attivo),
                    TotaleCanoneAnnuoInScadenza = await _registroRepository.SumImportoCanoneByStatoAsync(StatoRegistro.InScadenza),
                    PreventiviInBozza = await _registroRepository.CountByStatoAsync(StatoRegistro.Bozza),
                    PreventiviInviati = await _registroRepository.CountByStatoAsync(StatoRegistro.Inviato)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero delle statistiche");
                throw;
            }
        }

        /// <summary>
        /// Ottiene statistiche per un cliente
        /// </summary>
        public async Task<RegistroContrattiStatisticheClienteViewModel> GetStatisticheClienteAsync(Guid clienteId)
        {
            try
            {
                var cliente = await _soggettoRepository.GetByIdAsync(clienteId);
                var registri = await _registroRepository.GetByClienteIdAsync(clienteId);

                return new RegistroContrattiStatisticheClienteViewModel
                {
                    ClienteId = clienteId,
                    RagioneSociale = cliente?.NomeCompleto ?? "N/D",
                    TotaleRegistri = registri.Count(),
                    ContrattiAttivi = registri.Count(r => r.Stato == StatoRegistro.Attivo),
                    ContrattiInScadenza = registri.Count(r =>
                        r.Stato == StatoRegistro.InScadenza ||
                        r.Stato == StatoRegistro.InScadenzaPropostoRinnovo),
                    TotaleCanoneAnnuo = await _registroRepository.SumImportoCanoneByClienteAsync(clienteId)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero delle statistiche cliente {ClienteId}", clienteId);
                throw;
            }
        }

        // ===================================
        // DROPDOWN HELPERS
        // ===================================

        /// <summary>
        /// Prepara il ViewModel per la creazione con tutti i dropdown
        /// </summary>
        public async Task<RegistroContrattiCreateViewModel> PrepareCreateViewModelAsync(Guid? parentId = null)
        {
            var viewModel = new RegistroContrattiCreateViewModel
            {
                DataDocumento = DateTime.Now.Date,
                GiorniAlertScadenza = 60,
                ParentId = parentId
            };

            // Se ha un parent, popola info
            if (parentId.HasValue)
            {
                var parent = await _registroRepository.GetByIdAsync(parentId.Value);
                if (parent != null)
                {
                    viewModel.ParentNumeroProtocollo = parent.NumeroProtocollo;
                    viewModel.ParentOggetto = parent.Oggetto;
                }
            }

            await PopolaDropdownCreateAsync(viewModel);

            return viewModel;
        }

        /// <summary>
        /// Prepara il ViewModel per la modifica con tutti i dropdown
        /// </summary>
        public async Task<RegistroContrattiEditViewModel?> PrepareEditViewModelAsync(Guid id)
        {
            var viewModel = await GetForEditAsync(id);
            if (viewModel == null)
                return null;

            await PopolaDropdownEditAsync(viewModel);

            return viewModel;
        }

        /// <summary>
        /// Prepara il FilterViewModel con tutti i dropdown
        /// </summary>
        public async Task<RegistroContrattiFilterViewModel> PrepareFilterViewModelAsync(RegistroContrattiFilterViewModel? filters = null)
        {
            var viewModel = filters ?? new RegistroContrattiFilterViewModel();

            await PopolaDropdownFilterAsync(viewModel);

            return viewModel;
        }

        /// <summary>
        /// Prepara il ViewModel per la clonazione di un registro esistente
        /// </summary>
        public async Task<RegistroContrattiCreateViewModel?> PrepareCloneViewModelAsync(Guid id)
        {
            try
            {
                var registro = await _registroRepository.GetByIdAsync(id);
                if (registro == null)
                    return null;

                var viewModel = new RegistroContrattiCreateViewModel
                {
                    // Identificazione - Protocollo vuoto, il resto copiato
                    IdRiferimentoEsterno = null, // Anche questo potrebbe dover essere nuovo
                    NumeroProtocollo = null, // DA NON COPIARE
                    TipoRegistro = registro.TipoRegistro,

                    // Cliente - copiato
                    ClienteId = registro.ClienteId,
                    RagioneSociale = registro.RagioneSociale,
                    TipoControparte = registro.TipoControparte,

                    // Contenuto - copiato
                    Oggetto = registro.Oggetto,
                    CategoriaContrattoId = registro.CategoriaContrattoId,

                    // Responsabile - copiato
                    UtenteId = registro.UtenteId,

                    // Date - data documento = oggi, le altre copiate
                    DataDocumento = DateTime.Now.Date,
                    DataDecorrenza = registro.DataDecorrenza,
                    DataFineOScadenza = registro.DataFineOScadenza,

                    // Scadenze e Rinnovi - copiati
                    GiorniPreavvisoDisdetta = registro.GiorniPreavvisoDisdetta,
                    GiorniAlertScadenza = registro.GiorniAlertScadenza,
                    IsRinnovoAutomatico = registro.IsRinnovoAutomatico,
                    GiorniRinnovoAutomatico = registro.GiorniRinnovoAutomatico,

                    // Importi - copiati
                    ImportoCanoneAnnuo = registro.ImportoCanoneAnnuo,
                    ImportoUnatantum = registro.ImportoUnatantum,

                    // Gerarchia - NESSUN PARENT (documento indipendente)
                    ParentId = null,
                    ParentNumeroProtocollo = null,
                    ParentOggetto = null
                };

                // Popola dropdown
                await PopolaDropdownCreateAsync(viewModel);

                return viewModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la preparazione della clonazione per {RegistroId}", id);
                throw;
            }
        }

        /// <summary>
        /// Esporta i registri attivi e rinnovati in formato CSV
        /// </summary>
        public async Task<string> ExportAttiviCsvAsync()
        {
            try
            {
                var italianCulture = CultureInfo.GetCultureInfo("it-IT");

                // Recupera tutti i registri con stato Attivo o Rinnovato
                var registri = await _registroRepository.GetAllAsync();
                var registriFiltrati = registri
                    .Where(r => r.Stato == StatoRegistro.Attivo || r.Stato == StatoRegistro.Rinnovato)
                    .OrderBy(r => r.RagioneSociale)
                    .ThenByDescending(r => r.DataDocumento)
                    .ToList();

                var sb = new StringBuilder();

                // Header
                sb.AppendLine("Id Riferimento;Tipo;Protocollo;Categoria;Oggetto;Ragione Sociale;PIVA/CF;Tipo Cliente;Data Documento;Data Decorrenza;Data Fine/Scadenza;Importo Canone Annuo;Importo Una Tantum");

                // Righe dati
                foreach (var r in registriFiltrati)
                {
                    var idRif = EscapeCsv(r.IdRiferimentoEsterno);
                    var tipo = r.TipoRegistro == TipoRegistro.Preventivo ? "Preventivo" : "Contratto";
                    var protocollo = EscapeCsv(r.NumeroProtocollo);
                    var categoria = EscapeCsv(r.CategoriaContratto?.Nome);
                    var oggetto = EscapeCsv(r.Oggetto);
                    var ragioneSociale = EscapeCsv(r.RagioneSociale ?? r.Cliente?.NomeCompleto);
                    var partitaIva = r.ClienteId.HasValue ? EscapeCsv(r.Cliente?.PartitaIVA ?? r.Cliente?.CodiceFiscale) : "";
                    var tipoCliente = r.TipoControparte.HasValue
                        ? (r.TipoControparte == NaturaGiuridica.PA ? "PA" : "Privato")
                        : (r.Cliente?.NaturaGiuridica == NaturaGiuridica.PA ? "PA" : "Privato");
                    var dataDocumento = r.DataDocumento.ToString("dd/MM/yyyy");
                    var dataDecorrenza = r.DataDecorrenza?.ToString("dd/MM/yyyy") ?? "";
                    var dataFine = r.DataFineOScadenza?.ToString("dd/MM/yyyy") ?? "";
                    var canoneAnnuo = r.ImportoCanoneAnnuo?.ToString("F2", italianCulture) ?? "";
                    var unaTantum = r.ImportoUnatantum?.ToString("F2", italianCulture) ?? "";

                    sb.AppendLine($"{idRif};{tipo};{protocollo};{categoria};{oggetto};{ragioneSociale};{partitaIva};{tipoCliente};{dataDocumento};{dataDecorrenza};{dataFine};{canoneAnnuo};{unaTantum}");
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'esportazione CSV dei registri");
                throw;
            }
        }

        /// <summary>
        /// Escape per valori CSV (gestisce ; e ")
        /// </summary>
        private static string EscapeCsv(string? value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            // Se contiene ; o " o newline, wrappe in doppi apici ed escape i doppi apici
            if (value.Contains(';') || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }

            return value;
        }

        // ===================================
        // METODI PRIVATI
        // ===================================

        private async Task PopolaDropdownCreateAsync(RegistroContrattiCreateViewModel viewModel)
        {
            // Clienti
            var clienti = await _soggettoRepository.GetClientiAsync();
            viewModel.ClientiSelectList = new SelectList(
                clienti.Select(c => new { c.Id, Nome = c.NomeCompleto }),
                "Id", "Nome", viewModel.ClienteId);

            // Categorie
            var categorie = await _categoriaRepository.GetAttiveAsync();
            viewModel.CategorieSelectList = new SelectList(
                categorie.Select(c => new { c.Id, c.Nome }),
                "Id", "Nome", viewModel.CategoriaContrattoId);

            // Utenti
            var utenti = _userManager.Users.Where(u => u.IsAttivo).ToList();
            viewModel.UtentiSelectList = new SelectList(
                utenti.Select(u => new { u.Id, Nome = u.NomeCompleto }),
                "Id", "Nome", viewModel.UtenteId);

            // Tipi Registro
            viewModel.TipiRegistroSelectList = new SelectList(
                Enum.GetValues<TipoRegistro>().Select(t => new
                {
                    Value = (int)t,
                    Text = t == TipoRegistro.Preventivo ? "Preventivo" : "Contratto"
                }),
                "Value", "Text", (int)viewModel.TipoRegistro);

            // Nature Giuridiche
            viewModel.NatureGiuridicheSelectList = new SelectList(
                Enum.GetValues<NaturaGiuridica>().Select(n => new
                {
                    Value = (int)n,
                    Text = n == NaturaGiuridica.PA ? "Pubblica Amministrazione" : "Privato"
                }),
                "Value", "Text", viewModel.TipoControparte.HasValue ? (int)viewModel.TipoControparte.Value : null);


            // Parent disponibili (solo contratti attivi o in scadenza, senza parent loro stessi per evitare catene complesse)
            var potentialiParent = await _registroRepository.GetAllAsync();
            var parentDisponibili = potentialiParent
                .Where(r => //r.TipoRegistro == TipoRegistro.Contratto &&
                            (r.Stato == StatoRegistro.Attivo ||
                             r.Stato == StatoRegistro.InScadenza ||
                             r.Stato == StatoRegistro.InScadenzaPropostoRinnovo ||
                             r.Stato == StatoRegistro.Scaduto) &&
                            r.Id != viewModel.ParentId) // Escludi se stesso in caso di edit
                .OrderByDescending(r => r.DataDocumento)
                .Select(r => new
                {
                    r.Id,
                    Descrizione = $"{(string.IsNullOrEmpty(r.NumeroProtocollo) ? "N/D" : r.NumeroProtocollo)} - {r.Oggetto} ({r.RagioneSociale ?? "N/D"})"
                });

            viewModel.ParentSelectList = new SelectList(
                parentDisponibili,
                "Id", "Descrizione", viewModel.ParentId);
        }

        private async Task PopolaDropdownEditAsync(RegistroContrattiEditViewModel viewModel)
        {
            // Clienti
            var clienti = await _soggettoRepository.GetClientiAsync();
            viewModel.ClientiSelectList = new SelectList(
                clienti.Select(c => new { c.Id, Nome = c.NomeCompleto }),
                "Id", "Nome", viewModel.ClienteId);

            // Categorie (tutte, anche inattive, per edit)
            var categorie = await _categoriaRepository.GetAllAsync();
            viewModel.CategorieSelectList = new SelectList(
                categorie.Select(c => new { c.Id, c.Nome }),
                "Id", "Nome", viewModel.CategoriaContrattoId);

            // Utenti
            var utenti = _userManager.Users.Where(u => u.IsAttivo).ToList();
            viewModel.UtentiSelectList = new SelectList(
                utenti.Select(u => new { u.Id, Nome = u.NomeCompleto }),
                "Id", "Nome", viewModel.UtenteId);

            // Tipi Registro
            viewModel.TipiRegistroSelectList = new SelectList(
                Enum.GetValues<TipoRegistro>().Select(t => new
                {
                    Value = (int)t,
                    Text = t == TipoRegistro.Preventivo ? "Preventivo" : "Contratto"
                }),
                "Value", "Text", (int)viewModel.TipoRegistro);

            // Nature Giuridiche
            viewModel.NatureGiuridicheSelectList = new SelectList(
                Enum.GetValues<NaturaGiuridica>().Select(n => new
                {
                    Value = (int)n,
                    Text = n == NaturaGiuridica.PA ? "Pubblica Amministrazione" : "Privato"
                }),
                "Value", "Text", viewModel.TipoControparte.HasValue ? (int)viewModel.TipoControparte.Value : null);

            // Stati (solo quelli disponibili dalla transizione)
            viewModel.StatiSelectList = new SelectList(
                viewModel.StatiDisponibili.Select(s => new
                {
                    Value = (int)s,
                    Text = GetStatoDescrizione(s)
                }),
                "Value", "Text", (int)viewModel.Stato);
        }

        private async Task PopolaDropdownFilterAsync(RegistroContrattiFilterViewModel viewModel)
        {
            // Clienti
            var clienti = await _soggettoRepository.GetClientiAsync();
            viewModel.ClientiSelectList = new SelectList(
                new[] { new { Id = (Guid?)null, Nome = "-- Tutti --" } }
                    .Concat(clienti.Select(c => new { Id = (Guid?)c.Id, Nome = c.NomeCompleto })),
                "Id", "Nome", viewModel.ClienteId);

            // Categorie
            var categorie = await _categoriaRepository.GetAllAsync();
            viewModel.CategorieSelectList = new SelectList(
                new[] { new { Id = (Guid?)null, Nome = "-- Tutte --" } }
                    .Concat(categorie.Select(c => new { Id = (Guid?)c.Id, c.Nome })),
                "Id", "Nome", viewModel.CategoriaContrattoId);

            // Utenti
            var utenti = _userManager.Users.ToList();
            viewModel.UtentiSelectList = new SelectList(
                new[] { new { Id = (string?)null, Nome = "-- Tutti --" } }
                    .Concat(utenti.Select(u => new { Id = (string?)u.Id, Nome = u.NomeCompleto })),
                "Id", "Nome", viewModel.UtenteId);

            // Tipi Registro
            viewModel.TipiRegistroSelectList = new SelectList(
                new[] { new { Value = (int?)null, Text = "-- Tutti --" } }
                    .Concat(Enum.GetValues<TipoRegistro>().Select(t => new
                    {
                        Value = (int?)t,
                        Text = t == TipoRegistro.Preventivo ? "Preventivo" : "Contratto"
                    })),
                "Value", "Text", viewModel.TipoRegistro.HasValue ? (int)viewModel.TipoRegistro.Value : null);

            // Stati
            viewModel.StatiSelectList = new SelectList(
                new[] { new { Value = (int?)null, Text = "-- Tutti --" } }
                    .Concat(Enum.GetValues<StatoRegistro>().Select(s => new
                    {
                        Value = (int?)s,
                        Text = GetStatoDescrizione(s)
                    })),
                "Value", "Text", viewModel.Stato.HasValue ? (int)viewModel.Stato.Value : null);

            // Order By
            viewModel.OrderBySelectList = new SelectList(
                new[]
                {
                    new { Value = "DataDocumento", Text = "Data Documento" },
                    new { Value = "DataFineOScadenza", Text = "Data Scadenza" },
                    new { Value = "RagioneSociale", Text = "Cliente" },
                    new { Value = "Oggetto", Text = "Oggetto" },
                    new { Value = "Stato", Text = "Stato" },
                    new { Value = "Tipo", Text = "Tipo" },
                    new { Value = "Importo", Text = "Importo" },
                    new { Value = "Categoria", Text = "Categoria" }
                },
                "Value", "Text", viewModel.OrderBy);
        }

        private (bool CanChange, string? ErrorMessage) ValidateTransizioneStato(
            StatoRegistro statoAttuale,
            StatoRegistro nuovoStato)
        {
            // Stesso stato è sempre permesso
            if (statoAttuale == nuovoStato)
                return (true, null);

            // Definizione transizioni valide
            var transizioniValide = new Dictionary<StatoRegistro, StatoRegistro[]>
            {
                { StatoRegistro.Bozza, new[] { StatoRegistro.InRevisione, StatoRegistro.Inviato, StatoRegistro.Annullato } },
                { StatoRegistro.InRevisione, new[] { StatoRegistro.Bozza, StatoRegistro.Inviato, StatoRegistro.Annullato } },
                { StatoRegistro.Inviato, new[] { StatoRegistro.InRevisione, StatoRegistro.Attivo, StatoRegistro.Annullato } },
                { StatoRegistro.Attivo, new[] { StatoRegistro.InScadenza, StatoRegistro.Sospeso, StatoRegistro.Annullato } },
                { StatoRegistro.InScadenza, new[] { StatoRegistro.InScadenzaPropostoRinnovo, StatoRegistro.Attivo, StatoRegistro.Scaduto, StatoRegistro.Annullato } },
                { StatoRegistro.InScadenzaPropostoRinnovo, new[] { StatoRegistro.InScadenza, StatoRegistro.Rinnovato, StatoRegistro.Scaduto, StatoRegistro.Annullato } },
                { StatoRegistro.Scaduto, new[] { StatoRegistro.Rinnovato } },
                { StatoRegistro.Sospeso, new[] { StatoRegistro.Attivo, StatoRegistro.Annullato } },
                { StatoRegistro.Rinnovato, Array.Empty<StatoRegistro>() },
                { StatoRegistro.Annullato, Array.Empty<StatoRegistro>() }
            };

            if (transizioniValide.TryGetValue(statoAttuale, out var statiPermessi))
            {
                if (statiPermessi.Contains(nuovoStato))
                    return (true, null);
            }

            return (false, $"Transizione non permessa da {GetStatoDescrizione(statoAttuale)} a {GetStatoDescrizione(nuovoStato)}");
        }

        private string GetStatoDescrizione(StatoRegistro stato)
        {
            return stato switch
            {
                StatoRegistro.Bozza => "In bozza",
                StatoRegistro.InRevisione => "In revisione",
                StatoRegistro.Inviato => "Inviato",
                StatoRegistro.Attivo => "Attivo",
                StatoRegistro.InScadenza => "In scadenza",
                StatoRegistro.InScadenzaPropostoRinnovo => "In scadenza - Proposto rinnovo",
                StatoRegistro.Scaduto => "Scaduto",
                StatoRegistro.Rinnovato => "Rinnovato",
                StatoRegistro.Annullato => "Annullato",
                StatoRegistro.Sospeso => "Sospeso",
                _ => "Sconosciuto"
            };
        }
    }
}