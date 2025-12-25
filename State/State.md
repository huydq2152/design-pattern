# State Pattern

## 1. Definition

The State pattern allows an object to alter its behavior when its internal state changes. The object will appear to change its class. This pattern encapsulates state-specific behavior and delegates requests to the current state object, eliminating complex conditional logic and promoting cleaner, more maintainable code.

**How it works:**
- **Create a State interface or abstract class** that defines methods for handling operations
- **Implement concrete state classes** for each possible state with state-specific behavior
- **The Context maintains a reference** to the current state object
- **Delegate operations to the current state**, which performs state-specific behavior and may trigger state transitions

The key principle is **state-based polymorphism**: instead of using if/switch statements to vary behavior based on state, the context delegates to the current state object, and each state class implements behavior specific to that state.

## 2. Pros

- **Eliminates Conditional Logic**: Replaces complex if/switch statements with polymorphism
- **Single Responsibility**: Each state class handles only one state's behavior
- **Open/Closed Principle**: Can add new states without modifying existing code
- **Explicit State Transitions**: State changes are clear and traceable
- **State-Specific Behavior**: Each state encapsulates its own logic
- **Easier Testing**: Each state can be tested independently
- **Better Organization**: State-related code is localized in state classes
- **Maintainability**: Changes to one state don't affect others
- **Cleaner Context**: Context class remains simple, just delegates to states

## 3. Cons

- **More Classes**: Each state becomes a separate class (class explosion)
- **State Object Creation**: Creating new state instances on transitions (unless using Flyweight)
- **Complexity for Simple Cases**: Overkill if only 2-3 states with minimal logic
- **Shared State Access**: States may need access to context's data
- **Learning Curve**: Pattern may be less intuitive than simple conditionals
- **Transition Management**: Need to manage state transitions carefully
- **Debugging Complexity**: Behavior is spread across multiple classes
- **Memory Overhead**: Multiple state objects in memory

## 4. Real-world Use Cases in C# & .NET

### Document Workflow State Machine

```csharp
// Without State - Complex conditional logic
public class Document
{
    public enum Status { Draft, Review, Approved, Published }
    private Status _status = Status.Draft;
    private string _content;

    public void Publish()
    {
        if (_status == Status.Draft)
        {
            throw new InvalidOperationException("Cannot publish draft");
        }
        else if (_status == Status.Review)
        {
            throw new InvalidOperationException("Must be approved first");
        }
        else if (_status == Status.Approved)
        {
            _status = Status.Published;
            // Publish logic
        }
        else if (_status == Status.Published)
        {
            throw new InvalidOperationException("Already published");
        }
    }

    public void Edit(string content)
    {
        if (_status == Status.Published)
        {
            throw new InvalidOperationException("Cannot edit published document");
        }
        _content = content;
    }

    // More methods with complex conditionals...
}

// With State - Clean polymorphic design
public interface IDocumentState
{
    void Publish(Document doc);
    void Edit(Document doc, string content);
    void Submit(Document doc);
    void Approve(Document doc);
    void Reject(Document doc);
}

public class Document
{
    private IDocumentState _state;
    private string _content;
    private string _author;

    public Document(string author)
    {
        _author = author;
        _state = new DraftState();
    }

    public void SetState(IDocumentState state)
    {
        _state = state;
        Console.WriteLine($"Document state: {state.GetType().Name}");
    }

    public void Publish() => _state.Publish(this);
    public void Edit(string content) => _state.Edit(this, content);
    public void Submit() => _state.Submit(this);
    public void Approve() => _state.Approve(this);
    public void Reject() => _state.Reject(this);

    public void SetContent(string content) => _content = content;
    public string GetContent() => _content;
}

public class DraftState : IDocumentState
{
    public void Publish(Document doc)
    {
        throw new InvalidOperationException("Cannot publish draft directly");
    }

    public void Edit(Document doc, string content)
    {
        doc.SetContent(content);
        Console.WriteLine("Draft edited");
    }

    public void Submit(Document doc)
    {
        Console.WriteLine("Submitting draft for review");
        doc.SetState(new ReviewState());
    }

    public void Approve(Document doc)
    {
        throw new InvalidOperationException("Cannot approve draft");
    }

    public void Reject(Document doc)
    {
        throw new InvalidOperationException("Cannot reject draft");
    }
}

public class ReviewState : IDocumentState
{
    public void Publish(Document doc)
    {
        throw new InvalidOperationException("Must be approved first");
    }

    public void Edit(Document doc, string content)
    {
        throw new InvalidOperationException("Cannot edit during review");
    }

    public void Submit(Document doc)
    {
        throw new InvalidOperationException("Already in review");
    }

    public void Approve(Document doc)
    {
        Console.WriteLine("Document approved");
        doc.SetState(new ApprovedState());
    }

    public void Reject(Document doc)
    {
        Console.WriteLine("Document rejected, back to draft");
        doc.SetState(new DraftState());
    }
}

public class ApprovedState : IDocumentState
{
    public void Publish(Document doc)
    {
        Console.WriteLine("Publishing document");
        doc.SetState(new PublishedState());
    }

    public void Edit(Document doc, string content)
    {
        throw new InvalidOperationException("Cannot edit approved document");
    }

    public void Submit(Document doc)
    {
        throw new InvalidOperationException("Already approved");
    }

    public void Approve(Document doc)
    {
        throw new InvalidOperationException("Already approved");
    }

    public void Reject(Document doc)
    {
        Console.WriteLine("Approval revoked, back to draft");
        doc.SetState(new DraftState());
    }
}

public class PublishedState : IDocumentState
{
    public void Publish(Document doc)
    {
        Console.WriteLine("Already published");
    }

    public void Edit(Document doc, string content)
    {
        throw new InvalidOperationException("Cannot edit published document");
    }

    public void Submit(Document doc)
    {
        throw new InvalidOperationException("Already published");
    }

    public void Approve(Document doc)
    {
        throw new InvalidOperationException("Already published");
    }

    public void Reject(Document doc)
    {
        throw new InvalidOperationException("Cannot reject published document");
    }
}

// Usage
var doc = new Document("Alice");
doc.Edit("Initial content");
doc.Submit();       // Draft -> Review
doc.Approve();      // Review -> Approved
doc.Publish();      // Approved -> Published
```

