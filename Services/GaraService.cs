using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Trasformazioni.Data.Repositories;
using Trasformazioni.Mappings;
using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;
using Trasformazioni.Models.ViewModels;
using Trasformazioni.Services.Interfaces;

namespace Trasformazioni.Services
{
    /// <summary>
    /// Implementazione della business logic per la gestione Gare
    /// </summary>
    public class GaraService : IGaraService
    {
        private readonly IGaraRepository _garaRepository;
        private readonly ILottoRepository _lottoRepository;
        private readonly ILogger<GaraService> _logger;

        public GaraService(
            IGaraRepository garaRepository,
            ILottoRepository lottoRepository,
            ILogger<GaraService> logger)
        {
            _garaRepository = garaRepository;
            _lottoRepository = lottoRepository;
            _logger = logger;
        }

        // ===================================
        // QUERY - LETTURA
        // ===================================

        public async Task<IEnumerable<GaraListViewModel>> GetAllAsync()
        {
            try
            {
                var gare = await _garaRepository.GetAllAsync();
                return gare.Select(g => g.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero di tutte le gare");
                throw;
            }
        }

        public async Task<GaraDetailsViewModel?> GetByIdAsync(Guid id)
        {
            try
            {
                var gara = await _garaRepository.GetCompleteAsync(id);
                return gara?.ToDetailsViewModel();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero della gara ID: {Id}", id);
                throw;
            }
        }

        public async Task<GaraEditViewModel?> GetForEditAsync(Guid id)
        {
            try
            {
                var gara = await _garaRepository.GetWithDocumentiRichiestiAsync(id);
                return gara?.ToEditViewModel();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero della gara per modifica ID: {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<GaraListViewModel>> GetByStatoAsync(StatoGara stato)
        {
            try
            {
                var gare = await _garaRepository.GetByStatoAsync(stato);
                return gare.Select(g => g.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero delle gare per stato: {Stato}", stato);
                throw;
            }
        }

        public async Task<IEnumerable<GaraListViewModel>> GetByTipologiaAsync(TipologiaGara tipologia)
        {
            try
            {
                var gare = await _garaRepository.GetByTipologiaAsync(tipologia);
                return gare.Select(g => g.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero delle gare per tipologia: {Tipologia}", tipologia);
                throw;
            }
        }

        public async Task<IEnumerable<GaraListViewModel>> GetActiveGareAsync()
        {
            try
            {
                var gare = await _garaRepository.GetActiveGareAsync();
                return gare.Select(g => g.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero delle gare attive");
                throw;
            }
        }

        public async Task<IEnumerable<GaraListViewModel>> GetGareInScadenzaAsync(int giorniProssimi = 7)
        {
            try
            {
                var gare = await _garaRepository.GetGareInScadenzaAsync(giorniProssimi);
                return gare.Select(g => g.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero delle gare in scadenza");
                throw;
            }
        }

        public async Task<IEnumerable<GaraListViewModel>> SearchAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return Enumerable.Empty<GaraListViewModel>();

                var gare = await _garaRepository.SearchAsync(searchTerm);
                return gare.Select(g => g.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la ricerca gare con termine: {SearchTerm}", searchTerm);
                throw;
            }
        }

        public async Task<PagedResult<GaraListViewModel>> GetPagedAsync(GaraFilterViewModel filters)
        {
            try
            {
                var (items, totalCount) = await _garaRepository.GetPagedAsync(filters);
                var viewModels = items.Select(g =>
                {
                    var vm = g.ToListViewModel();
                    vm.NumeroLotti = _lottoRepository.CountByGaraIdAsync(g.Id).Result;
                    return vm;
                }).ToList();


                return new PagedResult<GaraListViewModel>
                {
                    Items = viewModels,
                    TotalItems = totalCount,
                    PageNumber = filters.PageNumber,
                    PageSize = filters.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero delle gare paginate");
                throw;
            }
        }

        // ===================================
        // COMANDI - SCRITTURA
        // ===================================

        public async Task<(bool Success, string? ErrorMessage, Guid? GaraId)> CreateAsync(GaraCreateViewModel model)
        {
            try
            {
                // 1. Validazione Codice Gara univoco
                if (!await IsCodiceGaraUniqueAsync(model.CodiceGara))
                {
                    return (false, "Il Codice Gara è già presente nel sistema", null);
                }

                // 2. Validazione CIG univoco (se presente)
                if (!string.IsNullOrWhiteSpace(model.CIG))
                {
                    if (!await IsCIGUniqueAsync(model.CIG))
                    {
                        return (false, "Il CIG è già presente nel sistema", null);
                    }
                }

                // 3. Validazione date: DataScadenza > DataPubblicazione
                if (model.DataPubblicazione.HasValue &&
                    model.DataTerminePresentazioneOfferte.HasValue &&
                    model.DataTerminePresentazioneOfferte.Value <= model.DataPubblicazione.Value)
                {
                    return (false, "La data di scadenza offerte deve essere successiva alla data di pubblicazione", null);
                }

                // 4. Validazione date sequenziali
                if (model.DataPubblicazione.HasValue &&
                    model.DataInizioPresentazioneOfferte.HasValue &&
                    model.DataInizioPresentazioneOfferte.Value < model.DataPubblicazione.Value)
                {
                    return (false, "La data inizio presentazione offerte deve essere successiva o uguale alla data di pubblicazione", null);
                }

                if (model.DataInizioPresentazioneOfferte.HasValue &&
                    model.DataTermineRichiestaChiarimenti.HasValue &&
                    model.DataTermineRichiestaChiarimenti.Value < model.DataInizioPresentazioneOfferte.Value)
                {
                    return (false, "La data termine richiesta chiarimenti deve essere successiva o uguale alla data inizio presentazione offerte", null);
                }

                // 5. Mapping e salvataggio
                var gara = model.ToEntity();
                await _garaRepository.AddAsync(gara);

                // 6. Aggiungi documenti richiesti (checklist)
                if (model.DocumentiRichiestiIds != null && model.DocumentiRichiestiIds.Any())
                {
                    await _garaRepository.UpdateDocumentiRichiestiAsync(gara.Id, model.DocumentiRichiestiIds);
                }

                _logger.LogInformation("Gara creata con successo: {CodiceGara} - ID: {Id}", gara.CodiceGara, gara.Id);

                return (true, null, gara.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la creazione della gara {CodiceGara}", model.CodiceGara);
                return (false, "Errore durante la creazione della gara", null);
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(GaraEditViewModel model)
        {
            try
            {
                // 1. Verifica esistenza
                var gara = await _garaRepository.GetByIdAsync(model.Id);
                if (gara == null)
                {
                    return (false, "Gara non trovata");
                }

                // 2. Validazione Codice Gara univoco (escludendo il record corrente)
                if (!await IsCodiceGaraUniqueAsync(model.CodiceGara, model.Id))
                {
                    return (false, "Il Codice Gara è già presente nel sistema");
                }

                // 3. Validazione CIG univoco (se presente)
                if (!string.IsNullOrWhiteSpace(model.CIG))
                {
                    if (!await IsCIGUniqueAsync(model.CIG, model.Id))
                    {
                        return (false, "Il CIG è già presente nel sistema");
                    }
                }

                // 4. Validazione date: DataScadenza > DataPubblicazione
                if (model.DataPubblicazione.HasValue &&
                    model.DataTerminePresentazioneOfferte.HasValue &&
                    model.DataTerminePresentazioneOfferte.Value <= model.DataPubblicazione.Value)
                {
                    return (false, "La data di scadenza offerte deve essere successiva alla data di pubblicazione");
                }

                // 5. Validazione date sequenziali
                if (model.DataPubblicazione.HasValue &&
                    model.DataInizioPresentazioneOfferte.HasValue &&
                    model.DataInizioPresentazioneOfferte.Value < model.DataPubblicazione.Value)
                {
                    return (false, "La data inizio presentazione offerte deve essere successiva o uguale alla data di pubblicazione");
                }

                if (model.DataInizioPresentazioneOfferte.HasValue &&
                    model.DataTermineRichiestaChiarimenti.HasValue &&
                    model.DataTermineRichiestaChiarimenti.Value < model.DataInizioPresentazioneOfferte.Value)
                {
                    return (false, "La data termine richiesta chiarimenti deve essere successiva o uguale alla data inizio presentazione offerte");
                }

                // 6. Aggiorna l'entità
                model.UpdateEntity(gara);
                await _garaRepository.UpdateAsync(gara);

                // 7. Aggiorna documenti richiesti (checklist)
                await _garaRepository.UpdateDocumentiRichiestiAsync(model.Id, model.DocumentiRichiestiIds);

                _logger.LogInformation("Gara aggiornata con successo: {CodiceGara} - ID: {Id}", gara.CodiceGara, gara.Id);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'aggiornamento della gara ID: {Id}", model.Id);
                return (false, "Errore durante l'aggiornamento della gara");
            }
        }

        /// <summary>
        /// Aggiorna solo lo stato di una gara esistente
        /// Metodo ottimizzato per cambi di stato senza modificare altri campi
        /// </summary>
        public async Task<(bool Success, string? ErrorMessage)> UpdateStatoAsync(Guid garaId, StatoGara nuovoStato)
        {
            try
            {
                // 1. Verifica esistenza
                var gara = await _garaRepository.GetByIdAsync(garaId);
                if (gara == null)
                {
                    return (false, "Gara non trovata");
                }

                // 2. Verifica se lo stato è effettivamente cambiato
                if (gara.Stato == nuovoStato)
                {
                    return (true, null); // Nessun cambio necessario
                }

                // 3. Validazioni specifiche per alcuni stati
                switch (nuovoStato)
                {
                    case StatoGara.Conclusa:
                        // Verifica che tutti i lotti siano in stato terminale
                        var lottiNonTerminali = await _lottoRepository.GetLottiNonTerminaliByGaraIdAsync(garaId);
                        if (lottiNonTerminali.Any())
                        {
                            return (false, $"Impossibile concludere la gara. Ci sono ancora {lottiNonTerminali.Count()} lotti non in stato finale.");
                        }
                        break;

                    case StatoGara.ChiusaManualmente:
                        // Nessuna validazione particolare, può essere chiusa manualmente in qualsiasi momento
                        break;

                    case StatoGara.InLavorazione:
                        // Se torna da ChiusaManualmente a InLavorazione, verifica che ci siano lotti attivi
                        if (gara.Stato == StatoGara.ChiusaManualmente)
                        {
                            var hasLotti = await _lottoRepository.HasLottiByGaraIdAsync(garaId);
                            if (!hasLotti)
                            {
                                return (false, "Impossibile riaprire la gara senza lotti associati.");
                            }
                        }
                        break;
                }

                // 4. Aggiorna lo stato
                gara.Stato = nuovoStato;

                // 5. Se viene chiusa manualmente, imposta i campi di chiusura
                if (nuovoStato == StatoGara.ChiusaManualmente && !gara.IsChiusaManualmente)
                {
                    gara.IsChiusaManualmente = true;
                    gara.DataChiusuraManuale = DateTime.Now;
                    // ChiusaDaId verrà impostato automaticamente dall'AuditInterceptor in ModifiedBy
                }

                // 6. Se viene riaperta, resetta i campi di chiusura manuale
                if (gara.IsChiusaManualmente && nuovoStato != StatoGara.ChiusaManualmente)
                {
                    gara.IsChiusaManualmente = false;
                    gara.DataChiusuraManuale = null;
                    gara.ChiusaDaUserId = null;
                    gara.MotivoChiusuraManuale = null;
                }

                await _garaRepository.UpdateAsync(gara);

                _logger.LogInformation(
                    "Stato gara aggiornato: {CodiceGara} - Da {VecchioStato} a {NuovoStato}",
                    gara.CodiceGara,
                    gara.Stato,
                    nuovoStato);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'aggiornamento dello stato della gara ID: {Id}", garaId);
                return (false, "Errore durante l'aggiornamento dello stato della gara");
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(Guid id)
        {
            try
            {
                // 1. Verifica esistenza
                var gara = await _garaRepository.GetWithLottiAsync(id);
                if (gara == null)
                {
                    return (false, "Gara non trovata");
                }

                // 2. Verifica che non abbia lotti (DeleteBehavior.Restrict in LottoConfig)
                if (gara.Lotti != null && gara.Lotti.Any())
                {
                    return (false, "Impossibile eliminare la gara: sono presenti lotti associati. Eliminare prima i lotti.");
                }

                // 3. Elimina (soft delete)
                await _garaRepository.DeleteAsync(id);

                _logger.LogInformation("Gara eliminata con successo: {CodiceGara} - ID: {Id}", gara.CodiceGara, id);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'eliminazione della gara ID: {Id}", id);
                return (false, "Errore durante l'eliminazione della gara");
            }
        }

        // ===================================
        // OPERAZIONI BUSINESS SPECIFICHE
        // ===================================

        public async Task<(bool Success, string? ErrorMessage)> ConcludiGaraAsync(Guid garaId)
        {
            try
            {
                // 1. Verifica esistenza
                var gara = await _garaRepository.GetWithLottiAsync(garaId);
                if (gara == null)
                {
                    return (false, "Gara non trovata");
                }

                // 2. Verifica che tutti i lotti siano in stato terminale
                if (!await AreAllLottiInStatoTerminaleAsync(garaId))
                {
                    return (false, "Impossibile concludere la gara: non tutti i lotti sono in stato terminale (Vinto, Perso, Scartato)");
                }

                // 3. Aggiorna stato gara
                gara.Stato = StatoGara.Conclusa;
                await _garaRepository.UpdateAsync(gara);

                _logger.LogInformation("Gara conclusa con successo: {CodiceGara} - ID: {Id}", gara.CodiceGara, garaId);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la conclusione della gara ID: {Id}", garaId);
                return (false, "Errore durante la conclusione della gara");
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> RiattivaGaraAsync(Guid garaId)
        {
            try
            {
                // 1. Verifica esistenza
                var gara = await _garaRepository.GetByIdAsync(garaId);
                if (gara == null)
                {
                    return (false, "Gara non trovata");
                }

                // 2. Verifica che sia chiusa manualmente
                if (!gara.IsChiusaManualmente)
                {
                    return (false, "La gara non è stata chiusa manualmente");
                }

                // 3. Riattiva
                gara.IsChiusaManualmente = false;
                gara.DataChiusuraManuale = null;
                gara.MotivoChiusuraManuale = null;
                gara.ChiusaDaUserId = null;

                await _garaRepository.UpdateAsync(gara);

                _logger.LogInformation("Gara riattivata con successo: {CodiceGara} - ID: {Id}", gara.CodiceGara, garaId);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la riattivazione della gara ID: {Id}", garaId);
                return (false, "Errore durante la riattivazione della gara");
            }
        }

        // ===================================
        // VALIDAZIONI
        // ===================================

        public async Task<bool> IsCodiceGaraUniqueAsync(string codiceGara, Guid? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(codiceGara))
                return false;

            var exists = await _garaRepository.ExistsByCodiceAsync(codiceGara, excludeId);
            return !exists;
        }

        public async Task<bool> IsCIGUniqueAsync(string cig, Guid? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(cig))
                return true; // CIG opzionale

            var exists = await _garaRepository.ExistsByCIGAsync(cig, excludeId);
            return !exists;
        }

        public async Task<bool> AreAllLottiInStatoTerminaleAsync(Guid garaId)
        {
            try
            {
                var lotti = await _lottoRepository.GetByGaraIdAsync(garaId);

                if (!lotti.Any())
                    return true; // Nessun lotto = ok per concludere

                var statiTerminali = new[] { StatoLotto.Vinto, StatoLotto.Perso, StatoLotto.Scartato };

                return lotti.All(l => statiTerminali.Contains(l.Stato));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la verifica degli stati dei lotti per gara ID: {GaraId}", garaId);
                return false;
            }
        }

        // ===================================
        // STATISTICHE
        // ===================================

        public async Task<Dictionary<StatoGara, int>> GetCountByStatoAsync()
        {
            try
            {
                return await _garaRepository.GetCountByStatoAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero delle statistiche gare per stato");
                throw;
            }
        }

        public async Task<Dictionary<StatoGara, decimal>> GetImportoTotaleByStatoAsync()
        {
            try
            {
                return await _garaRepository.GetImportoTotaleByStatoAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero degli importi totali per stato");
                throw;
            }
        }
    }
}