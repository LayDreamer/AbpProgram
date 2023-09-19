using AutoMapper;
using YaSha.DataManager.ProductInventory.AggregateRoot;
using YaSha.DataManager.ProductInventory.Dto;
using YaSha.DataManager.ProductRetrieval.Dto;

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
    }
}