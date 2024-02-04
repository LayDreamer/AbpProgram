using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using YaSha.DataManager.ProductInventory.Dto;
using Volo.Abp.Users;
using Volo.Abp.Emailing;
using System.Net.Mail;
using System.Text;
using Volo.Abp.Domain.Services;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Linq.Dynamic.Core;

namespace YaSha.DataManager.ProcessingLists
{
    public class ProcessingListAppService :
      CrudAppService<
         ProcessingList,
         ProcessingListDto,
         Guid,
         PagedAndSortedResultRequestDto,
         ProcessingListCreateDto>,
     IProcessingListAppService,
         IDomainService
    {
        protected readonly IRepository<ProcessingList, Guid> _repository;

        protected readonly IRepository<ProcessingData> _dataRepository;

        private readonly ICurrentUser _currentUser;

        private readonly IEmailSender _emailSender;



        public ProcessingListAppService(IRepository<ProcessingList, Guid> repository, IRepository<ProcessingData> dataRepository, ICurrentUser currentUser, IEmailSender emailSender)
            : base(repository)
        {
            _repository = repository;
            _dataRepository = dataRepository;
            _currentUser = currentUser;
            _emailSender = emailSender;
        }

        public async Task<ProcessingListDto> GetAsync(Guid id)
        {
            var source = await _repository.GetAsync(id);
            ProcessingListDto dto = base.ObjectMapper.Map<ProcessingList, ProcessingListDto>(source);
            return dto;
        }

        public async Task<PagedResultDto<ProcessingListDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            var source = await _repository.GetListAsync();
            if (!string.IsNullOrEmpty(input.Sorting))
            {
                source = source.AsQueryable().OrderBy(input.Sorting).ToList();
            }
            else
            {
                source = source.OrderByDescending(e => e.CreationTime).ToList();
            }
            return new PagedResultDto<ProcessingListDto>(source.Count,
              base.ObjectMapper.Map<List<ProcessingList>, List<ProcessingListDto>>(source));
        }



        public async Task<string> SendEmail(Object json)
        {
            try
            {
                string suffix = "@chinayasha.com";
                JObject jo = (JObject)JsonConvert.DeserializeObject(json.ToString());
                string recipient = jo["recipient"] == null ? "" : jo["recipient"].ToString();
                string licenseCode = jo["copys"] == null ? "" : jo["copys"].ToString();
                string emailTitle = jo["emailTitle"] == null ? "" : jo["emailTitle"].ToString();
                string emailContent = jo["emailContent"] == null ? "" : jo["emailContent"].ToString();

                string smtpServer = "smtp.exmail.qq.com"; //SMTP服务器
                string mailFrom = "gyhbds@chinayasha.com"; //登陆用户名，邮箱
                string userPassword = "JjrbE68ZHJ2KvbzF";//注意授权码  不是登录密码
                SmtpClient smtpClient = new SmtpClient();
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;//指定电子邮件发送方式
                smtpClient.Host = smtpServer; //指定SMTP服务器
                smtpClient.Credentials = new System.Net.NetworkCredential(mailFrom, userPassword);//用户名和密码

                // 设置收件人邮箱地址和抄送人邮箱地址
                string[] toAddresses = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(recipient);
                string[] ccAddresses = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(licenseCode);

                // 创建MailMessage对象
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(mailFrom);

                foreach (string toAddress in toAddresses)
                {
                    mailMessage.To.Add(new MailAddress(toAddress + suffix));
                }
                foreach (string ccAddress in ccAddresses)
                {
                    mailMessage.CC.Add(new MailAddress(ccAddress + suffix));
                }

                mailMessage.Subject = emailTitle;//主题
                mailMessage.Body = emailContent;//内容
                mailMessage.BodyEncoding = Encoding.UTF8;//正文编码
                mailMessage.IsBodyHtml = false;//设置为HTML格式
                mailMessage.Priority = MailPriority.Low;//优先级
                smtpClient.Send(mailMessage); // 发送邮件
                return "success";
            }
            catch (Exception ex)
            {
                return "error:" + ex.Message;
            }
        }



        public async Task<string> UploadExcel(IFormFile file)
        {
            try
            {
                //var serverPath = "D:\\0\\01收到资料\\ProcessingList\\";
                var serverPath = "/ServerData/FileManagement/ProcessingList/";
                string name = Path.GetFileNameWithoutExtension(file.FileName) + ".xlsx";
                int num = 1;
                while (File.Exists(serverPath + name))
                {
                    name = $"{Path.GetFileNameWithoutExtension(file.FileName)}({num}).xlsx";
                    num++;
                }
                using (FileStream fileStream = new FileStream(serverPath + name, FileMode.Create, FileAccess.Write))
                {
                    await file.CopyToAsync(fileStream);
                }
                return $"success:{name}";
            }
            catch (Exception ex)
            {
                return $"error:{ex.Message}";
            }
        }
        public async Task<PagedResultDto<ProcessingListDto>> GetAllListAsync(OrderNotificationSearchDto input)
        {
            IQueryable<ProcessingList> query = await _repository.GetQueryableAsync();
            var totalCount = await AsyncExecuter.CountAsync(query);
            query = ApplySorting(query, input);
            query = query.Skip((input.SkipCount - 1) * input.MaxResultCount).Take(input.MaxResultCount);
            var entities = await AsyncExecuter.ToListAsync(query);
            var entityDtos = await MapToGetListOutputDtosAsync(entities);
            return new PagedResultDto<ProcessingListDto>(
                totalCount,
                entityDtos
            );
        }

