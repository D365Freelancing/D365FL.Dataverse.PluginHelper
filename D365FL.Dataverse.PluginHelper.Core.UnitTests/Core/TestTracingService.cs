using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D365FL.Dataverse.PluginHelper.Core.UnitTests.Core
{
    public class TestTracingService : ITracingService
    {
        private readonly StringBuilder _traceText;

        public TestTracingService()
        {
            _traceText = new StringBuilder();
        }
        public void Trace(string format, params object[] args)
        {
            _traceText
                .AppendFormat(format, args)
                .AppendLine();
        }

        public string TraceLogs
        {
            get
            {
                return _traceText.ToString();
            }
        }
    }
}
