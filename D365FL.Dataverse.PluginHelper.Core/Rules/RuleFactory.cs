using D365FL.Dataverse.PluginHelper.Core.PluginExecutionContextExtensions;
using D365FL.Dataverse.PluginHelper.Core.TracingServiceExtension;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;

namespace D365FL.Dataverse.PluginHelper.Core.Rules
{
    public class RuleFactory
    {
        private readonly IPluginExecutionContext _context;
        private readonly ITracingService _tracer;
        private Dictionary<string, bool> _rules;
        public RuleFactory(
            IPluginExecutionContext context,
            ITracingService tracer = null)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            _context = context;
            _tracer = tracer;
            _rules = new Dictionary<string, bool>();
        }

        #region "Operation Rules"

        public RuleFactory AddIsPreValidationRule()
        {           
            _rules.Add("IsPreValidation", _context.IsPreValidation());

            return this;
        }
        public RuleFactory AddIsPreOperationRule()
        {
            _rules.Add("IsPreOperation", _context.IsPreOperation());

            return this;
        }
        public RuleFactory AddIsPostOperationRule()
        {
            _rules.Add("IsPostOperation", _context.IsPostOperation());

            return this;
        }

        #endregion

        #region "Target Rules"
        public RuleFactory AddHasTargetEntityRule()
        {
            _rules.Add("HasTargetEntity", _context.HasTargetEntity());

            return this;
        }
        public RuleFactory AddHasTargetEntityReferenceRule()
        {
            _rules.Add("HasTargetEntityReference", _context.HasTargetEntityReference());

            return this;
        }
        public RuleFactory AddTargetEntityLogicalNameRule(string expectedName)
        {
            
            var ruleName = $"HasTargetEntityLogicalName_{expectedName}";

            var isValid = false;
            if (_context.HasTargetEntity())
                isValid = _context.GetTargetEntity().LogicalName == expectedName;
            
            _rules.Add(ruleName, isValid);

            return this;
        }

        public RuleFactory AddTargetEntityReferenceLogicalNameRule(string expectedName)
        {

            var ruleName = $"HasTargetEntityReferenceLogicalName_{expectedName}";

            var isValid = false;
            if (_context.HasTargetEntityReference())
                isValid = _context.GetTargetEntityReference().LogicalName == expectedName;

            _rules.Add(ruleName, isValid);

            return this;
        }
        #endregion

        #region "Mode Rules"
        public RuleFactory AddIsAsynchronousRule()
        {
            _rules.Add("IsAsynchronous", _context.IsAsynchronous());
            return this;
        }

        public RuleFactory AddIsSynchronousRule()
        {
            _rules.Add("IsSynchronous", _context.IsSynchronous());
            return this;
        }

        #endregion

        #region "Message Rules"
        public RuleFactory AddIsCreateMessageRule()
        {
            _rules.Add("IsCreateMessage", _context.IsCreateMessage());
            return this;
        }

        public RuleFactory AddIsUpdateMessageRule()
        {
            _rules.Add("IsUpdateMessage", _context.IsUpdateMessage());
            return this;
        }

        public RuleFactory AddIsDeleteMessageRule()
        {
            _rules.Add("IsDeleteMessage", _context.IsDeleteMessage());
            return this;
        }

        public RuleFactory AddIsAssociateRule()
        {
            _rules.Add("IsAssociateMessage", _context.IsAssociateMessage());
            return this;
        }


        #endregion

        #region "Depth Rules"

        public RuleFactory AddDoesNotExceedMaxDepthRule(int maxDepthLimit)
        {
            _rules.Add($"DoesNotExceedMaxDepth_Of_{maxDepthLimit}", _context.Depth <= maxDepthLimit);
            return this;
        }

        #endregion

        #region "Image Rules"

        public RuleFactory AddHasPostImageRule(string imageName)
        {
            _rules.Add($"HasPostImage_Of_{imageName}", _context.HasPostImage(imageName));
            return this;
        }

        public RuleFactory AddHasPostImageRule()
        {
            _rules.Add($"HasPostImage", _context.HasPostImage());
            return this;
        }

        public RuleFactory AddHasPreImageRule(string imageName)
        {
            _rules.Add($"HasPreImage_Of_{imageName}", _context.HasPreImage(imageName));
            return this;
        }

        public RuleFactory AddHasPreImageRule()
        {
            _rules.Add($"HasPreImage", _context.HasPreImage());
            return this;
        }

        #endregion


        public IReadOnlyDictionary<string, bool> GetRuleDictionary() => _rules;

        public bool IsValid => _rules.All(r => r.Value);

        public string[] Errors => _rules.Where(r => !r.Value).Select(r => r.Key).ToArray();

        public void TraceRules(string tracingLabel = "RuleValidation")
        {
            var invalidRules = GetRuleDictionary().Where(r => !r.Value).ToList();
            var validRules = GetRuleDictionary().Where(r => r.Value).ToList();

            _tracer?.TraceWithKey(tracingLabel, $"Config Rules are Valid: {IsValid}");
            
            if (!IsValid)
            {
                _tracer?.TraceWithKey(tracingLabel, "  FAILED RULES");

                invalidRules.ForEach(r => _tracer?.TraceWithKey(tracingLabel, $"    {r.Key}"));
            }

            _tracer?.TraceWithKey(tracingLabel, "VALID RULES");
            validRules.ForEach(r => _tracer?.TraceWithKey(tracingLabel, $"    {r.Key}"));
        }
    }
}
