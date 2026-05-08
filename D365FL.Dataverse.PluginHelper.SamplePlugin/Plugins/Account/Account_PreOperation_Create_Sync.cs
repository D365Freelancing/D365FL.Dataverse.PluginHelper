using System;
using D365FL.Dataverse.PluginHelper.Core.EntityExtensions;
using D365FL.Dataverse.PluginHelper.Core.PluginExecutionContextExtensions;
using D365FL.Dataverse.PluginHelper.Core.Rules;
using Microsoft.Xrm.Sdk;

namespace D365FL.Dataverse.PluginHelper.SamplePlugin.Plugins.Account
{
    public class Account_PreOperation_Create_Sync : PluginBase
    {
        public Account_PreOperation_Create_Sync(string unsecureConfiguration, string secureConfiguration)
           : base(typeof(Account_PreOperation_Create_Sync))
        {

        }
        private void ValidateConfig(IPluginExecutionContext context, ITracingService tracingService)
        {
            var rules = new RuleFactory(context, tracingService);
            rules.AddIsPreOperationRule()
                .AddIsSynchronousRule()
                .AddHasTargetEntityRule()
                .AddTargetEntityLogicalNameRule("account")
                .AddIsCreateMessageRule()
                .AddDoesNotExceedMaxDepthRule(3)
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
            var tracer = localPluginContext.TracingService;

            ValidateConfig(context, tracer);

            var target = context.GetTargetEntity(tracer);
            var accountToUpdate = target.EmptyClone();

            var helper = new AccountPreOperationPluginHelper(tracer);
            try
            {
                helper.ValidateRequiredFields(target);

                // Assign directly to target —
                // Pre-Operation writes back to the database automatically

                helper.SetName(target, accountToUpdate); // Name calculation fields are required, therefore no SetNameTriggered check is required

                helper.DefaultContactCountToZero(accountToUpdate); // default the contact count to 0

                // Copy changed field value to target if they have changed.
                // Pre-Operation writes target back to the database automatically
                helper.CopyChangedFieldsToTarget(target, accountToUpdate);
            }
            catch (InvalidPluginExecutionException ex)
            {
                // Log it, but re-throw as-is — the message is already user-friendly
                tracer.Trace("Validation/plugin error: {0}", ex.ToString());
                throw;
            }
            catch (Exception ex)
            {
                // Unexpected error — log and wrap with a safe user-facing message
                tracer.Trace("Plugin Error: {0}", ex.ToString());
                throw new InvalidPluginExecutionException("An error occurred in the plug-in.", ex);
            }
        }
    }
}