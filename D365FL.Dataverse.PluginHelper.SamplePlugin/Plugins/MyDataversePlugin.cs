using D365FL.Dataverse.PluginHelper.Core.EntityExtensions;
using D365FL.Dataverse.PluginHelper.Core.PluginExecutionContextExtensions;
using D365FL.Dataverse.PluginHelper.Core.Rules;

namespace D365FL.Dataverse.PluginHelper.SamplePlugin.Plugins
{
    public class MyDataversePlugin : D365FLPluginBase
    {
        public MyDataversePlugin() : base(typeof(MyDataversePlugin))
        {
        }

        protected override string[] ValidateConfig()
        {
            var rules = new RuleFactory(Context, Tracer);
            rules.AddIsPreOperationRule()
                .AddIsSynchronousRule()
                .AddHasTargetEntityRule()
                .AddTargetEntityLogicalNameRule("myentity")
                .AddIsUpdateMessageRule()
                .AddDoesNotExceedMaxDepthRule(3)
                .AddHasPreImageRule();

            return rules.Errors;
        }

        protected override void Execute()
        {
            var target = base.Context.GetTargetEntity();
            var preImage = Context.GetPreImage(Tracer);

            // Merge preImage and target entity to ensure logic does not fail because of missing field values
            // its not used in this sample, but is included for demonstration purposes.
            var fullEntity = preImage.Merge(target, base.Tracer);

            // if required fields are dirty
            if (preImage.IsDirty(target, new[] { "field1", "field2", "field3" }))
            {
                base.Tracer.Trace("execute custom logic");
                // Then execute business logic
                // ... and perform operation on the target entity

                target["field1"] = "updated value";
                target["field2"] = "updated value";
                target["field3"] = "updated value";

                // Get changed fields as an entity
                var deltas = target.GetDirtyFields(fullEntity);
                
                // Save Changes
                InitiatingUserService.Update(deltas);
            }
        }
    }
}