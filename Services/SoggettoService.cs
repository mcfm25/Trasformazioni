using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using Trasformazioni.Data.Repositories;
using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;
using Trasformazioni.Models.ViewModels;
using Trasformazioni.Services.Interfaces;

namespace Trasformazioni.Services
{
    /// <summary>
    /// Implementazione della business logic per la gestione soggetti
    /// </summary>
    public class SoggettoService : ISoggettoService
    {
        private readonly ISoggettoRepository _soggettoRepository;
        private readonly ILogger<SoggettoService> _logger;

        public SoggettoService(
            ISoggettoRepository soggettoRepository,
            ILogger<SoggettoService> logger)
        {
            _soggettoRepository = soggettoRepository;
            _logger = logger;
        }

        // ===================================
        // QUERY - LETTURA
        // ===================================

        /// <summary>
        /// Ottiene tutti i soggetti
        /// </summary>
        public async Task<IEnumerable<SoggettoListViewModel>> GetAllAsync()
        {
            try
            {
                var soggetti = await _soggettoRepository.GetAllAsync();
                return soggetti.Select(s => MapToListViewModel(s));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero di tutti i soggetti");
                throw;
            }
        }

        /// <summary>
        /// Ottiene il dettaglio di un soggetto
        /// </summary>
        public async Task<SoggettoDetailsViewModel?> GetByIdAsync(Guid id)
        {
            try
            {
                var soggetto = await _soggettoRepository.GetByIdAsync(id);
                return soggetto != null ? MapToDetailsViewModel(soggetto) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero del soggetto {SoggettoId}", id);
                throw;
            }
        }

        /// <summary>
        /// Ottiene tutti i clienti
        /// </summary>
        public async Task<IEnumerable<SoggettoListViewModel>> GetClientiAsync()
        {
            try
            {
                var clienti = await _soggettoRepository.GetClientiAsync();
                return clienti.Select(s => MapToListViewModel(s));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei clienti");
                throw;
            }
        }

        /// <summary>
        /// Ottiene tutti i fornitori
        /// </summary>
        public async Task<IEnumerable<SoggettoListViewModel>> GetFornitoriAsync()
        {
            try
            {
                var fornitori = await _soggettoRepository.GetFornitoriAsync();
                return fornitori.Select(s => MapToListViewModel(s));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei fornitori");
                throw;
            }
        }

        /// <summary>
        /// Ottiene soggetti filtrati
        /// </summary>
        public async Task<IEnumerable<SoggettoListViewModel>> GetFilteredAsync(
            TipoSoggetto? tipo = null,
            NaturaGiuridica? natura = null,
            bool? isCliente = null,
            bool? isFornitore = null,
            string? searchTerm = null)
        {
            try
            {
                IEnumerable<Soggetto> soggetti;

                // Se c'è un termine di ricerca, usa la ricerca full-text
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    soggetti = await _soggettoRepository.SearchAsync(searchTerm);
                }
                else
                {
                    soggetti = await _soggettoRepository.GetFilteredAsync(tipo, natura, isCliente, isFornitore);
                }

                return soggetti.Select(s => MapToListViewModel(s));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei soggetti filtrati");
                throw;
            }
        }

        /// <summary>
        /// Cerca soggetti per testo
        /// </summary>
        public async Task<IEnumerable<SoggettoListViewModel>> SearchAsync(string searchTerm)
        {
            try
            {
                var soggetti = await _soggettoRepository.SearchAsync(searchTerm);
                return soggetti.Select(s => MapToListViewModel(s));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la ricerca soggetti con termine: {SearchTerm}", searchTerm);
                throw;
            }
        }
        
