using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;
using System.Linq.Expressions;
using System.Text.Json;

namespace MudExtensions
{
#nullable enable
    public partial class MudFilter<T> : MudComponentBase
    {
        protected string ClassName => new CssBuilder("mud-filter")
            .AddClass(Class)
            .Build();

        protected string StyleString => new StyleBuilder()
            .AddStyle(Style)
            .Build();

        protected string JsonString = string.Empty;

        /// <summary>
        /// Represents what members of T are eligible for filtering
        /// </summary>
        [Parameter] public RenderFragment? FilterTemplate { get; set; }
        [Parameter] public RenderFragment? PredicateUnitActionsTemplate { get; set; }
        [Parameter] public RenderFragment? LogicalOperatorTemplate { get; set; }
        [Parameter] public CompoundPredicate<T>? FilterRoot { get; set; } = new(null);
        [Parameter] public ICollection<Property<T>>? Properties { get; set; } = new List<Property<T>>();
        [Parameter] public Expression<Func<T, bool>>? Expression { get; set; }
        [Parameter] public EventCallback<Expression<Func<T, bool>>?> ExpressionChanged { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        internal void AddProperty(Property<T> property)
        {
            Properties?.Add(property);
        }

        public async Task CompileExpressionAsync()
        {
            Console.WriteLine("--> MudFilter<T>:CompileExpressionAsync()");
            ExpressionGenerator expressionGenerator = new ExpressionGenerator();

            var expression = expressionGenerator.CompilePredicateExpression(FilterRoot);

            Expression = expression;
            await ExpressionChanged.InvokeAsync(expression);
        }

        protected async Task OnOnExpressionChangedAsync()
        {
            await ExpressionChanged.InvokeAsync();
        }

        protected async Task SerializeAsync()
        {
            JsonSerializerOptions jsonSerializerOptions = new()
            {
                TypeInfoResolver = new PredicateUnitJsonTypeInfoResolver<T>()
            };
            jsonSerializerOptions.Converters.Add(new AtomicPredicateConverter<T>());

            if(FilterRoot is not null)
            {
                JsonString = JsonSerializer.Serialize<PredicateUnit<T>>(FilterRoot, jsonSerializerOptions);

                if(JsonString is not null)
                {
                    PredicateUnit<T>? pUnit = JsonSerializer.Deserialize<PredicateUnit<T>>(JsonString, jsonSerializerOptions);
                    
                    if(pUnit is not null)
                    {
                        FilterRoot = (CompoundPredicate<T>)pUnit;
                        StateHasChanged();
                    }
                }
            }
        }
    }

}
