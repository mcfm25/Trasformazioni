using Trasformazioni.Models.Enums;

namespace Trasformazioni.Models.ViewModels
{
    /// <summary>
    /// ViewModel per la visualizzazione del dettaglio di un registro contratti
    /// </summary>
    public class RegistroContrattiDetailsViewModel
    {
        /// <summary>
        /// Identificatore univoco
        /// </summary>
        public Guid Id { get; set; }

        // ===================================
        // IDENTIFICAZIONE
        // ===================================

        /// <summary>
        /// Riferimento esterno
        /// </summary>
        public string? IdRiferimentoEsterno { get; set; }

        /// <summary>
        /// Numero protocollo interno
        /// </summary>
        public string? NumeroProtocollo { get; set; }

        /// <summary>
        /// Tipo documento (Preventivo o Contratto)
        /// </summary>
        public TipoRegistro TipoRegistro { get; set; }

        /// <summary>
        /// Descrizione del tipo per display
        /// </summary>
        public string TipoRegistroDescrizione => TipoRegistro == TipoRegistro.Preventivo
            ? "Preventivo"
            : "Contratto";

        // ===================================
        // CLIENTE
        // ===================================

        /// <summary>
        /// ID del cliente
        /// </summary>
        public Guid? ClienteId { get; set; }

        /// <summary>
        /// Ragione sociale del cliente
        /// </summary>
        public string? RagioneSociale { get; set; }

        /// <summary>
        /// Tipo controparte (natura giuridica)
        /// </summary>
        public NaturaGiuridica? TipoControparte { get; set; }

        /// <summary>
        /// Descrizione tipo controparte
        /// </summary>
        public string? TipoControparteDescrizione => TipoControparte switch
        {
            NaturaGiuridica.PA => "Pubblica Amministrazione",
            NaturaGiuridica.Privato => "Privato",
            _ => null
        };

        // ===================================
        // CONTENUTO
        // ===================================

        /// <summary>
        /// Oggetto del contratto/preventivo
        /// </summary>
        public string Oggetto { get; set; } = string.Empty;

        /// <summary>
        /// ID della categoria
        /// </summary>
        public Guid CategoriaContrattoId { get; set; }

        /// <summary>
        /// Nome della categoria
        /// </summary>
        public string CategoriaNome { get; set; } = string.Empty;

        // ===================================
        // RESPONSABILE INTERNO
        // ===================================

        /// <summary>
        /// ID dell'utente responsabile
        /// </summary>
        public string? UtenteId { get; set; }

        /// <summary>
        /// Nome del responsabile interno
        /// </summary>
        public string? ResponsabileInterno { get; set; }

        // ===================================
        // DATE
        // ===================================

        /// <summary>
        /// Data del documento
        /// </summary>
        public DateTime DataDocumento { get; set; }

        /// <summary>
        /// Data di decorrenza
        /// </summary>
        public DateTime? DataDecorrenza { get; set; }

        /// <summary>
        /// Data di fine o scadenza
        /// </summary>
        public DateTime? DataFineOScadenza { get; set; }

        /// <summary>
        /// Data di invio al cliente
        /// </summary>
        public DateTime? DataInvio { get; set; }

        /// <summary>
        /// Data di accettazione
        /// </summary>
        public DateTime? DataAccettazione { get; set; }

        // ===================================
        // SCADENZE E RINNOVI
        // ===================================

        /// <summary>
        /// Giorni di preavviso per disdetta
        /// </summary>
        public int? GiorniPreavvisoDisdetta { get; set; }

        /// <summary>
        /// Giorni di anticipo per alert scadenza
        /// </summary>
        public int GiorniAlertScadenza { get; set; }

        /// <summary>
        /// Indica se il contratto si rinnova automaticamente
        /// </summary>
        public bool IsRinnovoAutomatico { get; set; }

        /// <summary>
        /// Durata del rinnovo automatico in giorni
        /// </summary>
        public int? GiorniRinnovoAutomatico { get; set; }

        // ===================================
        // IMPORTI
        // ===================================

        /// <summary>
        /// Importo canone annuo
        /// </summary>
        public decimal? ImportoCanoneAnnuo { get; set; }

        /// <summary>
        /// Importo una tantum
        /// </summary>
        public decimal? ImportoUnatantum { get; set; }

        /// <summary>
        /// Importo totale
        /// </summary>
        public decimal ImportoTotale => (ImportoCanoneAnnuo ?? 0) + (ImportoUnatantum ?? 0);

        // ===================================
        // STATO
        // ===================================

