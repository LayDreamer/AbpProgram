using System.Data;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using NUglify.Helpers;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Caching;
using YaSha.DataManager.ProductInventory;
using YaSha.DataManager.ProductRetrieval.Dto;
using YaSha.DataManager.ProductRetrieval.Manager;
using YaSha.DataManager.StandardAndPolicy.Dto;

namespace YaSha.DataManager.ProductRetrieval;

[Authorize(DataManagerPermissions.ProductRetrieval.Default)]
public class ProductRetrievalAppService : DataManagerAppService, IProductRetrievalAppService, ITransientDependency
{
    private readonly ProductRetrievalManager _productService;
    private readonly MaterialInventoryManager _materialInventory;
    private readonly ProjectInfoManager _projectInfoManager;

    public readonly IMaterialInventoryAppService _materialInventoryAppService;
    public readonly IProjectInfoAppService _projectInfoAppService;
    private readonly IDistributedCache<PagedResultDto<ProductRetrievalResultDto>> _cache;

    public ProductRetrievalAppService(ProductRetrievalManager productService, MaterialInventoryManager materialInventory, ProjectInfoManager projectInfoManager, IMaterialInventoryAppService materialInventoryAppService, IProjectInfoAppService projectInfoAppService, IDistributedCache<PagedResultDto<ProductRetrievalResultDto>> cache)
    {
        _productService = productService;
        _materialInventory = materialInventory;
        _projectInfoManager = projectInfoManager;
        _materialInventoryAppService = materialInventoryAppService;
        _projectInfoAppService = projectInfoAppService;
        _cache = cache;
    }

    public async Task<PagedResultDto<ProductRetrievalDto>> PageProductRetrieval(ProductIndexSearchDto input)
    {
        return await _productService.PageProductRetrieval(input);
    }
    public async Task<List<ProductRetrievalDto>> NoPageProductRetrieval(ProductIndexSearchDto input)
    {
        return await _productService.NoPageProductRetrieval(input);
    }


