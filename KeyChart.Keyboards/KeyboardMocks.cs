﻿using System.Collections.Generic;
using System.Linq;
using KeyChart.Keyboards.QMK;
using Piksel.TextSymbols;

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

        public static KeyboardInfo KeyboardInfo = new()
        {
            KeyboardName = "Mock Keyboard",
            Maintainer = "Maintainer Man",
            Manufacturer = "Acme Keyboards Inc.",
            Bootloader = "ntldr",
            Features = Features.All,
            Layouts = new() {{"LAYOUT", new()}},
            Url = "https://lmgtfy.com/q=?how+to+keyboard",
            ProcessorType = "IA-64",
            Processor = "Itanium 9760"
        };

        public static LayerStyle SecondaryLayerStyle(int layer, int styleIndex)
        {
            var (color, position) = LayerStyle.SecondaryStyles[styleIndex];
            return new LayerStyle(layer) {BaseLayer = false, Color = color, Display = true, Position = position};
        }

        public static KeyLayer[] KeyLayers = {
            new() {Text = "Key", LayerStyle = LayerStyles[0]},
            new(Fa6.Alien) {LayerStyle = LayerStyles[1]},
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
                    new(Fa6.Socks) { LayerStyle = LayerStyles[1] },
                    new() { Text = $"{pos.row}", LayerStyle = LayerStyles[2] },
                    new() { Text = "BR", LayerStyle = LayerStyles[3] },
                    new() { Text = $"{pos.col}", LayerStyle = LayerStyles[4] },
                }, ((byte)pos.col, (byte)pos.row)));

        public static KeyMap KeyMap(int index = 0)
            => new() { Keymap = $"mock_keyboard_keymap_{index}" };
    }

    public class MockKey : Key
    {
        public MockKey(LabelPosition? targetLayer = null) : base(0, 0, 0, 1, KeyboardMocks.KeyLayers, (0, 0))
        {
            if (targetLayer is null) return;
            var mockC = KeyboardMocks.KeyLayers[2];
            LayerC = new KeyLayer
            {
                Symbol = mockC.Symbol,
                Text = mockC.Text,
                LayerStyle = mockC.LayerStyle,
                TargetLayer = targetLayer,
            };
        }
    }
}
