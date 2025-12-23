# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

This is a C# design patterns educational repository implementing 23 Gang of Four design patterns. Each pattern includes conceptual implementations and real-world examples demonstrating practical applications in .NET development.

## Technology Stack

- **Language**: C# with .NET 8.0
- **Project Structure**: Visual Studio solution with multiple projects
- **Target Framework**: net8.0
- **Features**: ImplicitUsings and Nullable reference types enabled

## Project Structure

The repository is organized by design pattern category:

### Creational Patterns
- `Singleton/` - Single instance pattern with thread-safety considerations
- `FactoryMethod/` - Interface for object creation with subclass instantiation
- `AbstractFactory/` - Families of related objects without concrete classes
- `Builder/` - Step-by-step construction of complex objects
- `Prototype/` - Cloning existing objects

### Structural Patterns
- `Adapter/` - Interface compatibility between incompatible classes
- `Bridge/` - Separation of abstraction from implementation
- `Composite/` - Tree structures of objects
- `Decorator/` - Dynamic behavior addition to objects
- `Facade/` - Simplified interface to complex subsystems
- `Flyweight/` - Sharing common state between objects
- `Proxy/` - Placeholder or surrogate for another object

### Behavioral Patterns
- `ChainOfResponsibility/` - Request handling chain
- `Command/` - Encapsulated requests as objects
- `Iterator/` - Sequential access to collection elements
- `Mediator/` - Centralized communication between objects
- `Memento/` - Object state capture and restoration
- `Observer/` - Notification system for state changes
- `State/` - Behavior change based on internal state
- `Strategy/` - Interchangeable algorithm families
- `TemplateMethod/` - Algorithm skeleton with subclass steps
- `Visitor/` - Operations on object structure elements

### Pattern Directory Organization
Each pattern directory contains:
- `Conceptual/` - Basic implementation demonstrating the pattern structure
- Additional subdirectories with real-world examples (e.g., `DataReader/`, `DrawShape/`, `RemoteControlDevice/`)
- `PatternName.md` - Comprehensive documentation with definition, pros/cons, use cases, and expert advice

## Building and Running

### Build the entire solution
```bash
dotnet build DesignPattern.sln
```

### Build a specific pattern
```bash
dotnet build Singleton/Conceptual/Conceptual.csproj
dotnet build FactoryMethod/Conceptual/Conceptual.csproj
```

### Run a specific pattern example
```bash
dotnet run --project Singleton/Conceptual/Conceptual.csproj
dotnet run --project FactoryMethod/Conceptual/Conceptual.csproj
dotnet run --project Adapter/DataReader/DataReader.csproj
```

### Build all projects in release mode
```bash
dotnet build DesignPattern.sln -c Release
```

## Documentation Standards

Each pattern's markdown file follows this structure:
1. **Definition** - Clear explanation of the pattern's purpose and implementation approach
2. **Pros** - Benefits and advantages of using the pattern
3. **Cons** - Drawbacks and potential issues
4. **Real-world Use Cases in C# & .NET** - Practical examples with code snippets
5. **Modern Approach** - Dependency Injection and contemporary .NET practices
6. **Expert Advice** - When to use vs when not to use, with specific recommendations

## Code Organization Patterns

### Naming Conventions
- **Abstract classes**: `Creator`, `Product` (base pattern classes)
- **Concrete classes**: `ConcreteCreator1`, `ConcreteProduct1` (numbered implementations)
- **Interfaces**: `IProduct`, `IFactory` (I-prefix convention)
- **Subdirectories**: Pattern-specific names matching use case (e.g., `Creators/`, `Products/`)

### Project Configuration
All projects use:
- `OutputType`: Exe (console applications)
- `ImplicitUsings`: Enabled
- `Nullable`: Enabled (nullable reference types)
- Target framework: net8.0

## Dependency Injection Approach

The documentation emphasizes modern .NET practices:
- Prefer DI container registration over manual singleton implementation
- Use `IServiceProvider` for runtime object resolution
- Implement factory services that integrate with ASP.NET Core DI
- Register strategies and factories as scoped/singleton services
- Leverage .NET 8+ keyed dependency injection for strategy selection

## Working with This Repository

### Adding a New Pattern
1. Create a new solution folder in `DesignPattern.sln`
2. Add a `Conceptual/` subdirectory with basic implementation
3. Create a `PatternName.md` following the established template
4. Add real-world example subdirectories as needed
5. Ensure all projects target net8.0 with ImplicitUsings and Nullable enabled

### Modifying Existing Patterns
- Conceptual examples should remain simple and focused on pattern structure
- Real-world examples should demonstrate practical .NET scenarios
- Update corresponding markdown files when changing implementation approach
- Maintain consistency with existing naming conventions

### Documentation Updates
- Focus on C# and .NET-specific implementations
- Include code examples showing modern DI patterns
- Balance theoretical knowledge with practical guidance
- Provide clear "when to use" vs "when not to use" advice
