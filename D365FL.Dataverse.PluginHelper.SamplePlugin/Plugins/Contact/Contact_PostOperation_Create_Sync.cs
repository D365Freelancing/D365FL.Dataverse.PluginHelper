using System;
using D365FL.Dataverse.PluginHelper.Core.PluginExecutionContextExtensions;
using D365FL.Dataverse.PluginHelper.Core.Rules;
using Microsoft.Xrm.Sdk;

namespace D365FL.Dataverse.PluginHelper.SamplePlugin.Plugins.Contact
{
    public class Contact_PostOperation_Create_Sync : PluginBase
    {
        public Contact_PostOperation_Create_Sync(string unsecureConfiguration, string secureConfiguration)
          : base(typeof(Contact_PostOperation_Create_Sync))
        {
        }
        private void ValidateConfig(IPluginExecutionContext context, ITracingService tracingService)
        {
            var rules = new RuleFactory(context, tracingService);
            rules.AddIsPostOperationRule()
                .AddIsSynchronousRule()
                .AddHasTargetEntityRule()
                .AddTargetEntityLogicalNameRule("contact")
                .AddIsCreateMessageRule()
                .AddDoesNotExceedMaxDepthRule(3)
                .TraceRules();

            if (!rules.IsValid)
            {
                throw new InvalidPluginExecutionException("Plugin is not configured correctly");
            }
        }

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
            var helper = new ContactPostOperationPluginHelper(localPluginContext.InitiatingUserService, tracer);
            try
            {
                if (helper.AreContactCountFieldsDirty(target))
                {
                    var parentCustomerId = helper.GetParentCustomerId(target);
                    helper.UpdateChildContactCountOnAccount(new Guid[] { parentCustomerId });
                }
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