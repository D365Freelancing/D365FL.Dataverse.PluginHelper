using D365FL.Dataverse.PluginHelper.Core.IntegrationTests.DataverseAsserts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;


namespace D365FL.Dataverse.PluginHelper.Core.IntegrationTests.Plugins.Account
{
    [TestClass]
    public class Account_PreOperation_Update_Sync_ExecuteDataversePlugin
    {

        #region "Set Name Tests"

        [TestMethod]
        public void Account_PreOperation_Update_Sync_SetsNameCorrectly_WhenTickerSymbolAndPhoneSet()
        {
            // ARRANGE
            var account = AccountTestHelpers.CreateAccount();
            var id = AssemblyLifecycle.CreateAndTrackEntity(account);

            // ACT
            var telephone1 = "999 9999";
            var tickerSymbol = "QQQ";
            
            var updateEntity = AccountTestHelpers.UpdateAccount(id, telephone1, tickerSymbol);
            AssemblyLifecycle.OrgService.Update(updateEntity);

            // ASSERT
            var expectedName = AccountTestHelpers.CreateName(telephone1, tickerSymbol);
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
            var request = AccountTestHelpers.CreateAccountInCorruptedState();
            var id = AssemblyLifecycle.CreateAndTrackEntity(request);

            // ACT
            var telephone1 = "999 9999";
            var tickerSymbol = "QQ";

            var updateEntity = AccountTestHelpers.UpdateAccount(id, telephone1, tickerSymbol);
            AssemblyLifecycle.OrgService.Update(updateEntity);

            // ASSERT
            var expectedName = AccountTestHelpers.CreateName(telephone1, tickerSymbol);
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
            var account = AccountTestHelpers.CreateAccount();
            var id = AssemblyLifecycle.CreateAndTrackEntity(account);

            var telephone1 = "999 9999";
            var tickerSymbol = "";

            var updateEntity = AccountTestHelpers.UpdateAccount(id, telephone1, tickerSymbol);

            // ACT & ASSERT
            var expectedError = "Cannot save Account — the following required fields are missing or empty: tickersymbol";
            PluginErrorAsserts.AssertPluginError(() => AssemblyLifecycle.OrgService.Update(updateEntity), expectedError);
        }

        [TestMethod]
        public void Account_PreOperation_Update_Sync_ReturnsValidationError_WhenTickerSymbolIsNull()
        {
            // ARRANGE
            var account = AccountTestHelpers.CreateAccount();
            var id = AssemblyLifecycle.CreateAndTrackEntity(account);

            var telephone1 = "999 9999";
            string tickerSymbol = null;
            
            var updateEntity = AccountTestHelpers.UpdateAccount(id, telephone1, tickerSymbol);

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
            var request = AccountTestHelpers.CreateAccountInCorruptedState("999 9991");
            var id = AssemblyLifecycle.CreateAndTrackEntity(request);

            var telephone1 = "999 9999";

            var updateEntity = new Entity(AccountTestHelpers.accountEntityLogicalName, id);
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
            var account = AccountTestHelpers.CreateAccount();
            var id = AssemblyLifecycle.CreateAndTrackEntity(account);

            string telephone1 = "";
            var tickerSymbol = "QQQ";

            var updateEntity = AccountTestHelpers.UpdateAccount(id, telephone1, tickerSymbol);

            // ACT & ASSERT
            var expectedError = "Cannot save Account — the following required fields are missing or empty: telephone1";
            PluginErrorAsserts.AssertPluginError(() => AssemblyLifecycle.OrgService.Update(updateEntity), expectedError);
        }

        [TestMethod]
        public void Account_PreOperation_Update_Sync_ReturnsValidationError_WhenTelephone1IsNull()
        {
            // ARRANGE
            var account = AccountTestHelpers.CreateAccount();
            var id = AssemblyLifecycle.CreateAndTrackEntity(account);

            string telephone1 = null;
            var tickerSymbol = "QQQ";

            var updateEntity = AccountTestHelpers.UpdateAccount(id, telephone1, tickerSymbol);

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
            var request = AccountTestHelpers.CreateAccountInCorruptedState(null, "AA");
            var id = AssemblyLifecycle.CreateAndTrackEntity(request);

            var tickerSymbol = "TT";

            var updateEntity = new Entity(AccountTestHelpers.accountEntityLogicalName, id);
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
            var account = AccountTestHelpers.CreateAccount();
            var id = AssemblyLifecycle.CreateAndTrackEntity(account);

            var telephone1 = string.Empty;
            string tickerSymbol = null;
            
            var updateEntity = AccountTestHelpers.UpdateAccount(id, telephone1, tickerSymbol);

            // ACT & ASSERT
            var ex = PluginErrorAsserts.AssertPluginError(() => AssemblyLifecycle.OrgService.Update(updateEntity));
            Assert.IsTrue(ex.Detail.Message.Contains("telephone1"), "Error does not contain telephone1");
            Assert.IsTrue(ex.Detail.Message.Contains("tickersymbol"), "Error does not contain tickersymbol");
        }

        [TestMethod]
        public void Account_PreOperation_Update_Sync_ReturnsValidationError_WhenAllFieldsAreNull()
        {
            // ARRANGE
            var account = AccountTestHelpers.CreateAccount();
            var id = AssemblyLifecycle.CreateAndTrackEntity(account);

            string telephone1 = null;
            string tickerSymbol = null;

            var updateEntity = AccountTestHelpers.UpdateAccount(id, telephone1, tickerSymbol);

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
            var account = AccountTestHelpers.CreateAccount(telephone1, tickerSymbol);
            var id = AssemblyLifecycle.CreateAndTrackEntity(account);

            // ACT
            var updateEntity = AccountTestHelpers.UpdateAccount(id, telephone1, tickerSymbol);
            AssemblyLifecycle.OrgService.Update(updateEntity);

            // ASSERT
            var expectedName = AccountTestHelpers.CreateName(telephone1, tickerSymbol);
            var saveEntity = AssemblyLifecycle.OrgService.Retrieve("account", id, new ColumnSet("name"));
            var actualName = saveEntity.GetAttributeValue<string>("name");
            Assert.AreEqual(expectedName, actualName, "Name not set correctly");
        }
    }
}