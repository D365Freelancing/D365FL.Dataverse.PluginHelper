using D365FL.Dataverse.PluginHelper.Core.TracingServiceExtension;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;

namespace D365FL.Dataverse.PluginHelper.Core.EntityExtensions
{
    public static class EntityFieldsHaveChangedExtensions
    {
        public static bool IsDirty(
            this Entity originalEntity,
            Entity modifiedEntity,
            string fieldName,
            ITracingService tracer = null,
            string tracingLabel = "IsDirty")
        {
            if (originalEntity == null) throw new ArgumentNullException(nameof(originalEntity));
            if (modifiedEntity == null) throw new ArgumentNullException(nameof(modifiedEntity));
            if (string.IsNullOrEmpty(fieldName)) throw new ArgumentException("fieldName cannot be null or empty.", nameof(fieldName));

            var originalValue = originalEntity.Contains(fieldName) ? originalEntity[fieldName] : null;
            var modifiedValue = modifiedEntity.Contains(fieldName) ? modifiedEntity[fieldName] : null;

            var isDirty = !Equals(originalValue, modifiedValue);

            tracer?.TraceWithKey(tracingLabel, $" [{fieldName}] HasFieldChanged");
            tracer?.TraceWithKey(tracingLabel, $"   originalValue: {originalValue}");
            tracer?.TraceWithKey(tracingLabel, $"   modifiedValue: {modifiedValue}");
            tracer?.TraceWithKey(tracingLabel, $"   isDirty: {isDirty}");

            return isDirty;
        }

        public static bool IsDirty(
            this Entity originalEntity,
            Entity modifiedEntity,
            string[] fieldNames,
            ITracingService tracer = null,
            string tracingLabel = "IsDirty")
        {
            if (originalEntity == null) throw new ArgumentNullException(nameof(originalEntity));
            if (modifiedEntity == null) throw new ArgumentNullException(nameof(modifiedEntity));
            if (fieldNames == null) throw new ArgumentNullException(nameof(fieldNames));
            if (fieldNames.Length == 0) throw new ArgumentException("fieldNames cannot be empty.", nameof(fieldNames));

            var isDirty = fieldNames
                .Any(fieldName => originalEntity.IsDirty(modifiedEntity, fieldName, tracer, tracingLabel));

            tracer?.TraceWithKey(tracingLabel,$"   IsDirty: {isDirty}");

            return isDirty;
        }

        public static Entity GetDirtyFields(
            this Entity originalEntity,
            Entity modifiedEntity,
            ITracingService tracer = null,
            string tracingLabel = "GetDirtyFields")
        {
            if (originalEntity == null) throw new ArgumentNullException(nameof(originalEntity));
            if (modifiedEntity == null) throw new ArgumentNullException(nameof(modifiedEntity));

            var changedEntity = new Entity(originalEntity.LogicalName, originalEntity.Id);

            foreach (var field in modifiedEntity.Attributes)
            {
                if (originalEntity.IsDirty(modifiedEntity, field.Key, tracer, tracingLabel))
                {
                    changedEntity[field.Key] = field.Value;
                }
            }

            tracer?.TraceWithKey(tracingLabel, $"Changed Field Count: {changedEntity.Attributes.Count}");

            return changedEntity;
        }
    }
}

