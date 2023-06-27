using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
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

        protected string? PropertyName { get; set; }

        private Expression<Func<T, object>>? _propertyExpression;
        protected Expression<Func<T, object>>? PropertyExpression 
        {
            get => _propertyExpression;
            set
            {
                if (_propertyExpression != value)
                {
                    _propertyExpression = value;
                    OnPropertyExpressionChangedAsync().Wait();
                }
            }
        }

        protected string ClassName => new CssBuilder("mud-property-select")
            .AddClass(Class)
            .Build();

        protected string StyleString => new StyleBuilder()
            .AddStyle(Style)
            .Build();


        protected override async Task OnParametersSetAsync()
        {
            Console.WriteLine("--> PropertySelectComponent<T>:OnParametersSetAsync");

            await base.OnParametersSetAsync();
            if (AtomicPredicate is not null)
            {
                PropertyExpression = AtomicPredicate.PropertyExpression;
                PropertyName = GetPropertyName(AtomicPredicate.PropertyExpression) ?? "foo";
            }
        }

        protected async Task OnPropertyExpressionChangedAsync()
        {
            Console.WriteLine("--> PropertySelectComponent<T>:OnPropertyExpressionChangedAsync()");

            if (AtomicPredicate is not null)
            {
                AtomicPredicate.PropertyExpression = PropertyExpression;
            }
            
            await PropertySelectChanged.InvokeAsync();
        }

        protected async Task OnPropertyNameChangedAsync()
        {
            Console.WriteLine("--> PropertySelectComponent<T>:OnPropertyNameChangedAsync()");

            if (AtomicPredicate is not null)
            {
                AtomicPredicate.PropertyExpression = PropertyExpression;
            }

            await PropertySelectChanged.InvokeAsync();
        }

        protected string? GetPropertyName(Expression<Func<T, object>>? expression)
        {
            if (expression is null)
            {
                return null;
            }
            else if (expression.Body is MemberExpression memberExpression)
            {
                return memberExpression.Member.Name;
            }
            else if (expression.Body is UnaryExpression unaryExpression &&
                unaryExpression.Operand is MemberExpression)
            {
                // If the property is nullable, expression.Body will be an UnaryExpression
                return ((MemberExpression)unaryExpression.Operand).Member.Name;
            }

            throw new ArgumentException("Invalid expression", nameof(expression));
        }
    }
}
