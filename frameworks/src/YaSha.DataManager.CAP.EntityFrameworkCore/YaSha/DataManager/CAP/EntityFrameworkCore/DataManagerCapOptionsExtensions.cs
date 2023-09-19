// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class DataManagerCapOptionsExtensions
    {
        public static CapOptions SetCapDbConnectionString(this CapOptions options, string dbConnectionString)
        {
            options.RegisterExtension(new DataManagerEfCoreDbContextCapOptionsExtension
            {
                CapUsingDbConnectionString = dbConnectionString
            });
            
            return options;
        }
    }
}
