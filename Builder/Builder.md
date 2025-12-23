# Builder Pattern
## 1. Definition
The Builder Pattern separates the construction of a complex object from its representation, allowing the same construction process to create different representations. It constructs complex objects step by step, providing fine control over the construction process. Unlike other creational patterns that construct objects in one step, the Builder pattern builds objects incrementally through a series of method calls.

### Manual Implementation
1. Define a Product class representing the complex object being built
2. Create a Builder interface declaring construction steps (BuildPartA, BuildPartB, etc.)
3. Implement Concrete Builder classes that execute the construction steps and track the product being built
4. Optionally create a Director class that defines the order of construction steps for common configurations
5. Client code uses builders directly or through the director to construct products

## 2. Pros
- **Step-by-Step Construction**: Builds complex objects incrementally, deferring construction steps until needed
- **Code Reusability**: Same construction code can produce different product representations
- **Single Responsibility**: Isolates complex construction code from business logic
- **Fine-Grained Control**: Allows precise control over the construction process
- **Immutability Support**: Can build immutable objects by returning the final product only after all steps complete
- **Readable Code**: Method chaining creates fluent, self-documenting construction code

## 3. Cons
- **Increased Complexity**: Requires creating multiple new classes (Builder, Director, Product)
- **Code Duplication**: Similar builder methods across different concrete builders
- **Overhead for Simple Objects**: Overkill when object construction is straightforward
- **Builder Synchronization**: Builders are typically not thread-safe, requiring careful usage in concurrent scenarios
- **Interface Changes Ripple**: Adding new construction steps requires updating all concrete builders

## 4. Real-world Use Cases in C# & .NET
- **String Building**: StringBuilder for efficient string construction
```csharp
// StringBuilder is a classic builder pattern example
var sb = new StringBuilder();
sb.Append("Hello")
  .Append(" ")
  .Append("World")
  .AppendLine("!");
string result = sb.ToString();
```
- **HTTP Request Building**: HttpRequestMessage construction with multiple options
```csharp
var request = new HttpRequestMessage()
{
    Method = HttpMethod.Post,
    RequestUri = new Uri("https://api.example.com"),
    Content = new StringContent(json, Encoding.UTF8, "application/json")
};
request.Headers.Add("Authorization", "Bearer token");
```
- **Query Builders**: LINQ query construction, Entity Framework query builders
- **Configuration Objects**: Building complex configuration with multiple optional parameters
- **Test Data Builders**: Creating test objects with various configurations
- **UI Component Construction**: Building complex UI trees (WPF, Blazor components)
- **Document Generation**: PDF, Word, or HTML document builders with sections, styles, and content

## 5. Modern Approach: Fluent Builder with Method Chaining
```csharp
// Modern fluent builder pattern
public class EmailBuilder
{
    private string _to = string.Empty;
    private string _subject = string.Empty;
    private string _body = string.Empty;
    private List<string> _attachments = new();

    public EmailBuilder To(string to)
    {
        _to = to;
        return this; // Return 'this' for method chaining
    }

    public EmailBuilder WithSubject(string subject)
    {
        _subject = subject;
        return this;
    }

    public EmailBuilder WithBody(string body)
    {
        _body = body;
        return this;
    }

    public EmailBuilder AddAttachment(string filePath)
    {
        _attachments.Add(filePath);
        return this;
    }

    public Email Build()
    {
        // Validation before building
        if (string.IsNullOrEmpty(_to))
            throw new InvalidOperationException("Recipient is required");

        return new Email(_to, _subject, _body, _attachments);
    }
}

// Usage with fluent interface
var email = new EmailBuilder()
    .To("user@example.com")
    .WithSubject("Important Notice")
    .WithBody("This is the email body")
    .AddAttachment("report.pdf")
    .Build();

// Register builders in DI if needed
builder.Services.AddTransient<EmailBuilder>();
```

**Benefits over traditional Builder:**
- Fluent interface makes code more readable and self-documenting
- Method chaining reduces boilerplate code
- Can validate state before building
- Works seamlessly with dependency injection
- Easy to extend with new optional parameters

## 6. Expert Advice: When to Use vs When Not to Use

### ✅ When to Use Builder:
- **Many Constructor Parameters**: Objects with 4+ constructor parameters, especially optional ones
- **Immutable Objects**: Building immutable objects that require all properties set before creation
- **Step-by-Step Construction**: Object creation requires multiple steps in specific order
- **Multiple Representations**: Same construction process creates different object variants
- **Complex Initialization**: Objects requiring validation, computation, or complex logic during construction
- **Telescoping Constructor Problem**: Avoiding multiple overloaded constructors with different parameter combinations
- **Test Data Creation**: Creating test objects with various configurations in unit tests

### ❌ When NOT to Use Builder:
- **Simple Objects**: Objects with 1-3 parameters that can use constructors or object initializers
- **High-Frequency Object Creation**: When builder overhead impacts performance critically
- **Stable Object Structure**: Objects that rarely change and have straightforward construction
- **Value Objects**: Simple value objects where direct instantiation is clearer
- **Single Representation**: When there's only one way to construct the object
- **C# Object Initializers Sufficient**: When object initializer syntax is adequate

### 🎯 Expert Recommendations:
- **Use Fluent Interface**: Return `this` from builder methods for method chaining
- **Validate in Build()**: Perform validation in the Build() method before returning the product
- **Consider Required vs Optional**: Use constructor for required parameters, builder methods for optional
```csharp
public class UserBuilder
{
    private readonly string _email; // Required via constructor
    private string? _firstName; // Optional via builder method

    public UserBuilder(string email) // Required parameter
    {
        _email = email;
    }

    public UserBuilder WithFirstName(string firstName)
    {
        _firstName = firstName;
        return this;
    }
}
```
- **Use Records with with Expression**: For simple scenarios, C# records can replace builders
```csharp
// Record with default values
public record User(string Email, string? FirstName = null, string? LastName = null);

// Usage - simpler than builder for basic cases
var user = new User("user@example.com") with { FirstName = "John" };
```
- **Reset After Build()**: Clear builder state after Build() to allow reuse
- **Make Builders Thread-Safe When Needed**: Use separate builder instances per thread
- **Consider Init-Only Properties**: Use C# 9+ init properties for semi-immutable objects
- **Document Required Steps**: Clearly specify which builder methods must be called
- **Use Director for Common Configurations**: Encapsulate common construction sequences in a Director class
