# Visitor Pattern

## 1. Definition

The Visitor pattern allows you to add new operations to existing object structures without modifying those structures. It achieves this by separating algorithms from the objects they operate on through a technique called double dispatch. The pattern is particularly useful when you have a stable object structure but need to define new operations frequently.

**How it works:**
- **Create a Visitor interface** with a visit method for each concrete component type
- **Components implement an Accept method** that takes a visitor parameter
- **Components call the appropriate visitor method** passing themselves (double dispatch)
- **Add new operations** by creating new visitor implementations

The key principle is **double dispatch**: the operation executed depends on both the runtime type of the component (first dispatch) and the runtime type of the visitor (second dispatch), enabling type-safe operations on heterogeneous object structures.

## 2. Pros

- **Open/Closed for Operations**: Add new operations without modifying component classes
- **Single Responsibility**: Related operations grouped in one visitor class
- **Type Safety**: No casting needed, strongly-typed component references
- **Accumulate State**: Visitor can maintain state while traversing structure
- **Related Operations Together**: All logic for one operation in one place
- **Works with Heterogeneous Structures**: Different component types in same structure
- **Separation of Concerns**: Algorithms separated from data structures
- **Easy to Add Operations**: Just create new visitor implementation

## 3. Cons

- **Adding Component Types is Hard**: New component type requires modifying all visitors
- **Breaks Encapsulation**: Visitors need access to component internals
- **Circular Dependency**: Components depend on visitors, visitors depend on components
- **Complex Double Dispatch**: The double dispatch mechanism can be confusing
- **Large Visitor Interface**: Many component types = many visit methods
- **Violates OCP for Components**: Adding components breaks existing visitors
- **Not Always Appropriate**: Overkill if component types change frequently
- **Testing Complexity**: Need to test visitor with all component combinations

## 4. Real-world Use Cases in C# & .NET

### Compiler AST Processing

```csharp
// Expression tree components
public interface IExpression
{
    void Accept(IExpressionVisitor visitor);
}

public class NumberExpression : IExpression
{
    public int Value { get; }
    public NumberExpression(int value) => Value = value;

    public void Accept(IExpressionVisitor visitor)
    {
        visitor.VisitNumber(this);
    }
}

public class BinaryExpression : IExpression
{
    public IExpression Left { get; }
    public IExpression Right { get; }
    public string Operator { get; }

    public BinaryExpression(IExpression left, string op, IExpression right)
    {
        Left = left;
        Operator = op;
        Right = right;
    }

    public void Accept(IExpressionVisitor visitor)
    {
        visitor.VisitBinary(this);
    }
}

// Visitor interface
public interface IExpressionVisitor
{
    void VisitNumber(NumberExpression expr);
    void VisitBinary(BinaryExpression expr);
}

// Code generator visitor
public class CodeGeneratorVisitor : IExpressionVisitor
{
    private readonly StringBuilder _code = new();

    public void VisitNumber(NumberExpression expr)
    {
        _code.AppendLine($"PUSH {expr.Value}");
    }

    public void VisitBinary(BinaryExpression expr)
    {
        expr.Left.Accept(this);
        expr.Right.Accept(this);
        _code.AppendLine($"{expr.Operator.ToUpper()}");
    }

    public string GetCode() => _code.ToString();
}

// Interpreter visitor
public class InterpreterVisitor : IExpressionVisitor
{
    private readonly Stack<int> _stack = new();

    public void VisitNumber(NumberExpression expr)
    {
        _stack.Push(expr.Value);
    }

    public void VisitBinary(BinaryExpression expr)
    {
        expr.Left.Accept(this);
        expr.Right.Accept(this);

        var right = _stack.Pop();
        var left = _stack.Pop();

        var result = expr.Operator switch
        {
            "+" => left + right,
            "-" => left - right,
            "*" => left * right,
            "/" => left / right,
            _ => throw new NotSupportedException()
        };

        _stack.Push(result);
    }

    public int GetResult() => _stack.Pop();
}

// Usage
var expr = new BinaryExpression(
    new NumberExpression(5),
    "+",
    new BinaryExpression(
        new NumberExpression(3),
        "*",
        new NumberExpression(2)
    )
); // 5 + (3 * 2) = 11

var codeGen = new CodeGeneratorVisitor();
expr.Accept(codeGen);
Console.WriteLine(codeGen.GetCode());
// Output:
// PUSH 5
// PUSH 3
// PUSH 2
// MUL
// ADD

var interpreter = new InterpreterVisitor();
expr.Accept(interpreter);
Console.WriteLine(interpreter.GetResult()); // 11
```

