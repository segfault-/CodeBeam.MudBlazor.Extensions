using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudExtensions.Components.TypographyM3;

namespace MudExtensions
{
    public partial class MudTypographyProvider : MudComponentBase
    {
        /// <summary>
        /// Display Large values
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Item.Behavior)]
        public BaseTypographyM3 DisplayLargeTypo { get; set; } = new DisplayLarge();

        /// <summary>
        /// Display Medium values
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Item.Behavior)]
        public BaseTypographyM3 DisplayMediumTypo { get; set; } = new DisplayMedium();

        /// <summary>
        /// Display Small values
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Item.Behavior)]
        public BaseTypographyM3 DisplaySmallTypo { get; set; } = new DisplaySmall();


        [Parameter]
        [Category(CategoryTypes.Item.Behavior)]
        public BaseTypographyM3 HeadlineLargeTypo { get; set; } = new HeadlineLarge();

        /// <summary>
        /// Display Medium values
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Item.Behavior)]
        public BaseTypographyM3 HeadlineMediumTypo { get; set; } = new HeadlineMedium();

        /// <summary>
        /// Display Small values
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Item.Behavior)]
        public BaseTypographyM3 HeadlineSmallTypo { get; set; } = new HeadlineSmall();


        [Parameter]
        [Category(CategoryTypes.Item.Behavior)]
        public BaseTypographyM3 TitleLargeTypo { get; set; } = new TitleLarge();

        /// <summary>
        /// Display Medium values
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Item.Behavior)]
        public BaseTypographyM3 TitleMediumTypo { get; set; } = new TitleMedium();

        /// <summary>
        /// Display Small values
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Item.Behavior)]
        public BaseTypographyM3 TitleSmallTypo { get; set; } = new TitleSmall();


        [Parameter]
        [Category(CategoryTypes.Item.Behavior)]
        public BaseTypographyM3 LabelLargeTypo { get; set; } = new LabelLarge();

        /// <summary>
        /// Display Medium values
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Item.Behavior)]
        public BaseTypographyM3 LabelMediumTypo { get; set; } = new LabelMedium();

        /// <summary>
        /// Display Small values
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Item.Behavior)]
        public BaseTypographyM3 LabelSmallTypo { get; set; } = new LabelSmall();


        [Parameter]
        [Category(CategoryTypes.Item.Behavior)]
        public BaseTypographyM3 BodyLargeTypo { get; set; } = new BodyLarge();

        /// <summary>
        /// Display Medium values
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Item.Behavior)]
        public BaseTypographyM3 BodyMediumTypo { get; set; } = new BodyMedium();

        /// <summary>
        /// Display Small values
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Item.Behavior)]
        public BaseTypographyM3 BodySmallTypo { get; set; } = new BodySmall();

        public MudTypographyProvider()
            :base()
        {

        }
    }
}
