using Microsoft.AspNetCore.Http.HttpResults;
using System.Text.RegularExpressions;
using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;
using Trasformazioni.Models.ViewModels;

namespace Trasformazioni.Mappings
{
    /// <summary>
    /// Extension methods per il mapping tra entità Scadenza e ViewModels
    /// </summary>
    public static class ScadenzaMappingExtensions
    {
        // ===================================
        // ENTITY → VIEWMODEL
        // ===================================

        /// <summary>
        /// Mappa un'entità Scadenza a ScadenzaListViewModel
        /// </summary>
        public static ScadenzaListViewModel ToListViewModel(this Scadenza scadenza)
        {
            return new ScadenzaListViewModel
            {
                Id = scadenza.Id,

                // Relazioni
                GaraId = scadenza.GaraId,
                LottoId = scadenza.LottoId,
                PreventivoId = scadenza.PreventivoId,

                // Identificazione
                Tipo = scadenza.Tipo,
                DataScadenza = scadenza.DataScadenza,
                Descrizione = scadenza.Descrizione,

                // Stato
                IsAutomatica = scadenza.IsAutomatica,
                IsCompletata = scadenza.IsCompletata,
                DataCompletamento = scadenza.DataCompletamento,
                GiorniPreavviso = scadenza.GiorniPreavviso,

                // Info Contestuali
                CodiceGara = scadenza.Gara?.CodiceGara,
                CodiceLotto = scadenza.Lotto?.CodiceLotto,
                FornitorePreventivo = GetFornitorePreventivo(scadenza.Preventivo),
                EnteAppaltante = scadenza.Gara?.EnteAppaltante ?? scadenza.Lotto?.Gara?.EnteAppaltante,

                // Audit
                CreatedAt = scadenza.CreatedAt,
                ModifiedAt = scadenza.ModifiedAt
            };
        }

        /// <summary>
        /// Mappa un'entità Scadenza a ScadenzaDetailsViewModel
        /// Include tutte le relazioni e proprietà
        /// </summary>
        public static ScadenzaDetailsViewModel ToDetailsViewModel(this Scadenza scadenza)
        {
            return new ScadenzaDetailsViewModel
            {
                Id = scadenza.Id,

                // Relazioni
                GaraId = scadenza.GaraId,
                LottoId = scadenza.LottoId,
                PreventivoId = scadenza.PreventivoId,

                // Identificazione
                Tipo = scadenza.Tipo,
                DataScadenza = scadenza.DataScadenza,
                Descrizione = scadenza.Descrizione,

                // Stato
                IsAutomatica = scadenza.IsAutomatica,
                IsCompletata = scadenza.IsCompletata,
                DataCompletamento = scadenza.DataCompletamento,
                GiorniPreavviso = scadenza.GiorniPreavviso,

                // Note
                Note = scadenza.Note,

                // Info Gara (se presente)
                CodiceGara = scadenza.Gara?.CodiceGara,
                TitoloGara = scadenza.Gara?.Titolo,
                TipologiaGara = scadenza.Gara?.Tipologia,
                StatoGara = scadenza.Gara?.Stato,
                EnteAppaltante = scadenza.Gara?.EnteAppaltante ?? scadenza.Lotto?.Gara?.EnteAppaltante,
                Regione = scadenza.Gara?.Regione ?? scadenza.Lotto?.Gara?.Regione,

                // Info Lotto (se presente)
                CodiceLotto = scadenza.Lotto?.CodiceLotto,
                DescrizioneLotto = scadenza.Lotto?.Descrizione,
                TipologiaLotto = scadenza.Lotto?.Tipologia,
                StatoLotto = scadenza.Lotto?.Stato,
                ImportoBaseAstaLotto = scadenza.Lotto?.ImportoBaseAsta,

                // Info Preventivo (se presente)
                FornitorePreventivo = GetFornitorePreventivo(scadenza.Preventivo),
                ImportoOffertoPreventivo = scadenza.Preventivo?.ImportoOfferto,
                StatoPreventivo = scadenza.Preventivo?.Stato,
                DataRichiestaPreventivo = scadenza.Preventivo?.DataRichiesta,

                // Audit
                CreatedAt = scadenza.CreatedAt,
                CreatedBy = scadenza.CreatedBy,
                ModifiedAt = scadenza.ModifiedAt,
                ModifiedBy = scadenza.ModifiedBy
            };
        }

        // ===================================
        // VIEWMODEL → ENTITY
        // ===================================

        /// <summary>
        /// Crea un'entità Scadenza da ScadenzaCreateViewModel
        /// Include normalizzazione automatica dei campi
        /// </summary>
        public static Scadenza ToEntity(this ScadenzaCreateViewModel viewModel)
        {
            return new Scadenza
            {
                // Relazioni (opzionali)
                GaraId = viewModel.GaraId,
                LottoId = viewModel.LottoId,
                PreventivoId = viewModel.PreventivoId,

                // Identificazione
                Tipo = viewModel.Tipo,
                DataScadenza = viewModel.DataScadenza.Date,
                Descrizione = NormalizzaStringa(viewModel.Descrizione)!,

                // Stato (default per nuove scadenze manuali)
                IsAutomatica = false, // Le scadenze create dall'utente sono manuali
                IsCompletata = false,
                DataCompletamento = null,
                GiorniPreavviso = viewModel.GiorniPreavviso,

                // Note
                Note = NormalizzaStringa(viewModel.Note)

                // CreatedAt, CreatedBy, IsDeleted gestiti da AuditInterceptor
            };
        }