### TCP Connection State Machine

```csharp
public interface ITcpState
{
    void Open(TcpConnection connection);
    void Close(TcpConnection connection);
    void Acknowledge(TcpConnection connection);
}

public class TcpConnection
{
    private ITcpState _state;
    private string _localAddress;
    private string _remoteAddress;

    public TcpConnection(string localAddress, string remoteAddress)
    {
        _localAddress = localAddress;
        _remoteAddress = remoteAddress;
        _state = new TcpClosed();
    }

    public void SetState(ITcpState state)
    {
        _state = state;
        Console.WriteLine($"TCP State: {state.GetType().Name}");
    }

    public void Open() => _state.Open(this);
    public void Close() => _state.Close(this);
    public void Acknowledge() => _state.Acknowledge(this);

    public void SendData(string data)
    {
        Console.WriteLine($"Sending: {data}");
    }
}

public class TcpClosed : ITcpState
{
    public void Open(TcpConnection connection)
    {
        Console.WriteLine("Opening connection");
        connection.SetState(new TcpListen());
    }

    public void Close(TcpConnection connection)
    {
        Console.WriteLine("Already closed");
    }

    public void Acknowledge(TcpConnection connection)
    {
        throw new InvalidOperationException("No connection to acknowledge");
    }
}

public class TcpListen : ITcpState
{
    public void Open(TcpConnection connection)
    {
        Console.WriteLine("Waiting for SYN...");
        connection.SetState(new TcpEstablished());
    }

    public void Close(TcpConnection connection)
    {
        Console.WriteLine("Closing connection");
        connection.SetState(new TcpClosed());
    }

    public void Acknowledge(TcpConnection connection)
    {
        throw new InvalidOperationException("Not yet established");
    }
}

public class TcpEstablished : ITcpState
{
    public void Open(TcpConnection connection)
    {
        Console.WriteLine("Connection already established");
    }

    public void Close(TcpConnection connection)
    {
        Console.WriteLine("Closing established connection");
        connection.SetState(new TcpClosed());
    }

    public void Acknowledge(TcpConnection connection)
    {
        Console.WriteLine("ACK received");
        connection.SendData("Application data");
    }
}

// Usage
var tcp = new TcpConnection("192.168.1.1", "192.168.1.2");
tcp.Open();         // Closed -> Listen
tcp.Open();         // Listen -> Established
tcp.Acknowledge();  // Send data
tcp.Close();        // Established -> Closed
```

