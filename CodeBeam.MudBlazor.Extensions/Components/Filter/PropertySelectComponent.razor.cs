using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;
using System.Linq.Expressions;

namespace MudExtensions
{
#nullable enable
    public partial class PropertySelectComponent<TItem> : MudComponentBase
    {
        [Parameter] public MudFilter<TItem>? Filter { get; set; }
        [Parameter] public AtomicPredicate<TItem>? AtomicPredicate { get; set; }
        [Parameter] public EventCallback PropertySelectChanged { get; set; }


        protected Property<TItem>? Property { get; set; }
        protected string ClassName => new CssBuilder("mud-property-select")
            .AddClass(Class)
            .Build();
        protected string StyleString => new StyleBuilder()
            .AddStyle(Style)
            .Build();


        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);

            if (parameters.TryGetValue<MudFilter<TItem>>("Filter", out var filter))
            {
                Filter = filter;
            }

            if (parameters.TryGetValue<AtomicPredicate<TItem>>("AtomicPredicate", out var atomicPredicate))
            {
                AtomicPredicate = atomicPredicate;
            }

            if (parameters.TryGetValue<EventCallback>("PropertySelectChanged", out var propertySelectChanged))
            {
                PropertySelectChanged = propertySelectChanged;
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
        }

        protected override void OnParametersSet()
        {

            if(AtomicPredicate is not null)
            {
                Property = Filter?.Properties?.SingleOrDefault(p => GetPropertyName(p.PropertyExpression) == AtomicPredicate.Member);
               
            }
            base.OnParametersSet();
        }

        protected async Task OnPropertyChangedAsync()
        {
            Type? oldType = TypeIdentifier.GetPropertyTypeFromExpression(AtomicPredicate?.PropertyExpression);
            Type? newType = TypeIdentifier.GetPropertyTypeFromExpression(Property?.PropertyExpression);

            if (AtomicPredicate is not null)
            {
                var foo = GetPropertyName(Property?.PropertyExpression);
                AtomicPredicate.Member = foo;
            }

            if (oldType != newType)
            {
                await PropertySelectChanged.InvokeAsync();
            }        
        }

        protected Func<Property<TItem>, string, bool> SearchFunc => (property, value) => property.ComputedTitle?.Contains(value, StringComparison.OrdinalIgnoreCase) ?? false;


        public static string? GetPropertyName(Expression<Func<TItem, object>>? expression)
        {
            if(expression is null)
            {
                return null;
            }
            else if (expression.Body is MemberExpression memberExpression)
            {
                return memberExpression.Member.Name;
            }
            else if (expression.Body is UnaryExpression unaryExpression && unaryExpression.Operand is MemberExpression)
            {
                // If the property is nullable, expression.Body will be an UnaryExpression
                return ((MemberExpression)unaryExpression.Operand).Member.Name;
            }

            throw new ArgumentException("Invalid expression", nameof(expression));
        }

    }
}
