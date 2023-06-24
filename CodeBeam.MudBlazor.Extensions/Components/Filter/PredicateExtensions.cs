using System.Linq.Expressions;

namespace MudExtensions
{
    public static class PredicateExtensions
    {
        public static Expression<Func<T, bool>> ToExpression<T>(this CompoundPredicate<T> compoundPredicate)
        {
            var param = Expression.Parameter(typeof(T));

            var body = compoundPredicate.GetPredicatesInOrder()
                                        .Select(predicate => ToExpression<T>(predicate, param))
                                        .Aggregate<Expression, BinaryExpression>(null, (accumulated, next) =>
                                        {
                                            if (accumulated == null)
                                            {
                                                return (BinaryExpression)next;
                                            }

                                            return compoundPredicate.LogicalOperator switch
                                            {
                                                CompoundPredicateLogicalOperator.And => Expression.AndAlso(accumulated, next),
                                                CompoundPredicateLogicalOperator.Or => Expression.OrElse(accumulated, next),
                                                _ => throw new NotSupportedException($"Unsupported logical operator: {compoundPredicate.LogicalOperator}")
                                            };
                                        });

            return Expression.Lambda<Func<T, bool>>(body, param);
        }

        private static Expression ToExpression<T>(PredicateUnit<T> predicate, ParameterExpression param)
        {
            switch (predicate)
            {
                case AtomicPredicate<T> atomicPredicate:
                    var member = Expression.Property(param, atomicPredicate.Member);
                    var constant = Expression.Constant(atomicPredicate.Value, atomicPredicate.MemberType);
                    return atomicPredicate.Operator switch
                    {
                        "==" => Expression.Equal(member, constant),
                        "!=" => Expression.NotEqual(member, constant),
                        ">" => Expression.GreaterThan(member, constant),
                        "<" => Expression.LessThan(member, constant),
                        ">=" => Expression.GreaterThanOrEqual(member, constant),
                        "<=" => Expression.LessThanOrEqual(member, constant),
                        _ => throw new NotSupportedException($"Unsupported operator: {atomicPredicate.Operator}")
                    };
                case CompoundPredicate<T> compoundPredicate:
                    return compoundPredicate.ToExpression().Body;
                default:
                    throw new NotSupportedException($"Unsupported predicate type: {predicate.GetType()}");
            }
        }
    }

}