        public async Task<byte[]> DownloadExcelAsync(Guid id)
        {
            try
            {
                var source = await _repository.GetAsync(id);
                string path = source.FileAddress;
                if (string.IsNullOrEmpty(path)) return null;
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                var package = new ExcelPackage(new FileInfo(path));
                byte[] bytes = await package.GetAsByteArrayAsync();
                return bytes;
            }
            catch (Exception ex) { }
            return null;
        }



        public async Task<ProcessingListDto> CreateDataFromFile(ProcessingListCreateDto input)
        {
            try
            {
                var file = await this.CreateAsync(input);
                if (file != null)
                {
                    Guid? currFileId = file.Id;
                    int columnIndex = 6;
                    ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                    var package = new ExcelPackage(new FileInfo(file.FileAddress));
                    ExcelWorksheet sheet = package.Workbook.Worksheets[0];
                    if (sheet.Dimension == null) return null;
                    //获取worksheet的行数
                    int rows = sheet.Dimension.End.Row;
                    //获取worksheet的列数
                    int cols = sheet.Dimension.End.Column;

                    int productName = 4;//产品名称
                    int productCode = 5;//产品编码
                    int productLong = 6;//产品-长
                    int productWidth = 7;//产品-宽
                    int productHeight = 8;//产品-高

                    int moduleName = 12;//模块名称
                    int moduleCode = 13;//模块编码
                    int moduleLength = 15;//模块-长
                    int moduleWidth = 16;//模块-宽
                    int moduleHeight = 17;//模块-高

                    int materialName = 20;//物料名称
                    int materialCode = 21;//物料编码
                    int materialLength = 22;//物料-长
                    int materialWidth = 23;//物料-宽
                    int materialHeight = 24;//物料-高
                    int unit = 39;//物料单位
                    List<int> indexs = new List<int>() { productName, productCode, productLong, productWidth, productHeight,
                    moduleName,moduleCode,moduleLength, moduleWidth,moduleHeight, materialName,materialCode,materialLength,
                    materialWidth, materialHeight, unit };
                    List<ProcessingDataCreateDto> dataDtos = new List<ProcessingDataCreateDto>();
                    //此处为了确定哪几行数据是同一个产品，根据序号来判断
                    int num = 1;
                    Guid currProductId = Guid.NewGuid();
                    for (int i = columnIndex + 1; i <= rows; i++)
                    {
                        if (GetMergeValue(sheet, i, 1).Trim().Equals("*")) break;
                        string orderNum = GetMergeValue(sheet, i, 1);
                        if (orderNum.Split('.')[0] != num.ToString())
                        {
                            currProductId = Guid.NewGuid();
                            num = Convert.ToInt32(orderNum.Split('.')[0]);
                        }
                        ProcessingDataCreateDto dataDto = new ProcessingDataCreateDto(
                        currFileId,
                        currProductId,
                        GetMergeValue(sheet, i, productName),
                        GetMergeValue(sheet, i, productCode),
                        GetMergeValue(sheet, i, productLong),
                        GetMergeValue(sheet, i, productWidth),
                        GetMergeValue(sheet, i, productHeight),

                        GetMergeValue(sheet, i, moduleName),
                        GetMergeValue(sheet, i, moduleCode),
                        GetMergeValue(sheet, i, moduleLength),
                        GetMergeValue(sheet, i, moduleWidth),
                        GetMergeValue(sheet, i, moduleHeight),

                        GetMergeValue(sheet, i, materialName),
                        GetMergeValue(sheet, i, materialCode),
                        GetMergeValue(sheet, i, materialLength),
                        GetMergeValue(sheet, i, materialWidth),
                        GetMergeValue(sheet, i, materialHeight),
                        GetMergeValue(sheet, i, unit));
                        string s = "";
                        //不读取序号列
                        for (int j = 2; j <= cols; j++)
                        {
                            if (!indexs.Contains(j))
                            {
                                s += GetMergeValue(sheet, columnIndex, j) + ":" + GetMergeValue(sheet, i, j) + "#";
                            }
                        }
                        dataDto.Data = s;
                        dataDtos.Add(dataDto);
                    }
                    var datas = ObjectMapper.Map<List<ProcessingDataCreateDto>, List<ProcessingData>>(dataDtos);
                    await _dataRepository.InsertManyAsync(datas, autoSave: true);
                    return file;
                }
            }
            catch (Exception ex) { }
            return null;
        }




        public async Task DeleteFileAndData(Guid id)
        {
            string name = _currentUser.Name;
            var file = await this.GetAsync(id);
            if (!file.Creator.Equals(name)) return;
            var datas = await _dataRepository.GetListAsync();
            datas = datas.Where(x => x.FileId == id).ToList();
            await _dataRepository.DeleteManyAsync(datas);
            await this.DeleteAsync(id);
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
            catch { }
            return value == null ? "" : value;
        }


    }
}
