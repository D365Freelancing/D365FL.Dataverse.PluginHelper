using D365FL.Dataverse.PluginHelper.Core.UnitTests.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using D365FL.Dataverse.PluginHelper.Core.TracingServiceExtension;
using Microsoft.Xrm.Sdk;
using System;

namespace D365FL.Dataverse.PluginHelper.Core.UnitTests.TracingServiceExtensions
{
    [TestClass]
    public class TracingService_TraceEntity
    {

        private string Label => "TestEntity";
        private string EntityName => "Account";

        private Entity Entity => new Entity(EntityName) { Id = Guid.NewGuid() };

        [TestMethod]
        public void TraceEntity_OutputsEntityIsNull()
        {

            // ARRANGE
            Entity entity = null;
            // ACT
            var tracer = new TestTracingService();
            tracer.TraceEntity(entity, Label);

            // ASSERT
            var expectedOutput = $"[{Label}] Entity is null.";
            var actualOutput = tracer.TraceLogs;

            var expectedMessageWasLogged = actualOutput.Contains(expectedOutput);
            Assert.IsTrue(expectedMessageWasLogged);
        }

        [TestMethod]
        public void TraceEntity_OutputsEntityDetails()
        {

            // ARRANGE
            var entity = Entity;
            // ACT
            var tracer = new TestTracingService();
            tracer.TraceEntity(entity, Label);

            // ASSERT
            var expectedLine1 = $"[{Label}] LogicalName: {EntityName}";
            var expectedLine2 = $"[{Label}] Id: {entity.Id}";
            var expectedLine3 = $"[{Label}] Attribute Count: 0";

            var actualOutput = tracer.TraceLogs;

            Assert.IsTrue(actualOutput.Contains(expectedLine1), "Line 1 was NOT outputted");
            Assert.IsTrue(actualOutput.Contains(expectedLine2), "Line 2 was NOT outputted");
            Assert.IsTrue(actualOutput.Contains(expectedLine3), "Line 3 was NOT outputted");
        }

        [TestMethod]
        public void TraceEntity_OutputsStringAttribute()
        {
            // ARRANGE
            var entity = Entity;

            entity["stringValue"] = "string value";

            // ACT
            var tracer = new TestTracingService();
            tracer.TraceEntity(entity, Label);

            // ASSERT
            var actualOutput = tracer.TraceLogs;

            Assert.IsTrue(actualOutput.Contains($"[{Label}]  stringValue: [String] string value"), "String attribute not logged");
        }

        [TestMethod]
        public void TraceEntity_OutputsIntegerAttribute()
        {
            // ARRANGE
            var entity = Entity;
            entity["IntegerValue"] = 42;

            // ACT
            var tracer = new TestTracingService();
            tracer.TraceEntity(entity, Label);

            // ASSERT
            var actualOutput = tracer.TraceLogs;

            Assert.IsTrue(actualOutput.Contains($"[{Label}]  IntegerValue: [Int32] 42"), "Int32 attribute not logged");
        }

        [TestMethod]
        public void TraceEntity_OutputsLongIntegerAttribute()
        {
            // ARRANGE
            var entity = Entity;
            entity["IntegerValue"] = 9223372036854775807;

            // ACT
            var tracer = new TestTracingService();
            tracer.TraceEntity(entity, Label);

            // ASSERT
            var actualOutput = tracer.TraceLogs;

            Assert.IsTrue(actualOutput.Contains($"[{Label}]  IntegerValue: [Int64] 9223372036854775807"), "Int64 attribute not logged");
        }

        [TestMethod]
        public void TraceEntity_OutputsBooleanAttribute()
        {
            // ARRANGE
            var entity = Entity;

            entity["BooleanValue"] = true;

            // ACT
            var tracer = new TestTracingService();
            tracer.TraceEntity(entity, Label);

            // ASSERT
            var actualOutput = tracer.TraceLogs;

            Assert.IsTrue(actualOutput.Contains($"[{Label}]  BooleanValue: [Boolean] True"), "Boolean attribute not logged");
        }

