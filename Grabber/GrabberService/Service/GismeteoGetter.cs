using System.Threading.Tasks;
using GrabberService.Dao.Interfaces;
using GrabberService.Service.Interfaces;

namespace GrabberService.Service
{
    public class GismeteoGetter : IGismeteoGetter
    {
        private readonly IHttpClient httpClient;

        public GismeteoGetter(IHttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<string> GetMainPage()
        {
            return await httpClient.Get(string.Empty);
        }

        public async Task<string> GetCityWeatherPage(string cityName)
        {
            return await httpClient.Get("/weather-volgograd-5089/10-days/");
        }
    }
}