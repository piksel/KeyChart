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
            Info = new KeyboardInfo()
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
            KeyMap = new()
            {
                Keymap = $"mock_keyboard_keymap_{i}",

            };
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