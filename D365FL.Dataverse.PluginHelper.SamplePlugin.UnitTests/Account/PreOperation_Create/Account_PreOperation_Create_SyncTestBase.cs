using FakeXrmEasy.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace D365FL.Dataverse.PluginHelper.SamplePlugin.UnitTests.Account.PreOperation_Create
{
    public abstract class Account_PreOperation_Create_SyncTestBase : AccountPluginTestBase
    {
        protected IXrmFakedContext _context;

        [TestInitialize]
        public void Setup() => _context = CreateContext();
    }
}
