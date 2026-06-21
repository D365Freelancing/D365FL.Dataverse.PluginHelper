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

        // check if fields are dirty for a CREATE plugin
        internal bool AreContactCountFieldsDirty(Entity entity)
        {
            var countTriggerFields = new[] { "parentcustomerid" };

            var triggered = countTriggerFields.All(f => entity.Contains(f)); // if all required fields are in the entity

            _tracer?.Trace($"CalculateContactCountTriggered : {triggered}");

            return triggered;
        }

        // check if fields are dirty for a UPDATE plugin
        internal bool AreContactCountFieldsDirty(Entity target, Entity preImage)
        {
            var countTriggerFields = new[] { "parentcustomerid" };

            var triggered = preImage.IsDirty(target, countTriggerFields);

            _tracer?.Trace($"CalculateContactCountTriggered : {triggered}");
            return triggered;
        }

        internal void UpdateChildContactCountOnAccount(Guid[] accountsToUpdate)
        {
            var counter = new SetChildContactCountCommand(_orgService, _tracer);

            // Skip non-account customers (e.g. a contact-typed parentcustomerid) and nulls,
            // which arrive here as Guid.Empty. Updating an account count for them is invalid.
            var validAccountIds = accountsToUpdate.Where(id => id != Guid.Empty).Distinct();

            foreach (var accountId in validAccountIds)
                counter.Execute(accountId);
        }
    }
}