using Trasformazioni.Data.Repositories;
using Trasformazioni.Mappings;
using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;
using Trasformazioni.Models.ViewModels;

namespace Trasformazioni.Services
{
    /// <summary>
    /// Servizio per la gestione della business logic delle assegnazioni mezzi
    /// PARTE 1/3: Query Methods (Read operations)
    /// </summary>
    public partial class AssegnazioneMezzoService : IAssegnazioneMezzoService
    {
        private readonly IAssegnazioneMezzoRepository _assegnazioneRepository;
        private readonly IMezzoRepository _mezzoRepository;
        private readonly ILogger<AssegnazioneMezzoService> _logger;

        public AssegnazioneMezzoService(
            IAssegnazioneMezzoRepository assegnazioneRepository,
            IMezzoRepository mezzoRepository,
            ILogger<AssegnazioneMezzoService> logger)
        {
            _assegnazioneRepository = assegnazioneRepository;
            _mezzoRepository = mezzoRepository;
            _logger = logger;
        }

        #region Query Methods (Read)

        public async Task<IEnumerable<AssegnazioneMezzoListViewModel>> GetAllAsync()
        {
            var assegnazioni = await _assegnazioneRepository.GetAllAsync();
            return assegnazioni.Select(a => a.ToListViewModel());
        }

        public async Task<AssegnazioneMezzoDetailsViewModel?> GetByIdAsync(Guid id)
        {
            var assegnazione = await _assegnazioneRepository.GetByIdAsync(id);
            return assegnazione?.ToDetailsViewModel();
        }

        public async Task<IEnumerable<AssegnazioneMezzoListViewModel>> GetByMezzoIdAsync(Guid mezzoId, bool includeChiuse = true)
        {
            var assegnazioni = await _assegnazioneRepository.GetByMezzoIdAsync(mezzoId, includeChiuse);
            return assegnazioni.Select(a => a.ToListViewModel());
        }

        public async Task<IEnumerable<AssegnazioneMezzoListViewModel>> GetByUtenteIdAsync(string utenteId, bool includeChiuse = true)
        {
            var assegnazioni = await _assegnazioneRepository.GetByUtenteIdAsync(utenteId, includeChiuse);
            return assegnazioni.Select(a => a.ToListViewModel());
        }

        public async Task<AssegnazioneMezzoDetailsViewModel?> GetAssegnazioneAttivaByMezzoIdAsync(Guid mezzoId)
        {
            var assegnazione = await _assegnazioneRepository.GetAssegnazioneAttivaByMezzoIdAsync(mezzoId);
            return assegnazione?.ToDetailsViewModel();
        }

        public async Task<IEnumerable<AssegnazioneMezzoListViewModel>> GetAssegnazioniAttiveByUtenteIdAsync(string utenteId)
        {
            var assegnazioni = await _assegnazioneRepository.GetAssegnazioniAttiveByUtenteIdAsync(utenteId);
            return assegnazioni.Select(a => a.ToListViewModel());
        }

        public async Task<AssegnazioneMezzoCloseViewModel?> GetCloseViewModelAsync(Guid assegnazioneId)
        {
            var assegnazione = await _assegnazioneRepository.GetByIdAsync(assegnazioneId);
            return assegnazione?.ToCloseViewModel();
        }

        /// <summary>
        /// Ottiene i periodi occupati per visualizzazione calendario
        /// </summary>
        public async Task<IEnumerable<PeriodoOccupatoViewModel>> GetPeriodiOccupatiAsync(Guid mezzoId)
        {
            var assegnazioni = await _assegnazioneRepository.GetPeriodiOccupatiAsync(mezzoId);

            return assegnazioni.Select(a => new PeriodoOccupatoViewModel
            {
                Id = a.Id,
                DataInizio = a.DataInizio,
                DataFine = a.DataFine,
                UtenteNomeCompleto = a.Utente?.NomeCompleto ?? "Utente sconosciuto",
                IsInCorso = a.IsInCorso,
                IsPrenotazione = a.IsPrenotazione
            });
        }

