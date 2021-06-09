using System;
using System.Collections.Generic;
using System.Diagnostics;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

namespace KeyChart.GUI.Views
{
    public class ConsoleView : UserControl
    {
        public ConsoleView()
        {
            InitializeComponent();
            Scroller = this.FindControl<ScrollViewer>("Scroller");
            Lines = this.FindControl<ItemsControl>("Lines");
            
        }

        private ScrollViewer Scroller { get; }
        private ItemsControl Lines { get; }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void Lines_OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {

        }

        private void Scroller_OnScrollChanged(object? sender, ScrollChangedEventArgs e)
        {
            Debug.WriteLine($"Extent: {e.ExtentDelta}, ViewPort: {e.ViewportDelta}, {e.OffsetDelta}");
            if(e.ExtentDelta.Y >= double.Epsilon)
                Scroller.ScrollToEnd();
        }
    }

    public class ConsoleViewModel
    {
        public AvaloniaList<ConsoleRow> Lines { get; set; } = new();

        public void AddLine(string text)
            => Lines.Add(new ConsoleRow(text));
        
        public void AddLine(params TextSpan[] spans)
        {
            if (Lines.Count > 50)
            {
                Lines.RemoveAt(0);
            }
            Lines.Add(new ConsoleRow(spans));
        }
    }

    public record ConsoleRow(params TextSpan[] Spans)
    {
        public ConsoleRow(string text) : this(new TextSpan(text)) {}
    }

    public enum TextColor
    {
        Gray,
        Red,
        Green,
        Yellow,
        Blue,
        Magenta,
        Cyan,
        White,
    }

    public record TextSpan(string Text, TextColor Color = TextColor.Gray)
    {
        public bool Gray => Color == TextColor.Gray;
        public bool Red => Color == TextColor.Red;
        public bool Green => Color == TextColor.Green;
        public bool Yellow => Color == TextColor.Yellow;
        public bool Blue => Color == TextColor.Blue;
        public bool Magenta => Color == TextColor.Magenta;
        public bool Cyan => Color == TextColor.Cyan;
        public bool White => Color == TextColor.White;

        public static TextSpan CyanText(string text) => new (text, TextColor.Cyan);
    }

    public static class ConsoleViewHelpers
    {
        public static TextSpan GrayText(string text) => new (text, TextColor.Gray);
        public static TextSpan RedText(string text) => new (text, TextColor.Red);
        public static TextSpan GreenText(string text) => new (text, TextColor.Green);
        public static TextSpan YellowText(string text) => new (text, TextColor.Yellow);
        public static TextSpan BlueText(string text) => new (text, TextColor.Blue);
        public static TextSpan MagentaText(string text) => new (text, TextColor.Magenta);
        public static TextSpan CyanText(string text) => new (text, TextColor.Cyan);
        public static TextSpan WhiteText(string text) => new (text, TextColor.White);

    }
}