using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk.Query;

namespace D365FL.Dataverse.PluginHelper.Core.IntegrationTests.Plugins.Contact
{
    [TestClass]
    public class Contact_PostOperation_Delete_Sync_ExecuteDataversePlugin
    {        
        [TestMethod]
        public void Contact_PostOperation_Delete_Sync_GetsCorrectContactCount_AfterContactDeleted()
        {
            // ARRANGE
            var account = ContactTestHelpers.CreateAccount();
            
            var accounId = AssemblyLifecycle.CreateAndTrackEntity(account);

            var contact1 = ContactTestHelpers.CreateContact("John", "Smith", accounId);
            var contact2 = ContactTestHelpers.CreateContact("Jane", "Smith", accounId);
            
            contact1.Id = AssemblyLifecycle.CreateAndTrackEntity(contact1);
            contact2.Id = AssemblyLifecycle.Create(contact2);

            // ACT
            // delete contact 1
            AssemblyLifecycle.DeleteEntity(contact2);

            // ASSERT
            var accountEntityLogicalName = ContactTestHelpers.accountEntityLogicalName;
            var savedAccount = AssemblyLifecycle.OrgService.Retrieve(accountEntityLogicalName, accounId, new ColumnSet("d365fl_contactcount"));
            var contactCountForAccount = savedAccount.GetAttributeValue<int>("d365fl_contactcount");
            
            Assert.AreEqual(1, contactCountForAccount);
        }

        public void Contact_PostOperation_Delete_Sync_GetsCorrectContactCount_AfterAllContactsDeleted()
        {
            // ARRANGE
            var account = ContactTestHelpers.CreateAccount();

            var accounId = AssemblyLifecycle.CreateAndTrackEntity(account);

            var contact1 = ContactTestHelpers.CreateContact("John", "Smith", accounId);
            var contact2 = ContactTestHelpers.CreateContact("Jane", "Smith", accounId);

            contact1.Id = AssemblyLifecycle.Create(contact1);
            contact2.Id = AssemblyLifecycle.Create(contact2);

            // ACT
            // delete contact 1
            AssemblyLifecycle.DeleteEntity(contact1);
            AssemblyLifecycle.DeleteEntity(contact2);

            // ASSERT
            var accountEntityLogicalName = ContactTestHelpers.accountEntityLogicalName;
            var savedAccount = AssemblyLifecycle.OrgService.Retrieve(accountEntityLogicalName, accounId, new ColumnSet("d365fl_contactcount"));
            var contactCountForAccount = savedAccount.GetAttributeValue<int>("d365fl_contactcount");

            Assert.AreEqual(0, contactCountForAccount);
        }
    }
}
