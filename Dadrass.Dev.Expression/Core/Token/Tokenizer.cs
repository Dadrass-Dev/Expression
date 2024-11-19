namespace Dadrass.Dev.Expression.Core.Token;

/// <summary>
/// Tokenizes the input string into a sequence of tokens.
/// </summary>
class Tokenizer {
    /// <summary>
    /// The input string to be tokenized.
    /// </summary>
    readonly string _input;

    /// <summary>
    /// The current index within the input string.
    /// </summary>
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
            // Read the next character and determine its token type
            var currentChar = Advance();

            switch (currentChar)
            {
                case ';':
                    tokens.Add(new TokenModel(TokenType.Semicolon, ";"));
                    break;
                case '(':
                    tokens.Add(new TokenModel(TokenType.LeftParen, "("));
                    break;
                case ')':
                    tokens.Add(new TokenModel(TokenType.RightParen, ")"));
                    break;
                case '+':
                    tokens.Add(new TokenModel(TokenType.Plus, "+"));
                    break;
                case '-':
                    tokens.Add(new TokenModel(TokenType.Minus, "-"));
                    break;
                case '*':
                    tokens.Add(new TokenModel(TokenType.Star, "*"));
                    break;
                case '/':
                    tokens.Add(new TokenModel(TokenType.Slash, "/"));
                    break;
                case '!':
                    tokens.Add(Match('=') ? new TokenModel(TokenType.NotEqual, "!=") : new TokenModel(TokenType.Bang, "!"));
                    break;
                case '=':
                    tokens.Add(Match('=') ? new TokenModel(TokenType.EqualEqual, "==") : new TokenModel(TokenType.Assignment, "="));
                    break;
                case '<':
                    tokens.Add(Match('=') ? new TokenModel(TokenType.LessEqual, "<=") : new TokenModel(TokenType.Less, "<"));
                    break;
                case '>':
                    tokens.Add(Match('=') ? new TokenModel(TokenType.GreaterEqual, ">=") : new TokenModel(TokenType.Greater, ">"));
                    break;
                case '&':
                    if (Match('&'))
                    {
                        tokens.Add(new TokenModel(TokenType.And, "&&"));
                    }
                    else
                    {
                        throw new Exception("Unexpected character '&'.");
                    }
                    break;
                case '|':
                    if (Match('|'))
                    {
                        tokens.Add(new TokenModel(TokenType.Or, "||"));
                    }
                    else
                    {
                        throw new Exception("Unexpected character '|'.");
                    }
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
                case ',':
                    tokens.Add(new TokenModel(TokenType.Comma, ","));
                    break;
                case ' ':
                case '\t':
                case '\r':
                case '\n':
                    // Ignore whitespace
                    break;
                default:
                    if (char.IsDigit(currentChar))
                    {
                        tokens.Add(ParseDateTimeOrNumber());
                    }
                    else if (char.IsLetter(currentChar))
                    {
                        tokens.Add(ParseIdentifier());
                    }
                    else
                    {
                        throw new Exception($"Unexpected character: {currentChar}");
                    }
                    break;
            }
        }

        // Add an EOF token to indicate the end of the input
        tokens.Add(new TokenModel(TokenType.Eof, null));
        return tokens;
    }

    /// <summary>
    /// Parses a property enclosed in square brackets (e.g., [propertyName]).
    /// </summary>
    /// <returns>A token representing the property name.</returns>
    TokenModel ParseBracketedIdentifier()
    {
        var start = _currentIndex;
        while (!IsAtEnd() && Peek() != ']') Advance();
        Advance();// Consume the closing ']'
        var propertyName = _input[start..(_currentIndex - 1)];
        return new TokenModel(TokenType.Identifier, propertyName);
    }

    /// <summary>
    /// Parses a property or function enclosed in curly braces (e.g., {DateTime.Now}).
    /// </summary>
    /// <returns>A token representing the property or method call.</returns>
    TokenModel ParseCurlyBracedIdentifier()
    {
        var start = _currentIndex;
        var openBracesCount = 0;

        while (!IsAtEnd())
        {
            if (_input[_currentIndex - 1] == '{') openBracesCount++;
            if (Peek() == '}') openBracesCount--;

            if (openBracesCount == 0) break;
            Advance();
        }

        Advance();
        var content = _input[start..(_currentIndex - 1)];
        return new TokenModel(TokenType.Function, content);
    }

    /// <summary>
    /// Parses a string literal enclosed in single quotes.
    /// </summary>
    /// <returns>A token representing the string literal.</returns>
    TokenModel ParseString()
    {
        var start = _currentIndex;
        while (!IsAtEnd() && Peek() != '\'') Advance();
        Advance();// Consume the closing '\''
        var value = _input[start..(_currentIndex - 1)];
        return new TokenModel(TokenType.String, value);
    }

    /// <summary>
    /// Parses either a numeric literal or a date-time literal from the input.
    /// </summary>
    /// <returns>A token representing the parsed literal.</returns>
    TokenModel ParseDateTimeOrNumber()
    {
        var start = _currentIndex - 1;// Include the starting character
        while (!IsAtEnd() && (char.IsDigit(Peek()) || Peek() == '-' || Peek() == 'T' || Peek() == ':' || Peek() == '.')) Advance();
        var text = _input[start.._currentIndex];

        return DateTime.TryParse(text, out var dateTime)
            ? new TokenModel(TokenType.DateTime, dateTime)
            : new TokenModel(TokenType.Number, CastStringToNumber(text));
    }

    /// <summary>
    /// Attempts to convert a string into a numeric value.
    /// </summary>
    /// <param name="input">The string to convert.</param>
    /// <returns>The parsed numeric value.</returns>
    static object CastStringToNumber(string input)
    {
        if (int.TryParse(input, out var intResult)) return intResult;
        if (decimal.TryParse(input, out var decimalResult)) return decimalResult;
        if (double.TryParse(input, out var doubleResult)) return doubleResult;
        throw new FormatException("Invalid numeric format.");
    }

    /// <summary>
    /// Parses an identifier (e.g., variable or function name) from the input.
    /// </summary>
    /// <returns>A token representing the identifier.</returns>
    TokenModel ParseIdentifier()
    {
        var start = _currentIndex - 1;
        while (!IsAtEnd() && char.IsLetterOrDigit(Peek())) Advance();
        return new TokenModel(TokenType.Identifier, _input[start.._currentIndex]);
    }

    /// <summary>
    /// Advances the current index and returns the character at the new index.
    /// </summary>
    /// <returns>The character at the current index.</returns>
    char Advance() => _input[_currentIndex++];

    /// <summary>
    /// Matches the current character with the expected character and advances if matched.
    /// </summary>
    /// <param name="expected">The expected character to match.</param>
    /// <returns>True if matched, otherwise false.</returns>
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
    /// <returns>True if at the end of the input, otherwise false.</returns>
    bool IsAtEnd() => _currentIndex >= _input.Length;
}
