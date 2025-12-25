# Memento Pattern

## 1. Definition

The Memento pattern captures and externalizes an object's internal state without violating encapsulation, so that the object can be restored to this state later. It provides the ability to restore an object to its previous state (undo via rollback), enabling features like undo/redo, snapshots, and transaction rollback.

**How it works:**
- **The Originator** creates mementos containing snapshots of its internal state
- **The Memento** stores the state snapshot (opaque to everyone except Originator)
- **The Caretaker** manages mementos but never examines or modifies their contents
- **Restoration** happens by passing a memento back to the Originator

The key principle is **encapsulation preservation**: the Originator's internal state remains private. Only the Originator can read from or write to mementos. The Caretaker stores mementos without accessing their internals.

**Key Components:**
- **Originator**: Object whose state needs to be saved
- **Memento**: Immutable snapshot of Originator's state
- **Caretaker**: Manages memento storage and history

## 2. Pros

- **Encapsulation Preservation**: Object's internal state remains private and hidden
- **Undo/Redo Support**: Easy to implement by storing state history
- **Simplified Originator**: No need for complex state management code
- **Snapshot Capability**: Can capture state at any point in time
- **Error Recovery**: Can rollback to previous state on failure
- **Time-Travel Debugging**: Can restore to any historical state
- **Transaction Support**: Enables commit/rollback behavior
- **State History**: Maintains timeline of all state changes
- **No Impact on Originator**: Originator doesn't need undo logic

## 3. Cons

- **Memory Consumption**: Storing many mementos uses significant memory
- **Serialization Overhead**: Creating mementos can be expensive for complex objects
- **Storage Management**: Need to limit history size or implement cleanup
- **Deep Copy Complexity**: Cloning complex object graphs is challenging
- **Encapsulation Leakage**: If poorly implemented, can expose internal state
- **Caretaker Overhead**: Managing memento lifecycle adds complexity
- **Versioning Issues**: Old mementos may become incompatible with new Originator versions
- **Performance Impact**: Frequent saving can slow down the application

## 4. Real-world Use Cases in C# & .NET

### Text Editor with Undo/Redo

```csharp
// Without Memento - Manual state management
public class TextEditor
{
    private string _content;
    private Stack<string> _undoStack = new(); // Exposed internals

    public void Type(string text)
    {
        _undoStack.Push(_content); // Manually managing history
        _content += text;
    }

    public void Undo()
    {
        if (_undoStack.Any())
            _content = _undoStack.Pop();
    }
}

// With Memento - Clean separation of concerns
public interface IMemento
{
    DateTime Timestamp { get; }
    string GetName();
}

public class TextEditor
{
    private string _content = "";
    private int _cursorPosition = 0;
    private string _selectedText = "";

    public void Type(string text)
    {
        _content = _content.Insert(_cursorPosition, text);
        _cursorPosition += text.Length;
    }

    public void SelectText(int start, int length)
    {
        _selectedText = _content.Substring(start, length);
    }

    public IMemento Save()
    {
        return new EditorMemento(_content, _cursorPosition, _selectedText);
    }

    public void Restore(IMemento memento)
    {
        if (memento is EditorMemento editorMemento)
        {
            _content = editorMemento.Content;
            _cursorPosition = editorMemento.CursorPosition;
            _selectedText = editorMemento.SelectedText;
        }
    }

    // Private memento class - only TextEditor can access internals
    private class EditorMemento : IMemento
    {
        public string Content { get; }
        public int CursorPosition { get; }
        public string SelectedText { get; }
        public DateTime Timestamp { get; }

        public EditorMemento(string content, int cursor, string selected)
        {
            Content = content;
            CursorPosition = cursor;
            SelectedText = selected;
            Timestamp = DateTime.Now;
        }

        public string GetName() => $"{Timestamp:HH:mm:ss} - {Content.Substring(0, Math.Min(20, Content.Length))}...";
    }
}

// Caretaker manages undo/redo stacks
public class EditorHistory
{
    private readonly TextEditor _editor;
    private readonly Stack<IMemento> _undoStack = new();
    private readonly Stack<IMemento> _redoStack = new();

    public EditorHistory(TextEditor editor)
    {
        _editor = editor;
    }

    public void Backup()
    {
        _undoStack.Push(_editor.Save());
        _redoStack.Clear(); // Clear redo history on new action
    }

    public void Undo()
    {
        if (_undoStack.Count > 0)
        {
            _redoStack.Push(_editor.Save()); // Save current state for redo
            _editor.Restore(_undoStack.Pop());
        }
    }

    public void Redo()
    {
        if (_redoStack.Count > 0)
        {
            _undoStack.Push(_editor.Save()); // Save current state for undo
            _editor.Restore(_redoStack.Pop());
        }
    }
}

// Usage
var editor = new TextEditor();
var history = new EditorHistory(editor);

history.Backup(); // Save initial state
editor.Type("Hello ");

history.Backup(); // Save after first change
editor.Type("World");

history.Undo(); // Back to "Hello "
history.Undo(); // Back to ""
history.Redo(); // Forward to "Hello "
```

