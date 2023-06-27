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

        protected string jsonString = string.Empty;

        /// <summary>
        /// Represents what members of T are eligible for filtering
        /// </summary>
        [Parameter] public RenderFragment? FilterTemplate { get; set; }
        [Parameter] public CompoundPredicate<T>? FilterRoot { get; set; } = new(null);
        [Parameter] public ICollection<Property<T>>? Properties { get; set; } = new List<Property<T>>();
        [Parameter] public Expression<Func<T, bool>>? Expression { get; set; }
        [Parameter] public EventCallback<Expression<Func<T, bool>>> ExpressionChanged { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        internal void AddProperty(Property<T> property)
        {
            Properties?.Add(property);
        }

        internal void CallStateHasChanged()
        {
            StateHasChanged();
        }

        public async Task CompileExpressionAsync()
        {
            Console.WriteLine("--> MudFilter<T>:CompileExpressionAsync()");
            ExpressionGenerator expressionGenerator = new ExpressionGenerator();

            var expression = expressionGenerator.ParseExpressionOf(FilterRoot);

            Expression = expression;
            await ExpressionChanged.InvokeAsync(expression);
        }

        protected async Task OnOnExpressionChangedAsync()
        {
            await ExpressionChanged.InvokeAsync();
        }

        protected async Task SerializeAsync()
        {
            JsonSerializerOptions jsonSerializerOptions = new();
            jsonSerializerOptions.TypeInfoResolver = new PredicateUnitJsonTypeInfoResolver<T>();
            jsonSerializerOptions.Converters.Add(new AtomicPredicateConverter<T>());
            //jsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
            //jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());


            jsonString = JsonSerializer.Serialize<PredicateUnit<T>>(FilterRoot, jsonSerializerOptions);



            PredicateUnit<T> pUnit = JsonSerializer.Deserialize<PredicateUnit<T>>(jsonString, jsonSerializerOptions);

            FilterRoot = (CompoundPredicate<T>)pUnit;
            StateHasChanged();

        }

    }

}
