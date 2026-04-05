using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System;
using System.ServiceModel;

namespace D365FL.Dataverse.PluginHelper.Core.IntegrationTests.DataverseAsserts
{
    public static class PluginErrorAsserts
    {
        public static FaultException<OrganizationServiceFault> AssertPluginError(
            Action triggeringFunction,
            string expectedErrorMessage = null,
            string message = null)
        {
            if (triggeringFunction == null) throw new ArgumentNullException(nameof(triggeringFunction));

            var ex = Assert.ThrowsException<FaultException<OrganizationServiceFault>>(triggeringFunction);

            if (expectedErrorMessage != null)
            {
                Assert.AreEqual(expectedErrorMessage, ex.Detail.Message, message);
            }

            return ex;
        }
    }
}
