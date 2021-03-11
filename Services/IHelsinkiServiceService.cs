using System.Collections.Generic;
using System.Threading.Tasks;
using ServiceLocatorBackend.Models;

namespace ServiceLocatorBackend.Services
{
    public interface IHelsinkiServiceService
    {
        Task<HelsinkiServiceResponse> GetServices(string query);
    }
}