using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;

namespace MudExtensions
{
    public partial class MudFilter<T> : MudComponentBase
    {
        protected string ClassName => new CssBuilder("mud-filter")
            .AddClass(Class)
            .Build();

        protected string StyleString => new StyleBuilder()
            .AddStyle(Style)
            .Build();

        /// <summary>
        /// Represents what members of T are eligible for filtering
        /// </summary>
        [Parameter] public RenderFragment FilterTemplate { get; set; }
        [Parameter] public CompoundPredicate<T> FilterRoot { get; set; } = new();
        [Parameter] public ICollection<Property<T>> Properties { get; set; } = new List<Property<T>>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        internal void AddProperty(Property<T> property)
        {
            Properties.Add(property);
        }


        protected void AddPropertyExpression()
        {
           FilterRoot.FilterDescriptors.Add(new AtomicPredicate<T>());
        }


        protected void AddGroup()
        {
            FilterRoot.FilterDescriptors.Add(new CompoundPredicate<T>());
        }


        public void RemoveRule(FilterRule<T> rule)
        {
          // FilterRoot.Rules.Remove(rule);
        }

        protected CompoundPredicate<CatsDto> RootPredicate = new CompoundPredicate<CatsDto>
        {
            FilterDescriptors = new List<PredicateUnit<CatsDto>>
        {
            new AtomicPredicate<CatsDto> { PropertyExpression = x => x.Name },
            new CompoundPredicate<CatsDto>
            {
                LogicalOperator = FilterCompositionLogicalOperator.And,
                FilterDescriptors = new List<PredicateUnit<CatsDto>>
                {
                    new AtomicPredicate<CatsDto> { PropertyExpression = x => x.Gender },
                    new AtomicPredicate<CatsDto> { PropertyExpression = x => x.BirthDate }
                }
            }
        }
        };

        public class CatsDto
        {
            public string Name { get; set; }
            public string Gender { get; set; }
            public DateTime BirthDate { get; set; }
            public TabbyType CatType { get; set; }
            public bool IsSpade { get; set; }
            public double Temperature { get; set; }
            public Guid MicroId { get; set; }

            public enum TabbyType
            {
                OrangeTabby,
                GrayTabby,
                BlackTabby
            }

        }

    }
}
