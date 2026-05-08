using Microsoft.Xrm.Sdk;
using System;
using D365FL.Dataverse.PluginHelper.Core.TracingServiceExtension;
using System.Linq;
using D365FL.Dataverse.PluginHelper.Core.EntityExtensions;
using D365FL.Dataverse.PluginHelper.SamplePlugin.Logic.Account.Commands;

namespace D365FL.Dataverse.PluginHelper.SamplePlugin.Plugins.Contact
{
    internal class ContactPostOperationPluginHelper
    {
        private readonly IOrganizationService _orgService = null;
        private readonly ITracingService _tracer = null;
        internal ContactPostOperationPluginHelper(IOrganizationService orgService, ITracingService tracer = null) {
            _orgService = orgService;
            _tracer = tracer;

        }
        internal Guid GetParentCustomerId(Entity target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));

            var parentCustomer = target.GetAttributeValue<EntityReference>("parentcustomerid");

            _tracer?.TraceEntityReference(parentCustomer, "EntRef-parentcustomerid");

            // If parent customer was not set
            if (parentCustomer == null)
                return Guid.Empty;
            // If parent customer is for a Contact (and not an Account)
            if (!parentCustomer.LogicalName.Equals("account", StringComparison.OrdinalIgnoreCase))
                return Guid.Empty;

            return parentCustomer.Id;
        }

        internal bool CalculateContactCountTriggered(Entity entity)
        {
            // Check fields impacting account name have changed before recalculating the name.
            var requiredFields = new[] { "parentcustomerid" };

            var triggered = requiredFields.All(rf => entity.Contains(rf)); // if all required fields are in the pre image

            _tracer?.Trace($"CalculateContactCountTriggered : {triggered}");

            return triggered;
        }

        internal bool CalculateContactCountTriggered(Entity target, Entity preImage)
        {
            // Check fields impacting account name have changed before recalculating the name.
            var requiredFields = new[] { "parentcustomerid" };

            var triggered = preImage.HaveAnyFieldsChanged(target, requiredFields, _tracer);

            _tracer?.Trace($"CalculateContactCountTriggered : {triggered}");
            return triggered;
        }

        internal void UpdateChildContactCountOnAccount(Guid[] accountsToUpdate)
        {
            var counter = new SetChildContactCountCommand(_orgService, _tracer);

            foreach (var accountId in accountsToUpdate)
                counter.Execute(accountId);
        }
    }
}
