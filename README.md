# Dadrass Expression Library

Dadrass Expression is a C# library designed for evaluating dynamic expressions at runtime. The library supports
tokenizing, parsing, and evaluating mathematical, logical, and function-based expressions, with the ability to bind
values dynamically from a given data source.

## Key Features:

- **Expression Evaluation**: Evaluate mathematical and logical expressions.
- **Dynamic Data Binding**: Bind values from a dictionary or complex objects during evaluation.
- **Function Support**: Built-in support for functions like `SUM`, `AVG`, `IIF`, and `COALESCE`.
- **AST (Abstract Syntax Tree)**: Parsing expressions into an AST for flexible evaluation.

## Components

### 1. `Expression` Class

The `Expression` class provides methods to evaluate expressions dynamically. It supports evaluating expressions against
a provided data source and returning the result as a specified type.

#### Methods

- **`Evaluate<T>(string expression, Dictionary<string, object>? datasource = null)`**:
    - Evaluates the given expression and returns the result as the specified type `T`.
    - `T` must be a value type (e.g., `int`, `bool`).
    - Optionally takes a `datasource` dictionary to bind values during evaluation.

  **Example Usage**:
  ```csharp
  var result = Expression.Evaluate<int>("2 + 3");
  Console.WriteLine(result); // Outputs: 5
  ```

- **`AstEvaluate(string expression, Dictionary<string, object>? datasource = null)`**:
    - Tokenizes the input expression, parses it into an abstract syntax tree (AST), and evaluates it.
    - If a `datasource` is provided, it binds values (e.g., variables) during evaluation.

  **Example**:
  ```csharp
  var result = Expression.AstEvaluate("[x] + [y]", new Dictionary<string, object> { { "x", 5 }, { "y", 3 } });
  Console.WriteLine(result); // Outputs: 8
  ```

- **`FlattenToDictionary(object? obj, string prefix = "")`**:
    - Flattens a complex object (e.g., a nested object or collection) into a dictionary with property paths as keys.
    - Useful for binding values from complex data sources like nested objects.

  **Example**:
  ```csharp
  var person = new Person { Name = "John", Address = new Address { Street = "Main St" } };
  var flattened = Expression.FlattenToDictionary(person);
  // Result: { "Name": "John", "Address.Street": "Main St" }
  ```

- **`IsSimpleType(Type type)`**:
    - Determines whether a type is considered a "simple" type, such as primitives (`int`, `bool`), `string`, `decimal`,
      `DateTime`, `Guid`, etc.
    - This is used to determine how to handle values when flattening objects.

### 2. `Tokenizer` Class

The `Tokenizer` class is responsible for breaking the input expression into tokens. Tokens are the smallest units of the
expression (e.g., numbers, operators, parentheses).

### 3. `Parser` Class

The `Parser` class processes the tokens generated by the `Tokenizer` and builds an abstract syntax tree (AST). The AST
represents the structure of the expression, which can then be evaluated.

### 4. `ExpressionUtilities` Class

The `ExpressionUtilities` class provides utility methods for resolving functions, performing aggregations, and
evaluating built-in or custom functions.

#### Key Methods

- **`ResolveFunction(string functionName, params object?[]? args)`**:
    - Resolves a function name with arguments to its result. Supports built-in functions like `SUM`, `AVG`, `IIF`, and
      `COALESCE`.
    - Also allows custom functions to be registered and used dynamically.

- **Built-in Functions**:
    - `IIF(condition, trueValue, falseValue)` — Performs an "Immediate If" operation.
    - `COALESCE(arg1, arg2, ...)` — Returns the first non-null argument.
    - `SUM(values)` — Returns the sum of numeric values.
    - `AVG(values)` — Returns the average of numeric values.
    - `COUNT(values)` — Returns the count of non-null values.
    - `NOW()` — Returns the current date and time.
    - `TODAY()` — Returns the current date.

  **Example**:
  ```csharp
  var sum = ExpressionUtilities.ResolveFunction("SUM", new object[] { 1, 2, 3 });
  Console.WriteLine(sum); // Outputs: 6
  ```

### 5. `TokenModel` Class

The `TokenModel` class represents a token in the parsed expression, such as a number, operator, or function. It
includes:

- `TokenType`: The type of token (e.g., `Plus`, `Minus`, `Number`, `String`).
- `Literal`: The value of the token (e.g., `5`, `+`, `"hello"`).
- `Tokens`: A list of child tokens, used for complex expressions or function calls.

### 6. `TokenType` Enum

The `TokenType` enum defines various types of tokens that can appear in an expression, including:

- Arithmetic operators (`Plus`, `Minus`, `Star`, `Slash`).
- Comparison operators (`Less`, `Greater`, `EqualEqual`, `NotEqual`).
- Logical operators (`And`, `Or`, `Bang`).
- Literal types (`Number`, `String`, `DateTime`).
- Special symbols like `LeftParen`, `RightParen`, `Assignment`, and `Comma`.

## Usage

This library allows you to evaluate expressions dynamically against a data model. Here's a beautiful, simple example of
how to use it in a real-world scenario.

### Example:

```csharp
// Sample data model
var model = new Dictionary<string, object>()
{
    {
        "Dc1", new Person
        {
            Name = "Some Name", 
            Age = 30,
        }
    }
};

// Expression to evaluate
var input = "{IIF({IIF([Dc1.Name]=='Some Name',3,4)}==4,5,7)}"; 

// Evaluate the expression and get the result
var result = Expression.Evaluate<int>(input, model);

// Output the result
Console.WriteLine($"Result: {result}");

namespace Sample
{
    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}


## Installation

To use the `Dadrass Expression` library, simply add the source files to your project or compile them into a DLL and reference it in your project.

## License

This project is licensed under the MIT License. See the LICENSE file for more information.