        [TestMethod]
        public void TraceEntity_OutputsNullAttribute()
        {
            // ARRANGE
            var entity = Entity;

            entity["nullValue"] = null;

            // ACT
            var tracer = new TestTracingService();
            tracer.TraceEntity(entity, Label);

            // ASSERT
            var actualOutput = tracer.TraceLogs;

            Assert.IsTrue(actualOutput.Contains($"[{Label}]  nullValue: [null] null"), "null attribute not logged");
        }

        [TestMethod]
        public void TraceEntity_OutputsOptionSetValueAttribute()
        {
            // ARRANGE
            var entity = Entity;

            entity["OptionSetValue"] = new OptionSetValue(6009876);

            // ACT
            var tracer = new TestTracingService();
            tracer.TraceEntity(entity, Label);

            // ASSERT
            var actualOutput = tracer.TraceLogs;

            Assert.IsTrue(actualOutput.Contains($"[{Label}]  OptionSetValue: [OptionSetValue] OptionSetValue(6009876)"), "OptionSetValue attribute not logged");
        }

        [TestMethod]
        public void TraceEntity_OutputsMoneyAttribute()
        {
            // ARRANGE
            var entity = Entity;

            entity["MoneySetValue"] = new Money(9999.99m);

            // ACT
            var tracer = new TestTracingService();
            tracer.TraceEntity(entity, Label);

            // ASSERT
            var actualOutput = tracer.TraceLogs;

            Assert.IsTrue(actualOutput.Contains($"[{Label}]  MoneySetValue: [Money] Money(9999.99)"), "Money attribute not logged");
        }

        [TestMethod]
        public void TraceEntity_OutputsAliasedValueAttribute()
        {
            // ARRANGE
            var entity = Entity;

            entity["AliasedValue"] = new AliasedValue("contact", "contact", "contact");

            // ACT
            var tracer = new TestTracingService();
            tracer.TraceEntity(entity, Label);

            // ASSERT
            var actualOutput = tracer.TraceLogs;

            Assert.IsTrue(actualOutput.Contains($"[{Label}]  AliasedValue: [AliasedValue] AliasedValue(contact.contact = contact)"), "AliasedValue attribute not logged");
        }

        [TestMethod]
        public void TraceEntity_WhenEntityReferenceIsGuid_OutputsEntityReferenceAttribute()
        {
            // ARRANGE
            var entity = Entity;
            var referenceId = Guid.NewGuid();
            entity["EntityReferenceValue"] = new EntityReference("contact",referenceId) { Name="Tester"};

            // ACT
            var tracer = new TestTracingService();
            tracer.TraceEntity(entity, Label);

            // ASSERT
            var actualOutput = tracer.TraceLogs;

            Assert.IsTrue(actualOutput.Contains($"[{Label}]  EntityReferenceValue: [EntityReference] EntityReference(LogicalName=contact, Id={referenceId}, Name=Tester)"), "EntityReference attribute not logged");
        }


        [TestMethod]
        public void TraceEntity_OutputsAttributeCount()
        {
            // ARRANGE
            var entity = Entity;
            entity["stringValue"] = "string value";
            entity["IntegerValue"] = 42;
            entity["IntegerValue2"] = 9223372036854775807;
            entity["BooleanValue"] = true;
            entity["nullValue"] = null;
            entity["OptionSetValue"] = new OptionSetValue(6009876);
            entity["MoneySetValue"] = new Money(9999.99m);
            entity["AliasedValue"] = new AliasedValue("contact", "contact", "contact");
            entity["EntityReferenceValue"] = new EntityReference("contact", Guid.NewGuid()) { Name = "Tester" };

            // ACT
            var tracer = new TestTracingService();
            tracer.TraceEntity(entity, Label);

            // ASSERT
            var expectedAttributeCount = entity.Attributes.Count;
            var actualOutput = tracer.TraceLogs;
            Assert.IsTrue(actualOutput.Contains($"[{Label}] Attribute Count: {expectedAttributeCount}"), "Attribute Count not logged");
        }
    }
}