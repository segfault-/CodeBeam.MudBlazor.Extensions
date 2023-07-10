using System.Text.Json.Serialization;

namespace MudExtensions
{
#nullable enable
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