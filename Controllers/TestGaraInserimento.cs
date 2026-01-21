using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trasformazioni.Data;
using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;

namespace Trasformazioni.Controllers
{
    /// <summary>
    /// Controller di test per verificare l'inserimento di una gara con lotti
    /// DA USARE SOLO IN DEVELOPMENT - ELIMINARE IN PRODUZIONE
    /// </summary>
    public class TestGaraController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TestGaraController> _logger;

        public TestGaraController(
            ApplicationDbContext context,
            ILogger<TestGaraController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// GET: /TestGara/CreaGaraTest
        /// Crea una gara di test con 2 lotti
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> CreaGaraTest()
        {
            try
            {
                // ===== CREA GARA DI TEST =====
                var gara = new Gara
                {
                    Id = Guid.NewGuid(),
                    CodiceGara = "GAR-TEST-2025-001",
                    Titolo = "Fornitura Mezzi Speciali - Test",
                    Descrizione = "Gara di test per verifica funzionamento modulo",
                    Tipologia = TipologiaGara.AppaltoPubblico,
                    Stato = StatoGara.Bozza,

                    // Info Amministrazione
                    EnteAppaltante = "Comune di Milano",
                    Regione = "Lombardia",
                    NomePuntoOrdinante = "Ufficio Acquisti",
                    TelefonoPuntoOrdinante = "02-12345678",

                    // Codici
                    CIG = "Z1234567890",
                    CUP = "C12345678901234",
                    RDO = "RDO-2025-001",
                    Bando = "BANDO-001/2025",
                    Procedura = "Procedura aperta",
                    CriterioAggiudicazione = "Offerta economicamente più vantaggiosa",

                    // Date
                    DataPubblicazione = DateTime.Now.AddDays(-10),
                    DataInizioPresentazioneOfferte = DateTime.Now.AddDays(-5),
                    DataTermineRichiestaChiarimenti = DateTime.Now.AddDays(10),
                    DataTerminePresentazioneOfferte = DateTime.Now.AddDays(20),

                    // Info economiche
                    ImportoTotaleStimato = 500000.00m,

                    // Link
                    LinkPiattaforma = "https://piattaforma-gare.example.com/gara/001"
                };

                await _context.Gare.AddAsync(gara);

                // ===== CREA LOTTO 1 =====
                var lotto1 = new Lotto
                {
                    Id = Guid.NewGuid(),
                    GaraId = gara.Id,
                    CodiceLotto = "LOT-001",
                    Descrizione = "Furgoni Elettrici",
                    Tipologia = TipologiaLotto.Servizi,
                    Stato = StatoLotto.Bozza,

                    // Info economiche
                    ImportoBaseAsta = 200000.00m,
                    Quotazione = 195000.00m,

                    // Info generali
                    GiorniFornitura = 90,
                    LinkPiattaforma = "https://piattaforma-gare.example.com/gara/001/lotto/1",

                    // Contratto
                    DurataContratto = "24 mesi",
                    Fatturazione = "Mensile",

                    // Partecipazione
                    RichiedeFideiussione = true
                };

                await _context.Lotti.AddAsync(lotto1);

                // ===== CREA LOTTO 2 =====
                var lotto2 = new Lotto
                {
                    Id = Guid.NewGuid(),
                    GaraId = gara.Id,
                    CodiceLotto = "LOT-002",
                    Descrizione = "Autocarri Pesanti",
                    Tipologia = TipologiaLotto.Misto,
                    Stato = StatoLotto.Bozza,

                    // Info economiche
                    ImportoBaseAsta = 300000.00m,
                    Quotazione = 285000.00m,

                    // Info generali
                    GiorniFornitura = 120,
                    LinkPiattaforma = "https://piattaforma-gare.example.com/gara/001/lotto/2",

                    // Contratto
                    DurataContratto = "36 mesi",
                    Fatturazione = "Trimestrale",

                    // Partecipazione
                    RichiedeFideiussione = true
                };

                await _context.Lotti.AddAsync(lotto2);

                // ===== CREA SCADENZA DI TEST =====
                var scadenza = new Scadenza
                {
                    Id = Guid.NewGuid(),
                    GaraId = gara.Id,
                    Tipo = TipoScadenza.PresentazioneOfferta,
                    DataScadenza = DateTime.Now.AddDays(20),
                    Descrizione = "Termine presentazione offerte",
                    IsAutomatica = true,
                    IsCompletata = false,
                    GiorniPreavviso = 5
                };

                await _context.Scadenze.AddAsync(scadenza);

                // ===== SALVA TUTTO =====
                await _context.SaveChangesAsync();

                _logger.LogInformation("Gara di test creata con successo. GaraId: {GaraId}", gara.Id);

                // Ritorna una view con i dettagli
                var risultato = new
                {
                    Success = true,
                    Message = "Gara di test creata con successo!",
                    GaraId = gara.Id,
                    CodiceGara = gara.CodiceGara,
                    NumeroLotti = 2,
                    Lotti = new[]
                    {
                        new { Codice = lotto1.CodiceLotto, Descrizione = lotto1.Descrizione },
                        new { Codice = lotto2.CodiceLotto, Descrizione = lotto2.Descrizione }
                    }
                };

                return Json(risultato);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la creazione della gara di test");
                return Json(new
                {
                    Success = false,
                    Message = "Errore durante la creazione della gara di test",
                    Error = ex.Message,
                    InnerError = ex.InnerException?.Message
                });
            }
        }

        /// <summary>
        /// GET: /TestGara/VerificaGara?codiceGara=GAR-TEST-2025-001
        /// Verifica che la gara sia stata creata correttamente
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> VerificaGara(string codiceGara = "GAR-TEST-2025-001")
        {
            try
            {
                var gara = await _context.Gare
                    .Include(g => g.Lotti)
                    .FirstOrDefaultAsync(g => g.CodiceGara == codiceGara);

                if (gara == null)
                {
                    return Json(new
                    {
                        Success = false,
                        Message = $"Gara con codice '{codiceGara}' non trovata"
                    });
                }

                var risultato = new
                {
                    Success = true,
                    Message = "Gara trovata!",
                    Gara = new
                    {
                        gara.Id,
                        gara.CodiceGara,
                        gara.Titolo,
                        gara.Stato,
                        gara.ImportoTotaleStimato,
                        gara.EnteAppaltante,
                        NumeroLotti = gara.Lotti.Count,
                        Lotti = gara.Lotti.Select(l => new
                        {
                            l.Id,
                            l.CodiceLotto,
                            l.Descrizione,
                            l.Stato,
                            l.ImportoBaseAsta
                        }).ToList()
                    }
                };

                return Json(risultato);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la verifica della gara");
                return Json(new
                {
                    Success = false,
                    Message = "Errore durante la verifica",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// GET: /TestGara/EliminaGaraTest?codiceGara=GAR-TEST-2025-001
        /// Elimina la gara di test (soft delete)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> EliminaGaraTest(string codiceGara = "GAR-TEST-2025-001")
        {
            try
            {
                var gara = await _context.Gare
                    .Include(g => g.Lotti)
                    .FirstOrDefaultAsync(g => g.CodiceGara == codiceGara);

                if (gara == null)
                {
                    return Json(new
                    {
                        Success = false,
                        Message = $"Gara con codice '{codiceGara}' non trovata"
                    });
                }

                // Soft delete della gara e dei lotti
                _context.Gare.Remove(gara);
                await _context.SaveChangesAsync();

                return Json(new
                {
                    Success = true,
                    Message = "Gara di test eliminata con successo (soft delete)"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante l'eliminazione della gara");
                return Json(new
                {
                    Success = false,
                    Message = "Errore durante l'eliminazione",
                    Error = ex.Message
                });
            }
        }
    }
}