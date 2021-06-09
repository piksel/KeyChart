using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace KeyChart.GUI.Controls
{
    public class Spinner : UserControl
    {
        public Spinner()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}