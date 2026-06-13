using Microsoft.Xrm.Sdk;
using System;
using System.Linq;

namespace D365FL.Dataverse.PluginHelper.SamplePlugin.Plugins
{
    // TODO determine how to get this into a reusable library
    public abstract class D365FLPluginBase : PluginBase
    {
        public D365FLPluginBase(Type t)
        : base(t)
        {
        }

        public IPluginExecutionContext Context { get; private set; }
        public ITracingService Tracer { get; private set; }
        public IOrganizationService InitiatingUserService { get; private set; }
        protected abstract string[] ValidateConfig();
        protected abstract void Execute();
        protected override void ExecuteDataversePlugin(ILocalPluginContext localPluginContext)
        {
            if (localPluginContext == null)
            {
                throw new ArgumentNullException(nameof(localPluginContext));
            }

            Context = localPluginContext.PluginExecutionContext;
            Tracer = localPluginContext.TracingService;
            InitiatingUserService = localPluginContext.InitiatingUserService;

            try
            {
                var erroredConfigRules = ValidateConfig();
                if (erroredConfigRules.Any())
                {
                    var formattedErroredConfigRules = string.Join(",", erroredConfigRules);
                    throw new InvalidPluginExecutionException($"Plugin is not configured correctly. Errored config rules: {formattedErroredConfigRules}");
                }

                Execute();                
            }
            catch (InvalidPluginExecutionException ex)
            {
                // Log it, but re-throw as-is — the message is already user-friendly
                Tracer.Trace("Validation/plugin error: {0}", ex.ToString());
                throw;
            }
            catch (Exception ex)
            {
                // Unexpected error — log and wrap with a safe user-facing message
                Tracer.Trace("Plugin Error: {0}", ex.ToString());
                throw new InvalidPluginExecutionException("An error occurred in the plug-in.", ex);
            }
        }
    }
}
