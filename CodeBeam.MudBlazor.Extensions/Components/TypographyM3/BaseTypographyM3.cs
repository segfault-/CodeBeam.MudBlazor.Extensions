using System.Reflection.Emit;

namespace MudExtensions.Components.TypographyM3
{
    public class TypographyM3
    {
        public DisplayLarge DisplayLarge { get; set; } = new();
        public DisplayMedium DisplayMedium { get; set; } = new();
        public DisplaySmall DisplaySmall { get; set; } = new();
    }
    public class BaseTypographyM3
    {
        public string[] Font { get; set; }
        public double LineHeight { get; set; }
        public double Size { get; set; }
        public double Tracking { get; set; }
        public int Weight { get; set; }
    }

    public class DisplayLarge : BaseTypographyM3
    {
        public DisplayLarge()
        {
            Font = new string[] { "Roboto Regular" };
            LineHeight = (double)64 / 16;
            Size = (double)60 / 16;
            Tracking = 0;
            Weight = 400;
        }
    }

    public class DisplayMedium : BaseTypographyM3
    {
        public DisplayMedium()
        {
            Font = new string[] { "Roboto Regular" };
            LineHeight = (double)60 / 16;
            Size = (double)56 / 16;
            Tracking = 0;
            Weight = 400;
        }
    }

    public class DisplaySmall : BaseTypographyM3
    {
        public DisplaySmall()
        {
            Font = new string[] { "Roboto Regular" };
            LineHeight = (double)56 / 16;
            Size = (double)52 / 16;
            Tracking = 0;
            Weight = 400;
        }
    }

    public class HeadlineLarge : BaseTypographyM3
    {
        public HeadlineLarge()
        {
            Font = new string[] { "Roboto Regular" };
            LineHeight = (double)52 / 16;
            Size = (double)48 / 16;
            Tracking = 0;
            Weight = 400;
        }
    }

    public class HeadlineMedium : BaseTypographyM3
    {
        public HeadlineMedium()
        {
            Font = new string[] { "Roboto Regular" };
            LineHeight = (double)48 / 16;
            Size = (double)44 / 16;
            Tracking = 0;
            Weight = 400;
        }
    }
    public class HeadlineSmall : BaseTypographyM3
    {
        public HeadlineSmall()
        {
            Font = new string[] { "Roboto Regular" };
            LineHeight = (double)44 / 16;
            Size = (double)40 / 16;
            Tracking = 0;
            Weight = 400;
        }
    }
    public class TitleLarge : BaseTypographyM3
    {
        public TitleLarge()
        {
            Font = new string[] { "Roboto Regular" };
            LineHeight = (double)40 / 16;
            Size = (double)36 / 16;
            Tracking = 0;
            Weight = 400;
        }
    }
    public class TitleMedium : BaseTypographyM3
    {
        public TitleMedium()
        {
            Font = new string[] { "Roboto Regular" };
            LineHeight = (double)36 / 16;
            Size = (double)32 / 16;
            Tracking = 0;
            Weight = 400;
        }
    }
    public class TitleSmall : BaseTypographyM3
    {
        public TitleSmall()
        {
            Font = new string[] { "Roboto Regular" };
            LineHeight = (double)32 / 16;
            Size = (double)28 / 16;
            Tracking = 0;
            Weight = 400;
        }
    }
    public class LabelLarge : BaseTypographyM3
    {
        public LabelLarge()
        {
            Font = new string[] { "Roboto Regular" };
            LineHeight = (double)28 / 16;
            Size = (double)24 / 16;
            Tracking = 0;
            Weight = 400;
        }
    }
    public class LabelMedium : BaseTypographyM3
    {
        public LabelMedium()
        {
            Font = new string[] { "Roboto Regular" };
            LineHeight = (double)24 / 16;
            Size = (double)20 / 16;
            Tracking = 0;
            Weight = 400;
        }
    }
    public class LabelSmall : BaseTypographyM3
    {
        public LabelSmall()
        {
            Font = new string[] { "Roboto Regular" };
            LineHeight = (double)20 / 16;
            Size = (double)16 / 16;
            Tracking = 0;
            Weight = 400;
        }
    }
    public class BodyLarge : BaseTypographyM3
    {
        public BodyLarge()
        {
            Font = new string[] { "Roboto Regular" };
            LineHeight = (double)16 / 16;
            Size = (double)15 / 16;
            Tracking = 0;
            Weight = 400;
        }
    }
    public class BodyMedium : BaseTypographyM3
    {
        public BodyMedium()
        {
            Font = new string[] { "Roboto Regular" };
            LineHeight = (double)14 / 16;
            Size = (double)13 / 16;
            Tracking = 0;
            Weight = 400;
        }
    }
    public class BodySmall : BaseTypographyM3
    {
        public BodySmall()
        {
            Font = new string[] { "Roboto Regular" };
            LineHeight = (double)12 / 16;
            Size = (double)11 / 16;
            Tracking = 0;
            Weight = 400;
        }
    }
}
