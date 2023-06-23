using System.Linq.Expressions;

namespace MudExtensions
{
#nullable enable
    public abstract class PredicateUnit<T>
    {
        public Guid Id { get; set; }

        public PredicateUnit<T>? Parent { get; set; }

        protected PredicateUnit(PredicateUnit<T>? parent)
            : base()
        {
            Parent = parent;
        }
    }

    public class AtomicPredicate<T> : PredicateUnit<T>
    {
        public AtomicPredicate(PredicateUnit<T> parent)
            : base(parent)
        {
        }

        public string? ValueString { get; set; } = null;
        public double? ValueNumber { get; set; } = null;
        public Enum? ValueEnum { get; set; } = null;
        public bool? ValueBool { get; set; } = null;
        public DateTime? ValueDate { get; set; } = null;
        public TimeSpan? ValueTime { get; set; } = null;
        public object? Value { get; set; } = null;

        public string? Operator { get; set; } = null;

        public bool IsMultSelect { get; set; } = false;
        public IEnumerable<string> MultiSelectValues { get; set; } = new HashSet<string>();

        public Expression<Func<T, object>> PropertyExpression { get; set; }

        public string Member
        {
            get
            {
                return GetMemberName(PropertyExpression);
            }
        }

        public Type? MemberType
        {
            get
            {
                return GetMemberType(PropertyExpression);
            }
        }

        private string GetMemberName(Expression<Func<T, object>> expression)
        {
            if (expression.Body is MemberExpression member)
            {
                return GetFullMemberName(member);
            }

            if (expression.Body is UnaryExpression unary)
            {
                return GetFullMemberName((MemberExpression)unary.Operand);
            }

            throw new ArgumentException("Could not get the member name.");
        }

        private string GetFullMemberName(MemberExpression member)
        {
            if (member.Expression.NodeType == ExpressionType.MemberAccess)
            {
                return GetFullMemberName((MemberExpression)member.Expression) + "." + member.Member.Name;
            }

            return member.Member.Name;
        }

        private Type? GetMemberType(Expression<Func<T, object>>? expression)
        {
            if (expression?.Body is MemberExpression member)
            {
                return member.Type;
            }

            if (expression?.Body is UnaryExpression unary)
            {
                return unary.Operand.Type;
            }

            return null;
        }
    }

    public class CompoundPredicate<T> : PredicateUnit<T>
    {
        public CompoundPredicateLogicalOperator LogicalOperator { get; set; }
        public List<PredicateUnit<T>> FilterDescriptors { get; set; }

        public CompoundPredicate(PredicateUnit<T>? parent)
            : base(parent)
        {
            FilterDescriptors = new List<PredicateUnit<T>>();
        }
    }

    public enum CompoundPredicateLogicalOperator
    {
        And,
        Or
    }
}
