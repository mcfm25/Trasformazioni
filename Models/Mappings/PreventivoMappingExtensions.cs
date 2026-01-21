using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;
using Trasformazioni.Models.ViewModels;

namespace Trasformazioni.Mappings
{
    /// <summary>
    /// Extension methods per il mapping tra entità Preventivo e ViewModels
    /// </summary>
    public static class PreventivoMappingExtensions
    {
        // ===================================
        // ENTITY → VIEWMODEL
        // ===================================

        /// <summary>
        /// Mappa un'entità Preventivo a PreventivoListViewModel
        /// </summary>
        public static PreventivoListViewModel ToListViewModel(this Preventivo preventivo)
        {
            return new PreventivoListViewModel
            {
                Id = preventivo.Id,
                LottoId = preventivo.LottoId,
                SoggettoId = preventivo.SoggettoId,

                // Identificazione
                Descrizione = preventivo.Descrizione,
                Stato = preventivo.Stato,

                // Info Lotto
                CodiceLotto = preventivo.Lotto?.CodiceLotto ?? string.Empty,
                DescrizioneLotto = preventivo.Lotto?.Descrizione ?? string.Empty,
                CodiceGara = preventivo.Lotto?.Gara?.CodiceGara ?? string.Empty,

                // Info Fornitore
                //NomeFornitore = GetNomeFornitore(preventivo.Soggetto),
                NomeFornitore = preventivo.Soggetto.NomeCompleto,
                PartitaIVAFornitore = preventivo.Soggetto?.PartitaIVA,

                // Info Economiche
                ImportoOfferto = preventivo.ImportoOfferto,

                // Date
                DataRichiesta = preventivo.DataRichiesta,
                DataRicezione = preventivo.DataRicezione,
                DataScadenza = preventivo.DataScadenza,

                // Documento
                NomeFile = preventivo.NomeFile,
                DocumentPath = preventivo.DocumentPath,

                // Selezione
                IsSelezionato = preventivo.IsSelezionato,

                // Auto-rinnovo
                GiorniAutoRinnovo = preventivo.GiorniAutoRinnovo,

                // Audit
                CreatedAt = preventivo.CreatedAt,
                ModifiedAt = preventivo.ModifiedAt
            };
        }

        /// <summary>
        /// Mappa un'entità Preventivo a PreventivoDetailsViewModel
        /// Include tutte le relazioni e proprietà
        /// </summary>
        public static PreventivoDetailsViewModel ToDetailsViewModel(this Preventivo preventivo)
        {
            return new PreventivoDetailsViewModel
            {
                Id = preventivo.Id,
                LottoId = preventivo.LottoId,
                SoggettoId = preventivo.SoggettoId,

                // Identificazione
                Descrizione = preventivo.Descrizione,
                Stato = preventivo.Stato,

                // Info Lotto
                CodiceLotto = preventivo.Lotto?.CodiceLotto ?? string.Empty,
                DescrizioneLotto = preventivo.Lotto?.Descrizione ?? string.Empty,
                TipologiaLotto = preventivo.Lotto?.Tipologia ?? TipologiaLotto.Altro,
                StatoLotto = preventivo.Lotto?.Stato ?? StatoLotto.Bozza,
                CodiceGara = preventivo.Lotto?.Gara?.CodiceGara ?? string.Empty,
                TitoloGara = preventivo.Lotto?.Gara?.Titolo ?? string.Empty,
                EnteAppaltante = preventivo.Lotto?.Gara?.EnteAppaltante,
                ImportoBaseAstaLotto = preventivo.Lotto?.ImportoBaseAsta,

                // Info Fornitore
                //NomeFornitore = GetNomeFornitore(preventivo.Soggetto),
                NomeFornitore = preventivo.Soggetto.NomeCompleto,
                TipoSoggetto = preventivo.Soggetto?.TipoSoggetto ?? TipoSoggetto.Azienda,
                NaturaGiuridica = preventivo.Soggetto?.NaturaGiuridica,
                PartitaIVAFornitore = preventivo.Soggetto?.PartitaIVA,
                CodiceFiscaleFornitore = preventivo.Soggetto?.CodiceFiscale,
                EmailFornitore = preventivo.Soggetto?.Email,
                TelefonoFornitore = preventivo.Soggetto?.Telefono,
                //IndirizzoFornitore = GetIndirizzoCompleto(preventivo.Soggetto),
                IndirizzoFornitore = preventivo.Soggetto?.IndirizzoCompleto,

                // Info Economiche
                ImportoOfferto = preventivo.ImportoOfferto,

                // Date
                DataRichiesta = preventivo.DataRichiesta,
                DataRicezione = preventivo.DataRicezione,
                DataScadenza = preventivo.DataScadenza,

                // Auto-rinnovo
                GiorniAutoRinnovo = preventivo.GiorniAutoRinnovo,

                // Documento
                DocumentPath = preventivo.DocumentPath,
                NomeFile = preventivo.NomeFile,

                // Selezione
                IsSelezionato = preventivo.IsSelezionato,

                // Note
                Note = preventivo.Note,

                // Audit
                CreatedAt = preventivo.CreatedAt,
                CreatedBy = preventivo.CreatedBy,
                ModifiedAt = preventivo.ModifiedAt,
                ModifiedBy = preventivo.ModifiedBy
            };
        }

