using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MudExtensions
{
#nullable enable
    public partial class LogicalOperatorComponent<T> : MudComponentBase
    {
        [Parameter] public uint Depth { get; set; }
        [Parameter] public CompoundPredicateLogicalOperator? LogicalOperator { get; set; }
        [Parameter] public CompoundPredicateLogicalOperator? ParentLogicalOperator { get; set; }
        [Parameter] public bool IsFirstElement { get; set; }
        [Parameter] public bool DisplayParentOperator { get; set; }
        [Parameter] public RenderFragment? LogicalOperatorComponentTemplate { get; set; }

        protected bool IsCompoundPredicate => typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(CompoundPredicateComponent<>);
        protected bool IsAtomicPredicate => typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(AtomicPredicateComponent<>);
    }
}
