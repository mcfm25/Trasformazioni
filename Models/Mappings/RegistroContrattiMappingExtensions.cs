using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;
using Trasformazioni.Models.ViewModels;

namespace Trasformazioni.Mappings
{
    /// <summary>
    /// Extension methods per il mapping tra entità RegistroContratti e ViewModels
    /// </summary>
    public static class RegistroContrattiMappingExtensions
    {
        // ===================================
        // ENTITY → VIEWMODEL
        // ===================================

        /// <summary>
        /// Mappa un'entità RegistroContratti a RegistroContrattiListViewModel
        /// </summary>
        public static RegistroContrattiListViewModel ToListViewModel(this RegistroContratti registro)
        {
            return new RegistroContrattiListViewModel
            {
                Id = registro.Id,

                // Identificazione
                IdRiferimentoEsterno = registro.IdRiferimentoEsterno,
                NumeroProtocollo = registro.NumeroProtocollo,
                TipoRegistro = registro.TipoRegistro,

                // Cliente
                ClienteId = registro.ClienteId,
                RagioneSociale = registro.RagioneSociale ?? registro.Cliente?.NomeCompleto,

                // Contenuto
                Oggetto = registro.Oggetto,
                CategoriaNome = registro.CategoriaContratto?.Nome ?? string.Empty,

                // Responsabile
                ResponsabileInterno = registro.ResponsabileInterno ?? registro.Utente?.NomeCompleto,

                // Date
                DataDocumento = registro.DataDocumento,
                DataDecorrenza = registro.DataDecorrenza,
                DataFineOScadenza = registro.DataFineOScadenza,

                // Importi
                ImportoCanoneAnnuo = registro.ImportoCanoneAnnuo,
                ImportoUnatantum = registro.ImportoUnatantum,

                // Stato
                Stato = registro.Stato,

                // Gerarchia
                HasParent = registro.ParentId.HasValue,
                HasChildren = registro.Children?.Any() ?? false,
                NumeroAllegati = registro.Allegati?.Count(a => !a.IsDeleted) ?? 0,

                // Calcolate
                DataLimiteDisdetta = registro.DataLimiteDisdetta
            };
        }

        /// <summary>
        /// Mappa un'entità RegistroContratti a RegistroContrattiDetailsViewModel
        /// </summary>
        public static RegistroContrattiDetailsViewModel ToDetailsViewModel(this RegistroContratti registro)
        {
            return new RegistroContrattiDetailsViewModel
            {
                Id = registro.Id,

                // Identificazione
                IdRiferimentoEsterno = registro.IdRiferimentoEsterno,
                NumeroProtocollo = registro.NumeroProtocollo,
                TipoRegistro = registro.TipoRegistro,

                // Cliente
                ClienteId = registro.ClienteId,
                RagioneSociale = registro.RagioneSociale ?? registro.Cliente?.NomeCompleto,
                TipoControparte = registro.TipoControparte ?? registro.Cliente?.NaturaGiuridica,

                // Contenuto
                Oggetto = registro.Oggetto,
                CategoriaContrattoId = registro.CategoriaContrattoId,
                CategoriaNome = registro.CategoriaContratto?.Nome ?? string.Empty,

                // Responsabile
                UtenteId = registro.UtenteId,
                ResponsabileInterno = registro.ResponsabileInterno ?? registro.Utente?.NomeCompleto,

                // Date
                DataDocumento = registro.DataDocumento,
                DataDecorrenza = registro.DataDecorrenza,
                DataFineOScadenza = registro.DataFineOScadenza,
                DataInvio = registro.DataInvio,
                DataAccettazione = registro.DataAccettazione,

                // Scadenze e Rinnovi
                GiorniPreavvisoDisdetta = registro.GiorniPreavvisoDisdetta,
                GiorniAlertScadenza = registro.GiorniAlertScadenza,
                IsRinnovoAutomatico = registro.IsRinnovoAutomatico,
                GiorniRinnovoAutomatico = registro.GiorniRinnovoAutomatico,

                // Importi
                ImportoCanoneAnnuo = registro.ImportoCanoneAnnuo,
                ImportoUnatantum = registro.ImportoUnatantum,

                // Stato
                Stato = registro.Stato,

                // Gerarchia
                ParentId = registro.ParentId,
                ParentNumeroProtocollo = registro.Parent?.NumeroProtocollo,
                ParentOggetto = registro.Parent?.Oggetto,

                // Versioni (figli)
                Versioni = registro.Children?
                    .Where(c => !c.IsDeleted)
                    .OrderByDescending(c => c.DataDocumento)
                    .Select(c => new RegistroContrattiVersioneViewModel
                    {
                        Id = c.Id,
                        NumeroProtocollo = c.NumeroProtocollo,
                        Oggetto = c.Oggetto,
                        DataDocumento = c.DataDocumento,
                        Stato = c.Stato
                    })
                    .ToList() ?? new List<RegistroContrattiVersioneViewModel>(),

                // Allegati
                Allegati = registro.Allegati?
                    .Where(a => !a.IsDeleted)
                    .OrderByDescending(a => a.DataCaricamento)
                    .Select(a => a.ToListViewModel())
                    .ToList() ?? new List<AllegatoRegistroListViewModel>(),

                // Audit
                CreatedAt = registro.CreatedAt,
                CreatedBy = registro.CreatedBy,
                ModifiedAt = registro.ModifiedAt,
                ModifiedBy = registro.ModifiedBy
            };
        }

