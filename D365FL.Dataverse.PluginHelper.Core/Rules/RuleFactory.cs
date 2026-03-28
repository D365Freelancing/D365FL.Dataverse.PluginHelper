using D365FL.Dataverse.PluginHelper.Core.PluginExecutionContextExtensions;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using System.Linq;

namespace D365FL.Dataverse.PluginHelper.Core.Rules
{
    public class RuleFactory
    {
        private readonly IPluginExecutionContext _context;
        private readonly ITracingService _tracingService;
        private Dictionary<string, bool> _rules;
        public RuleFactory(
            IPluginExecutionContext context,
            ITracingService tracingService)
        {
            _context = context;
            _tracingService = tracingService;
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
        public RuleFactory AddHasTargertEntityRule()
        {
            _rules.Add("HasTargertEntity", _context.HasTargetEntity());

            return this;
        }
        public RuleFactory AddHasTargertEntityReferenceRule()
        {
            _rules.Add("HasTargertEntityReference", _context.HasTargetEntityReference());

            return this;
        }
        public RuleFactory AddTargetEntityLogicalNameRule(string expectedName)
        {
            _rules.Add(
                $"HasTargertEntityLogicalName_{expectedName}",
                _context.GetTargetEntity().LogicalName == expectedName);

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


        public Dictionary<string, bool> GetRuleDictionary()
        {
            return _rules;
        }

        public bool IsValid { get { return _rules.All(r => r.Value); } }

        public void TraceRules()
        {
            var invalidRules = GetRuleDictionary().Where(r => !r.Value).ToList();
            var validRules = GetRuleDictionary().Where(r => r.Value).ToList();

            _tracingService.Trace($"Config Rules are Valid: {IsValid}");
            
            if (!IsValid)
            {
                _tracingService.Trace("Config Rule FAILED");
                _tracingService.Trace("FAILED RULES");

                invalidRules.ForEach(r => _tracingService.Trace($"{r.Key}"));
            }

            _tracingService.Trace("VALID RULES");
            validRules.ToList().ForEach(r => _tracingService.Trace($"{r.Key}"));
        }
    }
}
