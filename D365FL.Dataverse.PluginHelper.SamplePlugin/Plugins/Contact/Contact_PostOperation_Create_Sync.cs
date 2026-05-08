using System;
using D365FL.Dataverse.PluginHelper.Core.PluginExecutionContextExtensions;
using D365FL.Dataverse.PluginHelper.Core.Rules;

namespace D365FL.Dataverse.PluginHelper.SamplePlugin.Plugins.Contact
{
    public class Contact_PostOperation_Create_Sync : D365FLPluginBase
    {
        public Contact_PostOperation_Create_Sync(string unsecureConfiguration, string secureConfiguration)
          : base(typeof(Contact_PostOperation_Create_Sync))
        {
        }
        protected override bool ValidateConfig()
        {
            var rules = new RuleFactory(base.Context, base.Tracer);
            rules.AddIsPostOperationRule()
                .AddIsSynchronousRule()
                .AddHasTargetEntityRule()
                .AddTargetEntityLogicalNameRule("contact")
                .AddIsCreateMessageRule()
                .AddDoesNotExceedMaxDepthRule(3)
                .TraceRules();

            return rules.IsValid;
        }

        protected override void Execute()
        {
            var target = base.Context.GetTargetEntity(base.Tracer);
            var helper = new ContactPostOperationPluginHelper(base.InitiatingUserService, base.Tracer);

            if (helper.AreContactCountFieldsDirty(target))
            {
                var parentCustomerId = helper.GetParentCustomerId(target);
                helper.UpdateChildContactCountOnAccount(new Guid[] { parentCustomerId });
            }
        }
    }
}