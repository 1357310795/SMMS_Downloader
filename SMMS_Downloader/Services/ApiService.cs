using System.Net.Http;
using System.Net;
using Teru.Code.Models;
using Newtonsoft.Json;
using SQLite;

namespace SMMS_Downloader.Services
{
    public class ApiService
    {
        public static bool IsLogined { get; set; }
        public static string Token { get; set; }
        public static CommonResult Login(string username, string password)
        {
            HttpClient client = new HttpClient(new HttpClientHandler() { AllowAutoRedirect = true, MaxAutomaticRedirections = 10, AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip }) { BaseAddress = new Uri("https://sm.ms") };

            Dictionary<string, string> forms = new Dictionary<string, string>();
            forms.Add("username", username);
            forms.Add("password", password);
            FormUrlEncodedContent content = new FormUrlEncodedContent(forms);
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, "/api/v2/token");
            req.Content = content;
            var res = client.SendAsync(req).Result;
            if (!res.IsSuccessStatusCode)
            {
                return new(false, $"请求失败，服务器响应{res.StatusCode}");
            }

            var json = JsonConvert.DeserializeObject<SmmsLoginResDto>(res.Content.ReadAsStringAsync().Result);

            if (!json.Success)
            {
                return new(false, $"登录失败：{json.Message}");
            }

            IsLogined = true;
            Token = json.Data.Token;

            return new(true, "登录成功");
        }

        public static CommonResult<SmmsUploadHistoryResDto> GetUploadHistory(int page)
        {
            HttpClient client = new HttpClient(new HttpClientHandler() { AllowAutoRedirect = true, MaxAutomaticRedirections = 10, AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip }) { BaseAddress = new Uri("https://sm.ms") };

            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, $"/api/v2/upload_history?page={page}");
            req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Token);
            var res = client.SendAsync(req).Result;
            if (!res.IsSuccessStatusCode)
            {
                return new(false, $"请求失败，服务器响应{res.StatusCode}");
            }

            var json = JsonConvert.DeserializeObject<SmmsUploadHistoryResDto>(res.Content.ReadAsStringAsync().Result);

            if (!json.Success)
            {
                return new(false, $"登录失败：{json.Message}");
            }

            return new(true, "登录成功", json);
        }
    }

    public partial class SmmsLoginResDto
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("data")]
        public SmmsLoginData Data { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("RequestId")]
        public string RequestId { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }
    }

    public partial class SmmsLoginData
    {
        [JsonProperty("token")]
        public string Token { get; set; }
    }

    public partial class SmmsUploadHistoryResDto
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("Count")]
        public int Count { get; set; }

        [JsonProperty("CurrentPage")]
        public int CurrentPage { get; set; }

        [JsonProperty("data")]
        public List<SmmsUploadHistoryItem> Data { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("PerPage")]
        public int PerPage { get; set; }

        [JsonProperty("RequestId")]
        public string RequestId { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("TotalPages")]
        public int TotalPages { get; set; }
    }

    public partial class SmmsUploadHistoryItem
    {
        [AutoIncrement]
        [PrimaryKey]
        public int Id { get; set; }

        public int State { get; set; }

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty("delete")]
        public string Delete { get; set; }

        [JsonProperty("filename")]
        public string Filename { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("page")]
        public string Page { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("size")]
        public long Size { get; set; }

        [JsonProperty("storename")]
        public string Storename { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }
    }
}
