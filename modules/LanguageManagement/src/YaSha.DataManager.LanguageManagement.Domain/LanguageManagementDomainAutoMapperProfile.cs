using YaSha.DataManager.LanguageManagement.Languages;
using YaSha.DataManager.LanguageManagement.LanguageTexts;

namespace YaSha.DataManager.LanguageManagement
{
    public class LanguageManagementDomainAutoMapperProfile : Profile
    {
        public LanguageManagementDomainAutoMapperProfile()
        {
            CreateMap<Language, LanguageDto>();
            CreateMap<LanguageText, LanguageTextDto>();
            CreateMap<Language, LanguageInfo>();
        }
    }
}