        /// <summary>
        /// Stato corrente
        /// </summary>
        public StatoRegistro Stato { get; set; }

        // ===================================
        // GERARCHIA / VERSIONAMENTO
        // ===================================

        /// <summary>
        /// ID del documento padre
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Numero protocollo del documento padre
        /// </summary>
        public string? ParentNumeroProtocollo { get; set; }

        /// <summary>
        /// Oggetto del documento padre
        /// </summary>
        public string? ParentOggetto { get; set; }

        /// <summary>
        /// Lista delle versioni figlie
        /// </summary>
        public List<RegistroContrattiVersioneViewModel> Versioni { get; set; } = new();

        /// <summary>
        /// Indica se ha un documento padre
        /// </summary>
        public bool HasParent => ParentId.HasValue;

        /// <summary>
        /// Indica se ha documenti figli
        /// </summary>
        public bool HasChildren => Versioni.Any();

        // ===================================
        // ALLEGATI
        // ===================================

        /// <summary>
        /// Lista degli allegati
        /// </summary>
        public List<AllegatoRegistroListViewModel> Allegati { get; set; } = new();

        /// <summary>
        /// Numero di allegati
        /// </summary>
        public int NumeroAllegati => Allegati.Count;

        // ===================================
        // AUDIT
        // ===================================

        /// <summary>
        /// Data di creazione
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Utente che ha creato il record
        /// </summary>
        public string? CreatedBy { get; set; }

        /// <summary>
        /// Data ultima modifica
        /// </summary>
        public DateTime? ModifiedAt { get; set; }

        /// <summary>
        /// Utente che ha modificato il record
        /// </summary>
        public string? ModifiedBy { get; set; }

        // ===================================
        // PROPRIETÀ CALCOLATE
        // ===================================

        /// <summary>
        /// Data limite disdetta
        /// </summary>
        public DateTime? DataLimiteDisdetta
        {
            get
            {
                if (DataFineOScadenza.HasValue && GiorniPreavvisoDisdetta.HasValue)
                    return DataFineOScadenza.Value.AddDays(-GiorniPreavvisoDisdetta.Value);
                return null;
            }
        }

        /// <summary>
        /// Data alert scadenza
        /// </summary>
        public DateTime? DataAlertScadenza
        {
            get
            {
                if (DataLimiteDisdetta.HasValue)
                    return DataLimiteDisdetta.Value.AddDays(-GiorniAlertScadenza);
                return null;
            }
        }

        /// <summary>
        /// Giorni rimanenti alla scadenza
        /// </summary>
        public int? GiorniAllaScadenza
        {
            get
            {
                if (!DataFineOScadenza.HasValue)
                    return null;

                var diff = (DataFineOScadenza.Value.Date - DateTime.Now.Date).Days;
                return diff;
            }
        }

        /// <summary>
        /// Giorni rimanenti alla disdetta
        /// </summary>
        public int? GiorniAllaDisdetta
        {
            get
            {
                if (!DataLimiteDisdetta.HasValue)
                    return null;

                var diff = (DataLimiteDisdetta.Value.Date - DateTime.Now.Date).Days;
                return diff;
            }
        }

        /// <summary>
        /// Indica se è in scadenza imminente
        /// </summary>
        public bool IsScadenzaImminente => GiorniAllaScadenza.HasValue &&
                                            GiorniAllaScadenza.Value >= 0 &&
                                            GiorniAllaScadenza.Value <= 30;

        /// <summary>
        /// Indica se è scaduto
        /// </summary>
        public bool IsScaduto => GiorniAllaScadenza.HasValue && GiorniAllaScadenza.Value < 0;

        /// <summary>
        /// Indica se può essere modificato
        /// </summary>
        public bool CanEdit => Stato != StatoRegistro.Annullato && Stato != StatoRegistro.Rinnovato;

        /// <summary>
        /// Indica se può essere eliminato
        /// </summary>
        public bool CanDelete => Stato == StatoRegistro.Bozza && !HasChildren;

        /// <summary>
        /// Indica se può essere inviato
        /// </summary>
        public bool CanInviare => Stato == StatoRegistro.Bozza || Stato == StatoRegistro.InRevisione;

        /// <summary>
        /// Indica se può essere attivato
        /// </summary>
        public bool CanAttivare => Stato == StatoRegistro.Inviato;

        /// <summary>
        /// Indica se può essere rinnovato
        /// </summary>
        public bool CanRinnovare => Stato == StatoRegistro.Attivo ||
                                     Stato == StatoRegistro.InScadenza ||
                                     Stato == StatoRegistro.InScadenzaPropostoRinnovo;

