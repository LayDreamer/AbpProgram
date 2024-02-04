using Volo.Abp.Application.Dtos;
using Volo.Abp.Users;
using YaSha.DataManager.NewFamilyLibrary.Repository;

namespace YaSha.DataManager.NewFamilyLibrary.Manager
{
    public class NewFamilyLibraryManager : DataManagerDomainService
    {
        private readonly INewFamilyTreeRepository _treeRepository;
        private readonly INewFamilyLibRepository _libRepository;
        private readonly ICurrentUser _currentUser;

        

        public NewFamilyLibraryManager(INewFamilyTreeRepository treeRepository, INewFamilyLibRepository libRepository,
            ICurrentUser currentUser)
        {
            _treeRepository = treeRepository;
            _libRepository = libRepository;
            _currentUser = currentUser;
        }


        public async Task<List<NewFamilyLib>> GetListByTreeIdAsync(List<Guid> categoryIds, string SearchValue, string SearchCode, string Sorting)
        {
            return await _libRepository.GetListByTreeIdAsync( categoryIds,  SearchValue,  SearchCode,  Sorting);
        }

        public async Task<List<NewFamilyLib>> GetById(Guid id)
        {
          return  await _libRepository.GetById(id);
        }


    }
}
