using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace KeyChart.GUI.Util
{
    public class MessageBox : Window
    {

        private string _message = "Welcome to Avalonia!";
        
        public static readonly DirectProperty<MessageBox, string> MessageProperty =
            AvaloniaProperty.RegisterDirect<MessageBox, string>(
                nameof(Message),
                o => o.Message);
        
        public string Message
        {
            get => _message;
            set => SetAndRaise(MessageProperty, ref _message, value);
        }
        
        public MessageBox()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public static Task ShowDialog(Window owner, string message, string title = null)
        {
            var dlg = new MessageBox();
            dlg.Message = message;
            dlg.Title = title ?? "Information!";
            dlg.Width = owner.Width;
            dlg.Height = owner.Height;
            return dlg.ShowDialog(owner);

        }

        private void Button_OnClick(object? sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}