### Media Player State Machine

```csharp
public interface IPlayerState
{
    void Play(MediaPlayer player);
    void Pause(MediaPlayer player);
    void Stop(MediaPlayer player);
}

public class MediaPlayer
{
    private IPlayerState _state;
    private string _currentTrack;

    public MediaPlayer()
    {
        _state = new StoppedState();
    }

    public void SetState(IPlayerState state)
    {
        _state = state;
        Console.WriteLine($"Player state: {state.GetType().Name}");
    }

    public void Play() => _state.Play(this);
    public void Pause() => _state.Pause(this);
    public void Stop() => _state.Stop(this);

    public void LoadTrack(string track)
    {
        _currentTrack = track;
        Console.WriteLine($"Loaded: {track}");
    }

    public void StartPlayback()
    {
        Console.WriteLine($"Playing: {_currentTrack}");
    }

    public void StopPlayback()
    {
        Console.WriteLine("Playback stopped");
    }
}

public class StoppedState : IPlayerState
{
    public void Play(MediaPlayer player)
    {
        player.StartPlayback();
        player.SetState(new PlayingState());
    }

    public void Pause(MediaPlayer player)
    {
        Console.WriteLine("Cannot pause when stopped");
    }

    public void Stop(MediaPlayer player)
    {
        Console.WriteLine("Already stopped");
    }
}

public class PlayingState : IPlayerState
{
    public void Play(MediaPlayer player)
    {
        Console.WriteLine("Already playing");
    }

    public void Pause(MediaPlayer player)
    {
        Console.WriteLine("Pausing playback");
        player.SetState(new PausedState());
    }

    public void Stop(MediaPlayer player)
    {
        player.StopPlayback();
        player.SetState(new StoppedState());
    }
}

public class PausedState : IPlayerState
{
    public void Play(MediaPlayer player)
    {
        Console.WriteLine("Resuming playback");
        player.SetState(new PlayingState());
    }

    public void Pause(MediaPlayer player)
    {
        Console.WriteLine("Already paused");
    }

    public void Stop(MediaPlayer player)
    {
        player.StopPlayback();
        player.SetState(new StoppedState());
    }
}

// Usage
var player = new MediaPlayer();
player.LoadTrack("Song.mp3");
player.Play();   // Stopped -> Playing
player.Pause();  // Playing -> Paused
player.Play();   // Paused -> Playing
player.Stop();   // Playing -> Stopped
```

### .NET Framework Examples

**Entity Framework - Entity State Tracking**
```csharp
// Entity Framework uses state pattern for entity tracking
using (var context = new AppDbContext())
{
    var customer = new Customer { Name = "Alice" };

    // Entity states: Added, Unchanged, Modified, Deleted, Detached
    context.Customers.Add(customer);
    Console.WriteLine(context.Entry(customer).State); // Added

    context.SaveChanges();
    Console.WriteLine(context.Entry(customer).State); // Unchanged

    customer.Name = "Bob";
    Console.WriteLine(context.Entry(customer).State); // Modified

    context.Customers.Remove(customer);
    Console.WriteLine(context.Entry(customer).State); // Deleted
}
```

**SignalR Connection States**
```csharp
// SignalR connection lifecycle uses state pattern
public class ChatHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        // Connection state: Connecting -> Connected
        Console.WriteLine($"Client connected: {Context.ConnectionId}");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        // Connection state: Connected -> Disconnected
        Console.WriteLine($"Client disconnected: {Context.ConnectionId}");
        await base.OnDisconnectedAsync(exception);
    }
}

// Client-side connection states
var connection = new HubConnectionBuilder()
    .WithUrl("https://example.com/chatHub")
    .Build();

connection.Closed += async (error) =>
{
    // Reconnection logic based on state
    await Task.Delay(5000);
    await connection.StartAsync();
};

// States: Disconnected -> Connecting -> Connected -> Reconnecting -> Disconnected
```

