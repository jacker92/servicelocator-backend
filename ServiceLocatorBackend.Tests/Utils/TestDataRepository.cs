using ServiceLocatorBackend.Models;

namespace ServiceLocatorBackend.Tests.Utils
{
    public static class TestDataRepository
    {
        public static HelsinkiServiceResponse CreateHelsinkiServiceResponse()
        {
            return new HelsinkiServiceResponse()
            {
                Count = 1
            };

        }
    }
}
