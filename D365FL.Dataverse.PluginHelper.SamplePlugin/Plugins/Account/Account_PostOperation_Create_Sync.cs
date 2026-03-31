using System;
using System.Linq;
using D365FL.Dataverse.PluginHelper.Core.EntityExtensions;
using D365FL.Dataverse.PluginHelper.Core.PluginExecutionContextExtensions;
using D365FL.Dataverse.PluginHelper.Core.Rules;
using D365FL.Dataverse.PluginHelper.Core.TracingServiceExtension;
using Microsoft.Xrm.Sdk;

namespace D365FL.Dataverse.PluginHelper.SamplePlugin.Plugins.Account
{
    public class Account_PostOperation_Create_Sync : PluginBase
    {
        public Account_PostOperation_Create_Sync(string unsecureConfiguration, string secureConfiguration)
           : base(typeof(Account_PostOperation_Create_Sync))
        {

        }
        private void ValidateConfig(IPluginExecutionContext context, ITracingService tracingService)
        {
            var rules = new RuleFactory(context, tracingService);
            rules.AddIsPostOperationRule()
                .AddIsSynchronousRule()
                .AddHasTargetEntityRule()
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

            tracingService.Trace("Execute1");
            var triggered = ValidateHasBeenTriggered(target, blankEntity);
            if (!triggered)
            {
                return; // exit plugin as triggered fields have not changed
            }

            tracingService.Trace("Execute2");
            // Execute Logic

            var accountUpdate = new Entity("account", target.Id);
            accountUpdate["name"] = "helloworld";
            
            InitiatingUserService.Update(accountUpdate);
            tracingService.Trace("updated");
            tracingService.TraceWithKey("OutputParameters", "");
            tracingService.TraceWithKey("OutputParameters", "Logging Output Parameters");
            tracingService.TraceWithKey("OutputParameters", $"Count {context.OutputParameters.Count}");
            context.OutputParameters.ToList().ForEach(p =>
            {
                tracingService.TraceWithKey("OutputParameters", $"    param: {p.Key}, value: {p.Value.ToString()} ");
            });
            // Only save modified fields

            // ValidateConfig

        }
    }
}
