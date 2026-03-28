using Microsoft.Xrm.Sdk;

namespace D365FL.Dataverse.PluginHelper.Core.PluginExecutionContextExtensions
{
    internal struct PluginMode
    {
        internal const int synchronous = 0;
        internal const int Asynchronous = 1;
    }

    public static class ModeExtensions
    {
        internal static bool IsMode(this IPluginExecutionContext context, int expectedMode)
        {
            return context.Mode == expectedMode;
        }
        public static bool IsAsynchronous(this IPluginExecutionContext context)
        {
            return context.IsMode(PluginMode.Asynchronous);
        }
        public static bool IsSynchronous(this IPluginExecutionContext context)
        {
            return context.IsMode(PluginMode.synchronous);
        }
    }
}
