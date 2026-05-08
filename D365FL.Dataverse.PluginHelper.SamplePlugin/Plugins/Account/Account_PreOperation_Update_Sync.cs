using System;
using D365FL.Dataverse.PluginHelper.Core.EntityExtensions;
using D365FL.Dataverse.PluginHelper.Core.PluginExecutionContextExtensions;
using D365FL.Dataverse.PluginHelper.Core.Rules;
using Microsoft.Xrm.Sdk;
using System.Text.Json;

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
            var accountUpdates = target.EmptyClone();

            // Merge preImage and target entity to ensure logic does not fail because of missing field values
            var fullEntity = preImage.Merge(target, tracer);

            var helper = new AccountPreOperationPluginHelper(tracer);

            try
            {
                helper.ValidateRequiredFields(fullEntity);

                if (helper.AreNameFieldsDirty(target, preImage))
                    helper.SetName(fullEntity, accountUpdates);

                // Copy changed field value to target if they have changed.
                // Pre-Operation writes target back to the database automatically
                helper.CopyChangedFieldsToTarget(target, accountUpdates);

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