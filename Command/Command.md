# Command Pattern

## 1. Definition

The Command pattern encapsulates a request as an object, thereby allowing you to parameterize clients with different requests, queue or log requests, and support undoable operations. It converts requests into standalone objects that contain all information about the request, decoupling the object that invokes the operation from the one that knows how to perform it.

**How it works:**
- **Create a Command interface** with an Execute() method
- **Implement concrete commands** that encapsulate specific operations
- **Commands may delegate to a Receiver** object that contains the actual business logic
- **An Invoker** stores and triggers commands without knowing their implementation
- **The Client** creates commands and configures the invoker

The key insight is that by turning requests into objects, you gain flexibility in handling those requests - they can be passed around, stored, queued, logged, undone, or composed into larger commands.

**Key Components:**
- **Command Interface**: Declares Execute() method
- **Concrete Commands**: Implement Execute(), may delegate to Receiver
- **Receiver**: Contains actual business logic
- **Invoker**: Stores and executes commands
- **Client**: Creates and configures commands

## 2. Pros

- **Decoupling**: Separates objects that invoke operations from objects that perform operations
- **Single Responsibility**: Each command has one responsibility
- **Open/Closed Principle**: Can add new commands without modifying existing code
- **Undo/Redo**: Easy to implement by storing command history and state
- **Deferred Execution**: Commands can be created now and executed later
- **Command Composition**: Can build macro commands from simpler commands
- **Queuing**: Commands are objects that can be queued for batch processing
- **Logging**: Commands can be logged for audit trails or crash recovery
- **Transactional Behavior**: Group commands and commit/rollback together
- **Remote Execution**: Commands can be serialized and sent over network

## 3. Cons

- **Class Proliferation**: Each operation requires a new command class
- **Indirection**: Adds extra layer between invoker and receiver
- **Complexity**: May be overkill for simple operations
- **Memory Overhead**: Storing command history for undo uses memory
- **Learning Curve**: Team needs to understand the abstraction
- **Boilerplate Code**: Similar structure repeated across command classes
- **Maintenance**: More classes to maintain and test
- **Debugging**: Extra layer can make stack traces longer

## 4. Real-world Use Cases in C# & .NET

### Text Editor with Undo/Redo

```csharp
// Command interface with undo support
public interface ICommand
{
    void Execute();
    void Undo();
}

// Receiver: The document being edited
public class Document
{
    private StringBuilder _content = new StringBuilder();

    public void InsertText(int position, string text)
    {
        _content.Insert(position, text);
        Console.WriteLine($"Inserted: {text}");
    }

    public void DeleteText(int position, int length)
    {
        _content.Remove(position, length);
        Console.WriteLine($"Deleted {length} characters");
    }

    public string GetContent() => _content.ToString();
}

// Concrete command for text insertion
public class InsertTextCommand : ICommand
{
    private readonly Document _document;
    private readonly int _position;
    private readonly string _text;

    public InsertTextCommand(Document document, int position, string text)
    {
        _document = document;
        _position = position;
        _text = text;
    }

    public void Execute()
    {
        _document.InsertText(_position, _text);
    }

    public void Undo()
    {
        _document.DeleteText(_position, _text.Length);
    }
}

// Concrete command for text deletion
public class DeleteTextCommand : ICommand
{
    private readonly Document _document;
    private readonly int _position;
    private readonly int _length;
    private string _deletedText;

    public DeleteTextCommand(Document document, int position, int length)
    {
        _document = document;
        _position = position;
        _length = length;
    }

    public void Execute()
    {
        // Store deleted text for undo
        _deletedText = _document.GetContent().Substring(_position, _length);
        _document.DeleteText(_position, _length);
    }

    public void Undo()
    {
        _document.InsertText(_position, _deletedText);
    }
}

// Invoker with undo/redo support
public class TextEditor
{
    private readonly Stack<ICommand> _undoStack = new();
    private readonly Stack<ICommand> _redoStack = new();

    public void ExecuteCommand(ICommand command)
    {
        command.Execute();
        _undoStack.Push(command);
        _redoStack.Clear(); // Clear redo stack on new command
    }

    public void Undo()
    {
        if (_undoStack.Count > 0)
        {
            var command = _undoStack.Pop();
            command.Undo();
            _redoStack.Push(command);
        }
    }

    public void Redo()
    {
        if (_redoStack.Count > 0)
        {
            var command = _redoStack.Pop();
            command.Execute();
            _undoStack.Push(command);
        }
    }
}

// Usage
var document = new Document();
var editor = new TextEditor();

editor.ExecuteCommand(new InsertTextCommand(document, 0, "Hello "));
editor.ExecuteCommand(new InsertTextCommand(document, 6, "World"));
// Document: "Hello World"

editor.Undo(); // Remove "World"
editor.Undo(); // Remove "Hello "
editor.Redo(); // Add "Hello " back
```

