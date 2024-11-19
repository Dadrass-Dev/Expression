namespace Dadrass.Dev.Expression.Core.Ast;

/// <summary>
/// Represents an identifier node (e.g., a variable or a function call).
/// </summary>
class FunctionExpression : AstNode {
    /// <summary>
    /// The name of the identifier.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The name of the identifier.
    /// </summary>
    public AstNode[] Args { get; }

    /// <summary>
    /// A function to resolve the value of the identifier at runtime.
    /// </summary>
    public Func<string, object?[]?, object?> Resolver { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FunctionExpression"/> class.
    /// </summary>
    /// <param name="name">The name of the function.</param>
    /// <param name="args">The args of the function.</param>
    /// <param name="resolver">The resolver function to determine the identifier's value.</param>
    public FunctionExpression(string name, AstNode[] args, Func<string, object?[]?, object?> resolver)
    {
        Name = name;
        Args = args;
        Resolver = resolver;
    }

    /// <summary>
    /// Resolves the identifier's value using the provided resolver function.
    /// </summary>
    /// <returns>The resolved value of the identifier.</returns>
    override public object? Evaluate() => Resolver(Name, Args.Select(s => s.Evaluate()).ToArray());
}
