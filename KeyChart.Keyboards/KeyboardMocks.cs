using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeyChart.Keyboards.QMK;

namespace KeyChart.Keyboards
{
    public static class KeyboardMocks
    {
        public static LayerStyle[] LayerStyles = {
            new (0) {BaseLayer = true, Display = true, Position = LabelPosition.Center},
            SecondaryLayerStyle(1, 0),
            SecondaryLayerStyle(2, 1),
            SecondaryLayerStyle(3, 2),
            SecondaryLayerStyle(4, 3),
        };

        public static LayerStyle SecondaryLayerStyle(int layer, int styleIndex)
        {
            var (color, position) = LayerStyle.SecondaryStyles[styleIndex];
            return new LayerStyle(layer) {BaseLayer = false, Color = color, Display = true, Position = position};
        }

        public static KeyLayer[] KeyLayers = {
            new() {Text = "Key", LayerStyle = LayerStyles[0]},
            new() {Text = "", Symbol = true, LayerStyle = LayerStyles[1]},
            new() {Text = "5", LayerStyle = LayerStyles[2]},
            new() {Text = "F", LayerStyle = LayerStyles[3]},
            new() {Text = "Del", LayerStyle = LayerStyles[4]},
        };

        public static IEnumerable<Key> KeyboardLayoutKeys(int cols, int rows) =>
            Enumerable.Range(0, cols)
                .SelectMany(col => Enumerable.Range(0, rows).Select(row => (col, row)))
                .Select((pos, index) => new Key(index, pos.col, pos.row, 1, new KeyLayer[]
                {
                    new() { Text = $"{index}", LayerStyle = LayerStyles[0] },
                    new() { Text = "TL", LayerStyle = LayerStyles[1] },
                    new() { Text = $"{pos.row}", LayerStyle = LayerStyles[2] },
                    new() { Text = "BR", LayerStyle = LayerStyles[3] },
                    new() { Text = $"{pos.col}", LayerStyle = LayerStyles[4] },
                }));
    }

    public class MockKey : Key
    {
        public MockKey() : base(0, 0, 0, 1, KeyboardMocks.KeyLayers) { }
    }
}
