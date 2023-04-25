using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;
using MudExtensions.Enums;

namespace MudExtensions
{
    public partial class MudTextM3 : MudComponentBase
    {
        protected string Classname =>
            new CssBuilder("mud-typographym3")
                .AddClass($"mud-typographym3-{TypeStyle.ToString().ToLower()}-{TypeSize.ToString().ToLower()}") // .mud-typographym3-#{$style}-#{$size}
                .AddClass(Class)
                .Build();

        /// <summary>
        /// Set the text-align on the component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Text.Appearance)]
        public TypeStyle TypeStyle { get; set; } = TypeStyle.Body;
        /// <summary>
        /// Set the text-align on the component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Text.Appearance)]
        public TypeSize TypeSize { get; set; } = TypeSize.Large;

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Text.Behavior)]
        public RenderFragment? ChildContent { get; set; }
    }
}