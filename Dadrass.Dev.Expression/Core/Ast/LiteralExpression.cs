namespace Dadrass.Dev.Expression.Core.Ast;

/// <summary>
/// Represents a literal value in the AST (e.g., "42", "true").
/// </summary>
public class LiteralExpression : AstNode {
    /// <summary>
    /// The literal value of this node.
    /// </summary>
    public object Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LiteralExpression"/> class.
    /// </summary>
    /// <param name="value">The literal value.</param>
    public LiteralExpression(object value)
    {
        Value = value;
    }

    /// <summary>
    /// Evaluates the literal expression.
    /// </summary>
    /// <returns>The literal value.</returns>
    override public object Evaluate() => Value;
}