### Game Save System

```csharp
public class GameState
{
    private Vector3 _playerPosition;
    private int _health;
    private int _score;
    private List<string> _inventory;
    private int _level;

    public void Move(Vector3 newPosition) => _playerPosition = newPosition;
    public void TakeDamage(int damage) => _health -= damage;
    public void AddScore(int points) => _score += points;
    public void AddItem(string item) => _inventory.Add(item);

    // Create save point
    public IGameSave CreateSave(string saveName)
    {
        return new GameSaveMemento(
            saveName,
            _playerPosition,
            _health,
            _score,
            new List<string>(_inventory), // Deep copy
            _level
        );
    }

    // Load from save
    public void LoadSave(IGameSave save)
    {
        if (save is GameSaveMemento memento)
        {
            _playerPosition = memento.PlayerPosition;
            _health = memento.Health;
            _score = memento.Score;
            _inventory = new List<string>(memento.Inventory);
            _level = memento.Level;
        }
    }

    // Private memento implementation
    private class GameSaveMemento : IGameSave
    {
        public string SaveName { get; }
        public DateTime SaveTime { get; }
        public Vector3 PlayerPosition { get; }
        public int Health { get; }
        public int Score { get; }
        public List<string> Inventory { get; }
        public int Level { get; }

        public GameSaveMemento(string name, Vector3 pos, int health, int score, List<string> inv, int level)
        {
            SaveName = name;
            SaveTime = DateTime.Now;
            PlayerPosition = pos;
            Health = health;
            Score = score;
            Inventory = inv;
            Level = level;
        }
    }
}

public interface IGameSave
{
    string SaveName { get; }
    DateTime SaveTime { get; }
}

// Save manager (Caretaker)
public class SaveManager
{
    private readonly Dictionary<string, IGameSave> _saves = new();

    public void SaveGame(GameState game, string saveName)
    {
        _saves[saveName] = game.CreateSave(saveName);
        Console.WriteLine($"Game saved: {saveName}");
    }

    public void LoadGame(GameState game, string saveName)
    {
        if (_saves.TryGetValue(saveName, out var save))
        {
            game.LoadSave(save);
            Console.WriteLine($"Game loaded: {saveName}");
        }
    }

    public void DeleteSave(string saveName)
    {
        _saves.Remove(saveName);
    }

    public List<string> GetSaveList()
    {
        return _saves.Keys.ToList();
    }
}

// Usage
var game = new GameState();
var saveManager = new SaveManager();

// Play game
game.Move(new Vector3(10, 0, 5));
game.AddScore(100);

// Save checkpoint
saveManager.SaveGame(game, "Checkpoint 1");

// Continue playing
game.TakeDamage(50);
game.AddItem("Magic Sword");

// Save again
saveManager.SaveGame(game, "Checkpoint 2");

// Load earlier save
saveManager.LoadGame(game, "Checkpoint 1");
```

### .NET Framework Examples

**Entity Framework Change Tracking**
```csharp
// Entity Framework uses memento-like pattern for change tracking
using (var context = new AppDbContext())
{
    var customer = context.Customers.Find(1);

    // Original values stored (memento)
    var originalValues = context.Entry(customer).OriginalValues;

    // Modify entity
    customer.Name = "New Name";
    customer.Email = "new@email.com";

    // Check what changed
    var currentValues = context.Entry(customer).CurrentValues;
    var changedProperties = context.Entry(customer)
        .Properties
        .Where(p => p.IsModified)
        .Select(p => p.Metadata.Name);

    // Revert changes (restore from memento)
    context.Entry(customer).CurrentValues.SetValues(originalValues);

    // Or save changes
    context.SaveChanges();
}
```

