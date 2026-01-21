using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;
using Trasformazioni.Models.ViewModels;

namespace Trasformazioni.Mappings
{
    /// <summary>
    /// Extension methods per il mapping tra entità Gara e ViewModels
    /// </summary>
    public static class GaraMappingExtensions
    {
        // ===================================
        // ENTITY → VIEWMODEL
        // ===================================

        /// <summary>
        /// Mappa un'entità Gara a GaraListViewModel
        /// </summary>
        public static GaraListViewModel ToListViewModel(this Gara gara)
        {
            // Calcola statistiche lotti
            var lotti = gara.Lotti?.ToList() ?? new List<Lotto>();
            var numeroLotti = lotti.Count;
            var lottiVinti = lotti.Count(l => l.Stato == StatoLotto.Vinto);
            var lottiPersi = lotti.Count(l => l.Stato == StatoLotto.Perso);
            var lottiInLavorazione = lotti.Count(l =>
                l.Stato != StatoLotto.Vinto &&
                l.Stato != StatoLotto.Perso &&
                l.Stato != StatoLotto.Scartato &&
                l.Stato != StatoLotto.Rifiutato);

            return new GaraListViewModel
            {
                Id = gara.Id,
                CodiceGara = gara.CodiceGara,
                Titolo = gara.Titolo,
                Tipologia = gara.Tipologia,
                Stato = gara.Stato,
                EnteAppaltante = gara.EnteAppaltante,
                Regione = gara.Regione,
                CIG = gara.CIG,
                CUP = gara.CUP,
                DataPubblicazione = gara.DataPubblicazione,
                DataTerminePresentazioneOfferte = gara.DataTerminePresentazioneOfferte,
                ImportoTotaleStimato = gara.ImportoTotaleStimato,
                NumeroLotti = numeroLotti,
                LottiVinti = lottiVinti,
                LottiPersi = lottiPersi,
                LottiInLavorazione = lottiInLavorazione,
                IsChiusaManualmente = gara.IsChiusaManualmente,
                DataChiusuraManuale = gara.DataChiusuraManuale,
                CreatedAt = gara.CreatedAt
            };
        }

        /// <summary>
        /// Mappa un'entità Gara a GaraDetailsViewModel
        /// Include tutte le relazioni e proprietà
        /// </summary>
        public static GaraDetailsViewModel ToDetailsViewModel(this Gara gara)
        {
            var viewModel = new GaraDetailsViewModel
            {
                Id = gara.Id,

                // Identificazione
                CodiceGara = gara.CodiceGara,
                Titolo = gara.Titolo,
                PNRR = gara.PNRR,
                Descrizione = gara.Descrizione,
                Tipologia = gara.Tipologia,
                Stato = gara.Stato,

                // Info Amministrazione
                EnteAppaltante = gara.EnteAppaltante,
                Regione = gara.Regione,
                NomePuntoOrdinante = gara.NomePuntoOrdinante,
                TelefonoPuntoOrdinante = gara.TelefonoPuntoOrdinante,

                // Codici Gara
                CIG = gara.CIG,
                CUP = gara.CUP,
                RDO = gara.RDO,
                Bando = gara.Bando,
                DenominazioneIniziativa = gara.DenominazioneIniziativa,
                Procedura = gara.Procedura,
                CriterioAggiudicazione = gara.CriterioAggiudicazione,

                // Date Critiche
                DataPubblicazione = gara.DataPubblicazione,
                DataInizioPresentazioneOfferte = gara.DataInizioPresentazioneOfferte,
                DataTermineRichiestaChiarimenti = gara.DataTermineRichiestaChiarimenti,
                DataTerminePresentazioneOfferte = gara.DataTerminePresentazioneOfferte,

                // Info Economiche
                ImportoTotaleStimato = gara.ImportoTotaleStimato,

                // Link
                LinkPiattaforma = gara.LinkPiattaforma,

                // Chiusura Manuale
                IsChiusaManualmente = gara.IsChiusaManualmente,
                DataChiusuraManuale = gara.DataChiusuraManuale,
                MotivoChiusuraManuale = gara.MotivoChiusuraManuale,
                ChiusaDaNomeCompleto = gara.ChiusaDa?.NomeCompleto,

                // Audit
                CreatedAt = gara.CreatedAt,
                CreatedBy = gara.CreatedBy,
                ModifiedAt = gara.ModifiedAt,
                ModifiedBy = gara.ModifiedBy,

                // Lotti
                Lotti = gara.Lotti?.Select(l => new LottoListItemViewModel
                {
                    Id = l.Id,
                    CodiceLotto = l.CodiceLotto,
                    Descrizione = l.Descrizione,
                    Tipologia = l.Tipologia,
                    Stato = l.Stato,
                    ImportoBaseAsta = l.ImportoBaseAsta,
                    Quotazione = l.Quotazione,
                    OperatoreAssegnatoNome = l.OperatoreAssegnato?.NomeCompleto,
                    NumeroPreventivi = l.Preventivi?.Count ?? 0,
                    NumeroRichiesteIntegrazione = l.RichiesteIntegrazione?.Count ?? 0
                }).OrderBy(l => l.CodiceLotto).ToList() ?? new List<LottoListItemViewModel>(),

                // Documenti
                Documenti = gara.Documenti?.Where(d => d.LottoId is null).Select(d => new DocumentoGaraListItemViewModel
                {
                    Id = d.Id,
                    //Tipo = d.Tipo,
                    TipoDocumentoId = d.TipoDocumentoId,
                    TipoDocumentoNome = d.TipoDocumento?.Nome ?? string.Empty,
                    TipoDocumentoCodiceRiferimento = d.TipoDocumento?.CodiceRiferimento,
                    NomeFile = d.NomeFile,
                    Descrizione = d.Descrizione,
                    DataUpload = d.CreatedAt
                }).OrderByDescending(d => d.DataUpload).ToList() ?? new List<DocumentoGaraListItemViewModel>(),

                //// Statistiche
                //NumeroDocumenti = gara.Documenti?.Count ?? 0

                // Checklist Documenti Richiesti
                ChecklistDocumenti = gara.DocumentiRichiesti?.Select(dr =>
                {
                    // Cerca se esiste un documento caricato di questo tipo (a livello gara, non lotto)
                    var documentoCaricato = gara.Documenti?
                        .FirstOrDefault(d => d.TipoDocumentoId == dr.TipoDocumentoId && d.LottoId == null);

                    return new ChecklistDocumentoViewModel
                    {
                        TipoDocumentoId = dr.TipoDocumentoId,
                        TipoDocumentoNome = dr.TipoDocumento?.Nome ?? string.Empty,
                        TipoDocumentoCodiceRiferimento = dr.TipoDocumento?.CodiceRiferimento,
                        IsPresente = documentoCaricato != null,
                        DataCaricamento = documentoCaricato?.CreatedAt,
                        DocumentoId = documentoCaricato?.Id,
                        NomeFile = documentoCaricato?.NomeFile
                    };
                }).OrderBy(c => c.TipoDocumentoNome).ToList() ?? new List<ChecklistDocumentoViewModel>()
            };

            return viewModel;
        }

