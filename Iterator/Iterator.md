# Iterator Pattern

## 1. Definition

The Iterator pattern provides a way to access elements of a collection sequentially without exposing the underlying representation (list, array, tree, etc.). It separates the traversal algorithm from the collection structure, allowing you to iterate over different types of collections with a uniform interface.

**How it works:**
- **Create an Iterator interface** (or use IEnumerator in .NET) that defines traversal methods
- **Implement concrete iterators** that maintain current position and traversal logic
- **Collections implement IEnumerable** to provide GetEnumerator() factory method
- **Clients use foreach** or manual iteration without knowing collection internals

The Iterator pattern is built into C# and .NET through IEnumerable/IEnumerator interfaces and the foreach statement. Modern C# also provides the `yield return` keyword that makes implementing iterators trivial.

**Key Components:**
- **Iterator (IEnumerator)**: Interface for accessing and traversing elements
- **Concrete Iterator**: Implements traversal algorithm, maintains current position
- **Aggregate (IEnumerable)**: Interface for creating iterators
- **Concrete Aggregate**: Collection that creates and returns iterators

## 2. Pros

- **Encapsulation**: Collection's internal structure hidden from clients
- **Single Responsibility**: Separates storage from traversal logic
- **Multiple Traversals**: Can have multiple iterators over same collection simultaneously
- **Uniform Interface**: All collections can be traversed using foreach
- **Lazy Evaluation**: Elements accessed on demand, not loaded upfront
- **Flexible Traversal**: Can implement different traversal strategies (forward, reverse, filtered)
- **Open/Closed Principle**: Can add new iterator types without modifying collection
- **Memory Efficient**: Don't need to create copy or load entire collection
- **Interchangeable Collections**: Can swap collection implementations without affecting iteration code

## 3. Cons

- **Limited Operations**: Standard iterators only support sequential forward traversal
- **Modification Issues**: Collection changes during iteration can cause errors or undefined behavior
- **Object Creation Overhead**: Creating iterator objects has minor cost (negligible in practice)
- **State Management**: Iterator maintains state (position), consuming memory
- **Read-Only**: Standard iterators don't support modification during iteration
- **Reset Limitations**: Many iterators don't support Reset() or throw NotSupportedException
- **Complexity for Simple Cases**: For simple arrays, direct indexing might be clearer

## 4. Real-world Use Cases in C# & .NET

### Custom Collection with yield return

```csharp
// Modern C# approach using yield return
public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Department { get; set; }
    public decimal Salary { get; set; }
}

public class EmployeeCollection : IEnumerable<Employee>
{
    private List<Employee> _employees = new List<Employee>();

    public void Add(Employee employee) => _employees.Add(employee);

    // Forward iteration
    public IEnumerator<Employee> GetEnumerator()
    {
        foreach (var employee in _employees)
        {
            yield return employee;
        }
    }

    // Reverse iteration
    public IEnumerable<Employee> Reverse()
    {
        for (int i = _employees.Count - 1; i >= 0; i--)
        {
            yield return _employees[i];
        }
    }

    // Filtered iteration by department
    public IEnumerable<Employee> ByDepartment(string department)
    {
        foreach (var employee in _employees)
        {
            if (employee.Department == department)
            {
                yield return employee;
            }
        }
    }

    // Sorted iteration by salary
    public IEnumerable<Employee> BySalary()
    {
        // yield return works with LINQ
        foreach (var employee in _employees.OrderBy(e => e.Salary))
        {
            yield return employee;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

// Usage
var employees = new EmployeeCollection();
employees.Add(new Employee { Id = 1, Name = "Alice", Department = "IT", Salary = 80000 });
employees.Add(new Employee { Id = 2, Name = "Bob", Department = "Sales", Salary = 60000 });
employees.Add(new Employee { Id = 3, Name = "Charlie", Department = "IT", Salary = 90000 });

// Different iteration strategies
Console.WriteLine("All employees:");
foreach (var emp in employees)
{
    Console.WriteLine(emp.Name);
}

Console.WriteLine("\nReverse order:");
foreach (var emp in employees.Reverse())
{
    Console.WriteLine(emp.Name);
}

Console.WriteLine("\nIT Department only:");
foreach (var emp in employees.ByDepartment("IT"))
{
    Console.WriteLine(emp.Name);
}

Console.WriteLine("\nBy salary:");
foreach (var emp in employees.BySalary())
{
    Console.WriteLine($"{emp.Name}: {emp.Salary:C}");
}
```

### Tree Traversal Iterator

