// The Observer pattern defines a one-to-many dependency between objects so that when one object changes state,
// all its dependents are notified and updated automatically. It's the foundation of event-driven programming.
// This conceptual example demonstrates the core structure and interactions of the pattern.

/// <summary>
/// The IObserver interface declares the update method used by subjects.
///
/// KEY PRINCIPLES:
/// - All observers implement the same interface
/// - Called by subject when state changes
/// - Receives notification with subject reference
/// - Can pull data from subject as needed
///
/// DESIGN BENEFITS:
/// - Loose coupling between subject and observers
/// - Multiple observers can react to same event
/// - Observers can be added/removed dynamically
///
/// REAL-WORLD EXAMPLES:
/// - Event handlers in UI frameworks
/// - Model-View relationship in MVC/MVVM
/// - Publish-subscribe messaging systems
/// - Stock price monitoring applications
/// </summary>
public interface IObserver
{
    /// <summary>
    /// Update method called when subject's state changes.
    ///
    /// DESIGN PATTERNS:
    /// - Pull model: Observer pulls data from subject (as shown)
    /// - Push model: Subject pushes data to observer
    /// </summary>
    void Update(ISubject subject);
}

/// <summary>
/// The ISubject interface declares methods for managing observers.
///
/// KEY RESPONSIBILITIES:
/// - Maintains list of observers
/// - Provides methods to attach/detach observers
/// - Notifies all observers when state changes
/// </summary>
public interface ISubject
{
    void Attach(IObserver observer);
    void Detach(IObserver observer);
    void Notify();
}

/// <summary>
/// Subject is the concrete implementation of observable object.
///
/// KEY RESPONSIBILITIES:
/// - Stores state that observers care about
/// - Maintains list of observers
/// - Notifies observers when state changes
///
/// REAL-WORLD ANALOGS:
/// - Stock ticker (price changes notify subscribers)
/// - Temperature sensor (readings notify displays)
/// - Model in MVC (changes notify views)
/// </summary>
public class Subject : ISubject
{
    public int State { get; set; } = -0;
    private List<IObserver> _observers = new List<IObserver>();

    public void Attach(IObserver observer)
    {
        Console.WriteLine("Subject: Attached an observer.");
        _observers.Add(observer);
    }

    public void Detach(IObserver observer)
    {
        _observers.Remove(observer);
        Console.WriteLine("Subject: Detached an observer.");
    }

    /// <summary>
    /// Notifies all observers about state change.
    ///
    /// NOTIFICATION ALGORITHM:
    /// - Iterates through observer list
    /// - Calls Update() on each observer
    /// - Passes 'this' so observers can pull data
    /// </summary>
    public void Notify()
    {
        Console.WriteLine("Subject: Notifying observers...");

        foreach (var observer in _observers)
        {
            observer.Update(this);
        }
    }

    /// <summary>
    /// Business logic that changes state and triggers notifications.
    ///
    /// TYPICAL FLOW:
    /// 1. Execute business logic
    /// 2. Modify state
    /// 3. Call Notify() to inform observers
    /// </summary>
    public void SomeBusinessLogic()
    {
        Console.WriteLine("\nSubject: I'm doing something important.");
        State = new Random().Next(0, 10);

        Thread.Sleep(15);

        Console.WriteLine("Subject: My state has just changed to: " + State);
        Notify();
    }
}

/// <summary>
/// ConcreteObserverA demonstrates a specific observer implementation.
///
/// CHARACTERISTICS:
/// - Reacts to specific state values
/// - Pulls data from subject when notified
/// - Independent from other observers
/// </summary>
public class ConcreteObserverA : IObserver
{
    public void Update(ISubject subject)
    {
        if ((subject as Subject).State < 3)
        {
            Console.WriteLine("ConcreteObserverA: Reacted to the event.");
        }
    }
}

/// <summary>
/// ConcreteObserverB demonstrates another observer with different reaction logic.
///
/// INDEPENDENCE:
/// - Different reaction criteria than ObserverA
/// - Same interface, different behavior
/// - Shows polymorphism in action
/// </summary>
public class ConcreteObserverB : IObserver
{
    public void Update(ISubject subject)
    {
        if ((subject as Subject).State == 0 || (subject as Subject).State >= 2)
        {
            Console.WriteLine("ConcreteObserverB: Reacted to the event.");
        }
    }
}

public static class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("=== Observer Pattern Demonstration ===\n");

        var subject = new Subject();
        var observerA = new ConcreteObserverA();
        subject.Attach(observerA);

        var observerB = new ConcreteObserverB();
        subject.Attach(observerB);

        subject.SomeBusinessLogic();
        subject.SomeBusinessLogic();

        subject.Detach(observerB);

        subject.SomeBusinessLogic();
    }
}

/*
 * KEY TAKEAWAYS:
 *
 * PATTERN COMPONENTS:
 * 1. IObserver - Interface for observers
 * 2. ISubject - Interface for observable subjects
 * 3. Subject - Concrete subject maintaining state and observers
 * 4. ConcreteObserverA/B - Specific observer implementations
 *
 * WHEN TO USE:
 * - One object's changes should trigger updates in others
 * - Need to broadcast notifications to multiple objects
 * - Want loose coupling between objects
 * - Number of observers can change at runtime
 *
 * BENEFITS:
 * 1. Loose Coupling: Subject and observers are independent
 * 2. Dynamic Relationships: Can add/remove observers at runtime
 * 3. Broadcast Communication: One-to-many notifications
 * 4. Open/Closed: Can add observers without modifying subject
 *
 * REAL-WORLD EXAMPLES:
 * - .NET Events: event EventHandler
 * - INotifyPropertyChanged: WPF/MVVM data binding
 * - Reactive Extensions (Rx.NET): IObservable<T>
 * - Stock market: Price changes notify traders
 *
 * MODERN .NET USAGE:
 * - event keyword with EventHandler
 * - INotifyPropertyChanged for data binding
 * - IObservable<T> in Reactive Extensions
 *
 * BEST PRACTICES:
 * 1. Always detach observers when done
 * 2. Use weak references for long-lived subjects
 * 3. Handle exceptions in observer notifications
 * 4. Consider async notifications for I/O
 */