### Task Scheduler with Command Queue

```csharp
public interface ICommand
{
    void Execute();
    string Description { get; }
}

// Email command
public class SendEmailCommand : ICommand
{
    private readonly string _to;
    private readonly string _subject;
    private readonly IEmailService _emailService;

    public SendEmailCommand(IEmailService emailService, string to, string subject)
    {
        _emailService = emailService;
        _to = to;
        _subject = subject;
    }

    public string Description => $"Send email to {_to}";

    public void Execute()
    {
        _emailService.Send(_to, _subject);
        Console.WriteLine($"Email sent to {_to}");
    }
}

// Report generation command
public class GenerateReportCommand : ICommand
{
    private readonly string _reportType;
    private readonly IReportService _reportService;

    public GenerateReportCommand(IReportService reportService, string reportType)
    {
        _reportService = reportService;
        _reportType = reportType;
    }

    public string Description => $"Generate {_reportType} report";

    public void Execute()
    {
        _reportService.Generate(_reportType);
        Console.WriteLine($"{_reportType} report generated");
    }
}

// Command scheduler
public class TaskScheduler
{
    private readonly Queue<ICommand> _commandQueue = new();
    private readonly List<ICommand> _executedCommands = new();

    public void ScheduleCommand(ICommand command)
    {
        _commandQueue.Enqueue(command);
        Console.WriteLine($"Scheduled: {command.Description}");
    }

    public void ExecuteAll()
    {
        while (_commandQueue.Count > 0)
        {
            var command = _commandQueue.Dequeue();
            Console.WriteLine($"Executing: {command.Description}");
            command.Execute();
            _executedCommands.Add(command);
        }
    }

    public void ExecuteNext()
    {
        if (_commandQueue.Count > 0)
        {
            var command = _commandQueue.Dequeue();
            command.Execute();
            _executedCommands.Add(command);
        }
    }

    public IReadOnlyList<ICommand> GetExecutionHistory() => _executedCommands.AsReadOnly();
}

// Usage
var scheduler = new TaskScheduler();
scheduler.ScheduleCommand(new SendEmailCommand(emailService, "user@example.com", "Welcome"));
scheduler.ScheduleCommand(new GenerateReportCommand(reportService, "Sales"));
scheduler.ScheduleCommand(new SendEmailCommand(emailService, "admin@example.com", "Report Ready"));

scheduler.ExecuteAll();
```

### Macro Command - Composite Pattern

```csharp
// Macro command that executes multiple commands
public class MacroCommand : ICommand
{
    private readonly List<ICommand> _commands = new();
    private readonly string _description;

    public MacroCommand(string description)
    {
        _description = description;
    }

    public string Description => _description;

    public void Add(ICommand command)
    {
        _commands.Add(command);
    }

    public void Execute()
    {
        Console.WriteLine($"Executing macro: {_description}");
        foreach (var command in _commands)
        {
            command.Execute();
        }
    }

    public void Undo()
    {
        // Undo in reverse order
        for (int i = _commands.Count - 1; i >= 0; i--)
        {
            if (_commands[i] is ICommand undoable)
            {
                undoable.Undo();
            }
        }
    }
}

// Usage: Create a "Save All" macro
var saveAllMacro = new MacroCommand("Save All Documents");
saveAllMacro.Add(new SaveDocumentCommand(document1));
saveAllMacro.Add(new SaveDocumentCommand(document2));
saveAllMacro.Add(new SaveDocumentCommand(document3));

saveAllMacro.Execute(); // Saves all three documents
```

### .NET Framework Examples

**ICommand in WPF/MVVM**
```csharp
// WPF uses ICommand extensively for button bindings
public class RelayCommand : ICommand
{
    private readonly Action<object> _execute;
    private readonly Predicate<object> _canExecute;

    public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public bool CanExecute(object parameter) => _canExecute?.Invoke(parameter) ?? true;

    public void Execute(object parameter) => _execute(parameter);

    public event EventHandler CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }
}

// ViewModel with commands
public class CustomerViewModel : INotifyPropertyChanged
{
    public ICommand SaveCommand { get; }
    public ICommand DeleteCommand { get; }

    public CustomerViewModel()
    {
        SaveCommand = new RelayCommand(
            execute: _ => SaveCustomer(),
            canExecute: _ => CanSaveCustomer());

        DeleteCommand = new RelayCommand(
            execute: _ => DeleteCustomer(),
            canExecute: _ => CanDeleteCustomer());
    }

    private void SaveCustomer() { /* ... */ }
    private bool CanSaveCustomer() => !string.IsNullOrEmpty(CustomerName);
}

// XAML binding
// <Button Command="{Binding SaveCommand}" Content="Save" />
```

