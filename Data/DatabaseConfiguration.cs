namespace Trasformazioni.Data
{
    public class DatabaseConfiguration
    {
        public string Host { get; set; } = string.Empty;
        public string Database { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int Port { get; set; } = 5432;
        public string Schema { get; set; } = "public"; // Default schema PostgreSQL

        public string GetConnectionString()
        {
            return $"Host={Host};Database={Database};Username={Username};Password={Password};Port={Port};SearchPath={Schema}";
        }
    }
}
