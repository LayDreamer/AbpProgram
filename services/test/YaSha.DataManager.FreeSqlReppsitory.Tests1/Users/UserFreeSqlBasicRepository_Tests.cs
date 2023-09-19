using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace YaSha.DataManager.Users
{
  

    public class UserFreeSqlBasicRepository_Tests: DataManagerFreeSqlRepositoryTestBase
    {
        //private readonly IUserFreeSqlBasicRepository _userFreeSqlBasicRepository;
        //public UserFreeSqlBasicRepositoryTest()
        //{
        //    _userFreeSqlBasicRepository = GetRequiredService<IUserFreeSqlBasicRepository>();
        //}

        [Fact]
        public void Should_NotThrow_ListAsyncTest()
        {
            //var result = await _userFreeSqlBasicRepository.GetListAsync();
            var s = 1;
        }
    }

}
