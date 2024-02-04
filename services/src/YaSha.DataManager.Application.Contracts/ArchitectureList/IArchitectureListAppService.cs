using Microsoft.AspNetCore.Http;
using System.Text.Json.Nodes;
using Volo.Abp.Application.Dtos;
using YaSha.DataManager.ArchitectureList.Dto;
using YaSha.DataManager.StandardAndPolicy.Dto;

namespace YaSha.DataManager.ArchitectureList;

public interface IArchitectureListAppService : IApplicationService
{
    Task<List<ArchitectureListModuleDto>> ImportFromExcel(string system, bool isProcess, List<ArchitectureListModuleCreateDto> input);

    Task<ApiResultDto> InsertFile(Guid id, ArchitectureListFileStatus status, List<IFormFile> files);

    Task<ApiResultDto> InsertModuleFiles(Guid moduleId, List<Guid> fileIds);

    Task<PagedResultDto<ArchitectureListModuleDto>> Page(ArchitectureSearchDto input);

    Task<List<ArchitectureListTreeDto>> GetTreeRoot();

    Task<ArchitectureListTreeDto> UpdateTreeNode(ArchitectureListTreeDto input);

    Task<List<ArchitectureListFileDto>> GetFileList(Guid treeId, ArchitectureListFileStatus status);

    Task<List<ArchitectureListModuleDto>> UpdateStatus(ArchitectureUpdateStatus input);

    Task<ArchitectureListModuleDto> InsertModule(ArchitectureListModuleDto input);

    Task<List<ArchitectureListModuleDto>> CopyModules(List<Guid> ids);

    Task<ArchitectureListModuleDto> Update(ArchitectureListModuleDto input);

    Task<ApiResultDto> Delete(List<Guid> id);

    Task<ApiResultDto> DeleteFiles(List<Guid> id);

    Task<object> RevitSearch(List<string> names);

    Task<object> CadSearch(List<string> names);
}