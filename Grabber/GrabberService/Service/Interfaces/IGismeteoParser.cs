using System.Threading.Tasks;

namespace GrabberService.Service.Interfaces
{
    public interface IGismeteoParser
    {
        Task<string> Do();
    }
}