```csharp
public class TreeNode<T>
{
    public T Value { get; set; }
    public List<TreeNode<T>> Children { get; set; } = new List<TreeNode<T>>();

    public TreeNode(T value) => Value = value;

    public void AddChild(TreeNode<T> child) => Children.Add(child);
}

public class BinaryTreeNode<T>
{
    public T Value { get; set; }
    public BinaryTreeNode<T> Left { get; set; }
    public BinaryTreeNode<T> Right { get; set; }

    public BinaryTreeNode(T value) => Value = value;
}

public static class TreeExtensions
{
    // Breadth-first traversal
    public static IEnumerable<T> BreadthFirst<T>(this TreeNode<T> root)
    {
        var queue = new Queue<TreeNode<T>>();
        queue.Enqueue(root);

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            yield return node.Value;

            foreach (var child in node.Children)
            {
                queue.Enqueue(child);
            }
        }
    }

    // Depth-first traversal
    public static IEnumerable<T> DepthFirst<T>(this TreeNode<T> root)
    {
        var stack = new Stack<TreeNode<T>>();
        stack.Push(root);

        while (stack.Count > 0)
        {
            var node = stack.Pop();
            yield return node.Value;

            // Push children in reverse order for left-to-right traversal
            for (int i = node.Children.Count - 1; i >= 0; i--)
            {
                stack.Push(node.Children[i]);
            }
        }
    }

    // In-order traversal (for binary trees)
    public static IEnumerable<T> InOrder<T>(this BinaryTreeNode<T> root)
    {
        if (root == null) yield break;

        // Traverse left subtree
        if (root.Left != null)
        {
            foreach (var value in InOrder(root.Left))
            {
                yield return value;
            }
        }

        // Visit root
        yield return root.Value;

        // Traverse right subtree
        if (root.Right != null)
        {
            foreach (var value in InOrder(root.Right))
            {
                yield return value;
            }
        }
    }

    // Pre-order traversal
    public static IEnumerable<T> PreOrder<T>(this BinaryTreeNode<T> root)
    {
        if (root == null) yield break;

        yield return root.Value;

        if (root.Left != null)
        {
            foreach (var value in PreOrder(root.Left))
                yield return value;
        }

        if (root.Right != null)
        {
            foreach (var value in PreOrder(root.Right))
                yield return value;
        }
    }

    // Post-order traversal
    public static IEnumerable<T> PostOrder<T>(this BinaryTreeNode<T> root)
    {
        if (root == null) yield break;

        if (root.Left != null)
        {
            foreach (var value in PostOrder(root.Left))
                yield return value;
        }

        if (root.Right != null)
        {
            foreach (var value in PostOrder(root.Right))
                yield return value;
        }

        yield return root.Value;
    }
}

// Usage
var root = new TreeNode<int>(1);
var child1 = new TreeNode<int>(2);
var child2 = new TreeNode<int>(3);
root.AddChild(child1);
root.AddChild(child2);
child1.AddChild(new TreeNode<int>(4));
child1.AddChild(new TreeNode<int>(5));

Console.WriteLine("Breadth-first:");
foreach (var value in root.BreadthFirst())
{
    Console.Write($"{value} "); // 1 2 3 4 5
}

Console.WriteLine("\nDepth-first:");
foreach (var value in root.DepthFirst())
{
    Console.Write($"{value} "); // 1 2 4 5 3
}
```

### Paginated Iterator

```csharp
public class PaginatedCollection<T> : IEnumerable<T>
{
    private readonly IList<T> _items;
    private readonly int _pageSize;

    public PaginatedCollection(IList<T> items, int pageSize = 10)
    {
        _items = items;
        _pageSize = pageSize;
    }

    public IEnumerable<IEnumerable<T>> Pages()
    {
        for (int i = 0; i < _items.Count; i += _pageSize)
        {
            yield return _items.Skip(i).Take(_pageSize);
        }
    }

    public IEnumerable<T> Page(int pageNumber)
    {
        return _items.Skip(pageNumber * _pageSize).Take(_pageSize);
    }

    public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int TotalPages => (int)Math.Ceiling((double)_items.Count / _pageSize);
    public int Count => _items.Count;
}

// Usage
var items = Enumerable.Range(1, 45).ToList();
var paginated = new PaginatedCollection<int>(items, pageSize: 10);

Console.WriteLine($"Total items: {paginated.Count}");
Console.WriteLine($"Total pages: {paginated.TotalPages}");

// Iterate all items
foreach (var item in paginated)
{
    Console.Write($"{item} ");
}

// Iterate by pages
foreach (var page in paginated.Pages())
{
    Console.WriteLine($"\nPage: {string.Join(", ", page)}");
}

// Get specific page
var page2 = paginated.Page(1); // Second page (0-indexed)
Console.WriteLine($"Page 2: {string.Join(", ", page2)}");
```