        /// <summary>
        /// Aggiorna un'entità Scadenza esistente con i dati del ScadenzaEditViewModel
        /// Include normalizzazione automatica dei campi
        /// </summary>
        public static void UpdateEntity(this ScadenzaEditViewModel viewModel, Scadenza scadenza)
        {
            // Relazioni (opzionali)
            scadenza.GaraId = viewModel.GaraId;
            scadenza.LottoId = viewModel.LottoId;
            scadenza.PreventivoId = viewModel.PreventivoId;

            // Identificazione
            scadenza.Tipo = viewModel.Tipo;
            scadenza.DataScadenza = viewModel.DataScadenza.Date;
            scadenza.Descrizione = NormalizzaStringa(viewModel.Descrizione)!;

            // Stato
            scadenza.IsAutomatica = viewModel.IsAutomatica;
            scadenza.IsCompletata = viewModel.IsCompletata;
            scadenza.DataCompletamento = viewModel.DataCompletamento?.Date;
            scadenza.GiorniPreavviso = viewModel.GiorniPreavviso;

            // Note
            scadenza.Note = NormalizzaStringa(viewModel.Note);

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
        /// Ottiene il nome del fornitore del preventivo
        /// </summary>
        private static string? GetFornitorePreventivo(Preventivo? preventivo)
        {
            if (preventivo?.Soggetto == null)
                return null;

            var soggetto = preventivo.Soggetto;

            if (soggetto.TipoSoggetto == TipoSoggetto.Azienda)
            {
                return soggetto.Denominazione;
            }
            else // PersonaFisica
            {
                var nome = soggetto.Nome?.Trim() ?? string.Empty;
                var cognome = soggetto.Cognome?.Trim() ?? string.Empty;

                if (string.IsNullOrEmpty(nome) && string.IsNullOrEmpty(cognome))
                    return null;

                return $"{nome} {cognome}".Trim();
            }
        }
    }
}


//---

//## ✅ **Riepilogo implementazione completa**

//### **Caratteristiche principali:**

//#### **IScadenzaService**
//- ✅ Metodi CRUD completi
//- ✅ Query per relazioni (gara, lotto, preventivo)
//- ✅ Query per tipo e stato (attive, completate, in scadenza, scadute, oggi)
//- ✅ Metodi per workflow (Completa, Riattiva)
//- ✅ **Validazione coerenza tipo/relazioni** molto dettagliata
//- ✅ Statistiche complete

//#### **ScadenzaService**
//- ✅ **Validazioni business robuste**:
//  -Coerenza tipo scadenza e relazioni (es. PresentazioneOfferta → richiede Gara)
//  - Validazione gerarchica (Preventivo → Lotto → Gara)
//  - Date non troppo vecchie (max 30 giorni)
//  - Completamento con data obbligatoria
//  - **Scadenze automatiche non eliminabili manualmente**
//- ✅ **Operazioni specifiche**:
//  - `CompletaAsync`: marca come completata con data
//  - `RiattivaAsync`: rimuove completamento
//  - Protezione scadenze automatiche (generate dal sistema)
//- ✅ **Logging dettagliato**
//- ✅ **Gestione eccezioni** con try-catch

//#### **ScadenzaMappingExtensions**
//- ✅ Mapping completo Entity ↔ ViewModel
//- ✅ Gestione info contestuali (gara, lotto, preventivo)
//- ✅ Helper per nome fornitore preventivo
//- ✅ Normalizzazione stringhe
//- ✅ Date normalizzate (.Date per rimuovere ore)
//- ✅ Default corretti per nuove scadenze (IsAutomatica = false, IsCompletata = false)

//---

//## 📝 **Note importanti:**

//### **1. Scadenze Automatiche vs Manuali**
//- **Automatiche**: generate dal sistema (es. da gare, preventivi)
//- **Manuali**: create dall'utente
//- Le scadenze automatiche **non possono essere eliminate manualmente**
//- Le scadenze automatiche sono gestite da background job

//### **2. Validazione Coerenza Tipo/Relazioni**
//```
//TipoScadenza                    | Richiede Relazione
//--------------------------------|--------------------
//PresentazioneOfferta           | Gara
//RichiestaChiarimenti           | Gara
//ScadenzaPreventivo             | Preventivo
//StipulaContratto               | Lotto
//ScadenzaContratto              | Lotto
//IntegrazioneDocumentazione     | Gara O Lotto
//Altro                          | Nessuna (opzionale)
//```

//### **3. Validazione Gerarchica**
//- Se specificato **Preventivo** → deve avere anche **Lotto**
//- Se specificato **Lotto** → deve avere anche **Gara**

//### **4. Workflow Completamento**
//```
//Creazione → [Attiva]
//    ↓ CompletaAsync()
//[Completata con DataCompletamento]
//    ↓ RiattivaAsync()
//[Attiva senza DataCompletamento]