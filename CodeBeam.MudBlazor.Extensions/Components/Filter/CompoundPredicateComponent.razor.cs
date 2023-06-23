using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;

namespace MudExtensions
{
#nullable enable
    public partial class CompoundPredicateComponent<T> : MudComponentBase
    {
        [CascadingParameter] public MudFilter<T>? Filter { get; set; }
        [Parameter] public CompoundPredicate<T>? CompoundPredicate { get; set; }

        protected string ClassName => new CssBuilder("mud-compound-predicate")
            .AddClass(Class)
            .Build();

        protected string StyleString => new StyleBuilder()
            .AddStyle(Style)
            .Build();

        protected void AddAtomicPredicate()
        {
            CompoundPredicate?.AddPredicate(new AtomicPredicate<T>(this.CompoundPredicate));
        }

        protected void AddCompoundPredicate()
        {
            CompoundPredicate?.AddPredicate(new CompoundPredicate<T>(this.CompoundPredicate));
        }

        protected void RemovePredicateUnit()
        {
            CompoundPredicate?.Remove();
            //Filter?.CallStateHasChanged();
        }

    }
}
