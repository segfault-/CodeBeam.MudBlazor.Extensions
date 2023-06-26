using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;

namespace MudExtensions
{
#nullable enable
    public partial class OperatorSelectComponent<T> : MudComponentBase
    {
        [Parameter] public AtomicPredicate<T>? AtomicPredicate { get; set; }
        [Parameter] public EventCallback OperatorSelectChanged { get; set; }

        protected string ClassName => new CssBuilder("mud-operator-select")
            .AddClass(Class)
            .Build();

        protected string StyleString => new StyleBuilder()
            .AddStyle(Style)
            .Build();

        protected async Task OnOperatorSelectChangedAsync()
        {
            await OperatorSelectChanged.InvokeAsync();
        }
    }
}
