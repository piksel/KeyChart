using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace KeyboardCheatSheet
{
#if NET5
    public record Layer(int Index, IEnumerable<string> Keys)
    {
        public string Name { get; set; } = $"Layer {Index}";
#else
    public class Layer {
        public int Index { set; get; }
        public IReadOnlyList<string> Keys { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public bool Display { get; set; } = true;

        public VerticalAlignment AlignY {get; set;} = VerticalAlignment.Center;
        public HorizontalAlignment AlignX { get; set; } = HorizontalAlignment.Center;

        public Layer(): this(-1, Array.Empty<string>(), "Layer ?", "??") { }
        public Layer(int index, IReadOnlyList<string> keys): this(index, keys, $"Layer {index}", $"L{index}") { }
        public Layer(int index, IReadOnlyList<string> keys, string name, string slug)
        {
            Index = index;
            Keys = keys;
            Name = name;
            Slug = slug;
        }
#endif


        public override string ToString() => $"[{Slug}] {Name}";

    }

    public class LayerCollection : ObservableCollection<Layer>
    {
        public LayerCollection(){ }
        public LayerCollection(IEnumerable<Layer> layers) : base(layers) { }
    }

    public class DesignLayerCollection : LayerCollection
    {
        /*
        public DesignLayerCollection(): base(new []
        {
            new Layer(0, Array.Empty<string>()),
            new Layer(1, Array.Empty<string>()),
            new Layer(2, Array.Empty<string>())
        })
        {
        }
        */
    }
}