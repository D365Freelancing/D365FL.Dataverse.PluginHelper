using System;
using D365FL.Dataverse.PluginHelper.Core.EntityExtensions;
using D365FL.Dataverse.PluginHelper.Core.PluginExecutionContextExtensions;
using D365FL.Dataverse.PluginHelper.Core.Rules;
using Microsoft.Xrm.Sdk;
using System.Text.Json;
using D365FL.Dataverse.PluginHelper.SamplePlugin.Logic.Account.Commands;

namespace D365FL.Dataverse.PluginHelper.SamplePlugin.Plugins.Account
{

    public class Account_PreOperation_Update_Sync : PluginBase
    {
        private class SecureConfig
        {
            public int MaxRetries { get; set; } = 1; // Default to 1 if config is not set on the plugin
        }

        private readonly SecureConfig _secureConfig = null;

        public Account_PreOperation_Update_Sync(string unsecureConfiguration, string secureConfiguration)
            : base(typeof(Account_PreOperation_Update_Sync))
        {
            _secureConfig = string.IsNullOrWhiteSpace(secureConfiguration)
                ? new SecureConfig()
                : JsonSerializer.Deserialize<SecureConfig>(
                    secureConfiguration,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        private void ValidateConfig(IPluginExecutionContext context, ITracingService tracingService)
        {
            var rules = new RuleFactory(context, tracingService);
            rules.AddIsPreOperationRule()
                .AddIsSynchronousRule()
                .AddHasTargetEntityRule()
                .AddTargetEntityLogicalNameRule("account")
                .AddIsUpdateMessageRule()
                .AddDoesNotExceedMaxDepthRule(_secureConfig.MaxRetries)
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
            var tracer = localPluginContext.TracingService;

            ValidateConfig(context, tracer);

            var target = context.GetTargetEntity(tracer);
            var preImage = context.GetPreImage(tracer);

            // Merge preImage and target entity to ensure logic does not fail because of missing field values
            var fullEntity = preImage.Merge(target, tracer);

            Execute(target, preImage, fullEntity, tracer);
        }

        private void Execute(Entity target, Entity preImage, Entity fullEntity, ITracingService tracer)
        {
            try
            {
                ValidateRequiredFields(fullEntity);
                SetName(target, preImage, fullEntity, tracer);
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

        private void SetName(Entity target, Entity preImage, Entity fullEntity, ITracingService tracer = null)
        {
            if (!SetNameTriggered(target, preImage))
            {
                // only set name if fields that are used to calculate the name have changed.
                tracer?.Trace("Set Name fields have not changed, therefore existing and NOT calculating new name");
                return;
            }

            var nameCalculator = new AccountNameCalculator(tracer);
            var newName = nameCalculator.CalculateName(fullEntity);

            // Assign to target (not fullEntity) — Pre-Operation writes target back to the database automatically
            target["name"] = newName;
        }

        private bool SetNameTriggered(Entity target, Entity preImage)
        {
            // Check fields impacting account name have changed before recalculating the name.
            var requiredFields = new[] { "tickersymbol", "telephone1" };

            var triggered = preImage.HaveAnyFieldsChanged(target, requiredFields);

            return triggered;
        }

        private void ValidateRequiredFields(Entity target, ITracingService tracer = null)
        {
            var validator = new ValidateRequiredFieldsCommand(tracer);
            validator.ValidateRequiredFields(target);
        }
    }
}