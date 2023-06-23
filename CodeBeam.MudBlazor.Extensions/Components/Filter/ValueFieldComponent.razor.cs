using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;

namespace MudExtensions
{
    public partial class ValueFieldComponent<T> : MudComponentBase
    {
        [Parameter] public AtomicPredicate<T> AtomicPredicate { get; set; }

        protected string ClassName => new CssBuilder("mud-value-field")
            .AddClass(Class)
            .Build();

        protected string StyleString => new StyleBuilder()
            .AddStyle(Style)
            .Build();
    }
}