**ASP.NET Core Session State**
```csharp
// Session state acts as memento storage
public class ShoppingCartController : Controller
{
    public IActionResult AddToCart(int productId)
    {
        // Get cart from session (memento)
        var cart = HttpContext.Session.Get<ShoppingCart>("Cart")
                   ?? new ShoppingCart();

        cart.AddItem(productId);

        // Save back to session (create memento)
        HttpContext.Session.Set("Cart", cart);

        return RedirectToAction("Index");
    }

    public IActionResult Checkout()
    {
        // Restore cart state from session
        var cart = HttpContext.Session.Get<ShoppingCart>("Cart");

        // Process checkout
        ProcessOrder(cart);

        // Clear session (clear memento)
        HttpContext.Session.Remove("Cart");

        return View("OrderComplete");
    }
}
```

**System.Transactions Savepoints**
```csharp
// Transaction savepoints use memento pattern
using (var connection = new SqlConnection(connectionString))
{
    connection.Open();
    using (var transaction = connection.BeginTransaction())
    {
        try
        {
            // Perform operations
            ExecuteCommand("INSERT INTO Orders ...", transaction);

            // Create savepoint (memento)
            transaction.Save("BeforeRiskyOperation");

            try
            {
                // Risky operation
                ExecuteCommand("UPDATE Inventory ...", transaction);
            }
            catch
            {
                // Rollback to savepoint (restore from memento)
                transaction.Rollback("BeforeRiskyOperation");
            }

            // Commit transaction
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
        }
    }
}
```

### Form Auto-Save for Crash Recovery

```csharp
public class FormState
{
    private Dictionary<string, object> _fieldValues = new();

    public void SetField(string name, object value)
    {
        _fieldValues[name] = value;
    }

    public object GetField(string name)
    {
        return _fieldValues.TryGetValue(name, out var value) ? value : null;
    }

    // Create memento
    public IFormMemento CreateSnapshot()
    {
        return new FormMemento(new Dictionary<string, object>(_fieldValues));
    }

    // Restore from memento
    public void RestoreSnapshot(IFormMemento memento)
    {
        if (memento is FormMemento formMemento)
        {
            _fieldValues = new Dictionary<string, object>(formMemento.FieldValues);
        }
    }

    private class FormMemento : IFormMemento
    {
        public Dictionary<string, object> FieldValues { get; }
        public DateTime Timestamp { get; }

        public FormMemento(Dictionary<string, object> fields)
        {
            FieldValues = fields;
            Timestamp = DateTime.Now;
        }
    }
}

public interface IFormMemento
{
    DateTime Timestamp { get; }
}

// Auto-save manager
public class AutoSaveManager
{
    private readonly FormState _formState;
    private readonly Timer _autoSaveTimer;
    private IFormMemento? _lastSave;

    public AutoSaveManager(FormState formState, TimeSpan autoSaveInterval)
    {
        _formState = formState;
        _autoSaveTimer = new Timer(
            _ => AutoSave(),
            null,
            autoSaveInterval,
            autoSaveInterval
        );
    }

    private void AutoSave()
    {
        _lastSave = _formState.CreateSnapshot();
        // Persist to local storage or database
        SaveToLocalStorage(_lastSave);
        Console.WriteLine($"Form auto-saved at {DateTime.Now}");
    }

    public void RecoverFromCrash()
    {
        var recovered = LoadFromLocalStorage();
        if (recovered != null)
        {
            _formState.RestoreSnapshot(recovered);
            Console.WriteLine("Form data recovered from auto-save");
        }
    }

    private void SaveToLocalStorage(IFormMemento memento)
    {
        // Save to browser local storage, file, or database
    }

    private IFormMemento? LoadFromLocalStorage()
    {
        // Load from storage
        return null;
    }
}
```

## 5. Modern Approach: Dependency Injection

In modern C# and ASP.NET Core applications, the Memento pattern can be integrated with dependency injection and persistence.

