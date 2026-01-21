using Trasformazioni.Models.Entities;
using Trasformazioni.Models.ViewModels;

namespace Trasformazioni.Models.Mappings
{
    /// <summary>
    /// Extension methods per il mapping tra ElaborazioneLotto e i suoi ViewModels
    /// Gestisce conversioni bidirezionali e calcoli derivati (scostamento, stato)
    /// </summary>
    public static class ElaborazioneLottoMappingExtensions
    {
        // ===================================
        // ENTITY → VIEWMODEL (QUERY)
        // ===================================

        /// <summary>
        /// Mappa un'entità ElaborazioneLotto a ElaborazioneLottoListViewModel
        /// Include calcolo scostamento e stato
        /// </summary>
        public static ElaborazioneLottoListViewModel ToListViewModel(this ElaborazioneLotto elaborazione)
        {
            return new ElaborazioneLottoListViewModel
            {
                Id = elaborazione.Id,
                LottoId = elaborazione.LottoId,

                // Info Lotto
                CodiceLotto = elaborazione.Lotto?.CodiceLotto ?? string.Empty,
                DescrizioneLotto = elaborazione.Lotto?.Descrizione ?? string.Empty,

                // Info Gara
                CodiceGara = elaborazione.Lotto?.Gara?.CodiceGara ?? string.Empty,
                EnteAppaltante = elaborazione.Lotto?.Gara?.EnteAppaltante,

                // Prezzi
                PrezzoDesiderato = elaborazione.PrezzoDesiderato,
                PrezzoRealeUscita = elaborazione.PrezzoRealeUscita,

                // Scostamento calcolato
                ScostamentoPercentuale = CalcolaScostamentoPercentuale(
                    elaborazione.PrezzoDesiderato,
                    elaborazione.PrezzoRealeUscita
                ),
                IsPrezzoRealeSuperiore = IsPrezzoRealeSuperiore(
                    elaborazione.PrezzoDesiderato,
                    elaborazione.PrezzoRealeUscita
                ),
                IsPrezzoRealeInferiore = IsPrezzoRealeInferiore(
                    elaborazione.PrezzoDesiderato,
                    elaborazione.PrezzoRealeUscita
                ),

                // Stato calcolato
                StatoElaborazione = CalcolaStatoElaborazione(
                    elaborazione.PrezzoDesiderato,
                    elaborazione.PrezzoRealeUscita
                ),

                // Motivazione
                HasMotivazione = !string.IsNullOrWhiteSpace(elaborazione.MotivazioneAdattamento),

                // Audit
                CreatedAt = elaborazione.CreatedAt,
                ModifiedAt = elaborazione.ModifiedAt
            };
        }

        /// <summary>
        /// Mappa un'entità ElaborazioneLotto a ElaborazioneLottoDetailsViewModel
        /// Include tutte le relazioni e proprietà computate
        /// </summary>
        public static ElaborazioneLottoDetailsViewModel ToDetailsViewModel(this ElaborazioneLotto elaborazione)
        {
            return new ElaborazioneLottoDetailsViewModel
            {
                Id = elaborazione.Id,
                LottoId = elaborazione.LottoId,

                // Prezzi
                PrezzoDesiderato = elaborazione.PrezzoDesiderato,
                PrezzoRealeUscita = elaborazione.PrezzoRealeUscita,

                // Scostamento calcolato
                ScostamentoPercentuale = CalcolaScostamentoPercentuale(
                    elaborazione.PrezzoDesiderato,
                    elaborazione.PrezzoRealeUscita
                ),
                DifferenzaAssoluta = CalcolaDifferenzaAssoluta(
                    elaborazione.PrezzoDesiderato,
                    elaborazione.PrezzoRealeUscita
                ),
                IsPrezzoRealeSuperiore = IsPrezzoRealeSuperiore(
                    elaborazione.PrezzoDesiderato,
                    elaborazione.PrezzoRealeUscita
                ),
                IsPrezzoRealeInferiore = IsPrezzoRealeInferiore(
                    elaborazione.PrezzoDesiderato,
                    elaborazione.PrezzoRealeUscita
                ),

                // Motivazione e Note
                MotivazioneAdattamento = elaborazione.MotivazioneAdattamento,
                Note = elaborazione.Note,

                // Stato calcolato
                StatoElaborazione = CalcolaStatoElaborazione(
                    elaborazione.PrezzoDesiderato,
                    elaborazione.PrezzoRealeUscita
                ),

                // Info Lotto
                CodiceLotto = elaborazione.Lotto?.CodiceLotto ?? string.Empty,
                DescrizioneLotto = elaborazione.Lotto?.Descrizione ?? string.Empty,
                TipologiaLotto = elaborazione.Lotto?.Tipologia,
                StatoLotto = elaborazione.Lotto?.Stato,
                ImportoBaseAstaLotto = elaborazione.Lotto?.ImportoBaseAsta,
                QuotazioneLotto = elaborazione.Lotto?.Quotazione,
                OperatoreAssegnatoNome = elaborazione.Lotto?.OperatoreAssegnato != null
                    ? $"{elaborazione.Lotto.OperatoreAssegnato.Nome} {elaborazione.Lotto.OperatoreAssegnato.Cognome}"
                    : null,

                // Info Gara
                CodiceGara = elaborazione.Lotto?.Gara?.CodiceGara ?? string.Empty,
                TitoloGara = elaborazione.Lotto?.Gara?.Titolo ?? string.Empty,
                TipologiaGara = elaborazione.Lotto?.Gara?.Tipologia,
                StatoGara = elaborazione.Lotto?.Gara?.Stato,
                EnteAppaltante = elaborazione.Lotto?.Gara?.EnteAppaltante,
                Regione = elaborazione.Lotto?.Gara?.Regione,
                DataTerminePresentazioneOfferte = elaborazione.Lotto?.Gara?.DataTerminePresentazioneOfferte,

                // Audit - Creazione
                CreatedAt = elaborazione.CreatedAt,
                CreatedBy = elaborazione.CreatedBy,

                // Audit - Modifica
                ModifiedAt = elaborazione.ModifiedAt,
                ModifiedBy = elaborazione.ModifiedBy
            };
        }