**ASP.NET Core Request Pipeline States**
```csharp
// Request processing goes through different states
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Request state: Received
        Console.WriteLine("Request received");

        // Request state: Processing
        await _next(context);

        // Request state: Completed
        Console.WriteLine($"Response: {context.Response.StatusCode}");
    }
}
```

### Order Processing State Machine

```csharp
public interface IOrderState
{
    void Submit(Order order);
    void Cancel(Order order);
    void Pay(Order order);
    void Ship(Order order);
    void Deliver(Order order);
}

public class Order
{
    private IOrderState _state;
    private List<OrderItem> _items;
    private decimal _totalAmount;

    public Order()
    {
        _state = new PendingState();
        _items = new List<OrderItem>();
    }

    public void SetState(IOrderState state)
    {
        _state = state;
        Console.WriteLine($"Order state: {state.GetType().Name}");
    }

    public void Submit() => _state.Submit(this);
    public void Cancel() => _state.Cancel(this);
    public void Pay() => _state.Pay(this);
    public void Ship() => _state.Ship(this);
    public void Deliver() => _state.Deliver(this);
}

public class PendingState : IOrderState
{
    public void Submit(Order order)
    {
        Console.WriteLine("Order submitted for validation");
        order.SetState(new ValidatedState());
    }

    public void Cancel(Order order)
    {
        Console.WriteLine("Order cancelled");
        order.SetState(new CancelledState());
    }

    public void Pay(Order order) =>
        throw new InvalidOperationException("Must submit first");

    public void Ship(Order order) =>
        throw new InvalidOperationException("Must pay first");

    public void Deliver(Order order) =>
        throw new InvalidOperationException("Must ship first");
}

public class ValidatedState : IOrderState
{
    public void Submit(Order order) =>
        Console.WriteLine("Already submitted");

    public void Cancel(Order order)
    {
        Console.WriteLine("Order cancelled");
        order.SetState(new CancelledState());
    }

    public void Pay(Order order)
    {
        Console.WriteLine("Payment processed");
        order.SetState(new PaidState());
    }

    public void Ship(Order order) =>
        throw new InvalidOperationException("Must pay first");

    public void Deliver(Order order) =>
        throw new InvalidOperationException("Must ship first");
}

public class PaidState : IOrderState
{
    public void Submit(Order order) =>
        Console.WriteLine("Already submitted");

    public void Cancel(Order order)
    {
        Console.WriteLine("Refunding payment");
        order.SetState(new CancelledState());
    }

    public void Pay(Order order) =>
        Console.WriteLine("Already paid");

    public void Ship(Order order)
    {
        Console.WriteLine("Order shipped");
        order.SetState(new ShippedState());
    }

    public void Deliver(Order order) =>
        throw new InvalidOperationException("Must ship first");
}

public class ShippedState : IOrderState
{
    public void Submit(Order order) =>
        Console.WriteLine("Already submitted");

    public void Cancel(Order order) =>
        throw new InvalidOperationException("Cannot cancel shipped order");

    public void Pay(Order order) =>
        Console.WriteLine("Already paid");

    public void Ship(Order order) =>
        Console.WriteLine("Already shipped");

    public void Deliver(Order order)
    {
        Console.WriteLine("Order delivered");
        order.SetState(new DeliveredState());
    }
}

public class DeliveredState : IOrderState
{
    public void Submit(Order order) =>
        Console.WriteLine("Already delivered");

    public void Cancel(Order order) =>
        throw new InvalidOperationException("Cannot cancel delivered order");

    public void Pay(Order order) =>
        Console.WriteLine("Already paid");

    public void Ship(Order order) =>
        Console.WriteLine("Already shipped");

    public void Deliver(Order order) =>
        Console.WriteLine("Already delivered");
}

public class CancelledState : IOrderState
{
    public void Submit(Order order) =>
        throw new InvalidOperationException("Order is cancelled");

    public void Cancel(Order order) =>
        Console.WriteLine("Already cancelled");

    public void Pay(Order order) =>
        throw new InvalidOperationException("Order is cancelled");

    public void Ship(Order order) =>
        throw new InvalidOperationException("Order is cancelled");

    public void Deliver(Order order) =>
        throw new InvalidOperationException("Order is cancelled");
}

// Usage
var order = new Order();
order.Submit();   // Pending -> Validated
order.Pay();      // Validated -> Paid
order.Ship();     // Paid -> Shipped
order.Deliver();  // Shipped -> Delivered
```

