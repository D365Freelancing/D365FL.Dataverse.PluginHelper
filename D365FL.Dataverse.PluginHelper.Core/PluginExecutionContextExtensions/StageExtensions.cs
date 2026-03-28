using Microsoft.Xrm.Sdk;

namespace D365FL.Dataverse.PluginHelper.Core.PluginExecutionContextExtensions
{
    internal struct PluginStage
    {
        internal const int PreValidation = 10;
        internal const int PreOperation = 20;
        internal const int PostOperation = 40;
    }

    public static class StageExtensions
    {
        internal static bool IsStage(this IPluginExecutionContext context, int expectedStage)
        {
            return context.Stage == expectedStage;
        }
        public static bool IsPreValidation(this IPluginExecutionContext context)
        {
            return context.IsStage(PluginStage.PreValidation);
        }

        public static bool IsPreOperation(this IPluginExecutionContext context)
        {
            return context.IsStage(PluginStage.PreOperation);
        }

        public static bool IsPostOperation(this IPluginExecutionContext context)
        {
            return context.IsStage(PluginStage.PostOperation);
        }
    }
}
