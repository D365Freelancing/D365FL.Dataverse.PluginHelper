using Microsoft.Xrm.Sdk;
using System;

namespace D365FL.Dataverse.PluginHelper.Core.EntityExtensions
{
    public static class CloneEntityExtensions
    {
        public static Entity EmptyClone(this Entity originalEntity)
        {
            if (originalEntity == null) throw new ArgumentNullException(nameof(originalEntity));

            return new Entity(originalEntity.LogicalName, originalEntity.Id);
        }
    }
}
