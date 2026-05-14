using D365FL.Dataverse.PluginHelper.SamplePlugin.Logic.Account.Commands;
using Microsoft.Xrm.Sdk;
using D365FL.Dataverse.PluginHelper.Core.EntityExtensions;

namespace D365FL.Dataverse.PluginHelper.SamplePlugin.Plugins.Account
{
    internal class AccountPreOperationPluginHelper
    {
        private readonly ITracingService _tracer = null;
        internal AccountPreOperationPluginHelper(ITracingService tracer = null)
        {
            _tracer = tracer;

        }

        internal void ValidateRequiredFields(Entity target)
        {
            var validator = new ValidateAccountRequiredFieldsCommand(_tracer);
            validator.ValidateRequiredFields(target);
        }

        internal void SetName(Entity fullAccountEntity, Entity accountToUpdate)
        {
            var nameCalculator = new AccountNameCalculator(_tracer);
            var name = nameCalculator.CalculateName(fullAccountEntity);
            _tracer?.Trace($"account name {name}");
            accountToUpdate["name"] = name;
        }

        internal void DefaultContactCountToZero(Entity accountToUpdate)
        {
            accountToUpdate["d365fl_contactcount"] = 0;
        }

        internal Entity CopyChangedFieldsToTarget(Entity target, Entity accountToUpdate)
        {
            var deltas = target.GetDirtyFields(accountToUpdate);
            deltas.CopyAttributeValues(target, _tracer);
            return deltas;
        }

        internal bool AreNameFieldsDirty(Entity target, Entity preImage)
        {
            // Check fields impacting account name have changed before recalculating the name.
            var triggered = preImage.IsDirty(target, new[] { "tickersymbol", "telephone1" });

            return triggered;
        }
    }
}
