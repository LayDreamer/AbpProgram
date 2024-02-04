using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Users;
using YaSha.DataManager.ListProcessing;
using YaSha.DataManager.ListProcessing.CommonMethods;
using YaSha.DataManager.MeasuringExcels;
using YaSha.DataManager.ProcessingLists;
using YaSha.DataManager.ProductInventory.Dto;
using YaSha.DataManager.ProductRetrieval.Dto;
using YaSha.DataManager.ProductRetrieval;
using YaSha.DataManager.SaleOderList.Classes;
using YaSha.DataManager.StandardAndPolicy.Dto;
using static FreeSql.Internal.GlobalFilter;
using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Volo.Abp.Ui.LayoutHooks.LayoutHooks;
using Common = YaSha.DataManager.SaleOderList.Classes.Common;
using ExcelHelper = YaSha.DataManager.SaleOderList.Classes.ExcelHelper;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using YaSha.DataManager.ProductInventory.AggregateRoot;


namespace YaSha.DataManager.SaleOderList
{
    public class SaleOderListAppService : ApplicationService, ISaleOderListAppService
    {

        private readonly IRepository<SaleOder, Guid> _saleOderItemRepository;

        private readonly ICurrentUser _currentUser;

        public static string path = "/ServerData/FileManagement";
        string website = "https://bds.chinayasha.com/bdsfileservice/SaleOderList/UploadFiles/";
        string factoryFilePath = path + "/SaleOderList/UploadFiles/";
        string saleOderFilePath = path + "/SaleOderList/";
        string finalSeverPath = "https://bds.chinayasha.com/bdsfileservice/SaleOderList/";
        
        public SaleOderListAppService(IRepository<SaleOder, Guid> saleOderRepository, ICurrentUser currentUser)
        {
            _saleOderItemRepository = saleOderRepository;
            _currentUser = currentUser;

        }

        public async Task<PagedResultDto<SaleOderDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            var source = await _saleOderItemRepository.GetListAsync();
            if (!string.IsNullOrEmpty(input.Sorting))
            {
                source = source.AsQueryable().OrderBy(input.Sorting).ToList();
            }
            else
            {
                source = source.OrderByDescending(e => e.CreationTime).ToList();
            }
            return new PagedResultDto<SaleOderDto>(source.Count,
              base.ObjectMapper.Map<List<SaleOder>, List<SaleOderDto>>(source));
        }

        public async Task<SaleOderDto> GetAsync(Guid id)
        {
            var source = await _saleOderItemRepository.GetAsync(id);
            SaleOderDto dto = base.ObjectMapper.Map<SaleOder, SaleOderDto>(source);
            return dto;
        }

        public async Task DeleteAsync(Guid id)
        {
            try
            {
                var source = await _saleOderItemRepository.GetAsync(id);
                string path = source.FilePath.Replace(finalSeverPath, saleOderFilePath);
                File.Delete(path);
            }
            catch { }
            await _saleOderItemRepository.DeleteAsync(id);
        }



        public async Task<ApiResultDto> UploadFactoryFile(IFormFile file)
        {
            string excelPath = "";
            bool success = false;
            try
            {
                if (!Directory.Exists(factoryFilePath))
                {
                    Directory.CreateDirectory(factoryFilePath);
                }
                excelPath = Path.Combine(factoryFilePath, Path.GetFileName(file.FileName));
                int num = 1;
                while (File.Exists(excelPath))
                {
                    string s = $"{Path.GetFileNameWithoutExtension(file.FileName)}({num}).xlsx";
                    excelPath = Path.Combine(factoryFilePath, s);
                    num++;
                }
                using (FileStream fileStream = new FileStream(excelPath, FileMode.Create, FileAccess.Write))
                {
                    await file.CopyToAsync(fileStream);
                }
                success = true;
            }
            catch (Exception ex) { }
            return new ApiResultDto()
            {
                Data = excelPath,
                Success = success,
            };
        }


