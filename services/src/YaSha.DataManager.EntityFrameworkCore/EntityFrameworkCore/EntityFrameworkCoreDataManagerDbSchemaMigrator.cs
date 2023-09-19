namespace YaSha.DataManager.EntityFrameworkCore
{
    public class EntityFrameworkCoreDataManagerDbSchemaMigrator
        : IDataManagerDbSchemaMigrator, ITransientDependency
    {
        private readonly IServiceProvider _serviceProvider;

        public EntityFrameworkCoreDataManagerDbSchemaMigrator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task MigrateAsync()
        {
            /* We intentionally resolving the DataManagerMigrationsDbContext
             * from IServiceProvider (instead of directly injecting it)
             * to properly get the connection string of the current tenant in the
             * current scope.
             */

            await _serviceProvider
                .GetRequiredService<DataManagerDbContext>()
                .Database
                .MigrateAsync();
        }
    }
}