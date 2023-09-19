namespace YaSha.DataManager.Data
{
    public interface IDataManagerDbSchemaMigrator
    {
        Task MigrateAsync();
    }
}
