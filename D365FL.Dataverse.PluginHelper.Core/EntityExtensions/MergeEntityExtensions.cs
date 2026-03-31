using D365FL.Dataverse.PluginHelper.Core.TracingServiceExtension;
using Microsoft.Xrm.Sdk;
using System;

namespace D365FL.Dataverse.PluginHelper.Core.EntityExtensions
{
    public static class MergeEntityExtensions
    {
        public static Entity Merge(
            this Entity baseEntity, 
            Entity overrides,
            ITracingService tracer = null,
            string tracingLabel = "MergedEntity")
        {
            if (baseEntity == null) throw new ArgumentNullException(nameof(baseEntity));
            if (overrides == null) throw new ArgumentNullException(nameof(overrides));

            // In Update plugins, target entity only contains fields included in the update payload
            // Merging the target with a pre or post image will ensure you have all field values.

            var merged = new Entity(baseEntity.LogicalName, baseEntity.Id);
            
            foreach (var attr in baseEntity.Attributes)
                merged[attr.Key] = attr.Value;

            // Fields in overrides take precedence over fields in baseEntity
            foreach (var attr in overrides.Attributes)
                merged[attr.Key] = attr.Value;

            tracer?.TraceEntity(merged, tracingLabel);

            return merged;
        }
    }
}
