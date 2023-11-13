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

        private static bool IsNullableEnum(Type? type)
        {
            return Nullable.GetUnderlyingType(type)?.IsEnum == true;
        }

        internal static Expression GenerateStringFilterExpression<T>(AtomicPredicate<T> rule, Expression parameterExpression)
        {
            var valueString = FieldType.ConvertToString(rule.Value);

            // Invoking 'Trim' method on 'parameterExpression' and storing in 'trimmedParameter'
            var trimmedParameter = Expression.Call(parameterExpression, TrimMethod!);

            // Creating expressions to check if 'parameterExpression' is null or not
            var isNullExpression = Expression.Equal(parameterExpression, Expression.Constant(null));
            var isNotNullExpression = Expression.NotEqual(parameterExpression, Expression.Constant(null));

            // The switch-case now directly refers to 'valueString', skipping the null check
            return rule.Operator switch
            {
                FilterOperator.String.Contains =>
                    Expression.AndAlso(isNotNullExpression,
                        Expression.Call(parameterExpression, ContainsMethod!, Expression.Constant(valueString))),

                FilterOperator.String.NotContains =>
                    Expression.AndAlso(isNotNullExpression,
                        Expression.Not(Expression.Call(parameterExpression, ContainsMethod!, Expression.Constant(valueString)))),

                FilterOperator.String.Equal =>
                    Expression.AndAlso(isNotNullExpression,
                        Expression.Equal(parameterExpression, Expression.Constant(valueString))),

                FilterOperator.String.NotEqual =>
                    Expression.AndAlso(isNotNullExpression,
                        Expression.Not(Expression.Equal(parameterExpression, Expression.Constant(valueString)))),

                FilterOperator.String.StartsWith =>
                    Expression.AndAlso(isNotNullExpression,
                        Expression.Call(parameterExpression, StartsWithMethod!, Expression.Constant(valueString))),

                FilterOperator.String.EndsWith =>
                    Expression.AndAlso(isNotNullExpression,
                        Expression.Call(parameterExpression, EndsWithMethod!, Expression.Constant(valueString))),

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
                        Expression.Equal(Expression.Property(parameterExpression, "Value"), enumConstantExpression),

                    // If operator is 'IsNot' and enum value is not null
                    FilterOperator.Enum.IsNot when rule.Value != null =>
                        Expression.NotEqual(Expression.Property(parameterExpression, "Value"), enumConstantExpression),

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
            if(rule.MemberType is null)
            {
                throw new InvalidOperationException("Rule MemberType is null");
            }


            // Parse the numeric value from the rule's Value property
            // Convert it to the specific type as indicated by rule.MemberType
            var numericValue = Convert.ChangeType(rule.Value, rule.MemberType);

            // Filter operations for numeric type
            return rule.Operator switch
            {
                // If operator is 'Equal' and numeric value is not null
                FilterOperator.Number.Equal when numericValue is not null =>
                    Expression.Equal(parameterExpression, Expression.Constant(numericValue, rule.MemberType)),

                // If operator is 'NotEqual' and numeric value is not null
                FilterOperator.Number.NotEqual when numericValue is not null =>
                    Expression.NotEqual(parameterExpression, Expression.Constant(numericValue, rule.MemberType)),

                // If operator is 'GreaterThan' and numeric value is not null
                FilterOperator.Number.GreaterThan when numericValue is not null =>
                    Expression.GreaterThan(parameterExpression, Expression.Constant(numericValue, rule.MemberType)),

                // If operator is 'GreaterThanOrEqual' and numeric value is not null
                FilterOperator.Number.GreaterThanOrEqual when numericValue is not null =>
                    Expression.GreaterThanOrEqual(parameterExpression, Expression.Constant(numericValue, rule.MemberType)),

                // If operator is 'LessThan' and numeric value is not null
                FilterOperator.Number.LessThan when numericValue is not null =>
                    Expression.LessThan(parameterExpression, Expression.Constant(numericValue, rule.MemberType)),

                // If operator is 'LessThanOrEqual' and numeric value is not null
                FilterOperator.Number.LessThanOrEqual when numericValue is not null =>
                    Expression.LessThanOrEqual(parameterExpression, Expression.Constant(numericValue, rule.MemberType)),

                // If operator is 'Empty'
                FilterOperator.Number.Empty =>
                    Expression.Equal(parameterExpression, Expression.Constant(null, rule.MemberType)),

                // If operator is 'NotEmpty'
                FilterOperator.Number.NotEmpty =>
                    Expression.NotEqual(parameterExpression, Expression.Constant(null, rule.MemberType)),

                // For any other operator, or if numericValue is null, return true, no filtering is performed
                _ => Expression.Constant(true, typeof(bool))
            };
        }
        internal static Expression GenerateDateTimeFilterExpression<T>(AtomicPredicate<T> rule, Expression parameterExpression)
        {
            var propertyType = rule.MemberType;

            // Extract DateTime value from the rule's Value property
            var dateTimeValue = FieldType.ConvertToDateTime(rule.Value);

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
            var booleanValue = FieldType.ConvertToBoolean(rule.Value);

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
        internal static Expression GenerateGuidFilterExpression<T>(AtomicPredicate<T> rule, Expression parameterExpression)
        {
            var valueGuid = FieldType.ConvertToGuid(rule.Value);

            // Filter operations for Guid type
            return rule.Operator switch
            {

                FilterOperator.Guid.Equal when valueGuid is not null =>
                    Expression.Equal(parameterExpression, Expression.Constant(valueGuid, typeof(Guid))),
                
                FilterOperator.Guid.NotEqual when valueGuid is not null =>
                    Expression.Not(Expression.Equal(parameterExpression, Expression.Constant(valueGuid, typeof(Guid)))),

                FilterOperator.Guid.Empty =>
                    Expression.Equal(parameterExpression, Expression.Constant(Guid.Empty, parameterExpression.Type)),

                FilterOperator.Guid.NotEmpty =>
                    Expression.NotEqual(parameterExpression, Expression.Constant(Guid.Empty, parameterExpression.Type)),

                _ => Expression.Constant(true, typeof(bool))
            };
        }




        /// <summary>
        /// Generates an expression tree for a given compound predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="compoundPredicate"></param>
        /// <param name="parameterExpression"></param>
        /// <returns></returns>
        private Expression? GenerateExpressionTree<T>(CompoundPredicate<T> compoundPredicate, ParameterExpression parameterExpression)
        {
            Expression? combinedExpression = null;

            // Flag to determine if the operator is AND or OR
            bool isAndOperator = compoundPredicate.LogicalOperator == CompoundPredicateLogicalOperator.And;

            // Helper function to bind two expressions with a binary operator
            Expression? CombineExpressions(Expression? left, Expression? right)
            {
                // If both are null, return null
                if (left is null && right is null)
                {
                    return null;
                }

                // If only left is null, return right
                if (left is null)
                {
                    return right;
                }

                // If only right is null, return left
                if (right is null)
                {
                    return left;
                }

                // Otherwise, combine them with the logical operator
                if (isAndOperator)
                {
                    return Expression.AndAlso(left, right);
                }
                else
                {
                    return Expression.OrElse(left, right);
                }
            }



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
                    if (atomicPredicate.Operator is null || atomicPredicate.MemberType is null)
                    {
                        continue;
                    }

                    // Operators "is empty" and "is not empty" should work with Value == null
                    if (atomicPredicate.Value is null && atomicPredicate.Operator != "is empty" && atomicPredicate.Operator != "is not empty")
                    {
                        continue;
                    }

                    var currentOperator = atomicPredicate.Operator;
                    var propertyExpression = GenerateNestedPropertyExpression(parameterExpression, atomicPredicate.Member);

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

        private MemberExpression? GenerateNestedPropertyExpression(ParameterExpression param, string? property)
        {
            if (string.IsNullOrWhiteSpace(property))
            {
                return null;
            }

            string[] parts = property.Split('.');
            Expression body = param;
            foreach (string part in parts)
            {
                body = Expression.Property(body, part);
            }

            return body as MemberExpression;
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

        private static Expression ProcessTypeBasedPredicate<T>(Expression combinedExpression, AtomicPredicate<T> atomicPredicate, MemberExpression propertyExpression, Func<Expression, Expression, Expression> combineExpressions)
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
            else if (TypeIdentifier.IsGuid(propertyType))
            {
                predicateExpression = GenerateGuidFilterExpression<T>(atomicPredicate, propertyExpression);
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

            // Check if the provided type is a nullable enum
            Type typeToParse = Nullable.GetUnderlyingType(enumType) ?? enumType;

            if (!typeToParse.IsEnum)
            {
                throw new ArgumentException("Provided type is not an enum or nullable enum.", nameof(enumType));
            }

            // Explicitly handle the nullable string scenario
            string valueToParse = enumStringValue.ToString();
            if (valueToParse == null)
            {
                throw new InvalidOperationException("Unexpected null value.");
            }

            return Enum.Parse(typeToParse, valueToParse);
        }


        public static MemberExpression? GetMemberExpression<T>(Expression<Func<T, object>>? expression)
        {
            return expression?.Body switch
            {
                MemberExpression memberExpression => memberExpression,
                UnaryExpression unaryExpression when unaryExpression.Operand is MemberExpression => unaryExpression.Operand as MemberExpression,
                _ => null
            };
        }

        public static string GetFullPropertyName<T>(Expression<Func<T, object>>? propertyExpression)
        {
            if (propertyExpression.Body is UnaryExpression unaryExpression)
            {
                return GetFullPropertyPath(unaryExpression.Operand);
            }
            else
            {
                return GetFullPropertyPath(propertyExpression.Body);
            }
        }

        public static string GetFullPropertyPath(Expression? expression)
        {
            if (expression is MemberExpression memberExpression)
            {
                var previousPath = GetFullPropertyPath(memberExpression.Expression);

                if (string.IsNullOrWhiteSpace(previousPath))
                {
                    return memberExpression.Member.Name;
                }
                else
                {
                    return previousPath + "." + memberExpression.Member.Name;
                }
            }
            else
            {
                return string.Empty;
            }
        }


    }
    public enum Condition
    {
        AND,
        OR
    }
}
