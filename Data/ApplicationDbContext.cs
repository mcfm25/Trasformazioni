using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection.Emit;
using Trasformazioni.Models.Entities;

namespace Trasformazioni.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options
            )
            : base(options)
        {
        }

        public DbSet<Mezzo> Mezzi { get; set; } = null!;
        public DbSet<AssegnazioneMezzo> AssegnazioniMezzi { get; set; } = null!;
        public DbSet<Soggetto> Soggetti { get; set; } = null!;

        // Modulo Gare
        public DbSet<Gara> Gare { get; set; } = null!;
        public DbSet<Lotto> Lotti { get; set; } = null!;
        public DbSet<ValutazioneLotto> ValutazioniLotti { get; set; } = null!;
        public DbSet<ElaborazioneLotto> ElaborazioniLotti { get; set; } = null!;
        public DbSet<Preventivo> Preventivi { get; set; } = null!;
        public DbSet<RichiestaIntegrazione> RichiesteIntegrazione { get; set; } = null!;
        public DbSet<PartecipanteLotto> PartecipantiLotti { get; set; } = null!;
        public DbSet<DocumentoGara> DocumentiGara { get; set; } = null!;
        public DbSet<Scadenza> Scadenze { get; set; } = null!;
        public DbSet<Notifica> Notifiche { get; set; } = null!;
        public DbSet<TipoDocumento> TipiDocumento { get; set; } = null!;
        public DbSet<LottoDocumentoRichiesto> LottoDocumentiRichiesti { get; set; }
        public DbSet<GaraDocumentoRichiesto> GaraDocumentiRichiesti { get; set; }

        // Modulo Registro Contratti
        public DbSet<CategoriaContratto> CategorieContratto { get; set; } = null!;
        public DbSet<RegistroContratti> RegistroContratti { get; set; } = null!;
        public DbSet<AllegatoRegistro> AllegatiRegistro { get; set; } = null!;


        public DbSet<Reparto> Reparti { get; set; }
        public DbSet<ConfigurazioneNotificaEmail> ConfigurazioniNotificaEmail { get; set; }
        public DbSet<DestinatarioNotificaEmail> DestinatariNotificaEmail { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(GetType().Assembly);

            //// SERVE SOLO PER ENTITA' CHE IMPLEMENTANO IAuditableEntity, SUL BASEENTITY E' PRESENTE IL QUERY FILTER
            //// si potrebbe eliminare e gestire nella BL il filtro IsDeleted in query
            // Applica filtro globale per soft delete su tutte le entità IAuditableEntity
            // QUESTO È IMPORTANTE: esclude automaticamente le entità con IsDeleted = true da TUTTE le query
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                if (typeof(IAuditableEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var property = Expression.Property(parameter, nameof(IAuditableEntity.IsDeleted));
                    var filter = Expression.Lambda(Expression.Not(property), parameter);

                    entityType.SetQueryFilter(filter);
                }
            }
        }
    }
}