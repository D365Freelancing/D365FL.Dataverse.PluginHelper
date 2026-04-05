using D365FL.Dataverse.PluginHelper.Core.IntegrationTests.DataverseAsserts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace D365FL.Dataverse.PluginHelper.Core.IntegrationTests.Plugins.Account
{
    [TestClass]
    public class Account_PreOperation_Update_Sync_ExecuteDataversePlugin
    {
        private const string accountEntityLogicalName = "account";

        private Entity CreateAccount(string telephone1 = "999 111", string tickerSymbol= "AA")
        {
            var account = new Entity(accountEntityLogicalName);
            account["telephone1"] = telephone1;
            account["tickersymbol"] = tickerSymbol;
            var id = AssemblyLifecycle.CreateAndTrackEntity(account);
            account.Id = id;
            return account;
        }

        private Entity CreateAccountInCorruptedState(
            string telephone1 = null, 
            string tickerSymbol = null)
        {
            var account = new Entity(accountEntityLogicalName);

            if(telephone1 != null) account["telephone1"] = telephone1;
            if (tickerSymbol != null) account["tickersymbol"] = tickerSymbol;
            
            var createRequest = new CreateRequest()
            {
                Target = account,
            };
            createRequest.Parameters.Add("BypassCustomPluginExecution", true);
                        
            var id = AssemblyLifecycle.CreateAndTrackEntity(createRequest);
            account.Id = id;
            return account;
        }

        private Entity UpdateAccount(Guid id, string telephone1, string tickerSymbol)
        {
            var account = new Entity(accountEntityLogicalName, id);
            account["telephone1"] = telephone1;
            account["tickersymbol"] = tickerSymbol;
            
            return account;
        }

        private string CreateName(string telephone1, string tickerSymbol)
        {
            return $"{tickerSymbol} - {telephone1}";
        }

        #region "Set Name Tests"

        [TestMethod]
        public void Account_PreOperation_Update_Sync_SetsNameCorrectly_WhenTickerSymbolAndPhoneSet()
        {
            // ARRANGE
            var account = CreateAccount();

            // ACT
            var telephone1 = "999 9999";
            var tickerSymbol = "QQQ";
            var id = account.Id;
            
            var updateEntity = UpdateAccount(id, telephone1, tickerSymbol);
            AssemblyLifecycle.OrgService.Update(updateEntity);

            // ASSERT
            var expectedName = CreateName(telephone1, tickerSymbol);
            var saveEntity = AssemblyLifecycle.OrgService.Retrieve("account", id, new ColumnSet("name"));
            var actualName = saveEntity.GetAttributeValue<string>("name");
            Assert.AreEqual(expectedName, actualName, "Name not set correctly");
        }

        [TestMethod]
        public void Account_PreOperation_Update_Sync_SetsNameCorrectly_WhenOriginalFieldsAreMissing()
        {
            // Create an account in a corrupted state. Plugins should
            // protect this state from ever being created, but we are testing
            // it anyway incase old data is in this state

            // ARRANGE
            var account = CreateAccountInCorruptedState();

            // ACT
            var telephone1 = "999 9999";
            var tickerSymbol = "QQ";
            var id = account.Id;
            var updateEntity = UpdateAccount(id, telephone1, tickerSymbol);
            AssemblyLifecycle.OrgService.Update(updateEntity);

            // ASSERT
            var expectedName = CreateName(telephone1, tickerSymbol);
            var saveEntity = AssemblyLifecycle.OrgService.Retrieve("account", id, new ColumnSet("name"));
            var actualName = saveEntity.GetAttributeValue<string>("name");
            Assert.AreEqual(expectedName, actualName, "Name not set correctly");
        }

        #endregion

        #region "Validate Required Field Tests - Ticker Symbol"

        [TestMethod]
        public void Account_PreOperation_Update_Sync_ReturnsValidationError_WhenTickerSymbolIsEmpty()
        {
            // ARRANGE
            var account = CreateAccount();

            var telephone1 = "999 9999";
            var tickerSymbol = "";
            var id = account.Id;

            var updateEntity = UpdateAccount(id, telephone1, tickerSymbol);

            // ACT & ASSERT
            var expectedError = "Cannot save Account — the following required fields are missing or empty: tickersymbol";
            PluginErrorAsserts.AssertPluginError(() => AssemblyLifecycle.OrgService.Update(updateEntity), expectedError);
        }

        [TestMethod]
        public void Account_PreOperation_Update_Sync_ReturnsValidationError_WhenTickerSymbolIsNull()
        {
            // ARRANGE
            var account = CreateAccount();
            var telephone1 = "999 9999";
            string tickerSymbol = null;
            var id = account.Id;

            var updateEntity = UpdateAccount(id, telephone1, tickerSymbol);

            // ACT & ASSERT
            var expectedError = "Cannot save Account — the following required fields are missing or empty: tickersymbol";
            PluginErrorAsserts.AssertPluginError(() => AssemblyLifecycle.OrgService.Update(updateEntity), expectedError);
        }

        [TestMethod]
        public void Account_PreOperation_Update_Sync_ReturnsValidationError_WhenTickerSymbolIsMissing()
        {
            // Create an account in a corrupted state. Plugins should
            // protect this state from ever being created, but we are testing
            // it anyway incase old data is in this state

            // the telephone1 and tickersymbol fields will never be missing unless they were not
            // set on create

            // ARRANGE
            var account = CreateAccountInCorruptedState("999 9991");

            var telephone1 = "999 9999";
            var id = account.Id;

            var updateEntity = new Entity(accountEntityLogicalName, id);
            updateEntity["telephone1"] = telephone1;

            // ACT & ASSERT
            var expectedError = "Cannot save Account — the following required fields are missing or empty: tickersymbol";
            PluginErrorAsserts.AssertPluginError(() => AssemblyLifecycle.OrgService.Update(updateEntity), expectedError);
        }

        #endregion

        #region "Validate Required Field Tests - Telephone1"

        [TestMethod]
        public void Account_PreOperation_Update_Sync_ReturnsValidationError_WhenTelephone1IsEmpty()
        {
            // ARRANGE
            var account = CreateAccount();

            string telephone1 = "";
            var tickerSymbol = "QQQ";
            var id = account.Id;

            var updateEntity = UpdateAccount(id, telephone1, tickerSymbol);

            // ACT & ASSERT
            var expectedError = "Cannot save Account — the following required fields are missing or empty: telephone1";
            PluginErrorAsserts.AssertPluginError(() => AssemblyLifecycle.OrgService.Update(updateEntity), expectedError);
        }

        [TestMethod]
        public void Account_PreOperation_Update_Sync_ReturnsValidationError_WhenTelephone1IsNull()
        {
            // ARRANGE
            var account = CreateAccount();

            string telephone1 = null;
            var tickerSymbol = "QQQ";
            var id = account.Id;

            var updateEntity = UpdateAccount(id, telephone1, tickerSymbol);

            // ACT & ASSERT
            var expectedError = "Cannot save Account — the following required fields are missing or empty: telephone1";
            PluginErrorAsserts.AssertPluginError(() => AssemblyLifecycle.OrgService.Update(updateEntity), expectedError);
        }

        [TestMethod]
        public void Account_PreOperation_Update_Sync_ReturnsValidationError_WhenTelephone1IsMissing()
        {
            // Create an account in a corrupted state. Plugins should
            // protect this state from ever being created, but we are testing
            // it anyway incase old data is in this state

            // the telephone1 and tickersymbol fields will never be missing unless they were not
            // set on create

            // ARRANGE
            var account = CreateAccountInCorruptedState(null, "AA");

            var tickerSymbol = "TT";
            var id = account.Id;

            var updateEntity = new Entity(accountEntityLogicalName, id);
            updateEntity["tickersymbol"] = tickerSymbol;

            // ACT & ASSERT
            var expectedError = "Cannot save Account — the following required fields are missing or empty: telephone1";
            PluginErrorAsserts.AssertPluginError(() => AssemblyLifecycle.OrgService.Update(updateEntity), expectedError);
        }

        #endregion 

        #region "Validate Required Field Tests - Both Fields"

        [TestMethod]
        public void Account_PreOperation_Update_Sync_ReturnsValidationError_WhenTelephone1IsEmptyAndTickerSymbolIsNull()
        {
            // ARRANGE
            var account = CreateAccount();

            var telephone1 = string.Empty;
            string tickerSymbol = null;
            var id = account.Id;

            var updateEntity = UpdateAccount(id, telephone1, tickerSymbol);

            // ACT & ASSERT
            var ex = PluginErrorAsserts.AssertPluginError(() => AssemblyLifecycle.OrgService.Update(updateEntity));
            Assert.IsTrue(ex.Detail.Message.Contains("telephone1"), "Error does not contain telephone1");
            Assert.IsTrue(ex.Detail.Message.Contains("tickersymbol"), "Error does not contain tickersymbol");
        }

        [TestMethod]
        public void Account_PreOperation_Update_Sync_ReturnsValidationError_WhenAllFieldsAreNull()
        {
            // ARRANGE
            var account = CreateAccount();

            string telephone1 = null;
            string tickerSymbol = null;
            var id = account.Id;

            var updateEntity = UpdateAccount(id, telephone1, tickerSymbol);

            // ACT & ASSERT
            var ex = PluginErrorAsserts.AssertPluginError(() => AssemblyLifecycle.OrgService.Update(updateEntity));
            Assert.IsTrue(ex.Detail.Message.Contains("telephone1"), "Error does not contain telephone1");
            Assert.IsTrue(ex.Detail.Message.Contains("tickersymbol"), "Error does not contain tickersymbol");
        }

        #endregion
        [TestMethod]
        public void Account_PreOperation_Update_Sync_PluginExits_WhenDataNotChanged()
        {
            // TODO how to determine plugin exited

            // ARRANGE
            var telephone1 = "999 9999";
            var tickerSymbol = "QQ";
            var account = CreateAccount(telephone1, tickerSymbol);

            // ACT
            var id = account.Id;
            var updateEntity = UpdateAccount(id, telephone1, tickerSymbol);
            AssemblyLifecycle.OrgService.Update(updateEntity);

            // ASSERT
            var expectedName = CreateName(telephone1, tickerSymbol);
            var saveEntity = AssemblyLifecycle.OrgService.Retrieve("account", id, new ColumnSet("name"));
            var actualName = saveEntity.GetAttributeValue<string>("name");
            Assert.AreEqual(expectedName, actualName, "Name not set correctly");
        }
    }
}