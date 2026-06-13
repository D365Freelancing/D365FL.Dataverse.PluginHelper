using D365FL.Dataverse.PluginHelper.SamplePlugin.Plugins.Account;
using D365FL.Dataverse.PluginHelper.SamplePlugin.UnitTests.TestBase;
using FakeXrmEasy.Plugins;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;

namespace D365FL.Dataverse.PluginHelper.SamplePlugin.UnitTests.Account.PreOperation_Create
{
    [TestClass]
    public class Account_PreOperation_Create_Sync_PluginConfigTests : Account_PreOperation_Create_SyncTestBase
    {
        // ─── Negative Tests ───────────────────────────────────────────────────────

        [TestMethod]
        [ExpectedExceptionWithMessageAttribute(typeof(InvalidPluginExecutionException), 
            ValidationMessages.DefaultErrorMessageForInvalidPluginConfig + ValidationMessages.IsPreOperation)]
        public void ValidateConfig_ThrowsInvalidPluginExecutionException_WhenStageIsNotPreOperation()
        {
            // Arrange
            var target = CreateValidAccountTarget();
            var pluginCtx = BuildAccountCreateContext(_context, target);
            pluginCtx.Stage = PostOperationStage; // Wrong stage

            // Act
            _context.ExecutePluginWithConfigurations<Account_PreOperation_Create_Sync>(pluginCtx, null, null);
        }

        [TestMethod]
        [ExpectedExceptionWithMessageAttribute(typeof(InvalidPluginExecutionException), 
            ValidationMessages.DefaultErrorMessageForInvalidPluginConfig + ValidationMessages.IsCreateMessage)]
        public void ValidateConfig_ThrowsInvalidPluginExecutionException_WhenMessageIsNotCreate()
        {
            // Arrange
            var target = CreateValidAccountTarget();
            var pluginCtx = BuildAccountCreateContext(_context, target);
            pluginCtx.MessageName = "Update"; // Wrong message

            // Act
            _context.ExecutePluginWithConfigurations<Account_PreOperation_Create_Sync>(pluginCtx, null, null);
        }

        [TestMethod]
        [ExpectedExceptionWithMessageAttribute(typeof(InvalidPluginExecutionException), 
            ValidationMessages.DefaultErrorMessageForInvalidPluginConfig + ValidationMessages.HasTargetEntityLogicalName_account)]
        public void ValidateConfig_ThrowsInvalidPluginExecutionException_WhenEntityIsNotAccount()
        {
            // Arrange
            var target = new Entity(ContactLogicalName, System.Guid.NewGuid());
            target["tickersymbol"] = "MSFT";
            target["telephone1"] = "555-1234";
            var pluginCtx = BuildAccountCreateContext(_context, target);
            pluginCtx.PrimaryEntityName = ContactLogicalName; // Wrong entity

            // Act
            _context.ExecutePluginWithConfigurations<Account_PreOperation_Create_Sync>(pluginCtx, null, null);
        }

        [TestMethod]
        [ExpectedExceptionWithMessageAttribute(typeof(InvalidPluginExecutionException), 
            ValidationMessages.DefaultErrorMessageForInvalidPluginConfig + ValidationMessages.IsSynchronous)]
        public void ValidateConfig_ThrowsInvalidPluginExecutionException_WhenModeIsNotSynchronous()
        {
            // Arrange
            var target = CreateValidAccountTarget();
            var pluginCtx = BuildAccountCreateContext(_context, target);
            pluginCtx.Mode = 1; // Asynchronous

            // Act
            _context.ExecutePluginWithConfigurations<Account_PreOperation_Create_Sync>(pluginCtx, null, null);
        }

        [TestMethod]
        [ExpectedExceptionWithMessageAttribute(typeof(InvalidPluginExecutionException), 
            ValidationMessages.DefaultErrorMessageForInvalidPluginConfig + ValidationMessages.DoesNotExceedMaxDepth_Of_3)]
        public void ValidateConfig_ThrowsInvalidPluginExecutionException_WhenDepthExceedsThree()
        {
            // Arrange
            var target = CreateValidAccountTarget();
            var pluginCtx = BuildAccountCreateContext(_context, target, depth: 4); // Depth > 3

            // Act
            _context.ExecutePluginWithConfigurations<Account_PreOperation_Create_Sync>(pluginCtx, null, null);
        }

        // ─── Boundary Tests ───────────────────────────────────────────────────────

        [TestMethod]
        public void ValidateConfig_Passes_WhenDepthIsExactlyThree()
        {
            // Arrange
            var target = CreateValidAccountTarget();
            var pluginCtx = BuildAccountCreateContext(_context, target, depth: 3); // Max allowed depth

            // Act — should not throw
            _context.ExecutePluginWithConfigurations<Account_PreOperation_Create_Sync>(pluginCtx, null, null);

            // Assert
            var result = (Entity)pluginCtx.InputParameters["Target"];
            Assert.IsNotNull(result["name"]);
        }
    }
}
