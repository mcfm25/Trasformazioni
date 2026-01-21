using Hangfire;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Trasformazioni.Authorization;
using Trasformazioni.Configuration;
using Trasformazioni.Data;
using Trasformazioni.Models.Entities;
using Trasformazioni.Services;
using Trasformazioni.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

var cultureInfo = new CultureInfo("it-IT");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

// Add services to the container.
builder.Services.AddRazorPages();

// --- Dependency Injection ---
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddAuthorizationPolicies();
builder.Services.AddClients(builder.Configuration);
builder.Services.AddMinIOService(builder.Configuration);
builder.Services.AddEmailService(builder.Configuration);

//builder.Services.AddLogging(); // verificare se serve o basta quello di OTL
builder.AddOpenTelemetryServices(builder.Configuration);
builder.AddLoggingOTLServices();
builder.Services.AddHealthChecksServices(builder.Configuration);

// --- Hangfire ---
builder.Services.AddHangfireServices(builder.Configuration);

// === Cookie Settings ===
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});

// --- Configurazione localizzazione --- // da verificare se serve
//builder.Services.Configure<RequestLocalizationOptions>(options =>
//{
//    var supportedCultures = new[] { "it-IT" };
//    options.SetDefaultCulture("it-IT");
//    options.AddSupportedCultures(supportedCultures);
//    options.AddSupportedUICultures(supportedCultures);
//});

// --- Controllers ---
builder.Services.AddControllersWithViews();

// --- Kestrel Server Configuration for Large File Uploads ---
//builder.WebHost.ConfigureKestrel(serverOptions =>
//{
//    //serverOptions.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(1);
//    serverOptions.Limits.MaxRequestBodySize = 130 * 1024 * 1024; // 130 MB - default 30 MB
//    //serverOptions.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(2);
//    //serverOptions.Limits.MinRequestBodyDataRate = new MinDataRate(100, TimeSpan.FromSeconds(30));
//    //serverOptions.Limits.MinResponseDataRate = new MinDataRate(100, TimeSpan.FromSeconds(30));
//});
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    // Leggi configurazione file upload
    var fileConfig = builder.Configuration
        .GetSection("FileUpload")
        .Get<FileUploadConfiguration>() ?? new FileUploadConfiguration();

    // Imposta limite Kestrel (+ 20% buffer per header/metadata)
    var maxSize = (long)(fileConfig.MaxFileSizeBytes * 1.2);
    serverOptions.Limits.MaxRequestBodySize = maxSize;

    //_logger.LogInformation(
    //    "Kestrel MaxRequestBodySize configurato a {MaxSizeMB} MB (file max: {FileSizeMB} MB + buffer)",
    //    maxSize / 1024 / 1024,
    //    fileConfig.MaxFileSizeMB);
});

//// --- Form Options for Large File Uploads ---
//builder.Services.Configure<FormOptions>(options =>
//{
//    options.MultipartBodyLengthLimit = 128 * 1024 * 1024; // 200 MB - default 128 MB
//    //options.MultipartBodyLengthLimit = default;
//    //options.MemoryBufferThreshold = 50 * 1024 * 1024; // 10 MB
//});
builder.Services.Configure<FormOptions>(options =>
{
    var fileConfig = builder.Configuration
        .GetSection("FileUpload")
        .Get<FileUploadConfiguration>() ?? new FileUploadConfiguration();

    // Stesso limite di MaxRequestBodySize
    options.MultipartBodyLengthLimit = (long)(fileConfig.MaxFileSizeBytes * 1.2);
});

var app = builder.Build();

// === Seed Database ===
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await DbInitializer.Initialize(userManager, roleManager, context);
        //await DbInitializer.Initialize(userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Errore durante il seeding del database");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseForwardedHeaders();

//app.UseHttpsRedirection();
//app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        // Cache per 1 anno per file con versioning (asp-append-version="true")
        // Il tag helper aggiunge ?v=hash, quindi se il file cambia, l'URL cambia
        var headers = ctx.Context.Response.Headers;

        // File CSS, JS, fonts, immagini: cache lunga (1 anno)
        var path = ctx.File.Name.ToLowerInvariant();
        if (path.EndsWith(".css") || path.EndsWith(".js") ||
            path.EndsWith(".woff2") || path.EndsWith(".woff") ||
            path.EndsWith(".png") || path.EndsWith(".jpg") ||
            path.EndsWith(".jpeg") || path.EndsWith(".gif") ||
            path.EndsWith(".svg") || path.EndsWith(".ico"))
        {
            // immutable = il file non cambierà mai a questo URL
            headers.CacheControl = "public, max-age=31536000, immutable";
        }
    }
});
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Health Check Endpoints
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";

        var response = new
        {
            status = report.Status.ToString(),
            totalDuration = report.TotalDuration.ToString(),
            checks = report.Entries.Select(x => new
            {
                name = x.Key,
                status = x.Value.Status.ToString(),
                description = x.Value.Description,
                duration = x.Value.Duration.ToString(),
                exception = x.Value.Exception?.Message,
                data = x.Value.Data
            }),
            timestamp = DateTime.Now
        };

        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true
        }));
    }
}).AllowAnonymous();

// Endpoint semplificato per load balancer/Kubernetes
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
}).AllowAnonymous();

// Endpoint per controlli live (sempre disponibile)
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false // Nessun check specifico, solo che l'app risponde
}).AllowAnonymous();

// Configurazione Hangfire Dashboard con autorizzazione
var hangfireConfig = builder.Configuration
    .GetSection("Hangfire")
    .Get<HangfireConfiguration>() ?? new HangfireConfiguration();

app.UseHangfireDashboard(hangfireConfig.DashboardPath, new DashboardOptions
{
    DashboardTitle = hangfireConfig.DashboardTitle,
    Authorization = new[]
    {
        new HangfireAuthorizationFilter() // Filtro custom (vedi sotto)
    }
});

// Configura i job ricorrenti
app.ConfigureHangfireJobs(builder.Configuration);

app.Run();