```csharp
// Memento interface
public interface IMemento<TState>
{
    TState State { get; }
    DateTime Timestamp { get; }
    string Description { get; }
}

// Generic memento implementation
public class Memento<TState> : IMemento<TState>
{
    public TState State { get; }
    public DateTime Timestamp { get; }
    public string Description { get; }

    public Memento(TState state, string description = "")
    {
        State = state;
        Timestamp = DateTime.Now;
        Description = description;
    }
}

// Originator interface
public interface IStatefulObject<TState>
{
    IMemento<TState> CreateMemento();
    void RestoreMemento(IMemento<TState> memento);
}

// Document editor with DI
public class Document : IStatefulObject<DocumentState>
{
    private DocumentState _state;
    private readonly ILogger<Document> _logger;

    public Document(ILogger<Document> logger)
    {
        _logger = logger;
        _state = new DocumentState { Content = "", Title = "Untitled" };
    }

    public void EditContent(string content)
    {
        _state.Content = content;
        _state.LastModified = DateTime.Now;
        _logger.LogInformation("Document content modified");
    }

    public void SetTitle(string title)
    {
        _state.Title = title;
        _logger.LogInformation("Document title changed to: {Title}", title);
    }

    public IMemento<DocumentState> CreateMemento()
    {
        // Deep clone state
        var stateCopy = new DocumentState
        {
            Content = _state.Content,
            Title = _state.Title,
            LastModified = _state.LastModified
        };

        return new Memento<DocumentState>(stateCopy, $"Snapshot: {_state.Title}");
    }

    public void RestoreMemento(IMemento<DocumentState> memento)
    {
        _state = new DocumentState
        {
            Content = memento.State.Content,
            Title = memento.State.Title,
            LastModified = memento.State.LastModified
        };

        _logger.LogInformation("Document restored from: {Description}", memento.Description);
    }
}

public class DocumentState
{
    public string Content { get; set; } = "";
    public string Title { get; set; } = "";
    public DateTime LastModified { get; set; }
}

// History manager with persistence
public interface IHistoryManager<TState>
{
    Task SaveSnapshotAsync(IMemento<TState> memento);
    Task<IMemento<TState>?> GetSnapshotAsync(DateTime timestamp);
    Task<List<IMemento<TState>>> GetHistoryAsync();
    Task UndoAsync();
    Task RedoAsync();
}

public class DocumentHistoryManager : IHistoryManager<DocumentState>
{
    private readonly Document _document;
    private readonly IDbContext _dbContext;
    private readonly Stack<IMemento<DocumentState>> _undoStack = new();
    private readonly Stack<IMemento<DocumentState>> _redoStack = new();

    public DocumentHistoryManager(Document document, IDbContext dbContext)
    {
        _document = document;
        _dbContext = dbContext;
    }

    public async Task SaveSnapshotAsync(IMemento<DocumentState> memento)
    {
        _undoStack.Push(memento);
        _redoStack.Clear();

        // Persist to database
        await _dbContext.DocumentSnapshots.AddAsync(new DocumentSnapshotEntity
        {
            Content = memento.State.Content,
            Title = memento.State.Title,
            Timestamp = memento.Timestamp,
            Description = memento.Description
        });

        await _dbContext.SaveChangesAsync();
    }

    public async Task UndoAsync()
    {
        if (_undoStack.Count > 0)
        {
            var currentState = _document.CreateMemento();
            _redoStack.Push(currentState);

            var previousState = _undoStack.Pop();
            _document.RestoreMemento(previousState);
        }
    }

    public async Task RedoAsync()
    {
        if (_redoStack.Count > 0)
        {
            var currentState = _document.CreateMemento();
            _undoStack.Push(currentState);

            var nextState = _redoStack.Pop();
            _document.RestoreMemento(nextState);
        }
    }

    public async Task<List<IMemento<DocumentState>>> GetHistoryAsync()
    {
        var snapshots = await _dbContext.DocumentSnapshots
            .OrderByDescending(s => s.Timestamp)
            .ToListAsync();

        return snapshots.Select(s => new Memento<DocumentState>(
            new DocumentState
            {
                Content = s.Content,
                Title = s.Title,
                LastModified = s.Timestamp
            },
            s.Description
        )).Cast<IMemento<DocumentState>>().ToList();
    }

    public async Task<IMemento<DocumentState>?> GetSnapshotAsync(DateTime timestamp)
    {
        var snapshot = await _dbContext.DocumentSnapshots
            .FirstOrDefaultAsync(s => s.Timestamp == timestamp);

        if (snapshot == null) return null;

        return new Memento<DocumentState>(
            new DocumentState
            {
                Content = snapshot.Content,
                Title = snapshot.Title,
                LastModified = snapshot.Timestamp
            },
            snapshot.Description
        );
    }
}

// Register in Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<IDbContext, AppDbContext>();
builder.Services.AddScoped<Document>();
builder.Services.AddScoped<IHistoryManager<DocumentState>, DocumentHistoryManager>();

var app = builder.Build();

// Controller using memento
[ApiController]
[Route("api/[controller]")]
public class DocumentController : ControllerBase
{
    private readonly Document _document;
    private readonly IHistoryManager<DocumentState> _historyManager;

    public DocumentController(
        Document document,
        IHistoryManager<DocumentState> historyManager)
    {
        _document = document;
        _historyManager = historyManager;
    }

    [HttpPost("edit")]
    public async Task<IActionResult> EditDocument([FromBody] EditRequest request)
    {
        // Save current state before editing
        await _historyManager.SaveSnapshotAsync(_document.CreateMemento());

        // Make changes
        _document.EditContent(request.Content);

        return Ok();
    }

    [HttpPost("undo")]
    public async Task<IActionResult> Undo()
    {
        await _historyManager.UndoAsync();
        return Ok();
    }

    [HttpPost("redo")]
    public async Task<IActionResult> Redo()
    {
        await _historyManager.RedoAsync();
        return Ok();
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetHistory()
    {
        var history = await _historyManager.GetHistoryAsync();
        return Ok(history);
    }
}
```

