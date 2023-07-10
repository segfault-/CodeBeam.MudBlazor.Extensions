using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;

namespace MudExtensions
{
#nullable enable
    public class ExpressionGenerator
    {
        // Cache commonly used MethodInfo
        private static readonly MethodInfo? TrimMethod = typeof(string).GetMethod("Trim", Type.EmptyTypes);
        private static readonly MethodInfo? ContainsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
        private static readonly MethodInfo? StartsWithMethod = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
        private static readonly MethodInfo? EndsWithMethod = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });

        // Method reference for 'Enumerable.Contains' method
        private readonly MethodInfo _methodContains = typeof(Enumerable)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .Single(m => m.Name == nameof(Enumerable.Contains) && m.GetParameters().Length == 2);

        internal static bool IsNullableEnum(Type t)
        {
            Type? u = Nullable.GetUnderlyingType(t);
            return (u is not null) && u.IsEnum;
        }
        internal static Expression GenerateStringFilterExpression<T>(AtomicPredicate<T> rule, Expression parameterExpression)
        {
            // As the data type is known to be string, no need to convert
            var valueAsString = rule.Value?.ToString();

            // Invoking 'Trim' method on 'parameterExpression' and storing in 'trimmedParameter'
            var trimmedParameter = Expression.Call(parameterExpression, TrimMethod!);

            // Creating expressions to check if 'parameterExpression' is null or not
            var isNullExpression = Expression.Equal(parameterExpression, Expression.Constant(null));
            var isNotNullExpression = Expression.NotEqual(parameterExpression, Expression.Constant(null));

            // The switch-case now directly refers to 'valueAsString', skipping the null check
            return rule.Operator switch
            {
                FilterOperator.String.Contains =>
                    Expression.AndAlso(isNotNullExpression,
                        Expression.Call(parameterExpression, ContainsMethod!, Expression.Constant(valueAsString))),

                FilterOperator.String.NotContains =>
                    Expression.AndAlso(isNotNullExpression,
                        Expression.Not(Expression.Call(parameterExpression, ContainsMethod!, Expression.Constant(valueAsString)))),

                FilterOperator.String.Equal =>
                    Expression.AndAlso(isNotNullExpression,
                        Expression.Equal(parameterExpression, Expression.Constant(valueAsString))),

                FilterOperator.String.NotEqual =>
                    Expression.AndAlso(isNotNullExpression,
                        Expression.Not(Expression.Equal(parameterExpression, Expression.Constant(valueAsString)))),

                FilterOperator.String.StartsWith =>
                    Expression.AndAlso(isNotNullExpression,
                        Expression.Call(parameterExpression, StartsWithMethod!, Expression.Constant(valueAsString))),

                FilterOperator.String.EndsWith =>
                    Expression.AndAlso(isNotNullExpression,
                        Expression.Call(parameterExpression, EndsWithMethod!, Expression.Constant(valueAsString))),

                FilterOperator.String.Empty =>
                    Expression.OrElse(isNullExpression,
                        Expression.Equal(trimmedParameter, Expression.Constant(string.Empty))),

                FilterOperator.String.NotEmpty =>
                    Expression.AndAlso(isNotNullExpression,
                        Expression.NotEqual(trimmedParameter, Expression.Constant(string.Empty))),

                _ => Expression.Constant(true, typeof(bool))
            };
        }
        internal static Expression GenerateEnumFilterExpression<T>(AtomicPredicate<T> rule, Expression parameterExpression)
        {
            var propertyType = rule.MemberType;

            // Parse the enum value from the rule's Value property
            var enumValue = ParseToEnum(rule.Value, propertyType);

            // Create expressions to handle null and not null cases
            var nullExpression = Expression.Constant(null, typeof(T?));

            // Check if the property type is nullable enum
            if (IsNullableEnum(propertyType))
            {
                // Create a constant expression with the parsed enum value
                var enumConstantExpression = Expression.Constant(enumValue, Nullable.GetUnderlyingType(propertyType));

                // Filter operations for nullable enum type are "Is" and "IsNot"
                return rule.Operator switch
                {
                    // If operator is 'Is' and enum value is not null
                    FilterOperator.Enum.Is when rule.Value != null =>
                        Expression.OrElse(
                            Expression.NotEqual(parameterExpression, nullExpression),
                            Expression.Equal(Expression.Property(parameterExpression, "Value"), enumConstantExpression)
                        ),

                    // If operator is 'IsNot' and enum value is not null
                    FilterOperator.Enum.IsNot when rule.Value != null =>
                        Expression.AndAlso(
                            Expression.Equal(parameterExpression, nullExpression),
                            Expression.NotEqual(Expression.Property(parameterExpression, "Value"), enumConstantExpression)
                        ),

                    // For any other operator, return true, no filtering is performed
                    _ => Expression.Constant(true, typeof(bool))
                };
            }
            else
            {
                // Create a constant expression with the parsed enum value
                var enumConstantExpression = Expression.Constant(enumValue, propertyType);

                // Filter operations for non-nullable enum type are "Is" and "IsNot"
                return rule.Operator switch
                {
                    // If operator is 'Is' and enum value is not null
                    FilterOperator.Enum.Is when rule.Value != null =>
                        Expression.Equal(parameterExpression, enumConstantExpression),

                    // If operator is 'IsNot' and enum value is not null
                    FilterOperator.Enum.IsNot when rule.Value != null =>
                        Expression.NotEqual(parameterExpression, enumConstantExpression),

                    // If operator is 'Is' and enum value is null
                    FilterOperator.Enum.Is when rule.Value == null =>
                        Expression.Equal(parameterExpression, nullExpression),

                    // If operator is 'IsNot' and enum value is null
                    FilterOperator.Enum.IsNot when rule.Value == null =>
                        Expression.NotEqual(parameterExpression, nullExpression),

                    // For any other operator, return true, no filtering is performed
                    _ => Expression.Constant(true, typeof(bool))
                };
            }
        }

        internal static Expression GenerateNumericFilterExpression<T>(AtomicPredicate<T> rule, Expression parameterExpression)
        {
            // Parse the numeric value from the rule's Value property
            var numericValue = ParseToNullableDouble(rule.Value);

            // Filter operations for numeric type
            return rule.Operator switch
            {
                // If operator is 'Equal' and numeric value is not null
                FilterOperator.Number.Equal when numericValue is not null =>
                    Expression.Equal(parameterExpression, Expression.Constant(numericValue.Value, typeof(double))),

                // If operator is 'NotEqual' and numeric value is not null
                FilterOperator.Number.NotEqual when numericValue is not null =>
                    Expression.NotEqual(parameterExpression, Expression.Constant(numericValue.Value, typeof(double))),

                // If operator is 'GreaterThan' and numeric value is not null
                FilterOperator.Number.GreaterThan when numericValue is not null =>
                    Expression.GreaterThan(parameterExpression, Expression.Constant(numericValue.Value, typeof(double))),

                // If operator is 'GreaterThanOrEqual' and numeric value is not null
                FilterOperator.Number.GreaterThanOrEqual when numericValue is not null =>
                    Expression.GreaterThanOrEqual(parameterExpression, Expression.Constant(numericValue.Value, typeof(double))),

                // If operator is 'LessThan' and numeric value is not null
                FilterOperator.Number.LessThan when numericValue is not null =>
                    Expression.LessThan(parameterExpression, Expression.Constant(numericValue.Value, typeof(double))),

                // If operator is 'LessThanOrEqual' and numeric value is not null
                FilterOperator.Number.LessThanOrEqual when numericValue is not null =>
                    Expression.LessThanOrEqual(parameterExpression, Expression.Constant(numericValue.Value, typeof(double))),

                // If operator is 'Empty'
                FilterOperator.Number.Empty =>
                    Expression.Equal(parameterExpression, Expression.Constant(null, typeof(double))),

                // If operator is 'NotEmpty'
                FilterOperator.Number.NotEmpty =>
                    Expression.NotEqual(parameterExpression, Expression.Constant(null, typeof(double))),

                // For any other operator, or if numericValue is null, return true, no filtering is performed
                _ => Expression.Constant(true, typeof(bool))
            };
        }


        internal static Expression GenerateDateTimeFilterExpression<T>(AtomicPredicate<T> rule, Expression parameterExpression)
        {
            var propertyType = rule.MemberType;

            // Extract DateTime value from the rule's Value property
            var dateTimeValue = ParseToNullableDateTime(rule.Value);

            // Prepare a constant expression with the parsed DateTime value
            var dateTimeConstantExpression = Expression.Constant(dateTimeValue, propertyType);

            // If the data type is DateTime or DateTime?
            if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
            {
                // Create expressions to handle null cases for DateTime?
                var nullExpression = Expression.Constant(null, typeof(DateTime?));

                // Filter operations for DateTime type
                return rule.Operator switch
                {
                    // If operator is 'Is' and dateTimeValue is not null
                    FilterOperator.DateTime.Is when rule.Value != null =>
                        Expression.Equal(parameterExpression, dateTimeConstantExpression),

                    // If operator is 'IsNot' and dateTimeValue is not null
                    FilterOperator.DateTime.IsNot when rule.Value != null =>
                        Expression.NotEqual(parameterExpression, dateTimeConstantExpression),

                    // If operator is 'After' and dateTimeValue is not null
                    FilterOperator.DateTime.After when rule.Value != null =>
                        Expression.GreaterThan(parameterExpression, dateTimeConstantExpression),

                    // If operator is 'OnOrAfter' and dateTimeValue is not null
                    FilterOperator.DateTime.OnOrAfter when rule.Value != null =>
                        Expression.GreaterThanOrEqual(parameterExpression, dateTimeConstantExpression),

                    // If operator is 'Before' and dateTimeValue is not null
                    FilterOperator.DateTime.Before when rule.Value != null =>
                        Expression.LessThan(parameterExpression, dateTimeConstantExpression),

                    // If operator is 'OnOrBefore' and dateTimeValue is not null
                    FilterOperator.DateTime.OnOrBefore when rule.Value != null =>
                        Expression.LessThanOrEqual(parameterExpression, dateTimeConstantExpression),

                    // If operator is 'Empty' (only applicable for nullable DateTime)
                    FilterOperator.DateTime.Empty when propertyType == typeof(DateTime?) =>
                        Expression.Equal(parameterExpression, nullExpression),

                    // If operator is 'NotEmpty' (only applicable for nullable DateTime)
                    FilterOperator.DateTime.NotEmpty when propertyType == typeof(DateTime?) =>
                        Expression.NotEqual(parameterExpression, nullExpression),

                    // For any other operator, return true, no filtering is performed
                    _ => Expression.Constant(true, typeof(bool))
                };
            }

            // If the property type is neither DateTime nor DateTime?
            throw new ArgumentException($"Unhandled property type: {propertyType}");
        }       
        internal static Expression GenerateBooleanFilterExpression<T>(AtomicPredicate<T> rule, Expression parameterExpression)
        {
            var propertyType = rule.MemberType;

            // Extract boolean value from the rule's Value property
            var booleanValue = ParseToNullableBool(rule.Value);

            // If the data type is boolean or nullable boolean
            if (propertyType == typeof(bool) || propertyType == typeof(bool?))
            {
                // Prepare a constant expression with the parsed boolean value
                var booleanConstantExpression = Expression.Constant(booleanValue, propertyType);

                // Filter operations for Boolean type
                return rule.Operator switch
                {
                    // If operator is 'Is' and booleanValue is not null
                    FilterOperator.Enum.Is when rule.Value != null =>
                        Expression.Equal(parameterExpression, booleanConstantExpression),

                    // For any other operator, return true, no filtering is performed
                    _ => Expression.Constant(true, typeof(bool))
                };
            }

            // If the property type is neither bool nor bool?
            throw new NotSupportedException($"Unsupported property type: {propertyType}");
        }

        /// <summary>
        /// Generates an expression tree for a given compound predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="compoundPredicate"></param>
        /// <param name="parameterExpression"></param>
        /// <returns></returns>
        private Expression GenerateExpressionTree<T>(CompoundPredicate<T> compoundPredicate, ParameterExpression parameterExpression)
        {
            Expression combinedExpression = null;

            // Flag to determine if the operator is AND or OR
            bool isAndOperator = compoundPredicate.LogicalOperator == CompoundPredicateLogicalOperator.And;

            // Helper function to bind two expressions with a binary operator
            //Expression CombineExpressions(Expression left, Expression right) =>
            //    left == null ? right : (isAndOperator ? Expression.AndAlso(left, right) : Expression.OrElse(left, right));
            Expression CombineExpressions(Expression left, Expression right) =>
                right == null ? left : (left == null ? right : (isAndOperator ? Expression.AndAlso(left, right) : Expression.OrElse(left, right)));


            foreach (var predicate in compoundPredicate.GetPredicatesInOrder())
            {
                if (predicate is CompoundPredicate<T> nestedCompoundPredicate)
                {
                    var rightExpression = GenerateExpressionTree<T>(nestedCompoundPredicate, parameterExpression);
                    combinedExpression = CombineExpressions(combinedExpression, rightExpression);
                    continue;
                }

                if (predicate is AtomicPredicate<T> atomicPredicate)
                {
                    if (atomicPredicate.Operator == null || atomicPredicate.Value == null || atomicPredicate.MemberType == null)
                    {
                        continue;
                    }

                    var currentOperator = atomicPredicate.Operator;
                    var propertyExpression = Expression.Property(parameterExpression, atomicPredicate.Member);

                    if (currentOperator.Equals("is one of") || currentOperator.Equals("is not one of"))
                    {
                        combinedExpression = ProcessInclusionPredicate(combinedExpression, atomicPredicate, propertyExpression, CombineExpressions, currentOperator);
                    }
                    else
                    {
                        combinedExpression = ProcessTypeBasedPredicate(combinedExpression, atomicPredicate, propertyExpression, CombineExpressions);
                    }
                }
            }

            return combinedExpression;
        }


        private Expression ProcessInclusionPredicate<T>(Expression combinedExpression, AtomicPredicate<T> atomicPredicate, MemberExpression propertyExpression, Func<Expression, Expression, Expression> combineExpressions, string currentOperator)
        {
            var predicateValues = atomicPredicate.Value as string;
            var propertyType = atomicPredicate.MemberType;
            var containsMethod = _methodContains.MakeGenericMethod(propertyType);
            var valueList = predicateValues.ToString().Split(',').Select(v => v.Trim()).ToList();

            var genericListType = typeof(List<>).MakeGenericType(propertyType);
            var predicateValueList = (IList)Activator.CreateInstance(genericListType);

            foreach (var value in valueList)
            {
                if (TypeIdentifier.IsEnum(propertyType))
                {
                    var nullableEnumType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
                    predicateValueList.Add(Enum.Parse(nullableEnumType, value));
                }
                else
                {
                    predicateValueList.Add(value);
                }
            }

            Expression rightExpression;
            if (currentOperator.Equals("is not one of"))
            {
                rightExpression = Expression.Not(Expression.Call(containsMethod, Expression.Constant(predicateValueList), propertyExpression));
            }
            else
            {
                rightExpression = Expression.Call(containsMethod, Expression.Constant(predicateValueList), propertyExpression);
            }

            return combineExpressions(combinedExpression, rightExpression);
        }

        private Expression ProcessTypeBasedPredicate<T>(Expression combinedExpression, AtomicPredicate<T> atomicPredicate, MemberExpression propertyExpression, Func<Expression, Expression, Expression> combineExpressions)
        {
            Expression predicateExpression;
            var propertyType = propertyExpression.Type;

            if (propertyType == typeof(string))
            {
                predicateExpression = GenerateStringFilterExpression<T>(atomicPredicate, propertyExpression);
            }
            else if (TypeIdentifier.IsEnum(propertyType))
            {
                predicateExpression = GenerateEnumFilterExpression<T>(atomicPredicate, propertyExpression);
            }
            else if (TypeIdentifier.IsNumber(propertyType))
            {
                predicateExpression = GenerateNumericFilterExpression<T>(atomicPredicate, propertyExpression);
            }
            else if (TypeIdentifier.IsBoolean(propertyType))
            {
                predicateExpression = GenerateBooleanFilterExpression<T>(atomicPredicate, propertyExpression);
            }
            else if (TypeIdentifier.IsDateTime(propertyType))
            {
                predicateExpression = GenerateDateTimeFilterExpression<T>(atomicPredicate, propertyExpression);
            }
            else
            {
                throw new ArgumentException("Unhandled property type");
            }

            return combineExpressions(combinedExpression, predicateExpression);
        }



        /// <summary>
        /// Compiles the root compound predicate into a function
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rootPredicate"></param>
        /// <returns></returns>
        public Func<T, bool>? CompilePredicateFunction<T>(CompoundPredicate<T> rootPredicate)
        {
            var parameterExpression = Expression.Parameter(typeof(T), "x");
            var expressionTree = GenerateExpressionTree(rootPredicate, parameterExpression);

            if (expressionTree is not null)
            {
                var lambda = Expression.Lambda<Func<T, bool>>(expressionTree, parameterExpression);
                return lambda.Compile();
            }

            return null;
        }

        /// <summary>
        /// Compiles the root compound predicate into an expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rootPredicate"></param>
        /// <returns></returns>
        public Expression<Func<T, bool>>? CompilePredicateExpression<T>(CompoundPredicate<T>? rootPredicate)
        {

            // Generate parameter for the lambda expression
            var parameterExpression = Expression.Parameter(typeof(T), "item");

            // Generate the body of the lambda expression
            var expressionBody = GenerateExpressionTree(rootPredicate, parameterExpression);

            // Check if an expression body was generated
            if (expressionBody is null)
            {
                expressionBody = Expression.Constant(true);
            }

            // Create a lambda expression with the generated body and parameter
            var lambdaExpression = Expression.Lambda<Func<T, bool>>(expressionBody, parameterExpression);

            return lambdaExpression;
        }


        /// <summary>
        /// Attempts to parse the input object into a list of strings.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static List<string>? ParseToListOfString(object? input)
        {
            if (input is null)
            {
                return null;
            }

            // Try to parse the input into a list of strings.
            // Assumes that input is a comma-separated string
            var stringList = input.ToString()?.Split(',').Select(v => v.Trim()).ToList();
            return stringList;
        }

        /// <summary>
        /// Attempts to parse the input object into a nullable DateTime.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static DateTime? ParseToNullableDateTime(object? input)
        {
            if (input is null)
            {
                return null;
            }

            // Convert the input to a DateTime and normalize it to UTC.
            return Convert.ToDateTime(input).ToUniversalTime();
        }

        /// <summary>
        /// Attempts to parse the input object into a nullable bool.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool? ParseToNullableBool(object? input)
        {
            if (input is null)
            {
                return null;
            }

            // Convert the input to a boolean.
            return Convert.ToBoolean(input);
        }

        /// <summary>
        /// Parses the input object into an enum of the specified type.
        /// </summary>
        /// <param name="enumStringValue"></param>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static object? ParseToEnum(object? enumStringValue, Type enumType)
        {
            // If null, return null
            if (enumStringValue is null)
            {
                return null;
            }

            // Parse the input string to the specified enum type.
            return Enum.Parse(enumType, enumStringValue.ToString());
        }

        /// <summary>
        /// Attempts to parse the input object into a nullable double.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static double? ParseToNullableDouble(object? input)
        {
            if (input is null)
            {
                return null;
            }

            // Convert the input to a double.
            return Convert.ToDouble(input);
        }
    }
    public enum Condition
    {
        AND,
        OR
    }
}
