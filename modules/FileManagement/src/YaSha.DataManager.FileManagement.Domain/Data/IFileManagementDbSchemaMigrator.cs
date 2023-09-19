namespace YaSha.DataManager.FileManagement.Data;

public interface IFileManagementDbSchemaMigrator
{
    Task MigrateAsync();
}