using D365FL.Dataverse.PluginHelper.SamplePlugin.Plugins.Contact;
using D365FL.Dataverse.PluginHelper.SamplePlugin.UnitTests.TestBase;
using FakeXrmEasy.Plugins;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System;

namespace D365FL.Dataverse.PluginHelper.SamplePlugin.UnitTests.Contact.PostOperation_Create
{
    [TestClass]
    public class Contact_PostOperation_Create_Sync_PluginConfigTests : Contact_PostOperation_Create_SyncTestBase
    {
        // ─── Negative Tests ───────────────────────────────────────────────────────

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(InvalidPluginExecutionException),
            ValidationMessages.DefaultErrorMessageForInvalidPluginConfig + ValidationMessages.IsPostOperation)]
        public void ValidateConfig_ThrowsInvalidPluginExecutionException_WhenStageIsNotPostOperation()
        {
            // Arrange
            var contact = CreateContactWithParent(Guid.NewGuid());
            var pluginCtx = BuildContactCreateContext(_context, contact);
            pluginCtx.Stage = PreOperationStage; // Wrong stage

            // Act
            _context.ExecutePluginWithConfigurations<Contact_PostOperation_Create_Sync>(pluginCtx, null, null);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(InvalidPluginExecutionException),
            ValidationMessages.DefaultErrorMessageForInvalidPluginConfig + ValidationMessages.IsCreateMessage)]
        public void ValidateConfig_ThrowsInvalidPluginExecutionException_WhenMessageIsNotCreate()
        {
            // Arrange
            var contact = CreateContactWithParent(Guid.NewGuid());
            var pluginCtx = BuildContactCreateContext(_context, contact);
            pluginCtx.MessageName = "Update"; // Wrong message

            // Act
            _context.ExecutePluginWithConfigurations<Contact_PostOperation_Create_Sync>(pluginCtx, null, null);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(InvalidPluginExecutionException),
            ValidationMessages.DefaultErrorMessageForInvalidPluginConfig + ValidationMessages.HasTargetEntityLogicalName_contact)]
        public void ValidateConfig_ThrowsInvalidPluginExecutionException_WhenEntityIsNotContact()
        {
            // Arrange
            var account = CreateAccountEntity(Guid.NewGuid());
            var pluginCtx = BuildContactCreateContext(_context, account);

            // Act
            _context.ExecutePluginWithConfigurations<Contact_PostOperation_Create_Sync>(pluginCtx, null, null);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(InvalidPluginExecutionException),
            ValidationMessages.DefaultErrorMessageForInvalidPluginConfig + ValidationMessages.IsSynchronous)]
        public void ValidateConfig_ThrowsInvalidPluginExecutionException_WhenModeIsNotSynchronous()
        {
            // Arrange
            var contact = CreateContactWithParent(Guid.NewGuid());
            var pluginCtx = BuildContactCreateContext(_context, contact);
            pluginCtx.Mode = 1; // Asynchronous

            // Act
            _context.ExecutePluginWithConfigurations<Contact_PostOperation_Create_Sync>(pluginCtx, null, null);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(InvalidPluginExecutionException),
            ValidationMessages.DefaultErrorMessageForInvalidPluginConfig + ValidationMessages.DoesNotExceedMaxDepth_Of_3)]
        public void ValidateConfig_ThrowsInvalidPluginExecutionException_WhenDepthExceedsThree()
        {
            // Arrange
            var contact = CreateContactWithParent(Guid.NewGuid());
            var pluginCtx = BuildContactCreateContext(_context, contact, depth: 4);

            // Act
            _context.ExecutePluginWithConfigurations<Contact_PostOperation_Create_Sync>(pluginCtx, null, null);
        }

        // ─── Boundary Tests ───────────────────────────────────────────────────────

        [TestMethod]
        public void ValidateConfig_Passes_WhenDepthIsExactlyThree()
        {
            // Arrange — depth=3 is the maximum allowed
            var accountId = Guid.NewGuid();
            var account = CreateAccountEntity(accountId);
            _context.Initialize(new System.Collections.Generic.List<Entity> { account });

            var contact = CreateContactWithParent(accountId);
            var pluginCtx = BuildContactCreateContext(_context, contact, depth: 3);

            // Act — ValidateConfig passes (depth=3 ≤ max); plugin then throws from the
            // aggregate query (FakeXrmEasy open-source limitation). Distinguish that from
            // a ValidateConfig failure by checking the inner exception.
            try
            {
                _context.ExecutePluginWithConfigurations<Contact_PostOperation_Create_Sync>(pluginCtx, null, null);
            }
            catch (InvalidPluginExecutionException ex) when (ex.InnerException != null)
            {
                // ValidateConfig passed; plugin threw from Execute due to aggregate query limitation.
            }
        }
    }
}
