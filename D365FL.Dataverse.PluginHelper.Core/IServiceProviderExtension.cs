using Microsoft.Xrm.Sdk;
using System;

namespace D365FL.Dataverse.PluginHelper.Core
{
    public static class IServiceProviderExtension
    {
        // TODO requires testing
        private static IOrganizationServiceFactory GetOrgServiceFactory(this IServiceProvider serviceProvider)
        {
            return (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
        }
        public static IOrganizationService GetAdminOrgService(this IServiceProvider serviceProvider)
        {
            return serviceProvider.GetOrgServiceFactory().CreateOrganizationService(null);
        }

        public static IOrganizationService GetOrgServiceAs(this IServiceProvider serviceProvider, Guid userId)
        {
            return serviceProvider.GetOrgServiceFactory().CreateOrganizationService(userId);
        }
    }
}
