using D365FL.Dataverse.PluginHelper.Core.EntityExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System;

namespace D365FL.Dataverse.PluginHelper.Core.UnitTests.EntityExtensions
{
    [TestClass]
    public class EntityExtensions_HasFieldChanged
    {
        private Entity CreateTestEntity(string columnName, object columnValue)
        {
            var entity = new Entity();
            entity.Attributes.Add(columnName, columnValue);
            return entity;
        }


        private bool HasChanged(object originalValue, object modifiedValue)
        {
            // ARRANGE
            var columnName = "test";

            var original = CreateTestEntity(columnName, originalValue);
            var modified = CreateTestEntity(columnName, modifiedValue);

            // ACT
            var hasChanged = original.HasFieldChanged(modified, columnName);

            return hasChanged;
        }

        #region "Missing Values Test"

        [TestMethod]
        public void HasFieldChange_ReturnsTrue_WhenColumnIsAbsentFromOriginalButPresentInModified()
        {
            var columnName = "test";
            var original = new Entity();
            var modified = new Entity();
            modified.Attributes.Add(columnName, true);

            var result = original.HasFieldChanged(modified, columnName);

            Assert.IsTrue(result, "HasFieldChanged DID NOT return true when column is absent from original but present in modified");
        }

        [TestMethod]
        public void HasFieldChange_ReturnsTrue_WhenColumnIsAbsentFromModifiedButPresentInOriginal()
        {
            var columnName = "test";
            var original = new Entity();
            var modified = new Entity();
            original.Attributes.Add(columnName, true);

            var result = original.HasFieldChanged(modified, columnName);

            Assert.IsTrue(result, "HasFieldChanged DID NOT return true when column is absent from modified but present in original");

            // TODO think about if this should be false or not
        }

        #endregion


        #region "String Field Tests"

        [TestMethod]
        public void HasFieldChange_ReturnsTrue_WhenFieldHasChanged()
        {

            var result = HasChanged("originalValue", "modifiedValue");
            
            // ASSERT
            Assert.IsTrue(result, "HasFieldChange DID NOT return true when field has changed");
        }

        [TestMethod]
        public void HasFieldChange_ReturnsTrue_WhenOriginalIsNull()
        {

            var result = HasChanged(null, "modifiedValue");

            // ASSERT
            Assert.IsTrue(result, "HasFieldChange DID NOT return true when original is null");
        }

        [TestMethod]
        public void HasFieldChange_ReturnsTrue_WhenModifiedIsNull()
        {

            var result = HasChanged("originalValue", null);

            // ASSERT
            Assert.IsTrue(result, "HasFieldChange DID NOT return true when modified is null");
        }

        [TestMethod]
        public void HasFieldChange_ReturnsFalse_WhenBothAreSameValue()
        {
            var value = "value";
            var result = HasChanged(value, value);

            // ASSERT
            Assert.IsFalse(result, "HasFieldChange DID NOT return false when both are same value");
        }

        [TestMethod]
        public void HasFieldChange_ReturnsFalse_WhenBothAreNull()
        {

            var result = HasChanged(null, null);

            // ASSERT
            Assert.IsFalse(result, "HasFieldChange DID NOT return false when both are null");
        }

        #endregion

        #region "Money Field Tests"

        [TestMethod]
        public void HasFieldChange_ReturnsTrue_WhenMoneyAndFieldHasChanged()
        {

            var result = HasChanged(new Money(50000.00m), new Money(50000.01m));

            // ASSERT
            Assert.IsTrue(result, "HasFieldChange DID NOT return true when money field has changed");
        }

        [TestMethod]
        public void HasFieldChange_ReturnsTrue_WhenMoneyAndOriginalIsNull()
        {

            var result = HasChanged(null, new Money(50000.00m));

            // ASSERT
            Assert.IsTrue(result, "HasFieldChange DID NOT return true when original is null");
        }

        [TestMethod]
        public void HasFieldChange_ReturnsTrue_WhenMoneyAndModifiedIsNull()
        {

            var result = HasChanged(new Money(50000.00m), null);

            // ASSERT
            Assert.IsTrue(result, "HasFieldChange DID NOT return true when modified is null");
        }

