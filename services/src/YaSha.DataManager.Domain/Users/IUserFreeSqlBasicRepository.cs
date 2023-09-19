namespace YaSha.DataManager.Users
{
    public interface IUserFreeSqlBasicRepository
    {
        Task<List<UserOutput>> GetListAsync();
    }
}
