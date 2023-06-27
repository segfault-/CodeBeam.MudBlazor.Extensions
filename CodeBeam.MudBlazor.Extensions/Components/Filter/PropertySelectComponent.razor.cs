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


        protected override void OnParametersSet()
        {
            Console.WriteLine("--> PropertySelectComponent<T>:OnParametersSet()");
            base.OnParametersSet();

            if(AtomicPredicate is not null)
            {
                Property = Filter?.Properties?.SingleOrDefault(p => GetPropertyName(p.PropertyExpression) == AtomicPredicate.Member);
            }
        }

        protected async Task OnPropertyChangedAsync()
        {
            Console.WriteLine("--> PropertySelectComponent<T>:OnPropertyChangedAsync()");
            if (AtomicPredicate is not null)
            {
                AtomicPredicate.Member = GetPropertyName(Property?.PropertyExpression);
            }
            await PropertySelectChanged.InvokeAsync();
        }

        public static string? GetPropertyName(Expression<Func<TItem, object>>? expression)
        {
            Console.WriteLine("--> PropertySelectComponent<T>:GetPropertyName()");
            if(expression is null)
            {
                Console.WriteLine("----> null");
                return null;
            }
            else if (expression.Body is MemberExpression memberExpression)
            {
                Console.WriteLine($"----> {memberExpression.Member.Name}");
                return memberExpression.Member.Name;
            }
            else if (expression.Body is UnaryExpression unaryExpression && unaryExpression.Operand is MemberExpression)
            {
                Console.WriteLine($"----> {((MemberExpression)unaryExpression.Operand).Member.Name}");
                // If the property is nullable, expression.Body will be an UnaryExpression
                return ((MemberExpression)unaryExpression.Operand).Member.Name;
            }

            throw new ArgumentException("Invalid expression", nameof(expression));
        }
    }
}
