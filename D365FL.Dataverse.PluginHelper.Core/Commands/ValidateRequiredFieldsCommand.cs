using D365FL.Dataverse.PluginHelper.Core.TracingServiceExtension;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;

namespace D365FL.Dataverse.PluginHelper.Core.Commands
{
    public class ValidateRequiredFieldsCommand
    {
        private readonly ITracingService _tracer = null;
        private const string TracerKey = "ValidateRequiredField";
        private readonly string[] _requiredFields = null;
        public ValidateRequiredFieldsCommand(string[] requiredFields, ITracingService tracer = null)
        {
            if (requiredFields == null) throw new ArgumentNullException(nameof(requiredFields));
            if (requiredFields.Length == 0) throw new ArgumentException("requiredFields cannot be empty.", nameof(requiredFields));

            _requiredFields = requiredFields;
            _tracer = tracer;
        }

        public void ValidateRequiredFields(
            Entity target,
            string entityName = null)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));

            var entityNameForMessage = GetEntityName(entityName, target);

            var missingFields = _requiredFields
                .Where(field => !target.Contains(field)
                    || target[field] == null
                    || (target[field] is string strValue && string.IsNullOrEmpty(strValue)))
                .ToList();

            _tracer?.TraceWithKey(TracerKey, $"{missingFields.Count} missing fields found.");

            if (missingFields.Count > 0)
            {

                var missingFieldsText = string.Join(", ", missingFields);

                _tracer?.TraceWithKey(TracerKey, $"Missing Fields - {missingFieldsText}.");
                var errorMessage = $"Cannot save {entityNameForMessage} — the following required fields are missing or empty: {missingFieldsText}";
                throw new InvalidPluginExecutionException(errorMessage);
            }
        }

        private string GetEntityName(string entityName, Entity target)
        {
            _tracer?.TraceWithKey(TracerKey, "Set entity name.");

            var entityNameForMessage =
                !string.IsNullOrEmpty(entityName) ? entityName : target.LogicalName;

            _tracer?.TraceWithKey(TracerKey, $"Entity name set to {entityNameForMessage}.");

            return entityNameForMessage;
        }
    }
}