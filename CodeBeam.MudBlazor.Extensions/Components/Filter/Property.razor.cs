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

        public string? PropertyName
        {
            get
            {
                Console.WriteLine("--> PropertyName");

                // The default value when the expression is null or can't be cast to MemberExpression
                var defaultName = string.Empty;

                if (PropertyExpression?.Body is MemberExpression memberExpression)
                {
                    defaultName =  memberExpression.Member.Name;
                }
                else if (PropertyExpression?.Body is UnaryExpression unaryExpression && unaryExpression.Operand is MemberExpression)
                {
                    defaultName = ((MemberExpression)unaryExpression.Operand).Member.Name;
                }
                Console.WriteLine($"--> PropertyName returning {defaultName}");
                return defaultName;
            }
        }

        public string? ComputedTitle
        {
            get
            {
                Console.WriteLine("--> ComputedTitle");
                if (Title is not null)
                {
                    return Title;
                }
                else
                {
                    // The default value when the expression is null or can't be cast to MemberExpression
                    var defaultName = string.Empty;

                    if (PropertyExpression?.Body is MemberExpression memberExpression)
                    {
                        return memberExpression.Member.Name;
                    }
                    else if (PropertyExpression?.Body is UnaryExpression unaryExpression && unaryExpression.Operand is MemberExpression)
                    {
                        return ((MemberExpression)unaryExpression.Operand).Member.Name;
                    }

                    return defaultName;
                }
            }
        }


        protected override void OnInitialized()
        {
            base.OnInitialized();
            Filter?.AddProperty(this);
        }

    }
}
