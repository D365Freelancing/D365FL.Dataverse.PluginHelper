using D365FL.Dataverse.PluginHelper.Core.PluginExecutionContextExtensions;
using D365FL.Dataverse.PluginHelper.Core.EntityExtensions;
using D365FL.Dataverse.PluginHelper.Core.Rules;

namespace D365FL.Dataverse.PluginHelper.SamplePlugin.Plugins.Account
{
    public class Account_PreOperation_Create_Sync : D365FLPluginBase
    {
        public Account_PreOperation_Create_Sync(string unsecureConfiguration, string secureConfiguration)
           : base(typeof(Account_PreOperation_Create_Sync))
        {

        }
        protected override string[] ValidateConfig()
        {
            var rules = new RuleFactory(base.Context, base.Tracer);
            rules.AddIsPreOperationRule()
                .AddIsSynchronousRule()
                .AddHasTargetEntityRule()
                .AddTargetEntityLogicalNameRule("account")
                .AddIsCreateMessageRule()
                .AddDoesNotExceedMaxDepthRule(3)
                .TraceRules();

            return rules.Errors;
        }

        protected override void Execute()
        {
            var target = base.Context.GetTargetEntity(base.Tracer);
            var accountToUpdate = target.EmptyClone();

            var helper = new AccountPreOperationPluginHelper(base.Tracer);

            helper.ValidateRequiredFields(target);

            helper.SetName(target, accountToUpdate); // Name calculation fields are required, therefore no SetNameTriggered check is required
            helper.DefaultContactCountToZero(accountToUpdate); // default the contact count to 0

            // Copy changed field value to target if they have changed - Pre-Operation writes target back to the database automatically
            helper.CopyChangedFieldsToTarget(target, accountToUpdate);
        }
    }
}