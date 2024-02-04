using Microsoft.AspNetCore.Http;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;
using YaSha.DataManager.StandardAndPolicy.Dto;

namespace YaSha.DataManager.NewFamilyLibrary
{
   
    public class NewFamilyTreeAppService :
        CrudAppService<
            NewFamilyTree,
            NewFamilyTreeDto,
            Guid,
            PagedAndSortedResultRequestDto,
            NewFamilyTreeCreateDto
           >,
         INewFamilyTreeAppService
    {
       
        public readonly IRepository<NewFamilyTree, Guid> _treeRepository;
        private readonly ICurrentUser _currentUser;

    
        List<string> lists = new List<string> { "详图族", "物料", "文件" };
        public NewFamilyTreeAppService(IRepository<NewFamilyTree, Guid> repository, ICurrentUser currentUser)
            : base(repository)
        {
            _treeRepository = repository;
            _currentUser = currentUser;
        }

    

        public async Task<List<NewFamilyTreeDto>> GetTreeRoot()
        {
            var roots = await _treeRepository.GetListAsync();
            var dtos = ObjectMapper.Map<List<NewFamilyTree>, List<NewFamilyTreeDto>>(roots);
            dtos = SetHierarchy(dtos);
            return dtos;
        }
   
        public List<NewFamilyTreeDto> SetHierarchy(List<NewFamilyTreeDto> dtos)
        {
            List<NewFamilyTreeDto> newDtos = new List<NewFamilyTreeDto>();
            var parents = dtos.Where(e => e.ParentId == null).ToList();
            newDtos.AddRange(parents);

            foreach (var dto in newDtos)
            {
                GetChildren(dto, dtos);
            }
            return newDtos;
        }

        public NewFamilyTreeDto GetChildren(NewFamilyTreeDto dto, List<NewFamilyTreeDto> dtos)
        {
            bool includeShow = _currentUser.Name.Equals("admin") || _currentUser.Name.Equals("huangming1") ? true : false;
            dto.Children = dtos.Where(x => x.ParentId == dto.Id && (includeShow || !lists.Any(y => x.DisplayName.Contains(y)))).ToList();
            foreach (var item in dto.Children)
            {
                GetChildren(item, dtos);
            }
            return dto;
        }


        public async Task<List<NewFamilyTreeDto>> GetListAsync()
        {
            var roots = await _treeRepository.GetListAsync();
            return ObjectMapper.Map<List<NewFamilyTree>, List<NewFamilyTreeDto>>(roots);
        }

        public async Task<List<NewFamilyTreeDto>> CreateListAsync(List<NewFamilyTreeCreateDto> dtos)
        {
            var trees = ObjectMapper.Map<List<NewFamilyTreeCreateDto>, List<NewFamilyTree>>(dtos);
            await RecursionAdd(trees);
            return ObjectMapper.Map<List<NewFamilyTree>, List<NewFamilyTreeDto>>(trees);
        }

        public async Task DeleteListAsync(List<NewFamilyTreeDto> dtos)
        {
            var entities = new List<NewFamilyTree>();
            foreach (var item in dtos)
            {
                var entity = await _treeRepository.FindAsync(item.Id);
                if (entity != null)
                {
                    entities.Add(entity);
                }
            }
            await RecursionDelete(entities);

        }

        public async Task RecursionAdd(List<NewFamilyTree> trees)
        {
            if (trees.Count == 0) return;
            var parents = trees.Where(e => e.ParentId == null || trees.Any(x => !x.Id.Equals(e.ParentId))).ToList();
            await _treeRepository.InsertManyAsync(parents, true);
            parents.ForEach(e => trees.Remove(e));
            await RecursionAdd(trees);
        }

        public async Task RecursionDelete(List<NewFamilyTree> trees)
        {
            if (trees.Count == 0) return;
            var childs = trees.Where(e => trees.Find(x => x.ParentId.Equals(e.Id)) == null).ToList();
            await _treeRepository.DeleteManyAsync(childs, true);
            childs.ForEach(e => trees.Remove(e));
            await RecursionDelete(trees);
        }

        public async Task<ApiResultDto> UploadFile(IFormFile file)
        {
            bool success = false;
            var serverPath = "/ServerData/FileManagement/FamilyLib/Revit/";
            string excelPath = serverPath + Path.GetFileName(file.FileName);
            try
            {
                using (FileStream fileStream = new FileStream(excelPath, FileMode.Create, FileAccess.Write))
                {
                    await file.CopyToAsync(fileStream);
                }
                success = true;
            }
            catch (Exception ex) 
            {
                excelPath = ex.Message;
            }
            return new ApiResultDto()
            {
                Data = excelPath,
                Success = success,
            };
        }

    }
}
