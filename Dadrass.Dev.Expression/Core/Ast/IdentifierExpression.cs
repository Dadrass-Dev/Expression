namespace Dadrass.Dev.Expression.Core.Ast;

/// <summary>
/// Represents an identifier node (e.g., a variable or a function call).
/// </summary>
class IdentifierExpression : AstNode {
    /// <summary>
    /// The name of the identifier.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// A function to resolve the value of the identifier at runtime.
    /// </summary>
    public Func<string, object?> Resolver { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="IdentifierExpression"/> class.
    /// </summary>
    /// <param name="name">The name of the identifier.</param>
    /// <param name="resolver">The resolver function to determine the identifier's value.</param>
    public IdentifierExpression(string name, Func<string, object?> resolver)
    {
        Name = name;
        Resolver = resolver;
    }

    /// <summary>
    /// Resolves the identifier's value using the provided resolver function.
    /// </summary>
    /// <returns>The resolved value of the identifier.</returns>
    override public object? Evaluate() => Resolver(Name);
}
