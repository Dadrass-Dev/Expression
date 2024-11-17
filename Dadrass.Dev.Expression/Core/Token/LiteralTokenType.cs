namespace Dadrass.Dev.Expression.Core.Token;

/// <summary>
/// Defines token types for literals (numbers, booleans, etc.).
/// </summary>
public enum LiteralTokenType {
    /// <summary>Represents the boolean literal "true".</summary>
    True,

    /// <summary>Represents the boolean literal "false".</summary>
    False,

    /// <summary>Represents a numeric literal (e.g., 42, 3.14).</summary>
    Number,

    /// <summary>Represents a string literal (e.g., "hello").</summary>
    String,

    /// <summary>Represents a dateTime literal (e.g., "2024-11-17 10:30:45").</summary>
    DateTime
}