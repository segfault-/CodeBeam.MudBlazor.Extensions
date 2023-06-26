using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;

namespace MudExtensions
{
#nullable enable
    public class ExpressionGenerator
    {
        private readonly MethodInfo _methodContains = typeof(Enumerable)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .Single(m => m.Name == nameof(Enumerable.Contains) && m.GetParameters().Length == 2);

        private delegate Expression Binder(Expression left, Expression right);

        private Expression ParseTree<T>(CompoundPredicate<T> compoundPredicate, ParameterExpression parm)
        {
            Expression left = null;

            var binder = compoundPredicate.LogicalOperator == CompoundPredicateLogicalOperator.And ? (Binder)Expression.And : Expression.Or;

            Expression Bind(Expression left, Expression right) =>
                left == null ? right : binder(left, right);

            foreach (var predicateUnit in compoundPredicate.GetPredicatesInOrder())
            {


                if (predicateUnit is CompoundPredicate<T> childCompoundPredicate)
                {

                    var right = ParseTree<T>(childCompoundPredicate, parm);
                    left = Bind(left, right);
                    continue;

                }

                if (predicateUnit is AtomicPredicate<T> atomicPredicate)
                {

                    if(atomicPredicate.Operator is null || atomicPredicate.Value is null || atomicPredicate.MemberType is null)
                    {
                        continue;
                    }


                    var @operator = atomicPredicate.Operator;
                    var property = Expression.Property(parm, atomicPredicate.Member);

                    if (@operator.Equals("is one of") || @operator.Equals("is not one of"))
                    {
                        var jsonElement = atomicPredicate.Value as string;
                        var propertyType = atomicPredicate.MemberType;
                        var contains = _methodContains.MakeGenericMethod(propertyType);

                        var val = jsonElement.ToString().Split(',').Select(v => v.Trim()).ToList();

                        var genericListType = typeof(List<>).MakeGenericType(propertyType);
                        var listy = (IList)Activator.CreateInstance(genericListType);

                        foreach (var s in val)
                        {
                            if (TypeIdentifier.IsEnum(propertyType))
                            {
                                var nullableEnumType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
                                listy.Add(Enum.Parse(nullableEnumType, s));
                            }
                            else
                            {
                                listy.Add(s);
                            }

                        }

                        if (@operator.Equals("is not one of"))
                        {
                            var right = Expression.Not(Expression.Call(
                                contains,
                                Expression.Constant(listy),
                                property));
                            left = Bind(left, right);
                        }
                        else
                        {
                            var right = Expression.Call(
                                contains,
                                Expression.Constant(listy),
                                property);
                            left = Bind(left, right);
                        }
                    }
                    else
                    {
                        Expression expression = null;
                        if (property.Type == typeof(string))
                        {
                            expression = GenerateFilterExpressionForStringType<T>(atomicPredicate, property);
                        }
                        else if (TypeIdentifier.IsEnum(property.Type))
                        {
                            expression = GenerateFilterExpressionForEnumType<T>(atomicPredicate, property);
                        }
                        else if (TypeIdentifier.IsNumber(property.Type))
                        {
                            expression = GenerateFilterExpressionForNumericType<T>(atomicPredicate, property);
                        }
                        else if (TypeIdentifier.IsBoolean(property.Type))
                        {
                            expression = GenerateFilterExpressionForBooleanType<T>(atomicPredicate, property);
                        }
                        else if (TypeIdentifier.IsDateTime(property.Type))
                        {
                            expression = GenerateFilterExpressionForDateTimeType<T>(atomicPredicate, property);
                        }
                        else
                        {
                            throw new ArgumentException("Unhandled property type");
                        }



                        left = Bind(left, expression);
                    }
                }
            }


            return left;
        }

