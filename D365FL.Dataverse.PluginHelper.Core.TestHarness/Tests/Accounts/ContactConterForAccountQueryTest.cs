using System;
using Microsoft.Xrm.Sdk;
using D365FL.Dataverse.PluginHelper.SamplePlugin.Logic.Account.Queries;

namespace D365FL.Dataverse.PluginHelper.Core.TestHarness.Tests.Accounts
{
    public class ContactConterForAccountQueryTest
    {
        private readonly IOrganizationService _orgService;

        public ContactConterForAccountQueryTest(IOrganizationService orgService)
        {
            _orgService = orgService;
        }

        public int Get(Guid accountId)
        {
            var query = new ContactCounterForAccountQuery(_orgService, tracer: null);
            return query.GetContactCountFor(accountId);
        }
    }
}
