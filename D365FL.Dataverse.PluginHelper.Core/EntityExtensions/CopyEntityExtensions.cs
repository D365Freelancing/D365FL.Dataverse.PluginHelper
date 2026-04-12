using D365FL.Dataverse.PluginHelper.Core.TracingServiceExtension;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;

namespace D365FL.Dataverse.PluginHelper.Core.EntityExtensions
{
    public static class CopyEntityExtensions
    {
        public static void CopyAttributeValues(
            this Entity sourceEntity,
            Entity targetEntity,
            ITracingService tracer = null,
            string traceLabel = "CopyFieldValues")
        {

            if (sourceEntity == null) throw new ArgumentNullException(nameof(sourceEntity));
            if (targetEntity == null) throw new ArgumentNullException(nameof(targetEntity));

            tracer?.TraceWithKey(traceLabel, "");
            tracer?.TraceWithKey(traceLabel, "Setting changed fields");
            foreach (var sourceAttribute in sourceEntity.Attributes)
            {
                var key = sourceAttribute.Key;
                var targetValue = GetTargetTraceableValue(targetEntity, key);
                var sourceValue = sourceAttribute.GetTraceableValue();
                
                var copyMessage = 
                    $"Changing target entity [{key}] value " +
                    $"from {targetValue} " +
                    $"to {sourceValue}";

                tracer?.TraceWithKey(traceLabel, copyMessage);

                targetEntity[key] = sourceEntity[key];
            }
        }
        private static string GetTargetTraceableValue(Entity entity, string key)
        {
            if (!entity.Contains(key)) return "null";
            return new KeyValuePair<string, object>(key, entity[key]).GetTraceableValue();
        }
    }
}