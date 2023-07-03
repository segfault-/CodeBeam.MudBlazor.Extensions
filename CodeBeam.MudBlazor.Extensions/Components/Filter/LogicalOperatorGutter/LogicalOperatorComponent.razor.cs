using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MudExtensions
{
#nullable enable
    public partial class LogicalOperatorComponent : MudComponentBase
    {
        [Parameter] public uint Depth { get; set; }
        [Parameter] public CompoundPredicateLogicalOperator? LogicalOperator { get; set; }
        [Parameter] public CompoundPredicateLogicalOperator? ParentLogicalOperator { get; set; }
        [Parameter] public bool IsFirstElement { get; set; }
        [Parameter] public bool DisplayParentOperator { get; set; }
        [Parameter] public RenderFragment? LogicalOperatorComponentTemplate { get; set; }
        [Parameter] public bool IsCompound { get; set; }

        protected bool IsCompoundPredicate => IsCompound;
        protected bool IsAtomicPredicate => !IsCompound;
    }
}
