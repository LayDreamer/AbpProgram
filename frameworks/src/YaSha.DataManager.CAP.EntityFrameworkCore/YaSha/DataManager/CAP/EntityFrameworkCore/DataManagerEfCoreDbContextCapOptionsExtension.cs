namespace YaSha.DataManager.CAP.EntityFrameworkCore;

public class DataManagerEfCoreDbContextCapOptionsExtension : ICapOptionsExtension
{
    public string CapUsingDbConnectionString { get; init; }
    
    public void AddServices(IServiceCollection services)
    {
        services.Configure<DataManagerEfCoreDbContextCapOptions>(options =>
        {
            options.CapUsingDbConnectionString = CapUsingDbConnectionString;
        });
    }
}