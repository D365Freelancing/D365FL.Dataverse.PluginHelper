# D365FL.Dataverse.PluginHelper

// PURPOSE
// 1. Provide descriptive methods to make reading the code easier
// 2. Provide descriptive methods to ensure no logical mistakes are made, and therefore avoid defects eg. context.Mesage = "Creat"


// Goal to have helper classes that provide highly readably, less error prone and self documenting code
// highly readably - The helper class makes the code highly readably with very descriptve method names. 
// less error prone - The helper class makes the code less error prone by having specific methods which removes the need for magic strings
// self documenting code - plugins and importantly plugin configuration is documented within the plugin code.
// If you have ever lost your plugin steps and had to re register them, then you will apprciate the plugin rules validating the plugin step
// registration config (NOTE this does rely on developers implementing the plugin rules correctly


// Entity Field Value Has Changed
// AttributeHasChanged - https://github.com/emerbrito/XrmUtils-Extensions/blob/master/src/XrmUtils.Extensions/Extensions/EntityExtensions.cs#L84

//var pluginUserService = localPluginContext.PluginUserService;
//var adminOrgService = localPluginContext.OrgSvcFactory.CreateOrganizationService(null);
//var adminOrgService2 = localPluginContext.ServiceProvider.GetAdminOrgService();
//var service= localPluginContext.InitiatingUserService
//var userId = Guid.NewGuid();
//var orgServiceAs = localPluginContext.OrgSvcFactory.CreateOrganizationService(userId);
//var orgServiceAs2 = localPluginContext.ServiceProvider.GetOrgServiceAs(userId);