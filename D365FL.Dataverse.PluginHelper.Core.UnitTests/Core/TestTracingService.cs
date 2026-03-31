using Microsoft.Xrm.Sdk;
using System.Text;

namespace D365FL.Dataverse.PluginHelper.Core.UnitTests.Core
{
    internal class TestTracingService : ITracingService
    {
        private readonly StringBuilder _traceText;

        internal TestTracingService()
        {
            _traceText = new StringBuilder();
        }
        public void Trace(string format, params object[] args)
        {
            _traceText
                .AppendFormat(format, args)
                .AppendLine();
        }

        internal string TraceLogs => _traceText.ToString();
    }
}
