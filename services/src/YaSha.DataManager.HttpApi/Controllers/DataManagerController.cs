namespace YaSha.DataManager.Controllers
{
    /* Inherit your controllers from this class.
     */
    public abstract class DataManagerController : AbpController
    {
        protected DataManagerController()
        {
            LocalizationResource = typeof(DataManagerResource);
        }
    }
}