        /// <summary>
        /// Ottiene soggetti paginati con filtri e ordinamento
        /// </summary>
        public async Task<PagedResult<SoggettoListViewModel>> GetPagedAsync(SoggettoFilterViewModel filters)
        {
            try
            {
                // 1. Validazione parametri paginazione
                if (filters.PageNumber < 1)
                    filters.PageNumber = 1;

                if (filters.PageSize < 1)
                    filters.PageSize = 20;

                if (filters.PageSize > 100)
                    filters.PageSize = 100; // Limite massimo per prevenire abusi

                // 2. Ottieni dati paginati dal repository
                var (items, totalCount) = await _soggettoRepository.GetPagedAsync(filters);

                // 3. Mappa entità a ViewModels
                var viewModels = items.Select(s => MapToListViewModel(s)).ToList();

                // 4. Costruisci risultato paginato
                var pagedResult = new PagedResult<SoggettoListViewModel>
                {
                    Items = viewModels,
                    PageNumber = filters.PageNumber,
                    PageSize = filters.PageSize,
                    TotalItems = totalCount
                };

                _logger.LogInformation(
                    "Recuperati {Count} soggetti su {Total} totali - Pagina {Page}/{TotalPages}",
                    viewModels.Count, totalCount, pagedResult.PageNumber, pagedResult.TotalPages);

                return pagedResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero paginato dei soggetti");
                throw;
            }
        }

        // ===================================
        // COMANDI - SCRITTURA
        // ===================================

