using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.ObjectMapping;
using YaSha.DataManager.ArchitectureList.AggregateRoot;
using YaSha.DataManager.ArchitectureList.Dto;
using YaSha.DataManager.ArchitectureList.Repository;
using YaSha.DataManager.ProductInventory;
using YaSha.DataManager.ProductInventory.AggregateRoot;
using YaSha.DataManager.ProductInventory.Dto;
using static Volo.Abp.Identity.Settings.IdentitySettingNames;

namespace YaSha.DataManager.ArchitectureList.Manager;

public class ArchitectureListManager : DataManagerDomainService
{
    private readonly IArchitectureListTreeRepository _treeRepository;
    private readonly IArchitectureListModuleRepository _moduleRepository;
    private readonly IArchitectureListMaterialRepository _materialRepository;
    private readonly IArchitectureListFileRepository _fileRepository;
    private readonly IArchitectureListModuleFileRepository _moduleFileRepository;

    public ArchitectureListManager(IArchitectureListTreeRepository treeRepository, IArchitectureListModuleRepository moduleRepository, IArchitectureListMaterialRepository materialRepository, IArchitectureListFileRepository fileRepository, IArchitectureListModuleFileRepository moduleFile)
    {
        _treeRepository = treeRepository;
        _moduleRepository = moduleRepository;
        _materialRepository = materialRepository;
        _fileRepository = fileRepository;
        _moduleFileRepository = moduleFile;
    }


    #region 增

    public async Task<ArchitectureListModuleDto> Insert(string userName, Guid id, ArchitectureListModuleCreateDto dto)
    {
        await Delete(id, dto.Name, dto.Model, dto.ProcessingMode);
        var module = ObjectMapper.Map<ArchitectureListModuleCreateDto, ArchitectureListModule>(dto);
        module.CreateUser = userName;
        module.SetParentId(id);
        var insert = await _moduleRepository.InsertAsync(module, true);
        var insertDto = ObjectMapper.Map<ArchitectureListModule, ArchitectureListModuleDto>(insert);
        return insertDto;
    }

    public async Task<List<ArchitectureListModuleDto>> Insert(string userName, Guid id, string path, bool isProcess, List<ArchitectureListModuleCreateDto> dto)
    {
        var module = ObjectMapper.Map<List<ArchitectureListModuleCreateDto>, List<ArchitectureListModule>>(dto);
        foreach (var item in module)
        {
            await Delete(id, item.Name, item.Model, item.ProcessingMode);
            item.Path = path;
            item.CreateUser = userName;
            item.SetParentId(id);
            if (isProcess)
            {
                //从产品模块中找
                var find = await _moduleRepository.FindByModel(item.Model, item.System);
                if (find != null)
                {
                    foreach (var material in find.Materials)
                    {
                        item.Materials.Add(material.Clone(item.Id));
                    }
                }
            }

            if (!string.IsNullOrEmpty(item.Optional))
            {
                dynamic jsonResponse = JsonConvert.DeserializeObject(item.Optional);
                foreach (var obj in jsonResponse)
                {
                    var bind = ((string)obj["value"]).Split("+");
                    if (bind.Length > 0)
                    {
                        var tmp = new List<string>();
                        foreach (var str in bind)
                        {
                            var o = item.Materials.FirstOrDefault(x => x.OptionalSerial == str);
                            if (o != null)
                            {
                                tmp.Add(o.Id.ToString());
                            }
                        }

                        obj["value"] = string.Join('+', tmp);
                    }
                }

                item.Optional = JsonConvert.SerializeObject(jsonResponse);
            }
        }

        await _moduleRepository.InsertManyAsync(module, true);
        var insertDto = ObjectMapper.Map<List<ArchitectureListModule>, List<ArchitectureListModuleDto>>(module);
        return insertDto;
    }

    public async Task<List<ArchitectureListModuleDto>> ImportFromExcel(string userName, string system, bool isProcess, List<ArchitectureListModuleCreateDto> input)
    {
        var results = new List<ArchitectureListModuleDto>();
        var tree = await _treeRepository.GetTreeByName(system, false);
        if (tree == null)
        {
            throw new BusinessException(ProductInventoryErrorCode.ErrorSystem);
        }

        var path = await _treeRepository.GetTreePath(tree.Id);

        return await Insert(userName, tree.Id, path, isProcess, input);
    }

