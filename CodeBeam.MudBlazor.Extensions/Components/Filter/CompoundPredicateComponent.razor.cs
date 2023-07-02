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
        [Parameter] public EventCallback CompoundPredicateComponentChanged { get; set; }

        protected string ClassName => new CssBuilder("mud-compound-predicate")
            .AddClass($"depth-{Depth}")
            .AddClass(Class)
            .Build();

        protected string StyleString => new StyleBuilder()
            .AddStyle(Style)
            .Build();

        protected async Task AddAtomicPredicateAsync()
        {
            CompoundPredicate?.AddPredicate(new AtomicPredicate<T>(CompoundPredicate));
            await CompoundPredicateComponentChanged.InvokeAsync();
        }

        protected async Task AddCompoundPredicateAsync()
        {
            CompoundPredicate?.AddPredicate(new CompoundPredicate<T>(CompoundPredicate));
            await CompoundPredicateComponentChanged.InvokeAsync();
        }

        protected async Task RemovePredicateUnitAsync()
        {
            CompoundPredicate?.Remove();
            await CompoundPredicateComponentChanged.InvokeAsync();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();


            if(CompoundPredicate?.AtomicPredicates.Any() == false)
            {
                CompoundPredicate?.AddPredicate(new AtomicPredicate<T>(CompoundPredicate));
            }


        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await CompoundPredicateComponentChanged.InvokeAsync();
        }

        protected async Task OnLogicalOperatorChangedAsync()
        {
            await CompoundPredicateComponentChanged.InvokeAsync();
        }

        protected async Task OnAtomicPredicateChangedAsync()
        {
            await CompoundPredicateComponentChanged.InvokeAsync();
        }

        protected Task OnRemoveAtomicPredicateComponentAsync(AtomicPredicate<T> item)
        {
            if (item is not null)
            {
                CompoundPredicate?.AtomicPredicates.Remove(item);
            }

            return Task.CompletedTask;
        }

    }
}
