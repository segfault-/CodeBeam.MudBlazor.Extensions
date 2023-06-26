using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;
using System.Linq.Expressions;

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

        protected async Task CompileExpressionAsync()
        {
            ExpressionGenerator expressionGenerator = new ExpressionGenerator();

            var expression = expressionGenerator.ParseExpressionOf(FilterRoot);

            Expression = expression;
            await ExpressionChanged.InvokeAsync(expression);
        }

        protected async Task OnOnExpressionChangedAsync()
        {
            await ExpressionChanged.InvokeAsync();
        }


    }
}
