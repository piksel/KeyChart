using System;
using System.Collections.Generic;
using System.Text;

namespace KeyboardCheatSheet
{
    public class MainWindowDesign
    {
        public LayerCollection Layers { get; } = new LayerCollection()
        {
            new Layer(0, Array.Empty<string>()),
            new Layer(1, Array.Empty<string>()),
            new Layer(2, Array.Empty<string>()),
        };
    }
}
