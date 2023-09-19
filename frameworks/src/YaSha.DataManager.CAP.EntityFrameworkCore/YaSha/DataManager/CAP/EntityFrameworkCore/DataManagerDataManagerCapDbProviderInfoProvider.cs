namespace YaSha.DataManager.CAP.EntityFrameworkCore;

public class DataManagerDataManagerCapDbProviderInfoProvider : IDataManagerCapDbProviderInfoProvider, ITransientDependency
{
    protected ConcurrentDictionary<string, DataManagerCapDbProviderInfo> CapDbProviderInfos { get; set; } = new();

    public virtual DataManagerCapDbProviderInfo GetOrNull(string dbProviderName)
    {
        return CapDbProviderInfos.GetOrAdd(dbProviderName, InternalGetOrNull);
    }
    
    protected virtual DataManagerCapDbProviderInfo InternalGetOrNull(string databaseProviderName)
    {
        switch (databaseProviderName)
        {
            case "Microsoft.EntityFrameworkCore.SqlServer":
                return new DataManagerCapDbProviderInfo(
                    "DotNetCore.CAP.SqlServerCapTransaction, DotNetCore.CAP.SqlServer",
                    "Microsoft.EntityFrameworkCore.Storage.CapEFDbTransaction, DotNetCore.CAP.SqlServer");
            case "Npgsql.EntityFrameworkCore.PostgreSQL":
                return new DataManagerCapDbProviderInfo(
                    "DotNetCore.CAP.PostgreSqlCapTransaction, DotNetCore.CAP.PostgreSql",
                    "Microsoft.EntityFrameworkCore.Storage.CapEFDbTransaction, DotNetCore.CAP.PostgreSQL");
            case "Pomelo.EntityFrameworkCore.MySql":
                return new DataManagerCapDbProviderInfo(
                    "DotNetCore.CAP.MySqlCapTransaction, DotNetCore.CAP.MySql",
                    "Microsoft.EntityFrameworkCore.Storage.CapEFDbTransaction, DotNetCore.CAP.MySql");
            case "Oracle.EntityFrameworkCore":
            case "Devart.Data.Oracle.Entity.EFCore":
                return new DataManagerCapDbProviderInfo(
                    "DotNetCore.CAP.OracleCapTransaction, DotNetCore.CAP.Oracle",
                    "Microsoft.EntityFrameworkCore.Storage.CapEFDbTransaction, DotNetCore.CAP.Oracle");
            case "Microsoft.EntityFrameworkCore.Sqlite":
                return new DataManagerCapDbProviderInfo(
                    "DotNetCore.CAP.SqliteCapTransaction, DotNetCore.CAP.Sqlite",
                    "Microsoft.EntityFrameworkCore.Storage.CapEFDbTransaction, DotNetCore.CAP.Sqlite");
            case "Microsoft.EntityFrameworkCore.InMemory":
                return new DataManagerCapDbProviderInfo(
                    "DotNetCore.CAP.InMemoryCapTransaction, DotNetCore.CAP.InMemoryStorage",
                    "Microsoft.EntityFrameworkCore.Storage.CapEFDbTransaction, DotNetCore.CAP.InMemoryStorage");
            default:
                return null;
        }
    }
}