### .NET Framework Examples

**.NET Collections**
```csharp
// All .NET collections implement IEnumerable<T>
var list = new List<int> { 1, 2, 3, 4, 5 };
var dict = new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 };
var set = new HashSet<int> { 1, 2, 3 };

// Uniform iteration interface
foreach (var item in list) { }     // Iterates values
foreach (var kvp in dict) { }      // Iterates KeyValuePair
foreach (var item in set) { }      // Iterates values

// All support LINQ
var filtered = list.Where(x => x > 2);     // Returns IEnumerable<int>
var mapped = dict.Select(x => x.Value);    // Returns IEnumerable<int>
```

**IAsyncEnumerable for Async Streams (C# 8.0+)**
```csharp
public class DataService
{
    // Async iterator for streaming data
    public async IAsyncEnumerable<Customer> GetCustomersAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        var command = new SqlCommand("SELECT * FROM Customers", connection);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new Customer
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1)
            };
        }
    }
}

// Usage with await foreach
await foreach (var customer in dataService.GetCustomersAsync())
{
    Console.WriteLine(customer.Name);
    // Process each customer as it's retrieved
    // Doesn't load all customers into memory at once
}
```

**File System Iteration**
```csharp
// DirectoryInfo provides lazy iteration
var directory = new DirectoryInfo(@"C:\MyFolder");

// Lazy evaluation - files accessed on demand
foreach (var file in directory.EnumerateFiles("*.txt"))
{
    Console.WriteLine(file.Name);
    // Can break early without enumerating all files
    if (file.Length > 1000000) break;
}

// Recursive directory traversal
public static IEnumerable<FileInfo> GetAllFiles(string directory)
{
    var dirInfo = new DirectoryInfo(directory);

    foreach (var file in dirInfo.EnumerateFiles())
    {
        yield return file;
    }

    foreach (var subDir in dirInfo.EnumerateDirectories())
    {
        foreach (var file in GetAllFiles(subDir.FullName))
        {
            yield return file;
        }
    }
}
```

## 5. Modern Approach: yield return and IAsyncEnumerable

Modern C# makes the Iterator pattern almost effortless with `yield return` and `IAsyncEnumerable<T>`.

### Using yield return

```csharp
public class NumberGenerator
{
    // Infinite sequence using yield return
    public IEnumerable<int> GetInfiniteSequence()
    {
        int i = 0;
        while (true)
        {
            yield return i++;
        }
    }

    // Fibonacci sequence
    public IEnumerable<int> Fibonacci(int count)
    {
        int a = 0, b = 1;

        for (int i = 0; i < count; i++)
        {
            yield return a;
            (a, b) = (b, a + b);
        }
    }

    // Prime numbers
    public IEnumerable<int> Primes(int max)
    {
        for (int num = 2; num <= max; num++)
        {
            if (IsPrime(num))
            {
                yield return num;
            }
        }
    }

    private bool IsPrime(int num)
    {
        if (num < 2) return false;
        for (int i = 2; i <= Math.Sqrt(num); i++)
        {
            if (num % i == 0) return false;
        }
        return true;
    }
}

// Usage - lazy evaluation
var generator = new NumberGenerator();

// Take only what you need from infinite sequence
foreach (var num in generator.GetInfiniteSequence().Take(10))
{
    Console.Write($"{num} "); // 0 1 2 3 4 5 6 7 8 9
}

// Fibonacci
foreach (var num in generator.Fibonacci(10))
{
    Console.Write($"{num} "); // 0 1 1 2 3 5 8 13 21 34
}

// Primes up to 30
foreach (var num in generator.Primes(30))
{
    Console.Write($"{num} "); // 2 3 5 7 11 13 17 19 23 29
}
```

### IAsyncEnumerable with Dependency Injection

```csharp
// Service interface
public interface IProductService
{
    IAsyncEnumerable<Product> GetProductsAsync(CancellationToken cancellationToken = default);
    IAsyncEnumerable<Product> SearchProductsAsync(string query, CancellationToken cancellationToken = default);
}

// Implementation
public class ProductService : IProductService
{
    private readonly AppDbContext _context;
    private readonly ILogger<ProductService> _logger;

    public ProductService(AppDbContext context, ILogger<ProductService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async IAsyncEnumerable<Product> GetProductsAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting product stream");

        await foreach (var product in _context.Products.AsAsyncEnumerable().WithCancellation(cancellationToken))
        {
            _logger.LogDebug("Yielding product {ProductId}", product.Id);
            yield return product;
        }

        _logger.LogInformation("Product stream complete");
    }

    public async IAsyncEnumerable<Product> SearchProductsAsync(
        string query,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var products = _context.Products
            .Where(p => p.Name.Contains(query))
            .AsAsyncEnumerable();

        await foreach (var product in products.WithCancellation(cancellationToken))
        {
            yield return product;
        }
    }
}

// Register in Program.cs
builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddScoped<IProductService, ProductService>();

// ASP.NET Core Controller with streaming response
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet("stream")]
    public async IAsyncEnumerable<Product> StreamProducts(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var product in _productService.GetProductsAsync(cancellationToken))
        {
            yield return product;
        }
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string query)
    {
        var products = new List<Product>();

        await foreach (var product in _productService.SearchProductsAsync(query))
        {
            products.Add(product);
        }

        return Ok(products);
    }
}
```

### LINQ Integration

```csharp
// Custom LINQ operators using yield return
public static class EnumerableExtensions
{
    // Batch elements into groups
    public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int batchSize)
    {
        var batch = new List<T>(batchSize);

        foreach (var item in source)
        {
            batch.Add(item);

            if (batch.Count == batchSize)
            {
                yield return batch;
                batch = new List<T>(batchSize);
            }
        }

        if (batch.Count > 0)
        {
            yield return batch;
        }
    }

    // Distinct by property
    public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
    {
        var seen = new HashSet<TKey>();

        foreach (var item in source)
        {
            var key = keySelector(item);
            if (seen.Add(key))
            {
                yield return item;
            }
        }
    }

    // Sliding window
    public static IEnumerable<IEnumerable<T>> Window<T>(this IEnumerable<T> source, int windowSize)
    {
        var window = new Queue<T>(windowSize);

        foreach (var item in source)
        {
            window.Enqueue(item);

            if (window.Count == windowSize)
            {
                yield return window.ToList();
                window.Dequeue();
            }
        }
    }
}

// Usage
var numbers = Enumerable.Range(1, 20);

// Batch into groups of 5
foreach (var batch in numbers.Batch(5))
{
    Console.WriteLine(string.Join(", ", batch));
}

// Distinct by property
var products = GetProducts();
foreach (var product in products.DistinctBy(p => p.CategoryId))
{
    Console.WriteLine(product.Name);
}

// Sliding window of 3
foreach (var window in numbers.Window(3))
{
    Console.WriteLine($"[{string.Join(", ", window)}]");
}
// [1, 2, 3]
// [2, 3, 4]
// [3, 4, 5]
// ...
```

## 6. Expert Advice: When to Use vs When Not to Use

### ✅ When to Use Iterator:

- **Traversing Collections**: Need uniform way to iterate different collection types
  ```csharp
  // Good: Consistent iteration across different collections
  foreach (var item in list) { }
  foreach (var item in dictionary.Values) { }
  foreach (var item in customCollection) { }
  ```

- **Hiding Internal Structure**: Don't want to expose collection implementation
  ```csharp
  // Good: Internal structure hidden, can change List to Array
  public class CustomerRepository
  {
      private List<Customer> _customers = new();

      public IEnumerable<Customer> GetAll()
      {
          foreach (var customer in _customers)
              yield return customer;
      }
  }
  ```

- **Lazy Evaluation**: Processing large datasets or infinite sequences
  ```csharp
  // Good: Process massive file without loading into memory
  public IEnumerable<LogEntry> ReadLogFile(string path)
  {
      foreach (var line in File.ReadLines(path))
      {
          yield return ParseLogEntry(line);
      }
  }

  // Only parse lines actually used
  var errors = ReadLogFile("huge.log").Where(e => e.IsError).Take(10);
  ```

- **Multiple Traversal Strategies**: Support different iteration orders
  ```csharp
  // Good: Same collection, multiple traversal methods
  collection.InOrder()
  collection.PreOrder()
  collection.PostOrder()
  collection.BreadthFirst()
  ```

- **Tree/Graph Traversal**: Complex data structures benefit from iterators
  ```csharp
  // Good: Abstract away complex traversal logic
  foreach (var node in tree.BreadthFirst()) { }
  foreach (var node in graph.DepthFirst(startNode)) { }
  ```

- **Streaming Data**: Processing data as it arrives
  ```csharp
  // Good: Stream database results
  await foreach (var record in database.StreamRecordsAsync())
  {
      await ProcessRecord(record);
  }
  ```

### ❌ When NOT to Use Iterator:

- **Random Access Needed**: Need to access elements by index
  ```csharp
  // Bad: Iterator doesn't support indexing
  var iterator = collection.GetEnumerator();
  // Can't do: iterator[5]

  // Better: Use List or array with direct indexing
  var list = new List<int> { 1, 2, 3, 4, 5 };
  var fifth = list[4];
  ```

- **Small Fixed Arrays**: Simple array iteration is clearer
  ```csharp
  // Bad: Overkill for simple array
  var numbers = new[] { 1, 2, 3, 4, 5 };
  foreach (var num in numbers) { }

  // Simpler: Direct for loop (when index matters)
  for (int i = 0; i < numbers.Length; i++)
  {
      Console.WriteLine($"Index {i}: {numbers[i]}");
  }
  ```

- **Modification During Iteration**: Need to modify collection while iterating
  ```csharp
  // Bad: Can't modify collection in foreach
  foreach (var item in list)
  {
      if (condition) list.Remove(item); // Throws exception!
  }

  // Better: Use for loop with index or separate list
  for (int i = list.Count - 1; i >= 0; i--)
  {
      if (condition) list.RemoveAt(i);
  }
  ```

### 🎯 Expert Recommendations:

**1. Use yield return for Simple Iterators**
```csharp
// Good: Compiler generates iterator state machine
public IEnumerable<int> GetNumbers()
{
    for (int i = 0; i < 10; i++)
    {
        yield return i;
    }
}

// Avoid: Manual iterator implementation unless necessary
public class NumbersIterator : IEnumerator<int> { /* complex code */ }
```

**2. Prefer IEnumerable<T> over IEnumerable**
```csharp
// Good: Type-safe, no boxing
public IEnumerable<Product> GetProducts() { }

// Avoid: Non-generic, requires casting
public IEnumerable GetProducts() { }
```

**3. Return IEnumerable for LINQ Composability**
```csharp
// Good: Supports LINQ chaining
public IEnumerable<Customer> GetActiveCustomers()
{
    return _customers.Where(c => c.IsActive);
}

// Client can chain LINQ
var topCustomers = repository.GetActiveCustomers()
    .Where(c => c.TotalSpent > 1000)
    .OrderByDescending(c => c.TotalSpent)
    .Take(10);
```

**4. Use IAsyncEnumerable for Async Streams**
```csharp
// Good: Async streaming from database
public async IAsyncEnumerable<Order> GetOrdersAsync()
{
    await foreach (var order in _context.Orders.AsAsyncEnumerable())
    {
        yield return order;
    }
}
```

**5. Implement IDisposable for Resources**
```csharp
// Good: Iterator holds database connection
public IEnumerable<Record> ReadRecords()
{
    using var connection = new SqlConnection(_connectionString);
    using var reader = command.ExecuteReader();

    while (reader.Read())
    {
        yield return CreateRecord(reader);
    }
    // Connection properly disposed when iteration completes
}
```

**6. Throw InvalidOperationException on Modification**
```csharp
// Good: Detect collection modification
public class SafeCollection<T> : IEnumerable<T>
{
    private List<T> _items = new();
    private int _version = 0;

    public void Add(T item)
    {
        _items.Add(item);
        _version++;
    }

    public IEnumerator<T> GetEnumerator()
    {
        int version = _version;

        foreach (var item in _items)
        {
            if (version != _version)
            {
                throw new InvalidOperationException(
                    "Collection modified during iteration");
            }

            yield return item;
        }
    }
}
```

**7. Use Index and Range for Slicing (C# 8.0+)**
```csharp
// Good: Modern C# slicing syntax
var numbers = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

var lastThree = numbers[^3..];      // 7, 8, 9
var firstFive = numbers[..5];       // 0, 1, 2, 3, 4
var middle = numbers[2..5];         // 2, 3, 4
var allButFirstAndLast = numbers[1..^1]; // 1, 2, 3, 4, 5, 6, 7, 8
```

**Real-World Decision Flow:**
1. Need to traverse collection? → Use foreach (Iterator pattern)
2. Need lazy evaluation? → Use yield return
3. Need async streaming? → Use IAsyncEnumerable
4. Need random access? → Use List/Array, not iterator
5. Need to modify while iterating? → Use for loop, not foreach
6. Building LINQ-style operators? → Return IEnumerable with yield
7. Complex traversal (tree, graph)? → Implement custom iterator

The Iterator pattern is fundamental to C# and .NET. Use built-in IEnumerable/IEnumerator with yield return for most scenarios. The pattern provides excellent encapsulation and flexibility for collection traversal.
