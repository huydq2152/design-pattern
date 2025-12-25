// The Iterator pattern provides a way to access elements of a collection sequentially
// without exposing the underlying representation (list, stack, tree, etc.).
// This conceptual example demonstrates implementing custom iterators in C#.

using System.Collections;

/// <summary>
/// The Iterator abstract class provides the interface for traversing a collection.
///
/// KEY RESPONSIBILITIES:
/// - Implements IEnumerator to integrate with C# foreach loops
/// - Provides methods for sequential access: MoveNext(), Current, Reset()
/// - Maintains current position in the traversal
/// - Decouples client code from collection's internal structure
///
/// DESIGN DECISIONS:
/// - Extends IEnumerator for .NET compatibility
/// - Adds Key() method for index tracking (not in standard IEnumerator)
/// - Abstract class allows sharing common behavior among iterators
/// - Current() is abstract to allow different return type strategies
///
/// REAL-WORLD EXAMPLES:
/// - IEnumerator<T> in .NET collections
/// - Database result set cursors
/// - File directory traversal
/// - Tree traversal (pre-order, in-order, post-order)
///
/// CHARACTERISTICS:
/// - Stateful: Maintains current position
/// - Single direction: Typically moves forward only (but can support reverse)
/// - Lazy evaluation: Can fetch elements on demand
/// - Independent: Multiple iterators can traverse same collection independently
/// </summary>
abstract class Iterator : IEnumerator
{
    /// <summary>
    /// Implements IEnumerator.Current using the abstract Current() method.
    ///
    /// EXPLICIT INTERFACE IMPLEMENTATION:
    /// - Required by IEnumerator interface
    /// - Returns object type (non-generic)
    /// - Delegates to abstract Current() method
    ///
    /// This enables foreach loop support in C#:
    /// foreach (var item in collection) { ... }
    /// </summary>
    object IEnumerator.Current => Current();

    /// <summary>
    /// Returns the current position/index in the collection.
    ///
    /// CUSTOM EXTENSION:
    /// - Not part of standard IEnumerator interface
    /// - Useful for tracking position during iteration
    /// - Allows clients to know where they are in the sequence
    ///
    /// Real-world use cases:
    /// - Displaying "Item 3 of 10"
    /// - Pagination (current page number)
    /// - Progress tracking
    /// </summary>
    public abstract int Key();

    /// <summary>
    /// Returns the current element in the collection.
    ///
    /// CONTRACT:
    /// - Should return the element at the current position
    /// - Behavior is undefined if called before MoveNext() or after iteration ends
    /// - Should not advance the iterator (that's MoveNext's job)
    ///
    /// DESIGN NOTE:
    /// - Returns object (non-generic) for maximum flexibility
    /// - In real applications, prefer generic Iterator<T> with T Current()
    /// - Called by foreach loop after each successful MoveNext()
    /// </summary>
    public abstract object Current();

    /// <summary>
    /// Advances to the next element in the collection.
    ///
    /// CONTRACT:
    /// - Returns true if advanced to a valid element
    /// - Returns false if reached end of collection
    /// - Must be called before first Current() access
    ///
    /// USAGE PATTERN:
    /// while (iterator.MoveNext())
    /// {
    ///     var current = iterator.Current();
    ///     // Process current
    /// }
    ///
    /// FOREACH TRANSLATION:
    /// foreach (var item in collection)
    /// Translates to:
    /// var enumerator = collection.GetEnumerator();
    /// while (enumerator.MoveNext())
    /// {
    ///     var item = enumerator.Current;
    /// }
    /// </summary>
    public abstract bool MoveNext();

    /// <summary>
    /// Resets the iterator to the beginning of the collection.
    ///
    /// PURPOSE:
    /// - Allows reusing the same iterator for multiple traversals
    /// - Required by IEnumerator interface
    ///
    /// IMPLEMENTATION NOTES:
    /// - Not all iterators support Reset() (can throw NotSupportedException)
    /// - Rarely used in practice (usually create new iterator instead)
    /// - Useful for bidirectional iteration or restarting traversal
    ///
    /// WARNING:
    /// - Some iterators throw NotSupportedException for Reset()
    /// - Collection modifications may invalidate the iterator
    /// - Consider creating new iterator instead of resetting
    /// </summary>
    public abstract void Reset();
}

