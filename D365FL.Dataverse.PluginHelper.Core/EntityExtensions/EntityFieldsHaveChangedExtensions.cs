using Microsoft.Xrm.Sdk;
using System;
using System.Linq;

namespace D365FL.Dataverse.PluginHelper.Core.EntityExtensions
{
    // TODO: Add unit tests, especially for EntityReference, OptionSetValue, and Money field comparisons
    public static class EntityFieldsHaveChangedExtensions
    {
        public static bool HasFieldChanged(
            this Entity originalEntity,
            Entity modifiedEntity,
            string fieldName,
            ITracingService tracer = null,
            string tracingLabel = "HasFieldChanged")
        {
            if (originalEntity == null) throw new ArgumentNullException(nameof(originalEntity));
            if (modifiedEntity == null) throw new ArgumentNullException(nameof(modifiedEntity));
            if (string.IsNullOrEmpty(fieldName)) throw new ArgumentException("fieldName cannot be null or empty.", nameof(fieldName));

            // TODO: Use GetAttributeValue to get values and compare
            var originalValue = originalEntity.Contains(fieldName) ? originalEntity[fieldName] : null;
            var modifiedValue = modifiedEntity.Contains(fieldName) ? modifiedEntity[fieldName] : null;

            var changed = !Equals(originalValue, modifiedValue);

            tracer?.Trace($"{tracingLabel} [{fieldName}] HasFieldChanged");
            tracer?.Trace($"{tracingLabel}   originalValue: {originalValue}");
            tracer?.Trace($"{tracingLabel}   modifiedValue: {modifiedValue}");
            tracer?.Trace($"{tracingLabel}   changed: {changed}");

            return changed;
        }

        public static bool HaveAnyFieldsChanged(
            this Entity originalEntity,
            Entity modifiedEntity,
            string[] fieldNames,
            ITracingService tracer = null,
            string tracingLabel = "HaveAnyFieldsChanged")
        {
            if (originalEntity == null) throw new ArgumentNullException(nameof(originalEntity));
            if (modifiedEntity == null) throw new ArgumentNullException(nameof(modifiedEntity));
            if (fieldNames == null) throw new ArgumentNullException(nameof(fieldNames));

            var anyFieldsHaveChanged = fieldNames
                .Any(fieldName => originalEntity.HasFieldChanged(modifiedEntity, fieldName, tracer, tracingLabel));

            tracer?.Trace($"{tracingLabel}   anyFieldsChanged: {anyFieldsHaveChanged}");

            return anyFieldsHaveChanged;
        }
    }
}
