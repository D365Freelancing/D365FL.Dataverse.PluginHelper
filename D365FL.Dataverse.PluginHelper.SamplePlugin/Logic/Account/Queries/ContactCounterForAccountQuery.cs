using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using D365FL.Dataverse.PluginHelper.Core.TracingServiceExtension;

namespace D365FL.Dataverse.PluginHelper.SamplePlugin.Logic.Account.Queries
{
    public class ContactCounterForAccountQuery
    {
        private readonly IOrganizationService _orgService = null;
        private readonly ITracingService _tracer = null;

        public ContactCounterForAccountQuery(IOrganizationService orgService, ITracingService tracer = null)
        {
            if (orgService == null) throw new ArgumentNullException(nameof(orgService));

            _orgService = orgService;
            _tracer = tracer;
        }

        public int GetContactCountFor(Guid parentCustomerId)
        {
            if (parentCustomerId == Guid.Empty)
                throw new ArgumentException("parentCustomerId cannot be empty.", nameof(parentCustomerId));

            _tracer?.TraceWithKey("GetContactCountFor", $"GetContactCountFor parentCustomerId: {parentCustomerId}");

            // https://learn.microsoft.com/en-us/power-apps/developer/data-platform/org-service/queryexpression/aggregate-data

            var query = new QueryExpression("contact")
            {

                ColumnSet = new ColumnSet(false)
                {
                    AttributeExpressions = {
                        new XrmAttributeExpression(
                            attributeName: "contactid",
                            alias: "Count",
                            aggregateType: XrmAggregateType.Count)
                    }
                },
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                    {
                        new ConditionExpression("parentcustomerid", ConditionOperator.Equal, parentCustomerId)
                    }
                }
            };

            var results = _orgService.RetrieveMultiple(query);

            if (results.Entities.Count == 0)
                return 0;

            var count = (int)((AliasedValue)results.Entities[0]["Count"]).Value;


            _tracer?.TraceWithKey("GetContactCountFor", $"contactCount: {count}");

            return count;
        }
    }
}
