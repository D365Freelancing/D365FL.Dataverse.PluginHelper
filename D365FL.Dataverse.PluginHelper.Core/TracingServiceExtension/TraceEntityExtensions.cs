using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;

namespace D365FL.Dataverse.PluginHelper.Core.TracingServiceExtension
{
    public static class TraceEntityExtensions
    {
        public static void TraceEntity(this ITracingService tracingService, Entity entity, string label = "Entity")
        {
            if (entity == null)
            {
                tracingService.Trace($"[{label}] Entity is null.");
                return;
            }

            tracingService.Trace($"[{label}] LogicalName: {entity.LogicalName}");
            tracingService.Trace($"[{label}] Id: {entity.Id}");
            tracingService.Trace($"[{label}] Attribute Count: {entity.Attributes.Count}");

            foreach (var attribute in entity.Attributes)
            {
                var attributeName = attribute.Key;
                var attributeValue = GetAttributeValue(attribute);
                var attributeType = GetAttributeType(attribute);

                tracingService.Trace($"[{label}]  {attributeName}: [{attributeType}] {attributeValue}");
            }
        }

        private static string GetAttributeType(KeyValuePair<string, object> attribute)
        {
            return attribute.Value?.GetType().Name ?? "null";
        }

        private static string GetAttributeValue(KeyValuePair<string, object> attr)
        {
            var attributeValue = "";
            switch (attr.Value)
            {
                case EntityReference er:
                    var name = !string.IsNullOrEmpty(er.Name) ? er.Name : "NOT SET";
                    attributeValue = $"EntityReference(LogicalName={er.LogicalName}, Id={er.Id}, Name={name})";
                    break;
                case OptionSetValue osv:
                    attributeValue = $"OptionSetValue({osv.Value})";
                    break;
                case Money money:
                    attributeValue = $"Money({money.Value})";
                    break;
                case AliasedValue av:
                    attributeValue = $"AliasedValue({av.EntityLogicalName}.{av.AttributeLogicalName} = {av.Value})";
                    break;
                case null:
                    attributeValue = "null";
                    break;
                default:
                    attributeValue = attr.Value.ToString();
                    break;
            }

            return attributeValue;
        }

        public static void TraceEntityReference(this ITracingService tracingService, EntityReference entityRef, string label = "EntityReference")
        {

            if (entityRef == null)
            {
                tracingService.Trace($"[{label}] is null.");
                return;
            }

            tracingService.Trace($"[{label}] ===== EntityReference Trace =====");
            tracingService.Trace($"[{label}] LogicalName : {entityRef.LogicalName ?? "(null)"}");
            tracingService.Trace($"[{label}] Id          : {entityRef.Id}");
            tracingService.Trace($"[{label}] Name        : {entityRef.Name ?? "(null)"}");
            tracingService.Trace($"[{label}] KeyAttributes Count: {entityRef.KeyAttributes?.Count ?? 0}");

            if (entityRef.KeyAttributes != null && entityRef.KeyAttributes.Count > 0)
            {
                foreach (var key in entityRef.KeyAttributes)
                {
                    tracingService.Trace($"[{label}]   KeyAttribute - {key.Key}: {key.Value ?? "(null)"}");
                }
            }

            tracingService.Trace($"[{label}] ===== End of EntityReference =====");
        }
    }
}
