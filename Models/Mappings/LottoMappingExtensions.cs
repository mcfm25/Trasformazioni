using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;
using Trasformazioni.Models.ViewModels;

namespace Trasformazioni.Mappings
{
    /// <summary>
    /// Extension methods per il mapping tra entità Lotto e ViewModels
    /// </summary>
    public static class LottoMappingExtensions
    {
        // ===================================
        // ENTITY → VIEWMODEL
        // ===================================

        /// <summary>
        /// Mappa un'entità Lotto a LottoListViewModel
        /// </summary>
        public static LottoListViewModel ToListViewModel(this Lotto lotto)
        {
            return new LottoListViewModel
            {
                Id = lotto.Id,
                GaraId = lotto.GaraId,

                // Identificazione
                CodiceLotto = lotto.CodiceLotto,
                Descrizione = lotto.Descrizione,
                Tipologia = lotto.Tipologia,
                Stato = lotto.Stato,

                // Info Gara
                CodiceGara = lotto.Gara?.CodiceGara ?? string.Empty,
                TitoloGara = lotto.Gara?.Titolo ?? string.Empty,
                EnteAppaltante = lotto.Gara?.EnteAppaltante,

                // Info Economiche
                ImportoBaseAsta = lotto.ImportoBaseAsta,
                Quotazione = lotto.Quotazione,

                // Info Operatore
                OperatoreAssegnatoNome = lotto.OperatoreAssegnato?.NomeCompleto,
                OperatoreAssegnatoId = lotto.OperatoreAssegnatoId,

                // Date Critiche
                DataTerminePresentazioneOfferte = lotto.Gara?.DataTerminePresentazioneOfferte,
                DataInizioEsameEnte = lotto.DataInizioEsameEnte,

                // Statistiche
                NumeroPreventivi = lotto.Preventivi?.Count ?? 0,
                NumeroRichiesteIntegrazione = lotto.RichiesteIntegrazione?.Count ?? 0,
                RichiesteIntegrazioneAperte = lotto.RichiesteIntegrazione?.Count(r => !r.IsChiusa) ?? 0,
                NumeroPartecipanti = lotto.Partecipanti?.Count ?? 0,

                // Rifiuto
                MotivoRifiuto = lotto.MotivoRifiuto,

                // Audit
                CreatedAt = lotto.CreatedAt,
                ModifiedAt = lotto.ModifiedAt
            };
        }