### Document Export System

```csharp
public interface IDocumentElement
{
    void Accept(IDocumentVisitor visitor);
}

public class Paragraph : IDocumentElement
{
    public string Text { get; }
    public Paragraph(string text) => Text = text;

    public void Accept(IDocumentVisitor visitor) => visitor.VisitParagraph(this);
}

public class Image : IDocumentElement
{
    public string Url { get; }
    public int Width { get; }
    public int Height { get; }

    public Image(string url, int width, int height)
    {
        Url = url;
        Width = width;
        Height = height;
    }

    public void Accept(IDocumentVisitor visitor) => visitor.VisitImage(this);
}

public interface IDocumentVisitor
{
    void VisitParagraph(Paragraph paragraph);
    void VisitImage(Image image);
}

public class HtmlExportVisitor : IDocumentVisitor
{
    private readonly StringBuilder _html = new();

    public void VisitParagraph(Paragraph paragraph)
    {
        _html.AppendLine($"<p>{paragraph.Text}</p>");
    }

    public void VisitImage(Image image)
    {
        _html.AppendLine($"<img src=\"{image.Url}\" width=\"{image.Width}\" height=\"{image.Height}\" />");
    }

    public string GetHtml() => _html.ToString();
}

public class MarkdownExportVisitor : IDocumentVisitor
{
    private readonly StringBuilder _markdown = new();

    public void VisitParagraph(Paragraph paragraph)
    {
        _markdown.AppendLine(paragraph.Text);
        _markdown.AppendLine();
    }

    public void VisitImage(Image image)
    {
        _markdown.AppendLine($"![Image]({image.Url})");
    }

    public string GetMarkdown() => _markdown.ToString();
}

// Usage
var document = new List<IDocumentElement>
{
    new Paragraph("Hello World"),
    new Image("photo.jpg", 800, 600),
    new Paragraph("This is a test")
};

var htmlExporter = new HtmlExportVisitor();
foreach (var element in document)
    element.Accept(htmlExporter);

Console.WriteLine(htmlExporter.GetHtml());

var markdownExporter = new MarkdownExportVisitor();
foreach (var element in document)
    element.Accept(markdownExporter);

Console.WriteLine(markdownExporter.GetMarkdown());
```

### .NET Framework Examples

**Roslyn Syntax Tree Visitor**
```csharp
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public class MethodCounterVisitor : CSharpSyntaxWalker
{
    public int MethodCount { get; private set; }

    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        MethodCount++;
        base.VisitMethodDeclaration(node);
    }
}

// Usage
var code = @"
    public class MyClass
    {
        public void Method1() { }
        public void Method2() { }
    }
";

var tree = CSharpSyntaxTree.ParseText(code);
var root = tree.GetRoot();

var visitor = new MethodCounterVisitor();
visitor.Visit(root);
Console.WriteLine($"Methods: {visitor.MethodCount}"); // 2
```

