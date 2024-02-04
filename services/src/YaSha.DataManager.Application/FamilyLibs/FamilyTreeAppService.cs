using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using YaSha.DataManager.FamilyTrees;

namespace YaSha.DataManager.FamilyLibs
{
    public class FamilyTreeAppService :
        CrudAppService<
            FamilyTree,
            FamilyTreeDto,
            Guid,
            PagedAndSortedResultRequestDto,
            FamilyTreeCreateDto
            >,
        IFamilyTreeAppService
    {
        public readonly IRepository<FamilyTree, Guid> _repository;


        public FamilyTreeAppService(IRepository<FamilyTree, Guid> repository)
            : base(repository)
        {
            _repository = repository;
        }


        public async Task<List<FamilyTreeDto>> GetTreeListAsync()
        {
            var familyTrees = await _repository.GetListAsync();
            return ObjectMapper.Map<List<FamilyTree>, List<FamilyTreeDto>>(familyTrees);
        }

        public async Task<List<Guid>> GetCategoryTreeChildrenGuids(Guid guid)
        {
            List<Guid> guids = new();
            var allFamilyTrees = await _repository.GetListAsync();
            await GetTreeGuids(guids, guid, allFamilyTrees);
            return guids;
        }


        public async Task GetTreeGuids(List<Guid> guids, Guid guid, List<FamilyTree> allFamilyTrees)
        {
            var familyTrees = allFamilyTrees.Where(e => e.ParentId.Equals(guid)).ToList();
            foreach (var familyTree in familyTrees)
            {
                if (string.IsNullOrEmpty(familyTree.Id.ToString()))
                    continue;
                guids.Add(familyTree.Id);
                await GetTreeGuids(guids, familyTree.Id, allFamilyTrees);
            }
        }


        public async Task DeleteListAsync(List<FamilyTreeDto> dtos)
        {
            var entities = new List<FamilyTree>();
            foreach (var item in dtos)
            {
                var entity = await _repository.FindAsync(item.Id);
                if (entity != null)
                {
                    entities.Add(entity);
                }
            }
            await RecursionDelete(entities);
        }

        public async Task<List<FamilyTreeDto>> CreateListAsync(List<FamilyTreeCreateDto> dtos)
        {
            var trees = ObjectMapper.Map<List<FamilyTreeCreateDto>, List<FamilyTree>>(dtos);
            await RecursionAdd(trees);
            return ObjectMapper.Map<List<FamilyTree>, List<FamilyTreeDto>>(trees);
        }


        public async Task RecursionAdd(List<FamilyTree> trees)
        {
            if (trees.Count == 0) return;
            var parents = trees.Where(e => e.ParentId == null || trees.Any(x => !x.Id.Equals(e.ParentId))).ToList();
            await _repository.InsertManyAsync(parents, true);
            parents.ForEach(e => trees.Remove(e));
            await RecursionAdd(trees);
        }

        public async Task RecursionDelete(List<FamilyTree> trees)
        {
            if (trees.Count == 0) return;
            var childs = trees.Where(e => trees.Find(x => x.ParentId.Equals(e.Id)) == null).ToList();
            await _repository.DeleteManyAsync(childs, true);
            childs.ForEach(e => trees.Remove(e));
            await RecursionDelete(trees);
        }



    }
}
