using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;

namespace MudExtensions
{
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
        [Parameter] public RenderFragment FilterTemplate { get; set; }
        [Parameter] public CompoundPredicate<T> FilterRoot { get; set; } = new(null);
        [Parameter] public ICollection<Property<T>> Properties { get; set; } = new List<Property<T>>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        internal void AddProperty(Property<T> property)
        {
            Properties.Add(property);
        }

        protected void AddPropertyExpression()
        {
           FilterRoot.AddPredicate(new AtomicPredicate<T>(FilterRoot));
        }

        protected void AddGroup()
        {
            FilterRoot.AddPredicate(new CompoundPredicate<T>(FilterRoot));
        }
    }
}
