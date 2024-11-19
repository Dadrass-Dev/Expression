namespace Dadrass.Dev.Expression.Core.Token;

/// <summary>
/// Defines token types for arithmetic operators.
/// </summary>
enum TokenType {
    /// <summary>Represents the "+" operator.</summary>
    Plus,

    /// <summary>Represents the "-" operator.</summary>
    Minus,

    /// <summary>Represents the "*" operator.</summary>
    Star,

    /// <summary>Represents the "/" operator.</summary>
    Slash,

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
    NotEqual,

    /// <summary>Represents the boolean literal "true".</summary>
    True,

    /// <summary>Represents the boolean literal "false".</summary>
    False,

    /// <summary>Represents a numeric literal (e.g., 42, 3.14).</summary>
    Number,

    /// <summary>Represents a string literal (e.g., "hello").</summary>
    String,

    /// <summary>Represents a dateTime literal (e.g., "2024-11-17 10:30:45").</summary>
    DateTime,

    /// <summary>Represents the "&&" operator.</summary>
    And,

    /// <summary>Represents the "||" operator.</summary>
    Or,

    /// <summary>Represents the "!" operator.</summary>
    Bang,

    /// <summary>Represents a variable or function name identifier.</summary>
    Identifier,

    /// <summary>Represents the left parenthesis "(". </summary>
    LeftParen,

    /// <summary>Represents the right parenthesis ")".</summary>
    RightParen,

    /// <summary>Represents the assignment "=".</summary>
    Assignment,


    /// <summary>Represents the Semicolon ";" token.</summary>
    Semicolon,

    /// <summary>Represents the function token.</summary>
    Function,

    /// <summary>Represents the Semicolon "," token.</summary>
    Comma,

    Eof
}
