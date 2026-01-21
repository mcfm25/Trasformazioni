using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Trasformazioni.Authorization;
using Trasformazioni.Constants;
using Trasformazioni.Models.Entities;
using Trasformazioni.Models.Enums;

namespace Trasformazioni.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            // Crea i ruoli
            await SeedRolesAsync(roleManager);

            // Crea utente admin di default
            await SeedAdminUserAsync(userManager);

            // Crea utente admin di default
            await SeedTipiDocumentoAsync(context);

            // Crea categorie contratto di default
            await SeedCategorieContrattoAsync(context);

            // Crea categorie contratto di default
            await SeedConfigurazioniNotificaEmailAsync(context);
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles =
            {
                RoleNames.DataEntry,
                RoleNames.Amministrazione,
                RoleNames.UfficioGare
            };

            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        private static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager)
        {
            var adminEmail = "admin@trasformazioni.it";

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    Nome = "Admin",
                    Cognome = "Sistema",
                    //Reparto = "IT",
                    DataAssunzione = DateTime.Now,
                    IsAttivo = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, RoleNames.Amministrazione);
                }
            }
        }

        /// <summary>
        /// Crea i tipi documento di sistema se non esistono.
        /// Migra i valori dall'enum TipoDocumentoGara originale.
        /// Il CodiceRiferimento corrisponde al nome dell'enum per controlli workflow.
        /// </summary>
        private static async Task SeedTipiDocumentoAsync(ApplicationDbContext context)
        {
            // Se esistono già tipi di sistema, skip
            if (await context.TipiDocumento.AnyAsync(t => t.IsSystem))
            {
                return;
            }

            var now = DateTime.Now;

            var tipiSistema = new List<TipoDocumento>
            {
                // =======================================================
                // AREA GARE - Tipi a livello di gara
                // =======================================================
                new TipoDocumento
                {
                    Id = Guid.NewGuid(),
                    Nome = "Documento Generale",
                    Descrizione = "Documento generico a livello di gara",
                    Area = AreaDocumento.Gare,
                    IsSystem = true,
                    CodiceRiferimento = nameof(TipoDocumentoGara.DocumentoGeneraleGara),
                    CreatedAt = now,
                    CreatedBy = "system"
                },
                new TipoDocumento
                {
                    Id = Guid.NewGuid(),
                    Nome = "Bando Originale",
                    Descrizione = "Bando originale pubblicato dall'ente",
                    Area = AreaDocumento.Gare,
                    IsSystem = true,
                    CodiceRiferimento = nameof(TipoDocumentoGara.BandoOriginale),
                    CreatedAt = now,
                    CreatedBy = "system"
                },
                new TipoDocumento
                {
                    Id = Guid.NewGuid(),
                    Nome = "Disciplinare",
                    Descrizione = "Disciplinare di gara",
                    Area = AreaDocumento.Gare,
                    IsSystem = true,
                    CodiceRiferimento = nameof(TipoDocumentoGara.Disciplinare),
                    CreatedAt = now,
                    CreatedBy = "system"
                },
                new TipoDocumento
                {
                    Id = Guid.NewGuid(),
                    Nome = "Capitolato Speciale",
                    Descrizione = "Capitolato speciale d'appalto",
                    Area = AreaDocumento.Gare,
                    IsSystem = true,
                    CodiceRiferimento = nameof(TipoDocumentoGara.CapitolatoSpeciale),
                    CreatedAt = now,
                    CreatedBy = "system"
                },

                // =======================================================
                // AREA LOTTI - Valutazione
                // =======================================================
                new TipoDocumento
                {
                    Id = Guid.NewGuid(),
                    Nome = "Documento Valutazione Tecnica",
                    Descrizione = "Documento relativo alla valutazione tecnica del lotto",
                    Area = AreaDocumento.Lotti,
                    IsSystem = true,
                    CodiceRiferimento = nameof(TipoDocumentoGara.DocumentoValutazioneTecnica),
                    CreatedAt = now,
                    CreatedBy = "system"
                },
                new TipoDocumento
                {
                    Id = Guid.NewGuid(),
                    Nome = "Documento Valutazione Economica",
                    Descrizione = "Documento relativo alla valutazione economica del lotto",
                    Area = AreaDocumento.Lotti,
                    IsSystem = true,
                    CodiceRiferimento = nameof(TipoDocumentoGara.DocumentoValutazioneEconomica),
                    CreatedAt = now,
                    CreatedBy = "system"
                },
                new TipoDocumento
                {
                    Id = Guid.NewGuid(),
                    Nome = "Preventivo",
                    Descrizione = "Preventivo da fornitore",
                    Area = AreaDocumento.Lotti,
                    IsSystem = true,
                    CodiceRiferimento = nameof(TipoDocumentoGara.Preventivo),
                    CreatedAt = now,
                    CreatedBy = "system"
                },

                // =======================================================
                // AREA LOTTI - Elaborazione
                // =======================================================
                new TipoDocumento
                {
                    Id = Guid.NewGuid(),
                    Nome = "Documento Presentazione",
                    Descrizione = "Documento di presentazione per il lotto",
                    Area = AreaDocumento.Lotti,
                    IsSystem = true,
                    CodiceRiferimento = nameof(TipoDocumentoGara.DocumentoPresentazione),
                    CreatedAt = now,
                    CreatedBy = "system"
                },
                new TipoDocumento
                {
                    Id = Guid.NewGuid(),
                    Nome = "Offerta Tecnica",
                    Descrizione = "Offerta tecnica da presentare",
                    Area = AreaDocumento.Lotti,
                    IsSystem = true,
                    CodiceRiferimento = nameof(TipoDocumentoGara.OffertaTecnica),
                    CreatedAt = now,
                    CreatedBy = "system"
                },
                new TipoDocumento
                {
                    Id = Guid.NewGuid(),
                    Nome = "Offerta Economica",
                    Descrizione = "Offerta economica da presentare",
                    Area = AreaDocumento.Lotti,
                    IsSystem = true,
                    CodiceRiferimento = nameof(TipoDocumentoGara.OffertaEconomica),
                    CreatedAt = now,
                    CreatedBy = "system"
                },

                // =======================================================
                // AREA LOTTI - Integrazioni
                // =======================================================
                new TipoDocumento
                {
                    Id = Guid.NewGuid(),
                    Nome = "Richiesta Integrazione Ente",
                    Descrizione = "Richiesta di integrazione documentale da parte dell'ente",
                    Area = AreaDocumento.Lotti,
                    IsSystem = true,
                    CodiceRiferimento = nameof(TipoDocumentoGara.RichiestaIntegrazioneEnte),
                    CreatedAt = now,
                    CreatedBy = "system"
                },
                new TipoDocumento
                {
                    Id = Guid.NewGuid(),
                    Nome = "Risposta Integrazione",
                    Descrizione = "Risposta ad integrazione richiesta",
                    Area = AreaDocumento.Lotti,
                    IsSystem = true,
                    CodiceRiferimento = nameof(TipoDocumentoGara.RispostaIntegrazione),
                    CreatedAt = now,
                    CreatedBy = "system"
                },

                // =======================================================
                // AREA LOTTI - Altro
                // =======================================================
                new TipoDocumento
                {
                    Id = Guid.NewGuid(),
                    Nome = "Comunicazione Ente",
                    Descrizione = "Comunicazione generica dall'ente",
                    Area = AreaDocumento.Lotti,
                    IsSystem = true,
                    CodiceRiferimento = nameof(TipoDocumentoGara.ComunicazioneEnte),
                    CreatedAt = now,
                    CreatedBy = "system"
                },
                new TipoDocumento
                {
                    Id = Guid.NewGuid(),
                    Nome = "Contratto",
                    Descrizione = "Contratto stipulato",
                    Area = AreaDocumento.Lotti,
                    IsSystem = true,
                    CodiceRiferimento = nameof(TipoDocumentoGara.Contratto),
                    CreatedAt = now,
                    CreatedBy = "system"
                },
                new TipoDocumento
                {
                    Id = Guid.NewGuid(),
                    Nome = "Altro",
                    Descrizione = "Altro tipo di documento non classificato",
                    Area = AreaDocumento.Lotti,
                    IsSystem = true,
                    CodiceRiferimento = nameof(TipoDocumentoGara.Altro),
                    CreatedAt = now,
                    CreatedBy = "system"
                },

                // =======================================================
                // AREA MEZZI
                // =======================================================
                new TipoDocumento
                {
                    Id = Guid.NewGuid(),
                    Nome = "Libretto di Circolazione",
                    Descrizione = "Libretto di circolazione del veicolo",
                    Area = AreaDocumento.Mezzi,
                    IsSystem = true,
                    CodiceRiferimento = null,  // Nessun enum di riferimento
                    CreatedAt = now,
                    CreatedBy = "system"
                },
                new TipoDocumento
                {
                    Id = Guid.NewGuid(),
                    Nome = "Assicurazione",
                    Descrizione = "Polizza assicurativa del veicolo",
                    Area = AreaDocumento.Mezzi,
                    IsSystem = true,
                    CodiceRiferimento = null,
                    CreatedAt = now,
                    CreatedBy = "system"
                },
                new TipoDocumento
                {
                    Id = Guid.NewGuid(),
                    Nome = "Revisione",
                    Descrizione = "Certificato di revisione",
                    Area = AreaDocumento.Mezzi,
                    IsSystem = true,
                    CodiceRiferimento = null,
                    CreatedAt = now,
                    CreatedBy = "system"
                },
                new TipoDocumento
                {
                    Id = Guid.NewGuid(),
                    Nome = "Contratto Noleggio",
                    Descrizione = "Contratto di noleggio del veicolo",
                    Area = AreaDocumento.Mezzi,
                    IsSystem = true,
                    CodiceRiferimento = null,
                    CreatedAt = now,
                    CreatedBy = "system"
                },
                new TipoDocumento
                {
                    Id = Guid.NewGuid(),
                    Nome = "Altro",
                    Descrizione = "Altri documenti relativi al mezzo",
                    Area = AreaDocumento.Mezzi,
                    IsSystem = true,
                    CodiceRiferimento = null,
                    CreatedAt = now,
                    CreatedBy = "system"
                },

                // =======================================================
                // AREA SOGGETTI
                // =======================================================
                new TipoDocumento
                {
                    Id = Guid.NewGuid(),
                    Nome = "Visura Camerale",
                    Descrizione = "Visura camerale aggiornata",
                    Area = AreaDocumento.Soggetti,
                    IsSystem = true,
                    CodiceRiferimento = null,
                    CreatedAt = now,
                    CreatedBy = "system"
                },
                new TipoDocumento
                {
                    Id = Guid.NewGuid(),
                    Nome = "DURC",
                    Descrizione = "Documento Unico di Regolarità Contributiva",
                    Area = AreaDocumento.Soggetti,
                    IsSystem = true,
                    CodiceRiferimento = null,
                    CreatedAt = now,
                    CreatedBy = "system"
                },
                new TipoDocumento
                {
                    Id = Guid.NewGuid(),
                    Nome = "Certificazione ISO",
                    Descrizione = "Certificazioni ISO del fornitore",
                    Area = AreaDocumento.Soggetti,
                    IsSystem = true,
                    CodiceRiferimento = null,
                    CreatedAt = now,
                    CreatedBy = "system"
                },
                new TipoDocumento
                {
                    Id = Guid.NewGuid(),
                    Nome = "Altro",
                    Descrizione = "Altri documenti relativi al soggetto",
                    Area = AreaDocumento.Soggetti,
                    IsSystem = true,
                    CodiceRiferimento = null,
                    CreatedAt = now,
                    CreatedBy = "system"
                },

                // =======================================================
                // AREA AZIENDA
                // =======================================================
                new TipoDocumento
                {
                    Id = Guid.NewGuid(),
                    Nome = "Visura Camerale",
                    Descrizione = "Visura camerale aziendale",
                    Area = AreaDocumento.Azienda,
                    IsSystem = true,
                    CodiceRiferimento = null,
                    CreatedAt = now,
                    CreatedBy = "system"
                },
                new TipoDocumento
                {
                    Id = Guid.NewGuid(),
                    Nome = "Certificazione ISO",
                    Descrizione = "Certificazioni ISO aziendali",
                    Area = AreaDocumento.Azienda,
                    IsSystem = true,
                    CodiceRiferimento = null,
                    CreatedAt = now,
                    CreatedBy = "system"
                },
                new TipoDocumento
                {
                    Id = Guid.NewGuid(),
                    Nome = "DURC",
                    Descrizione = "DURC aziendale",
                    Area = AreaDocumento.Azienda,
                    IsSystem = true,
                    CodiceRiferimento = null,
                    CreatedAt = now,
                    CreatedBy = "system"
                },
                new TipoDocumento
                {
                    Id = Guid.NewGuid(),
                    Nome = "Bilancio",
                    Descrizione = "Bilancio aziendale",
                    Area = AreaDocumento.Azienda,
                    IsSystem = true,
                    CodiceRiferimento = null,
                    CreatedAt = now,
                    CreatedBy = "system"
                },
                new TipoDocumento
                {
                    Id = Guid.NewGuid(),
                    Nome = "Altro",
                    Descrizione = "Altri documenti aziendali",
                    Area = AreaDocumento.Azienda,
                    IsSystem = true,
                    CodiceRiferimento = null,
                    CreatedAt = now,
                    CreatedBy = "system"
                },

                
                // =======================================================
                // AREA  REGISTRO CONTRATTI
                // =======================================================
                new TipoDocumento
                {
                    Id = Guid.NewGuid(),
                    Nome = "Preventivo",
                    Descrizione = "Preventivo",
                    Area = AreaDocumento.RegistroContratti,
                    IsSystem = true,
                    CodiceRiferimento = null,
                    CreatedAt = now,
                    CreatedBy = "system"
                },
                new TipoDocumento
                {
                    Id = Guid.NewGuid(),
                    Nome = "Contratto",
                    Descrizione = "Contratto",
                    Area = AreaDocumento.RegistroContratti,
                    IsSystem = true,
                    CodiceRiferimento = null,
                    CreatedAt = now,
                    CreatedBy = "system"
                }
            };

            await context.TipiDocumento.AddRangeAsync(tipiSistema);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Inizializza le categorie contratto di default
        /// </summary>
        private static async Task SeedCategorieContrattoAsync(ApplicationDbContext context)
        {
            if (await context.CategorieContratto.AnyAsync())
                return;

            var categorie = new List<CategoriaContratto>
            {
                new() { Id = Guid.NewGuid(), Nome = "Assistenza e Manutenzione", Descrizione = "Contratti di assistenza tecnica e manutenzione", Ordine = 1, IsAttivo = true },
                new() { Id = Guid.NewGuid(), Nome = "Licenze Software", Descrizione = "Licenze d'uso software e abbonamenti", Ordine = 2, IsAttivo = true },
                new() { Id = Guid.NewGuid(), Nome = "Consulenza", Descrizione = "Contratti di consulenza professionale", Ordine = 3, IsAttivo = true },
                new() { Id = Guid.NewGuid(), Nome = "Fornitura Beni", Descrizione = "Contratti di fornitura prodotti e materiali", Ordine = 4, IsAttivo = true },
                new() { Id = Guid.NewGuid(), Nome = "Servizi Cloud", Descrizione = "Servizi cloud, hosting e infrastruttura", Ordine = 5, IsAttivo = true },
                new() { Id = Guid.NewGuid(), Nome = "Locazione", Descrizione = "Contratti di locazione e noleggio", Ordine = 6, IsAttivo = true },
                new() { Id = Guid.NewGuid(), Nome = "Utenze", Descrizione = "Contratti utenze (energia, telefonia, ecc.)", Ordine = 7, IsAttivo = true },
                new() { Id = Guid.NewGuid(), Nome = "Assicurazioni", Descrizione = "Polizze assicurative", Ordine = 8, IsAttivo = true },
                new() { Id = Guid.NewGuid(), Nome = "Altro", Descrizione = "Altre tipologie di contratto", Ordine = 99, IsAttivo = true }
            };

            await context.CategorieContratto.AddRangeAsync(categorie);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Seeder per le configurazioni notifiche email
        /// Crea le configurazioni predefinite al primo avvio (senza destinatari)
        /// </summary>    
        public static async Task SeedConfigurazioniNotificaEmailAsync(ApplicationDbContext context)
        {
            // Se esistono già configurazioni, non fare nulla
            if (await context.ConfigurazioniNotificaEmail.AnyAsync())
                return;

            var configurazioni = CodiciNotifica.GetAll()
                .Select(c => new ConfigurazioneNotificaEmail
                {
                    Id = Guid.NewGuid(),
                    Codice = c.Codice,
                    Descrizione = c.Descrizione,
                    Modulo = c.Modulo,
                    IsAttiva = true,
                    OggettoEmailDefault = GetOggettoDefault(c.Codice),
                    CreatedAt = DateTime.Now,
                    CreatedBy = "SYSTEM"
                })
                .ToList();

            await context.ConfigurazioniNotificaEmail.AddRangeAsync(configurazioni);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Restituisce l'oggetto email di default per ogni operazione
        /// </summary>
        private static string GetOggettoDefault(string codice)
        {
            return codice switch
            {
                // RegistroContratti
                CodiciNotifica.ContrattoInScadenza => "⚠️ Contratto in scadenza",
                CodiciNotifica.ContrattoScaduto => "❌ Contratto scaduto",
                CodiciNotifica.RinnovoAutomatico => "🔄 Rinnovo automatico",

                // Gare
                CodiciNotifica.GaraNuova => "📋 Nuova gara",
                CodiciNotifica.GaraCambioStato => "🔔 Cambio stato gara",
                CodiciNotifica.LottoInScadenza => "⚠️ Lotto in scadenza",

                // Mezzi
                CodiciNotifica.ScadenzaMezzo => "⚠️ Scadenza mezzo",
                CodiciNotifica.AssegnazioneMezzo => "🚗 Assegnazione mezzo",
                CodiciNotifica.RiconsegnaMezzo => "🔙 Riconsegna mezzo",

                // Preventivi
                CodiciNotifica.PreventivoNuovo => "📝 Nuovo preventivo",
                CodiciNotifica.PreventivoCambioStato => "🔔 Cambio stato preventivo",

                // Richieste Integrazione
                CodiciNotifica.RichiestaIntegrazioneNuova => "📩 Nuova richiesta integrazione",
                CodiciNotifica.RichiestaIntegrazioneRisposta => "✉️ Risposta richiesta integrazione",

                _ => "Notifica"
            };
        }
       
    }
}