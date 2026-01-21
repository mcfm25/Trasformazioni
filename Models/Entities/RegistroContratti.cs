using Trasformazioni.Models.Enums;

namespace Trasformazioni.Models.Entities
{
    /// <summary>
    /// Entità principale per il Registro Contratti e Preventivi
    /// Gestisce preventivi emessi e contratti stipulati con i clienti
    /// Supporta versionamento tramite struttura gerarchica (ParentId)
    /// </summary>
    public class RegistroContratti : BaseEntity
    {
        /// <summary>
        /// Identificatore univoco
        /// </summary>
        public Guid Id { get; set; }

        // ===================================
        // IDENTIFICAZIONE
        // ===================================

        /// <summary>
        /// Codice di riferimento a programma esterno
        /// </summary>
        public string? IdRiferimentoEsterno { get; set; }

        /// <summary>
        /// Numero protocollo interno aziendale
        /// </summary>
        public string? NumeroProtocollo { get; set; }

        /// <summary>
        /// Tipo di documento (Preventivo o Contratto)
        /// </summary>
        public TipoRegistro TipoRegistro { get; set; }

        // ===================================
        // CLIENTE
        // ===================================

        /// <summary>
        /// FK al cliente (Soggetto con IsCliente = true)
        /// Opzionale per permettere inserimento manuale
        /// </summary>
        public Guid? ClienteId { get; set; }

        /// <summary>
        /// Ragione sociale - autopopolata se presente ClienteId, altrimenti inserimento manuale
        /// </summary>
        public string? RagioneSociale { get; set; }

        /// <summary>
        /// Natura giuridica controparte - autopopolata se presente ClienteId
        /// </summary>
        public NaturaGiuridica? TipoControparte { get; set; }

        // ===================================
        // CONTENUTO
        // ===================================

        /// <summary>
        /// Oggetto del contratto/preventivo
        /// </summary>
        public string Oggetto { get; set; } = string.Empty;

        /// <summary>
        /// FK alla categoria contratto
        /// </summary>
        public Guid CategoriaContrattoId { get; set; }

        // ===================================
        // RESPONSABILE INTERNO
        // ===================================

        /// <summary>
        /// FK all'utente responsabile interno
        /// </summary>
        public string? UtenteId { get; set; }

        /// <summary>
        /// Nome responsabile - autopopolato se presente UtenteId
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
        /// Data di decorrenza/inizio validità
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
        /// Data di accettazione da parte del cliente
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
        /// Giorni di anticipo per alert scadenza (default 60)
        /// </summary>
        public int GiorniAlertScadenza { get; set; } = 60;

        /// <summary>
        /// Indica se il contratto si rinnova automaticamente
        /// </summary>
        public bool IsRinnovoAutomatico { get; set; } = false;

        /// <summary>
        /// Durata del rinnovo automatico in giorni
        /// </summary>
        public int? GiorniRinnovoAutomatico { get; set; }

        // ===================================
        // IMPORTI
        // ===================================

        /// <summary>
        /// Importo del canone annuo
        /// </summary>
        public decimal? ImportoCanoneAnnuo { get; set; }

        /// <summary>
        /// Importo una tantum (setup, attivazione, ecc.)
        /// </summary>
        public decimal? ImportoUnatantum { get; set; }

        // ===================================
        // STATO
        // ===================================

        /// <summary>
        /// Stato corrente del documento
        /// </summary>
        public StatoRegistro Stato { get; set; } = StatoRegistro.Bozza;

        // ===================================
        // GERARCHIA / VERSIONAMENTO
        // ===================================

        /// <summary>
        /// FK al record padre (per versionamento e alberatura)
        /// </summary>
        public Guid? ParentId { get; set; }

        // ===================================
        // NAVIGATION PROPERTIES
        // ===================================

        /// <summary>
        /// Cliente associato
        /// </summary>
        public virtual Soggetto? Cliente { get; set; }

        /// <summary>
        /// Categoria del contratto
        /// </summary>
        public virtual CategoriaContratto CategoriaContratto { get; set; } = null!;

        /// <summary>
        /// Utente responsabile interno
        /// </summary>
        public virtual ApplicationUser? Utente { get; set; }

        /// <summary>
        /// Record padre (per versionamento)
        /// </summary>
        public virtual RegistroContratti? Parent { get; set; }

        /// <summary>
        /// Record figli (versioni successive)
        /// </summary>
        public virtual ICollection<RegistroContratti>? Children { get; set; }

        /// <summary>
        /// Allegati del documento
        /// </summary>
        public virtual ICollection<AllegatoRegistro>? Allegati { get; set; }

        // ===================================
        // COMPUTED PROPERTIES
        // ===================================

        /// <summary>
        /// Data limite per la disdetta (DataFineOScadenza - GiorniPreavvisoDisdetta)
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
        /// Data di attivazione alert scadenza (DataLimiteDisdetta - GiorniAlertScadenza)
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
        /// Importo totale (canone annuo + una tantum)
        /// </summary>
        public decimal ImportoTotale => (ImportoCanoneAnnuo ?? 0) + (ImportoUnatantum ?? 0);
    }
}