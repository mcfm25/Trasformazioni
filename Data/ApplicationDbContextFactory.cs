using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Trasformazioni.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // Crea la configurazione leggendo l'appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            // Ottieni la configurazione del database
            var dbConfig = configuration.GetSection("Database").Get<DatabaseConfiguration>()!;
            var connectionString = dbConfig.GetConnectionString();

            // Crea le opzioni del DbContext
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            var options = Options.Create(dbConfig);
            var serviceProvider = new ServiceCollection().BuildServiceProvider();

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}