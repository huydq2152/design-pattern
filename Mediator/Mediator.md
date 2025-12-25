# Mediator Pattern

## 1. Definition

The Mediator pattern defines an object that encapsulates how a set of objects interact. It promotes loose coupling by keeping objects from referring to each other explicitly and lets you vary their interaction independently. The mediator centralizes complex communications and control logic between related objects.

**How it works:**
- **Create a Mediator interface** that defines communication methods
- **Components communicate through the mediator** instead of directly with each other
- **The mediator coordinates** component interactions and workflows
- **Components only know about the mediator**, not about other components

The key principle is **centralized coordination**: instead of components having many-to-many relationships with each other, they have one-to-one relationships with the mediator, which orchestrates all interactions.

## 2. Pros

- **Loose Coupling**: Components don't reference each other directly, reducing dependencies
- **Centralized Control**: All interaction logic is in one place, easy to understand
- **Reusability**: Components can be reused in different contexts with different mediators
- **Single Responsibility**: Components focus on their work, mediator focuses on coordination
- **Easy to Modify**: Change workflows by modifying the mediator without changing components
- **Simplified Components**: Components become simpler since they don't manage interactions
- **Clear Communication Flow**: Interaction patterns are explicit and traceable
- **Easier Testing**: Can test components in isolation by mocking the mediator

## 3. Cons

- **God Object Risk**: Mediator can become overly complex if it manages too many components
- **Single Point of Failure**: All communication depends on the mediator
- **Difficult to Reuse Mediator**: Mediators are often specific to a particular set of components
- **Performance Overhead**: Extra indirection for all component communications
- **Debugging Complexity**: Call flow is less obvious than direct component interaction
- **Mediator Can Become Rigid**: Adding new event types may require modifying the mediator
- **Increased Initial Complexity**: Requires more setup than direct component communication
- **Potential Bottleneck**: All communication goes through one object

## 4. Real-world Use Cases in C# & .NET

### Chat Application with ChatRoom Mediator

```csharp
// Without Mediator - Users directly message each other
public class User
{
    private List<User> _contacts = new();

    public void SendMessage(User recipient, string message)
    {
        recipient.ReceiveMessage(this, message);
    }

    public void ReceiveMessage(User sender, string message)
    {
        Console.WriteLine($"{Name} received: {message} from {sender.Name}");
    }

    // Must manage all connections
    public void AddContact(User user) => _contacts.Add(user);
}

// With Mediator - ChatRoom coordinates all messaging
public interface IChatMediator
{
    void SendMessage(string message, User sender);
    void RegisterUser(User user);
}

public class ChatRoom : IChatMediator
{
    private readonly List<User> _users = new();

    public void RegisterUser(User user)
    {
        _users.Add(user);
        user.SetMediator(this);
        Console.WriteLine($"{user.Name} joined the chat room");
    }

    public void SendMessage(string message, User sender)
    {
        // Mediator coordinates: send to all except sender
        foreach (var user in _users.Where(u => u != sender))
        {
            user.ReceiveMessage(sender.Name, message);
        }
    }
}

public class User
{
    public string Name { get; }
    private IChatMediator? _mediator;

    public User(string name) => Name = name;

    public void SetMediator(IChatMediator mediator)
    {
        _mediator = mediator;
    }

    public void Send(string message)
    {
        Console.WriteLine($"{Name} sends: {message}");
        _mediator?.SendMessage(message, this);
    }

    public void ReceiveMessage(string senderName, string message)
    {
        Console.WriteLine($"{Name} received from {senderName}: {message}");
    }
}

// Usage
var chatRoom = new ChatRoom();

var alice = new User("Alice");
var bob = new User("Bob");
var charlie = new User("Charlie");

chatRoom.RegisterUser(alice);
chatRoom.RegisterUser(bob);
chatRoom.RegisterUser(charlie);

alice.Send("Hello everyone!");  // Bob and Charlie receive it
bob.Send("Hi Alice!");          // Alice and Charlie receive it
```

### UI Form Dialog Mediator