    public async Task<ArchitectureListFileDto> InsertFile(string name, string path, string path1, Guid id, ArchitectureListFileStatus status)
    {
        var file = new ArchitectureListFile()
        {
            FileName = name,
            FilePath = path,
            FileEncryptionPath = path1,
            Type = status,
            TreeId = id,
        };
        var insert = await _fileRepository.InsertAsync(file, true);
        return ObjectMapper.Map<ArchitectureListFile, ArchitectureListFileDto>(insert);
    }

    public async Task<List<ArchitectureListFileDto>> InsertModuleFiles(Guid moduleId, List<Guid> fileIds)
    {
        await _moduleFileRepository.InsertAndUpdate(moduleId, fileIds);
        var files = (await _moduleFileRepository.FindByModuleId(moduleId))
            .Select(x => x.File).ToList();
        return ObjectMapper.Map<List<ArchitectureListFile>, List<ArchitectureListFileDto>>(files);
    }


    public async Task<ArchitectureListModuleDto> InsertModule(string createUser, ArchitectureListModuleDto dto)
    {
        if (string.IsNullOrEmpty(dto.System))
        {
            throw new BusinessException(ProductInventoryErrorCode.MissedSystemOrSeries);
        }
        var roots = await _treeRepository.GetRootTree();

        var findSystem = roots.FirstOrDefault()?.Children.FirstOrDefault(x => x.Name == "固化")?.Children.FirstOrDefault(x => x.Name == dto.System);
        if (findSystem == null)
        {
            throw new BusinessException(ProductInventoryErrorCode.ErrorSystem);
        }
        var module = ObjectMapper.Map<ArchitectureListModuleDto, ArchitectureListModule>(dto);
        module.SetParentId(findSystem.Id);
        module.CreateUser = createUser;
        module.Status = ArchitectureListPublishStatus.DelayInMark;
        //foreach (ArchitectureListMaterialDto child in dto.Materials)
        //{
        //    module.Materials.Add(
        //                ObjectMapper.Map<ArchitectureListMaterialDto, ArchitectureListMaterial>(child));
        //}
        await _moduleRepository.InsertAsync(module, true);
        return ObjectMapper.Map<ArchitectureListModule, ArchitectureListModuleDto>(module);
    }

    public async Task<List<ArchitectureListModuleDto>> CopyModules(string createUser, List<Guid> ids)
    {
        var copys = new List<ArchitectureListModule>();

        var modules = await _moduleRepository.FindByIds(ids, true);
        Dictionary<Guid, List<Guid>> moduleFiles = new Dictionary<Guid, List<Guid>>();
        foreach (var module in modules)
        {
            var createDto = ObjectMapper.Map<ArchitectureListModule, ArchitectureListModuleCreateDto>(module);
            var createModule = ObjectMapper.Map<ArchitectureListModuleCreateDto, ArchitectureListModule>(createDto);
            createModule.SetParentId(module.ParentId);
            createModule.SetCopyName();
            //createModule.Path= module.Path;
            createModule.CreateUser = createUser;
            createModule.Status = ArchitectureListPublishStatus.DelayInMark;
            //copys.Add(createModule);
            var moduleFileList = await _moduleFileRepository.FindByModuleId(module.Id);
            var insert = await _moduleRepository.InsertAsync(createModule, true);
            if (moduleFiles.Count > 0)
                await InsertModuleFiles(insert.Id, moduleFileList.Select(e => e.FileId).ToList());
        }

        //await _moduleRepository.InsertManyAsync(copys, autoSave: true);

        //if (moduleFiles.Count > 0)
        //{
        //    foreach (var item in moduleFiles)
        //    {
        //        await InsertModuleFiles(item.Key, item.Value);
        //    }
        //}

        return ObjectMapper.Map<List<ArchitectureListModule>, List<ArchitectureListModuleDto>>(copys);
    }

    #endregion

    #region 删

