using Microsoft.Extensions.Logging;
using Trasformazioni.Data.Repositories;
using Trasformazioni.Mappings;
using Trasformazioni.Models.Entities;
using Trasformazioni.Models.ViewModels;
using Trasformazioni.Services.Interfaces;

namespace Trasformazioni.Services
{
    /// <summary>
    /// Implementazione della business logic per la gestione Richieste Integrazione
    /// Gestisce il ping-pong di richieste/risposte con l'ente appaltante
    /// </summary>
    public class RichiestaIntegrazioneService : IRichiestaIntegrazioneService
    {
        private readonly IRichiestaIntegrazioneRepository _richiestaRepository;
        private readonly ILottoRepository _lottoRepository;
        private readonly ILogger<RichiestaIntegrazioneService> _logger;

        public RichiestaIntegrazioneService(
            IRichiestaIntegrazioneRepository richiestaRepository,
            ILottoRepository lottoRepository,
            ILogger<RichiestaIntegrazioneService> logger)
        {
            _richiestaRepository = richiestaRepository;
            _lottoRepository = lottoRepository;
            _logger = logger;
        }

        // ===================================
        // QUERY - LETTURA
        // ===================================

        public async Task<IEnumerable<RichiestaIntegrazioneListViewModel>> GetAllAsync()
        {
            try
            {
                var richieste = await _richiestaRepository.GetAllAsync();
                return richieste.Select(r => r.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero di tutte le richieste di integrazione");
                throw;
            }
        }

        public async Task<RichiestaIntegrazioneDetailsViewModel?> GetByIdAsync(Guid id)
        {
            try
            {
                var richiesta = await _richiestaRepository.GetCompleteAsync(id);
                return richiesta?.ToDetailsViewModel();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero della richiesta di integrazione ID: {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<RichiestaIntegrazioneListViewModel>> GetByLottoIdAsync(Guid lottoId)
        {
            try
            {
                var richieste = await _richiestaRepository.GetByLottoIdAsync(lottoId);
                return richieste.Select(r => r.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero delle richieste per lotto ID: {LottoId}", lottoId);
                throw;
            }
        }

        public async Task<IEnumerable<RichiestaIntegrazioneListViewModel>> GetAperteAsync()
        {
            try
            {
                var richieste = await _richiestaRepository.GetAperteAsync();
                return richieste.Select(r => r.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero delle richieste aperte");
                throw;
            }
        }

        public async Task<IEnumerable<RichiestaIntegrazioneListViewModel>> GetChiuseAsync()
        {
            try
            {
                var richieste = await _richiestaRepository.GetChiuseAsync();
                return richieste.Select(r => r.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero delle richieste chiuse");
                throw;
            }
        }

        public async Task<IEnumerable<RichiestaIntegrazioneListViewModel>> GetNonRisposteAsync()
        {
            try
            {
                var richieste = await _richiestaRepository.GetNonRisposteAsync();
                return richieste.Select(r => r.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero delle richieste non risposte");
                throw;
            }
        }

        public async Task<IEnumerable<RichiestaIntegrazioneListViewModel>> GetRisposteNonChiuseAsync()
        {
            try
            {
                var richieste = await _richiestaRepository.GetRisposteNonChiuseAsync();
                return richieste.Select(r => r.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero delle richieste risposte non chiuse");
                throw;
            }
        }

        public async Task<IEnumerable<RichiestaIntegrazioneListViewModel>> GetScaduteAsync(int giorniScadenza = 7)
        {
            try
            {
                var richieste = await _richiestaRepository.GetScaduteAsync(giorniScadenza);
                return richieste.Select(r => r.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero delle richieste scadute");
                throw;
            }
        }

        public async Task<PagedResult<RichiestaIntegrazioneListViewModel>> GetPagedAsync(RichiestaIntegrazioneFilterViewModel filters)
        {
            try
            {
                var (items, totalCount) = await _richiestaRepository.GetPagedAsync(filters);
                var viewModels = items.Select(r => r.ToListViewModel()).ToList();

                return new PagedResult<RichiestaIntegrazioneListViewModel>
                {
                    Items = viewModels,
                    TotalItems = totalCount,
                    PageNumber = filters.PageNumber,
                    PageSize = filters.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero delle richieste paginate");
                throw;
            }
        }

        // ===================================
        // COMANDI - SCRITTURA
        // ===================================

        public async Task<(bool Success, string? ErrorMessage, Guid? RichiestaId)> CreateAsync(
            RichiestaIntegrazioneCreateViewModel model,
            string? documentoRichiestaPath = null)
        {
            try
            {
                // 1. Verifica esistenza lotto
                var lottoExists = await _lottoRepository.ExistsAsync(model.LottoId);
                if (!lottoExists)
                {
                    return (false, "Lotto non trovato", null);
                }

                // 2. Validazione date
                if (model.DataRichiestaEnte > DateTime.Now)
                {
                    return (false, "La data richiesta ente non può essere futura", null);
                }

                // 3. Validazione testo richiesta (minimo 10 caratteri)
                if (string.IsNullOrWhiteSpace(model.TestoRichiestaEnte) || model.TestoRichiestaEnte.Length < 10)
                {
                    return (false, "Il testo della richiesta deve contenere almeno 10 caratteri", null);
                }

                // 4. Genera numero progressivo automatico
                var numeroProgressivo = await _richiestaRepository.GetNextNumeroProgressivoAsync(model.LottoId);

                // 5. Mapping e salvataggio
                var richiesta = model.ToEntity(numeroProgressivo, documentoRichiestaPath);
                await _richiestaRepository.AddAsync(richiesta);

                _logger.LogInformation("Richiesta integrazione creata con successo per lotto {LottoId} - Numero {NumeroProgressivo} - ID: {Id}",
                    model.LottoId, numeroProgressivo, richiesta.Id);

                return (true, null, richiesta.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la creazione della richiesta di integrazione per lotto {LottoId}", model.LottoId);
                return (false, "Errore durante la creazione della richiesta di integrazione", null);
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(
            RichiestaIntegrazioneEditViewModel model,
            string? nuovoDocumentoRichiestaPath = null,
            string? nuovoDocumentoRispostaPath = null)
        {
            try
            {
                // 1. Verifica esistenza
                var richiesta = await _richiestaRepository.GetByIdAsync(model.Id);
                if (richiesta == null)
                {
                    return (false, "Richiesta di integrazione non trovata");
                }

                // 2. Validazione date
                if (model.DataRichiestaEnte > DateTime.Now)
                {
                    return (false, "La data richiesta ente non può essere futura");
                }

                if (model.DataRispostaAzienda.HasValue)
                {
                    if (model.DataRispostaAzienda.Value < model.DataRichiestaEnte)
                    {
                        return (false, "La data risposta non può essere precedente alla data richiesta");
                    }

                    if (model.DataRispostaAzienda.Value > DateTime.Now)
                    {
                        return (false, "La data risposta non può essere futura");
                    }
                }

                // 3. Validazione testo richiesta
                if (string.IsNullOrWhiteSpace(model.TestoRichiestaEnte) || model.TestoRichiestaEnte.Length < 10)
                {
                    return (false, "Il testo della richiesta deve contenere almeno 10 caratteri");
                }

                // 4. Validazione risposta: se c'è data risposta, deve esserci anche il testo
                if (model.DataRispostaAzienda.HasValue && string.IsNullOrWhiteSpace(model.TestoRispostaAzienda))
                {
                    return (false, "Se è presente la data risposta, è obbligatorio inserire anche il testo della risposta");
                }

                // 5. Validazione chiusura: può essere chiusa solo se ha risposta
                if (model.IsChiusa && !model.DataRispostaAzienda.HasValue)
                {
                    return (false, "La richiesta può essere chiusa solo dopo aver fornito una risposta");
                }

                // 6. Aggiorna l'entità
                model.UpdateEntity(richiesta, nuovoDocumentoRichiestaPath, nuovoDocumentoRispostaPath);
                await _richiestaRepository.UpdateAsync(richiesta);

                _logger.LogInformation("Richiesta integrazione aggiornata con successo: ID {Id}", richiesta.Id);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'aggiornamento della richiesta di integrazione ID: {Id}", model.Id);
                return (false, "Errore durante l'aggiornamento della richiesta di integrazione");
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(Guid id)
        {
            try
            {
                // 1. Verifica esistenza
                var richiesta = await _richiestaRepository.GetByIdAsync(id);
                if (richiesta == null)
                {
                    return (false, "Richiesta di integrazione non trovata");
                }

                // 2. Verifica che non sia già stata risposta (logica business: non eliminare se già gestita)
                if (richiesta.DataRispostaAzienda.HasValue)
                {
                    return (false, "Impossibile eliminare una richiesta che ha già ricevuto risposta");
                }

                // 3. Elimina (soft delete)
                await _richiestaRepository.DeleteAsync(id);

                _logger.LogInformation("Richiesta integrazione eliminata con successo: ID {Id}", id);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'eliminazione della richiesta di integrazione ID: {Id}", id);
                return (false, "Errore durante l'eliminazione della richiesta di integrazione");
            }
        }

        // ===================================
        // OPERAZIONI BUSINESS SPECIFICHE
        // ===================================

        public async Task<(bool Success, string? ErrorMessage)> RispondiAsync(
            Guid richiestaId,
            string testoRisposta,
            DateTime dataRisposta,
            string? documentoRispostaPath = null)
        {
            try
            {
                var richiesta = await _richiestaRepository.GetByIdAsync(richiestaId);
                if (richiesta == null)
                {
                    return (false, "Richiesta di integrazione non trovata");
                }

                // Validazione: non può già avere una risposta
                if (richiesta.DataRispostaAzienda.HasValue)
                {
                    return (false, "La richiesta ha già ricevuto risposta. Usa l'operazione di modifica per aggiornare la risposta.");
                }

                // Validazione testo risposta
                if (string.IsNullOrWhiteSpace(testoRisposta) || testoRisposta.Length < 10)
                {
                    return (false, "Il testo della risposta deve contenere almeno 10 caratteri");
                }

                // Validazione data
                if (dataRisposta < richiesta.DataRichiestaEnte)
                {
                    return (false, "La data risposta non può essere precedente alla data richiesta");
                }

                if (dataRisposta > DateTime.Now)
                {
                    return (false, "La data risposta non può essere futura");
                }

                richiesta.DataRispostaAzienda = dataRisposta;
                richiesta.TestoRispostaAzienda = testoRisposta.Trim();

                if (!string.IsNullOrWhiteSpace(documentoRispostaPath))
                {
                    richiesta.DocumentoRispostaPath = documentoRispostaPath;
                    richiesta.NomeFileRisposta = System.IO.Path.GetFileName(documentoRispostaPath);
                }

                await _richiestaRepository.UpdateAsync(richiesta);

                _logger.LogInformation("Risposta aggiunta alla richiesta integrazione {RichiestaId}", richiestaId);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'aggiunta della risposta alla richiesta ID: {RichiestaId}", richiestaId);
                return (false, "Errore durante l'aggiunta della risposta");
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> ChiudiAsync(Guid richiestaId)
        {
            try
            {
                var richiesta = await _richiestaRepository.GetByIdAsync(richiestaId);
                if (richiesta == null)
                {
                    return (false, "Richiesta di integrazione non trovata");
                }

                // Validazione: deve avere una risposta per essere chiusa
                if (!await CanChiudiAsync(richiestaId))
                {
                    return (false, "La richiesta può essere chiusa solo dopo aver fornito una risposta");
                }

                // Validazione: se già chiusa
                if (richiesta.IsChiusa)
                {
                    return (false, "La richiesta è già chiusa");
                }

                richiesta.IsChiusa = true;
                await _richiestaRepository.UpdateAsync(richiesta);

                _logger.LogInformation("Richiesta integrazione {RichiestaId} chiusa", richiestaId);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la chiusura della richiesta ID: {RichiestaId}", richiestaId);
                return (false, "Errore durante la chiusura della richiesta");
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> RiapriAsync(Guid richiestaId)
        {
            try
            {
                var richiesta = await _richiestaRepository.GetByIdAsync(richiestaId);
                if (richiesta == null)
                {
                    return (false, "Richiesta di integrazione non trovata");
                }

                // Validazione: se già aperta
                if (!richiesta.IsChiusa)
                {
                    return (false, "La richiesta è già aperta");
                }

                richiesta.IsChiusa = false;
                await _richiestaRepository.UpdateAsync(richiesta);

                _logger.LogInformation("Richiesta integrazione {RichiestaId} riaperta", richiestaId);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la riapertura della richiesta ID: {RichiestaId}", richiestaId);
                return (false, "Errore durante la riapertura della richiesta");
            }
        }

        // ===================================
        // VALIDAZIONI
        // ===================================

        public Task<bool> ValidaDateAsync(DateTime dataRichiesta, DateTime? dataRisposta)
        {
            if (!dataRisposta.HasValue)
                return Task.FromResult(true);

            // La risposta deve essere successiva alla richiesta
            var isValid = dataRisposta.Value >= dataRichiesta && dataRisposta.Value <= DateTime.Now;
            return Task.FromResult(isValid);
        }

        public async Task<bool> CanChiudiAsync(Guid richiestaId)
        {
            var richiesta = await _richiestaRepository.GetByIdAsync(richiestaId);
            if (richiesta == null)
                return false;

            // Può essere chiusa solo se ha una risposta
            return richiesta.DataRispostaAzienda.HasValue;
        }

        /// <summary>
        /// Verifica se tutte le richieste di un lotto sono chiuse
        /// Usato per decidere il cambio stato automatico RichiestaIntegrazione → InEsame
        /// </summary>
        public async Task<bool> AreAllRequestsClosedAsync(Guid lottoId)
        {
            try
            {
                var richieste = await _richiestaRepository.GetByLottoIdAsync(lottoId);

                // Se non ci sono richieste, considera "tutte chiuse" = true
                if (!richieste.Any())
                {
                    _logger.LogInformation("Nessuna richiesta trovata per Lotto {LottoId} - considera tutte chiuse", lottoId);
                    return true;
                }

                // Verifica se TUTTE le richieste sono chiuse
                var tutteChiuse = richieste.All(r => r.IsChiusa);

                _logger.LogInformation(
                    "Lotto {LottoId}: {TotaleRichieste} richieste, {RichiesteChiuse} chiuse, tutte chiuse = {TutteChiuse}",
                    lottoId,
                    richieste.Count(),
                    richieste.Count(r => r.IsChiusa),
                    tutteChiuse
                );

                return tutteChiuse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la verifica richieste chiuse per Lotto {LottoId}", lottoId);
                throw;
            }
        }

        // ===================================
        // STATISTICHE
        // ===================================

        public async Task<int> GetCountAperteAsync()
        {
            try
            {
                var richieste = await _richiestaRepository.GetAperteAsync();
                return richieste.Count();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il conteggio delle richieste aperte");
                throw;
            }
        }

        public async Task<int> GetCountNonRisposteAsync()
        {
            try
            {
                var richieste = await _richiestaRepository.GetNonRisposteAsync();
                return richieste.Count();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il conteggio delle richieste non risposte");
                throw;
            }
        }

        public async Task<int> GetCountByLottoIdAsync(Guid lottoId)
        {
            try
            {
                return await _richiestaRepository.GetCountByLottoAsync(lottoId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il conteggio delle richieste per lotto ID: {LottoId}", lottoId);
                throw;
            }
        }

        public async Task<double?> GetTempoMedioRispostaAsync()
        {
            try
            {
                var richieste = await _richiestaRepository.GetAllAsync();
                var richiesteConRisposta = richieste
                    .Where(r => r.DataRispostaAzienda.HasValue)
                    .ToList();

                if (!richiesteConRisposta.Any())
                    return null;

                var tempi = richiesteConRisposta
                    .Select(r => (r.DataRispostaAzienda!.Value - r.DataRichiestaEnte).TotalDays)
                    .ToList();

                return tempi.Average();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il calcolo del tempo medio di risposta");
                throw;
            }
        }
    }
}