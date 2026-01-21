using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;
using Trasformazioni.Models.ViewModels;

namespace Trasformazioni.Models.Mappings
{
    /// <summary>
    /// Extension methods per il mapping tra PartecipanteLotto e i suoi ViewModels
    /// Gestisce conversioni bidirezionali e calcoli derivati (stato partecipante, nome soggetto)
    /// </summary>
    public static class PartecipanteLottoMappingExtensions
    {
        // ===================================
        // ENTITY → VIEWMODEL (QUERY)
        // ===================================

        /// <summary>
        /// Mappa un'entità PartecipanteLotto a PartecipanteLottoListViewModel
        /// Include calcolo stato e nome soggetto
        /// </summary>
        public static PartecipanteLottoListViewModel ToListViewModel(this PartecipanteLotto partecipante)
        {
            return new PartecipanteLottoListViewModel
            {
                Id = partecipante.Id,
                LottoId = partecipante.LottoId,
                SoggettoId = partecipante.SoggettoId,

                // Dati Partecipante
                RagioneSociale = partecipante.RagioneSociale,
                OffertaEconomica = partecipante.OffertaEconomica,
                Note = partecipante.Note,

                // Flags
                IsAggiudicatario = partecipante.IsAggiudicatario,
                IsScartatoDallEnte = partecipante.IsScartatoDallEnte,
                HasSoggetto = partecipante.SoggettoId.HasValue,

                // Info Lotto
                CodiceLotto = partecipante.Lotto?.CodiceLotto ?? string.Empty,
                DescrizioneLotto = partecipante.Lotto?.Descrizione ?? string.Empty,

                // Info Gara
                CodiceGara = partecipante.Lotto?.Gara?.CodiceGara ?? string.Empty,
                EnteAppaltante = partecipante.Lotto?.Gara?.EnteAppaltante,

                // Info Soggetto (se presente)
                NomeSoggetto = GetNomeSoggetto(partecipante.Soggetto),

                // Stato calcolato
                StatoPartecipante = CalcolaStatoPartecipante(
                    partecipante.IsAggiudicatario,
                    partecipante.IsScartatoDallEnte
                ),

                // Audit
                CreatedAt = partecipante.CreatedAt,
                ModifiedAt = partecipante.ModifiedAt
            };
        }

        /// <summary>
        /// Mappa un'entità PartecipanteLotto a PartecipanteLottoDetailsViewModel
        /// Include tutte le relazioni e proprietà complete
        /// </summary>
        public static PartecipanteLottoDetailsViewModel ToDetailsViewModel(this PartecipanteLotto partecipante)
        {
            return new PartecipanteLottoDetailsViewModel
            {
                Id = partecipante.Id,
                LottoId = partecipante.LottoId,
                SoggettoId = partecipante.SoggettoId,

                // Dati Partecipante
                RagioneSociale = partecipante.RagioneSociale,
                OffertaEconomica = partecipante.OffertaEconomica,
                Note = partecipante.Note,

                // Flags
                IsAggiudicatario = partecipante.IsAggiudicatario,
                IsScartatoDallEnte = partecipante.IsScartatoDallEnte,
                HasSoggetto = partecipante.SoggettoId.HasValue,

                // Stato calcolato
                StatoPartecipante = CalcolaStatoPartecipante(
                    partecipante.IsAggiudicatario,
                    partecipante.IsScartatoDallEnte
                ),

                // Info Lotto
                CodiceLotto = partecipante.Lotto?.CodiceLotto ?? string.Empty,
                DescrizioneLotto = partecipante.Lotto?.Descrizione ?? string.Empty,
                TipologiaLotto = partecipante.Lotto?.Tipologia,
                StatoLotto = partecipante.Lotto?.Stato,
                ImportoBaseAstaLotto = partecipante.Lotto?.ImportoBaseAsta,

                // Info Gara
                CodiceGara = partecipante.Lotto?.Gara?.CodiceGara ?? string.Empty,
                TitoloGara = partecipante.Lotto?.Gara?.Titolo ?? string.Empty,
                TipologiaGara = partecipante.Lotto?.Gara?.Tipologia,
                StatoGara = partecipante.Lotto?.Gara?.Stato,
                EnteAppaltante = partecipante.Lotto?.Gara?.EnteAppaltante,
                Regione = partecipante.Lotto?.Gara?.Regione,
                DataTerminePresentazioneOfferte = partecipante.Lotto?.Gara?.DataTerminePresentazioneOfferte,

                // Info Soggetto (se collegato)
                TipoSoggetto = partecipante.Soggetto?.TipoSoggetto,
                DenominazioneSoggetto = GetNomeSoggetto(partecipante.Soggetto),
                CodiceFiscaleSoggetto = partecipante.Soggetto?.CodiceFiscale,
                EmailSoggetto = partecipante.Soggetto?.Email,
                TelefonoSoggetto = partecipante.Soggetto?.Telefono,
                CittaSoggetto = partecipante.Soggetto?.Citta,
                ProvinciaSoggetto = partecipante.Soggetto?.Provincia,

                // Audit - Creazione
                CreatedAt = partecipante.CreatedAt,
                CreatedBy = partecipante.CreatedBy,

                // Audit - Modifica
                ModifiedAt = partecipante.ModifiedAt,
                ModifiedBy = partecipante.ModifiedBy
            };
        }