```csharp
// Mediator for coordinating form controls
public interface IFormMediator
{
    void Notify(object sender, string eventName);
}

public class RegistrationFormMediator : IFormMediator
{
    private readonly TextBox _usernameTextBox;
    private readonly TextBox _passwordTextBox;
    private readonly TextBox _confirmPasswordTextBox;
    private readonly CheckBox _termsCheckBox;
    private readonly Button _submitButton;

    public RegistrationFormMediator(
        TextBox username,
        TextBox password,
        TextBox confirmPassword,
        CheckBox terms,
        Button submit)
    {
        _usernameTextBox = username;
        _passwordTextBox = password;
        _confirmPasswordTextBox = confirmPassword;
        _termsCheckBox = terms;
        _submitButton = submit;

        // Register mediator with all components
        _usernameTextBox.SetMediator(this);
        _passwordTextBox.SetMediator(this);
        _confirmPasswordTextBox.SetMediator(this);
        _termsCheckBox.SetMediator(this);
    }

    public void Notify(object sender, string eventName)
    {
        if (eventName == "TextChanged")
        {
            // When any field changes, validate the form
            ValidateForm();
        }
        else if (eventName == "TermsChecked" || eventName == "TermsUnchecked")
        {
            // When terms checkbox changes, validate
            ValidateForm();
        }
    }

    private void ValidateForm()
    {
        // Mediator coordinates validation across components
        var isUsernameValid = !string.IsNullOrWhiteSpace(_usernameTextBox.Text);
        var isPasswordValid = _passwordTextBox.Text.Length >= 8;
        var passwordsMatch = _passwordTextBox.Text == _confirmPasswordTextBox.Text;
        var termsAccepted = _termsCheckBox.IsChecked;

        // Enable submit button only if all validations pass
        _submitButton.Enabled = isUsernameValid && isPasswordValid &&
                                passwordsMatch && termsAccepted;

        // Show validation messages
        if (!passwordsMatch && !string.IsNullOrEmpty(_confirmPasswordTextBox.Text))
        {
            _confirmPasswordTextBox.ShowError("Passwords don't match");
        }
    }
}

// Form controls notify mediator
public class TextBox : FormControl
{
    public string Text { get; set; } = "";

    public void OnTextChanged()
    {
        _mediator?.Notify(this, "TextChanged");
    }
}

public class CheckBox : FormControl
{
    public bool IsChecked { get; private set; }

    public void Toggle()
    {
        IsChecked = !IsChecked;
        _mediator?.Notify(this, IsChecked ? "TermsChecked" : "TermsUnchecked");
    }
}
```

### .NET Framework Examples

**MediatR Library - CQRS Pattern**
```csharp
// MediatR is the industry-standard mediator for .NET
// Install: dotnet add package MediatR

// Command (request)
public class CreateOrderCommand : IRequest<OrderResult>
{
    public int CustomerId { get; init; }
    public List<OrderItem> Items { get; init; }
    public decimal TotalAmount { get; init; }
}

// Command Handler
public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, OrderResult>
{
    private readonly IOrderRepository _orderRepo;
    private readonly IInventoryService _inventory;
    private readonly IPaymentService _payment;

    public CreateOrderHandler(
        IOrderRepository orderRepo,
        IInventoryService inventory,
        IPaymentService payment)
    {
        _orderRepo = orderRepo;
        _inventory = inventory;
        _payment = payment;
    }

    public async Task<OrderResult> Handle(
        CreateOrderCommand request,
        CancellationToken cancellationToken)
    {
        // Mediator coordinates the workflow
        await _inventory.ReserveItems(request.Items);
        var paymentResult = await _payment.ProcessPayment(request.TotalAmount);

        if (paymentResult.Success)
        {
            var order = new Order
            {
                CustomerId = request.CustomerId,
                Items = request.Items,
                TotalAmount = request.TotalAmount
            };

            await _orderRepo.AddAsync(order);
            return OrderResult.Success(order.Id);
        }

        await _inventory.ReleaseReservation(request.Items);
        return OrderResult.Failure(paymentResult.Error);
    }
}

// Controller uses mediator
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
        // Send command through mediator
        var result = await _mediator.Send(command);
        return result.Success ? Ok(result) : BadRequest(result.Error);
    }
}
```