**Benefits of DI Integration:**
- **Testability**: Easy to mock dependencies
- **Persistence**: Snapshots saved to database
- **Logging**: Built-in logging for state changes
- **Scalability**: History can be distributed across storage
- **Async Support**: Non-blocking save/restore operations

## 6. Expert Advice: When to Use vs When Not to Use

### ✅ When to Use Memento:

- **Undo/Redo Functionality**: Need to implement undo/redo in applications
  ```csharp
  // Good: Text editor, graphics editor, any tool with undo
  var editor = new TextEditor();
  var history = new EditorHistory(editor);

  editor.Type("Hello");
  history.Backup();

  editor.Type(" World");
  history.Undo(); // Back to "Hello"
  ```

- **Transaction Rollback**: Need to rollback on error
  ```csharp
  // Good: Database operations, business transactions
  var memento = order.Save();
  try
  {
      order.Process();
      order.Charge();
  }
  catch
  {
      order.Restore(memento); // Rollback
  }
  ```

- **Checkpoint Systems**: Gaming, long-running processes
  ```csharp
  // Good: Game save points, workflow checkpoints
  saveManager.CreateCheckpoint("Level 1 Complete");
  // Player can restore to this checkpoint later
  ```

- **State History**: Need to track and navigate state changes
  ```csharp
  // Good: Version control, audit trails
  var history = document.GetHistory();
  var yesterday = history.First(h => h.Timestamp.Date == DateTime.Today.AddDays(-1));
  document.Restore(yesterday);
  ```

- **Form Auto-Save**: Recover from crashes
  ```csharp
  // Good: Long forms, surveys, editors
  autoSaveManager.SaveEvery(TimeSpan.FromMinutes(1));
  // Recover data if browser crashes
  ```

### ❌ When NOT to Use Memento:

- **Immutable State**: State never changes
  ```csharp
  // Bad: Unnecessary for immutable objects
  public record Product(string Name, decimal Price); // Already immutable

  // Better: Immutable objects don't need mementos
  var product = new Product("Widget", 9.99m);
  ```

- **Simple State**: State is trivial to recreate
  ```csharp
  // Bad: Overkill for simple counter
  public class Counter
  {
      private int _value;
      public IMemento Save() => new Memento(_value);
  }

  // Better: Just store the value
  int previousValue = counter.Value;
  ```

