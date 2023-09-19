namespace YaSha.DataManager.EntityFrameworkCore
{
    /* This class is needed for EF Core console commands
     * (like Add-Migration and Update-Database commands) */
    public class DataManagerMigrationsDbContextFactory : IDesignTimeDbContextFactory<DataManagerDbContext>
    {
        public DataManagerDbContext CreateDbContext(string[] args)
        {
            DataManagerEfCoreEntityExtensionMappings.Configure();

            var configuration = BuildConfiguration();

            var builder = new DbContextOptionsBuilder<DataManagerDbContext>()
                .UseMySql(configuration.GetConnectionString("Default"), MySqlServerVersion.LatestSupportedServerVersion);

            return new DataManagerDbContext(builder.Options);
        }

        private static IConfigurationRoot BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath
                (
                    Path.Combine
                    (
                        Directory.GetCurrentDirectory(),
                        "../YaSha.DataManager.DbMigrator/"
                    )
                )
                .AddJsonFile
                (
                    "appsettings.json",
                    false
                );

            return builder.Build();
        }
    }
}