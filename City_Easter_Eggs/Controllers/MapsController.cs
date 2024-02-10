namespace City_Easter_Eggs.Controllers
{
    public class MapsController
    {
        private string _apiKey;

        public MapsController(IConfiguration config)
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