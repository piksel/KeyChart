using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace KeyChart.GUI.Controls
{
    public class ChartKey : UserControl
    {
        public ChartKey()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}