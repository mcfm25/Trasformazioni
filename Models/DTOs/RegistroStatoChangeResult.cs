using Trasformazioni.Models.Enums;

namespace Trasformazioni.Models.DTOs
{
    /// <summary>
    /// DTO per rappresentare il risultato di un cambio stato registro
    /// Usato dai job batch per raccogliere info da inviare via email
    /// </summary>
    public class RegistroStatoChangeResult
    {
        /// <summary>
        /// ID del registro modificato
        /// </summary>
        public Guid RegistroId { get; set; }

        /// <summary>
        /// Numero protocollo del registro
        /// </summary>
        public string? NumeroProtocollo { get; set; }

        /// <summary>
        /// Oggetto/descrizione del registro
        /// </summary>
        public string Oggetto { get; set; } = string.Empty;

        /// <summary>
        /// Ragione sociale del cliente
        /// </summary>
        public string? RagioneSociale { get; set; }

        /// <summary>
        /// Data di scadenza del registro
        /// </summary>
        public DateTime? DataScadenza { get; set; }

        /// <summary>
        /// Stato prima della modifica
        /// </summary>
        public StatoRegistro StatoPrecedente { get; set; }

        /// <summary>
        /// Nuovo stato dopo la modifica
        /// </summary>
        public StatoRegistro NuovoStato { get; set; }

        /// <summary>
        /// ID del nuovo registro creato (solo per rinnovi)
        /// </summary>
        public Guid? NuovoRegistroId { get; set; }

        /// <summary>
        /// Protocollo del nuovo registro creato (solo per rinnovi)
        /// </summary>
        public string? NuovoNumeroProtocollo { get; set; }

        /// <summary>
        /// Descrizione leggibile del cambio stato
        /// </summary>
        public string DescrizioneCambioStato => NuovoStato switch
        {
            StatoRegistro.InScadenza => "è passato in stato IN SCADENZA",
            StatoRegistro.Scaduto => "è SCADUTO",
            StatoRegistro.Rinnovato => "è stato RINNOVATO automaticamente",
            _ => $"è passato allo stato {NuovoStato}"
        };
    }
}