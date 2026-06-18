using FakeXrmEasy.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace D365FL.Dataverse.PluginHelper.SamplePlugin.UnitTests.Contact.PostOperation_Update
{
    public abstract class Contact_PostOperation_Update_SyncTestBase : ContactPluginTestBase
    {
        protected IXrmFakedContext _context;

        [TestInitialize]
        public void Setup() => _context = CreateContext();
    }
}
