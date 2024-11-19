namespace Dadrass.Dev.Expression.Core.Token;

/// <summary>
/// Represents a token in the parsed expression.
/// </summary>
public class TokenModel {
    /// <summary>
    /// Gets or sets the arithmetic operator type of the token.
    /// </summary>
    public TokenType? TokenType { get; set; }

    /// <summary>
    /// Gets the list of child tokens.
    /// </summary>
    public List<TokenModel> Tokens { get; set; } = [];

    /// <summary>
    /// Gets or sets the actual value (literal) of the token.
    /// </summary>
    public object? Literal { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenModel"/> class.
    /// </summary>
    public TokenModel(TokenType type, object? literal, List<TokenModel>? tokens = null)
    {
        Literal = literal;
        if (literal != null)
            Tokens = tokens!;

        TokenType = type;
    }
}
