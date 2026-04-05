using D365FL.Dataverse.PluginHelper.Core.IntegrationTests.Dependencies;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Concurrent;
using D365FL.Dataverse.PluginHelper.Core.IEnumerableExtensions;

namespace D365FL.Dataverse.PluginHelper.Core.IntegrationTests
{
    [TestClass]
    public class AssemblyLifecycle
    {
        public static ServiceClient OrgService { get; private set; } = null;

        private static ConcurrentBag<EntityReference> _entitiesToDelete = new ConcurrentBag<EntityReference>();

        public static Guid CreateAndTrackEntity(Entity entity)
        {
            var id = OrgService.Create(entity);
            AddEntityToDelete(entity.LogicalName, id);

            return id;
        }

        public static Guid CreateAndTrackEntity(CreateRequest request)
        {
            var response = (CreateResponse)OrgService.Execute(request);
            AddEntityToDelete(request.Target.LogicalName, response.id);

            return response.id;
        }
        public static void AddEntityToDelete(string entityLogicalName, Guid id)
        {
            _entitiesToDelete.Add(new EntityReference(entityLogicalName, id));
        }

        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            OrgService = ServiceClientFactory.GetServiceClient();
        }

        private static void DeleteTrackedEntities()
        {
            if (_entitiesToDelete.Count == 0)
                return; // Exit as there is nothing to delete

            var batches = _entitiesToDelete.Chunkify(100);

            foreach (var batch in batches)
            {
                var requests = new ExecuteMultipleRequest
                {
                    Requests = new OrganizationRequestCollection(),
                    Settings = new ExecuteMultipleSettings { ContinueOnError = true }
                };

                batch.ForEach(e => { requests.Requests.Add(new DeleteRequest { Target = e }); });

                var response = (ExecuteMultipleResponse)OrgService.Execute(requests);

                // TODO log failures to a text file
                foreach (var result in response.Responses)
                    if (result.Fault != null)
                        Console.WriteLine($"Delete failed for index {result.RequestIndex}: {result.Fault.Message}");
            }
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            DeleteTrackedEntities();
            OrgService?.Dispose();
        }
    }
}
