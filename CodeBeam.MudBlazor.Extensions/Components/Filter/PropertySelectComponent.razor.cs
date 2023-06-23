using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;

namespace MudExtensions
{
    public partial class PropertySelectComponent<T> : MudComponentBase
    {
        [CascadingParameter] public MudFilter<T> Filter { get; set; }
        [Parameter] public AtomicPredicate<T> AtomicPredicate { get; set; }
        [Parameter] public EventCallback PropertySelectChanged { get; set; }

        protected string ClassName => new CssBuilder("mud-property-select")
            .AddClass(Class)
            .Build();

        protected string StyleString => new StyleBuilder()
            .AddStyle(Style)
            .Build();

        protected async Task OnPropertyChangedAsync()
        {
            await PropertySelectChanged.InvokeAsync();
        }

    }
}
