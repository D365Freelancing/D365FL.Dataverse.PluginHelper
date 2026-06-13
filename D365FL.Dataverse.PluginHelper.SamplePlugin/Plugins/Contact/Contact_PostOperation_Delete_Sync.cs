using System;
using D365FL.Dataverse.PluginHelper.Core.PluginExecutionContextExtensions;
using D365FL.Dataverse.PluginHelper.Core.Rules;

namespace D365FL.Dataverse.PluginHelper.SamplePlugin.Plugins.Contact
{
    public class Contact_PostOperation_Delete_Sync : D365FLPluginBase
    {
        public Contact_PostOperation_Delete_Sync(string unsecureConfiguration, string secureConfiguration)
          : base(typeof(Contact_PostOperation_Delete_Sync))
        {

        }
        protected override string[] ValidateConfig()
        {
            var rules = new RuleFactory(base.Context, base.Tracer);
            rules.AddIsPostOperationRule()
                .AddIsSynchronousRule()
                .AddHasTargetEntityReferenceRule()
                .AddTargetEntityReferenceLogicalNameRule("contact")
                .AddIsDeleteMessageRule()
                .AddDoesNotExceedMaxDepthRule(3)
                .AddHasPreImageRule()
                .TraceRules();

            return rules.Errors;
        }

        protected override void Execute()
        {
            var preImage = base.Context.GetPreImage(base.Tracer);

            var helper = new ContactPostOperationPluginHelper(base.InitiatingUserService, base.Tracer);

            if (helper.AreContactCountFieldsDirty(preImage))
            {
                var parentCustomerId = helper.GetParentCustomerId(preImage);
                helper.UpdateChildContactCountOnAccount(new Guid[] { parentCustomerId });
            }
        }
    }
}