**Expression Tree Visitor**
```csharp
using System.Linq.Expressions;

public class ParameterReplacer : ExpressionVisitor
{
    private readonly ParameterExpression _oldParameter;
    private readonly ParameterExpression _newParameter;

    public ParameterReplacer(ParameterExpression oldParam, ParameterExpression newParam)
    {
        _oldParameter = oldParam;
        _newParameter = newParam;
    }

    protected override Expression VisitParameter(ParameterExpression node)
    {
        return node == _oldParameter ? _newParameter : base.VisitParameter(node);
    }
}

// Usage
var oldParam = Expression.Parameter(typeof(int), "x");
var newParam = Expression.Parameter(typeof(int), "y");
var expr = Expression.Lambda<Func<int, int>>(
    Expression.Add(oldParam, Expression.Constant(1)),
    oldParam
);

var replacer = new ParameterReplacer(oldParam, newParam);
var newExpr = (LambdaExpression)replacer.Visit(expr);
// x => x + 1 becomes y => y + 1
```

## 5. Modern Approach: Dependency Injection

```csharp
// Visitor with DI
public class ValidationVisitor : IDocumentVisitor
{
    private readonly IValidationService _validationService;
    private readonly ILogger<ValidationVisitor> _logger;
    private readonly List<string> _errors = new();

    public ValidationVisitor(
        IValidationService validationService,
        ILogger<ValidationVisitor> logger)
    {
        _validationService = validationService;
        _logger = logger;
    }

    public void VisitParagraph(Paragraph paragraph)
    {
        if (string.IsNullOrWhiteSpace(paragraph.Text))
        {
            _errors.Add("Paragraph cannot be empty");
            _logger.LogWarning("Empty paragraph found");
        }
    }

    public void VisitImage(Image image)
    {
        if (!_validationService.IsValidUrl(image.Url))
        {
            _errors.Add($"Invalid image URL: {image.Url}");
            _logger.LogWarning("Invalid image URL: {Url}", image.Url);
        }
    }

    public IReadOnlyList<string> GetErrors() => _errors.AsReadOnly();
}

// Register in Program.cs
builder.Services.AddScoped<IValidationService, ValidationService>();
builder.Services.AddScoped<ValidationVisitor>();
builder.Services.AddScoped<HtmlExportVisitor>();
```

## 6. Expert Advice: When to Use vs When Not to Use

### ✅ When to Use Visitor:

- **Stable Object Structure**: Component types rarely change
- **Frequent New Operations**: Need to add operations often
- **Related Operations**: Want to group all logic for one operation together
- **Complex Operations**: Operations need to accumulate state across traversal
- **Type-Specific Behavior**: Different operations for different component types

### ❌ When NOT to Use Visitor:

- **Unstable Structure**: Component types change frequently
- **Few Operations**: Only 1-2 operations on the structure
- **Simple Operations**: Operations don't need component-specific behavior
- **Encapsulation Critical**: Components shouldn't expose internals

### 🎯 Expert Recommendations:

**1. Use for Compiler/AST Processing**
```csharp
// Perfect use case: stable syntax tree, many operations
public class OptimizationVisitor : IExpressionVisitor { }
public class CodeGeneratorVisitor : IExpressionVisitor { }
public class TypeCheckerVisitor : IExpressionVisitor { }
```

**2. Consider Alternatives for Changing Structures**
```csharp
// If components change often, use Strategy or Command instead
public interface IOperation
{
    void Execute(IComponent component);
}
```

**3. Return Values from Visit Methods**
```csharp
public interface IVisitor<T>
{
    T VisitParagraph(Paragraph p);
    T VisitImage(Image img);
}
```

**4. Use Base Visitor for Common Logic**
```csharp
public abstract class DocumentVisitorBase : IDocumentVisitor
{
    protected virtual void BeforeVisit() { }
    protected virtual void AfterVisit() { }

    public void VisitParagraph(Paragraph p)
    {
        BeforeVisit();
        ProcessParagraph(p);
        AfterVisit();
    }

    protected abstract void ProcessParagraph(Paragraph p);
}
```

The Visitor pattern excels at adding operations to stable object structures. Use it for compilers, document processing, or any scenario where you have a fixed set of types but frequently add new operations. Avoid it when your object structure changes frequently.