        private static Expression GenerateFilterExpressionForStringType<T>(AtomicPredicate<T> rule, Expression parameter)
        {
            var dataType = rule.MemberType;
            var field = parameter;
            var valueString = rule.Value?.ToString();
            var trim = Expression.Call(field, dataType.GetMethod("Trim", Type.EmptyTypes));
            var isnull = Expression.Equal(field, Expression.Constant(null));
            var isnotnull = Expression.NotEqual(field, Expression.Constant(null));

            return rule.Operator switch
            {
                FilterOperator.String.Contains when rule.Value != null =>
                    Expression.AndAlso(isnotnull,
                        Expression.Call(field, dataType.GetMethod("Contains", new[] { dataType }), Expression.Constant(valueString))),

                FilterOperator.String.NotContains when rule.Value != null =>
                    Expression.AndAlso(isnotnull,
                        Expression.Not(Expression.Call(field, dataType.GetMethod("Contains", new[] { dataType }), Expression.Constant(valueString)))),

                FilterOperator.String.Equal when rule.Value != null =>
                    Expression.AndAlso(isnotnull,
                        Expression.Equal(field, Expression.Constant(valueString))),

                FilterOperator.String.NotEqual when rule.Value != null =>
                    Expression.AndAlso(isnotnull,
                    Expression.Not(Expression.Equal(field, Expression.Constant(valueString)))),

                FilterOperator.String.StartsWith when rule.Value != null =>
                    Expression.AndAlso(isnotnull,
                        Expression.Call(field, dataType.GetMethod("StartsWith", new[] { dataType }), Expression.Constant(valueString))),

                FilterOperator.String.EndsWith when rule.Value != null =>
                    Expression.AndAlso(isnotnull,
                        Expression.Call(field, dataType.GetMethod("EndsWith", new[] { dataType }), Expression.Constant(valueString))),

                FilterOperator.String.Empty =>
                    Expression.OrElse(isnull,
                        Expression.Equal(trim, Expression.Constant(string.Empty, dataType))),

                FilterOperator.String.NotEmpty =>
                    Expression.AndAlso(isnotnull,
                        Expression.NotEqual(trim, Expression.Constant(string.Empty, dataType))),

                _ => Expression.Constant(true, typeof(bool))
            };
        }

        private static Expression GenerateFilterExpressionForEnumType<T>(AtomicPredicate<T> rule, Expression parameter)
        {
            var dataType = rule.MemberType;
            var field = parameter;
            var valueEnum = GetEnumFromObject(rule.Value, dataType);
            var @null = Expression.Convert(Expression.Constant(null), dataType);
            var isnull = Expression.Equal(field, @null);
            var isnotnull = Expression.NotEqual(field, @null);
            var valueEnumConstant = Expression.Convert(Expression.Constant(valueEnum), dataType);

            return rule.Operator switch
            {
                FilterOperator.Enum.Is when rule.Value != null =>
                    IsNullableEnum(dataType) ? Expression.AndAlso(isnotnull,
                            Expression.Equal(field, valueEnumConstant))
                        : Expression.Equal(field, valueEnumConstant),

                FilterOperator.Enum.IsNot when rule.Value != null =>
                    IsNullableEnum(dataType) ? Expression.OrElse(isnull,
                            Expression.NotEqual(field, valueEnumConstant))
                        : Expression.NotEqual(field, valueEnumConstant),

                _ => Expression.Constant(true, typeof(bool))
            };

        }

        private static Expression GenerateFilterExpressionForNumericType<T>(AtomicPredicate<T> rule, Expression parameter)
        {
            var dataType = rule.MemberType;
            var field = Expression.Convert(parameter, typeof(double?));
            var valueNumber = GetDoubleFromObject(rule.Value);
            var notNullNumber = Expression.Convert(field, typeof(double?));
            var valueNumberConstant = Expression.Constant(valueNumber, typeof(double?));


            return rule.Operator switch
            {
                FilterOperator.Number.Equal when rule.Value != null =>
                        Expression.Equal(notNullNumber, valueNumberConstant),

                FilterOperator.Number.NotEqual when rule.Value != null =>
                        Expression.NotEqual(notNullNumber, valueNumberConstant),

                FilterOperator.Number.GreaterThan when rule.Value != null =>
                        Expression.GreaterThan(notNullNumber, valueNumberConstant),

                FilterOperator.Number.GreaterThanOrEqual when rule.Value != null =>
                        Expression.GreaterThanOrEqual(notNullNumber, valueNumberConstant),

                FilterOperator.Number.LessThan when rule.Value != null =>
                        Expression.LessThan(notNullNumber, valueNumberConstant),

                FilterOperator.Number.LessThanOrEqual when rule.Value != null =>
                        Expression.LessThanOrEqual(notNullNumber, valueNumberConstant),

                FilterOperator.Number.Empty =>
                    Expression.Equal(field, Expression.Constant(null, typeof(double?))),

                FilterOperator.Number.NotEmpty =>
                    Expression.NotEqual(field, Expression.Constant(null, typeof(double?))),

                _ => Expression.Constant(true, typeof(bool))
            };
        }

