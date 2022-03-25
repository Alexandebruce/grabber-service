using System.Threading.Tasks;

namespace GrabberService.Service.Interfaces
{
    public interface IGismeteoGetter
    {
        Task<string> GetMainPage();
        Task<string> GetCityWeatherPage(string cityName);
    }
}