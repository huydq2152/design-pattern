namespace Conceptual.Components;

// ConcreteComponent provides the base implementation of the Component interface
// This is the object being decorated - the core functionality that decorators enhance
//
// Characteristics:
// - Implements the actual business logic
// - Can be used standalone or wrapped by decorators
// - Unaware of decorators (follows dependency inversion)
//
// Real-world examples:
// - EmailNotificationService (can be decorated with logging, retry, etc.)
// - FileStream (can be decorated with BufferedStream, GZipStream, etc.)
// - BasicRepository (can be decorated with caching, logging, validation)
// - SimpleTextProcessor (can be decorated with spell check, formatting, etc.)
public class ConcreteComponent : Component
{
    // Provides the base implementation without any additional behavior
    // Decorators will wrap this and add their own behavior
    public override string Operation()
    {
        return "ConcreteComponent";
    }
}
