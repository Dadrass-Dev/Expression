namespace Dadrass.Dev.Expression.Core.Parse;

using Ast;
using Token;
using Utilities;

/// <summary>
/// Parses a list of tokens into an Abstract Syntax Tree (AST).
/// </summary>
public class Parser {
    readonly List<TokenModel> _tokens;
    readonly Dictionary<string, object> _parameters;
    int _current;

    /// <summary>
    /// Initializes a new instance of the <see cref="Parser"/> class.
    /// </summary>
    /// <param name="tokens">The list of tokens to parse.</param>
    /// <param name="parameters">The dictionary of parameters to replace.</param>
    public Parser(List<TokenModel> tokens, Dictionary<string, object> parameters)
    {
        _tokens = tokens;
        _parameters = parameters;
        _current = 0;
    }

    /// <summary>
    /// Parses the tokens into an expression AST.
    /// </summary>
    /// <returns>The root node of the parsed AST.</returns>
    public AstNode ParseExpression() => ParseLogicalOr();

    /// <summary>
    /// Parses logical OR expressions.
    /// </summary>
    AstNode ParseLogicalOr()
    {
        var node = ParseLogicalAnd();

        while (Match(LogicalTokenType.Or))
        {
            var op = Previous();
            var right = ParseLogicalAnd();
            node = new BinaryExpression(node, right, op);
        }

        return node;
    }

    /// <summary>
    /// Parses logical AND expressions.
    /// </summary>
    AstNode ParseLogicalAnd()
    {
        var node = ParseEquality();

        while (Match(LogicalTokenType.And))
        {
            var op = Previous();
            var right = ParseEquality();
            node = new BinaryExpression(node, right, op);
        }

        return node;
    }

    /// <summary>
    /// Parses equality expressions (e.g., "==" and "!=").
    /// </summary>
    AstNode ParseEquality()
    {
        var node = ParseComparison();

        while (Match(ComparisonTokenType.EqualEqual, ComparisonTokenType.NotEqual))
        {
            var op = Previous();
            var right = ParseComparison();
            node = new BinaryExpression(node, right, op);
        }

        return node;
    }

    /// <summary>
    /// Parses comparison expressions (e.g., "&lt;", "&gt;=", etc.).
    /// </summary>
    AstNode ParseComparison()
    {
        var node = ParsePrimary();

        while (Match(ComparisonTokenType.Less, ComparisonTokenType.LessEqual, ComparisonTokenType.Greater, ComparisonTokenType.GreaterEqual))
        {
            var op = Previous();
            var right = ParsePrimary();
            node = new BinaryExpression(node, right, op);
        }

        return node;
    }

    /// <summary>
    /// Parses primary expressions, such as literals or identifiers.
    /// </summary>
    AstNode ParsePrimary()
    {
        if (Match(LiteralTokenType.Number, LiteralTokenType.String, LiteralTokenType.DateTime, LiteralTokenType.True, LiteralTokenType.False))
        {
            return new LiteralExpression(Previous().Literal!);
        }

        if (Match(OtherTokenType.Identifier))
        {
            return new IdentifierExpression(Previous().Literal!.ToString()!, ResolveIdentifier);
        }

        if (Match(OtherTokenType.LeftParen))
        {
            var expr = ParseExpression();
            Consume(OtherTokenType.RightParen, "Expect ')' after expression.");
            return expr;
        }

        throw new Exception("Invalid expression.");
    }

    /// <summary>
    /// Checks if the current token matches any of the given token types.
    /// If it matches, advances to the next token.
    /// </summary>
    /// <param name="types">The types to match.</param>
    /// <returns><see langword="true"/> if a match is found; otherwise, <see langword="false"/>.</returns>
    bool Match(params object[] types)
    {
        if (!types.Any(Check))
            return false;
        Advance();
        return true;
    }

    /// <summary>
    /// Checks if the current token is of the given type.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns><see langword="true"/> if the token matches the type; otherwise, <see langword="false"/>.</returns>
    bool Check(object type)
    {
        if (IsAtEnd()) return false;

        var currentToken = Peek();
        return type switch
        {
            LogicalTokenType logical => currentToken.LogicalType == logical,
            ComparisonTokenType comparison => currentToken.ComparisonType == comparison,
            LiteralTokenType literal => currentToken.LiteralType == literal,
            OtherTokenType other => currentToken.OtherType == other,
            _ => false
        };
    }

    /// <summary>
    /// Parses function calls (e.g., IIF, COALESCE).
    /// </summary>
    object? ParseFunctionCall(string functionName)
    {
        Consume(OtherTokenType.LeftParen, "Expected '(' after function name.");

        List<object> arguments = [];

        if (!Check(OtherTokenType.RightParen))
        {
            do
            {
                arguments.Add(ParseExpression());
            } while (Match(OtherTokenType.Comma));
        }

        Consume(OtherTokenType.RightParen, "Expected ')' after function arguments.");
        return ExpressionUtilities.ResolveFunction(functionName, arguments.ToArray());
    }

    /// <summary>
    /// Advances to the next token in the list.
    /// </summary>
    /// <returns>The current token before advancing.</returns>
    void Advance()
    {
        if (!IsAtEnd()) _current++;
        Previous();
    }

    /// <summary>
    /// Checks if the parser has reached the end of the token list.
    /// </summary>
    /// <returns><see langword="true"/> if at the end of the token list; otherwise, <see langword="false"/>.</returns>
    bool IsAtEnd() => Peek().OtherType == OtherTokenType.Eof;

    /// <summary>
    /// Gets the current token without advancing.
    /// </summary>
    /// <returns>The current token.</returns>
    TokenModel Peek() => _tokens[_current];

    /// <summary>
    /// Gets the previous token (the last token the parser advanced from).
    /// </summary>
    /// <returns>The previous token.</returns>
    TokenModel Previous() => _tokens[_current - 1];

    /// <summary>
    /// Consumes the current token if it matches the given type; otherwise, throws an exception.
    /// </summary>
    /// <param name="type">The expected token type.</param>
    /// <param name="message">The error message if the token does not match.</param>
    /// <returns>The consumed token.</returns>
    void Consume(object type, string message)
    {
        if (!Check(type))
        {
            throw new Exception(message);
        }
        Advance();
    }

    /// <summary>
    /// Resolves the value of an identifier by its name.
    /// </summary>
    /// <param name="name">The name of the identifier.</param>
    /// <returns>The resolved value of the identifier.</returns>
    object ResolveIdentifier(string name)
    {
        // This method should be implemented to resolve identifiers in your context.
        // For example, map "DateTime.Now" to its actual value.
        return ParseFunctionCall(name) ?? _parameters[name];
    }
}
