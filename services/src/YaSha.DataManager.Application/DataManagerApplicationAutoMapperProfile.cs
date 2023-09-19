using YaSha.DataManager.FamilyLibs;
using YaSha.DataManager.FamilyTrees;

namespace YaSha.DataManager
{
    public class DataManagerApplicationAutoMapperProfile : Profile
    {
        public DataManagerApplicationAutoMapperProfile()
        {
            CreateMap<FamilyTree, FamilyTreeDto>();
            CreateMap<FamilyLib, FamilyLibDto>();
        }
    }
}
