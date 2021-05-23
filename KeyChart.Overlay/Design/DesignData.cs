using System.Linq;
using KeyChart.Avalonia.Views;
using KeyChart.Keyboards;

namespace KeyChart.Avalonia.Design
{
    public static class DesignData
    {
        public static KeyboardViewModel KeyboardViewModel => new()
        {
            LayerStyles = KeyboardMocks.LayerStyles,
            Layout = KeyboardMocks.KeyboardLayoutKeys(12, 5).ToList(),
            Info = KeyboardMocks.KeyboardInfo,
            KeyMap = KeyboardMocks.KeyMap(),
        };

        public static MockKey ChartKeyModel = new();
    }
}