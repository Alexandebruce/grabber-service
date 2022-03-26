using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrabberService.Service.Interfaces
{
    public interface IGismeteoGetter
    {
        Task<string> GetMainPage();
        Task<KeyValuePair<string, string>> GetCityWeatherPage(string cityLink, Dictionary<string, string> mainCities);
    }
}