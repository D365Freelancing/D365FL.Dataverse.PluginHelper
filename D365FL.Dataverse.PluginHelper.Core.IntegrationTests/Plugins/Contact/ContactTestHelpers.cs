using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;

namespace D365FL.Dataverse.PluginHelper.Core.IntegrationTests.Plugins.Contact
{
    public static class ContactTestHelpers
    {
        public const string accountEntityLogicalName = "account";
        public const string contactEntityLogicalName = "contact";
        public static Entity CreateAccount(string telephone1 = "000", string tickerSymbol = "Test")
        {
            var account = new Entity(accountEntityLogicalName);
            account["telephone1"] = telephone1;
            account["tickersymbol"] = tickerSymbol;
            return account;
        }
        public static Entity CreateContact(string firstName, string lastName, Guid? parentCustomerId = null)
        {
            var contact = new Entity(contactEntityLogicalName);
            contact["firstname"] = firstName;
            contact["lastname"] = lastName;
            
            if(parentCustomerId != null)
                contact["parentcustomerid"] = new EntityReference(accountEntityLogicalName, parentCustomerId.Value);

            return contact;
        }
        public static void UpdateParentCustomerIdToAccount(Entity contact, Guid parentCustomerId)
        {
            contact["parentcustomerid"] = new EntityReference(accountEntityLogicalName, parentCustomerId);
        }

        public static void UpdateParentCustomerIdToContact(Entity contact, Guid parentCustomerId)
        {
            contact["parentcustomerid"] = new EntityReference(contactEntityLogicalName, parentCustomerId);
        }
        public static List<Entity> CreateBulkContacts(Guid accountId, int totalContacts)
        {
            var contacts = new List<Entity>();
            for (int i = 1; i <= totalContacts; i++)
            {
                var contact = CreateContact($"FirstName_{i}", $"LastName_{i}", accountId);
                contacts.Add(contact);
            }

            return contacts;
        }
    }
}
