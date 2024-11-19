namespace Dadrass.Dev.Expression.Core.Utilities;

/// <summary>
/// Provides utility methods for resolving dynamic expressions, performing aggregations, and evaluating functions.
/// </summary>
public static class ExpressionUtilities {

    /// <summary>
    /// Function registry for custom functions
    /// </summary>
    public readonly static Dictionary<string, Func<object?[]?, object>> CustomFunctionRegistry = [];

    /// <summary>
    /// Function registry mapping function names to their implementations.
    /// </summary>
    readonly static Dictionary<string, Func<object?[]?, object>> FunctionRegistry = new()
    {
        {
            "IIF", Iif
        },
        {
            "COALESCE", Coalesce
        },
        {
            "SUM", Sum
        },
        {
            "AVG", Avg
        },
        {
            "COUNT", Count
        },
        {
            "NOW", _ => DateTime.Now
        },
        {
            "TODAY", _ => DateTime.Today
        }
    };

    #region Core Functionality

    /// <summary>
    /// Resolves a function name with arguments to its value.
    /// </summary>
    /// <param name="functionName">The name of the function.</param>
    /// <param name="args">The arguments to pass to the function.</param>
    /// <returns>The result of the function.</returns>
    public static object? ResolveFunction(string functionName, params object?[]? args)
    {
        var functions =
            FunctionRegistry
                .Concat(CustomFunctionRegistry)
                .ToDictionary(keySelector: d =>
                    d.Key.ToLower(),
                elementSelector: d => d.Value
                );
        return !functions.ContainsKey(functionName.ToLower()) ? null : functions[functionName.ToLower()](args);
    }

    /// <summary>
    /// Implements the IIF (Immediate If) function.
    /// </summary>
    static object Iif(object?[]? args)
    {
        ArgumentNullException.ThrowIfNull(args);

        if (args.Length != 3)
        {
            throw new ArgumentException("IIF requires exactly three arguments: condition, true value, false value.");
        }
        return (Convert.ToBoolean(args[0]) ? args[1] : args[2])!;
    }


    /// <summary>
    /// Implements the COALESCE function, which returns the first non-null argument.
    /// </summary>
    static object Coalesce(object?[]? args)
    {
        ArgumentNullException.ThrowIfNull(args);
        return args.FirstOrDefault(arg => arg != null) ?? throw new Exception("All arguments are null.");
    }

    #endregion

    #region Aggregation Functions

    /// <summary>
    /// Implements the SUM function.
    /// </summary>
    static object Sum(object?[]? args)
    {
        ArgumentNullException.ThrowIfNull(args);
        return args.Sum(Convert.ToDouble);
    }

    /// <summary>
    /// Implements the AVG (average) function.
    /// </summary>
    static object Avg(object?[]? args)
    {
        ArgumentNullException.ThrowIfNull(args);
        return args.Average(Convert.ToDouble);
    }

    /// <summary>
    /// Implements the COUNT function, which returns the number of non-null arguments.
    /// </summary>
    static object Count(object?[]? args)
    {
        ArgumentNullException.ThrowIfNull(args);
        return args.Count(arg => arg != null);
    }

    #endregion

}
