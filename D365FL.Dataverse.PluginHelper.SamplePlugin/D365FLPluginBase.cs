using Microsoft.Xrm.Sdk;
using System;

namespace D365FL.Dataverse.PluginHelper.SamplePlugin
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
        protected abstract bool ValidateConfig();
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
                if(!ValidateConfig())
                {
                    throw new InvalidPluginExecutionException("Plugin is not configured correctly");
                }

                Execute();                
            }
            catch (InvalidPluginExecutionException ex)
            {
                // Log it, but re-throw as-is — the message is already user-friendly
                this.Tracer.Trace("Validation/plugin error: {0}", ex.ToString());
                throw;
            }
            catch (Exception ex)
            {
                // Unexpected error — log and wrap with a safe user-facing message
                this.Tracer.Trace("Plugin Error: {0}", ex.ToString());
                throw new InvalidPluginExecutionException("An error occurred in the plug-in.", ex);
            }
        }


    }
}
