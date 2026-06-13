using System;
using FakeXrmEasy.Plugins;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using D365FL.Dataverse.PluginHelper.SamplePlugin.Plugins.Account;

namespace D365FL.Dataverse.PluginHelper.SamplePlugin.UnitTests.Account.PreOperation_Update
{
    [TestClass]
    public class Account_PreOperation_Update_Sync_SetNameTests : Account_PreOperation_Update_SyncTestBase
    {
        // ─── Positive Tests ───────────────────────────────────────────────────────

        [TestMethod]
        public void Execute_RecalculatesName_WhenTickerSymbolIsDirty()
        {
            // Arrange — preImage has old ticker, target has new ticker
            var accountId = Guid.NewGuid();
            var preImage = CreateAccountPreImage(accountId, tickerSymbol: "OLD", telephone1: "555-0000");
            var target = new Entity(AccountLogicalName, accountId);
            target["tickersymbol"] = "NEW"; // Changed
            var pluginCtx = BuildAccountUpdateContext(_context, target, preImage);

            // Act
            _context.ExecutePluginWithConfigurations<Account_PreOperation_Update_Sync>(pluginCtx, null, null);

            // Assert — merged entity: tickersymbol=NEW, telephone1=555-0000 (from preImage)
            var result = (Entity)pluginCtx.InputParameters["Target"];
            Assert.AreEqual("NEW - 555-0000", result["name"]);
        }

        [TestMethod]
        public void Execute_RecalculatesName_WhenTelephone1IsDirty()
        {
            // Arrange — preImage has old phone, target has new phone
            var accountId = Guid.NewGuid();
            var preImage = CreateAccountPreImage(accountId, tickerSymbol: "MSFT", telephone1: "000-0000");
            var target = new Entity(AccountLogicalName, accountId);
            target["telephone1"] = "999-9999"; // Changed
            var pluginCtx = BuildAccountUpdateContext(_context, target, preImage);

            // Act
            _context.ExecutePluginWithConfigurations<Account_PreOperation_Update_Sync>(pluginCtx, null, null);

            // Assert
            var result = (Entity)pluginCtx.InputParameters["Target"];
            Assert.AreEqual("MSFT - 999-9999", result["name"]);
        }

        [TestMethod]
        public void Execute_DoesNotRecalculateName_WhenNeitherNameFieldIsDirty()
        {
            // Arrange — target explicitly carries the same tickersymbol and telephone1 as preImage,
            // plus an unrelated field. IsDirty compares preImage vs target attribute-by-attribute;
            // a field absent from target is treated as null (changed), so same values must be
            // present in the target to register as "no change".
            var accountId = Guid.NewGuid();
            var preImage = CreateAccountPreImage(accountId, tickerSymbol: "MSFT", telephone1: "555-1234");
            var target = new Entity(AccountLogicalName, accountId);
            target["tickersymbol"] = "MSFT"; // Same as preImage — IsDirty returns false
            target["telephone1"] = "555-1234"; // Same as preImage — IsDirty returns false
            target["description"] = "Some description change"; // Unrelated field
            var pluginCtx = BuildAccountUpdateContext(_context, target, preImage);

            // Act
            _context.ExecutePluginWithConfigurations<Account_PreOperation_Update_Sync>(pluginCtx, null, null);

            // Assert — AreNameFieldsDirty returns false → SetName not called → "name" absent from target
            var result = (Entity)pluginCtx.InputParameters["Target"];
            Assert.IsFalse(result.Contains("name"));
        }

        [TestMethod]
        public void Execute_UsesPreImageValues_WhenTargetIsMissingField()
        {
            // Arrange — target only provides telephone1; tickersymbol comes from preImage via merge
            var accountId = Guid.NewGuid();
            var preImage = CreateAccountPreImage(accountId, tickerSymbol: "AAPL", telephone1: "111-0000");
            var target = new Entity(AccountLogicalName, accountId);
            target["telephone1"] = "222-9999"; // Only this changes
            var pluginCtx = BuildAccountUpdateContext(_context, target, preImage);

            // Act
            _context.ExecutePluginWithConfigurations<Account_PreOperation_Update_Sync>(pluginCtx, null, null);

            // Assert — tickersymbol from preImage, telephone1 from target
            var result = (Entity)pluginCtx.InputParameters["Target"];
            Assert.AreEqual("AAPL - 222-9999", result["name"]);
        }

        // ─── Negative Tests ───────────────────────────────────────────────────────

        [TestMethod]
        [ExpectedException(typeof(InvalidPluginExecutionException))]
        public void Execute_ThrowsInvalidPluginExecutionException_WhenMergedEntityMissingTickerSymbol()
        {
            // Arrange — neither preImage nor target has tickersymbol; telephone1 triggers name recalc
            var accountId = Guid.NewGuid();
            var preImage = new Entity(AccountLogicalName, accountId);
            preImage["telephone1"] = "555-0000"; // No tickersymbol
            var target = new Entity(AccountLogicalName, accountId);
            target["telephone1"] = "555-9999"; // Changed → triggers name recalc
            var pluginCtx = BuildAccountUpdateContext(_context, target, preImage);

            // Act — merge has no tickersymbol → ArgumentException → InvalidPluginExecutionException
            _context.ExecutePluginWithConfigurations<Account_PreOperation_Update_Sync>(pluginCtx, null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidPluginExecutionException))]
        public void Execute_ThrowsInvalidPluginExecutionException_WhenMergedEntityMissingTelephone1()
        {
            // Arrange — neither preImage nor target has telephone1; tickersymbol triggers name recalc
            var accountId = Guid.NewGuid();
            var preImage = new Entity(AccountLogicalName, accountId);
            preImage["tickersymbol"] = "MSFT"; // No telephone1
            var target = new Entity(AccountLogicalName, accountId);
            target["tickersymbol"] = "GOOG"; // Changed → triggers name recalc
            var pluginCtx = BuildAccountUpdateContext(_context, target, preImage);

            // Act
            _context.ExecutePluginWithConfigurations<Account_PreOperation_Update_Sync>(pluginCtx, null, null);
        }
    }
}
