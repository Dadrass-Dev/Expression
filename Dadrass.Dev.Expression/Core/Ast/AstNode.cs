namespace Dadrass.Dev.Expression.Core.Ast;

/// <summary>
/// Abstract base class representing a node in the Abstract Syntax Tree (AST).
/// </summary>
public abstract class AstNode {
    /// <summary>
    /// The literal value of this node.
    /// </summary>
    public virtual object? Value { get; set; }

    /// <summary>
    /// Evaluates the AST node and returns its result.
    /// </summary>
    /// <returns>The evaluated result of the node.</returns>
    public abstract object? Evaluate();
}
