using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;

namespace MudExtensions
{
    public partial class CompoundPredicateComponent<T> : MudComponentBase
    {
        [CascadingParameter] public MudFilter<T> Filter { get; set; }
        [Parameter] public CompoundPredicate<T> CompoundPredicate { get; set; }

        protected string ClassName => new CssBuilder("mud-compound-predicate")
            .AddClass(Class)
            .Build();

        protected string StyleString => new StyleBuilder()
            .AddStyle(Style)
            .Build();

        protected void AddPropertyExpression()
        {
            CompoundPredicate?.FilterDescriptors.Add(new AtomicPredicate<T>(this.CompoundPredicate));
        }

        protected void AddGroup()
        {
            CompoundPredicate?.FilterDescriptors.Add(new CompoundPredicate<T>(this.CompoundPredicate));
        }

        protected void RemovePredicateUnit()
        {

        }
    }
}
