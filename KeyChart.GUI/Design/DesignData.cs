using System.Collections.Generic;
using System.Linq;
using Avalonia.Collections;
using KeyChart.GUI.Views;
using KeyChart.Keyboards;
using static KeyChart.GUI.Views.ConsoleViewHelpers;

namespace KeyChart.GUI.Design
{
    public static class DesignData
    {
        public static ConsoleViewModel ConsoleViewModel = new ConsoleViewModel()
        {
            Lines = new AvaloniaList<ConsoleRow>
            {
                new (new TextSpan( "Row 1", TextColor.Magenta)),
                new (new TextSpan("Row 2")),
                new (CyanText("Row"), WhiteText("with"), GrayText("multiple"), GreenText("colors")),
                new (GrayText("Gray")),
                new (RedText("Red")),
                new (GreenText("Green")),
                new (YellowText("Yellow")),
                new (BlueText("Blue")),
                new (MagentaText("Magenta")),
                new (CyanText("Cyan")),
                new (WhiteText("White")),
                new (new TextSpan("Row")),
                new (new TextSpan("Row")),
                new (new TextSpan("Row")),
                new (new TextSpan("Row")),
                new (new TextSpan("Row")),
                new (new TextSpan("Row")),
                new (new TextSpan("Row")),
                new (new TextSpan("Row")),

            },
        };
        public static KeyboardViewModel KeyboardViewModel => new()
        {
            LayerStyles = KeyboardMocks.LayerStyles,
            Layout = KeyboardMocks.KeyboardLayoutKeys(12, 5).ToList(),
            Info = KeyboardMocks.KeyboardInfo,
            KeyMap = KeyboardMocks.KeyMap(),
        };

        public static MockKey ChartKeyModel = new();
        public static MockKey ChartKeyBottomLeft = new(LabelPosition.BottomLeft);
        public static MockKey ChartKeyBottomRight = new(LabelPosition.BottomRight);

        public static MainWindowViewModel MainWindowViewModelLoading = new()
        {
            Loaded = false,
            KeyboardViewModel = null,
            Console = ConsoleViewModel,
        };

        public static MainWindowViewModel MainWindowViewModelLoaded = new()
        {
            Loaded = true,
            KeyboardViewModel = KeyboardViewModel,
            Console = ConsoleViewModel,
        };
    }
}