        /// <summary>
        /// Mappa un'entità RegistroContratti a RegistroContrattiEditViewModel
        /// </summary>
        public static RegistroContrattiEditViewModel ToEditViewModel(this RegistroContratti registro)
        {
            return new RegistroContrattiEditViewModel
            {
                Id = registro.Id,

                // Identificazione
                IdRiferimentoEsterno = registro.IdRiferimentoEsterno,
                NumeroProtocollo = registro.NumeroProtocollo,
                TipoRegistro = registro.TipoRegistro,

                // Cliente
                ClienteId = registro.ClienteId,
                RagioneSociale = registro.RagioneSociale,
                TipoControparte = registro.TipoControparte,

                // Contenuto
                Oggetto = registro.Oggetto,
                CategoriaContrattoId = registro.CategoriaContrattoId,

                // Responsabile
                UtenteId = registro.UtenteId,

                // Date
                DataDocumento = registro.DataDocumento,
                DataDecorrenza = registro.DataDecorrenza,
                DataFineOScadenza = registro.DataFineOScadenza,
                DataInvio = registro.DataInvio,
                DataAccettazione = registro.DataAccettazione,

                // Scadenze e Rinnovi
                GiorniPreavvisoDisdetta = registro.GiorniPreavvisoDisdetta,
                GiorniAlertScadenza = registro.GiorniAlertScadenza,
                IsRinnovoAutomatico = registro.IsRinnovoAutomatico,
                GiorniRinnovoAutomatico = registro.GiorniRinnovoAutomatico,

                // Importi
                ImportoCanoneAnnuo = registro.ImportoCanoneAnnuo,
                ImportoUnatantum = registro.ImportoUnatantum,

                // Stato
                Stato = registro.Stato,

                // Gerarchia
                ParentId = registro.ParentId,
                ParentNumeroProtocollo = registro.Parent?.NumeroProtocollo,
                ParentOggetto = registro.Parent?.Oggetto,

                // Info aggiuntive
                HasChildren = registro.Children?.Any(c => !c.IsDeleted) ?? false,
                NumeroAllegati = registro.Allegati?.Count(a => !a.IsDeleted) ?? 0
            };
        }

        // ===================================
        // VIEWMODEL → ENTITY
        // ===================================