        // ===================================
        // VIEWMODEL → ENTITY (COMANDI)
        // ===================================

        /// <summary>
        /// Crea un'entità PartecipanteLotto da PartecipanteLottoCreateViewModel
        /// I campi audit vengono gestiti automaticamente dall'AuditInterceptor
        /// </summary>
        public static PartecipanteLotto ToEntity(this PartecipanteLottoCreateViewModel viewModel)
        {
            return new PartecipanteLotto
            {
                LottoId = viewModel.LottoId,
                SoggettoId = viewModel.SoggettoId,
                RagioneSociale = NormalizzaStringa(viewModel.RagioneSociale) ?? string.Empty,
                OffertaEconomica = viewModel.OffertaEconomica,
                IsAggiudicatario = viewModel.IsAggiudicatario,
                IsScartatoDallEnte = viewModel.IsScartatoDallEnte,
                Note = NormalizzaStringa(viewModel.Note)
            };
        }

        /// <summary>
        /// Aggiorna un'entità PartecipanteLotto esistente con dati da PartecipanteLottoEditViewModel
        /// I campi audit (ModifiedAt, ModifiedBy) vengono gestiti automaticamente dall'AuditInterceptor
        /// </summary>
        public static void UpdateEntity(this PartecipanteLottoEditViewModel viewModel, PartecipanteLotto entity)
        {
            // Aggiorna solo i campi modificabili
            entity.SoggettoId = viewModel.SoggettoId;
            entity.RagioneSociale = NormalizzaStringa(viewModel.RagioneSociale) ?? string.Empty;
            entity.OffertaEconomica = viewModel.OffertaEconomica;
            entity.IsAggiudicatario = viewModel.IsAggiudicatario;
            entity.IsScartatoDallEnte = viewModel.IsScartatoDallEnte;
            entity.Note = NormalizzaStringa(viewModel.Note);

            // LottoId NON viene aggiornato (relazione immutabile)
            // I campi audit vengono gestiti dall'AuditInterceptor
        }

        /// <summary>
        /// Mappa un'entità PartecipanteLotto a PartecipanteLottoEditViewModel
        /// Usato per popolare la form di modifica
        /// </summary>
        public static PartecipanteLottoEditViewModel ToEditViewModel(this PartecipanteLotto partecipante)
        {
            return new PartecipanteLottoEditViewModel
            {
                Id = partecipante.Id,
                LottoId = partecipante.LottoId,
                SoggettoId = partecipante.SoggettoId,
                RagioneSociale = partecipante.RagioneSociale,
                OffertaEconomica = partecipante.OffertaEconomica,
                IsAggiudicatario = partecipante.IsAggiudicatario,
                IsScartatoDallEnte = partecipante.IsScartatoDallEnte,
                Note = partecipante.Note,

                // Info visualizzazione (readonly)
                CodiceLotto = partecipante.Lotto?.CodiceLotto,
                DescrizioneLotto = partecipante.Lotto?.Descrizione,
                NomeSoggetto = GetNomeSoggetto(partecipante.Soggetto),

                // Audit - Creazione
                CreatedAt = partecipante.CreatedAt,
                CreatedBy = partecipante.CreatedBy,
                ModifiedAt = partecipante.ModifiedAt,
                ModifiedBy = partecipante.ModifiedBy,
            };
        }

        // ===================================
        // METODI HELPER PRIVATI
        // ===================================

        /// <summary>
        /// Calcola lo stato del partecipante in base ai flag
        /// </summary>
        private static string CalcolaStatoPartecipante(bool isAggiudicatario, bool isScartatoDallEnte)
        {
            if (isAggiudicatario)
                return "Aggiudicatario";

            if (isScartatoDallEnte)
                return "Scartato";

            return "Partecipante";
        }

        /// <summary>
        /// Ottiene il nome completo o denominazione del Soggetto
        /// </summary>
        private static string? GetNomeSoggetto(Soggetto? soggetto)
        {
            if (soggetto == null)
                return null;

            if (soggetto.TipoSoggetto == TipoSoggetto.Azienda)
            {
                return soggetto.Denominazione;
            }
            else // PersonaFisica
            {
                var nome = soggetto.Nome?.Trim() ?? string.Empty;
                var cognome = soggetto.Cognome?.Trim() ?? string.Empty;

                if (string.IsNullOrEmpty(nome) && string.IsNullOrEmpty(cognome))
                    return soggetto.Denominazione; // Fallback

                return $"{nome} {cognome}".Trim();
            }
        }

        /// <summary>
        /// Normalizza stringhe: trim, null se vuota/whitespace
        /// </summary>
        private static string? NormalizzaStringa(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            return input.Trim();
        }
    }
}