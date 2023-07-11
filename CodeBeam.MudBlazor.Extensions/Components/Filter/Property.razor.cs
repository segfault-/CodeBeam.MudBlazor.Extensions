using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Linq.Expressions;

namespace MudExtensions
{
#nullable enable
    public partial class Property<T> : MudComponentBase
    {
        [CascadingParameter] public MudFilter<T>? Filter { get; set; }
        [Parameter, EditorRequired] public Expression<Func<T, object>>? PropertyExpression { get; set; } = Expression.Lambda<Func<T, object>>(Expression.Default(typeof(object)), Expression.Parameter(typeof(T)));
        [Parameter] public string? Title { get; set; }

        public string? PropertyName => ExpressionGenerator.GetMemberExpression(PropertyExpression)?.Member.Name;

        public string? ComputedTitle => Title ?? PropertyName;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Filter?.AddProperty(this);
        }
    }
}
