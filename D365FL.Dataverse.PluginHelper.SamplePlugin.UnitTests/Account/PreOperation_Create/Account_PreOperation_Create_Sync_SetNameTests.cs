using D365FL.Dataverse.PluginHelper.SamplePlugin.Plugins.Account;
using D365FL.Dataverse.PluginHelper.SamplePlugin.UnitTests.TestBase;
using FakeXrmEasy.Plugins;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System;

namespace D365FL.Dataverse.PluginHelper.SamplePlugin.UnitTests.Account.PreOperation_Create
{
    [TestClass]
    public class Account_PreOperation_Create_Sync_SetNameTests : Account_PreOperation_Create_SyncTestBase
    {
        // ─── Positive Tests ───────────────────────────────────────────────────────

        [TestMethod]
        public void Execute_SetsAccountName_WhenTickerSymbolAndTelephone1ArePresent()
        {
            // Arrange
            var target = CreateValidAccountTarget();
            var pluginCtx = BuildAccountCreateContext(_context, target);

            // Act
            _context.ExecutePluginWithConfigurations<Account_PreOperation_Create_Sync>(pluginCtx, null, null);

            // Assert
            var result = (Entity)pluginCtx.InputParameters["Target"];
            Assert.AreEqual("MSFT - 555-1234", result["name"]);
        }

        // ─── Negative Tests ───────────────────────────────────────────────────────

        [TestMethod]
        [ExpectedExceptionWithMessageAttribute(typeof(InvalidPluginExecutionException), 
            RequiredFieldsMessageCombinations.MissingTickerSymbolMessage)]
        public void Execute_ThrowsInvalidPluginExecutionException_WhenTickerSymbolIsMissing()
        {
            // Arrange
            var target = new Entity(AccountLogicalName, Guid.NewGuid());
            target["telephone1"] = "555-1234";
            // tickersymbol deliberately omitted
            var pluginCtx = BuildAccountCreateContext(_context, target);

            // Act
            _context.ExecutePluginWithConfigurations<Account_PreOperation_Create_Sync>(pluginCtx, null, null);
        }

        
        [TestMethod]
        [ExpectedExceptionWithMessageAttribute(typeof(InvalidPluginExecutionException),
            RequiredFieldsMessageCombinations.MissingTelephone1Message)]
        public void Execute_ThrowsInvalidPluginExecutionException_WhenTelephone1IsMissing()
        {
            // Arrange
            var target = new Entity(AccountLogicalName, Guid.NewGuid());
            target["tickersymbol"] = "MSFT";
            // telephone1 deliberately omitted
            var pluginCtx = BuildAccountCreateContext(_context, target);

            // Act
            _context.ExecutePluginWithConfigurations<Account_PreOperation_Create_Sync>(pluginCtx, null, null);
        }

        
        [TestMethod]
        [ExpectedExceptionWithMessageAttribute(typeof(InvalidPluginExecutionException),
            RequiredFieldsMessageCombinations.MissingTelephone1AndTickerSymbolMessage)]
        public void Execute_ThrowsInvalidPluginExecutionException_WhenBothRequiredFieldsMissing()
        {
            // Arrange
            var target = new Entity(AccountLogicalName, Guid.NewGuid());
            // Both tickersymbol and telephone1 omitted
            var pluginCtx = BuildAccountCreateContext(_context, target);

            // Act
            _context.ExecutePluginWithConfigurations<Account_PreOperation_Create_Sync>(pluginCtx, null, null);
        }

        // ─── Boundary Tests ───────────────────────────────────────────────────────

        [TestMethod]
        public void Execute_SetsNameCorrectly_WhenTickerSymbolIsAtMaxLength()
        {
            // Arrange
            var longTicker = new string('A', TickerSymbolMaxLength);
            var target = CreateValidAccountTarget();
            target["tickersymbol"] = longTicker;
            target["telephone1"] = "555-0001";
            var pluginCtx = BuildAccountCreateContext(_context, target);

            // Act
            _context.ExecutePluginWithConfigurations<Account_PreOperation_Create_Sync>(pluginCtx, null, null);

            // Assert
            var result = (Entity)pluginCtx.InputParameters["Target"];
            Assert.AreEqual($"{longTicker} - 555-0001", result["name"]);
        }

        [TestMethod]
        public void Execute_SetsNameCorrectly_WhenTelephone1ContainsSpecialCharacters()
        {
            // Arrange
            var target = CreateValidAccountTarget();
            target["telephone1"] = SpecialCharacters;
            var pluginCtx = BuildAccountCreateContext(_context, target);

            // Act
            _context.ExecutePluginWithConfigurations<Account_PreOperation_Create_Sync>(pluginCtx, null, null);

            // Assert
            var result = (Entity)pluginCtx.InputParameters["Target"];
            Assert.AreEqual($"MSFT - {SpecialCharacters}", result["name"]);
        }

        [TestMethod]
        public void Execute_SetsNameCorrectly_WhenTickerSymbolHasSingleCharacter()
        {
            // Arrange
            var target = CreateValidAccountTarget();
            target["tickersymbol"] = "X";
            var pluginCtx = BuildAccountCreateContext(_context, target);

            // Act
            _context.ExecutePluginWithConfigurations<Account_PreOperation_Create_Sync>(pluginCtx, null, null);

            // Assert
            var result = (Entity)pluginCtx.InputParameters["Target"];
            Assert.AreEqual("X - 555-1234", result["name"]);
        }
    }
}
