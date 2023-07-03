using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MudExtensions
{
#nullable enable
    public partial class PredicateUnitActionsComponent<T> : MudComponentBase
    {
        [Parameter] public EventCallback AddAtomicPredicateAsync { get; set; }
        [Parameter] public EventCallback AddCompoundPredicateAsync { get; set; }
        [Parameter] public EventCallback RemovePredicateUnitAsync { get; set; }
        [Parameter] public bool IsFirstElement { get; set; }
        [Parameter] public RenderFragment? PredicateUnitActionsTemplate { get; set; }

        protected bool IsCompoundPredicate => typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(CompoundPredicateComponent<>);
        protected bool IsAtomicPredicate => typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(AtomicPredicateComponent<>);


    }
}
