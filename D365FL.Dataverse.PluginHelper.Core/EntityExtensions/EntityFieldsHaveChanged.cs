using Microsoft.Xrm.Sdk;
using System.Linq;

namespace D365FL.Dataverse.PluginHelper.Core.EntityExtensions
{
    public static class EntityFieldsHaveChanged
    {
        public static bool HasFieldChanged(this Entity originalEntity, Entity modifiedEntity, string fieldName)
        {
            var originalValue = originalEntity.Contains(fieldName) ? originalEntity[fieldName] : null;
            var modifiedValue = modifiedEntity.Contains(fieldName) ? modifiedEntity[fieldName] : null;

            var changed = !Equals(originalValue, modifiedValue);
 
            return changed;
        }

        public static bool HaveAnyFieldsChanged(this Entity originalEntity, Entity modifiedEntity, string[] fieldNames)
        {
            var anyFieldsHaveChanged = 
                fieldNames
                .ToList()
                .Any(fieldName=>originalEntity.HasFieldChanged(modifiedEntity, fieldName));

            return anyFieldsHaveChanged;
        }
    }
}
