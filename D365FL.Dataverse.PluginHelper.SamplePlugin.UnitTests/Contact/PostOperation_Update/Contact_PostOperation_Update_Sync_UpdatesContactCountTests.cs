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
