using TencentCloud.Common;
using TencentCloud.Common.Profile;
using TencentCloud.Tiia.V20190529;
using TencentCloud.Tiia.V20190529.Models;
using YaSha.DataManager.MaterailManage;
using YaSha.DataManager.StandardAndPolicy.Dto;

namespace YaSha.DataManager.MaterialManage;

public class TencentApiAppService : DataManagerAppService, ITencentApiAppService
{
    private readonly Credential cred = new() { SecretId = "AKID3ygDas7aYZY1Z6f6Siqy9DE5jNvZ3Z1a", SecretKey = "hFlHsMsFTaaTErLkBbv6BA9J4tfTI4mP" };
    private const string libId = "5c2a1252090d0be712ab43fe0f453317";

    public async Task<ApiResultDto> CreatedLibrary()
    {
        try
        {
            return await Task.Run(() =>
            {
                var clientProfile = new ClientProfile();
                var httpProfile = new HttpProfile();
                httpProfile.Endpoint = ("tiia.tencentcloudapi.com");
                clientProfile.HttpProfile = httpProfile;
                var client = new TiiaClient(cred, "ap-shanghai", clientProfile);
                var req = new CreateGroupRequest
                {
                    GroupId = libId,
                    GroupName = "gyh材质库",
                    MaxCapacity = 10000
                };
                var resp = client.CreateGroupSync(req);
                return new ApiResultDto()
                {
                    Success = true,
                    Data = resp.RequestId,
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

    public async Task<ApiResultDto> CreatedImage(string entId, string filePath)
    {
        try
        {
            return await Task.Run(() =>
            {
                var clientProfile = new ClientProfile();
                var httpProfile = new HttpProfile();
                httpProfile.Endpoint = ("tiia.tencentcloudapi.com");
                clientProfile.HttpProfile = httpProfile;
                var client = new TiiaClient(cred, "ap-shanghai", clientProfile);
                var req = new CreateImageRequest();
                req.GroupId = libId;
                req.EntityId = entId;
                req.PicName = Guid.NewGuid().ToString();
                req.ImageBase64 = GetFileContentAsBase64(filePath);
                var resp = client.CreateImageSync(req);
                return new ApiResultDto()
                {
                    Success = true,
                    Data = resp.RequestId,
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

    public async Task<ApiResultDto> DeleteImage(string entId)
    {
        try
        {
            return await Task.Run(() =>
            {
                var clientProfile = new ClientProfile();
                var httpProfile = new HttpProfile();
                httpProfile.Endpoint = ("tiia.tencentcloudapi.com");
                clientProfile.HttpProfile = httpProfile;
                var client = new TiiaClient(cred, "ap-shanghai", clientProfile);
                var req = new DeleteImagesRequest
                {
                    GroupId = libId,
                    EntityId = entId
                };
                var resp = client.DeleteImagesSync(req);
                return new ApiResultDto()
                {
                    Success = true,
                    Data = resp.RequestId,
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

    public async Task<ApiResultDto> GetLibraryInfo()
    {
        try
        {
            return await Task.Run(() =>
            {
                var clientProfile = new ClientProfile();
                var httpProfile = new HttpProfile();
                httpProfile.Endpoint = ("tiia.tencentcloudapi.com");
                clientProfile.HttpProfile = httpProfile;
                var client = new TiiaClient(cred, "ap-shanghai", clientProfile);
                var req = new DescribeGroupsRequest
                {
                    GroupId = libId
                };
                var resp = client.DescribeGroupsSync(req);
                return new ApiResultDto()
                {
                    Success = true,
                    Data = resp.Groups,
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

    public async Task<ApiResultDto> GetImageInfo(string entId)
    {
        try
        {
            return await Task.Run(() =>
            {
                var clientProfile = new ClientProfile();
                var httpProfile = new HttpProfile();
                httpProfile.Endpoint = ("tiia.tencentcloudapi.com");
                clientProfile.HttpProfile = httpProfile;
                var client = new TiiaClient(cred, "ap-shanghai", clientProfile);
                var req = new DescribeImagesRequest
                {
                    GroupId = libId,
                    EntityId = entId
                };
                var resp = client.DescribeImagesSync(req);
                return new ApiResultDto()
                {
                    Success = true,
                    Data = resp.ImageInfos,
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

    public async Task<ApiResultDto> SearchImage(string filePath)
    {
        try
        {
            return await Task.Run(() =>
            {
                var clientProfile = new ClientProfile();
                var httpProfile = new HttpProfile();
                httpProfile.Endpoint = ("tiia.tencentcloudapi.com");
                clientProfile.HttpProfile = httpProfile;
                var client = new TiiaClient(cred, "ap-shanghai", clientProfile);
                var req = new SearchImageRequest
                {
                    GroupId = libId,
                    ImageBase64 = GetFileContentAsBase64(filePath),
                    MatchThreshold = 10,
                    Limit = 100
                };
                var resp = client.SearchImageSync(req);
                return new ApiResultDto()
                {
                    Success = true,
                    Data = resp.ImageInfos,
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