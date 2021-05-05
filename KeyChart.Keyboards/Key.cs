using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.Json.Serialization;

namespace KeyChart.Keyboards
{
    public class Key
    {
        const int KeyU = 80;
        const int KeyP = 4;
        const int KeyB = KeyU + KeyP;

        public Key(int index, double x, double y, double w, KeyLayer[] layers)
        {
            Index = index;
            Width = (KeyB * w) - KeyP;
            Bounds = GetKeyBounds((float)x, (float)y, (float)w);

            Layers = layers;
            // layers = new string[layerCount];
        }

        public static RectangleF GetKeyBounds(float x, float y, float w) 
            => new(x * KeyB, y * KeyB, (KeyB * w) - KeyP, KeyU);

        public RectangleF Bounds { get; set; }

        public int Index { get; }
        public double Width { get; }
        public static double Height => KeyU;

        public KeyLayer[] Layers { get; }
    }

    public class KeyLayer
    {
        public string Text { get; set; }

        public bool Symbol { get; set; }

        [JsonIgnore] public LayerStyle LayerStyle { get; set; } = LayerStyle.Empty;
    }
}
