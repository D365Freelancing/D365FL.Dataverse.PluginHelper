using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;

namespace D365FL.Dataverse.PluginHelper.Core.IntegrationTests.Plugins.Account
{
    public static class AccountTestHelpers
    {
        public const string accountEntityLogicalName = "account";

        public static Entity CreateAccount(string telephone1 = "999 111", string tickerSymbol = "AA")
        {
            var account = new Entity(accountEntityLogicalName);
            account["telephone1"] = telephone1;
            account["tickersymbol"] = tickerSymbol;
            return account;
        }

        public static string CreateName(string telephone1, string tickerSymbol)
        {
            return $"{tickerSymbol} - {telephone1}";
        }

        public static CreateRequest CreateAccountInCorruptedState(
          string telephone1 = null,
          string tickerSymbol = null)
        {
            var account = new Entity(accountEntityLogicalName);
            if (telephone1 != null) account["telephone1"] = telephone1;
            if (tickerSymbol != null) account["tickersymbol"] = tickerSymbol;

            var createRequest = new CreateRequest()
            {
                Target = account,
            };
            createRequest.Parameters.Add("BypassCustomPluginExecution", true);

            return createRequest;
        }

        public static Entity UpdateAccount(Guid id, string telephone1, string tickerSymbol)
        {
            var account = new Entity(accountEntityLogicalName, id);
            account["telephone1"] = telephone1;
            account["tickersymbol"] = tickerSymbol;

            return account;
        }
    }
}
