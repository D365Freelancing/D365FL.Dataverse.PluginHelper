using D365FL.Dataverse.PluginHelper.Core.IntegrationTests.DataverseAsserts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace D365FL.Dataverse.PluginHelper.Core.IntegrationTests.Plugins.Account
{
    [TestClass]
    public class Account_PreOperation_Create_Sync_ExecuteDataversePlugin
    {
        private const string accountEntityLogicalName = "account";

        private Entity CreateAccount(string telephone1, string tickerSymbol)
        {
            var account = new Entity(accountEntityLogicalName);
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
        public void Account_PreOperation_Create_Sync_SetsNameCorrectly_WhenTickerSymbolAndPhoneSet()
        {
            // ARRANGE
            var telephone1 = "999 9999";
            var tickerSymbol = "QQQ";
            var expectedName = CreateName(telephone1, tickerSymbol);
            var account = CreateAccount(telephone1, tickerSymbol);

            // ACT
            var id = AssemblyLifecycle.CreateAndTrackEntity(account);

            var createdEntity = AssemblyLifecycle.OrgService.Retrieve("account", id, new ColumnSet("name"));
            var actualName = createdEntity.GetAttributeValue<string>("name");

            // ASSERT
            Assert.AreEqual(expectedName, actualName, "Name not set correctly");
        }

        #endregion

        #region "Validate Required Field Tests - Ticker Symbol"

        [TestMethod]
        public void Account_PreOperation_Create_Sync_ReturnsValidationError_WhenTickerSymbolIsNull()
        {
            // ARRANGE
            var telephone1 = "999 9999";
            string tickerSymbol = null;
            var account = CreateAccount(telephone1, tickerSymbol);

            // ACT & ASSERT
            var expectedError = "Cannot save Account — the following required fields are missing or empty: tickersymbol";
            PluginErrorAsserts.AssertPluginError(() => AssemblyLifecycle.OrgService.Create(account), expectedError);
        }

        [TestMethod]
        public void Account_PreOperation_Create_Sync_ReturnsValidationError_WhenTickerSymbolIsEmpty()
        {
            // ARRANGE
            var telephone1 = "999 9999";
            string tickerSymbol = string.Empty;
            var account = CreateAccount(telephone1, tickerSymbol);

            // ACT & ASSERT
            var expectedError = "Cannot save Account — the following required fields are missing or empty: tickersymbol";
            PluginErrorAsserts.AssertPluginError(() => AssemblyLifecycle.OrgService.Create(account), expectedError);
        }

        [TestMethod]
        public void Account_PreOperation_Create_Sync_ReturnsValidationError_WhenTickerSymbolIsMissing()
        {
            // ARRANGE
            var telephone1 = "999 9999";

            var account = new Entity(accountEntityLogicalName);
            account["telephone1"] = telephone1;

            // ACT & ASSERT
            var expectedError = "Cannot save Account — the following required fields are missing or empty: tickersymbol";
            PluginErrorAsserts.AssertPluginError(() => AssemblyLifecycle.OrgService.Create(account), expectedError);
        }
        #endregion

        #region "Validate Required Field Tests - Telephone1"

        [TestMethod]
        public void Account_PreOperation_Create_Sync_ReturnsValidationError_WhenTelephone1IsNull()
        {
            // ARRANGE
            string telephone1 = null;
            var tickerSymbol = "QQQ";
            var account = CreateAccount(telephone1, tickerSymbol);

            // ACT & ASSERT
            var expectedError = "Cannot save Account — the following required fields are missing or empty: telephone1";
            PluginErrorAsserts.AssertPluginError(() => AssemblyLifecycle.OrgService.Create(account), expectedError);
        }

        [TestMethod]
        public void Account_PreOperation_Create_Sync_ReturnsValidationError_WhenTelephone1IsEmpty()
        {
            // ARRANGE
            string telephone1 = string.Empty;
            var tickerSymbol = "QQQ";
            var account = CreateAccount(telephone1, tickerSymbol);

            // ACT & ASSERT
            var expectedError = "Cannot save Account — the following required fields are missing or empty: telephone1";
            PluginErrorAsserts.AssertPluginError(() => AssemblyLifecycle.OrgService.Create(account), expectedError);
        }

        [TestMethod]
        public void Account_PreOperation_Create_Sync_ReturnsValidationError_WhenTelephone1IsMissing()
        {
            // ARRANGE
            var tickerSymbol = "QQQ";

            var account = new Entity(accountEntityLogicalName);
            account["tickersymbol"] = tickerSymbol;

            // ACT & ASSERT
            var expectedError = "Cannot save Account — the following required fields are missing or empty: telephone1";
            PluginErrorAsserts.AssertPluginError(() => AssemblyLifecycle.OrgService.Create(account), expectedError);
        }

        #endregion

        #region "Validate Required Field Tests - Both Fields"

        [TestMethod]
        public void Account_PreOperation_Create_Sync_ReturnsValidationError_WhenAllFieldsAreNull()
        {
            // ARRANGE
            string telephone1 = null;
            string tickerSymbol = null;
            var account = CreateAccount(telephone1, tickerSymbol);

            // ACT & ASSERT
            var ex = PluginErrorAsserts.AssertPluginError(() => AssemblyLifecycle.OrgService.Create(account));
            Assert.IsTrue(ex.Detail.Message.Contains("telephone1"), "Error does not contain telephone1");
            Assert.IsTrue(ex.Detail.Message.Contains("tickersymbol"), "Error does not contain tickersymbol");
        }

        [TestMethod]
        public void Account_PreOperation_Create_Sync_ReturnsValidationError_WhenAllFieldsAreMissing()
        {
            // ARRANGE
            var account = new Entity(accountEntityLogicalName);

            // ACT & ASSERT
            var ex = PluginErrorAsserts.AssertPluginError(() => AssemblyLifecycle.OrgService.Create(account));
            Assert.IsTrue(ex.Detail.Message.Contains("telephone1"), "Error does not contain telephone1");
            Assert.IsTrue(ex.Detail.Message.Contains("tickersymbol"), "Error does not contain tickersymbol");
        }

        [TestMethod]
        public void Account_PreOperation_Create_Sync_ReturnsValidationError_WhenAllFieldsAreEmpty()
        {
            // ARRANGE
            string telephone1 = string.Empty;
            string tickerSymbol = string.Empty;
            var account = CreateAccount(telephone1, tickerSymbol);

            // ACT & ASSERT
            var ex = PluginErrorAsserts.AssertPluginError(() => AssemblyLifecycle.OrgService.Create(account));
            Assert.IsTrue(ex.Detail.Message.Contains("telephone1"), "Error does not contain telephone1");
            Assert.IsTrue(ex.Detail.Message.Contains("tickersymbol"), "Error does not contain tickersymbol");
        }

        #endregion
    }
}