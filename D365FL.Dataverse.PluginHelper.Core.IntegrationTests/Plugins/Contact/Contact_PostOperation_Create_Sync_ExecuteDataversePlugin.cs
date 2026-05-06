using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;

namespace D365FL.Dataverse.PluginHelper.Core.IntegrationTests.Plugins.Contact
{
    [TestClass]
    public class Contact_PostOperation_Create_Sync_ExecuteDataversePlugin
    {
        private const string accountEntityLogicalName = "account";
        private const string contactEntityLogicalName = "contact";
        private Entity CreateAccount(string telephone1 = "000", string tickerSymbol = "Test")
        {
            var account = new Entity(accountEntityLogicalName);
            account["telephone1"] = telephone1;
            account["tickersymbol"] = tickerSymbol;
            return account;
        }

        private Entity CreateContact(string firstName, string lastName, Guid parentCustomerId)
        {
            var contact = new Entity(contactEntityLogicalName);
            contact["firstname"] = firstName;
            contact["lastname"] = lastName;
            contact["parentcustomerid"] = new EntityReference(accountEntityLogicalName, parentCustomerId);
            return contact;
        }

        private List<Entity> CreateBulkContacts(Guid accountId, int totalContacts)
        {
            var contacts = new List<Entity>();
            for (int i = 1; i <= totalContacts; i++)
            {
                var contact = CreateContact($"FirstName_{i}", $"LastName_{i}", accountId);
                contacts.Add(contact);
            }

            return contacts;
        }

        
        [TestMethod]
        public void Contact_PostOperation_Create_Sync_GetsCorrectContactCount_AfterCreatingMultipleContacts()
        {
            // ARRANGE
            var account = CreateAccount();
            var accountId = AssemblyLifecycle.CreateAndTrackEntity(account);

            var contact1 = CreateContact("John", "Smith", accountId);
            var contact2 = CreateContact("Jane", "Smith", accountId);

            // ACT
            AssemblyLifecycle.CreateAndTrackEntity(contact1);
            AssemblyLifecycle.CreateAndTrackEntity(contact2);

            // ASSERT
            var savedAccount = AssemblyLifecycle.OrgService.Retrieve(accountEntityLogicalName, accountId, new ColumnSet("d365fl_contactcount"));
            var contactCount = savedAccount.GetAttributeValue<int>("d365fl_contactcount");
            Assert.AreEqual(2, contactCount);
        }

        [TestMethod]
        public void OnAccountCreate_HasZeroContactCount_ByDefault()
        {
            // ARRANGE
            var account = CreateAccount();

            // ACT
            var accountId = AssemblyLifecycle.CreateAndTrackEntity(account);

            // ASSERT
            var savedAccount = AssemblyLifecycle.OrgService.Retrieve(accountEntityLogicalName, accountId, new ColumnSet("d365fl_contactcount"));
            var contactCount = savedAccount.GetAttributeValue<int>("d365fl_contactcount");
            Assert.AreEqual(0, contactCount);
        }

        [TestMethod]
        public void Contact_PostOperation_Create_Sync_GetsCorrectContactCount_AfterCreatingOver5000Contacts()
        {
            // this is a performance test
            // and tests the max 5000 record limit in power platform
            // ARRANGE
            var account = CreateAccount();
            var accountId = AssemblyLifecycle.CreateAndTrackEntity(account);
            int totalContacts = 5010;
            int batchSize = 100; // Dataverse max is 1000; smaller batches reduce timeout risk

            // ACT
            var contacts = CreateBulkContacts(accountId, totalContacts); 
            AssemblyLifecycle.CreateAndTrackBatchEntities(contacts, batchSize);

            // ASSERT
            var savedAccount = AssemblyLifecycle.OrgService.Retrieve(accountEntityLogicalName, accountId, new ColumnSet("d365fl_contactcount"));
            var contactCount = savedAccount.GetAttributeValue<int>("d365fl_contactcount");
            Assert.AreEqual(totalContacts, contactCount);
        }
    }
}
