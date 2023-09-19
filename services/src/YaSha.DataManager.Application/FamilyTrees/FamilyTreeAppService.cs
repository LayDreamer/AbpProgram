using EasyAbp.Abp.Trees;
using MySqlX.XDevAPI.Common;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

namespace YaSha.DataManager.FamilyTrees
{
    public class FamilyTreeAppService :
        CrudAppService<
            FamilyTree,
            FamilyTreeDto,
            Guid,
            PagedAndSortedResultRequestDto>,
        IFamilyTreeAppService
    {
        public readonly ITreeRepository<FamilyTree> _repository;
        public FamilyTreeAppService(ITreeRepository<FamilyTree> treeRepository)
            : base(treeRepository)
        {
            _repository = treeRepository;
        }

        public async Task<PagedResultDto<FamilyTreeDto>> GetChildrenListAsync(Guid parentId)
        {
            var familyChildrenList = await _repository.GetChildrenAsync(parentId);
            var count = familyChildrenList.Count;
            var result = ObjectMapper.Map<List<FamilyTree>, List<FamilyTreeDto>>(familyChildrenList);
            return new PagedResultDto<FamilyTreeDto> { Items = result, TotalCount = count };
        }

        public async Task<PagedResultDto<FamilyTreeDto>> GetTreeListAsync()
        {
            var familyTrees = await _repository.GetListAsync();
            var count = familyTrees.Count;
            var result = ObjectMapper.Map<List<FamilyTree>, List<FamilyTreeDto>>(familyTrees);
            return new PagedResultDto<FamilyTreeDto> { Items = result, TotalCount = count };
        }

        public async Task<List<Guid>> GetCategoryTreeChildrenGuids(Guid guid)
        {
            List<Guid> guids = new();
            await GetTreeGuids(guids, guid);
            return guids;
        }


        public async Task GetTreeGuids(List<Guid> guids, Guid guid)
        {
            var familyTrees = await _repository.GetChildrenAsync(guid);
            foreach (var familyTree in familyTrees)
            {
                if (string.IsNullOrEmpty(familyTree.Id.ToString()))
                    continue;
                guids.Add(familyTree.Id);
                await GetTreeGuids(guids, familyTree.Id);
            }
        }
    }
}