**SignalR Hub as Mediator**
```csharp
// SignalR Hub acts as mediator for real-time communication
public class ChatHub : Hub
{
    // Mediator method: coordinate messaging between clients
    public async Task SendMessage(string user, string message)
    {
        // Broadcast to all connected clients
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }

    public async Task SendToGroup(string groupName, string user, string message)
    {
        // Send to specific group
        await Clients.Group(groupName).SendAsync("ReceiveMessage", user, message);
    }

    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        await Clients.Group(groupName).SendAsync("UserJoined", Context.ConnectionId);
    }
}
```

**Air Traffic Control System**
```csharp
public interface IAirTrafficControl
{
    void RegisterAirplane(Airplane airplane);
    void RequestLanding(Airplane airplane);
    void RequestTakeoff(Airplane airplane);
}

public class AirportTower : IAirTrafficControl
{
    private readonly List<Airplane> _airplanes = new();
    private readonly Queue<Airplane> _landingQueue = new();
    private bool _runwayAvailable = true;

    public void RegisterAirplane(Airplane airplane)
    {
        _airplanes.Add(airplane);
        airplane.SetAirTrafficControl(this);
        Console.WriteLine($"Tower: {airplane.CallSign} registered");
    }

    public void RequestLanding(Airplane airplane)
    {
        Console.WriteLine($"Tower: {airplane.CallSign} requests landing");

        if (_runwayAvailable)
        {
            _runwayAvailable = false;
            airplane.GrantedLanding();
        }
        else
        {
            _landingQueue.Enqueue(airplane);
            airplane.HoldingPattern();
        }
    }

    public void RequestTakeoff(Airplane airplane)
    {
        Console.WriteLine($"Tower: {airplane.CallSign} requests takeoff");

        if (_runwayAvailable)
        {
            _runwayAvailable = false;
            airplane.GrantedTakeoff();
        }
    }

    public void RunwayClear()
    {
        _runwayAvailable = true;

        // Process next in landing queue
        if (_landingQueue.Count > 0)
        {
            var next = _landingQueue.Dequeue();
            RequestLanding(next);
        }
    }
}

public class Airplane
{
    public string CallSign { get; }
    private IAirTrafficControl? _tower;

    public Airplane(string callSign) => CallSign = callSign;

    public void SetAirTrafficControl(IAirTrafficControl tower)
    {
        _tower = tower;
    }

    public void RequestLanding()
    {
        _tower?.RequestLanding(this);
    }

    public void GrantedLanding()
    {
        Console.WriteLine($"{CallSign}: Landing cleared, coming in");
    }

    public void HoldingPattern()
    {
        Console.WriteLine($"{CallSign}: Entering holding pattern");
    }
}
```

## 5. Modern Approach: Dependency Injection

In modern C# and ASP.NET Core applications, mediators are typically integrated with dependency injection for better testability and flexibility.