/// <summary>
/// The IteratorAggregate abstract class defines the interface for creating iterators.
///
/// KEY RESPONSIBILITIES:
/// - Implements IEnumerable to enable foreach loops
/// - Provides factory method for creating iterators
/// - Decouples collection from iterator implementation
///
/// DESIGN PATTERN:
/// - Factory Method: GetEnumerator() creates appropriate iterator
/// - Abstraction: Clients depend on IEnumerable, not concrete collection
///
/// CHARACTERISTICS:
/// - Can return different iterator types for different traversal strategies
/// - Supports multiple simultaneous iterations
/// - Hides internal collection structure from clients
///
/// REAL-WORLD EXAMPLES:
/// - IEnumerable<T> in .NET (List<T>, Dictionary<T,V>, etc.)
/// - Queryable collections (LINQ)
/// - Custom data structures (trees, graphs)
/// </summary>
abstract class IteratorAggregate : IEnumerable
{
    /// <summary>
    /// Returns an iterator for traversing the collection.
    ///
    /// FACTORY METHOD PATTERN:
    /// - Creates and returns appropriate iterator for this collection
    /// - Can return different iterator types (forward, reverse, filtered)
    /// - Each call typically returns a new iterator instance
    ///
    /// ENABLES FOREACH:
    /// This method is what makes foreach work:
    /// foreach (var item in collection)
    ///
    /// The compiler translates this to:
    /// var enumerator = collection.GetEnumerator();
    /// while (enumerator.MoveNext()) { var item = enumerator.Current; }
    ///
    /// MULTIPLE ITERATIONS:
    /// Each GetEnumerator() call returns independent iterator:
    /// var iter1 = collection.GetEnumerator();
    /// var iter2 = collection.GetEnumerator();
    /// // iter1 and iter2 can traverse independently
    /// </summary>
    public abstract IEnumerator GetEnumerator();
}

/// <summary>
/// AlphabeticalOrderIterator provides forward and reverse iteration over a collection.
///
/// KEY CHARACTERISTICS:
/// - Concrete implementation of Iterator for WordsCollection
/// - Supports bidirectional traversal (forward and reverse)
/// - Maintains current position state
/// - Implements lazy iteration (doesn't load all elements upfront)
///
/// DESIGN DECISIONS:
/// - Holds reference to collection (not a copy)
/// - Position starts at -1 (before first element)
/// - Reverse flag determines traversal direction
/// - Validates position bounds before moving
///
/// REAL-WORLD EXAMPLES:
/// - Sorted list traversal
/// - Alphabetical name listing
/// - Reverse chronological timeline
/// - Bidirectional pagination
///
/// THREAD SAFETY:
/// - Not thread-safe: multiple threads shouldn't share iterator
/// - Collection modification during iteration causes undefined behavior
/// - For thread safety, lock collection or create snapshot
/// </summary>
class AlphabeticalOrderIterator : Iterator
{
    private WordsCollection _collection;

    // Current position in the collection
    // -1 = before first element (initial state)
    // 0 to Count-1 = valid positions
    // Count or -1 = after last element
    private int _position = -1;

    private bool _reverse;

    /// <summary>
    /// Initializes the iterator for the given collection.
    ///
    /// INITIALIZATION STRATEGY:
    /// - Forward iteration: position = -1 (before first element)
    /// - Reverse iteration: position = count (after last element)
    ///
    /// WHY -1 INITIAL POSITION:
    /// - MoveNext() must be called before first Current()
    /// - -1 indicates "haven't started yet"
    /// - First MoveNext() advances to position 0 (first element)
    ///
    /// REVERSE ITERATION:
    /// - Starts after last element (position = count)
    /// - First MoveNext() decrements to count-1 (last element)
    /// - Continues decrementing until position < 0
    /// </summary>
    /// <param name="collection">The collection to iterate over</param>
    /// <param name="reverse">True for reverse iteration, false for forward</param>
    public AlphabeticalOrderIterator(WordsCollection collection, bool reverse = false)
    {
        _collection = collection;
        _reverse = reverse;

        if (reverse)
        {
            // Start after last element for reverse iteration
            _position = collection.getItems().Count;
        }
    }