        /// <summary>
        /// Mappa un'entità Lotto a LottoDetailsViewModel
        /// Include tutte le relazioni e proprietà
        /// </summary>
        public static LottoDetailsViewModel ToDetailsViewModel(this Lotto lotto)
        {
            return new LottoDetailsViewModel
            {
                Id = lotto.Id,
                GaraId = lotto.GaraId,

                // Identificazione
                CodiceLotto = lotto.CodiceLotto,
                Descrizione = lotto.Descrizione,
                Tipologia = lotto.Tipologia,
                Stato = lotto.Stato,

                // Rifiuto
                MotivoRifiuto = lotto.MotivoRifiuto,

                // Info Generali
                LinkPiattaforma = lotto.LinkPiattaforma,
                OperatoreAssegnatoId = lotto.OperatoreAssegnatoId,
                OperatoreAssegnatoNome = lotto.OperatoreAssegnato?.NomeCompleto,
                GiorniFornitura = lotto.GiorniFornitura,

                // Info Economiche
                ImportoBaseAsta = lotto.ImportoBaseAsta,
                Quotazione = lotto.Quotazione,

                // Info Contratto
                DurataContratto = lotto.DurataContratto,
                DataStipulaContratto = lotto.DataStipulaContratto,
                DataScadenzaContratto = lotto.DataScadenzaContratto,
                Fatturazione = lotto.Fatturazione,

                // Info Partecipazione
                RichiedeFideiussione = lotto.RichiedeFideiussione,

                // Esame Ente
                DataInizioEsameEnte = lotto.DataInizioEsameEnte,

                // Info Gara
                CodiceGara = lotto.Gara?.CodiceGara ?? string.Empty,
                TitoloGara = lotto.Gara?.Titolo ?? string.Empty,
                EnteAppaltante = lotto.Gara?.EnteAppaltante,
                Regione = lotto.Gara?.Regione,
                CIG = lotto.Gara?.CIG,
                DataPubblicazioneGara = lotto.Gara?.DataPubblicazione,
                DataTerminePresentazioneOfferte = lotto.Gara?.DataTerminePresentazioneOfferte,

                // Valutazione Tecnica
                DataValutazioneTecnica = lotto.Valutazione?.DataValutazioneTecnica,
                ValutatoreTecnicoNome = lotto.Valutazione?.ValutatoreTecnico?.NomeCompleto,
                TecnicaApprovata = lotto.Valutazione?.TecnicaApprovata,
                MotivoRifiutoTecnico = lotto.Valutazione?.MotivoRifiutoTecnico,
                NoteTecniche = lotto.Valutazione?.NoteTecniche,

                // Valutazione Economica
                DataValutazioneEconomica = lotto.Valutazione?.DataValutazioneEconomica,
                ValutatoreEconomicoNome = lotto.Valutazione?.ValutatoreEconomico?.NomeCompleto,
                EconomicaApprovata = lotto.Valutazione?.EconomicaApprovata,
                MotivoRifiutoEconomico = lotto.Valutazione?.MotivoRifiutoEconomico,
                NoteEconomiche = lotto.Valutazione?.NoteEconomiche,

                // Elaborazione
                PrezzoDesiderato = lotto.Elaborazione?.PrezzoDesiderato,
                PrezzoRealeUscita = lotto.Elaborazione?.PrezzoRealeUscita,
                MotivazioneAdattamento = lotto.Elaborazione?.MotivazioneAdattamento,
                NoteElaborazione = lotto.Elaborazione?.Note,
                DataElaborazione = lotto.Elaborazione?.CreatedAt,
                ElaboratoDa = lotto.Elaborazione?.CreatedBy,

                // Audit
                CreatedAt = lotto.CreatedAt,
                CreatedBy = lotto.CreatedBy,
                ModifiedAt = lotto.ModifiedAt,
                ModifiedBy = lotto.ModifiedBy,

                // Collezioni
                Preventivi = lotto.Preventivi?.Select(p => new PreventivoListItemViewModel
                {
                    Id = p.Id,
                    Descrizione = p.Descrizione,
                    NomeFornitore = p.Soggetto?.Denominazione ??
                                   (p.Soggetto != null ? $"{p.Soggetto.Nome} {p.Soggetto.Cognome}" : "N/D"),
                    Stato = p.Stato,
                    Importo = p.ImportoOfferto,
                    DataRichiesta = p.DataRichiesta,
                    DataScadenza = p.DataScadenza,
                    IsSelezionato = p.IsSelezionato
                }).OrderByDescending(p => p.DataRichiesta).ToList() ?? new List<PreventivoListItemViewModel>(),

                RichiesteIntegrazione = lotto.RichiesteIntegrazione?.Select(r => new RichiestaIntegrazioneListItemViewModel
                {
                    Id = r.Id,
                    NumeroProgressivo = r.NumeroProgressivo,
                    DataRichiestaEnte = r.DataRichiestaEnte,
                    DataRispostaAzienda = r.DataRispostaAzienda,
                    IsChiusa = r.IsChiusa,
                    Oggetto = TruncateText(r.TestoRichiestaEnte, 100) // Tronca il testo della richiesta
                }).OrderBy(r => r.NumeroProgressivo).ToList() ?? new List<RichiestaIntegrazioneListItemViewModel>(),

                Partecipanti = lotto.Partecipanti?.Select(p => new PartecipanteLottoListItemViewModel
                {
                    Id = p.Id,
                    Nome = p.RagioneSociale,
                    IsNostraAzienda = false, // TODO: logica per determinare se è la nostra azienda
                    Posizione = null, // TODO: calcolare posizione in graduatoria
                    ImportoOfferto = p.OffertaEconomica
                }).OrderBy(p => p.ImportoOfferto).ToList() ?? new List<PartecipanteLottoListItemViewModel>(),

                Documenti = lotto.Documenti?.Select(d => new DocumentoGaraListItemViewModel
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

                // Checklist Documenti Richiesti
                ChecklistDocumenti = lotto.DocumentiRichiesti?.Select(dr =>
                {
                    // Cerca un documento caricato di questo tipo
                    var documentoCaricato = lotto.Documenti?
                        .FirstOrDefault(d => d.TipoDocumentoId == dr.TipoDocumentoId);

                    return new ChecklistDocumentoViewModel
                    {
                        TipoDocumentoId = dr.TipoDocumentoId,
                        TipoDocumentoNome = dr.TipoDocumento?.Nome ?? string.Empty,
                        TipoDocumentoCodiceRiferimento = dr.TipoDocumento?.CodiceRiferimento,
                        IsPresente = documentoCaricato != null,
                        DataCaricamento = documentoCaricato?.DataCaricamento,
                        DocumentoId = documentoCaricato?.Id,
                        NomeFile = documentoCaricato?.NomeFile
                    };
                }).OrderBy(c => c.TipoDocumentoNome).ToList() ?? new List<ChecklistDocumentoViewModel>(),

                // Checklist Documenti Richiesti a livello Gara
                ChecklistDocumentiGara = lotto.Gara?.DocumentiRichiesti?.Select(dr =>
                {
                    // Cerca un documento caricato di questo tipo a livello Gara (LottoId = null)
                    var documentoCaricato = lotto.Gara?.Documenti?
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
        }

        /// <summary>
        /// Mappa un'entità Lotto a LottoEditViewModel
        /// </summary>
        public static LottoEditViewModel ToEditViewModel(this Lotto lotto)
        {
            return new LottoEditViewModel
            {
                Id = lotto.Id,
                GaraId = lotto.GaraId,

                // Identificazione
                CodiceLotto = lotto.CodiceLotto,
                Descrizione = lotto.Descrizione,
                Tipologia = lotto.Tipologia,
                Stato = lotto.Stato,

                // Rifiuto
                MotivoRifiuto = lotto.MotivoRifiuto,

                // Info Generali
                LinkPiattaforma = lotto.LinkPiattaforma,
                OperatoreAssegnatoId = lotto.OperatoreAssegnatoId,
                GiorniFornitura = lotto.GiorniFornitura,

                // Info Economiche
                ImportoBaseAsta = lotto.ImportoBaseAsta,
                Quotazione = lotto.Quotazione,

                // Info Contratto
                DurataContratto = lotto.DurataContratto,
                DataStipulaContratto = lotto.DataStipulaContratto,
                DataScadenzaContratto = lotto.DataScadenzaContratto,
                Fatturazione = lotto.Fatturazione,

                // Info Partecipazione
                RichiedeFideiussione = lotto.RichiedeFideiussione,

                // Esame Ente
                DataInizioEsameEnte = lotto.DataInizioEsameEnte,

                // Checklist Documenti Richiesti
                DocumentiRichiestiIds = lotto.DocumentiRichiesti?
                    .Select(dr => dr.TipoDocumentoId)
                    .ToList() ?? new List<Guid>(),

                // Info Gara (read-only)
                CodiceGara = lotto.Gara?.CodiceGara,
                TitoloGara = lotto.Gara?.Titolo,
                EnteAppaltante = lotto.Gara?.EnteAppaltante,
                DataTerminePresentazioneOfferte = lotto.Gara?.DataTerminePresentazioneOfferte,

                // Statistiche (read-only)
                NumeroPreventivi = lotto.Preventivi?.Count ?? 0,
                NumeroRichiesteIntegrazione = lotto.RichiesteIntegrazione?.Count ?? 0,
                NumeroPartecipanti = lotto.Partecipanti?.Count ?? 0,
                NumeroDocumenti = lotto.Documenti?.Count ?? 0,

                // Audit
                CreatedAt = lotto.CreatedAt,
                CreatedBy = lotto.CreatedBy,
                ModifiedAt = lotto.ModifiedAt,
                ModifiedBy = lotto.ModifiedBy
            };
        }

        // ===================================
        // VIEWMODEL → ENTITY
        // ===================================

        /// <summary>
        /// Crea un'entità Lotto da LottoCreateViewModel
        /// Include normalizzazione automatica dei campi
        /// </summary>
        public static Lotto ToEntity(this LottoCreateViewModel viewModel)
        {
            return new Lotto
            {
                GaraId = viewModel.GaraId,

                // Identificazione
                CodiceLotto = NormalizzaStringa(viewModel.CodiceLotto)!,
                Descrizione = NormalizzaStringa(viewModel.Descrizione)!,
                Tipologia = viewModel.Tipologia,
                Stato = StatoLotto.Bozza, // Default per nuovi lotti

                // Info Generali
                LinkPiattaforma = NormalizzaStringa(viewModel.LinkPiattaforma),
                OperatoreAssegnatoId = NormalizzaStringa(viewModel.OperatoreAssegnatoId),
                GiorniFornitura = viewModel.GiorniFornitura,

                // Info Economiche
                ImportoBaseAsta = viewModel.ImportoBaseAsta,
                Quotazione = viewModel.Quotazione,

                // Info Contratto
                DurataContratto = NormalizzaStringa(viewModel.DurataContratto),
                DataStipulaContratto = viewModel.DataStipulaContratto,
                DataScadenzaContratto = viewModel.DataScadenzaContratto,
                Fatturazione = NormalizzaStringa(viewModel.Fatturazione),

                // Info Partecipazione
                RichiedeFideiussione = viewModel.RichiedeFideiussione

                // CreatedAt, CreatedBy, IsDeleted gestiti da AuditInterceptor
            };
        }

        /// <summary>
        /// Aggiorna un'entità Lotto esistente con i dati del LottoEditViewModel
        /// Include normalizzazione automatica dei campi
        /// </summary>
        public static void UpdateEntity(this LottoEditViewModel viewModel, Lotto lotto)
        {
            // Identificazione
            lotto.CodiceLotto = NormalizzaStringa(viewModel.CodiceLotto)!;
            lotto.Descrizione = NormalizzaStringa(viewModel.Descrizione)!;
            lotto.Tipologia = viewModel.Tipologia;
            lotto.Stato = viewModel.Stato;

            // Rifiuto
            lotto.MotivoRifiuto = NormalizzaStringa(viewModel.MotivoRifiuto);

            // Info Generali
            lotto.LinkPiattaforma = NormalizzaStringa(viewModel.LinkPiattaforma);
            lotto.OperatoreAssegnatoId = NormalizzaStringa(viewModel.OperatoreAssegnatoId);
            lotto.GiorniFornitura = viewModel.GiorniFornitura;

            // Info Economiche
            lotto.ImportoBaseAsta = viewModel.ImportoBaseAsta;
            lotto.Quotazione = viewModel.Quotazione;

            // Info Contratto
            lotto.DurataContratto = NormalizzaStringa(viewModel.DurataContratto);
            lotto.DataStipulaContratto = viewModel.DataStipulaContratto;
            lotto.DataScadenzaContratto = viewModel.DataScadenzaContratto;
            lotto.Fatturazione = NormalizzaStringa(viewModel.Fatturazione);

            // Info Partecipazione
            lotto.RichiedeFideiussione = viewModel.RichiedeFideiussione;

            // Esame Ente
            lotto.DataInizioEsameEnte = viewModel.DataInizioEsameEnte;

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

        /// <summary>
        /// Tronca un testo alla lunghezza specificata aggiungendo "..." se necessario
        /// </summary>
        private static string? TruncateText(string? text, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;

            if (text.Length <= maxLength)
                return text;

            return text.Substring(0, maxLength - 3) + "...";
        }
    }
}