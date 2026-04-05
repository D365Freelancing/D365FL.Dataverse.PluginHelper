using System;
using D365FL.Dataverse.PluginHelper.Core.TestHarness.Tests.Accounts;
using Microsoft.PowerPlatform.Dataverse.Client;

namespace D365FL.Dataverse.PluginHelper.Core.TestHarness
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            string envUrl = GetEnvironmentUrl();

            // LoginPrompt=Always triggers the interactive login pop-up
            string connectionString = $@"
                AuthType=OAuth;
                Url={envUrl};
                AppId=51f81489-12ee-4a9e-aaae-a2591f45987d;
                RedirectUri=app://58145B91-0C36-4500-8554-080854F2AC97;
                LoginPrompt=Always";

            using (ServiceClient serviceClient = new ServiceClient(connectionString))
            {
                var contactsForAccountQuery = new ContactConterForAccountQueryTest(serviceClient);
                var count = contactsForAccountQuery.Get(new Guid("F0056072-772B-F111-88B3-00224814D648"));

                Console.WriteLine($"ContactConterForAccountQueryTest returned {count}");
            }

            Console.WriteLine("Press any key to exit");
            Console.Read();
        }

        private static string GetEnvironmentUrl()
        {
            var envUrl = System.Configuration.ConfigurationManager.AppSettings["EnvUrl"];
            if (string.IsNullOrEmpty(envUrl))
            {
                Console.Write("Enter your Dataverse Environment URL (e.g., https://orgname.crm.dynamics.com): ");
                envUrl = Console.ReadLine()?.Trim();
            }

            return envUrl;
        }
    }
}
