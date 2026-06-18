using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace D365FL.Dataverse.PluginHelper.SamplePlugin.UnitTests.TestBase
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class ExpectedInnerExceptionWithMessageAttribute : ExpectedExceptionBaseAttribute
    {
        private readonly Type _exceptionType;
        private readonly Type _innerExceptionType;
        private readonly string _expectedInnerMessage;

        public ExpectedInnerExceptionWithMessageAttribute(Type exceptionType, Type innerExceptionType, string expectedInnerMessage)
            : base($"Expected exception of type '{exceptionType?.Name}' with inner '{innerExceptionType?.Name}' was not thrown.")
        {
            _exceptionType = exceptionType ?? throw new ArgumentNullException(nameof(exceptionType));
            _innerExceptionType = innerExceptionType ?? throw new ArgumentNullException(nameof(innerExceptionType));
            _expectedInnerMessage = expectedInnerMessage ?? throw new ArgumentNullException(nameof(expectedInnerMessage));
        }

        protected override void Verify(Exception exception)
        {
            RethrowIfAssertException(exception);

            Assert.AreEqual(
                _exceptionType,
                exception.GetType(),
                $"Expected outer exception '{_exceptionType.FullName}' but got '{exception.GetType().FullName}'.");

            Assert.IsNotNull(
                exception.InnerException,
                $"Expected inner exception of type '{_innerExceptionType.FullName}' but InnerException was null.");

            Assert.AreEqual(
                _innerExceptionType,
                exception.InnerException.GetType(),
                $"Expected inner exception '{_innerExceptionType.FullName}' but got '{exception.InnerException.GetType().FullName}'.");

            Assert.IsTrue(
                string.Equals(_expectedInnerMessage, exception.InnerException.Message, StringComparison.Ordinal),
                $"Expected inner exception message:{Environment.NewLine}  \"{_expectedInnerMessage}\"{Environment.NewLine}Actual:{Environment.NewLine}  \"{exception.InnerException.Message}\"");
        }
    }
}