        /// <summary>
        /// Crea un'entità RegistroContratti da RegistroContrattiCreateViewModel
        /// </summary>
        public static RegistroContratti ToEntity(this RegistroContrattiCreateViewModel viewModel)
        {
            return new RegistroContratti
            {
                Id = Guid.NewGuid(),

                // Identificazione
                IdRiferimentoEsterno = NormalizzaStringa(viewModel.IdRiferimentoEsterno),
                NumeroProtocollo = NormalizzaStringa(viewModel.NumeroProtocollo),
                TipoRegistro = viewModel.TipoRegistro,

                // Cliente
                ClienteId = viewModel.ClienteId,
                RagioneSociale = NormalizzaStringa(viewModel.RagioneSociale),
                TipoControparte = viewModel.TipoControparte,

                // Contenuto
                Oggetto = NormalizzaStringa(viewModel.Oggetto)!,
                CategoriaContrattoId = viewModel.CategoriaContrattoId,

                // Responsabile
                UtenteId = viewModel.UtenteId,

                // Date
                DataDocumento = viewModel.DataDocumento,
                DataDecorrenza = viewModel.DataDecorrenza,
                DataFineOScadenza = viewModel.DataFineOScadenza,

                // Scadenze e Rinnovi
                GiorniPreavvisoDisdetta = viewModel.GiorniPreavvisoDisdetta,
                GiorniAlertScadenza = viewModel.GiorniAlertScadenza,
                IsRinnovoAutomatico = viewModel.IsRinnovoAutomatico,
                GiorniRinnovoAutomatico = viewModel.GiorniRinnovoAutomatico,

                // Importi
                ImportoCanoneAnnuo = viewModel.ImportoCanoneAnnuo,
                ImportoUnatantum = viewModel.ImportoUnatantum,

                // Stato - sempre Bozza per nuovi record
                Stato = StatoRegistro.Bozza,

                // Gerarchia
                ParentId = viewModel.ParentId

                // CreatedAt, CreatedBy, IsDeleted gestiti da AuditInterceptor
            };
        }

        /// <summary>
        /// Aggiorna un'entità RegistroContratti con i dati del RegistroContrattiEditViewModel
        /// </summary>
        public static void UpdateEntity(
            this RegistroContrattiEditViewModel viewModel,
            RegistroContratti registro)
        {
            // Identificazione
            registro.IdRiferimentoEsterno = NormalizzaStringa(viewModel.IdRiferimentoEsterno);
            registro.NumeroProtocollo = NormalizzaStringa(viewModel.NumeroProtocollo);
            registro.TipoRegistro = viewModel.TipoRegistro;

            // Cliente
            registro.ClienteId = viewModel.ClienteId;
            registro.RagioneSociale = NormalizzaStringa(viewModel.RagioneSociale);
            registro.TipoControparte = viewModel.TipoControparte;

            // Contenuto
            registro.Oggetto = NormalizzaStringa(viewModel.Oggetto)!;
            registro.CategoriaContrattoId = viewModel.CategoriaContrattoId;

            // Responsabile
            registro.UtenteId = viewModel.UtenteId;

            // Date
            registro.DataDocumento = viewModel.DataDocumento;
            registro.DataDecorrenza = viewModel.DataDecorrenza;
            registro.DataFineOScadenza = viewModel.DataFineOScadenza;
            //registro.DataInvio = viewModel.DataInvio; // lasciamo invariato il primo dato salvato
            //registro.DataAccettazione = viewModel.DataAccettazione;

            // Scadenze e Rinnovi
            registro.GiorniPreavvisoDisdetta = viewModel.GiorniPreavvisoDisdetta;
            registro.GiorniAlertScadenza = viewModel.GiorniAlertScadenza;
            registro.IsRinnovoAutomatico = viewModel.IsRinnovoAutomatico;
            registro.GiorniRinnovoAutomatico = viewModel.GiorniRinnovoAutomatico;

            // Importi
            registro.ImportoCanoneAnnuo = viewModel.ImportoCanoneAnnuo;
            registro.ImportoUnatantum = viewModel.ImportoUnatantum;

            // Stato
            registro.Stato = viewModel.Stato;

            // ModifiedAt, ModifiedBy gestiti da AuditInterceptor
        }

