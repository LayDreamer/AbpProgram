namespace YaSha.DataManager.CAP;

public interface IDataManagerCapTransactionApiFactory
{
    Type TransactionApiType { get; }
    
    ITransactionApi Create(ITransactionApi originalApi);
}