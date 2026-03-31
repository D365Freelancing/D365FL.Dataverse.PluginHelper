using Microsoft.Xrm.Sdk;
using System;

namespace D365FL.Dataverse.PluginHelper.Core.TracingServiceExtension
{
    public static class TraceWithKeyExtension
    {
        public static void TraceWithKey(this ITracingService tracer, string key, string message, params object[] args)
        {
            if (tracer == null) throw new ArgumentNullException(nameof(tracer));
            if (string.IsNullOrEmpty(key)) throw new ArgumentException("key cannot be null or empty.", nameof(key));
            if (message == null) throw new ArgumentNullException(nameof(message));

            var formattedMessage = string.Format(message, args);
            tracer.Trace($"[{key}] {formattedMessage}");
        }
    }
}