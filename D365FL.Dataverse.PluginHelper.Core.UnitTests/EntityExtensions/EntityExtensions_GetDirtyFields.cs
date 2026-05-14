using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System;
using D365FL.Dataverse.PluginHelper.Core.EntityExtensions;


namespace D365FL.Dataverse.PluginHelper.Core.UnitTests.EntityExtensions
{
    [TestClass]
    public class EntityExtensions_GetDirtyFields
    {
        private const string LogicalName = "account";
        private readonly Guid _entityId = Guid.NewGuid();

        private Entity CreateEntity(string logicalName, Guid id)
        {
            return new Entity(logicalName) { Id = id };
        }

        #region "Null and Empty Tests"

        [TestMethod]
        public void GetDirtyFields_ReturnsEmptyEntity_WhenNoFieldsHaveChanged()
        {
            // ARRANGE
            var original = CreateEntity(LogicalName, _entityId);
            var modified = CreateEntity(LogicalName, _entityId);
            original["name"] = "Contoso";
            modified["name"] = "Contoso";

            // ACT
            var result = original.GetDirtyFields(modified);

            // ASSERT
            Assert.AreEqual(0, result.Attributes.Count, "GetDirtyFields DID NOT return empty entity when no fields changed");
        }

        [TestMethod]
        public void GetDirtyFields_ReturnsEntityWithCorrectIdAndLogicalName()
        {
            // ARRANGE
            var original = CreateEntity(LogicalName, _entityId);
            var modified = CreateEntity(LogicalName, _entityId);

            // ACT
            var result = original.GetDirtyFields(modified);

            // ASSERT
            Assert.AreEqual(LogicalName, result.LogicalName, "GetDirtyFields DID NOT return entity with correct LogicalName");
            Assert.AreEqual(_entityId, result.Id, "GetDirtyFields DID NOT return entity with correct Id");
        }

        #endregion

        #region "String Field Tests"

        [TestMethod]
        public void GetDirtyFields_ReturnsChangedField_WhenStringFieldHasChanged()
        {
            // ARRANGE
            var original = CreateEntity(LogicalName, _entityId);
            var modified = CreateEntity(LogicalName, _entityId);
            original["name"] = "Contoso";
            modified["name"] = "Fabrikam";

            // ACT
            var result = original.GetDirtyFields(modified);

            // ASSERT
            Assert.IsTrue(result.Contains("name"), "GetDirtyFields DID NOT include changed string field");
            Assert.AreEqual("Fabrikam", result["name"], "GetDirtyFields DID NOT return correct new value");
        }

        [TestMethod]
        public void GetDirtyFields_DoesNotReturnField_WhenStringFieldHasNotChanged()
        {
            // ARRANGE
            var original = CreateEntity(LogicalName, _entityId);
            var modified = CreateEntity(LogicalName, _entityId);
            original["name"] = "Contoso";
            modified["name"] = "Contoso";

            // ACT
            var result = original.GetDirtyFields(modified);

            // ASSERT
            Assert.IsFalse(result.Contains("name"), "GetDirtyFields DID include unchanged string field");
            Assert.AreEqual(0, result.Attributes.Count, "GetDirtyFields DID NOT return empty entity when no fields changed");

        }

        [TestMethod]
        public void GetDirtyFields_ReturnsChangedField_WhenStringChangedToNull()
        {
            // ARRANGE
            var original = CreateEntity(LogicalName, _entityId);
            var modified = CreateEntity(LogicalName, _entityId);
            original["name"] = "Contoso";
            modified["name"] = null;

            // ACT
            var result = original.GetDirtyFields(modified);

            // ASSERT
            Assert.IsTrue(result.Contains("name"), "GetDirtyFields DID NOT include field changed to null");
            Assert.AreEqual(null, result["name"], "GetDirtyFields DID NOT return correct new value");
        }