        /// <summary>
        /// Mappa un'entità Gara a GaraEditViewModel
        /// Utilizzato per pre-popolare il form di modifica
        /// </summary>
        public static GaraEditViewModel ToEditViewModel(this Gara gara)
        {
            return new GaraEditViewModel
            {
                Id = gara.Id,

                // Identificazione
                CodiceGara = gara.CodiceGara,
                Titolo = gara.Titolo,
                PNRR = gara.PNRR,
                Descrizione = gara.Descrizione,
                Tipologia = gara.Tipologia,
                Stato = gara.Stato,

                // Info Amministrazione
                EnteAppaltante = gara.EnteAppaltante,
                Regione = gara.Regione,
                NomePuntoOrdinante = gara.NomePuntoOrdinante,
                TelefonoPuntoOrdinante = gara.TelefonoPuntoOrdinante,

                // Codici Gara
                CIG = gara.CIG,
                CUP = gara.CUP,
                RDO = gara.RDO,
                Bando = gara.Bando,
                DenominazioneIniziativa = gara.DenominazioneIniziativa,
                Procedura = gara.Procedura,
                CriterioAggiudicazione = gara.CriterioAggiudicazione,

                // Date Critiche
                DataPubblicazione = gara.DataPubblicazione,
                DataInizioPresentazioneOfferte = gara.DataInizioPresentazioneOfferte,
                DataTermineRichiestaChiarimenti = gara.DataTermineRichiestaChiarimenti,
                DataTerminePresentazioneOfferte = gara.DataTerminePresentazioneOfferte,

                // Info Economiche
                ImportoTotaleStimato = gara.ImportoTotaleStimato,

                // Link
                LinkPiattaforma = gara.LinkPiattaforma,

                // Chiusura Manuale
                IsChiusaManualmente = gara.IsChiusaManualmente,
                DataChiusuraManuale = gara.DataChiusuraManuale,
                MotivoChiusuraManuale = gara.MotivoChiusuraManuale,

                // Info Sistema
                NumeroLotti = gara.Lotti?.Count ?? 0,
                CreatedAt = gara.CreatedAt,
                CreatedBy = gara.CreatedBy,
                ModifiedAt = gara.ModifiedAt,
                ModifiedBy = gara.ModifiedBy,

                // Checklist Documenti Richiesti
                DocumentiRichiestiIds = gara.DocumentiRichiesti?
                    .Select(dr => dr.TipoDocumentoId)
                    .ToList() ?? new List<Guid>()
            };
        }

        // ===================================
        // VIEWMODEL → ENTITY
        // ===================================