    /// <summary>
    /// Returns the current element at the current position.
    ///
    /// CONTRACT:
    /// - MoveNext() must be called first (position must be valid)
    /// - Throws if position is out of bounds
    /// - Does not advance position (that's MoveNext's job)
    ///
    /// IMPLEMENTATION:
    /// - Directly accesses collection by index
    /// - Assumes position is valid (validated by MoveNext)
    ///
    /// POTENTIAL IMPROVEMENTS:
    /// - Add bounds checking
    /// - Return null for invalid position
    /// - Throw InvalidOperationException for invalid state
    /// </summary>
    public override object Current()
    {
        return _collection.getItems()[_position];
    }

    /// <summary>
    /// Returns the current position index in the collection.
    ///
    /// USE CASES:
    /// - Display "Item X of Y" to user
    /// - Track progress through collection
    /// - Implement pagination
    /// - Debugging and logging
    ///
    /// NOTE: Not part of standard IEnumerator interface
    /// This is a custom extension specific to this implementation.
    /// </summary>
    public override int Key()
    {
        return _position;
    }

    /// <summary>
    /// Advances the iterator to the next element.
    ///
    /// FORWARD ITERATION:
    /// - Increments position by 1
    /// - Returns true if new position is valid (within bounds)
    /// - Returns false if reached end of collection
    ///
    /// REVERSE ITERATION:
    /// - Decrements position by 1
    /// - Returns true if new position is valid (>= 0)
    /// - Returns false if reached beginning of collection
    ///
    /// BOUNDARY CONDITIONS:
    /// Forward: position < Count
    /// Reverse: position >= 0
    ///
    /// DESIGN PATTERN:
    /// - Calculate next position first
    /// - Validate before committing
    /// - Only update position if valid
    /// - Return boolean to indicate success
    ///
    /// This ensures Current() is only called on valid positions.
    /// </summary>
    public override bool MoveNext()
    {
        // Calculate next position based on direction
        int updatedPosition = _position + (_reverse ? -1 : 1);

        // Check if new position is within bounds
        if (updatedPosition >= 0 && updatedPosition < _collection.getItems().Count)
        {
            // Valid position: update and return true
            _position = updatedPosition;
            return true;
        }

        // Invalid position: end of iteration
        return false;
    }

    /// <summary>
    /// Resets the iterator to the beginning of the collection.
    ///
    /// RESET BEHAVIOR:
    /// - Forward: position = 0 (first element)
    /// - Reverse: position = count-1 (last element)
    ///
    /// NOTE: This implementation starts AT the first/last element,
    /// not BEFORE it like the constructor. This means Current() can
    /// be called immediately after Reset() without calling MoveNext().
    ///
    /// ALTERNATIVE DESIGN:
    /// Could reset to -1 (forward) or count (reverse) to maintain
    /// consistency with constructor, requiring MoveNext() before Current().
    ///
    /// USE CASES:
    /// - Restarting iteration without creating new iterator
    /// - Implementing circular iteration
    /// - Resetting after partial traversal
    /// </summary>
    public override void Reset()
    {
        _position = _reverse ? _collection.getItems().Count - 1 : 0;
    }
}

/// <summary>
/// WordsCollection is a concrete collection that provides iterator access.
///
/// KEY RESPONSIBILITIES:
/// - Stores a collection of strings
/// - Provides methods to add items
/// - Creates iterators for traversal
/// - Controls iteration direction
///
/// DESIGN DECISIONS:
/// - Uses List<string> internally (implementation detail hidden from clients)
/// - Provides public method to access items (could be replaced with read-only property)
/// - Maintains direction state (could be removed, passing direction to GetEnumerator)
/// - Implements IEnumerable via IteratorAggregate
///
/// ITERATOR PATTERN BENEFIT:
/// - Clients iterate using foreach without knowing internal structure
/// - Could change from List to Array, LinkedList, etc. without affecting clients
/// - Supports multiple simultaneous iterations
/// - Can provide different iteration strategies (alphabetical, reverse, filtered)
///
/// REAL-WORLD EXAMPLES:
/// - List<T>, Dictionary<K,V> in .NET
/// - Custom business collections (OrderCollection, CustomerCollection)
/// - Result sets from queries
/// - Tree or graph data structures
///
/// CHARACTERISTICS:
/// - Encapsulation: Internal list is private
/// - Flexibility: Can change internal structure without affecting iterators
/// - Multiple iterators: Each GetEnumerator() creates independent iterator
/// </summary>
class WordsCollection : IteratorAggregate
{
    // Internal storage - implementation detail hidden from clients
    List<string> _collection = new List<string>();