        public string CreateNewSplitFile(SaleOderFormDto input)
        {
            input.SplitFilePath = !string.IsNullOrEmpty(input.SplitFilePath) ? Path.Combine(factoryFilePath, input.SplitFilePath) : "";
            input.ConvertFilePath = !string.IsNullOrEmpty(input.ConvertFilePath) ? Path.Combine(factoryFilePath, input.ConvertFilePath) : "";
            input.ProductCodeFilePath = !string.IsNullOrEmpty(input.ProductCodeFilePath) ? Path.Combine(factoryFilePath, input.ProductCodeFilePath) : "";


            //含产成品编码的新拆单表生成：判断拆单表是否存在产成品编码列，若不存在则必须要上传含产成品编码文件，若存在并且有产成品编码文件，则先复制一份拆单表,读取含产成品编码文件，匹配赋值
            string errorMessage = "";
            string right = "success:";
            string wrong = "error:";
            List<string> list = new List<string>() { "模块名称", "模块编码", "安装码", "模块-长", "模块-宽", "模块-高" };
            string text = "产成品编码";
            try
            {
                string resFileName = $"{input.FileRename}.xlsx";
                if (File.Exists(Path.Combine(saleOderFilePath, _currentUser.UserName, resFileName))) return wrong + $"已存在名为【{resFileName}】文件，请重新命名";
                if (!File.Exists(input.SplitFilePath)) return wrong + "拆单表文件未找到！";
                DataTable hardwareDt = ExcelHelper.filePathToDb(input.SplitFilePath, "发货五金", 6, ref errorMessage);
                DataTable packDt = ExcelHelper.filePathToDb(input.SplitFilePath, "包装", 6, ref errorMessage);
                if (!hardwareDt.Columns.Contains(text) && !packDt.Columns.Contains(text) && !File.Exists(input.ProductCodeFilePath)) return wrong + "拆单表中不包含产成品编码，请上传含产成品编码文件！";

                if (!File.Exists(input.ProductCodeFilePath)) return right;
                DataTable codeDt = Common.CommonWorksheetToDb(input.ProductCodeFilePath, 2);
                foreach (var item in list)
                {
                    if (!codeDt.Columns.Contains(item))
                    {
                        errorMessage += Path.GetFileName(input.ProductCodeFilePath) + "中缺少列名为【" + item + "】的列\r\n";
                    }
                }
                string name = Path.GetFileNameWithoutExtension(input.SplitFilePath);
                string copySplitFilePath = input.SplitFilePath.Replace(name, name + "(含产成品编码)");
                System.IO.File.Copy(input.SplitFilePath, copySplitFilePath, true);
                bool shiped = ExcelHelper.AddFinishedCodeToExcel(codeDt, list, copySplitFilePath, "发货五金", 6, ref errorMessage);
                bool packed = ExcelHelper.AddFinishedCodeToExcel(codeDt, list, copySplitFilePath, "包装", 6, ref errorMessage);
                if (shiped && packed && string.IsNullOrEmpty(errorMessage))
                    return right + Path.Combine(website, Path.GetFileName(copySplitFilePath));
                return wrong + errorMessage;
            }
            catch (Exception ex)
            {
                return wrong + errorMessage + "\r\n" + ex.Message;
            }
        }

