using Microsoft.Extensions.Configuration;

namespace NerdStore.BDD.Tests.Config
{
    public class ConfigurationHelper
    {
       private readonly IConfiguration _config;

        public ConfigurationHelper()
        {
            _config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
        }

        public string WebDrivers => $"{_config.GetSection("WebDrivers").Value}";
    }
}