using System.Net;
using GrabberService.Properties;
using System.Text;
using System.Threading.Tasks;
using GrabberService.Dao.Interfaces;

namespace GrabberService.Dao
{
    public class HttpClient : IHttpClient
    {
        private readonly string mainPageAddress;

        public HttpClient(AppSettings appSettings)
        {
            mainPageAddress = appSettings.GismeteoUrl ?? string.Empty;
        }
        
        public async Task<string> Get(string address)
        {
            string fullAddress = $"{mainPageAddress}{address}";
            using var client = new WebClient { Encoding = Encoding.UTF8 };
            var result = await client.DownloadStringTaskAsync(fullAddress);

            return string.IsNullOrWhiteSpace(result) ? string.Empty : result;
        }
    }
}