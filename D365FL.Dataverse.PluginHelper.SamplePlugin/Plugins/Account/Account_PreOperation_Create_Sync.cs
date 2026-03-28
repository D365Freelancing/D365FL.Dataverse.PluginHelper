using System;
using System.Linq;
using D365FL.Dataverse.PluginHelper.Core.PluginExecutionContextExtensions;
using D365FL.Dataverse.PluginHelper.Core.Rules;
using D365FL.Dataverse.PluginHelper.SamplePlugin.Logic.Account;
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

        private void ValidateRequiredFields(Entity target)
        {
            // If required fields have NOT been set, display a validation message to the user
            // and prevent further processing, as missing fields will cause unhandled exceptions.
            var requiredFields = new string[] { "tickersymbol", "telephone1" };

            var missingFields = requiredFields
                .Where(field => !target.Contains(field) || string.IsNullOrEmpty(target.GetAttributeValue<string>(field)))
                .ToList();

            if (missingFields.Count > 0)
            {
                var missingFieldsText = string.Join(", ", missingFields);
                var errorMessage = $"Cannot create Account — the following required fields are missing or empty: {missingFieldsText}";
                throw new InvalidPluginExecutionException(errorMessage);
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

            ValidateRequiredFields(target);
                       
            SetName(target, tracingService);
        }

        private static void SetName(Entity target, ITracingService tracingService)
        {
            var nameCalculator = new AccountNameCalculator(tracingService);
            var newName = nameCalculator.CalculateName(target);

            // Assign directly to target — Pre-Operation writes back to the database automatically
            target["name"] = newName;
        }
    }
}