        private static Expression GenerateFilterExpressionForDateTimeType<T>(AtomicPredicate<T> rule, Expression parameter)
        {
            var dataType = rule.MemberType;
            if (dataType == typeof(DateTime))
            {
                var field = parameter;
                var valueDateTime = GetDateTimeFromObject(rule.Value);
                var notNullDateTime = Expression.Convert(field, typeof(DateTime));
                var valueDateTimeConstant = Expression.Constant(valueDateTime);

                return rule.Operator switch
                {
                    FilterOperator.DateTime.Is when null != rule.Value =>
                            Expression.Equal(notNullDateTime, valueDateTimeConstant),

                    FilterOperator.DateTime.IsNot when null != rule.Value =>
                            Expression.NotEqual(notNullDateTime, valueDateTimeConstant),

                    FilterOperator.DateTime.After when null != rule.Value =>
                            Expression.GreaterThan(notNullDateTime, valueDateTimeConstant),

                    FilterOperator.DateTime.OnOrAfter when null != rule.Value =>
                            Expression.GreaterThanOrEqual(notNullDateTime, valueDateTimeConstant),

                    FilterOperator.DateTime.Before when null != rule.Value =>
                            Expression.LessThan(notNullDateTime, valueDateTimeConstant),

                    FilterOperator.DateTime.OnOrBefore when null != rule.Value =>
                            Expression.LessThanOrEqual(notNullDateTime, valueDateTimeConstant),

                    _ => Expression.Constant(true, typeof(bool))
                };
            }
            else if (dataType == typeof(DateTime?))
            {
                var field = parameter;
                var valueDateTime = GetDateTimeFromObject(rule.Value);
                var notNullDateTime = Expression.Convert(field, typeof(DateTime?));
                var valueDateTimeConstant = Expression.Constant(valueDateTime, typeof(DateTime?));

                return rule.Operator switch
                {
                    FilterOperator.DateTime.Is when null != rule.Value =>
                        Expression.Equal(notNullDateTime, valueDateTimeConstant),

                    FilterOperator.DateTime.IsNot when null != rule.Value =>
                        Expression.NotEqual(notNullDateTime, valueDateTimeConstant),

                    FilterOperator.DateTime.After when null != rule.Value =>
                        Expression.GreaterThan(notNullDateTime, valueDateTimeConstant),

                    FilterOperator.DateTime.OnOrAfter when null != rule.Value =>
                        Expression.GreaterThanOrEqual(notNullDateTime, valueDateTimeConstant),

                    FilterOperator.DateTime.Before when null != rule.Value =>
                        Expression.LessThan(notNullDateTime, valueDateTimeConstant),

                    FilterOperator.DateTime.OnOrBefore when null != rule.Value =>
                        Expression.LessThanOrEqual(notNullDateTime, valueDateTimeConstant),

                    FilterOperator.DateTime.Empty =>
                        Expression.Equal(field, Expression.Constant(null, typeof(DateTime?))),

                    FilterOperator.DateTime.NotEmpty =>
                        Expression.NotEqual(field, Expression.Constant(null, typeof(DateTime?))),

                    _ => Expression.Constant(true, typeof(bool))
                };
            }
            else
            {
                throw new ArgumentException("uhandled dataType");
            }



        }

