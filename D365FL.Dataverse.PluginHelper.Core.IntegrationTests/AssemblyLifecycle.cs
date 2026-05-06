using D365FL.Dataverse.PluginHelper.Core.IntegrationTests.Dependencies;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Concurrent;
using D365FL.Dataverse.PluginHelper.Core.IEnumerableExtensions;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Configuration;

namespace D365FL.Dataverse.PluginHelper.Core.IntegrationTests
{
    [TestClass]
    public class AssemblyLifecycle
    {
        private static bool _skipPerformanceTests = false;
        private static bool _skipPerformanceTestsSet = false;
        public static bool SkipPerformanceTests { get { 
                if(!_skipPerformanceTestsSet)
                {
                    _skipPerformanceTests = bool.Parse(ConfigurationManager.AppSettings["SkipPerformanceTests"]);
                    _skipPerformanceTestsSet = true;
                }
                return _skipPerformanceTests;
            } }
        public static ServiceClient OrgService { get; private set; } = null;

        private static ConcurrentBag<EntityReference> _entitiesToDelete = new ConcurrentBag<EntityReference>();
        public static Guid CreateAndTrackEntity(Entity entity)
        {
            var id = OrgService.Create(entity);
            AddEntityToDelete(entity.LogicalName, id);

            return id;
        }

        public static void UpdateEntity(Entity entity)
        {
            OrgService.Update(entity);
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
        private static List<ExecuteMultipleRequest> CreateBatchCreateRequests(IEnumerable<List<Entity>> batches)
        {
            var request = new List<ExecuteMultipleRequest>();
            foreach (var batch in batches)
            {
                var executeMultipleRequest = new ExecuteMultipleRequest
                {
                    Settings = new ExecuteMultipleSettings
                    {
                        ContinueOnError = false,  // Stop if one record fails
                        ReturnResponses = true   // Capture responses for error checking
                    },
                    Requests = new OrganizationRequestCollection()
                };

                foreach (var entity in batch)
                {
                    executeMultipleRequest.Requests.Add(new CreateRequest { Target = entity });
                }

                request.Add(executeMultipleRequest);
            }

            return request;
        }

        private static void ExecuteBatchRequests(IEnumerable<ExecuteMultipleRequest> batchRequests, string entityLogicalName)
        {
            foreach (var request in batchRequests)
            {
                var response =
                    (ExecuteMultipleResponse)OrgService.Execute(request);

                var sb = new StringBuilder();
                if (response.IsFaulted)
                {
                    foreach (ExecuteMultipleResponseItem responseItem in response.Responses)
                    {
                        if (responseItem.Fault != null)
                        {
                            sb.AppendLine(
                                $"  Error on request index {responseItem.RequestIndex}: " +
                                $"{responseItem.Fault.Message}"
                            );
                        }
                    }

                    throw new SystemException(sb.ToString());
                }
                else
                {
                    foreach (ExecuteMultipleResponseItem responseItem in response.Responses)
                    {
                        var id = ((CreateResponse)responseItem.Response).id;
                        AddEntityToDelete(entityLogicalName, id);
                    }
                }
            }
        }
        public static void CreateAndTrackBatchEntities(IEnumerable<Entity> entitiesToCreate, int batchSize = 100)
        {
            
            if (!entitiesToCreate.Any())
                throw new ArgumentException("entitiesToCreate cannot be empty.");
            
            var logicalNames = entitiesToCreate.Select(e => e.LogicalName).Distinct().ToList();
            if (logicalNames.Count != 1)
                throw new ArgumentException("Batch must be for 1 entity. It cannot be for multiple entities eg Contacts and Accounts.");

            var entityLogicalName = logicalNames.First();

            var batches = entitiesToCreate.Chunkify(batchSize);
            var batchRequests = CreateBatchCreateRequests(batches);

            ExecuteBatchRequests(batchRequests, entityLogicalName);

        }

        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            OrgService = ServiceClientFactory.GetServiceClient();
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            DeleteTrackedEntities();
            OrgService?.Dispose();
        }
    }
}