    public async Task Delete(Guid id, string name, string code, string mode)
    {
        try
        {
            var module = await _moduleRepository.Exists(id, name, code, mode);
            if (module != null)
            {
                await Delete(module);
            }
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task Delete(List<Guid> id)
    {
        try
        {
            var module = await _moduleRepository.FindByIds(id);
            var materialsDeletes = new List<ArchitectureListMaterial>();
            module.ForEach(x => materialsDeletes.AddRange(x.Materials));
            await _moduleFileRepository.DeleteByModuleId(id);
            await _materialRepository.DeleteManyAsync(materialsDeletes, true);
            await _moduleRepository.DeleteManyAsync(module, true);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task Delete(Guid id)
    {
        try
        {
            var module = await _moduleRepository.FindById(id);
            if (module != null)
            {
                await Delete(module);
            }
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }


    private async Task Delete(ArchitectureListModule module)
    {
        try
        {
            await _materialRepository.DeleteManyAsync(module.Materials, autoSave: true);
            await _moduleRepository.DeleteAsync(module, autoSave: true);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<List<ArchitectureListFileDto>> DeleteFile(List<Guid> ids)
    {
        try
        {
            var dto = await _fileRepository.FindByIds(ids);
            var map = ObjectMapper.Map<List<ArchitectureListFile>, List<ArchitectureListFileDto>>(dto);
            await _moduleFileRepository.DeleteByFileId(ids);
            await _fileRepository.DeleteManyAsync(dto, true);
            return map;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    #endregion

    #region 改

    public async Task<List<ArchitectureListModuleDto>> UpdateStatus(string user, List<Guid> id, ArchitectureListPublishStatus status)
    {
        var results = new List<ArchitectureListModuleDto>();
        var update = await _moduleRepository.FindByIds(id, false);
        update.ForEach(x => { x.Status = status; x.ModifyUser = user; });
        await _moduleRepository.UpdateManyAsync(update, autoSave: true);
        return results;
    }

    void UpdateModule(string user, ArchitectureListModule module, ArchitectureListModuleDto dto)
    {
        module.ModifyUser = user;
        module.Name = dto.Name;
        module.Model = dto.Model;
        module.Length = dto.Length;
        module.Width = dto.Width;
        module.Height = dto.Height;
        module.Category = dto.Category;
        module.System = dto.System;
        module.ProcessingMode = dto.ProcessingMode;
        module.ProcessingCode = dto.ProcessingCode;
        module.Unit = dto.Unit;
        module.SupplyMode = dto.SupplyMode;
        module.DetailNum = dto.DetailNum;
        module.AssemblyDrawingNum = dto.AssemblyDrawingNum;
        module.ProcessNum = dto.ProcessNum;
        module.ModuleSpecification = dto.ModuleSpecification;
        module.Optional = dto.Optional;
        module.Remark = dto.Remark;
        var deleteMaterials = module.Materials.Where(e => dto.Materials.All(x => !x.Id.Equals(e.Id))).ToList();
        if (deleteMaterials.Count() > 0)
        {
            module.Materials.RemoveAll(deleteMaterials);
        }
    }

    void UpdateMaterial(ArchitectureListMaterial material, ArchitectureListMaterialDto dto)
    {
        material.Name = dto.Name;
        material.Code = dto.Code;
        material.Composition = dto.Composition;
        material.Length = dto.Length;
        material.Width = dto.Width;
        material.Height = dto.Height;
        material.MaterialQuality = dto.MaterialQuality;
        material.BasicPerformance = dto.BasicPerformance;
        material.Unit = dto.Unit;
        material.IsProcess = dto.IsProcess;
        material.InstallationCode = dto.InstallationCode;
        material.Remark = dto.Remark;
        material.Usage = dto.Usage;
        
    }

    public async Task<ArchitectureListModuleDto> Update(string user, ArchitectureListModuleDto input)
    {
        var module = await _moduleRepository.FindById(input.Id);
        if (input.ModifyStatus == ArchitectureListModifyStatus.Modify)
        {
            UpdateModule(user,module, input);
        }

        foreach (var item in input.Materials)
        {
            if (item.ModifyStatus == ArchitectureListModifyStatus.Modify)
            {
                var material = module.Materials.FirstOrDefault(x => x.Id.Equals(item.Id));
                if (material != null)
                {
                    UpdateMaterial(material, item);
                }
            }
            else if (item.ModifyStatus == ArchitectureListModifyStatus.Insert)
            {
                var insert = ObjectMapper.Map<ArchitectureListMaterialDto, ArchitectureListMaterial>(item);
                insert.SetParentId(module.Id);
                await _materialRepository.InsertAsync(insert, true);
            }
        }

        var result = await _moduleRepository.UpdateAsync(module, true);
        return ObjectMapper.Map<ArchitectureListModule, ArchitectureListModuleDto>(result);
    }


    public async Task<ArchitectureListTreeDto> UpdateTreeNode(string user, ArchitectureListTreeDto input)
    {
        var selectTreeNode = await _treeRepository.GetAsync(input.Id);
        if (selectTreeNode != null)
        {
            selectTreeNode.Data = input.Data;
            await _treeRepository.UpdateAsync(selectTreeNode, true);
        }
        return ObjectMapper.Map<ArchitectureListTree, ArchitectureListTreeDto>(selectTreeNode);
    }
    #endregion

    #region 查

    public async Task<List<ArchitectureListTreeDto>> GetTreeRoot()
    {
        var roots = await _treeRepository.GetRootTree();
        return ObjectMapper.Map<List<ArchitectureListTree>, List<ArchitectureListTreeDto>>(roots);
    }

    public async Task<PagedResultDto<ArchitectureListModuleDto>> Page(
        string key,
        string searchValue,
        string searchCode,
        ArchitectureListPublishStatus status,
        string sorting,
        int skipCount,
        int maxResultCount)
    {
        var treeIds = new List<Guid>();
        if (!string.IsNullOrEmpty(key))
        {
            var trees = await _treeRepository.GetChildren(Guid.Parse(key));
            treeIds = trees.Select(x => x.Id).ToList();
        }
        else
        {
            return new PagedResultDto<ArchitectureListModuleDto>(
                0,
                new List<ArchitectureListModuleDto>()
            );
        }

        var result = await _moduleRepository.PageModule(
            treeIds,
            searchValue,
            searchCode,
            status,
            sorting,
            skipCount,
            maxResultCount);

        int totalCount = result.Item1;

        var dto = ObjectMapper.Map<List<ArchitectureListModule>, List<ArchitectureListModuleDto>>(result.Item2);

        foreach (var item in dto)
        {
            item.Materials = item.Materials.OrderBy(x => x.Name).ToList();
            var files = (await _moduleFileRepository.FindByModuleId(item.Id)).Select(x => x.File).ToList();
            var fileDto = ObjectMapper.Map<List<ArchitectureListFile>, List<ArchitectureListFileDto>>(files);
            item.Files = fileDto;
        }

        return new PagedResultDto<ArchitectureListModuleDto>(
            totalCount, dto
        );
    }


    public async Task<List<ArchitectureListFileDto>> GetFileList(Guid treeId, ArchitectureListFileStatus status)
    {
        var files = await _fileRepository.FindByStatus(treeId, status);

        var fileDtos = ObjectMapper.Map<List<ArchitectureListFile>, List<ArchitectureListFileDto>>(files);
        fileDtos = fileDtos.OrderByDescending(e => e.FilePath.EndsWith("pdf")).ToList();
        return fileDtos;
    }

    public async Task<object> RevitSearch(List<string> names)
    {
        List<ArchitectureListModule> modules = new List<ArchitectureListModule>();
        foreach (var item in names)
        {
            string name = item.Contains('@') ? item.Split('@')[0] : item;
            string process = item.Contains('@') ? item.Split('@')[1] : "标准加工";
            var module = await _moduleRepository.FindByNameAndProcess(name, process);
            if (module != null)
            {
                modules.Add(module);
            }
        }

        var dtos = ObjectMapper.Map<List<ArchitectureListModule>, List<ArchitectureListModuleDto>>(modules);
        foreach (var dto in dtos)
        {
            dto.Materials = dto.Materials.Where(e => e.Tag == ArchitectureListMaterialTag.Install).ToList();
        }

        return dtos;
    }


    public async Task<object> CadSearch(List<string> names)
    {
        List<ArchitectureListModule> modules = new List<ArchitectureListModule>();
        for (int i = 0; i < names.Count; i++)
        {
            string type = names[i].Split('%')[0];
            string processMode = names[i].Split('%')[1];

            var module = await _moduleRepository.FindByTypeAndProcess(type, processMode);
            if (module != null)
            {
                modules.Add(module);
            }
        }
        var dtos = ObjectMapper.Map<List<ArchitectureListModule>, List<ArchitectureListModuleDto>>(modules);
        return dtos;
    }
    #endregion
}