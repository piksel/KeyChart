using System;
using System.Collections.Generic;
using Col = System.Drawing.Color;

namespace KeyChart.Keyboards
{
    [Flags]
    public enum LabelPosition
    {
        Right = 1 << 0,
        Bottom = 1 << 1,
        Center = 1 << 3,

        TopLeft = 0,
        TopRight = Right,
        BottomLeft = Bottom,
        BottomRight = Bottom | Right
    }

    public class LayerStyle
    {
        public int Index { set; get; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public bool Display { get; set; } = true;

        public LabelPosition Position { get; set; } = LabelPosition.TopLeft;

        public bool BaseLayer { get; set; }
        public string Color { get; set; } = "White";
        
        public LayerStyle() : this(0, "LayerStyle ?", "??") { }
        public LayerStyle(int index) : this(index, $"LayerStyle {index}", $"L{index}") { }
        public LayerStyle(int index, string name, string slug)
        {
            Index = index;
            Name = name;
            Slug = slug;
        }

        public override string ToString() => $"[{Slug}] {Name}";

        public static LayerStyle Empty = new LayerStyle();

        public static IReadOnlyList<(string, LabelPosition)> SecondaryStyles = new[]
        {
            (Hex(Col.LightBlue),  LabelPosition.BottomLeft),
            (Hex(Col.LightPink),  LabelPosition.BottomRight),
            (Hex(Col.LightGreen),  LabelPosition.TopLeft),
            (Hex(Col.LightYellow), LabelPosition.TopRight),
        };

        private static string Hex(Col c) => $"#{c.A:x2}{c.R:x2}{c.G:x2}{c.B:x2}";
    }
}
