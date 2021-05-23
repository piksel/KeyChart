using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using KeyChart.Keyboards;
using KeyChart.Keyboards.QMK;

namespace KeyChart.Design
{
    public class DesignKeyboardModel : KeyboardModel
    {
        private static readonly DesignLayerCollection MockLayerStyles = new();
        public DesignKeyboardModel(): this(0) {}
        public DesignKeyboardModel(int i)
        {
            Name = $"Mock keyboard {i}";
            Info = KeyboardMocks.KeyboardInfo;
            KeyMap = KeyboardMocks.KeyMap(i);
            LayerStyles = MockLayerStyles;
        }
    }

    public class DesignKeyboardModelCollection : ObservableCollection<KeyboardModel>
    {
        public DesignKeyboardModelCollection()
        {
            foreach (var i in Enumerable.Range(1, 5))
            {
                Add(new DesignKeyboardModel(i));
            }
        }
    }

    public class DesignKeyboardLayout : KeyboardLayout
    {
        public DesignKeyboardLayout() : base(KeyboardMocks.KeyboardLayoutKeys(16, 4).ToList())
        { }
    }

    public class DesignKeyLayers : ObservableCollection<KeyLayer>
    {
        public DesignKeyLayers(): base(KeyboardMocks.KeyLayers) {}
    }


}