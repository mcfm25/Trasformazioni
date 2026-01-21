using Trasformazioni.Models.Enums;

namespace Trasformazioni.Models.Entities
{
    /// <summary>
    /// Entità per la gestione di soggetti (Aziende e Persone Fisiche)
    /// Può essere Cliente, Fornitore, o entrambi
    /// </summary>
    public class Soggetto : BaseEntity
    {
        /// <summary>
        /// Identificatore univoco
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Codice identificativo interno aziendale
        /// </summary>
        public string? CodiceInterno { get; set; }

        // ===================================
        // CLASSIFICAZIONE
        // ===================================

        /// <summary>
        /// Tipologia di soggetto (Azienda o Persona Fisica)
        /// </summary>
        public TipoSoggetto TipoSoggetto { get; set; }

        /// <summary>
        /// Natura giuridica (PA o Privato)
        /// </summary>
        public NaturaGiuridica NaturaGiuridica { get; set; }

        /// <summary>
        /// Indica se il soggetto è un cliente
        /// </summary>
        public bool IsCliente { get; set; }

        /// <summary>
        /// Indica se il soggetto è un fornitore
        /// </summary>
        public bool IsFornitore { get; set; }

        // ===================================
        // DATI ANAGRAFICI (condizionali per tipo)
        // ===================================

        /// <summary>
        /// Denominazione / Ragione Sociale (per Azienda)
        /// OBBLIGATORIO se TipoSoggetto = Azienda
        /// </summary>
        public string? Denominazione { get; set; }

        /// <summary>
        /// Nome (per Persona Fisica)
        /// OBBLIGATORIO se TipoSoggetto = PersonaFisica
        /// </summary>
        public string? Nome { get; set; }

        /// <summary>
        /// Cognome (per Persona Fisica)
        /// OBBLIGATORIO se TipoSoggetto = PersonaFisica
        /// </summary>
        public string? Cognome { get; set; }

        /// <summary>
        /// Codice Fiscale (formato italiano - 16 caratteri alfanumerici)
        /// OBBLIGATORIO se TipoSoggetto = PersonaFisica
        /// </summary>
        public string? CodiceFiscale { get; set; }

        /// <summary>
        /// Partita IVA (formato internazionale)
        /// OBBLIGATORIO se TipoSoggetto = Azienda
        /// </summary>
        public string? PartitaIVA { get; set; }

        /// <summary>
        /// Codice SDI per fatturazione elettronica (7 caratteri)
        /// "0000000" se si usa PEC
        /// </summary>
        public string? CodiceSDI { get; set; }

        /// <summary>
        /// Codice IPA (Indice delle Pubbliche Amministrazioni)
        /// OBBLIGATORIO se NaturaGiuridica = PA
        /// Max 6 caratteri alfanumerici
        /// </summary>
        public string? CodiceIPA { get; set; }

        /// <summary>
        /// Nome del referente aziendale (per Aziende)
        /// </summary>
        public string? Referente { get; set; }

        // ===================================
        // CONTATTI
        // ===================================

        /// <summary>
        /// Email (OBBLIGATORIA)
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Numero di telefono
        /// </summary>
        public string? Telefono { get; set; }

        /// <summary>
        /// Posta Elettronica Certificata
        /// </summary>
        public string? PEC { get; set; }

        // ===================================
        // INDIRIZZO (dettagliato e separato)
        // ===================================

        /// <summary>
        /// Tipo via (es. "Via", "Piazza", "Viale", "Corso")
        /// </summary>
        public string? TipoVia { get; set; }

        /// <summary>
        /// Nome della via (es. "Giacomo Leopardi", "Roma")
        /// </summary>
        public string? NomeVia { get; set; }

        /// <summary>
        /// Numero civico (es. "15", "42/A", "s.n.c.")
        /// </summary>
        public string? NumeroCivico { get; set; }

        /// <summary>
        /// Città
        /// </summary>
        public string? Citta { get; set; }

        /// <summary>
        /// Codice Avviamento Postale
        /// </summary>
        public string? CAP { get; set; }

        /// <summary>
        /// Provincia (sigla - es. "RM", "MI")
        /// </summary>
        public string? Provincia { get; set; }

        /// <summary>
        /// Nazione (es. "IT", "Italia")
        /// </summary>
        public string? Nazione { get; set; }

        // ===================================
        // DATI COMMERCIALI
        // ===================================

        /// <summary>
        /// Condizioni di pagamento (es. "30 gg d.f.f.m.", "60 gg d.f.")
        /// </summary>
        public string? CondizioniPagamento { get; set; }

        /// <summary>
        /// IBAN per bonifici
        /// </summary>
        public string? IBAN { get; set; }

        /// <summary>
        /// Percentuale sconto partner (applicabile solo se IsFornitore = true)
        /// </summary>
        public decimal? ScontoPartner { get; set; }

        // ===================================
        // ALTRO
        // ===================================

        /// <summary>
        /// Note generiche
        /// </summary>
        public string? Note { get; set; }

        // ===================================
        // COMPUTED PROPERTIES
        // ===================================

        /// <summary>
        /// Restituisce il nome completo/denominazione del soggetto
        /// </summary>
        public string NomeCompleto
        {
            get
            {
                return TipoSoggetto == TipoSoggetto.Azienda
                    ? Denominazione ?? "N/D"
                    : $"{Nome} {Cognome}".Trim();
            }
        }

        /// <summary>
        /// Restituisce l'indirizzo completo formattato
        /// </summary>
        public string IndirizzoCompleto
        {
            get
            {
                var parts = new List<string>();

                // Via completa
                if (!string.IsNullOrWhiteSpace(TipoVia) && !string.IsNullOrWhiteSpace(NomeVia))
                {
                    var via = $"{TipoVia} {NomeVia}";
                    if (!string.IsNullOrWhiteSpace(NumeroCivico))
                        via += $", {NumeroCivico}";
                    parts.Add(via);
                }
                else if (!string.IsNullOrWhiteSpace(NomeVia))
                {
                    var via = NomeVia;
                    if (!string.IsNullOrWhiteSpace(NumeroCivico))
                        via += $", {NumeroCivico}";
                    parts.Add(via);
                }

                // Città, CAP, Provincia
                if (!string.IsNullOrWhiteSpace(Citta))
                {
                    var cittaCompleta = CAP != null ? $"{CAP} {Citta}" : Citta;
                    if (!string.IsNullOrWhiteSpace(Provincia))
                        cittaCompleta += $" ({Provincia})";
                    parts.Add(cittaCompleta);
                }

                // Nazione (solo se diversa da Italia)
                if (!string.IsNullOrWhiteSpace(Nazione) &&
                    Nazione.ToUpper() != "IT" &&
                    Nazione.ToUpper() != "ITALIA")
                {
                    parts.Add(Nazione);
                }

                return parts.Any() ? string.Join(" - ", parts) : "N/D";
            }
        }

        /// <summary>
        /// Restituisce la descrizione del ruolo (Cliente, Fornitore, o entrambi)
        /// </summary>
        public string RuoloDescrizione
        {
            get
            {
                if (IsCliente && IsFornitore)
                    return "Cliente e Fornitore";
                if (IsCliente)
                    return "Cliente";
                if (IsFornitore)
                    return "Fornitore";
                return "N/D";
            }
        }
    }
}