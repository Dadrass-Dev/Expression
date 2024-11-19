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
    protected AstNode ParseLogicalOr()
    {
        var node = ParseLogicalAnd();

        while (Match(TokenType.Or))
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
    protected AstNode ParseLogicalAnd()
    {
        var node = ParseEquality();

        while (Match(TokenType.And))
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
    protected AstNode ParseEquality()
    {
        var node = ParseComparison();

        while (Match(TokenType.EqualEqual, TokenType.NotEqual))
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
    protected AstNode ParseComparison()
    {
        var node = ParsePrimary();

        while (Match(TokenType.Less, TokenType.LessEqual, TokenType.Greater, TokenType.GreaterEqual))
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

        var expr = ParseExpression();
        Consume(TokenType.RightParen, "Expect ')' after expression.");
        return expr;
    }


    protected AstNode ResolveFunction()
    {
        var arguments = SplitListByDelimiter(Previous().Tokens).Select(s => {
            var parser = new Parser(s, _parameters);
            return parser.ParseExpression();
        }).ToArray();

        return new FunctionExpression(Previous().Literal!.ToString()!, arguments, ExpressionUtilities.ResolveFunction);
    }

    protected object? ResolveIdentifier(string identifier)
    {
        return _parameters.GetValueOrDefault(identifier);
    }

    /// <summary>
    /// Checks if the current token matches any of the given token types.
    /// If it matches, advances to the next token.
    /// </summary>
    /// <param name="types">The types to match.</param>
    /// <returns><see langword="true"/> if a match is found; otherwise, <see langword="false"/>.</returns>
    protected bool Match(params object[] types)
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
    protected bool Check(object type)
    {
        if (IsAtEnd()) return false;

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
    /// <returns>The current token before advancing.</returns>
    protected void Advance()
    {
        if (!IsAtEnd()) _current++;
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
    /// <returns>The consumed token.</returns>
    protected void Consume(object type, string message)
    {
        if (!Check(type))
        {
            throw new Exception(message);
        }
        Advance();
    }


    static List<List<TokenModel>> SplitListByDelimiter(List<TokenModel> list)
    {
        var result = new List<List<TokenModel>>();
        var current = new List<TokenModel>();

        foreach (var item in list)
        {
            if (item.TokenType == TokenType.Comma)
            {
                if (current.Count <= 0)
                {
                    continue;
                }
                result.Add([..current]);// Add the current group
                current.Clear();// Start a new group
            }
            else
            {
                current.Add(item);
            }
        }

        // Add the last group if it exists
        if (current.Count > 0)
        {
            result.Add(current);
        }

        return result;
    }
}
