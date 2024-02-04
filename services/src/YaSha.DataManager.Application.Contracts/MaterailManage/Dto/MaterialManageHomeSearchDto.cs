using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;

namespace YaSha.DataManager.MaterailManage.Dto;

public class MaterialManageHomeSearchDto : PagedAndSortedResultRequestDto
{
    public List<string>  MaterialType { get; set; }
    public List<string> MaterialTexture { get; set; }
    public List<string> MaterialSurface { get; set; }
    public string Search { get; set; }
    public string CameraSearchKey { get; set; }

   
}