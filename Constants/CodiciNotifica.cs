namespace Trasformazioni.Constants
{
    /// <summary>
    /// Codici delle operazioni che generano notifiche email
    /// </summary>
    public static class CodiciNotifica
    {
        // ===================================
        // REGISTRO CONTRATTI
        // ===================================

        public const string ContrattoInScadenza = "CONTRATTO_IN_SCADENZA";
        public const string ContrattoScaduto = "CONTRATTO_SCADUTO";
        public const string RinnovoAutomatico = "RINNOVO_AUTOMATICO";

        // ===================================
        // GARE
        // ===================================

        public const string GaraNuova = "GARA_NUOVA";
        public const string GaraCambioStato = "GARA_CAMBIO_STATO";
        public const string LottoInScadenza = "LOTTO_IN_SCADENZA";

        // ===================================
        // MEZZI
        // ===================================

        public const string ScadenzaMezzo = "SCADENZA_MEZZO";
        public const string AssegnazioneMezzo = "ASSEGNAZIONE_MEZZO";
        public const string RiconsegnaMezzo = "RICONSEGNA_MEZZO";

        // ===================================
        // PREVENTIVI
        // ===================================

        public const string PreventivoNuovo = "PREVENTIVO_NUOVO";
        public const string PreventivoCambioStato = "PREVENTIVO_CAMBIO_STATO";

        // ===================================
        // RICHIESTE INTEGRAZIONE
        // ===================================

        public const string RichiestaIntegrazioneNuova = "RICHIESTA_INTEGRAZIONE_NUOVA";
        public const string RichiestaIntegrazioneRisposta = "RICHIESTA_INTEGRAZIONE_RISPOSTA";

        // ===================================
        // HELPER
        // ===================================

        /// <summary>
        /// Restituisce tutti i codici con descrizione e modulo per il seeding
        /// </summary>
        public static List<(string Codice, string Descrizione, string Modulo)> GetAll()
        {
            return new List<(string, string, string)>
            {
                // RegistroContratti
                (ContrattoInScadenza, "Contratto in scadenza", "RegistroContratti"),
                (ContrattoScaduto, "Contratto scaduto", "RegistroContratti"),
                (RinnovoAutomatico, "Rinnovo automatico creato", "RegistroContratti"),

                // Gare
                (GaraNuova, "Nuova gara creata", "Gare"),
                (GaraCambioStato, "Cambio stato gara", "Gare"),
                (LottoInScadenza, "Lotto in scadenza", "Gare"),

                // Mezzi
                (ScadenzaMezzo, "Scadenza mezzo", "Mezzi"),
                (AssegnazioneMezzo, "Assegnazione mezzo", "Mezzi"),
                (RiconsegnaMezzo, "Riconsegna mezzo", "Mezzi"),

                // Preventivi
                (PreventivoNuovo, "Nuovo preventivo", "Preventivi"),
                (PreventivoCambioStato, "Cambio stato preventivo", "Preventivi"),

                // Richieste Integrazione
                (RichiestaIntegrazioneNuova, "Nuova richiesta integrazione", "RichiesteIntegrazione"),
                (RichiestaIntegrazioneRisposta, "Risposta richiesta integrazione", "RichiesteIntegrazione")
            };
        }
    }
}