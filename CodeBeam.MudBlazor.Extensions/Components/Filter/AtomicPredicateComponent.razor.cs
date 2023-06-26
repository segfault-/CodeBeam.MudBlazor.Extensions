using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;

namespace MudExtensions
{
#nullable enable
    public partial class AtomicPredicateComponent<T> : MudComponentBase
    {
        [Parameter] public MudFilter<T>? Filter { get; set; }
        [Parameter] public CompoundPredicate<T>? Parent { get; set; }
        [Parameter] public AtomicPredicate<T>? AtomicPredicate { get; set; }
        [Parameter] public bool IsFirstElement { get; set; }
        [Parameter] public CompoundPredicateLogicalOperator LogicalOperator { get; set; }
        [Parameter] public uint Depth { get; set; }
        protected string ClassName => new CssBuilder("mud-atomic-predicate")
            .AddClass(Class)
            .Build();

        protected string StyleString => new StyleBuilder()
            .AddStyle(Style)
            .Build();

        public Task OnPropertySelectChangedAsync()
        {
            AtomicPredicate?.ClearOperatorAndValues();
            return Task.CompletedTask;
        }

        protected void RemovePredicateUnit()
        {
            AtomicPredicate?.Remove();
            Filter?.CallStateHasChanged();
        }
    }
}
