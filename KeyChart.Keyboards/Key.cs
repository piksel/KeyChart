using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using Piksel.TextSymbols;

namespace KeyChart.Keyboards
{
    public class Key
    {
        const int KeyU = 80;
        const int KeyP = 4;
        const int KeyB = KeyU + KeyP;

        public static float KeyWidth1U => GetKeyBounds(0, 0, 1).Width;
        
        public Key(int index, double x, double y, double w, KeyLayer[] layers, (byte X, byte Y) matrix)
        {
            Index = index;
            Bounds = GetKeyBounds((float)x, (float)y, (float)w);
            Matrix = matrix;
            Layers = layers;
            // layers = new string[layerCount];



            LayerC = layers.FirstOrDefault(l => l.LayerStyle.Position == LabelPosition.Center);
            LayerTL = layers.FirstOrDefault(l => l.LayerStyle.Position == LabelPosition.TopLeft);
            LayerTR = layers.FirstOrDefault(l => l.LayerStyle.Position == LabelPosition.TopRight);
            LayerBR = layers.FirstOrDefault(l => l.LayerStyle.Position == LabelPosition.BottomRight);
            LayerBL = layers.FirstOrDefault(l => l.LayerStyle.Position == LabelPosition.BottomLeft);
        }

        public (byte X, byte Y) Matrix { get; set; }

        public int MatrixOffset => (Matrix.Y * 6) + Matrix.X;

        public static RectangleF GetKeyBounds(float x, float y, float w) 
            => new(x * KeyB, y * KeyB, (KeyB * w) - KeyP, KeyU);

        public RectangleF Bounds { get; set; }

        public int Index { get; }
        public double Width => Bounds.Width;
        public static double Height => KeyU;

        public KeyLayer[] Layers { get; }

        public KeyLayer? LayerTL { get; set; }
        public KeyLayer? LayerTR { get; set; }
        public KeyLayer? LayerC { get; set; }
        public KeyLayer? LayerBL { get; set; }
        public KeyLayer? LayerBR { get; set; }
    }

    public class KeyLayer
    {
        public KeyLayer(){}

        public KeyLayer(TextSymbol symbol)
        {
            Text = symbol;
            Symbol = true;
        }
        
        public string Text { get; set; }

        public bool Symbol { get; set; }

        [JsonIgnore] 
        public LayerStyle LayerStyle { get; set; } = LayerStyle.Empty;
        
        public LabelPosition? TargetLayer { get; set; }

        public bool TargetTopLeft => TargetLayer is LabelPosition.TopLeft;
        public bool TargetTopRight => TargetLayer is LabelPosition.TopRight;
        public bool TargetBottomLeft => TargetLayer is LabelPosition.BottomLeft;
        public bool TargetBottomRight => TargetLayer is LabelPosition.BottomRight;
    }
}