    // Controls default iteration direction
    // Alternative: Could pass direction to GetEnumerator() instead
    bool _direction;

    /// <summary>
    /// Toggles the iteration direction between forward and reverse.
    ///
    /// DESIGN NOTE:
    /// This approach stores direction as state. Alternatively, could accept
    /// direction parameter in GetEnumerator():
    ///
    /// public IEnumerator GetEnumerator(bool reverse = false)
    /// {
    ///     return new AlphabeticalOrderIterator(this, reverse);
    /// }
    ///
    /// TRADE-OFFS:
    /// Current approach (state):
    /// - Simpler foreach usage: foreach (var item in collection)
    /// - Direction applies to all subsequent iterations
    /// - Thread-unsafe if direction changes during iteration
    ///
    /// Parameter approach:
    /// - More explicit: collection.GetForwardEnumerator() vs GetReverseEnumerator()
    /// - Thread-safe: each iteration specifies direction
    /// - More flexible: different iterations can use different directions
    /// </summary>
    public void ReverseDirection()
    {
        _direction = !_direction;
    }

    /// <summary>
    /// Returns the internal collection.
    ///
    /// ENCAPSULATION CONCERN:
    /// - Exposes internal list directly (breaks encapsulation)
    /// - Allows external modification of collection
    /// - Iterator may behave unexpectedly if collection is modified
    ///
    /// BETTER ALTERNATIVES:
    /// 1. Return read-only collection:
    ///    public IReadOnlyList<string> GetItems() => _collection.AsReadOnly();
    ///
    /// 2. Return copy:
    ///    public List<string> GetItems() => new List<string>(_collection);
    ///
    /// 3. Remove this method entirely:
    ///    Iterators access _collection via constructor parameter
    ///    Clients use iterator only, never direct access
    ///
    /// CURRENT USAGE:
    /// Used by AlphabeticalOrderIterator to access elements.
    /// Consider making WordsCollection and AlphabeticalOrderIterator
    /// nested classes or using friend-like pattern.
    /// </summary>
    public List<string> getItems()
    {
        return _collection;
    }

    /// <summary>
    /// Adds an item to the collection.
    ///
    /// COLLECTION MODIFICATION:
    /// - Adding items is safe if no active iterations
    /// - Modifying collection during iteration causes undefined behavior
    /// - .NET collections throw InvalidOperationException if modified during iteration
    ///
    /// THREAD SAFETY:
    /// - Not thread-safe: concurrent Add() calls may corrupt list
    /// - Consider using ConcurrentBag or locking for multi-threaded scenarios
    ///
    /// ITERATOR IMPLICATIONS:
    /// - Existing iterators may not see newly added items
    /// - Iterators may throw if collection is modified during iteration
    /// - Create new iterator after modification for consistent view
    /// </summary>
    public void AddItem(string item)
    {
        _collection.Add(item);
    }

    /// <summary>
    /// Creates and returns an iterator for this collection.
    ///
    /// FACTORY METHOD:
    /// - Creates AlphabeticalOrderIterator configured with current direction
    /// - Each call creates new independent iterator
    /// - Enables foreach loop support
    ///
    /// ITERATION INDEPENDENCE:
    /// var iter1 = collection.GetEnumerator();
    /// var iter2 = collection.GetEnumerator();
    /// // iter1 and iter2 traverse independently
    ///
    /// DIRECTION HANDLING:
    /// - Uses current _direction state
    /// - All iterations created after ReverseDirection() will be reversed
    /// - Consider parameterized version for more flexibility
    ///
    /// FOREACH SUPPORT:
    /// This method enables:
    /// foreach (var word in wordsCollection) { ... }
    ///
    /// Compiler translates to:
    /// var enumerator = wordsCollection.GetEnumerator();
    /// try {
    ///     while (enumerator.MoveNext()) {
    ///         var word = enumerator.Current;
    ///         ...
    ///     }
    /// }
    /// finally {
    ///     (enumerator as IDisposable)?.Dispose();
    /// }
    /// </summary>
    public override IEnumerator GetEnumerator()
    {
        return new AlphabeticalOrderIterator(this, _direction);
    }
}

