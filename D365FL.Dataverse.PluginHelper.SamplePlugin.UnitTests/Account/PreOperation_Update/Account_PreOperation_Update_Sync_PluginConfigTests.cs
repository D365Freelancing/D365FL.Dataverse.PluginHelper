using D365FL.Dataverse.PluginHelper.SamplePlugin.Plugins.Account;
using D365FL.Dataverse.PluginHelper.SamplePlugin.UnitTests.TestBase;
using FakeXrmEasy.Plugins;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System;

namespace D365FL.Dataverse.PluginHelper.SamplePlugin.UnitTests.Account.PreOperation_Update
{
    [TestClass]
    public class Account_PreOperation_Update_Sync_PluginConfigTests : Account_PreOperation_Update_SyncTestBase
    {
        // ─── Positive Tests ───────────────────────────────────────────────────────

        [TestMethod]
        public void Execute_WithDefaultSecureConfig_AllowsDepthOne()
        {
            // Arrange — null secureConfig means MaxRetries=1; depth 1 is within limit
            var accountId = Guid.NewGuid();
            var preImage = CreateAccountPreImage(accountId);
            var target = new Entity(AccountLogicalName, accountId);
            target["tickersymbol"] = "TSLA";
            var pluginCtx = BuildAccountUpdateContext(_context, target, preImage, depth: 1);

            // Act — should not throw
            _context.ExecutePluginWithConfigurations<Account_PreOperation_Update_Sync>(pluginCtx, null, null);

            // Assert
            var result = (Entity)pluginCtx.InputParameters["Target"];
            Assert.AreEqual("TSLA - 555-1234", result["name"]);
        }

        // ─── Negative Tests ───────────────────────────────────────────────────────

        [TestMethod]
        [ExpectedExceptionWithMessageAttribute(typeof(InvalidPluginExecutionException), ValidationMessages.DefaultErrorMessageForInvalidPluginConfig + ValidationMessages.HasPreImage)]
        public void ValidateConfig_ThrowsInvalidPluginExecutionException_WhenPreImageIsMissing()
        {
            // Arrange — no pre-image registered
            var target = CreateValidAccountTarget();
            var pluginCtx = _context.GetDefaultPluginContext();
            pluginCtx.MessageName = "Update";
            pluginCtx.Stage = PreOperationStage;
            pluginCtx.Mode = SynchronousMode;
            pluginCtx.Depth = 1;
            pluginCtx.PrimaryEntityName = AccountLogicalName;
            pluginCtx.InputParameters = new ParameterCollection { ["Target"] = target };
            pluginCtx.PreEntityImages = new EntityImageCollection(); // Empty — no preImage

            // Act
            _context.ExecutePluginWithConfigurations<Account_PreOperation_Update_Sync>(pluginCtx, null, null);
        }

        [TestMethod]
        [ExpectedExceptionWithMessageAttribute(typeof(InvalidPluginExecutionException), ValidationMessages.DefaultErrorMessageForInvalidPluginConfig + ValidationMessages.IsPreOperation)]
        public void ValidateConfig_ThrowsInvalidPluginExecutionException_WhenStageIsNotPreOperation()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var preImage = CreateAccountPreImage(accountId);
            var target = CreateValidAccountTarget(accountId);
            var pluginCtx = BuildAccountUpdateContext(_context, target, preImage);
            pluginCtx.Stage = PostOperationStage; // Wrong stage

            // Act
            _context.ExecutePluginWithConfigurations<Account_PreOperation_Update_Sync>(pluginCtx, null, null);
        }

        [TestMethod]
        [ExpectedExceptionWithMessageAttribute(typeof(InvalidPluginExecutionException), ValidationMessages.DefaultErrorMessageForInvalidPluginConfig + ValidationMessages.IsUpdateMessage)]
        public void ValidateConfig_ThrowsInvalidPluginExecutionException_WhenMessageIsNotUpdate()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var preImage = CreateAccountPreImage(accountId);
            var target = CreateValidAccountTarget(accountId);
            var pluginCtx = BuildAccountUpdateContext(_context, target, preImage);
            pluginCtx.MessageName = "Create"; // Wrong message

