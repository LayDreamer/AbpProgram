using YaSha.DataManager.FreeSqlReppsitory.Tests;
using YaSha.DataManager.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YaSha.DataManager
{
    public abstract class DataManagerFreeSqlRepositoryTestBase: DataManagerTestBase<DataManagerFreeSqlRepositoryTestModule>
    {
        public DataManagerFreeSqlRepositoryTestBase()
        {
            ServiceProvider.InitializeLocalization();
        }
    }
}