## 5. Modern Approach: Dependency Injection

In modern C# and ASP.NET Core applications, the State pattern can be integrated with dependency injection for better testability and flexibility.

```csharp
// State interface
public interface IOrderState
{
    Task ProcessAsync(OrderContext context);
    Task CancelAsync(OrderContext context);
    Task CompleteAsync(OrderContext context);
}

// Context with DI support
public class OrderContext
{
    private IOrderState _currentState;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OrderContext> _logger;
    public int OrderId { get; }
    public decimal Amount { get; set; }
    public List<string> Items { get; set; }

    public OrderContext(
        int orderId,
        IServiceProvider serviceProvider,
        ILogger<OrderContext> logger)
    {
        OrderId = orderId;
        _serviceProvider = serviceProvider;
        _logger = logger;
        Items = new List<string>();

        // Start in pending state
        _currentState = _serviceProvider.GetRequiredService<PendingOrderState>();
    }

    public void SetState<TState>() where TState : IOrderState
    {
        _currentState = _serviceProvider.GetRequiredService<TState>();
        _logger.LogInformation("Order {OrderId} transitioned to {State}",
            OrderId, typeof(TState).Name);
    }

    public async Task ProcessAsync() => await _currentState.ProcessAsync(this);
    public async Task CancelAsync() => await _currentState.CancelAsync(this);
    public async Task CompleteAsync() => await _currentState.CompleteAsync(this);
}

// Concrete states with dependencies
public class PendingOrderState : IOrderState
{
    private readonly IInventoryService _inventory;
    private readonly ILogger<PendingOrderState> _logger;

    public PendingOrderState(
        IInventoryService inventory,
        ILogger<PendingOrderState> logger)
    {
        _inventory = inventory;
        _logger = logger;
    }

    public async Task ProcessAsync(OrderContext context)
    {
        _logger.LogInformation("Validating order {OrderId}", context.OrderId);

        // Check inventory with injected service
        var available = await _inventory.CheckAvailabilityAsync(context.Items);

        if (available)
        {
            context.SetState<ProcessingOrderState>();
        }
        else
        {
            throw new InvalidOperationException("Items not available");
        }
    }

    public async Task CancelAsync(OrderContext context)
    {
        _logger.LogInformation("Cancelling pending order {OrderId}", context.OrderId);
        context.SetState<CancelledOrderState>();
        await Task.CompletedTask;
    }

    public Task CompleteAsync(OrderContext context)
    {
        throw new InvalidOperationException("Cannot complete pending order");
    }
}

public class ProcessingOrderState : IOrderState
{
    private readonly IPaymentService _payment;
    private readonly ILogger<ProcessingOrderState> _logger;

    public ProcessingOrderState(
        IPaymentService payment,
        ILogger<ProcessingOrderState> logger)
    {
        _payment = payment;
        _logger = logger;
    }

    public async Task ProcessAsync(OrderContext context)
    {
        _logger.LogInformation("Processing payment for order {OrderId}", context.OrderId);

        var paymentResult = await _payment.ProcessPaymentAsync(context.Amount);

        if (paymentResult.Success)
        {
            context.SetState<CompletedOrderState>();
        }
        else
        {
            context.SetState<FailedOrderState>();
        }
    }

    public async Task CancelAsync(OrderContext context)
    {
        _logger.LogInformation("Cancelling processing order {OrderId}", context.OrderId);
        context.SetState<CancelledOrderState>();
        await Task.CompletedTask;
    }

    public Task CompleteAsync(OrderContext context)
    {
        throw new InvalidOperationException("Order is still processing");
    }
}

public class CompletedOrderState : IOrderState
{
    private readonly INotificationService _notification;
    private readonly ILogger<CompletedOrderState> _logger;

    public CompletedOrderState(
        INotificationService notification,
        ILogger<CompletedOrderState> logger)
    {
        _notification = notification;
        _logger = logger;
    }

    public Task ProcessAsync(OrderContext context)
    {
        _logger.LogInformation("Order {OrderId} already completed", context.OrderId);
        return Task.CompletedTask;
    }

    public Task CancelAsync(OrderContext context)
    {
        throw new InvalidOperationException("Cannot cancel completed order");
    }

    public async Task CompleteAsync(OrderContext context)
    {
        await _notification.SendOrderConfirmationAsync(context.OrderId);
        _logger.LogInformation("Confirmation sent for order {OrderId}", context.OrderId);
    }
}

public class CancelledOrderState : IOrderState
{
    private readonly ILogger<CancelledOrderState> _logger;

    public CancelledOrderState(ILogger<CancelledOrderState> logger)
    {
        _logger = logger;
    }

    public Task ProcessAsync(OrderContext context)
    {
        throw new InvalidOperationException("Cannot process cancelled order");
    }

    public Task CancelAsync(OrderContext context)
    {
        _logger.LogInformation("Order {OrderId} already cancelled", context.OrderId);
        return Task.CompletedTask;
    }

    public Task CompleteAsync(OrderContext context)
    {
        throw new InvalidOperationException("Cannot complete cancelled order");
    }
}

public class FailedOrderState : IOrderState
{
    private readonly ILogger<FailedOrderState> _logger;

    public FailedOrderState(ILogger<FailedOrderState> logger)
    {
        _logger = logger;
    }

    public Task ProcessAsync(OrderContext context)
    {
        throw new InvalidOperationException("Cannot process failed order");
    }

    public async Task CancelAsync(OrderContext context)
    {
        _logger.LogInformation("Cancelling failed order {OrderId}", context.OrderId);
        context.SetState<CancelledOrderState>();
        await Task.CompletedTask;
    }

    public Task CompleteAsync(OrderContext context)
    {
        throw new InvalidOperationException("Cannot complete failed order");
    }
}

// Register in Program.cs
var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// Register all state implementations
builder.Services.AddTransient<PendingOrderState>();
builder.Services.AddTransient<ProcessingOrderState>();
builder.Services.AddTransient<CompletedOrderState>();
builder.Services.AddTransient<CancelledOrderState>();
builder.Services.AddTransient<FailedOrderState>();

// Register order context factory
builder.Services.AddScoped<IOrderContextFactory, OrderContextFactory>();

var app = builder.Build();

// Factory for creating order contexts
public interface IOrderContextFactory
{
    OrderContext Create(int orderId);
}

public class OrderContextFactory : IOrderContextFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OrderContext> _logger;

    public OrderContextFactory(
        IServiceProvider serviceProvider,
        ILogger<OrderContext> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public OrderContext Create(int orderId)
    {
        return new OrderContext(orderId, _serviceProvider, _logger);
    }
}

// Controller using state pattern with DI
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderContextFactory _orderFactory;

    public OrdersController(IOrderContextFactory orderFactory)
    {
        _orderFactory = orderFactory;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var orderContext = _orderFactory.Create(request.OrderId);
        orderContext.Items = request.Items;
        orderContext.Amount = request.Amount;

        await orderContext.ProcessAsync(); // Pending -> Processing
        await orderContext.ProcessAsync(); // Processing -> Completed
        await orderContext.CompleteAsync(); // Send confirmation

        return Ok(new { OrderId = orderContext.OrderId, Status = "Completed" });
    }

    [HttpDelete("{orderId}")]
    public async Task<IActionResult> CancelOrder(int orderId)
    {
        var orderContext = _orderFactory.Create(orderId);
        await orderContext.CancelAsync();

        return Ok(new { OrderId = orderId, Status = "Cancelled" });
    }
}
```

