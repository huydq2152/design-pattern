// The Memento pattern captures and externalizes an object's internal state without violating encapsulation,
// so that the object can be restored to this state later. It's commonly used for undo/redo functionality.
// This conceptual example demonstrates the core structure and interactions of the pattern.

/// <summary>
/// The Originator class contains important state that may change over time.
///
/// KEY RESPONSIBILITIES:
/// - Holds the current state that needs to be saved
/// - Creates mementos containing snapshots of its current state
/// - Restores its state from mementos
/// - Has full access to memento internals
///
/// DESIGN DECISIONS:
/// - Knows how to save its own state (who better to know what's important?)
/// - Creates memento objects with all necessary state data
/// - Can restore from mementos without exposing internal structure
/// - Memento is an opaque object to everyone except the Originator
///
/// REAL-WORLD EXAMPLES:
/// - Text editor (saves document state for undo)
/// - Game state (saves player position, health, inventory)
/// - Form data (saves user input for recovery)
/// - Drawing application (saves canvas state)
///
/// CHARACTERISTICS:
/// - Single Responsibility: Manages its business logic and state saving
/// - Encapsulation: Internal state remains private
/// - No dependency on how mementos are stored (that's Caretaker's job)
/// - Can have complex state spanning multiple fields
/// </summary>
public class Originator
{
    private string _state;

    public Originator(string state)
    {
        _state = state;
        Console.WriteLine("Originator: My initial state is: " + state);
    }

    /// <summary>
    /// Business logic that changes the Originator's state.
    ///
    /// PATTERN DEMONSTRATION:
    /// - Simulates work that modifies state
    /// - In real applications, this would be actual business operations
    ///
    /// USE CASES:
    /// - User editing a document (state = document content)
    /// - Player moving in game (state = position, health, etc.)
    /// - User filling out form (state = form field values)
    /// </summary>
    public void DoSomething()
    {
        Console.WriteLine("Originator: I'm doing something important.");
        _state = GenerateRandomString(30);
        Console.WriteLine($"Originator: and my state has changed to: {_state}");
    }

    private string GenerateRandomString(int length = 10)
    {
        var allowedSymbols = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var result = string.Empty;

        while (length > 0)
        {
            result += allowedSymbols[new Random().Next(0, allowedSymbols.Length)];

            Thread.Sleep(12);

            length--;
        }

        return result;
    }

    /// <summary>
    /// Saves the current state to a Memento object.
    ///
    /// PATTERN CORE:
    /// - Creates a snapshot of current state
    /// - Returns opaque memento object
    /// - Only Originator can read memento contents
    ///
    /// ENCAPSULATION:
    /// - Internal state remains private
    /// - Memento provides controlled access to state snapshot
    /// - Maintains object-oriented principles
    /// </summary>
    public IMemento Save()
    {
        return new ConcreteMemento(_state);
    }

    /// <summary>
    /// Restores the Originator's state from a Memento.
    ///
    /// RESTORATION PROCESS:
    /// - Validates memento type
    /// - Extracts state from memento
    /// - Updates internal state
    ///
    /// BENEFITS:
    /// - Undo functionality
    /// - Rollback on errors
    /// - State recovery after crashes
    /// </summary>
    public void Restore(IMemento memento)
    {
        if (memento is not ConcreteMemento)
        {
            throw new Exception("Unknown memento class " + memento);
        }

        _state = memento.GetState();
        Console.Write($"Originator: My state has changed to: {_state}");
    }
}

/// <summary>
/// The IMemento interface defines the contract for memento objects.
///
/// KEY PRINCIPLES:
/// - Provides narrow interface to Caretaker (limited access)
/// - Provides wide interface to Originator (full access)
/// - Hides implementation details from clients
///
/// DESIGN BENEFITS:
/// - Encapsulation: State is hidden from everyone except Originator
/// - Type safety: Can use interface for polymorphism
/// - Flexibility: Can have multiple memento implementations
/// </summary>
public interface IMemento
{
    string GetName();
    string GetState();
    DateTime GetDate();
}

/// <summary>
/// ConcreteMemento stores the Originator's state.
///
/// KEY RESPONSIBILITIES:
/// - Stores snapshot of Originator's state
/// - Immutable after creation (state doesn't change)
/// - Provides metadata about the snapshot
///
/// CHARACTERISTICS:
/// - Serializable: Can be saved to disk or database
/// - Immutable: Once created, never changes
/// - Lightweight: Just data, minimal overhead
/// - Independent: No dependencies on Originator
/// </summary>
public class ConcreteMemento : IMemento
{
    private readonly string _state;
    private readonly DateTime _date;

