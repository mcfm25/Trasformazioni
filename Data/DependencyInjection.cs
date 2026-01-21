using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Net.Http.Headers;
using System.Text;
using Trasformazioni.Configuration;
using Trasformazioni.Data.Interceptors;
using Trasformazioni.Data.Repositories;
using Trasformazioni.HealthChecks;
using Trasformazioni.Jobs;
using Trasformazioni.Models.Entities;
using Trasformazioni.Repositories;
using Trasformazioni.Repositories.Interfaces;
using Trasformazioni.Services;
using Trasformazioni.Services.Interfaces;

namespace Trasformazioni.Data
{
    public static class DependencyInjection
    {
        /// <summary>
        /// Configures and registers the persistence layer services, including database context and related
        /// dependencies, into the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <remarks>This method performs the following actions: <list type="bullet">
        /// <item><description>Registers the <see cref="DatabaseConfiguration"/> as a singleton, using the configuration
        /// section "Database".</description></item> <item><description>Configures the <see
        /// cref="ApplicationDbContext"/> to use a PostgreSQL database, with the connection string retrieved from the
        /// <see cref="DatabaseConfiguration"/>.</description></item> <item><description>Adds an <see
        /// cref="IHttpContextAccessor"/> to the service collection.</description></item> </list> Ensure that the
        /// "Database" section is properly configured in the application's configuration file.</remarks>
        /// <param name="services">The <see cref="IServiceCollection"/> to which the persistence services will be added.</param>
        /// <param name="configuration">The application's configuration, used to retrieve database settings.</param>
        /// <returns>The updated <see cref="IServiceCollection"/> instance with the persistence services registered.</returns>
        public static IServiceCollection AddPersistenceServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // --- Configurazioni Database ---
            services.AddSingleton(sp =>
                sp.GetRequiredService<IOptions<DatabaseConfiguration>>().Value);

            // --- Database ---
            services.Configure<DatabaseConfiguration>(
                configuration.GetSection("Database"));

            //var dbSettings = configuration.GetSection("Database").Get<DatabaseConfiguration>()!;
            //var connectionString = dbSettings.GetConnectionString();
            services.AddHttpContextAccessor();

            // Registra l'interceptor per l'audit trail
            services.AddScoped<AuditInterceptor>();

            services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
            {
                //var dbConfig = serviceProvider.GetRequiredService<IOptions<DatabaseConfiguration>>().Value;
                //var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
                //options.UseNpgsql(dbConfig.GetConnectionString());
                var dbConfig = serviceProvider.GetRequiredService<IOptions<DatabaseConfiguration>>().Value;
                var auditInterceptor = serviceProvider.GetRequiredService<AuditInterceptor>();

                options.UseNpgsql(dbConfig.GetConnectionString(), npgsqlOptions =>
                {
                    // Configura la tabella delle migration nello schema specificato
                    if (!string.IsNullOrEmpty(dbConfig.Schema) && dbConfig.Schema != "public")
                    {
                        npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", dbConfig.Schema);
                    }
                })
                .AddInterceptors(auditInterceptor);
            });

