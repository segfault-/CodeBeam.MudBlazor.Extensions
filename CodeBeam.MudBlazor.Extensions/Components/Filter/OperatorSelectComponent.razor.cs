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

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);

            if (AtomicPredicate is not null)
            {
                Operator = AtomicPredicate.Operator;
            }

            //if (parameters.TryGetValue<AtomicPredicate<T>>("AtomicPredicate", out var atomicPredicate))
            //{
            //    AtomicPredicate = atomicPredicate;
            //    Operator = AtomicPredicate.Operator;
            //}

            if (parameters.TryGetValue<EventCallback>("OperatorSelectChanged", out var operatorSelectChanged))
            {
                OperatorSelectChanged = operatorSelectChanged;
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
        }


        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            if (AtomicPredicate is not null)
            {
                Operator = AtomicPredicate.Operator;
            }
        }

        protected async Task OnOperatorSelectChangedAsync()
        {
            if (AtomicPredicate is not null)
            {
                AtomicPredicate.Operator = Operator;
            }
            await OperatorSelectChanged.InvokeAsync();
        }

        protected Func<string, string, bool> SearchFunc => (op, value) => op.Contains(value, StringComparison.OrdinalIgnoreCase);

    }
}
