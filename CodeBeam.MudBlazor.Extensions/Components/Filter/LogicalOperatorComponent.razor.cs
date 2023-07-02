using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace MudExtensions
{
    public partial class LogicalOperatorComponent : MudComponentBase
    {
        [Parameter] public uint Depth { get; set; }
        [Parameter] public string LogicalOperator { get; set; }
        [Parameter] public string ParentLogicalOperator { get; set; }
        [Parameter] public bool IsFirstElement { get; set; }
        [Parameter] public bool DisplayParentOperator { get; set; }
    }
}
