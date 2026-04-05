using Microsoft.PowerPlatform.Dataverse.Client;
using System.Configuration;

namespace D365FL.Dataverse.PluginHelper.Core.IntegrationTests.Dependencies
{
    public static class ServiceClientFactory
    {
        public static ServiceClient GetServiceClient()
        {
            var dataverseUrl = RequiredSetting("EnvUrl");
            var clientId = RequiredSetting("ClientId");
            var clientSecret = RequiredSetting("Secret");

            var connectionString =
                $@"AuthType=ClientSecret;
                Url={dataverseUrl};
                ClientId={clientId};
                ClientSecret={clientSecret};";

            return new ServiceClient(connectionString);
        }

        private static string RequiredSetting(string key)
        {
            var value = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrWhiteSpace(value))
                throw new ConfigurationErrorsException($"Missing or empty required app setting: '{key}'");
            return value;
        }
            
    }
}
