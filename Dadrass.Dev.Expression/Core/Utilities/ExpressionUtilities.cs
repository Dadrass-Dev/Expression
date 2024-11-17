namespace Dadrass.Dev.Expression.Core.Utilities {
/// <summary>
/// Provides utility methods for resolving dynamic expressions, performing aggregations, and evaluating functions.
/// </summary>
public static class ExpressionUtilities {
    /// <summary>
    /// Function registry mapping function names to their implementations.
    /// </summary>
    readonly static Dictionary<string, Func<object[], object>> FunctionRegistry = new()
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
        },
        {
            "ADD_DAYS", args => AdjustDateTime(DateTime.Now, Convert.ToInt32(args[0]), "days")
        },
        {
            "ADD_HOURS", args => AdjustDateTime(DateTime.Now, Convert.ToInt32(args[0]), "hours")
        },
    };

    #region Core Functionality

    /// <summary>
    /// Resolves a function name with arguments to its value.
    /// </summary>
    /// <param name="functionName">The name of the function.</param>
    /// <param name="args">The arguments to pass to the function.</param>
    /// <returns>The result of the function.</returns>
    /// <exception cref="Exception">Thrown when the function is not found.</exception>
    public static object? ResolveFunction(string functionName, params object[] args)
    {
        return !FunctionRegistry.ContainsKey(functionName.ToUpper()) ? null : FunctionRegistry[functionName.ToUpper()](args);
    }

    /// <summary>
    /// Implements the IIF (Immediate If) function.
    /// </summary>
    private static object Iif(object[] args)
    {
        if (args.Length != 3)
            throw new ArgumentException("IIF requires exactly three arguments: condition, true value, false value.");
        return Convert.ToBoolean(args[0]) ? args[1] : args[2];
    }

    /// <summary>
    /// Implements the COALESCE function, which returns the first non-null argument.
    /// </summary>
    private static object Coalesce(object?[] args)
    {
        return args.FirstOrDefault(arg => arg != null) ?? throw new Exception("All arguments are null.");
    }

    #endregion

    #region Aggregation Functions

    /// <summary>
    /// Implements the SUM function.
    /// </summary>
    private static object Sum(object[] args)
    {
        return args.Sum(Convert.ToDouble);
    }

    /// <summary>
    /// Implements the AVG (average) function.
    /// </summary>
    private static object Avg(object[] args)
    {
        return args.Average(Convert.ToDouble);
    }

    /// <summary>
    /// Implements the COUNT function, which returns the number of non-null arguments.
    /// </summary>
    private static object Count(object?[] args)
    {
        return args.Count(arg => arg != null);
    }

    #endregion

    #region DateTime Utilities

    /// <summary>
    /// Adjusts a DateTime value by a specified amount and unit.
    /// </summary>
    private static DateTime AdjustDateTime(DateTime dateTime, int amount, string unit)
    {
        return unit.ToLower() switch
        {
            "days" => dateTime.AddDays(amount),
            "hours" => dateTime.AddHours(amount),
            "minutes" => dateTime.AddMinutes(amount),
            "seconds" => dateTime.AddSeconds(amount),
            "milliseconds" => dateTime.AddMilliseconds(amount),
            _ => throw new Exception($"Invalid time unit: {unit}")
        };
    }

    #endregion

}
}
