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

### Key Features

- ✅ **Clean Code Architecture** - Separation of concerns and maintainable plugin structure
- ✅ **Dirty Field Tracking** - Trigger logic only when required fields are modified
- ✅ **Delta Updates** - Save only modified fields back to Dataverse to prevent cascading plugin executions
- ✅ **Prevent Max Depth Issues** - Intelligent change detection reduces plugin recursion
- ✅ **Best Practices Enforcement** - Built-in patterns for secure, efficient plugin development
- ✅ **Fast Development** - Boilerplate reduction and reusable components accelerate plugin creation
- ✅ **Easy to Read** - Intuitive API design for improved code clarity and maintainability
- ✅ **Testable Design** - Framework patterns support both unit testing and integration testing
- ✅ **Plugin Testing Guidance** - Examples of plugin unit tests and integration tests included

## Why D365FL.Dataverse.PluginHelper?

### The Problem

Microsoft Dataverse plugins are powerful but can quickly become complex and problematic:

- **Plugin Depth Exceeded Errors** - Recursive plugin executions trigger max depth limits (16 levels)
- **Unnecessary Data Operations** - Saving all fields causes downstream plugins to fire repeatedly
- **Messy Code** - Plugin business logic becomes tangled without proper structure
- **Performance Issues** - Inefficient field handling and redundant operations slow down processes
- **Maintenance Burden** - Hard-to-read code increases bugs and development time
- **Testing Difficulty** - Plugins are hard to test without a proper framework structure

### The Solution

D365FL.Dataverse.PluginHelper provides a structured framework that addresses these challenges:

```csharp
// Only trigger when specific fields change
if (entity.IsDirty("fieldname"))
{
    // Your business logic here
}

// Only save modified fields back to Dataverse
entity.UpdateDelta(service);
```

This approach:
- Prevents unnecessary plugin executions
- Reduces plugin depth issues automatically
- Keeps code clean and readable
- Improves overall system performance
- Enables easy testing and validation

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

```bash
dotnet add package D365FL.Dataverse.PluginHelper
```

Or via Package Manager:

```
Install-Package D365FL.Dataverse.PluginHelper
```

### Clone the Repository (for samples and learning)

```bash
git clone https://github.com/D365Freelancing/D365FL.Dataverse.PluginHelper.git
cd D365FL.Dataverse.PluginHelper
```

## Quick Start

### Basic Plugin Setup

```csharp
using D365FL.Dataverse.PluginHelper;

public class MyDataversePlugin : IPlugin
{
    public void Execute(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetService(typeof(IPluginExecutionContext)) as IPluginExecutionContext;
        var service = serviceProvider.GetService(typeof(IOrganizationService)) as IOrganizationService;
        
        // Get the target entity from the context
        var entity = context.InputParameters["Target"] as Entity;
        
        // Check if specific field is dirty (modified)
        if (entity.IsDirty("new_fieldname"))
        {
            // Execute logic only when field changes
            ProcessFieldChange(entity, service);
        }
        
        // Update only modified fields back to Dataverse
        entity.UpdateDelta(service);
    }
    
    private void ProcessFieldChange(Entity entity, IOrganizationService service)
    {
        // Your business logic here
    }
}
```

### Dirty Field Tracking

Track which fields have been modified without manual comparison:

```csharp
// Check if a single field is dirty
if (entity.IsDirty("new_fieldname"))
{
    // Field was modified - execute logic
}

// Check if any field in a list is dirty
if (entity.IsDirty(new[] { "field1", "field2", "field3" }))
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
entity["new_fieldname"] = "new value";

// Update only the modified fields back to Dataverse
entity.UpdateDelta(service);

// This prevents downstream plugins from being triggered by unchanged fields
```

## Testing Plugins

### Unit Testing Example

The framework is designed for easy unit testing. Here's a sample unit test:

```csharp
[TestClass]
public class AccountPluginTests
{
    [TestMethod]
    public void WhenAccountNameChanges_ShouldUpdateDescription()
    {
        // Arrange
        var account = new Entity("account", Guid.NewGuid());
        account["name"] = "Original Name";
        
        var updatedAccount = new Entity("account", account.Id);
        updatedAccount["name"] = "Updated Name";
        
        var mockService = new Mock<IOrganizationService>();
        
        // Act
        var plugin = new AccountPlugin();
        var isDirty = updatedAccount.IsDirty("name");
        
        // Assert
        Assert.IsTrue(isDirty);
    }
    
    [TestMethod]
    public void WhenNonCriticalFieldChanges_ShouldNotExecuteLogic()
    {
        // Arrange
        var account = new Entity("account");
        account["name"] = "Test Account";
        account["description"] = "Old Description";
        
        var updatedAccount = new Entity("account");
        updatedAccount["name"] = "Test Account";
        updatedAccount["description"] = "New Description";
        
        // Act
        var nameIsDirty = updatedAccount.IsDirty("name");
        
        // Assert
        Assert.IsFalse(nameIsDirty); // Only description changed, name is clean
    }
}
```

