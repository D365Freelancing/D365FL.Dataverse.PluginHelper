using D365FL.Dataverse.PluginHelper.SamplePlugin.UnitTests.TestBase;
using FakeXrmEasy.Abstractions;
using FakeXrmEasy.Plugins;
using Microsoft.Xrm.Sdk;
using System;

namespace D365FL.Dataverse.PluginHelper.SamplePlugin.UnitTests.Contact
{
    public class ContactPluginTestBase: PluginTestBase
    {
        protected Entity CreateAccountEntity(Guid accountId)
        {
            return new Entity(AccountLogicalName, accountId);
        }


        protected XrmFakedPluginExecutionContext BuildContactCreateContext(IXrmBaseContext ctx, Entity target, int depth = 1)
        {
            var pluginCtx = ctx.GetDefaultPluginContext();
            pluginCtx.MessageName = "Create";
            pluginCtx.Stage = PostOperationStage;
            pluginCtx.Mode = SynchronousMode;
            pluginCtx.Depth = depth;
            pluginCtx.PrimaryEntityName = ContactLogicalName;
            pluginCtx.InputParameters = new ParameterCollection { ["Target"] = target };
            pluginCtx.PreEntityImages = new EntityImageCollection();
            pluginCtx.PostEntityImages = new EntityImageCollection();
            return pluginCtx;
        }

        protected XrmFakedPluginExecutionContext BuildContactDeleteContext(IXrmBaseContext ctx, EntityReference targetRef, Entity preImage, int depth = 1)
        {
            var pluginCtx = ctx.GetDefaultPluginContext();
            pluginCtx.MessageName = "Delete";
            pluginCtx.Stage = PostOperationStage;
            pluginCtx.Mode = SynchronousMode;
            pluginCtx.Depth = depth;
            pluginCtx.PrimaryEntityName = ContactLogicalName;
            pluginCtx.InputParameters = new ParameterCollection { ["Target"] = targetRef };
            pluginCtx.PreEntityImages = new EntityImageCollection { [PreImageName] = preImage };
            pluginCtx.PostEntityImages = new EntityImageCollection();
            return pluginCtx;
        }

        protected XrmFakedPluginExecutionContext BuildContactUpdateContext(IXrmBaseContext ctx, Entity target, Entity preImage, int depth = 1)
        {
            var pluginCtx = ctx.GetDefaultPluginContext();
            pluginCtx.MessageName = "Update";
            pluginCtx.Stage = PostOperationStage;
            pluginCtx.Mode = SynchronousMode;
            pluginCtx.Depth = depth;
            pluginCtx.PrimaryEntityName = ContactLogicalName;
            pluginCtx.InputParameters = new ParameterCollection { ["Target"] = target };
            pluginCtx.PreEntityImages = new EntityImageCollection { [PreImageName] = preImage };
            pluginCtx.PostEntityImages = new EntityImageCollection();
            return pluginCtx;
        }
    }
}
