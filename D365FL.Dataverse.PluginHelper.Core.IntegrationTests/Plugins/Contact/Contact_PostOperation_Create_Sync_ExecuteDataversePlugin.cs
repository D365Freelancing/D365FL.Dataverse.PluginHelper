using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace D365FL.Dataverse.PluginHelper.Core.IntegrationTests.Plugins.Contact
{
    [TestClass]
    public class Contact_PostOperation_Create_Sync_ExecuteDataversePlugin
    {
        
        [TestMethod]
        public void Contact_PostOperation_Create_Sync_GetsCorrectContactCount_AfterCreatingMultipleContacts()
        {
            // ARRANGE
            var account = ContactTestHelpers.CreateAccount();
            var accountId = AssemblyLifecycle.CreateAndTrackEntity(account);

            var contact1 = ContactTestHelpers.CreateContact("John", "Smith", accountId);
            var contact2 = ContactTestHelpers.CreateContact("Jane", "Smith", accountId);

            // ACT
            AssemblyLifecycle.CreateAndTrackEntity(contact1);
            AssemblyLifecycle.CreateAndTrackEntity(contact2);

            // ASSERT
            var accountEntityLogicalName = ContactTestHelpers.accountEntityLogicalName;
            var savedAccount = AssemblyLifecycle.OrgService.Retrieve(accountEntityLogicalName, accountId, new ColumnSet("d365fl_contactcount"));
            var contactCount = savedAccount.GetAttributeValue<int>("d365fl_contactcount");
            Assert.AreEqual(2, contactCount);
        }

        [TestMethod]
        public void OnAccountCreate_HasZeroContactCount_ByDefault()
        {
            // ARRANGE
            var account = ContactTestHelpers.CreateAccount();

            // ACT
            var accountId = AssemblyLifecycle.CreateAndTrackEntity(account);

            // ASSERT
            var accountEntityLogicalName = ContactTestHelpers.accountEntityLogicalName;
            var savedAccount = AssemblyLifecycle.OrgService.Retrieve(accountEntityLogicalName, accountId, new ColumnSet("d365fl_contactcount"));
            var contactCount = savedAccount.GetAttributeValue<int>("d365fl_contactcount");
            Assert.AreEqual(0, contactCount);
        }

        [TestMethod]
        public void Contact_PostOperation_Create_Sync_GetsCorrectContactCount_AfterCreatingOver5000Contacts()
        {
            if(AssemblyLifecycle.SkipPerformanceTests)
            {
                Assert.Inconclusive("Skipped performance tests");
            }
            // this is a performance test
            // and tests the max 5000 record limit in power platform
            // ARRANGE
            var account = ContactTestHelpers.CreateAccount();
            var accountId = AssemblyLifecycle.CreateAndTrackEntity(account);
            int totalContacts = 5010;
            int batchSize = 100; // Dataverse max is 1000; smaller batches reduce timeout risk

            // ACT
            var contacts = ContactTestHelpers.CreateBulkContacts(accountId, totalContacts); 
            AssemblyLifecycle.CreateAndTrackBatchEntities(contacts, batchSize);

            // ASSERT
            var accountEntityLogicalName = ContactTestHelpers.accountEntityLogicalName;
            var savedAccount = AssemblyLifecycle.OrgService.Retrieve(accountEntityLogicalName, accountId, new ColumnSet("d365fl_contactcount"));
            var contactCount = savedAccount.GetAttributeValue<int>("d365fl_contactcount");
            Assert.AreEqual(totalContacts, contactCount);
        }

        [TestMethod]
        public void Contact_PostOperation_Create_Sync_DoesNotThrow_WhenParentCustomerIsContact()
        {
            // ARRANGE — create a "parent" contact, then create a child contact whose Company is that contact
            var parentContact = ContactTestHelpers.CreateContact("Parent", "Contact");
            parentContact.Id = AssemblyLifecycle.CreateAndTrackEntity(parentContact);

            var childContact = new Entity(ContactTestHelpers.contactEntityLogicalName);
            childContact["firstname"] = "Child";
            childContact["lastname"] = "Contact";
            childContact["parentcustomerid"] = new EntityReference(ContactTestHelpers.contactEntityLogicalName, parentContact.Id);

            // ACT — must NOT throw; parentcustomerid referencing a contact is valid in the UI
            AssemblyLifecycle.CreateAndTrackEntity(childContact);

            // ASSERT — no exception means the create succeeded
        }
    }
}