**Using Stateless Library for Complex State Machines**
```csharp
// Install: dotnet add package Stateless

using Stateless;

public class OrderStateMachine
{
    private enum State { Pending, Validated, Paid, Shipped, Delivered, Cancelled }
    private enum Trigger { Submit, Pay, Ship, Deliver, Cancel }

    private readonly StateMachine<State, Trigger> _machine;

    public OrderStateMachine()
    {
        _machine = new StateMachine<State, Trigger>(State.Pending);

        _machine.Configure(State.Pending)
            .Permit(Trigger.Submit, State.Validated)
            .Permit(Trigger.Cancel, State.Cancelled);

        _machine.Configure(State.Validated)
            .Permit(Trigger.Pay, State.Paid)
            .Permit(Trigger.Cancel, State.Cancelled);

        _machine.Configure(State.Paid)
            .Permit(Trigger.Ship, State.Shipped)
            .Permit(Trigger.Cancel, State.Cancelled);

        _machine.Configure(State.Shipped)
            .Permit(Trigger.Deliver, State.Delivered);

        _machine.Configure(State.Cancelled)
            .OnEntry(() => Console.WriteLine("Order cancelled"));

        _machine.Configure(State.Delivered)
            .OnEntry(() => Console.WriteLine("Order delivered"));
    }

    public void Submit() => _machine.Fire(Trigger.Submit);
    public void Pay() => _machine.Fire(Trigger.Pay);
    public void Ship() => _machine.Fire(Trigger.Ship);
    public void Deliver() => _machine.Fire(Trigger.Deliver);
    public void Cancel() => _machine.Fire(Trigger.Cancel);

    public string CurrentState => _machine.State.ToString();
}

// Usage
var order = new OrderStateMachine();
order.Submit();  // Pending -> Validated
order.Pay();     // Validated -> Paid
order.Ship();    // Paid -> Shipped
order.Deliver(); // Shipped -> Delivered
```

