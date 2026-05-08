using D365FL.Dataverse.PluginHelper.SamplePlugin.Logic.Account.Queries;
using Microsoft.Xrm.Sdk;
using System;

namespace D365FL.Dataverse.PluginHelper.SamplePlugin.Logic.Account.Commands
{
    public class SetChildContactCountCommand
    {
        private readonly IOrganizationService _orgService = null;
        private readonly ITracingService _tracer = null;
        private readonly ContactCounterForAccountQuery _counter = null;

        public SetChildContactCountCommand(IOrganizationService orgService, ITracingService tracer = null)
        {
            _orgService = orgService;
            _tracer = tracer;
            _counter = new ContactCounterForAccountQuery(orgService, tracer);
        }

        public void Execute(Guid parentCustomerId)
        {
            // save new count on the old account
            var accountToUpdate = new Entity("account", parentCustomerId);
            accountToUpdate["d365fl_contactcount"] = _counter.GetContactCountFor(parentCustomerId);
            _orgService.Update(accountToUpdate);
        }
    }
}