        /// <summary>
        /// Crea un nuovo soggetto
        /// </summary>
        public async Task<(bool Success, string? ErrorMessage, Guid? SoggettoId)> CreateAsync(
            SoggettoCreateViewModel model,
            string currentUserId)
        {
            try
            {
                // 1. Validazione
                var (isValid, errors) = await ValidateAsync(model, null);
                if (!isValid)
                {
                    return (false, string.Join("; ", errors), null);
                }

                // 2. Mapping da ViewModel a Entity
                var soggetto = new Soggetto
                {
                    Id = Guid.NewGuid(),
                    CodiceInterno = model.CodiceInterno?.Trim(),
                    TipoSoggetto = model.TipoSoggetto,
                    NaturaGiuridica = model.NaturaGiuridica,
                    IsCliente = model.IsCliente,
                    IsFornitore = model.IsFornitore,

                    // Dati anagrafici
                    Denominazione = model.Denominazione?.Trim(),
                    Nome = model.Nome?.Trim(),
                    Cognome = model.Cognome?.Trim(),
                    CodiceFiscale = model.CodiceFiscale?.Trim().ToUpper(),
                    PartitaIVA = model.PartitaIVA?.Trim(),
                    CodiceSDI = model.CodiceSDI?.Trim(),
                    CodiceIPA = model.CodiceIPA?.Trim().ToUpper(),
                    Referente = model.Referente?.Trim(),

                    // Contatti
                    Email = model.Email.Trim().ToLower(),
                    Telefono = model.Telefono?.Trim(),
                    PEC = model.PEC?.Trim().ToLower(),

                    // Indirizzo
                    TipoVia = model.TipoVia?.Trim(),
                    NomeVia = model.NomeVia?.Trim(),
                    NumeroCivico = model.NumeroCivico?.Trim(),
                    Citta = model.Citta?.Trim(),
                    CAP = model.CAP?.Trim(),
                    Provincia = model.Provincia?.Trim().ToUpper(),
                    Nazione = model.Nazione?.Trim(),

                    // Dati commerciali
                    CondizioniPagamento = model.CondizioniPagamento?.Trim(),
                    IBAN = model.IBAN?.Trim().ToUpper().Replace(" ", ""),
                    ScontoPartner = model.ScontoPartner,

                    // Altro
                    Note = model.Note?.Trim(),

                    // Audit (gestiti da interceptor, ma possiamo impostare CreatedBy manualmente)
                    //CreatedAt = DateTime.Now,
                    //CreatedBy = currentUserId,
                    //ModifiedAt = DateTime.Now,
                    //ModifiedBy = currentUserId,
                    //IsDeleted = false
                };

                // 3. Salva nel database
                await _soggettoRepository.AddAsync(soggetto);
                await _soggettoRepository.SaveChangesAsync();

                _logger.LogInformation(
                    "Soggetto creato con successo: {SoggettoId} - {Nome} da utente {UserId}",
                    soggetto.Id, soggetto.NomeCompleto, currentUserId);

                return (true, null, soggetto.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la creazione del soggetto");
                return (false, "Errore durante la creazione del soggetto", null);
            }
        }

        /// <summary>
        /// Aggiorna un soggetto esistente
        /// </summary>
        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(
            SoggettoEditViewModel model,
            string currentUserId)
        {
            try
            {
                // 1. Verifica esistenza
                var soggetto = await _soggettoRepository.GetByIdAsync(model.Id);
                if (soggetto == null)
                {
                    return (false, "Soggetto non trovato");
                }

                // 2. Validazione
                var createModel = MapEditToCreate(model);
                var (isValid, errors) = await ValidateAsync(createModel, model.Id);
                if (!isValid)
                {
                    return (false, string.Join("; ", errors));
                }

                // 3. Aggiorna proprietà
                soggetto.CodiceInterno = model.CodiceInterno?.Trim();
                soggetto.TipoSoggetto = model.TipoSoggetto;
                soggetto.NaturaGiuridica = model.NaturaGiuridica;
                soggetto.IsCliente = model.IsCliente;
                soggetto.IsFornitore = model.IsFornitore;

                soggetto.Denominazione = model.Denominazione?.Trim();
                soggetto.Nome = model.Nome?.Trim();
                soggetto.Cognome = model.Cognome?.Trim();
                soggetto.CodiceFiscale = model.CodiceFiscale?.Trim().ToUpper();
                soggetto.PartitaIVA = model.PartitaIVA?.Trim();
                soggetto.CodiceSDI = model.CodiceSDI?.Trim();
                soggetto.CodiceIPA = model.CodiceIPA?.Trim().ToUpper();
                soggetto.Referente = model.Referente?.Trim();

                soggetto.Email = model.Email.Trim().ToLower();
                soggetto.Telefono = model.Telefono?.Trim();
                soggetto.PEC = model.PEC?.Trim().ToLower();

                soggetto.TipoVia = model.TipoVia?.Trim();
                soggetto.NomeVia = model.NomeVia?.Trim();
                soggetto.NumeroCivico = model.NumeroCivico?.Trim();
                soggetto.Citta = model.Citta?.Trim();
                soggetto.CAP = model.CAP?.Trim();
                soggetto.Provincia = model.Provincia?.Trim().ToUpper();
                soggetto.Nazione = model.Nazione?.Trim();

                soggetto.CondizioniPagamento = model.CondizioniPagamento?.Trim();
                soggetto.IBAN = model.IBAN?.Trim().ToUpper().Replace(" ", "");
                soggetto.ScontoPartner = model.ScontoPartner;

                soggetto.Note = model.Note?.Trim();

                // Audit (ModifiedAt e ModifiedBy gestiti da interceptor)
                //soggetto.ModifiedAt = DateTime.Now;
                //soggetto.ModifiedBy = currentUserId;

                // 4. Salva modifiche
                await _soggettoRepository.UpdateAsync(soggetto);
                await _soggettoRepository.SaveChangesAsync();

                _logger.LogInformation(
                    "Soggetto aggiornato con successo: {SoggettoId} - {Nome} da utente {UserId}",
                    soggetto.Id, soggetto.NomeCompleto, currentUserId);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'aggiornamento del soggetto {SoggettoId}", model.Id);
                return (false, "Errore durante l'aggiornamento del soggetto");
            }
        }

        /// <summary>
        /// Elimina un soggetto (soft delete)
        /// </summary>
        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(Guid id, string currentUserId)
        {
            try
            {
                // 1. Verifica esistenza
                var soggetto = await _soggettoRepository.GetByIdAsync(id);
                if (soggetto == null)
                {
                    return (false, "Soggetto non trovato");
                }

                // 2. TODO: Verifica che il soggetto non sia usato in altre entità
                // (es. Gare, Preventivi, Ordini, Fatture)
                // Per ora permettiamo sempre la cancellazione

                // 3. Soft delete
                soggetto.IsDeleted = true;
                soggetto.DeletedAt = DateTime.Now;
                soggetto.DeletedBy = currentUserId;

                await _soggettoRepository.UpdateAsync(soggetto);
                await _soggettoRepository.SaveChangesAsync();

                _logger.LogInformation(
                    "Soggetto eliminato (soft delete): {SoggettoId} - {Nome} da utente {UserId}",
                    id, soggetto.NomeCompleto, currentUserId);

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'eliminazione del soggetto {SoggettoId}", id);
                return (false, "Errore durante l'eliminazione del soggetto");
            }
        }

        // ===================================
        // VALIDAZIONI
        // ===================================

        /// <summary>
        /// Valida i dati di un soggetto
        /// </summary>
        public async Task<(bool IsValid, List<string> Errors)> ValidateAsync(
            SoggettoCreateViewModel model,
            Guid? excludeId = null)
        {
            var errors = new List<string>();

            // 1. Validazione: Almeno un ruolo (Cliente o Fornitore)
            if (!model.IsCliente && !model.IsFornitore)
            {
                errors.Add("Il soggetto deve essere almeno Cliente o Fornitore");
            }

            // PersonaFisica NON può essere PA
            if (model.TipoSoggetto == TipoSoggetto.PersonaFisica && model.NaturaGiuridica == NaturaGiuridica.PA)
            {
                errors.Add("Una Persona Fisica non può essere una Pubblica Amministrazione");
            }

            // Se PA, CodiceIPA è obbligatorio
            if (model.NaturaGiuridica == NaturaGiuridica.PA)
            {
                if (string.IsNullOrWhiteSpace(model.CodiceIPA))
                {
                    errors.Add("Il Codice IPA è obbligatorio per le Pubbliche Amministrazioni");
                }
                else if (model.CodiceIPA.Length != 6)
                {
                    errors.Add("Il Codice IPA deve essere di 6 caratteri");
                }
            }

            // 2. Validazione: Campi obbligatori per AZIENDA
            if (model.TipoSoggetto == TipoSoggetto.Azienda)
            {
                if (string.IsNullOrWhiteSpace(model.Denominazione))
                {
                    errors.Add("La Denominazione è obbligatoria per le aziende");
                }

                if (string.IsNullOrWhiteSpace(model.PartitaIVA))
                {
                    errors.Add("La Partita IVA è obbligatoria per le aziende");
                }
                else
                {
                    // Verifica unicità Partita IVA
                    var partitaIvaExists = await _soggettoRepository.ExistsByPartitaIVAAsync(model.PartitaIVA, excludeId);
                    if (partitaIvaExists)
                    {
                        errors.Add("Partita IVA già presente nel sistema");
                    }
                }
            }

            // 3. Validazione: Campi obbligatori per PERSONA FISICA
            if (model.TipoSoggetto == TipoSoggetto.PersonaFisica)
            {
                if (string.IsNullOrWhiteSpace(model.Nome))
                {
                    errors.Add("Il Nome è obbligatorio per le persone fisiche");
                }

                if (string.IsNullOrWhiteSpace(model.Cognome))
                {
                    errors.Add("Il Cognome è obbligatorio per le persone fisiche");
                }

                if (string.IsNullOrWhiteSpace(model.CodiceFiscale))
                {
                    errors.Add("Il Codice Fiscale è obbligatorio per le persone fisiche");
                }
                else
                {
                    // Verifica formato Codice Fiscale italiano (16 caratteri alfanumerici)
                    if (!IsValidCodiceFiscale(model.CodiceFiscale))
                    {
                        errors.Add("Il Codice Fiscale deve essere di 16 caratteri alfanumerici");
                    }

                    // Verifica unicità Codice Fiscale
                    var cfExists = await _soggettoRepository.ExistsByCodiceFiscaleAsync(model.CodiceFiscale, excludeId);
                    if (cfExists)
                    {
                        errors.Add("Codice Fiscale già presente nel sistema");
                    }
                }
            }

            // 4. Validazione: Email obbligatoria e formato
            if (string.IsNullOrWhiteSpace(model.Email))
            {
                errors.Add("L'Email è obbligatoria");
            }
            else
            {
                if (!IsValidEmail(model.Email))
                {
                    errors.Add("Formato Email non valido");
                }

                // Verifica unicità Email
                var emailExists = await _soggettoRepository.ExistsByEmailAsync(model.Email, excludeId);
                if (emailExists)
                {
                    errors.Add("Email già presente nel sistema");
                }
            }

            // 5. Validazione: Codice Interno (se presente, deve essere unico)
            if (!string.IsNullOrWhiteSpace(model.CodiceInterno))
            {
                var codiceExists = await _soggettoRepository.ExistsByCodiceInternoAsync(model.CodiceInterno, excludeId);
                if (codiceExists)
                {
                    errors.Add("Codice Interno già presente nel sistema");
                }
            }

            // 6. Validazione: IBAN formato (se presente)
            if (!string.IsNullOrWhiteSpace(model.IBAN))
            {
                if (!IsValidIBAN(model.IBAN))
                {
                    errors.Add("Formato IBAN non valido");
                }
            }

            // 7. Validazione: Codice SDI formato (se presente)
            if (!string.IsNullOrWhiteSpace(model.CodiceSDI))
            {
                if (model.CodiceSDI.Length != 7)
                {
                    errors.Add("Il Codice SDI deve essere di 7 caratteri");
                }
            }

            // 8. Validazione: PEC formato (se presente)
            if (!string.IsNullOrWhiteSpace(model.PEC))
            {
                if (!IsValidEmail(model.PEC))
                {
                    errors.Add("Formato PEC non valido");
                }
            }

            // 9. Validazione: Sconto Partner (se Fornitore)
            if (model.IsFornitore && model.ScontoPartner.HasValue)
            {
                if (model.ScontoPartner.Value < 0 || model.ScontoPartner.Value > 100)
                {
                    errors.Add("Lo Sconto Partner deve essere tra 0 e 100");
                }
            }

            return (errors.Count == 0, errors);
        }

        /// <summary>
        /// Verifica se un codice interno è già utilizzato
        /// </summary>
        public async Task<bool> IsCodiceInternoUniqueAsync(string codiceInterno, Guid? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(codiceInterno))
                return true;

            var exists = await _soggettoRepository.ExistsByCodiceInternoAsync(codiceInterno, excludeId);
            return !exists;
        }

