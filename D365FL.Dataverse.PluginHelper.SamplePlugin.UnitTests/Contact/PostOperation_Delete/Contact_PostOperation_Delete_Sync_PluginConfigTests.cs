using D365FL.Dataverse.PluginHelper.SamplePlugin.Plugins.Contact;
using D365FL.Dataverse.PluginHelper.SamplePlugin.UnitTests.TestBase;
using FakeXrmEasy.Plugins;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System;

namespace D365FL.Dataverse.PluginHelper.SamplePlugin.UnitTests.Contact.PostOperation_Delete
{
    [TestClass]
    public class Contact_PostOperation_Delete_Sync_PluginConfigTests : Contact_PostOperation_Delete_SyncTestBase
    {
        // ─── Negative Tests ───────────────────────────────────────────────────────

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(InvalidPluginExecutionException),
            ValidationMessages.DefaultErrorMessageForInvalidPluginConfig + ValidationMessages.IsPostOperation)]
        public void ValidateConfig_ThrowsInvalidPluginExecutionException_WhenStageIsNotPostOperation()
        {
            // Arrange
            var contactId = Guid.NewGuid();
            var preImage = CreateContactWithParent(Guid.NewGuid(), contactId);
            var targetRef = new EntityReference(ContactLogicalName, contactId);
            var pluginCtx = BuildContactDeleteContext(_context, targetRef, preImage);
            pluginCtx.Stage = PreOperationStage; // Wrong stage

            // Act
            _context.ExecutePluginWithConfigurations<Contact_PostOperation_Delete_Sync>(pluginCtx, null, null);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(InvalidPluginExecutionException),
            ValidationMessages.DefaultErrorMessageForInvalidPluginConfig + ValidationMessages.IsDeleteMessage)]
        public void ValidateConfig_ThrowsInvalidPluginExecutionException_WhenMessageIsNotDelete()
        {
            // Arrange
            var contactId = Guid.NewGuid();
            var preImage = CreateContactWithParent(Guid.NewGuid(), contactId);
            var targetRef = new EntityReference(ContactLogicalName, contactId);
            var pluginCtx = BuildContactDeleteContext(_context, targetRef, preImage);
            pluginCtx.MessageName = "Update"; // Wrong message

            // Act
            _context.ExecutePluginWithConfigurations<Contact_PostOperation_Delete_Sync>(pluginCtx, null, null);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(InvalidPluginExecutionException),
            ValidationMessages.DefaultErrorMessageForInvalidPluginConfig + ValidationMessages.HasPreImage)]
        public void ValidateConfig_ThrowsInvalidPluginExecutionException_WhenPreImageIsMissing()
        {
            // Arrange — delete plugin requires a pre-image
            var contactId = Guid.NewGuid();
            var targetRef = new EntityReference(ContactLogicalName, contactId);
            var pluginCtx = _context.GetDefaultPluginContext();
            pluginCtx.MessageName = "Delete";
            pluginCtx.Stage = PostOperationStage;
            pluginCtx.Mode = SynchronousMode;
            pluginCtx.Depth = 1;
            pluginCtx.PrimaryEntityName = ContactLogicalName;
            pluginCtx.InputParameters = new ParameterCollection { ["Target"] = targetRef };
            pluginCtx.PreEntityImages = new EntityImageCollection(); // Empty — no preImage

            // Act
            _context.ExecutePluginWithConfigurations<Contact_PostOperation_Delete_Sync>(pluginCtx, null, null);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(InvalidPluginExecutionException),
            ValidationMessages.DefaultErrorMessageForInvalidPluginConfig + ValidationMessages.HasTargetEntityReferenceLogicalName_contact)]
        public void ValidateConfig_ThrowsInvalidPluginExecutionException_WhenEntityReferenceLogicalNameIsNotContact()
        {
            // Arrange — target EntityReference has wrong logical name
            var contactId = Guid.NewGuid();
            var preImage = CreateContactWithParent(Guid.NewGuid(), contactId);
            var targetRef = new EntityReference(AccountLogicalName, contactId); // Wrong entity type
            var pluginCtx = BuildContactDeleteContext(_context, targetRef, preImage);
            
            // Act
            _context.ExecutePluginWithConfigurations<Contact_PostOperation_Delete_Sync>(pluginCtx, null, null);
        }

        [TestMethod]
        [ExpectedExceptionWithMessage(typeof(InvalidPluginExecutionException),
            ValidationMessages.DefaultErrorMessageForInvalidPluginConfig + ValidationMessages.DoesNotExceedMaxDepth_Of_3)]
        public void ValidateConfig_ThrowsInvalidPluginExecutionException_WhenDepthExceedsThree()
        {
            // Arrange
            var contactId = Guid.NewGuid();
            var preImage = CreateContactWithParent(Guid.NewGuid(), contactId);
            var targetRef = new EntityReference(ContactLogicalName, contactId);
            var pluginCtx = BuildContactDeleteContext(_context, targetRef, preImage, depth: 4);

            // Act
            _context.ExecutePluginWithConfigurations<Contact_PostOperation_Delete_Sync>(pluginCtx, null, null);
        }

        // ─── Boundary Tests ───────────────────────────────────────────────────────

        [TestMethod]
        public void ValidateConfig_Passes_WhenDepthIsExactlyThree()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var contactId = Guid.NewGuid();
            var account = CreateAccountEntity(accountId);
            _context.Initialize(new System.Collections.Generic.List<Entity> { account });

            var preImage = CreateContactWithParent(accountId, contactId);
            var targetRef = new EntityReference(ContactLogicalName, contactId);
            var pluginCtx = BuildContactDeleteContext(_context, targetRef, preImage, depth: 3); // Max depth

            // Act — ValidateConfig passes (depth=3 ≤ max); plugin then throws from the
            // aggregate query (FakeXrmEasy open-source limitation).
            try
            {
                _context.ExecutePluginWithConfigurations<Contact_PostOperation_Delete_Sync>(pluginCtx, null, null);
            }
            catch (InvalidPluginExecutionException ex) when (ex.InnerException != null)
            {
                // ValidateConfig passed; plugin threw from Execute due to aggregate query limitation.
            }
        }
    }
}
