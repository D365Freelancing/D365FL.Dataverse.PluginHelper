using Microsoft.Xrm.Sdk;

namespace D365FL.Dataverse.PluginHelper.Core.TracingServiceExtension
{
    public static class TraceWithKeyExtension
    {
        public static void TraceWithKey(this ITracingService tracer, string key, string message, params object[] args)
        {
            var formattedMessage = string.Format(message, args);
            tracer.Trace($"[{key}] {formattedMessage}");
        }
    }
}
