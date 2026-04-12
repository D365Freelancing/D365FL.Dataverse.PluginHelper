using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;

namespace D365FL.Dataverse.PluginHelper.Core.TracingServiceExtension
{
    public static class TraceEntityExtensions
    {
        public static void TraceEntity(this ITracingService tracer, Entity entity, string label = "Entity")
        {
            if (tracer == null) throw new ArgumentNullException(nameof(tracer));

            tracer.TraceWithKey(label, "");

            if (entity == null)
            {
                tracer.TraceWithKey(label, "Entity is null.");
                return;
            }

            tracer.TraceWithKey(label, $"LogicalName: {entity.LogicalName}");
            tracer.TraceWithKey(label, $"Id: {entity.Id}");
            tracer.TraceWithKey(label, $"Attribute Count: {entity.Attributes.Count}");

            foreach (var attribute in entity.Attributes)
            {
                var attributeName = attribute.Key;
                var attributeValue = attribute.GetTraceableValue();
                var attributeType = GetAttributeType(attribute);

                tracer.TraceWithKey(label, $" {attributeName}: [{attributeType}] {attributeValue}");
            }
        }

        private static string GetAttributeType(KeyValuePair<string, object> attribute)
        {
            return attribute.Value?.GetType().Name ?? "null";
        }

        public static void TraceEntityReference(this ITracingService tracer, EntityReference entityRef, string label = "EntityReference")
        {
            if (tracer == null) throw new ArgumentNullException(nameof(tracer));
            
            tracer.TraceWithKey(label, "");

            if (entityRef == null)
            {
                tracer.TraceWithKey(label, "EntityReference is null.");
                return;
            }

            tracer.TraceWithKey(label, $"===== EntityReference Trace =====");
            tracer.TraceWithKey(label, $"LogicalName : {entityRef.LogicalName ?? "(null)"}");
            tracer.TraceWithKey(label, $"Id          : {entityRef.Id}");
            tracer.TraceWithKey(label, $"Name        : {entityRef.Name ?? "(null)"}");
            tracer.TraceWithKey(label, $"KeyAttributes Count: {entityRef.KeyAttributes?.Count ?? 0}");

            if (entityRef.KeyAttributes != null && entityRef.KeyAttributes.Count > 0)
            {
                foreach (var key in entityRef.KeyAttributes)
                {
                    tracer.TraceWithKey(label, $"   KeyAttribute - {key.Key}: {key.Value ?? "(null)"}");
                }
            }

            tracer.TraceWithKey(label, $"===== End of EntityReference =====");
        }
    }
}