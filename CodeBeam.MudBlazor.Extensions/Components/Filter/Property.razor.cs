using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Linq.Expressions;
using System.Reflection;

namespace MudExtensions
{
    public partial class Property<T> : MudComponentBase
    {
        [CascadingParameter] public MudFilter<T> Filter { get; set; }

        [Parameter, EditorRequired] public Expression<Func<T, object>> PropertyExpression { get; set; } = Expression.Lambda<Func<T, object>>(Expression.Default(typeof(object)), Expression.Parameter(typeof(T)));

        [Parameter] public string Title { get; set; }

        private object Value
        {
            get
            {
                var compiledFunc = PropertyExpression.Compile();
                return compiledFunc(TargetObject);
            }
            set
            {
                var memberExpression = (MemberExpression)PropertyExpression.Body;
                var propertyInfo = (PropertyInfo)memberExpression.Member;
                propertyInfo.SetValue(TargetObject, value);
            }
        }

        [Parameter]
        public T TargetObject { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Filter.AddProperty(this);
        }

    }
}