        [TestMethod]
        public void GetDirtyFields_ReturnsChangedField_WhenStringChangedFromNull()
        {
            // ARRANGE
            var original = CreateEntity(LogicalName, _entityId);
            var modified = CreateEntity(LogicalName, _entityId);
            original["name"] = null;
            modified["name"] = "Contoso";

            // ACT
            var result = original.GetDirtyFields(modified);

            // ASSERT
            Assert.IsTrue(result.Contains("name"), "GetDirtyFields DID NOT include field changed from null");
            Assert.AreEqual("Contoso", result["name"], "GetDirtyFields DID NOT return correct new value");
        }

        #endregion

        #region "Multiple Fields Tests"

        [TestMethod]
        public void GetDirtyFields_ReturnsOnlyChangedFields_WhenSomeFieldsHaveChanged()
        {
            // ARRANGE
            var original = CreateEntity(LogicalName, _entityId);
            var modified = CreateEntity(LogicalName, _entityId);
            original["name"] = "Contoso";
            original["telephone1"] = "12345";
            original["cost"] = new Money(99.99m);
            modified["name"] = "Fabrikam";
            modified["telephone1"] = "12345";
            modified["cost"] = new Money(99.99m);

            // ACT
            var result = original.GetDirtyFields(modified);

            // ASSERT
            Assert.AreEqual(1, result.Attributes.Count, "GetDirtyFields DID NOT return only changed fields");
            Assert.IsTrue(result.Contains("name"), "GetDirtyFields DID NOT include changed field");
            Assert.IsFalse(result.Contains("telephone1"), "GetDirtyFields DID include unchanged field");
            Assert.IsFalse(result.Contains("cost"), "GetDirtyFields DID include unchanged field");
        }

        [TestMethod]
        public void GetDirtyFields_ReturnsAllFields_WhenAllFieldsHaveChanged()
        {
            // ARRANGE
            var original = CreateEntity(LogicalName, _entityId);
            var modified = CreateEntity(LogicalName, _entityId);
            original["name"] = "Contoso";
            original["telephone1"] = "12345";
            original["cost"] = new Money(88.88m);
            modified["name"] = "Fabrikam";
            modified["telephone1"] = "99999";
            modified["cost"] = new Money(99.99m);
            // ACT
            var result = original.GetDirtyFields(modified);

            // ASSERT
            Assert.AreEqual(3, result.Attributes.Count, "GetDirtyFields DID NOT return all changed fields");
            Assert.AreEqual("Fabrikam", result["name"], "GetDirtyFields DID NOT return correct new value");
            Assert.AreEqual("99999", result["telephone1"], "GetDirtyFields DID NOT return correct new value");
            Assert.AreEqual(new Money(99.99m), result["cost"], "GetDirtyFields DID NOT return correct new value");
        }

        #endregion

        #region "Dataverse Type Tests"

        [TestMethod]
        public void GetDirtyFields_ReturnsChangedField_WhenMoneyFieldHasChanged()
        {
            // ARRANGE
            var original = CreateEntity(LogicalName, _entityId);
            var modified = CreateEntity(LogicalName, _entityId);
            original["revenue"] = new Money(1000.00m);
            modified["revenue"] = new Money(2000.00m);

            // ACT
            var result = original.GetDirtyFields(modified);

            // ASSERT
            Assert.IsTrue(result.Contains("revenue"), "GetDirtyFields DID NOT include changed Money field");
            Assert.AreEqual(new Money(2000.00m), result["revenue"],
                "GetDirtyFields DID NOT include changed Money field");
        }

        [TestMethod]
        public void GetDirtyFields_ReturnsChangedField_WhenOptionSetFieldHasChanged()
        {
            // ARRANGE
            var original = CreateEntity(LogicalName, _entityId);
            var modified = CreateEntity(LogicalName, _entityId);
            original["statuscode"] = new OptionSetValue(1);
            modified["statuscode"] = new OptionSetValue(2);

            // ACT
            var result = original.GetDirtyFields(modified);

            // ASSERT
            Assert.IsTrue(result.Contains("statuscode"), "GetDirtyFields DID NOT include changed OptionSetValue field");
            Assert.AreEqual(new OptionSetValue(2), result["statuscode"],
                "GetDirtyFields DID NOT include changed OptionSetValue field");
        }