        // ===================================
        // OPERAZIONI SPECIALI
        // ===================================

        /// <summary>
        /// Crea un nuovo RegistroContratti come rinnovo di uno esistente
        /// </summary>
        public static RegistroContratti ToRinnovoEntity(
            this RegistroContratti registroOriginale,
            int? giorniRinnovo = null)
        {
            var durata = giorniRinnovo ?? registroOriginale.GiorniRinnovoAutomatico ?? 365;

            return new RegistroContratti
            {
                Id = Guid.NewGuid(),

                // Identificazione - nuovo protocollo da assegnare
                IdRiferimentoEsterno = registroOriginale.IdRiferimentoEsterno,
                NumeroProtocollo = null, // Da assegnare
                TipoRegistro = registroOriginale.TipoRegistro,

                // Cliente - copiato
                ClienteId = registroOriginale.ClienteId,
                RagioneSociale = registroOriginale.RagioneSociale,
                TipoControparte = registroOriginale.TipoControparte,

                // Contenuto - copiato
                Oggetto = registroOriginale.Oggetto,
                CategoriaContrattoId = registroOriginale.CategoriaContrattoId,

                // Responsabile - copiato
                UtenteId = registroOriginale.UtenteId,
                ResponsabileInterno = registroOriginale.ResponsabileInterno,

                // Date - ricalcolate
                DataDocumento = DateTime.Now.Date,
                DataDecorrenza = registroOriginale.DataFineOScadenza?.AddDays(1) ?? DateTime.Now.Date,
                DataFineOScadenza = (registroOriginale.DataFineOScadenza?.AddDays(1) ?? DateTime.Now.Date).AddDays(durata),

                // Scadenze e Rinnovi - copiati
                GiorniPreavvisoDisdetta = registroOriginale.GiorniPreavvisoDisdetta,
                GiorniAlertScadenza = registroOriginale.GiorniAlertScadenza,
                IsRinnovoAutomatico = registroOriginale.IsRinnovoAutomatico,
                GiorniRinnovoAutomatico = registroOriginale.GiorniRinnovoAutomatico,

                // Importi - copiati
                ImportoCanoneAnnuo = registroOriginale.ImportoCanoneAnnuo,
                ImportoUnatantum = null, // Una tantum non si rinnova

                // Stato - Bozza
                Stato = StatoRegistro.Bozza,

                // Gerarchia - collegato al padre
                ParentId = registroOriginale.Id
            };
        }

        /// <summary>
        /// Popola i campi derivati dal Cliente se presente
        /// </summary>
        public static void PopolaDaCliente(
            this RegistroContratti registro,
            Soggetto? cliente)
        {
            if (cliente == null)
                return;

            registro.RagioneSociale = cliente.NomeCompleto;
            registro.TipoControparte = cliente.NaturaGiuridica;
        }

        /// <summary>
        /// Popola il campo ResponsabileInterno dall'utente se presente
        /// </summary>
        public static void PopolaDaUtente(
            this RegistroContratti registro,
            ApplicationUser? utente)
        {
            if (utente == null)
                return;

            registro.ResponsabileInterno = utente.NomeCompleto;
        }

        // ===================================
        // COLLECTION MAPPING
        // ===================================

        /// <summary>
        /// Mappa una collezione di RegistroContratti a lista di RegistroContrattiListViewModel
        /// </summary>
        public static IEnumerable<RegistroContrattiListViewModel> ToListViewModels(
            this IEnumerable<RegistroContratti> registri)
        {
            return registri.Select(r => r.ToListViewModel());
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