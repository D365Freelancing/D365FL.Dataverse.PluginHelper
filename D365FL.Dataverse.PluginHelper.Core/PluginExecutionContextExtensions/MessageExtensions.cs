using Microsoft.Xrm.Sdk;
using System;

namespace D365FL.Dataverse.PluginHelper.Core.PluginExecutionContextExtensions
{
    internal struct PluginMessage
    {
        internal const string Create = "Create";
        internal const string Update = "Update";
        internal const string Delete = "Delete";
        internal const string Associate = "Associate";
    }

    public static class MessageExtensions
    {
        internal static bool IsMessage(this IPluginExecutionContext context, string expectedMessage)
        {
            return context.MessageName.Equals(expectedMessage, StringComparison.InvariantCultureIgnoreCase);
        }
        public static bool IsCreateMessage(this IPluginExecutionContext context)
        {
            return context.IsMessage(PluginMessage.Create);
        }

        public static bool IsUpdateMessage(this IPluginExecutionContext context)
        {
            return context.IsMessage(PluginMessage.Update);
        }

        public static bool IsDeleteMessage(this IPluginExecutionContext context)
        {
            return context.IsMessage(PluginMessage.Delete);
        }

        public static bool IsAssociateMessage(this IPluginExecutionContext context)
        {
            return context.IsMessage(PluginMessage.Associate);
        }
    }
}
