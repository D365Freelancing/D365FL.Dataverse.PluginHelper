# D365FL.Dataverse.PluginHelper

> A powerful C# framework for building clean, maintainable, and high-performance Microsoft Dataverse plugins with best practices built-in. Includes comprehensive samples and demonstrations of plugin unit testing and integration testing.

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![Language: C#](https://img.shields.io/badge/Language-C%23-239120)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Platform: Dataverse](https://img.shields.io/badge/Platform-Microsoft%20Dataverse-0078D4)](https://docs.microsoft.com/en-us/power-apps/developer/data-platform/)

## Overview

**D365FL.Dataverse.PluginHelper** is a comprehensive C# framework designed to simplify Dataverse plugin development while enforcing industry best practices. It helps developers write cleaner, more maintainable, and performant plugin code that reduces plugin depth issues and minimizes unnecessary data operations.

This repository includes:
- 📦 **Framework** - Reusable helper classes and utilities for plugin development
- 📚 **Sample Plugins** - Real-world examples demonstrating framework usage
- ✅ **Unit Testing Samples** - Demonstrates how to unit test plugins in isolation
- 🔗 **Integration Testing Samples** - Shows integration testing against Dataverse

## Why D365FL.Dataverse.PluginHelper?

- ✅ **Clean Code Architecture** - Encourages separation of concerns, making plugins **easier to read, maintain, and test**.
- ✅ **Dirty Field Tracking** - Provides granular control to trigger business logic only when required fields are modified, ensuring expensive operations are not executed unnecessarily.
- ✅ **Delta Updates** - Ensures modified fields are saved back to Dataverse preventing accidental cascade firing of additional plugins.
- ✅ **Prevent Max Depth Issues** - **Dirty Field Tracking** and **Delta Updates** prevent Max Depth Issues — *a cascading chain reaction where plugins trigger other plugins* - creating expensive and time-consuming problems that are difficult to debug.
- ✅ **Best Practice Exception Management** - Built-in exception handling and logging ensure plugins do not fail silently, providing visibility for faster debugging and preventing costly production issues.
- ✅ **Unit Testing Examples** - Extensive FakeXrmEasy samples demonstrating plugin unit testing with rapid feedback loops, ensuring defects are caught before deployment and reducing development cycles.
- ✅ **Integration Testing Examples** - Integration test samples demonstrating end-to-end plugin validation in live Dataverse environments, ensuring defects are caught before production and providing developer confidence in plugin reliability

## Project Structure

```
D365FL.Dataverse.PluginHelper/
├── src/
│   └── D365FL.Dataverse.PluginHelper/
│       ├── EntityExtensions.cs          # Core framework - dirty field tracking
│       ├── ServiceExtensions.cs          # Framework helpers for services
│       ├── PluginBase.cs                # Base class for plugins
│       └── ... (other framework utilities)
├── samples/
│   ├── D365FL.Samples.Plugins/
│   │   ├── Examples/
│   │   │   ├── AccountPlugin.cs         # Sample plugin using framework
│   │   │   ├── ContactPlugin.cs         # Another sample implementation
│   │   │   └── OrderPlugin.cs           # Multi-entity example
│   │   └── README.md                    # Sample plugin documentation
│   ├── D365FL.Samples.UnitTests/
│   │   ├── EntityExtensionsTests.cs     # Unit tests for dirty field tracking
│   │   ├── AccountPluginTests.cs        # Plugin unit test examples
│   │   ├── ContactPluginTests.cs        # Testing with mocks
│   │   └── README.md                    # Unit testing guide
│   └── D365FL.Samples.IntegrationTests/
│       ├── AccountPluginIntegrationTests.cs  # Live Dataverse tests
│       ├── ContactPluginIntegrationTests.cs  # End-to-end testing
│       └── README.md                         # Integration testing guide
├── docs/
│   ├── API.md                           # Framework API reference
│   ├── BEST_PRACTICES.md                # Best practices guide
│   ├── TESTING_GUIDE.md                 # Comprehensive testing documentation
│   └── TROUBLESHOOTING.md               # Common issues and solutions
└── README.md                            # This file
```

## Installation

### NuGet Package

    - Comming Soon. Watch this space!

Or via Package Manager:

    - Comming Soon. Watch this space!

### Clone the Repository (for samples and learning)

```bash
git clone https://github.com/D365Freelancing/D365FL.Dataverse.PluginHelper.git
cd D365FL.Dataverse.PluginHelper
```

## Quick Start

### Basic Plugin Setup

```csharp
using D365FL.Dataverse.PluginHelper.Core.EntityExtensions;
using D365FL.Dataverse.PluginHelper.Core.PluginExecutionContextExtensions;
using D365FL.Dataverse.PluginHelper.Core.Rules;

namespace D365FL.Dataverse.PluginHelper.SamplePlugin.Plugins
{
    public class MyDataversePlugin : D365FLPluginBase
    {
        public MyDataversePlugin() : base(typeof(MyDataversePlugin))
        {
        }

        protected override bool ValidateConfig()
        {
            var rules = new RuleFactory(Context, Tracer);
            rules.AddIsPreOperationRule()
                .AddIsSynchronousRule()
                .AddHasTargetEntityRule()
                .AddTargetEntityLogicalNameRule("myentity")
                .AddIsUpdateMessageRule()
                .AddDoesNotExceedMaxDepthRule(3)
                .AddHasPreImageRule();

            return rules.IsValid;
        }

        protected override void Execute()
        {
            var target = base.Context.GetTargetEntity();
            var preImage = Context.GetPreImage(Tracer);

            // Merge preImage and target entity to ensure logic does not fail because of missing field values
            // its not used in this sample, but is included for demonstration purposes.
            var fullEntity = preImage.Merge(target, base.Tracer);

            // if required fields are dirty
            if (preImage.IsDirty(fullEntity, new[] { "field1", "field2", "field3" }))
            {
                base.Tracer.Trace("execute custom logic");
                // Then execute business logic
                // ... and perform operation on the target entity

                target["field1"] = "updated value";
                target["field2"] = "updated value";
                target["field3"] = "updated value";

                // Get changed fields as an entity
                var deltas = target.GetDirtyFields(fullEntity);
                
                // Save Changes
                InitiatingUserService.Update(deltas);
            }
        }
    }
}
```

### Dirty Field Tracking

Track which fields have been modified without manual comparison:

```csharp
// Check if a single field is dirty
if (preImage.IsDirty(target, "field1"))
{
    // Field was modified - execute logic
}

// Check if any field in a list is dirty
if (preImage.IsDirty(target, new[] { "field1", "field2", "field3" }))
{
    // One or more fields were modified
}

// Get all dirty fields
var dirtyFields = entity.GetDirtyFields();
```

### Delta Updates

Save only modified fields to prevent cascading plugin executions:

```csharp
// Modify entity
entity["field1"] = "new value";

// Update only the modified fields back to Dataverse
// This prevents downstream plugins from being triggered by saving unchanged fields

var deltas = entity.GetDirtyFields(fullEntity);
orgService.Update(delta);

```

## Testing Plugins

### Unit Testing Example

    - Comming soon. Watch this space!

### Integration Testing Example

    - Comming soon. Watch this space!

## Documentation

For detailed documentation and advanced usage scenarios, see:

    - NOTE Below documentation and more will be created soon. Watch this space!

- [API Reference](docs/API.md) - Complete framework API documentation
- [Best Practices Guide](docs/BEST_PRACTICES.md) - Dataverse plugin development best practices
- [Testing Guide](docs/TESTING_GUIDE.md) - Comprehensive plugin unit testing and integration testing guide
- [Troubleshooting](docs/TROUBLESHOOTING.md) - Common issues and solutions
- [Sample Plugins](samples/D365FL.Samples.Plugins/README.md) - Real-world plugin examples
- [Unit Testing Samples](samples/D365FL.Samples.UnitTests/README.md) - Unit test examples and patterns
- [Integration Testing Samples](samples/D365FL.Samples.IntegrationTests/README.md) - Integration test examples and setup

## System Requirements

- **.NET Framework**: 4.6.2 or higher (for on-premises)
- **Dataverse**: All versions (Power Apps, Dynamics 365, and on-premises)
- **CRM SDK**: 9.0 or higher
- **Testing**: xUnit, NUnit, or MSTest (samples use MSTest)

## Contributing

Contributions are welcome! To maintain code quality and consistency:

- Submit a Pull Request with a clear description of changes
- Include unit tests for all new functionality
- Ensure your code follows C# best practices and conventions
- Verify all tests pass (unit and integration)

**Before starting major work**, please open an [Issue](../../issues) to discuss your idea. This passion project is maintained with high standards for code quality, and we want to ensure your contribution aligns with the framework's direction.

## Requesting a Feature

Have an idea to improve D365FL.Dataverse.PluginHelper? We'd love to hear it!

### Before You Submit

1. **Check existing issues** - Search [GitHub Issues](../../issues) to see if your feature has already been requested
2. **Consider the scope** - Does this feature align with the framework's goal of making Dataverse plugins cleaner and more maintainable?
3. **Think about use cases** - How would this feature help developers? What problems does it solve?

### Submitting a Feature Request

Please open a [GitHub Issue](../../issues) with the following information:

- **Title** - Clear, concise description of the feature
- **Problem Statement** - What problem does this solve? Include real-world examples
- **Proposed Solution** - How should this feature work? Include code examples if possible
- **Alternatives Considered** - Are there other ways to solve this problem?
- **Additional Context** - Any other relevant information (links, screenshots, etc.)

### Example Feature Request

**Title:** Add retry logic helper for transient Dataverse errors

**Problem:** When plugins make external API calls, transient network errors can cause plugin failures. Currently, developers must implement retry logic manually.

**Proposed Solution:** Add a `RetryHelper` class that handles exponential backoff and transient error detection.

**Example Code:**
```csharp
var result = RetryHelper.Execute(
    action: () => externalService.CallApi(),
    maxAttempts: 3,
    delayMs: 1000
);
```

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Support

- 📖 **Documentation**: Check the docs folder for comprehensive guides
- 📚 **Samples**: Browse sample plugins and tests in the `samples/` directory
- 🐛 **Issues**: Report bugs via [GitHub Issues](../../issues)
- 💬 **Discussions**: Ask questions in [GitHub Discussions](../../discussions)
- 📧 **Email**: Contact the maintainer for support [consulting@d365freelancing.com](consulting@d365freelancing.com)


## Related Resources

- [Microsoft Dataverse Developer Documentation](https://docs.microsoft.com/en-us/power-apps/developer/data-platform/)
- [Plugin Development Best Practices](https://docs.microsoft.com/en-us/power-apps/developer/data-platform/plug-ins)
- [Understanding Plugin Depth](https://docs.microsoft.com/en-us/power-apps/developer/data-platform/understand-plug-in-execution)
- [Dynamics 365 Plugin Development Guide](https://docs.microsoft.com/en-us/dynamics365/customer-engagement/developer/plugins-overview)
- [FakeXrmEasy](https://github.com/jordimontana82/fake-xrm-easy) - Testing framework for Dataverse plugins

## Changelog

See [CHANGELOG.md](CHANGELOG.md) for version history and updates.

---

**Made with ❤️ for Dataverse developers**

*D365FL.Dataverse.PluginHelper - Framework, Samples & Testing Guide for Clean Dataverse Plugins*
