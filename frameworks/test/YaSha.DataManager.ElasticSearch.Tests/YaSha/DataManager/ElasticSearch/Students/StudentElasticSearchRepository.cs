using Volo.Abp.DependencyInjection;

namespace YaSha.DataManager.ElasticSearch.Students;

public class StudentElasticSearchRepository : ElasticSearchRepository<Student>, IStudentElasticSearchRepository, ITransientDependency
{
    public StudentElasticSearchRepository(IElasticsearchProvider elasticsearchProvider) : base(elasticsearchProvider)
    {
    }

    // index 只能是小写
    protected override string IndexName => "Students20230701".ToLower();
}