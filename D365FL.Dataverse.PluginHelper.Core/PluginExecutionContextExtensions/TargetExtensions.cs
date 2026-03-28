using D365FL.Dataverse.PluginHelper.Core.TracingServiceExtension;
using Microsoft.Xrm.Sdk;
using System;
using System.Runtime.Remoting.Services;

namespace D365FL.Dataverse.PluginHelper.Core.PluginExecutionContextExtensions
{
    internal struct InputParameterNames
    {
        internal const string Target = "Target";
    }

    public static class TargetExtensions
    {

        internal static T GetInputParameter<T>(this IPluginExecutionContext context, string parameterKey)
        {
            if(!context.InputParameterExists(parameterKey))
            {
                throw new ArgumentException($"Input Parameter Key \"{parameterKey}\" does not exist ");
            }

            if (context.InputParameterIsType<T>(parameterKey) is T)
            {
                var typeName = typeof(T).FullName;
                throw new ArgumentException($"Input Parameter Key \"{parameterKey}\" is not of type \"{typeName}\" ");
            }

            return (T)context.InputParameters[parameterKey];
        }

        internal static bool InputParameterExists(this IPluginExecutionContext context, string parameterKey)
        {
            return context.InputParameters.ContainsKey(parameterKey);
        }

        internal static bool InputParameterIsType<T>(this IPluginExecutionContext context, string parameterKey)
        {
            return context.InputParameters[parameterKey] is T;
        }

        internal static bool HasTarget<T>(this IPluginExecutionContext context)
        {
            return 
                context.InputParameterExists(InputParameterNames.Target) &&
                context.InputParameterIsType<T>(InputParameterNames.Target);
        }

        public static bool HasTargetEntity(this IPluginExecutionContext context)
        {
            return HasTarget<Entity>(context);
        }

        public static bool HasTargetEntityReference(this IPluginExecutionContext context)
        {
            return HasTarget<EntityReference>(context);
        }

        public static Entity GetTargetEntity(this IPluginExecutionContext context, ITracingService tracer = null)
        {
            var entity = context.GetInputParameter<Entity>(InputParameterNames.Target);

            if (tracer != null) tracer.TraceEntity(entity, "Target");

            return entity;
        }

        public static EntityReference GetTargetEntityReference(this IPluginExecutionContext context)
        {
            return context.GetInputParameter<EntityReference>(InputParameterNames.Target);
        }
    }
}