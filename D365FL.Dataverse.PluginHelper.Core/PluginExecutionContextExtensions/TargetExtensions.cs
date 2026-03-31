using D365FL.Dataverse.PluginHelper.Core.TracingServiceExtension;
using Microsoft.Xrm.Sdk;
using System;

namespace D365FL.Dataverse.PluginHelper.Core.PluginExecutionContextExtensions
{
    internal struct InputParameterNames
    {
        internal const string Target = "Target";
    }

    public static class TargetExtensions
    {
        // TODO Get ouput parameter
        // TODO create unit test
        internal static T GetInputParameter<T>(this IPluginExecutionContext context, string parameterKey)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (string.IsNullOrEmpty(parameterKey)) 
                throw new ArgumentException($"parameterKey cannot be null or empty", nameof(parameterKey));
            if (!context.InputParameterExists(parameterKey))
                throw new ArgumentException($"Input Parameter Key \"{parameterKey}\" does not exist");
            if (!context.InputParameterIsType<T>(parameterKey))
                throw new ArgumentException($"Input Parameter Key \"{parameterKey}\" is not of type \"{typeof(T).FullName}\"");

            return (T)context.InputParameters[parameterKey];
        }

        internal static bool InputParameterExists(this IPluginExecutionContext context, string parameterKey)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            return context.InputParameters.ContainsKey(parameterKey);
        }

        internal static bool InputParameterIsType<T>(this IPluginExecutionContext context, string parameterKey)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            return context.InputParameters[parameterKey] is T;
        }

        public static bool HasTargetEntity(this IPluginExecutionContext context)
        {
            return
                context.InputParameterExists(InputParameterNames.Target) &&
                context.InputParameterIsType<Entity>(InputParameterNames.Target);
        }

        public static bool HasTargetEntityReference(this IPluginExecutionContext context)
        {
            return
               context.InputParameterExists(InputParameterNames.Target) &&
               context.InputParameterIsType<EntityReference>(InputParameterNames.Target);
        }

        public static Entity GetTargetEntity(this IPluginExecutionContext context, ITracingService tracer = null)
        {
            var entity = context.GetInputParameter<Entity>(InputParameterNames.Target);

            tracer?.TraceEntity(entity);
            return entity;
        }

        public static EntityReference GetTargetEntityReference(this IPluginExecutionContext context, ITracingService tracer = null)
        {
            var entityReference = context.GetInputParameter<EntityReference>(InputParameterNames.Target);

            tracer?.TraceEntityReference(entityReference);

            return entityReference;
        }
    }
}