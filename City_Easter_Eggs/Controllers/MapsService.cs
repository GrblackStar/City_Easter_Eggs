namespace City_Easter_Eggs.Controllers
{
    public class MapsService
    {
        private string _apiKey;

        public MapsService(IConfiguration config)
        {
            IConfigurationSection mapConfig = config.GetSection("Maps");
            _apiKey = mapConfig["APIKey"] ?? "";
        }

        public string GetAPIKey()
        {
            return _apiKey;
        }
    }
}