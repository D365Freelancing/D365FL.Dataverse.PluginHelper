// PURPOSE
// 1. Provide descriptive methods to make reading the code easier
// 2. Provide descriptive methods to ensure no logical mistakes are made, and therefore avoid defects eg. context.Mesage = "Creat"


// Goal to have helper classes that provide highly readably, less error prone and self documenting code
// highly readably - The helper class makes the code highly readably with very descriptve method names. 
// less error prone - The helper class makes the code less error prone by having specific methods which removes the need for magic strings
// self documenting code - plugins and importantly plugin configuration is documented within the plugin code.
// If you have ever lost your plugin steps and had to re register them, then you will apprciate the plugin rules validating the plugin step
// registration config (NOTE this does rely on developers implementing the plugin rules correctly


// Entity Field Value Has Changed
// AttributeHasChanged - https://github.com/emerbrito/XrmUtils-Extensions/blob/master/src/XrmUtils.Extensions/Extensions/EntityExtensions.cs#L84
using System;
using D365FL.Dataverse.PluginHelper.Core.EntityExtensions;
using D365FL.Dataverse.PluginHelper.Core.PluginExecutionContextExtensions;
using D365FL.Dataverse.PluginHelper.Core.Rules;
using Microsoft.Xrm.Sdk;

namespace D365FL.Dataverse.PluginHelper.SamplePlugin
{

 
    /// <summary>
    /// Plugin development guide: https://docs.microsoft.com/powerapps/developer/common-data-service/plug-ins
    /// Best practices and guidance: https://docs.microsoft.com/powerapps/developer/common-data-service/best-practices/business-logic/
    /// </summary>
    /// 
    public class Plugin1 : PluginBase
    {
        public Plugin1(string unsecureConfiguration, string secureConfiguration)
            : base(typeof(Plugin1))
        {
            // TODO: Implement your custom configuration handling
            // https://docs.microsoft.com/powerapps/developer/common-data-service/register-plug-in#set-configuration-data
        }

        private void ValidateConfig(IPluginExecutionContext context, ITracingService tracingService)
        {
            var rules = new RuleFactory(context, tracingService);
            rules.AddIsPostOperationRule()
                .AddIsAsynchronousRule()
                .AddHasTargetEntityRule()
                .AddTargetEntityLogicalNameRule("account")
                .AddIsCreateMessageRule()
                .AddDoesNotExceedMaxDepthRule(3)
                .AddHasPreImageRule()
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
            var preImage = context.GetPreImage();

            var triggered = ValidateHasBeenTriggered(target, preImage);
            if (!triggered) {
                return; // exit plugin as triggered fields have not changed
            }

            // Execute Logic
            
            // Only save modified fields
            
            // ValidateConfig

        }
    }
}

//var pluginUserService = localPluginContext.PluginUserService;
//var adminOrgService = localPluginContext.OrgSvcFactory.CreateOrganizationService(null);
//var adminOrgService2 = localPluginContext.ServiceProvider.GetAdminOrgService();

//var userId = Guid.NewGuid();
//var orgServiceAs = localPluginContext.OrgSvcFactory.CreateOrganizationService(userId);
//var orgServiceAs2 = localPluginContext.ServiceProvider.GetOrgServiceAs(userId);