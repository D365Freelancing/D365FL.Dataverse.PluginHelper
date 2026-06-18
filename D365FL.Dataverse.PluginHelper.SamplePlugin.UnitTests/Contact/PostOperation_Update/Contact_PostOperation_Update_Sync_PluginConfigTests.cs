using D365FL.Dataverse.PluginHelper.SamplePlugin.Plugins.Contact;
using D365FL.Dataverse.PluginHelper.SamplePlugin.UnitTests.TestBase;
using FakeXrmEasy.Plugins;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System;

namespace D365FL.Dataverse.PluginHelper.SamplePlugin.UnitTests.Contact.PostOperation_Update
{
    [TestClass]
    public class Contact_PostOperation_Update_Sync_PluginConfigTests : Contact_PostOperation_Update_SyncTestBase
    {
        // ─── Negative Tests ───────────────────────────────────────────────────────

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(InvalidPluginExecutionException),
            ValidationMessages.DefaultErrorMessageForInvalidPluginConfig + ValidationMessages.HasPreImage)]
        public void ValidateConfig_ThrowsInvalidPluginExecutionException_WhenPreImageIsMissing()
        {
            // Arrange — update plugin requires a pre-image
            var contactId = Guid.NewGuid();
            var target = CreateContactWithParent(Guid.NewGuid(), contactId);
            var pluginCtx = _context.GetDefaultPluginContext();
            pluginCtx.MessageName = "Update";
            pluginCtx.Stage = PostOperationStage;
            pluginCtx.Mode = SynchronousMode;
            pluginCtx.Depth = 1;
            pluginCtx.PrimaryEntityName = ContactLogicalName;
            pluginCtx.InputParameters = new ParameterCollection { ["Target"] = target };
            pluginCtx.PreEntityImages = new EntityImageCollection(); // Empty — no preImage

            // Act
            _context.ExecutePluginWithConfigurations<Contact_PostOperation_Update_Sync>(pluginCtx, null, null);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(InvalidPluginExecutionException),
            ValidationMessages.DefaultErrorMessageForInvalidPluginConfig + ValidationMessages.IsPostOperation)]
        public void ValidateConfig_ThrowsInvalidPluginExecutionException_WhenStageIsNotPostOperation()
        {
            // Arrange
            var contactId = Guid.NewGuid();
            var preImage = CreateContactWithParent(Guid.NewGuid(), contactId);
            var target = CreateContactWithParent(Guid.NewGuid(), contactId);
            var pluginCtx = BuildContactUpdateContext(_context, target, preImage);
            pluginCtx.Stage = PreOperationStage; // Wrong stage

            // Act
            _context.ExecutePluginWithConfigurations<Contact_PostOperation_Update_Sync>(pluginCtx, null, null);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(InvalidPluginExecutionException),
            ValidationMessages.DefaultErrorMessageForInvalidPluginConfig + ValidationMessages.IsUpdateMessage)]
        public void ValidateConfig_ThrowsInvalidPluginExecutionException_WhenMessageIsNotUpdate()
        {
            // Arrange
            var contactId = Guid.NewGuid();
            var preImage = CreateContactWithParent(Guid.NewGuid(), contactId);
            var target = CreateContactWithParent(Guid.NewGuid(), contactId);
            var pluginCtx = BuildContactUpdateContext(_context, target, preImage);
            pluginCtx.MessageName = "Create"; // Wrong message

            // Act
            _context.ExecutePluginWithConfigurations<Contact_PostOperation_Update_Sync>(pluginCtx, null, null);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(InvalidPluginExecutionException),
            ValidationMessages.DefaultErrorMessageForInvalidPluginConfig + ValidationMessages.HasTargetEntityLogicalName_contact)]
        public void ValidateConfig_ThrowsInvalidPluginExecutionException_WhenEntityIsNotContact()
        {
            // Arrange
            var contactId = Guid.NewGuid();
            var preImage = CreateContactWithParent(Guid.NewGuid(), contactId);
            var target = CreateAccountEntity(Guid.NewGuid());
            var pluginCtx = BuildContactUpdateContext(_context, target, preImage);

            // Act
            _context.ExecutePluginWithConfigurations<Contact_PostOperation_Update_Sync>(pluginCtx, null, null);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(InvalidPluginExecutionException),
            ValidationMessages.DefaultErrorMessageForInvalidPluginConfig + ValidationMessages.DoesNotExceedMaxDepth_Of_3)]
        public void ValidateConfig_ThrowsInvalidPluginExecutionException_WhenDepthExceedsThree()
        {
            // Arrange
            var contactId = Guid.NewGuid();
            var preImage = CreateContactWithParent(Guid.NewGuid(), contactId);
            var target = CreateContactWithParent(Guid.NewGuid(), contactId);
            var pluginCtx = BuildContactUpdateContext(_context, target, preImage, depth: 4);

            // Act
            _context.ExecutePluginWithConfigurations<Contact_PostOperation_Update_Sync>(pluginCtx, null, null);
        }

        // ─── Boundary Tests ───────────────────────────────────────────────────────

        [TestMethod]
        public void ValidateConfig_Passes_WhenDepthIsExactlyThree()
        {
            // Arrange — depth=3 is the maximum allowed
            var accountId = Guid.NewGuid();
            var contactId = Guid.NewGuid();
            _context.Initialize(new System.Collections.Generic.List<Entity> { CreateAccountEntity(accountId) });

            var preImage = CreateContactWithParent(accountId, contactId);
            var target = new Entity(ContactLogicalName, contactId);
            target["parentcustomerid"] = new EntityReference(AccountLogicalName, accountId);
            var pluginCtx = BuildContactUpdateContext(_context, target, preImage, depth: 3);

            // Act — ValidateConfig passes (depth=3 ≤ max); AreContactCountFieldsDirty returns false
            // (same parent), so no aggregate query is executed and no exception is thrown.
            _context.ExecutePluginWithConfigurations<Contact_PostOperation_Update_Sync>(pluginCtx, null, null);
        }
    }
}
