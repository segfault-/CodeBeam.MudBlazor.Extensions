using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace MudExtensions
{
#nullable enable
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

        private object? _value;
        public object? Value 
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }

        private string? _operator;
        public string? Operator 
        {
            get => _operator;
            set => SetProperty(ref _operator, value);
        }

        public bool IsMultiSelect => MultiSelectValues?.Any() ?? false;
        public IEnumerable<string>? MultiSelectValues { get; set; }

        [JsonIgnore] public Expression<Func<T, object>>? PropertyExpression { get; set; }
        [JsonIgnore] public Type? MemberType { get; set; }
        [JsonIgnore] public override PredicateUnit<T>? Parent { get; set; }

        private string? _member;
        public string? Member
        {
            get => _member;
            set
            {
                if (SetProperty(ref _member, value))
                {
                    if (_member != null)
                    {
                        var parameter = Expression.Parameter(typeof(T), "x");
                        Expression memberExpression = parameter;
                        foreach (var memberName in _member.Split('.'))
                        {
                            memberExpression = Expression.PropertyOrField(memberExpression, memberName);
                        }

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
        }


        /// <summary>
        /// Request "this" be removed from parent
        /// </summary>
        public bool? Remove()
        {
            return Parent?.RemovePredicate(this);
        }
    }
}