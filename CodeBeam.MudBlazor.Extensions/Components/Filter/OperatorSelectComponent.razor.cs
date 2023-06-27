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

        protected string? Operator { get; set; }

        protected string ClassName => new CssBuilder("mud-operator-select")
            .AddClass(Class)
            .Build();

        protected string StyleString => new StyleBuilder()
            .AddStyle(Style)
            .Build();

        protected override async Task OnParametersSetAsync()
        {
            Console.WriteLine("--> OperatorSelectComponent<T>:OnParametersSetAsync");

            await base.OnParametersSetAsync();
            if (AtomicPredicate is not null)
            {
                Operator = AtomicPredicate.Operator;

            }
        }

        protected async Task OnOperatorSelectChangedAsync()
        {
            Console.WriteLine("--> OperatorSelectComponent<T>:OnOperatorSelectChangedAsync");
            if (AtomicPredicate is not null)
            {
                AtomicPredicate.Operator = Operator;
            }
            await OperatorSelectChanged.InvokeAsync();
        }
    }
}
