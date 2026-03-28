using D365FL.Dataverse.PluginHelper.Core.EntityExtensions;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;

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
        public void HasFieldChange_ReturnsTrue_WhenMoneyFieldHasChanged()
        {

            var result = HasChanged(new Money(50000.00m), new Money(50000.01m));

            // ASSERT
            Assert.IsTrue(result, "HasFieldChange DID NOT return true when money field has changed");
        }

        [TestMethod]
        public void HasFieldChange_ReturnsTrue_WhenMoneyOriginalIsNull()
        {

            var result = HasChanged(null, new Money(50000.00m));

            // ASSERT
            Assert.IsTrue(result, "HasFieldChange DID NOT return true when original is null");
        }

        [TestMethod]
        public void HasFieldChange_ReturnsTrue_WhenMoneyModifiedIsNull()
        {

            var result = HasChanged(new Money(50000.00m), null);

            // ASSERT
            Assert.IsTrue(result, "HasFieldChange DID NOT return true when modified is null");
        }

        [TestMethod]
        public void HasFieldChange_ReturnsFalse_WhenMoneyBothAreSameValue()
        {
            var value = new Money(50000.00m);
            var result = HasChanged(value, value);

            // ASSERT
            Assert.IsFalse(result, "HasFieldChange DID NOT return false when both are same value");
        }

        [TestMethod]
        public void HasFieldChange_ReturnsFalse_WhenMoneyBothAreNull()
        {

            var result = HasChanged(null, null);

            // ASSERT
            Assert.IsFalse(result, "HasFieldChange DID NOT return false when both are null");
        }

        #endregion




    }
}
