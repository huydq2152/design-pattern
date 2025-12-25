# Observer Pattern

## 1. Definition

The Observer pattern defines a one-to-many dependency between objects so that when one object (the subject) changes state, all its dependents (observers) are notified and updated automatically. This pattern promotes loose coupling by allowing observers to subscribe to and receive notifications from a subject without the subject knowing the concrete classes of its observers.

**How it works:**
- **Subject (Publisher)** maintains a list of observers and provides methods to attach/detach them
- **Observers (Subscribers)** implement a common interface with an update method
- **When the subject's state changes**, it notifies all registered observers
- **Observers receive notifications** and can query the subject for updated state

The key principle is **publish-subscribe**: the subject publishes events, and observers subscribe to be notified of changes, enabling a decoupled one-to-many notification system.

## 2. Pros

- **Loose Coupling**: Subject and observers are loosely coupled through interfaces
- **Dynamic Relationships**: Observers can be added/removed at runtime
- **One-to-Many Notifications**: Subject automatically notifies all observers
- **Open/Closed Principle**: Add new observers without modifying subject
- **Broadcast Communication**: Single change notifies multiple observers
- **Supports Event-Driven Architecture**: Natural fit for event-based systems
- **Separation of Concerns**: Subject manages state, observers handle updates

## 3. Cons

- **Unexpected Updates**: Observers may receive notifications unexpectedly
- **Performance Issues**: Many observers = many notification calls
- **Memory Leaks**: Observers not detached can prevent garbage collection
- **Update Order Dependency**: If observers depend on update order, issues arise
- **Complexity**: Can be hard to trace notification flow
- **Debugging Difficulty**: Indirect relationships make debugging harder
- **Cascade Updates**: Observers updating can trigger more notifications

## 4. Real-world Use Cases in C# & .NET

### Stock Price Monitoring

```csharp
// Subject interface
public interface IStockSubject
{
    void Attach(IStockObserver observer);
    void Detach(IStockObserver observer);
    void Notify();
}

// Observer interface
public interface IStockObserver
{
    void Update(Stock stock);
}

// Concrete subject
public class Stock : IStockSubject
{
    private readonly List<IStockObserver> _observers = new();
    private decimal _price;

    public string Symbol { get; }
    public decimal Price
    {
        get => _price;
        set
        {
            if (_price != value)
            {
                _price = value;
                Notify();
            }
        }
    }

    public Stock(string symbol, decimal price)
    {
        Symbol = symbol;
        _price = price;
    }

    public void Attach(IStockObserver observer)
    {
        if (!_observers.Contains(observer))
            _observers.Add(observer);
    }

    public void Detach(IStockObserver observer)
    {
        _observers.Remove(observer);
    }

    public void Notify()
    {
        foreach (var observer in _observers)
        {
            observer.Update(this);
        }
    }
}

// Concrete observers
public class StockDisplay : IStockObserver
{
    public void Update(Stock stock)
    {
        Console.WriteLine($"Display: {stock.Symbol} is now ${stock.Price}");
    }
}

public class StockAlert : IStockObserver
{
    private readonly decimal _threshold;

    public StockAlert(decimal threshold) => _threshold = threshold;

    public void Update(Stock stock)
    {
        if (stock.Price > _threshold)
        {
            Console.WriteLine($"ALERT: {stock.Symbol} exceeded ${_threshold}!");
        }
    }
}

// Usage
var appleStock = new Stock("AAPL", 150.00m);

var display = new StockDisplay();
var alert = new StockAlert(160.00m);

appleStock.Attach(display);
appleStock.Attach(alert);

appleStock.Price = 155.00m; // Display updates
appleStock.Price = 165.00m; // Display updates, Alert triggers
```

### .NET Framework Examples

**C# Events (Built-in Observer)**
```csharp
public class Button
{
    // Event declaration (subject)
    public event EventHandler? Click;

    public void OnClick()
    {
        // Notify all subscribers
        Click?.Invoke(this, EventArgs.Empty);
    }
}

// Usage
var button = new Button();

// Subscribe (attach observer)
button.Click += (sender, e) => Console.WriteLine("Button clicked!");
button.Click += (sender, e) => Console.WriteLine("Another handler");

// Trigger event
button.OnClick();
// Output:
// Button clicked!
// Another handler
```

**IObservable/IObserver (Reactive Extensions)**
```csharp
using System.Reactive.Linq;

public class TemperatureSensor : IObservable<double>
{
    private readonly List<IObserver<double>> _observers = new();

    public IDisposable Subscribe(IObserver<double> observer)
    {
        _observers.Add(observer);
        return new Unsubscriber(_observers, observer);
    }

    public void PublishTemperature(double temperature)
    {
        foreach (var observer in _observers)
        {
            observer.OnNext(temperature);
        }
    }

    private class Unsubscriber : IDisposable
    {
        private readonly List<IObserver<double>> _observers;
        private readonly IObserver<double> _observer;

        public Unsubscriber(List<IObserver<double>> observers, IObserver<double> observer)
        {
            _observers = observers;
            _observer = observer;
        }

        public void Dispose() => _observers.Remove(_observer);
    }
}

// Observer
public class TemperatureDisplay : IObserver<double>
{
    public void OnNext(double temperature)
    {
        Console.WriteLine($"Temperature: {temperature}°C");
    }

    public void OnError(Exception error) { }
    public void OnCompleted() { }
}

// Usage
var sensor = new TemperatureSensor();
var display = new TemperatureDisplay();

var subscription = sensor.Subscribe(display);
sensor.PublishTemperature(25.5);
subscription.Dispose(); // Unsubscribe
```