        /// <summary>
        /// Verifica se una partita IVA è già utilizzata
        /// </summary>
        public async Task<bool> IsPartitaIVAUniqueAsync(string partitaIva, Guid? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(partitaIva))
                return true;

            var exists = await _soggettoRepository.ExistsByPartitaIVAAsync(partitaIva, excludeId);
            return !exists;
        }

        /// <summary>
        /// Verifica se un codice fiscale è già utilizzato
        /// </summary>
        public async Task<bool> IsCodiceFiscaleUniqueAsync(string codiceFiscale, Guid? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(codiceFiscale))
                return true;

            var exists = await _soggettoRepository.ExistsByCodiceFiscaleAsync(codiceFiscale, excludeId);
            return !exists;
        }

        /// <summary>
        /// Verifica se un'email è già utilizzata
        /// </summary>
        public async Task<bool> IsEmailUniqueAsync(string email, Guid? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(email))
                return true;

            var exists = await _soggettoRepository.ExistsByEmailAsync(email, excludeId);
            return !exists;
        }

        // ===================================
        // UTILITY / STATISTICHE
        // ===================================

        /// <summary>
        /// Ottiene statistiche sui soggetti
        /// </summary>
        public async Task<SoggettoStatisticheViewModel> GetStatisticheAsync()
        {
            try
            {
                var totale = await _soggettoRepository.CountAsync();
                var clienti = await _soggettoRepository.CountClientiAsync();
                var fornitori = await _soggettoRepository.CountFornitoriAsync();

                return new SoggettoStatisticheViewModel
                {
                    TotaleSoggetti = totale,
                    TotaleClienti = clienti,
                    TotaleFornitori = fornitori,
                    ClientiEFornitori = clienti + fornitori - totale // Soggetti che sono sia clienti che fornitori
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero delle statistiche soggetti");
                throw;
            }
        }

        /// <summary>
        /// Verifica se un soggetto esiste
        /// </summary>
        public async Task<bool> ExistsAsync(Guid id)
        {
            var soggetto = await _soggettoRepository.GetByIdAsync(id);
            return soggetto != null;
        }

        // ===================================
        // METODI PRIVATI - MAPPING
        // ===================================

        private SoggettoListViewModel MapToListViewModel(Soggetto soggetto)
        {
            return new SoggettoListViewModel
            {
                Id = soggetto.Id,
                CodiceInterno = soggetto.CodiceInterno,
                TipoSoggetto = soggetto.TipoSoggetto,
                NaturaGiuridica = soggetto.NaturaGiuridica,
                NomeCompleto = soggetto.NomeCompleto,
                Email = soggetto.Email,
                Telefono = soggetto.Telefono,
                Citta = soggetto.Citta,
                IsCliente = soggetto.IsCliente,
                IsFornitore = soggetto.IsFornitore,
                RuoloDescrizione = soggetto.RuoloDescrizione
            };
        }

        private SoggettoDetailsViewModel MapToDetailsViewModel(Soggetto soggetto)
        {
            return new SoggettoDetailsViewModel
            {
                Id = soggetto.Id,
                CodiceInterno = soggetto.CodiceInterno,
                TipoSoggetto = soggetto.TipoSoggetto,
                NaturaGiuridica = soggetto.NaturaGiuridica,
                IsCliente = soggetto.IsCliente,
                IsFornitore = soggetto.IsFornitore,

                Denominazione = soggetto.Denominazione,
                Nome = soggetto.Nome,
                Cognome = soggetto.Cognome,
                CodiceFiscale = soggetto.CodiceFiscale,
                PartitaIVA = soggetto.PartitaIVA,
                CodiceSDI = soggetto.CodiceSDI,
                CodiceIPA = soggetto.CodiceIPA,
                Referente = soggetto.Referente,

                Email = soggetto.Email,
                Telefono = soggetto.Telefono,
                PEC = soggetto.PEC,

                TipoVia = soggetto.TipoVia,
                NomeVia = soggetto.NomeVia,
                NumeroCivico = soggetto.NumeroCivico,
                Citta = soggetto.Citta,
                CAP = soggetto.CAP,
                Provincia = soggetto.Provincia,
                Nazione = soggetto.Nazione,

                CondizioniPagamento = soggetto.CondizioniPagamento,
                IBAN = soggetto.IBAN,
                ScontoPartner = soggetto.ScontoPartner,

                Note = soggetto.Note,

                // Computed
                NomeCompleto = soggetto.NomeCompleto,
                IndirizzoCompleto = soggetto.IndirizzoCompleto,
                RuoloDescrizione = soggetto.RuoloDescrizione,

                // Audit
                CreatedAt = soggetto.CreatedAt,
                CreatedBy = soggetto.CreatedBy,
                ModifiedAt = soggetto.ModifiedAt,
                ModifiedBy = soggetto.ModifiedBy
            };
        }

        private SoggettoCreateViewModel MapEditToCreate(SoggettoEditViewModel edit)
        {
            return new SoggettoCreateViewModel
            {
                CodiceInterno = edit.CodiceInterno,
                TipoSoggetto = edit.TipoSoggetto,
                NaturaGiuridica = edit.NaturaGiuridica,
                IsCliente = edit.IsCliente,
                IsFornitore = edit.IsFornitore,
                Denominazione = edit.Denominazione,
                Nome = edit.Nome,
                Cognome = edit.Cognome,
                CodiceFiscale = edit.CodiceFiscale,
                PartitaIVA = edit.PartitaIVA,
                CodiceSDI = edit.CodiceSDI,
                Referente = edit.Referente,
                Email = edit.Email,
                Telefono = edit.Telefono,
                PEC = edit.PEC,
                TipoVia = edit.TipoVia,
                NomeVia = edit.NomeVia,
                NumeroCivico = edit.NumeroCivico,
                Citta = edit.Citta,
                CAP = edit.CAP,
                Provincia = edit.Provincia,
                Nazione = edit.Nazione,
                CondizioniPagamento = edit.CondizioniPagamento,
                IBAN = edit.IBAN,
                ScontoPartner = edit.ScontoPartner,
                Note = edit.Note,
                CodiceIPA = edit.CodiceIPA
            };
        }

        // ===================================
        // METODI PRIVATI - VALIDAZIONI FORMATO
        // ===================================

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                return regex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidCodiceFiscale(string codiceFiscale)
        {
            if (string.IsNullOrWhiteSpace(codiceFiscale))
                return false;

            // Formato italiano: 16 caratteri alfanumerici
            var regex = new Regex(@"^[A-Z]{6}\d{2}[A-Z]\d{2}[A-Z]\d{3}[A-Z]$", RegexOptions.IgnoreCase);
            return regex.IsMatch(codiceFiscale.Trim());
        }

        private bool IsValidIBAN(string iban)
        {
            if (string.IsNullOrWhiteSpace(iban))
                return false;

            // Rimuovi spazi
            iban = iban.Replace(" ", "").ToUpper();

            // Lunghezza minima/massima IBAN
            if (iban.Length < 15 || iban.Length > 34)
                return false;

            // Formato base: 2 lettere (paese) + 2 cifre (check) + alfanumerico
            var regex = new Regex(@"^[A-Z]{2}\d{2}[A-Z0-9]+$");
            return regex.IsMatch(iban);
        }
    }
}