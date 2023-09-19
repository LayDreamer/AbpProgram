using EasyAbp.Abp.Trees;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using YaSha.DataManager.Common;
using YaSha.DataManager.FamilyTrees;

namespace YaSha.DataManager.FamilyLibs
{
    public class FamilyLibAppService :
       CrudAppService<
           FamilyLib,
           FamilyLibDto,
           Guid,
           PagedAndSortedResultRequestDto>,
       IFamilyLibAppService
    {
        protected readonly ITreeRepository<FamilyLib> _repository;
        protected readonly ITreeRepository<FamilyTree> _treeRepository;

        public FamilyLibAppService(ITreeRepository<FamilyLib> repository, ITreeRepository<FamilyTree> treeRepository)
            : base(repository)
        {
            _repository = repository;
            _treeRepository = treeRepository;
        }


        public async Task<PagedResultDto<FamilyLibDto>> GetListByIdAsync(OrderNotificationSearchDto input)
        {
            if (input.Key == null || input.Key.Trim() == "null")
            {
                return null;
                //return new PagedResultDto<FamilyLibDto> { Items = null, TotalCount = 0 };
            }
            //query = familyLibs.AsQueryable();

            #region 检查当前Guid并获取相关节点Guid

            Guid guid = Guid.Empty;
            Guid.TryParse(input.Key, out guid);
            if (guid == Guid.Empty)
                return null;
            List<Guid> categoryIds = new() { guid };
            categoryIds.AddRange(await new FamilyTreeAppService(_treeRepository).GetCategoryTreeChildrenGuids(guid));

            #endregion

            //IQueryable<FamilyLib> query = await CreateFilteredQueryAsync(input);
            //List<FamilyLib> familyList = await _repository.GetListAsync();
            IQueryable<FamilyLib> query = await _repository.GetQueryableAsync();

            List<Guid> productIds = new();
            List<FamilyLib> familyLibs = new();
            IQueryable<FamilyLib> familyLibsQuery = familyLibs.AsQueryable();
            //分类过滤
            if (categoryIds.Count != 0)
            {
                query = query.Where(x => categoryIds.Any(e => e == x.CategoryId));
            }

            bool isCollection = true;
            //筛选匹配的产品/模块/物料名称
            if (!string.IsNullOrEmpty(input.SearchValue) && !string.IsNullOrEmpty(input.SearchCode))
            {
                familyLibsQuery = query.Where(e => e.DisplayName.Contains(input.SearchValue) && e.Number.Contains(input.SearchCode));
            }
            else if (!string.IsNullOrEmpty(input.SearchValue))
            {
                familyLibsQuery = query.Where(e => e.DisplayName.Contains(input.SearchValue));
            }
            else if (!string.IsNullOrEmpty(input.SearchCode))
            {
                familyLibsQuery = query.Where(e => e.Number.Contains(input.SearchCode));
            }
            else
            {
                isCollection = false;
            }

            if (isCollection)
            {
                foreach (var item in familyLibsQuery)
                {
                    if (item.ParentId == null && item.Description.Trim() != "X")
                    {
                        productIds.Add(item.Id);
                    }
                    else
                    {
                        var parentProduct = await FamilyLibUtils.GetTopParent(_repository, item);
                        if (parentProduct != null && !productIds.Contains(parentProduct.Id))
                        {
                            productIds.Add(parentProduct.Id);
                        }
                    }
                }
                if (productIds.Count > 0)
                {
                    query = query.Where(x => productIds.Any(e => e == x.Id));
                }
                else
                {
                    return new PagedResultDto<FamilyLibDto> { Items = null, TotalCount = 0 };
                }
            }


            //过滤显示产品
            //query = query.Where(e => e.Description == "C");
            query = query.Where(e => e.ParentId == null && e.Description.Trim() != "X");

            var totalCount = await AsyncExecuter.CountAsync(query);

            query = ApplySorting(query, input);

            query = query.Skip((input.SkipCount - 1) * input.MaxResultCount).Take(input.MaxResultCount);

            var entities = await AsyncExecuter.ToListAsync(query);

            var entityDtos = await MapToGetListOutputDtosAsync(entities);

            return new PagedResultDto<FamilyLibDto>(
                totalCount,
                entityDtos
            );
        }

        public async Task<List<FamilyLibDto>> GetSubElemntsGuids(Guid guid)
        {
            List<FamilyLib> subLibs = new List<FamilyLib>();

            var find = await _repository.FindAsync(guid);

            if(find == null)
            {
                return new List<FamilyLibDto>();
            }

            if (find.Description.ToLower() != "c")
            {
                find.Children.Clear();
                return new List<FamilyLibDto>() { ObjectMapper.Map<FamilyLib, FamilyLibDto>(find) };
            }

            var familyLibs = await _repository.GetChildrenAsync(guid);

            if (familyLibs.Count > 0)
            {
                familyLibs.ForEach(e => subLibs.Add(e));
            }
            foreach (var childrenFamily in familyLibs)
            {
                if (string.IsNullOrEmpty(childrenFamily.Id.ToString()))
                    continue;
                subLibs.AddRange(await _repository.GetChildrenAsync(childrenFamily.Id));
            }
            subLibs.Add(find);
            var result = ObjectMapper.Map<List<FamilyLib>, List<FamilyLibDto>>(subLibs);
            return result;
        }

        public async Task<List<FamilyLibDto>> GetFamilyModuleList(Guid guid)
        {
            List<FamilyLibDto> familyLibDtos = new();
            List<FamilyLibDto> resultDto = await GetSubElemntsGuids(guid);
            var familyLibsGroup = resultDto.OrderBy(e => e.Description).GroupBy(e => e.Description);
            foreach (var item in familyLibsGroup)
            {
                familyLibDtos.AddRange(item.OrderBy(e => (e.DisplayName, e.Length)));
            }
            return familyLibDtos;
        }
    }

    public class FamilyLibUtils
    {
        /// <summary>
        /// 获取顶层父节点
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="familyLib"></param>
        /// <returns></returns>
        public static async Task<FamilyLib> GetTopParent(ITreeRepository<FamilyLib> repository, FamilyLib familyLib)
        {
            FamilyLib parentFamily = familyLib;
            if (familyLib == null)
                return null;
            if (familyLib.ParentId != Guid.Empty && familyLib.ParentId != null)
            {
                parentFamily = await repository.GetAsync(familyLib.ParentId.Value);
                if (parentFamily != null && parentFamily.Description != "C")
                {
                    parentFamily = await GetTopParent(repository, parentFamily);
                }
            }
            return parentFamily;
        }
    }
}