**INotifyPropertyChanged (WPF/MAUI)**
```csharp
public class Person : INotifyPropertyChanged
{
    private string _name;

    public string Name
    {
        get => _name;
        set
        {
            if (_name != value)
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

// Usage in WPF
var person = new Person();
person.PropertyChanged += (sender, e) =>
{
    Console.WriteLine($"Property {e.PropertyName} changed");
};

person.Name = "Alice"; // Triggers notification
```

## 5. Modern Approach: Dependency Injection

```csharp
// Event aggregator pattern with DI
public interface IEventAggregator
{
    void Subscribe<TEvent>(Action<TEvent> handler);
    void Publish<TEvent>(TEvent eventData);
}

public class EventAggregator : IEventAggregator
{
    private readonly Dictionary<Type, List<Delegate>> _subscribers = new();

    public void Subscribe<TEvent>(Action<TEvent> handler)
    {
        var eventType = typeof(TEvent);
        if (!_subscribers.ContainsKey(eventType))
            _subscribers[eventType] = new List<Delegate>();

        _subscribers[eventType].Add(handler);
    }

    public void Publish<TEvent>(TEvent eventData)
    {
        var eventType = typeof(TEvent);
        if (_subscribers.TryGetValue(eventType, out var handlers))
        {
            foreach (var handler in handlers.Cast<Action<TEvent>>())
            {
                handler(eventData);
            }
        }
    }
}

// Events
public record OrderCreatedEvent(int OrderId, decimal Amount);

// Observers (handlers)
public class EmailNotificationService
{
    public EmailNotificationService(IEventAggregator eventAggregator)
    {
        eventAggregator.Subscribe<OrderCreatedEvent>(OnOrderCreated);
    }

    private void OnOrderCreated(OrderCreatedEvent evt)
    {
        Console.WriteLine($"Sending email for order {evt.OrderId}");
    }
}

public class InventoryService
{
    public InventoryService(IEventAggregator eventAggregator)
    {
        eventAggregator.Subscribe<OrderCreatedEvent>(OnOrderCreated);
    }

    private void OnOrderCreated(OrderCreatedEvent evt)
    {
        Console.WriteLine($"Updating inventory for order {evt.OrderId}");
    }
}

// Register in Program.cs
builder.Services.AddSingleton<IEventAggregator, EventAggregator>();
builder.Services.AddScoped<EmailNotificationService>();
builder.Services.AddScoped<InventoryService>();

// Publish event
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IEventAggregator _eventAggregator;

    public OrdersController(IEventAggregator eventAggregator)
    {
        _eventAggregator = eventAggregator;
    }

    [HttpPost]
    public IActionResult CreateOrder(CreateOrderRequest request)
    {
        // ... create order ...

        _eventAggregator.Publish(new OrderCreatedEvent(order.Id, order.Amount));

        return Ok(order);
    }
}
```

## 6. Expert Advice: When to Use vs When Not to Use

### ✅ When to Use Observer:

- **One-to-Many Notifications**: One change affects multiple objects
- **Dynamic Subscriptions**: Observers added/removed at runtime
- **Loose Coupling**: Subject and observers should be independent
- **Event-Driven Systems**: Natural fit for event-based architectures
- **UI Updates**: Model changes update multiple views

### ❌ When NOT to Use Observer:

- **Performance Critical**: Many observers = performance overhead
- **Simple Relationships**: Direct method calls are simpler
- **Order Matters**: Observers execute in unpredictable order
- **Synchronous Required**: Need guaranteed execution order

### 🎯 Expert Recommendations:

**1. Use C# Events for Simple Cases**
```csharp
public class DataSource
{
    public event EventHandler<DataChangedEventArgs>? DataChanged;

    protected virtual void OnDataChanged(DataChangedEventArgs e)
    {
        DataChanged?.Invoke(this, e);
    }
}
```

**2. Use WeakReferences to Prevent Memory Leaks**
```csharp
public class WeakEventManager
{
    private readonly List<WeakReference<IObserver>> _observers = new();

    public void Attach(IObserver observer)
    {
        _observers.Add(new WeakReference<IObserver>(observer));
    }

    public void Notify()
    {
        _observers.RemoveAll(wr => !wr.TryGetTarget(out _));
        foreach (var wr in _observers)
        {
            if (wr.TryGetTarget(out var observer))
                observer.Update();
        }
    }
}
```

**3. Use Async for Long-Running Observers**
```csharp
public async Task NotifyAsync()
{
    var tasks = _observers.Select(obs => obs.UpdateAsync());
    await Task.WhenAll(tasks);
}
```

**4. Consider MediatR for CQRS**
```csharp
// MediatR notification (observer pattern)
public class OrderCreatedNotification : INotification
{
    public int OrderId { get; init; }
}

public class EmailHandler : INotificationHandler<OrderCreatedNotification>
{
    public async Task Handle(OrderCreatedNotification notification, CancellationToken cancellationToken)
    {
        // Send email
    }
}
```

The Observer pattern is ideal for implementing event-driven systems and maintaining consistency across dependent objects. Use it when you need one-to-many notifications with loose coupling. For .NET applications, prefer built-in event mechanisms or libraries like MediatR for production code.
