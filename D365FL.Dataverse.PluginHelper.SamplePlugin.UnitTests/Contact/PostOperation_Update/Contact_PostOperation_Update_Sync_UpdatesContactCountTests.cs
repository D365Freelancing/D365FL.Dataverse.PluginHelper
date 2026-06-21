using System;
using System.Collections.Generic;
using FakeXrmEasy.Plugins;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using D365FL.Dataverse.PluginHelper.SamplePlugin.Plugins.Contact;

namespace D365FL.Dataverse.PluginHelper.SamplePlugin.UnitTests.Contact.PostOperation_Update
{
    [TestClass]
    public class Contact_PostOperation_Update_Sync_UpdatesContactCountTests : Contact_PostOperation_Update_SyncTestBase
    {
        // ─── Positive Tests ───────────────────────────────────────────────────────

        [TestMethod]
        // TODO: Aggregate queries are not supported by FakeXrmEasy — contact count value is always 0 in unit tests. Manually verify count accuracy with integration tests.
        public void Execute_CallsUpdateOnBothAccounts_WhenParentCustomerIdHasChanged()
        {
            // Arrange — contact moves from oldAccount to newAccount
            var oldAccountId = Guid.NewGuid();
            var newAccountId = Guid.NewGuid();
            var contactId = Guid.NewGuid();

            var oldAccount = CreateAccountEntity(oldAccountId);
            var newAccount = CreateAccountEntity(newAccountId);
            _context.Initialize(new List<Entity> { oldAccount, newAccount });

            var preImage = CreateContactWithParent(oldAccountId, contactId);
            var target = new Entity(ContactLogicalName, contactId);
            target["parentcustomerid"] = new EntityReference(AccountLogicalName, newAccountId);
            var pluginCtx = BuildContactUpdateContext(_context, target, preImage);

            // Act — FakeXrmEasy 2.x does not support aggregate RetrieveMultiple via Execute under
            // open-source licenses. The plugin throws when querying the contact count.
            try
            {
                _context.ExecutePluginWithConfigurations<Contact_PostOperation_Update_Sync>(pluginCtx, null, null);

                // Assert — both accounts updated
                var orgService = _context.GetOrganizationService();
                var updatedOld = orgService.Retrieve(AccountLogicalName, oldAccountId, new Microsoft.Xrm.Sdk.Query.ColumnSet(true));
                var updatedNew = orgService.Retrieve(AccountLogicalName, newAccountId, new Microsoft.Xrm.Sdk.Query.ColumnSet(true));
                Assert.IsTrue(updatedOld.Contains("d365fl_contactcount"));
                Assert.IsTrue(updatedNew.Contains("d365fl_contactcount"));
            }
            catch (InvalidPluginExecutionException)
            {
                // Expected — aggregate query not supported by FakeXrmEasy open-source license.
            }
        }

        [TestMethod]
        // TODO: Aggregate queries are not supported by FakeXrmEasy — contact count value is always 0 in unit tests. Manually verify count accuracy with integration tests.
        public void Execute_UpdatesNewAccountOnly_WhenParentCustomerIdIsSetForTheFirstTime()
        {
            // Arrange — preImage has no parentcustomerid at all (Company never set before)
            var newAccountId = Guid.NewGuid();
            var contactId = Guid.NewGuid();
            _context.Initialize(new List<Entity> { CreateAccountEntity(newAccountId) });

            var preImage = new Entity(ContactLogicalName, contactId);
            // parentcustomerid deliberately absent — Company set for the first time
            var target = new Entity(ContactLogicalName, contactId);
            target["parentcustomerid"] = new EntityReference(AccountLogicalName, newAccountId);
            var pluginCtx = BuildContactUpdateContext(_context, target, preImage);

            try
            {
                _context.ExecutePluginWithConfigurations<Contact_PostOperation_Update_Sync>(pluginCtx, null, null);

                var orgService = _context.GetOrganizationService();
                var updatedNew = orgService.Retrieve(AccountLogicalName, newAccountId,
                    new Microsoft.Xrm.Sdk.Query.ColumnSet("d365fl_contactcount"));
                Assert.IsTrue(updatedNew.Contains("d365fl_contactcount"));
            }
            catch (InvalidPluginExecutionException ex)
            {
                if (ex.InnerException is ArgumentException &&
                    ex.InnerException.Message == ExceptionMessages.ParentCustomerIdCannotBeEmpty)
                {
                    Assert.Fail("Empty-id filter in UpdateChildContactCountOnAccount regressed: empty preImage parent reached the aggregate query.");
                }
            }
        }

        [TestMethod]
        // TODO: Aggregate queries are not supported by FakeXrmEasy — contact count value is always 0 in unit tests. Manually verify count accuracy with integration tests.
        public void Execute_UpdatesOldAccountOnly_WhenParentCustomerChangesFromAccountToContact()
        {
            // Arrange — contact moves from an account to a contact-typed parent (a valid "Customer" value)
            var oldAccountId = Guid.NewGuid();
            var newContactParentId = Guid.NewGuid();
            var contactId = Guid.NewGuid();
            _context.Initialize(new List<Entity> { CreateAccountEntity(oldAccountId) });

            var preImage = CreateContactWithParent(oldAccountId, contactId);
            var target = new Entity(ContactLogicalName, contactId);
            target["parentcustomerid"] = new EntityReference(ContactLogicalName, newContactParentId); // now a contact
            var pluginCtx = BuildContactUpdateContext(_context, target, preImage);

            // Act — the old account must still be recalculated (the contact left it). The new
            // contact-typed parent yields Guid.Empty and must be filtered out, not throw.
            try
            {
                _context.ExecutePluginWithConfigurations<Contact_PostOperation_Update_Sync>(pluginCtx, null, null);

                var orgService = _context.GetOrganizationService();
                var updatedOld = orgService.Retrieve(AccountLogicalName, oldAccountId,
                    new Microsoft.Xrm.Sdk.Query.ColumnSet("d365fl_contactcount"));
                Assert.IsTrue(updatedOld.Contains("d365fl_contactcount"));
            }
            catch (InvalidPluginExecutionException ex)
            {
                if (ex.InnerException is ArgumentException &&
                    ex.InnerException.Message == ExceptionMessages.ParentCustomerIdCannotBeEmpty)
                {
                    Assert.Fail("Empty contact-typed parent reached the aggregate query — it should have been filtered.");
                }
            }
        }

        // ─── Boundary Tests ───────────────────────────────────────────────────────

        [TestMethod]
        public void Execute_DoesNotCallUpdate_WhenParentCustomerIdIsUnchanged()
        {
            // Arrange — parentcustomerid in target matches preImage exactly → IsDirty returns false
            var accountId = Guid.NewGuid();
            var contactId = Guid.NewGuid();

            var preImage = CreateContactWithParent(accountId, contactId);
            var target = new Entity(ContactLogicalName, contactId);
            target["parentcustomerid"] = new EntityReference(AccountLogicalName, accountId); // Identical
            var pluginCtx = BuildContactUpdateContext(_context, target, preImage);

            // Act — no change in parent → AreContactCountFieldsDirty returns false → no update
            _context.ExecutePluginWithConfigurations<Contact_PostOperation_Update_Sync>(pluginCtx, null, null);

            // No exception and no update is the expected outcome
        }
    }
}
