using Trasformazioni.Models.Entities;
using Trasformazioni.Models.ViewModels;

namespace Trasformazioni.Extensions
{
    /// <summary>
    /// Extension methods per il mapping tra ValutazioneLotto entity e ViewModels
    /// Gestisce le conversioni bidirezionali e la normalizzazione dei dati
    /// </summary>
    public static class ValutazioneLottoMappingExtensions
    {
        // ===================================
        // ENTITY → VIEWMODEL
        // ===================================

        /// <summary>
        /// Converte ValutazioneLotto entity in ListViewModel per liste paginate
        /// </summary>
        public static ValutazioneLottoListViewModel ToListViewModel(this ValutazioneLotto valutazione)
        {
            return new ValutazioneLottoListViewModel
            {
                Id = valutazione.Id,
                LottoId = valutazione.LottoId,

                // Info Lotto
                CodiceLotto = valutazione.Lotto?.CodiceLotto ?? string.Empty,
                DescrizioneLotto = valutazione.Lotto?.Descrizione ?? string.Empty,
                CodiceGara = valutazione.Lotto?.Gara?.CodiceGara ?? string.Empty,

                // Valutazione Tecnica
                DataValutazioneTecnica = valutazione.DataValutazioneTecnica,
                ValutatoreTecnicoNome = valutazione.ValutatoreTecnico?.NomeCompleto,
                TecnicaApprovata = valutazione.TecnicaApprovata,

                // Valutazione Economica
                DataValutazioneEconomica = valutazione.DataValutazioneEconomica,
                ValutatoreEconomicoNome = valutazione.ValutatoreEconomico?.NomeCompleto,
                EconomicaApprovata = valutazione.EconomicaApprovata,

                // Audit
                CreatedAt = valutazione.CreatedAt,
                ModifiedAt = valutazione.ModifiedAt
            };
        }

        /// <summary>
        /// Converte ValutazioneLotto entity in DetailsViewModel per visualizzazione completa
        /// Include tutte le relazioni e le informazioni dettagliate
        /// </summary>
        public static ValutazioneLottoDetailsViewModel ToDetailsViewModel(this ValutazioneLotto valutazione)
        {
            return new ValutazioneLottoDetailsViewModel
            {
                Id = valutazione.Id,
                LottoId = valutazione.LottoId,

                // Info Lotto
                CodiceLotto = valutazione.Lotto?.CodiceLotto ?? string.Empty,
                DescrizioneLotto = valutazione.Lotto?.Descrizione ?? string.Empty,
                StatoLotto = valutazione.Lotto?.Stato.ToString() ?? string.Empty,
                TipologiaLotto = valutazione.Lotto?.Tipologia.ToString() ?? string.Empty,
                ImportoBaseAsta = valutazione.Lotto?.ImportoBaseAsta,

                // Info Gara
                GaraId = valutazione.Lotto?.GaraId ?? Guid.Empty,
                CodiceGara = valutazione.Lotto?.Gara?.CodiceGara ?? string.Empty,
                TitoloGara = valutazione.Lotto?.Gara?.Titolo ?? string.Empty,
                EnteAppaltante = valutazione.Lotto?.Gara?.EnteAppaltante,

                // Valutazione Tecnica
                DataValutazioneTecnica = valutazione.DataValutazioneTecnica,
                ValutatoreTecnicoId = valutazione.ValutatoreTecnicoId,
                ValutatoreTecnicoNome = valutazione.ValutatoreTecnico?.NomeCompleto,
                ValutatoreTecnicoEmail = valutazione.ValutatoreTecnico?.Email,
                TecnicaApprovata = valutazione.TecnicaApprovata,
                MotivoRifiutoTecnico = valutazione.MotivoRifiutoTecnico,
                NoteTecniche = valutazione.NoteTecniche,

                // Valutazione Economica
                DataValutazioneEconomica = valutazione.DataValutazioneEconomica,
                ValutatoreEconomicoId = valutazione.ValutatoreEconomicoId,
                ValutatoreEconomicoNome = valutazione.ValutatoreEconomico?.NomeCompleto,
                ValutatoreEconomicoEmail = valutazione.ValutatoreEconomico?.Email,
                EconomicaApprovata = valutazione.EconomicaApprovata,
                MotivoRifiutoEconomico = valutazione.MotivoRifiutoEconomico,
                NoteEconomiche = valutazione.NoteEconomiche,

                // Audit
                CreatedAt = valutazione.CreatedAt,
                CreatedBy = valutazione.CreatedBy,
                ModifiedAt = valutazione.ModifiedAt,
                ModifiedBy = valutazione.ModifiedBy
            };
        }

