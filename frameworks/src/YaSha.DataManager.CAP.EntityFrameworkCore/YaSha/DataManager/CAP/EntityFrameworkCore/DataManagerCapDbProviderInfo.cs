namespace YaSha.DataManager.CAP.EntityFrameworkCore;

public class DataManagerCapDbProviderInfo
{
    public Type CapTransactionType { get; }
    
    public Type CapEfDbTransactionType { get; }
    
    public DataManagerCapDbProviderInfo(string capTransactionTypeName, string capEfDbTransactionTypeName)
    {
        CapTransactionType = Type.GetType(capTransactionTypeName, false);
        CapEfDbTransactionType = Type.GetType(capEfDbTransactionTypeName, false);
    }
}