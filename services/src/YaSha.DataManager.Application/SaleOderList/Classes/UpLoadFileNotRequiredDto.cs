using Microsoft.AspNetCore.Http;

namespace YaSha.DataManager.SaleOderList.Classes
{
    public class UpLoadFileNotRequiredDto
    {
        public IFormFile File { get; set; }

        public string Name { get; set; }


    }

    public class UpLoadFileDto
    {
        private IFormFile file;

        public IFormFile File
        {
            get { return file; }
            set
            {
                try
                {
                    file = value;
                    if (file.Length > 102400000)
                    {
                        throw new Volo.Abp.UserFriendlyException($"上传文件不允许超过100M！");
                    }
                }
                catch (Exception ex)
                {
                    throw new Volo.Abp.UserFriendlyException($"选择文件错误：{ex.Message}，请重试！");
                }
            }
        }


        public string Name { get; set; }
    }

}
