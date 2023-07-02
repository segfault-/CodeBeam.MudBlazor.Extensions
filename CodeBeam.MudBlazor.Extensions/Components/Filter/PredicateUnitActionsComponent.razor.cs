using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MudExtensions
{
    public partial class PredicateUnitActionsComponent<T> : MudComponentBase
    {
        [Parameter] public EventCallback AddAtomicPredicateAsync { get; set; }
        [Parameter] public EventCallback AddCompoundPredicateAsync { get; set; }
        [Parameter] public EventCallback RemovePredicateUnitAsync { get; set; }
        [Parameter] public bool IsFirstElement { get; set; }

        protected bool IsCompoundPredicate { get; set; }
        protected bool IsAtomicPredicate { get; set; }
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            Console.WriteLine($"--> PredicateUnitActionsComponent<T> --> T={typeof(T)}");
            IsAtomicPredicate = false;
            IsCompoundPredicate = false;

            if (typeof(T).IsAssignableFrom(typeof(AtomicPredicateComponent<>)) || (typeof(T).GetGenericTypeDefinition() == typeof(AtomicPredicateComponent<>)))
            {
                IsAtomicPredicate = true;
            }

            if (typeof(T).IsAssignableFrom(typeof(CompoundPredicateComponent<>)) || (typeof(T).GetGenericTypeDefinition() == typeof(CompoundPredicateComponent<>)))
            {
                IsCompoundPredicate = true;
            }
        }

    }
}