**ADO.NET IDbCommand**
```csharp
// ADO.NET uses command pattern for database operations
using (var connection = new SqlConnection(connectionString))
{
    connection.Open();

    // Create command object (encapsulates SQL operation)
    var command = new SqlCommand("SELECT * FROM Customers WHERE Id = @Id", connection);
    command.Parameters.AddWithValue("@Id", customerId);

    // Execute command
    using (var reader = command.ExecuteReader())
    {
        while (reader.Read())
        {
            Console.WriteLine(reader["Name"]);
        }
    }
}
```

## 5. Modern Approach: Dependency Injection and MediatR

In modern C# and ASP.NET Core applications, the Command pattern is often implemented using libraries like MediatR for CQRS (Command Query Responsibility Segregation).

### MediatR Command Pattern

```csharp
// Install: dotnet add package MediatR

// Command definition
public class CreateOrderCommand : IRequest<CreateOrderResult>
{
    public int CustomerId { get; set; }
    public List<OrderItem> Items { get; set; }
    public decimal TotalAmount { get; set; }
}

public class CreateOrderResult
{
    public int OrderId { get; set; }
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
}

// Command handler (replaces Receiver)
public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, CreateOrderResult>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IInventoryService _inventoryService;
    private readonly IPaymentService _paymentService;
    private readonly ILogger<CreateOrderCommandHandler> _logger;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        IInventoryService inventoryService,
        IPaymentService paymentService,
        ILogger<CreateOrderCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _inventoryService = inventoryService;
        _paymentService = paymentService;
        _logger = logger;
    }

    public async Task<CreateOrderResult> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating order for customer {CustomerId}", request.CustomerId);

        try
        {
            // Reserve inventory
            var reservationResult = await _inventoryService.ReserveItems(request.Items);
            if (!reservationResult.Success)
            {
                return new CreateOrderResult
                {
                    Success = false,
                    ErrorMessage = "Failed to reserve inventory"
                };
            }

            // Process payment
            var paymentResult = await _paymentService.ProcessPayment(request.TotalAmount);
            if (!paymentResult.Success)
            {
                await _inventoryService.ReleaseReservation(request.Items);
                return new CreateOrderResult
                {
                    Success = false,
                    ErrorMessage = "Payment failed"
                };
            }

            // Create order
            var order = new Order
            {
                CustomerId = request.CustomerId,
                Items = request.Items,
                TotalAmount = request.TotalAmount,
                CreatedAt = DateTime.UtcNow
            };

            await _orderRepository.AddAsync(order);

            _logger.LogInformation("Order {OrderId} created successfully", order.Id);

            return new CreateOrderResult
            {
                OrderId = order.Id,
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order");
            throw;
        }
    }
}

// Register MediatR in Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

var app = builder.Build();

// Controller using MediatR (Invoker)
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
    {
        // MediatR acts as the invoker
        var result = await _mediator.Send(command);

        return result.Success
            ? Ok(result)
            : BadRequest(result.ErrorMessage);
    }
}
```

### Pipeline Behaviors (Middleware for Commands)

```csharp
// Logging behavior - executes before/after every command
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        _logger.LogInformation("Handling {RequestName}", requestName);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var response = await next();

            _logger.LogInformation(
                "Handled {RequestName} in {ElapsedMs}ms",
                requestName,
                stopwatch.ElapsedMilliseconds);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling {RequestName}", requestName);
            throw;
        }
    }
}

// Validation behavior
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Count != 0)
            {
                throw new ValidationException(failures);
            }
        }

        return await next();
    }
}

// Register behaviors
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
```

### Background Job Queue with Commands