        // ===================================
        // VIEWMODEL → ENTITY (COMANDI)
        // ===================================

        /// <summary>
        /// Crea un'entità ElaborazioneLotto da ElaborazioneLottoCreateViewModel
        /// I campi audit vengono gestiti automaticamente dall'AuditInterceptor
        /// </summary>
        public static ElaborazioneLotto ToEntity(this ElaborazioneLottoCreateViewModel viewModel)
        {
            return new ElaborazioneLotto
            {
                LottoId = viewModel.LottoId,
                PrezzoDesiderato = viewModel.PrezzoDesiderato,
                PrezzoRealeUscita = viewModel.PrezzoRealeUscita,
                MotivazioneAdattamento = NormalizzaStringa(viewModel.MotivazioneAdattamento),
                Note = NormalizzaStringa(viewModel.Note)
            };
        }

        /// <summary>
        /// Aggiorna un'entità ElaborazioneLotto esistente con dati da ElaborazioneLottoEditViewModel
        /// I campi audit (ModifiedAt, ModifiedBy) vengono gestiti automaticamente dall'AuditInterceptor
        /// </summary>
        public static void UpdateEntity(this ElaborazioneLottoEditViewModel viewModel, ElaborazioneLotto entity)
        {
            // Aggiorna solo i campi modificabili
            entity.PrezzoDesiderato = viewModel.PrezzoDesiderato;
            entity.PrezzoRealeUscita = viewModel.PrezzoRealeUscita;
            entity.MotivazioneAdattamento = NormalizzaStringa(viewModel.MotivazioneAdattamento);
            entity.Note = NormalizzaStringa(viewModel.Note);

            // LottoId NON viene aggiornato (relazione immutabile)
            // I campi audit vengono gestiti dall'AuditInterceptor
        }

        /// <summary>
        /// Mappa un'entità ElaborazioneLotto a ElaborazioneLottoEditViewModel
        /// Usato per popolare la form di modifica
        /// </summary>
        public static ElaborazioneLottoEditViewModel ToEditViewModel(this ElaborazioneLotto elaborazione)
        {
            return new ElaborazioneLottoEditViewModel
            {
                Id = elaborazione.Id,
                LottoId = elaborazione.LottoId,
                PrezzoDesiderato = elaborazione.PrezzoDesiderato,
                PrezzoRealeUscita = elaborazione.PrezzoRealeUscita,
                MotivazioneAdattamento = elaborazione.MotivazioneAdattamento,
                Note = elaborazione.Note,

                // Info visualizzazione (readonly)
                CodiceLotto = elaborazione.Lotto?.CodiceLotto,
                DescrizioneLotto = elaborazione.Lotto?.Descrizione
            };
        }

        // ===================================
        // METODI HELPER PRIVATI
        // ===================================

        /// <summary>
        /// Calcola lo scostamento percentuale tra prezzo desiderato e reale
        /// Formula: |PrezzoReale - PrezzoDesiderato| / PrezzoDesiderato * 100
        /// </summary>
        private static decimal? CalcolaScostamentoPercentuale(decimal? prezzoDesiderato, decimal? prezzoReale)
        {
            if (!prezzoDesiderato.HasValue || !prezzoReale.HasValue || prezzoDesiderato.Value == 0)
                return null;

            var differenza = Math.Abs(prezzoReale.Value - prezzoDesiderato.Value);
            return Math.Round((differenza / prezzoDesiderato.Value) * 100, 2);
        }

        /// <summary>
        /// Calcola la differenza assoluta tra i prezzi (PrezzoReale - PrezzoDesiderato)
        /// </summary>
        private static decimal? CalcolaDifferenzaAssoluta(decimal? prezzoDesiderato, decimal? prezzoReale)
        {
            if (!prezzoDesiderato.HasValue || !prezzoReale.HasValue)
                return null;

            return prezzoReale.Value - prezzoDesiderato.Value;
        }

        /// <summary>
        /// Verifica se il prezzo reale è superiore al desiderato
        /// </summary>
        private static bool IsPrezzoRealeSuperiore(decimal? prezzoDesiderato, decimal? prezzoReale)
        {
            if (!prezzoDesiderato.HasValue || !prezzoReale.HasValue)
                return false;

            return prezzoReale.Value > prezzoDesiderato.Value;
        }

        /// <summary>
        /// Verifica se il prezzo reale è inferiore al desiderato
        /// </summary>
        private static bool IsPrezzoRealeInferiore(decimal? prezzoDesiderato, decimal? prezzoReale)
        {
            if (!prezzoDesiderato.HasValue || !prezzoReale.HasValue)
                return false;

            return prezzoReale.Value < prezzoDesiderato.Value;
        }

        /// <summary>
        /// Calcola lo stato dell'elaborazione in base ai prezzi valorizzati
        /// </summary>
        private static string CalcolaStatoElaborazione(decimal? prezzoDesiderato, decimal? prezzoReale)
        {
            if (!prezzoDesiderato.HasValue && !prezzoReale.HasValue)
                return "Da iniziare";

            if (prezzoDesiderato.HasValue && !prezzoReale.HasValue)
                return "Prezzo desiderato definito";

            if (!prezzoDesiderato.HasValue && prezzoReale.HasValue)
                return "Solo prezzo reale";

            return "Completata";
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