using System.Linq.Expressions;

namespace MudExtensions
{
#nullable enable
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

    }
}
