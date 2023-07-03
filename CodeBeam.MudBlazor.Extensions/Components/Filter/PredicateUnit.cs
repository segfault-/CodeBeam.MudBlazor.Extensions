using System.Linq.Expressions;
using System.Text.Json.Serialization;

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

        [JsonIgnore] public abstract PredicateUnit<T>? Parent { get; set; }

        public virtual bool? RemovePredicate(PredicateUnit<T> predicate) 
        { 
            return false; 
        }
    }

    /// <summary>
    /// Represents atomic predicates
    /// </summary>
    public class AtomicPredicate<T> : PredicateUnit<T>
    {
        public AtomicPredicate() 
            :base(null)
        { 
        }

        public AtomicPredicate(PredicateUnit<T> parent)
            : base(parent)
        {
        }

        public object? Value { get; set; }
        public string? Operator { get; set; }
        public bool IsMultiSelect { get; set; } = false;
        public IEnumerable<string> MultiSelectValues { get; set; } = new HashSet<string>();

        [JsonIgnore] public Expression<Func<T, object>>? PropertyExpression { get; set; }
        [JsonIgnore] public Type? MemberType { get; set; }
        [JsonIgnore] public override PredicateUnit<T>? Parent { get; set; }


        private string? _member;
        public string? Member
        {
            get => _member;
            set
            {
                _member = value;
                if (_member != null)
                {
                    var parameter = Expression.Parameter(typeof(T), "x");
                    var memberExpression = Expression.PropertyOrField(parameter, _member);

                    // Convert the expression to object
                    var convertedExpression = Expression.Convert(memberExpression, typeof(object));

                    PropertyExpression = Expression.Lambda<Func<T, object>>(convertedExpression, parameter);
                    MemberType = memberExpression.Type;
                }
                else
                {
                    PropertyExpression = null;
                    MemberType = null;
                }
            }
        }



        /// <summary>
        /// Clears operator and values
        /// </summary>
        public void ClearOperatorAndValues()
        {
            Value = null;
            Operator = null;
        }

        /// <summary>
        /// Request "this" be removed from parent
        /// </summary>
        public bool? Remove()
        {
            return Parent?.RemovePredicate(this);
        }
    }

    /// <summary>
    /// Represents compound predicates
    /// </summary>
    public class CompoundPredicate<T> : PredicateUnit<T>
    {
        public CompoundPredicate()
            : base(null)
        {
            IsFirstElement = true;
            AtomicPredicates = new List<AtomicPredicate<T>>();
            CompoundPredicates = new List<CompoundPredicate<T>>();
        }
        public CompoundPredicate(PredicateUnit<T>? parent)
            : base(parent)
        {
            IsFirstElement = true;
            AtomicPredicates = new List<AtomicPredicate<T>>();
            CompoundPredicates = new List<CompoundPredicate<T>>();
        }

        public CompoundPredicateLogicalOperator LogicalOperator { get; set; }
        public bool IsFirstElement { get; set; }
        public List<AtomicPredicate<T>> AtomicPredicates { get; set; }
        public List<CompoundPredicate<T>> CompoundPredicates { get; set; }
        [JsonIgnore] public override PredicateUnit<T>? Parent { get; set; }

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