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
        /// <summary>
        /// Render fragment to customize actions like add atomic predicate, add compound predicate, and delete
        /// </summary>
        [Parameter] public RenderFragment? PredicateUnitActionsTemplate { get; set; }
        /// <summary>
        /// Render fragment to allow selection of logical operator
        /// </summary>
        [Parameter] public RenderFragment? LogicalOperatorTemplate { get; set; }
        /// <summary>
        /// The compound predicate that represents the base of our expression tree
        /// </summary>
        [Parameter] public CompoundPredicate<T>? FilterRoot { get; set; } = new(null);
        /// <summary>
        /// The properties that are allowed to participate in filtering
        /// </summary>
        [Parameter] public ICollection<Property<T>>? Properties { get; set; } = new List<Property<T>>();
        /// <summary>
        /// An expression that can be used with IQueryable where() method
        /// </summary>
        [Parameter] public Expression<Func<T, bool>>? Expression { get; set; }
        /// <summary>
        /// Callback when expression changes, can be used to reload data grid or refresh metric visuals
        /// </summary>
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
            ExpressionGenerator expressionGenerator = new ExpressionGenerator();

            var expression = expressionGenerator.CompilePredicateExpression(FilterRoot);

            Expression = expression;
            await ExpressionChanged.InvokeAsync(expression);
        }

        protected async Task OnOnExpressionChangedAsync()
        {
            await ExpressionChanged.InvokeAsync();
        }

        /// <summary>
        /// Example demonstrating how to serialize / deserialize.  this is how you transfer the expression definition over the wire, apply the results server side and presumably send the filtered dataset back
        /// </summary>
        /// <returns></returns>
        protected Task SerializeAsync()
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
            return Task.CompletedTask;
        }
    }

}