        #endregion

        #region Validation Methods

        /// <summary>
        /// Verifica disponibilità mezzo nel periodo specificato
        /// Supporta validazione per assegnazioni multiple in coda
        /// </summary>
        public async Task<bool> IsMezzoDisponibileAsync(Guid mezzoId, DateTime dataInizio, DateTime? dataFine)
        {
            // Verifica che non ci sia sovrapposizione di periodi
            return !await _assegnazioneRepository.HasSovrapposizionePeriodoAsync(
                mezzoId,
                dataInizio,
                dataFine);
        }

        public async Task<bool> CanUtenteBeAssignedAsync(string utenteId)
        {
            // Per ora permettiamo sempre (può avere più mezzi assegnati)
            // In futuro si potrebbe aggiungere un limite
            return true;
        }

        public async Task<bool> CanUserCloseAssegnazioneAsync(Guid assegnazioneId, string currentUserId, bool isAdmin)
        {
            // Admin può chiudere qualsiasi assegnazione
            if (isAdmin)
                return true;

            // Verifica che l'assegnazione appartenga all'utente corrente
            var assegnazione = await _assegnazioneRepository.GetByIdAsync(assegnazioneId);
            if (assegnazione == null)
                return false;

            return assegnazione.UtenteId == currentUserId;
        }

        #endregion

        #region Create Method

