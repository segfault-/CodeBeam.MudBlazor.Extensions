using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;

namespace MudExtensions
{
#nullable enable
    public partial class CompoundPredicateComponent<T> : MudComponentBase
    {
        [Parameter] public MudFilter<T>? Filter { get; set; }
        [Parameter] public CompoundPredicate<T>? CompoundPredicate { get; set; }
        [Parameter] public CompoundPredicateLogicalOperator? ParentLogicalOperator { get; set; }
        [Parameter] public bool IsFirstElement { get; set; }
        [Parameter] public uint Depth { get; set; }
        [Parameter] public EventCallback LogicalOperatorChanged { get; set; }

        protected string ClassName => new CssBuilder("mud-compound-predicate")
            .AddClass($"depth-{Depth}")
            .AddClass(Class)
            .Build();

        protected string StyleString => new StyleBuilder()
            .AddStyle(Style)
            .Build();

        protected void AddAtomicPredicate()
        {
            CompoundPredicate?.AddPredicate(new AtomicPredicate<T>(CompoundPredicate));
        }

        protected void AddCompoundPredicate()
        {
            CompoundPredicate?.AddPredicate(new CompoundPredicate<T>(CompoundPredicate));
        }

        protected void RemovePredicateUnit()
        {
            CompoundPredicate?.Remove();
            Filter?.CallStateHasChanged();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();


            if(CompoundPredicate?.AtomicPredicates.Any() == false)
            {
                CompoundPredicate?.AddPredicate(new AtomicPredicate<T>(CompoundPredicate));
                CompoundPredicate?.AddPredicate(new AtomicPredicate<T>(CompoundPredicate));
            }
        }

        protected async Task OnLogicalOperatorChangedAsync()
        {
            await LogicalOperatorChanged.InvokeAsync();
        }

    }
}
