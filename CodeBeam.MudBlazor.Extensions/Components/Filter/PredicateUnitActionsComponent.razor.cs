using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;

namespace MudExtensions
{
    public partial class PredicateUnitActionsComponent : MudComponentBase
    {
        [Parameter] public EventCallback AddAtomicPredicateAsync { get; set; }
        [Parameter] public EventCallback AddCompoundPredicateAsync { get; set; }
        [Parameter] public EventCallback RemovePredicateUnitAsync { get; set; }
        [Parameter] public bool IsCompoundPredicate { get; set; }
        [Parameter] public bool IsFirstElement { get; set; }
    }
}
