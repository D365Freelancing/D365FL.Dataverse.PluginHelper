using Microsoft.Xrm.Sdk;
using System;
using System.Linq;

namespace D365FL.Dataverse.PluginHelper.SamplePlugin.Logic.Account.Commands
{
    public class ValidateRequiredFieldsCommand
    {
        private readonly ITracingService _tracer = null;

        public ValidateRequiredFieldsCommand(ITracingService tracer = null)
        {
            _tracer = tracer;
        }
        public void ValidateRequiredFields(Entity target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));

            // TODO make this a generic class to be used accross multiple entities
            // TODO add tracing comments if tracer is not null

            // If required fields have NOT been set, display a validation message to the user
            // and prevent further processing, as missing fields will cause unhandled exceptions.

            // account required fields
            var requiredFields = new string[] { "tickersymbol", "telephone1" };

            var missingFields = requiredFields
                .Where(field => !target.Contains(field)
                    || target[field] == null
                    || (target[field] is string strValue && string.IsNullOrEmpty(strValue)))
                .ToList();

            if (missingFields.Count > 0)
            {
                var missingFieldsText = string.Join(", ", missingFields);
                var errorMessage = $"Cannot save Account — the following required fields are missing or empty: {missingFieldsText}";
                throw new InvalidPluginExecutionException(errorMessage);
            }
        }
    }
}