            return services;
        }

        /// <summary>
        /// Configures and adds ASP.NET Core Identity services to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <remarks>This method configures Identity with the following default settings: <list
        /// type="bullet"> <item><description>Password requirements include a minimum length of 8 characters, at least
        /// one digit, one uppercase letter, and one lowercase letter. Non-alphanumeric characters are
        /// optional.</description></item> <item><description>Users are required to have unique email
        /// addresses.</description></item> <item><description>Account lockout is enabled with a default lockout time
        /// span of 5 minutes after 5 failed access attempts.</description></item> </list> The method also registers the
        /// <see cref="ApplicationDbContext"/> as the store for Identity and adds default token providers.</remarks>
        /// <param name="services">The <see cref="IServiceCollection"/> to which the Identity services will be added.</param>
        /// <param name="configuration">The application's configuration, used to customize Identity settings if needed.</param>
        /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
        public static IServiceCollection AddIdentityServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // === Identity ===
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;

                // User settings
                options.User.RequireUniqueEmail = true;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            return services;
        }

        /// <summary>
        /// Configura e registra i servizi applicativi (business logic)
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <returns>The updated service collection</returns>
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {

            // Configurazione File Upload
            services.Configure<FileUploadConfiguration>(
                configuration.GetSection("FileUpload"));

            // === UTENTI E RUOLI ===
            services.AddScoped<IUserService, UserService>();

            // Qui aggiungeremo altri servizi in futuro
            // services.AddScoped<IExampleService, ExampleService>();

            // === GESTIONE MEZZI ===
            services.AddScoped<IMezzoRepository, MezzoRepository>();
            services.AddScoped<IMezzoService, MezzoService>();

            // === ASSEGNAZIONI MEZZI ===
            services.AddScoped<IAssegnazioneMezzoRepository, AssegnazioneMezzoRepository>();
            services.AddScoped<IAssegnazioneMezzoService, AssegnazioneMezzoService>();

            // === SOGGETTI ===
            services.AddScoped<ISoggettoRepository, SoggettoRepository>();
            services.AddScoped<ISoggettoService, SoggettoService>();

            // === GARE ===
            services.AddScoped<IGaraRepository, GaraRepository>();
            services.AddScoped<ILottoRepository, LottoRepository>();
            services.AddScoped<IPreventivoRepository, PreventivoRepository>();
            services.AddScoped<IRichiestaIntegrazioneRepository, RichiestaIntegrazioneRepository>();
            services.AddScoped<IScadenzaRepository, ScadenzaRepository>();
            services.AddScoped<IGaraService, GaraService>();
            services.AddScoped<ILottoService, LottoService>();
            services.AddScoped<IPreventivoService, PreventivoService>();
            services.AddScoped<IRichiestaIntegrazioneService, RichiestaIntegrazioneService>();
            services.AddScoped<IScadenzaService, ScadenzaService>();
            services.AddScoped<IDocumentoGaraRepository, DocumentoGaraRepository>();
            services.AddScoped<IDocumentoGaraService, DocumentoGaraService>();

            // === VALUTAZIONI LOTTI ===
            services.AddScoped<IValutazioneLottoRepository, ValutazioneLottoRepository>();
            services.AddScoped<IValutazioneLottoService, ValutazioneLottoService>();

            // === ELABORAZIONI LOTTI ===
            services.AddScoped<IElaborazioneLottoRepository, ElaborazioneLottoRepository>();
            services.AddScoped<IElaborazioneLottoService, ElaborazioneLottoService>();

            // === PARTECIPANTI LOTTI ===
            services.AddScoped<IPartecipanteLottoRepository, PartecipanteLottoRepository>();
            services.AddScoped<IPartecipanteLottoService, PartecipanteLottoService>();

            // === Tipi Documento ===
            services.AddScoped<ITipoDocumentoRepository, TipoDocumentoRepository>();
            services.AddScoped<ITipoDocumentoService, TipoDocumentoService>();

            // Registro Contratti Repository
            services.AddScoped<ICategoriaContrattoRepository, CategoriaContrattoRepository>();
            services.AddScoped<IRegistroContrattiRepository, RegistroContrattiRepository>();
            services.AddScoped<IAllegatoRegistroRepository, AllegatoRegistroRepository>();
            // Registro Contratti Service
            services.AddScoped<ICategoriaContrattoService, CategoriaContrattoService>();
            services.AddScoped<IRegistroContrattiService, RegistroContrattiService>();
            services.AddScoped<IAllegatoRegistroService, AllegatoRegistroService>();

            // === REPARTI ===
            services.AddScoped<IRepartoRepository, RepartoRepository>();
            services.AddScoped<IRepartoService, RepartoService>();

            // === Configurazioni notifiche email ===
            services.AddScoped<INotificaEmailConfigRepository, NotificaEmailConfigRepository>();
            services.AddScoped<INotificaEmailConfigService, NotificaEmailConfigService>();

            return services;
        }

        /// <summary>
        /// Adds global authorization policies to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <remarks>"secure by default" [Authorize] global policy by default,
        /// unless [AllowAnonymous] or specific policy is used.</remarks>
        /// <remarks>This method configures a fallback authorization policy that requires all users to be
        /// authenticated  unless a specific policy is explicitly applied to an endpoint.</remarks>
        /// <param name="services">The <see cref="IServiceCollection"/> to which the authorization policies will be added.</param>
        /// <returns>The <see cref="IServiceCollection"/> with the authorization policies configured.</returns>
        public static IServiceCollection AddAuthorizationPolicies(
            this IServiceCollection services)
        {
            // --- Authorization globale ---
            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });
            return services;
        }


        public static IServiceCollection AddClients(
            this IServiceCollection services
            , IConfiguration configuration)
        {
            services.AddHttpClient<ITraccarService, TraccarService>(
                httpClient => {
                    httpClient.BaseAddress = new Uri(configuration.GetValue<string>("Traccar:BaseUrl")!);
                    httpClient.Timeout = TimeSpan.FromSeconds(30);
                    httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                    // Basic Authentication
                    var authToken = Convert.ToBase64String(
                        Encoding.ASCII.GetBytes($"{configuration.GetValue<string>("Traccar:Username")}:{configuration.GetValue<string>("Traccar:Password")}"));
                    httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Basic", authToken);
                }
            );

            return services;
        }

        public static IServiceCollection AddMinIOService(
            this IServiceCollection services
            , IConfiguration configuration)
        {
            // Configurazione MinIO
            services.Configure<MinIOConfiguration>(
                configuration.GetSection("MinIO"));

            // Registrazione servizi
            services.AddSingleton<IMinIOService, MinIOService>();

            return services;
        }

        /// <summary>
        /// Configura e registra il servizio Email
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configuration">The application configuration</param>
        /// <returns>The updated service collection</returns>
        public static IServiceCollection AddEmailService(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Configurazione Email
            services.Configure<EmailConfiguration>(
                configuration.GetSection("Email"));

            // Registrazione servizi
            services.AddScoped<EmailTemplateService>();
            services.AddScoped<IEmailService, EmailService>();

            return services;
        }

        /// <summary>
        /// Configurazione per Health Checks
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IServiceCollection AddHealthChecksServices(
            this IServiceCollection services,
            IConfiguration configuration
            )
        {
            // Health Checks Configuration
            services.AddHealthChecks()
                // Database health check
                .AddNpgSql(
                    configuration.GetSection("Database").Get<DatabaseConfiguration>()!.GetConnectionString(),
                    name: "PostgreSQL DB Check",
                    failureStatus: HealthStatus.Degraded,
                    tags: ["db", "ready"]
                )
                // Memory health check
                .AddCheck<MemoryHealthCheck>(
                    "memory",
                    tags: ["memory"]
                )
                .AddCheck<ConfigurationHealthCheck>(
                    "configuration",
                    failureStatus: HealthStatus.Unhealthy,
                    tags: ["config", "ready"]
                );

            return services;
        }

        /// <summary>
        /// Configurazione per OpenTelemetry
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static WebApplicationBuilder AddOpenTelemetryServices(
            this WebApplicationBuilder builder
            , IConfiguration configuration
            )
        {
            // OpenTelemetry: Resource
            builder.Services.AddOpenTelemetry()
                .ConfigureResource(rb =>
                    rb.AddService(
                        serviceName: configuration.GetValue<string>("OTL:ServiceName")!, 
                        serviceVersion: configuration.GetValue<string>("OTL:ServiceVersion"))
                      .AddAttributes(new KeyValuePair<string, object>[]
                      {
                        new("deployment.environment", builder.Environment.EnvironmentName)
                      }))
                    // Traces
                    .WithTracing(t => t
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddOtlpExporter())
                    // Metrics
                    .WithMetrics(m => m
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddRuntimeInstrumentation()
                        .AddOtlpExporter());

            return builder;
        }

        /// <summary>
        /// Configurazione per il logging con OpenTelemetry
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static WebApplicationBuilder AddLoggingOTLServices(
            this WebApplicationBuilder builder
            )
        {
            // Logs via OpenTelemetry
            builder.Logging.ClearProviders(); // opzionale: evita doppio invio su Console
            builder.Logging.AddOpenTelemetry(ot =>
            {
                ot.IncludeScopes = true;
                ot.IncludeFormattedMessage = true;
                ot.ParseStateValues = true;
                ot.AddOtlpExporter();
            });

            return builder;
        }

        /// <summary>
        /// Configura Hangfire per job scheduling
        /// </summary>
        public static IServiceCollection AddHangfireServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Carica configurazione
            services.Configure<HangfireConfiguration>(
                configuration.GetSection("Hangfire"));

            var hangfireConfig = configuration
                .GetSection("Hangfire")
                .Get<HangfireConfiguration>() ?? new HangfireConfiguration();

            var dbConfig = configuration
                .GetSection("Database")
                .Get<DatabaseConfiguration>()!;

            // Configura Hangfire con PostgreSQL
            services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(options =>
                {
                    options.UseNpgsqlConnection(dbConfig.GetConnectionString());
                }, new PostgreSqlStorageOptions
                {
                    SchemaName = hangfireConfig.Schema,
                    QueuePollInterval = TimeSpan.FromSeconds(hangfireConfig.QueuePollInterval),
                    InvisibilityTimeout = TimeSpan.FromMinutes(hangfireConfig.InvisibilityTimeoutMinutes),
                    DistributedLockTimeout = TimeSpan.FromMinutes(hangfireConfig.DistributedLockTimeoutMinutes),
                    PrepareSchemaIfNecessary = true // Crea tabelle automaticamente
                }));

            // Configura server Hangfire
            services.AddHangfireServer(options =>
            {
                options.WorkerCount = hangfireConfig.WorkerCount;
                options.Queues = new[] { "critical", "default", "scheduled" }; // Ordine = priorità
                options.ServerName = $"{Environment.MachineName}:{Guid.NewGuid():N}";
            });

            // Registra la classe dei job
            services.AddScoped<RegistroContrattiJobs>();

            return services;
        }

        /// <summary>
        /// Configura i job ricorrenti di Hangfire
        /// </summary>
        public static void ConfigureHangfireJobs(
            this IApplicationBuilder app,
            IConfiguration configuration)
        {
            var hangfireConfig = configuration
                .GetSection("Hangfire")
                .Get<HangfireConfiguration>() ?? new HangfireConfiguration();

            // Job: Aggiorna Stati Scadenza
            if (hangfireConfig.Jobs.AggiornaStatiScadenza.Enabled)
            {
                RecurringJob.AddOrUpdate<RegistroContrattiJobs>(
                    "registro-aggiorna-stati-scadenza",
                    job => job.AggiornaStatiScadenzaAsync(),
                    hangfireConfig.Jobs.AggiornaStatiScadenza.CronExpression,
                    new RecurringJobOptions
                    {
                        TimeZone = TimeZoneInfo.Local
                    });
            }
            else
            {
                RecurringJob.RemoveIfExists("registro-aggiorna-stati-scadenza");
            }

            // Job: Processa Rinnovi Automatici
            if (hangfireConfig.Jobs.ProcessaRinnoviAutomatici.Enabled)
            {
                RecurringJob.AddOrUpdate<RegistroContrattiJobs>(
                    "registro-processa-rinnovi",
                    job => job.ProcessaRinnoviAutomaticiAsync(),
                    hangfireConfig.Jobs.ProcessaRinnoviAutomatici.CronExpression,
                    new RecurringJobOptions
                    {
                        TimeZone = TimeZoneInfo.Local
                    });
            }
            else
            {
                RecurringJob.RemoveIfExists("registro-processa-rinnovi");
            }
        }
    }
}