        // ===================================
        // VIEWMODEL → ENTITY
        // ===================================

        /// <summary>
        /// Crea un'entità Preventivo da PreventivoCreateViewModel
        /// Include normalizzazione automatica dei campi
        /// </summary>
        public static Preventivo ToEntity(this PreventivoCreateViewModel viewModel, string? documentPath = null)
        {
            return new Preventivo
            {
                LottoId = viewModel.LottoId,
                SoggettoId = viewModel.SoggettoId,

                // Identificazione
                Descrizione = NormalizzaStringa(viewModel.Descrizione)!,
                Stato = StatoPreventivo.InAttesa, // Default per nuovi preventivi

                // Info Economiche
                ImportoOfferto = viewModel.ImportoOfferto,

                // Date
                DataRichiesta = viewModel.DataRichiesta.Date,
                DataRicezione = viewModel.DataRicezione?.Date,
                DataScadenza = viewModel.DataScadenza.Date,

                // Auto-rinnovo
                GiorniAutoRinnovo = viewModel.GiorniAutoRinnovo,

                // Documento
                DocumentPath = NormalizzaStringa(documentPath) ?? string.Empty,
                NomeFile = !string.IsNullOrWhiteSpace(documentPath)
                    ? System.IO.Path.GetFileName(documentPath)
                    : string.Empty,

                // Selezione
                IsSelezionato = false, // Default per nuovi preventivi

                // Note
                Note = NormalizzaStringa(viewModel.Note)

                // CreatedAt, CreatedBy, IsDeleted gestiti da AuditInterceptor
            };
        }

        /// <summary>
        /// Aggiorna un'entità Preventivo esistente con i dati del PreventivoEditViewModel
        /// Include normalizzazione automatica dei campi
        /// </summary>
        public static void UpdateEntity(this PreventivoEditViewModel viewModel, Preventivo preventivo, string? newDocumentPath = null)
        {
            // Identificazione
            preventivo.Descrizione = NormalizzaStringa(viewModel.Descrizione)!;
            preventivo.Stato = viewModel.Stato;

            // Info Economiche
            preventivo.ImportoOfferto = viewModel.ImportoOfferto;

            // Date
            preventivo.DataRichiesta = viewModel.DataRichiesta.Date;
            preventivo.DataRicezione = viewModel.DataRicezione?.Date;
            preventivo.DataScadenza = viewModel.DataScadenza.Date;

            // Auto-rinnovo
            preventivo.GiorniAutoRinnovo = viewModel.GiorniAutoRinnovo;


            if (!viewModel.MantieniFIleEsistente)
            {
                preventivo.DocumentPath = string.Empty;
                preventivo.NomeFile = string.Empty;
            }

            // Documento (aggiorna solo se c'è un nuovo file)
            if (!string.IsNullOrWhiteSpace(newDocumentPath))
            {
                preventivo.DocumentPath = newDocumentPath;
                preventivo.NomeFile = Path.GetFileName(newDocumentPath);
            }

            // Selezione
            preventivo.IsSelezionato = viewModel.IsSelezionato;

            // Note
            preventivo.Note = NormalizzaStringa(viewModel.Note);

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

        ///// <summary>
        ///// Ottiene il nome del fornitore (Denominazione per aziende, Nome Cognome per persone fisiche)
        ///// </summary>
        //private static string GetNomeFornitore(Soggetto? soggetto)
        //{
        //    if (soggetto == null)
        //        return "N/D";

        //    if (soggetto.TipoSoggetto == TipoSoggetto.Azienda)
        //    {
        //        return soggetto.Denominazione ?? "N/D";
        //    }
        //    else // PersonaFisica
        //    {
        //        var nome = soggetto.Nome?.Trim() ?? string.Empty;
        //        var cognome = soggetto.Cognome?.Trim() ?? string.Empty;

        //        if (string.IsNullOrEmpty(nome) && string.IsNullOrEmpty(cognome))
        //            return "N/D";

        //        return $"{nome} {cognome}".Trim();
        //    }
        //}

        ///// <summary>
        ///// Costruisce l'indirizzo completo del fornitore
        ///// </summary>
        //private static string? GetIndirizzoCompleto(Soggetto? soggetto)
        //{
        //    if (soggetto == null)
        //        return null;

        //    var parti = new List<string>();

        //    if (!string.IsNullOrWhiteSpace(soggetto.Indirizzo))
        //        parti.Add(soggetto.Indirizzo.Trim());

        //    if (!string.IsNullOrWhiteSpace(soggetto.Citta))
        //        parti.Add(soggetto.Citta.Trim());

        //    if (!string.IsNullOrWhiteSpace(soggetto.Provincia))
        //        parti.Add(soggetto.Provincia.Trim());

        //    if (!string.IsNullOrWhiteSpace(soggetto.CAP))
        //        parti.Add(soggetto.CAP.Trim());

        //    return parti.Count > 0 ? string.Join(", ", parti) : null;
        //}
    }
}