        [TestMethod]
        public void HasFieldChange_ReturnsFalse_WhenMoneyAndBothAreSameValue_ByReference()
        {
            var value = new Money(50000.00m);
            var result = HasChanged(value, value);

            // ASSERT
            Assert.IsFalse(result, "HasFieldChange DID NOT return false when both are same value");
        }

        [TestMethod]
        public void HasFieldChange_ReturnsFalse_WhenMoneyAndBothAreSameValue_ByValue()
        {
            var result = HasChanged(new Money(50000.00m), new Money(50000.00m));

            // ASSERT
            Assert.IsFalse(result, "HasFieldChange DID NOT return false when both are same value");
        }

        [TestMethod]
        public void HasFieldChange_ReturnsFalse_WhenMoneyAndBothAreNull()
        {

            var result = HasChanged(null, null);

            // ASSERT
            Assert.IsFalse(result, "HasFieldChange DID NOT return false when both are null");
        }

        #endregion

        #region "Entity Reference Test"

        [TestMethod]
        public void HasFieldChange_ReturnsTrue_WhenEntityReferenceAndFieldHasChanged()
        {
            var originalValue = new EntityReference("account", Guid.NewGuid());
            var newValue = new EntityReference("account", Guid.NewGuid());
            var result = HasChanged(originalValue, newValue);

            // ASSERT
            Assert.IsTrue(result, "HasFieldChange DID NOT return true when EntityReference field has changed");
        }

        [TestMethod]
        public void HasFieldChange_ReturnsFalse_WhenEntityReferenceAndBothAreSameValue()
        {
            var id = Guid.NewGuid();
            var value1 = new EntityReference("account", id);
            var value2 = new EntityReference("account", id);
            var result = HasChanged(value1, value2);

            // ASSERT
            Assert.IsFalse(result, "HasFieldChange DID NOT return false when both are same value");
        }

        [TestMethod]
        public void HasFieldChange_ReturnsFalse_WhenEntityReferenceAndBothAreSameValue_ByReference()
        {

            var value = new EntityReference("account", Guid.NewGuid());
            var result = HasChanged(value, value);

            // ASSERT
            Assert.IsFalse(result, "HasFieldChange DID NOT return false when both are same value");
        }

        // TODO test entity references with unique key vaulses

        #endregion

        #region "OptionSets Test"

        [TestMethod]
        public void HasFieldChange_ReturnsTrue_WhenOptionSetAndFieldHasChanged()
        {
            var result = HasChanged(new OptionSetValue(1), new OptionSetValue(2));

            // ASSERT
            Assert.IsTrue(result, "HasFieldChange DID NOT return true when OptionSetValue field has changed");
        }

        [TestMethod]
        public void HasFieldChange_ReturnsFalse_WhenOptionSetAndBothAreSameValue_ByReference()
        {
            var value = new OptionSetValue(1);
            var result = HasChanged(value, value);

            // ASSERT
            Assert.IsFalse(result, "HasFieldChange DID NOT return false when both are same value");
        }
        
        [TestMethod]
        public void HasFieldChange_ReturnsFalse_WhenOptionSetAndBothAreSameValue()
        {
            var value1 = new OptionSetValue(1);
            var value2 = new OptionSetValue(1);
            var result = HasChanged(value1, value2);

            // ASSERT
            Assert.IsFalse(result, "HasFieldChange DID NOT return false when both are same value");
        }

        #endregion

        #region "Boolean Field Tests"

        [TestMethod]
        public void HasFieldChange_ReturnsTrue_WhenBooleanAndFieldHasChanged()
        {
            var result = HasChanged(false, true);

            // ASSERT
            Assert.IsTrue(result, "HasFieldChanged DID NOT return true when boolean field has changed");
        }

        [TestMethod]
        public void HasFieldChange_ReturnsFalse_WhenBooleanAndBothAreSameValue()
        {
            var result = HasChanged(true, true);

            // ASSERT
            Assert.IsFalse(result, "HasFieldChanged DID NOT return false when both boolean values are the same");
        }

        #endregion

    }
}