**Benefits of DI Integration:**
- **Testability**: Easy to mock state dependencies
- **Flexibility**: Can swap implementations without changing state logic
- **Logging**: Built-in logging for state transitions
- **Async Support**: States can perform async operations
- **Service Access**: States can use injected services for complex operations

## 6. Expert Advice: When to Use vs When Not to Use

### ✅ When to Use State:

- **Complex State-Dependent Behavior**: Object behavior varies significantly based on state
  ```csharp
  // Good: Document workflow with many states and transitions
  public class Document
  {
      private IDocumentState _state;
      public void Submit() => _state.Submit(this);
      public void Approve() => _state.Approve(this);
      public void Publish() => _state.Publish(this);
  }
  ```

- **Eliminating Conditional Logic**: Many if/switch statements based on state
  ```csharp
  // Bad: Complex conditionals
  if (state == "Draft") { /* ... */ }
  else if (state == "Review") { /* ... */ }
  else if (state == "Approved") { /* ... */ }

  // Good: State pattern
  _state.Handle(this);
  ```

- **State Transitions with Rules**: Clear state transition rules
  ```csharp
  // Good: Order processing with defined transitions
  // Pending -> Validated -> Paid -> Shipped -> Delivered
  public class Order
  {
      private IOrderState _state;
      // State controls valid transitions
  }
  ```

- **Growing Number of States**: States will increase over time
  ```csharp
  // Good: Adding new states doesn't affect existing states
  public class NewApprovalState : IDocumentState { }
  ```

### ❌ When NOT to Use State:

- **Simple Two-State Logic**: Only two states with minimal behavior
  ```csharp
  // Bad: Overkill for simple boolean state
  public class StateA : IState { }
  public class StateB : IState { }

  // Better: Use boolean flag
  private bool _isActive;
  if (_isActive) { /* ... */ } else { /* ... */ }
  ```

- **No State Transitions**: State never changes
  ```csharp
  // Bad: State never changes
  public class FixedState : IState { }

  // Better: Don't use state pattern
  public class SimpleClass { }
  ```

- **Minimal State-Specific Behavior**: Behavior barely differs between states
  ```csharp
  // Bad: States do almost the same thing
  public class StateA : IState
  {
      public void Handle() => Console.WriteLine("A");
  }
  public class StateB : IState
  {
      public void Handle() => Console.WriteLine("B");
  }

  // Better: Pass parameter instead
  public void Handle(string stateType) => Console.WriteLine(stateType);
  ```

- **Performance Critical**: State object creation overhead matters
  ```csharp
  // Bad: Creating state objects in tight loop
  for (int i = 0; i < 1000000; i++)
  {
      context.SetState(new NewState()); // Expensive!
  }

  // Better: Reuse state instances (Flyweight pattern)
  ```

