using System;
using D365FL.Dataverse.PluginHelper.Core.PluginExecutionContextExtensions;
using D365FL.Dataverse.PluginHelper.Core.Rules;
using D365FL.Dataverse.PluginHelper.SamplePlugin.Logic.Account.Commands;
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

            Execute(target, tracer);
        }

        private void Execute(Entity target, ITracingService tracer)
        {
            try
            {
                ValidateRequiredFields(target);

                SetName(target, tracer);
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
        private void SetName(Entity target, ITracingService tracer)
        {
            var nameCalculator = new AccountNameCalculator(tracer);
            var newName = nameCalculator.CalculateName(target);

            // Assign directly to target — Pre-Operation writes back to the database automatically
            target["name"] = newName;
        }

        private void ValidateRequiredFields(Entity target, ITracingService tracer = null)
        {
            var validator = new ValidateAccountRequiredFieldsCommand(tracer);
            validator.ValidateRequiredFields(target);

        }
    }
}