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

        protected async Task OnPropertySelectChangedAsync()
        {
            Console.WriteLine("--> AtomicPredicateComponent<T>:OnPropertySelectChangedAsync()");
            //AtomicPredicate?.ClearOperatorAndValues();
            if (Filter is not null)
            {
                await Filter.CompileExpressionAsync();
            }
        }

        protected async Task OnValueFieldChangedAsync()
        {
            Console.WriteLine("--> AtomicPredicateComponent<T>:OnValueFieldChangedAsync()");
            if (Filter is not null)
            {
                await Filter.CompileExpressionAsync();
            }
        }

        protected async Task OnOperatorSelectChangedAsync()
        {
            Console.WriteLine("--> AtomicPredicateComponent<T>:OnOperatorSelectChangedAsync()");
            if (Filter is not null)
            {
                await Filter.CompileExpressionAsync();
            }
        }

        protected async Task RemovePredicateUnitAsync()
        {
            Console.WriteLine("--> AtomicPredicateComponent<T>:RemovePredicateUnitAsync()");
            AtomicPredicate?.Remove();
            Filter?.CallStateHasChanged();
            if (Filter is not null)
            {
                await Filter.CompileExpressionAsync();
            }
        }
    }
}