```csharp
// Background command queue service
public interface IBackgroundCommandQueue
{
    void QueueCommand(ICommand command);
    Task<ICommand> DequeueAsync(CancellationToken cancellationToken);
}

public class BackgroundCommandQueue : IBackgroundCommandQueue
{
    private readonly Channel<ICommand> _queue;

    public BackgroundCommandQueue(int capacity)
    {
        var options = new BoundedChannelOptions(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait
        };
        _queue = Channel.CreateBounded<ICommand>(options);
    }

    public void QueueCommand(ICommand command)
    {
        if (!_queue.Writer.TryWrite(command))
        {
            throw new InvalidOperationException("Command queue is full");
        }
    }

    public async Task<ICommand> DequeueAsync(CancellationToken cancellationToken)
    {
        return await _queue.Reader.ReadAsync(cancellationToken);
    }
}

// Background service that processes commands
public class CommandProcessorService : BackgroundService
{
    private readonly IBackgroundCommandQueue _commandQueue;
    private readonly ILogger<CommandProcessorService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public CommandProcessorService(
        IBackgroundCommandQueue commandQueue,
        ILogger<CommandProcessorService> logger,
        IServiceProvider serviceProvider)
    {
        _commandQueue = commandQueue;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Command Processor Service is starting");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var command = await _commandQueue.DequeueAsync(stoppingToken);

                _logger.LogInformation("Processing command: {CommandType}", command.GetType().Name);

                // Create scope for scoped dependencies
                using (var scope = _serviceProvider.CreateScope())
                {
                    command.Execute();
                }

                _logger.LogInformation("Command processed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing command");
            }
        }

        _logger.LogInformation("Command Processor Service is stopping");
    }
}

// Register services
builder.Services.AddSingleton<IBackgroundCommandQueue>(new BackgroundCommandQueue(100));
builder.Services.AddHostedService<CommandProcessorService>();

// Usage in controller
[HttpPost("queue-report")]
public IActionResult QueueReportGeneration([FromServices] IBackgroundCommandQueue queue)
{
    var command = new GenerateReportCommand(reportService, "Sales");
    queue.QueueCommand(command);
    return Accepted("Report generation queued");
}
```

**Benefits of Modern DI Approach:**
- **Testability**: Easy to mock dependencies and test handlers independently
- **Separation of Concerns**: Commands are pure data, handlers contain logic
- **Pipeline Behaviors**: Cross-cutting concerns (logging, validation) applied declaratively
- **Async Support**: Native async/await support
- **Type Safety**: Strongly typed requests and responses
- **Dependency Injection**: Automatic resolution of dependencies

## 6. Expert Advice: When to Use vs When Not to Use

### ✅ When to Use Command:

- **Undo/Redo Functionality**: Need to reverse operations
  ```csharp
  // Good: Text editor, graphics editor, CAD software
  var editor = new TextEditor();
  editor.ExecuteCommand(new InsertTextCommand(doc, 0, "Hello"));
  editor.Undo(); // Easy to implement with command pattern
  ```

- **Queuing Operations**: Need to queue, schedule, or batch operations
  ```csharp
  // Good: Task scheduler, background job processor
  var scheduler = new TaskScheduler();
  scheduler.ScheduleCommand(new SendEmailCommand(...));
  scheduler.ScheduleCommand(new GenerateReportCommand(...));
  scheduler.ExecuteAll();
  ```

- **Logging/Audit Trail**: Need to track all operations performed
  ```csharp
  // Good: Audit systems, compliance requirements
  public class AuditingInvoker
  {
      public void Execute(ICommand command)
      {
          _logger.Log($"Executing: {command.Description} at {DateTime.Now}");
          command.Execute();
          _auditRepository.Save(new AuditEntry(command));
      }
  }
  ```

- **Parameterizing Objects with Actions**: UI elements configured with different actions
  ```csharp
  // Good: Button commands in WPF, menu items
  var saveButton = new Button { Command = new SaveCommand(document) };
  var printButton = new Button { Command = new PrintCommand(document) };
  ```

- **CQRS Architecture**: Separating commands (writes) from queries (reads)
  ```csharp
  // Good: Complex business applications
  await _mediator.Send(new CreateOrderCommand(...));  // Write operation
  var order = await _mediator.Send(new GetOrderQuery(id)); // Read operation
  ```

- **Macro/Composite Operations**: Combining multiple operations into one
  ```csharp
  // Good: Batch operations, workflows
  var macro = new MacroCommand("Prepare Report");
  macro.Add(new FetchDataCommand());
  macro.Add(new AnalyzeDataCommand());
  macro.Add(new GenerateReportCommand());
  macro.Add(new SendEmailCommand());
  macro.Execute(); // Execute entire workflow
  ```

### ❌ When NOT to Use Command:

- **Simple Operations**: Operation is straightforward and doesn't need encapsulation
  ```csharp
  // Bad: Overkill for simple method call
  public class PrintCommand : ICommand
  {
      public void Execute() => Console.WriteLine("Hello");
  }

  // Better: Just call the method directly
  Console.WriteLine("Hello");
  ```