        /// <summary>
        /// Crea un'entità Gara da GaraCreateViewModel
        /// Include normalizzazione automatica dei campi
        /// </summary>
        public static Gara ToEntity(this GaraCreateViewModel viewModel)
        {
            return new Gara
            {
                // Identificazione
                CodiceGara = NormalizzaStringa(viewModel.CodiceGara)!,
                Titolo = NormalizzaStringa(viewModel.Titolo)!,
                PNRR = viewModel.PNRR,
                Descrizione = NormalizzaStringa(viewModel.Descrizione),
                Tipologia = viewModel.Tipologia,
                Stato = StatoGara.Bozza,  // DEFAULT per nuove gare

                // Info Amministrazione
                EnteAppaltante = NormalizzaStringa(viewModel.EnteAppaltante),
                Regione = NormalizzaStringa(viewModel.Regione),
                NomePuntoOrdinante = NormalizzaStringa(viewModel.NomePuntoOrdinante),
                TelefonoPuntoOrdinante = NormalizzaStringa(viewModel.TelefonoPuntoOrdinante),

                // Codici Gara
                CIG = NormalizzaStringa(viewModel.CIG),
                CUP = NormalizzaStringa(viewModel.CUP),
                RDO = NormalizzaStringa(viewModel.RDO),
                Bando = NormalizzaStringa(viewModel.Bando),
                DenominazioneIniziativa = NormalizzaStringa(viewModel.DenominazioneIniziativa),
                Procedura = NormalizzaStringa(viewModel.Procedura),
                CriterioAggiudicazione = NormalizzaStringa(viewModel.CriterioAggiudicazione),

                // Date Critiche
                DataPubblicazione = viewModel.DataPubblicazione,
                DataInizioPresentazioneOfferte = viewModel.DataInizioPresentazioneOfferte,
                DataTermineRichiestaChiarimenti = viewModel.DataTermineRichiestaChiarimenti,
                DataTerminePresentazioneOfferte = viewModel.DataTerminePresentazioneOfferte,

                // Info Economiche
                ImportoTotaleStimato = viewModel.ImportoTotaleStimato,

                // Link
                LinkPiattaforma = NormalizzaStringa(viewModel.LinkPiattaforma)

                // CreatedAt, CreatedBy, IsDeleted gestiti da AuditInterceptor
            };
        }

        /// <summary>
        /// Aggiorna un'entità Gara esistente con i dati del GaraEditViewModel
        /// Include normalizzazione automatica dei campi
        /// </summary>
        public static void UpdateEntity(this GaraEditViewModel viewModel, Gara gara)
        {
            // Identificazione
            gara.CodiceGara = NormalizzaStringa(viewModel.CodiceGara)!;
            gara.Titolo = NormalizzaStringa(viewModel.Titolo)!;
            gara.PNRR = viewModel.PNRR;
            gara.Descrizione = NormalizzaStringa(viewModel.Descrizione);
            gara.Tipologia = viewModel.Tipologia;
            gara.Stato = viewModel.Stato;

            // Info Amministrazione
            gara.EnteAppaltante = NormalizzaStringa(viewModel.EnteAppaltante);
            gara.Regione = NormalizzaStringa(viewModel.Regione);
            gara.NomePuntoOrdinante = NormalizzaStringa(viewModel.NomePuntoOrdinante);
            gara.TelefonoPuntoOrdinante = NormalizzaStringa(viewModel.TelefonoPuntoOrdinante);

            // Codici Gara
            gara.CIG = NormalizzaStringa(viewModel.CIG);
            gara.CUP = NormalizzaStringa(viewModel.CUP);
            gara.RDO = NormalizzaStringa(viewModel.RDO);
            gara.Bando = NormalizzaStringa(viewModel.Bando);
            gara.DenominazioneIniziativa = NormalizzaStringa(viewModel.DenominazioneIniziativa);
            gara.Procedura = NormalizzaStringa(viewModel.Procedura);
            gara.CriterioAggiudicazione = NormalizzaStringa(viewModel.CriterioAggiudicazione);

            // Date Critiche
            gara.DataPubblicazione = viewModel.DataPubblicazione;
            gara.DataInizioPresentazioneOfferte = viewModel.DataInizioPresentazioneOfferte;
            gara.DataTermineRichiestaChiarimenti = viewModel.DataTermineRichiestaChiarimenti;
            gara.DataTerminePresentazioneOfferte = viewModel.DataTerminePresentazioneOfferte;

            // Info Economiche
            gara.ImportoTotaleStimato = viewModel.ImportoTotaleStimato;

            // Link
            gara.LinkPiattaforma = NormalizzaStringa(viewModel.LinkPiattaforma);

            // ModifiedAt e ModifiedBy gestiti da AuditInterceptor
        }

        // ===================================
        // HELPER METHODS
        // ===================================

        /// <summary>
        /// Normalizza una stringa: trim e null se vuota
        /// </summary>
        private static string? NormalizzaStringa(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            var trimmed = value.Trim();
            return string.IsNullOrEmpty(trimmed) ? null : trimmed;
        }
    }
}