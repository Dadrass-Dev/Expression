namespace Dadrass.Dev.Expression.Core.Token;

/// <summary>
/// Defines other token types such as parentheses and identifiers.
/// </summary>
public enum OtherTokenType {
    /// <summary>Represents a variable or function name identifier.</summary>
    Identifier,

    /// <summary>Represents the left parenthesis "(". </summary>
    LeftParen,

    /// <summary>Represents the right parenthesis ")".</summary>
    RightParen,

    /// <summary>Represents the assignment "=".</summary>
    Assignment,

    /// <summary>Represents the end of the file (EOF) token.</summary>
    Eof,

    /// <summary>Represents the Semicolon ";" token.</summary>
    Semicolon,

    /// <summary>Represents the function token.</summary>
    Function,

    /// <summary>Represents the Semicolon "," token.</summary>
    Comma
}