### 🎯 Expert Recommendations:

**1. Use Singleton States for Stateless States**
```csharp
// Good: Reuse state instances if they have no data
public class DraftState : IDocumentState
{
    private static readonly DraftState _instance = new();
    public static DraftState Instance => _instance;

    private DraftState() { } // Private constructor
}

// Usage
doc.SetState(DraftState.Instance); // Reuse instance
```

**2. Validate State Transitions**
```csharp
// Good: Enforce valid transitions
public abstract class DocumentState
{
    public abstract bool CanTransitionTo(Type stateType);

    protected void TransitionTo<TState>(Document doc) where TState : DocumentState
    {
        if (!CanTransitionTo(typeof(TState)))
            throw new InvalidOperationException($"Cannot transition to {typeof(TState).Name}");

        doc.SetState(GetStateInstance<TState>());
    }
}
```

**3. Use Enums with States for Clarity**
```csharp
// Good: Combine enum with state pattern
public enum OrderStatus { Pending, Validated, Paid, Shipped }

public abstract class OrderState
{
    public abstract OrderStatus Status { get; }
}

public class PendingState : OrderState
{
    public override OrderStatus Status => OrderStatus.Pending;
}
```

**4. Log State Transitions**
```csharp
// Good: Track state changes for debugging
public void SetState(IOrderState newState)
{
    var oldState = _state?.GetType().Name ?? "None";
    _state = newState;
    _logger.LogInformation("Order {OrderId}: {OldState} -> {NewState}",
        OrderId, oldState, newState.GetType().Name);
}
```

**5. Consider State Machine Libraries**
```csharp
// Good: Use Stateless library for complex state machines
var stateMachine = new StateMachine<State, Trigger>(State.Pending);
stateMachine.Configure(State.Pending)
    .Permit(Trigger.Submit, State.Validated)
    .OnEntry(() => Console.WriteLine("Order submitted"));

// Provides: Transition validation, state diagrams, guards, async support
```

**6. Combine with Strategy Pattern**
```csharp
// Good: States can use strategies for varying algorithms
public class ProcessingState : IOrderState
{
    private readonly IPaymentStrategy _paymentStrategy;

    public ProcessingState(IPaymentStrategy paymentStrategy)
    {
        _paymentStrategy = paymentStrategy;
    }

    public async Task ProcessAsync(Order order)
    {
        await _paymentStrategy.ProcessAsync(order.Amount);
    }
}
```

**7. Handle Invalid Operations Gracefully**
```csharp
// Good: Clear error messages for invalid operations
public class PublishedState : IDocumentState
{
    public void Edit(Document doc, string content)
    {
        throw new InvalidOperationException(
            "Cannot edit published document. Create a new version instead.");
    }
}

// Better: Return result object instead of throwing
public OperationResult Edit(Document doc, string content)
{
    return OperationResult.Failure("Cannot edit published document");
}
```

**8. Document State Transitions**
```csharp
/// <summary>
/// Order state machine:
///
/// Pending --Submit--> Validated --Pay--> Paid --Ship--> Shipped --Deliver--> Delivered
///    |                   |                |
///    +------Cancel-------+-----Cancel-----+
///    |                   |                |
///    +------------------Cancelled---------+
///
/// Valid transitions:
/// - Pending: Submit, Cancel
/// - Validated: Pay, Cancel
/// - Paid: Ship, Cancel
/// - Shipped: Deliver
/// - Delivered: (final state)
/// - Cancelled: (final state)
/// </summary>
public class Order { }
```

**Real-World Decision Flow:**
1. Many if/else or switch on state variable? → Use State pattern
2. Behavior varies significantly by state? → Use State pattern
3. Need to add new states frequently? → Use State pattern
4. State transitions have complex rules? → Use State pattern
5. Only 2-3 simple states? → Don't use State pattern
6. State never changes? → Don't use State pattern
7. Complex state machine? → Use Stateless library
8. Performance critical? → Use Flyweight pattern with states

The State pattern excels at eliminating complex conditional logic and making state transitions explicit and manageable. Use it when object behavior depends significantly on state and you want to avoid if/switch statements. For simple cases, stick with conditional logic. For complex state machines, consider using a library like Stateless for production-ready state management.
