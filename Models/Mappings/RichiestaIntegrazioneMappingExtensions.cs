using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;
using Trasformazioni.Models.ViewModels;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Trasformazioni.Mappings
{
    /// <summary>
    /// Extension methods per il mapping tra entità RichiestaIntegrazione e ViewModels
    /// </summary>
    public static class RichiestaIntegrazioneMappingExtensions
    {
        // ===================================
        // ENTITY → VIEWMODEL
        // ===================================

        /// <summary>
        /// Mappa un'entità RichiestaIntegrazione a RichiestaIntegrazioneListViewModel
        /// </summary>
        public static RichiestaIntegrazioneListViewModel ToListViewModel(this RichiestaIntegrazione richiesta)
        {
            return new RichiestaIntegrazioneListViewModel
            {
                Id = richiesta.Id,
                LottoId = richiesta.LottoId,

                // Identificazione
                NumeroProgressivo = richiesta.NumeroProgressivo,
                IsChiusa = richiesta.IsChiusa,

                // Richiesta Ente
                DataRichiestaEnte = richiesta.DataRichiestaEnte,
                TestoRichiestaEnte = richiesta.TestoRichiestaEnte,
                NomeFileRichiesta = richiesta.NomeFileRichiesta,
                DocumentoRichiestaPath = richiesta.DocumentoRichiestaPath,

                // Risposta Azienda
                DataRispostaAzienda = richiesta.DataRispostaAzienda,
                TestoRispostaAzienda = richiesta.TestoRispostaAzienda,
                NomeFileRisposta = richiesta.NomeFileRisposta,
                RispostoDaNome = richiesta.RispostaDa?.NomeCompleto,

                // Info Lotto
                CodiceLotto = richiesta.Lotto?.CodiceLotto ?? string.Empty,
                DescrizioneLotto = richiesta.Lotto?.Descrizione ?? string.Empty,
                CodiceGara = richiesta.Lotto?.Gara?.CodiceGara ?? string.Empty,
                EnteAppaltante = richiesta.Lotto?.Gara?.EnteAppaltante,

                // Audit
                CreatedAt = richiesta.CreatedAt,
                ModifiedAt = richiesta.ModifiedAt
            };
        }

        /// <summary>
        /// Mappa un'entità RichiestaIntegrazione a RichiestaIntegrazioneDetailsViewModel
        /// Include tutte le relazioni e proprietà
        /// </summary>
        public static RichiestaIntegrazioneDetailsViewModel ToDetailsViewModel(this RichiestaIntegrazione richiesta)
        {
            return new RichiestaIntegrazioneDetailsViewModel
            {
                Id = richiesta.Id,
                LottoId = richiesta.LottoId,

                // Identificazione
                NumeroProgressivo = richiesta.NumeroProgressivo,
                IsChiusa = richiesta.IsChiusa,

                // Richiesta Ente
                DataRichiestaEnte = richiesta.DataRichiestaEnte,
                TestoRichiestaEnte = richiesta.TestoRichiestaEnte,
                DocumentoRichiestaPath = richiesta.DocumentoRichiestaPath,
                NomeFileRichiesta = richiesta.NomeFileRichiesta,

                // Risposta Azienda
                DataRispostaAzienda = richiesta.DataRispostaAzienda,
                TestoRispostaAzienda = richiesta.TestoRispostaAzienda,
                DocumentoRispostaPath = richiesta.DocumentoRispostaPath,
                NomeFileRisposta = richiesta.NomeFileRisposta,
                RispostaDaUserId = richiesta.RispostaDaUserId,
                RispostoDaNome = richiesta.RispostaDa?.NomeCompleto,
                RispostoDaEmail = richiesta.RispostaDa?.Email,

                // Info Lotto
                CodiceLotto = richiesta.Lotto?.CodiceLotto ?? string.Empty,
                DescrizioneLotto = richiesta.Lotto?.Descrizione ?? string.Empty,
                TipologiaLotto = richiesta.Lotto?.Tipologia ?? TipologiaLotto.Altro,
                StatoLotto = richiesta.Lotto?.Stato ?? StatoLotto.Bozza,
                ImportoBaseAstaLotto = richiesta.Lotto?.ImportoBaseAsta,

                // Info Gara
                CodiceGara = richiesta.Lotto?.Gara?.CodiceGara ?? string.Empty,
                TitoloGara = richiesta.Lotto?.Gara?.Titolo ?? string.Empty,
                EnteAppaltante = richiesta.Lotto?.Gara?.EnteAppaltante,
                DataTerminePresentazioneOfferte = richiesta.Lotto?.Gara?.DataTerminePresentazioneOfferte,

                // Audit
                CreatedAt = richiesta.CreatedAt,
                CreatedBy = richiesta.CreatedBy,
                ModifiedAt = richiesta.ModifiedAt,
                ModifiedBy = richiesta.ModifiedBy
            };
        }

        // ===================================
        // VIEWMODEL → ENTITY
        // ===================================

        /// <summary>
        /// Crea un'entità RichiestaIntegrazione da RichiestaIntegrazioneCreateViewModel
        /// Include normalizzazione automatica dei campi e assegnazione numero progressivo
        /// </summary>
        public static RichiestaIntegrazione ToEntity(
            this RichiestaIntegrazioneCreateViewModel viewModel,
            int numeroProgressivo,
            string? documentoRichiestaPath = null)
        {
            return new RichiestaIntegrazione
            {
                LottoId = viewModel.LottoId,

                // Identificazione
                NumeroProgressivo = numeroProgressivo,
                IsChiusa = false, // Default per nuove richieste

                // Richiesta Ente
                DataRichiestaEnte = viewModel.DataRichiestaEnte.Date,
                TestoRichiestaEnte = NormalizzaStringa(viewModel.TestoRichiestaEnte)!,
                DocumentoRichiestaPath = NormalizzaStringa(documentoRichiestaPath),
                NomeFileRichiesta = !string.IsNullOrWhiteSpace(documentoRichiestaPath)
                    ? System.IO.Path.GetFileName(documentoRichiestaPath)
                    : null,

                // Risposta Azienda (vuota per nuove richieste)
                DataRispostaAzienda = null,
                TestoRispostaAzienda = null,
                DocumentoRispostaPath = null,
                NomeFileRisposta = null,
                RispostaDaUserId = null

                // CreatedAt, CreatedBy, IsDeleted gestiti da AuditInterceptor
            };
        }

        /// <summary>
        /// Aggiorna un'entità RichiestaIntegrazione esistente con i dati del RichiestaIntegrazioneEditViewModel
        /// Include normalizzazione automatica dei campi
        /// </summary>
        public static void UpdateEntity(
            this RichiestaIntegrazioneEditViewModel viewModel,
            RichiestaIntegrazione richiesta,
            string? nuovoDocumentoRichiestaPath = null,
            string? nuovoDocumentoRispostaPath = null)
        {
            // Identificazione
            richiesta.IsChiusa = viewModel.IsChiusa;

            // Richiesta Ente
            richiesta.DataRichiestaEnte = viewModel.DataRichiestaEnte.Date;
            richiesta.TestoRichiestaEnte = NormalizzaStringa(viewModel.TestoRichiestaEnte)!;

            // Aggiorna documento richiesta solo se c'è un nuovo file
            if (!string.IsNullOrWhiteSpace(nuovoDocumentoRichiestaPath))
            {
                richiesta.DocumentoRichiestaPath = nuovoDocumentoRichiestaPath;
                richiesta.NomeFileRichiesta = System.IO.Path.GetFileName(nuovoDocumentoRichiestaPath);
            }
            else if (!viewModel.MantieniDocumentoRichiestaEsistente)
            {
                // Se l'utente ha deselezionato "mantieni documento esistente" e non ha caricato un nuovo file
                richiesta.DocumentoRichiestaPath = null;
                richiesta.NomeFileRichiesta = null;
            }

            // Risposta Azienda
            richiesta.DataRispostaAzienda = viewModel.DataRispostaAzienda?.Date;
            richiesta.TestoRispostaAzienda = NormalizzaStringa(viewModel.TestoRispostaAzienda);

            // Aggiorna documento risposta solo se c'è un nuovo file
            if (!string.IsNullOrWhiteSpace(nuovoDocumentoRispostaPath))
            {
                richiesta.DocumentoRispostaPath = nuovoDocumentoRispostaPath;
                richiesta.NomeFileRisposta = System.IO.Path.GetFileName(nuovoDocumentoRispostaPath);
            }
            else if (!viewModel.MantieniDocumentoRispostaEsistente)
            {
                // Se l'utente ha deselezionato "mantieni documento esistente" e non ha caricato un nuovo file
                richiesta.DocumentoRispostaPath = null;
                richiesta.NomeFileRisposta = null;
            }

            // RispostaDaUserId viene gestito automaticamente da AuditInterceptor (ModifiedBy)
            // oppure può essere impostato esplicitamente nel service quando si usa RispondiAsync

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
    }
}


//---

//## ✅ **Riepilogo implementazione completa**

//### **Caratteristiche principali:**

//#### **IRichiestaIntegrazioneService**
//- ✅ Metodi CRUD completi
//- ✅ Query per stato (aperte, chiuse, non risposte, scadute)
//- ✅ Metodi per workflow (Rispondi, Chiudi, Riapri)
//- ✅ Validazioni business
//- ✅ Statistiche (conteggi, tempo medio risposta)

//#### **RichiestaIntegrazioneService**
//- ✅ **Numero progressivo automatico**: generato automaticamente per ogni lotto
//- ✅ **Validazioni business robuste**:
//  -Esistenza lotto
//  - Date coerenti(risposta >= richiesta)
//  - Testo minimo 10 caratteri
//  - Risposta obbligatoria prima della chiusura
//  - Non eliminabile se già risposta
//- ✅ **Operazioni specifiche**:
//  - `RispondiAsync`: aggiunge risposta dell'azienda
//  - `ChiudiAsync`: chiude la richiesta (solo se ha risposta)
//  - `RiapriAsync`: riapre una richiesta chiusa
//- ✅ **Gestione documenti**: supporto per documenti allegati sia per richiesta che per risposta
//- ✅ **Logging dettagliato**
//- ✅ **Gestione eccezioni** con try-catch

//#### **RichiestaIntegrazioneMappingExtensions**
//- ✅ Mapping completo Entity ↔ ViewModel
//- ✅ Gestione documenti (richiesta e risposta)
//- ✅ Supporto per "mantieni documento esistente" nel ViewModel Edit
//- ✅ Normalizzazione stringhe
//- ✅ Date normalizzate (.Date per rimuovere ore)
//- ✅ Mapping completo info lotto e gara

//---

//## 📝 **Note importanti:**

//### **1. Numero Progressivo Automatico**
//Il numero progressivo viene generato automaticamente dal repository usando il metodo `GetNextNumeroProgressivoAsync(lottoId)`, che cerca il massimo numero progressivo per quel lotto e aggiunge 1.

//### **2. Gestione Documenti**
//- Supporto per documenti sia nella richiesta che nella risposta
//- I path vengono gestiti dal service/controller (upload fisico)
//- Il mapping gestisce il flag `MantieniDocumentoEsistente` per decidere se eliminare o mantenere i documenti esistenti

//### **3. Workflow Richieste**
//```
//Creazione → [Aperta, Non Risposta]
//    ↓ RispondiAsync()
//[Aperta, Con Risposta]
//    ↓ ChiudiAsync()
//[Chiusa]
//    ↓ RiapriAsync()(se necessario)
//[Aperta, Con Risposta]