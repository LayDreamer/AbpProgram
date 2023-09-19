namespace YaSha.DataManager.FreeSqlRepository.Tests.Users
{
  

    public class UserFreeSqlBasicRepository_Tests: DataManagerFreeSqlRepositoryTestBase
    {
        private readonly IUserFreeSqlBasicRepository _userFreeSqlBasicRepository;
        public UserFreeSqlBasicRepository_Tests()
        {
            _userFreeSqlBasicRepository = GetRequiredService<IUserFreeSqlBasicRepository>();
        }

        [Fact]
        public async Task Should_NotThrow_ListAsyncTest()
        {
            var result = await _userFreeSqlBasicRepository.GetListAsync();
            result.ShouldNotBeNull();
        }
    }

}
