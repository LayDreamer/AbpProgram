using System.Drawing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YaSha.DataManager.MaterialManage.Dto;

namespace YaSha.DataManager.MaterailManage.Dto;

public class MaterialManageSaveDto
{
    public List<MaterialManageDto> Insert { get; set; } = new();

    public List<MaterialManageDto> Delete { get; set; } = new();

    public List<MaterialManageDto> Modify { get; set; } = new();
}