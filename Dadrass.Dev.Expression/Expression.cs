    namespace Dadrass.Dev.Expression;

    using System.Collections;
    using System.Reflection;
    using Core.Parse;
    using Core.Token;

    /// <summary>
    /// Provides methods for evaluating expressions against a given data source.
    /// </summary>
    public static class Expression {
        /// <summary>
        /// Evaluates the given expression and returns the result as the specified type.
        /// </summary>
        /// <typeparam name="T">The expected return type. Must be a value type.</typeparam>
        /// <param name="expression">The expression to evaluate.</param>
        /// <param name="datasource">An optional dictionary providing values to bind during evaluation.</param>
        /// <returns>The result of the evaluation cast to type <typeparamref name="T"/>.</returns>
        public static T Evaluate<T>(string expression, Dictionary<string, object>? datasource = null) where T : struct
        {
            // Evaluate the expression and return the result cast to the specified type
            var result = AstEvaluate(expression, datasource);
            return result != null ? (T)result : default;
        }

        /// <summary>
        /// Evaluates an abstract syntax tree (AST) built from the given expression.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <param name="datasource">An optional dictionary providing values to bind during evaluation.</param>
        /// <returns>The result of the evaluation as an object, or null if the evaluation fails.</returns>
        static object? AstEvaluate(string expression, Dictionary<string, object>? datasource = null)
        {
            // Flatten the data source into a dictionary with fully qualified keys
            Dictionary<string, object?>? model = null;
            if (datasource != null)
            {
                model = datasource
                    .SelectMany(entry => FlattenToDictionary(entry.Value, entry.Key))
                    .ToDictionary();
            }

            // Tokenize the input expression
            var tokenizer = new Tokenizer(expression);
            var tokens = tokenizer.Tokenize();

            // Parse the tokens into an abstract syntax tree (AST)
            var parser = new Parser(tokens, model);
            var ast = parser.ParseExpression();

            // Evaluate the AST and return the result
            return ast.Evaluate();
        }

        /// <summary>
        /// Flattens a complex object into a dictionary where keys represent property paths.
        /// </summary>
        /// <param name="obj">The object to flatten.</param>
        /// <param name="prefix">The prefix for nested property paths.</param>
        /// <returns>A dictionary where keys are property paths and values are property values.</returns>
        static Dictionary<string, object?> FlattenToDictionary(object? obj, string prefix = "")
        {
            var dictionary = new Dictionary<string, object?>();

            if (obj == null)
            {
                return dictionary;
            }

            // Retrieve all public instance properties of the object
            var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                var value = property.GetValue(obj);
                var key = string.IsNullOrEmpty(prefix) ? property.Name : $"{prefix}.{property.Name}";

                if (value == null || IsSimpleType(value.GetType()))
                {
                    // Add simple types (or null) directly to the dictionary
                    dictionary[key] = value;
                }
                else if (value is IEnumerable enumerable and not string)
                {
                    // Handle collections (e.g., lists or arrays)
                    var list = new List<object>();
                    foreach (var item in enumerable)
                    {
                        if (IsSimpleType(item.GetType()))
                        {
                            list.Add(item);
                        }
                        else
                        {
                            // Recursively flatten nested objects within the collection
                            list.AddRange(FlattenToDictionary(item).Select(subProperty =>
                                new Dictionary<string, object?>
                                {
                                    {
                                        subProperty.Key, subProperty.Value
                                    }
                                }));
                        }
                    }
                    dictionary[key] = list;
                }
                else
                {
                    // Recursively flatten complex nested objects
                    foreach (var subProperty in FlattenToDictionary(value, key))
                    {
                        dictionary[subProperty.Key] = subProperty.Value;
                    }
                }
            }

            return dictionary;
        }

        /// <summary>
        /// Determines whether a given type is considered a "simple" type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>
        /// True if the type is a primitive type, an enum, or one of the common types
        /// such as string, decimal, DateTime, Guid, or TimeSpan; otherwise, false.
        /// </returns>
        static bool IsSimpleType(Type type) =>
            type.IsPrimitive || type.IsEnum || type == typeof(string) || type == typeof(decimal) ||
            type == typeof(DateTime) || type == typeof(Guid) || type == typeof(TimeSpan);
    }