        public async Task<ApiResultDto> CreateSaleOderFile(object json)
        {
            List<SaleOrderDetail> saleOrderDetails = new List<SaleOrderDetail>();
            //发货五金（系统）
            List<ShippingHardwareSystem> shippingHardwareSystems = new();
            //包装（系统）
            List<PackingListSystem> packingListSystems = new();
            //材料单位转换数据
            List<MaterialUnitConvert> materialUnits = new();
            string errorMes = "error:";

            try
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(json.ToString());
                string orderData = jo["saleOrder"] == null ? "" : jo["saleOrder"].ToString();
                string materials = jo["materials"] == null ? "" : jo["materials"].ToString();

                SaleOderFormDto input = JsonConvert.DeserializeObject<SaleOderFormDto>(orderData);
                List<ProductInventoryEditDto> data = JsonConvert.DeserializeObject<List<ProductInventoryEditDto>>(materials);

                input.ConvertFilePath = !string.IsNullOrEmpty(input.ConvertFilePath) ? Path.Combine(factoryFilePath, input.ConvertFilePath) : "";
                input.ProductCodeFilePath = !string.IsNullOrEmpty(input.ProductCodeFilePath) ? Path.Combine(factoryFilePath, input.ProductCodeFilePath) : "";
                input.SplitFilePath = input.SplitFilePath.Contains(website) ? input.SplitFilePath.Replace(website, factoryFilePath) : Path.Combine(factoryFilePath, input.SplitFilePath);

                var templatePath = Path.Combine(saleOderFilePath, "template");

                string resFileName = $"{input.FileRename}.xlsx";
                #region 解析上传的Excel文件
                ExcelHelper excelHelper = new ExcelHelper();
                shippingHardwareSystems = excelHelper.ReadInfoFromExcel<ShippingHardwareSystem>(input.SplitFilePath, "发货五金", 6, 67, 1, ref errorMes);
                packingListSystems = excelHelper.ReadInfoFromExcel<PackingListSystem>(input.SplitFilePath, "包装", 6, 39, 1, ref errorMes);
                if (File.Exists(input.ConvertFilePath))
                {
                    materialUnits = excelHelper.ReadInfoFromExcel<MaterialUnitConvert>(input.ConvertFilePath, null, 2, -1, 1, ref errorMes);
                }
                else if (data == null)
                {
                    List<string> materialCodes = new List<string>();
                    shippingHardwareSystems.Where(x => !string.IsNullOrEmpty(x.MaterialCode)).ToList().ForEach(e => materialCodes.Add(e.MaterialCode));
                    packingListSystems.Where(x => !string.IsNullOrEmpty(x.MaterialCode)).ToList().ForEach(e => materialCodes.Add(e.MaterialCode));
                    materialCodes = materialCodes.Distinct().ToList();
                    return new ApiResultDto()
                    {
                        Data = materialCodes,
                        Success = false
                    };
                }
                else
                {
                    foreach (var item in data)
                    {
                        MaterialUnitConvert material = new MaterialUnitConvert();
                        material.MaterialCode = item.Code;
                        material.MaterialName = item.Name;
                        material.PurchasingUnit = item.Unit;
                        material.UsageUnit = item.UnitBase;
                        materialUnits.Add(material);
                    }
                }
                if (shippingHardwareSystems.Count == 0 || shippingHardwareSystems.Count == 0 || materialUnits.Count == 0)
                {
                    errorMes += "表格数据解析错误\r\n";
                }
                ShippingHardwareSystemConvert shippingHardware = new()
                {
                    shippingHardwareSystems = shippingHardwareSystems,
                    materialUnits = materialUnits,
                };
                shippingHardware.Excute();

                PackingListSystemConvert packingList = new()
                {
                    packingListSystems = packingListSystems,
                    materialUnits = materialUnits,
                };
                packingList.Excute();

                saleOrderDetails.AddRange(packingList.resSaleOrders);
                saleOrderDetails.AddRange(shippingHardware.resSaleOrders);
                //去除物料编码为0的数据
                saleOrderDetails.Where(e => e.MaterialCode == 0).ToList().ForEach(e => saleOrderDetails.Remove(e));
                foreach (var saleOrder in saleOrderDetails)
                {
                    saleOrder.Remark = input.Notes;
                    saleOrder.ArrivalDate = input.RequireDate;
                    saleOrder.ArrivalCode = input.CustomerCode;
                    saleOrder.OrderType = input.OrderType;
                }

                #endregion
                string fileName = "销售订单模板.xlsx";
                string filePath = Path.Combine(templatePath, fileName);
                string resDir = Path.Combine(saleOderFilePath, _currentUser.UserName);
                string resFilePath = Path.Combine(resDir, resFileName);

                if (!File.Exists(filePath))
                {
                    return new ApiResultDto()
                    {
                        Data = $"模板地址不存在：{filePath}",
                        Success = false
                    };
                }
                await excelHelper.OutPutExcel(filePath, resFilePath, saleOrderDetails, false);
                SaleOderCreateDto saleOrderDto = new SaleOderCreateDto();
                saleOrderDto.Name = resFileName;
                saleOrderDto.UploadUser = _currentUser.UserName;
                saleOrderDto.FilePath = $"{finalSeverPath}{_currentUser.UserName}/{resFileName}";
                var oder = ObjectMapper.Map<SaleOderCreateDto, SaleOder>(saleOrderDto);
                await _saleOderItemRepository.InsertAsync(oder, autoSave: true);

                return new ApiResultDto()
                {
                    Data = errorMes,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return new ApiResultDto()
                {
                    Data = errorMes + ex.Message,
                    Success = false
                };
            }
        }



    }


}
