namespace YaSha.DataManager
{
    /* Inherit your application services from this class.
     */
    public abstract class DataManagerAppService : ApplicationService
    {
        protected DataManagerAppService()
        {
            LocalizationResource = typeof(DataManagerResource);
        }
    }
}
