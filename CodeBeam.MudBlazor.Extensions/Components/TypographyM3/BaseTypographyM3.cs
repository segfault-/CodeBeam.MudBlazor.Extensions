﻿namespace MudExtensions.Components.TypographyM3
{
    public class TypographyM3
    {
        public DisplayLarge DisplayLarge { get; set; } = new();
        public DisplayMedium DisplayMedium { get; set; } = new();
        public DisplaySmall DisplaySmall { get; set; } = new();

        public HeadlineLarge HeadlineLarge { get; set; } = new();
        public HeadlineMedium HeadlineMedium { get; set; } = new();
        public HeadlineSmall HeadlineSmall { get; set; } = new();

        public TitleLarge TitleLarge { get; set; } = new();
        public TitleMedium TitleMedium { get; set; } = new();
        public TitleSmall TitleSmall { get; set; } = new();

        public LabelLarge LabelLarge { get; set; } = new();
        public LabelMedium LabelMedium { get; set; } = new();
        public LabelSmall LabelSmall { get; set; } = new();

        public BodyLarge BodyLarge { get; set; } = new();
        public BodyMedium BodyMedium { get; set; } = new();
        public BodySmall BodySmall { get; set; } = new();
    }

    public class BaseTypographyM3
    {
        public string[] Font { get; set; }

        private double _lineHeight;
        public double LineHeight 
        { 
            get => _lineHeight;
            set
            {
                _lineHeight = value / 16;
            }
        }

        private double _size;
        public double Size
        {
            get => _size;
            set
            {
                _size = value / 16;
            }
        }

        private double _tracking;
        public double Tracking
        {
            get => _tracking;
            set
            {
                _tracking = value / (Size * 16);
            }
        }
        public int Weight { get; set; }
    }

    public class DisplayLarge : BaseTypographyM3
    {
        public DisplayLarge() : base()
        {
            Font = new string[] { "Roboto" };
            LineHeight = 64;
            Size = 57;
            Tracking = -0.25;
            Weight = 400;
        }
    }

    public class DisplayMedium : BaseTypographyM3
    {
        public DisplayMedium() : base()
        {
            Font = new string[] { "Roboto" };
            LineHeight = 52;
            Size = 45;
            Tracking = 0;
            Weight = 400;
        }
    }

    public class DisplaySmall : BaseTypographyM3
    {
        public DisplaySmall() : base()
        {
            Font = new string[] { "Roboto" };
            LineHeight = 44;
            Size = 36;
            Tracking = 0;
            Weight = 400;
        }
    }

    public class HeadlineLarge : BaseTypographyM3
    {
        public HeadlineLarge() : base()
        {
            Font = new string[] { "Roboto" };
            LineHeight = 40;
            Size = 32;
            Tracking = 0;
            Weight = 400;
        }
    }

    public class HeadlineMedium : BaseTypographyM3
    {
        public HeadlineMedium() : base()
        {
            Font = new string[] { "Roboto" };
            LineHeight = 36;
            Size = 28;
            Tracking = 0;
            Weight = 400;
        }
    }
    public class HeadlineSmall : BaseTypographyM3
    {
        public HeadlineSmall() : base()
        {
            Font = new string[] { "Roboto" };
            LineHeight = 32;
            Size = 24;
            Tracking = 0;
            Weight = 400;
        }
    }
    public class TitleLarge : BaseTypographyM3
    {
        public TitleLarge() : base()
        {
            Font = new string[] { "Roboto" };
            LineHeight = 28;
            Size = 22;
            Tracking = 0;
            Weight = 400;
        }
    }
    public class TitleMedium : BaseTypographyM3
    {
        public TitleMedium() : base()
        {
            Font = new string[] { "Roboto" };
            LineHeight = 24;
            Size = 16;
            Tracking = 0.15;
            Weight = 500;
        }
    }
    public class TitleSmall : BaseTypographyM3
    {
        public TitleSmall() : base()
        {
            Font = new string[] { "Roboto" };
            LineHeight = 20;
            Size = 14;
            Tracking = 0.1;
            Weight = 500;
        }
    }
    public class BodyLarge : BaseTypographyM3
    {
        public BodyLarge() : base()
        {
            Font = new string[] { "Roboto" };
            LineHeight = 24;
            Size = 16;
            Tracking = 0.5;
            Weight = 400;
        }
    }
    public class BodyMedium : BaseTypographyM3
    {
        public BodyMedium() : base()
        {
            Font = new string[] { "Roboto" };
            LineHeight = 20;
            Size = 14;
            Tracking = 0.25;
            Weight = 400;
        }
    }
    public class BodySmall : BaseTypographyM3
    {
        public BodySmall() : base()
        {
            Font = new string[] { "Roboto" };
            LineHeight = 16;
            Size = 12;
            Tracking = 0.4;
            Weight = 400;
        }
    }
    public class LabelLarge : BaseTypographyM3
    {
        public LabelLarge() : base()
        {
            Font = new string[] { "Roboto" };
            LineHeight = 20;
            Size = 14;
            Tracking = 0.1;
            Weight = 500;
        }
    }
    public class LabelMedium : BaseTypographyM3
    {
        public LabelMedium() : base()
        {
            Font = new string[] { "Roboto" };
            LineHeight = 16;
            Size = 12;
            Tracking = 0.5;
            Weight = 500;
        }
    }
    public class LabelSmall : BaseTypographyM3
    {
        public LabelSmall() : base()
        {
            Font = new string[] { "Roboto" };
            LineHeight = 16;
            Size = 11;
            Tracking = 0.5;
            Weight = 500;
        }
    }
}