        /// <summary>
        /// Crea una nuova assegnazione/prenotazione con validazione sovrapposizione periodi
        /// </summary>
        public async Task<(bool Success, string? ErrorMessage, Guid? AssegnazioneId)> CreateAsync(AssegnazioneMezzoCreateViewModel model)
        {
            try
            {
                // 1. Validazione: Mezzo esiste
                var mezzo = await _mezzoRepository.GetByIdAsync(model.MezzoId);
                if (mezzo == null)
                {
                    return (false, "Mezzo non trovato", null);
                }

                // 2. MODIFICATO: Validazione sovrapposizione periodo (invece di solo "assegnazione attiva")
                if (await _assegnazioneRepository.HasSovrapposizionePeriodoAsync(
                    model.MezzoId,
                    model.DataInizio,
                    model.DataFine))
                {
                    return (false, "Il mezzo non è disponibile nel periodo selezionato. Controlla il calendario per vedere i periodi occupati.", null);
                }

                // 3. Validazione: Mezzo non è dismesso
                if (mezzo.Stato == StatoMezzo.Dismesso)
                {
                    return (false, "Non è possibile assegnare un mezzo dismesso", null);
                }

                // 4. Validazione: DataInizio non troppo nel passato (max 7 giorni fa)
                if (model.DataInizio.Date < DateTime.Today.AddDays(-7))
                {
                    return (false, "La data di inizio non può essere più vecchia di 7 giorni", null);
                }

                // 5. Validazione: DataFine > DataInizio (se popolata)
                if (model.DataFine.HasValue && model.DataFine.Value <= model.DataInizio)
                {
                    return (false, "La data di fine deve essere successiva alla data di inizio", null);
                }

                // 6. Validazione: DataFine non troppo nel futuro (max 1 anno)
                if (model.DataFine.HasValue && model.DataFine.Value > DateTime.Today.AddYears(1))
                {
                    return (false, "La data di fine non può essere oltre 1 anno nel futuro", null);
                }

                // 7. Mapping ViewModel → Entity
                var assegnazione = model.ToEntity();
                var dataInizio = model.DataInizio; //.ToUniversalTime();

                // 8. MODIFICATO: Determina stato mezzo basandosi su DataInizio e DataFine
                StatoMezzo nuovoStato;

                if (model.DataFine.HasValue)
                {
                    // Assegnazione con DataFine definita
                    // Se inizia oggi/passato = InUso, se inizia futuro = Prenotato
                    nuovoStato = dataInizio <= DateTime.Now
                        ? StatoMezzo.InUso
                        : StatoMezzo.Prenotato;
                }
                else
                {
                    // Assegnazione a tempo indeterminato (blocca completamente il mezzo)
                    nuovoStato = dataInizio <= DateTime.Now
                        ? StatoMezzo.InUso
                        : StatoMezzo.Prenotato;
                }

                // 9. IMPORTANTE: Se il mezzo ha altre prenotazioni future MA è disponibile ora,
                // mantieni Disponibile solo se la nuova assegnazione inizia in futuro
                var altrePrenotazioni = await _assegnazioneRepository.GetPeriodiOccupatiAsync(model.MezzoId);
                var haAssegnazioniInCorsoOggi = altrePrenotazioni.Any(a => a.IsInCorso);

                if (!haAssegnazioniInCorsoOggi && nuovoStato == StatoMezzo.Prenotato)
                {
                    // Il mezzo rimane Disponibile se ha solo prenotazioni future
                    // (non cambiamo stato se la nuova assegnazione è futura)
                    // Ma dobbiamo comunque salvare l'assegnazione
                }
                else
                {
                    // Aggiorna stato mezzo solo se necessario
                    mezzo.Stato = nuovoStato;
                }

                // 10. Aggiorna chilometraggio del mezzo se fornito
                if (model.ChilometraggioInizio.HasValue)
                {
                    mezzo.Chilometraggio = model.ChilometraggioInizio.Value;
                }

                // 11. Salva assegnazione e aggiorna mezzo
                await _assegnazioneRepository.AddAsync(assegnazione);
                await _mezzoRepository.UpdateAsync(mezzo);

                var tipoAssegnazione = model.DataFine.HasValue ? "temporanea" : "a tempo indeterminato";
                _logger.LogInformation(
                    "Assegnazione {Tipo} creata: ID {AssegnazioneId}, Mezzo {Targa}, Periodo {DataInizio} - {DataFine}, Stato {Stato}",
                    tipoAssegnazione, assegnazione.Id, mezzo.Targa, model.DataInizio, model.DataFine?.ToString() ?? "indefinito", nuovoStato);

                return (true, null, assegnazione.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la creazione dell'assegnazione per mezzo {MezzoId}", model.MezzoId);
                return (false, "Errore durante la creazione dell'assegnazione", null);
            }
        }

        //public async Task<(bool Success, string? ErrorMessage, Guid? AssegnazioneId)> CreateAsync(AssegnazioneMezzoCreateViewModel model)
        //{
        //    try
        //    {
        //        // 1. Validazione: Mezzo esiste
        //        var mezzo = await _mezzoRepository.GetByIdAsync(model.MezzoId);
        //        if (mezzo == null)
        //        {
        //            return (false, "Mezzo non trovato", null);
        //        }

        //        // 2. Validazione: Mezzo non ha già un'assegnazione attiva
        //        if (await _assegnazioneRepository.HasAssegnazioneAttivaAsync(model.MezzoId))
        //        {
        //            return (false, "Il mezzo ha già un'assegnazione o prenotazione attiva", null);
        //        }

        //        // 3. Validazione: Mezzo non è dismesso
        //        if (mezzo.Stato == StatoMezzo.Dismesso)
        //        {
        //            return (false, "Non è possibile assegnare un mezzo dismesso", null);
        //        }

        //        // 4. Validazione: DataInizio non troppo nel passato (max 7 giorni fa)
        //        if (model.DataInizio.Date < DateTime.Today.AddDays(-7))
        //        {
        //            return (false, "La data di inizio non può essere più vecchia di 7 giorni", null);
        //        }

        //        // 5. Validazione: Chilometraggio coerente con quello del mezzo
        //        if (model.ChilometraggioInizio.HasValue && mezzo.Chilometraggio.HasValue)
        //        {
        //            if (model.ChilometraggioInizio.Value < mezzo.Chilometraggio.Value)
        //            {
        //                return (false, $"Il chilometraggio iniziale non può essere inferiore al chilometraggio attuale del mezzo ({mezzo.Chilometraggio.Value:N0} km)", null);
        //            }
        //        }

        //        // 6. Mapping ViewModel → Entity
        //        var assegnazione = model.ToEntity();

        //        // 7. Determina il nuovo stato del mezzo
        //        StatoMezzo nuovoStato;
        //        if (model.IsPrenotazione)
        //        {
        //            nuovoStato = StatoMezzo.Prenotato;
        //            _logger.LogInformation("Creazione prenotazione mezzo {Targa} per utente {UserId} dal {DataInizio}",
        //                mezzo.Targa, model.UtenteId, model.DataInizio);
        //        }
        //        else
        //        {
        //            nuovoStato = StatoMezzo.InUso;
        //            _logger.LogInformation("Assegnazione mezzo {Targa} a utente {UserId} dal {DataInizio}",
        //                mezzo.Targa, model.UtenteId, model.DataInizio);
        //        }

        //        // 8. Aggiorna stato del mezzo
        //        mezzo.Stato = nuovoStato;

        //        // 9. Aggiorna chilometraggio del mezzo se fornito
        //        if (model.ChilometraggioInizio.HasValue)
        //        {
        //            mezzo.Chilometraggio = model.ChilometraggioInizio.Value;
        //        }

        //        // 10. Salva assegnazione e aggiorna mezzo
        //        await _assegnazioneRepository.AddAsync(assegnazione);
        //        await _mezzoRepository.UpdateAsync(mezzo);

        //        _logger.LogInformation("Assegnazione creata con successo: ID {AssegnazioneId}, Mezzo {Targa}, Stato {Stato}",
        //            assegnazione.Id, mezzo.Targa, nuovoStato);

        //        return (true, null, assegnazione.Id);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Errore durante la creazione dell'assegnazione per mezzo {MezzoId}", model.MezzoId);
        //        return (false, "Errore durante la creazione dell'assegnazione", null);
        //    }
        //}

        #endregion

        #region Close & Cancel Methods

        /// <summary>
        /// Chiude un'assegnazione (riconsegna mezzo)
        /// MODIFICATO: Considera altre assegnazioni attive per determinare stato mezzo
        /// </summary>
        public async Task<(bool Success, string? ErrorMessage)> CloseAsync(Guid assegnazioneId, AssegnazioneMezzoCloseViewModel model)
        {
            try
            {
                // 1. Validazione: Assegnazione esiste
                var assegnazione = await _assegnazioneRepository.GetByIdAsync(assegnazioneId);
                if (assegnazione == null)
                {
                    return (false, "Assegnazione non trovata");
                }

                // 2. Validazione: Assegnazione ancora attiva
                if (!assegnazione.IsAttiva)
                {
                    return (false, "L'assegnazione è già stata chiusa");
                }

                // 3. Validazione: DataFine >= DataInizio (con ore)
                if (model.DataFine < assegnazione.DataInizio)
                {
                    return (false, "La data e ora di riconsegna non può essere precedente alla data e ora di inizio");
                }

                // 4. Validazione: DataFine non nel futuro (con ore)
                if (model.DataFine > DateTime.Now)
                {
                    return (false, "La data e ora di riconsegna non può essere nel futuro");
                }

                // 5. Validazione: ChilometraggioFine >= ChilometraggioInizio
                if (model.ChilometraggioFine.HasValue && assegnazione.ChilometraggioInizio.HasValue)
                {
                    if (model.ChilometraggioFine.Value < assegnazione.ChilometraggioInizio.Value)
                    {
                        return (false, $"Il chilometraggio finale ({model.ChilometraggioFine.Value:N0} km) non può essere inferiore al chilometraggio iniziale ({assegnazione.ChilometraggioInizio.Value:N0} km)");
                    }
                }

                // 6. Ottieni il mezzo
                var mezzo = await _mezzoRepository.GetByIdAsync(assegnazione.MezzoId);
                if (mezzo == null)
                {
                    return (false, "Mezzo associato non trovato");
                }

                // 7. Aggiorna assegnazione con dati di chiusura
                model.UpdateEntityForClose(assegnazione);

                // 8. Aggiorna chilometraggio del mezzo
                if (model.ChilometraggioFine.HasValue)
                {
                    mezzo.Chilometraggio = model.ChilometraggioFine.Value;
                }

                // 9. MODIFICATO: Determina stato mezzo considerando TUTTE le assegnazioni attive
                var ora = DateTime.Now;
                var altreAssegnazioniAttive = (await _assegnazioneRepository.GetByMezzoIdAsync(mezzo.Id, includeChiuse: false))
                    .Where(a => a.Id != assegnazioneId && !a.IsDeleted)
                    .ToList();

                // 9a. Verifica se ci sono assegnazioni in corso OGGI
                var assegnazioniInCorsoOggi = altreAssegnazioniAttive
                    .Where(a => a.DataInizio <= ora && (a.DataFine == null || a.DataFine > ora))
                    .ToList();

                if (assegnazioniInCorsoOggi.Any())
                {
                    // C'è ancora un'assegnazione in corso oggi
                    mezzo.Stato = StatoMezzo.InUso;
                    _logger.LogInformation("Mezzo {Targa} rimane InUso (altre {Count} assegnazioni attive)",
                        mezzo.Targa, assegnazioniInCorsoOggi.Count);
                }
                else
                {
                    // 9b. Verifica se ci sono prenotazioni future
                    var haPrenotazioniFuture = altreAssegnazioniAttive
                        .Any(a => a.DataInizio > ora);

                    if (haPrenotazioniFuture)
                    {
                        // Ha prenotazioni future ma disponibile ora
                        mezzo.Stato = StatoMezzo.Disponibile;
                        _logger.LogInformation("Mezzo {Targa} disponibile (con prenotazioni future)", mezzo.Targa);
                    }
                    else
                    {
                        // Nessuna assegnazione attiva o futura
                        mezzo.Stato = StatoMezzo.Disponibile;
                        _logger.LogInformation("Mezzo {Targa} completamente disponibile", mezzo.Targa);
                    }
                }

                // 10. Salva modifiche
                await _assegnazioneRepository.UpdateAsync(assegnazione);
                await _mezzoRepository.UpdateAsync(mezzo);

                _logger.LogInformation(
                    "Assegnazione {AssegnazioneId} chiusa. Mezzo {Targa}, Utente {UserId}, Durata {Ore} ore",
                    assegnazione.Id, mezzo.Targa, assegnazione.UtenteId, assegnazione.DurataOre);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la chiusura dell'assegnazione {AssegnazioneId}", assegnazioneId);
                return (false, "Errore durante la chiusura dell'assegnazione");
            }
        }

        //public async Task<(bool Success, string? ErrorMessage)> CloseAsync(Guid assegnazioneId, AssegnazioneMezzoCloseViewModel model)
        //{
        //    try
        //    {
        //        // 1. Validazione: Assegnazione esiste
        //        var assegnazione = await _assegnazioneRepository.GetByIdAsync(assegnazioneId);
        //        if (assegnazione == null)
        //        {
        //            return (false, "Assegnazione non trovata");
        //        }

        //        // 2. Validazione: Assegnazione ancora attiva
        //        if (assegnazione.DataFine.HasValue)
        //        {
        //            return (false, "L'assegnazione è già stata chiusa");
        //        }

        //        // 3. Validazione: DataFine >= DataInizio
        //        if (model.DataFine.Date < assegnazione.DataInizio.Date)
        //        {
        //            return (false, "La data di riconsegna non può essere precedente alla data di inizio");
        //        }

        //        // 4. Validazione: DataFine non nel futuro
        //        if (model.DataFine.Date > DateTime.Today)
        //        {
        //            return (false, "La data di riconsegna non può essere nel futuro");
        //        }

        //        // 5. Validazione: ChilometraggioFine >= ChilometraggioInizio
        //        if (model.ChilometraggioFine.HasValue && assegnazione.ChilometraggioInizio.HasValue)
        //        {
        //            if (model.ChilometraggioFine.Value < assegnazione.ChilometraggioInizio.Value)
        //            {
        //                return (false, $"Il chilometraggio finale ({model.ChilometraggioFine.Value:N0} km) non può essere inferiore al chilometraggio iniziale ({assegnazione.ChilometraggioInizio.Value:N0} km)");
        //            }
        //        }

        //        // 6. Ottieni il mezzo
        //        var mezzo = await _mezzoRepository.GetByIdAsync(assegnazione.MezzoId);
        //        if (mezzo == null)
        //        {
        //            return (false, "Mezzo associato non trovato");
        //        }

        //        // 7. Aggiorna assegnazione con dati di chiusura
        //        model.UpdateEntityForClose(assegnazione);

        //        // 8. Aggiorna chilometraggio del mezzo
        //        if (model.ChilometraggioFine.HasValue)
        //        {
        //            mezzo.Chilometraggio = model.ChilometraggioFine.Value;
        //        }

        //        // 9. Verifica se ci sono altre assegnazioni/prenotazioni attive per questo mezzo
        //        var altreAssegnazioniAttive = await _assegnazioneRepository.GetByMezzoIdAsync(mezzo.Id, includeChiuse: false);
        //        var haAltreAssegnazioni = altreAssegnazioniAttive.Any(a => a.Id != assegnazioneId);

        //        // 10. Cambia stato mezzo solo se non ci sono altre assegnazioni attive
        //        if (!haAltreAssegnazioni)
        //        {
        //            mezzo.Stato = StatoMezzo.Disponibile;
        //            _logger.LogInformation("Mezzo {Targa} tornato disponibile dopo chiusura assegnazione", mezzo.Targa);
        //        }
        //        else
        //        {
        //            _logger.LogWarning("Mezzo {Targa} ha altre assegnazioni attive, stato non modificato", mezzo.Targa);
        //        }

        //        // 11. Salva modifiche
        //        await _assegnazioneRepository.UpdateAsync(assegnazione);
        //        await _mezzoRepository.UpdateAsync(mezzo);

        //        _logger.LogInformation("Assegnazione {AssegnazioneId} chiusa con successo. Mezzo {Targa}, Utente {UserId}, Durata {Giorni} giorni",
        //            assegnazione.Id, mezzo.Targa, assegnazione.UtenteId, assegnazione.DurataGiorni);

        //        return (true, null);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Errore durante la chiusura dell'assegnazione {AssegnazioneId}", assegnazioneId);
        //        return (false, "Errore durante la chiusura dell'assegnazione");
        //    }
        //}

        /// <summary>
        /// Cancella una prenotazione futura (soft delete)
        /// MODIFICATO: Considera altre assegnazioni per determinare stato mezzo
        /// </summary>
        public async Task<(bool Success, string? ErrorMessage)> CancellaPrenotazioneAsync(Guid assegnazioneId, string currentUserId, bool isAdmin)
        {
            try
            {
                // 1. Validazione: Assegnazione esiste
                var assegnazione = await _assegnazioneRepository.GetByIdAsync(assegnazioneId);
                if (assegnazione == null)
                {
                    return (false, "Assegnazione non trovata");
                }

                // 2. Validazione: È una prenotazione futura
                if (!assegnazione.IsPrenotazione && !isAdmin)
                {
                    return (false, "Può essere cancellata solo una prenotazione futura. Per assegnazioni in corso usa la riconsegna.");
                }

                // 3. Validazione: Autorizzazioni (proprietario o admin)
                if (!isAdmin && assegnazione.UtenteId != currentUserId)
                {
                    return (false, "Non hai i permessi per cancellare questa prenotazione");
                }

                // 4. Ottieni il mezzo
                var mezzo = await _mezzoRepository.GetByIdAsync(assegnazione.MezzoId);
                if (mezzo == null)
                {
                    return (false, "Mezzo associato non trovato");
                }

                // 5. Soft delete dell'assegnazione
                await _assegnazioneRepository.DeleteAsync(assegnazione);

                // 6. MODIFICATO: Determina stato mezzo considerando altre assegnazioni
                var ora = DateTime.Now;
                var altreAssegnazioniAttive = (await _assegnazioneRepository.GetByMezzoIdAsync(mezzo.Id, includeChiuse: false))
                    .Where(a => a.Id != assegnazioneId && !a.IsDeleted)
                    .ToList();

                // Verifica se ci sono assegnazioni in corso
                var hasAssegnazioniInCorso = altreAssegnazioniAttive
                    .Any(a => a.DataInizio <= ora && (a.DataFine == null || a.DataFine > ora));

                if (hasAssegnazioniInCorso)
                {
                    mezzo.Stato = StatoMezzo.InUso;
                }
                else
                {
                    // Verifica se ci sono altre prenotazioni future
                    var hasAltrePrenotazioni = altreAssegnazioniAttive
                        .Any(a => a.DataInizio > ora);

                    mezzo.Stato = hasAltrePrenotazioni
                        ? StatoMezzo.Disponibile  // Disponibile ora, con prenotazioni future
                        : StatoMezzo.Disponibile;  // Completamente disponibile
                }

                await _mezzoRepository.UpdateAsync(mezzo);

                _logger.LogInformation(
                    "Prenotazione {AssegnazioneId} cancellata. Mezzo {Targa} ora {Stato}",
                    assegnazione.Id, mezzo.Targa, mezzo.Stato);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la cancellazione della prenotazione {AssegnazioneId}", assegnazioneId);
                return (false, "Errore durante la cancellazione della prenotazione");
            }
        }

        //public async Task<(bool Success, string? ErrorMessage)> CancellaPrenotazioneAsync(Guid assegnazioneId, string currentUserId, bool isAdmin)
        //{
        //    try
        //    {
        //        // 1. Validazione: Assegnazione esiste
        //        var assegnazione = await _assegnazioneRepository.GetByIdAsync(assegnazioneId);
        //        if (assegnazione == null)
        //        {
        //            return (false, "Assegnazione non trovata");
        //        }

        //        // 2. Validazione: È una prenotazione futura
        //        if (!assegnazione.IsPrenotazione)
        //        {
        //            return (false, "Può essere cancellata solo una prenotazione futura. Per assegnazioni in corso usa la riconsegna.");
        //        }

        //        // 3. Validazione: Autorizzazioni (proprietario o admin)
        //        if (!isAdmin && assegnazione.UtenteId != currentUserId)
        //        {
        //            return (false, "Non hai i permessi per cancellare questa prenotazione");
        //        }

        //        // 4. Ottieni il mezzo
        //        var mezzo = await _mezzoRepository.GetByIdAsync(assegnazione.MezzoId);
        //        if (mezzo == null)
        //        {
        //            return (false, "Mezzo associato non trovato");
        //        }

        //        // 5. Soft delete dell'assegnazione
        //        await _assegnazioneRepository.DeleteAsync(assegnazione);

        //        // 6. Verifica se ci sono altre assegnazioni/prenotazioni attive
        //        var altreAssegnazioniAttive = await _assegnazioneRepository.GetByMezzoIdAsync(mezzo.Id, includeChiuse: false);
        //        var haAltreAssegnazioni = altreAssegnazioniAttive.Any(a => a.Id != assegnazioneId && !a.IsDeleted);

        //        // 7. Cambia stato mezzo solo se non ci sono altre assegnazioni attive
        //        if (!haAltreAssegnazioni)
        //        {
        //            mezzo.Stato = StatoMezzo.Disponibile;
        //            await _mezzoRepository.UpdateAsync(mezzo);
        //            _logger.LogInformation("Mezzo {Targa} tornato disponibile dopo cancellazione prenotazione", mezzo.Targa);
        //        }

        //        _logger.LogInformation("Prenotazione {AssegnazioneId} cancellata con successo da utente {UserId}. Mezzo {Targa}",
        //            assegnazioneId, currentUserId, mezzo.Targa);

        //        return (true, null);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Errore durante la cancellazione della prenotazione {AssegnazioneId}", assegnazioneId);
        //        return (false, "Errore durante la cancellazione della prenotazione");
        //    }
        //}

        #endregion
    }
}