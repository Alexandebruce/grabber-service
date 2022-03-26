using System.Collections.Generic;
using System.Threading.Tasks;
using GrabberService.Dao.Interfaces;
using GrabberService.Service.Interfaces;

namespace GrabberService.Service
{
    public class GismeteoGetter : IGismeteoGetter
    {
        private readonly IHttpClient httpClient;
        private const string Postfix = "10-days/";
        
        public GismeteoGetter(IHttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<string> GetMainPage()
        {
            return await httpClient.Get(string.Empty);
        }

        public async Task<KeyValuePair<string,string>> GetCityWeatherPage(string cityLink, Dictionary<string, string> mainCities)
        {
            return new (mainCities[cityLink], await httpClient.Get($"{cityLink}{Postfix}")) ;
        }
    }
}