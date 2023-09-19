namespace YaSha.DataManager.ElasticSearch.Exceptions;

public class DataManagerElasticSearchEntityNotFoundException : BusinessException
{
    public DataManagerElasticSearchEntityNotFoundException(
        string code = null,
        string message = null,
        string details = null,
        Exception innerException = null,
        LogLevel logLevel = LogLevel.Error)
        : base(
            code,
            message,
            details,
            innerException,
            logLevel
        )
    {
    }

    public DataManagerElasticSearchEntityNotFoundException(SerializationInfo serializationInfo, StreamingContext context) : base(serializationInfo, context)
    {
    }
}