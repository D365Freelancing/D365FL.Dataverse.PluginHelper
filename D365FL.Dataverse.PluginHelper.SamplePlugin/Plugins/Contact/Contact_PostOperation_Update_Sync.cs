using System;
using System.Linq;
using D365FL.Dataverse.PluginHelper.Core.PluginExecutionContextExtensions;
using D365FL.Dataverse.PluginHelper.Core.Rules;
using D365FL.Dataverse.PluginHelper.SamplePlugin.Plugins.Account;

namespace D365FL.Dataverse.PluginHelper.SamplePlugin.Plugins.Contact
{
    public class Contact_PostOperation_Update_Sync : D365FLPluginBase
    {
        public Contact_PostOperation_Update_Sync(string unsecureConfiguration, string secureConfiguration)
          : base(typeof(Contact_PostOperation_Update_Sync))
        {

        }
        protected override string[] ValidateConfig()
        {
            var rules = new RuleFactory(base.Context, base.Tracer);
            rules.AddIsPostOperationRule()
                .AddIsSynchronousRule()
                .AddHasTargetEntityRule()
                .AddTargetEntityLogicalNameRule("contact")
                .AddIsUpdateMessageRule()
                .AddDoesNotExceedMaxDepthRule(3)
                .AddHasPreImageRule()
                .TraceRules();

            return rules.Errors;
        }

        protected override void Execute()
        {
            var helper = new ContactPostOperationPluginHelper(base.InitiatingUserService, base.Tracer);

            var target = base.Context.GetTargetEntity(base.Tracer);
            var preImage = base.Context.GetPreImage(base.Tracer);

            if (helper.AreContactCountFieldsDirty(target, preImage))
            {
                var accountIds = new Guid[2]; // array to hold the old and new account ids

                var newParentCustomerId = helper.GetParentCustomerId(target); // new account id
                accountIds[0] = newParentCustomerId;

                var oldParentCustomerId = helper.GetParentCustomerId(preImage); // old account id
                accountIds[1] = oldParentCustomerId;

                var nonEmptyAccountIds = 
                    accountIds.Where(id => id != Guid.Empty).Distinct().ToArray(); // remove empty ids

                helper.UpdateChildContactCountOnAccount(nonEmptyAccountIds); // update child contact count on the old and new account
            }
        }
    }
}