            // Act
            _context.ExecutePluginWithConfigurations<Account_PreOperation_Update_Sync>(pluginCtx, null, null);
        }

        [TestMethod]
        [ExpectedExceptionWithMessageAttribute(typeof(InvalidPluginExecutionException), ValidationMessages.DefaultErrorMessageForInvalidPluginConfig + ValidationMessages.HasTargetEntityLogicalName_account)]
        public void ValidateConfig_ThrowsInvalidPluginExecutionException_WhenEntityIsNotAccount()
        {
            // Arrange — AddTargetEntityLogicalNameRule checks target.LogicalName, NOT PrimaryEntityName,
            // so the target entity itself must have the wrong logical name.
            var accountId = Guid.NewGuid();
            var preImage = CreateAccountPreImage(accountId);
            var target = new Entity(ContactLogicalName, accountId); // Wrong entity type
            target["tickersymbol"] = "MSFT";
            target["telephone1"] = "555-1234";
            var pluginCtx = BuildAccountUpdateContext(_context, target, preImage);
            pluginCtx.PrimaryEntityName = ContactLogicalName;

            // Act
            _context.ExecutePluginWithConfigurations<Account_PreOperation_Update_Sync>(pluginCtx, null, null);
        }

        [TestMethod]
        [ExpectedExceptionWithMessageAttribute(typeof(InvalidPluginExecutionException), ValidationMessages.DefaultErrorMessageForInvalidPluginConfig + ValidationMessages.DoesNotExceedMaxDepth_Of_1)]
        public void ValidateConfig_ThrowsInvalidPluginExecutionException_WhenDepthExceedsMaxRetries()
        {
            // Arrange — default MaxRetries=1, so depth=2 should fail
            var accountId = Guid.NewGuid();
            var preImage = CreateAccountPreImage(accountId);
            var target = CreateValidAccountTarget(accountId);
            var pluginCtx = BuildAccountUpdateContext(_context, target, preImage, depth: 2);

            // Act
            _context.ExecutePluginWithConfigurations<Account_PreOperation_Update_Sync>(pluginCtx, null, null);
        }

        // ─── Boundary Tests ───────────────────────────────────────────────────────

        [TestMethod]
        public void Execute_WithSecureConfigMaxRetriesTwo_AllowsDepthTwo()
        {
            // Arrange — secureConfig sets MaxRetries=2, so depth=2 is valid
            var accountId = Guid.NewGuid();
            var preImage = CreateAccountPreImage(accountId, tickerSymbol: "OLD", telephone1: "555-0000");
            var target = new Entity(AccountLogicalName, accountId);
            target["tickersymbol"] = "NEW";
            var pluginCtx = BuildAccountUpdateContext(_context, target, preImage, depth: 2);

            // Act — should not throw
            _context.ExecutePluginWithConfigurations<Account_PreOperation_Update_Sync>(
                pluginCtx,
                unsecureConfiguration: null,
                secureConfiguration: "{\"MaxRetries\": 2}");

            // Assert
            var result = (Entity)pluginCtx.InputParameters["Target"];
            Assert.AreEqual("NEW - 555-0000", result["name"]);
        }

        [TestMethod]
        [ExpectedExceptionWithMessageAttribute(typeof(InvalidPluginExecutionException), ValidationMessages.DefaultErrorMessageForInvalidPluginConfig + ValidationMessages.DoesNotExceedMaxDepth_Of_2)]
        public void ValidateConfig_ThrowsInvalidPluginExecutionException_WhenDepthIsOneMoreThanMaxRetries()
        {
            // Arrange — secureConfig MaxRetries=2, so depth=3 exceeds limit
            var accountId = Guid.NewGuid();
            var preImage = CreateAccountPreImage(accountId);
            var target = CreateValidAccountTarget(accountId);
            var pluginCtx = BuildAccountUpdateContext(_context, target, preImage, depth: 3);

            // Act
            _context.ExecutePluginWithConfigurations<Account_PreOperation_Update_Sync>(
                pluginCtx,
                unsecureConfiguration: null,
                secureConfiguration: "{\"MaxRetries\": 2}");
        }

        [TestMethod]
        public void Execute_WithEmptyStringSecureConfig_DefaultsMaxRetriesToOne()
        {
            // Arrange — empty secureConfig string → MaxRetries defaults to 1; depth=1 is valid
            var accountId = Guid.NewGuid();
            var preImage = CreateAccountPreImage(accountId);
            var target = new Entity(AccountLogicalName, accountId);
            target["tickersymbol"] = "DFLT"; // Triggers name recalc
            var pluginCtx = BuildAccountUpdateContext(_context, target, preImage, depth: 1);

            // Act — should not throw
            _context.ExecutePluginWithConfigurations<Account_PreOperation_Update_Sync>(
                pluginCtx,
                unsecureConfiguration: null,
                secureConfiguration: ""); // Empty string → default config

            // Assert
            var result = (Entity)pluginCtx.InputParameters["Target"];
            Assert.AreEqual("DFLT - 555-1234", result["name"]);
        }
    }
}
