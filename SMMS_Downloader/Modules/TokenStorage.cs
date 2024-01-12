using Newtonsoft.Json;
using SMMS_Downloader.Services;
using System.IO;
using Teru.Code.Models;

namespace SMMS_Downloader.Modules
{
    public class TokenStorage : StorageableBase
    {
        public static TokenStorage Default = new TokenStorage();
        public override string FileName => "token.txt";
        public override StorageMode Mode => StorageMode.ProgramFolder;

        public void Save()
        {
            Save(ApiService.Token);
        }

        public CommonResult Read()
        {
            try
            {
                if (!File.Exists(base.FilePath))
                    return new CommonResult(false, "未找到文件");
                var token = base.Read();
                ApiService.Token = token;
                ApiService.IsLogined = true;
            }
            catch (Exception ex)
            {
                return new CommonResult(false, ex.Message);
            }
            return new CommonResult(true, "");
        }
    }
}
