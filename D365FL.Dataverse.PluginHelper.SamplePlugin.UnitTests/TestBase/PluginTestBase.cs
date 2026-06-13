using System;
using System.Reflection;
using FakeXrmEasy.Abstractions;
using FakeXrmEasy.Abstractions.Enums;
using FakeXrmEasy.Middleware;
using FakeXrmEasy.Middleware.Crud;
using FakeXrmEasy.Middleware.Crud.FakeMessageExecutors;
using FakeXrmEasy.Middleware.Messages;
using FakeXrmEasy.Plugins;
using Microsoft.Xrm.Sdk;

namespace D365FL.Dataverse.PluginHelper.SamplePlugin.UnitTests.TestBase
{

    public abstract class PluginTestBase
    {
        protected const string AccountLogicalName = "account";
        protected const string ContactLogicalName = "contact";
        protected const string PreImageName = "PreImage";

        protected const int PreOperationStage = 20;
        protected const int PostOperationStage = 40;
        protected const int SynchronousMode = 0;

        public static class ValidationMessages
        {
            public const string DefaultErrorMessageForInvalidPluginConfig = "Plugin is not configured correctly. Errored config rules: ";
            public const string DoesNotExceedMaxDepth_Of_1 = "DoesNotExceedMaxDepth_Of_1";
            public const string DoesNotExceedMaxDepth_Of_2 = "DoesNotExceedMaxDepth_Of_2";
            public const string DoesNotExceedMaxDepth_Of_3 = "DoesNotExceedMaxDepth_Of_3";
            public const string HasTargetEntityLogicalName_account = "HasTargetEntityLogicalName_account";
            public const string IsUpdateMessage = "IsUpdateMessage";
            public const string IsCreateMessage = "IsCreateMessage";
            public const string HasPreImage = "HasPreImage";
            public const string IsPreOperation = "IsPreOperation";
            public const string IsSynchronous = "IsSynchronous";
        }
        
        // Boundary value constants
        protected const string SpecialCharacters = "!@#$%^&*()-_=+;:',.<>?";

        protected static IXrmFakedContext CreateContext()
        {
            return MiddlewareBuilder
                .New()
                .SetLicense(FakeXrmEasyLicense.RPL_1_5)
                .AddCrud()
                .AddFakeMessageExecutors(Assembly.GetAssembly(typeof(RetrieveMultipleRequestExecutor)))
                .Build();
        }

        protected Entity CreateContactWithParent(Guid accountId, Guid? contactId = null)
        {
            var entity = new Entity(ContactLogicalName, contactId ?? Guid.NewGuid());
            entity["parentcustomerid"] = new EntityReference(AccountLogicalName, accountId);
            return entity;
        }
    }
}
