using System;
using D365FL.Dataverse.PluginHelper.Core.EntityExtensions;
using D365FL.Dataverse.PluginHelper.Core.PluginExecutionContextExtensions;
using D365FL.Dataverse.PluginHelper.Core.Rules;
using D365FL.Dataverse.PluginHelper.SamplePlugin.Logic.Account;
using Microsoft.Xrm.Sdk;

namespace D365FL.Dataverse.PluginHelper.SamplePlugin.Plugins.Account
{
    public class Account_PreOperation_Update_Sync : PluginBase
    {
        public Account_PreOperation_Update_Sync(string unsecureConfiguration, string secureConfiguration)
            : base(typeof(Account_PreOperation_Update_Sync))
        {
        }

        private void ValidateConfig(IPluginExecutionContext context, ITracingService tracingService)
        {
            var rules = new RuleFactory(context, tracingService);
            rules.AddIsPreOperationRule()
                .AddIsSynchronousRule()
                .AddHasTargetEntityRule()
                .AddTargetEntityLogicalNameRule("account")
                .AddIsUpdateMessageRule()
                .AddDoesNotExceedMaxDepthRule(3)
                .AddHasPreImageRule()
                .TraceRules();

            if (!rules.IsValid)
            {
                throw new InvalidPluginExecutionException("Plugin is not configured correctly");
            }
        }

        // Entry point for custom business logic execution
        protected override void ExecuteDataversePlugin(ILocalPluginContext localPluginContext)
        {
            if (localPluginContext == null)
            {
                throw new ArgumentNullException(nameof(localPluginContext));
            }

            var context = localPluginContext.PluginExecutionContext;
            var tracingService = localPluginContext.TracingService;

            ValidateConfig(context, tracingService);

            var target = context.GetTargetEntity();
            var preImage = context.GetPreImage();

            // Merge preImage and target entity to ensure logic does not fail because of missing field values
            var fullEntity = preImage.CloneAndMergeEntities(target);

            if (SetNameTriggered(target, preImage))
            {
                // only set name if fields that are used to calculate the name have changed.
                SetName(target, fullEntity, tracingService);
            }
        }

        private bool SetNameTriggered(Entity target, Entity preImage)
        {
            // Check fields impacting account name have changed before recalculating the name.
            var requiredFields = new[] { "tickersymbol", "telephone1" };

            var triggered = preImage.HaveAnyFieldsChanged(target, requiredFields);

            return triggered;
        }
        private static void SetName(Entity target, Entity fullEntity, ITracingService tracingService)
        {
            var nameCalculator = new AccountNameCalculator(tracingService);
            var newName = nameCalculator.CalculateName(fullEntity);

            // Assign to target (not fullEntity) — Pre-Operation writes target back to the database automatically
            target["name"] = newName;
        }
    }
}