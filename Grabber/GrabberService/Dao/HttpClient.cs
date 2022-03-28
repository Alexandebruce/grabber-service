using System.IO;
using System.Net.Http;
using GrabberService.Properties;
using System.Threading.Tasks;
using GrabberService.Dao.Interfaces;

namespace GrabberService.Dao
{
    public class HttpClient : IHttpClient
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly string mainPageAddress;
        public HttpClient(IHttpClientFactory httpClientFactory, AppSettings appSettings)
        {
            this.httpClientFactory = httpClientFactory;
            mainPageAddress = appSettings.GismeteoUrl ?? string.Empty;
        }

        public async Task<string> Get(string address)
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, $"{mainPageAddress}/{address}");

            var httpClient = httpClientFactory.CreateClient();
            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                using var contentStream =
                    await httpResponseMessage.Content.ReadAsStreamAsync();
                
                StreamReader reader = new StreamReader(contentStream);
                return await reader.ReadToEndAsync();
            }

            return default;
        }
    }
}