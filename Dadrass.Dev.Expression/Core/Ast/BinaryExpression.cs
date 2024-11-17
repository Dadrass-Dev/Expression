namespace Dadrass.Dev.Expression.Core.Ast;

using Token;

/// <summary>
/// Represents a binary expression node (e.g., "a && b" or "x > y").
/// </summary>
public class BinaryExpression : AstNode {
    /// <summary>
    /// The left operand of the binary expression.
    /// </summary>
    public AstNode Left { get; }

    /// <summary>
    /// The right operand of the binary expression.
    /// </summary>
    public AstNode Right { get; }

    /// <summary>
    /// The operator token (e.g., &&, >, +) for the binary expression.
    /// </summary>
    public TokenModel Operator { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BinaryExpression"/> class.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <param name="op">The operator token.</param>
    public BinaryExpression(AstNode left, AstNode right, TokenModel op)
    {
        Left = left;
        Right = right;
        Operator = op;
    }

    /// <summary>
    /// Evaluates the binary expression.
    /// </summary>
    /// <returns>The result of evaluating the binary expression.</returns>
    override public object Evaluate()
    {
        // Evaluate both sides of the expression
        var leftValue = Left.Evaluate();
        var rightValue = Right.Evaluate();

        // Handle logical operators (e.g., &&, ||)
        if (Operator.LogicalType.HasValue)
        {
            return Operator.LogicalType switch
            {
                LogicalTokenType.And => (bool)leftValue && (bool)rightValue,
                LogicalTokenType.Or => (bool)leftValue || (bool)rightValue,
                _ => throw new Exception($"Unsupported logical operator: {Operator.LogicalType}")
            };
        }

        // Handle comparison operators (e.g., ==, >)
        if (Operator.ComparisonType.HasValue)
        {
            return Operator.ComparisonType switch
            {
                ComparisonTokenType.EqualEqual => Equals(leftValue, rightValue),
                ComparisonTokenType.NotEqual => !Equals(leftValue, rightValue),
                ComparisonTokenType.Less => (dynamic)leftValue < (dynamic)rightValue,
                ComparisonTokenType.LessEqual => (dynamic)leftValue <= (dynamic)rightValue,
                ComparisonTokenType.Greater => (dynamic)leftValue > (dynamic)rightValue,
                ComparisonTokenType.GreaterEqual => (dynamic)leftValue >= (dynamic)rightValue,
                _ => throw new Exception($"Unsupported comparison operator: {Operator.ComparisonType}")
            };
        }

        throw new Exception($"Unsupported operator in binary expression: {Operator}");
    }
}