        // ===================================
        // VIEWMODEL → ENTITY (CREATE)
        // ===================================

        /// <summary>
        /// Crea una nuova entità ValutazioneLotto da ValutazioneTecnicaViewModel
        /// Utilizzato per creare la valutazione con solo la parte tecnica
        /// </summary>
        public static ValutazioneLotto ToEntity(this ValutazioneTecnicaViewModel viewModel)
        {
            return new ValutazioneLotto
            {
                LottoId = viewModel.LottoId,

                // Valutazione Tecnica
                DataValutazioneTecnica = DateTime.Now,
                ValutatoreTecnicoId = NormalizzaStringa(viewModel.ValutatoreTecnicoId),
                TecnicaApprovata = viewModel.TecnicaApprovata,
                MotivoRifiutoTecnico = NormalizzaStringa(viewModel.MotivoRifiutoTecnico),
                NoteTecniche = NormalizzaStringa(viewModel.NoteTecniche),

                // Valutazione Economica (null per ora)
                DataValutazioneEconomica = null,
                ValutatoreEconomicoId = null,
                EconomicaApprovata = null,
                MotivoRifiutoEconomico = null,
                NoteEconomiche = null

                // CreatedAt, CreatedBy, IsDeleted gestiti da AuditInterceptor
            };
        }

        // ===================================
        // VIEWMODEL → ENTITY (UPDATE)
        // ===================================

        /// <summary>
        /// Aggiorna un'entità ValutazioneLotto esistente con i dati di ValutazioneTecnicaViewModel
        /// Modifica solo i campi della valutazione tecnica
        /// </summary>
        public static void UpdateFromTecnicaViewModel(this ValutazioneTecnicaViewModel viewModel, ValutazioneLotto valutazione)
        {
            // Valutazione Tecnica
            valutazione.DataValutazioneTecnica = DateTime.Now;
            valutazione.ValutatoreTecnicoId = NormalizzaStringa(viewModel.ValutatoreTecnicoId);
            valutazione.TecnicaApprovata = viewModel.TecnicaApprovata;
            valutazione.MotivoRifiutoTecnico = NormalizzaStringa(viewModel.MotivoRifiutoTecnico);
            valutazione.NoteTecniche = NormalizzaStringa(viewModel.NoteTecniche);

            // ModifiedAt, ModifiedBy gestiti da AuditInterceptor
        }

        /// <summary>
        /// Aggiorna un'entità ValutazioneLotto esistente con i dati di ValutazioneEconomicaViewModel
        /// Modifica solo i campi della valutazione economica
        /// NON tocca i campi della valutazione tecnica
        /// </summary>
        public static void UpdateFromEconomicaViewModel(this ValutazioneEconomicaViewModel viewModel, ValutazioneLotto valutazione)
        {
            // Valutazione Economica
            valutazione.DataValutazioneEconomica = DateTime.Now;
            valutazione.ValutatoreEconomicoId = NormalizzaStringa(viewModel.ValutatoreEconomicoId);
            valutazione.EconomicaApprovata = viewModel.EconomicaApprovata;
            valutazione.MotivoRifiutoEconomico = NormalizzaStringa(viewModel.MotivoRifiutoEconomico);
            valutazione.NoteEconomiche = NormalizzaStringa(viewModel.NoteEconomiche);

            // ModifiedAt, ModifiedBy gestiti da AuditInterceptor
        }

        // ===================================
        // HELPER METHODS
        // ===================================

        /// <summary>
        /// Normalizza una stringa: trim, null se vuota
        /// </summary>
        private static string? NormalizzaStringa(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            var normalized = input.Trim();
            return string.IsNullOrEmpty(normalized) ? null : normalized;
        }

        /// <summary>
        /// Tronca un testo alla lunghezza massima specificata
        /// Aggiunge "..." se troncato
        /// </summary>
        private static string? TruncateText(string? text, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            text = text.Trim();

            if (text.Length <= maxLength)
                return text;

            return text.Substring(0, maxLength - 3) + "...";
        }
    }
}