### Integration Testing Example

Test your plugins against a real or test Dataverse environment:

```csharp
[TestClass]
public class AccountPluginIntegrationTests
{
    private IOrganizationService _service;
    
    [TestInitialize]
    public void Setup()
    {
        // Connect to your test Dataverse environment
        _service = new CrmServiceClient("Url=https://yourorg.crm.dynamics.com/; AuthType=OAuth; ...");
    }
    
    [TestMethod]
    public void WhenCreatingAccount_ShouldExecutePlugin()
    {
        // Arrange
        var account = new Entity("account");
        account["name"] = "Integration Test Account";
        account["new_customfield"] = "Test Value";
        
        // Act
        var accountId = _service.Create(account);
        var createdAccount = _service.Retrieve("account", accountId, new ColumnSet(true));
        
        // Assert
        Assert.IsNotNull(createdAccount);
        Assert.AreEqual("Integration Test Account", createdAccount["name"]);
    }
}
```

## Best Practices

This framework encourages and enables these Dataverse plugin best practices:

### 1. Conditional Logic Based on Field Changes

Always check if a field is dirty before executing related logic:

```csharp
if (entity.IsDirty("statuscode"))
{
    UpdateRelatedRecords(entity, service);
}
```

### 2. Minimize Plugin Depth

Reduce cascading plugin executions by updating only necessary fields:

```csharp
// Instead of updating the entire entity
entity.Attributes.Clear(); // Avoid this!

// Update only what changed
entity.UpdateDelta(service);
```

### 3. Separate Concerns

Use a layered architecture for complex business logic:

```csharp
public class MyPlugin : IPlugin
{
    public void Execute(IServiceProvider serviceProvider)
    {
        var handler = new PluginHandler(serviceProvider);
        handler.Execute();
    }
}

public class PluginHandler
{
    private IPluginExecutionContext _context;
    private IOrganizationService _service;
    
    // Separate business logic here
}
```

### 4. Efficient Data Retrieval

Only query and modify the fields you need:

```csharp
// Retrieve only necessary columns
var query = new QueryExpression("account")
{
    ColumnSet = new ColumnSet("name", "new_customfield")
};

var results = service.RetrieveMultiple(query);
```

### 5. Make Plugins Testable

Design plugins with dependency injection for testability:

```csharp
public class MyPlugin : IPlugin
{
    private readonly IPluginHandler _handler;
    
    public MyPlugin() : this(new PluginHandler()) { }
    
    public MyPlugin(IPluginHandler handler)
    {
        _handler = handler;
    }
    
    public void Execute(IServiceProvider serviceProvider)
    {
        _handler.Execute(serviceProvider);
    }
}
```

## Documentation

For detailed documentation and advanced usage scenarios, see:

- [API Reference](docs/API.md) - Complete framework API documentation
- [Best Practices Guide](docs/BEST_PRACTICES.md) - Dataverse plugin development best practices
- [Testing Guide](docs/TESTING_GUIDE.md) - Comprehensive plugin unit testing and integration testing guide
- [Troubleshooting](docs/TROUBLESHOOTING.md) - Common issues and solutions
- [Sample Plugins](samples/D365FL.Samples.Plugins/README.md) - Real-world plugin examples
- [Unit Testing Samples](samples/D365FL.Samples.UnitTests/README.md) - Unit test examples and patterns
- [Integration Testing Samples](samples/D365FL.Samples.IntegrationTests/README.md) - Integration test examples and setup

## System Requirements

- **.NET Framework**: 4.6.2 or higher (for on-premises) / .NET 6.0+ (for cloud)
- **Dataverse**: All versions (Power Apps, Dynamics 365, and on-premises)
- **CRM SDK**: 9.0 or higher
- **Testing**: xUnit, NUnit, or MSTest (samples use MSTest)

## Contributing

Contributions are welcome! Please read our [Contributing Guidelines](CONTRIBUTING.md) before submitting pull requests.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Support

- 📖 **Documentation**: Check the docs folder for comprehensive guides
- 📚 **Samples**: Browse sample plugins and tests in the `samples/` directory
- 🐛 **Issues**: Report bugs via [GitHub Issues](../../issues)
- 💬 **Discussions**: Ask questions in [GitHub Discussions](../../discussions)
- 📧 **Email**: Contact the maintainer for support

## Roadmap

- [ ] Support for table-driven configuration
- [ ] Performance analytics integration
- [ ] Enhanced debugging tools
- [ ] Plugin template generation
- [ ] Async plugin support documentation
- [ ] FakeXrmEasy integration samples
- [ ] Mocking library recommendations and examples

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
