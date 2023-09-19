using YaSha.DataManager.Jobs;

namespace YaSha.DataManager.Extensions.Hangfire
{
    public static class RecurringJobsExtensions
    {
        public static void CreateRecurringJob(this ApplicationInitializationContext context)
        {
            RecurringJob.AddOrUpdate<TestJob>("测试Job", t => t.ExecuteAsync(), CronType.Minute(1),TimeZoneInfo.Local);
        }
    }
}