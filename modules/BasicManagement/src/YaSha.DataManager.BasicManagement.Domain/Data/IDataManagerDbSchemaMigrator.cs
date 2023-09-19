namespace YaSha.DataManager.BasicManagement.Data
{
    public interface IDataManagerDbSchemaMigrator
    {
        Task MigrateAsync();
    }
}
