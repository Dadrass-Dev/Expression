namespace Dadrass.Dev.Expression.Core.Token;

/// <summary>
/// Represents a token in the parsed expression.
/// </summary>
public class TokenModel {
    /// <summary>
    /// Gets or sets the arithmetic operator type of the token.
    /// </summary>
    public ArithmeticTokenType? ArithmeticType { get; set; }

    /// <summary>
    /// Gets or sets the logical operator type of the token.
    /// </summary>
    public LogicalTokenType? LogicalType { get; set; }

    /// <summary>
    /// Gets or sets the comparison operator type of the token.
    /// </summary>
    public ComparisonTokenType? ComparisonType { get; set; }

    /// <summary>
    /// Gets or sets the literal value associated with the token.
    /// </summary>
    public LiteralTokenType? LiteralType { get; set; }

    /// <summary>
    /// Gets or sets other token types like identifier or parentheses.
    /// </summary>
    public OtherTokenType? OtherType { get; set; }

    /// <summary>
    /// Gets or sets the actual value (literal) of the token.
    /// </summary>
    public object? Literal { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenModel"/> class.
    /// </summary>
    public TokenModel(object type, object? literal)
    {
        Literal = literal;

        switch (type)
        {
            // Assign types based on the type of the object passed
            case ArithmeticTokenType arithmeticType:
                ArithmeticType = arithmeticType;
                break;
            case LogicalTokenType logicalType:
                LogicalType = logicalType;
                break;
            case ComparisonTokenType comparisonType:
                ComparisonType = comparisonType;
                break;
            case LiteralTokenType literalType:
                LiteralType = literalType;
                break;
            case OtherTokenType otherType:
                OtherType = otherType;
                break;
            default:
                throw new Exception("Unsupported token type.");
        }
    }
}
