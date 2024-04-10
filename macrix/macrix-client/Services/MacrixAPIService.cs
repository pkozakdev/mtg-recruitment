using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace macrix_client.Data
{

    public interface IMacrixAPIService
    {
        Task<string> CallRestMethod(RestMethod method, int userId = 0, User user = null);
    }
    internal class MacrixAPIService : IMacrixAPIService
    {

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private Uri _baseUri { get; set; }
        private static BasicCredentials _credentials { get; set; }


        public MacrixAPIService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = new ConfigurationBuilder().AddJsonFile($"appsettings.json").Build();
            _credentials = _configuration.GetSection("BasicCredentials").Get<BasicCredentials>();
            _baseUri = new Uri(_configuration.GetSection("APISettings").GetValue<string>("BaseApiUrl"));
        }

        public async Task<string> CallRestMethod(RestMethod method, int userId = 0, User user = null)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    "Basic", Convert.ToBase64String(
                        ASCIIEncoding.ASCII.GetBytes($"{_credentials.Username}:{_credentials.Password}")));
                client.DefaultRequestHeaders.Add("User-Agent", "PostmanRuntime/7.29.2");
                client.DefaultRequestHeaders.Add("Accept", "*/*");
                client.DefaultRequestHeaders.Add("Cache-control", "no-cache");
                client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
                client.DefaultRequestHeaders.Add("Connection", "keep-alive");
                client.BaseAddress = _baseUri;
                var apiUrl = _baseUri + "api/User" + (userId != 0 ? "/" + userId : "");


                if (method == RestMethod.GET)
                {

                    using HttpResponseMessage response = await client.GetAsync(apiUrl);
                    return await response.Content.ReadAsStringAsync();
                }
                else if (method == RestMethod.POST)
                {
                    var content = JsonConvert.SerializeObject(user);
                    var bytes = Encoding.UTF8.GetBytes(content);
                    var byteContent = new ByteArrayContent(bytes);
                    byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    using HttpResponseMessage response = await client.PostAsync(apiUrl, byteContent);
                    return await response.Content.ReadAsStringAsync();

                }
                else if (method == RestMethod.DELETE)
                {
                    if (userId == 0)
                    {
                        return "DELETE method requires a userId";
                    }
                    else
                    {
                        using HttpResponseMessage response = await client.DeleteAsync(apiUrl);
                        return await response.Content.ReadAsStringAsync();
                    }

                }
                else
                {
                    return "Invalid method specified - Allowed: GET, POST, DELETE";
                }

            }
            catch (Exception ex)
            {
                return await Task.FromException<string>(ex);
            }

        }

    }

}

