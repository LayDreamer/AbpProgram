namespace YaSha.DataManager.ElasticSearch.Exceptions;

public class DataManagerElasticSearchException : BusinessException
{
    public DataManagerElasticSearchException(
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

    public DataManagerElasticSearchException(SerializationInfo serializationInfo, StreamingContext context) : base(serializationInfo, context)
    {
    }
}