using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;
using System.Linq.Expressions;

namespace MudExtensions
{
#nullable enable
    public partial class PropertySelectComponent<T> : MudComponentBase
    {
        [Parameter] public MudFilter<T>? Filter { get; set; }
        [Parameter] public AtomicPredicate<T>? AtomicPredicate { get; set; }
        [Parameter] public EventCallback PropertySelectChanged { get; set; }
        
        protected Expression<Func<T, object>>? PropertyExpression { get; set; }

        protected string ClassName => new CssBuilder("mud-property-select")
            .AddClass(Class)
            .Build();

        protected string StyleString => new StyleBuilder()
            .AddStyle(Style)
            .Build();

        protected async Task OnPropertyExpressionChangedAsync()
        {
            Type? oldType = TypeIdentifier.GetPropertyTypeFromExpression(AtomicPredicate?.PropertyExpression);
            Type? newType = TypeIdentifier.GetPropertyTypeFromExpression(PropertyExpression);

            if (AtomicPredicate is not null)
            {
                AtomicPredicate.PropertyExpression = PropertyExpression;
            }
            
            if (oldType != newType)
            {
                await PropertySelectChanged.InvokeAsync();
            }
        }
    }
}