/// <summary>
/// Program demonstrates the Iterator pattern in action.
/// Shows how to iterate over a collection in forward and reverse directions.
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Iterator Pattern Demonstration ===\n");

        Console.WriteLine("--- Example 1: Forward Iteration ---\n");

        // Create a collection and populate it
        var collection = new WordsCollection();
        collection.AddItem("First");
        collection.AddItem("Second");
        collection.AddItem("Third");

        Console.WriteLine("Straight traversal:");

        // foreach automatically uses GetEnumerator()
        // This calls: GetEnumerator() -> returns iterator
        // Then repeatedly: MoveNext() and Current() until MoveNext() returns false
        foreach (var element in collection)
        {
            Console.WriteLine(element);
        }
        // Output: First, Second, Third

        Console.WriteLine();

        Console.WriteLine("--- Example 2: Reverse Iteration ---\n");

        Console.WriteLine("Reverse traversal:");

        // Toggle direction for future iterations
        collection.ReverseDirection();

        // Same foreach syntax, but now iterates in reverse
        // The collection's GetEnumerator() creates a reverse iterator
        foreach (var element in collection)
        {
            Console.WriteLine(element);
        }
        // Output: Third, Second, First

        Console.WriteLine();

        Console.WriteLine("--- Example 3: Manual Iterator Usage ---\n");

        // Can also use iterator manually (without foreach)
        collection.ReverseDirection(); // Back to forward
        var iterator = collection.GetEnumerator();

        Console.WriteLine("Manual iteration with position tracking:");
        while (iterator.MoveNext())
        {
            var current = iterator.Current;
            var position = (iterator as AlphabeticalOrderIterator)?.Key();
            Console.WriteLine($"Position {position}: {current}");
        }

        Console.WriteLine();

        Console.WriteLine("--- Example 4: Multiple Independent Iterators ---\n");

        // Multiple iterators can traverse the same collection independently
        var iter1 = collection.GetEnumerator();
        var iter2 = collection.GetEnumerator();

        Console.WriteLine("Two independent iterators:");
        iter1.MoveNext();
        Console.WriteLine($"Iterator 1: {iter1.Current}"); // First

        iter2.MoveNext();
        iter2.MoveNext();
        Console.WriteLine($"Iterator 2: {iter2.Current}"); // Second

        iter1.MoveNext();
        Console.WriteLine($"Iterator 1: {iter1.Current}"); // Second

        // Iterators maintain independent positions

        Console.WriteLine();

        Console.WriteLine("--- Real-World Benefits ---");
        Console.WriteLine("1. Encapsulation: Internal structure hidden from client");
        Console.WriteLine("2. Multiple traversals: Can iterate same collection multiple ways");
        Console.WriteLine("3. Uniform interface: foreach works with any IEnumerable");
        Console.WriteLine("4. Lazy evaluation: Elements accessed on demand");
        Console.WriteLine("5. Memory efficient: Don't need to load entire collection");
        Console.WriteLine("6. Flexibility: Can change internal structure without affecting clients");
    }
}