        /// <summary>
        /// Indica se può essere annullato
        /// </summary>
        public bool CanAnnullare => Stato != StatoRegistro.Annullato &&
                                     Stato != StatoRegistro.Rinnovato &&
                                     Stato != StatoRegistro.Scaduto;

        /// <summary>
        /// Indica se può essere sospeso
        /// </summary>
        public bool CanSospendere => Stato == StatoRegistro.Attivo;

        /// <summary>
        /// Indica se può essere riattivato
        /// </summary>
        public bool CanRiattivare => Stato == StatoRegistro.Sospeso;

        /// <summary>
        /// Indica se può essere mandato in revisione (da Bozza)
        /// </summary>
        public bool CanMandareInRevisione => Stato == StatoRegistro.Bozza;

        /// <summary>
        /// Indica se può tornare in bozza (da InRevisione)
        /// </summary>
        public bool CanTornareInBozza => Stato == StatoRegistro.InRevisione;

        /// <summary>
        /// Indica se lo stato può essere cambiato
        /// </summary>
        public bool CanChangeStato => Stato != StatoRegistro.Rinnovato &&
                                       Stato != StatoRegistro.Annullato;

        /// <summary>
        /// Lista degli stati selezionabili in base allo stato corrente
        /// </summary>
        public IEnumerable<StatoRegistro> StatiDisponibili
        {
            get
            {
                return Stato switch
                {
                    StatoRegistro.Bozza => new[]
                    {
                        StatoRegistro.Bozza,
                        StatoRegistro.InRevisione,
                        StatoRegistro.Annullato
                    },
                    StatoRegistro.InRevisione => new[]
                    {
                        StatoRegistro.InRevisione,
                        StatoRegistro.Bozza,
                        StatoRegistro.Inviato,
                        StatoRegistro.Annullato
                    },
                    StatoRegistro.Inviato => new[]
                    {
                        StatoRegistro.Inviato,
                        StatoRegistro.InRevisione,
                        StatoRegistro.Attivo,
                        StatoRegistro.Annullato
                    },
                    StatoRegistro.Attivo => new[]
                    {
                        StatoRegistro.Attivo,
                        StatoRegistro.InScadenza,
                        StatoRegistro.Sospeso,
                        StatoRegistro.Annullato
                    },
                    StatoRegistro.InScadenza => new[]
                    {
                        StatoRegistro.InScadenza,
                        StatoRegistro.InScadenzaPropostoRinnovo,
                        StatoRegistro.Attivo,
                        StatoRegistro.Scaduto,
                        StatoRegistro.Annullato
                    },
                    StatoRegistro.InScadenzaPropostoRinnovo => new[]
                    {
                        StatoRegistro.InScadenzaPropostoRinnovo,
                        StatoRegistro.InScadenza,
                        StatoRegistro.Rinnovato,
                        StatoRegistro.Scaduto,
                        StatoRegistro.Annullato
                    },
                    StatoRegistro.Scaduto => new[]
                    {
                        StatoRegistro.Scaduto,
                        StatoRegistro.Rinnovato
                    },
                    StatoRegistro.Sospeso => new[]
                    {
                        StatoRegistro.Sospeso,
                        StatoRegistro.Attivo,
                        StatoRegistro.Annullato
                    },
                    StatoRegistro.Rinnovato => new[]
                    {
                        StatoRegistro.Rinnovato
                    },
                    StatoRegistro.Annullato => new[]
                    {
                        StatoRegistro.Annullato
                    },
                    _ => new[] { Stato }
                };
            }
        }

        /// <summary>
        /// Descrizione testuale dello stato per la dropdown
        /// </summary>
        public static string GetStatoDescrizione(StatoRegistro stato) => stato switch
        {
            StatoRegistro.Bozza => "In bozza",
            StatoRegistro.InRevisione => "In revisione",
            StatoRegistro.Inviato => "Inviato",
            StatoRegistro.Attivo => "Attivo",
            StatoRegistro.InScadenza => "In scadenza",
            StatoRegistro.InScadenzaPropostoRinnovo => "In scadenza - Proposto rinnovo",
            StatoRegistro.Scaduto => "Scaduto",
            StatoRegistro.Rinnovato => "Rinnovato",
            StatoRegistro.Annullato => "Annullato",
            StatoRegistro.Sospeso => "Sospeso",
            _ => "Sconosciuto"
        };

        /// <summary>
        /// Badge CSS per il tipo
        /// </summary>
        public string TipoBadgeClass => TipoRegistro == TipoRegistro.Preventivo
            ? "badge bg-info"
            : "badge bg-primary";