        [TestMethod]
        public void GetDirtyFields_ReturnsChangedField_WhenOptionSetFieldChangedToNull()
        {
            // ARRANGE
            var original = CreateEntity(LogicalName, _entityId);
            var modified = CreateEntity(LogicalName, _entityId);
            original["statuscode"] = new OptionSetValue(1);
            modified["statuscode"] = null;

            // ACT
            var result = original.GetDirtyFields(modified);

            // ASSERT
            Assert.IsTrue(result.Contains("statuscode"), "GetDirtyFields DID NOT include changed OptionSetValue field");
            Assert.AreEqual(null, result["statuscode"],
                "GetDirtyFields DID NOT include changed OptionSetValue field");
        }
        [TestMethod]
        public void GetDirtyFields_ReturnsChangedField_WhenEntityReferenceHasChanged()
        {
            // ARRANGE
            var modifiedId = Guid.NewGuid();
            var original = CreateEntity(LogicalName, _entityId);
            var modified = CreateEntity(LogicalName, _entityId);
            original["primarycontactid"] = new EntityReference("contact", Guid.NewGuid());
            modified["primarycontactid"] = new EntityReference("contact", modifiedId);

            // ACT
            var result = original.GetDirtyFields(modified);

            // ASSERT
            Assert.IsTrue(result.Contains("primarycontactid"), "GetDirtyFields DID NOT include changed EntityReference field");
            Assert.AreEqual(new EntityReference("contact", modifiedId), result["primarycontactid"],
                "GetDirtyFields DID NOT include changed EntityReference field");
        }

        [TestMethod]
        public void GetDirtyFields_ReturnsChangedField_WhenEntityReferenceChangedFromNull()
        {
            // ARRANGE
            var modifiedId = Guid.NewGuid();
            var original = CreateEntity(LogicalName, _entityId);
            var modified = CreateEntity(LogicalName, _entityId);
            original["primarycontactid"] = null;
            modified["primarycontactid"] = new EntityReference("contact", modifiedId);

            // ACT
            var result = original.GetDirtyFields(modified);

            // ASSERT
            Assert.IsTrue(result.Contains("primarycontactid"), "GetDirtyFields DID NOT include changed EntityReference field");
            Assert.AreEqual(new EntityReference("contact", modifiedId), result["primarycontactid"],
                "GetDirtyFields DID NOT include changed EntityReference field");
        }

        #endregion

        #region "Missing Field Tests"

        [TestMethod]
        public void GetDirtyFields_ReturnsField_WhenFieldAbsentFromOriginal()
        {
            // ARRANGE
            var original = CreateEntity(LogicalName, _entityId);
            var modified = CreateEntity(LogicalName, _entityId);
            modified["name"] = "Contoso";

            // ACT
            var result = original.GetDirtyFields(modified);

            // ASSERT
            Assert.IsTrue(result.Contains("name"), "GetDirtyFields DID NOT include field absent from original");
            Assert.AreEqual("Contoso", result["name"], "GetDirtyFields DID NOT include field absent from original");
        }

        [TestMethod]
        public void GetDirtyFields_DoesNotReturnField_WhenFieldAbsentFromModified()
        {
            // ARRANGE
            var original = CreateEntity(LogicalName, _entityId);
            var modified = CreateEntity(LogicalName, _entityId);
            original["name"] = "Contoso";

            // ACT
            var result = original.GetDirtyFields(modified);

            // ASSERT
            Assert.IsFalse(result.Contains("name"), "GetDirtyFields DID include field absent from modified — absent fields should not be treated as changed");
        }

        #endregion
    }
}