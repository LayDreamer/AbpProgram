using AutoMapper;
using YaSha.DataManager.ArchitectureList.AggregateRoot;
using YaSha.DataManager.ArchitectureList.Dto;
using YaSha.DataManager.ListProcessing;
using YaSha.DataManager.MaterialManage.AggregateRoot;
using YaSha.DataManager.MaterialManage.Dto;
using YaSha.DataManager.ProcessingLists;
using YaSha.DataManager.ProductInventory.AggregateRoot;
using YaSha.DataManager.ProductInventory.Dto;
using YaSha.DataManager.ProductRetrieval;
using YaSha.DataManager.ProductRetrieval.AggregateRoot;
using YaSha.DataManager.ProductRetrieval.Dto;
using YaSha.DataManager.StandardAndPolicy;
using YaSha.DataManager.StandardAndPolicy.AggregateRoot;
using YaSha.DataManager.StandardAndPolicy.Dto;


namespace YaSha.DataManager;

public class DataManagerDomainAutoMapperProfile : Profile
{
    public DataManagerDomainAutoMapperProfile()
    {
        CreateMap<ProductInventTree, ProductInventoryTreeDto>();

        CreateMap<ProductInventoryProductCreateDto, ProductInventProduct>();
        CreateMap<ProductInventProduct, ProductInventoryProductCreateDto>();
        CreateMap<ProductInventProduct, ProductInventoryProductDto>();
        CreateMap<ProductInventProduct, ProductInventoryEditDto>();
        CreateMap<ProductInventProduct, ProductRetrievalDto>();
        CreateMap<ProductInventoryEditDto, ProductInventProduct>();

        CreateMap<ProductInventoryModuleCreateDto, ProductInventModule>();
        CreateMap<ProductInventModule, ProductInventoryModuleCreateDto>();
        CreateMap<ProductInventModule, ProductInventoryModuleDto>();
        CreateMap<ProductInventModule, ProductInventoryEditDto>();
        CreateMap<ProductInventModule, ProductRetrievalDto>();
        CreateMap<ProductInventoryEditDto, ProductInventModule>();

        CreateMap<ProductInventoryMaterialCreateDto, ProductInventMaterial>();
        CreateMap<ProductInventMaterial, ProductInventoryMaterialCreateDto>();
        CreateMap<ProductInventMaterial, ProductInventoryMaterialDto>();
        CreateMap<ProductInventMaterial, ProductInventoryEditDto>();
        CreateMap<ProductInventMaterial, ProductRetrievalDto>();
        CreateMap<ProductInventoryEditDto, ProductInventMaterial>();

        CreateMap<MaterialInventoryCreateDto, MaterialInventory>();
        CreateMap<MaterialInventory, MaterialInventoryDto>();

        CreateMap<ProjectInfoCreateDto, ProjectInfo>();
        CreateMap<ProjectInfo, ProjectInfoDto>();

        CreateMap<StandardAndPolicyTree, StandardAndPolicyTreeDto>();
        CreateMap<StandardAndPolicyLib, StandardAndPolicyLibDto>();
        CreateMap<StandardAndPolicyLibDto, StandardAndPolicyLib>();
        CreateMap<StandardAndPolicyLib, StandardAndPolicyCardDto>();
        CreateMap<StandardAndPolicyTheme, StandardAndPolicyThemeDto>();
        CreateMap<StandardAndPolicyThemeDto, StandardAndPolicyTheme>();
        CreateMap<StandardAndPolicyCollect, StandardAndPolicyCollectDto>();

        CreateMap<ListProcessing.ListProcessing, ListProcessingDto>();

        CreateMap<ArchitectureListTree, ArchitectureListTreeDto>();
        CreateMap<ArchitectureListModuleCreateDto, ArchitectureListModule>();
        CreateMap<ArchitectureListModule, ArchitectureListModuleCreateDto>();
        CreateMap<ArchitectureListModule, ArchitectureListModuleDto>();
        CreateMap<ArchitectureListModuleDto, ArchitectureListModule>();
        CreateMap<ArchitectureListMaterialCreateDto, ArchitectureListMaterial>();
        CreateMap<ArchitectureListMaterial, ArchitectureListMaterialCreateDto>();
        CreateMap<ArchitectureListMaterial, ArchitectureListMaterialDto>();
        CreateMap<ArchitectureListMaterialDto, ArchitectureListMaterial>();
        CreateMap<ArchitectureListFile, ArchitectureListFileDto>();

        CreateMap<MaterialManageDto, MaterialManageInfo>();
        CreateMap<MaterialManageInfo, MaterialManageDto>();
        CreateMap<MaterialManageInfo, MaterialManageFullDto>();
    }
}