- **Large Objects**: Memento storage would be prohibitive
  ```csharp
  // Bad: Huge objects with frequent changes
  public class VideoEditor
  {
      private byte[] _videoData; // 4GB file
      public IMemento Save() => new Memento(_videoData); // Too expensive!
  }

  // Better: Use incremental saves or delta compression
  ```

- **Performance Critical**: Memento overhead is unacceptable
  ```csharp
  // Bad: Real-time gaming physics
  for (int i = 0; i < 1000000; i++)
  {
      memento = physicsEngine.Save(); // Too slow!
      physicsEngine.Update();
  }

  // Better: Don't save every frame
  ```

### 🎯 Expert Recommendations:

**1. Use Nested Private Classes for Strong Encapsulation**
```csharp
// Good: Memento only accessible to Originator
public class TextEditor
{
    private string _content;

    public IMemento Save() => new EditorMemento(_content);

    public void Restore(IMemento memento)
    {
        if (memento is EditorMemento m)
            _content = m.Content;
    }

    // Private nested class
    private class EditorMemento : IMemento
    {
        internal string Content { get; }
        public EditorMemento(string content) => Content = content;
    }
}
```

**2. Limit History Size**
```csharp
// Good: Prevent memory issues
public class HistoryManager
{
    private const int MaxHistorySize = 50;
    private Queue<IMemento> _history = new();

    public void Save(IMemento memento)
    {
        _history.Enqueue(memento);
        if (_history.Count > MaxHistorySize)
            _history.Dequeue(); // Remove oldest
    }
}
```

**3. Use Immutable Mementos**
```csharp
// Good: Prevent accidental modifications
public record DocumentMemento(
    string Content,
    string Title,
    DateTime Timestamp) : IMemento;

// Fields are init-only, cannot be changed after construction
```

**4. Implement Both Undo and Redo**
```csharp
// Good: Full undo/redo support
public class UndoRedoManager
{
    private Stack<IMemento> _undoStack = new();
    private Stack<IMemento> _redoStack = new();

    public void Execute(Action action)
    {
        _undoStack.Push(CreateMemento());
        _redoStack.Clear(); // Clear redo on new action
        action();
    }

    public void Undo()
    {
        if (_undoStack.Any())
        {
            _redoStack.Push(CreateMemento());
            Restore(_undoStack.Pop());
        }
    }

    public void Redo()
    {
        if (_redoStack.Any())
        {
            _undoStack.Push(CreateMemento());
            Restore(_redoStack.Pop());
        }
    }
}
```

**5. Consider Incremental Saves for Large Objects**
```csharp
// Good: Store only changes, not full state
public class DeltaMemento : IMemento
{
    public Dictionary<string, object> Changes { get; }

    public DeltaMemento(Dictionary<string, object> changes)
    {
        Changes = changes;
    }
}

// Only save what changed
var memento = new DeltaMemento(new Dictionary<string, object>
{
    ["Title"] = newTitle // Only title changed
});
```

**6. Persist Important Mementos**
```csharp
// Good: Survive application crashes
public class PersistentHistoryManager
{
    public async Task SaveAsync(IMemento memento)
    {
        // Save to database or file
        await File.WriteAllTextAsync(
            $"save_{DateTime.Now.Ticks}.json",
            JsonSerializer.Serialize(memento)
        );
    }
}
```

**7. Use Records for Simple Mementos**
```csharp
// Good: Modern C# approach
public record GameStateMemento(
    Vector3 Position,
    int Health,
    List<string> Inventory,
    DateTime SaveTime) : IMemento;

// Automatically immutable, value equality, readable
```

**Real-World Decision Flow:**
1. Need undo/redo? → Use Memento
2. Need to rollback on errors? → Use Memento
3. State is immutable? → Don't use Memento
4. State is simple (single value)? → Just store the value
5. Object is huge? → Consider incremental/delta mementos
6. Need crash recovery? → Use Memento with persistence
7. Performance critical? → Limit memento frequency

The Memento pattern excels at providing undo/redo functionality and state recovery while maintaining encapsulation. Use it when you need to capture and restore object state without exposing internal structure. Avoid it for simple state or when the overhead of creating snapshots outweighs the benefits.