        /// <summary>
        /// Badge CSS per lo stato
        /// </summary>
        public string StatoBadgeClass => Stato switch
        {
            StatoRegistro.Bozza => "badge bg-secondary",
            StatoRegistro.InRevisione => "badge bg-warning text-dark",
            StatoRegistro.Inviato => "badge bg-info",
            StatoRegistro.Attivo => "badge bg-success",
            StatoRegistro.InScadenza => "badge bg-warning text-dark",
            StatoRegistro.InScadenzaPropostoRinnovo => "badge bg-warning text-dark",
            StatoRegistro.Scaduto => "badge bg-danger",
            StatoRegistro.Rinnovato => "badge bg-primary",
            StatoRegistro.Annullato => "badge bg-dark",
            StatoRegistro.Sospeso => "badge bg-secondary",
            _ => "badge bg-light text-dark"
        };

        /// <summary>
        /// Icona FontAwesome per lo stato
        /// </summary>
        public string StatoIcon => Stato switch
        {
            StatoRegistro.Bozza => "fa-solid fa-file-pen",
            StatoRegistro.InRevisione => "fa-solid fa-magnifying-glass",
            StatoRegistro.Inviato => "fa-solid fa-paper-plane",
            StatoRegistro.Attivo => "fa-solid fa-check-circle",
            StatoRegistro.InScadenza => "fa-solid fa-clock",
            StatoRegistro.InScadenzaPropostoRinnovo => "fa-solid fa-rotate",
            StatoRegistro.Scaduto => "fa-solid fa-calendar-xmark",
            StatoRegistro.Rinnovato => "fa-solid fa-arrows-rotate",
            StatoRegistro.Annullato => "fa-solid fa-ban",
            StatoRegistro.Sospeso => "fa-solid fa-pause",
            _ => "fa-solid fa-question"
        };

        /// <summary>
        /// Descrizione dello stato
        /// </summary>
        public string StatoDescrizione => Stato switch
        {
            StatoRegistro.Bozza => "In bozza",
            StatoRegistro.InRevisione => "In revisione",
            StatoRegistro.Inviato => "Inviato",
            StatoRegistro.Attivo => "Attivo",
            StatoRegistro.InScadenza => "In scadenza",
            StatoRegistro.InScadenzaPropostoRinnovo => "In scadenza - Proposto rinnovo",
            StatoRegistro.Scaduto => "Scaduto",
            StatoRegistro.Rinnovato => "Rinnovato",
            StatoRegistro.Annullato => "Annullato",
            StatoRegistro.Sospeso => "Sospeso",
            _ => "Sconosciuto"
        };
    }

    /// <summary>
    /// ViewModel per la visualizzazione delle versioni di un registro
    /// </summary>
    public class RegistroContrattiVersioneViewModel
    {
        public Guid Id { get; set; }
        public string? NumeroProtocollo { get; set; }
        public string Oggetto { get; set; } = string.Empty;
        public DateTime DataDocumento { get; set; }
        public StatoRegistro Stato { get; set; }

        public string StatoDescrizione => Stato switch
        {
            StatoRegistro.Bozza => "In bozza",
            StatoRegistro.InRevisione => "In revisione",
            StatoRegistro.Inviato => "Inviato",
            StatoRegistro.Attivo => "Attivo",
            StatoRegistro.InScadenza => "In scadenza",
            StatoRegistro.InScadenzaPropostoRinnovo => "In scadenza - Proposto rinnovo",
            StatoRegistro.Scaduto => "Scaduto",
            StatoRegistro.Rinnovato => "Rinnovato",
            StatoRegistro.Annullato => "Annullato",
            StatoRegistro.Sospeso => "Sospeso",
            _ => "Sconosciuto"
        };

        public string StatoBadgeClass => Stato switch
        {
            StatoRegistro.Bozza => "badge bg-secondary",
            StatoRegistro.InRevisione => "badge bg-warning text-dark",
            StatoRegistro.Inviato => "badge bg-info",
            StatoRegistro.Attivo => "badge bg-success",
            StatoRegistro.InScadenza => "badge bg-warning text-dark",
            StatoRegistro.InScadenzaPropostoRinnovo => "badge bg-warning text-dark",
            StatoRegistro.Scaduto => "badge bg-danger",
            StatoRegistro.Rinnovato => "badge bg-primary",
            StatoRegistro.Annullato => "badge bg-dark",
            StatoRegistro.Sospeso => "badge bg-secondary",
            _ => "badge bg-light text-dark"
        };
    }
}