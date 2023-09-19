using YaSha.DataManager.Localization;
using Microsoft.Extensions.Localization;
using Shouldly;
using Volo.Abp.Localization;
using Xunit;

namespace YaSha.DataManager
{
    public sealed class DataManagerLocalizationTests : DataManagerLocalizationTestBase
    {
        private readonly IStringLocalizer<DataManagerLocalizationResource> _stringLocalizer;

        public DataManagerLocalizationTests()
        {
            _stringLocalizer = GetRequiredService<IStringLocalizer<DataManagerLocalizationResource>>();
        }

        [Fact]
        public void Test()
        {
            using (CultureHelper.Use("en"))
            {
                _stringLocalizer["Welcome"].Value.ShouldBe("Welcome");
                _stringLocalizer[DataManagerLocalizationErrorCodes.ErrorCode100001].Value.ShouldBe("The start page must be greater than or equal to 1");
                _stringLocalizer[DataManagerLocalizationErrorCodes.ErrorCode100003,"Name"].Value.ShouldBe("Name can not be empty");
            }

            using (CultureHelper.Use("zh-Hans"))
            {
                _stringLocalizer["Welcome"].Value.ShouldBe("欢迎");
                _stringLocalizer[DataManagerLocalizationErrorCodes.ErrorCode100001].Value.ShouldBe("起始页必须大于等于1");
                _stringLocalizer[DataManagerLocalizationErrorCodes.ErrorCode100003,"Name"].Value.ShouldBe("Name不能为空");
            }
        }
    }
}