    public async Task<PagedResultDto<ProductRetrievalResultDto>> FindProductRetrievalByInput(ProductRetrievalSearchDto input)
    {
        var cacheKey = input.CalculateCacheKey();
        return await _cache.GetOrAddAsync(cacheKey, async () =>
        {
            List<string> codes = new();
            List<ProjectInfoSearchCodeDto> searchCodeDtos = new List<ProjectInfoSearchCodeDto>();
            var pagedResult = await _productService.FindProductRetrievalByInputs(input);
            if (input.SearchInfo == null || input.SearchInfo.Count == 0)
                return pagedResult;
            foreach (var item in pagedResult.Items)
            {
                codes.Add(item.MaterialCode);
            }
            codes = codes.Distinct().ToList();
            var materialStockInfo = await _materialInventoryAppService.GetByErpAsync(codes);
            if (materialStockInfo.Success && materialStockInfo.Data is List<MaterialInventoryDto> materialInventoryDtos && materialInventoryDtos.Count > 0)
            {
                foreach (var item in pagedResult.Items)
                {
                    double count = 0;
                    double money = 0;
                    var inventoryDtos = materialInventoryDtos.Where(x => x.MaterialCode == item.MaterialCode);
                    if (inventoryDtos.Any())
                    {
                        inventoryDtos.ForEach(e => { count += e.InventoryQuantity; money += e.InventoryAmount; });
                        item.MaterialCount = Math.Round(count, 4).ToString();
                        item.MaterialMoney = Math.Round(money, 2).ToString();
                    }
                }
            }
            return pagedResult;
        }, () => new DistributedCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2)
        });
    }

    public async Task<List<ProductRetrievalResultDto>> FindAllProductRetrievalByInput(ProductRetrievalSearchDto input)
    {
        return await _productService.FindAllProductRetrievalByInputs(input);
    }

    public async Task<ApiResultDto> ImportElementsFromExcel(IFormFile file)
    {
        //string json = "";
        ApiResultDto resultDto = new ApiResultDto();
        var name = Guid.NewGuid() + Path.GetFileName(file.FileName);
        var serverPath = "/ServerData/FileManagement/ProductRetrievalList/" + name;
        //var serverPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "testFile", name);
        try
        {
            await UploadFile(file, serverPath);
        }
        catch (Exception ex)
        {
            resultDto.Success = false;
            resultDto.Error = $"无法上传文件到服务器:{ex.Message}";
            return resultDto;
        }

        try
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            var package = new ExcelPackage(new FileInfo(serverPath));
            //Dictionary<string, ProductInventroyTag> dic = new Dictionary<string, ProductInventroyTag>();

            List<SearchInfoDto> searchInfo = new();
            foreach (ExcelWorksheet sheet in package.Workbook.Worksheets)
            {
                DataTable dt = WorksheetToTable(sheet);
                foreach (DataRow dr in dt.Rows)
                {
                    string code = "";
                    if (!string.IsNullOrEmpty(dr[0].ToString()) && dr[0].ToString() != "/")
                    {
                        code = dr[0].ToString();
                        if (!string.IsNullOrEmpty(code) && searchInfo.All(e => !e.Code.Equals(code)))
                        {
                            //dic.Add(code, ProductInventroyTag.Product);
                            searchInfo.Add(new SearchInfoDto
                            {
                                Code = code,
                                Tag = ProductInventroyTag.Product,
                            });
                        }
                    }
                    else if (!string.IsNullOrEmpty(dr[1].ToString()) && dr[1].ToString() != "/")
                    {
                        code = dr[1].ToString();
                        if (!string.IsNullOrEmpty(code) && searchInfo.All(e => !e.Code.Equals(code)))
                        {
                            //dic.Add(code, ProductInventroyTag.Modules);
                            searchInfo.Add(new SearchInfoDto
                            {
                                Code = code,
                                Tag = ProductInventroyTag.Modules,
                            });
                        }
                    }
                    else
                    {
                        code = dr[2].ToString();
                        if (!string.IsNullOrEmpty(code) && searchInfo.All(e => !e.Code.Equals(code)))
                        {
                            //dic.Add(code, ProductInventroyTag.Material);
                            searchInfo.Add(new SearchInfoDto
                            {
                                Code = code,
                                Tag = ProductInventroyTag.Material,
                            });
                        }
                    }
                }
            }
            //json = JsonConvert.SerializeObject(dic);
            searchInfo.ForEach(e => e.Index = searchInfo.IndexOf(e));
            resultDto.Success = true;
            resultDto.Data = searchInfo;
        }
        catch (Exception ex)
        {
            resultDto.Success = false;
            resultDto.Error = $"文件解析失败:{ex.Message}";
        }

        return resultDto;
    }

    public async Task<byte[]> ExportElementsToExcel(List<ProductRetrievalResultDto> input)
    {
        ExcelPackage package = null;
        try
        {
            var productDtos = input;
            var codes = new List<string>();
            var projectDtos = new List<List<ProjectInfoDto>>();
            foreach (var item in productDtos)
            {
                codes.Add(item.MaterialCode);
                string code = String.Empty;
                var type = ProjectInfoInputType.Undefined;
                switch (item.Tag)
                {
                    case ProductInventroyTag.Product: type = ProjectInfoInputType.Producut; code = item.ProductCode; break;
                    case ProductInventroyTag.Modules: type = ProjectInfoInputType.Module; code = item.ModuleCode; break;
                    case ProductInventroyTag.Material: type = ProjectInfoInputType.Material; code = item.MaterialCode; break;
                }
                var project = await _projectInfoManager.FindByCodeAndType(code, type);
                //project = project.DistinctBy(x => x.ProductCode).ToList();
                projectDtos.Add(project);
            }
            var materialsInfoDtos = await _materialInventory.Find(codes);
            materialsInfoDtos = materialsInfoDtos.GroupBy(x => new { x.MaterialCode, x.Warehouse, x.WarehouseLocationName }).Select(y => y.First()).ToList();
            string templatePath = "/ServerData/FileManagement/ProductRetrievalList/template/【产品检索】导出模板.xlsx";
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            package = new ExcelPackage(new FileInfo(templatePath));
            ExcelWorksheet sheet = package.Workbook.Worksheets[0];
            string inventoryTimeValue = sheet.Cells[11, 1].Value.ToString();
            int startRow = 11;
            int xuhao = 1;
            int pCode = 2;
            int pName = 3;
            int pOpinion = 4;
            int mCode = 5;
            int mName = 6;
            int mOpinion = 7;
            int wCode = 8;
            int wName = 9;
            int unit = 10;
            int inventoryNum = 11;
            int inventorymoney = 12;

            #region 填写表格基本信息

            sheet.InsertRow(startRow, productDtos.Count);
            sheet.Cells[startRow, 13, startRow + productDtos.Count, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[startRow, 13, startRow + productDtos.Count, 13].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            var group = productDtos.GroupBy(e => new { e.ProductName, e.ProductCode }).ToList();

            for (int i = 0; i < group.Count; i++)
            {
                #region 设置产品信息并合并单元格

                var modules = group[i].ToList().GroupBy(e => new { e.ModuleName, e.ModuleCode }).ToList();
                int pNum = 0;
                foreach (var item in modules)
                {
                    var matters = group[i].ToList().Where(e => e.ModuleCode == item.Key.ModuleCode).ToList();
                    pNum += matters.Count;
                }

                sheet.Cells[startRow, xuhao].Value = (i + 1).ToString();
                sheet.Cells[startRow, pCode].Value = group[i].Key.ProductCode;
                sheet.Cells[startRow, pName].Value = group[i].Key.ProductName;
                pNum = pNum >= 1 ? pNum - 1 : pNum;
                MergeAndCenter(sheet, startRow, xuhao, startRow + pNum, xuhao);
                MergeAndCenter(sheet, startRow, pCode, startRow + pNum, pCode);
                MergeAndCenter(sheet, startRow, pName, startRow + pNum, pName);
                MergeAndCenter(sheet, startRow, pOpinion, startRow + pNum, pOpinion);

                #endregion

                for (int j = 0; j < modules.Count; j++)
                {
                    #region 设置模块信息

                    var matters = group[i].ToList().Where(e => e.ModuleCode == modules[j].FirstOrDefault().ModuleCode).ToList();
                    int count = matters.Count;
                    sheet.Cells[startRow, mCode].Value = modules[j].Key.ModuleCode;
                    sheet.Cells[startRow, mName].Value = modules[j].Key.ModuleName;
                    count = count >= 1 ? count - 1 : count;
                    MergeAndCenter(sheet, startRow, mCode, startRow + count, mCode);
                    MergeAndCenter(sheet, startRow, mName, startRow + count, mName);
                    MergeAndCenter(sheet, startRow, mOpinion, startRow + count, mOpinion);
                    #endregion

                    for (int k = 0; k < matters.Count; k++)
                    {
                        #region 写入物料数据

                        sheet.Cells[startRow + k, wCode].Value = matters[k].MaterialCode;
                        sheet.Cells[startRow + k, wName].Value = matters[k].MaterialName;
                        var infos = materialsInfoDtos.Where(e => e.MaterialCode == matters[k].MaterialCode).ToList();
                        if (infos == null) continue;
                        sheet.Cells[startRow + k, unit].Value = infos.Count > 0 ? infos[0].Unit : "";

                        infos = infos.Where(e => e.WarehouseLocationName.Contains("合格") && (e.Warehouse.Contains("原材料") ||
                        e.Warehouse.Contains("半成品")) && e.InventoryQuantity > 0).ToList();
                        sheet.Cells[startRow + k, inventoryNum].Value = GetInventoryQuantity(infos);
                        sheet.Cells[startRow + k, inventorymoney].Value = GetInventoryAmount(infos);

                        if (sheet.Cells[startRow + k, inventoryNum].Value.ToString().Contains("\n"))
                        {
                            sheet.Cells[startRow + k, inventoryNum].Style.WrapText = true;
                            sheet.Cells[startRow + k, inventorymoney].Style.WrapText = true;
                        }

                        #endregion
                        sheet.Row(startRow + k).CustomHeight = true;//自动调整行高
                        sheet.Cells[startRow + k, wCode, startRow + k, inventorymoney].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        sheet.Cells[startRow + k, wCode, startRow + k, inventorymoney].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    }

                    startRow += matters.Count;
                }
            }

            #endregion

            #region  填写日期
            string time = materialsInfoDtos.Count > 0 ? materialsInfoDtos[0].CreationTime.ToString("yyyy-MM-dd") : "";
            try
            {
                string[] ss = time.Split('/');
                time = $"{ss[0]}年{ss[1]}月{ss[2]}日";
            }
            catch
            {
            }

            sheet.Cells[startRow, 1].Value = inventoryTimeValue + time;

            #endregion

            #region 填写项目信息

            int projectStart = startRow + 3;
            int closedNum = 1;
            int closedCode = 2;
            int closedName = 3;
            int closedProject = 4;
            int num = 1;
            //记录已写入表格的编码，避免重复
            List<string> hasCodes = new List<string>();

            for (int i = 0; i < projectDtos.Count; i++)
            {
                try
                {
                    if (projectDtos[i] == null || projectDtos[i].Count == 0) continue;
                    string code = GetCodeByTag(productDtos[i]);
                    if (hasCodes.Contains(code)) continue;
                    sheet.InsertRow(projectStart, projectDtos[i].Count);

                    sheet.Cells[projectStart, closedNum].Value = num.ToString();
                    sheet.Cells[projectStart, closedCode].Value = code;
                    sheet.Cells[projectStart, closedName].Value = GetNameByTag(productDtos[i]);
                    hasCodes.Add(code);

                    MergeAndCenter(sheet, projectStart, closedNum, projectStart + projectDtos[i].Count - 1, closedNum);
                    MergeAndCenter(sheet, projectStart, closedCode, projectStart + projectDtos[i].Count - 1, closedCode);
                    MergeAndCenter(sheet, projectStart, closedName, projectStart + projectDtos[i].Count - 1, closedName);


                    for (int j = 0; j < projectDtos[i].Count; j++)
                    {
                        sheet.Cells[projectStart + j, closedProject].Value = GetProjectInfo(projectDtos[i][j]);
                        MergeAndCenter(sheet, projectStart + j, closedProject, projectStart + j, closedProject + 2);
                        sheet.Cells[projectStart + j, closedProject, projectStart + j, closedProject].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        sheet.Cells[projectStart + j, closedProject, projectStart + j, closedProject].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    }
                    projectStart = projectStart + projectDtos[i].Count;
                    num++;
                }
                catch (Exception e)
                {

                }

            }

            #endregion

            byte[] bytes = await package.GetAsByteArrayAsync();
            return bytes;
        }
        catch (Exception ex)
        {
        }

        return null;
    }

    #region  方法

    public static void MergeAndCenter(ExcelWorksheet sheet, int startRow, int startColumn, int endRow, int endColumn)
    {
        sheet.Cells[startRow, startColumn, endRow, endColumn].Merge = true;
        sheet.Cells[startRow, startColumn, endRow, endColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        sheet.Cells[startRow, startColumn, endRow, endColumn].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
    }




    public static string GetProjectInfo(ProjectInfoDto projectDto)
    {
        if (projectDto == null) return "";
        return $"项目：{projectDto.ProjectName}——编码：{projectDto.ProjectCode}";
    }

    public static string GetCodeByTag(ProductRetrievalResultDto result)
    {
        string code = "";
        switch (result.Tag)
        {
            case ProductInventroyTag.Product:
                code = result.ProductCode;
                break;
            case ProductInventroyTag.Modules:
                code = result.ModuleCode;
                break;
            case ProductInventroyTag.Material:
                code = result.MaterialCode;
                break;
            default:
                break;
        }

        return code;
    }

    public static string GetNameByTag(ProductRetrievalResultDto result)
    {
        string name = "";
        switch (result.Tag)
        {
            case ProductInventroyTag.Product:
                name = result.ProductName;
                break;
            case ProductInventroyTag.Modules:
                name = result.ModuleName;
                break;
            case ProductInventroyTag.Material:
                name = result.MaterialName;
                break;
            default:
                break;
        }

        return name;
    }

    public static string GetInventoryQuantity(List<MaterialInventoryDto> details)
    {
        string value = "";
        for (int i = 0; i < details.Count; i++)
        {
            value += details[i].InventoryQuantity;
            if (i != details.Count - 1)
            {
                value += "；\n";
            }
        }

        return value;
    }

    public static string GetInventoryAmount(List<MaterialInventoryDto> details)
    {
        string value = "";
        for (int i = 0; i < details.Count; i++)
        {
            value += details[i].InventoryAmount;
            if (i != details.Count - 1)
            {
                value += "；\n";
            }
        }

        return value;
    }


    /// <summary>
    /// 将文件上传服务器
    /// </summary>
    /// <param name="file"></param>
    public async Task UploadFile(IFormFile file, string serverPath)
    {
        using (FileStream fileStream = new FileStream(serverPath, FileMode.Create, FileAccess.Write))
        {
            await file.CopyToAsync(fileStream);
        }
    }

    /// <summary>
    /// 读取数据
    /// </summary>
    /// <param name="worksheet"></param>
    /// <returns></returns>
    public static DataTable WorksheetToTable(ExcelWorksheet worksheet)
    {
        DataTable dt = new DataTable(worksheet.Name);
        if (worksheet.Dimension == null)
        {
            return dt;
        }

        //获取worksheet的行数
        int rows = worksheet.Dimension.End.Row;
        //获取worksheet的列数
        int cols = worksheet.Dimension.End.Column;
        int startRow = -1;
        int productCol = -1;
        int moduleCol = -1;
        int materialCol = -1;
        try
        {
            for (int i = 1; i < rows; i++)
            {
                for (int j = 1; j <= cols; j++)
                {
                    string value = GetMergeValue(worksheet, i, j);
                    productCol = value.Trim().Equals("产品编码") ? j : productCol;
                    moduleCol = value.Trim().Equals("模块编码") ? j : moduleCol;
                    materialCol = value.Trim().Equals("物料编码") ? j : materialCol;
                }

                if (productCol != -1 && moduleCol != -1 && materialCol != -1)
                {
                    startRow = i + 1;
                    break;
                }
                else
                {
                    productCol = moduleCol = materialCol = -1;
                }
            }

            if (startRow == -1 || productCol == -1 || moduleCol == -1 || materialCol == -1) return dt;
            dt.Columns.Add("产品编码");
            dt.Columns.Add("模块编码");
            dt.Columns.Add("物料编码");
            int emptyCount = 0;
            for (int i = startRow; i <= rows; i++)
            {
                if (i == 249)
                {
                    string s = "";
                }

                //当连续十行没有数据时，不再读取
                if (emptyCount == 10)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        dt.Rows.RemoveAt(dt.Rows.Count - 1);
                    }
                }

                string productString = GetMergeValue(worksheet, i, productCol);

                //当产品编码中包含中文时，不再继续读下去
                bool hasChinese = Regex.IsMatch(productString, @"[\u4e00-\u9fa5]");
                if (hasChinese) break;


                DataRow dr = dt.NewRow();
                dr[0] = productString;
                dr[1] = GetMergeValue(worksheet, i, moduleCol);
                dr[2] = GetMergeValue(worksheet, i, materialCol);
                dt.Rows.Add(dr);

                if (string.IsNullOrEmpty(dr[0].ToString()) && string.IsNullOrEmpty(dr[1].ToString()) && string.IsNullOrEmpty(dr[2].ToString()))
                {
                    emptyCount++;
                }
                else
                {
                    emptyCount = 0;
                }
            }
        }
        catch
        {
        }

        return dt;
    }


    public static string GetMergeValue(ExcelWorksheet worksheet, int row, int col)
    {
        string value = "";
        try
        {
            if (worksheet.Cells[row, col].Merge)
            {
                var aa = worksheet.Cells[worksheet.MergedCells[row, col]].First().Value;
                if (aa == null) return "";
                value = aa.ToString();
            }
            else
            {
                value = worksheet.GetValue<string>(row, col);
            }
        }
        catch
        {
        }

        return value == null ? "" : value;
    }

    #endregion
}