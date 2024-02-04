using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;
using YaSha.DataManager.NewFamilyLibrary.Manager;
using YaSha.DataManager.ProductInventory.Dto;

namespace YaSha.DataManager.NewFamilyLibrary
{
    public class NewFamilyLibAppService :
        CrudAppService<
            NewFamilyLib,
            NewFamilyLibDto,
            Guid,
            PagedAndSortedResultRequestDto,
             NewFamilyLibCreateDto>,
        INewFamilyLibAppService
    {
        private readonly NewFamilyLibraryManager _newFamilyLibraryManager;
        protected readonly IRepository<NewFamilyTree, Guid> _treeRepository;
        protected readonly IRepository<NewFamilyLib, Guid> _libRepository;
        private readonly ICurrentUser _currentUser;

        List<string> lists = new List<string> { "详图族", "物料", "文件" };
        public NewFamilyLibAppService(NewFamilyLibraryManager newFamilyLibraryManager, IRepository<NewFamilyTree, Guid> treeRepository, IRepository<NewFamilyLib, Guid> repository, ICurrentUser currentUser)
            : base(repository)
        {
            _newFamilyLibraryManager = newFamilyLibraryManager;
            _treeRepository = treeRepository;
            _libRepository = repository;
            _currentUser = currentUser;
        }


        public async Task<PagedResultDto<NewFamilyLibDto>> GetListByTreeIdAsync(NewFamilyLibSearchDto input)
        {
            var categoryIds = new List<Guid>();
            if (!string.IsNullOrEmpty(input.Key))
            {
                var trees = await _treeRepository.GetListAsync();
                var dtos = ObjectMapper.Map<List<NewFamilyTree>, List<NewFamilyTreeDto>>(trees);
                categoryIds = GetChildrenIds(Guid.Parse(input.Key), dtos);
            }
            else
            {
                return new PagedResultDto<NewFamilyLibDto>(
                    0,
                    new List<NewFamilyLibDto>()
                );
            }
            var libs = await _newFamilyLibraryManager.GetListByTreeIdAsync(categoryIds, input.SearchValue, input.SearchCode, input.Sorting);
            var totalDtos = ObjectMapper.Map<List<NewFamilyLib>, List<NewFamilyLibDto>>(libs);
            var totalCount = totalDtos.Count();
            var resultsDtos = totalDtos.Skip((input.SkipCount - 1) * input.MaxResultCount).Take(input.MaxResultCount)
                .ToList();

            resultsDtos.ForEach(e => e.Children = null);

            return new PagedResultDto<NewFamilyLibDto>(
                totalCount,
                resultsDtos
            );
        }
        public List<Guid> GetChildrenIds(Guid id, List<NewFamilyTreeDto> dtos)
        {
            bool includeShow = _currentUser.Name.Equals("admin") || _currentUser.Name.Equals("huangming1") ? true : false;
            List<Guid> ids = new List<Guid>();
            ids.Add(id);
            var childs = dtos.Where(e => e.ParentId == id && (includeShow || !lists.Any(y => e.DisplayName.Contains(y)))).ToList();

            foreach (var item in childs)
            {
                ids.AddRange(GetChildrenIds(item.Id, dtos));
            }
            return ids;
        }
        public async Task<List<NewFamilyLibDto>> GetFamilyInfo(Guid id)
        {
            var libs = await _newFamilyLibraryManager.GetById(id);
            var dtos = ObjectMapper.Map<List<NewFamilyLib>, List<NewFamilyLibDto>>(libs);
            foreach (var item in dtos)
            {
                item.Number = item.Type;//在前端中，模块显示型号，物料显示编码
                item.Children.ForEach(e => e.Children = null);
                item.Children = item.Children.OrderBy(e => e.Description).ToList();
            }
            return dtos;
        }

        public async Task<ImageResponseDto> UploadFamilyLibImage(ImageFileDto dto)
        {
            var path = "/ServerData/FileManagement/FamilyLib/Revit";

            var imageServerPath = "";
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                var name = Guid.NewGuid().ToString() + Path.GetExtension(dto.File.FileName);
                var imageLocalPath = path + "\\" + name;
                using (FileStream fileStream = new FileStream(imageLocalPath, FileMode.Create, FileAccess.Write))
                {
                    await dto.File.CopyToAsync(fileStream);
                }
                imageServerPath = "https://bds.chinayasha.com/bdsfileservice/FamilyLib/Revit/" + name;

                var lib = await _libRepository.FindAsync(dto.Id);

                if (lib != null)
                {
                    lib.ImagePath = imageServerPath;
                    lib.UploadUser = _currentUser.UserName;
                    await _libRepository.UpdateAsync(lib);
                }
            }
            catch (Exception e)
            {
                return new ImageResponseDto()
                {
                    Code = -1,
                    Error = e.Message,
                };
            }

            return new ImageResponseDto()
            {
                Code = 1,
                ServerPath = imageServerPath,
            };
        }

