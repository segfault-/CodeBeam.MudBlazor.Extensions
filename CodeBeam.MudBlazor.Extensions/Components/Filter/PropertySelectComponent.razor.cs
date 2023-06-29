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
            Console.WriteLine("--> PropertySelectComponent<T>:SetParametersAsync()");
            await base.SetParametersAsync(parameters);

            if (parameters.TryGetValue<MudFilter<TItem>>("Filter", out var filter))
            {
                Filter = filter;
                Console.WriteLine($"SomeParameter: {Filter}");
            }

            if (parameters.TryGetValue<AtomicPredicate<TItem>>("AtomicPredicate", out var atomicPredicate))
            {
                AtomicPredicate = atomicPredicate;
                Console.WriteLine($"SomeParameter: {AtomicPredicate}");
            }

            if (parameters.TryGetValue<EventCallback>("PropertySelectChanged", out var propertySelectChanged))
            {
                PropertySelectChanged = propertySelectChanged;
                Console.WriteLine($"SomeParameter: {PropertySelectChanged}");
            }
        }

        protected override void OnInitialized()
        {
            Console.WriteLine("--> PropertySelectComponent<T>:OnInitialized()");
            base.OnInitialized();
        }

        protected override void OnParametersSet()
        {
            Console.WriteLine("--> PropertySelectComponent<T>:OnParametersSet()");
            

            if(AtomicPredicate is not null)
            {
                Property = Filter?.Properties?.SingleOrDefault(p => GetPropertyName(p.PropertyExpression) == AtomicPredicate.Member);
               
            }
            base.OnParametersSet();
        }

        protected async Task OnPropertyChangedAsync()
        {
            Console.WriteLine("--> PropertySelectComponent<T>:OnPropertyChangedAsync()");

            Type? oldType = TypeIdentifier.GetPropertyTypeFromExpression(AtomicPredicate?.PropertyExpression);
            Type? newType = TypeIdentifier.GetPropertyTypeFromExpression(Property?.PropertyExpression);


            if (AtomicPredicate is not null)
            {
                var foo = GetPropertyName(Property?.PropertyExpression);
                Console.WriteLine(foo);
                AtomicPredicate.Member = foo;
            }

            if (oldType != newType)
            {
                await PropertySelectChanged.InvokeAsync();
            }

            
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

            Console.WriteLine("----> throwing exception");
            throw new ArgumentException("Invalid expression", nameof(expression));
        }
    }
}
