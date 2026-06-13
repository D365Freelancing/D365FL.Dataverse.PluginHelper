using FakeXrmEasy.Plugins;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using D365FL.Dataverse.PluginHelper.SamplePlugin.Plugins.Account;

namespace D365FL.Dataverse.PluginHelper.SamplePlugin.UnitTests.Account.PreOperation_Create
{
    [TestClass]
    public class Account_PreOperation_Create_Sync_SetDefaultContactCountTests : Account_PreOperation_Create_SyncTestBase
    {
        // ─── Positive Tests ───────────────────────────────────────────────────────

        [TestMethod]
        public void Execute_SetsContactCountToZero_WhenValidTargetProvided()
        {
            // Arrange
            var target = CreateValidAccountTarget();
            var pluginCtx = BuildAccountCreateContext(_context, target);

            // Act
            _context.ExecutePluginWithConfigurations<Account_PreOperation_Create_Sync>(pluginCtx, null, null);

            // Assert
            var result = (Entity)pluginCtx.InputParameters["Target"];
            Assert.AreEqual(0, result["d365fl_contactcount"]);
        }
    }
}
