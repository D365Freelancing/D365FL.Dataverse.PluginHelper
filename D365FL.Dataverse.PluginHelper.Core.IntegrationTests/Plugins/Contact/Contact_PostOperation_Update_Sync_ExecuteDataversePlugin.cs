using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk.Query;

namespace D365FL.Dataverse.PluginHelper.Core.IntegrationTests.Plugins.Contact
{
    [TestClass]
    public class Contact_PostOperation_Update_Sync_ExecuteDataversePlugin
    {        
        [TestMethod]
        public void Contact_PostOperation_Update_Sync_GetsCorrectContactCount_ForOriginalAndNewAccount()
        {
            // ARRANGE
            var account1 = ContactTestHelpers.CreateAccount("1", "ACC");
            var account2 = ContactTestHelpers.CreateAccount("2", "ACC");
            var accoun1Id = AssemblyLifecycle.CreateAndTrackEntity(account1);
            var account2Id = AssemblyLifecycle.CreateAndTrackEntity(account2);

            var contact1 = ContactTestHelpers.CreateContact("John", "Smith", accoun1Id);
            var contact2 = ContactTestHelpers.CreateContact("Jane", "Smith", account2Id);
            
            contact1.Id = AssemblyLifecycle.CreateAndTrackEntity(contact1);
            contact2.Id = AssemblyLifecycle.CreateAndTrackEntity(contact2);

            // ACT
            // update contact1 to account2
            ContactTestHelpers.UpdateParentCustomerIdToAccount(contact1, account2Id); 
            AssemblyLifecycle.UpdateEntity(contact1);

            // ASSERT
            var accountEntityLogicalName = ContactTestHelpers.accountEntityLogicalName;
            var savedAccount1 = AssemblyLifecycle.OrgService.Retrieve(accountEntityLogicalName, accoun1Id, new ColumnSet("d365fl_contactcount"));
            var savedAccount2 = AssemblyLifecycle.OrgService.Retrieve(accountEntityLogicalName, account2Id, new ColumnSet("d365fl_contactcount"));
            var contactCountForAccount1 = savedAccount1.GetAttributeValue<int>("d365fl_contactcount");
            var contactCountForAccount2 = savedAccount2.GetAttributeValue<int>("d365fl_contactcount");
            
            Assert.AreEqual(0, contactCountForAccount1);
            Assert.AreEqual(2, contactCountForAccount2);
        }

        [TestMethod]
        public void Contact_PostOperation_Update_Sync_GetsCorrectContactCount_WhenCompanyIsSetForTheFirstTime()
        {
            // ARRANGE — contact created with no Company/parentcustomerid
            var account = ContactTestHelpers.CreateAccount("1", "ACC");
            var accountId = AssemblyLifecycle.CreateAndTrackEntity(account);

            var contact = ContactTestHelpers.CreateContact("John", "Smith");
            contact.Id = AssemblyLifecycle.CreateAndTrackEntity(contact);

            // ACT — set Company for the first time
            ContactTestHelpers.UpdateParentCustomerIdToAccount(contact, accountId);
            AssemblyLifecycle.UpdateEntity(contact);

            // ASSERT
            var savedAccount = AssemblyLifecycle.OrgService.Retrieve(
                ContactTestHelpers.accountEntityLogicalName, accountId, new ColumnSet("d365fl_contactcount"));
            var contactCount = savedAccount.GetAttributeValue<int>("d365fl_contactcount");

            Assert.AreEqual(1, contactCount);
        }

        [TestMethod]
        public void Contact_PostOperation_Update_Sync_DoesNotThrow_WhenCompanyChangesFromAccountToContact()
        {
            // ARRANGE — contact starts with an account parent, then is reassigned to a contact parent
            var account = ContactTestHelpers.CreateAccount("1", "ACC");
            var accountId = AssemblyLifecycle.CreateAndTrackEntity(account);

            var parentContact = ContactTestHelpers.CreateContact("Parent", "Contact");
            parentContact.Id = AssemblyLifecycle.CreateAndTrackEntity(parentContact);

            var contact = ContactTestHelpers.CreateContact("John", "Smith", accountId);
            contact.Id = AssemblyLifecycle.CreateAndTrackEntity(contact);

            // ACT — reassign to a contact-typed parent (valid Customer lookup value)
            ContactTestHelpers.UpdateParentCustomerIdToContact(contact, parentContact.Id);
            
            AssemblyLifecycle.UpdateEntity(contact);

            // ASSERT — old account's count should drop to 0, no exception thrown
            var savedAccount = AssemblyLifecycle.OrgService.Retrieve(
                ContactTestHelpers.accountEntityLogicalName, accountId, new ColumnSet("d365fl_contactcount"));
            var contactCount = savedAccount.GetAttributeValue<int>("d365fl_contactcount");
            Assert.AreEqual(0, contactCount);
        }
    }
}
