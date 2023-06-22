using System.Linq.Expressions;

namespace MudExtensions
{
    public class FilterRule<T>
    {
        public FilterRule()
            : base()
        {
            Id = Guid.NewGuid();
            Rules = new();
        }

        public FilterRule(FilterRule<T> parent)
            : this()
        {
            Parent = parent;
        }

        public FilterRule<T> DeepClone()
        {
            var rule = new FilterRule<T>()
            {
                Id = this.Id,
                Disabled = this.Disabled,
                Label = this.Label,
                Field = this.Field,
                Operator = this.Operator,
                Value = this.Value,
                Condition = this.Condition,

                Rules = new List<FilterRule<T>>(this.Rules.Select(r => r.DeepClone()).ToList()),
            };

            return rule;
        }

        public bool HasChild => Rules != null && Rules.Count > 0;

        public bool IsExpanded { get; set; } = true;
        public Guid Id { get; set; }
        public bool Disabled { get; set; }
        public string Label { get; set; }

        public string Field { get; set; }
        public string Operator { get; set; }
        public object Value { get; set; }

        public Type PropertyType { get; set; }
        public FieldType FieldType => FieldType.Identify(PropertyType);

        public Condition? Condition { get; set; }
        public FilterRule<T> Parent { get; set; }
        public List<FilterRule<T>> Rules { get; set; }


        public CompoundPredicate<T> gz { get; set; } = new CompoundPredicate<T>()
        {
            LogicalOperator = FilterCompositionLogicalOperator.And,
            FilterDescriptors = new List<PredicateUnit<T>>
            {
                new AtomicPredicate<T>
                {
                    Value = 25,
                    Operator = FilterOperatorz.IsGreaterThanOrEqualTo.ToString()
                },
                new AtomicPredicate<T>
                {
                    Value = 40,
                    Operator = FilterOperatorz.IsLessThanOrEqualTo.ToString()
                },
                new AtomicPredicate<T>
                {
                    Value = "France",
                    Operator = FilterOperatorz.IsEqualTo.ToString()
                },
                new CompoundPredicate<T>
                {
                    LogicalOperator = FilterCompositionLogicalOperator.Or,
                    FilterDescriptors = new List<PredicateUnit<T>>
                    {
                        new AtomicPredicate<T>
                        {
                            Value = "La maison d'Asie",
                            Operator = FilterOperatorz.Contains.ToString()
                        },
                        new AtomicPredicate<T>
                        {
                            Value = "Victuailles en stock",
                            Operator = FilterOperatorz.Contains.ToString()
                        }
                    },
                }
            }
        };

    }


    public abstract class PredicateUnit<T>
    {
        public Guid Id { get; set; }
    }

    public class AtomicPredicate<T> : PredicateUnit<T>
    {
        public object ValueObject;
        public string ValueString;
        public double? ValueNumber;
        public Enum ValueEnum = null;
        public bool? ValueBool;
        public DateTime? ValueDate;
        public TimeSpan? ValueTime;
        public bool IsMultSelect;
        public IEnumerable<string> MultiSelectValues { get; set; } = new HashSet<string>();


        public Expression<Func<T, object>> PropertyExpression { get; set; }

        public string Member
        {
            get
            {
                return GetMemberName(PropertyExpression);
            }
        }

        public Type MemberType
        {
            get
            {
                return GetMemberType(PropertyExpression);
            }
        }

        public object Value { get; set; }
        public string Operator { get; set; }


        private string GetMemberName(Expression<Func<T, object>> expression)
        {
            if (expression.Body is MemberExpression member)
                return GetFullMemberName(member);

            if (expression.Body is UnaryExpression unary)
                return GetFullMemberName((MemberExpression)unary.Operand);

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

        private Type GetMemberType(Expression<Func<T, object>> expression)
        {
            if (expression.Body is MemberExpression member)
                return member.Type;

            if (expression.Body is UnaryExpression unary)
                return unary.Operand.Type;

            throw new ArgumentException("Could not get the member type.");
        }
    }

    public class CompoundPredicate<T> : PredicateUnit<T>
    {
        public FilterCompositionLogicalOperator LogicalOperator { get; set; }
        public List<PredicateUnit<T>> FilterDescriptors { get; set; }

        public CompoundPredicate()
        {
            FilterDescriptors = new List<PredicateUnit<T>>();
        }
    }



    public enum FilterCompositionLogicalOperator
    {
        And,
        Or
        // Additional logical operators can be added here as required
    }


    public enum FilterOperatorz
    {
        IsGreaterThanOrEqualTo,
        IsLessThanOrEqualTo,
        IsEqualTo,
        Contains
        // Additional filter operators can be added here as required
    }








}
