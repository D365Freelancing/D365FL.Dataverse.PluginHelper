using System;
using D365FL.Dataverse.PluginHelper.Core.PluginExecutionContextExtensions;
using D365FL.Dataverse.PluginHelper.Core.Rules;
using D365FL.Dataverse.PluginHelper.SamplePlugin.Logic.Account.Commands;
using Microsoft.Xrm.Sdk;

namespace D365FL.Dataverse.PluginHelper.SamplePlugin.Plugins.Contact
{
    public class Contact_PostOperation_Update_Sync : PluginBase
    {
        public Contact_PostOperation_Update_Sync(string unsecureConfiguration, string secureConfiguration)
          : base(typeof(Contact_PostOperation_Update_Sync))
        {

        }
        private void ValidateConfig(IPluginExecutionContext context, ITracingService tracingService)
        {
            var rules = new RuleFactory(context, tracingService);
            rules.AddIsPostOperationRule()
                .AddIsSynchronousRule()
                .AddHasTargetEntityRule()
                .AddTargetEntityLogicalNameRule("contact")
                .AddIsUpdateMessageRule()
                .AddDoesNotExceedMaxDepthRule(3)
                .AddHasPreImageRule()
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

            var helper = new ContactPostOperationPluginHelper(localPluginContext.InitiatingUserService, tracer);

            var target = context.GetTargetEntity(tracer);
            var preImage = context.GetPreImage(tracer);
            try
            {

                if (helper.AreContactCountFieldsDirty(target, preImage))
                {
                    var newParentCustomerId = helper.GetParentCustomerId(target); // new account id
                    var oldParentCustomerId = helper.GetParentCustomerId(preImage); // old account id
                    helper.UpdateChildContactCountOnAccount(new Guid[] { oldParentCustomerId, newParentCustomerId }); // update child contact count on the old and new account
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