        public async Task<List<NewFamilyLibDto>> CreateListAsync(List<NewFamilyLibCreateDto> dtos)
        {
            var libs = ObjectMapper.Map<List<NewFamilyLibCreateDto>, List<NewFamilyLib>>(dtos);
            await RecursionAdd(libs);
            return ObjectMapper.Map<List<NewFamilyLib>, List<NewFamilyLibDto>>(libs);
        }

        public async Task RecursionAdd(List<NewFamilyLib> libs)
        {
            if (libs.Count == 0) return;
            var parents = libs.Where(e => e.ParentId == null || libs.Find(x => x.Id.Equals(e.ParentId)) == null).ToList();
            await _libRepository.InsertManyAsync(parents, true);
            parents.ForEach(e => libs.Remove(e));
            await RecursionAdd(libs);
        }
        public async Task DeleteListAsync(List<NewFamilyLibDto> dtos)
        {
            var allFamilyLibs = await _libRepository.GetListAsync();
            var entities = new List<NewFamilyLib>();
            List<NewFamilyLib> childs = new List<NewFamilyLib>();

            foreach (var lib in dtos)
            {
                var entity = allFamilyLibs.Find(x => x.Id.Equals(lib.Id));
                if (entity != null)
                {
                    entities.Add(entity);
                    GetChildren(childs, lib.Id, allFamilyLibs);
                }
            }
            await RecursionDelete(childs);
            await _libRepository.DeleteManyAsync(entities, true);
        }
        public async Task RecursionDelete(List<NewFamilyLib> libs)
        {
            if (libs.Count == 0) return;
            var childs = libs.Where(e => libs.Find(x => x.ParentId.Equals(e.Id)) == null).ToList();
            await _libRepository.DeleteManyAsync(childs, true);
            childs.ForEach(e => libs.Remove(e));
            await RecursionDelete(libs);
        }
        public void GetChildren(List<NewFamilyLib> childs, Guid guid, List<NewFamilyLib> allFamilyLibs)
        {
            var familyLibs = allFamilyLibs.Where(e => e.ParentId.Equals(guid)).ToList();
            foreach (var familyLib in familyLibs)
            {
                childs.Add(familyLib);
                GetChildren(childs, familyLib.Id, allFamilyLibs);
            }
        }
        public async Task<List<NewFamilyLibDto>> GetListAsync()
        {
            var libs = await _libRepository.GetListAsync();
            return ObjectMapper.Map<List<NewFamilyLib>, List<NewFamilyLibDto>>(libs);
        }

        public async Task<NewFamilyLibDto> ArchSearch(string name, string type, string processMode)
        {
            var allFamilyLibs = await _libRepository.GetListAsync();
            var find = allFamilyLibs.FirstOrDefault(e => e.DisplayName.Trim().Contains(name.Trim()) &&
                                                         e.Type.Trim().Equals(type.Trim()) && 
                                                         e.ProcessMode.Trim().Equals(processMode.Trim()));
            return find == null? null:ObjectMapper.Map<NewFamilyLib, NewFamilyLibDto>(find);
        }
    }
}
