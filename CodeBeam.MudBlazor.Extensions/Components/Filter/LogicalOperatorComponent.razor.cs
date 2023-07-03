using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MudExtensions
{
    public partial class LogicalOperatorComponent<T> : MudComponentBase
    {
        [Parameter] public uint Depth { get; set; }
        [Parameter] public string LogicalOperator { get; set; }
        [Parameter] public string ParentLogicalOperator { get; set; }
        [Parameter] public bool IsFirstElement { get; set; }
        [Parameter] public bool DisplayParentOperator { get; set; }

        protected bool IsCompoundPredicate { get; set; }
        protected bool IsAtomicPredicate { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            IsAtomicPredicate = false;
            IsCompoundPredicate = false;

            if (typeof(T).IsGenericType)
            {
                if (typeof(T).GetGenericTypeDefinition() == typeof(AtomicPredicateComponent<>))
                {
                    IsAtomicPredicate = true;
                }

                if (typeof(T).GetGenericTypeDefinition() == typeof(CompoundPredicateComponent<>))
                {
                    IsCompoundPredicate = true;
                }
            }
        }
    }
}
