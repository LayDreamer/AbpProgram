using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Users;
using YaSha.DataManager.ProductInventory;
using YaSha.DataManager.StandardAndPolicy.Dto;
using YaSha.DataManager.StandardAndPolicy.Manager;

namespace YaSha.DataManager.StandardAndPolicy;

public class StandardAndPolicyThemeAppService : DataManagerAppService, IStandardAndPolicyThemeAppService
{
    private readonly StandardAndPolicyThemeManager _service;

    private readonly ICurrentUser _currentUser;

    public StandardAndPolicyThemeAppService(StandardAndPolicyThemeManager service, ICurrentUser currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    public async Task<ApiResultDto> UploadFile(IFormFile file)
    {
        try
        {
            if (!Directory.Exists(StandardAndPolicyConsts.StandardServerPath))
            {
                Directory.CreateDirectory(StandardAndPolicyConsts.StandardServerPath);
            }

            var name = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var localPath = StandardAndPolicyConsts.StandardServerPath + "\\" + name;
            using (FileStream fileStream = new FileStream(localPath, FileMode.Create, FileAccess.Write))
            {
                await file.CopyToAsync(fileStream);
            }

            var serverPath = "https://bds.chinayasha.com/bdsfileservice/StandardAndPolicy/" + name;
            return new ApiResultDto()
            {
                Data = serverPath,
                Success = true,
            };
        }
        catch (Exception e)
        {
            return new ApiResultDto()
            {
                Error = e.Message,
                Success = false,
            };
        }
    }

    public async Task<List<StandardAndPolicyThemeDto>> InsertStandardAndPolicyTheme(StandardAndPolicyCreateAndUpdateDto input)
    {
        return await _service.Insert(input);
    }

    public async Task<List<StandardAndPolicyPageDto>> PageHome(int showCount = 4)
    {
        return await _service.PageHome(showCount);
    }

    public async Task<List<StandardAndPolicyPageDto>> PageSelectTree(Guid id, int showCount = 4)
    {
        return await _service.PageSelectTree(id, showCount);
    }

    public async Task<PagedResultDto<StandardAndStandLibWithAllTreeDto>> PageSearchDetail(StandardAndPolicySearchDto input)
    {
        if (_currentUser.Id == null)
        {
            throw new BusinessException("YaSha.DataManager:LoginOver");
        }

        return await _service.Page(
            _currentUser.GetId(),
            input.Id,
            input.Type,
            input.Category,
            input.Search,
            input.Province,
            input.IsCollect,
            input.Themes,
            input.Sorting,
            input.SkipCount,
            input.MaxResultCount);
    }

    public async Task<StandardAndPolicyCardDetailDto> GetCardDetail(Guid id)
    {
        if (_currentUser.Id == null)
        {
            throw new BusinessException("YaSha.DataManager:LoginOver");
        }

        return await _service.GetCardDetail(_currentUser.GetId(), id);
    }

    public async Task<StandardAndPolicyCollectDto> UpdateCollectStatus(Guid id, bool status)
    {
        if (_currentUser.Id == null)
        {
            throw new BusinessException("YaSha.DataManager:LoginOver");
        }

        return await _service.UpdateCollectStatus(_currentUser.GetId(), id, status);
    }

    public async Task<StandardAndPolicyLibDto> UpdateLib(Guid id, StandardAndPolicyCreateAndUpdateDto input)
    {
        return await _service.UpdateLib(id, input);
    }

    public async Task<ApiResultDto> DeleteLibs(List<Guid> ids)
    {
        try
        {
            var delets = await _service.Delete(ids);
            foreach (var dto in delets)
            {
                var deletePath = StandardAndPolicyConsts.StandardServerPath + "/" + Path.GetFileName(dto.ImagePath);
                File.Delete(deletePath);
                deletePath = StandardAndPolicyConsts.StandardServerPath + "/" + Path.GetFileName(dto.PdfPath);
                File.Delete(deletePath);
            }
        }
        catch (Exception e)
        {
            return new ApiResultDto()
            {
                Success = false,
                Error = e.Message,
            };
        }

        return new ApiResultDto()
        {
            Success = true,
        };
    }

    public async Task<byte[]> ExportExcel()
    {
        try
        {
            var policy = (await _service.GetAllPolicyLists()).OrderByDescending(x => x.PublishingDate).ToList();
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            // 创建一个新的ExcelPackage
            using (var package = new ExcelPackage())
            {
                // 添加一个工作表
                var worksheet = package.Workbook.Worksheets.Add("政策报表");
                // 在工作表中添加标题行
                worksheet.Cells["A1"].Value = "序号";
                worksheet.Cells["B1"].Value = "发布日期";
                worksheet.Cells["C1"].Value = "政策名称";
                worksheet.Cells["D1"].Value = "发布单位";
                worksheet.Cells["E1"].Value = "原文链接";

                worksheet.Column(1).Width = 10;
                worksheet.Column(2).Width = 10;
                worksheet.Column(3).Width = 30;
                worksheet.Column(4).Width = 25;
                worksheet.Column(5).Width = 80;

                // 设置标题行样式
                using (var range = worksheet.Cells["A1:E1"])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                // 将数据填充到工作表中
                int row = 2;
                foreach (var item in policy)
                {
                    worksheet.Cells[$"A{row}"].Value = policy.IndexOf(item) + 1;
                    worksheet.Cells[$"A{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[$"B{row}"].Value = item.PublishingDate.ToString("yyyy-MM-dd");
                    worksheet.Cells[$"B{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[$"C{row}"].Value = item.Name;
                    worksheet.Cells[$"C{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[$"D{row}"].Value = item.PublishingUnit;
                    worksheet.Cells[$"D{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[$"E{row}"].Value = item.LinkPath;
                    worksheet.Cells[$"E{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[$"F{row}"].Value = "";
                    row++;
                }
                byte[] bytes = await package.GetAsByteArrayAsync();
                return bytes;
            }
        }
        catch
        {
            return Array.Empty<byte>();
        }
    }
}