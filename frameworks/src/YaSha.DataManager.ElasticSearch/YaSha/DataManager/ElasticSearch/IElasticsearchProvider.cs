namespace YaSha.DataManager.ElasticSearch;

public interface IElasticsearchProvider
{
    IElasticClient GetClient();
}