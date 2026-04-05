using Microsoft.Xrm.Sdk;
using System;
using D365FL.Dataverse.PluginHelper.Core.TracingServiceExtension;

namespace D365FL.Dataverse.PluginHelper.SamplePlugin.Logic.Account.Commands
{
    public class AccountNameCalculator
    {
        private readonly ITracingService _tracer = null;
        public AccountNameCalculator(ITracingService tracer = null)
        {
            _tracer = tracer;
        }

        public string CalculateName(Entity account)
        {
            if (account == null) throw new ArgumentNullException(nameof(account));

            _tracer?.TraceWithKey("CalculateAccountName", "calculating name");

            string tickerSymbol = account.GetAttributeValue<string>("tickersymbol");
            string telephone1 = account.GetAttributeValue<string>("telephone1"); 

            if (tickerSymbol == null || telephone1 == null)
                throw new ArgumentException("tickerSymbol and telephone1 are required to calculate account name");

            _tracer?.TraceWithKey("CalculateAccountName", "name calculated");
            return $"{tickerSymbol} - {telephone1}";
        }
    }
}
