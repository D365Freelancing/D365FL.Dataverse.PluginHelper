using System;
using D365FL.Dataverse.PluginHelper.Core.EntityExtensions;
using D365FL.Dataverse.PluginHelper.Core.PluginExecutionContextExtensions;
using D365FL.Dataverse.PluginHelper.Core.Rules;
using D365FL.Dataverse.PluginHelper.Core.TracingServiceExtension;
using D365FL.Dataverse.PluginHelper.SamplePlugin.Logic.Account.Queries;
using Microsoft.Xrm.Sdk;

namespace D365FL.Dataverse.PluginHelper.SamplePlugin.Plugins.Contact
{
    public class Contact_PostOperation_Update_Sync : PluginBase
    {
        public Contact_PostOperation_Update_Sync(string unsecureConfiguration, string secureConfiguration)
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
                .AddIsUpdateMessageRule()
                .AddDoesNotExceedMaxDepthRule(3)
                .AddHasPreImageRule()
                .TraceRules();

            if (!rules.IsValid)
            {
                throw new SystemException("Plugin is not configured correctly");
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
            var preImage = context.GetPreImage(tracer);
            try
            {

                if (CalculateContactCountTriggered(target, preImage, tracer))
                {
                    var counter = new ContactCounterForAccountQuery(localPluginContext.InitiatingUserService);

                    // save new count on the old account
                    var parentCustomerId = GetParentCustomerId(target, tracer);
                    var newAccountToUpdate = new Entity("account", parentCustomerId);
                    newAccountToUpdate["d365fl_contactcount"] = counter.GetContactCountFor(parentCustomerId);
                    localPluginContext.InitiatingUserService.Update(newAccountToUpdate);

                    // save new count on the original account
                    var preImageparentCustomerId = GetParentCustomerId(preImage);
                    var originalAccountToUpdate = new Entity("account", preImageparentCustomerId);
                    originalAccountToUpdate["d365fl_contactcount"] = counter.GetContactCountFor(preImageparentCustomerId);
                    localPluginContext.InitiatingUserService.Update(originalAccountToUpdate);
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

        private Guid GetParentCustomerId(Entity target, ITracingService tracer = null)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));

            var parentCustomer = target.GetAttributeValue<EntityReference>("parentcustomerid");

            tracer?.TraceEntityReference(parentCustomer, "EntRef-parentcustomerid");

            // If parent customer was not set
            if (parentCustomer == null)
                return Guid.Empty;
            // If parent customer is for a Contact (and not an Account)
            if (!parentCustomer.LogicalName.Equals("account", StringComparison.OrdinalIgnoreCase))
                return Guid.Empty;

            return parentCustomer.Id;
        }

        private bool CalculateContactCountTriggered(Entity target, Entity preImage, ITracingService tracer = null)
        {
            // Check fields impacting account name have changed before recalculating the name.
            var requiredFields = new[] { "parentcustomerid" };

            var triggered = preImage.HaveAnyFieldsChanged(target, requiredFields, tracer);

            tracer?.Trace($"CalculateContactCountTriggered : {triggered}");

            return triggered;
        }
    }
}