namespace Dadrass.Dev.Expression.Core.Token;

/// <summary>
/// Defines token types for comparison operators.
/// </summary>
public enum ComparisonTokenType {
    /// <summary>Represents the "&lt;" operator.</summary>
    Less,

    /// <summary>Represents the "&gt;=" operator.</summary>
    LessEqual,

    /// <summary>Represents the "&gt;" operator.</summary>
    Greater,

    /// <summary>Represents the "&gt;=" operator.</summary>
    GreaterEqual,

    /// <summary>Represents the "==" operator.</summary>
    EqualEqual,

    /// <summary>Represents the "!=" operator.</summary>
    NotEqual
}
