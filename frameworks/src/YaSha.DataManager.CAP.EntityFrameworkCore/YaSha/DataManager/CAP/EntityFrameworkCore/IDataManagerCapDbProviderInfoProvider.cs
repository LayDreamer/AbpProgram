namespace YaSha.DataManager.CAP.EntityFrameworkCore;

public interface IDataManagerCapDbProviderInfoProvider
{
    DataManagerCapDbProviderInfo GetOrNull(string dbProviderName);
}