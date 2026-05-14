# D365FL.Dataverse.PluginHelper

> A powerful C# framework for building clean, maintainable, and high-performance Microsoft Dataverse plugins with best practices built-in.

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![Language: C#](https://img.shields.io/badge/Language-C%23-239120)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Platform: Dataverse](https://img.shields.io/badge/Platform-Microsoft%20Dataverse-0078D4)](https://docs.microsoft.com/en-us/power-apps/developer/data-platform/)

## Overview

**D365FL.Dataverse.PluginHelper** is a comprehensive framework designed to simplify Dataverse plugin development while enforcing industry best practices. It helps developers write cleaner, more maintainable, and performant plugin code that reduces plugin depth issues and minimizes unnecessary data operations.

### Key Features

- ✅ **Clean Code Architecture** - Separation of concerns and maintainable plugin structure
- ✅ **Dirty Field Tracking** - Trigger logic only when required fields are modified
- ✅ **Delta Updates** - Save only modified fields back to Dataverse to prevent cascading plugin executions
- ✅ **Prevent Max Depth Issues** - Intelligent change detection reduces plugin recursion
- ✅ **Best Practices Enforcement** - Built-in patterns for secure, efficient plugin development
- ✅ **Fast Development** - Boilerplate reduction and reusable components accelerate plugin creation
- ✅ **Easy to Read** - Intuitive API design for improved code clarity and maintainability

## Why D365FL.Dataverse.PluginHelper?

### The Problem

Microsoft Dataverse plugins are powerful but can quickly become complex and problematic:

- **Plugin Depth Exceeded Errors** - Recursive plugin executions trigger max depth limits (16 levels)
- **Unnecessary Data Operations** - Saving all fields causes downstream plugins to fire repeatedly
- **Messy Code** - Plugin business logic becomes tangled without proper structure
- **Performance Issues** - Inefficient field handling and redundant operations slow down processes
- **Maintenance Burden** - Hard-to-read code increases bugs and development time

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

## Installation

### NuGet Package

```bash
dotnet add package D365FL.Dataverse.PluginHelper
```

Or via Package Manager:

```
Install-Package D365FL.Dataverse.PluginHelper
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

## Documentation

For detailed documentation and advanced usage scenarios, see:

- [API Reference](docs/API.md)
- [Configuration Guide](docs/CONFIGURATION.md)
- [Best Practices Guide](docs/BEST_PRACTICES.md)
- [Troubleshooting](docs/TROUBLESHOOTING.md)

## System Requirements

- **.NET Framework**: 4.6.2 or higher (for on-premises) / .NET 6.0+ (for cloud)
- **Dataverse**: All versions (Power Apps, Dynamics 365, and on-premises)
- **CRM SDK**: 9.0 or higher

## Contributing

Contributions are welcome! Please read our [Contributing Guidelines](CONTRIBUTING.md) before submitting pull requests.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Support

- 📖 **Documentation**: Check the docs folder for comprehensive guides
- 🐛 **Issues**: Report bugs via [GitHub Issues](../../issues)
- 💬 **Discussions**: Ask questions in [GitHub Discussions](../../discussions)
- 📧 **Email**: Contact the maintainer for support

## Roadmap

- [ ] Support for table-driven configuration
- [ ] Performance analytics integration
- [ ] Enhanced debugging tools
- [ ] Plugin template generation
- [ ] Async plugin support documentation

## Related Resources

- [Microsoft Dataverse Developer Documentation](https://docs.microsoft.com/en-us/power-apps/developer/data-platform/)
- [Plugin Development Best Practices](https://docs.microsoft.com/en-us/power-apps/developer/data-platform/plug-ins)
- [Understanding Plugin Depth](https://docs.microsoft.com/en-us/power-apps/developer/data-platform/understand-plug-in-execution)
- [Dynamics 365 Plugin Development Guide](https://docs.microsoft.com/en-us/dynamics365/customer-engagement/developer/plugins-overview)

## Changelog

See [CHANGELOG.md](CHANGELOG.md) for version history and updates.

---

**Made with ❤️ for Dataverse developers**

*D365FL.Dataverse.PluginHelper - Making Dataverse plugins clean, maintainable, and fast.*
