using FakeXrmEasy.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace D365FL.Dataverse.PluginHelper.SamplePlugin.UnitTests.Account.PreOperation_Update
{
    public abstract class Account_PreOperation_Update_SyncTestBase : AccountPluginTestBase
    {
        protected IXrmFakedContext _context;

        [TestInitialize]
        public void Setup() => _context = CreateContext();
    }
}
