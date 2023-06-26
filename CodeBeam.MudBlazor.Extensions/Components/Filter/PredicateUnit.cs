using System.Linq.Expressions;

namespace MudExtensions
{
#nullable enable

    /// <summary>
    /// Base class for predicate units
    /// </summary>
    public abstract class PredicateUnit<T>
    {
        protected PredicateUnit(PredicateUnit<T>? parent)
        {
            Parent = parent;
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public abstract PredicateUnit<T>? Parent { get; set; }

        public abstract bool? RemovePredicate(PredicateUnit<T> predicate);
    }

    /// <summary>
    /// Represents atomic predicates
    /// </summary>
    public class AtomicPredicate<T> : PredicateUnit<T>
    {
        public AtomicPredicate(PredicateUnit<T> parent)
            : base(parent)
        {
        }


        private string? _valueString;
        public string? ValueString 
        {
            get => _valueString;
            set
            {
                _valueString = value;
                Value = value;
            }
        }

        private double? _valueNumber;
        public double? ValueNumber 
        {
            get => _valueNumber;
            set
            {
                _valueNumber = value;
                Value = value;
            }
        }

        private Enum? _valueEnum;
        public Enum? ValueEnum
        {
            get => _valueEnum;
            set
            {
                _valueEnum = value;
                Value = value;
            }
        }

        private bool? _valueBool;
        public bool? ValueBool 
        {
            get => _valueBool;
            set
            {
                _valueBool = value;
                Value = value;
            }
        }

        public DateTime? ValueDate { get; set; }
        public TimeSpan? ValueTime { get; set; }
        public object? Value { get; set; }

        public string? Operator { get; set; }

        public bool IsMultiSelect { get; set; } = false;
        public IEnumerable<string> MultiSelectValues { get; set; } = new HashSet<string>();

        public Expression<Func<T, object>>? PropertyExpression { get; set; }

        public string Member => GetMemberName(PropertyExpression);

        public Type? MemberType => GetMemberType(PropertyExpression);

        public override PredicateUnit<T>? Parent { get; set; }

        /// <summary>
        /// Clears operator and values
        /// </summary>
        public void ClearOperatorAndValues()
        {
            ValueString = null;
            ValueNumber = null;
            ValueEnum = null;
            ValueBool = null;
            ValueDate = null;
            ValueTime = null;
            Value = null;
            Operator = null;
        }

        public override bool? RemovePredicate(PredicateUnit<T> predicate)
        {
            return false;
        }

        /// <summary>
        /// Request "this" be removed from parent
        /// </summary>
        public bool? Remove()
        {
            return Parent?.RemovePredicate(this);
        }

        private string GetMemberName(Expression<Func<T, object>>? expression)
        {
            if(expression is null)
            {
                return string.Empty;
            }

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
            if (member.Expression?.NodeType == ExpressionType.MemberAccess)
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

    /// <summary>
    /// Represents compound predicates
    /// </summary>
    public class CompoundPredicate<T> : PredicateUnit<T>
    {
        public CompoundPredicate(PredicateUnit<T>? parent)
            : base(parent)
        {
            AtomicPredicates = new List<AtomicPredicate<T>>();
            CompoundPredicates = new List<CompoundPredicate<T>>();
        }

        public CompoundPredicateLogicalOperator LogicalOperator { get; set; }
        public List<AtomicPredicate<T>> AtomicPredicates { get; set; }
        public List<CompoundPredicate<T>> CompoundPredicates { get; set; }
        public override PredicateUnit<T>? Parent { get; set; }

        /// <summary>
        /// Adds a predicate to the appropriate list
        /// </summary>
        public void AddPredicate(PredicateUnit<T> predicate)
        {
            switch (predicate)
            {
                case AtomicPredicate<T> atomicPredicate:
                    AtomicPredicates.Add(atomicPredicate);
                    break;

                case CompoundPredicate<T> compoundPredicate:
                    CompoundPredicates.Add(compoundPredicate);
                    break;

                default:
                    throw new InvalidOperationException($"Unsupported predicate type: {predicate.GetType()}");
            }
        }

        public override bool? RemovePredicate(PredicateUnit<T> predicate)
        {
            switch (predicate)
            {
                case AtomicPredicate<T> atomicPredicate:
                    return AtomicPredicates.Remove(atomicPredicate);

                case CompoundPredicate<T> compoundPredicate:
                    return CompoundPredicates.Remove(compoundPredicate);

                default:
                    throw new InvalidOperationException($"Unsupported predicate type: {predicate.GetType()}");
            }
        }

        /// <summary>
        /// Request "this" be removed from parent
        /// </summary>
        public bool? Remove()
        {
            if (Parent is null)
            {
                AtomicPredicates?.Clear();
                CompoundPredicates?.Clear();
                return true;
            }

            return Parent?.RemovePredicate(this);
        }

        /// <summary>
        /// Gets the predicates in order of addition
        /// </summary>
        public IEnumerable<PredicateUnit<T>> GetPredicatesInOrder()
        {
            foreach (var predicate in AtomicPredicates)
                yield return predicate;

            foreach (var predicate in CompoundPredicates)
                yield return predicate;
        }
    }

    public enum CompoundPredicateLogicalOperator
    {
        And,
        Or
    }
}