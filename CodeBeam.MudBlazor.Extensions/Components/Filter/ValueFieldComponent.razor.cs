using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;

namespace MudExtensions
{
#nullable enable
    public partial class ValueFieldComponent<T> : MudComponentBase
    {
        [Parameter] public AtomicPredicate<T>? AtomicPredicate { get; set; }
        [Parameter] public EventCallback ValueFieldChanged { get; set; }

        protected FieldType? FieldType;
        protected string ClassName => new CssBuilder("mud-value-field")
            .AddClass(Class)
            .Build();

        protected string StyleString => new StyleBuilder()
            .AddStyle(Style)
            .Build();


        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if(AtomicPredicate is not null)
            {
                FieldType = FieldType.Identify(AtomicPredicate.MemberType);
            }         
        }

        protected async Task OnValueFieldChangedAsync()
        {
            await ValueFieldChanged.InvokeAsync();
        }

    }
}
