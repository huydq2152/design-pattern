namespace Conceptual;

// The singleton class defines the `GetInstance` method that serves as an alternative
// to constructor and lets clients access the same instance of this class over and over.

// The singleton should always be a thread-safe class.

// The singleton should always be a sealed class to prevent class inheritance through external class and nested classes.

// Improment: with C#, can use Lazy<T> to make the singleton thread-safe without using locks. 
public sealed class Singleton
{
    
    // The Singleton's constructor should always be private to prevent direct construction calls with the `new` operator
    // from outside the class.
    private Singleton()
    {
    }

    private static Singleton? _instance;
    private static readonly object Lock = new object();
    public string? Value { get; private init; }

    public static Singleton GetInstance(string? value)
    {
        if (_instance is null)
        {
            lock (Lock)
            {
                _instance ??= new Singleton
                {
                    Value = value
                };
            }
        }

        return _instance;
    }
    
    
    // Any singleton should define some business logic, which can be executed on its instance.
    public void PrintSingletonValue()
    {
        Console.WriteLine($"Current singleton value is {Value}");
    }
    
    public void DoSomething()
    {
        Console.WriteLine("Doing something...");
    }
}