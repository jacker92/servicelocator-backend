namespace ServiceLocatorBackend.Models
{
    public class HelsinkiServiceResponse
    {
        public int Count { get; set; }
        public string Next { get; set; }
        public string Previous { get; set; }
        public object[] Results { get; set; }
    }
}