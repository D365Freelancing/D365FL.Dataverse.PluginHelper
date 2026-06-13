using D365FL.Dataverse.PluginHelper.SamplePlugin.UnitTests.TestBase;
using FakeXrmEasy.Abstractions;
using FakeXrmEasy.Plugins;
using Microsoft.Xrm.Sdk;
using System;

namespace D365FL.Dataverse.PluginHelper.SamplePlugin.UnitTests.Account
{
    public class AccountPluginTestBase : PluginTestBase
    {
        public static class RequiredFieldsMessageCombinations
        {
            public const string MissingTickerSymbolMessage =
            "Cannot save Account — the following required fields are missing or empty: tickersymbol";
            public const string MissingTelephone1Message =
               "Cannot save Account — the following required fields are missing or empty: telephone1";
            public const string MissingTelephone1AndTickerSymbolMessage =
                "Cannot save Account — the following required fields are missing or empty: tickersymbol, telephone1";
        }

        // Boundary value constants
        protected const int TickerSymbolMaxLength = 10;

        protected Entity CreateValidAccountTarget(Guid? id = null)
        {
            var entity = new Entity(AccountLogicalName, id ?? Guid.NewGuid());
            entity["tickersymbol"] = "MSFT";
            entity["telephone1"] = "555-1234";
            return entity;
        }

        protected Entity CreateAccountPreImage(Guid accountId, string tickerSymbol = "MSFT", string telephone1 = "555-1234")
        {
            var entity = new Entity(AccountLogicalName, accountId);
            entity["tickersymbol"] = tickerSymbol;
            entity["telephone1"] = telephone1;
            return entity;
        }

        protected XrmFakedPluginExecutionContext BuildAccountCreateContext(IXrmBaseContext ctx, Entity target, int depth = 1)
        {
            var pluginCtx = ctx.GetDefaultPluginContext();
            pluginCtx.MessageName = "Create";
            pluginCtx.Stage = PreOperationStage;
            pluginCtx.Mode = SynchronousMode;
            pluginCtx.Depth = depth;
            pluginCtx.PrimaryEntityName = AccountLogicalName;
            pluginCtx.InputParameters = new ParameterCollection { ["Target"] = target };
            pluginCtx.PreEntityImages = new EntityImageCollection();
            pluginCtx.PostEntityImages = new EntityImageCollection();
            return pluginCtx;
        }

        protected XrmFakedPluginExecutionContext BuildAccountUpdateContext(IXrmBaseContext ctx, Entity target, Entity preImage, int depth = 1)
        {
            var pluginCtx = ctx.GetDefaultPluginContext();
            pluginCtx.MessageName = "Update";
            pluginCtx.Stage = PreOperationStage;
            pluginCtx.Mode = SynchronousMode;
            pluginCtx.Depth = depth;
            pluginCtx.PrimaryEntityName = AccountLogicalName;
            pluginCtx.InputParameters = new ParameterCollection { ["Target"] = target };
            pluginCtx.PreEntityImages = new EntityImageCollection { [PreImageName] = preImage };
            pluginCtx.PostEntityImages = new EntityImageCollection();
            return pluginCtx;
        }
    }
}
