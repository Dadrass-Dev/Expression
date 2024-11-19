namespace Dadrass.Dev.Expression.Core.Parse;

using Ast;
using Token;
using Utilities;

/// <summary>
/// Parses a list of tokens into an Abstract Syntax Tree (AST).
/// </summary>
class Parser {
    readonly List<TokenModel> _tokens;
    readonly Dictionary<string, object?>? _parameters;
    int _current;

    /// <summary>
    /// Initializes a new instance of the <see cref="Parser"/> class.
    /// </summary>
    /// <param name="tokens">The list of tokens to parse.</param>
    /// <param name="parameters">The dictionary of parameters to replace.</param>
    public Parser(List<TokenModel> tokens, Dictionary<string, object?>? parameters)
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
    /// <returns>The root node of the logical OR expression.</returns>
    protected AstNode ParseLogicalOr()
    {
        var node = ParseLogicalAnd();

        while (Match(TokenType.Or))
        {
            var operatorToken = Previous();
            var rightNode = ParseLogicalAnd();
            node = new BinaryExpression(node, rightNode, operatorToken);
        }

        return node;
    }

    /// <summary>
    /// Parses logical AND expressions.
    /// </summary>
    /// <returns>The root node of the logical AND expression.</returns>
    protected AstNode ParseLogicalAnd()
    {
        var node = ParseEquality();

        while (Match(TokenType.And))
        {
            var operatorToken = Previous();
            var rightNode = ParseEquality();
            node = new BinaryExpression(node, rightNode, operatorToken);
        }

        return node;
    }

    /// <summary>
    /// Parses equality expressions (e.g., "==" and "!=").
    /// </summary>
    /// <returns>The root node of the equality expression.</returns>
    protected AstNode ParseEquality()
    {
        var node = ParseComparison();

        while (Match(TokenType.EqualEqual, TokenType.NotEqual))
        {
            var operatorToken = Previous();
            var rightNode = ParseComparison();
            node = new BinaryExpression(node, rightNode, operatorToken);
        }

        return node;
    }

    /// <summary>
    /// Parses comparison expressions (e.g., "&lt;", "&gt;=", etc.).
    /// </summary>
    /// <returns>The root node of the comparison expression.</returns>
    protected AstNode ParseComparison()
    {
        var node = ParsePrimary();

        while (Match(TokenType.Less, TokenType.LessEqual, TokenType.Greater, TokenType.GreaterEqual))
        {
            var operatorToken = Previous();
            var rightNode = ParsePrimary();
            node = new BinaryExpression(node, rightNode, operatorToken);
        }

        return node;
    }

    /// <summary>
    /// Parses primary expressions, such as literals or identifiers.
    /// </summary>
    /// <returns>The root node of the primary expression.</returns>
    protected AstNode ParsePrimary()
    {
        if (Match(TokenType.Number, TokenType.String, TokenType.DateTime, TokenType.True, TokenType.False))
        {
            return new LiteralExpression(Previous().Literal!);
        }

        if (Match(TokenType.Identifier))
        {
            return new IdentifierExpression(Previous().Literal!.ToString()!, ResolveIdentifier);
        }

        if (Match(TokenType.Function))
        {
            return ResolveFunction();
        }

        var expression = ParseExpression();
        Consume(TokenType.RightParen, "Expected ')' after expression.");
        return expression;
    }

    /// <summary>
    /// Resolves a function expression and its arguments.
    /// </summary>
    /// <returns>The root node of the function expression.</returns>
    protected AstNode ResolveFunction()
    {
        var arguments = SplitListByDelimiter(Previous().Tokens)
            .Select(tokens => {
                var parser = new Parser(tokens, _parameters);
                return parser.ParseExpression();
            })
            .ToArray();

        return new FunctionExpression(Previous().Literal!.ToString()!, arguments, ExpressionUtilities.ResolveFunction);
    }

    /// <summary>
    /// Resolves the value of an identifier.
    /// </summary>
    /// <param name="identifier">The identifier name.</param>
    /// <returns>The resolved value of the identifier, or <see langword="null"/> if not found.</returns>
    protected object? ResolveIdentifier(string identifier) => _parameters?.GetValueOrDefault(identifier);

    /// <summary>
    /// Checks if the current token matches any of the given token types.
    /// If it matches, advances to the next token.
    /// </summary>
    /// <param name="types">The types to match.</param>
    /// <returns><see langword="true"/> if a match is found; otherwise, <see langword="false"/>.</returns>
    protected bool Match(params object[] types)
    {
        if (!types.Any(Check))
        {
            return false;
        }
        Advance();
        return true;
    }

    /// <summary>
    /// Checks if the current token is of the given type.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns><see langword="true"/> if the token matches the type; otherwise, <see langword="false"/>.</returns>
    protected bool Check(object type)
    {
        if (IsAtEnd())
        {
            return false;
        }

        var currentToken = Peek();
        return type switch
        {
            TokenType tokenType => currentToken.TokenType == tokenType,
            _ => false
        };
    }

    /// <summary>
    /// Advances to the next token in the list.
    /// </summary>
    protected void Advance()
    {
        if (!IsAtEnd())
        {
            _current++;
        }
        Previous();
    }

    /// <summary>
    /// Checks if the parser has reached the end of the token list.
    /// </summary>
    /// <returns><see langword="true"/> if at the end of the token list; otherwise, <see langword="false"/>.</returns>
    protected bool IsAtEnd() => Peek().TokenType == TokenType.Eof;

    /// <summary>
    /// Gets the current token without advancing.
    /// </summary>
    /// <returns>The current token.</returns>
    protected TokenModel Peek() => _tokens[_current];

    /// <summary>
    /// Gets the previous token (the last token the parser advanced from).
    /// </summary>
    /// <returns>The previous token.</returns>
    protected TokenModel Previous() => _tokens[_current - 1];

    /// <summary>
    /// Consumes the current token if it matches the given type; otherwise, throws an exception.
    /// </summary>
    /// <param name="type">The expected token type.</param>
    /// <param name="message">The error message if the token does not match.</param>
    protected void Consume(object type, string message)
    {
        if (!Check(type))
        {
            throw new Exception(message);
        }
        Advance();
    }

    /// <summary>
    /// Splits a list of tokens by a delimiter.
    /// </summary>
    /// <param name="tokens">The list of tokens to split.</param>
    /// <returns>A list of token groups.</returns>
    static List<List<TokenModel>> SplitListByDelimiter(List<TokenModel> tokens)
    {
        var result = new List<List<TokenModel>>();
        var currentGroup = new List<TokenModel>();

        foreach (var token in tokens)
        {
            if (token.TokenType == TokenType.Comma)
            {
                if (currentGroup.Count <= 0)
                {
                    continue;
                }
                result.Add([..currentGroup]);
                currentGroup.Clear();
            }
            else
            {
                currentGroup.Add(token);
            }
        }

        if (currentGroup.Count > 0)
        {
            result.Add(currentGroup);
        }

        return result;
    }
}