```csharp
// Define mediator interface
public interface IOrderWorkflowMediator
{
    Task<OrderResult> ProcessOrderAsync(CreateOrderRequest request);
    Task<OrderResult> CancelOrderAsync(int orderId);
}

// Implement mediator
public class OrderWorkflowMediator : IOrderWorkflowMediator
{
    private readonly IInventoryService _inventory;
    private readonly IPaymentService _payment;
    private readonly IShippingService _shipping;
    private readonly INotificationService _notification;
    private readonly ILogger<OrderWorkflowMediator> _logger;

    public OrderWorkflowMediator(
        IInventoryService inventory,
        IPaymentService payment,
        IShippingService shipping,
        INotificationService notification,
        ILogger<OrderWorkflowMediator> logger)
    {
        _inventory = inventory;
        _payment = payment;
        _shipping = shipping;
        _notification = notification;
        _logger = logger;
    }

    public async Task<OrderResult> ProcessOrderAsync(CreateOrderRequest request)
    {
        _logger.LogInformation("Processing order for customer {CustomerId}", request.CustomerId);

        try
        {
            // Mediator coordinates the workflow
            var reservationResult = await _inventory.ReserveItemsAsync(request.Items);
            if (!reservationResult.Success)
            {
                return OrderResult.Failure("Insufficient inventory");
            }

            var paymentResult = await _payment.ProcessPaymentAsync(request.Payment);
            if (!paymentResult.Success)
            {
                await _inventory.ReleaseReservationAsync(request.Items);
                return OrderResult.Failure("Payment failed");
            }

            await _inventory.DeductItemsAsync(request.Items);
            var shipmentResult = await _shipping.CreateShipmentAsync(request);
            await _notification.SendOrderConfirmationAsync(request.CustomerId);

            _logger.LogInformation("Order processed successfully");
            return OrderResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing order");
            throw;
        }
    }

    public async Task<OrderResult> CancelOrderAsync(int orderId)
    {
        // Coordinate cancellation workflow
        // ...
        return OrderResult.Success();
    }
}

// Register in Program.cs
var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IShippingService, ShippingService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// Register mediator
builder.Services.AddScoped<IOrderWorkflowMediator, OrderWorkflowMediator>();

// Or use MediatR library
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblyContaining<Program>());

var app = builder.Build();

// Controller uses mediator
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderWorkflowMediator _mediator;

    public OrdersController(IOrderWorkflowMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var result = await _mediator.ProcessOrderAsync(request);
        return result.Success ? Ok(result) : BadRequest(result.Error);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> CancelOrder(int id)
    {
        var result = await _mediator.CancelOrderAsync(id);
        return result.Success ? Ok(result) : BadRequest(result.Error);
    }
}
```

**Using MediatR with Pipeline Behaviors**
```csharp
// Pipeline behavior for logging (applied to all requests)
public class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
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
        _logger.LogInformation("Handling {RequestName}", typeof(TRequest).Name);
        var response = await next();
        _logger.LogInformation("Handled {RequestName}", typeof(TRequest).Name);
        return response;
    }
}

// Register pipeline behaviors
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
```

**Benefits of DI Integration:**
- **Testability**: Easy to mock mediator or its dependencies
- **Flexibility**: Can swap implementations without changing client code
- **Lifetime Management**: Container manages object creation and disposal
- **Cross-Cutting Concerns**: Pipeline behaviors for logging, validation, caching
- **Type Safety**: Strongly-typed requests and responses

## 6. Expert Advice: When to Use vs When Not to Use

### ✅ When to Use Mediator:

- **Complex Component Interactions**: Multiple components need to interact in complex ways
  ```csharp
  // Good: UI form with many interdependent controls
  var mediator = new FormMediator(textBox, dropdown, checkbox, submitButton);
  // Mediator coordinates: when dropdown changes, update textbox options,
  // when checkbox clicked, enable/disable submit, etc.
  ```

- **Reducing Many-to-Many Relationships**: Components have many direct connections
  ```csharp
  // Good: Chat application with many users
  var chatRoom = new ChatRoomMediator();
  // Without mediator: Each user would need references to all other users
  // With mediator: Users only know about the chat room
  ```

- **Workflow Orchestration**: Need to coordinate a multi-step process
  ```csharp
  // Good: Order processing workflow
  await _mediator.ProcessOrderAsync(order);
  // Coordinates: validation -> payment -> inventory -> shipping -> notification
  ```

- **Centralizing Communication Logic**: Want all interaction rules in one place
  ```csharp
  // Good: Air traffic control
  var tower = new AirportTower();
  // All landing/takeoff coordination logic centralized
  ```

- **Reusable Components**: Components should work in different contexts
  ```csharp
  // Good: Widget library where widgets work in different parent containers
  var widget = new DatePicker();
  // Can work with different form mediators
  ```

### ❌ When NOT to Use Mediator:

- **Simple Two-Component Interaction**: Only two components need to communicate
  ```csharp
  // Bad: Overkill for simple case
  public class TwoComponentMediator
  {
      private readonly Button _button;
      private readonly Label _label;
      // Just have button update label directly
  }

  // Better: Direct interaction
  button.Click += (s, e) => label.Text = "Clicked";
  ```

