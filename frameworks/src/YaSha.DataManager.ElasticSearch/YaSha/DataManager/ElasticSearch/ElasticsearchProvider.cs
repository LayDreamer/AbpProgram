namespace YaSha.DataManager.ElasticSearch;

public class ElasticsearchProvider : IElasticsearchProvider, ISingletonDependency
{
    private readonly DataManagerElasticSearchOptions _options;

    public ElasticsearchProvider(IOptions<DataManagerElasticSearchOptions> options)
    {
        _options = options.Value;
    } 

    public virtual IElasticClient GetClient()
    {
        var connectionPool = new SingleNodeConnectionPool(new Uri(_options.Host));
        var settings = new ConnectionSettings(connectionPool);
        settings.BasicAuthentication(_options.UserName, _options.Password);
        return new ElasticClient(settings);
    }
}