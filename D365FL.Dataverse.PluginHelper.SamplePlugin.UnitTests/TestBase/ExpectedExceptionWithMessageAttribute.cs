using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace D365FL.Dataverse.PluginHelper.SamplePlugin.UnitTests.TestBase
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class ExpectedExceptionWithMessageAttribute : ExpectedExceptionBaseAttribute
    {
        private readonly Type _exceptionType;
        private readonly string _expectedMessage;

        public ExpectedExceptionWithMessageAttribute(Type exceptionType, string expectedMessage)
            : base($"Expected exception of type '{exceptionType?.Name}' was not thrown.")
        {
            _exceptionType = exceptionType ?? throw new ArgumentNullException(nameof(exceptionType));
            _expectedMessage = expectedMessage ?? throw new ArgumentNullException(nameof(expectedMessage));
        }

        protected override void Verify(Exception exception)
        {
            // Let Assert failures inside the test body propagate — don't treat them as the expected exception.
            RethrowIfAssertException(exception);

            Assert.AreEqual(
                _exceptionType,
                exception.GetType(),
                $"Expected exception type '{_exceptionType.FullName}' but got '{exception.GetType().FullName}'.");

            Assert.IsTrue(
                string.Equals(_expectedMessage, exception.Message, StringComparison.Ordinal),
                $"Expected exception message:{Environment.NewLine}  \"{_expectedMessage}\"{Environment.NewLine}Actual:{Environment.NewLine}  \"{exception.Message}\"");
        }
    }
}
