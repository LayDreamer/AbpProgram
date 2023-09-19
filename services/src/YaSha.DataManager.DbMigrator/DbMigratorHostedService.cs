using Volo.Abp.Data;

namespace YaSha.DataManager.DbMigrator
{
    public class DbMigratorHostedService : IHostedService
    {
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly IConfiguration _configuration;
        public DbMigratorHostedService(IHostApplicationLifetime hostApplicationLifetime,
            IConfiguration configuration)
        {
            _hostApplicationLifetime = hostApplicationLifetime;
            _configuration = configuration;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var application = await AbpApplicationFactory.CreateAsync<DataManagerDbMigratorModule>(options =>
                   {
                       options.Services.ReplaceConfiguration(_configuration);
                       options.UseAutofac();
                       options.Services.AddLogging(c => c.AddSerilog());
                       // https://github.com/abpframework/abp/pull/15208
                       options.AddDataMigrationEnvironment();
                   }))
            {
                await application.InitializeAsync();
                var conn = _configuration.GetValue<string>("ConnectionStrings:Default");
                Console.WriteLine(conn);
                await application
                    .ServiceProvider
                    .GetRequiredService<DataManagerDbMigrationService>()
                    .MigrateAsync();

                await application.ShutdownAsync();

                _hostApplicationLifetime.StopApplication();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
