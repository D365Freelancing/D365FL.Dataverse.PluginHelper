using System;
using D365FL.Dataverse.PluginHelper.Core.EntityExtensions;
using D365FL.Dataverse.PluginHelper.Core.PluginExecutionContextExtensions;
using D365FL.Dataverse.PluginHelper.Core.Rules;
using D365FL.Dataverse.PluginHelper.Core.TracingServiceExtension;
using Microsoft.Xrm.Sdk;

namespace D365FL.Dataverse.PluginHelper.SamplePlugin.Account
{
    public class Account_PostOperation_Create_Sync : PluginBase
    {
        public Account_PostOperation_Create_Sync(string unsecureConfiguration, string secureConfiguration)
           : base(typeof(Account_PostOperation_Create_Sync))
        {
            // TODO: Implement your custom configuration handling
            // https://docs.microsoft.com/powerapps/developer/common-data-service/register-plug-in#set-configuration-data
        }
        private void ValidateConfig(IPluginExecutionContext context, ITracingService tracingService)
        {
            var rules = new RuleFactory(context, tracingService);
            rules.AddIsPostOperationRule()
                .AddIsSynchronousRule()
                .AddHasTargertEntityRule()
                .AddTargetEntityLogicalNameRule("account")
                .AddIsCreateMessageRule()
                .AddDoesNotExceedMaxDepthRule(3)
                .TraceRules();

            if (!rules.IsValid)
            {
                throw new SystemException("Plugin is not configured correctly");
            }
        }

        private bool ValidateHasBeenTriggered(Entity target, Entity image)
        {
            var watchedFields = new string[] { "company", "phoneNumber" }; // TODO test Lookup, Money, OptionSet

            var triggered = target.HaveAnyFieldsChanged(image, watchedFields);

            return triggered;
        }


        // Entry point for custom business logic execution
        protected override void ExecuteDataversePlugin(ILocalPluginContext localPluginContext)
        {
            if (localPluginContext == null)
            {
                throw new ArgumentNullException(nameof(localPluginContext));
            }

            var context = localPluginContext.PluginExecutionContext;
            var InitiatingUserService = localPluginContext.InitiatingUserService;
            var tracingService = localPluginContext.TracingService;

            ValidateConfig(context, tracingService);

            var target = context.GetTargetEntity();
            var blankEntity = new Entity();


            var triggered = ValidateHasBeenTriggered(target, blankEntity);
            if (!triggered)
            {
                return; // exit plugin as triggered fields have not changed
            }

            // Execute Logic

            // Only save modified fields

            // ValidateConfig

        }
    }
}
