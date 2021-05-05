using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using QMK = KeyChart.Keyboards.QMK;

namespace KeyChart.Keyboards
{
    public class KeyboardModel
    {

        public static KeyboardModel FromKeyboardKeyMap(QMK.KeyboardInfo keyboardInfo, QMK.KeyMap keymap)
        {
            var layerCount = keymap.Layers.GetLength(0);
            var layerStyles = Enumerable.Range(0, layerCount).Select(i => new LayerStyle(i)).ToArray();

            // Use the first layer as base layer by default
            layerStyles[0].BaseLayer = true;


            foreach (var kc in keymap.Layers.SelectMany(layerKeys => layerKeys))
            {
                if (!QMK.KeyCodes.IsLayerKey(kc, out var layerKey)) continue;
                if (layerKey.Layer < 0 || layerKey.Layer >= layerCount) continue;

                var layerStyle = layerStyles[layerKey.Layer];

                // At least one key previously checked sets it as default, consider it a base layer
                if (layerStyle.BaseLayer) continue;

                // Set as base layer if the key sets it as default
                layerStyle.BaseLayer = layerKey.Mode == QMK.LayerKeyMode.SetDefault;

                // Only display the first base layer (and all referenced toggle layers)
                layerStyle.Display = !layerStyle.BaseLayer || layerKey.Layer == 0;
            }

            var toggleLabelCount = 0;
            foreach (var layerStyle in layerStyles)
            {
                if (layerStyle.BaseLayer)
                {
                    layerStyle.Position = LabelPosition.Center;
                    continue;
                }

                if (!layerStyle.Display) continue;
                
                // Set the label style to the next in loop
                (layerStyle.Color, layerStyle.Position) = LayerStyle.SecondaryStyles[toggleLabelCount++ % 4];

                // Only the first 4 will be displayed by default, or else they would overlap
                layerStyle.Display = toggleLabelCount <= 4;
            }


            // Resolve all keycodes into key labels
            var layerKeyLabels = keymap.Layers
                .Select(layer => layer.Select(KeyLabel.FromKeycode).ToList()).ToList();

            return new KeyboardModel() { KeyMap = keymap, Info = keyboardInfo, LayerStyles = layerStyles, LayerKeyLabels = layerKeyLabels };
        }

        public async Task BuildKeyLayout()
        {
            Layout = await Task.Run(() =>
            {
                var layoutId = KeyMap.Layout ?? "LAYOUT";
                var layout = Info.Layouts[layoutId].Layout;
                var keysCount = layout.Length;
                var keyLayout = new List<Key>(keysCount);

                var firstLayerKeyCount = LayerKeyLabels.FirstOrDefault()?.Count() ?? -1;
                if (firstLayerKeyCount < keysCount)
                {
                    // Just bail for now!
                    Debug.WriteLine("First layer key count too low! Was {0}, expected {1}", firstLayerKeyCount, keysCount);
                    return keyLayout;
                }

                for (var i = 0; i < keysCount; i++)
                {
                    
                    var key = layout[i];
                    var layers = LayerKeyLabels
                        .Select(keyLabels => keyLabels[i])
                        .Zip(LayerStyles,
                            (label, layerSettings) => new KeyLayer() {LayerStyle = layerSettings, Text = label.Text, Symbol = label.Symbol})
                        .ToArray();


                    keyLayout.Add(new Key(i, key.X, key.Y, key.W, layers));
                }

                return keyLayout;
            });
        }

        public List<List<KeyLabel>> LayerKeyLabels { get; set; } = new();

        public IReadOnlyList<LayerStyle> LayerStyles { get; set; } = Array.Empty<LayerStyle>();

        [JsonIgnore]
        public IReadOnlyList<Key> Layout { get; set; } = new List<Key>();

        public QMK.KeyMap KeyMap { get; set; } = new();
        public QMK.KeyboardInfo Info { get; set; } = new ();

        public string Name { get; set; } = "";

        public string Source { get; set; } = "";
    }

}