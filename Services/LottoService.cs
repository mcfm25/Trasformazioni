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
    /// Implementazione della business logic per la gestione Lotti
    /// Include gestione workflow complesso e transizioni di stato
    /// </summary>
    public class LottoService : ILottoService
    {
        private readonly ILottoRepository _lottoRepository;
        private readonly IGaraRepository _garaRepository;
        private readonly IPreventivoRepository _preventivoRepository;
        private readonly ILogger<LottoService> _logger;
        private readonly IPartecipanteLottoRepository _partecipanteLottoRepository;
        private readonly IValutazioneLottoRepository _valutazioneLottoRepository;
        private readonly IElaborazioneLottoRepository _elaborazioneLottoRepository;

        public LottoService(
            ILottoRepository lottoRepository,
            IGaraRepository garaRepository,
            IPreventivoRepository preventivoRepository,
            ILogger<LottoService> logger,
            IPartecipanteLottoRepository partecipanteLottoRepository,
            IValutazioneLottoRepository valutazioneLottoRepository,
            IElaborazioneLottoRepository elaborazioneLottoRepository)
        {
            _lottoRepository = lottoRepository;
            _garaRepository = garaRepository;
            _preventivoRepository = preventivoRepository;
            _logger = logger;
            _partecipanteLottoRepository = partecipanteLottoRepository;
            _valutazioneLottoRepository = valutazioneLottoRepository;
            _elaborazioneLottoRepository = elaborazioneLottoRepository;
        }

        // ===================================
        // QUERY - LETTURA
        // ===================================

        public async Task<IEnumerable<LottoListViewModel>> GetAllAsync()
        {
            try
            {
                var lotti = await _lottoRepository.GetAllAsync();
                return lotti.Select(l => l.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero di tutti i lotti");
                throw;
            }
        }

        public async Task<LottoDetailsViewModel?> GetByIdAsync(Guid id)
        {
            try
            {
                var lotto = await _lottoRepository.GetCompleteAsync(id);
                return lotto?.ToDetailsViewModel();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero del lotto ID: {Id}", id);
                throw;
            }
        }

        public async Task<LottoEditViewModel?> GetForEditAsync(Guid id)
        {
            try
            {
                var lotto = await _lottoRepository.GetCompleteAsync(id);
                return lotto?.ToEditViewModel();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero del lotto per edit ID: {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<LottoListViewModel>> GetByGaraIdAsync(Guid garaId)
        {
            try
            {
                var lotti = await _lottoRepository.GetByGaraIdAsync(garaId);
                return lotti.Select(l => l.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei lotti per gara ID: {GaraId}", garaId);
                throw;
            }
        }

        public async Task<IEnumerable<LottoListViewModel>> GetByStatoAsync(StatoLotto stato)
        {
            try
            {
                var lotti = await _lottoRepository.GetByStatoAsync(stato);
                return lotti.Select(l => l.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei lotti per stato: {Stato}", stato);
                throw;
            }
        }

        public async Task<IEnumerable<LottoListViewModel>> GetByOperatoreAsync(string operatoreId)
        {
            try
            {
                var lotti = await _lottoRepository.GetByOperatoreAsync(operatoreId);
                return lotti.Select(l => l.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei lotti per operatore ID: {OperatoreId}", operatoreId);
                throw;
            }
        }

        public async Task<IEnumerable<LottoListViewModel>> GetInValutazioneTecnicaAsync()
        {
            try
            {
                var lotti = await _lottoRepository.GetInValutazioneTecnicaAsync();
                return lotti.Select(l => l.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei lotti in valutazione tecnica");
                throw;
            }
        }

        public async Task<IEnumerable<LottoListViewModel>> GetInValutazioneEconomicaAsync()
        {
            try
            {
                var lotti = await _lottoRepository.GetInValutazioneEconomicaAsync();
                return lotti.Select(l => l.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei lotti in valutazione economica");
                throw;
            }
        }

        public async Task<IEnumerable<LottoListViewModel>> GetInElaborazioneAsync()
        {
            try
            {
                var lotti = await _lottoRepository.GetInElaborazioneAsync();
                return lotti.Select(l => l.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei lotti in elaborazione");
                throw;
            }
        }

        public async Task<IEnumerable<LottoListViewModel>> GetPresentatiAsync()
        {
            try
            {
                var lotti = await _lottoRepository.GetPresentatiAsync();
                return lotti.Select(l => l.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei lotti presentati");
                throw;
            }
        }

        public async Task<IEnumerable<LottoListViewModel>> GetVintiAsync()
        {
            try
            {
                var lotti = await _lottoRepository.GetVintiAsync();
                return lotti.Select(l => l.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei lotti vinti");
                throw;
            }
        }

        public async Task<IEnumerable<LottoListViewModel>> GetConIntegrazioniAperteAsync()
        {
            try
            {
                var lotti = await _lottoRepository.GetConIntegrazioniAperteAsync();
                return lotti.Select(l => l.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei lotti con integrazioni aperte");
                throw;
            }
        }

        public async Task<IEnumerable<LottoListViewModel>> GetLottiInScadenzaAsync(int giorniProssimi = 7)
        {
            try
            {
                var lotti = await _lottoRepository.GetLottiInScadenzaAsync(giorniProssimi);
                return lotti.Select(l => l.ToListViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei lotti in scadenza");
                throw;
            }
        }

        public async Task<PagedResult<LottoListViewModel>> GetPagedAsync(LottoFilterViewModel filters)
        {
            try
            {
                var (items, totalCount) = await _lottoRepository.GetPagedAsync(filters);
                var viewModels = items.Select(l => l.ToListViewModel()).ToList();

                return new PagedResult<LottoListViewModel>
                {
                    Items = viewModels,
                    TotalItems = totalCount,
                    PageNumber = filters.PageNumber,
                    PageSize = filters.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei lotti paginati");
                throw;
            }
        }

        // ===================================
        // COMANDI - SCRITTURA
        // ===================================

        public async Task<(bool Success, string? ErrorMessage, Guid? LottoId)> CreateAsync(LottoCreateViewModel model)
        {
            try
            {
                // 1. Verifica esistenza gara
                var garaExists = await _garaRepository.ExistsAsync(model.GaraId);
                if (!garaExists)
                {
                    return (false, "Gara non trovata", null);
                }

                // 2. Validazione Codice Lotto univoco all'interno della gara
                if (!await IsCodiceLottoUniqueInGaraAsync(model.CodiceLotto, model.GaraId))
                {
                    return (false, "Il Codice Lotto è già presente per questa gara", null);
                }

                // 3. Validazione importi
                if (model.Quotazione.HasValue && model.ImportoBaseAsta.HasValue &&
                    model.Quotazione.Value > model.ImportoBaseAsta.Value)
                {
                    _logger.LogWarning("Quotazione ({Quotazione}) superiore a ImportoBaseAsta ({ImportoBaseAsta}) per lotto {CodiceLotto}",
                        model.Quotazione, model.ImportoBaseAsta, model.CodiceLotto);
                    // Non blocchiamo, solo warning
                }

                // 4. Validazione date contratto
                if (model.DataStipulaContratto.HasValue && model.DataScadenzaContratto.HasValue &&
                    model.DataScadenzaContratto.Value <= model.DataStipulaContratto.Value)
                {
                    return (false, "La data di scadenza contratto deve essere successiva alla data di stipula", null);
                }

                //// 5. Mapping e salvataggio
                //var lotto = model.ToEntity();
                //await _lottoRepository.AddAsync(lotto);

                //_logger.LogInformation("Lotto creato con successo: {CodiceLotto} - ID: {Id}", lotto.CodiceLotto, lotto.Id);

                // 5. Mapping e salvataggio
                var lotto = model.ToEntity();

                // Aggiungi documenti richiesti (checklist)
                if (model.DocumentiRichiestiIds != null && model.DocumentiRichiestiIds.Any())
                {
                    foreach (var tipoDocId in model.DocumentiRichiestiIds)
                    {
                        lotto.DocumentiRichiesti.Add(new LottoDocumentoRichiesto
                        {
                            Id = Guid.NewGuid(),
                            LottoId = lotto.Id,
                            TipoDocumentoId = tipoDocId
                        });
                    }
                }

                await _lottoRepository.AddAsync(lotto);

                _logger.LogInformation("Lotto creato con successo: {CodiceLotto} - ID: {Id}, DocumentiRichiesti: {Count}",
                    lotto.CodiceLotto, lotto.Id, lotto.DocumentiRichiesti.Count);

                return (true, null, lotto.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la creazione del lotto {CodiceLotto}", model.CodiceLotto);
                return (false, "Errore durante la creazione del lotto", null);
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(LottoEditViewModel model)
        {
            try
            {
                // 1. Verifica esistenza
                var lotto = await _lottoRepository.GetWithDocumentiRichiestiAsync(model.Id);
                if (lotto == null)
                {
                    return (false, "Lotto non trovato");
                }

                // 2. Validazione Codice Lotto univoco (escludendo il record corrente)
                if (!await IsCodiceLottoUniqueInGaraAsync(model.CodiceLotto, model.GaraId, model.Id))
                {
                    return (false, "Il Codice Lotto è già presente per questa gara");
                }

                // 3. Validazione importi
                if (model.Quotazione.HasValue && model.ImportoBaseAsta.HasValue &&
                    model.Quotazione.Value > model.ImportoBaseAsta.Value)
                {
                    _logger.LogWarning("Quotazione ({Quotazione}) superiore a ImportoBaseAsta ({ImportoBaseAsta}) per lotto {CodiceLotto}",
                        model.Quotazione, model.ImportoBaseAsta, model.CodiceLotto);
                    // Non blocchiamo, solo warning
                }

                // 4. Validazione date contratto
                if (model.DataStipulaContratto.HasValue && model.DataScadenzaContratto.HasValue &&
                    model.DataScadenzaContratto.Value <= model.DataStipulaContratto.Value)
                {
                    return (false, "La data di scadenza contratto deve essere successiva alla data di stipula");
                }

                // 5. Aggiorna l'entità
                model.UpdateEntity(lotto);
                //await _lottoRepository.UpdateAsync(lotto);

                await _lottoRepository.UpdateAsync(lotto);

                // 6. Aggiorna documenti richiesti (checklist) - SEPARATAMENTE
                await _lottoRepository.UpdateDocumentiRichiestiAsync(model.Id, model.DocumentiRichiestiIds);


                _logger.LogInformation("Lotto aggiornato con successo: {CodiceLotto} - ID: {Id}", lotto.CodiceLotto, lotto.Id);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'aggiornamento del lotto ID: {Id}", model.Id);
                return (false, "Errore durante l'aggiornamento del lotto");
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(Guid id)
        {
            try
            {
                // 1. Verifica esistenza
                var lotto = await _lottoRepository.GetWithPreventiviAsync(id);
                if (lotto == null)
                {
                    return (false, "Lotto non trovato");
                }

                // 2. Verifica che non abbia preventivi (DeleteBehavior.Restrict in PreventivoConfig)
                if (lotto.Preventivi != null && lotto.Preventivi.Any())
                {
                    return (false, "Impossibile eliminare il lotto: sono presenti preventivi associati. Eliminare prima i preventivi.");
                }

                // 3. Elimina (soft delete)
                await _lottoRepository.DeleteAsync(id);

                _logger.LogInformation("Lotto eliminato con successo: {CodiceLotto} - ID: {Id}", lotto.CodiceLotto, id);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'eliminazione del lotto ID: {Id}", id);
                return (false, "Errore durante l'eliminazione del lotto");
            }
        }

        // ===================================
        // OPERAZIONI BUSINESS SPECIFICHE - WORKFLOW
        // ===================================

        public async Task<(bool Success, string? ErrorMessage)> AssegnaOperatoreAsync(Guid lottoId, string operatoreId)
        {
            try
            {
                var lotto = await _lottoRepository.GetByIdAsync(lottoId);
                if (lotto == null)
                {
                    return (false, "Lotto non trovato");
                }

                if (string.IsNullOrWhiteSpace(operatoreId))
                {
                    return (false, "ID operatore non valido");
                }

                lotto.OperatoreAssegnatoId = operatoreId;
                await _lottoRepository.UpdateAsync(lotto);

                _logger.LogInformation("Operatore {OperatoreId} assegnato al lotto {LottoId}", operatoreId, lottoId);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'assegnazione operatore al lotto ID: {LottoId}", lottoId);
                return (false, "Errore durante l'assegnazione dell'operatore");
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> ImpostaDataInizioEsameAsync(Guid lottoId, DateTime dataInizioEsame)
        {
            try
            {
                var lotto = await _lottoRepository.GetByIdAsync(lottoId);
                if (lotto == null)
                {
                    return (false, "Lotto non trovato");
                }

                // Verifica che il lotto sia in stato Presentato
                if (lotto.Stato != StatoLotto.Presentato)
                {
                    return (false, "Il lotto deve essere in stato Presentato per impostare la data di inizio esame");
                }

                lotto.DataInizioEsameEnte = dataInizioEsame;
                await _lottoRepository.UpdateAsync(lotto);

                _logger.LogInformation("Data inizio esame impostata per lotto {LottoId}: {DataInizioEsame}. Il cambio stato a InEsame avverrà automaticamente tramite background job.",
                    lottoId, dataInizioEsame);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'impostazione data inizio esame per lotto ID: {LottoId}", lottoId);
                return (false, "Errore durante l'impostazione della data di inizio esame");
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> RifiutaLottoAsync(Guid lottoId, string motivo)
        {
            try
            {
                var lotto = await _lottoRepository.GetByIdAsync(lottoId);
                if (lotto == null)
                {
                    return (false, "Lotto non trovato");
                }

                if (string.IsNullOrWhiteSpace(motivo))
                {
                    return (false, "Il motivo del rifiuto è obbligatorio");
                }

                lotto.Stato = StatoLotto.Rifiutato;
                lotto.MotivoRifiuto = motivo.Trim();
                await _lottoRepository.UpdateAsync(lotto);

                _logger.LogInformation("Lotto {LottoId} rifiutato. Motivo: {Motivo}", lottoId, motivo);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il rifiuto del lotto ID: {LottoId}", lottoId);
                return (false, "Errore durante il rifiuto del lotto");
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> CambiaStatoAsync(Guid lottoId, StatoLotto nuovoStato)
        {
            try
            {
                var lotto = await _lottoRepository.GetByIdAsync(lottoId);
                if (lotto == null)
                {
                    return (false, "Lotto non trovato");
                }

                // Validazione transizione
                if (!await IsTransizioneStatoValidaAsync(lotto.Stato, nuovoStato))
                {
                    return (false, $"Transizione di stato non valida da {lotto.Stato} a {nuovoStato}");
                }

                var vecchioStato = lotto.Stato;
                lotto.Stato = nuovoStato;
                await _lottoRepository.UpdateAsync(lotto);

                _logger.LogInformation("Stato lotto {LottoId} cambiato da {VecchioStato} a {NuovoStato}",
                    lottoId, vecchioStato, nuovoStato);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il cambio stato del lotto ID: {LottoId}", lottoId);
                return (false, "Errore durante il cambio di stato");
            }
        }

        // ===================================
        // VALIDAZIONI
        // ===================================

        public async Task<bool> IsCodiceLottoUniqueInGaraAsync(string codiceLotto, Guid garaId, Guid? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(codiceLotto))
                return false;

            var exists = await _lottoRepository.ExistsByGaraAndCodiceAsync(garaId, codiceLotto, excludeId);
            return !exists;
        }

        public Task<bool> IsTransizioneStatoValidaAsync(StatoLotto statoCorrente, StatoLotto nuovoStato)
        {
            // Definizione delle transizioni valide
            var transizioniValide = new Dictionary<StatoLotto, HashSet<StatoLotto>>
            {
                { StatoLotto.Bozza, new HashSet<StatoLotto> { StatoLotto.InValutazioneTecnica, StatoLotto.Rifiutato } },
                { StatoLotto.InValutazioneTecnica, new HashSet<StatoLotto> { StatoLotto.InValutazioneEconomica, StatoLotto.Rifiutato } },
                { StatoLotto.InValutazioneEconomica, new HashSet<StatoLotto> { StatoLotto.Approvato, StatoLotto.Rifiutato } },
                { StatoLotto.Approvato, new HashSet<StatoLotto> { StatoLotto.InElaborazione } },
                { StatoLotto.InElaborazione, new HashSet<StatoLotto> { StatoLotto.Presentato } },
                { StatoLotto.Presentato, new HashSet<StatoLotto> { StatoLotto.InEsame } },
                { StatoLotto.InEsame, new HashSet<StatoLotto> { StatoLotto.RichiestaIntegrazione, StatoLotto.Vinto, StatoLotto.Perso, StatoLotto.Scartato } },
                { StatoLotto.RichiestaIntegrazione, new HashSet<StatoLotto> { StatoLotto.InEsame, StatoLotto.Scartato } },
                // Stati terminali non hanno transizioni valide
                { StatoLotto.Rifiutato, new HashSet<StatoLotto>() },
                { StatoLotto.Vinto, new HashSet<StatoLotto>() },
                { StatoLotto.Perso, new HashSet<StatoLotto>() },
                { StatoLotto.Scartato, new HashSet<StatoLotto>() }
            };

            if (!transizioniValide.ContainsKey(statoCorrente))
                return Task.FromResult(false);

            return Task.FromResult(transizioniValide[statoCorrente].Contains(nuovoStato));
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            try
            {
                return await _lottoRepository.ExistsAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nella verifica esistenza lotto {Id}", id);
                throw;
            }
        }

        // ===================================
        // STATISTICHE
        // ===================================

        public async Task<Dictionary<StatoLotto, int>> GetCountByStatoAsync()
        {
            try
            {
                return await _lottoRepository.GetCountByStatoAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero delle statistiche lotti per stato");
                throw;
            }
        }

        public async Task<Dictionary<StatoLotto, int>> GetCountByStatoForGaraAsync(Guid garaId)
        {
            try
            {
                return await _lottoRepository.GetCountByStatoForGaraAsync(garaId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero delle statistiche lotti per stato per gara ID: {GaraId}", garaId);
                throw;
            }
        }

        public async Task<decimal> GetImportoTotaleVintiAsync()
        {
            try
            {
                return await _lottoRepository.GetImportoTotaleVintiAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dell'importo totale lotti vinti");
                throw;
            }
        }

        public async Task<decimal> GetTassoSuccessoAsync()
        {
            try
            {
                return await _lottoRepository.GetTassoSuccessoAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero del tasso di successo");
                throw;
            }
        }


        // ===================================
        // METODI AUSILIARI - Fase 1 - Modulo LOTTI
        // ===================================

        /// <summary>
        /// Cambia lo stato di un lotto con validazione workflow
        /// </summary>
        public async Task<bool> UpdateStatoAsync(Guid lottoId, StatoLotto nuovoStato, string userId)
        {
            var lotto = await _lottoRepository.GetByIdAsync(lottoId);
            if (lotto == null)
            {
                return false;
            }

            // Validazione transizione workflow
            if (!IsValidTransition(lotto.Stato, nuovoStato))
            {
                throw new InvalidOperationException(
                    $"Transizione non valida: {lotto.Stato} → {nuovoStato}. " +
                    $"Controlla il workflow degli stati."
                );
            }

            // Aggiornamento stato
            lotto.Stato = nuovoStato;
            lotto.ModifiedAt = DateTime.Now;
            lotto.ModifiedBy = userId;

            // NOTA: Lotto non ha DataConclusione, quindi rimuoviamo questa parte
            // Se stato terminale, puoi aggiornare altri campi se necessario

            await _lottoRepository.UpdateAsync(lotto);

            _logger.LogInformation(
                "Lotto {LottoId} - Stato cambiato da {StatoVecchio} a {StatoNuovo} da utente {UserId}",
                lottoId, lotto.Stato, nuovoStato, userId
            );

            return true;
        }

        /// <summary>
        /// Valida se una transizione di stato è permessa dal workflow
        /// </summary>
        private bool IsValidTransition(StatoLotto statoCorrente, StatoLotto nuovoStato)
        {
            // Matrice di transizioni valide basata sugli stati reali
            var transizioniValide = new Dictionary<StatoLotto, List<StatoLotto>>
            {
                // Da Bozza
                [StatoLotto.Bozza] = new List<StatoLotto>
                {
                    StatoLotto.InValutazioneTecnica,
                    StatoLotto.InValutazioneEconomica,
                    StatoLotto.Rifiutato  // Può essere rifiutato subito
                },

                // Da InValutazioneTecnica
                [StatoLotto.InValutazioneTecnica] = new List<StatoLotto>
                {
                    StatoLotto.InValutazioneEconomica,  // Passa a valutazione economica
                    StatoLotto.Rifiutato,               // Può essere rifiutato
                    StatoLotto.Bozza                    // Può tornare in bozza
                },

                // Da InValutazioneEconomica
                [StatoLotto.InValutazioneEconomica] = new List<StatoLotto>
                {
                    StatoLotto.Approvato,               // Approvato dopo entrambe valutazioni
                    StatoLotto.Rifiutato,               // Può essere rifiutato
                    StatoLotto.InValutazioneTecnica     // Può tornare a valutazione tecnica
                },

                // Da Approvato
                [StatoLotto.Approvato] = new List<StatoLotto>
                {
                    StatoLotto.InElaborazione,          // Inizia elaborazione
                    StatoLotto.Rifiutato                // Può essere rifiutato
                },

                // Da Rifiutato (terminale - solo può tornare a Bozza se riabilitato)
                [StatoLotto.Rifiutato] = new List<StatoLotto>
                {
                    StatoLotto.Bozza                    // Può essere riabilitato
                },

                // Da InElaborazione
                [StatoLotto.InElaborazione] = new List<StatoLotto>
                {
                    StatoLotto.Presentato,              // Presenta offerta
                    StatoLotto.Approvato,               // Torna ad approvato
                    StatoLotto.Rifiutato                // Può essere rifiutato
                },

                // Da Presentato
                [StatoLotto.Presentato] = new List<StatoLotto>
                {
                    StatoLotto.InEsame,                 // Va in esame ente
                    StatoLotto.InElaborazione           // Torna in elaborazione
                },

                // Da InEsame
                [StatoLotto.InEsame] = new List<StatoLotto>
                {
                    StatoLotto.Vinto,                   // Aggiudicato
                    StatoLotto.Perso,                   // Perso
                    StatoLotto.Scartato,                // Scartato
                    StatoLotto.RichiestaIntegrazione    // Richiesta integrazione
                },

                // Da RichiestaIntegrazione
                [StatoLotto.RichiestaIntegrazione] = new List<StatoLotto>
                {
                    StatoLotto.InEsame,                 // Torna in esame dopo integrazione
                    StatoLotto.Scartato                 // Può essere scartato
                },

                // Da Vinto (terminale)
                [StatoLotto.Vinto] = new List<StatoLotto>
                {
                    // Nessuna transizione possibile (stato terminale)
                },

                // Da Perso (terminale)
                [StatoLotto.Perso] = new List<StatoLotto>
                {
                    // Nessuna transizione possibile (stato terminale)
                },

                // Da Scartato (terminale)
                [StatoLotto.Scartato] = new List<StatoLotto>
                {
                    // Nessuna transizione possibile (stato terminale)
                }
            };

            // Verifica se la transizione è valida
            if (transizioniValide.TryGetValue(statoCorrente, out var statiPermessi))
            {
                return statiPermessi.Contains(nuovoStato);
            }

            return false;
        }

        /// <summary>
        /// Verifica se uno stato è terminale
        /// </summary>
        private bool IsStatoTerminale(StatoLotto stato)
        {
            return stato == StatoLotto.Vinto ||
                   stato == StatoLotto.Perso ||
                   stato == StatoLotto.Scartato ||
                   stato == StatoLotto.Rifiutato;
        }

        // Metodi helper da aggiungere per le statistiche (usati in Details)
        public async Task<int> GetNumeroPartecipantiAsync(Guid lottoId)
        {
            // Implementazione tramite repository
            // Assumendo che esista un metodo nel repository
            return await _partecipanteLottoRepository.CountByLottoIdAsync(lottoId);
        }

        public async Task<int> GetNumeroValutazioniAsync(Guid lottoId)
        {
            // Implementazione tramite repository
            return await _valutazioneLottoRepository.CountByLottoIdAsync(lottoId);
        }

        public async Task<int> GetNumeroElaborazioniAsync(Guid lottoId)
        {
            // Implementazione tramite repository
            return await _elaborazioneLottoRepository.CountByLottoIdAsync(lottoId);
        }

        public async Task<bool> CanDeleteAsync(Guid lottoId)
        {
            // Verifica se ci sono entità dipendenti
            var hasPartecipanti = await _partecipanteLottoRepository.CountByLottoIdAsync(lottoId) > 0;
            var hasValutazioni = await _valutazioneLottoRepository.CountByLottoIdAsync(lottoId) > 0;
            var hasElaborazioni = await _elaborazioneLottoRepository.CountByLottoIdAsync(lottoId) > 0;

            return !hasPartecipanti && !hasValutazioni && !hasElaborazioni;
        }
    }
}