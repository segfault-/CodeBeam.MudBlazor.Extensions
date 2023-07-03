using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MudExtensions
{
#nullable enable
    public partial class LogicalOperatorTemplate : MudComponentBase
    {
        [Parameter] public uint Depth { get; set; }
        [Parameter] public CompoundPredicateLogicalOperator? ParentLogicalOperator { get; set; }
        [Parameter, EditorRequired] public Func<int, bool>? TextContentCondition { get; set; }
    }
}
