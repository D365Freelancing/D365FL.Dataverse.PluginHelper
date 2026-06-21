using System;
using D365FL.Dataverse.PluginHelper.Core.PluginExecutionContextExtensions;
using D365FL.Dataverse.PluginHelper.Core.Rules;

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
                var accountIds = new[]
                {
                    helper.GetParentCustomerId(target),   // new account id (Guid.Empty if non-account)
                    helper.GetParentCustomerId(preImage)  // old account id (Guid.Empty if non-account)
                };

                // Empty/non-account ids are filtered inside the helper.
                helper.UpdateChildContactCountOnAccount(accountIds);
            }
        }
    }
}