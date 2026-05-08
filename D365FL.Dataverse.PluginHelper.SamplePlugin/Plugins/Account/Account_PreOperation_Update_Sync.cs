using D365FL.Dataverse.PluginHelper.Core.EntityExtensions;
using D365FL.Dataverse.PluginHelper.Core.PluginExecutionContextExtensions;
using D365FL.Dataverse.PluginHelper.Core.Rules;
using System.Text.Json;

namespace D365FL.Dataverse.PluginHelper.SamplePlugin.Plugins.Account
{
    public class Account_PreOperation_Update_Sync : D365FLPluginBase
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
        protected override bool ValidateConfig()
        {
            var rules = new RuleFactory(base.Context, base.Tracer);
            rules.AddIsPreOperationRule()
                .AddIsSynchronousRule()
                .AddHasTargetEntityRule()
                .AddTargetEntityLogicalNameRule("account")
                .AddIsUpdateMessageRule()
                .AddDoesNotExceedMaxDepthRule(_secureConfig.MaxRetries)
                .AddHasPreImageRule()
                .TraceRules();

            return rules.IsValid;
        }

        protected override void Execute()
        {
            var target = base.Context.GetTargetEntity(base.Tracer);
            var preImage = base.Context.GetPreImage(base.Tracer);
            var accountUpdates = target.EmptyClone();

            // Merge preImage and target entity to ensure logic does not fail because of missing field values
            var fullEntity = preImage.Merge(target, base.Tracer);

            var helper = new AccountPreOperationPluginHelper(base.Tracer);

            helper.ValidateRequiredFields(fullEntity);

            if (helper.AreNameFieldsDirty(target, preImage))
                helper.SetName(fullEntity, accountUpdates);

            // Copy changed field value to target if they have changed. - Pre-Operation writes target back to the database automatically
            helper.CopyChangedFieldsToTarget(target, accountUpdates);
        }
    }
}