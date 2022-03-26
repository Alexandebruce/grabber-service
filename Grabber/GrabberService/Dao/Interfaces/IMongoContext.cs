using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrabberService.Dao.Interfaces
{
    public interface IMongoContext
    {
        Task Add<T>(T input);
        Task AddMany<T>(IEnumerable<T> input);
    }
}