- **One-Way Dependencies**: Clear parent-child or one-way relationships
  ```csharp
  // Bad: Unnecessary mediator
  public class ViewModelMediator
  {
      private readonly ViewModel _viewModel;
      private readonly View _view;
      // View already depends on ViewModel naturally
  }

  // Better: Direct dependency
  public class View
  {
      public View(ViewModel viewModel) { }
  }
  ```

- **Performance Critical**: Extra indirection matters
  ```csharp
  // Bad: Mediator in tight loop
  for (int i = 0; i < 1000000; i++)
  {
      mediator.Notify(component, "Update");
  }

  // Better: Direct call
  component.Update();
  ```

- **Already Have Better Patterns**: Framework provides suitable alternatives
  ```csharp
  // Bad: Custom mediator in ASP.NET Core
  // Better: Use built-in middleware pipeline, action filters, or MediatR
  app.UseMiddleware<RequestLoggingMiddleware>();
  ```

### 🎯 Expert Recommendations:

**1. Use MediatR for CQRS**
```csharp
// Good: Industry-standard pattern
public class CreateOrderCommand : IRequest<OrderResult> { }
public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, OrderResult> { }

// Clean separation of commands and queries
await _mediator.Send(new CreateOrderCommand(...));  // Command
var order = await _mediator.Send(new GetOrderQuery(id));  // Query
```

**2. Keep Mediators Focused**
```csharp
// Good: Separate mediators for different concerns
public class OrderProcessingMediator { }  // Order workflow
public class NotificationMediator { }     // Notifications
public class ValidationMediator { }       // Validation

// Bad: One giant mediator for everything
public class ApplicationMediator { }  // Too many responsibilities
```

**3. Use Strongly-Typed Events**
```csharp
// Good: Type-safe events
public record FormFieldChanged(string FieldName, string NewValue);
public record FormSubmitted();

public void Notify<TEvent>(TEvent eventData) where TEvent : class
{
    // Handle based on event type
}

// Bad: String-based events
public void Notify(string eventName)  // Not type-safe
```

**4. Avoid God Object Mediators**
```csharp
// Good: Split large mediators
public class UserRegistrationMediator { }   // Focused on registration
public class UserProfileMediator { }        // Focused on profile

// Bad: One mediator with too many responsibilities
public class UserMediator
{
    // Registration, profile, settings, notifications, permissions, etc.
}
```

**5. Document Interaction Patterns**
```csharp
/// <summary>
/// Coordinates order processing workflow:
/// 1. Validate order
/// 2. Reserve inventory
/// 3. Process payment
/// 4. Update inventory
/// 5. Create shipment
/// 6. Send notifications
/// </summary>
public class OrderWorkflowMediator
{
    // Clear documentation helps maintainers
}
```

**6. Combine with Other Patterns**
```csharp
// Good: Mediator + Observer for event publishing
public class EventMediatorMediator : IMediator
{
    private readonly IEventAggregator _eventAggregator;

    public void Notify(object sender, string eventName)
    {
        // Coordinate and publish events
        _eventAggregator.Publish(new ComponentEvent(sender, eventName));
    }
}
```

**7. Consider Event Aggregator Alternative**
```csharp
// Alternative: Event Aggregator (pub/sub pattern)
public class EventAggregator
{
    public void Subscribe<TEvent>(Action<TEvent> handler) { }
    public void Publish<TEvent>(TEvent eventData) { }
}

// Use when: Loose coupling with pub/sub is sufficient
// Mediator when: Need centralized coordination logic
```

**Real-World Decision Flow:**
1. Multiple components with complex interactions? → Consider Mediator
2. Need to coordinate workflows? → Use Mediator
3. Want to centralize interaction logic? → Use Mediator
4. Components should be reusable independently? → Use Mediator
5. Simple two-component communication? → Don't use Mediator
6. Building CQRS application? → Use MediatR library
7. Need pub/sub pattern? → Consider Event Aggregator instead

The Mediator pattern excels at centralizing complex coordination logic and reducing coupling between components. Use it when component interactions are complex and need to be managed in one place. For .NET applications, strongly consider using the MediatR library for a production-ready, well-tested implementation.