        private static Expression GenerateFilterExpressionForBooleanType<T>(AtomicPredicate<T> rule, Expression parameter)
        {
            var dataType = rule.MemberType;

            if (dataType == typeof(bool))
            {
                var field = Expression.Convert(parameter, typeof(bool));
                var valueBool = GetBooleanFromObject(rule.Value);
                var notNullBool = Expression.Convert(field, typeof(bool));

                return rule.Operator switch
                {
                    FilterOperator.Enum.Is when rule.Value != null =>
                        Expression.Equal(notNullBool, Expression.Constant(valueBool, typeof(bool))),

                    _ => Expression.Constant(true, typeof(bool))
                };

            }
            else if (dataType == typeof(bool?))
            {
                var field = Expression.Convert(parameter, typeof(bool?));
                var valueBool = GetBooleanFromObject(rule.Value);
                var notNullBool = Expression.Convert(field, typeof(bool?));

                return rule.Operator switch
                {
                    FilterOperator.Enum.Is when rule.Value != null =>
                        Expression.Equal(notNullBool, Expression.Constant(valueBool, typeof(bool?))),

                    _ => Expression.Constant(true, typeof(bool))
                };
            }
            else
            {
                throw new NotSupportedException("The property type is not supported");
            }
        }



        private static bool IsNullableEnum(Type t)
        {
            Type u = Nullable.GetUnderlyingType(t);
            return (u != null) && u.IsEnum;
        }

        public static List<string> GetListFromObject(object o)
        {
            if (o == null)
                return null;

            if (o is JsonElement element)
            {
                // return element.EnumerateArray().Select(e => e.GetString()).ToList();
                var x = element.ToString().Split(',').Select(v => v.Trim()).ToList();
                return x;
            }
            else
            {
                return null;

            }
        }

        public static DateTime? GetDateTimeFromObject(object o)
        {
            if (o == null)
                return null;

            if (o is JsonElement element)
            {
                return (DateTime?)Convert.ToDateTime(element.ToString()).ToUniversalTime();
            }
            else
            {
                return (DateTime?)Convert.ToDateTime(o).ToUniversalTime();
            }
        }

        public static bool? GetBooleanFromObject(object o)
        {
            if (o == null)
                return null;

            if (o is JsonElement element)
            {
                return (bool?)Convert.ToBoolean(element.ToString());
            }
            else
            {
                return (bool?)Convert.ToBoolean(o);
            }
        }

        public static Enum GetEnumFromObject(object o, Type t)
        {
            if (o == null)
                return null;

            var enumType = Nullable.GetUnderlyingType(t) ?? t;
            if (o is JsonElement element)
            {
                return (Enum)Enum.ToObject(enumType, element.GetInt32());
            }
            else if (enumType != null)
            {
                return (Enum)Enum.ToObject(enumType, o);
            }
            else
            {
                return (Enum)Enum.ToObject(t, o);
            }
        }

        public static double? GetDoubleFromObject(object o)
        {
            if (o == null)
                return null;

            if (o is JsonElement element)
            {
                return (double?)Convert.ToDouble(element.ToString());
            }
            else
            {
                return (double?)Convert.ToDouble(o);
            }
        }

        public Expression<Func<T, bool>> ParseExpressionOf<T>(CompoundPredicate<T>? root)
        {
            Expression<Func<T, bool>>? query = item => true;
            var itemExpression = Expression.Parameter(typeof(T));
            var conditions = ParseTree<T>(root, itemExpression);
            if (conditions != null)
            {
                if (conditions.CanReduce)
                {
                    conditions = conditions.ReduceAndCheck();
                }

                Console.WriteLine(conditions?.ToString());

                query = Expression.Lambda<Func<T, bool>>(conditions, itemExpression);
            }

            return query;
        }

        public Func<T, bool> ParsePredicateOf<T>(CompoundPredicate<T> root)
        {
            var query = ParseExpressionOf<T>(root);
            if (query != null)
            {
                return query.Compile();
            }
            else
            {
                return null;
            }

        }
    }

    public enum Condition
    {
        AND,
        OR
    }
}
