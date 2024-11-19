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
    override public object? Evaluate()
    {
        // Evaluate both sides of the expression
        var leftValue = Left.Evaluate();
        var rightValue = Right.Evaluate();

        // Handle logical operators (e.g., &&, ||)
        if (Operator.TokenType.HasValue)
        {
            return Operator switch
            {
                { TokenType: TokenType.And } => (bool)leftValue! && (bool)rightValue!,
                { TokenType: TokenType.Or } => (bool)leftValue! || (bool)rightValue!,
                { TokenType: TokenType.EqualEqual } => Equals(leftValue, rightValue),
                { TokenType: TokenType.NotEqual } => !Equals(leftValue, rightValue),
                { TokenType: TokenType.Less } => (dynamic)leftValue! < (dynamic)rightValue!,
                { TokenType: TokenType.LessEqual } => (dynamic)leftValue! <= (dynamic)rightValue!,
                { TokenType: TokenType.Greater } => (dynamic)leftValue! > (dynamic)rightValue!,
                { TokenType: TokenType.GreaterEqual } => (dynamic)leftValue! >= (dynamic)rightValue!,
                _ => throw new Exception($"Unsupported comparison operator: {Operator.TokenType}")
            };
        }

        throw new Exception($"Unsupported operator in binary expression: {Operator}");
    }
}
