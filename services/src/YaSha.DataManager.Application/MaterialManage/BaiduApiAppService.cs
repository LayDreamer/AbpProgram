using Newtonsoft.Json;
using YaSha.DataManager.MaterailManage;
using YaSha.DataManager.MaterailManage.Dto;
using YaSha.DataManager.StandardAndPolicy.Dto;

namespace YaSha.DataManager.MaterialManage;

public class BaiduApiAppService: DataManagerAppService, IBaiduApiAppService
{
    const string API_KEY = "arMSjMr1G5TApIDIWko8QqI7";
    const string SECRET_KEY = "U7vGdKXQytNeZEdnKGySNqryQzK2a3qp";
    
    public async Task<ApiResultDto> CreatedImage(string filePath, string brief, string tag = "1")
    {
        try
        {
            return await Task.Run(async () =>
            {
                var token = await GetAccessToken();
                var url = $"https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/similar/add?access_token={token}";
                var httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromMinutes(1);
                var parameters = new Dictionary<string, string>
                {
                    { "image", GetFileContentAsBase64(filePath) },
                    { "brief", brief },
                    { "tags", tag }
                };
                var content = new FormUrlEncodedContent(parameters);
                var response = await httpClient.PostAsync(url, content);
                var responseText = await response.Content.ReadAsStringAsync();
                dynamic jsonResponse = JsonConvert.DeserializeObject(responseText);
                var sign = (string)jsonResponse["cont_sign"];
                return new ApiResultDto()
                {
                    Success = true,
                    Data = sign,
                };
            });
        }
        catch (Exception e)
        {
            return new ApiResultDto()
            {
                Success = false,
                Error = e.Message,
            };
        }
    }
    
    public async Task<ApiResultDto> DeleteImage(string sign)
    {
        try
        {
            return await Task.Run(async () =>
            {
                var token = await GetAccessToken();
                var url = $"https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/similar/delete?access_token={token}";
                var httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromMinutes(1);
                var parameters = new Dictionary<string, string>
                {
                    { "cont_sign", sign },
                };
                var content = new FormUrlEncodedContent(parameters);
                var response = await httpClient.PostAsync(url, content);
                var responseText = await response.Content.ReadAsStringAsync();
                dynamic jsonResponse = JsonConvert.DeserializeObject(responseText);
                return new ApiResultDto()
                {
                    Success = true,
                    Data = jsonResponse,
                };
            });
        }
        catch (Exception e)
        {
            return new ApiResultDto()
            {
                Success = false,
                Error = e.Message,
            };
        }
    }
    
    public async Task<ApiResultDto> UpdateImage(string sign, string brief, string tags)
    {
        try
        {
            return await Task.Run(async () =>
            {
                var token = await GetAccessToken();
                var url = $"https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/similar/update?access_token={token}";
                var httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromMinutes(1);
                var parameters = new Dictionary<string, string>
                {
                    { "brief", brief },
                    { "tags", tags },
                };
                var content = new FormUrlEncodedContent(parameters);
                var response = await httpClient.PostAsync(url, content);
                var responseText = await response.Content.ReadAsStringAsync();
                dynamic jsonResponse = JsonConvert.DeserializeObject(responseText);
                return new ApiResultDto()
                {
                    Success = true,
                    Data = jsonResponse,
                };
            });
        }
        catch (Exception e)
        {
            return new ApiResultDto()
            {
                Success = false,
                Error = e.Message,
            };
        }
    }
    
    public async Task<ApiResultDto> SearchImage(string filePath, string tags = "1", string logic = "1")
    {
        try
        {
            return await Task.Run(async () =>
            {
                var token = await GetAccessToken();
                var url = $"https://aip.baidubce.com/rest/2.0/image-classify/v1/realtime_search/similar/search?access_token={token}";
                var httpClient = new HttpClient();
            
                httpClient.Timeout = TimeSpan.FromMinutes(1);
                var parameters = new Dictionary<string, string>
                {
                    { "image", GetFileContentAsBase64(filePath) },
                    { "tags", tags },
                    { "tag_logic", logic }
                };
                var content = new FormUrlEncodedContent(parameters);
                var response = await httpClient.PostAsync(url, content);
                var responseText = await response.Content.ReadAsStringAsync();
                dynamic jsonResponse = JsonConvert.DeserializeObject(responseText);
                var data = new List<MaterialBaiduSearchDto>();
                foreach (var item in jsonResponse.result)
                {
                    var sign = (string)item["cont_sign"];
                    var score = (float)item["score"];
                    var brief = (string)item["brief"];
                    data.Add(new MaterialBaiduSearchDto()
                    {
                        ContSign = sign,
                        Brief = brief,
                        Score = score,
                    });
                }
                return new ApiResultDto()
                {
                    Success = true,
                    Data = data,
                };                
            });
        }
        catch (Exception e)
        {
            return new ApiResultDto()
            {
                Success = false,
                Error = e.Message,
            };
        }
    }
    
    
    async Task<string> GetAccessToken() {
        var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri("https://aip.baidubce.com/oauth/2.0/");
        httpClient.Timeout = TimeSpan.FromMinutes(1);

        var parameters = new Dictionary<string, string>
        {
            { "grant_type", "client_credentials" },
            { "client_id", API_KEY },
            { "client_secret", SECRET_KEY }
        };
        var content = new FormUrlEncodedContent(parameters);

        var response = await httpClient.PostAsync("token", content);
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<dynamic>(responseContent);
        return result.access_token.ToString();
    }
    
    string GetFileContentAsBase64(string path)
    {
        using (FileStream filestream = new FileStream(path, FileMode.Open))
        {
            byte[] arr = new byte[filestream.Length];
            filestream.Read(arr, 0, (int)filestream.Length);
            string base64 = Convert.ToBase64String(arr);
            return base64;
        }
    }
}