/*
 * KEY TAKEAWAYS:
 *
 * PATTERN COMPONENTS:
 * 1. Iterator - Abstract class/interface for traversing (IEnumerator in .NET)
 * 2. ConcreteIterator - Implements iteration logic (AlphabeticalOrderIterator)
 * 3. Aggregate - Abstract class/interface for collections (IEnumerable in .NET)
 * 4. ConcreteAggregate - Implements collection and creates iterators (WordsCollection)
 *
 * WHEN TO USE:
 * - Need to traverse a collection without exposing its internal structure
 * - Want to provide multiple traversal methods (forward, reverse, filtered)
 * - Need to iterate over different data structures with uniform interface
 * - Want to decouple iteration algorithm from collection implementation
 * - Support lazy evaluation and on-demand element access
 * - Enable multiple simultaneous traversals of same collection
 *
 * BENEFITS:
 * 1. Encapsulation: Collection internals hidden from clients
 * 2. Single Responsibility: Collection focuses on storage, iterator on traversal
 * 3. Open/Closed: Can add new iterators without modifying collection
 * 4. Uniform Interface: All collections iterable via foreach
 * 5. Multiple Iterators: Independent simultaneous traversals
 * 6. Lazy Evaluation: Elements accessed only when needed
 * 7. Flexibility: Can change collection structure without affecting iteration
 *
 * REAL-WORLD EXAMPLES IN .NET:
 * - IEnumerable<T> and IEnumerator<T>: Core .NET iteration interfaces
 * - List<T>, Dictionary<K,V>, HashSet<T>: All implement IEnumerable
 * - LINQ: Query operators return IEnumerable for lazy evaluation
 * - yield return: C# language feature for easy iterator implementation
 * - foreach loop: Syntactic sugar for iterator pattern
 * - IAsyncEnumerable<T>: Async iteration in .NET Core 3.0+
 * - DbDataReader: Iterates database query results
 * - DirectoryInfo.EnumerateFiles(): Lazy file system iteration
 *
 * C# YIELD KEYWORD:
 * Modern C# makes iterators trivial with yield return:
 *
 * public IEnumerable<string> GetWords()
 * {
 *     yield return "First";
 *     yield return "Second";
 *     yield return "Third";
 * }
 *
 * Compiler automatically generates iterator state machine.
 *
 * ITERATOR TYPES:
 * 1. External Iterator: Client controls iteration (shown in this example)
 * 2. Internal Iterator: Collection controls iteration (ForEach methods)
 * 3. Lazy Iterator: Elements computed on demand (LINQ, yield return)
 * 4. Snapshot Iterator: Iterates over collection snapshot
 * 5. Robust Iterator: Handles collection modifications during iteration
 *
 * TRADE-OFFS:
 * - Overhead: Creating iterator objects has cost (minimal in practice)
 * - Modification issues: Collection changes during iteration can cause errors
 * - Memory: Iterator maintains state (position, references)
 * - Complexity: Simple loops might be clearer for simple cases
 *
 * COMPARISON WITH OTHER PATTERNS:
 * - Composite: Iterator often used to traverse Composite structures
 * - Factory Method: GetEnumerator() is a factory method for iterators
 * - Memento: Iterator maintains traversal state similar to memento
 * - Visitor: Alternative traversal pattern with different trade-offs
 *
 * MODERN .NET USAGE:
 * - IEnumerable<T> and IEnumerator<T> are the standard
 * - yield return for easy iterator implementation
 * - LINQ uses iterators extensively for query composition
 * - IAsyncEnumerable<T> for async streaming data
 * - foreach pattern recognition (duck typing, not just IEnumerable)
 * - Index and Range operators (C# 8.0+) complement iteration
 *
 * BEST PRACTICES:
 * 1. Implement IEnumerable<T> (generic) instead of IEnumerable when possible
 * 2. Use yield return for simple iterators (compiler generates state machine)
 * 3. Make iterators read-only: don't allow modification during iteration
 * 4. Consider IAsyncEnumerable for async data sources (databases, streams)
 * 5. Document whether iterator is snapshot or live view
 * 6. Implement IDisposable if iterator holds resources
 * 7. Support LINQ: return IEnumerable<T> for method chaining
 * 8. Consider concurrent collections for thread-safe iteration
 * 9. Throw InvalidOperationException if collection modified during iteration
 * 10. Prefer foreach over manual iteration (cleaner, safer)
 *
 * YIELD RETURN EXAMPLE (Modern C#):
 * public class WordsCollection : IEnumerable<string>
 * {
 *     private List<string> _words = new List<string>();
 *
 *     public IEnumerator<string> GetEnumerator()
 *     {
 *         // Forward iteration
 *         foreach (var word in _words)
 *             yield return word;
 *     }
 *
 *     public IEnumerator<string> GetReverseEnumerator()
 *     {
 *         // Reverse iteration
 *         for (int i = _words.Count - 1; i >= 0; i--)
 *             yield return _words[i];
 *     }
 *
 *     IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
 * }
 *
 * Much simpler than manual iterator implementation!
 */
