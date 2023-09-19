namespace YaSha.DataManager.CAP;

public static class DataManagerCapServiceCollectionExtensions
{
    public static ServiceConfigurationContext AddAbpCap(this ServiceConfigurationContext context, Action<CapOptions> capAction)
    {
        context.Services.Replace(ServiceDescriptor.Transient<IUnitOfWork, DataManagerCapUnitOfWork>());
        context.Services.Replace(ServiceDescriptor.Transient<UnitOfWork, DataManagerCapUnitOfWork>());
        context.Services.AddTransient<DataManagerCapUnitOfWork>();
        context.Services.AddCap(capAction);
        return context;
    }
}