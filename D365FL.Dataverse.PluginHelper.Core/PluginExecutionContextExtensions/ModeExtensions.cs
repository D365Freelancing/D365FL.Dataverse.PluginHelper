using Microsoft.Xrm.Sdk;
using System;

namespace D365FL.Dataverse.PluginHelper.Core.PluginExecutionContextExtensions
{
    internal struct PluginMode
    {
        internal const int Synchronous = 0;
        internal const int Asynchronous = 1;
    }

    public static class ModeExtensions
    {
        internal static bool IsMode(this IPluginExecutionContext context, int expectedMode)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            return context.Mode == expectedMode;
        }
        public static bool IsAsynchronous(this IPluginExecutionContext context)
        {
            return context.IsMode(PluginMode.Asynchronous);
        }
        public static bool IsSynchronous(this IPluginExecutionContext context)
        {
            return context.IsMode(PluginMode.Synchronous);
        }
    }
}