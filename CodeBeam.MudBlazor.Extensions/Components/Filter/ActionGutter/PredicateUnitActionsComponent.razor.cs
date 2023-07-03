using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MudExtensions
{
#nullable enable
    public partial class PredicateUnitActionsComponent : MudComponentBase
    {
        [Parameter] public EventCallback AddAtomicPredicateAsync { get; set; }
        [Parameter] public EventCallback AddCompoundPredicateAsync { get; set; }
        [Parameter] public EventCallback RemovePredicateUnitAsync { get; set; }
        [Parameter] public bool IsFirstElement { get; set; }
        [Parameter] public bool IsCompound { get; set; }

        protected bool IsCompoundPredicate => IsCompound;
        protected bool IsAtomicPredicate => !IsCompound;
    }
}
