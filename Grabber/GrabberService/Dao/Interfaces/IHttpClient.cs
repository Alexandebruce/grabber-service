using System.Threading.Tasks;

namespace GrabberService.Dao.Interfaces
{
    public interface IHttpClient
    {
        Task<string> Get(string address);
    }
}