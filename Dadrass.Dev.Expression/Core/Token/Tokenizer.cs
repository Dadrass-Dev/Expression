namespace Dadrass.Dev.Expression.Core.Token;

/// <summary>
/// Tokenizes the input string into a sequence of tokens.
/// </summary>
public class Tokenizer {
    readonly string _input;
    int _currentIndex;

    /// <summary>
    /// Initializes a new instance of the <see cref="Tokenizer"/> class.
    /// </summary>
    /// <param name="input">The input string to tokenize.</param>
    public Tokenizer(string input)
    {
        _input = input;
        _currentIndex = 0;
    }

    /// <summary>
    /// Tokenizes the entire input string into a list of tokens.
    /// </summary>
    /// <returns>A list of tokens parsed from the input string.</returns>
    public List<TokenModel> Tokenize()
    {
        var tokens = new List<TokenModel>();

        while (!IsAtEnd())
        {
            var c = Advance();

            switch (c)
            {
                case ';':
                    tokens.Add(new TokenModel(OtherTokenType.Semicolon, ";"));
                    break;
                case '(':
                    tokens.Add(new TokenModel(OtherTokenType.LeftParen, "("));
                    break;
                case ')':
                    tokens.Add(new TokenModel(OtherTokenType.RightParen, ")"));
                    break;
                case '+':
                    tokens.Add(new TokenModel(ArithmeticTokenType.Plus, "+"));
                    break;
                case '-':
                    tokens.Add(new TokenModel(ArithmeticTokenType.Minus, "-"));
                    break;
                case '*':
                    tokens.Add(new TokenModel(ArithmeticTokenType.Star, "*"));
                    break;
                case '/':
                    tokens.Add(new TokenModel(ArithmeticTokenType.Slash, "/"));
                    break;
                case '!':
                    tokens.Add(Match('=') ? new TokenModel(ComparisonTokenType.NotEqual, "!=") : new TokenModel(LogicalTokenType.Bang, "!"));
                    break;
                case '=':
                    tokens.Add(Match('=') ? new TokenModel(ComparisonTokenType.EqualEqual, "==") : new TokenModel(OtherTokenType.Assignment, "="));
                    break;
                case '<':
                    tokens.Add(Match('=') ? new TokenModel(ComparisonTokenType.LessEqual, "<=") : new TokenModel(ComparisonTokenType.Less, "<"));
                    break;
                case '>':
                    tokens.Add(Match('=') ? new TokenModel(ComparisonTokenType.GreaterEqual, ">=") : new TokenModel(ComparisonTokenType.Greater, ">"));
                    break;
                case '&':
                    if (Match('&'))
                        tokens.Add(new TokenModel(LogicalTokenType.And, "&&"));
                    else
                        throw new Exception("Unexpected character '&'");
                    break;
                case '|':
                    if (Match('|'))
                        tokens.Add(new TokenModel(LogicalTokenType.Or, "||"));
                    else
                        throw new Exception("Unexpected character '|'");
                    break;
                case '\'':
                    tokens.Add(ParseString());
                    break;
                case '[':
                    tokens.Add(ParseBracketedIdentifier());
                    break;
                case '{':
                    tokens.Add(ParseCurlyBracedIdentifier());
                    break;
                case ' ':
                case '\t':
                case '\r':
                case '\n':
                    break;
                default:
                    if (char.IsDigit(c))
                        tokens.Add(ParseDateTimeOrNumber());
                    else if (char.IsLetter(c))
                        tokens.Add(ParseIdentifier());
                    else
                        throw new Exception($"Unexpected character: {c}");
                    break;
            }
        }

        tokens.Add(new TokenModel(OtherTokenType.Eof, null));// EOF token
        return tokens;
    }

    /// <summary>
    /// Parses a property enclosed in square brackets (e.g., [propertyName]).
    /// </summary>
    /// <returns>A token representing the property name.</returns>
    TokenModel ParseBracketedIdentifier()
    {
        var start = _currentIndex;
        while (!IsAtEnd() && Peek() != ']')
        {
            Advance();
        }
        Advance();// Consume closing ']'
        var propertyName = _input[start..(_currentIndex - 1)];
        return new TokenModel(OtherTokenType.Identifier, propertyName);
    }

    /// <summary>
    /// Parses a property enclosed in curly braces (e.g., {DateTime.Now}).
    /// </summary>
    /// <returns>A token representing the property or method access.</returns>
    TokenModel ParseCurlyBracedIdentifier()
    {
        var start = _currentIndex;
        while (!IsAtEnd() && Peek() != '}')
        {
            Advance();
        }
        Advance();
        var propertyName = _input[start..(_currentIndex - 1)];
        return new TokenModel(OtherTokenType.Identifier, propertyName);
    }

    /// <summary>
    /// Parses a string literal enclosed in single quotes.
    /// </summary>
    /// <returns>A token representing the string literal.</returns>
    TokenModel ParseString()
    {
        var start = _currentIndex;
        while (!IsAtEnd() && Peek() != '\'')
        {
            Advance();
        }
        Advance();// Consume closing '\''
        var value = _input[start..(_currentIndex - 1)];
        return new TokenModel(LiteralTokenType.String, value);
    }

    /// <summary>
    /// Parses either a numeric literal or a date-time literal from the input.
    /// </summary>
    /// <returns>A token representing the parsed literal.</returns>
    TokenModel ParseDateTimeOrNumber()
    {
        var start = _currentIndex - 1;// Include the starting character

        // Parse date-time in format YYYY-MM-DD or YYYY-MM-DD T HH:mm:ss
        while (!IsAtEnd() && (char.IsDigit(Peek()) || Peek() == '-' || Peek() == 'T' || Peek() == ':' || Peek() == '.'))
        {
            Advance();
        }

        var text = _input[start.._currentIndex];

        // Try to parse as a date-time
        return DateTime.TryParse(text, out var dateTime)
            ? new TokenModel(LiteralTokenType.DateTime, dateTime)
            :
            // Fallback to number parsing
            new TokenModel(LiteralTokenType.Number, double.Parse(text));

    }

    /// <summary>
    /// Parses an identifier (such as a variable or function name) from the input.
    /// </summary>
    /// <returns>A token representing the parsed identifier.</returns>
    TokenModel ParseIdentifier()
    {
        var start = _currentIndex - 1;

        // Parse until the next non-letter or non-digit character
        while (!IsAtEnd() && char.IsLetterOrDigit(Peek()))
            Advance();

        var text = _input[start.._currentIndex];

        return new TokenModel(OtherTokenType.Identifier, text);
    }

    /// <summary>
    /// Advances the current index and returns the character at the new index.
    /// </summary>
    /// <returns>The character at the current index.</returns>
    char Advance() => _input[_currentIndex++];

    /// <summary>
    /// Matches the current character with the expected character and advances the index if matched.
    /// </summary>
    /// <param name="expected">The character to match.</param>
    /// <returns>True if the current character matches the expected character, otherwise false.</returns>
    bool Match(char expected)
    {
        if (IsAtEnd() || _input[_currentIndex] != expected) return false;
        _currentIndex++;
        return true;
    }

    /// <summary>
    /// Returns the current character without advancing the index.
    /// </summary>
    /// <returns>The character at the current index.</returns>
    char Peek() => _input[_currentIndex];

    /// <summary>
    /// Checks if the end of the input string has been reached.
    /// </summary>
    /// <returns>True if the end of the input is reached, otherwise false.</returns>
    bool IsAtEnd() => _currentIndex >= _input.Length;
}