    public ConcreteMemento(string state)
    {
        _state = state;
        _date = DateTime.Now;
    }

    public string GetState()
    {
        return _state;
    }

    public string GetName()
    {
        return $"{_date} / ({_state.Substring(0, 9)})...";
    }

    public DateTime GetDate()
    {
        return _date;
    }
}

/// <summary>
/// The Caretaker manages memento storage and retrieval.
///
/// KEY RESPONSIBILITIES:
/// - Stores mementos (save history)
/// - Retrieves mementos for restoration
/// - Never modifies or inspects memento contents
///
/// CHARACTERISTICS:
/// - Single Responsibility: Only manages memento storage
/// - Depends on IMemento interface, not concrete implementation
/// - Provides convenient methods for save/undo operations
///
/// REAL-WORLD ANALOGS:
/// - Undo/redo stack in text editor
/// - Save game manager
/// - Version control system
/// </summary>
public class Caretaker
{
    private List<IMemento> _mementos = new List<IMemento>();
    private Originator _originator;

    public Caretaker(Originator originator)
    {
        _originator = originator;
    }

    /// <summary>
    /// Saves the current state of the Originator.
    ///
    /// WHEN TO USE:
    /// - Before risky operations
    /// - On user command (Ctrl+S)
    /// - Periodic auto-save
    /// </summary>
    public void Backup()
    {
        Console.WriteLine("\nCaretaker: Saving Originator's state...");
        _mementos.Add(_originator.Save());
    }

    /// <summary>
    /// Restores the Originator to its previous state.
    ///
    /// UNDO BEHAVIOR:
    /// - Last-In-First-Out (LIFO) - like Ctrl+Z
    /// - Removes memento from history
    /// </summary>
    public void Undo()
    {
        if (_mementos.Count == 0)
        {
            return;
        }

        var memento = _mementos.Last();
        _mementos.Remove(memento);

        Console.WriteLine("Caretaker: Restoring state to: " + memento.GetName());

        try
        {
            _originator.Restore(memento);
        }
        catch (Exception)
        {
            Undo();
        }
    }

    public void ShowHistory()
    {
        Console.WriteLine("Caretaker: Here's the list of mementos:");

        foreach (var memento in _mementos)
        {
            Console.WriteLine(memento.GetName());
        }
    }
}

public static class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("=== Memento Pattern Demonstration ===\n");

        var originator = new Originator("Super-duper-super-puper-super.");
        var caretaker = new Caretaker(originator);

        caretaker.Backup();
        originator.DoSomething();

        caretaker.Backup();
        originator.DoSomething();

        caretaker.Backup();
        originator.DoSomething();

        Console.WriteLine();
        caretaker.ShowHistory();

        Console.WriteLine("\nClient: Now, let's rollback!\n");
        caretaker.Undo();

        Console.WriteLine("\n\nClient: Once more!\n");
        caretaker.Undo();

        Console.WriteLine();
    }
}

/*
 * KEY TAKEAWAYS:
 *
 * PATTERN COMPONENTS:
 * 1. Originator - Object whose state needs to be saved
 * 2. Memento - Stores snapshot of Originator's state
 * 3. Caretaker - Manages memento storage and retrieval
 * 4. IMemento - Interface providing narrow access to memento
 *
 * WHEN TO USE:
 * - Need undo/redo functionality
 * - Need to save and restore object state
 * - Want to implement snapshots or checkpoints
 * - Need to rollback changes on error
 *
 * BENEFITS:
 * 1. Encapsulation: Object internals remain private
 * 2. Undo/Redo: Easy to implement by storing mementos
 * 3. Simplified Originator: No complex state management code
 * 4. Snapshot Support: Can save state at any time
 * 5. Error Recovery: Can rollback on failures
 *
 * REAL-WORLD EXAMPLES:
 * - Text Editor: Ctrl+Z/Ctrl+Y undo/redo
 * - Game Saves: Save/load player state
 * - Form Auto-Save: Recover user input after crash
 * - Database Transactions: Savepoints for rollback
 * - Configuration Backup: Save settings before changes
 *
 * TRADE-OFFS:
 * - Memory Usage: Storing many mementos consumes memory
 * - Serialization Cost: Creating mementos can be expensive
 * - Storage Management: Need to limit history or clean up
 *
 * MODERN .NET USAGE:
 * - Entity Framework: context.Entry(entity).OriginalValues
 * - System.Transactions: Transaction savepoints
 * - ASP.NET Core: Session state preservation
 *
 * BEST PRACTICES:
 * 1. Use immutable mementos
 * 2. Limit history size
 * 3. Make memento opaque to Caretaker
 * 4. Implement redo with separate stack
 * 5. Persist important saves to disk
 */
