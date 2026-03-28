using Microsoft.Xrm.Sdk;
using System;
using D365FL.Dataverse.PluginHelper.Core.TracingServiceExtension;

namespace D365FL.Dataverse.PluginHelper.SamplePlugin.Logic.Account
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

            _tracer?.TraceWithKey("CalculateAccountName", "calculating name");

            string tickerSymbol = account.Contains("tickersymbol") ? account["tickersymbol"].ToString() : null;
            string phone = account.Contains("telephone1") ? account["telephone1"].ToString() : null;

            if (tickerSymbol == null || phone == null)
                throw new ArgumentException("tickerSymbol and phone are required to calculate account name");

            _tracer?.TraceWithKey("CalculateAccountName", "name calculated");
            return $"{tickerSymbol} - {phone}";
        }
    }
}
