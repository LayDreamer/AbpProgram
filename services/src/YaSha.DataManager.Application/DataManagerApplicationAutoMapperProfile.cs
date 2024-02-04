using YaSha.DataManager.FamilyLibs;
using YaSha.DataManager.FamilyTrees;
using YaSha.DataManager.MeasuringExcels;
using YaSha.DataManager.NewFamilyLibrary;
using YaSha.DataManager.ProcessingLists;
using YaSha.DataManager.ProductRetrieval;
using YaSha.DataManager.ProductRetrieval.AggregateRoot;
using YaSha.DataManager.ProductRetrieval.Dto;
using YaSha.DataManager.SaleOderList;
using YaSha.DataManager.StandardAndPolicy;
using YaSha.DataManager.StandardAndPolicy.AggregateRoot;

namespace YaSha.DataManager
{
    public class DataManagerApplicationAutoMapperProfile : Profile
    {
        public DataManagerApplicationAutoMapperProfile()
        {
            CreateMap<FamilyTree, FamilyTreeDto>();
            CreateMap<FamilyLib, FamilyLibDto>();

            CreateMap<FamilyTreeCreateDto, FamilyTree>();
            CreateMap<FamilyLibCreateDto, FamilyLib>();

            CreateMap<ProcessingList, ProcessingListDto>();
            CreateMap<ProcessingListCreateDto, ProcessingList>();

            CreateMap<ProcessingData, ProcessingDataDto>();
            CreateMap<ProcessingDataCreateDto, ProcessingData>();

            CreateMap<MeasuringExcel, MeasuringExcelDto>();
            CreateMap<MeasuringExcelCreateDto, MeasuringExcel>();

            CreateMap<SaleOder, SaleOderDto>();
            CreateMap<SaleOderCreateDto, SaleOder>();

            CreateMap<NewFamilyTree, NewFamilyTreeDto>();
            CreateMap<NewFamilyLib, NewFamilyLibDto>();

            CreateMap<NewFamilyTreeCreateDto, NewFamilyTree>();
            CreateMap<NewFamilyLibCreateDto, NewFamilyLib>();

        }
    }
}