- **No Need for Deferred Execution**: Operations are always executed immediately
  ```csharp
  // Bad: Unnecessary abstraction
  invoker.Execute(new CalculateCommand(5, 3));

  // Better: Direct calculation
  var result = calculator.Add(5, 3);
  ```

- **Single Use Case**: Command will only be used once, in one place
  ```csharp
  // Bad: Creating command for single use
  public class OneTimeCommand : ICommand { ... }

  // Better: Use Action delegate or direct method call
  Action action = () => DoSomething();
  ```

- **Performance Critical Code**: Overhead of command objects matters
  ```csharp
  // Bad: In tight loop where allocation overhead matters
  for (int i = 0; i < 1000000; i++)
  {
      var cmd = new IncrementCommand(counter);
      cmd.Execute();
  }

  // Better: Direct call
  for (int i = 0; i < 1000000; i++)
  {
      counter.Increment();
  }
  ```

### 🎯 Expert Recommendations:

**1. Use MediatR for CQRS**
```csharp
// Good: Modern C# applications with CQRS
public class CreateOrderCommand : IRequest<OrderResult> { }
public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, OrderResult> { }

// Register in DI
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());
```

**2. Implement Undo with Memento Pattern**
```csharp
// Good: Store state for complex undo operations
public class DrawCommand : ICommand
{
    private readonly Canvas _canvas;
    private Memento _backup;

    public void Execute()
    {
        _backup = _canvas.CreateMemento(); // Save state
        _canvas.DrawShape(...);
    }

    public void Undo()
    {
        _canvas.Restore(_backup); // Restore state
    }
}
```

**3. Use Action Delegates for Simple Commands**
```csharp
// Good: Lightweight alternative for simple cases
public class Button
{
    public Action OnClick { get; set; }
}

var button = new Button
{
    OnClick = () => SaveDocument()
};

// No need for separate command classes
```

**4. Add Validation to Commands**
```csharp
// Good: Validate before execution
public class CreateOrderCommand : IRequest<OrderResult>
{
    public int CustomerId { get; set; }
    public List<OrderItem> Items { get; set; }
}

public class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.CustomerId).GreaterThan(0);
        RuleFor(x => x.Items).NotEmpty();
    }
}
```

**5. Use Pipeline Behaviors for Cross-Cutting Concerns**
```csharp
// Good: Apply logging, validation, caching to all commands
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
```

**6. Make Commands Immutable**
```csharp
// Good: Commands as immutable data objects
public record CreateOrderCommand(
    int CustomerId,
    IReadOnlyList<OrderItem> Items,
    decimal TotalAmount) : IRequest<OrderResult>;

// Thread-safe, can be safely queued or logged
```

**7. Separate Command from Command Handler**
```csharp
// Good: Command is pure data, handler contains logic
public class CreateOrderCommand : IRequest<OrderResult>
{
    // Just data, no behavior
    public int CustomerId { get; init; }
    public List<OrderItem> Items { get; init; }
}

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, OrderResult>
{
    // All behavior in handler
    public async Task<OrderResult> Handle(CreateOrderCommand request, ...)
    {
        // Business logic here
    }
}
```

**8. Use Async Commands for I/O Operations**
```csharp
// Good: Async for database, API calls
public class SaveDocumentCommand : IRequest<SaveResult>
{
    public Document Document { get; init; }
}

public class SaveDocumentHandler : IRequestHandler<SaveDocumentCommand, SaveResult>
{
    public async Task<SaveResult> Handle(SaveDocumentCommand request, CancellationToken ct)
    {
        await _repository.SaveAsync(request.Document, ct);
        return new SaveResult { Success = true };
    }
}
```

**Real-World Decision Flow:**
1. Need undo/redo? → Use Command pattern with state storage
2. CQRS architecture? → Use MediatR with IRequest/IRequestHandler
3. Queueing operations? → Use Command pattern with background queue
4. Simple one-off operation? → Use Action delegate or direct call
5. UI button actions? → Use ICommand (WPF) or RelayCommand
6. Need to log all operations? → Use Command pattern with logging invoker
7. Macro/workflow orchestration? → Use Macro Command (composite)

The Command pattern excels at decoupling operation invocation from execution and enabling advanced features like undo, queuing, and logging. Use it when you need these capabilities, but prefer simpler alternatives (Action delegates, direct method calls) for straightforward scenarios. In modern C#, consider MediatR for a production-ready implementation.
