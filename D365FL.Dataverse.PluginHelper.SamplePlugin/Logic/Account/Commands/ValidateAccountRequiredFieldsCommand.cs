using D365FL.Dataverse.PluginHelper.Core.Commands;
using Microsoft.Xrm.Sdk;
using System;

namespace D365FL.Dataverse.PluginHelper.SamplePlugin.Logic.Account.Commands
{
    public class ValidateAccountRequiredFieldsCommand
    {
        private static readonly string[] RequiredFields = { "tickersymbol", "telephone1" };
        private readonly ValidateRequiredFieldsCommand _validator = null;
        public ValidateAccountRequiredFieldsCommand(ITracingService tracer = null)
        {
            _validator = new ValidateRequiredFieldsCommand(RequiredFields, tracer);
        }
        public void ValidateRequiredFields(Entity target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));

            _validator.ValidateRequiredFields(target, "Account");
        }
    }
}